using System;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _11Verify : MyUserControl, IWizard, IDeactivate, IActivate
    {
        public _11Verify()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        public int WizardValidate()
        {
            timer1.Stop();
            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // GPS - every pass
            if (MainV2.comPort.MAV.cs.lat != 0 && MainV2.comPort.MAV.cs.gpsstatus > 0)
            {
                lbl_gps.BackColor = Color.Green;
                chk_gps.Checked = true;
                //stage = 1;
            }
            else
            {
                lbl_gps.BackColor = Color.Red;
                chk_gps.Checked = false;
            }

            // ACCEL
            try
            {
                if (lbl_accel.BackColor != Color.Green)
                {
                    float val1 = (float) MainV2.comPort.GetParam("INS_ACCOFFS_X");
                    float val2 = (float) MainV2.comPort.GetParam("INS_ACCSCAL_X");

                    if (MainV2.comPort.GetParam("INS_ACCOFFS_X") != 0f && MainV2.comPort.GetParam("INS_ACCSCAL_X") != 1f)
                    {
                        lbl_accel.BackColor = Color.Green;
                        chk_accel.Checked = true;
                    }
                    else
                    {
                        lbl_accel.BackColor = Color.Red;
                        chk_accel.Checked = false;
                    }
                }
            }
            catch
            {
            }

            // COMPASS
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS_X") &&
                MainV2.comPort.MAV.param["COMPASS_OFS_X"].ToString() != "0")
            {
                lbl_compass.BackColor = Color.Green;
                chk_compass.Checked = true;
            }
            else
            {
                lbl_compass.BackColor = Color.Red;
                chk_compass.Checked = false;
            }

            // RC
            if (MainV2.comPort.MAV.param.ContainsKey("RC1_MIN"))
            {
                if (((float) MainV2.comPort.MAV.param["RC1_MIN"]) < 1300 &&
                    ((float) MainV2.comPort.MAV.param["RC1_MAX"]) > 1700 &&
                    ((float) MainV2.comPort.MAV.param["RC2_MIN"]) < 1300 &&
                    ((float) MainV2.comPort.MAV.param["RC2_MAX"]) > 1700 &&
                    ((float) MainV2.comPort.MAV.param["RC3_MIN"]) < 1300 &&
                    ((float) MainV2.comPort.MAV.param["RC3_MAX"]) > 1700 &&
                    ((float) MainV2.comPort.MAV.param["RC4_MIN"]) < 1300 &&
                    ((float) MainV2.comPort.MAV.param["RC4_MAX"]) > 1700 &&
                    ((float) MainV2.comPort.MAV.param["RC3_MIN"]) != 1100 &&
                    ((float) MainV2.comPort.MAV.param["RC3_MAX"]) != 1900)
                {
                    lbl_rc.BackColor = Color.Green;
                    chk_rc.Checked = true;
                }
                else
                {
                    lbl_rc.BackColor = Color.Red;
                    chk_rc.Checked = false;
                }
            }
            else
            {
                lbl_rc.BackColor = Color.Red;
                chk_rc.Checked = false;
            }

            // arm check
            if (MainV2.comPort.MAV.cs.armed)
            {
                lbl_prearm.BackColor = Color.Green;
                try
                {
                    MainV2.comPort.doARM(false);
                }
                catch
                {
                }
                chk_perarm.Checked = true;
            }
            else
            {
                if (!chk_perarm.Checked)
                    lbl_prearm.BackColor = Color.Red;
                //chk_perarm.Checked = false;
            }
        }

        private void BUT_start_test_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}