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
                try
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

                    ipEP = new IPEndPoint(IPAddress.Parse("192.168.144.10"), 14553);

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
                    //mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);


                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_BASIC_ID,
                        new MAVLink.mavlink_open_drone_id_basic_id_t()
                        {
                            target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL,
                            target_system = 42,
                            id_or_mac = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, (byte)DateTime.Now.Second },
                            id_type = (byte)MAVLink.MAV_ODID_ID_TYPE.SERIAL_NUMBER,
                            ua_type = (byte)MAVLink.MAV_ODID_UA_TYPE.HELICOPTER_OR_MULTIROTOR,




                        }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);


                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_LOCATION,
                        new MAVLink.mavlink_open_drone_id_location_t()
                        {
                            target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL,
                            target_system = 42,
                            latitude = (int)(47.3980398 * 1e7),
                            longitude = (int)(8.5455728 * 1e7),
                            altitude_barometric = 100,
                            altitude_geodetic = 100,
                            height = 100,
                            timestamp = (float)DateTime.UtcNow.Second


                        }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);



                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SELF_ID,
            new MAVLink.mavlink_open_drone_id_self_id_t()
            {
                target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL,
                target_system = 42,

            }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SYSTEM,
            new MAVLink.mavlink_open_drone_id_system_t()
            {
                target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL,
                target_system = 42,

            }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_SYSTEM_UPDATE,
            new MAVLink.mavlink_open_drone_id_system_update_t()
            {
                target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ODID_TXRX_1,
                target_system = 42,

            }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                    sendpacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.OPEN_DRONE_ID_OPERATOR_ID,
          new MAVLink.mavlink_open_drone_id_operator_id_t()
          {
              target_component = (byte)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ALL,
              target_system = 42,

          }, false, 255, 190, seqno++);
                    mavlinkudp.Send(sendpacket, sendpacket.Length, ipEP);

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }

    internal struct mavlink_vfr_hud_EDIT_t
    {
    }
}
