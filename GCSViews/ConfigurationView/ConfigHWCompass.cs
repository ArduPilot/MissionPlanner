﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass : UserControl, IActivate
    {
        private const float rad2deg = (float) (180/Math.PI);
        private const float deg2rad = (float) (1.0/rad2deg);
        private const int THRESHOLD_OFS_RED = 600;
        private const int THRESHOLD_OFS_YELLOW = 400;
        private bool startup;

        private enum CompassNumber
        {
            Compass1 = 0,
            Compass2,
            Compass3
        };

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

            // General Compass Settings
            CHK_enablecompass.setup(1, 0, "MAG_ENABLE", MainV2.comPort.MAV.param);
            CHK_compass_learn.setup(1, 0, "COMPASS_LEARN", MainV2.comPort.MAV.param);
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


            // Compass 1 settings
            CHK_compass1_use.setup(1, 0, "COMPASS_USE", MainV2.comPort.MAV.param);
            CHK_compass1_external.setup(1, 0, "COMPASS_EXTERNAL", MainV2.comPort.MAV.param);
            CMB_compass1_orient.setup(typeof (Common.Rotation), "COMPASS_ORIENT", MainV2.comPort.MAV.param);

            if (!MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS_X"))
            {
                Enabled = false;
                return;
            }

            int offset1_x = (int) MainV2.comPort.MAV.param["COMPASS_OFS_X"];
            int offset1_y = (int) MainV2.comPort.MAV.param["COMPASS_OFS_Y"];
            int offset1_z = (int) MainV2.comPort.MAV.param["COMPASS_OFS_Z"];
            // Turn offsets red if any offset exceeds a threshold, or all values are 0 (not yet calibrated)
            if (absmax(offset1_x, offset1_y, offset1_z) > THRESHOLD_OFS_RED)
                LBL_compass1_offset.ForeColor = Color.Red;
            else if (absmax(offset1_x, offset1_y, offset1_z) > THRESHOLD_OFS_YELLOW)
                LBL_compass1_offset.ForeColor = Color.Yellow;
            else if (offset1_x == 0 && offset1_y == 0 & offset1_z == 0)
                LBL_compass1_offset.ForeColor = Color.Red;
            else
                LBL_compass1_offset.ForeColor = Color.Green;


            LBL_compass1_offset.Text = "OFFSETS  X: " +
                                       offset1_x.ToString() +
                                       ",   Y: " + offset1_y.ToString() +
                                       ",   Z: " + offset1_z.ToString();
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_MOT_X"))
            {
                LBL_compass1_mot.Text = "MOT          X: " +
                                        ((int) MainV2.comPort.MAV.param["COMPASS_MOT_X"]).ToString() +
                                        ",   Y: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT_Y"]).ToString() +
                                        ",   Z: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT_Z"]).ToString();
            }


            // Compass 2 settings
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_EXTERN2"))
            {
                CHK_compass2_use.setup(1, 0, "COMPASS_USE2", MainV2.comPort.MAV.param);
                CHK_compass2_external.setup(1, 0, "COMPASS_EXTERN2", MainV2.comPort.MAV.param);
                CMB_compass2_orient.setup(typeof (Common.Rotation), "COMPASS_ORIENT2", MainV2.comPort.MAV.param);

                CMB_primary_compass.setup(typeof (CompassNumber), "COMPASS_PRIMARY", MainV2.comPort.MAV.param);

                int offset2_x = (int) MainV2.comPort.MAV.param["COMPASS_OFS2_X"];
                int offset2_y = (int) MainV2.comPort.MAV.param["COMPASS_OFS2_Y"];
                int offset2_z = (int) MainV2.comPort.MAV.param["COMPASS_OFS2_Z"];

                if (absmax(offset2_x, offset2_y, offset2_z) > THRESHOLD_OFS_RED)
                    LBL_compass2_offset.ForeColor = Color.Red;
                else if (absmax(offset2_x, offset2_y, offset2_z) > THRESHOLD_OFS_YELLOW)
                    LBL_compass2_offset.ForeColor = Color.Yellow;
                else if (offset2_x == 0 && offset2_y == 0 & offset2_z == 0)
                    LBL_compass2_offset.ForeColor = Color.Red;
                else
                    LBL_compass2_offset.ForeColor = Color.Green;


                LBL_compass2_offset.Text = "OFFSETS  X: " +
                                           offset2_x.ToString() +
                                           ",   Y: " + offset2_y.ToString() +
                                           ",   Z: " + offset2_z.ToString();
                if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_MOT2_X"))
                {
                    LBL_compass2_mot.Text = "MOT          X: " +
                                            ((int) MainV2.comPort.MAV.param["COMPASS_MOT2_X"]).ToString() +
                                            ",   Y: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT2_Y"]).ToString() +
                                            ",   Z: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT2_Z"]).ToString();
                }
            }
            else
            {
                groupBoxCompass2.Hide();
            }

            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_EXTERN3"))
            {
                CHK_compass3_external.setup(1, 0, "COMPASS_EXTERN3", MainV2.comPort.MAV.param);
                CHK_compass3_use.setup(1, 0, "COMPASS_USE3", MainV2.comPort.MAV.param);
                CMB_compass3_orient.setup(typeof (Common.Rotation), "COMPASS_ORIENT3", MainV2.comPort.MAV.param);

                int offset3_x = (int) MainV2.comPort.MAV.param["COMPASS_OFS3_X"];
                int offset3_y = (int) MainV2.comPort.MAV.param["COMPASS_OFS3_Y"];
                int offset3_z = (int) MainV2.comPort.MAV.param["COMPASS_OFS3_Z"];

                if (absmax(offset3_x, offset3_y, offset3_z) > THRESHOLD_OFS_RED)
                    LBL_compass3_offset.ForeColor = Color.Red;
                else if (absmax(offset3_x, offset3_y, offset3_z) > THRESHOLD_OFS_YELLOW)
                    LBL_compass3_offset.ForeColor = Color.Yellow;
                else if (offset3_x == 0 && offset3_y == 0 & offset3_z == 0)
                    LBL_compass3_offset.ForeColor = Color.Red;
                else
                    LBL_compass3_offset.ForeColor = Color.Green;


                LBL_compass3_offset.Text = "OFFSETS  X: " +
                                           offset3_x.ToString() +
                                           ",   Y: " + offset3_y.ToString() +
                                           ",   Z: " + offset3_z.ToString();
                if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_MOT3_X"))
                {
                    LBL_compass3_mot.Text = "MOT          X: " +
                                            ((int) MainV2.comPort.MAV.param["COMPASS_MOT3_X"]).ToString() +
                                            ",   Y: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT3_Y"]).ToString() +
                                            ",   Z: " + ((int) MainV2.comPort.MAV.param["COMPASS_MOT3_Z"]).ToString();
                }
            }
            else
            {
                groupBoxCompass3.Hide();
            }

            mavlinkComboBoxfitness.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_CAL_FIT",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_CAL_FIT", MainV2.comPort.MAV.param);

            ShowRelevantFields();

            startup = false;
        }

        // Find the maximum absolute value of three values. Used to detect abnormally high or
        // low compass offsets.
        private int absmax(int val1, int val2, int val3)
        {
            return Math.Max(Math.Max(Math.Abs(val1), Math.Abs(val2)), Math.Abs(val3));
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void BUT_MagCalibration_Click(object sender, EventArgs e)
        {
            MagCalib.DoGUIMagCalib();
            Activate(); // Necessary to refresh offset values displayed on form
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
            // I am commenting this out with caution. I don't see why
            // enabling/disabling the compass shoudl change whether or
            // not autodec is enabled, but am keeping code here and commented
            // just in case I'm missing something.
            //if (((CheckBox) sender).Checked)
            //{
            //    CHK_autodec.Enabled = true;
            //    TXT_declination_deg.Enabled = true;
            //    TXT_declination_min.Enabled = true;
            // }
            //else
            //{
            //    CHK_autodec.Enabled = false;
            //    TXT_declination_deg.Enabled = false;
            //    TXT_declination_min.Enabled = false;
            //}

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

        private List<MAVLink.mavlink_mag_cal_progress_t> mprog = new List<MAVLink.mavlink_mag_cal_progress_t>();
        private List<MAVLink.mavlink_mag_cal_report_t> mrep = new List<MAVLink.mavlink_mag_cal_report_t>();

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (packet.msgid == (byte) MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
            {
                var mprog = packet.ToStructure<MAVLink.mavlink_mag_cal_progress_t>();

                lock (this.mprog)
                {
                    this.mprog.Add(mprog);
                }

                return true;
            }
            else if (packet.msgid == (byte) MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT)
            {
                var mrep = packet.ToStructure<MAVLink.mavlink_mag_cal_report_t>();

                lock (this.mrep)
                {
                    this.mrep.Add(mrep);
                }

                return true;
            }

            return true;
        }

        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> packetsub1;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> packetsub2;

        private void BUT_OBmagcalstart_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 1, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                CustomMessageBox.Show("Failed to start MAG CAL, check the autopilot is still responding.\n"+ex.ToString(),Strings.ERROR);
                return;
            }

            mprog.Clear();
            mrep.Clear();
            horizontalProgressBar1.Value = 0;
            horizontalProgressBar2.Value = 0;
            horizontalProgressBar3.Value = 0;

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
            lbl_obmagresult.Clear();
            int compasscount = 0;
            int completecount = 0;
            lock (mprog)
            {
                // somewhere to save our %
                Dictionary<byte,byte> status = new Dictionary<byte, byte>();
                foreach (var item in mprog)
                {
                    status[item.compass_id] = item.completion_pct;
                }

                // message for user
                string message = "";
                foreach (var item in status)
                {
                    try
                    {
                        if (item.Key == 0)
                            horizontalProgressBar1.Value = item.Value;
                        if (item.Key == 1)
                            horizontalProgressBar2.Value = item.Value;
                        if (item.Key == 2)
                            horizontalProgressBar3.Value = item.Value;
                    }
                    catch { }

                    message += "id:" + item.Key + " " + item.Value.ToString() + "% ";
                    compasscount++;
                }
                lbl_obmagresult.AppendText(message + "\n");
            }

            lock (mrep)
            {
                // somewhere to save our answer
                Dictionary<byte, MAVLink.mavlink_mag_cal_report_t> status = new Dictionary<byte, MAVLink.mavlink_mag_cal_report_t>();
                foreach (var item in mrep)
                {
                    if (item.compass_id == 0 && item.ofs_x == 0)
                        continue;

                    status[item.compass_id] = item;
                }

                // message for user
                foreach (var item in status.Values)
                {
                    lbl_obmagresult.AppendText("id:" + item.compass_id + " x:" + item.ofs_x.ToString("0.0") + " y:" + item.ofs_y.ToString("0.0") + " z:" +
                                             item.ofs_z.ToString("0.0") + " fit:" + item.fitness.ToString("0.0") + " " + (MAVLink.MAG_CAL_STATUS)item.cal_status + "\n");

                    try
                    {
                        if (item.compass_id == 0)
                            horizontalProgressBar1.Value = 100;
                        if (item.compass_id == 1)
                            horizontalProgressBar2.Value = 100;
                        if (item.compass_id == 2)
                            horizontalProgressBar3.Value = 100;
                    }
                    catch { }

                    if ((MAVLink.MAG_CAL_STATUS)item.cal_status != MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    {
                        //CustomMessageBox.Show(Strings.CommandFailed);
                    }

                    if (item.autosaved == 1)
                    {
                        completecount++;
                        timer1.Interval = 1000;
                    }
                }

                if (compasscount == completecount && compasscount != 0)
                {
                    BUT_OBmagcalcancel.Enabled = false;
                    BUT_OBmagcalaccept.Enabled = false;
                    timer1.Stop();
                    CustomMessageBox.Show("Please reboot the autopilot");
                }
            }
        }

        private void buttonQuickPixhawk_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected);
                MainV2.View.Reload();
                return;
            }

            try
            {
                // TODO: check this code against the original. I don't understand what the original does
                // with the different firmware versions, and I changed something about the externality
                MainV2.comPort.setParam("COMPASS_USE", 1);
                MainV2.comPort.setParam("COMPASS_USE2", 1);
                MainV2.comPort.setParam("COMPASS_USE3", 0);
                MainV2.comPort.setParam("COMPASS_EXTERNAL", 1);
                MainV2.comPort.setParam("COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam("COMPASS_EXTERN3", 0);

                MainV2.comPort.setParam("COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam("COMPASS_LEARN", 1);

                if (
                    CustomMessageBox.Show("is the FW version greater than APM:copter 3.01 or APM:Plane 2.74?", "",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CMB_compass1_orient.SelectedIndex = (int) Common.Rotation.ROTATION_NONE;
                }
                else
                {
                    CMB_compass1_orient.SelectedIndex = (int) Common.Rotation.ROTATION_ROLL_180;
                    MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
                }
            }
            catch (Exception)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            Activate();
        }

        private void QuickAPM25_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected);
                MainV2.View.Reload();
                return;
            }
            try
            {
                CMB_compass1_orient.SelectedIndex = (int) Common.Rotation.ROTATION_NONE;
                MainV2.comPort.setParam("COMPASS_USE1", 1);
                MainV2.comPort.setParam("COMPASS_USE2", 0);
                MainV2.comPort.setParam("COMPASS_USE3", 0);
                MainV2.comPort.setParam("COMPASS_EXTERNAL", 0);
                MainV2.comPort.setParam("COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam("COMPASS_EXTERN3", 0);
                MainV2.comPort.setParam("COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam("COMPASS_LEARN", 1);
            }
            catch (Exception)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            Activate();
        }

        private void buttonAPMExternal_Click(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected);
                MainV2.View.Reload();
                return;
            }
            try
            {
                CMB_compass1_orient.SelectedIndex = (int) Common.Rotation.ROTATION_ROLL_180;
                MainV2.comPort.setParam("COMPASS_EXTERNAL", 1);
                MainV2.comPort.setParam("COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam("COMPASS_EXTERN3", 0);

                MainV2.comPort.setParam("COMPASS_USE1", 1);
                MainV2.comPort.setParam("COMPASS_USE2", 0);
                MainV2.comPort.setParam("COMPASS_USE3", 0);

                MainV2.comPort.setParam("COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam("COMPASS_LEARN", 1);
            }
            catch (Exception)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            Activate();
        }

        private void CHK_compasslearn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort.MAV.param["COMPASS_LEARN"] == null)
                {
                    CustomMessageBox.Show("Not Available on " + MainV2.comPort.MAV.cs.firmware);
                }
                else
                {
                    MainV2.comPort.setParam("COMPASS_LEARN", ((CheckBox) sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show("Set COMPASS_LEARN Failed");
            }
        }


        private void CHK_compass(object sender, EventArgs e)
        {
            ShowRelevantFields();
        }

        private void ShowRelevantFields()
        {
            TXT_declination_deg.Enabled = !CHK_autodec.Checked;
            TXT_declination_min.Enabled = !CHK_autodec.Checked;

            CMB_compass1_orient.Visible = CHK_compass1_external.Checked;
            CMB_compass2_orient.Visible = CHK_compass2_external.Checked;
            CMB_compass3_orient.Visible = CHK_compass3_external.Checked;

            LBL_compass1_mot.Visible = CHK_compass1_use.Checked;
            LBL_compass1_offset.Visible = CHK_compass1_use.Checked;

            LBL_compass2_mot.Visible = CHK_compass2_use.Checked;
            LBL_compass2_offset.Visible = CHK_compass2_use.Checked;

            LBL_compass3_mot.Visible = CHK_compass3_use.Checked;
            LBL_compass3_offset.Visible = CHK_compass3_use.Checked;

            // Toggle primary compass controls as appropriate
            CMB_primary_compass.Visible = MainV2.comPort.MAV.param.ContainsKey("COMPASS_PRIMARY");
            LBL_primary_compass.Visible = MainV2.comPort.MAV.param.ContainsKey("COMPASS_PRIMARY");
        }
    }
}