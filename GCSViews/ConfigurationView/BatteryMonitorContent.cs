using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class BatteryMonitorContent : MyUserControl, IActivate, IDeactivate
    {
        private bool startup;
        private readonly string _prefix; // e.g. "BATT" or "BATT2"
        private readonly Func<double> _getVoltage;
        private readonly Func<double> _getCurrent;
        private MavlinkComboBox _b2VoltPin;
        private MavlinkComboBox _b2CurrPin;
        private Label _b2VoltLabel;
        private Label _b2CurrLabel;
        private Label _b2MahLabel;

        public BatteryMonitorContent(string prefix, Func<double> getVoltage, Func<double> getCurrent)
        {
            _prefix = prefix;
            _getVoltage = getVoltage;
            _getCurrent = getCurrent;
            InitializeComponent();
        }

        private string P(string name)
        {
            return _prefix + "_" + name;
        }

        public void Activate()
        {
            var monParam = P("MONITOR");
            if (!MainV2.comPort.BaseStream.IsOpen || !MainV2.comPort.MAV.param.ContainsKey(monParam))
            {
                Enabled = false;
                return;
            }

            startup = true;

            CMB_batmontype.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt(monParam,
                    MainV2.comPort.MAV.cs.firmware.ToString()), monParam, MainV2.comPort.MAV.param);

            var capParam = P("CAPACITY");
            if (MainV2.comPort.MAV.param[capParam] != null)
                TXT_battcapacity.Text = MainV2.comPort.MAV.param[capParam].ToString();

            TXT_voltage.Text = _getVoltage().ToString();

            // BATT2 layout: reuse Battery1 positions for pin combos
            if (_prefix == "BATT2")
            {
                // Create mAh label for BM2 at same position as label2 in BM1 (316, 47)
                if (_b2MahLabel == null)
                {
                    _b2MahLabel = new Label
                    {
                        Text = "mAh",
                        AutoSize = true,
                        Location = new System.Drawing.Point(316, 47)
                    };
                    this.Controls.Add(_b2MahLabel);
                }
                _b2MahLabel.Visible = true;
                _b2MahLabel.BringToFront();

                // hide MP Low Batt Alert for BM2 if no functional diff
                if (CHK_speechbattery != null) CHK_speechbattery.Visible = false;
                // Hide sensor type UI (not used by BATT2) but reuse its position
                if (CMB_batmonsensortype != null) CMB_batmonsensortype.Visible = false;
                var sensorLabelLoc = label47 != null ? label47.Location : new System.Drawing.Point(0, 0);
                if (label47 != null) label47.Visible = false;

                // Capture original HW Ver label position before reassigning
                var hwVerLabelLoc = label1 != null ? label1.Location : new System.Drawing.Point(0, 0);

                // Assign Volt Pin label at Sensor label position
                if (label1 != null)
                {
                    label1.Text = "Volt Pin:";
                    label1.Location = sensorLabelLoc;
                    label1.Visible = true;
                }
                // Assign Curr Pin label at HW Ver label position
                if (label2 != null)
                {
                    label2.Text = "Curr Pin:";
                    label2.Location = hwVerLabelLoc;
                    label2.Visible = true;
                }

                // Prepare BATT2 volt pin combo positioned exactly where HW Version sits
                if (_b2VoltPin == null)
                {
                    _b2VoltPin = new MavlinkComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DropDownWidth = 200,
                        Width = 180
                    };
                    this.Controls.Add(_b2VoltPin);
                }
                if (CMB_batmonsensortype != null)
                {
                    // Place Volt Pin combo where Sensor Type combo was
                    _b2VoltPin.Location = CMB_batmonsensortype.Location;
                    _b2VoltPin.Size = CMB_batmonsensortype.Size;
                    _b2VoltPin.Anchor = CMB_batmonsensortype.Anchor;
                }
                if (CMB_HWVersion != null)
                {
                    // Place Curr Pin combo where HW Version combo was
                    _b2CurrPin.Location = CMB_HWVersion.Location;
                    _b2CurrPin.Size = CMB_HWVersion.Size;
                    _b2CurrPin.Anchor = CMB_HWVersion.Anchor;
                    CMB_HWVersion.Visible = false;
                }

                // Prepare BATT2 curr pin combo positioned exactly where Sensor Type combo sits
                if (_b2CurrPin == null)
                {
                    _b2CurrPin = new MavlinkComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DropDownWidth = 200,
                        Width = 180
                    };
                    this.Controls.Add(_b2CurrPin);
                }
                // CMB_batmonsensortype already hidden above

                // bind to params
                _b2VoltPin.setup(ParameterMetaDataRepository.GetParameterOptionsInt(P("VOLT_PIN"), MainV2.comPort.MAV.cs.firmware.ToString()), P("VOLT_PIN"), MainV2.comPort.MAV.param);
                _b2CurrPin.setup(ParameterMetaDataRepository.GetParameterOptionsInt(P("CURR_PIN"), MainV2.comPort.MAV.cs.firmware.ToString()), P("CURR_PIN"), MainV2.comPort.MAV.param);
            }

            // preferred names
            var ampPerVoltNew = _prefix == "BATT2" ? P("AMP_PERVOL") : P("AMP_PERVOLT");
            var voltMult = P("VOLT_MULT");

            if (MainV2.comPort.MAV.param.ContainsKey(P("AMP_PERVLT")))
                TXT_AMP_PERVLT.Text = MainV2.comPort.MAV.param[P("AMP_PERVLT")].ToString();

            if (MainV2.comPort.MAV.param.ContainsKey(voltMult))
                TXT_divider_VOLT_MULT.Text = MainV2.comPort.MAV.param[voltMult].ToString();

            if (MainV2.comPort.MAV.param.ContainsKey(ampPerVoltNew))
                TXT_AMP_PERVLT.Text = MainV2.comPort.MAV.param[ampPerVoltNew].ToString();

            // legacy fallbacks for BATT only
            if (_prefix == "BATT")
            {
                if (MainV2.comPort.MAV.param.ContainsKey("VOLT_DIVIDER"))
                    TXT_divider_VOLT_MULT.Text = MainV2.comPort.MAV.param["VOLT_DIVIDER"].ToString();

                if (MainV2.comPort.MAV.param.ContainsKey("AMP_PER_VOLT"))
                    TXT_AMP_PERVLT.Text = MainV2.comPort.MAV.param["AMP_PER_VOLT"].ToString();
            }

            if (Settings.Instance.GetBoolean("speechbatteryenabled") && Settings.Instance.GetBoolean("speechenable"))
            {
                CHK_speechbattery.Checked = true;
            }
            else
            {
                CHK_speechbattery.Checked = false;
            }

            // determine the sensor type from current fields (only for BATT)
            if (_prefix != "BATT")
                goto skipSensorType;
            if (TXT_AMP_PERVLT.Text == (13.6612).ToString() && TXT_divider_VOLT_MULT.Text == (4.127115).ToString())
                CMB_batmonsensortype.SelectedIndex = 1;
            else if (TXT_AMP_PERVLT.Text == (27.3224).ToString() && TXT_divider_VOLT_MULT.Text == (15.70105).ToString())
                CMB_batmonsensortype.SelectedIndex = 2;
            else if (TXT_AMP_PERVLT.Text == (54.64481).ToString() && TXT_divider_VOLT_MULT.Text == (15.70105).ToString())
                CMB_batmonsensortype.SelectedIndex = 3;
            else if (TXT_AMP_PERVLT.Text == (18.0018).ToString() && TXT_divider_VOLT_MULT.Text == (10.10101).ToString())
                CMB_batmonsensortype.SelectedIndex = 4;
            else if (TXT_AMP_PERVLT.Text == (17).ToString() && TXT_divider_VOLT_MULT.Text == (12.02).ToString())
                CMB_batmonsensortype.SelectedIndex = 5;
            else if (TXT_AMP_PERVLT.Text == (24).ToString() && TXT_divider_VOLT_MULT.Text == (12.02).ToString())
                CMB_batmonsensortype.SelectedIndex = 6;
            else if (TXT_AMP_PERVLT.Text == (39.877).ToString() && TXT_divider_VOLT_MULT.Text == (12.02).ToString())
                CMB_batmonsensortype.SelectedIndex = 7;
            else if (TXT_AMP_PERVLT.Text == (24).ToString() && TXT_divider_VOLT_MULT.Text == (18).ToString())
                CMB_batmonsensortype.SelectedIndex = 8;
            else if (TXT_AMP_PERVLT.Text == (36.364).ToString() && TXT_divider_VOLT_MULT.Text == (18.182).ToString())
                CMB_batmonsensortype.SelectedIndex = 9;
            else
                CMB_batmonsensortype.SelectedIndex = 0;

        skipSensorType:

            // determine the board type
            var voltPinName = P("VOLT_PIN");
            var currPinName = P("CURR_PIN");
            // Board type UI/logic only applies to BATT
            if (_prefix == "BATT" && MainV2.comPort.MAV.param.ContainsKey(voltPinName) && MainV2.comPort.MAV.param[voltPinName] != null)
            {
                CMB_HWVersion.Enabled = true;

                var value = (double)MainV2.comPort.MAV.param[voltPinName];
                if (value == 0) CMB_HWVersion.SelectedIndex = 0; // apm1
                else if (value == 1) CMB_HWVersion.SelectedIndex = 1; // apm2
                else if (value == 13) CMB_HWVersion.SelectedIndex = 2; // apm2.5
                else if (value == 100) CMB_HWVersion.SelectedIndex = 3; // px4
                else if (value == 2) CMB_HWVersion.SelectedIndex = 4; // pixhawk
                else if (value == 6) CMB_HWVersion.SelectedIndex = 7; // vrbrain4
                else if (value == 10)
                {
                    // vrbrain 5 or micro
                    if (MainV2.comPort.MAV.param.ContainsKey(currPinName) && (double)MainV2.comPort.MAV.param[currPinName] == 11)
                        CMB_HWVersion.SelectedIndex = 5;
                    else
                        CMB_HWVersion.SelectedIndex = 6;
                }
                else if (value == 14) CMB_HWVersion.SelectedIndex = 8; // cubeorange
                else if (value == 16) CMB_HWVersion.SelectedIndex = 9; // durandal
                else if (value == 8) CMB_HWVersion.SelectedIndex = 10; // Pixhawk 6C/Pix32 v6
            }
            else
            {
                CMB_HWVersion.Enabled = false;
            }

            startup = false;

            CMB_batmontype_SelectedIndexChanged(null, null);
            if (_prefix == "BATT")
                CMB_batmonsensortype_SelectedIndexChanged(null, null);

            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
            startup = true;
        }

        private void TXT_battcapacity_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                var cap = P("CAPACITY");
                if (MainV2.comPort.MAV.param[cap] == null)
                {
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, cap, float.Parse(TXT_battcapacity.Text));
                }
            }
            catch
            {
                CustomMessageBox.Show("Set " + P("CAPACITY") + " Failed", Strings.ERROR);
            }
        }

        private void CMB_batmontype_SelectedIndexChanged(object sender, EventArgs e)
        {
            // BATT2 uses direct pin combos in the legacy UI; avoid BATT-specific side effects here
            if (_prefix == "BATT2")
                return;
            if (startup)
                return;
            try
            {
                var monitor = P("MONITOR");
                if (!MainV2.comPort.MAV.param.ContainsKey(monitor) || MainV2.comPort.MAV.param[monitor] == null)
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
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), -1);
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), -1);
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

                    if (MainV2.comPort.MAV.param.ContainsKey(monitor) &&
                        MainV2.comPort.MAV.param[monitor].Value == 0 &&
                        selection != 0)
                    {
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, monitor, selection);
                        MainV2.comPort.getParamList();
                        this.Activate();
                    }
                    else
                    {
                        MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, monitor, selection);
                    }
                }
            }
            catch
            {
                CustomMessageBox.Show("Set " + P("MONITOR") + "," + P("VOLT_PIN") + "," + P("CURR_PIN") + " Failed", Strings.ERROR);
            }
        }

        private void TXT_measuredvoltage_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !float.TryParse(TXT_measuredvoltage.Text, out _);
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
                if (_prefix == "BATT")
                    MainV2.comPort.setParam(new[] { "VOLT_DIVIDER", P("VOLT_MULT") }, float.Parse(TXT_divider_VOLT_MULT.Text));
                else
                    MainV2.comPort.setParam(new[] { P("VOLT_MULT") }, float.Parse(TXT_divider_VOLT_MULT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey(P("MONITOR")) &&
                    (MainV2.comPort.MAV.param[P("MONITOR")].Value == 3 ||
                     MainV2.comPort.MAV.param[P("MONITOR")].Value == 4))
                {
                    CustomMessageBox.Show("Set " + P("VOLT_MULT") + " Failed", Strings.ERROR);
                }
            }
        }

        private void TXT_divider_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !float.TryParse(TXT_divider_VOLT_MULT.Text, out _);
        }

        private void TXT_divider_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                if (_prefix == "BATT")
                    MainV2.comPort.setParam(new[] { "VOLT_DIVIDER", P("VOLT_MULT") }, float.Parse(TXT_divider_VOLT_MULT.Text));
                else
                    MainV2.comPort.setParam(new[] { P("VOLT_MULT") }, float.Parse(TXT_divider_VOLT_MULT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey(P("MONITOR")) &&
                    (MainV2.comPort.MAV.param[P("MONITOR")].Value == 3 ||
                     MainV2.comPort.MAV.param[P("MONITOR")].Value == 4))
                {
                    CustomMessageBox.Show("Set " + P("VOLT_MULT") + " Failed", Strings.ERROR);
                }
            }
        }

        private void TXT_ampspervolt_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !float.TryParse(TXT_AMP_PERVLT.Text, out _);
        }

        private void TXT_ampspervolt_Validated(object sender, EventArgs e)
        {
            if (startup || ((TextBox)sender).Enabled == false)
                return;
            try
            {
                if (_prefix == "BATT")
                    MainV2.comPort.setParam(new[] { "AMP_PER_VOLT", P("AMP_PERVOLT"), P("AMP_PERVLT") }, float.Parse(TXT_AMP_PERVLT.Text));
                else
                    MainV2.comPort.setParam(new[] { P("AMP_PERVOL") }, float.Parse(TXT_AMP_PERVLT.Text));
            }
            catch
            {
                if (MainV2.comPort.MAV.param.ContainsKey(P("MONITOR")) &&
                    (MainV2.comPort.MAV.param[P("MONITOR")].Value == 3 ||
                     MainV2.comPort.MAV.param[P("MONITOR")].Value == 4))
                {
                    CustomMessageBox.Show("Set " + (_prefix == "BATT" ? P("AMP_PERVOLT") : P("AMP_PERVOL")) + " Failed", Strings.ERROR);
                }
            }
        }

        private void CMB_batmonsensortype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_prefix != "BATT")
                return;
            var selection = int.Parse(CMB_batmonsensortype.Text.Substring(0, 1));

            if (selection == 1) // atto 45
            {
                var maxvolt = 13.6f;
                var maxamps = 44.7f;
                var mvpervolt = 242.3f;
                var mvperamp = 73.20f;

                var topvolt = (maxvolt * mvpervolt) / 1000;
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
            else if (selection == 6) // hv 3dr apm
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

            // re-enable if needed
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
            TXT_voltage.Text = _getVoltage().ToString();
            txt_current.Text = _getCurrent().ToString();
        }

        private void CMB_apmversion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_prefix != "BATT")
                return;
            if (startup)
                return;

            var selection = int.Parse(CMB_HWVersion.Text.Substring(0, 2).Replace(":", ""));

            try
            {
                if (selection == 0)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 0);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 1);
                }
                else if (selection == 1)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 1);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 2);
                }
                else if (selection == 2)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 13);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 12);
                }
                else if (selection == 3)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 100);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 101);
                }
                else if (selection == 4)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 2);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 3);
                }
                else if (selection == 5)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 10);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 11);
                }
                else if (selection == 6)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 10);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), -1);
                }
                else if (selection == 7)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 6);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 7);
                }
                else if (selection == 8)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 14);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 15);
                }
                else if (selection == 9)
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 16);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 17);
                }
                else if (selection == 10)
                {
                    //Pixhawk 6C/Pix32 v6
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("VOLT_PIN"), 8);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, P("CURR_PIN"), 4);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set " + P("????_PIN") + " Failed", Strings.ERROR);
            }
        }

        private void btnCalcVoltage_Click(object sender, EventArgs e)
        {
            TXT_measuredvoltage_Validated(TXT_measuredvoltage, EventArgs.Empty);
        }

        private void btnCalcCurrent_Click(object sender, EventArgs e)
        {
            txt_meascurrent_Validated(txt_meascurrent, EventArgs.Empty);
        }

        private void CHK_speechbattery_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            Settings.Instance["speechbatteryenabled"] = ((CheckBox)sender).Checked.ToString();
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
                if (_prefix == "BATT")
                    MainV2.comPort.setParam(new[] { "AMP_PER_VOLT", P("AMP_PERVOLT"), P("AMP_PERVLT") }, float.Parse(TXT_AMP_PERVLT.Text));
                else
                    MainV2.comPort.setParam(new[] { P("AMP_PERVOL") }, float.Parse(TXT_AMP_PERVLT.Text));
            }
            catch
            {
                CustomMessageBox.Show("Set " + (_prefix == "BATT" ? P("AMP_PERVOLT") : P("AMP_PERVOL")) + " Failed", Strings.ERROR);
            }
        }
    }
}
