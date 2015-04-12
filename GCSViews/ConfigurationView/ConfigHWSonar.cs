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
    public partial class ConfigHWSonar : UserControl, IActivate, IDeactivate
    {
  
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWSonar()
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

            CMB_sonartype.setup(Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("RNGFND_TYPE", MainV2.comPort.MAV.cs.firmware.ToString()), "RNGFND_TYPE", MainV2.comPort.MAV.param);

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_dist.Text = MainV2.comPort.MAV.cs.sonarrange.ToString();
            LBL_volt.Text = MainV2.comPort.MAV.cs.sonarvoltage.ToString();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }
    }
}