using System;
using System.Collections.Generic;
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
            //LOG
            using (CollectionBuffer col = new CollectionBuffer(File.OpenRead(logfile)))
            using (var outfilestream = File.Open(outputfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                foreach (var dfItem in col.GetEnumeratorTypeAll())
                {
                    if (dfItem.msgtype.StartsWith("GPS"))
                        continue;
                    if (dfItem.msgtype.StartsWith("CMD"))
                        continue;
                    if (dfItem.msgtype.StartsWith("CAM"))
                        continue;
                    if (dfItem.msgtype.StartsWith("TRIG"))
                        continue;
                    if (dfItem.msgtype.StartsWith("AHR2"))
                        continue;
                    if (dfItem.msgtype.StartsWith("POS"))
                        continue;
                    if (dfItem.msgtype.StartsWith("TERR"))
                        continue;
                    if (dfItem.msgtype.StartsWith("SBFE"))
                        continue;
                    if (dfItem.msgtype.StartsWith("ORGN"))
                        continue;

                    var str = String.Join(",", dfItem.items);
                    outfilestream.Write(ASCIIEncoding.ASCII.GetBytes(str), 0, str.Length);
                }
            }

            return;
            // TLOG
            Comms.CommsFile tlogFile = new CommsFile();
            tlogFile.Open(logfile);

            using (var stream = new CommsStream(tlogFile, tlogFile.BytesToRead))
            using (var outfilestream = File.Open(outputfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                MAVLink.MavlinkParse parse = new MAVLink.MavlinkParse();

                double lat = 0;
                double lng = 0;

                while (stream.Position < stream.Length)
                {
                    var packet = parse.ReadPacket(stream);

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                    {
                        packet.ToStructure<MAVLink.mavlink_global_position_int_t>();
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.POSITION_TARGET_GLOBAL_INT)
                    {
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.SET_HOME_POSITION)
                    {
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.HOME_POSITION)
                    {
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.SET_POSITION_TARGET_GLOBAL_INT)
                    {
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.LOCAL_POSITION_NED)
                    {
                    }

                    if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.COMMAND_LONG)
                    {
                    }

                    if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM)
                    {
                    }

                    if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT)
                    {
                    }

                    byte[] datearray =
                        BitConverter.GetBytes(
                            (UInt64) ((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000));
                    Array.Reverse(datearray);
                    outfilestream.Write(datearray, 0, datearray.Length);
                    outfilestream.Write(packet.buffer, 0, packet.Length);
                }
            }
        }
    }
}
