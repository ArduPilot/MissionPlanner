using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Swarm.SRB
{
    public partial class Control : Form
    {
        private static Controller ctl = new Controller();

        public Control()
        {
            InitializeComponent();
        }

        private void but_start_Click(object sender, EventArgs e)
        {
            ctl.Stop();

            ctl = new Controller();

            ctl.DG.TakeOffAlt = (float)num_TakeOffAlt.Value;
            ctl.DG.MinOffset = (float)num_minoffset.Value;
            ctl.DG.MaxOffset = (float)num_maxoffset.Value;

            ctl.Start();
        }

        private void but_z_Click(object sender, EventArgs e)
        {
            ctl.DG.CurrentMode = DroneGroup.Mode.z;
        }

        private void but_land_Click(object sender, EventArgs e)
        {
            ctl.DG.CurrentMode = DroneGroup.Mode.LandAlt;
        }

        private void but_stop_Click(object sender, EventArgs e)
        {
            ctl.Stop();
        }
    }
}
