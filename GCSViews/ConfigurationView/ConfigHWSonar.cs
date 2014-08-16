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
        bool startup = false;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWSonar()
        {
            InitializeComponent();

            CMB_sonartype.setup(Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("SONAR_TYPE"), "SONAR_TYPE", MainV2.comPort.MAV.param);
        }

        private void CHK_enablesonar_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["SONAR_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    MainV2.comPort.setParam("SONAR_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set SONAR_ENABLE Failed"); }
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

            timer1.Start();
  
            CHK_enablesonar.setup(1, 0, "SONAR_ENABLE", MainV2.comPort.MAV.param, CMB_sonartype);

            if (MainV2.comPort.MAV.param["SONAR_TYPE"] != null)
            {
                try
                {
                    CMB_sonartype.SelectedIndex = int.Parse(MainV2.comPort.MAV.param["SONAR_TYPE"].ToString());
                }
                catch { }
            }

            startup = false;
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