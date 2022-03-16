using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using MissionPlanner.Plugin;

namespace MissionPlanner
{
    public class GridPluginv2 : MissionPlanner.Plugin.Plugin
    {
        ToolStripMenuItem but;

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

        public PluginHost Host2 { get; private set; }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            Host2 = Host;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridUIv2));
            var temp = (string)(resources.GetObject("$this.Text"));

            but = new ToolStripMenuItem(temp);
            but.Click += but_Click;

            bool hit = false;
            ToolStripItemCollection col = Host.FPMenuMap.Items;
            int index = col.Count;
            foreach (ToolStripItem item in col)
            {
                if (item.Text.Equals(Strings.AutoWP))
                {
                    index = col.IndexOf(item);
                    ((ToolStripMenuItem)item).DropDownItems.Add(but);
                    hit = true;
                    break;
                }
            }

            if (hit == false)
                col.Add(but);

            return true;
        }

        void but_Click(object sender, EventArgs e)
        {
            using (var gridui = new GridUIv2(this))
            {
                MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(gridui);

                gridui.ShowDialog();
            }
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
