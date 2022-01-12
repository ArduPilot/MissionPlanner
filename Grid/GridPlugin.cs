using System;
using System.Windows.Forms;

namespace MissionPlanner.Grid
{
    public class GridPlugin : MissionPlanner.Plugin.Plugin
    {


        ToolStripMenuItem but;

        public override string Name
        {
            get { return "Grid"; }
        }

        public override string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
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
            return true;
        }

        public void but_Click(object sender, EventArgs e)
        {
            using (var gridui = new GridUI(this))
            {
                MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(gridui);

                if (Host.FPDrawnPolygon != null && Host.FPDrawnPolygon.Points.Count > 2)
                {
                    gridui.ShowDialog();
                }
                else
                {
                    if (
                        CustomMessageBox.Show("No polygon defined. Load a file?", "Load File", MessageBoxButtons.YesNo) ==
                        (int)DialogResult.Yes)
                    {
                        gridui.LoadGrid();
                        gridui.ShowDialog();
                    }
                    else
                    {
                        CustomMessageBox.Show("Please define a polygon.", "Error");
                    }
                }
            }
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
