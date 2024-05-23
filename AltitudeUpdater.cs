
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Suriya made changes here

namespace MissionPlanner
{
    public class AltitudeUpdater
    {

        private System.Windows.Forms.Timer altitudeUpdateTimer;

        public void showAltitude(ToolStripMenuItem toolStripMenuItem)
        {

            if (altitudeUpdateTimer == null)
            {

                // Create and start the timer for dynamic updates
                altitudeUpdateTimer = new System.Windows.Forms.Timer();
                altitudeUpdateTimer.Interval = 1; // Update every 0.1 seconds (adjust as needed)
                altitudeUpdateTimer.Tick += (sender, e) => altitudeUpdateTimer_Tick(sender, e, toolStripMenuItem);
                altitudeUpdateTimer.Start();
            }

            else if (!altitudeUpdateTimer.Enabled)
            {
                altitudeUpdateTimer.Start();
            }

        }


        private void altitudeUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                float altitude = MainV2.comPort.MAV.cs.alt;
                if (altitude == null)
                {
                    toolStripMenuItem.Text = "Alt: 0.00 m";
                }

                else
                {
                    toolStripMenuItem.Text = "Alt: " + "\n" + altitude.ToString("0.00") + "m";
                }

            }
            else
            {
                // Handle case where data is unavailable
                toolStripMenuItem.Text = "Alt: 0.00 m";
            }
        }


    }
}
*/
/*

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Suriya made changes here

namespace MissionPlanner
{
    public class AltitudeUpdater
    {

        private System.Windows.Forms.Timer altitudeUpdateTimer;

        public void showAltitude(ToolStripMenuItem toolStripMenuItem)
        {

            if (altitudeUpdateTimer == null)
            {

                // Create and start the timer for dynamic updates
                altitudeUpdateTimer = new System.Windows.Forms.Timer();
                altitudeUpdateTimer.Interval = 1; // Update every 0.1 seconds (adjust as needed)
                altitudeUpdateTimer.Tick += (sender, e) => altitudeUpdateTimer_Tick(sender, e, toolStripMenuItem);
                altitudeUpdateTimer.Start();
            }

            else if (!altitudeUpdateTimer.Enabled)
            {
                altitudeUpdateTimer.Start();
            }

        }

        internal void showAltitude(ToolStripButton menuConnect)
        {
            throw new NotImplementedException();
        }

        private void altitudeUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                float altitude = MainV2.comPort.MAV.cs.alt;

                if (altitude != null)
                {
                    // Set a fixed width for the label and align the text to center
                    toolStripMenuItem.AutoSize = false;
                    toolStripMenuItem.Width = 80; // Adjust the width as needed
                                                  // toolStripMenuItem.TextAlign = ContentAlignment.MiddleCenter;

                    // Format the text with a fixed number of decimal places
                    toolStripMenuItem.Text = "Alt: " + "\n" + altitude.ToString("0.00") + " m";
                }
                else
                {
                    // Reset label properties and display default text
                    toolStripMenuItem.AutoSize = true;
                    toolStripMenuItem.Text = "Alt: 0.00 m";
                }
            }
            else
            {
                // Reset label properties and display default text
                toolStripMenuItem.AutoSize = true;
                toolStripMenuItem.Text = "Alt: 0.00 m";
            }
        }

    }
}
*/