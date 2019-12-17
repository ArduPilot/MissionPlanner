using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Core.ExtendedObjects;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    public class LogSort
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void SortLogs(string[] logs, string masterdestdir = "")
        {
            Parallel.ForEach(logs, logfile =>
                //foreach (var logfile in logs)
            {
                if (masterdestdir == "")
                    masterdestdir = Path.GetDirectoryName(logfile);

                try
                {
                    FileInfo info;

           
                        info = new FileInfo(logfile);

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

                            return;
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

                            MoveFileUsingMask(logfile, destdir);
                        }
                        catch
                        {
                        }

                        return;
                    }
                    
                    bool issitl = false;
                    var sysid = 0;
                    var compid = 0;
                    var aptype = MAVLink.MAV_TYPE.GENERIC;
                    var packetsseen = 0;

                    if (logfile.ToLower().EndsWith(".bin")|| logfile.ToLower().EndsWith(".log"))
                    {
                        var logBuffer = new DFLogBuffer(File.OpenRead(logfile));

                        //PARM, 68613507, SYSID_THISMAV, 1

                        var sysidlist = logBuffer.GetEnumeratorType("PARM").Where(a => a["Name"] == "SYSID_THISMAV");

                        sysid = int.Parse(sysidlist.First()["Value"].ToString());

                        //logBuffer.dflog

                        if (logBuffer.SeenMessageTypes.Contains("SIM"))
                        {
                            logBuffer.Dispose();

                            var destdir = masterdestdir + Path.DirectorySeparatorChar
                                                        + "SITL" + Path.DirectorySeparatorChar
                                                        + aptype.ToString() + Path.DirectorySeparatorChar
                                                        + sysid + Path.DirectorySeparatorChar;


                            if (!Directory.Exists(destdir))
                                Directory.CreateDirectory(destdir);

                            MoveFileUsingMask(logfile, destdir);
                        }

                        return;
                    }
                    else
                    {
                    }


                    var parse = new MAVLink.MavlinkParse(true);
                    using (var binfile = File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var midpoint = binfile.Length / 8;

                        binfile.Seek(midpoint, SeekOrigin.Begin);

                        MAVLink.MAVLinkMessage packet;

                        List<MAVLink.MAVLinkMessage> hblist = new List<MAVLink.MAVLinkMessage>();

                        try
                        {
                            var length = binfile.Length;
                            while (binfile.Position < length)
                            {
                                packet = parse.ReadPacket(binfile);
                                // no gcs packets
                                if (packet == null || packet.compid == 190)
                                    continue;

                                if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.SIMSTATE)
                                {
                                    packetsseen++;
                                    issitl = true;
                                }
                                else if (packet.msgid == (uint) MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                                {
                                    packetsseen++;
                                    hblist.Add(packet);

                                    var hb = (MAVLink.mavlink_heartbeat_t) packet.data;
                                    sysid = packet.sysid;
                                    compid = packet.compid;
                                    aptype = (MAVLink.MAV_TYPE) hb.type;

                                    if (hblist.Count > 10)
                                        break;
                                }
                                else if (packet != MAVLink.MAVLinkMessage.Invalid)
                                {
                                    packetsseen++;
                                    if (sysid == 0)
                                        sysid = packet.sysid;
                                    if(compid == 0)
                                        compid = packet.compid;
                                }
                            }
                        } catch (EndOfStreamException) { }
                        catch { }

                        if (hblist.Count == 0)
                        {
                            binfile.Close();

                            if (!Directory.Exists(masterdestdir + Path.DirectorySeparatorChar + "BAD"))
                                Directory.CreateDirectory(masterdestdir + Path.DirectorySeparatorChar +
                                                          "BAD");

                            log.Info("Move log bad " + logfile + " to " + masterdestdir +
                                     Path.DirectorySeparatorChar + "BAD" + Path.DirectorySeparatorChar +
                                     Path.GetFileName(logfile) + " " + info.Length);

                            MoveFileUsingMask(logfile,
                                masterdestdir + Path.DirectorySeparatorChar + "BAD" +
                                Path.DirectorySeparatorChar);
                            return;
                        }

                        // find most appropriate
                        if (hblist.GroupBy(a => a.sysid * 256 + a.compid).ToArray().Length > 1)
                        {
                            foreach (var mav in hblist)
                            {
                                var hb = (MAVLink.mavlink_heartbeat_t) mav.data;
                                if (hb.type == (byte) MAVLink.MAV_TYPE.ANTENNA_TRACKER)
                                    continue;
                                if (hb.type == (byte) MAVLink.MAV_TYPE.GCS)
                                    continue;

                                sysid = mav.sysid;
                                compid = mav.compid;
                                aptype = (MAVLink.MAV_TYPE) hb.type;
                            }
                        }

                        binfile.Close();

                        string destdir = masterdestdir + Path.DirectorySeparatorChar
                                                       + aptype.ToString() + Path.DirectorySeparatorChar
                                                       + sysid + Path.DirectorySeparatorChar;

                        if (issitl)
                        {
                            destdir = masterdestdir + Path.DirectorySeparatorChar
                                                    + "SITL" + Path.DirectorySeparatorChar
                                                    + aptype.ToString() + Path.DirectorySeparatorChar
                                                    + sysid + Path.DirectorySeparatorChar;
                        }

                        if (!Directory.Exists(destdir))
                            Directory.CreateDirectory(destdir);

                        MoveFileUsingMask(logfile, destdir);
                    }
                }
                catch
                {
                    return;
                }
            });
        }

        static void MoveFileUsingMask(string logfile, string destdir)
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