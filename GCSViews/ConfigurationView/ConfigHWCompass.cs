using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass : MyUserControl, IActivate
    {
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

            if (MainV2.comPort.MAV.cs.version > Version.Parse("3.2.1") &&
                MainV2.comPort.MAV.cs.firmware == Firmwares.ArduCopter2)
            {
                QuickAPM25.Visible = false;
                buttonAPMExternal.Visible = false;
                buttonQuickPixhawk.Visible = false;
                label1.Visible = false;
            }

            if (MainV2.comPort.MAV.cs.version >= Version.Parse("3.7.1") &&
                MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane
                || Control.ModifierKeys == Keys.Control)
            {
                groupBoxonboardcalib.Visible = true;
                label4.Visible = true;
                groupBoxmpcalib.Visible = true;
            }
            else if ((MainV2.comPort.MAV.cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.COMPASS_CALIBRATION) == 0)
            {
                groupBoxonboardcalib.Visible = false;
                label4.Visible = false;
                groupBoxmpcalib.Visible = true;
            }
            else
            {
                groupBoxonboardcalib.Visible = true;
                label4.Visible = false;
                groupBoxmpcalib.Visible = false;
            }

            // General Compass Settings

            CHK_compass_learn.setup(1, 0, "COMPASS_LEARN", MainV2.comPort.MAV.param);
            if (MainV2.comPort.MAV.param["COMPASS_DEC"] != null)
            {
                var dec = MainV2.comPort.MAV.param["COMPASS_DEC"].Value * MathHelper.rad2deg;

                var min = (dec - (int)dec) * 60;

                TXT_declination_deg.Text = ((int)dec).ToString("0");
                TXT_declination_min.Text = min.ToString("0");
            }

            if (MainV2.comPort.MAV.param["COMPASS_AUTODEC"] != null)
            {
                CHK_autodec.Checked = MainV2.comPort.MAV.param["COMPASS_AUTODEC"].ToString() == "1" ? true : false;
            }


            // Compass 1 settings
            CHK_compass1_use.setup(1, 0, "COMPASS_USE", MainV2.comPort.MAV.param);
            CHK_compass1_external.setup(1, 0, "COMPASS_EXTERNAL", MainV2.comPort.MAV.param);
            CMB_compass1_orient.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_ORIENT",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_ORIENT", MainV2.comPort.MAV.param);

            if (!MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS_X"))
            {
                Enabled = false;
                return;
            }

            int offset1_x = (int)MainV2.comPort.MAV.param["COMPASS_OFS_X"];
            int offset1_y = (int)MainV2.comPort.MAV.param["COMPASS_OFS_Y"];
            int offset1_z = (int)MainV2.comPort.MAV.param["COMPASS_OFS_Z"];
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
                                        ((int)MainV2.comPort.MAV.param["COMPASS_MOT_X"]).ToString() +
                                        ",   Y: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT_Y"]).ToString() +
                                        ",   Z: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT_Z"]).ToString();
            }

            if (!MainV2.DisplayConfiguration.displayCompassConfiguration)
            {
                CHK_compass1_use.Enabled = false;
                CHK_compass1_external.Enabled = false;
                CMB_compass1_orient.Enabled = false;
            }

            // Compass 2 settings
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_EXTERN2"))
            {
                CHK_compass2_use.setup(1, 0, "COMPASS_USE2", MainV2.comPort.MAV.param);
                CHK_compass2_external.setup(1, 0, "COMPASS_EXTERN2", MainV2.comPort.MAV.param);
                CMB_compass2_orient.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_ORIENT2",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_ORIENT2", MainV2.comPort.MAV.param);

                CMB_primary_compass.setup(typeof(CompassNumber), "COMPASS_PRIMARY", MainV2.comPort.MAV.param);

                int offset2_x = (int)MainV2.comPort.MAV.param["COMPASS_OFS2_X"];
                int offset2_y = (int)MainV2.comPort.MAV.param["COMPASS_OFS2_Y"];
                int offset2_z = (int)MainV2.comPort.MAV.param["COMPASS_OFS2_Z"];

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
                                            ((int)MainV2.comPort.MAV.param["COMPASS_MOT2_X"]).ToString() +
                                            ",   Y: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT2_Y"]).ToString() +
                                            ",   Z: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT2_Z"]).ToString();
                }

                if (!MainV2.DisplayConfiguration.displayCompassConfiguration)
                {
                    CHK_compass2_use.Enabled = false;
                    CHK_compass2_external.Enabled = false;
                    CMB_compass2_orient.Enabled = false;
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
                CMB_compass3_orient.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_ORIENT3",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_ORIENT3", MainV2.comPort.MAV.param);

                int offset3_x = (int)MainV2.comPort.MAV.param["COMPASS_OFS3_X"];
                int offset3_y = (int)MainV2.comPort.MAV.param["COMPASS_OFS3_Y"];
                int offset3_z = (int)MainV2.comPort.MAV.param["COMPASS_OFS3_Z"];

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
                                            ((int)MainV2.comPort.MAV.param["COMPASS_MOT3_X"]).ToString() +
                                            ",   Y: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT3_Y"]).ToString() +
                                            ",   Z: " + ((int)MainV2.comPort.MAV.param["COMPASS_MOT3_Z"]).ToString();
                }

                if (!MainV2.DisplayConfiguration.displayCompassConfiguration)
                {
                    CHK_compass3_use.Enabled = false;
                    CHK_compass3_external.Enabled = false;
                    CMB_compass3_orient.Enabled = false;
                }
            }
            else
            {
                groupBoxCompass3.Hide();
            }

            mavlinkComboBoxfitness.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_CAL_FIT",
                    MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_CAL_FIT", MainV2.comPort.MAV.param);

            ShowRelevantFields();

            if (!MainV2.DisplayConfiguration.displayCompassConfiguration)
            {
                CHK_compass_learn.Enabled = false;
                CHK_autodec.Enabled = false;
                CMB_primary_compass.Enabled = false;
            }

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
                            dec -= (float.Parse(min) / 60);
                        else
                            dec += (float.Parse(min) / 60);
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.InvalidNumberEntered, Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_DEC", dec * MathHelper.deg2rad);
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
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "MAG_ENABLE", ((CheckBox)sender).Checked ? 1 : 0);
                }
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "MAG_ENABLE"), Strings.ERROR);
            }
        }

        private async void BUT_MagCalibrationLog_Click(object sender, EventArgs e)
        {
            var minthro = "30";
            if (DialogResult.Cancel ==
                InputBox.Show("Min Throttle", "Use only data above this throttle percent.", ref minthro))
                return;

            var ans = 0;
            int.TryParse(minthro, out ans);

            await MagCalib.ProcessLog(ans).ConfigureAwait(true);
        }

        private void CHK_autodec_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
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
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_AUTODEC", ((CheckBox)sender).Checked ? 1 : 0);
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

        private List<MAVLink.MAVLinkMessage> mprog = new List<MAVLink.MAVLinkMessage>();
        private List<MAVLink.MAVLinkMessage> mrep = new List<MAVLink.MAVLinkMessage>();
        private Dictionary<byte, MAVLink.MAG_CAL_STATUS> lastFailureStatus = new Dictionary<byte, MAVLink.MAG_CAL_STATUS>();
        private HashSet<byte> _startedCompasses = new HashSet<byte>();
        private HashSet<byte> _succeededCompasses = new HashSet<byte>();
        private HashSet<byte> _autosavedCompasses = new HashSet<byte>();
        private byte _activeCalMask;

        // Firmware uses 0-based compass_id on the wire; users see "Mag 1/2/3" in the UI.
        // Log strings show both so operators can correlate the visible row with logs and
        // firmware messages. Keep this the sole formatter to avoid drift.
        private static string CompassLabel(byte compassId) => "Mag " + (compassId + 1) + " (id: " + compassId + ")";

        // Prefer the MAVLink [Description] if the dialect XML carries one for this code;
        // otherwise fall back to the raw enum name. This keeps failure messages in sync
        // with upstream firmware wording across mavlink bumps at zero maintenance cost.
        // Note: mavgen emits its own MAVLink.Description attribute (see MavlinkParse.cs),
        // NOT System.ComponentModel.DescriptionAttribute.
        private static string StatusText(MAVLink.MAG_CAL_STATUS status)
        {
            var field = typeof(MAVLink.MAG_CAL_STATUS).GetField(status.ToString());
            var attr = field == null ? null
                : (MAVLink.Description)Attribute.GetCustomAttribute(field, typeof(MAVLink.Description));
            return string.IsNullOrEmpty(attr?.Text)
                ? status.ToString()
                : status + " \u2014 " + attr.Text;
        }

        private static int CountBits(byte mask)
        {
            int count = 0;
            while (mask != 0)
            {
                count += mask & 1;
                mask >>= 1;
            }

            return count;
        }

        private int ExpectedCompassCount()
        {
            return _activeCalMask == 0 ? _startedCompasses.Count : CountBits(_activeCalMask);
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                MainV2.comPort.DebugPacket(packet, true);

            if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
            {
                lock (this.mprog)
                {
                    this.mprog.Add(packet);
                }

                return true;
            }
            else if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT)
            {
                lock (this.mrep)
                {
                    this.mrep.Add(packet);
                }

                return true;
            }

            return true;
        }

        private int packetsub1;
        private int packetsub2;

        private void BUT_OBmagcalstart_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 1, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                CustomMessageBox.Show("Failed to start MAG CAL, check the autopilot is still responding.\n" + ex.ToString(), Strings.ERROR);
                return;
            }

            mprog.Clear();
            mrep.Clear();
            lastFailureStatus.Clear();
            _startedCompasses.Clear();
            _succeededCompasses.Clear();
            _autosavedCompasses.Clear();
            _activeCalMask = 0;
            horizontalProgressBar1.Value = 0;
            horizontalProgressBar2.Value = 0;
            horizontalProgressBar3.Value = 0;
            // Reset the poll cadence — the tick handler bumps this to 1000 ms once
            // autosave lands; without this reset the second cal per page visit polls
            // once per second and the bars update visibly laggy.
            timer1.Interval = 100;

            // Unsubscribe any prior subscriptions from an earlier Start click so we don't
            // stack duplicates when the user restarts calibration without leaving the screen.
            // Fields default to 0; UnSubscribeToPacketType(0) safely no-ops.
            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            packetsub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, ReceviedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
            packetsub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, ReceviedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

            BUT_OBmagcalstart.Enabled = false;
            BUT_OBmagcalaccept.Enabled = true;
            BUT_OBmagcalcancel.Enabled = true;
            timer1.Start();
        }

        private void BUT_OBmagcalaccept_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_ACCEPT_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);

            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR, MessageBoxButtons.OK);
            }

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
            BUT_OBmagcalstart.Enabled = true;
        }

        private void BUT_OBmagcalcancel_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR, MessageBoxButtons.OK);
            }

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
            BUT_OBmagcalstart.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbl_obmagresult.Clear();
            int compasscount = 0;
            lock (mprog)
            {
                // Sorted by compass_id so the progress line always reads Mag 1, Mag 2, Mag 3
                // regardless of the order firmware's three calibrators first transmit.
                SortedDictionary<byte, MAVLink.MAVLinkMessage> status = new SortedDictionary<byte, MAVLink.MAVLinkMessage>();
                foreach (var item in mprog)
                {
                    status[((MAVLink.mavlink_mag_cal_progress_t)item.data).compass_id] = item;
                }

                // message for user
                string message = "";
                foreach (var item in status)
                {
                    var obj = (MAVLink.mavlink_mag_cal_progress_t)item.Value.data;
                    _activeCalMask |= obj.cal_mask;

                    try
                    {
                        // Don't let a stale progress packet overwrite the 100/green we set
                        // when the SUCCESS report arrived.
                        if (!_succeededCompasses.Contains(item.Key))
                        {
                            if (item.Key == 0)
                                horizontalProgressBar1.Value = obj.completion_pct;
                            if (item.Key == 1)
                                horizontalProgressBar2.Value = obj.completion_pct;
                            if (item.Key == 2)
                                horizontalProgressBar3.Value = obj.completion_pct;
                        }
                    }
                    catch { }

                    // Firmware caps completion_pct at ~99 and signals 100 implicitly via the
                    // MAG_CAL_REPORT with MAG_CAL_SUCCESS. Match the bar (forced to 100 in the
                    // report handler) so text and visual agree — for SUCCESS, not just autosaved.
                    var pct = _succeededCompasses.Contains(item.Key) ? 100 : obj.completion_pct;
                    message += CompassLabel(item.Key) + " " + pct.ToString() + "% ";
                    _startedCompasses.Add(item.Key);
                    compasscount++;
                }
                lbl_obmagresult.AppendText(message + Environment.NewLine);
            }

            lock (mrep)
            {
                // somewhere to save our answer
                Dictionary<byte, MAVLink.MAVLinkMessage> status = new Dictionary<byte, MAVLink.MAVLinkMessage>();
                foreach (var item in mrep)
                {
                    var obj = (MAVLink.mavlink_mag_cal_report_t)item.data;

                    if (obj.ofs_x == 0 && obj.ofs_y == 0 && obj.ofs_z == 0
                        && obj.cal_status == (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED)
                        continue;

                    status[obj.compass_id] = item;
                }

                // message for user
                var consumedCompassIds = new List<byte>();
                foreach (var item in status.Values)
                {
                    var obj = (MAVLink.mavlink_mag_cal_report_t)item.data;
                    _activeCalMask |= obj.cal_mask;

                    // Any report from a compass proves it was actually calibrated (even if
                    // it failed on the very first sample check with zero progress packets).
                    // The progress loop also adds to this set, but reports can arrive
                    // without any preceding progress under the new PR #32757 early-abort
                    // firmware — without this line the completion check below would fire
                    // prematurely, declaring cal "done" while a compass was still retrying.
                    _startedCompasses.Add(obj.compass_id);

                    lbl_obmagresult.AppendText(CompassLabel(obj.compass_id) + " x:" + obj.ofs_x.ToString("0.0") + " y:" +
                                               obj.ofs_y.ToString("0.0") + " z:" +
                                               obj.ofs_z.ToString("0.0") + " fit:" + obj.fitness.ToString("0.0") + " " +
                                               StatusText((MAVLink.MAG_CAL_STATUS)obj.cal_status) + Environment.NewLine);

                    try
                    {
                        // Green + 100 on numeric SUCCESS (autosaved is a stricter
                        // downstream state used only to drive the reboot popup below).
                        if ((MAVLink.MAG_CAL_STATUS)obj.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                        {
                            if (obj.compass_id == 0)
                                horizontalProgressBar1.Value = 100;
                            if (obj.compass_id == 1)
                                horizontalProgressBar2.Value = 100;
                            if (obj.compass_id == 2)
                                horizontalProgressBar3.Value = 100;
                        }
                    }
                    catch
                    {
                    }

                    var calStatus = (MAVLink.MAG_CAL_STATUS)obj.cal_status;
                    // The "please reboot" completion check below is gated on autosaved==1
                    // (the point at which firmware persisted the offsets), so it fires only
                    // when rebooting actually matters.
                    if (calStatus > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    {
                        lastFailureStatus[obj.compass_id] = calStatus;
                        consumedCompassIds.Add(obj.compass_id);
                        _autosavedCompasses.Remove(obj.compass_id);
                        // if a prior SUCCESS was later reversed by firmware, drop the flag so
                        // the retry's progress packets can drive the bar again
                        _succeededCompasses.Remove(obj.compass_id);
                        // purge stale progress so the old 99% can't overwrite the reset
                        lock (mprog)
                        {
                            mprog.RemoveAll(m => ((MAVLink.mavlink_mag_cal_progress_t)m.data).compass_id == obj.compass_id);
                        }
                        // reset bar so the user sees the retry starting from 0
                        try
                        {
                            if (obj.compass_id == 0) horizontalProgressBar1.Value = 0;
                            if (obj.compass_id == 1) horizontalProgressBar2.Value = 0;
                            if (obj.compass_id == 2) horizontalProgressBar3.Value = 0;
                        }
                        catch { }
                    }
                    else if (calStatus == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    {
                        lastFailureStatus.Remove(obj.compass_id);
                        _succeededCompasses.Add(obj.compass_id);
                        consumedCompassIds.Add(obj.compass_id);
                    }
                    // running/waiting states leave lastFailureStatus unchanged so the
                    // previous failure reason stays visible while calibration retries

                    if (obj.autosaved == 1)
                    {
                        _autosavedCompasses.Add(obj.compass_id);
                        timer1.Interval = 1000;
                    }
                }

                // consume terminal reports so we don't re-render the same "x:… y:… z:… fit:… SUCCESS"
                // line every timer tick and so a failure report can't fight retry progress. Any
                // later report from firmware (e.g. a second MAG_CAL_REPORT with autosaved==1)
                // will re-enter mrep via ReceviedPacket and be handled afresh next tick.
                // lastFailureStatus preserves the failure message for the sticky footer.
                if (consumedCompassIds.Count > 0)
                    mrep.RemoveAll(m => consumedCompassIds.Contains(((MAVLink.mavlink_mag_cal_report_t)m.data).compass_id));
            }

            // show last known failure reason per compass (persists across firmware auto-restarts)
            if (lastFailureStatus.Count > 0)
            {
                string failures = "";
                foreach (var kv in lastFailureStatus)
                    failures += CompassLabel(kv.Key) + ": " + StatusText(kv.Value) + Environment.NewLine;
                lbl_obmagresult.AppendText(failures);
            }

            // Mixed-result partial save: PR #32757 firmware autosaves successful compasses
            // individually as soon as they hit SUCCESS, even when other compasses are still
            // failing/retrying. The all-successful "Please reboot" popup below does NOT fire
            // in this case, so without this explicit banner the user only learns about the
            // partial persist via a cryptic PreArm "Compass calibrated requires reboot" on
            // the next arm attempt.
            bool partialSave = _autosavedCompasses.Count > 0 && lastFailureStatus.Count > 0;
            if (partialSave)
            {
                var savedList = string.Join(", ",
                    _autosavedCompasses.OrderBy(id => id).Select(id => CompassLabel(id)));
                lbl_obmagresult.AppendText(
                    "Partial save: " + savedList +
                    " persisted to params. Reboot required before those changes apply." +
                    " Failed compasses continue to retry." + Environment.NewLine);
            }

            int expectedCompassCount = ExpectedCompassCount();
            if (expectedCompassCount > 0 && lastFailureStatus.Count == 0 && _autosavedCompasses.Count >= expectedCompassCount)
            {
                BUT_OBmagcalcancel.Enabled = false;
                BUT_OBmagcalaccept.Enabled = false;
                BUT_OBmagcalstart.Enabled = true;
                timer1.Stop();
                CustomMessageBox.Show("Please reboot the autopilot");
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
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE2", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE3", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERNAL", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN3", 0);

                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_LEARN", 1);

                if (
                    CustomMessageBox.Show("is the FW version greater than APM:copter 3.01 or APM:Plane 2.74?", "",
                        MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                {
                    CMB_compass1_orient.SelectedIndex = (int)Rotation.ROTATION_NONE;
                }
                else
                {
                    CMB_compass1_orient.SelectedIndex = (int)Rotation.ROTATION_ROLL_180;
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERNAL", 0);
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
                CMB_compass1_orient.SelectedIndex = (int)Rotation.ROTATION_NONE;
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE1", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE2", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE3", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERNAL", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN3", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_LEARN", 1);
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
                CMB_compass1_orient.SelectedIndex = (int)Rotation.ROTATION_ROLL_180;
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERNAL", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN2", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_EXTERN3", 0);

                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE1", 1);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE2", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_USE3", 0);

                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_PRIMARY", 0);
                MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_LEARN", 1);
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
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "COMPASS_LEARN", ((CheckBox)sender).Checked ? 1 : 0);
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

        private void but_largemagcal_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (InputBox.Show("MagCal Yaw", "Enter current heading in degrees\nNOTE: gps lock is required. Heading is true, not magnetic", ref value) == DialogResult.OK)
            {
                try
                {
                    if (MainV2.comPort.doCommand(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid,
                        MAVLink.MAV_CMD.FIXED_MAG_CAL_YAW, (float)value, 0, 0, 0, 0, 0, 0))
                    {
                        CustomMessageBox.Show(Strings.Completed, Strings.Completed);
                    }
                    else
                    {
                        CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                    }
                }
                catch (Exception)
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
        }
    }
}