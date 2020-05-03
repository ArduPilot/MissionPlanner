using System.Linq;
using MissionPlanner.Plugin;

namespace MavlinkMessagePlugin
{
    public class MavlinkMessagePlugin : Plugin
    {
        string _Name = "MavlinkMessagePlugin";
        string _Version = "0.1";
        string _Author = "Michael Oborne";

        public override string Name
        {
            get { return _Name; }
        }

        public override string Version
        {
            get { return _Version; }
        }

        public override string Author
        {
            get { return _Author; }
        }

        public override bool Init()
        {
            var oldlist = MAVLink.MAVLINK_MESSAGE_INFOS.ToList();
            oldlist.Add(new MAVLink.message_info(27499, "VFR_HUD_EDIT", 20, 20, 20, typeof(mavlink_vfr_hud_EDIT_t)));
            MAVLink.MAVLINK_MESSAGE_INFOS = oldlist.ToArray();

            var parse = new MAVLink.MavlinkParse();
            var packet = parse.GenerateMAVLinkPacket20((MAVLink.MAVLINK_MSG_ID) 274, new mavlink_vfr_hud_EDIT_t());
            

            loopratehz = 1f;
            return true;
        }

        public override bool Loaded()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}