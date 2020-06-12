using MissionPlanner.Grid;
using System;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class FaceMapPlugin : MissionPlanner.Plugin.Plugin
    {


        ToolStripMenuItem but;

        public override string Name
        {
            get { return "Face Map"; }
        }

        public override string Version
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public override string Author
        {
            get { return "Jonathan Wang"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridUI));

            but = new ToolStripMenuItem(Name);
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

        public void but_Click(object sender, EventArgs e)
        {
            using (var gridui = new FaceMapUI(this))
            {
                MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(gridui);

                if ((GCSViews.FlightPlanner.altmode)Host.MainForm.FlightPlanner.CMB_altmode.SelectedValue == GCSViews.FlightPlanner.altmode.Terrain)
                {
                    CustomMessageBox.Show("Terrain following altitude mode cannot be used with the face mapping tool.", "Error");
                }
                else if (Host.FPDrawnPolygon != null && Host.FPDrawnPolygon.Points.Count > 2)
                {
                    gridui.ShowDialog();
                }
                else
                {
                    if (
                        CustomMessageBox.Show("No polygon defined. Load a file?", "Load File", MessageBoxButtons.YesNo) ==
                        (int)DialogResult.Yes)
                    {
                        gridui.LoadFaceMap();
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
