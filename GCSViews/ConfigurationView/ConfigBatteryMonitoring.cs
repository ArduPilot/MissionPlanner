using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigBatteryMonitoring : MyUserControl, IActivate, IDeactivate
    {
        private bool startup;
        private BatteryMonitorGB _gb1, _gb2;
        private BatteryMonitorContent _content1, _content2;
        private bool _initializedDual;

        public ConfigBatteryMonitoring()
        {
            InitializeComponent();
            InitializeDualLayout();
        }

        private void InitializeDualLayout()
        {
            if (_initializedDual)
                return;

            _content1 = new BatteryMonitorContent("BATT", () => MainV2.comPort.MAV.cs.battery_voltage, () => MainV2.comPort.MAV.cs.current);
            _content2 = new BatteryMonitorContent("BATT2", () => MainV2.comPort.MAV.cs.battery_voltage2, () => MainV2.comPort.MAV.cs.current2);

            _gb1 = new BatteryMonitorGB { GroupTitle = "Battery Monitor 1", InnerControl = _content1 };
            _gb2 = new BatteryMonitorGB { GroupTitle = "Battery Monitor 2", InnerControl = _content2 };

            _gb1.Dock = DockStyle.None;
            _gb2.Dock = DockStyle.None;

            var tlp = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                ColumnCount = 2,
                RowCount = 1
            };
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlp.Controls.Add(_gb1, 0, 0);
            tlp.Controls.Add(_gb2, 1, 0);

            this.Controls.Clear();
            this.Controls.Add(tlp);

            _initializedDual = true;
        }

        public void Activate()
        {
            InitializeDualLayout();
            _content1?.Activate();
            _content2?.Activate();
        }

        public void Deactivate()
        {
            _content1?.Deactivate();
            _content2?.Deactivate();
            startup = true;
        }

        private void TXT_battcapacity_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["BATT_CAPACITY"] == null)
                {
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CAPACITY", float.Parse(TXT_battcapacity.Text));
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT_CAPACITY Failed", Strings.ERROR);
            }
        }

        private void CMB_batmontype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["BATT_MONITOR"] == null)
                {
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    var selection = (int)CMB_batmontype.SelectedValue;

                    CMB_batmonsensortype.Enabled = true;

                    // leave voltage label enabled for normal appearance

                    if (selection == 0)
                    {
                        CMB_batmonsensortype.Enabled = false;
                        CMB_HWVersion.Enabled = false;
                        groupBox4.Enabled = false;
                        if (groupBoxCalibrate != null) groupBoxCalibrate.Enabled = false;
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", -1);
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", -1);
                    }
                    else if (selection == 4)
                    {
                        CMB_batmonsensortype.Enabled = true;
                        CMB_HWVersion.Enabled = true;
                        groupBox4.Enabled = true;
                        if (groupBoxCalibrate != null) groupBoxCalibrate.Enabled = true;
                        TXT_AMP_PERVLT.Enabled = true;
                    }
                    else if (selection == 3)
                    {
                        groupBox4.Enabled = true;
                        if (groupBoxCalibrate != null) groupBoxCalibrate.Enabled = true;
                        CMB_batmonsensortype.Enabled = false;
                        CMB_HWVersion.Enabled = true;
                        TXT_AMP_PERVLT.Enabled = false;
                        TXT_measuredvoltage.Enabled = true;
                        TXT_divider_VOLT_MULT.Enabled = true;
                    }

                    if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                        MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 0 &&
                        selection != 0)
                    {
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_MONITOR", selection);
                        MainV2.comPort.getParamList();
                        this.Activate();
                    }
                    else
                    {
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_MONITOR", selection);
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT_MONITOR,BATT_VOLT_PIN,BATT_CURR_PIN Failed", Strings.ERROR);
            }
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
                var measuredvoltage = float.Parse(TXT_measuredvoltage.Text);
                var voltage = float.Parse(TXT_voltage.Text);
                var divider = float.Parse(TXT_divider_VOLT_MULT.Text);
                if (voltage == 0)
                    return;
                var new_divider = (measuredvoltage * divider) / voltage;
                TXT_divider_VOLT_MULT.Text = new_divider.ToString();
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }

            try
            {
                MainV2.comPort.setParam(new[] { "VOLT_DIVIDER", "BATT_VOLT_MULT" }, float.Parse(TXT_divider_VOLT_MULT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                    (MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 3 ||
                     MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 4)) {
                   CustomMessageBox.Show("Set BATT_VOLT_MULT Failed", Strings.ERROR);
                }
            }
        }

        private void TXT_divider_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_divider_VOLT_MULT.Text, out ans);
        }

        private void TXT_divider_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                MainV2.comPort.setParam(new[] { "VOLT_DIVIDER", "BATT_VOLT_MULT" }, float.Parse(TXT_divider_VOLT_MULT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                    (MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 3 ||
                     MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 4)) {
                  CustomMessageBox.Show("Set BATT_VOLT_MULT Failed", Strings.ERROR);
                }
            }
        }

        private void TXT_ampspervolt_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_AMP_PERVLT.Text, out ans);
        }

        private void TXT_ampspervolt_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                MainV2.comPort.setParam(new[] { "AMP_PER_VOLT", "BATT_AMP_PERVOLT", "BATT_AMP_PERVLT" }, float.Parse(TXT_AMP_PERVLT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                    (MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 3 ||
                     MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 4)) {
                  CustomMessageBox.Show("Set BATT_AMP_PERVOLT Failed", Strings.ERROR);
                }
            }
        }

        private void CMB_batmonsensortype_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = int.Parse(CMB_batmonsensortype.Text.Substring(0, 1));

            if (selection == 1) // atto 45
            {
                var maxvolt = 13.6f;
                var maxamps = 44.7f;
                var mvpervolt = 242.3f;
                var mvperamp = 73.20f;

                // ~ 3.295v
                var topvolt = (maxvolt * mvpervolt) / 1000;
                // ~ 3.294v
                var topamps = (maxamps * mvperamp) / 1000;

                TXT_divider_VOLT_MULT.Text = (maxvolt / topvolt).ToString();
                TXT_AMP_PERVLT.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 2) // atto 90
            {
                var maxvolt = 50f;
                var maxamps = 89.4f;
                var mvpervolt = 63.69f;
                var mvperamp = 36.60f;

                var topvolt = (maxvolt * mvpervolt) / 1000;
                var topamps = (maxamps * mvperamp) / 1000;

                TXT_divider_VOLT_MULT.Text = (maxvolt / topvolt).ToString();
                TXT_AMP_PERVLT.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 3) // atto 180
            {
                var maxvolt = 50f;
                var maxamps = 178.8f;
                var mvpervolt = 63.69f;
                var mvperamp = 18.30f;

                var topvolt = (maxvolt * mvpervolt) / 1000;
                var topamps = (maxamps * mvperamp) / 1000;

                TXT_divider_VOLT_MULT.Text = (maxvolt / topvolt).ToString();
                TXT_AMP_PERVLT.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 4) // 3dr iv
            {
                var maxvolt = 50f;
                var maxamps = 90f;
                var mvpervolt = 99f;
                var mvperamp = 55.55f;

                var topvolt = (maxvolt * mvpervolt) / 1000;
                var topamps = (maxamps * mvperamp) / 1000;

                TXT_divider_VOLT_MULT.Text = (maxvolt / topvolt).ToString();
                TXT_AMP_PERVLT.Text = (maxamps / topamps).ToString();
            }
            else if (selection == 5) // 3dr 4 in one esc
            {
                TXT_divider_VOLT_MULT.Text = (12.02).ToString();
                TXT_AMP_PERVLT.Text = (17).ToString();
            }
            else if (selection == 6) // hv 3dr apm - what i have
            {
                TXT_divider_VOLT_MULT.Text = (12.02).ToString();
                TXT_AMP_PERVLT.Text = (24).ToString();
            }
            else if (selection == 7) // hv 3dr px4 cube
            {
                TXT_divider_VOLT_MULT.Text = (12.02).ToString();
                TXT_AMP_PERVLT.Text = (39.877).ToString();
            }
            else if (selection == 8) // pixhack
            {
                TXT_divider_VOLT_MULT.Text = (18).ToString();
                TXT_AMP_PERVLT.Text = (24).ToString();
            }
            else if (selection == 9) // Holybro Pixhawk4
            {
                TXT_divider_VOLT_MULT.Text = (18.182).ToString();
                TXT_AMP_PERVLT.Text = (36.364).ToString();
            }

            // enable to update
            TXT_divider_VOLT_MULT.Enabled = true;
            TXT_AMP_PERVLT.Enabled = true;
            TXT_measuredvoltage.Enabled = true;

            // update
            TXT_ampspervolt_Validated(TXT_AMP_PERVLT, null);

            TXT_divider_Validated(TXT_divider_VOLT_MULT, null);

            // disable
            TXT_divider_VOLT_MULT.Enabled = false;
            TXT_AMP_PERVLT.Enabled = false;
            TXT_measuredvoltage.Enabled = false;

            //reenable if needed
            if (selection == 0)
            {
                TXT_divider_VOLT_MULT.Enabled = true;
                TXT_AMP_PERVLT.Enabled = true;
                TXT_measuredvoltage.Enabled = true;
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
            TXT_voltage.Text = MainV2.comPort.MAV.cs.battery_voltage.ToString();
            txt_current.Text = MainV2.comPort.MAV.cs.current.ToString();
        }

        private void CMB_apmversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            var selection = int.Parse(CMB_HWVersion.Text.Substring(0, 2).Replace(":", ""));

            try
            {
                if (selection == 0)
                {
                    // apm1
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 0);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 1);
                }
                else if (selection == 1)
                {
                    // apm2
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 1);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 2);
                }
                else if (selection == 2)
                {
                    //apm2.5
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 13);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 12);
                }
                else if (selection == 3)
                {
                    //px4
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 100);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 101);
                }
                else if (selection == 4)
                {
                    //px4
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 2);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 3);
                }
                else if (selection == 5)
                {
                    //vrbrain 5
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 10);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 11);
                }
                else if (selection == 6)
                {
                    //vr micro brain 5
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 10);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", -1);
                }
                else if (selection == 7)
                {
                    //vr brain 4
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 6);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 7);
                }
                else if (selection == 8)
                {
                    //cube orange
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 14);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 15);
                }
                else if (selection == 9)
                {
                    //durandal
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 16);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 17);
                }
                else if (selection == 10)
                {
                    //Pixhawk 6C/Pix32 v6
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_VOLT_PIN", 8);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "BATT_CURR_PIN", 4);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set BATT_????_PIN Failed", Strings.ERROR);
            }
        }

        private void btnCalcVoltage_Click(object sender, EventArgs e)
        {
            // Trigger the same logic as leaving the measured voltage field
            TXT_measuredvoltage_Validated(TXT_measuredvoltage, EventArgs.Empty);
        }

        private void btnCalcCurrent_Click(object sender, EventArgs e)
        {
            // Trigger the same logic as leaving the measured current field
            txt_meascurrent_Validated(txt_meascurrent, EventArgs.Empty);
        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            // enable the battery event
            Settings.Instance["speechbatteryenabled"] = ((CheckBox)sender).Checked.ToString();
            // enable speech engine
            Settings.Instance["speechenable"] = true.ToString();

            if (((CheckBox)sender).Checked)
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
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                var measuredcurrent = float.Parse(txt_meascurrent.Text);
                var current = float.Parse(txt_current.Text);
                var divider = float.Parse(TXT_AMP_PERVLT.Text);
                if (current == 0)
                    return;
                var new_divider = (measuredcurrent * divider) / current;
                TXT_AMP_PERVLT.Text = new_divider.ToString();
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                return;
            }

            try
            {
                MainV2.comPort.setParam(new[] { "AMP_PER_VOLT", "BATT_AMP_PERVOLT", "BATT_AMP_PERVLT" }, float.Parse(TXT_AMP_PERVLT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BATT_MONITOR") &&
                    (MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 3 ||
                     MainV2.comPort.MAV.param["BATT_MONITOR"].Value == 4)) {
                  CustomMessageBox.Show("Set BATT_AMP_PERVOLT Failed", Strings.ERROR);
                }
            }
        }
    }
}
