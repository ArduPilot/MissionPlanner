using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;

namespace MissionPlanner.SimpleGrid
{
    public class GridPlugin : MissionPlanner.Plugin.Plugin
    {
        public static MissionPlanner.Plugin.PluginHost Host2;

        ToolStripMenuItem but;

        public override string Name
        {
            get { return "SimpleGrid"; }
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

            but = new ToolStripMenuItem("SimpleGrid");
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
            if (Host.FPDrawnPolygon != null && Host.FPDrawnPolygon.Points.Count > 2)
            {
                using (Form gridui = new GridUI(this))
                {
                    MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(gridui);
                    gridui.ShowDialog();
                }
            }
            else
            {
                CustomMessageBox.Show("Please define a polygon.", "Error");
            }
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
