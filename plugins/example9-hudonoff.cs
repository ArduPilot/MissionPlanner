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


namespace hudonoff
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {

        public override string Name
        {
            get { return "HUD"; }
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

        private string configname = "HudItems";

        public override bool Loaded()
        {
            var rootbut = new ToolStripMenuItem("HUD Items");
            ToolStripItemCollection col = Host.FDMenuHud.Items;
            col.Add(rootbut);

            var items = new Dictionary<string,string>()
            {
                {"displayheading", "Heading"},
                {"displayspeed", "Speed"},
                {"displayalt", "Alt"},
                {"displayconninfo", "Connection"},
                {"displayxtrack", "X-Track"},
                {"displayrollpitch", "Roll/Pitch"},
                {"displaygps", "GPS"},
                {"batteryon", "Battery"},
                {"displayekf", "EKF"},
                {"displayvibe", "Vibe"},
                {"displayAOASSA", "AOA"},
            };

            var hide = Settings.Instance.GetList(configname);

            foreach (var item in items)
            {
                var but = new ToolStripMenuItem(item.Value);
                but.CheckOnClick = true;
                but.Checked = true;
                but.Click += (s,e)=>
                {
                    FlightData.myhud.GetType().GetProperty(item.Key).SetValue(FlightData.myhud, but.Checked);
                    if (!but.Checked)
                        Settings.Instance.AppendList(configname, item.Value);
                    if (but.Checked)
                        Settings.Instance.RemoveList(configname, item.Value);
                };
                if (hide.Contains(item.Value))
                {
                    but.Checked = false;
                    FlightData.myhud.GetType().GetProperty(item.Key).SetValue(FlightData.myhud, but.Checked);
                }
                rootbut.DropDownItems.Add(but);
            }

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