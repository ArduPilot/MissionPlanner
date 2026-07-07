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

        private bool _calChangesRequireReboot = false;

        // Number of physical compass slots the UI can display (progress bars + indicators).
        private const int MaxCompassInstances = 3;

        // Firmware raw packet stream (MAG_CAL_PROGRESS + MAG_CAL_REPORT), filled on the comms
        // thread and drained on the UI timer. A single queue keeps progress and report handling
        // in one ordered pass instead of two.
        private readonly List<MAVLink.MAVLinkMessage> _calPackets = new List<MAVLink.MAVLinkMessage>();

        // Derived UI state, owned exclusively by the UI thread.
        //  _latestReports is the single source of truth for terminal per-compass results
        //  (success / failure / autosave); every line below the progress row is derived from
        //  it. _liveProgress is the transient 0-99% feed for the progress bars until a
        //  terminal report supersedes it. _attempt is the current firmware retry attempt per
        //  compass (from progress packets), shown on the progress row while a retry is running.
        private readonly Dictionary<byte, MAVLink.mavlink_mag_cal_report_t> _latestReports =
            new Dictionary<byte, MAVLink.mavlink_mag_cal_report_t>();
        private readonly Dictionary<byte, byte> _liveProgress = new Dictionary<byte, byte>();
        private readonly Dictionary<byte, byte> _attempt = new Dictionary<byte, byte>();
        private byte _activeCalMask;
        // Show reboot modal on the next timer tick so the final SUCCESS lines are
        // visible before the modal dialog steals focus from the control repaint.
        private bool _rebootPromptPending;
        private int _rebootPromptDelayTicks;

        // Firmware uses 0-based compass_id on the wire; users see "Mag 1/2/3" in the UI.
        // Log strings show both so operators can correlate the visible row with logs and
        // firmware messages. Keep this the sole formatter to avoid drift.
        private static string CompassLabel(byte compassId) => "Mag " + (compassId + 1) + " (id: " + compassId + ")";

        // Return MAVLink status text directly from the enum [Description] so wording stays in sync
        // with upstream and new status messages are picked up without UI-side string mapping.
        // Note: mavgen emits its own MAVLink.Description attribute (see MavlinkParse.cs),
        // NOT System.ComponentModel.DescriptionAttribute.
        private static string StatusText(MAVLink.MAG_CAL_STATUS status)
        {
            if (status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                return "Success";

            var field = typeof(MAVLink.MAG_CAL_STATUS).GetField(status.ToString());
            var attr = field == null ? null
                : (MAVLink.Description)Attribute.GetCustomAttribute(field, typeof(MAVLink.Description));

            return string.IsNullOrWhiteSpace(attr?.Text) ? status.ToString() : attr.Text;
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

        private bool IsSucceeded(byte compassId)
        {
            if (!_latestReports.TryGetValue(compassId, out var report))
            {
                return false;
            }

            return (MAVLink.MAG_CAL_STATUS)report.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS;
        }

        private bool IsFailed(byte compassId)
        {
            return _latestReports.TryGetValue(compassId, out var report)
                   && (MAVLink.MAG_CAL_STATUS)report.cal_status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS;
        }

        private bool AnyFailedCompass()
        {
            foreach (var report in _latestReports.Values)
            {
                if ((MAVLink.MAG_CAL_STATUS)report.cal_status > MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    return true;
            }
            return false;
        }

        private int SucceededCompassCount()
        {
            int count = 0;
            foreach (var report in _latestReports.Values)
                if ((MAVLink.MAG_CAL_STATUS)report.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    count++;
            return count;
        }

        private int ExpectedCompassCount()
        {
            return _activeCalMask == 0 ? _latestReports.Count : CountBits(_activeCalMask);
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

        // Compasses that both succeeded and were autosaved by firmware, ordered by id.
        private List<byte> AutosavedSuccessCompasses()
        {
            return _latestReports
                .Where(kv => kv.Value.autosaved == 1
                             && (MAVLink.MAG_CAL_STATUS)kv.Value.cal_status == MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                .Select(kv => kv.Key)
                .OrderBy(id => id)
                .ToList();
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
            // Cancel any in-progress calibration on page nav so firmware can't silently
            // autosave a compass while the user is off this page — they can't see per-
            // compass status anymore and would only learn about the partial save via
            // PreArm "Compass calibrated requires reboot" on the next arm attempt.
            // Only fire if calibration was actually running (timer1 stops on
            // Cancel/completion).
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
            // Restore the button strip to its resting state so that on page re-entry
            // the user can start a fresh cal. Without this, Start (which the Start
            // handler disabled) stays greyed out because Activate() doesn't touch
            // button state — the user would be locked out until they clicked Cancel
            // (which is itself disabled here).
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
                        // doReboot returns true on success
                        if (!MainV2.comPort.doReboot())
                        {
                            CustomMessageBox.Show("Reboot failed. please manually reboot the hardware.", Strings.ERROR);
                        }
                        _calChangesRequireReboot = false;
                        UpdateRebootButtonState();
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
            but_reboot.Text = _calChangesRequireReboot ? "Reboot \u26A0" : "Reboot";
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
                bool p1 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO1_ID",
                    int.Parse(myDataGridView1.Rows[0].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p1)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }

            if (myDataGridView1.Rows.Count >= 2)
            {
                list[1]._index = 1;
                bool p2 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    int.Parse(myDataGridView1.Rows[1].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p2)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            else
            {
                // clear it
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    0);
            }

            if (myDataGridView1.Rows.Count >= 3)
            {
                list[2]._index = 2;
                bool p3 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO3_ID",
                    int.Parse(myDataGridView1.Rows[2].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p3)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }
            else
            {
                //clear it
                await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
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
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 1, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                CustomMessageBox.Show("Failed to start MAG CAL, check the autopilot is still responding.\n" + ex.ToString(), Strings.ERROR);
                return;
            }

            _calPackets.Clear();
            _latestReports.Clear();
            _liveProgress.Clear();
            _attempt.Clear();
            _activeCalMask = 0;
            _rebootPromptPending = false;
            _rebootPromptDelayTicks = 0;
            horizontalProgressBar1.Value = 0;
            horizontalProgressBar2.Value = 0;
            horizontalProgressBar3.Value = 0;
            // Reset the per-compass status indicators so a fresh cal doesn't start
            // with stale green/red from the previous attempt for the first ~30s.
            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;
            // Poll fast for the whole run so progress bars stay smooth, including a
            // compass that is still retrying after another has already saved.
            timer1.Interval = 100;

            // Unsubscribe any prior subscriptions from an earlier Start click so we don't
            // stack duplicates when the user restarts calibration without leaving the screen.
            // Fields default to 0; UnSubscribeToPacketType(0) safely no-ops.
            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            packetsub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, ReceviedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);
            packetsub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, ReceviedPacket, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

            BUT_OBmagcalstart.Enabled = false;
            BUT_OBmagcalcancel.Enabled = true;
            timer1.Start();
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                MainV2.comPort.DebugPacket(packet, true);

            // Queue progress and report packets together, preserving receive order, so the
            // tick handler can attribute a failure to the attempt that produced it.
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
            BUT_OBmagcalcancel.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Deferred reboot prompt: fire one tick after completion so the final
            // SUCCESS lines paint before the modal dialog steals focus.
            if (_rebootPromptPending)
            {
                if (_rebootPromptDelayTicks > 0)
                {
                    _rebootPromptDelayTicks--;
                    return;
                }

                _rebootPromptPending = false;
                timer1.Stop();
                // Stop listening: firmware keeps re-broadcasting terminal MAG_CAL_REPORTs
                // until the calibrator is stopped, and with the timer stopped nothing
                // would drain the queue any more.
                MainV2.comPort.UnSubscribeToPacketType(packetsub1);
                MainV2.comPort.UnSubscribeToPacketType(packetsub2);
                // Autosave was requested on start, so all params are already written to flash.
                // Offer an immediate reboot to activate them.
                CheckReboot();
                return;
            }

            IngestPackets();
            RenderCalibrationState();
            EvaluateCompletion();
        }

        // Drain the queued MAG_CAL packets. Progress packets feed the bar and the current
        // attempt; report packets set the terminal per-compass result and the reboot flag.
        private void IngestPackets()
        {
            lock (_calPackets)
            {
                foreach (var item in _calPackets)
                {
                    if (item.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
                    {
                        var p = (MAVLink.mavlink_mag_cal_progress_t)item.data;
                        _activeCalMask |= p.cal_mask;
                        _attempt[p.compass_id] = p.attempt;
                        _liveProgress[p.compass_id] = p.completion_pct;
                        continue;
                    }

                    var obj = (MAVLink.mavlink_mag_cal_report_t)item.data;

                    // Skip the empty NOT_STARTED placeholder some firmware emits.
                    if (obj.ofs_x == 0 && obj.ofs_y == 0 && obj.ofs_z == 0
                        && obj.cal_status == (byte)MAVLink.MAG_CAL_STATUS.MAG_CAL_NOT_STARTED)
                        continue;

                    _activeCalMask |= obj.cal_mask;
                    _latestReports[obj.compass_id] = obj;

                    // We deliberately do NOT touch _liveProgress here. The bar is a pure mirror
                    // of the firmware's completion_pct: while running it comes from
                    // MAG_CAL_PROGRESS, and on retry the firmware itself resets it to 0
                    // (reset_state) and re-streams it. The failure *reason* is held in
                    // _latestReports until the compass passes, so it stays readable even though
                    // the firmware's FAILED status is only transient.
                    if (obj.autosaved == 1)
                    {
                        // Firmware persisted this compass's offsets and raised
                        // _cal_requires_reboot; a reboot is now mandatory before arming.
                        // Route into the class-wide prompt so Deactivate/CheckReboot also
                        // fire on page navigation.
                        _calChangesRequireReboot = true;
                        UpdateRebootButtonState();
                    }
                }

                _calPackets.Clear();
            }
        }

        // Repaint the result panel from state. The progress bars follow the firmware stream;
        // the only extra thing the operator needs is the last failure reason so they can fix
        // it and retry. This is the sole place that writes to the bars, indicators and text.
        private void RenderCalibrationState()
        {
            lbl_obmagresult.Clear();

            // Progress row spans every compass we have heard from, so a failed compass is
            // still listed instead of silently vanishing from the row.
            var ids = new SortedSet<byte>(_liveProgress.Keys);
            foreach (var id in _latestReports.Keys)
                ids.Add(id);
            for (byte i = 0; i < MaxCompassInstances; i++)
                if (((_activeCalMask >> i) & 1) != 0)
                    ids.Add(i);

            var progressRow = new StringBuilder();
            foreach (var id in ids)
            {
                // Just show what the stream reports; a SUCCESS report pins the bar to 100.
                int pct = IsSucceeded(id) ? 100 : (_liveProgress.TryGetValue(id, out var live) ? live : 0);
                SetProgressBar(id, pct);

                if (IsSucceeded(id))
                    SetIndicator(id, Color.Green);
                else if (IsFailed(id))
                    SetIndicator(id, Color.Red);

                progressRow.Append(CompassLabel(id)).Append(' ').Append(pct).Append('%');
                // While a retry is under way (attempt >= 2) show it, so it's obvious the
                // climbing bar is a fresh attempt that follows an earlier failed one.
                if (!IsSucceeded(id) && _attempt.TryGetValue(id, out var att) && att >= 2)
                    progressRow.Append(" (attempt ").Append(att).Append(')');
                progressRow.Append("  ");
            }

            lbl_obmagresult.AppendText(progressRow.ToString().TrimEnd() + Environment.NewLine);

            // One terminal status line per compass, ordered by id, so the operator sees which
            // compasses saved and which need fixing. The attempt count lives on the progress
            // row above, so this line is just the neutral result.
            foreach (var kv in _latestReports.OrderBy(kv => kv.Key))
                lbl_obmagresult.AppendText(
                    CompassLabel(kv.Key) + ": " + StatusText((MAVLink.MAG_CAL_STATUS)kv.Value.cal_status) + Environment.NewLine);

            // Partial-save guidance: firmware autosaves successful compasses individually —
            // their params are already written. Reboot is required before those values take effect.
            var autosaved = AutosavedSuccessCompasses();
            if (autosaved.Count > 0 && AnyFailedCompass())
            {
                var savedList = string.Join(", ", autosaved.Select(id => CompassLabel(id)));
                lbl_obmagresult.AppendText(
                    "Partial save: " + savedList +
                    " already persisted to params." +
                    " Reboot required before those changes take effect." +
                    " Failed compasses continue to retry." + Environment.NewLine);
            }
        }

        // Fire the completion path once every expected compass has succeeded and none is in a
        // failed state. Uses succeeded count (not autosaved count) so firmware that reports
        // MAG_CAL_SUCCESS without autosaved==1 still triggers the reboot prompt.
        // The _rebootPromptPending guard prevents re-arming on subsequent ticks.
        private void EvaluateCompletion()
        {
            if (_rebootPromptPending)
                return;

            int expected = ExpectedCompassCount();

            if (expected > 0 && !AnyFailedCompass() && SucceededCompassCount() >= expected)
            {
                BUT_OBmagcalcancel.Enabled = false;
                BUT_OBmagcalstart.Enabled = true;
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
            if (CustomMessageBox.Show("Reboot the autopilot now?") == CustomMessageBox.DialogResult.OK)
            {
                // Cancel any in-flight cal before rebooting so firmware doesn't keep
                // running the calibrator while the link tears down. Idempotent if no
                // cal is running; keeps this button safe to click any time.
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

                MainV2.comPort.doReboot(false, true);
                _calChangesRequireReboot = false;
                UpdateRebootButtonState();
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
