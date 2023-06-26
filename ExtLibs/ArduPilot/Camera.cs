using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Camera
    {
        public void test(MAVLinkInterface mavint)
        {
            try
            {
                var mav = mavint.MAVlist.FirstOrDefault(a => a.compid == (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_CAMERA);

                if (mav == null)
                    return;

                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_VIDEO_STREAM_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_SETTINGS, 0, 0, 0, 0, 0, 0, 0);
                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.SET_CAMERA_MODE, 0, 0, 0, 0, 0, 0, 0);  // p2 = 1 for recodring hint
                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_STORAGE_INFORMATION, 0, 0, 0, 0, 0, 0, 0);
                mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_START_STREAMING, 0, 0, 0, 0, 0, 0, 0);
                //mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_STOP_STREAMING, 0, 0, 0, 0, 0, 0, 0);
                //mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_START_CAPTURE, 0, 0, 0, 0, 0, 0, 0);
                //mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.VIDEO_STOP_CAPTURE, 0, 0, 0, 0, 0, 0, 0);
                //mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.IMAGE_START_CAPTURE, 0, 0, 0, 0, 0, 0, 0);
                //mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_CAPTURE_STATUS, 0, 0, 0, 0, 0, 0, 0);
                //MAVLINK_MSG_ID_SET_VIDEO_STREAM_SETTINGS
            }
            catch { }
        }

        public void CameraInfo(MAVState mav, Action<MAVLink.mavlink_camera_information_t> act)
        {
            var sub = mav.parent.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION,
                message =>
                {
                    act((MAVLink.mavlink_camera_information_t)message.data);
                    return true;
                }, mav.sysid, mav.compid);

            mav.parent.doCommand(mav.sysid, mav.compid, MAVLink.MAV_CMD.REQUEST_CAMERA_INFORMATION, 0, 0, 0, 0, 0, 0, 0);

            new Timer((a) => { mav.parent.UnSubscribeToPacketType(sub); }, sub, 10000, Timeout.Infinite);
        }
    }
}
