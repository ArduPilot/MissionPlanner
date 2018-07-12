using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class Privacy
    {
        public static void anonymise(string logfile, string outputfile)
        {
            if (!File.Exists(logfile))
                return;

            var rand = new Random();

            var latrandom = rand.NextDouble() * (rand.NextDouble() * 3);
            
            // TLOG
            if (logfile.ToLower().EndsWith(".tlog"))
            {
                Comms.CommsFile tlogFile = new CommsFile();
                tlogFile.Open(logfile);

                using (var stream = new CommsStream(tlogFile, tlogFile.BytesToRead))
                using (var outfilestream = File.Open(outputfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    MAVLink.MavlinkParse parse = new MAVLink.MavlinkParse(true);

                    var block = new Type[]
                    {
                        typeof(MAVLink.mavlink_fence_point_t),
                        typeof(MAVLink.mavlink_simstate_t),
                        typeof(MAVLink.mavlink_rally_point_t),
                        typeof(MAVLink.mavlink_ahrs2_t),
                        typeof(MAVLink.mavlink_camera_feedback_t),
                        typeof(MAVLink.mavlink_ahrs3_t),
                        typeof(MAVLink.mavlink_deepstall_t),
                        typeof(MAVLink.mavlink_gps_raw_int_t),
                        typeof(MAVLink.mavlink_global_position_int_t),
                        typeof(MAVLink.mavlink_set_gps_global_origin_t),
                        typeof(MAVLink.mavlink_gps_global_origin_t),
                        typeof(MAVLink.mavlink_global_position_int_cov_t),
                        typeof(MAVLink.mavlink_set_position_target_global_int_t),
                        typeof(MAVLink.mavlink_hil_state_t),
                        typeof(MAVLink.mavlink_sim_state_t),
                        typeof(MAVLink.mavlink_hil_gps_t),
                        typeof(MAVLink.mavlink_hil_state_quaternion_t),
                        typeof(MAVLink.mavlink_gps2_raw_t),
                        typeof(MAVLink.mavlink_terrain_request_t),
                        typeof(MAVLink.mavlink_terrain_check_t),
                        typeof(MAVLink.mavlink_terrain_report_t),
                        typeof(MAVLink.mavlink_follow_target_t),
                        typeof(MAVLink.mavlink_gps_input_t),
                        typeof(MAVLink.mavlink_high_latency_t),
                        typeof(MAVLink.mavlink_home_position_t),
                        typeof(MAVLink.mavlink_set_home_position_t),
                        typeof(MAVLink.mavlink_adsb_vehicle_t),
                        typeof(MAVLink.mavlink_camera_image_captured_t),
                        typeof(MAVLink.mavlink_uavionix_adsb_out_dynamic_t),
                        typeof(MAVLink.mavlink_global_position_int_t),
                        typeof(MAVLink.mavlink_set_home_position_t),
                        typeof(MAVLink.mavlink_home_position_t),
                        typeof(MAVLink.mavlink_set_position_target_global_int_t),
                        typeof(MAVLink.mavlink_local_position_ned_t),
                        typeof(MAVLink.mavlink_command_long_t),
                        typeof(MAVLink.mavlink_mission_item_t),
                        typeof(MAVLink.mavlink_mission_item_int_t),
                        typeof(MAVLink.mavlink_uavionix_adsb_out_cfg_t)
                    };

                    var checks = new string[]
                    {
                        "lat", "latitude", "lat_int", "landing_lat",
                        "path_lat", "arc_entry_lat", "gpsLat", "gpsOffsetLat"
                    };

                    while (stream.Position < stream.Length)
                    {
                        var packet = parse.ReadPacket(stream);

                        if (packet == null)
                            continue;

                        var msginfo = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo(packet.msgid);

                        if (block.Contains(msginfo.type))
                        {
                            bool valid = false;
                            var oldrxtime = packet.rxtime;
                            foreach (var check in checks)
                            {
                                var field = msginfo.type.GetField(check);

                                if (field != null)
                                {
                                    var pkt = packet.data;
                                    var value = field.GetValue(pkt);
                                    if (value is Int32)
                                    {
                                        if ((int) value != 0)
                                            field.SetValue(pkt, (int) ((int) value + latrandom * 1e7));

                                        packet = new MAVLink.MAVLinkMessage(
                                            parse.GenerateMAVLinkPacket20((MAVLink.MAVLINK_MSG_ID) msginfo.msgid,
                                                pkt, false, packet.sysid, packet.compid, packet.seq));
                                        valid = true;
                                    }
                                    else if (value is Single)
                                    {
                                        if ((Single) value != 0)
                                            field.SetValue(pkt, (Single) value + latrandom);

                                        packet = new MAVLink.MAVLinkMessage(
                                            parse.GenerateMAVLinkPacket20((MAVLink.MAVLINK_MSG_ID) msginfo.msgid,
                                                pkt, false, packet.sysid, packet.compid, packet.seq));
                                        valid = true;
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                            }

                            packet.rxtime = oldrxtime;
                        }


                        byte[] datearray =
                            BitConverter.GetBytes(
                                (UInt64) ((packet.rxtime.ToUniversalTime() - new DateTime(1970, 1, 1))
                                          .TotalMilliseconds * 1000));
                        Array.Reverse(datearray);
                        outfilestream.Write(datearray, 0, datearray.Length);
                        outfilestream.Write(packet.buffer, 0, packet.Length);
                    }
                }
            }
            else //LOG
            {
                using (CollectionBuffer col = new CollectionBuffer(File.OpenRead(logfile)))
                using (var outfilestream = File.Open(outputfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                {
                    foreach (var dfItem in col.GetEnumeratorTypeAll())
                    {
                        var index = col.dflog.FindMessageOffset(dfItem.msgtype, "lat");

                        if (index != -1)
                        {
                            dfItem.items[index] =
                                (Double.Parse(dfItem.items[index], CultureInfo.InvariantCulture) + latrandom).ToString(
                                    CultureInfo.InvariantCulture);
                        }

                        var str = String.Join(",", dfItem.items) + "\r\n";
                        outfilestream.Write(ASCIIEncoding.ASCII.GetBytes(str), 0, str.Length);
                    }
                }
          

            }
        }
    }
}
