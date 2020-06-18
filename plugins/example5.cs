using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace LatencyTracker
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        private List<MAVLink.mavlink_gps_raw_int_t> gpsraw = new List<MAVLink.mavlink_gps_raw_int_t>();
        private List<MAVLink.mavlink_system_time_t> systemtime = new List<MAVLink.mavlink_system_time_t>();

        private UInt32 last_boot_ms = 0;
        private Panel pnl;
        private Label lbl;
        private FileStream log;

        public override string Name
        {
            get { return "LatencyTracker"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            // to enable change this
            return false;
        }

        public override bool Loaded()
        {
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT, message =>
            {
                var gps = (MAVLink.mavlink_gps_raw_int_t) message.data;
                gpsraw.Add(gps);
                if (gpsraw.Count > 20)
                    gpsraw.RemoveAt(0);
                return true;
            });
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SYSTEM_TIME, message =>
            {
                var time = (MAVLink.mavlink_system_time_t)message.data;
                if (time.time_boot_ms == 0 || time.time_unix_usec == 0)
                    return true;
                systemtime.Add(time);
                last_boot_ms = time.time_boot_ms;
                if (systemtime.Count > 20)
                    systemtime.RemoveAt(0);
                return true;
            });
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, message =>
            {
                var pos = (MAVLink.mavlink_global_position_int_t) message.data;
                last_boot_ms = pos.time_boot_ms;
                return true;
            });
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.LOCAL_POSITION_NED, message =>
            {
                var pos = (MAVLink.mavlink_local_position_ned_t) message.data;
                last_boot_ms = pos.time_boot_ms;
                return true;
            });
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.TIMESYNC, message =>
            {
                var time = (MAVLink.mavlink_timesync_t) message.data;
                last_boot_ms = (uint) (time.ts1 / 1000.0 / 1000.0);
                return true;
            });

            pnl = new Panel();
            lbl = new Label();
            lbl.Dock = DockStyle.Fill;
            lbl.Font = new Font(lbl.Font.FontFamily, 16);
            pnl.Controls.Add(lbl);
            pnl.Location = new Point(350, 10);
            pnl.Size = new Size(60, 30);

            Host.MainForm.Controls.Add(pnl);
            Host.MainForm.Controls.SetChildIndex(pnl, 0);

            return true;
        }

        public override bool Loop()
        {
            this.loopratehz = 2;

            {
                if (systemtime.Count == 0)
                    return true;

                var avgoffset = systemtime.Average(a => (double) ((a.time_unix_usec/1000.0) - a.time_boot_ms));

                var newtime = ((last_boot_ms + avgoffset)/1000.0).fromUnixTime();

                var delta = (DateTime.UtcNow - newtime).TotalSeconds;

                if (MainV2.comPort.BaseStream != null && MainV2.comPort.BaseStream.IsOpen)
                {
                    if (log == null)
                    {
                        var dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                        log = File.OpenWrite(Settings.GetDefaultLogDir() + Path.DirectorySeparatorChar + dt +
                                             "-latency.csv");
                        var headerline = "TimeNow,PacketTime,Delta(s),Boot_MS,SysTimeOffset\r\n";
                        log.Write(ASCIIEncoding.ASCII.GetBytes(headerline), 0, headerline.Length);
                    }

                    var logline =
                        new string[]
                                {DateTime.UtcNow.ToString("O"), newtime.ToString("O"), delta.ToString(), last_boot_ms.ToString(), avgoffset.ToString()}
                            .Aggregate((a, b) =>
                                a + "," + b) + "\r\n";

                    log.Write(ASCIIEncoding.ASCII.GetBytes(logline), 0, logline.Length);

                    if (DateTime.Now.Second == 5 && log != null)
                        log.Flush();
                }
                else
                {
                    if(log != null)
                        log.Close();
                    log = null;
                }

                if (delta < 0.35)
                    pnl.BackColor = Color.Green;
                if (delta >= 0.35 && delta < 0.5)
                    pnl.BackColor = Color.Yellow;
                if (delta >= 0.5)
                    pnl.BackColor = Color.Red;
                
                CurrentState.custom_field_names["customfield9"] = "Latency";
                MainV2.comPort.MAV.cs.customfield9 = (float)delta;

                if (!MainV2.instance.Disposing && !MainV2.instance.IsDisposed)
                    MainV2.instance.BeginInvoke((Action) delegate()
                    {
                        lbl.ForeColor = Color.Black;
                        lbl.Text = delta.ToString("0.000");
                    });

                pnl.Invalidate();

            }
            return true;
        }

        public override bool Exit()
        {
            if (log != null)
                log.Close();
            return true;
        }
    }
}