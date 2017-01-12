using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MissionPlanner.Log
{
    class LogSort
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static bool issitl = false;

        public static void SortLogs(string[] logs, string masterdestdir = "")
        {
            foreach (var logfile in logs)
            {
                if(masterdestdir == "")
                    masterdestdir = Path.GetDirectoryName(logfile);

                issitl = false;

                FileInfo info = new FileInfo(logfile);

                // delete 0 size files
                if (info.Length == 0)
                {
                    try
                    {
                        File.Delete(logfile);
                    }
                    catch
                    {
                    }
                    continue;
                }

                // move small logs - most likerly invalid
                if (info.Length <= 1024)
                {
                    try
                    {
                        string destdir = masterdestdir + Path.DirectorySeparatorChar
                                         + "SMALL" + Path.DirectorySeparatorChar;

                        if (!Directory.Exists(destdir))
                            Directory.CreateDirectory(destdir);

                        log.Info("Move log small " + logfile + " to " + destdir + Path.GetFileName(logfile));

                        movefileusingmask(logfile, destdir);
                    }
                    catch
                    {
                    }
                    continue;
                }

                try
                {
                    using (MAVLinkInterface mine = new MAVLinkInterface())
                    using (
                        mine.logplaybackfile =
                            new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        mine.logreadmode = true;
                        mine.speechenabled = false;

                        var midpoint = mine.logplaybackfile.BaseStream.Length / 2;

                        mine.logplaybackfile.BaseStream.Seek(midpoint, SeekOrigin.Begin);

                        // used for sitl detection
                        mine.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SIMSTATE, sitldetection);
                        mine.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SIM_STATE, sitldetection);

                        MAVLink.MAVLinkMessage hbpacket = mine.getHeartBeat();
                        MAVLink.MAVLinkMessage hbpacket1 = mine.getHeartBeat();
                        MAVLink.MAVLinkMessage hbpacket2 = mine.getHeartBeat();
                        MAVLink.MAVLinkMessage hbpacket3 = mine.getHeartBeat();

                        if (hbpacket.Length == 0 && hbpacket1.Length == 0 && hbpacket2.Length == 0 && hbpacket3.Length == 0)
                        {
                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();

                            if (!Directory.Exists(masterdestdir + Path.DirectorySeparatorChar + "BAD"))
                                Directory.CreateDirectory(masterdestdir + Path.DirectorySeparatorChar +
                                                          "BAD");

                            log.Info("Move log bad " + logfile + " to " + masterdestdir +
                                     Path.DirectorySeparatorChar + "BAD" + Path.DirectorySeparatorChar +
                                     Path.GetFileName(logfile));

                            movefileusingmask(logfile,
                                masterdestdir + Path.DirectorySeparatorChar + "BAD" +
                                Path.DirectorySeparatorChar);
                            continue;
                        }

                        if (hbpacket.Length != 0)
                        {
                            MAVLink.mavlink_heartbeat_t hb = (MAVLink.mavlink_heartbeat_t) mine.DebugPacket(hbpacket);
                        }

                        if (hbpacket1.Length != 0)
                        {
                            MAVLink.mavlink_heartbeat_t hb1 = (MAVLink.mavlink_heartbeat_t)mine.DebugPacket(hbpacket1);
                        }

                        if (hbpacket2.Length != 0)
                        {
                            MAVLink.mavlink_heartbeat_t hb2 = (MAVLink.mavlink_heartbeat_t)mine.DebugPacket(hbpacket2);
                        }

                        if (hbpacket3.Length != 0)
                        {
                            MAVLink.mavlink_heartbeat_t hb3 = (MAVLink.mavlink_heartbeat_t)mine.DebugPacket(hbpacket3);
                        }

                        // find most appropriate
                        if (mine.MAVlist.Count > 1)
                        {
                            foreach (var mav in mine.MAVlist)
                            {
                                if (mav.aptype == MAVLink.MAV_TYPE.ANTENNA_TRACKER)
                                    continue;
                                if (mav.aptype == MAVLink.MAV_TYPE.GCS)
                                    continue;

                                mine.sysidcurrent = mav.sysid;
                                mine.compidcurrent = mav.compid;
                            }
                        }

                        mine.logreadmode = false;
                        mine.logplaybackfile.Close();

                        string destdir = masterdestdir + Path.DirectorySeparatorChar
                                         + mine.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                                         + mine.MAV.sysid + Path.DirectorySeparatorChar;

                        if (issitl)
                        {
                            destdir = masterdestdir + Path.DirectorySeparatorChar 
                                + "SITL" + Path.DirectorySeparatorChar 
                                + mine.MAV.aptype.ToString() + Path.DirectorySeparatorChar 
                                + mine.MAV.sysid + Path.DirectorySeparatorChar;
                        }

                        if (!Directory.Exists(destdir))
                            Directory.CreateDirectory(destdir);

                        movefileusingmask(logfile, destdir);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private static bool sitldetection(MAVLink.MAVLinkMessage arg)
        {
            issitl = true;

            return true;
        }

        static void movefileusingmask(string logfile, string destdir)
        {
            string dir = Path.GetDirectoryName(logfile);
            string filter = Path.GetFileNameWithoutExtension(logfile) + "*";

            string[] files = Directory.GetFiles(dir, filter);
            foreach (var file in files)
            {
                log.Info("Move log " + file + " to " + destdir + Path.GetFileName(file));

                if (file == destdir + Path.GetFileName(file))
                    continue;

                File.Move(file, destdir + Path.GetFileName(file));
            }
        }
    }
}