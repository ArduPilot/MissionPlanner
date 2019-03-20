using System;
using System.Windows.Forms;

namespace MissionPlanner.Swarm.FollowLeader
{
    public partial class Control : Form
    {
        static DroneGroup DG = new DroneGroup();
        static bool threadrun;

        public Control()
        {
            InitializeComponent();
        }

        private void but_master_Click(object sender, EventArgs e)
        {
            DG.groundmaster = MainV2.comPort.MAV;

            DG.Drones.Clear();

            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    DG.Drones.Add(new Drone() { MavState = MAV});
                }
            }
        }

        private void but_arm_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    MAV.parent.doARM(MAV.sysid, MAV.compid, true);
                }
            }
        }

        private void but_takeoff_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    MAV.parent.setMode(MAV.sysid, MAV.compid, "GUIDED");

                    MAV.parent.doCommand(MAV.sysid, MAV.compid, MAVLink.MAV_CMD.TAKEOFF, 0, 0, 0, 0, 0, 0, 5);
                }
            }
        }

        private void but_auto_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    MAV.parent.setMode(MAV.sysid, MAV.compid, "AUTO");
                }
            }
        }

        private void but_start_Click(object sender, EventArgs e)
        {
            if (threadrun == true)
            {
                threadrun = false;
                but_start.Text = Strings.Start;
                return;
            }

            //if (SwarmInterface != null)
            {
                new System.Threading.Thread(mainloop) { IsBackground = true }.Start();
                but_start.Text = Strings.Stop;
            }
        }

        private void mainloop()
        {
            threadrun = true;

            while (threadrun)
            {
                DG.UpdatePositions();

                System.Threading.Thread.Sleep(100);
            }
        }

        private void but_guided_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    MAV.parent.setMode(MAV.sysid, MAV.compid, "GUIDED");
                }
            }
        }

        private void but_navguided_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    MAV.parent.doCommand(MAV.sysid, MAV.compid, MAVLink.MAV_CMD.GUIDED_ENABLE, 1, 0, 0, 0, 0, 0, 0);
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DG.Seperation = (double)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            DG.Lead = (double)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            DG.Altitude = (double)numericUpDown3.Value;
        }

        private void but_airmaster_Click(object sender, EventArgs e)
        {
            DG.airmaster = MainV2.comPort.MAV;

            DG.Drones.Clear();

            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    DG.Drones.Add(new Drone() { MavState = MAV });
                }
            }
        }
    }
}
