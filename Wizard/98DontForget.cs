using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.Wizard
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(((LinkLabel)sender).Tag.ToString());
        }
    }
}
