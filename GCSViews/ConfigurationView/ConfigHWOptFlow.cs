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
    public partial class ConfigHWOptFlow : UserControl, IActivate
    {
        bool startup = false;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWOptFlow()
        {
            InitializeComponent();
        }

        private void CHK_enableoptflow_CheckedChanged(object sender, EventArgs e)
        {

            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["FLOW_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("FLOW_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set FLOW_ENABLE Failed"); }
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

           
            CHK_enableoptflow.setup(1,0,"FLOW_ENABLE", MainV2.comPort.MAV.param);
      
            startup = false;
        }

    }
}