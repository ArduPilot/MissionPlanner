using System;
using System.Windows.Forms;
using System.Drawing;
using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace ElevationGraphShortcut
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        private ToolStripButton btnElevation;

        public override string Name
        {
            get { return "Elevation Graph Shortcut"; }
        }

        public override string Version
        {
            get { return "1.0"; }
        }

        public override string Author
        {
            get { return "Custom Plugin"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            // Create a new button for the main toolbar
            btnElevation = new ToolStripButton();
            btnElevation.Text = "Elevation Graph";
            btnElevation.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            btnElevation.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnElevation.Click += BtnElevation_Click;

            // Style the button to stand out
            btnElevation.ForeColor = Color.White;
            btnElevation.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnElevation.Margin = new Padding(4, 1, 0, 2);
            btnElevation.ToolTipText = "Open Elevation Profile for current mission";

            // Add the button to the main menu toolbar (top bar)
            var mainMenu = Host.MainForm.Controls.Find("MainMenu", true);
            if (mainMenu.Length > 0 && mainMenu[0] is ToolStrip toolStrip)
            {
                // Insert before the last items (connection controls)
                toolStrip.Items.Add(btnElevation);
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

        private void BtnElevation_Click(object sender, EventArgs e)
        {
            try
            {
                // Call the existing elevation graph method on FlightPlanner
                Host.MainForm.FlightPlanner.elevationGraphToolStripMenuItem_Click(sender, e);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    "Could not open Elevation Graph.\n" +
                    "Make sure you have waypoints loaded in the Flight Planner.\n\n" +
                    "Error: " + ex.Message,
                    "Elevation Graph",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
    }
}
