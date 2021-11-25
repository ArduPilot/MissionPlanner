using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using GMap.NET.WindowsForms;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using MissionPlanner.Comms;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;

namespace mass
{
    public class Pluginmass : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "mass Control"; }
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
            return true;
        }

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("Mass");
            //rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            var but = new ToolStripMenuItem("Arm");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav =>
                {
                    try
                    {
                        mav.doARM(true);
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Arm(Force)");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.doARM(true, true);
                    } catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("GUIDED");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.setMode("GUIDED");
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("AUTO");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.setMode("AUTO");
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("TAKEOFF");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.setMode("TAKEOFF");
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("TakeOff (2m)");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.doCommand((byte)mav.sysidcurrent, (byte)mav.compidcurrent,
                            MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 2);
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Fly To");
            but.Click += (s, e) => {
                Parallel.ForEach(MainV2.Comports, mav => {
                    try
                    {
                        mav.setGuidedModeWP(new Locationwp
                        {
                            alt = 50,
                            lat = Host.FDMenuMapPosition.Lat / 1e7,
                            lng = Host.FDMenuMapPosition.Lng / 1e7
                        });
                    }
                    catch { }
                });
            };
            rootbut.DropDownItems.Add(but);


            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}