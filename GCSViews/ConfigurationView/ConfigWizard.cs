using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
            Wizard.Wizard cfg = new Wizard.Wizard();

            cfg.ShowDialog(this);
        }
    }
}
