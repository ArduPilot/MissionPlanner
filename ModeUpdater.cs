using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

// Suriya made changes here

namespace MissionPlanner
{
    public class ModeUpdater
    {
        private System.Windows.Forms.Timer modeTimer;

        public void showMode(ToolStripMenuItem toolStripMenuItem)
        {
            if (modeTimer == null)
            {
                modeTimer = new System.Windows.Forms.Timer();
                modeTimer.Interval = 1;
                modeTimer.Tick += (sender, e) => findMode(sender, e, toolStripMenuItem);
                modeTimer.Start();
            }
            else if (!modeTimer.Enabled)
            {
                modeTimer.Start();
            }
        }

        private void findMode(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            if (MainV2.comPort != null && MainV2.comPort.MAV != null && MainV2.comPort.MAV.cs != null)
            {
                string modeMessage = MainV2.comPort.MAV.cs.mode;
                if (modeMessage != null)
                {
                    toolStripMenuItem.Text = modeMessage;
                }

                else
                {
                    toolStripMenuItem.Text = "Not connected";
                }
            }
            else
            {
                toolStripMenuItem.Text = "Not connected";
            }
        }

    }
}
