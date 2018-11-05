using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWParachute : MyUserControl, IActivate
    {
        private bool startup = true;

        public ConfigHWParachute()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            var copy = new Dictionary<string, double>((Dictionary<string, double>) MainV2.comPort.MAV.param);

            foreach (string item in copy.Keys)
            {
                if (item.EndsWith("_FUNCTION") && MainV2.comPort.MAV.param[item].ToString() == "27")
                {
                    mavlinkComboBoxServoNum.Text = item.Replace("_FUNCTION", "");
                    break;
                }
            }

            startup = false;

            mavlinkCheckBoxEnable.setup(1, 0, "CHUTE_ENABLED", MainV2.comPort.MAV.param);

            var options = new List<KeyValuePair<int, string>>();
            options.Add(new KeyValuePair<int, string>(0, "First Relay"));
            options.Add(new KeyValuePair<int, string>(1, "Second Relay"));
            options.Add(new KeyValuePair<int, string>(2, "Third Relay"));
            options.Add(new KeyValuePair<int, string>(3, "Fourth Relay"));
            options.Add(new KeyValuePair<int, string>(10, "Servo"));
            mavlinkComboBoxType.setup(options, "CHUTE_TYPE", MainV2.comPort.MAV.param);

            mavlinkNumericUpDownResting.setup(1000, 2000, 1, 1, "CHUTE_SERVO_OFF", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownDeploy.setup(1000, 2000, 1, 1, "CHUTE_SERVO_ON", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownMinAlt.setup(0, 32000, 1, 1, "CHUTE_ALT_MIN", MainV2.comPort.MAV.param);
        }

        private void mavlinkComboBoxServoNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            ensureDisabled((ComboBox) sender, 27, mavlinkComboBoxServoNum.Text);

            MainV2.comPort.setParam(mavlinkComboBoxServoNum.Text + "_FUNCTION", 27);
        }

        private void ensureDisabled(ComboBox cmb, int number, string exclude = "")
        {
            foreach (string item in cmb.Items)
            {
                if (MainV2.comPort.MAV.param.ContainsKey(item + "_FUNCTION"))
                {
                    var ans = (float) MainV2.comPort.MAV.param[item + "_FUNCTION"];

                    if (item == exclude)
                        continue;

                    if (ans == number)
                    {
                        MainV2.comPort.setParam(item + "_FUNCTION", 0);
                    }
                }
            }
        }
    }
}