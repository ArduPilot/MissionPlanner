using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using ArdupilotMega.Controls;
using MissionPlanner.Controls;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass : UserControl, IActivate
    {
        bool startup = false;

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public ConfigHWCompass()
        {
            InitializeComponent();
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("Data will be collected for 60 seconds, Please click ok and move the apm around all axises");

            ProgressReporterDialogue prd = new ProgressReporterDialogue();

            Utilities.ThemeManager.ApplyThemeTo(prd);

            prd.DoWork += prd_DoWork;

            prd.RunBackgroundOperationAsync();
        }

        void prd_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            // list of x,y,z 's
            List<Tuple<float, float, float>> data = new List<Tuple<float, float, float>>();

            // backup current rate and set to 10 hz
            byte backupratesens = MainV2.comPort.MAV.cs.ratesensors;
            MainV2.comPort.MAV.cs.ratesensors = 10;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors); // mag captures at 10 hz

            DateTime deadline = DateTime.Now.AddSeconds(60);

            float oldmx = 0;
            float oldmy = 0;
            float oldmz = 0;

            while (deadline > DateTime.Now)
            {
                double timeremaining = (deadline - DateTime.Now).TotalSeconds;
                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus((int)(((60 - timeremaining) / 60) * 100), timeremaining.ToString("0") + " Seconds");

                if (e.CancelRequested)
                {
                    // restore old sensor rate
                    MainV2.comPort.MAV.cs.ratesensors = backupratesens;
                    MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

                    e.CancelAcknowledged = true;
                    return;
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
                }
            }

            // restore old sensor rate
            MainV2.comPort.MAV.cs.ratesensors = backupratesens;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

            if (data.Count < 10)
            {
                CustomMessageBox.Show("Log does not contain enough data");
                return;
            }

            double[] ans = MagCalib.LeastSq(data);

            MagCalib.SaveOffsets(ans);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("http://www.ngdc.noaa.gov/geomagmodels/Declination.jsp");
                System.Diagnostics.Process.Start("http://www.magnetic-declination.com/");
            }
            catch { CustomMessageBox.Show("Webpage open failed... do you have a virus?\nhttp://www.magnetic-declination.com/"); }
        }

        private void TXT_declination_Validating(object sender, CancelEventArgs e)
        {
            float ans = 0;
            e.Cancel = !float.TryParse(TXT_declination.Text, out ans);
        }

        private void TXT_declination_Validated(object sender, EventArgs e)
        {
            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["COMPASS_DEC"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    float dec = 0.0f;
                    try
                    {
                        string declination = TXT_declination.Text;
                        float.TryParse(declination, out dec);
                        float deg = (float)((int)dec);
                        float mins = (dec - deg);
                        if (dec > 0)
                        {
                            dec += ((mins) / 60.0f);
                        }
                        else
                        {
                            dec -= ((mins) / 60.0f);
                        }
                    }
                    catch { CustomMessageBox.Show("Invalid input!"); return; }

                    TXT_declination.Text = dec.ToString();

                    MainV2.comPort.setParam("COMPASS_DEC", dec * deg2rad);
                }
            }
            catch { CustomMessageBox.Show("Set COMPASS_DEC Failed"); }
        }


        private void CHK_enablecompass_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked == true)
            {
                CHK_autodec.Enabled = true;
                TXT_declination.Enabled = true;
            }
            else
            {
                CHK_autodec.Enabled = false;
                TXT_declination.Enabled = false;
            }

            if (startup)
                return;
            try
            {
                if (MainV2.comPort.MAV.param["MAG_ENABLE"] == null)
                {
                    CustomMessageBox.Show("Not Available");
                }
                else
                {
                    MainV2.comPort.setParam("MAG_ENABLE", ((CheckBox)sender).Checked == true ? 1 : 0);
                }
            }
            catch { CustomMessageBox.Show("Set MAG_ENABLE Failed"); }
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

            startup = true;

            CMB_compass_orient.setup(typeof(Common.Rotation), "COMPASS_ORIENT", MainV2.comPort.MAV.param);

  
            CHK_enablecompass.setup(1, 0, "MAG_ENABLE", MainV2.comPort.MAV.param, TXT_declination);
   

            if (MainV2.comPort.MAV.param["COMPASS_DEC"] != null)
            {
                TXT_declination.Text = (float.Parse(MainV2.comPort.MAV.param["COMPASS_DEC"].ToString()) * rad2deg).ToString();
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
                TXT_declination.Enabled = false;
            }
            else
            {
                TXT_declination.Enabled = true;
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
            if (radioButton1.Checked)
            {
                CMB_compass_orient.SelectedIndex =  (int)Common.Rotation.ROTATION_NONE;
            }
            if (radioButton2.Checked)
            {
                CMB_compass_orient.SelectedIndex = (int)Common.Rotation.ROTATION_ROLL_180;
            }
            if (radioButton3.Checked)
            {
                CMB_compass_orient.SelectedIndex = (int)Common.Rotation.ROTATION_ROLL_180;
            }
        }
    }
}