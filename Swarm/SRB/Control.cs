using System;
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
            ctl.DG.ZSpeed = (float)num_zspeed.Value;

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = "BasePos: " + ctl.DG.GetBasePosition()?.ToString();
            label5.Text = "BaseVel: " + ctl.DG.GetBaseVelocity()?.ToString();

            label6.Text = "Mode: "+ctl.DG.CurrentMode.ToString();

            label7.Text = "BaseHeading: " + ctl.DG.GetBasePosition()?.Heading.ToString();
        }

        private void num_TakeOffAlt_ValueChanged(object sender, EventArgs e)
        {
            ctl.DG.TakeOffAlt = (float)num_TakeOffAlt.Value;
        }

        private void num_minoffset_ValueChanged(object sender, EventArgs e)
        {
            ctl.DG.MinOffset = (float)num_minoffset.Value;
        }

        private void num_maxoffset_ValueChanged(object sender, EventArgs e)
        {
            ctl.DG.MaxOffset = (float)num_maxoffset.Value;
        }

        private void num_zspeed_ValueChanged(object sender, EventArgs e)
        {
            ctl.DG.ZSpeed = (float)num_zspeed.Value;
        }
    }
}
