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
    public partial class ConfigHWOSD : UserControl, IActivate
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWOSD()
        {
            InitializeComponent();
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
        }

        private void BUT_osdrates_Click(object sender, EventArgs e)
        {
            /*
SR0_EXT_STAT,0
SR0_EXTRA1,0
SR0_EXTRA2,0
SR0_EXTRA3,0
SR0_PARAMS,50
SR0_POSITION,0
SR0_RAW_CTRL,0
SR0_RAW_SENS,0
SR0_RC_CHAN,0
    */
            try
            {
                MainV2.comPort.setParam("SR0_EXT_STAT", 2);
                MainV2.comPort.setParam("SR0_EXTRA1", 2);
                MainV2.comPort.setParam("SR0_EXTRA2", 2);
                MainV2.comPort.setParam("SR0_EXTRA3", 2);
                MainV2.comPort.setParam("SR0_POSITION", 2);
                MainV2.comPort.setParam("SR0_RAW_CTRL", 2);
                MainV2.comPort.setParam("SR0_RAW_SENS", 2);
                MainV2.comPort.setParam("SR0_RC_CHAN", 2);

                MainV2.comPort.setParam("SR3_EXT_STAT", 2);
                MainV2.comPort.setParam("SR3_EXTRA1", 2);
                MainV2.comPort.setParam("SR3_EXTRA2", 2);
                MainV2.comPort.setParam("SR3_EXTRA3", 2);
                MainV2.comPort.setParam("SR3_POSITION", 2);
                MainV2.comPort.setParam("SR3_RAW_CTRL", 2);
                MainV2.comPort.setParam("SR3_RAW_SENS", 2);
                MainV2.comPort.setParam("SR3_RC_CHAN", 2);
            }
            catch { CustomMessageBox.Show("Failed to set OSD rates."); }
        }
    }
}