using System;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace MissionPlanner.Swarm.WaypointLeader
{
    public partial class WPControl : Form
    {
        DroneGroup DG = new DroneGroup();
        bool threadrun;

        public WPControl()
        {
            InitializeComponent();

            zedGraphControl1.GraphPane.AddCurve("Path", DG.path_to_fly, Color.Red, SymbolType.None);

            zedGraphControl1.GraphPane.XAxis.Title.Text = "Distance";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "Altitude";

            DG.Drones.Clear();
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

            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    if (MAV.cs.armed && MAV.cs.alt > 1)
                    {
                        var result = CustomMessageBox.Show("There appears to be a drone in the air at the moment. Are you sure you want to continue?", "continue", MessageBoxButtons.YesNo);
                        if (result == (int)DialogResult.Yes)
                            break;
                        return;
                    }
                }
            }

            zedGraphControl1.AxisChange();

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            txt_mode.Text = DG.CurrentMode.ToString();

            // clean up old
            foreach (Control ctl in PNL_status.Controls)
            {
                var found = false;
                foreach (var port in MainV2.Comports)
                {
                    foreach (var MAV in port.MAVlist)
                    {
                        if (ctl.Tag == MAV)
                        {
                            found = true;
                        }
                    }
                }

                if(!found)
                    ctl.Dispose();
            }

            // setup new
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    bool exists = false;
                    foreach (Control ctl in PNL_status.Controls)
                    {
                        if (ctl is Status && ctl.Tag == MAV)
                        {
                            exists = true;
                            if (MAV.cs.gpsstatus < 3)
                            {
                                ((Status) ctl).GPS.Text = "Bad";
                            }
                            else if (MAV.cs.gpsstatus >= 3)
                            {
                                ((Status) ctl).GPS.Text = "OK " + Math.Max(MAV.cs.gpsstatus, MAV.cs.gpsstatus2);
                            }
                            ((Status) ctl).Armed.Text = MAV.cs.armed.ToString();
                            ((Status) ctl).Mode.Text = MAV.cs.mode;
                            ((Status) ctl).MAV.Text = String.Format("MAV {0}-{1}",MAV.sysid,MAV.compid);
                            ((Status) ctl).Guided.Text = MAV.GuidedMode.x + "," + MAV.GuidedMode.y + "," +
                                                         MAV.GuidedMode.z;
                            ((Status)ctl).Location1.Text = MAV.cs.lat.ToString("0.00000") + "," + MAV.cs.lng.ToString("0.00000") + "," +
                                                      MAV.cs.alt;
                            ((Status)ctl).Speed.Text = MAV.cs.groundspeed.ToString("0.00");

                            if (MAV == DG.airmaster)
                                ((Status) ctl).MAV.Text = String.Format("MAV {0}-{1} airmaster", MAV.sysid, MAV.compid);

                            if (MAV == DG.groundmaster)
                                ((Status)ctl).MAV.Text = String.Format("MAV {0}-{1} groundmaster", MAV.sysid, MAV.compid);

                            if (MAV == DG.airmaster || MAV == DG.groundmaster)
                            {
                                ((Status) ctl).ForeColor = Color.Red;
                            }
                            else
                            {
                                ((Status) ctl).ForeColor = Color.Black;
                            }
                        }
                    }

                    if (!exists)
                    {
                        Status newstatus = new Status();
                        newstatus.Tag = MAV;
                        PNL_status.Controls.Add(newstatus);
                    }
                }
            }

            foreach (var drone in DG.Drones)
            {
                // check if curve exists
                if (zedGraphControl1.GraphPane.CurveList["MAV " + drone.MavState.sysid.ToString()] == null)
                {
                    if (drone.Location != null)
                    {
                        // create the curve
                        zedGraphControl1.GraphPane.CurveList.Add(
                            new LineItem("MAV " + drone.MavState.sysid.ToString(),
                                new PointPairList(new[] {(double) drone.PathIndex}, new[] {drone.Location.Alt}),
                                colours[zedGraphControl1.GraphPane.CurveList.Count % colours.Length],
                                SymbolType.Triangle));

                        zedGraphControl1.ZoomOutAll(zedGraphControl1.GraphPane);
                    }
                }
                else
                {
                    // update the curve
                    var curve = zedGraphControl1.GraphPane.CurveList["MAV " + drone.MavState.sysid.ToString()];
                    curve.Clear();
                    try
                    {
                        curve.AddPoint((double)(drone.PathIndex * 0.1), drone.Location.Alt);
                    } catch { }
                }
            }

            zedGraphControl1.Invalidate();
        }

       Color[] colours = new Color[]
       {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Orange,
            Color.Yellow,
            Color.Violet,
            Color.Pink,
            Color.Teal,
            Color.Wheat,
            Color.Silver,
            Color.Purple,
            Color.Aqua,
            Color.Brown,
            Color.WhiteSmoke
       };

        private void but_resetmode_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    if (MAV.cs.armed && MAV.cs.alt > 1)
                    {
                        var result = CustomMessageBox.Show("There appears to be a drone in the air at the moment. Are you sure you want to continue?", "continue", MessageBoxButtons.YesNo);
                        if (result == (int)DialogResult.Yes)
                            break;
                        return;
                    }
                }
            }

            DG.CurrentMode = DroneGroup.Mode.idle;
        }

        private void but_rth_Click(object sender, EventArgs e)
        {
            DG.CurrentMode = DroneGroup.Mode.RTH;
        }

        private void num_useroffline_ValueChanged(object sender, EventArgs e)
        {
            DG.OffPathTrigger = (double)num_useroffline.Value;
        }

        private void chk_V_CheckedChanged(object sender, EventArgs e)
        {
            DG.V = chk_V.Checked;
        }

        private void num_rtl_alt_ValueChanged(object sender, EventArgs e)
        {
            DG.Takeoff_Land_alt_sep = (double)num_rtl_alt.Value;
        }

        private void chk_alt_interleave_CheckedChanged(object sender, EventArgs e)
        {
            DG.AltInterleave = chk_alt_interleave.Checked;
        }

        private void WPControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
            System.Threading.Thread.Sleep(500);
        }

        private void but_setmoderltland_Click(object sender, EventArgs e)
        {
            DG.CurrentMode = DroneGroup.Mode.LandAlt;
        }

        private void num_wpnav_accel_ValueChanged(object sender, EventArgs e)
        {
            DG.WPNAV_ACCEL = num_wpnav_accel.Value;
        }
    }
}
