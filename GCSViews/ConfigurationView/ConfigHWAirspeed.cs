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
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWAirspeed : UserControl, IActivate
    {
        bool startup = false;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWAirspeed()
        {
            InitializeComponent();
        }


        private void CHK_enableairspeed_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["ARSPD_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware.ToString(), "Error");
                }
                else
                {
                    MainV2.comPort.setParam("ARSPD_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set ARSPD_ENABLE Failed", "Error"); }
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                this.Enabled = false;
                return;
            }
            else
            {
                this.Enabled = true;
            }

            startup = true;

       
            CHK_airspeeduse.setup(1, 0, "ARSPD_USE", MainV2.comPort.MAV.param);
            CHK_enableairspeed.setup(1, 0, "ARSPD_ENABLE", MainV2.comPort.MAV.param);

            List<KeyValuePair<int, string>> options = new List<KeyValuePair<int, string>>();
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
            options.Add(new KeyValuePair<int, string>(15, "Pixhawk EagleTree or MEAS I2C AS Sensor"));

            mavlinkCheckBoxAirspeed_pin.setup(options, "ARSPD_PIN", MainV2.comPort.MAV.param);
          

            startup = false;
        }

       
    }
}