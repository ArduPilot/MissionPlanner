using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;

// Suriya made changes here

namespace MissionPlanner
{
    public class BatteryUpdater
    {

        private System.Windows.Forms.Timer batteryUpdateTimer;

        public void showBattery(ToolStripMenuItem toolStripMenuItem)
        {

            if (batteryUpdateTimer == null)
            {
                batteryUpdateTimer = new System.Windows.Forms.Timer();
                batteryUpdateTimer.Interval = 1; // Update every 0.5 seconds (adjust as needed)
                batteryUpdateTimer.Tick += (sender, e) => batteryUpdateTimer_Tick(sender, e, toolStripMenuItem);
                batteryUpdateTimer.Start();
            }
            else if (!batteryUpdateTimer.Enabled)
            {
                batteryUpdateTimer.Start();
            }

        }

        private void batteryUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {

                float battery = MainV2.comPort.MAV.cs.battery_remaining;
                if (battery != null)
                {

                    toolStripMenuItem.Text = "Battery: " + battery + "%";

                    if (battery > 20)
                    {
                        toolStripMenuItem.ForeColor = Color.Green; // Above 20% - Green
                    }
                    else if (battery <= 20 && battery > 10)
                    {
                        toolStripMenuItem.ForeColor = Color.Orange; // 20%-10% - Orange
                    }
                    else
                    {
                        toolStripMenuItem.ForeColor = Color.Red; // Below 10% - Red
                    }

                }
                else
                {
                    toolStripMenuItem.Text = "Battery not connected";
                }
            }
            else
            {
                toolStripMenuItem.Text = "Battery not connected";
            }
        }

    }
}
