using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass2 : MyUserControl, IActivate, IDeactivate
    {
        private List<CompassDeviceInfo> list;

        private bool _calChangesRequireReboot;

        // Number of physical compass slots the UI can display (progress bars + indicators).
        private const int MaxCompassInstances = 3;

        // MAG_CAL_PROGRESS/REPORT packets queued on the comms thread and drained on the UI timer.
        private readonly List<MAVLink.MAVLinkMessage> _calPackets = new List<MAVLink.MAVLinkMessage>();

        // Per-compass calibration state, updated on the UI thread.
        private readonly MagCalTracker _cal = new MagCalTracker();
        // Delays the reboot modal one tick so the final SUCCESS lines paint first.
        private bool _rebootPromptPending;
        private int _rebootPromptDelayTicks;

        // Wire compass_id is 0-based; the UI shows "Mag 1/2/3" plus the id so rows match logs.
        private static string CompassLabel(byte compassId) => "Mag " + (compassId + 1) + " (id: " + compassId + ")";

        // Loaded param value, or null if the param isn't present.
        private static float? TryGetParam(string name)
        {
            var p = MainV2.comPort.MAV.param;
            return p.ContainsKey(name) ? (float?)p[name].Value : null;
        }

        // Maps a compass id to its progress bar, clamping to the control's valid range.
        private void SetProgressBar(byte compassId, int percent)
        {
            percent = Math.Max(0, Math.Min(100, percent));
            switch (compassId)
            {
                case 0: horizontalProgressBar1.Value = percent; break;
                case 1: horizontalProgressBar2.Value = percent; break;
                case 2: horizontalProgressBar3.Value = percent; break;
            }
        }

        // Maps a compass id to its status indicator swatch.
        private void SetIndicator(byte compassId, Color color)
        {
            switch (compassId)
            {
                case 0: pictureBox1.BackColor = color; break;
                case 1: pictureBox2.BackColor = color; break;
                case 2: pictureBox3.BackColor = color; break;
            }
        }

        private int packetsub1;
        private int packetsub2;

        public class CompassDeviceInfo : DeviceInfo
        {
            private string _orient;
            private bool _external;

            public CompassDeviceInfo(int index, string ParamName, uint id) : base(index, ParamName, id)
            {
                //set initial state
                var id1 = MainV2.comPort.MAV.param[new[] { "COMPASS_DEV_ID", "COMPASS1_DEV_ID"}];
                var id2 = MainV2.comPort.MAV.param[new[] { "COMPASS_DEV_ID2", "COMPASS2_DEV_ID"}];
                var id3 = MainV2.comPort.MAV.param[new[] { "COMPASS_DEV_ID3", "COMPASS3_DEV_ID"}];

                var idO1 = MainV2.comPort.MAV.param[new[] { "COMPASS_ORIENT", "COMPASS1_ORIENT"}];
                var idO2 = MainV2.comPort.MAV.param[new[] { "COMPASS_ORIENT2", "COMPASS2_ORIENT"}];
                var idO3 = MainV2.comPort.MAV.param[new[] { "COMPASS_ORIENT3", "COMPASS3_ORIENT"}];

                var idE1 = MainV2.comPort.MAV.param[new[] { "COMPASS_EXTERNAL", "COMPASS1_EXTERN"}];
                var idE2 = MainV2.comPort.MAV.param[new[] { "COMPASS_EXTERN2", "COMPASS2_EXTERN"}];
                var idE3 = MainV2.comPort.MAV.param[new[] { "COMPASS_EXTERN3", "COMPASS3_EXTERN"}];

                if (id1 != null && id1?.Value == id)
                {
                    _orient = idO1?.ToString();
                    _external = idE1?.Value > 0 ? true : false;
                }
                if (id2 != null && id2?.Value == id)
                {
                    _orient = idO2?.ToString();
                    _external = idE2?.Value > 0 ? true : false;
                }
                if (id3 != null && id3?.Value == id)
                {
                    _orient = idO3?.ToString();
                    _external = idE3?.Value > 0 ? true : false;
                }
            }

            public string Orient
            {
                get => _orient;
                set => _orient = value;
            }

            public bool External
            {
                get => _external;
                set => _external = value;
            }

            public bool Missing
            {
                get;
                set;
            }
        }

        public ConfigHWCompass2()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            // COMPASS_DEV_ID get a list of all connected devices
            list = MainV2.comPort.MAV.param.Where(a => a.Name.StartsWith("COMPASS") && a.Name.Contains("DEV_ID") && a.Value != 0)
                .Select((a, b) => new CompassDeviceInfo(b, a.Name, (uint) a.Value))
                .OrderBy((a) => a.ParamName).ToList();

            // COMPASS_PRIO get a list of all prios
            var prio = MainV2.comPort.MAV.param.Where(a => a.Name.StartsWith("COMPASS_PRIO") && a.Value != 0)
                .Select((a, b) => new CompassDeviceInfo(b, a.Name, (uint)a.Value))
                .OrderBy((a) => a.ParamName).ToList();

            var anymissing = false;
            // mark missing
            prio.ForEach(a =>
            {
                if (a.DevID == 0 || list.Any(b => b.DevID == a.DevID))
                    a.Missing = false;
                else
                {
                    a.Missing = true;
                    anymissing = true;
                }
            });

            //filter list removing prio dups from the list
            list = list.Where(a => !prio.Any(b => b.DevID == a.DevID)).ToList();

            // insert prios at the top
            list.InsertRange(0, prio);

            var bs = new BindingSource();
            bs.DataSource = list;
            myDataGridView1.DataSource = bs;

            mavlinkComboBoxfitness.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_CAL_FIT",
                MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_CAL_FIT", MainV2.comPort.MAV.param);

            mavlinkCheckBoxUseCompass1.setup(1, 0, new[] { "COMPASS_USE", "COMPASS1_USE"}, MainV2.comPort.MAV.param);
            mavlinkCheckBoxUseCompass2.setup(1, 0, new[] { "COMPASS_USE2", "COMPASS2_USE"}, MainV2.comPort.MAV.param);
            mavlinkCheckBoxUseCompass3.setup(1, 0, new[] { "COMPASS_USE3", "COMPASS3_USE"}, MainV2.comPort.MAV.param);

            CHK_compass_learn.setup(1, 0, "COMPASS_LEARN", MainV2.comPort.MAV.param);

            {
                // set the default items
                var orient_param = MainV2.comPort.MAV.param[new[] { "COMPASS_ORIENT", "COMPASS1_ORIENT"}];
                var source = ParameterMetaDataRepository.GetParameterOptionsInt(orient_param.Name,
                        MainV2.comPort.MAV.cs.firmware.ToString())
                    .Select(a => new KeyValuePair<string, string>(a.Key.ToString(), a.Value)).ToList();
                Orientation.DataSource = source;
                Orientation.DisplayMember = "Value";
                Orientation.ValueMember = "Key";
            }

            if (anymissing)
            {
                CustomMessageBox.Show("Your compass configuration has changed, please review the missing compass", Strings.ERROR);
            }
        }

        public void Deactivate()
        {
            // Cancel a running cal so firmware can't autosave while the user is off this page.
            if (timer1.Enabled)
            {
                try
                {
                    MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent,
                        MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);
                }
                catch (Exception ex) { this.LogError(ex); }
            }
            timer1.Stop();
            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);
            // Activate() doesn't reset the buttons, so Start would stay disabled on re-entry.
            BUT_OBmagcalstart.Enabled = true;
            BUT_OBmagcalcancel.Enabled = false;

            CheckReboot();
        }

        private bool CheckReboot()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return true;

            if (_calChangesRequireReboot)
            {
                if (CustomMessageBox.Show("Compass changes have been saved to parameters but require a reboot to take effect. Reboot now?", "Reboot",
                        CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
                {
                    try
                    {
                        // doReboot returns true on success; only clear the flag when the
                        // reboot actually went through so the ⚠ button stays visible if it fails.
                        if (MainV2.comPort.doReboot())
                        {
                            _calChangesRequireReboot = false;
                            UpdateRebootButtonState();
                        }
                        else
                        {
                            CustomMessageBox.Show("Reboot failed. Please manually reboot the hardware.", Strings.ERROR);
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
                    }

                    return true;
                }
            }

            return false;
        }

        private void UpdateRebootButtonState()
        {
            if (_calChangesRequireReboot)
            {
                but_reboot.Text = "Reboot Now \u26A0";
                toolTipReboot.SetToolTip(but_reboot, "Compass calibration saved - reboot required to take effect");
            }
            else
            {
                but_reboot.Text = "Reboot";
                toolTipReboot.SetToolTip(but_reboot, string.Empty);
            }
        }

        private async void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Up.Index && e.RowIndex != 0)
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex - 1, item);

                await UpdateFirst3();
            }

            if (e.ColumnIndex == Down.Index && e.RowIndex < (myDataGridView1.RowCount-1))
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex + 1, item);

                await UpdateFirst3();
            }
        }

        private async Task UpdateFirst3()
        {
            if (myDataGridView1.Rows.Count >= 1)
            {
                list[0]._index = 0;
                bool p1 = await MainV2.comPort.setParamAsync(MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO1_ID",
                    int.Parse(myDataGridView1.Rows[0].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p1)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }

            if (myDataGridView1.Rows.Count >= 2)
            {
                list[1]._index = 1;
                bool p2 = await MainV2.comPort.setParamAsync(MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    int.Parse(myDataGridView1.Rows[1].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p2)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            else
            {
                // clear it
                await MainV2.comPort.setParamAsync(MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    0);
            }

            if (myDataGridView1.Rows.Count >= 3)
            {
                list[2]._index = 2;
                bool p3 = await MainV2.comPort.setParamAsync(MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO3_ID",
                    int.Parse(myDataGridView1.Rows[2].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p3)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            else
            {
                //clear it
                await MainV2.comPort.setParamAsync(MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO3_ID",
                    0);
            }

            _calChangesRequireReboot = true;
            UpdateRebootButtonState();

            myDataGridView1.Invalidate();
        }

        private void BUT_OBmagcalstart_Click(object sender, EventArgs e)
        {
            if (_calChangesRequireReboot && !CheckReboot())
            {
                return;
            }

            try
            {
                MainV2.comPort.doCommand(MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 1, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                CustomMessageBox.Show("Failed to start MAG CAL, check the autopilot is still responding.\n" + ex.ToString(), Strings.ERROR);
                return;
            }

            _calPackets.Clear();
            _cal.Clear();
            _rebootPromptPending = false;
            _rebootPromptDelayTicks = 0;
            horizontalProgressBar1.Value = 0;
            horizontalProgressBar2.Value = 0;
            horizontalProgressBar3.Value = 0;
            // Clear green/red swatches left by the previous run.
            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;
            // Drop subscriptions from an earlier Start so they don't stack; id 0 is a no-op.
            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            packetsub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, ReceviedPacket, MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
            packetsub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, ReceviedPacket, MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

            BUT_OBmagcalstart.Enabled = false;
            BUT_OBmagcalcancel.Enabled = true;
            timer1.Start();
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                MainV2.comPort.DebugPacket(packet, true);

            // Queue both message types in receive order.
            if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS ||
                packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT)
            {
                lock (this._calPackets)
                {
                    this._calPackets.Add(packet);
                }
            }

            return true;
        }

        private void BUT_OBmagcalcancel_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand(MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR, MessageBoxButtons.OK);
            }

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
            BUT_OBmagcalstart.Enabled = true;
            BUT_OBmagcalcancel.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_rebootPromptPending)
            {
                if (_rebootPromptDelayTicks > 0)
                {
                    _rebootPromptDelayTicks--;
                    return;
                }

                _rebootPromptPending = false;
                timer1.Stop();
                // Terminal reports keep streaming and nothing drains the queue once the timer stops.
                MainV2.comPort.UnSubscribeToPacketType(packetsub1);
                MainV2.comPort.UnSubscribeToPacketType(packetsub2);
                CheckReboot();
                return;
            }

            IngestPackets();
            RenderCalibrationState();
            EvaluateCompletion();
        }

        // Feed queued MAG_CAL packets to the tracker; a saved report means a reboot is needed.
        private void IngestPackets()
        {
            lock (_calPackets)
            {
                foreach (var item in _calPackets)
                {
                    if (item.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
                    {
                        _cal.Apply((MAVLink.mavlink_mag_cal_progress_t)item.data);
                    }
                    else if (_cal.Apply((MAVLink.mavlink_mag_cal_report_t)item.data))
                    {
                        // Offsets are persisted; a reboot is needed before they take effect.
                        _calChangesRequireReboot = true;
                        UpdateRebootButtonState();
                    }
                }

                _calPackets.Clear();
            }
        }

        // Repaints bars, swatches and result text each tick. The text is built in one StringBuilder
        // and assigned once to avoid per-call scroll-to-end repaints.
        private void RenderCalibrationState()
        {
            var sb = new StringBuilder();

            // --- progress row: every compass heard from, so a failed one doesn't vanish ---
            var progressRow = new StringBuilder();
            foreach (var id in _cal.CompassIds(MaxCompassInstances))
            {
                int pct = _cal.Percent(id);
                SetProgressBar(id, pct);

                if (_cal.IsSucceeded(id))
                    SetIndicator(id, Color.Green);
                else if (_cal.IsFailed(id))
                    SetIndicator(id, Color.Red);

                progressRow.Append(CompassLabel(id)).Append(' ').Append(pct).Append('%');
                // attempt >= 2 marks a retry after a failed attempt.
                if (!_cal.IsSucceeded(id) && _cal.Attempt(id) >= 2)
                    progressRow.Append(" (attempt ").Append(_cal.Attempt(id)).Append(')');
                progressRow.Append("  ");
            }
            sb.AppendLine(progressRow.ToString().TrimEnd());

            // --- one terminal status line per compass, ordered by id ---
            foreach (var kv in _cal.Reports)
                sb.AppendLine(
                    CompassLabel(kv.Key) + ": "
                    + MagCalText.Status((MAVLink.MAG_CAL_STATUS)kv.Value.cal_status)
                    + MagCalText.Detail(kv.Value, TryGetParam("COMPASS_OFFS_MAX")));

            // --- partial-save guidance ---
            // Firmware autosaves successful compasses individually — their params are already
            // written. Reboot is required before those values take effect.
            var autosaved = _cal.AutosavedSuccessIds();
            if (autosaved.Count > 0 && _cal.AnyFailed)
            {
                var savedList = string.Join(", ", autosaved.Select(id => CompassLabel(id)));
                sb.AppendLine(
                    "Partial save: " + savedList +
                    " already persisted to params." +
                    " Reboot required before those changes take effect." +
                    " Failed compasses continue to retry.");
            }

            lbl_obmagresult.Text = sb.ToString();
        }

        // Completes once every expected compass has succeeded and none is failed.
        private void EvaluateCompletion()
        {
            if (_rebootPromptPending)
                return;

            if (_cal.IsComplete)
            {
                BUT_OBmagcalcancel.Enabled = false;
                BUT_OBmagcalstart.Enabled = true;
                // Autosave was requested at start, so SUCCESS means the params are written. Don't wait
                // for an autosaved==1 report that may arrive after we stop listening.
                _calChangesRequireReboot = true;
                UpdateRebootButtonState();
                _rebootPromptPending = true;
                _rebootPromptDelayTicks = 1;
            }
        }

        private void myDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (myDataGridView1.Rows[e.RowIndex].Cells[Priority.Index].Value?.ToString() != (e.RowIndex + 1).ToString())
            {
                myDataGridView1.Rows[e.RowIndex].Cells[Priority.Index].Value = (e.RowIndex + 1).ToString();


            }
        }

        private void but_largemagcal_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (InputBox.Show("MagCal Yaw", "Enter current heading in degrees\nNOTE: gps lock is required. Heading is true, not magnetic", ref value) == DialogResult.OK)
            {
                try
                {
                    if (MainV2.comPort.doCommand(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid,
                        MAVLink.MAV_CMD.FIXED_MAG_CAL_YAW, (float) value, 0, 0, 0, 0, 0, 0))
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

        private void but_reboot_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Reboot the autopilot now?", "Reboot",
                    CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                // Stop a running cal before rebooting.
                if (timer1.Enabled)
                {
                    try
                    {
                        MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent,
                            MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);
                    }
                    catch (Exception ex) { this.LogError(ex); }
                    MainV2.comPort.UnSubscribeToPacketType(packetsub1);
                    MainV2.comPort.UnSubscribeToPacketType(packetsub2);
                    timer1.Stop();
                    BUT_OBmagcalstart.Enabled = true;
                    BUT_OBmagcalcancel.Enabled = false;
                }

                if (MainV2.comPort.doReboot(false, true))
                {
                    _calChangesRequireReboot = false;
                    UpdateRebootButtonState();
                }
                else
                {
                    CustomMessageBox.Show("Reboot failed. Please manually reboot the hardware.", Strings.ERROR);
                }
            }
        }

        private void myDataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void compassDeviceInfoBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private async void but_missing_ClickAsync(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dataGridViewRow in myDataGridView1.Rows)
            {
                if (dataGridViewRow.Cells[Missing.Index].Value.Equals(true))
                {
                    myDataGridView1.Rows.Remove(dataGridViewRow);
                    but_missing_ClickAsync(null, null);
                    return;
                }
            }

            await UpdateFirst3();
        }
    }
}
