using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class PreFlightChecklist : UserControl
    {
        public PreFlightChecklist()
        {
            InitializeComponent();
            BindData();
        }

        public void BindData()
        {
            //Need to add one more condition
            if (MainV2.comPort.MAV.cs.satcount >= 5 && MainV2.comPort.MAV.cs.gpsstatus >= 3)
            {
                lblGPS.BackColor = Color.Green;
                chBoxGPS.Checked = true;
                lblGPS.Text = "3D fix and 5 or more satellites connected";
            }
            else
            {
                lblGPS.BackColor = Color.Red;
                chBoxGPS.Checked = false;
                lblGPS.Text = "3D fix or satellites failed or both";
            }

            if (MainV2.comPort.MAV.cs.linkqualitygcs >= 95)
            {
                lblTel.BackColor = Color.Green;
                lblTel.Text = "signal >= 95%";
                chBoxTel.Checked = true;
            }
            else
            {
                lblTel.BackColor = Color.Red;
                lblTel.Text = "signal < 95%";
                chBoxTel.Checked = false;
            }

            int cells = (int)(MainV2.comPort.MAV.cs.battery_voltage / 4.22) + 1;

            if (MainV2.comPort.MAV.cs.battery_voltage >= (cells * 4))
            {
                lblBattery.Text = "Voltage > " + (cells * 4).ToString("0.00");
                lblBattery.BackColor = Color.Green;
                chBoxBattery.Checked = true;
            }
            else if (MainV2.comPort.MAV.cs.battery_voltage >= (cells * 3.6) &&
                     MainV2.comPort.MAV.cs.battery_voltage < (cells * 4))
            {
                lblBattery.Text = "Voltage between " + (cells * 3.6).ToString("0.00") + " and " +
                                  (cells * 4).ToString("0.00");
                lblBattery.BackColor = Color.Yellow;
                chBoxBattery.Checked = true;
            }
            else
            {
                lblBattery.Text = "Voltage less than " + (cells * 3.6).ToString("0.00");
                lblBattery.BackColor = Color.Red;
                chBoxBattery.Checked = false;
            }

            if (MainV2.comPort.MAV.cs.mode.Equals("FBWA", StringComparison.OrdinalIgnoreCase) ||
                MainV2.comPort.MAV.cs.mode.Equals("stabalize", StringComparison.OrdinalIgnoreCase) ||
                MainV2.comPort.MAV.cs.mode.Equals("manual", StringComparison.OrdinalIgnoreCase))
            {
                lblRemote.Text = MainV2.comPort.MAV.cs.mode;
                lblRemote.BackColor = Color.Green;
                chBoxRemote.Checked = true;
            }
            else
            {
                lblRemote.Text = MainV2.comPort.MAV.cs.mode;
                lblRemote.BackColor = Color.Red;
                chBoxRemote.Checked = false;
            }

            if (MainV2.comPort.MAV.cs.alt <= 10)
            {
                lblAltitude.Text = MainV2.comPort.MAV.cs.alt.ToString();
                lblAltitude.BackColor = Color.Green;
                chBoxAltitude.Checked = true;
            }
            else
            {
                lblAltitude.Text = MainV2.comPort.MAV.cs.alt.ToString();
                lblAltitude.BackColor = Color.Red;
                chBoxAltitude.Checked = false;
            }

            BindUserCheckList();
        }

        private void BindUserCheckList()
        {
            //if (chBoxURCIn.Checked)
            //    lblURcStick.BackColor = Color.Green;
            //else
            //    lblURcStick.BackColor = Color.Red;
            SetColorAndTextOnState(lblURcStick, chBoxURCIn);

            //if (chBoxUTilt.Checked)
            //    lblUtilt.BackColor = Color.Green;
            //else
            //    lblUtilt.BackColor = Color.Red;
            SetColorAndTextOnState(lblUtilt, chBoxUTilt);

            //if (chBoxUCent.Checked)
            //    lblUCent.BackColor = Color.Green;
            //else
            //    lblUCent.BackColor = Color.Red;
            SetColorAndTextOnState(lblUCent, chBoxUCent);

            //if (chBoxUCam.Checked)
            //    lblUCam.BackColor = Color.Green;
            //else
            //    lblUCam.BackColor = Color.Red;
            SetColorAndTextOnState(lblUCam, chBoxUCam);

            //if (chBoxUServo.Checked)
            //    lblUServo.BackColor = Color.Green;
            //else
            //    lblUServo.BackColor = Color.Red;
            SetColorAndTextOnState(lblUServo, chBoxUServo);

            //if (chBoxUWing.Checked)
            //    lblUWing.BackColor = Color.Green;
            //else
            //    lblUWing.BackColor = Color.Red;
            SetColorAndTextOnState(lblUWing, chBoxUWing);
        }

        private void SetColorAndTextOnState(Label lbl, CheckBox chBox)
        {
            if (chBox.Checked)
            {
                lbl.BackColor = Color.Green;
                lbl.Text = "Yes";
            }
            else
            {
                lbl.BackColor = Color.Red;
                lbl.Text = "No";
            }
        }

        private void chBoxURCIn_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxUTilt_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxUCent_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxUCam_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxUServo_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxUWing_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }

        private void chBoxURub_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }
    }
}