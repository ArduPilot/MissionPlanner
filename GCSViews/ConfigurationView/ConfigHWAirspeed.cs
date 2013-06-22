using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
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
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("ARSPD_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set ARSPD_ENABLE Failed"); }
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
          

            startup = false;
        }

       
    }
}