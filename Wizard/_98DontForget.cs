using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _98DontForget : MyUserControl, IWizard, IDeactivate, IActivate
    {
        public _98DontForget()
        {
            InitializeComponent();
        }

        public void Activate()
        {
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(((LinkLabel) sender).Tag.ToString());
            }
            catch (Exception)
            {
                CustomMessageBox.Show("Failed to open the link " + ((LinkLabel) sender).Tag.ToString(), Strings.ERROR,
                    MessageBoxButtons.OK);
            }
        }
    }
}