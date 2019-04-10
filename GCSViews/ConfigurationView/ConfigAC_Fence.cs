using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAC_Fence : MyUserControl, IActivate
    {
        public ConfigAC_Fence()
        {
            InitializeComponent();

            label6maxalt.Text += "[" + CurrentState.DistanceUnit + "]";
            label7maxrad.Text += "[" + CurrentState.DistanceUnit + "]";
            label2rtlalt.Text += "[" + CurrentState.DistanceUnit + "]";
        }

        public void Activate()
        {
            mavlinkCheckBox1.setup(1, 0, "FENCE_ENABLE", MainV2.comPort.MAV.param);

            mavlinkComboBox1.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("FENCE_TYPE",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "FENCE_TYPE", MainV2.comPort.MAV.param);


            mavlinkComboBox2.setup(
                ParameterMetaDataRepository.GetParameterOptionsInt("FENCE_ACTION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "FENCE_ACTION", MainV2.comPort.MAV.param);


            // 3
            mavlinkNumericUpDown1.setup(10, 1000, (float) CurrentState.fromDistDisplayUnit(1), 1, "FENCE_ALT_MAX",
                MainV2.comPort.MAV.param);

            mavlinkNumericUpDown2.setup(30, 65536, (float) CurrentState.fromDistDisplayUnit(1), 1, "FENCE_RADIUS",
                MainV2.comPort.MAV.param);

            mavlinkNumericUpDown3.setup(1, 500, (float) CurrentState.fromDistDisplayUnit(100), 1, "RTL_ALT",
                MainV2.comPort.MAV.param);
        }
    }
}