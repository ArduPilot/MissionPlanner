using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Controls
{
    public partial class ServoOptions : UserControl
    {
        // start at 5 increment each instance
        static int servo = 5;

        int thisservo = 0;

        public ServoOptions()
        {
            InitializeComponent();

            thisservo = servo;

            TXT_rcchannel.Text = thisservo.ToString();

            loadSettings();

            servo++;

            TXT_rcchannel.BackColor = Color.Silver;
        }

        void loadSettings()
        {
            string desc = MainV2.getConfig("Servo" + thisservo + "_desc");
            string low = MainV2.getConfig("Servo" + thisservo + "_low");
            string high = MainV2.getConfig("Servo" + thisservo + "_high");

            if (low != "")
            {
                TXT_pwm_low.Text = low;
            }

            if (high != "")
            {
                TXT_pwm_high.Text = high;
            }

            if (desc != "")
            {
                TXT_rcchannel.Text = desc;
            }
        }

        private void BUT_Low_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_low.Text), 0, 0, 0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
                else
                {
                    CustomMessageBox.Show("Command Failed");
                }
            }
            catch (Exception ex) { CustomMessageBox.Show("Command Failed " + ex.ToString()); }
        }

        private void BUT_High_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_high.Text), 0, 0, 0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }
                else
                {
                    CustomMessageBox.Show("Command Failed");
                }
            }
            catch (Exception ex) { CustomMessageBox.Show("Command Failed " + ex.ToString()); }
        }

        private void BUT_Repeat_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_low.Text), 0, 0, 0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_high.Text), 0, 0, 0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Green;
                }

                Application.DoEvents();
                System.Threading.Thread.Sleep(200);

                if (MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_low.Text), 0, 0, 0, 0, 0))
                {
                    TXT_rcchannel.BackColor = Color.Red;
                }
            }
            catch (Exception ex) { CustomMessageBox.Show("Command Failed "+ ex.ToString()); }
            // MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_SET_SERVO, int.Parse(TXT_rcchannel.Text), int.Parse(TXT_pwm_high.Text), 10, 1000, 0, 0, 0);         
        }

        private void TXT_pwm_low_TextChanged(object sender, EventArgs e)
        {
            MainV2.config["Servo" + thisservo + "_low"] = TXT_pwm_low.Text;
        }

        private void TXT_pwm_high_TextChanged(object sender, EventArgs e)
        {
            MainV2.config["Servo" + thisservo + "_high"] = TXT_pwm_high.Text;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string desc = TXT_rcchannel.Text;
            MissionPlanner.Controls.InputBox.Show("Description", "Enter new Description", ref desc);
            TXT_rcchannel.Text = desc;
            MainV2.config["Servo" + thisservo + "_desc"] = desc;
        }
    }
}
