using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace MissionPlanner.ArduPilot.Mavlink
{
    public class CameraProtocol
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MAVState parent;

        public static event EventHandler<String> OnRTSPDetected;

        public async Task StartID(MAVState mavState)
        {
            parent = mavState;

            mavState.parent.OnPacketReceived += (sender, message) =>
            {
                if (message.sysid != parent.sysid || message.compid != parent.compid)
                    return;

                switch ((MAVLink.MAVLINK_MSG_ID) message.msgid)
                {
                    case MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION:

                        CameraInformation = ((MAVLink.mavlink_camera_information_t) message.data);

                        if (ASCIIEncoding.UTF8.GetString(CameraInformation.cam_definition_uri) != "")
                        {
                            // get the uri
                        }

                        //if ((info.flags & (int) MAVLink.CAMERA_CAP_FLAGS.HAS_MODES) > 0)
                        try
                        {
                            Task.Run(async () =>
                            {
                                if ((CameraInformation.flags & (int) MAVLink.CAMERA_CAP_FLAGS.CAPTURE_IMAGE) > 0)
                                    await parent.parent.doCommandAsync(parent.sysid, parent.compid,
                                        MAVLink.MAV_CMD.REQUEST_CAMERA_SETTINGS, 0, 0, 0, 0, 0, 0, 0).ConfigureAwait(false);
                                if ((CameraInformation.flags & (int) MAVLink.CAMERA_CAP_FLAGS.CAPTURE_VIDEO) > 0)
                                    await parent.parent.doCommandAsync(parent.sysid, parent.compid,
                                        MAVLink.MAV_CMD.REQUEST_VIDEO_STREAM_INFORMATION, 0, 0, 0, 0, 0, 0, 0).ConfigureAwait(false);
                                if ((CameraInformation.flags & (int) MAVLink.CAMERA_CAP_FLAGS.CAPTURE_VIDEO) > 0)
                                    await parent.parent.doCommandAsync(parent.sysid, parent.compid,
                                        MAVLink.MAV_CMD.REQUEST_STORAGE_INFORMATION, 0, 0, 0, 0, 0, 0, 0).ConfigureAwait(false);
                            });
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        break;
                    case MAVLink.MAVLINK_MSG_ID.CAMERA_SETTINGS:
                        var cameraSettings = ((MAVLink.mavlink_camera_settings_t) message.data);

                        break;
                    case MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_INFORMATION:
                        VideoStreamInformation = ((MAVLink.mavlink_video_stream_information_t) message.data);
                        var uri = ASCIIEncoding.UTF8.GetString(VideoStreamInformation.uri);
                        int ind = uri.IndexOf('\0');
                        if (ind != -1)
                            uri = uri.Substring(0, ind);
                        if (uri.ToLower().StartsWith("rtsp://"))
                        {
                            Task.Run(() =>
                            {
                                try
                                {
                                    OnRTSPDetected?.Invoke(this, uri);
                                }
                                catch (Exception e)
                                {
                                    log.Error(e);
                                }
                            });
                        }
                        break;
                    case MAVLink.MAVLINK_MSG_ID.VIDEO_STREAM_STATUS:
                        var videoStreamStatus = ((MAVLink.mavlink_video_stream_status_t) message.data);

                        break;
                    case MAVLink.MAVLINK_MSG_ID.CAMERA_CAPTURE_STATUS:
                        var cameraCaptureStatus = ((MAVLink.mavlink_camera_capture_status_t) message.data);
                        
                        break;
                    case MAVLink.MAVLINK_MSG_ID.CAMERA_IMAGE_CAPTURED:
                        var cameraImageCaptured = ((MAVLink.mavlink_camera_image_captured_t)message.data);

                        break;
                }
            };

            try
            {
                var resp = await parent.parent.doCommandAsync(parent.sysid, parent.compid,
                    MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
                if (resp)
                {
                    // no use
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public MAVLink.mavlink_video_stream_information_t VideoStreamInformation { get; set; }
        public MAVLink.mavlink_camera_information_t CameraInformation { get; set; }

        public MAVLink.CAMERA_CAP_FLAGS GetCameraModes()
        {
            return (MAVLink.CAMERA_CAP_FLAGS)CameraInformation.flags;
        }
    }
}
