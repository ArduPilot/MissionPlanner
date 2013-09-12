using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.Wizard
{
    public partial class _11Verify : MyUserControl, IWizard, IDeactivate
    {
        int stage = 0;

        public _11Verify()
        {
            InitializeComponent();
         }

        public void Deactivate()
        {
            timer1.Stop();
        }

        public int WizardValidate()
        {
            return 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (stage)
            {
                case 0:// GPS
                    if (MainV2.comPort.MAV.cs.lat != 0 && MainV2.comPort.MAV.cs.gpsstatus > 0)
                    {
                        lbl_gps.BackColor = Color.Green;
                        stage = 1;
                    }
                    else
                    {
                        lbl_gps.BackColor = Color.Red;
                    }
                    break;
                case 1: // ACCEL
                    try
                    {
                        float val1 = MainV2.comPort.GetParam("INS_ACCOFFS_X");
                        float val2 = MainV2.comPort.GetParam("INS_ACCSCAL_X");

                        if (MainV2.comPort.GetParam("INS_ACCOFFS_X") != 0f && MainV2.comPort.GetParam("INS_ACCSCAL_X") != 1f)
                        {
                            lbl_accel.BackColor = Color.Green;
                            stage = 2;
                        }
                        else
                        {
                            lbl_accel.BackColor = Color.Red;
                        }
                    }
                    catch { }
                    break;
                case 2: // COMPASS
                    if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS_X") && MainV2.comPort.MAV.param["COMPASS_OFS_X"].ToString() != "0")
                    {
                        lbl_compass.BackColor = Color.Green;
                        stage = 3;
                    }
                    else
                    {
                        lbl_compass.BackColor = Color.Red;
                    }
                    break;
                case 3: // RC
                    if (MainV2.comPort.MAV.param.ContainsKey("RC1_MIN"))
                    {
                        if (((float)MainV2.comPort.MAV.param["RC1_MIN"]) < 1300 && ((float)MainV2.comPort.MAV.param["RC1_MAX"]) > 1700 &&
                            ((float)MainV2.comPort.MAV.param["RC2_MIN"]) < 1300 && ((float)MainV2.comPort.MAV.param["RC2_MAX"]) > 1700 &&
                            ((float)MainV2.comPort.MAV.param["RC3_MIN"]) < 1300 && ((float)MainV2.comPort.MAV.param["RC3_MAX"]) > 1700 &&
                            ((float)MainV2.comPort.MAV.param["RC4_MIN"]) < 1300 && ((float)MainV2.comPort.MAV.param["RC4_MAX"]) > 1700 &&
                            ((float)MainV2.comPort.MAV.param["RC3_MIN"]) != 1100 && ((float)MainV2.comPort.MAV.param["RC3_MAX"]) != 1900)
                        {
                            
                            lbl_rc.BackColor = Color.Green;
                            stage = 4;
                        }
                        else
                        {
                            lbl_rc.BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        lbl_rc.BackColor = Color.Red;
                    }
                    break;
                case 4: // PREARM
                    //MainV2.comPort.doARM(true);

                    if (MainV2.comPort.MAV.cs.armed)
                    {
                        lbl_prearm.BackColor = Color.Green;
                        MainV2.comPort.doARM(false);
                        stage = 5;
                    }
                    else
                    {
                        lbl_prearm.BackColor = Color.Red;
                    }

                    break;
                case 5: // DONE
                    timer1.Stop();
                    break;
            }
        }

        private void BUT_start_test_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
