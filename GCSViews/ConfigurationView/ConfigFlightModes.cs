using System;
using System.Diagnostics;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFlightModes : MyUserControl, IActivate, IDeactivate
    {
        [Flags]
        public enum SimpleMode
        {
            None = 0,
            Simple1 = 1,
            Simple2 = 2,
            Simple3 = 4,
            Simple4 = 8,
            Simple5 = 16,
            Simple6 = 32
        }

        private readonly Timer timer = new Timer();

        public ConfigFlightModes()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Activate()
        {
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx) // APM
            {
                CB_simple1.Visible = false;
                CB_simple2.Visible = false;
                CB_simple3.Visible = false;
                CB_simple4.Visible = false;
                CB_simple5.Visible = false;
                CB_simple6.Visible = false;

                chk_ss1.Visible = false;
                chk_ss2.Visible = false;
                chk_ss3.Visible = false;
                chk_ss4.Visible = false;
                chk_ss5.Visible = false;
                chk_ss6.Visible = false;

                linkLabel1_ss.Visible = false;

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
                }
                catch
                {
                }
            }
            else if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover) // APM
            {
                CB_simple1.Visible = false;
                CB_simple2.Visible = false;
                CB_simple3.Visible = false;
                CB_simple4.Visible = false;
                CB_simple5.Visible = false;
                CB_simple6.Visible = false;

                chk_ss1.Visible = false;
                chk_ss2.Visible = false;
                chk_ss3.Visible = false;
                chk_ss4.Visible = false;
                chk_ss5.Visible = false;
                chk_ss6.Visible = false;

                linkLabel1_ss.Visible = false;

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
                    CMB_fmode6.SelectedValue = int.Parse(MainV2.comPort.MAV.param["MODE6"].ToString());
                }
                catch
                {
                }
            }
            else if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2) // ac2
            {
                if (MainV2.DisplayConfiguration.standardFlightModesOnly)
                {
                    CB_simple1.Visible = false;
                    CB_simple2.Visible = false;
                    CB_simple3.Visible = false;
                    CB_simple4.Visible = false;
                    CB_simple5.Visible = false;
                    CB_simple6.Visible = false;

                    chk_ss1.Visible = false;
                    chk_ss2.Visible = false;
                    chk_ss3.Visible = false;
                    chk_ss4.Visible = false;
                    chk_ss5.Visible = false;
                    chk_ss6.Visible = false;

                    linkLabel1_ss.Visible = false;
                }
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

                    if (MainV2.comPort.MAV.param.ContainsKey("SIMPLE"))
                    {
                        var simple = int.Parse(MainV2.comPort.MAV.param["SIMPLE"].ToString());

                        CB_simple1.Checked = ((simple >> 0 & 1) == 1);
                        CB_simple2.Checked = ((simple >> 1 & 1) == 1);
                        CB_simple3.Checked = ((simple >> 2 & 1) == 1);
                        CB_simple4.Checked = ((simple >> 3 & 1) == 1);
                        CB_simple5.Checked = ((simple >> 4 & 1) == 1);
                        CB_simple6.Checked = ((simple >> 5 & 1) == 1);
                    }

                    if (MainV2.comPort.MAV.param.ContainsKey("SUPER_SIMPLE"))
                    {
                        var simple = int.Parse(MainV2.comPort.MAV.param["SUPER_SIMPLE"].ToString());

                        chk_ss1.Checked = ((simple >> 0 & 1) == 1);
                        chk_ss2.Checked = ((simple >> 1 & 1) == 1);
                        chk_ss3.Checked = ((simple >> 2 & 1) == 1);
                        chk_ss4.Checked = ((simple >> 3 & 1) == 1);
                        chk_ss5.Checked = ((simple >> 4 & 1) == 1);
                        chk_ss6.Checked = ((simple >> 5 & 1) == 1);
                    }
                }
                catch
                {
                }
            }
            else if (MainV2.comPort.MAV.cs.firmware == Firmwares.PX4) // APM
            {
                CB_simple1.Visible = false;
                CB_simple2.Visible = false;
                CB_simple3.Visible = false;
                CB_simple4.Visible = false;
                CB_simple5.Visible = false;
                CB_simple6.Visible = false;

                chk_ss1.Visible = false;
                chk_ss2.Visible = false;
                chk_ss3.Visible = false;
                chk_ss4.Visible = false;
                chk_ss5.Visible = false;
                chk_ss6.Visible = false;

                linkLabel1_ss.Visible = false;

                try
                {
                    updateDropDown(CMB_fmode1, "COM_FLTMODE1");
                    CMB_fmode1.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE1", "PX4");
                    updateDropDown(CMB_fmode2, "COM_FLTMODE2");
                    CMB_fmode2.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE2", "PX4");
                    updateDropDown(CMB_fmode3, "COM_FLTMODE3");
                    CMB_fmode3.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE3", "PX4");
                    updateDropDown(CMB_fmode4, "COM_FLTMODE4");
                    CMB_fmode4.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE4", "PX4");
                    updateDropDown(CMB_fmode5, "COM_FLTMODE5");
                    CMB_fmode5.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE5", "PX4");
                    updateDropDown(CMB_fmode6, "COM_FLTMODE6");
                    CMB_fmode6.DataSource = ParameterMetaDataRepository.GetParameterOptionsInt("COM_FLTMODE6", "PX4");

                    CMB_fmode1.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE1"].ToString());
                    CMB_fmode2.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE2"].ToString());
                    CMB_fmode3.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE3"].ToString());
                    CMB_fmode4.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE4"].ToString());
                    CMB_fmode5.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE5"].ToString());
                    CMB_fmode6.SelectedValue = int.Parse(MainV2.comPort.MAV.param["COM_FLTMODE6"].ToString());
                }
                catch
                {
                }
            }

            timer.Tick += timer_Tick;

            timer.Enabled = true;
            timer.Interval = 100;
            timer.Start();
        }

        public void Deactivate()
        {
            timer.Stop();
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

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(currentStateBindingSource.UpdateDataSource(MainV2.comPort.MAV.cs));
            }
            catch
            {
            }

            float pwm = 0;

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane ||
                MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover ||
                MainV2.comPort.MAV.cs.firmware == Firmwares.Ateryx) // APM 
            {
                if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE_CH") ||
                    MainV2.comPort.MAV.param.ContainsKey("MODE_CH"))
                {
                    var sw = 0;
                    if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE_CH"))
                    {
                        sw = (int) MainV2.comPort.MAV.param["FLTMODE_CH"].Value;
                    }
                    else
                    {
                        sw = (int) MainV2.comPort.MAV.param["MODE_CH"].Value;
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
                        LBL_flightmodepwm.Text = MainV2.comPort.MAV.param["FLTMODE_CH"] + ": " + pwm;
                    }
                    else
                    {
                        LBL_flightmodepwm.Text = MainV2.comPort.MAV.param["MODE_CH"] + ": " + pwm;
                    }
                }
            }

            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2) // ac2
            {
                pwm = MainV2.comPort.MAV.cs.ch5in;
                LBL_flightmodepwm.Text = "5: " + MainV2.comPort.MAV.cs.ch5in;
            }

            Control[] fmodelist = {CMB_fmode1, CMB_fmode2, CMB_fmode3, CMB_fmode4, CMB_fmode5, CMB_fmode6};

            foreach (var ctl in fmodelist)
            {
                ThemeManager.ApplyThemeTo(ctl);
            }

            var no = readSwitch(pwm);

            fmodelist[no].BackColor = ThemeManager.CurrentPPMBackground;
        }

        // from arducopter code
        private byte readSwitch(float inpwm)
        {
            var pulsewidth = (int) inpwm; // default for Arducopter

            if (pulsewidth > 1230 && pulsewidth <= 1360) return 1;
            if (pulsewidth > 1360 && pulsewidth <= 1490) return 2;
            if (pulsewidth > 1490 && pulsewidth <= 1620) return 3;
            if (pulsewidth > 1620 && pulsewidth <= 1749) return 4; // Software Manual
            if (pulsewidth >= 1750) return 5; // Hardware Manual
            return 0;
        }

        private void BUT_SaveModes_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param.ContainsKey("FLTMODE1"))
                {
                    MainV2.comPort.setParam("FLTMODE1", int.Parse(CMB_fmode1.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE2", int.Parse(CMB_fmode2.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE3", int.Parse(CMB_fmode3.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE4", int.Parse(CMB_fmode4.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE5", int.Parse(CMB_fmode5.SelectedValue.ToString()));
                    MainV2.comPort.setParam("FLTMODE6", int.Parse(CMB_fmode6.SelectedValue.ToString()));
                }
                else if (MainV2.comPort.MAV.param.ContainsKey("MODE1"))
                {
                    MainV2.comPort.setParam("MODE1", int.Parse(CMB_fmode1.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE2", int.Parse(CMB_fmode2.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE3", int.Parse(CMB_fmode3.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE4", int.Parse(CMB_fmode4.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE5", int.Parse(CMB_fmode5.SelectedValue.ToString()));
                    MainV2.comPort.setParam("MODE6", int.Parse(CMB_fmode6.SelectedValue.ToString()));
                }
                else if (MainV2.comPort.MAV.param.ContainsKey("COM_FLTMODE1"))
                {
                    MainV2.comPort.setParam("COM_FLTMODE1", int.Parse(CMB_fmode1.SelectedValue.ToString()));
                    MainV2.comPort.setParam("COM_FLTMODE2", int.Parse(CMB_fmode2.SelectedValue.ToString()));
                    MainV2.comPort.setParam("COM_FLTMODE3", int.Parse(CMB_fmode3.SelectedValue.ToString()));
                    MainV2.comPort.setParam("COM_FLTMODE4", int.Parse(CMB_fmode4.SelectedValue.ToString()));
                    MainV2.comPort.setParam("COM_FLTMODE5", int.Parse(CMB_fmode5.SelectedValue.ToString()));
                    MainV2.comPort.setParam("COM_FLTMODE6", int.Parse(CMB_fmode6.SelectedValue.ToString()));
                }

                if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2) // ac2
                {
                    // simple
                    var value = (float) (CB_simple1.Checked ? (int) SimpleMode.Simple1 : 0) +
                                (CB_simple2.Checked ? (int) SimpleMode.Simple2 : 0) +
                                (CB_simple3.Checked ? (int) SimpleMode.Simple3 : 0)
                                + (CB_simple4.Checked ? (int) SimpleMode.Simple4 : 0) +
                                (CB_simple5.Checked ? (int) SimpleMode.Simple5 : 0) +
                                (CB_simple6.Checked ? (int) SimpleMode.Simple6 : 0);
                    if (MainV2.comPort.MAV.param.ContainsKey("SIMPLE"))
                        MainV2.comPort.setParam("SIMPLE", value);

                    // supersimple
                    value = (float) (chk_ss1.Checked ? (int) SimpleMode.Simple1 : 0) +
                            (chk_ss2.Checked ? (int) SimpleMode.Simple2 : 0) +
                            (chk_ss3.Checked ? (int) SimpleMode.Simple3 : 0)
                            + (chk_ss4.Checked ? (int) SimpleMode.Simple4 : 0) +
                            (chk_ss5.Checked ? (int) SimpleMode.Simple5 : 0) +
                            (chk_ss6.Checked ? (int) SimpleMode.Simple6 : 0);
                    if (MainV2.comPort.MAV.param.ContainsKey("SUPER_SIMPLE"))
                        MainV2.comPort.setParam("SUPER_SIMPLE", value);
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            BUT_SaveModes.Text = "Complete";
        }

        private void updateDropDown(ComboBox ctl, string param)
        {
            ctl.DataSource = Common.getModesList(MainV2.comPort.MAV.cs.firmware);
            ctl.DisplayMember = "Value";
            ctl.ValueMember = "Key";
        }

        private void linkLabel1_ss_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("http://ardupilot.org/copter/docs/simpleandsuper-simple-modes.html");
            }
            catch
            {
                CustomMessageBox.Show(Strings.ERROR +
                                      " http://ardupilot.org/copter/docs/simpleandsuper-simple-modes.html");
            }
        }

        private void flightmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                var sender2 = (Control) sender;
                var currentmode = sender2.Text.ToLower();

                if (currentmode.Contains("althold") || currentmode.Contains("auto") ||
                    currentmode.Contains("autotune") || currentmode.Contains("land") ||
                    currentmode.Contains("loiter") || currentmode.Contains("ofloiter") ||
                    currentmode.Contains("poshold") || currentmode.Contains("rtl") ||
                    currentmode.Contains("sport") || currentmode.Contains("stabilize"))
                {
                    //CMB_fmode1
                    //CB_simple1
                    //chk_ss1

                    var number = sender2.Name.Substring(sender2.Name.Length - 1);

                    findandenableordisable("CB_simple" + number, true);
                    findandenableordisable("chk_ss" + number, true);
                }
                else
                {
                    var number = sender2.Name.Substring(sender2.Name.Length - 1);

                    findandenableordisable("CB_simple" + number, false);
                    findandenableordisable("chk_ss" + number, false);
                }
            }
        }

        private void findandenableordisable(string ctl, bool enable)
        {
            var items = Controls.Find(ctl, true);

            if (items.Length > 0)
            {
                items[0].Enabled = enable;
            }
        }
    }
}