using System;
using System.Collections.Generic;
using System.IO;

namespace MissionPlanner.Utilities
{
    public interface IMAVLinkInterface
    {
        uint sysidcurrent { get; set; }
        int compidcurrent { get; set; }

        void sendPacket(object indata, uint sysid, int compid);

        int SubscribeToPacketType(
            MAVLink.MAVLINK_MSG_ID type,
            Func<MAVLink.MAVLinkMessage, bool> function, uint sysid, byte compid, bool exclusive = false);

        void UnSubscribeToPacketType(int msgid);

        event EventHandler<MAVLink.MAVLinkMessage> OnPacketReceived;
        event EventHandler<MAVLink.MAVLinkMessage> OnPacketSent;
        event EventHandler ParamListChanged;
        event EventHandler MavChanged;
        event EventHandler CommsClose;
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