using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class ServoOptions : UserControl
    {
        // start at 5 increment each instance
        static int servo = 5;

        public int thisservo { get; set; }

        public ServoOptions()
        {
            InitializeComponent();

            thisservo = servo;

            TXT_rcchannel.Text = thisservo.ToString();

            loadSettings();

            servo++;

            TXT_rcchannel.BackColor = Color.Gray;
        }

        void loadSettings()
        {
            string desc = Settings.Instance["Servo" + thisservo + "_desc"];
            string low = Settings.Instance["Servo" + thisservo + "_low"];
            string high = Settings.Instance["Servo" + thisservo + "_high"];

            string highdesc = Settings.Instance["Servo" + thisservo + "_highdesc"];
            string lowdesc = Settings.Instance["Servo" + thisservo + "_lowdesc"];

            if (!string.IsNullOrEmpty(low))
            {
                TXT_pwm_low.Text = low;
            }

            if (!string.IsNullOrEmpty(high))
            {
                TXT_pwm_high.Text = high;
            }

            if (!string.IsNullOrEmpty(desc))
            {
                TXT_rcchannel.Text = desc;
            }

            if (!string.IsNullOrEmpty(highdesc))
            {
                BUT_High.Text = highdesc;
            }

            if (!string.IsNullOrEmpty(lowdesc))
            {
                BUT_Low.Text = lowdesc;
            }
        }

        private void BUT_Low_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, thisservo, int.Parse(TXT_pwm_low.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void BUT_High_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, thisservo, int.Parse(TXT_pwm_high.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }
                else
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
        }

        private void BUT_Repeat_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, thisservo, int.Parse(TXT_pwm_low.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, thisservo, int.Parse(TXT_pwm_high.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, thisservo, int.Parse(TXT_pwm_low.Text), 0, 0,
                    0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.CommandFailed + ex.ToString(), Strings.ERROR);
            }
            // MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_high.Text), 10, 1000, 0, 0, 0);         
        }

        private void TXT_pwm_low_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["Servo" + thisservo + "_low"] = TXT_pwm_low.Text;
        }

        private void TXT_pwm_high_TextChanged(object sender, EventArgs e)
        {
            Settings.Instance["Servo" + thisservo + "_high"] = TXT_pwm_high.Text;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Control sourcectl = ((ContextMenuStrip) renameToolStripMenuItem.Owner).SourceControl;

            string desc = sourcectl.Text;
            MissionPlanner.Controls.InputBox.Show("Description", "Enter new Description", ref desc);
            sourcectl.Text = desc;

            if (sourcectl == BUT_High)
            {
                Settings.Instance["Servo" + thisservo + "_highdesc"] = desc;
            }
            else if (sourcectl == BUT_Low)
            {
                Settings.Instance["Servo" + thisservo + "_lowdesc"] = desc;
            }
            else if (sourcectl == TXT_rcchannel)
            {
                Settings.Instance["Servo" + thisservo + "_desc"] = desc;
            }
        }
    }
}