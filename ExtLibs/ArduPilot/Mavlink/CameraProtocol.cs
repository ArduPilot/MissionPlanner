using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Geometry;
using GeoAPI.DataStructures;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot.Mavlink
{
    /// <summary>
    /// Handles communication and control for camera operations via MAVLink protocol. 
    /// This includes starting/stopping video capture, taking pictures, and fetching camera settings and status.
    /// </summary>
    public class CameraProtocol
    {
        // Logger for capturing runtime information and errors
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Reference to the parent MAVState, used for MAVLink communication
        private MAVState parent;

        // Tracks whether we have received a `CAMERA_INFORMATION` message yet
        private bool have_camera_information = false;

        // Lease-based streaming rates managed by the connection's MessageRateManager.
        private readonly object _leaseLock = new object();
        private List<MessageRateLease> _streamingLeases = new List<MessageRateLease>();
        private MessageRateLease _leaseTracking;
        private int _lastRateHz = -1;
        private int _lastTrackingRateHz;

        public MAVLink.mavlink_camera_information_t CameraInformation { get; private set; }
        public MAVLink.mavlink_camera_settings_t CameraSettings { get; private set; }
        public MAVLink.mavlink_camera_capture_status_t CameraCaptureStatus { get; private set; }
        public MAVLink.mavlink_camera_fov_status_t CameraFOVStatus { get; private set; }
        public MAVLink.mavlink_camera_tracking_image_status_t CameraTrackingImageStatus { get; private set; }

        public static ConcurrentDictionary<(byte, byte, byte), MAVLink.mavlink_video_stream_information_t> VideoStreams { get; private set; } = new ConcurrentDictionary<(byte, byte, byte), MAVLink.mavlink_video_stream_information_t>();

        public static string GStreamerPipeline(MAVLink.mavlink_video_stream_information_t stream)
        {
            var type = (MAVLink.VIDEO_STREAM_TYPE)stream.type;
            var uri = System.Text.Encoding.UTF8.GetString(stream.uri).Split('\0')[0];

            // Allow a uri that starts with "gst://" to be used directly as a GStreamer pipeline
            // (this is my personal hack to allow for custom pipelines for testing)
            if (uri.StartsWith("gst://"))
            {
                return uri.Substring("gst://".Length);
            }

            // For the UDP transports, extract the port number from the URI. The URI should be only the port number,
            // but we will attempt to handle malformed ones like "udp://127.0.0.1:5600" as well.
            int port = 0;
            if (type == MAVLink.VIDEO_STREAM_TYPE.RTPUDP || type == MAVLink.VIDEO_STREAM_TYPE.MPEG_TS)
            {
                if (!int.TryParse(uri, out port))
                {
                    var match = Regex.Match(uri, ":(\\d+)"); // Match a colon followed by digits
                    if (match.Success)
                    {
                        port = int.Parse(match.Groups[1].Value);
                    }
                }
                if (port < 1 || port > 65535)
                {
                    return "";
                }
            }

            // Otherwise, correctly generate a pipeline based on the stream type
            switch (type)
            {
            case MAVLink.VIDEO_STREAM_TYPE.RTSP:
                uri = "rtsp://" + Regex.Replace(uri, "^.*://", "");
                return $"rtspsrc location={uri} latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";

            case MAVLink.VIDEO_STREAM_TYPE.RTPUDP:
                // Assume unknown encodings are H264
                string encoding_name = stream.encoding == (byte)MAVLink.VIDEO_STREAM_ENCODING.H265 ? "H265" : "H264";
                return $"udpsrc port={port} buffer-size=90000 ! application/x-rtp,media=(string)video,clock-rate=(int)90000,encoding-name=(string){encoding_name} ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";

            case MAVLink.VIDEO_STREAM_TYPE.TCP_MPEG:
                var match = Regex.Match(uri, @"^(?:.*://)?([^:/]+):(\d+)");
                if (match.Success)
                {
                    return $"tcpclientsrc host={match.Groups[1].Value} port={match.Groups[2].Value} ! decodebin ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";
                }
                return "";

            case MAVLink.VIDEO_STREAM_TYPE.MPEG_TS:
                return $"udpsrc port={port} buffer-size=90000 ! tsparse ! tsdemux ! decodebin ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false";
            default:
                return "";
            }
        }

        /// <summary>
        /// True if the camera has different modes, like image mode and video mode
        /// </summary>
        public bool HasModes { get => (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_MODES) > 0; }

        /// <summary>
        /// True if the camera supports zoom in/out commands.
        /// </summary>
        public bool HasZoom { get => (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_BASIC_ZOOM) > 0; }

        /// <summary>
        /// True if the camera supports focus control commands.
        /// </summary>
        public bool HasFocus { get => (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_BASIC_FOCUS) > 0; }

        /// <summary>
        /// True if the camera is currently able to capture a video, based on the capabilities and the current mode.
        /// </summary>
        public bool CanCaptureVideo
        {
            get
            {
                // If we don't have video capture at all, return false immediately
                if ((CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAPTURE_VIDEO) == 0)
                {
                    return false;
                }
                // If we don't have modes, or if we are in video mode return true
                if (!HasModes || CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.VIDEO)
                {
                    return true;
                }
                // If we are in image mode, see if we can capture a video in image mode
                if (CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.IMAGE ||
                    CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.IMAGE_SURVEY)
                {
                    return (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAN_CAPTURE_VIDEO_IN_IMAGE_MODE) > 0;
                }
                // Unknown mode, assume we cannot capture a video
                return false;
            }
        }

        /// <summary>
        /// True if the camera is currently able to capture an image, based on the capabilities and the current mode.
        /// </summary>
        public bool CanCaptureImage
        {
            get
            {
                // If we don't have image capture at all, return false immediately
                if ((CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAPTURE_IMAGE) == 0)
                {
                    return false;
                }
                // If we don't have modes, or we are in image mode, return true;
                // (includes image-survey, even though it's not explicitly mentioned whether manual
                //  image capture is available in this mode)
                if (!HasModes ||
                    CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.IMAGE ||
                    CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.IMAGE_SURVEY)
                {
                    return true;
                }
                // If we are in video mode, see if we can capture an image in video mode
                if (CameraSettings.mode_id == (byte)MAVLink.CAMERA_MODE.VIDEO)
                {
                    return (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAN_CAPTURE_IMAGE_IN_VIDEO_MODE) > 0;
                }
                // Unknown mode, assume we cannot capture an image
                return false;
            }
        }

        public bool UseFOVStatus { get; set; } = true;

        public float _hfov = float.NaN;
        /// <summary>
        /// Horizontal field of view of the camera, in degrees. Uses the latest received value from the camera if available and `UseFOVStatus` is true.
        /// </summary>
        public float HFOV
        {
            get
            {
                if (!UseFOVStatus || CameraFOVStatus.hfov == float.NaN)
                {
                    return _hfov;
                }
                return CameraFOVStatus.hfov;
            }
            set
            {
                _hfov = value;
            }
        }

        public float _vfov = float.NaN;
        public float VFOV
        {
            get
            {
                if (!UseFOVStatus || CameraFOVStatus.vfov == float.NaN)
                {
                    return _vfov;
                }
                return CameraFOVStatus.vfov;
            }
            set
            {
                _vfov = value;
            }
        }

        /// <summary>
        /// Initializes the camera protocol by setting up message parsing and requesting camera information.
        /// </summary>
        /// <remarks>
        /// Sends fire-and-forget SET_MESSAGE_INTERVAL + GET_MESSAGE_INTERVAL for
        /// CAMERA_INFORMATION, then watches the MESSAGE_INTERVAL reply to confirm.
        /// Retries up to 3 times; stops on success, interval_us == 0 (not supported),
        /// or if CAMERA_INFORMATION arrives in the meantime.
        /// </remarks>
        /// <param name="mavState">The MAVState instance for the target camera.</param>
        public async Task StartID(MAVState mavState)
        {
            parent = mavState;

            mavState.parent.OnPacketReceived += ParseMessages;

            if (parent?.parent == null)
                return;

            var port = parent.parent;

            // Ask the target to stream CAMERA_INFORMATION every ~30s so we discover
            // cameras even if plugged in after boot.
            const ushort camInfoId = (ushort)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION;
            const float intervalUs = 30_000_000; // 30 s

            // Watch MESSAGE_INTERVAL replies for our message to confirm acceptance or rejection.
            bool confirmed = false;
            var sub = port.SubscribeToPacketType(
                MAVLink.MAVLINK_MSG_ID.MESSAGE_INTERVAL,
                msg =>
                {
                    var data = msg.ToStructure<MAVLink.mavlink_message_interval_t>();
                    if (data.message_id != camInfoId)
                        return true;

                    if (data.interval_us == 0)
                        log.Info("Camera: CAMERA_INFORMATION not supported (interval_us=0)");
                    else
                        log.InfoFormat("Camera: CAMERA_INFORMATION interval confirmed at {0} us", data.interval_us);

                    confirmed = true;
                    return true;
                },
                parent.sysid, parent.compid);

            try
            {
                for (int attempt = 0; attempt < 3; attempt++)
                {
                    if (have_camera_information || confirmed)
                        break;

                    try
                    {
                        _ = port.doCommandAsync(parent.sysid, parent.compid,
                            MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                            camInfoId, intervalUs,
                            0, 0, 0, 0, 0, false);

                        _ = port.doCommandAsync(parent.sysid, parent.compid,
                            MAVLink.MAV_CMD.GET_MESSAGE_INTERVAL,
                            camInfoId,
                            0, 0, 0, 0, 0, 0, false);
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Camera: SET/GET_MESSAGE_INTERVAL failed: " + ex.Message);
                    }

                    await Task.Delay(5000).ConfigureAwait(false);
                }
            }
            finally
            {
                port.UnSubscribeToPacketType(sub);
            }
        }

        /// <summary>
        /// Parses incoming MAVLink messages and updates camera state.
        /// </summary>
        /// <param name="sender">The MAVLink interface that received the message.</param>
        /// <param name="message">The received MAVLink message.</param>
        public void ParseMessages(object sender, MAVLink.MAVLinkMessage message)
        {
            if (message.sysid != parent.sysid || message.compid != parent.compid)
                return;

            switch ((MAVLink.MAVLINK_MSG_ID)message.msgid)
            {
            case MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION:
                CameraInformation = (MAVLink.mavlink_camera_information_t)message.data;
                if (!have_camera_information)
                {
                    have_camera_information = true;
                    if (_lastRateHz > 0)
                        TakeStreamingLeases(_lastRateHz);

                    if ((CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_VIDEO_STREAM) != 0)
                        RequestVideoStreamWithRetry();
                }
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS:
                CameraSettings = (MAVLink.mavlink_camera_settings_t)message.data;
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS:
                CameraCaptureStatus = (MAVLink.mavlink_camera_capture_status_t)message.data;
                break;
            case MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION:
                var video_stream_info = (MAVLink.mavlink_video_stream_information_t)message.data;
                VideoStreams[(parent.sysid, parent.compid, video_stream_info.stream_id)] = video_stream_info;
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_FOV_STATUS:
                CameraFOVStatus = (MAVLink.mavlink_camera_fov_status_t)message.data;
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_TRACKING_IMAGE_STATUS:
                CameraTrackingImageStatus = (MAVLink.mavlink_camera_tracking_image_status_t)message.data;
                break;
            }
        }

        /// <summary>
        /// Re-subscribes streaming leases if the desired rate has changed.
        /// </summary>
        /// <remarks>
        /// Call periodically (e.g. from the rate-request loop). Does nothing until
        /// CAMERA_INFORMATION has been received. New leases are taken before old
        /// ones are disposed so there is no gap in coverage.
        /// Zero or negative values release all streaming leases (the RateManager
        /// restores the autopilot's default rate).
        /// </remarks>
        /// <param name="ratehz">Desired rate in Hz. &lt;= 0 releases leases.</param>
        public void UpdateRateIfChanged(int ratehz)
        {
            if (!have_camera_information || parent?.parent == null)
                return;

            if (ratehz == _lastRateHz)
                return;

            if (ratehz <= 0)
                ReleaseStreamingLeases();
            else
                TakeStreamingLeases(ratehz);
        }

        /// <summary>
        /// Takes (or replaces) streaming leases at the given rate based on camera capabilities.
        /// </summary>
        private void TakeStreamingLeases(int ratehz)
        {
            var rm = parent.parent.RateManager;
            double hz = ratehz;
            string owner = $"Camera({parent.sysid},{parent.compid})";

            // Build new lease set, then swap and dispose old
            var newLeases = new List<MessageRateLease>
            {
                rm.Subscribe(parent.sysid, parent.compid, MAVLink.MAVLINK_MSG_ID.CAMERA_FOV_STATUS, hz, owner),
                rm.Subscribe(parent.sysid, parent.compid, MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS, hz, owner),
                rm.Subscribe(parent.sysid, parent.compid, MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS, hz, owner),
            };

            List<MessageRateLease> old;
            lock (_leaseLock)
            {
                old = _streamingLeases;
                _streamingLeases = newLeases;
                _lastRateHz = ratehz;
            }
            foreach (var lease in old)
                lease.Dispose();
        }

        /// <summary>
        /// Releases all streaming leases, restoring the autopilot's default rates.
        /// </summary>
        private void ReleaseStreamingLeases()
        {
            List<MessageRateLease> old;
            lock (_leaseLock)
            {
                old = _streamingLeases;
                _streamingLeases = new List<MessageRateLease>();
                _lastRateHz = 0;
            }
            foreach (var lease in old)
                lease.Dispose();
        }

        /// <summary>
        /// Subscribes to CAMERA_TRACKING_IMAGE_STATUS at the given rate.
        /// </summary>
        /// <remarks>
        /// Replaces any existing tracking lease.
        /// </remarks>
        /// <param name="ratehz">Desired rate in Hz.</param>
        public void SubscribeTracking(int ratehz)
        {
            if (!have_camera_information || parent?.parent == null)
                return;

            if (ratehz == _lastTrackingRateHz)
                return;

            var newLease = parent.parent.RateManager.Subscribe(
                parent.sysid, parent.compid,
                MAVLink.MAVLINK_MSG_ID.CAMERA_TRACKING_IMAGE_STATUS,
                ratehz,
                $"Camera({parent.sysid},{parent.compid})");

            MessageRateLease old;
            lock (_leaseLock)
            {
                old = _leaseTracking;
                _leaseTracking = newLease;
                _lastTrackingRateHz = ratehz;
            }
            old?.Dispose();
        }

        /// <summary>
        /// Releases the tracking message lease if one is active.
        /// </summary>
        public void StopTracking()
        {
            MessageRateLease old;
            lock (_leaseLock)
            {
                old = _leaseTracking;
                _leaseTracking = null;
                _lastTrackingRateHz = 0;
            }
            old?.Dispose();
        }

        /// <summary>
        /// Sends REQUEST_MESSAGE for VIDEO_STREAM_INFORMATION with retries and backoff.
        /// </summary>
        private void RequestVideoStreamWithRetry()
        {
            if (parent?.parent == null)
                return;

            var port = parent.parent;
            byte sysid = parent.sysid, compid = parent.compid;

            Task.Run(async () =>
            {
                for (int i = 0; i < 3; i++)
                {
                    // Stop trying if we disconnect
                    if (parent?.parent?.BaseStream?.IsOpen != true)
                        break;

                    if (VideoStreams.Keys.Any(k => k.Item1 == sysid && k.Item2 == compid))
                        break;

                    try
                    {
                        _ = port.doCommandAsync(sysid, compid,
                            MAVLink.MAV_CMD.REQUEST_MESSAGE,
                            (float)MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION,
                            0, 0, 0, 0, 0, 0, false);
                    }
                    catch (Exception ex)
                    {
                        log.Debug("Camera: REQUEST_MESSAGE(VIDEO_STREAM_INFORMATION) failed: " + ex.Message);
                    }

                    await Task.Delay(5000).ConfigureAwait(false);
                }
            });
        }

        /// <summary>
        /// Requests VIDEO_STREAM_INFORMATION from the camera (one-shot, best-effort).
        /// </summary>
        public void RequestVideoStreamInformation()
        {
            if (parent?.parent == null)
                return;

            parent.parent.doCommand(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.REQUEST_MESSAGE,
                (float)MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION,
                0, 0, 0, 0, 0, 0, false);
        }

        /// <summary>
        /// Sends command to capture one image.
        /// </summary>
        /// <param name="camera">The index of the camera to trigger, defaults to 0 meaning "all cameras".</param>
        private int _image_sequence = 1;
        public Task TakeSinglePictureAsync(int camera = 0)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.IMAGE_START_CAPTURE,
                camera,
                0, // Interval
                1, // One image
                _image_sequence++, // Sequence number (prevents retries from accidentally double-triggering)
                0, 0, 0
            );
        }

        /// <summary>
        /// Start capturing images at a specified rate.
        /// </summary>
        /// <param name="interval">Seconds between each image</param>
        /// <param name="camera">Camera index to trigger (optional). Defaults to 0 for "all cameras"</param>
        public Task StartIntervalCaptureAsync(float interval, int camera = 0)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.IMAGE_START_CAPTURE,
                camera,
                interval, // Interval
                0, // "Capture forever"
                0, // Sequence (unused in interval, set to 0)
                0, 0, // Reserved (set to 0)
                float.NaN // Reserved (set to NaN)
            );
        }

        /// <summary>
        /// Stop capturing images
        /// </summary>
        /// <param name="camera">Camera index to trigger (optional). Defaults to 0 for "all cameras"</param>
        public Task StopIntervalCaptureAsync(int camera = 0)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.IMAGE_STOP_CAPTURE,
                camera,
                float.NaN, float.NaN, float.NaN, // Reserved (set to NaN)
                0, 0, // Reserved (set to 0)
                float.NaN // Reserved (set to NaN)
            );
        }

        /// <summary>
        /// Start capturing video
        /// </summary>
        /// <param name="stream_id">Stream ID to record (optional). Defaults to 0 for "all streams"</param>
        public Task StartRecordingAsync(int stream_id = 0)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.VIDEO_START_CAPTURE,
                stream_id,
                float.NaN, // Frequency of CAMERA_CAPTURE_STATUS messages sent while recording (this parameter is not actually implemented in ArduPilot, and we are requesting CAMERA_CAPTURE_STATUS at all times anyway)
                float.NaN, float.NaN, // Reserved (set to NaN)
                0, 0, // Reserved (set to 0)
                float.NaN // Reserved (set to NaN)
            );
        }

        /// <summary>
        /// Stop capturing video
        /// </summary>
        /// <param name="stream_id">Stream ID to stop (optional). Defaults to 0 for "all streams"</param>
        public Task StopRecordingAsync(int stream_id = 0)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.VIDEO_STOP_CAPTURE,
                stream_id,
                float.NaN, float.NaN, float.NaN, // Reserved (set to NaN)
                0, 0, // Reserved (set to 0)
                float.NaN // Reserved (set to NaN)
            );
        }

        /// <summary>
        /// Control the camera zoom level.
        /// </summary>
        /// <param name="zoom_level">The zoom level to set. The range of valid values depend on the zoom type.</param>
        /// <param name="zoom_type">The type of zoom to perform</param>
        public Task SetZoomAsync(float zoom_level, MAVLink.CAMERA_ZOOM_TYPE zoom_type = MAVLink.CAMERA_ZOOM_TYPE.ZOOM_TYPE_RANGE)
        {
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.SET_CAMERA_ZOOM,
                (float)zoom_type,
                zoom_level,
                0, 0, 0, 0, 0
            );
        }

        /// <summary>
        /// Command the camera to track a point in the image.
        /// </summary>
        /// <param name="x">x position in the image, -1 to 1 (positive right)</param>
        /// <param name="y">y position in the image, -1 to 1 (positive down)</param>
        /// <returns></returns>
        public Task<bool> SetTrackingPointAsync(float x, float y)
        {
            // Check capabilities.
            if ((CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_TRACKING_POINT) == 0)
            {
                return Task.FromResult(false);
            }
            // Map -1:1 to 0:1
            x = (x + 1) / 2;
            y = (y + 1) / 2;
            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.CAMERA_TRACK_POINT,
                x, y,
                0, 0, 0, 0, 0
            );
        }


        /// <summary>
        /// Command the camera to track a rectangle in the image.
        /// </summary>
        /// <param name="x1">x position of one corner of the rectangle, -1 to 1 (positive right)</param>
        /// <param name="y1">y position of one corner of the rectangle, -1 to 1 (positive down)</param>
        /// <param name="x2">x position of the other corner of the rectangle, -1 to 1 (positive right)</param>
        /// <param name="y2">y position of the other corner of the rectangle, -1 to 1 (positive down)</param>
        /// <returns></returns>
        public Task<bool> SetTrackingRectangleAsync(float x1, float y1, float x2, float y2)
        {
            // Check capabilities.
            if ((CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.HAS_TRACKING_RECTANGLE) == 0)
            {
                return Task.FromResult(false);
            }

            // Map -1:1 to 0:1
            x1 = (x1 + 1) / 2;
            y1 = (y1 + 1) / 2;
            x2 = (x2 + 1) / 2;
            y2 = (y2 + 1) / 2;

            // Ensure x1 < x2 and y1 < y2
            if (x1 > x2)
            {
                (x2, x1) = (x1, x2);
            }
            if (y1 > y2)
            {
                (y2, y1) = (y1, y2);
            }

            return parent.parent.doCommandAsync(
                parent.sysid, parent.compid,
                MAVLink.MAV_CMD.CAMERA_TRACK_RECTANGLE,
                x1, y1, x2, y2,
                0, 0, 0
            );
        }

        /// <summary>
        /// Calculate the lat/lon/alt-msl of a point in the image, given its x/y position in the image.
        /// <param name="x">x position in the image, -1 to 1 (positive right)</param>
        /// <param name="y">y position in the image, -1 to 1 (positive down)</param>
        /// <returns>PointLatLngAlt with the calculated position, or null if the calculation failed</returns>
        public PointLatLngAlt CalculateImagePointLocation(double x, double y)
        {
            var imagePosition = new PointLatLngAlt(CameraFOVStatus.lat_image * 1e-7, CameraFOVStatus.lon_image * 1e-7, CameraFOVStatus.alt_image * 1e-3);
            if (x == 0 && y == 0)
            {
                return imagePosition;
            }

            var camPosition = new PointLatLngAlt(CameraFOVStatus.lat_camera * 1e-7, CameraFOVStatus.lon_camera * 1e-7, CameraFOVStatus.alt_camera * 1e-3);

            var height = camPosition.Alt - imagePosition.Alt;
            if (height < 0)
            {
                return null;
            }

            var dist = camPosition.GetDistance(imagePosition);
            var down_elevation = Math.Atan2(height, dist); // zero means pointing level, pi/2 is straight down
            down_elevation += y / 2 * VFOV * Math.PI / 180;
            down_elevation = Math.Max(0.0001, down_elevation);
            var out_distance = height * Math.Cos(down_elevation) / Math.Sin(down_elevation);
            out_distance = Math.Min(out_distance, 1e5);

            var side_angle = x / 2 * HFOV * Math.PI / 180;
            var side_distance = Math.Sqrt(out_distance * out_distance + height * height) * Math.Tan(side_angle);

            var bearing = camPosition.GetBearing(imagePosition);
            var pos = camPosition.newpos(bearing, out_distance).newpos(bearing + 90, side_distance);
            pos.Alt = imagePosition.Alt;
            return pos;
        }


        /// <summary>
        /// Calculate the 3D unit vector of a pixel in the camera frame, given its x/y position in the image.
        /// </summary>
        /// <param name="x">x position in the image, -1 to 1 (positive right)</param>
        /// <param name="y">y position in the image, -1 to 1 (positive down)</param>
        /// <returns></returns>
        private Vector3 CalculateImagePointVectorCameraFrame(double x, double y)
        {
            var vector = new Vector3(1, 0, 0); // Camera-frame vector pointing straight ahead
            if (HFOV != float.NaN && VFOV != float.NaN && x != 0 && y != 0)
            {
                var hfov = HFOV * Math.PI / 180;
                var vfov = VFOV * Math.PI / 180;

                vector.y = Math.Tan(x * hfov / 2); // x in the image is toward the right side of the plane (positive y in camera frame)
                vector.z = Math.Tan(y * vfov / 2); // y in the image is down (z in camera frame)
                vector.normalize();
            }

            return vector;
        }

        /// <summary>
        /// Calculate the 3D unit vector of a pixel in the world frame, given its x/y position in the image.
        /// </summary>
        /// <param name="x">x position in the image, -1 to 1 (positive right)</param>
        /// <param name="y">y position in the image, -1 to 1 (positive down)</param>
        /// <returns></returns>
        public Vector3 CalculateImagePointVector(double x, double y)
        {
            if (CameraFOVStatus.q == null)
            {
                return new Vector3(1);
            }
            var v = CalculateImagePointVectorCameraFrame(x, y);
            var q = new Quaternion(CameraFOVStatus.q[0], CameraFOVStatus.q[1], CameraFOVStatus.q[2], CameraFOVStatus.q[3]);
            return q.body_to_earth(v);
        }

        /// <summary>
        /// Calculate a rotation quaternion that will rotate the camera to point at a pixel in the image.
        /// </summary>
        /// <param name="x">x position in the image, -1 to 1 (positive right)</param>
        /// <param name="y">y position in the image, -1 to 1 (positive down)</param>
        /// <returns></returns>
        public Quaternion CalculateImagePointRotation(double x, double y)
        {
            var v1 = CalculateImagePointVectorCameraFrame(0, 0);
            var v2 = CalculateImagePointVectorCameraFrame(x, y);

            if (v1 == -v2)
            {
                return Quaternion.from_axis_angle(new Vector3(0, 0, 1), Math.PI); // 180 degree rotation around z axis
            }

            // The axis of rotation is the cross product of the two vectors
            var axis = v1 % v2;
            if(axis.length() == 0)
            {
                return new Quaternion();
            }
            axis.normalize();

            return Quaternion.from_axis_angle(axis, Math.Acos(v1 * v2));
        }
    }
}
