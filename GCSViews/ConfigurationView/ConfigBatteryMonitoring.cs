using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigBatteryMonitoring : UserControl, IActivate, IDeactivate
    {
        bool startup = false;

        public ConfigBatteryMonitoring()
        {
            InitializeComponent();
        }

        private void CHK_enablebattmon_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (((CheckBox)sender).Checked == false)
                {
                    CMB_batmontype.SelectedIndex = 0;
                }
                else
                {
                    if (CMB_batmontype.SelectedIndex <= 0)
                        CMB_batmontype.SelectedIndex = 1;
                }
            }
            catch { CustomMessageBox.Show("Set BATT_MONITOR Failed", "Error"); }
        }
        private void TXT_battcapacity_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_battcapacity.Text, out ans);
        }
        private void TXT_battcapacity_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["BATT_CAPACITY"] == null)
                {
                    CustomMessageBox.Show("Not Available", "Error");
                }
                else
                {
                    MainV2.comPort.setParam("BATT_CAPACITY", float.Parse(TXT_battcapacity.Text));
                }
            }
            catch { CustomMessageBox.Show("Set BATT_CAPACITY Failed", "Error"); }
        }
        private void CMB_batmontype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["BATT_MONITOR"] == null)
                {
                    CustomMessageBox.Show("Not Available", "Error");
                }
                else
                {
                    int selection = int.Parse(CMB_batmontype.Text.Substring(0, 1));

                    CMB_batmonsensortype.Enabled = true;

                    TXT_voltage.Enabled = false;

                    if (selection == 0)
                    {
                        CMB_batmonsensortype.Enabled = false;
                        CMB_apmversion.Enabled = false;
                        groupBox4.Enabled = false;
                        MainV2.comPort.setParam("BATT_VOLT_PIN", -1);
                        MainV2.comPort.setParam("BATT_CURR_PIN", -1);
                    }
                    else if (selection == 4)
                    {
                        CMB_batmonsensortype.Enabled = true;
                        CMB_apmversion.Enabled = true;
                        groupBox4.Enabled = true;
                        TXT_ampspervolt.Enabled = true;
                    }
                    else if (selection == 3)
                    {
                        groupBox4.Enabled = true;
                        CMB_batmonsensortype.Enabled = false;
                        CMB_apmversion.Enabled = true;
                        TXT_ampspervolt.Enabled = false;
                        TXT_measuredvoltage.Enabled = true;
                        TXT_divider.Enabled = true;
                    }

                    MainV2.comPort.setParam("BATT_MONITOR", selection);
                }
            }
            catch { CustomMessageBox.Show("Set BATT_MONITOR,BATT_VOLT_PIN,BATT_CURR_PIN Failed", "Error"); }
        }
   
        private void TXT_measuredvoltage_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_measuredvoltage.Text, out ans);
        }
        private void TXT_measuredvoltage_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                float measuredvoltage = float.Parse(TXT_measuredvoltage.Text);
                float voltage = float.Parse(TXT_voltage.Text);
                float divider = float.Parse(TXT_divider.Text);
                if (voltage == 0)
                    return;
                float new_divider = (measuredvoltage * divider) / voltage;
                TXT_divider.Text = new_divider.ToString();
            }
            catch { CustomMessageBox.Show("Invalid number entered", "Error"); return; }

            try
            {
                
                    MainV2.comPort.setParam(new string[] {"VOLT_DIVIDER","BATT_VOLT_MULT"}, float.Parse(TXT_divider.Text));
                
            }
            catch { CustomMessageBox.Show("Set BATT_VOLT_MULT Failed", "Error"); }
        }
        private void TXT_divider_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_divider.Text, out ans);
        }
        private void TXT_divider_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                
                    MainV2.comPort.setParam(new string[] {"VOLT_DIVIDER","BATT_VOLT_MULT"}, float.Parse(TXT_divider.Text));
                
            }
            catch { CustomMessageBox.Show("Set BATT_VOLT_MULT Failed", "Error"); }
        }
        private void TXT_ampspervolt_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_ampspervolt.Text, out ans);
        }
        private void TXT_ampspervolt_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                
                    MainV2.comPort.setParam(new string[] {"AMP_PER_VOLT","BATT_AMP_PERVOLT"}, float.Parse(TXT_ampspervolt.Text));
                
            }
            catch { CustomMessageBox.Show("Set BATT_AMP_PERVOLT Failed", "Error"); }
        }

        private void CMB_batmonsensortype_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selection = int.Parse(CMB_batmonsensortype.Text.Substring(0, 1));

            if (selection == 1) // atto 45
            {
                float maxvolt = 13.6f;
                float maxamps = 44.7f;
                float mvpervolt = 242.3f;
                float mvperamp = 73.20f;

                // ~ 3.295v
                float topvolt = (maxvolt * mvpervolt) / 1000;
                // ~ 3.294v
                float topamps = (maxamps * mvperamp) / 1000;

                TXT_divider.Text = (maxvolt / topvolt).ToString();
                TXT_ampspervolt.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 2) // atto 90
            {
                float maxvolt = 50f;
                float maxamps = 89.4f;
                float mvpervolt = 63.69f;
                float mvperamp = 36.60f;

                float topvolt = (maxvolt * mvpervolt) / 1000;
                float topamps = (maxamps * mvperamp) / 1000;

                TXT_divider.Text = (maxvolt / topvolt).ToString();
                TXT_ampspervolt.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 3) // atto 180
            {
                float maxvolt = 50f;
                float maxamps = 178.8f;
                float mvpervolt = 63.69f;
                float mvperamp = 18.30f;

                float topvolt = (maxvolt * mvpervolt) / 1000;
                float topamps = (maxamps * mvperamp) / 1000;

                TXT_divider.Text = (maxvolt / topvolt).ToString();
                TXT_ampspervolt.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 4) // 3dr iv
            {
                float maxvolt = 50f;
                float maxamps = 90f;
                float mvpervolt = 99f;
                float mvperamp = 55.55f;

                float topvolt = (maxvolt * mvpervolt) / 1000;
                float topamps = (maxamps * mvperamp) / 1000;

                TXT_divider.Text = (maxvolt / topvolt).ToString();
                TXT_ampspervolt.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 5) // 3dr 4 in one esc
            {
                TXT_divider.Text = (12.02).ToString();
                TXT_ampspervolt.Text = (17).ToString();
            }
            else if (selection == 6) // hv 3dr apm - what i have
            {
                TXT_divider.Text = (12.02).ToString();
                TXT_ampspervolt.Text = (24).ToString();
            }
            else if (selection == 7) // hv 3dr px4
            {
                TXT_divider.Text = (12.02).ToString();
                TXT_ampspervolt.Text = (33.52).ToString();
            }

            // enable to update
            TXT_divider.Enabled = true;
            TXT_ampspervolt.Enabled = true;
            TXT_measuredvoltage.Enabled = true;

            // update
            TXT_ampspervolt_Validated(TXT_ampspervolt, null);

            TXT_divider_Validated(TXT_divider, null);

            // disable
            TXT_divider.Enabled = false;
            TXT_ampspervolt.Enabled = false;
            TXT_measuredvoltage.Enabled = false;

            //reenable if needed
            if (selection == 0)
            {
                TXT_divider.Enabled = true;
                TXT_ampspervolt.Enabled = true;
                TXT_measuredvoltage.Enabled = true;
            }
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen || !MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR"))
            {
                this.Enabled = false;
                return;
            }

            startup = true;
            if (MainV2.comPort.MAV.param["BATT_MONITOR"] != null)
            {
                if ((float)MainV2.comPort.MAV.param["BATT_MONITOR"] != 0)
                {
                    CMB_batmontype.SelectedIndex = getIndex(CMB_batmontype, (int)float.Parse(MainV2.comPort.MAV.param["BATT_MONITOR"].ToString()));
                }
                else
                {
                    CMB_batmontype.SelectedIndex = 0;
                }
            }

            if (MainV2.comPort.MAV.param["BATT_CAPACITY"] != null)
                TXT_battcapacity.Text = MainV2.comPort.MAV.param["BATT_CAPACITY"].ToString();

            TXT_voltage.Text = MainV2.comPort.MAV.cs.battery_voltage.ToString();
            TXT_measuredvoltage.Text = TXT_voltage.Text;

			// new
            if (MainV2.comPort.MAV.param["BATT_VOLT_MULT"] != null)
                TXT_divider.Text = MainV2.comPort.MAV.param["BATT_VOLT_MULT"].ToString();

            if (MainV2.comPort.MAV.param["BATT_AMP_PERVOLT"] != null)
                TXT_ampspervolt.Text = MainV2.comPort.MAV.param["BATT_AMP_PERVOLT"].ToString();
			// old
            if (MainV2.comPort.MAV.param["VOLT_DIVIDER"] != null)
                TXT_divider.Text = MainV2.comPort.MAV.param["VOLT_DIVIDER"].ToString();

            if (MainV2.comPort.MAV.param["AMP_PER_VOLT"] != null)
                TXT_ampspervolt.Text = MainV2.comPort.MAV.param["AMP_PER_VOLT"].ToString();

            if (MainV2.config["speechbatteryenabled"] != null && MainV2.config["speechbatteryenabled"].ToString() == "True" && MainV2.config["speechenable"] != null && MainV2.config["speechenable"].ToString() == "True")
            {
                CHK_speechbattery.Checked = true;
            }
            else
            {
                CHK_speechbattery.Checked = false;
            }
            
            // determine the sensor type
            if (TXT_ampspervolt.Text == (13.6612).ToString() && TXT_divider.Text == (4.127115).ToString())
            {
                CMB_batmonsensortype.SelectedIndex = 1;
            }
            else if (TXT_ampspervolt.Text == (27.3224).ToString() && TXT_divider.Text == (15.70105).ToString())
            {
                CMB_batmonsensortype.SelectedIndex = 2;
            }
            else if (TXT_ampspervolt.Text == (54.64481).ToString() && TXT_divider.Text == (15.70105).ToString())
            {
                CMB_batmonsensortype.SelectedIndex = 3;
            }
            else if (TXT_ampspervolt.Text == (18.0018).ToString() && TXT_divider.Text == (10.10101).ToString())
            {
                CMB_batmonsensortype.SelectedIndex = 4;
            }
            else if (TXT_ampspervolt.Text == (17).ToString() && TXT_divider.Text == (12.02).ToString())
            {
                CMB_batmonsensortype.SelectedIndex = 5;
            }
            else
            {
                CMB_batmonsensortype.SelectedIndex = 0;
            }

            // determine the board type
            if (MainV2.comPort.MAV.param["BATT_VOLT_PIN"] != null)
            {
                CMB_apmversion.Enabled = true;

                float value = (float)MainV2.comPort.MAV.param["BATT_VOLT_PIN"];
                if (value == 0) // apm1
                {
                    CMB_apmversion.SelectedIndex = 0;
                }
                else if (value == 1) // apm2
                {
                    CMB_apmversion.SelectedIndex = 1;
                }
                else if (value == 13) // apm2.5
                {
                    CMB_apmversion.SelectedIndex = 2;
                }
                else if (value == 100) // px4
                {
                    CMB_apmversion.SelectedIndex = 3;
                }
                else if (value == 2)
                { // pixhawk
                    CMB_apmversion.SelectedIndex = 4;
                }
            }
            else
            {
                CMB_apmversion.Enabled = false;
            }

            startup = false;

            CMB_batmontype_SelectedIndexChanged(null, null);
            CMB_batmonsensortype_SelectedIndexChanged(null, null);

            timer1.Start();
        }

        int getIndex(ComboBox ctl, int no)
        {
            foreach (var item in ctl.Items)
            {
                int ans = int.Parse(item.ToString().Substring(0, 1));

                if (ans == no)
                    return ctl.Items.IndexOf(item);
            }

            return -1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TXT_voltage.Text = MainV2.comPort.MAV.cs.battery_voltage.ToString();
            txt_current.Text = MainV2.comPort.MAV.cs.current.ToString();
        }

        private void CMB_apmversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            int selection = int.Parse(CMB_apmversion.Text.Substring(0, 1));

            try
            {
                if (selection == 0)
                {
                    // apm1
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 0);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 1);
                }
                else if (selection == 1)
                {
                    // apm2
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 1);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 2);
                }
                else if (selection == 2)
                {
                    //apm2.5
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 13);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 12);
                }
                else if (selection == 3)
                {
                    //px4
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 100);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 101);
                    MainV2.comPort.setParam(new string[] {"VOLT_DIVIDER","BATT_VOLT_MULT"}, 1);
                    TXT_divider.Text = "1";
                }
                else if (selection == 4)
                {
                    //px4
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 2);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 3);
                }
                else if (selection == 5)
                {
                    //vrbrain 5
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 10);
                    MainV2.comPort.setParam("BATT_CURR_PIN", 11);
                    MainV2.comPort.setParam(new string[] { "VOLT_DIVIDER", "BATT_VOLT_MULT" }, 10);
                    TXT_divider.Text = "10";
                }
                else if (selection == 6)
                {
                    //vr micro brain 5
                    MainV2.comPort.setParam("BATT_VOLT_PIN", 10);
                    MainV2.comPort.setParam("BATT_CURR_PIN", -1);
                    MainV2.comPort.setParam(new string[] { "VOLT_DIVIDER", "BATT_VOLT_MULT" }, 10);
                    TXT_divider.Text = "10";
                }
            }
            catch { CustomMessageBox.Show("Set BATT_????_PIN Failed", "Error"); }

        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            // enable the battery event
            MainV2.config["speechbatteryenabled"] = ((CheckBox)sender).Checked.ToString();
            // enable speech engine
            MainV2.config["speechenable"] = true.ToString();

            if (((CheckBox)sender).Checked)
            {
                string speechstring = "WARNING, Battery at {batv} Volt, {batp} percent";
                if (MainV2.config["speechbattery"] != null)
                    speechstring = MainV2.config["speechbattery"].ToString();
                if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                MainV2.config["speechbattery"] = speechstring;

                speechstring = "9.6";
                if (MainV2.config["speechbatteryvolt"] != null)
                    speechstring = MainV2.config["speechbatteryvolt"].ToString();
                if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("Battery Level", "What Voltage do you want to warn at?", ref speechstring))
                    return;
                MainV2.config["speechbatteryvolt"] = speechstring;

                speechstring = "20";
                if (MainV2.config["speechbatterypercent"] != null)
                    speechstring = MainV2.config["speechbatterypercent"].ToString();
                if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("Battery Level", "What percentage do you want to warn at?", ref speechstring))
                    return;
                MainV2.config["speechbatterypercent"] = speechstring;
            }
        }

        private void TXT_measuredvoltage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                TXT_measuredvoltage_Validated(sender, e);
        }

        private void TXT_ampspervolt_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                TXT_ampspervolt_Validated(sender, e);
        }

        private void TXT_divider_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                TXT_divider_Validated(sender, e);
        }

        private void txt_meascurrent_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                float measuredcurrent = float.Parse(txt_meascurrent.Text);
                float current = float.Parse(txt_current.Text);
                float divider = float.Parse(TXT_ampspervolt.Text);
                if (current == 0)
                    return;
                float new_divider = (measuredcurrent * divider) / current;
                TXT_ampspervolt.Text = new_divider.ToString();
            }
            catch { CustomMessageBox.Show("Invalid number entered", "Error"); return; }

            try
            {

                MainV2.comPort.setParam(new string[] { "AMP_PER_VOLT", "BATT_AMP_PERVOLT" }, float.Parse(TXT_ampspervolt.Text));

            }
            catch { CustomMessageBox.Show("Set BATT_AMP_PERVOLT Failed", "Error"); }
        }
    }
}
