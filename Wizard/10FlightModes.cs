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
    public partial class _10FlightModes : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _10FlightModes()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            configFlightModes1.Activate();
        }

        public void Deactivate()
        {
            configFlightModes1.Deactivate();
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