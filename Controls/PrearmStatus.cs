using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class PrearmStatus : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private DateTime lastRequestTime = DateTime.MaxValue;
        private DateTime searchTime = DateTime.MaxValue;
        public PrearmStatus()
        {
            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            updatetexttimer.Start();
            requestchecktimer.Start();
            requestchecktimer_Tick(null, null);
        }

        private void requestchecktimer_Tick(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.prearmstatus) return; // Don't request prearm checks if we are ready to arm

            try
            {
                // Request prearm checks to be performed
                MainV2.comPort.doCommand(
                    (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                    MAVLink.MAV_CMD.RUN_PREARM_CHECKS,
                    0, 0, 0, 0, 0, 0, 0,
                    false // don't require ack
                );
                
                lastRequestTime = DateTime.Now;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
        }

        private void updatetexttimer_Tick(object sender, EventArgs e)
        {
            // If armed, close the form
            if(MainV2.comPort.MAV.cs.armed) this.Close();
            
            // If prearm prearm checks are passing, display a message
            if (MainV2.comPort.MAV.cs.prearmstatus)
            {
                TXT_PrearmErrors.Text = "Ready to Arm";
                return;
            }

            // If it has been at least 1 second since the last request, search for all messages since the last request
            if (DateTime.Now > lastRequestTime.AddSeconds(1))
                searchTime = lastRequestTime;

            // Fill text box with all unique messages since searchTime that start with "PreArm:"
            var prearmMessages = MainV2.comPort.MAV.cs.messages.Where(m => m.time > searchTime && m.message.ToLower().StartsWith("prearm:")).Select(m => m.message).Distinct();
            // If there are no messages, inform the user
            if (!prearmMessages.Any())
                prearmMessages = new[] { "Prearm checks failing", "Waiting for error messages..." };
            TXT_PrearmErrors.Text = string.Join(Environment.NewLine, prearmMessages);
        }
    }
}
