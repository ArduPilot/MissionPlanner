using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Suriya made changes here

namespace MissionPlanner
{
    public class GPSUpdater
    {
        private System.Windows.Forms.Timer gpsUpdateTimer;

        public void showGPS(ToolStripMenuItem toolStripMenuItem)
        {

            if (gpsUpdateTimer == null)
            {
                gpsUpdateTimer = new System.Windows.Forms.Timer();
                gpsUpdateTimer.Interval = 1; // Update every 0.5 seconds (adjust as needed)
                gpsUpdateTimer.Tick += (sender, e) => gpsUpdateTimer_Tick(sender, e, toolStripMenuItem);
                gpsUpdateTimer.Start();
            }
            else if (!gpsUpdateTimer.Enabled)
            {
                gpsUpdateTimer.Start();
            }


        }

        private void gpsUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                float gpsStatus = MainV2.comPort.MAV.cs.gpsstatus;
                if (gpsStatus != null)
                {
                    toolStripMenuItem.Text = "GPS: " + "\n" + gpsStatus.ToString("0.00");
                }

                else
                {
                    toolStripMenuItem.Text = "GPS not connected";
                }

            }
            else
            {
                // Handle case where data is unavailable
                toolStripMenuItem.Text = "GPS not connected";
            }
        }


    }
}
