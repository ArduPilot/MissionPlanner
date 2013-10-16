using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Log
{
    class LogSort
    {
        public static void SortLogs(string[] logs)
        {
            foreach (var logfile in logs)
            {
                FileInfo info = new FileInfo(logfile);

                if (info.Length == 0)
                {
                    try
                    {
                        File.Delete(logfile);
                    }
                    catch { }
                    continue;
                }

                MAVLinkInterface mine = new MAVLinkInterface();

                try
                {
                    using (mine.logplaybackfile = new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        mine.logreadmode = true;

                        byte[] hbpacket = mine.getHeartBeat();

                        if (hbpacket.Length == 0)
                            continue;

                        MAVLink.mavlink_heartbeat_t hb = (MAVLink.mavlink_heartbeat_t)mine.DebugPacket(hbpacket);

                        mine.logreadmode = false;
                        mine.logplaybackfile.Close();

                        string destdir = Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar
                            + mine.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                            + hbpacket[3] + Path.DirectorySeparatorChar;

                        if (!Directory.Exists(destdir))
                            Directory.CreateDirectory(destdir);

                        File.Move(logfile, destdir + Path.GetFileName(logfile));

                        try
                        {
                            File.Move(logfile.Replace(".tlog", ".rlog"), destdir + Path.GetFileName(logfile).Replace(".tlog", ".rlog"));
                        }
                        catch { }
                    }
                }
                catch (Exception ex) { continue; }
            }
        }
    }
}