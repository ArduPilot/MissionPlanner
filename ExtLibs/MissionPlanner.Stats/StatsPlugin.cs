using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace MissionPlanner.Stats
{
    public class StatsPlugin : MissionPlanner.Plugin.Plugin
    {
        whattostat statsoverall = new whattostat();
        whattostat statssession = new whattostat();

        string statsfile = Settings.GetUserDataDirectory() + "stats.xml";

        DateTime display = DateTime.MinValue;

        PointLatLngAlt lastpos;
        bool connectedstate = false;
        double lastmahused = 0;

        public override string Name
        {
            get { return "Stats"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        //[DebuggerHidden]
        public override bool Init()
        {
            loopratehz = 1;

            if (File.Exists(statsfile))
            {
                try
                {
                    System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(statsoverall.GetType());

                    var file = new System.IO.StreamReader(statsfile);

                    statsoverall = (whattostat)reader.Deserialize(file);

                    file.Close();
                }
                catch { }
            }

            MainV2.instance.Invoke((Action)
                delegate
                {

            System.Windows.Forms.ToolStripMenuItem men = new System.Windows.Forms.ToolStripMenuItem() { Text = "Stats" };
            men.Click += men_Click;
            Host.FDMenuMap.Items.Add(men);
            });

            statsoverall.appstarts++;

            return true;
        }

        void men_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog ofd = new FolderBrowserDialog();

            ofd.ShowDialog();

            if (Directory.Exists(ofd.SelectedPath))
            {
                string[] files = Directory.GetFiles(ofd.SelectedPath,"*.tlog");


            }
        }

        public override bool Loaded()
        {
            return true;
        }

        public class whattostat
        {
            public double armedtime = 0;
            public double distTraveledmeters = 0;
            public double gpslocktime = 0;
            public double timeInAir = 0;
            public double connects = 0;
            public double connectedtime = 0;
            public double maxalt = double.MinValue;
            public double minalt = double.MaxValue;
            public double mahused = 0;
            public DateTime lastconnect = DateTime.MinValue;
            public DateTime lastdisconnect = DateTime.MinValue;
            public double maxspeed = 0;
            public double avgspeed { get { return _avgspeed / avgspeedsamples; } set { _avgspeed += value; avgspeedsamples++; } }
            private double _avgspeed = 0;
            private double avgspeedsamples = 1;
            public double appstarts = 0;

            public override string ToString()
            {
                return String.Format(
@"armedtime {0} disttraveled {1} gpslocktime {2} timeinair {3}
connects {4} connectedtime {5} maxalt {6} minalt {7} mahused {8}
connecttime {9} disconnecttime {10} maxspeed {11} avgspeed {12}"
                , armedtime, distTraveledmeters, gpslocktime, timeInAir, connects, connectedtime, maxalt, minalt, mahused, lastconnect, lastdisconnect, maxspeed,avgspeed);
            }
        }

        public override bool Loop()
        {
            // loaded from file at app start
            dostats(statsoverall);

            // starts blank
            dostats(statssession);

            // display to console
            if (display.AddSeconds(5) < DateTime.Now)
            {
               // Console.WriteLine(statssession.ToString());

                display = DateTime.Now;
            }

            return true;
        }

        void dostats(whattostat stats) 
        {
            // connects
            if (connectedstate != Host.comPort.BaseStream.IsOpen)
            {
                if (Host.comPort.BaseStream.IsOpen == true)
                {
                    stats.lastconnect = DateTime.Now;
                    stats.connects++;
                    connectedstate = true;
                }
                else
                {
                    stats.lastdisconnect = DateTime.Now;
                    connectedstate = false;
                }
            }

            // if we are not connected, dont do anything
            if (!Host.comPort.BaseStream.IsOpen)
                return;
            
            // armed time
            if (Host.cs.armed)
            {
                stats.armedtime++;
            }

            // distance traveled
            if (Host.cs.armed && Host.cs.gpsstatus >= 3 && (Host.cs.ch3percent > 12 || Host.cs.groundspeed > 3.0))
            {
                stats.timeInAir++;

                if (lastpos != null && lastpos.Lat != 0 && lastpos.Lng != 0)
                {
                    double dist = lastpos.GetDistance(new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, ""));
                    // max jump size is 400 m
                    if (dist < 400)
                    {
                        stats.distTraveledmeters += dist;
                    }
                    lastpos = new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, "");
                }
                else
                {
                    lastpos = new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, "");
                }
            }

            // altitude gained
            if (Host.cs.armed && Host.cs.gpsstatus >= 3)
            {
                stats.maxalt = Math.Max(Host.cs.altasl,stats.maxalt);

                stats.minalt = Math.Min(Host.cs.altasl,stats.minalt);

                stats.maxspeed = Math.Max(Host.cs.groundspeed, stats.maxspeed);

                stats.avgspeed = Host.cs.groundspeed;
            }

            // gps lock time
            if (Host.cs.gpsstatus >= 3) {
                stats.gpslocktime++;
            }

            if (Host.cs.battery_usedmah > 0)
            {
                stats.mahused += Host.cs.battery_usedmah - lastmahused;
                lastmahused = Host.cs.battery_usedmah;
            }
            else
            {
                lastmahused = 0;
            }

            // bytes received
             //stats["bytesreceived"] += Host.comPort.BytesReceived.Buffer(TimeSpan.FromSeconds(1)).Select(bytes => bytes.Sum());

            // bytes sent
              //stats["bytessent"] += Host.comPort.BytesSent.Buffer(TimeSpan.FromSeconds(1)).Select(bytes => bytes.Sum());

            // connect time
            if (Host.comPort.BaseStream.IsOpen)
            {
                stats.connectedtime++;
            }
            return;
        }

        public override bool Exit()
        {
            // save stats
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(whattostat));

            var file = new System.IO.StreamWriter(statsfile);

            writer.Serialize(file, statsoverall);
            
            return true;
        }
    }
}
