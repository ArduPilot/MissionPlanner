using System;
using System.Collections.Generic;
using System.IO;

namespace MissionPlanner.Utilities
{
    public interface IMAVLinkInterface
    {
        int sysidcurrent { get; set; }
        int compidcurrent { get; set; }

        void sendPacket(object indata, int sysid, int compid);

        KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> SubscribeToPacketType(
            MAVLink.MAVLINK_MSG_ID type,
            Func<MAVLink.MAVLinkMessage, bool> function, bool exclusive = false);

        void UnSubscribeToPacketType(KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> item);
        void UnSubscribeToPacketType(MAVLink.MAVLINK_MSG_ID msgtype, Func<MAVLink.MAVLinkMessage, bool> item);
    }

    public interface IMAVLinkInterfaceLogRead
    {
        BinaryReader logplaybackfile { get; set; }
        bool logreadmode { get; set; }
        bool speechenabled { get; set; }

        MAVLink.MAVLinkMessage readPacket();

        DateTime lastlogread { get; set; }
    }
}