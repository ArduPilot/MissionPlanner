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
                lblGPS.Image = MissionPlanner.Properties.Resources.Green_panel1;
                chBoxGPS.Checked = true;
                lblGPS.Text = "3D fix and 5 or more satellites connected";
            }
            else
            {
               // lblGPS.Image = MissionPlanner.Properties.Resources.Red_panel;
                chBoxGPS.Checked = false;
                lblGPS.Text = "3D fix or satellites failed or both";
            }

            if (MainV2.comPort.MAV.cs.linkqualitygcs >= 90)
            {
                lblTel.Image = MissionPlanner.Properties.Resources.Green_panel1;
                lblTel.Text = "signal >= 90%";
                chBoxTel.Checked = true;
            }
            else
            {
               // lblTel.Image = MissionPlanner.Properties.Resources.Red_panel;
                lblTel.Text = "signal < 90%";
                chBoxTel.Checked = false;
            }

            if (MainV2.comPort.MAV.cs.battery_voltage >= 15.99)
            {
                lblBattery.Text = "Voltage > 15.99";
                lblBattery.Image = MissionPlanner.Properties.Resources.Green_panel1;
                chBoxBattery.Checked = true;
            }
            else if (MainV2.comPort.MAV.cs.battery_voltage >= 14.5 && MainV2.comPort.MAV.cs.battery_voltage <= 15.98)
            {
                lblBattery.Text = "Voltage between 14.5 and 15.98";
                lblBattery.Image = MissionPlanner.Properties.Resources.Yellow_panel;
                chBoxBattery.Checked = true;
            }
            else
            {
                lblBattery.Text = "Voltage less than 14.5";
                lblBattery.Image = MissionPlanner.Properties.Resources.Red_panel;
                chBoxBattery.Checked = false;
            }

            //if (MainV2.comPort.MAV.cs.mode.Equals("FBWA", StringComparison.OrdinalIgnoreCase))
            //{
            //    lblRemote.Text = MainV2.comPort.MAV.cs.mode;
            //    lblRemote.Image = MissionPlanner.Properties.Resources.Green_panel1;
            //    chBoxRemote.Checked = true;
            //}
            //else
            //{
            //    lblRemote.Text = MainV2.comPort.MAV.cs.mode;
            //    lblRemote.Image = MissionPlanner.Properties.Resources.Red_panel;
            //    chBoxRemote.Checked = false;
            //}

            //if (MainV2.comPort.MAV.cs.alt <= 10)
            //{
            //    lblAltitude.Text = MainV2.comPort.MAV.cs.alt.ToString();
            //    lblAltitude.Image = MissionPlanner.Properties.Resources.Green_panel1;
            //    chBoxAltitude.Checked = true;
            //}
            //else
            //{
            //    lblAltitude.Text = MainV2.comPort.MAV.cs.alt.ToString();
            //    lblAltitude.Image = MissionPlanner.Properties.Resources.Red_panel;
            //    chBoxAltitude.Checked = false;
            //}

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

            SetColorAndTextOnState(lblWPCheck, CHK_WPCheck);

            SetColorAndTextOnState(lbl_CompassCheck, CHK_CompassCheck);
        }

        private void SetColorAndTextOnState(Label lbl, CheckBox chBox)
        {
            if (chBox.Checked)
            {
                lbl.Image = MissionPlanner.Properties.Resources.Green_panel1;
                lbl.Text = "Yes";
            }
            else
            {
                lbl.Image = MissionPlanner.Properties.Resources.Red_panel;
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

        private void CHK_WPCheck_CheckedChanged(object sender, EventArgs e)
        {
            BindUserCheckList();
        }
    }
}
