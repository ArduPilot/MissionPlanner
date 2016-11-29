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
    public partial class _13GeoFence : MyUserControl, IWizard, IDeactivate, IActivate
    {
        public _13GeoFence()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (MainV2.comPort.MAV.aptype != MAVLink.MAV_TYPE.FIXED_WING &&
                MainV2.comPort.MAV.aptype != MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                configAC_Fence1.Activate();
            }
            else
            {
                // no sonar - keep going
                Wizard.instance.BeginInvoke((MethodInvoker) delegate { Wizard.instance.GoNext(1, false); });
            }
        }

        public void Deactivate()
        {
        }

        public int WizardValidate()
        {
            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }
    }
}