using System;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigBatteryMonitoring2 : UserControl, IActivate, IDeactivate
    {
        private bool _startup;

        public ConfigBatteryMonitoring2()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen || !MainV2.comPort.MAV.param.ContainsKey("BATT2_MONITOR"))
            {
                Enabled = false;
                return;
            }

            _startup = true;

            if (MainV2.comPort.MAV.param["BATT2_CAPACITY"] != null)
                TXT_battcapacity.Text = MainV2.comPort.MAV.param["BATT2_CAPACITY"].ToString();

            TXT_voltage.Text = MainV2.comPort.MAV.cs._battery_voltage2.ToString();
            TXT_measuredvoltage.Text = TXT_voltage.Text;

            // new
            if (MainV2.comPort.MAV.param["BATT2_VOLT_MULT"] != null)
                TXT_divider.Text = MainV2.comPort.MAV.param["BATT2_VOLT_MULT"].ToString();

            if (MainV2.comPort.MAV.param["BATT2_AMP_PERVOL"] != null)
                TXT_ampspervolt.Text = MainV2.comPort.MAV.param["BATT2_AMP_PERVOL"].ToString();

            if (Settings.Instance.GetBoolean("speechbatteryenabled") && Settings.Instance.GetBoolean("speechenable"))
            {
                CHK_speechbattery.Checked = true;
            }
            else
            {
                CHK_speechbattery.Checked = false;
            }

            //http://plane.ardupilot.com/wiki/common-pixhawk-overview/#pixhawk_analog_input_pins_virtual_pin_firmware_mapped_pin_id
            //

            mavlinkComboBox1.setup(ParameterMetaDataRepository.GetParameterOptionsInt("BATT2_MONITOR",
                MainV2.comPort.MAV.cs.firmware.ToString()), "BATT2_MONITOR", MainV2.comPort.MAV.param);
            mavlinkComboBox2.setup(ParameterMetaDataRepository.GetParameterOptionsInt("BATT2_VOLT_PIN",
                MainV2.comPort.MAV.cs.firmware.ToString()), "BATT2_VOLT_PIN", MainV2.comPort.MAV.param);
            mavlinkComboBox3.setup(ParameterMetaDataRepository.GetParameterOptionsInt("BATT2_CURR_PIN",
                MainV2.comPort.MAV.cs.firmware.ToString()), "BATT2_CURR_PIN", MainV2.comPort.MAV.param);

            _startup = false;

            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
            _startup = true;
        }

        private void TXT_battcapacity_Validated(object sender, EventArgs e)
        {
            if (_startup || ((TextBox) sender).Enabled == false)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["BATT2_CAPACITY"] == null)
                {
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    MainV2.comPort.setParam("BATT2_CAPACITY", float.Parse(TXT_battcapacity.Text));
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT2_CAPACITY Failed", Strings.ERROR);
            }
        }

        private void TXT_measuredvoltage_Validated(object sender, EventArgs e)
        {
            if (_startup || ((TextBox) sender).Enabled == false)
                return;
            try
            {
                var measuredvoltage = float.Parse(TXT_measuredvoltage.Text);
                var voltage = float.Parse(TXT_voltage.Text);
                var divider = float.Parse(TXT_divider.Text);
                if (voltage == 0)
                    return;
                var newDivider = (measuredvoltage*divider)/voltage;
                TXT_divider.Text = newDivider.ToString();
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }

            try
            {
                MainV2.comPort.setParam(new[] {"BATT2_VOLT_MULT"}, float.Parse(TXT_divider.Text));
            }
            catch
            {
                CustomMessageBox.Show("Set BATT2_VOLT_MULT Failed", Strings.ERROR);
            }
        }

        private void TXT_divider_Validated(object sender, EventArgs e)
        {
            if (_startup || ((TextBox) sender).Enabled == false)
                return;
            try
            {
                MainV2.comPort.setParam(new[] {"BATT2_VOLT_MULT"}, float.Parse(TXT_divider.Text));
            }
            catch
            {
                CustomMessageBox.Show("Set BATT2_VOLT_MULT Failed", Strings.ERROR);
            }
        }

        private void TXT_ampspervolt_Validated(object sender, EventArgs e)
        {
            if (_startup || ((TextBox) sender).Enabled == false)
                return;
            try
            {
                MainV2.comPort.setParam(new[] {"BATT2_AMP_PERVOL"}, float.Parse(TXT_ampspervolt.Text));
            }
            catch
            {
                CustomMessageBox.Show("Set BATT2_AMP_PERVOL Failed", Strings.ERROR);
            }
        }

        private int getIndex(ComboBox ctl, int no)
        {
            foreach (var item in ctl.Items)
            {
                var ans = int.Parse(item.ToString().Substring(0, 1));

                if (ans == no)
                    return ctl.Items.IndexOf(item);
            }

            return -1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TXT_voltage.Text = MainV2.comPort.MAV.cs._battery_voltage2.ToString();
            txt_current.Text = MainV2.comPort.MAV.cs.current2.ToString();
        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (_startup)
                return;

            // enable the battery event
            Settings.Instance["speechbatteryenabled"] = ((CheckBox) sender).Checked.ToString();
            // enable speech engine
            Settings.Instance["speechenable"] = true.ToString();

            if (((CheckBox) sender).Checked)
            {
                var speechstring = "WARNING, Battery at {batv} Volt, {batp} percent";
                if (Settings.Instance["speechbattery"] != null)
                    speechstring = Settings.Instance["speechbattery"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Notification", "What do you want it to say?", ref speechstring))
                    return;
                Settings.Instance["speechbattery"] = speechstring;

                speechstring = "9.6";
                if (Settings.Instance["speechbatteryvolt"] != null)
                    speechstring = Settings.Instance["speechbatteryvolt"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Battery Level", "What Voltage do you want to warn at?", ref speechstring))
                    return;
                Settings.Instance["speechbatteryvolt"] = speechstring;

                speechstring = "20";
                if (Settings.Instance["speechbatterypercent"] != null)
                    speechstring = Settings.Instance["speechbatterypercent"].ToString();
                if (DialogResult.Cancel ==
                    InputBox.Show("Battery Level", "What percentage do you want to warn at?", ref speechstring))
                    return;
                Settings.Instance["speechbatterypercent"] = speechstring;
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
            if (_startup || ((TextBox) sender).Enabled == false)
                return;
            try
            {
                var measuredcurrent = float.Parse(txt_meascurrent.Text);
                var current = float.Parse(txt_current.Text);
                var divider = float.Parse(TXT_ampspervolt.Text);
                if (current == 0)
                    return;
                var newDivider = (measuredcurrent*divider)/current;
                TXT_ampspervolt.Text = newDivider.ToString();
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }

            try
            {
                MainV2.comPort.setParam(new[] {"BATT2_AMP_PERVOL"}, float.Parse(TXT_ampspervolt.Text));
            }
            catch
            {
                CustomMessageBox.Show("Set BATT2_AMP_PERVOL Failed", Strings.ERROR);
            }
        }
    }
}