using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class Plugin : ArdupilotMega.Plugin.Plugin
    {
        public static ArdupilotMega.Plugin.PluginHost Host2;

        public override string Name
        {
            get { return "Grid"; }
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
            return true;
        }

        public override bool Loaded()
        {
            Host2 = Host;

            ToolStripButton but = new ToolStripButton("Grid V2");
            but.Click += but_Click;

            Host.FPMenuMap.Items.Add(but);

            return true;
        }

        void but_Click(object sender, EventArgs e)
        {
            Form gridui = new GridUI();
            gridui.Show();
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
