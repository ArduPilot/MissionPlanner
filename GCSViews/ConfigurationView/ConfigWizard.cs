using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigWizard : MyUserControl, IActivate
    {
        public ConfigWizard()
        {
            InitializeComponent();
            PIC_wizard.Image = MainV2.displayicons.wizard;
        }

        public void Activate()
        {
            PIC_wizard_Click(null, null);
        }

        private void PIC_wizard_Click(object sender, EventArgs e)
        {
            var cfg = new Wizard.Wizard();

            cfg.ShowDialog(this);
        }
    }
}