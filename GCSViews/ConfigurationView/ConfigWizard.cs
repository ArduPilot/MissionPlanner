using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigWizard : UserControl, IActivate
    {
        public ConfigWizard()
        {
            InitializeComponent();
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