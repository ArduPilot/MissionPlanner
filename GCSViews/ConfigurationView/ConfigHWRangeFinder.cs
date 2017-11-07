using System;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWRangeFinder : UserControl, IActivate, IDeactivate
    {
        bool startup = true;
        private const float rad2deg = (float) (180/Math.PI);
        private const float deg2rad = (float) (1.0/rad2deg);

        public ConfigHWRangeFinder()
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

            CMB_sonartype.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("RNGFND_TYPE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "RNGFND_TYPE", MainV2.comPort.MAV.param);

            timer1.Start();

            startup = false;
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_dist.Text = MainV2.comPort.MAV.cs.sonarrange.ToString();
            LBL_volt.Text = MainV2.comPort.MAV.cs.sonarvoltage.ToString();
        }

        private void CMB_sonartype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (startup)
                return;

            if (CMB_sonartype.Text == "TeraRangerOne-I2C")
            {
                // set min and max to 20cm - 10m
                MainV2.comPort.setParam("RNGFND_MAX_CM", 100);
                MainV2.comPort.setParam("RNGFND_MIN_CM ", 20);
            }
        }
    }
}