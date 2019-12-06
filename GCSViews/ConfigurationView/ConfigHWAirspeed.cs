using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWAirspeed : MyUserControl, IActivate
    {
        private bool startup;

        public ConfigHWAirspeed()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

            startup = true;

            if (!MainV2.comPort.MAV.param.ContainsKey("ARSPD_USE"))
                CHK_airspeeduse.Visible = false;

            if (!MainV2.comPort.MAV.param.ContainsKey("ARSPD_ENABLE"))
                CHK_enableairspeed.Visible = false;

            CHK_airspeeduse.setup(1, 0, "ARSPD_USE", MainV2.comPort.MAV.param);
            CHK_enableairspeed.setup(1, 0, "ARSPD_ENABLE", MainV2.comPort.MAV.param);

            mavlinkComboBoxARSPD_TYPE.setup(ParameterMetaDataRepository.GetParameterOptionsInt("ARSPD_TYPE",
                MainV2.comPort.MAV.cs.firmware.ToString()), "ARSPD_TYPE", MainV2.comPort.MAV.param);

            var options = new List<KeyValuePair<int, string>>();
            options.Add(new KeyValuePair<int, string>(0, "APM 2 analog pin 0"));
            options.Add(new KeyValuePair<int, string>(1, "APM 2 analog pin 1"));
            options.Add(new KeyValuePair<int, string>(2, "APM 2 analog pin 2"));
            options.Add(new KeyValuePair<int, string>(3, "APM 2 analog pin 3"));
            options.Add(new KeyValuePair<int, string>(4, "APM 2 analog pin 4"));
            options.Add(new KeyValuePair<int, string>(5, "APM 2 analog pin 5"));
            options.Add(new KeyValuePair<int, string>(6, "APM 2 analog pin 6"));
            options.Add(new KeyValuePair<int, string>(7, "APM 2 analog pin 7"));
            options.Add(new KeyValuePair<int, string>(8, "APM 2 analog pin 8"));
            options.Add(new KeyValuePair<int, string>(9, "APM 2 analog pin 9"));

            options.Add(new KeyValuePair<int, string>(64, "APM 1 AS Port"));

            options.Add(new KeyValuePair<int, string>(11, "PX4 Analog AS Port"));
            options.Add(new KeyValuePair<int, string>(15, "Pixhawk Analog AS Port"));
            options.Add(new KeyValuePair<int, string>(65, "PX4/Pixhawk EagleTree or MEAS I2C AS Sensor"));

            mavlinkCheckBoxAirspeed_pin.setup(options, "ARSPD_PIN", MainV2.comPort.MAV.param);


            startup = false;
        }

        private void CHK_enableairspeed_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["ARSPD_ENABLE"] == null)
                {
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    MainV2.comPort.setParam("ARSPD_ENABLE", ((CheckBox) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "ARSPD_ENABLE"), Strings.ERROR);
            }
        }
    }
}