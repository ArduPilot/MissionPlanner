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

               
                //FlightData.AddMarker(); //creating the marker functuon
            }

            else if (!airspeedUpdateTimer.Enabled)
            {
                airspeedUpdateTimer.Start();
            }

            // Create and start the timer for dynamic updates

        }

        internal void showSpeed(ToolStripButton menuConnect)
        {
            throw new NotImplementedException();
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
                    // Set a fixed width for the label and align the text to center
                    toolStripMenuItem.AutoSize = false;
                    toolStripMenuItem.Width = 80; // Adjust the width as needed
                    toolStripMenuItem.TextAlign = ContentAlignment.MiddleCenter;

                    // Format the text with a fixed number of decimal places
                    toolStripMenuItem.Text = "Speed: " + "\n" + airspeed.ToString("0.00") + " Km/h";
                }
                else
                {
                    // Reset label properties and display default text
                    toolStripMenuItem.AutoSize = true;
                    toolStripMenuItem.Text = "Speed: 0.00 Km/h";
                }
               
            }
            else
            {
                // Reset label properties and display default text
                toolStripMenuItem.AutoSize = true;
                toolStripMenuItem.Text = "Speed: 0.00 Km/h";
            }
        }
    }
}