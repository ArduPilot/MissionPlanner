using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using Xamarin.Essentials;

namespace Carbonix
{
    public partial class EmergencyTab : UserControl
    {
        private readonly PluginHost Host;
        private readonly Dictionary<string, string> settings;

        private readonly Timer messageBoxTimer = new Timer();
        
        public EmergencyTab(PluginHost Host, Dictionary<string, string> settings)
        {
            this.Host = Host;
            this.settings = settings;

            InitializeComponent();

            messageBoxTimer.Tick += messageBoxTimer_Tick;
        }

        int last_msg_time;
        private void messageBoxTimer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                messageBoxTimer.Stop();
                return;
            }

            var messagetime = Host.comPort.MAV.cs.messages.LastOrDefault().time;
            if (last_msg_time != messagetime.toUnixTime())
            {
                try
                {
                    StringBuilder message = new StringBuilder();
                    Host.comPort.MAV.cs.messages.ForEach(x =>
                    {
                        message.Insert(0, x.time + " : " + x.message + "\r\n");
                    });
                    txt_messagebox.Text = message.ToString();

                    last_msg_time = messagetime.toUnixTime();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

        }

        private void EmergencyTab_VisibleChanged(object sender, EventArgs e)
        {
            messageBoxTimer.Enabled = true;
            messageBoxTimer.Interval = 200;
            messageBoxTimer.Start();
        }

        private void but_glide_Click(object sender, EventArgs e)
        {
            // Set TECS_SPDWEIGHT to 2
            Host.comPort.setParam((byte)Host.comPort.sysidcurrent, (byte)Host.comPort.compidcurrent, "TECS_SPDWEIGHT", 2);
        }

        private void but_noglide_Click(object sender, EventArgs e)
        {
            // Restore TECS_SPDWEIGHT
            double spdweight = 1.0;
            if (settings.ContainsKey("TECS_SPDWEIGHT"))
            {
                double.TryParse(settings["TECS_SPDWEIGHT"], NumberStyles.Any, CultureInfo.InvariantCulture, out spdweight);
            }
            Host.comPort.setParam((byte)Host.comPort.sysidcurrent, (byte)Host.comPort.compidcurrent, "TECS_SPDWEIGHT", spdweight);
        }

        private void but_mode_Click(object sender, EventArgs e)
        {
            byte sysid = (byte)Host.comPort.sysidcurrent;
            byte compid = (byte)Host.comPort.compidcurrent;
            try
            {
                // Disable the button while we send the message
                ((Control)sender).Enabled = false;
                // Command the mode change
                MainV2.comPort.setMode(sysid, compid, ((Control)sender).Text);
            }
            catch
            {
                // Alert failures
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }

            // Re-enable the button
            ((Control)sender).Enabled = true;
        }

        private void but_asdisable_Click(object sender, EventArgs e)
        {
            // Send aux function of 106:DISABLE_AIRSPEED_USE to high
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 106, 2, 0, 0, 0, 0, 0);
        }

        private void but_asenable_Click(object sender, EventArgs e)
        {
            // Send aux function of 106:DISABLE_AIRSPEED_USE to low
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 106, 0, 0, 0, 0, 0, 0);
        }

        private void but_qadisable_Click(object sender, EventArgs e)
        {
            // Send aux function of 82:Q_ASSIST to low
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 82, 0, 0, 0, 0, 0, 0);
        }

        private void but_qaenable_Click(object sender, EventArgs e)
        {
            // Send aux function of 82:Q_ASSIST to mid
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 82, 1, 0, 0, 0, 0, 0);
        }

        private void but_qaforce_Click(object sender, EventArgs e)
        {
            // Send aux function of 82:Q_ASSIST to high
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 82, 2, 0, 0, 0, 0, 0);
        }

        private void but_fencedisable_Click(object sender, EventArgs e)
        {
            // Send aux function of 11:FENCE to low
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 11, 0, 0, 0, 0, 0, 0);
        }

        private void but_fenceenable_Click(object sender, EventArgs e)
        {
            // Send aux function of 11:FENCE to high
            Host.comPort.doCommand(MAVLink.MAV_CMD.DO_AUX_FUNCTION, 11, 2, 0, 0, 0, 0, 0);
        }
    }
}
