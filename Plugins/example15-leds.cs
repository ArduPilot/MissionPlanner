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
using System.Threading;

namespace leds
{
    public class Pluginleds : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "LED Control"; }
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
            var rootbut = new ToolStripMenuItem("LED");
            //rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            var but = new ToolStripMenuItem("Red");
            but.Click += (s, e) => {
                var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { 255, 0, 0 });
                MainV2.comPort.sendPacket(led, 1, 1);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Green");
            but.Click += (s, e) => {
                var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { 0, 255, 0 });
                MainV2.comPort.sendPacket(led, 1, 1);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Blue");
            but.Click += (s, e) => {
                var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { 0, 0, 255 });
                MainV2.comPort.sendPacket(led, 1, 1);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("White");
            but.Click += (s, e) => {
                var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { 255, 255, 255 });
                MainV2.comPort.sendPacket(led, 1, 1);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Black");
            but.Click += (s, e) => {
                var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { 0, 0, 0 });
                MainV2.comPort.sendPacket(led, 1, 1);
            };
            rootbut.DropDownItems.Add(but);

            but = new ToolStripMenuItem("Rainbow");
            but.Click += (s, e) => {
                for (float a = 0; a <= 1; a += 0.005f) {
                    var color = Rainbow(a);
                    var led = new MAVLink.mavlink_led_control_t(1, 1, 255, 0, 3, new byte[] { color.R, color.G, color.B });
                    MainV2.comPort.sendPacket(led, 1, 1);
                    Thread.Sleep(50);
                }
            };
            rootbut.DropDownItems.Add(but);


            return true;
        }


        public static Color Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 6);
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(255, 255, ascending, 0);
                case 1:
                    return Color.FromArgb(255, descending, 255, 0);
                case 2:
                    return Color.FromArgb(255, 0, 255, ascending);
                case 3:
                    return Color.FromArgb(255, 0, descending, 255);
                case 4:
                    return Color.FromArgb(255, ascending, 0, 255);
                default: // case 5:
                    return Color.FromArgb(255, 255, 0, descending);
            }
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