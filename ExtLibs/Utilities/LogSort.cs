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
                    var brdsernum = 0;
                    var aptype = MAVLink.MAV_TYPE.GENERIC;
                    var packetsseen = 0;

                    if (logfile.ToLower().EndsWith(".bin")|| logfile.ToLower().EndsWith(".log"))
                    {
                        var logBuffer = new DFLogBuffer(logfile);

                        //PARM, 68613507, SYSID_THISMAV, 1

                        var sysidlist = logBuffer.GetEnumeratorType("PARM").Where(a => a["Name"] == "SYSID_THISMAV");
                        var brdsernumlist = logBuffer.GetEnumeratorType("PARM").Where(a => a["Name"] == "BRD_SERIAL_NUM");
                        var msgs = logBuffer.GetEnumeratorType("MSG").Take(100);
                        try
                        {
                            sysid = int.Parse(sysidlist.First()["Value"].ToString());
                        }
                        catch { }
                        try
                        {
                            brdsernum = int.Parse(brdsernumlist.First()["Value"].ToString());
                        }
                        catch { }
                        try
                        {
                            if (msgs.Select(a => a["Message"].ToLower().Contains("copter")).Count() > 0)
                            {
                                aptype = MAVLink.MAV_TYPE.QUADROTOR;
                                var frame = msgs.Select(a => a["Message"].ToLower().Contains("frame:"));
                                if (frame.Count() > 0)
                                { 
                                    if(frame.First().ToString().Contains("hexa"))
                                        aptype = MAVLink.MAV_TYPE.HEXAROTOR;
                                    if (frame.First().ToString().Contains("octo"))
                                        aptype = MAVLink.MAV_TYPE.OCTOROTOR;

                                }
                            }
                            if (msgs.Select(a => a["Message"].ToLower().Contains("plane")).Count() > 0)
                            {
                                aptype = MAVLink.MAV_TYPE.FIXED_WING;
                            }
                            if (msgs.Select(a => a["Message"].ToLower().Contains("rover")).Count() > 0)
                            {
                                aptype = MAVLink.MAV_TYPE.GROUND_ROVER;
                            }
                        }
                        catch { }

                        //logBuffer.dflog

                        if (logBuffer.SeenMessageTypes.Contains("SIM"))
                        {
                            logBuffer.Dispose();

                            var destdir = masterdestdir + Path.DirectorySeparatorChar
                                                        + "SITL" + Path.DirectorySeparatorChar
                                                        + aptype.ToString() + Path.DirectorySeparatorChar
                                                        + sysid + Path.DirectorySeparatorChar;
                            
                            // add on board serial number parameter if different than default value 0
                            if (brdsernum != 0)
                            {
                                destdir += brdsernum + Path.DirectorySeparatorChar;
                            }


                            if (!Directory.Exists(destdir))
                                Directory.CreateDirectory(destdir);

                            MoveFileUsingMask(logfile, destdir);

                            return;
                        }

                        if (sysid != 0 && aptype != MAVLink.MAV_TYPE.GENERIC)
                        {
                            logBuffer.Dispose();

                            var destdir = masterdestdir + Path.DirectorySeparatorChar
                                                        + aptype.ToString() + Path.DirectorySeparatorChar
                                                        + sysid + Path.DirectorySeparatorChar;

                            // add on board serial number parameter if different than default value 0
                            if (brdsernum != 0)
                            {
                                destdir += brdsernum + Path.DirectorySeparatorChar;
                            }


                            if (!Directory.Exists(destdir))
                                Directory.CreateDirectory(destdir);

                            MoveFileUsingMask(logfile, destdir);

                            return;
                        }

                        return;
                    }
                    else
                    {
                    }


                    var parse = new MAVLink.MavlinkParse(logfile.ToLower().EndsWith("tlog"));
                    using (var binfile = File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var midpoint = binfile.Length / 8;

                        binfile.Seek(0, SeekOrigin.Begin);

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

                                if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.SIMSTATE || packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HIL_CONTROLS)
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

                                    if (hblist.Count > 100)
                                        break;
                                }
                                else if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.PARAM_VALUE)
                                {
                                    packetsseen++;
                                    var parampkt = (MAVLink.mavlink_param_value_t)packet.data;
                                    if (System.Text.Encoding.ASCII.GetString(parampkt.param_id) == "BRD_SERIAL_NUM")
                                        brdsernum = (int)parampkt.param_value;
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

                        string destdir = masterdestdir + Path.DirectorySeparatorChar;

                        if (issitl)
                        {
                            destdir += "SITL" + Path.DirectorySeparatorChar;
                        }

                        destdir += aptype.ToString() + Path.DirectorySeparatorChar
                                   + sysid + Path.DirectorySeparatorChar;

                        if (brdsernum != 0)
                        {
                            Console.WriteLine("found a BRD_SERIAL_NUM");
                            destdir += brdsernum + Path.DirectorySeparatorChar;
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
                Console.Write(".");
                if (file == destdir + Path.GetFileName(file))
                    continue;

                log.Info("Move log " + file + " to " + destdir + Path.GetFileName(file));

                File.Move(file, destdir + Path.GetFileName(file));
            }
        }
    }
}