using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _9RadioCalibration : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _9RadioCalibration()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            configRadioInput1.Activate();
        }

        public void Deactivate()
        {
            configRadioInput1.Deactivate();
        }

        public int WizardValidate()
        {
            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }

        private void BUT_continue_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
            BUT_continue.Visible = false;
            configRadioInput1.Visible = true;
        }
    }
}