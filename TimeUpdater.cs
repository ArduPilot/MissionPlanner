using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Suriya made changes here

namespace MissionPlanner
{

    public class TimeUpdater
    {
        private System.Windows.Forms.Timer timeUpdateTimer;

        public void StartTimer(ToolStripMenuItem toolStripMenuItem)
        {
            if (timeUpdateTimer == null)
            {
                timeUpdateTimer = new System.Windows.Forms.Timer();
                timeUpdateTimer.Interval = 1000; // Update every 1 second
                timeUpdateTimer.Tick += (sender, e) => TimeUpdateTimer_Tick(sender, e, toolStripMenuItem);
                timeUpdateTimer.Start();
            }
            else if (!timeUpdateTimer.Enabled)
            {
                timeUpdateTimer.Start();
            }
        }

        private void TimeUpdateTimer_Tick(object sender, EventArgs e, ToolStripMenuItem toolStripMenuItem)
        {
            toolStripMenuItem.Text = "Time: " + "\n" + DateTime.Now.ToString("hh:mm:ss"); // Update menu text
        }
    }

}
