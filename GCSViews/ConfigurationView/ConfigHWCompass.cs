using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls.BackstageView;
using MissionPlanner.Controls;
using netDxf;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass : UserControl, IActivate
    {
        bool startup = false;
        double[] ans;
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWCompass()
        {
            InitializeComponent();
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("Please click ok and move the apm around all axises");

            ProgressReporterSphere prd = new ProgressReporterSphere();

            prd.btnCancel.Text = "Done";

            Utilities.ThemeManager.ApplyThemeTo(prd);

            prd.DoWork += prd_DoWork;

            prd.RunBackgroundOperationAsync();

            if (ans != null)
                MagCalib.SaveOffsets(ans);
        }

        void prd_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            // list of x,y,z 's
            List<Tuple<float, float, float>> data = new List<Tuple<float, float, float>>();

            // backup current rate and set to 10 hz
            byte backupratesens = MainV2.comPort.MAV.cs.ratesensors;
            MainV2.comPort.MAV.cs.ratesensors = 10;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors); // mag captures at 10 hz

            float oldmx = 0;
            float oldmy = 0;
            float oldmz = 0;

            ((ProgressReporterSphere)sender).sphere1.Clear();

            while (true)
            {
                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(-1, "Got " + data.Count + " Samples");

                if (e.CancelRequested)
                {
                    // restore old sensor rate
                    MainV2.comPort.MAV.cs.ratesensors = backupratesens;
                    MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

                    e.CancelAcknowledged = false;
                    e.CancelRequested = false;
                    break;
                }

                if (oldmx != MainV2.comPort.MAV.cs.mx &&
                    oldmy != MainV2.comPort.MAV.cs.my &&
                    oldmz != MainV2.comPort.MAV.cs.mz)
                {
                    data.Add(new Tuple<float, float, float>(
                        MainV2.comPort.MAV.cs.mx - (float)MainV2.comPort.MAV.cs.mag_ofs_x,
                        MainV2.comPort.MAV.cs.my - (float)MainV2.comPort.MAV.cs.mag_ofs_y,
                        MainV2.comPort.MAV.cs.mz - (float)MainV2.comPort.MAV.cs.mag_ofs_z));

                    oldmx = MainV2.comPort.MAV.cs.mx;
                    oldmy = MainV2.comPort.MAV.cs.my;
                    oldmz = MainV2.comPort.MAV.cs.mz;

                    ((ProgressReporterSphere)sender).sphere1.AddPoint(new OpenTK.Vector3(oldmx, oldmy, oldmz));
                }
            }

            // restore old sensor rate
            MainV2.comPort.MAV.cs.ratesensors = backupratesens;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

            if (data.Count < 10)
            {
                e.ErrorMessage = "Log does not contain enough data";
                ans = null;
                return;
            }

            bool ellipsoid = false;

            if (MainV2.comPort.MAV.param.ContainsKey("MAG_DIA"))
            {
                ellipsoid = true;
            }

            ans = MagCalib.LeastSq(data, ellipsoid);

            //find the mean radius
            HIL.Vector3 centre = new HIL.Vector3((float)-ans[0], (float)-ans[1], (float)-ans[2]);
            HIL.Vector3 point;
            float radius = 0;
            for(int i = 0; i < data.Count; i++) {
                point = new HIL.Vector3(data[i].Item1, data[i].Item2, data[i].Item3);
                radius += (float)(point - centre).length();
            }
            radius /= data.Count;

            //test that we can find one point near a set of points all around the sphere surface
            int factor = 2; // 4 point check 2x2
            float max_distance = radius / 3; //pretty generouse
            for (int j = 0; j < factor; j++)
            {
                double theta = (Math.PI * (j+0.5)) / factor;

                for(int i = 0; i < factor; i++)
                {
                    double phi = (2 * Math.PI * i) / factor;

                    HIL.Vector3 point_sphere = new HIL.Vector3(
                        (float)(Math.Sin(theta) * Math.Cos(phi) * radius),
                        (float)(Math.Sin(theta) * Math.Sin(phi) * radius),
                        (float)(Math.Cos(theta) * radius)) + centre;

                    Console.WriteLine("{0} {1}",theta * rad2deg,phi* rad2deg);

                    bool found = false;
                    for(int k = 0; k < data.Count; k++)
                    {
                        point = new HIL.Vector3(data[k].Item1, data[k].Item2, data[k].Item3);
                        double d = (point_sphere - point).length();
                        if (d < max_distance)
                        {
                            found = true;
                            break;
                        }
                    }
                    if(!found)
                    {
                        e.ErrorMessage = "Data missing for some directions";
                        ans = null;
                        return;
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("http://www.ngdc.noaa.gov/geomagmodels/Declination.jsp");
                System.Diagnostics.Process.Start("http://www.magnetic-declination.com/");
            }
            catch { CustomMessageBox.Show("Webpage open failed... do you have a virus?\nhttp://www.magnetic-declination.com/", "Mag"); }
        }

        private void TXT_declination_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_declination_deg.Text, out ans);
        }

        private void TXT_declination_Validated(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["COMPASS_DEC"] == null)
                {
                    CustomMessageBox.Show("Not Available", "Error");
                }
                else
                {
                    float dec = 0.0f;
                    try
                    {
                        string deg = TXT_declination_deg.Text;

                        string min = TXT_declination_min.Text;

                        dec = float.Parse(deg) + (float.Parse(min) / 60);

                        MainV2.comPort.setParam("COMPASS_DEC", dec * deg2rad);
                    }
                    catch { CustomMessageBox.Show("Invalid input!", "Error"); return; }
                }
            }
            catch { CustomMessageBox.Show("Set COMPASS_DEC Failed", "Error"); }
        }


        private void CHK_enablecompass_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                CHK_autodec.Enabled = true;
                TXT_declination_deg.Enabled = true;
                TXT_declination_min.Enabled = true;
            }
            else
            {
                CHK_autodec.Enabled = false;
                TXT_declination_deg.Enabled = false;
                TXT_declination_min.Enabled = false;
            }

            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["MAG_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available", "Error");
                }
                else
                {
                    MainV2.comPort.setParam("MAG_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set MAG_ENABLE Failed", "Error"); }
        }

 


        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                this.Enabled = false;
                return;
            }
            else
            {
                this.Enabled = true;
            }

            if (!MainV2.Advanced)
            {
                BUT_MagCalibrationLog.Visible = false;
                label4.Visible = false;
            }
            else
            {
                BUT_MagCalibrationLog.Visible = true;
                label4.Visible = true;
            }

            startup = true;

            CMB_compass_orient.setup(typeof(Common.Rotation), "COMPASS_ORIENT", MainV2.comPort.MAV.param);

  
            CHK_enablecompass.setup(1, 0, "MAG_ENABLE", MainV2.comPort.MAV.param, TXT_declination_deg);
   

            if (MainV2.comPort.MAV.param["COMPASS_DEC"] != null)
            {
                float dec = (float)MainV2.comPort.MAV.param["COMPASS_DEC"] * rad2deg;

                float min = (dec - (int)dec) * 60;

                TXT_declination_deg.Text = ((int)dec).ToString("0");
                TXT_declination_min.Text = min.ToString("0");
            }
 
            if (MainV2.comPort.MAV.param["COMPASS_AUTODEC"] != null)
            {
                CHK_autodec.Checked = MainV2.comPort.MAV.param["COMPASS_AUTODEC"].ToString() == "1" ? true : false;
            }

            startup = false;
        }

        private void BUT_MagCalibrationLog_Click(object sender, EventArgs e)
        {
            string minthro = "30";
            if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("Min Throttle", "Use only data above this throttle percent.", ref minthro))
                return;

            int ans = 0;
            int.TryParse(minthro, out ans);

            MagCalib.ProcessLog(ans);
        }

        private void CHK_autodec_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                TXT_declination_deg.Enabled = false;

                TXT_declination_min.Enabled = false;
            }
            else
            {
                TXT_declination_deg.Enabled = true;
                TXT_declination_min.Enabled = true;
            }

            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["COMPASS_AUTODEC"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware.ToString());
                }
                else
                {
                    MainV2.comPort.setParam("COMPASS_AUTODEC", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set COMPASS_AUTODEC Failed"); }
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=DmsueBS0J3E");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_onboard.Checked)
            {
                CMB_compass_orient.SelectedIndex =  (int)Common.Rotation.ROTATION_NONE;
                MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
            }
            if (radioButton_external.Checked)
            {
                CMB_compass_orient.SelectedIndex = (int)Common.Rotation.ROTATION_ROLL_180;
                MainV2.comPort.setParam("COMPASS_EXTERNAL",1);
            }
            if (rb_px4pixhawk.Checked)
            {
                if (CustomMessageBox.Show("is the FW version greater than APM:copter 3.01 or APM:Plane 2.74?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CMB_compass_orient.SelectedIndex = (int)Common.Rotation.ROTATION_NONE;
                }
                else
                {
                    CMB_compass_orient.SelectedIndex = (int)Common.Rotation.ROTATION_ROLL_180;
                    MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
                }
                
            }
        }
    }
}