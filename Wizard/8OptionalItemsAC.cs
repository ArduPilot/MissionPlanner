using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Controls.BackstageView;

namespace MissionPlanner.Wizard
{
    public partial class _8OptionalItemsAC : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _8OptionalItemsAC()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            //
            if (MainV2.comPort.MAV.param.ContainsKey("SONAR_ENABLE"))
            {
                mavlinkCheckBox1.setup(1, 0, "SONAR_ENABLE", MainV2.comPort.MAV.param, mavlinkComboBox1);

                mavlinkComboBox1.setup(Utilities.ParameterMetaDataRepository.GetParameterOptionsInt("SONAR_TYPE"), "SONAR_TYPE", MainV2.comPort.MAV.param);

                mavlinkCheckBox2.setup(1, 0, "FLOW_ENABLE", MainV2.comPort.MAV.param);

                timer1.Start();
            }
            else
            {
                // no sonar - keep going
                Wizard.instance.BeginInvoke((MethodInvoker)delegate
                {
                    Wizard.instance.GoNext(1,false);
                });
            }
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        public int WizardValidate()
        {
            timer1.Stop();

            return 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_dist.Text = MainV2.comPort.MAV.cs.sonarrange.ToString();
            LBL_volt.Text = MainV2.comPort.MAV.cs.sonarvoltage.ToString();
        }
    }
}
