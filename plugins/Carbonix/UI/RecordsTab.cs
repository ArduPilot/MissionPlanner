using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Plugin;

namespace Carbonix
{
    public partial class RecordsTab : UserControl
    {
        private readonly PluginHost Host;
        public RecordsTab(PluginHost Host, List<string> pilots)
        {
            this.Host = Host;
            
            InitializeComponent();

            // Populate the pilot list
            foreach (var pilot in pilots)
            {
                cmb_pic.Items.Add(pilot);
                cmb_gso.Items.Add(pilot);
            }
        }

        private void RecordsTab_VisibleChanged(object sender, EventArgs e)
        {
            bool enabled = !Host.cs.armed || !Host.comPort.BaseStream.IsOpen;
            cmb_pic.Enabled = enabled;
            cmb_gso.Enabled = enabled;
            num_avbatid.Enabled = enabled;
            num_vtolbatid.Enabled = enabled;
        }
    }
}
