using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Utilities;
using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigFlightModes : UserControl, IActivate, IDeactivate
    {
        Timer timer = new Timer();

        public ConfigFlightModes()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.S))
            {
                BUT_SaveModes_Click(null, null);
                return true;
            }

            return false;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource);
            }
            catch { }

            float pwm = 0;

            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx) // APM 
            {
                if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE_CH") || MainV2.comPort.MAV.param.ContainsKey("MODE_CH"))
                {
                    int sw = 0;
                    if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE_CH"))
                    {
                        sw = (int)(float)MainV2.comPort.MAV.param["FLTMODE_CH"];
                    }
                    else
                    {
                        sw = (int)(float)MainV2.comPort.MAV.param["MODE_CH"];
                    }

                    switch (sw)
                    {
                        case 5:
                            pwm = MainV2.comPort.MAV.cs.ch5in;
                            break;
                        case 6:
                            pwm = MainV2.comPort.MAV.cs.ch6in;
                            break;
                        case 7:
                            pwm = MainV2.comPort.MAV.cs.ch7in;
                            break;
                        case 8:
                            pwm = MainV2.comPort.MAV.cs.ch8in;
                            break;
                        default:

                            break;
                    }

                    if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE_CH"))
                    {
                        LBL_flightmodepwm.Text = MainV2.comPort.MAV.param["FLTMODE_CH"].ToString() + ": " + pwm.ToString();
                    }
                    else
                    {
                        LBL_flightmodepwm.Text = MainV2.comPort.MAV.param["MODE_CH"].ToString() + ": " + pwm.ToString();
                    }
                }
            }

            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) // ac2
            {
                pwm = MainV2.comPort.MAV.cs.ch5in;
                LBL_flightmodepwm.Text = "5: " + MainV2.comPort.MAV.cs.ch5in.ToString();
            }

            Control[] fmodelist = new Control[] { CMB_fmode1, CMB_fmode2, CMB_fmode3, CMB_fmode4, CMB_fmode5, CMB_fmode6 };

            foreach (Control ctl in fmodelist)
            {
                ThemeManager.ApplyThemeTo(ctl);
            }

            byte no = readSwitch(pwm);

            fmodelist[no].BackColor = Color.Green;
        }

        // from arducopter code
        byte readSwitch(float inpwm)
        {
            int pulsewidth = (int)inpwm;			// default for Arducopter

            if (pulsewidth > 1230 && pulsewidth <= 1360) return 1;
            if (pulsewidth > 1360 && pulsewidth <= 1490) return 2;
            if (pulsewidth > 1490 && pulsewidth <= 1620) return 3;
            if (pulsewidth > 1620 && pulsewidth <= 1749) return 4;	// Software Manual
            if (pulsewidth >= 1750) return 5;	// Hardware Manual
            return 0;
        }

        private void BUT_SaveModes_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.param.ContainsKey("FLTMODE1"))
                {
                    MainV2.comPort.setParam("FLTMODE1", (float)Int32.Parse(CMB_fmode1.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE2", (float)Int32.Parse(CMB_fmode2.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE3", (float)Int32.Parse(CMB_fmode3.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE4", (float)Int32.Parse(CMB_fmode4.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE5", (float)Int32.Parse(CMB_fmode5.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE6", (float)Int32.Parse(CMB_fmode6.SelectedValue.ToString()));
                }
                else if (MainV2.comPort.param.ContainsKey("MODE1"))
                {
                    MainV2.comPort.setParam("MODE1", (float)Int32.Parse(CMB_fmode1.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE2", (float)Int32.Parse(CMB_fmode2.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE3", (float)Int32.Parse(CMB_fmode3.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE4", (float)Int32.Parse(CMB_fmode4.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE5", (float)Int32.Parse(CMB_fmode5.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE6", (float)Int32.Parse(CMB_fmode6.SelectedValue.ToString()));
                }

                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) // ac2
                {
                    float value = (float)(CB_simple1.Checked ? (int)SimpleMode.Simple1 : 0) + (CB_simple2.Checked ? (int)SimpleMode.Simple2 : 0) + (CB_simple3.Checked ? (int)SimpleMode.Simple3 : 0)
                        + (CB_simple4.Checked ? (int)SimpleMode.Simple4 : 0) + (CB_simple5.Checked ? (int)SimpleMode.Simple5 : 0) + (CB_simple6.Checked ? (int)SimpleMode.Simple6 : 0);
                    if (MainV2.comPort.MAV.param.ContainsKey("SIMPLE"))
                        MainV2.comPort.setParam("SIMPLE", value);
                }
            }
            catch { CustomMessageBox.Show("Failed to set Flight modes"); }
            BUT_SaveModes.Text = "Complete";
        }

        [Flags]
        public enum SimpleMode
        {
            None = 0,
            Simple1 = 1,
            Simple2 = 2,
            Simple3 = 4,
            Simple4 = 8,
            Simple5 = 16,
            Simple6 = 32,
        }

        public void Deactivate()
        {
            timer.Stop();
        }

        public void Activate()
        {
            if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane || MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.Ateryx) // APM
            {
                CB_simple1.Visible = false;
                CB_simple2.Visible = false;
                CB_simple3.Visible = false;
                CB_simple4.Visible = false;
                CB_simple5.Visible = false;
                CB_simple6.Visible = false;


                try
                {
                    updateDropDown(CMB_fmode1, "FLTMODE1");
                    updateDropDown(CMB_fmode2, "FLTMODE2");
                    updateDropDown(CMB_fmode3, "FLTMODE3");
                    updateDropDown(CMB_fmode4, "FLTMODE4");
                    updateDropDown(CMB_fmode5, "FLTMODE5");
                    updateDropDown(CMB_fmode6, "FLTMODE6");

                    CMB_fmode1.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE1"].ToString());
                    CMB_fmode2.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE2"].ToString());
                    CMB_fmode3.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE3"].ToString());
                    CMB_fmode4.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE4"].ToString());
                    CMB_fmode5.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE5"].ToString());
                    CMB_fmode6.Text = "Manual";
                    CMB_fmode6.Enabled = false;
                }
                catch { }
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover) // APM
            {
                CB_simple1.Visible = false;
                CB_simple2.Visible = false;
                CB_simple3.Visible = false;
                CB_simple4.Visible = false;
                CB_simple5.Visible = false;
                CB_simple6.Visible = false;


                try
                {
                    updateDropDown(CMB_fmode1, "MODE1");
                    updateDropDown(CMB_fmode2, "MODE2");
                    updateDropDown(CMB_fmode3, "MODE3");
                    updateDropDown(CMB_fmode4, "MODE4");
                    updateDropDown(CMB_fmode5, "MODE5");
                    updateDropDown(CMB_fmode6, "MODE6");

                    CMB_fmode1.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE1"].ToString());
                    CMB_fmode2.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE2"].ToString());
                    CMB_fmode3.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE3"].ToString());
                    CMB_fmode4.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE4"].ToString());
                    CMB_fmode5.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE5"].ToString());
                    CMB_fmode6.Text = "Manual";
                    CMB_fmode6.Enabled = false;
                }
                catch { }
            }
            else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduCopter2) // ac2
            {
                try
                {
                    updateDropDown(CMB_fmode1, "FLTMODE1");
                    updateDropDown(CMB_fmode2, "FLTMODE2");
                    updateDropDown(CMB_fmode3, "FLTMODE3");
                    updateDropDown(CMB_fmode4, "FLTMODE4");
                    updateDropDown(CMB_fmode5, "FLTMODE5");
                    updateDropDown(CMB_fmode6, "FLTMODE6");

                    CMB_fmode1.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE1"].ToString());
                    CMB_fmode2.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE2"].ToString());
                    CMB_fmode3.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE3"].ToString());
                    CMB_fmode4.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE4"].ToString());
                    CMB_fmode5.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE5"].ToString());
                    CMB_fmode6.SelectedValue = int.Parse(MainV2.comPort.MAV.param["FLTMODE6"].ToString());
                    CMB_fmode6.Enabled = true;

                    int simple = int.Parse(MainV2.comPort.MAV.param["SIMPLE"].ToString());

                    CB_simple1.Checked = ((simple >> 0 & 1) == 1);
                    CB_simple2.Checked = ((simple >> 1 & 1) == 1);
                    CB_simple3.Checked = ((simple >> 2 & 1) == 1);
                    CB_simple4.Checked = ((simple >> 3 & 1) == 1);
                    CB_simple5.Checked = ((simple >> 4 & 1) == 1);
                    CB_simple6.Checked = ((simple >> 5 & 1) == 1);
                }
                catch { }
            }



            timer.Tick += new EventHandler(timer_Tick);

            timer.Enabled = true;
            timer.Interval = 100;
            timer.Start();
        }

        void updateDropDown(ComboBox ctl, string param)
        {
            ctl.DataSource = Common.getOptions(param).ToList();
            ctl.DisplayMember = "Value";
            ctl.ValueMember = "Key";
        }
    }
}