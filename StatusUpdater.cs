/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


// Suriya made changes here

namespace MissionPlanner
{
    public class StatusUpdater
    {

        private System.Windows.Forms.Timer statusTimer;

        public void showStatus(ToolStripMenuItem toolStripMenuItem)
        {
            if (statusTimer == null)
            {
                statusTimer = new System.Windows.Forms.Timer();
                statusTimer.Interval = 1;
                statusTimer.Tick += (sender, e) => findstatus(sender, e, toolStripMenuItem);
                statusTimer.Start();
            }

            else if (!statusTimer.Enabled)
            {
                statusTimer.Start();
            }
        }

        private void findstatus(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {


            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {

                string statusMessage = MainV2.comPort.MAV.cs.message;
                var checkTime = MainV2.comPort.MAV.cs.battery_kmleft;


                if (string.IsNullOrWhiteSpace(statusMessage))
                {
                    toolStripMenuItem.Text = "Disconnected";
                    toolStripMenuItem.ForeColor = Color.Red;
                   // toolStripMenuItem.Image = MissionPlanner.Properties.Resources.dark_connect_icon;

                }

                else
                {

                    if (statusMessage != null)
                    {
                        string pattern1 = @"^GMs\s\d(\d{4})";
                        Match match1 = Regex.Match(statusMessage, pattern1);

                        if (match1.Success)
                        {

                            toolStripMenuItem.Text = "Connected";
                            toolStripMenuItem.ForeColor = Color.Green;
                            //toolStripMenuItem.Image = MissionPlanner.Properties.Resources.dark_disconnect_icon;

                        }
                        else
                        {
                            toolStripMenuItem.Text = "Disconnected";
                            toolStripMenuItem.ForeColor = Color.Red;
                           // toolStripMenuItem.Image = MissionPlanner.Properties.Resources.dark_connect_icon;
                        }
                    }
                    else
                    {
                        toolStripMenuItem.Text = "Disconnected";
                        toolStripMenuItem.ForeColor = Color.Red;
                       // toolStripMenuItem.Image = MissionPlanner.Properties.Resources.dark_connect_icon;
                    }

                }

            }

            else
            {
                toolStripMenuItem.Text = "Disconnected";
                toolStripMenuItem.ForeColor = Color.Red;
                //toolStripMenuItem.Image = MissionPlanner.Properties.Resources.dark_connect_icon;
            }

        }

    }
}*/






using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class StatusUpdater
    {
        private System.Windows.Forms.Timer statusTimer;
        private System.Windows.Forms.Timer checkTimer;
        private bool patternMatched;

        public void showStatus(ToolStripMenuItem toolStripMenuItem)
        {
            if (statusTimer == null)
            {
                statusTimer = new System.Windows.Forms.Timer();
                statusTimer.Interval = 1000; // Check every second
                statusTimer.Tick += (sender, e) => findstatus(sender, e, toolStripMenuItem);
                statusTimer.Start();
            }
            else if (!statusTimer.Enabled)
            {
                statusTimer.Start();
            }

            if (checkTimer == null)
            {
                checkTimer = new System.Windows.Forms.Timer();
                checkTimer.Interval = 5000; // 5 seconds interval
                checkTimer.Tick += (sender, e) => checkConnectionStatus(toolStripMenuItem);
                checkTimer.Start();
            }
            else if (!checkTimer.Enabled)
            {
                checkTimer.Start();
            }
        }

        private void findstatus(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                string statusMessage = MainV2.comPort.MAV.cs.message;

                if (string.IsNullOrWhiteSpace(statusMessage))
                {
                    patternMatched = false;
                }
                else
                {
                    string pattern1 = @"^GMs\s\d(\d{4})";
                    Match match1 = Regex.Match(statusMessage, pattern1);

                    if (match1.Success)
                    {
                        toolStripMenuItem.Text = "Connected";
                        toolStripMenuItem.ForeColor = Color.Green;
                        patternMatched = true;
                    }
                    else
                    {
                        patternMatched = false;
                    }
                }
            }
            else
            {
                patternMatched = false;
            }
        }

        private void checkConnectionStatus(ToolStripMenuItem toolStripMenuItem)
        {
            if (!patternMatched)
            {
                toolStripMenuItem.Text = "Disconnected";
                toolStripMenuItem.ForeColor = Color.Red;
            }
            patternMatched = false; // Reset for the next interval
        }
    }
}

 