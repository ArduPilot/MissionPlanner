using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArdupilotMega.Swarm
{
    public partial class FollowLeaderControl : Form
    {
        FollowLeader SwarmInterface = null;
        bool threadrun = false;

        public FollowLeaderControl()
        {
            InitializeComponent();

            SwarmInterface = new FollowLeader();

            bindingSource1.DataSource = MainV2.Comports;

            CMB_mavs.DataSource = bindingSource1;

            updateicons();

            this.MouseWheel += new MouseEventHandler(FollowLeaderControl_MouseWheel);
        }

        void FollowLeaderControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                grid1.setScale(grid1.getScale() + 2);
            }
            else
            {
                grid1.setScale(grid1.getScale() - 2);
            }
        }

        void updateicons()
        {
            bindingSource1.ResetBindings(false);

            foreach (var port in MainV2.Comports)
            {
                if (port == SwarmInterface.getLeader())
                {
                    ((FollowLeader)SwarmInterface).setOffsets(port, 0, 0, 0);
                    var vector = SwarmInterface.getOffsets(port);
                    grid1.UpdateIcon(port, (float)vector.x, (float)vector.y, (float)vector.z, false);
                }
                else
                {
                    var vector = SwarmInterface.getOffsets(port);
                    grid1.UpdateIcon(port, (float)vector.x, (float)vector.y, (float)vector.z, true);
                }
            }
            grid1.Invalidate();
        }

        private void CMB_mavs_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                if (port.ToString() == CMB_mavs.Text)
                {
                    MainV2.comPort = port;
                }
            }
        }

        private void BUT_Start_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                new System.Threading.Thread(mainloop) { IsBackground = true }.Start();
            }
        }

        void mainloop()
        {
            threadrun = true;

            while (threadrun)
            {
                // update leader pos
                SwarmInterface.Update();

                // update other mavs
                SwarmInterface.SendCommand();

                // 10 hz
                System.Threading.Thread.Sleep(100);
            }
        }

        private void BUT_Arm_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Arm();
            }
        }

        private void BUT_Disarm_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Disarm();
            }
        }

        private void BUT_Takeoff_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Takeoff();
            }
        }

        private void BUT_Land_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Land();
            }
        }

        private void BUT_leader_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                var vectorlead = SwarmInterface.getOffsets(MainV2.comPort);

                foreach (var port in MainV2.Comports)
                {
                    var vector = SwarmInterface.getOffsets(port);

                    SwarmInterface.setOffsets(port,(float)( vector.x - vectorlead.x),(float)(vector.y - vectorlead.y),(float)(vector.z - vectorlead.z));
                }

                SwarmInterface.setLeader(MainV2.comPort);
                updateicons();
            }
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            Comms.CommsSerialScan.Scan(false);

            DateTime deadline = DateTime.Now.AddSeconds(50);

            while (Comms.CommsSerialScan.foundport == false)
            {
                System.Threading.Thread.Sleep(100);

                if (DateTime.Now > deadline)
                {
                    CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                    return;
                }
            }

            MAVLink com2 = new MAVLink();

            com2.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
            com2.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

            com2.Open(true);

            MainV2.Comports.Add(com2);

           // CMB_mavs.DataSource = MainV2.Comports;

            //CMB_mavs.DataSource

            updateicons();

            bindingSource1.ResetBindings(false);
        }

        private void grid1_UpdateOffsets(MAVLink mav, float x, float y, float z, Grid.icon ico)
        {
            if (mav == SwarmInterface.Leader)
            {
                CustomMessageBox.Show("Can not move Leader");
                ico.z = 0;
            }
            else
            {
                ((FollowLeader)SwarmInterface).setOffsets(mav, x, y, z);
            }
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
        }
    }
}
