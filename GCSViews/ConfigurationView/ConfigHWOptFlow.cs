using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWOptFlow : MyUserControl, IActivate
    {
        private bool startup;

        public ConfigHWOptFlow()
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

            // FLOW_ENABLE only used on older firmwares - hide if not found
            CHK_enableoptflow.setup(1, 0, "FLOW_ENABLE", MainV2.comPort.MAV.param);

            if (CHK_enableoptflow.Enabled == false) {
                CHK_enableoptflow.Hide();
            }
            else
            {
                // use legacy panel
                DROP_optflowtype.Hide();
                mavlinkNumericUpDown_yaw.Hide();
                mavlinkNumericUpDownFX.Hide();
                mavlinkNumericUpDownFY.Hide();
                mavlinkNumericUpDownX.Hide();
                mavlinkNumericUpDownY.Hide();
                mavlinkNumericUpDownZ.Hide();
                startup = false;
                return;
            }

            // Doing new-style panel from here onwards

            DROP_optflowtype.setup(ParameterMetaDataRepository.GetParameterOptionsInt("FLOW_TYPE",
                MainV2.comPort.MAV.cs.firmware.ToString()), "FLOW_TYPE", MainV2.comPort.MAV.param);

            mavlinkNumericUpDown_yaw.setup(-179000, 180000, 100, 1, "FLOW_ORIENT_YAW", MainV2.comPort.MAV.param);
            mavlinkNumericUpDown_yaw.Maximum = 180;
            mavlinkNumericUpDown_yaw.Minimum = -179;
            mavlinkNumericUpDown_yaw.Increment = 1;

            mavlinkNumericUpDownFX.setup(-200, 200, 1, 1, "FLOW_FXSCALER", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownFY.setup(-200, 200, 1, 1, "FLOW_FYSCALER", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownX.setup(-5, 5, 1, 0.01F, "FLOW_POS_X", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownY.setup(-5, 5, 1, 0.01F, "FLOW_POS_Y", MainV2.comPort.MAV.param);
            mavlinkNumericUpDownZ.setup(-5, 5, 1, 0.01F, "FLOW_POS_Z", MainV2.comPort.MAV.param);

            mavlinkNumericUpDownHGTOVR.setup(0, 2, 1, 0.01F, "FLOW_HGT_OVR", MainV2.comPort.MAV.param);

            // hide FLOW_HGT_OVR if not rover firmware
            if (!MainV2.comPort.MAV.VersionString.Contains("ArduRover"))
            {
                mavlinkNumericUpDownHGTOVR.Hide();
                label15.Hide();
                label16.Hide();
            }

            startup = false;
        }

        private void CHK_enableoptflow_CheckedChanged(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["FLOW_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "FLOW_ENABLE", ((CheckBox)sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set FLOW_ENABLE Failed");
            }
        }

        private void DROP_optflowtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            // hide FLOW_HGT_OVR if not rover firmware
            if (!MainV2.comPort.MAV.VersionString.Contains("ArduRover"))
            {
                mavlinkNumericUpDownHGTOVR.Hide();
                label15.Hide();
                label16.Hide();
            }
            else
            {
                mavlinkNumericUpDownHGTOVR.Show();
                label15.Show();
                label16.Show();
            }
        }
    }
}