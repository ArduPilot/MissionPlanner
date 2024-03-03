using System;
using System.Reflection;
using System.Threading.Tasks;
using Core.Geometry;
using GeoAPI.DataStructures;
using log4net;

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

        public MAVLink.mavlink_camera_information_t CameraInformation { get; set; }
        public MAVLink.mavlink_camera_settings_t CameraSettings { get; set; }
        public MAVLink.mavlink_camera_capture_status_t CameraCaptureStatus { get; set; }

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
                if(parent?.parent != null)
                {
                    // New-style request
                    var resp = await parent?.parent?.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.REQUEST_MESSAGE,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION,
                        0, 0, 0, 0, 0, 0
                    );
                    // Fall back to deprecated request message
                    if (resp)
                    {
                        await parent?.parent?.doCommandAsync(
                            parent.sysid, parent.compid,
                            MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION,
                            0, 0, 0, 0, 0, 0, 0
                        );
                    }
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
                CameraInformation = ((MAVLink.mavlink_camera_information_t)message.data);
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS:
                CameraSettings = ((MAVLink.mavlink_camera_settings_t)message.data);
                break;
            case MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS:
                CameraCaptureStatus = ((MAVLink.mavlink_camera_capture_status_t)message.data);
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
            if(parent?.parent == null)
            {
                return;
            }

            float interval_us = (float)(1e6 / ratehz);

            if (!have_camera_information)
            {
                Task.Run(RequestCameraInformationAsync);
            }

            if (HasModes || HasZoom || HasFocus)
            {
                Task.Run(async () =>
                {
                    await parent.parent.doCommandAsync(
                        parent.sysid, parent.compid,
                        MAVLink.MAV_CMD.SET_MESSAGE_INTERVAL,
                        (float)MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS,
                        interval_us,
                        0, 0, 0, 0, 0
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
                        0, 0, 0, 0, 0
                    ).ConfigureAwait(false);
                });
            }
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
    }
}
