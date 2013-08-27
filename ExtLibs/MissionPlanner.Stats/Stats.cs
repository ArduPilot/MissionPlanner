using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MissionPlanner.Stats
{
    public class Stats : ArdupilotMega.Plugin.Plugin
    {
        Dictionary<string, UInt64> stats = new Dictionary<string, ulong>();

        PointLatLngAlt lastpos;
        bool connectedstate = false;

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

        public override bool Init()
        {
            loopratehz = 1;
            return true;
        }

        public override bool Loaded()
        {
            stats["armed"] = 0;
            stats["distTraveled"] = 0;
            stats["gpslock"] = 0;
            stats["timeInAir"] = 0;
            stats["connects"] = 0;
            stats["connectedtime"] = 0;
            stats["maxalt"] = 0;
            stats["minalt"] = 0;
            return true;
        }

        public override bool Loop()
        {
            // connects
            if (connectedstate != Host.comPort.BaseStream.IsOpen)
            {
                if (Host.comPort.BaseStream.IsOpen == true)
                {
                    stats["connects"]++;
                    connectedstate = true;
                }
            }

            // if we are not connected, dont do anything
            if (!Host.comPort.BaseStream.IsOpen)
                return true;

            // armed time
            if (Host.cs.armed)
            {
                stats["armed"]++;
            }

            // distance traveled
            if (Host.cs.armed && Host.cs.gpsstatus == 3)
            {
                if (lastpos.Lat != 0 && lastpos.Lng != 0 && Host.cs.armed)
                {
                    stats["distTraveled"] += (ulong)lastpos.GetDistance(new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, ""));
                    lastpos = new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, "");
                }
                else
                {
                    lastpos = new PointLatLngAlt(Host.cs.lat, Host.cs.lng, Host.cs.altasl, "");
                }
            }

            // altitude gained
            if (Host.cs.armed && Host.cs.gpsstatus == 3)
            {
                stats["maxalt"] = (ulong)Host.cs.altasl;

                stats["minalt"] = (ulong)Host.cs.altasl;
            }

            // gps lock time
            if (Host.cs.gpsstatus == 3) {
                stats["gpslock"]++;
            }

            // time in air
            if (Host.cs.ch3percent > 12 || Host.cs.groundspeed > 3.0)
            {
                stats["timeInAir"]++;
            }

            // bytes received
             //stats["bytesreceived"] += Host.comPort.BytesReceived.Buffer(TimeSpan.FromSeconds(1)).Select(bytes => bytes.Sum());

            // bytes sent
              //stats["bytessent"] += Host.comPort.BytesSent.Buffer(TimeSpan.FromSeconds(1)).Select(bytes => bytes.Sum());

            // connect time
            if (Host.comPort.BaseStream.IsOpen)
            {
                stats["connectedtime"]++;
            }

            return true;
        }

        public override bool Exit()
        {
            // save stats
            return true;
        }
    }
}
