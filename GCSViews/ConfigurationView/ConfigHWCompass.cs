using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass : UserControl, IActivate
    {
        private const float rad2deg = (float) (180/Math.PI);
        private const float deg2rad = (float) (1.0/rad2deg);
        private bool startup;

        public ConfigHWCompass()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                Enabled = false;
                return;
            }
            Enabled = true;

            startup = true;

            CMB_compass_orient.setup(typeof (Common.Rotation), "COMPASS_ORIENT", MainV2.comPort.MAV.param);


            CHK_enablecompass.setup(1, 0, "MAG_ENABLE", MainV2.comPort.MAV.param, TXT_declination_deg);


            if (MainV2.comPort.MAV.param["COMPASS_DEC"] != null)
            {
                var dec = MainV2.comPort.MAV.param["COMPASS_DEC"].Value*rad2deg;

                var min = (dec - (int) dec)*60;

                TXT_declination_deg.Text = ((int) dec).ToString("0");
                TXT_declination_min.Text = min.ToString("0");
            }

            if (MainV2.comPort.MAV.param["COMPASS_AUTODEC"] != null)
            {
                CHK_autodec.Checked = MainV2.comPort.MAV.param["COMPASS_AUTODEC"].ToString() == "1" ? true : false;
            }

            startup = false;
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            MagCalib.DoGUIMagCalib();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start("http://www.ngdc.noaa.gov/geomagmodels/Declination.jsp");
                Process.Start("http://www.magnetic-declination.com/");
            }
            catch
            {
                CustomMessageBox.Show(
                    "Webpage open failed... do you have a virus?\nhttp://www.magnetic-declination.com/", "Mag");
            }
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
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    var dec = 0.0f;
                    try
                    {
                        var deg = TXT_declination_deg.Text;

                        var min = TXT_declination_min.Text;

                        dec = float.Parse(deg);

                        if (dec < 0)
                            dec -= (float.Parse(min)/60);
                        else
                            dec += (float.Parse(min)/60);
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam("COMPASS_DEC", dec*deg2rad);
                }
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "COMPASS_DEC"), Strings.ERROR);
            }
        }

        private void CHK_enablecompass_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox) sender).Checked)
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
                    CustomMessageBox.Show(Strings.ErrorFeatureNotEnabled, Strings.ERROR);
                }
                else
                {
                    MainV2.comPort.setParam("MAG_ENABLE", ((CheckBox) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "MAG_ENABLE"), Strings.ERROR);
            }
        }

        private void BUT_MagCalibrationLog_Click(object sender, EventArgs e)
        {
            var minthro = "30";
            if (DialogResult.Cancel ==
                InputBox.Show("Min Throttle", "Use only data above this throttle percent.", ref minthro))
                return;

            var ans = 0;
            int.TryParse(minthro, out ans);

            MagCalib.ProcessLog(ans);
        }

        private void CHK_autodec_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox) sender).Checked)
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
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("COMPASS_AUTODEC", ((CheckBox) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set COMPASS_AUTODEC Failed");
            }
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("https://www.youtube.com/watch?v=DmsueBS0J3E");
            }
            catch
            {
                CustomMessageBox.Show(Strings.ERROR + " https://www.youtube.com/watch?v=DmsueBS0J3E");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected);
                MainV2.View.Reload();
                return;
            }

            try
            {
                if (radioButton_onboard.Checked && sender == radioButton_onboard)
                {
                    CMB_compass_orient.SelectedIndex = (int) Common.Rotation.ROTATION_NONE;
                    MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
                }

                if (radioButton_external.Checked && sender == radioButton_external)
                {
                    CMB_compass_orient.SelectedIndex = (int) Common.Rotation.ROTATION_ROLL_180;
                    MainV2.comPort.setParam("COMPASS_EXTERNAL", 1);
                }

                if (rb_px4pixhawk.Checked && sender == rb_px4pixhawk)
                {
                    if (
                        CustomMessageBox.Show("is the FW version greater than APM:copter 3.01 or APM:Plane 2.74?", "",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CMB_compass_orient.SelectedIndex = (int) Common.Rotation.ROTATION_NONE;
                    }
                    else
                    {
                        CMB_compass_orient.SelectedIndex = (int) Common.Rotation.ROTATION_ROLL_180;
                        MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
                    }
                }
            }
            catch (Exception)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
        }

        private List<MAVLink.mavlink_mag_cal_progress_t> mprog = new List<MAVLink.mavlink_mag_cal_progress_t>();
        private List<MAVLink.mavlink_mag_cal_report_t> mrep = new List<MAVLink.mavlink_mag_cal_report_t>();

        private bool ReceviedPacket(byte[] packet)
        {
            if (packet[5] == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
            {
                var mprog = packet.ByteArrayToStructure<MAVLink.mavlink_mag_cal_progress_t>();

                lock (this.mprog)
                {
                    this.mprog.Add(mprog);
                }

                return true;
            }
            else if (packet[5] == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT)
            {
                var mrep = packet.ByteArrayToStructure<MAVLink.mavlink_mag_cal_report_t>();

                lock (this.mrep)
                {
                    this.mrep.Add(mrep);
                }

                return true;
            }

            return true;
        }

        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<byte[], bool>> packetsub1;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<byte[], bool>> packetsub2;

        private void BUT_OBmagcalstart_Click(object sender, EventArgs e)
        {
            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);

            packetsub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, ReceviedPacket);
            packetsub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, ReceviedPacket);

            BUT_OBmagcalaccept.Enabled = true;
            BUT_OBmagcalcancel.Enabled = true;
            timer1.Start();
        }

        private void BUT_OBmagcalaccept_Click(object sender, EventArgs e)
        {
            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_ACCEPT_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
        }

        private void BUT_OBmagcalcancel_Click(object sender, EventArgs e)
        {
            MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (mprog)
            {
                foreach (var item in mprog)
                {
                    lbl_obmagresult.AppendText("id:" + item.compass_id + " " + item.completion_pct.ToString() + "% " + "\n");
                }

                mprog.Clear();
            }

            lock (mrep)
            {
                foreach (var item in mrep)
                {
                    if (item.compass_id == 0 && item.ofs_x == 0)
                        continue;

                    lbl_obmagresult.AppendText("id:" + item.compass_id + " x:" + item.ofs_x + " y:" + item.ofs_y + " z:" + item.ofs_z + " fit:" + item.fitness+ "\n");
                }

                mrep.Clear();
            }

            lbl_obmagresult.SelectionStart = lbl_obmagresult.Text.Length - 1;
        }
    }
}