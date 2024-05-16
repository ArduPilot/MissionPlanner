using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// Suriya made changes here

namespace MissionPlanner
{
    public class AirspeedUpdater
    {
        // creating a timer
        private System.Windows.Forms.Timer airspeedUpdateTimer;

        public void showSpeed(ToolStripMenuItem toolStripMenuItem)
        {

            if (airspeedUpdateTimer == null)
            {
                airspeedUpdateTimer = new System.Windows.Forms.Timer();
                airspeedUpdateTimer.Interval = 1; // Update every 1 second
                airspeedUpdateTimer.Tick += (sender, e) => airspeedUpdateTimer_Tick(sender, e, toolStripMenuItem);
                airspeedUpdateTimer.Start();
            }

            else if (!airspeedUpdateTimer.Enabled)
            {
                airspeedUpdateTimer.Start();
            }

            // Create and start the timer for dynamic updates

        }

        private void airspeedUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                float airspeed = MainV2.comPort.MAV.cs.airspeed;
                var speedUnit = MainV2.comPort.MAV.cs.GetNameandUnit("speed");
                var checkTime = MainV2.comPort.MAV.cs.datetime;
                if (airspeed != null)
                {
                    toolStripMenuItem.Text = "Speed: " + "\n" + airspeed.ToString("0.00") +" m/s";

                }
                else
                {
                    toolStripMenuItem.Text = "Speed: 0.00 m/s";

                }

            }
            else
            {
                toolStripMenuItem.Text = "Speed: 0.00 m/s";

            }
        }

    }
}
