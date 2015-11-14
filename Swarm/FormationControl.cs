using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.CoordinateSystems;
using ProjNet.Converters;
using MissionPlanner;

namespace MissionPlanner.Swarm
{
    public partial class FormationControl : Form
    {
        Formation SwarmInterface = null;
        bool threadrun = false;

        public FormationControl()
        {
            InitializeComponent();

            SwarmInterface = new Formation();

            bindingSource1.DataSource = MainV2.Comports;

            CMB_mavs.DataSource = bindingSource1;

            updateicons();

            this.MouseWheel += new MouseEventHandler(FollowLeaderControl_MouseWheel);

            MessageBox.Show("this is beta, use at own risk");

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        void FollowLeaderControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                grid1.setScale(grid1.getScale() + 4);
            }
            else
            {
                grid1.setScale(grid1.getScale() - 4);
            }
        }

        void updateicons()
        {
            bindingSource1.ResetBindings(false);

            foreach (var port in MainV2.Comports)
            {
                if (port == SwarmInterface.getLeader())
                {
                    ((Formation) SwarmInterface).setOffsets(port, 0, 0, 0);
                    var vector = SwarmInterface.getOffsets(port);
                    grid1.UpdateIcon(port, (float) vector.x, (float) vector.y, (float) vector.z, false);
                }
                else
                {
                    var vector = SwarmInterface.getOffsets(port);
                    grid1.UpdateIcon(port, (float) vector.x, (float) vector.y, (float) vector.z, true);
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
            if (threadrun == true)
            {
                threadrun = false;
                BUT_Start.Text = Strings.Start;
                return;
            }

            if (SwarmInterface != null)
            {
                new System.Threading.Thread(mainloop) {IsBackground = true}.Start();
                BUT_Start.Text = Strings.Stop;
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

                // 5 hz
                System.Threading.Thread.Sleep(200);
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

                    SwarmInterface.setOffsets(port, (float) (vector.x - vectorlead.x), (float) (vector.y - vectorlead.y),
                        (float) (vector.z - vectorlead.z));
                }

                SwarmInterface.setLeader(MainV2.comPort);
                updateicons();
                BUT_Start.Enabled = true;
                BUT_Updatepos.Enabled = true;
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

            MAVLinkInterface com2 = new MAVLinkInterface();

            com2.BaseStream.PortName = Comms.CommsSerialScan.portinterface.PortName;
            com2.BaseStream.BaudRate = Comms.CommsSerialScan.portinterface.BaudRate;

            com2.Open(true);

            MainV2.Comports.Add(com2);

            // CMB_mavs.DataSource = MainV2.Comports;

            //CMB_mavs.DataSource

            updateicons();

            bindingSource1.ResetBindings(false);
        }

        public HIL.Vector3 getOffsetFromLeader(MAVLinkInterface leader, MAVLinkInterface mav)
        {
            //convert Wgs84ConversionInfo to utm
            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

            GeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

            int utmzone = (int) ((leader.MAV.cs.lng - -186.0)/6.0);

            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                leader.MAV.cs.lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            double[] masterpll = {leader.MAV.cs.lng, leader.MAV.cs.lat};

            // get leader utm coords
            double[] masterutm = trans.MathTransform.Transform(masterpll);

            double[] mavpll = {mav.MAV.cs.lng, mav.MAV.cs.lat};

            //getLeader follower utm coords
            double[] mavutm = trans.MathTransform.Transform(mavpll);

            return new HIL.Vector3(masterutm[1] - mavutm[1], masterutm[0] - mavutm[0], 0);
        }

        private void grid1_UpdateOffsets(MAVLinkInterface mav, float x, float y, float z, Grid.icon ico)
        {
            if (mav == SwarmInterface.Leader)
            {
                CustomMessageBox.Show("Can not move Leader");
                ico.z = 0;
            }
            else
            {
                ((Formation) SwarmInterface).setOffsets(mav, x, y, z);
            }
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
        }

        private void BUT_Updatepos_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                port.MAV.cs.UpdateCurrentSettings(null, true, port);

                if (port == SwarmInterface.Leader)
                    continue;

                HIL.Vector3 offset = getOffsetFromLeader(((Formation) SwarmInterface).getLeader(), port);

                if (Math.Abs(offset.x) < 200 && Math.Abs(offset.y) < 200)
                {
                    grid1.UpdateIcon(port, (float) offset.x, (float) offset.y, (float) offset.z, true);
                    //((Formation)SwarmInterface).setOffsets(port, offset.x, offset.y, offset.z);
                }
            }
        }

        private void timer_status_Tick(object sender, EventArgs e)
        {
            // clean up old
            foreach (Control ctl in PNL_status.Controls)
            {
                if (!MainV2.Comports.Contains((MAVLinkInterface) ctl.Tag))
                {
                    ctl.Dispose();
                }
            }

            // setup new
            foreach (var port in MainV2.Comports)
            {
                bool exists = false;
                foreach (Control ctl in PNL_status.Controls)
                {
                    if (ctl is Status && ctl.Tag == port)
                    {
                        exists = true;
                        ((Status) ctl).GPS.Text = port.MAV.cs.gpsstatus >= 3 ? "OK" : "Bad";
                        ((Status) ctl).Armed.Text = port.MAV.cs.armed.ToString();
                        ((Status) ctl).Mode.Text = port.MAV.cs.mode;
                        ((Status) ctl).MAV.Text = port.ToString();
                        ((Status) ctl).Guided.Text = port.MAV.GuidedMode.x + "," + port.MAV.GuidedMode.y + "," +
                                                     port.MAV.GuidedMode.z;
                    }
                }

                if (!exists)
                {
                    Status newstatus = new Status();
                    newstatus.Tag = port;
                    PNL_status.Controls.Add(newstatus);
                }
            }
        }
    }
}