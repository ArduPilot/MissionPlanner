using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Controls;


namespace MapIconDesc
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "MapIconDesc"; }
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
            var but = new ToolStripMenuItem("Change icon Description");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(but);

            return true;
        }

        private void but_Click(object sender, EventArgs e)
        {
            var descstring =
                "{alt}{altunit} {airspeed}{speedunit} id:{sysid} Sats:{satcount} HDOP:{gpshdop} Volts:{battery_voltage}";

            if (Settings.Instance["mapicondesc"] != null && Settings.Instance["mapicondesc"] != "")
                    descstring = Settings.Instance["mapicondesc"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Description", "What do you want it to show?", ref descstring))
                    return;

            //ArduPilot.Common.speechConversion(comPort.MAV, "" + Settings.Instance["mapicondesc"])

            Settings.Instance["mapicondesc"] = descstring;
        }

        public override bool Loaded()
        {
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