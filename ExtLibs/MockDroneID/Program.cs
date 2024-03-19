using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Mock
{
    class Program
    {
        static void Main(string[] args)
        {
            var mavlinkudp = new UdpClient(14550);
            var mavparse = new MAVLink.MavlinkParse();
            var seqno = 0;
            IPEndPoint ipEP = new IPEndPoint(IPAddress.Any, 0);


            while (true)
            {
                while (mavlinkudp.Available > 0)
                {
                    ipEP = new IPEndPoint(IPAddress.Any, 0);
                    try
                    {
                        var rawpacket = mavlinkudp.Receive(ref ipEP);

                        var packet = mavparse.ReadPacket(new MemoryStream(rawpacket));
                        Console.WriteLine(packet);
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (ipEP.Address == IPAddress.Any)
                {
                    Thread.Sleep(100);
                    continue;
                }

                var sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT,
                    new MAVLink.mavlink_heartbeat_t()
                    {
                        autopilot = (byte)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                        base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                        system_status = (byte)MAVLink.MAV_STATE.ACTIVE,
                        type = (byte)MAVLink.MAV_TYPE.QUADROTOR
                    }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);


                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_BASIC_ID,
                    new MAVLink.mavlink_open_drone_id_basic_id_t()
                    {

                    }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);


                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_LOCATION,
                    new MAVLink.mavlink_open_drone_id_location_t()
                    {

                    }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

              

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SELF_ID,
        new MAVLink.mavlink_open_drone_id_self_id_t()
        {

        }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SYSTEM,
        new MAVLink.mavlink_open_drone_id_system_t()
        {

        }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SYSTEM_UPDATE,
        new MAVLink.mavlink_open_drone_id_system_update_t()
        {

        }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_OPERATOR_ID,
      new MAVLink.mavlink_open_drone_id_operator_id_t()
      {

      }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                Thread.Sleep(1000);
            }
        }
    }

    internal struct mavlink_vfr_hud_EDIT_t
    {
    }
}
