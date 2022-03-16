using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using Newtonsoft.Json;

namespace tracemp
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "tracemp"; }
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
            var but = new ToolStripMenuItem("tracemp");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            
            // uncomment to enable
            col.Add(but);

            return true;
        }

        private void but_Click(object sender, EventArgs e)
        {
            
            Program.TraceMe();
            
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