using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
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

        public MAVLink.mavlink_camera_information_t CameraInformation { get; private set; }
        public MAVLink.mavlink_camera_settings_t CameraSettings { get; private set; }
        public MAVLink.mavlink_camera_capture_status_t CameraCaptureStatus { get; private set; }
        public MAVLink.mavlink_camera_fov_status_t CameraFOVStatus { get; private set; }
        public MAVLink.mavlink_camera_tracking_image_status_t CameraTrackingImageStatus { get; private set; }

        public static ConcurrentDictionary<(byte, byte, byte), MAVLink.mavlink_video_stream_information_t> VideoStreams { get; private set; } = new ConcurrentDictionary<(byte, byte, byte), MAVLink.mavlink_video_stream_information_t>();

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
        /// Initializes the camera protocol by setting up message parsing and requesting initial camera information.
        /// </summary>
        /// <param name="mavState">MAVState parent of this driver</param>
        public Task StartID(MAVState mavState)
        {
            parent = mavState;

            mavState.parent.OnPacketReceived += ParseMessages;

            return RequestCameraInformationAsync();
        }

        /// <summary>
        /// Sends an asynchronous request to fetch camera information via.
        /// </summary>
        private async Task RequestCameraInformationAsync()
        {
            try
            {
                if (parent?.parent != null)
                {
                    // New-style request
                    var resp = await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.REQUEST_MESSAGE,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION,
                        0, 0, 0, 0, 0, 0
                    );
                    // Fall back to deprecated request message
                    if (!resp)
                    {
                        await parent.parent.doCommandAsync(
                            parent.sysid, parent.compid,
                            MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION,
                            0, 0, 0, 0, 0, 0, 0,
                            false // Don't wait for response
                        );
                    }

                    // Get video stream information as well
                    await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.REQUEST_MESSAGE,
                        (float)MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION,
                        0, 0, 0, 0, 0, 0,
                        false // Don't wait for response
                    );
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Event handler for OnPacketReceived.
        /// Parses incoming MAVLink messages related to camera operations and updates internal state accordingly.
        /// </summary>
        /// <param name="sender">MAVLink interface</param>
        /// <param name="message">MAVLink message to parse</param>
        public void ParseMessages(object sender, MAVLink.MAVLinkMessage message)
        {
            if (message.sysid != parent.sysid || message.compid != parent.compid)
                return;

            switch ((MAVLink.MAVLINK_MSG_ID)message.msgid)
            {
            case MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION:
                have_camera_information = true;
                CameraInformation = (MAVLink.mavlink_camera_information_t)message.data;
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
        /// Requests that the camera send specific messages types at a specified rate.
        /// The messages are selected based on the camera's reported capabilities.
        /// </summary>
        /// <param name="ratehz">Message frequency in messages per second.</param>
        public void RequestMessageIntervals(int ratehz)
        {
            if (parent?.parent == null)
            {
                return;
            }

            float interval_us = (float)(1e6 / ratehz);

            Task.Run(RequestCameraInformationAsync);

            // Request FOV status
            Task.Run(async () =>
            {
                await parent.parent.doCommandAsync(
                    parent.sysid, parent.compid,
                    MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                    (float)MAVLink.MAVLINK_MSG_ID.CAMERA_FOV_STATUS,
                    interval_us,
                    0, 0, 0, 0, 0,
                    false // Don't wait for response
                ).ConfigureAwait(false);
            });

            // Get camera settings
            if (HasModes || HasZoom || HasFocus)
            {
                Task.Run(async () =>
                {
                    await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS,
                        interval_us,
                        0, 0, 0, 0, 0,
                        false // Don't wait for response
                    ).ConfigureAwait(false);
                });
            }

            // We use the capability flags directly here, and NOT whether we are currently able to do these things
            var can_capture_video = (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAPTURE_VIDEO) > 0;
            var can_capture_image = (CameraInformation.flags & (int)MAVLink.CAMERA_CAP_FLAGS.CAPTURE_IMAGE) > 0;
            if (can_capture_video || can_capture_image)
            {
                Task.Run(async () =>
                {
                    await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS,
                        interval_us,
                        0, 0, 0, 0, 0,
                        false // Don't wait for response
                    ).ConfigureAwait(false);
                });
            }
        }

        public void RequestTrackingMessageInterval(int ratehz)
        {
            if (parent?.parent == null)
            {
                return;
            }

            float interval_us = (float)(1e6 / ratehz);

            Task.Run(async () =>
            {
                await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_TRACKING_IMAGE_STATUS,
                        interval_us,
                        0, 0, 0, 0, 0,
                        false // Don't wait for response
                ).ConfigureAwait(false);
            });
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
