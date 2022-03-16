using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MavlinkMessagePlugin;

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


            var oldlist = MAVLink.MAVLINK_MESSAGE_INFOS.ToList();
            oldlist.Add(new MAVLink.message_info(27499, "VFR_HUD_EDIT", 20, 20, 20, typeof(mavlink_vfr_hud_EDIT_t)));

            MAVLink.MAVLINK_MESSAGE_INFOS = oldlist.ToArray();

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
                        autopilot = (byte) MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                        base_mode = (byte) MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED,
                        system_status = (byte) MAVLink.MAV_STATE.ACTIVE,
                        type = (byte) MAVLink.MAV_TYPE.QUADROTOR
                    }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                //sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION,new MAVLink.mavlink_autopilot_version_t(){ }, false, 1, 1, seqno++);
                //mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);


                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.AHRS,
                    new MAVLink.mavlink_ahrs_t()
                        { }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.ATTITUDE,
                    new MAVLink.mavlink_attitude_t()
                        {pitch = 0, roll = 0, yaw = 0, time_boot_ms = (uint)DateTime.Now.Ticks}, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT,
                    new MAVLink.mavlink_global_position_int_t()
                    {
                        lat = (int) (-35 * 1e7), lon = (int) (117.89 * 1e7), alt = (40 * 1000), hdg = 45 * 100,
                        time_boot_ms = (uint) DateTime.Now.Ticks
                    }, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.PARAM_VALUE,
                    new MAVLink.mavlink_param_value_t()
                        {param_count = 0, param_index = 0, param_type = 0, param_value = 0}, false, 1, 1, seqno++);
                mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);





                Thread.Sleep(100);
            }
        }
    }

    internal struct mavlink_vfr_hud_EDIT_t
    {
    }
}
