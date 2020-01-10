using MissionPlanner.Controls;
using System.Windows.Forms;
using MissionPlanner.Warnings;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigAdvanced : MyUserControl, IActivate
    {
        public ConfigAdvanced()
        {
            InitializeComponent();
        }

        public void Activate()
        {
        }

        private void but_warningmanager_Click(object sender, System.EventArgs e)
        {
            new WarningsManager().Show();
        }

        private void but_mavinspector_Click(object sender, System.EventArgs e)
        {
            new MAVLinkInspector(MainV2.comPort).Show();
        }

        private void BUT_outputMavlink_Click(object sender, System.EventArgs e)
        {
            new SerialOutputPass().Show();
        }

        private void but_signkey_Click(object sender, System.EventArgs e)
        {
            new AuthKeys().Show();
        }

        private void but_proximity_Click(object sender, System.EventArgs e)
        {
            new ProximityControl(MainV2.comPort.MAV).Show();
        }

        private void BUT_outputnmea_Click(object sender, System.EventArgs e)
        {
            new SerialOutputNMEA().Show();
        }
    }
}