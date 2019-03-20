using System;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.Wizard
{
    public partial class _6CompassCalib : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _6CompassCalib()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
        }

        void DeselectAll()
        {
            foreach (var ctl in this.panel1.Controls)
            {
                if (ctl.GetType() == typeof (PictureBoxMouseOver))
                {
                    (ctl as PictureBoxMouseOver).selected = false;
                }
            }
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected, Strings.ERROR);
                Wizard.instance.Close();
            }

            try
            {
                MainV2.comPort.MAV.cs.ratesensors = 2;

                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.EXTRA3, MainV2.comPort.MAV.cs.ratesensors);
                MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

                MainV2.comPort.setParam("MAG_ENABLE", 1);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected, Strings.ERROR);
                Wizard.instance.Close();
            }

            MagCalib.DoGUIMagCalib();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=DmsueBS0J3E");
            }
            catch
            {
                CustomMessageBox.Show("Your default http association is not set correctly.");
            }
        }

        int step = 0;
        Vector3 north;
        Vector3 east;
        Vector3 south;
        Vector3 west;

        private void BUT_compassorient_Click(object sender, EventArgs e)
        {
            BUT_compassorient.Text = "Continue";

            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, 10);

            switch (step)
            {
                case 0:
                    label5.Text = "Please face the autopilot north";
                    break;
                case 1:
                    north = new Vector3(MainV2.comPort.MAV.cs.mx, MainV2.comPort.MAV.cs.my, MainV2.comPort.MAV.cs.mz);
                    label5.Text = "Please face the autopilot east";
                    break;
                case 2:
                    east = new Vector3(MainV2.comPort.MAV.cs.mx, MainV2.comPort.MAV.cs.my, MainV2.comPort.MAV.cs.mz);
                    label5.Text = "Please face the autopilot south";
                    break;
                case 3:
                    south = new Vector3(MainV2.comPort.MAV.cs.mx, MainV2.comPort.MAV.cs.my, MainV2.comPort.MAV.cs.mz);
                    label5.Text = "Please face the autopilot west";
                    break;
                case 4:
                    west = new Vector3(MainV2.comPort.MAV.cs.mx, MainV2.comPort.MAV.cs.my, MainV2.comPort.MAV.cs.mz);
                    label5.Text = "Calculating";
                    if (docalc())
                    {
                    }
                    else
                    {
                        label5.Text = "Error, please try again, verify where north is.";
                    }
                    BUT_compassorient.Text = Strings.Start;
                    step = 0;
                    return;
            }
            step++;
        }

        float calcheading(Vector3 mag)
        {
            Matrix3 dcm_matrix = new Matrix3();
            dcm_matrix.from_euler(0, 0, 0);

            // Tilt compensated magnetic field Y component:
            double headY = mag.y*dcm_matrix.c.z - mag.z*dcm_matrix.c.y;

            // Tilt compensated magnetic field X component:
            double headX = mag.x + dcm_matrix.c.x*(headY - mag.x*dcm_matrix.c.x);

            // magnetic heading
            // 6/4/11 - added constrain to keep bad values from ruining DCM Yaw - Jason S.
            double heading = constrain_float((float) Math.Atan2(-headY, headX), -3.15f, 3.15f);

            return (float) ((heading*MathHelper.rad2deg) + 360)%360f;
        }

        float constrain_float(float input, float min, float max)
        {
            if (input > max)
                return max;
            if (input < min)
                return min;
            return input;
        }


        bool docalc()
        {
            try
            {
                //  HIL.Vector3 magoff = new HIL.Vector3((float)MainV2.comPort.MAV.param["COMPASS_OFS_X"], (float)MainV2.comPort.MAV.param["COMPASS_OFS_Y"], (float)MainV2.comPort.MAV.param["COMPASS_OFS_Z"]);
                //north -= magoff;
                //east -= magoff;
                //south -= magoff;
                //west -= magoff;
            }
            catch
            {
            }

            foreach (Rotation item in Enum.GetValues(typeof (Rotation)))
            {
                // copy them, as we dont want to change the originals
                Vector3 northc = new Vector3(north);
                Vector3 eastc = new Vector3(east);
                Vector3 southc = new Vector3(south);
                Vector3 westc = new Vector3(west);

                northc.rotate(item);
                eastc.rotate(item);
                southc.rotate(item);
                westc.rotate(item);

                // test the copies
                if (withinMargin(calcheading(northc), 35, 0))
                {
                    if (withinMargin(calcheading(eastc), 35, 90))
                    {
                        if (withinMargin(calcheading(southc), 35, 180))
                        {
                            if (withinMargin(calcheading(westc), 35, 270))
                            {
                                Console.WriteLine("Rotation " + item.ToString());
                                label5.Text = "Done Rotation: " + item.ToString();
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        bool withinMargin(float value, float margin, float target)
        {
            // to prevent the near 0 issues
            value += 360;
            target += 360;

            Console.WriteLine("{0} = {1} within +-{2}", value%360, target%360, margin);

            if (value >= (target - margin))
            {
                if (value <= (target + margin))
                {
                    return true;
                }
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float target = (MainV2.comPort.MAV.cs.yaw%90);

            if (target > 45)
                target -= 90f;


            label6.Text = target.ToString("0");
        }
    }
}