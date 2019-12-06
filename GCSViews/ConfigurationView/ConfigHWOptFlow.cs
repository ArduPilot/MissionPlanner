using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

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


            CHK_enableoptflow.setup(1, 0, "FLOW_ENABLE", MainV2.comPort.MAV.param);

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
                    MainV2.comPort.setParam("FLOW_ENABLE", ((CheckBox) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set FLOW_ENABLE Failed");
            }
        }
    }
}