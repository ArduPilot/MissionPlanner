using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    /// <summary>A non-modal camera definition editor. All session and control updates run on the UI thread.</summary>
    public sealed class CameraCustomSettingsForm : Form
    {
        private sealed class Row
        {
            public Control Editor;
            public Panel Panel;
            public Label Detail;
            public Button Apply;
            public Label SliderValue;
            public decimal SliderMinimum, SliderStep;
            public object Value;
            public bool Submitted;
        }

        // Like MAVProxy's EVT_SCROLL_CHANGED and QGC's camera slider with live=false,
        // commit completed gestures rather than sending every intermediate drag position.
        private sealed class SettingsSlider : TrackBar
        {
            public bool Adjusting { get; private set; }
            private int gestureStart;
            public event EventHandler Committed;
            private static bool NavigationKey(Keys key) => key == Keys.Left || key == Keys.Right || key == Keys.Up
                || key == Keys.Down || key == Keys.Home || key == Keys.End || key == Keys.PageUp || key == Keys.PageDown;

            private void Commit()
            {
                Adjusting = false;
                if (Value != gestureStart) Committed?.Invoke(this, EventArgs.Empty);
            }
            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    gestureStart = Value;
                    Adjusting = true;
                }
                base.OnMouseDown(e);
            }
            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                if (e.Button == MouseButtons.Left && Adjusting) Commit();
            }
            protected override void OnMouseCaptureChanged(EventArgs e)
            {
                base.OnMouseCaptureChanged(e);
                if (!Capture && Adjusting) Commit();
            }
            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (NavigationKey(e.KeyCode) && !Adjusting)
                {
                    gestureStart = Value;
                    Adjusting = true;
                }
                base.OnKeyDown(e);
            }
            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);
                if (NavigationKey(e.KeyCode) && Adjusting) Commit();
            }
            protected override void OnMouseWheel(MouseEventArgs e)
            {
                gestureStart = Value;
                base.OnMouseWheel(e);
                Commit();
            }
            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                if (Adjusting) Commit();
            }
        }
        public CameraProtocol Camera { get; }
        private readonly MAVState target;
        private readonly ConcurrentQueue<MAVLink.MAVLinkMessage> incoming = new ConcurrentQueue<MAVLink.MAVLinkMessage>();
        private readonly Dictionary<string, Row> rows = new Dictionary<string, Row>();
        private readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = 100 };
        private readonly FlowLayoutPanel settings = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        private readonly Label status = new Label { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(8), AutoEllipsis = true };
        private readonly Button refresh = new MyButton { Text = "Refresh", AutoSize = true };
        private CameraSettingsSession session;
        private CancellationTokenSource download;
        private TaskCompletionSource<MAVLink.mavlink_camera_information_t> informationRequest;
        private string definitionUri;
        private ushort definitionVersion;
        private bool loading, updating, closed;

        public CameraCustomSettingsForm(CameraProtocol camera)
        {
            Camera = camera;
            target = camera.MavState ?? throw new ArgumentException("Camera is not connected");
            Text = "Custom Settings";
            Size = new Size(780, 650);
            MinimumSize = new Size(600, 350);
            StartPosition = FormStartPosition.CenterParent;
            AutoScaleMode = AutoScaleMode.Font;
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 42, Padding = new Padding(5) };
            var close = new MyButton { Text = "Close", AutoSize = true };
            close.Click += (s, e) => Close();
            refresh.Click += async (s, e) => await ReloadAsync();
            buttons.Controls.AddRange(new Control[] { refresh, close });
            Controls.Add(settings);
            Controls.Add(status);
            Controls.Add(buttons);
            settings.SizeChanged += (s, e) => ResizeRows();
            Shown += async (s, e) => await ReloadAsync();
            timer.Tick += (s, e) => Poll();
            target.parent.OnPacketReceived += PacketReceived;
            timer.Start();
            ThemeManager.ApplyThemeTo(this);
        }

        private void PacketReceived(object sender, MAVLink.MAVLinkMessage message)
        {
            if (message.sysid != target.sysid || message.compid != target.compid) return;
            if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION)
            {
                Volatile.Read(ref informationRequest)?.TrySetResult((MAVLink.mavlink_camera_information_t)message.data);
                incoming.Enqueue(message);
            }
            else if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.PARAM_EXT_VALUE || message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.PARAM_EXT_ACK)
                incoming.Enqueue(message);
        }

        private async Task ReloadAsync()
        {
            if (loading || closed) return;
            loading = true;
            refresh.Enabled = false;
            settings.Enabled = false;
            session = null;
            status.Text = "Requesting camera information…";
            var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            download = cancel;
            try
            {
                var request = new TaskCompletionSource<MAVLink.mavlink_camera_information_t>(TaskCreationOptions.RunContinuationsAsynchronously);
                Interlocked.Exchange(ref informationRequest, request);
                // The packet, rather than COMMAND_ACK, supplies the definition URI.
                var command = Camera.RequestCameraInformationAsync(requireAck: false);
                await Task.WhenAny(request.Task, Task.Delay(5000, cancel.Token));
                cancel.Token.ThrowIfCancellationRequested();
                var info = request.Task.IsCompleted ? await request.Task : Camera.HasCameraInformation ? Camera.CameraInformation
                    : throw new TimeoutException("Camera did not send CAMERA_INFORMATION");
                var uri = Encoding.UTF8.GetString(info.cam_definition_uri ?? new byte[0]).Split('\0')[0].Trim();
                definitionUri = uri;
                definitionVersion = info.cam_definition_version;
                if (uri.Length == 0) throw new InvalidOperationException("Camera does not advertise a custom settings definition");
                status.Text = "Loading camera definition…";
                var definition = await Task.Run(() => CameraDefinitionLoader.Load(uri, target, cancel), cancel.Token);
                if (closed) return;
                MAVLink.MAVLinkMessage latestInformation = null;
                while (incoming.TryDequeue(out var queued))
                    if (queued.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION) latestInformation = queued;
                // Do not lose an updated URI/version advertised while the file was downloading.
                if (latestInformation != null) incoming.Enqueue(latestInformation);
                session = new CameraSettingsSession(definition, (byte)target.sysid, target.compid,
                    packet => target.parent.sendPacket(packet, target.sysid, target.compid));
                BuildRows();
                Text = "Custom Settings — " + definition.Model + " (" + target.sysid + ":" + target.compid + ")";
                session.RequestAll(DateTime.UtcNow);
                settings.Enabled = true;
                UpdateRows();
            }
            catch (Exception ex)
            {
                if (!closed) status.Text = "Could not load custom settings: " + ex.Message + ". Use Refresh to retry.";
            }
            finally
            {
                Interlocked.Exchange(ref informationRequest, null);
                download = null;
                cancel.Dispose();
                loading = false;
                if (!closed) refresh.Enabled = true;
            }
        }

        private void BuildRows()
        {
            settings.SuspendLayout();
            foreach (var row in rows.Values) row.Panel.Dispose();
            settings.Controls.Clear();
            rows.Clear();
            foreach (var p in session.Definition.Parameters.Values.Where(p => p.Control))
            {
                var panel = new Panel { Height = 63, Margin = new Padding(8, 4, 8, 4) };
                var label = new Label { Text = p.Description, AutoEllipsis = true, Location = new Point(4, 6), Size = new Size(325, 23) };
                var row = new Row { Panel = panel, Detail = new Label { AutoEllipsis = true, Location = new Point(4, 33), Size = new Size(690, 23) } };
                if (p.Options.Count > 0 || session.Definition.Parameters.Values.Any(source => source.Options.Any(o => o.Ranges.Any(r => r.Target == p.Name))))
                {
                    var combo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 270 };
                    combo.SelectionChangeCommitted += (s, e) =>
                    {
                        if (combo.SelectedItem is CameraParameter.Option option && option.Value != null) Submit(p.Name, CameraParameter.Format(option.Value));
                    };
                    row.Editor = combo;
                }
                else if (SliderRange(p, out var minimum, out var step, out var steps))
                {
                    row.SliderMinimum = minimum;
                    row.SliderStep = step;
                    var slider = new SettingsSlider
                    {
                        Minimum = 0, Maximum = steps, SmallChange = 1, LargeChange = Math.Max(1, steps / 10),
                        TickStyle = TickStyle.None, AutoSize = false, Size = new Size(200, 27)
                    };
                    row.SliderValue = new Label { Location = new Point(540, 6), Size = new Size(120, 23), AutoEllipsis = true };
                    slider.ValueChanged += (s, e) =>
                    {
                        if (!updating && !loading && !closed) row.SliderValue.Text = SliderText(p, row, slider.Value);
                    };
                    slider.Committed += (s, e) =>
                    {
                        if (updating || loading || closed) return;
                        var text = SliderText(p, row, slider.Value);
                        if (!Equals(row.Value, p.ConvertValue(text))) Submit(p.Name, text);
                    };
                    row.Editor = slider;
                    panel.Controls.Add(row.SliderValue);
                }
                else
                {
                    var text = new TextBox { Width = 195 };
                    text.KeyDown += (s, e) =>
                    {
                        if (e.KeyCode != Keys.Enter) return;
                        Submit(p.Name, text.Text);
                        e.Handled = e.SuppressKeyPress = true;
                    };
                    row.Editor = text;
                    if (!p.ReadOnly)
                    {
                        row.Apply = new MyButton { Text = "Apply", Location = new Point(535, 2), Size = new Size(68, 27) };
                        row.Apply.Click += (s, e) => Submit(p.Name, text.Text);
                        panel.Controls.Add(row.Apply);
                    }
                }
                row.Editor.Location = new Point(335, 3);
                row.Editor.AccessibleName = p.Description;
                panel.Controls.AddRange(new[] { label, row.Editor, row.Detail });
                rows.Add(p.Name, row);
                settings.Controls.Add(panel);
            }
            ThemeManager.ApplyThemeTo(settings);
            ResizeRows();
            settings.ResumeLayout();
        }

        private static bool SliderRange(CameraParameter parameter, out decimal minimum, out decimal step, out int steps)
        {
            minimum = step = 0;
            steps = 0;
            if (parameter.Minimum == null || parameter.Maximum == null || parameter.Step == null) return false;
            if (!decimal.TryParse(CameraParameter.Format(parameter.Minimum), NumberStyles.Float, CultureInfo.InvariantCulture, out minimum)
                || !decimal.TryParse(CameraParameter.Format(parameter.Maximum), NumberStyles.Float, CultureInfo.InvariantCulture, out var maximum)
                || !decimal.TryParse(parameter.Step, NumberStyles.Float, CultureInfo.InvariantCulture, out step) || step <= 0) return false;
            if (parameter.WireType <= 8 && step != decimal.Truncate(step)) return false;
            // Decimal cannot represent very small floating-point bounds. Use a text editor
            // if converting through decimal would move either endpoint onto another wire value.
            if (!Equals(parameter.Minimum, parameter.ConvertValue(minimum.ToString(parameter.WireType <= 8 ? "0" : "G29", CultureInfo.InvariantCulture)))
                || !Equals(parameter.Maximum, parameter.ConvertValue(maximum.ToString(parameter.WireType <= 8 ? "0" : "G29", CultureInfo.InvariantCulture)))) return false;
            try
            {
                decimal count = decimal.Floor((maximum - minimum) / step);
                if (count < 1 || count > 100000) return false;
                steps = (int)count;
                return true;
            }
            catch (OverflowException) { return false; }
        }

        private static string SliderText(CameraParameter parameter, Row row, int position) => CameraParameter.Format(
            parameter.ConvertValue((row.SliderMinimum + position * row.SliderStep).ToString(parameter.WireType <= 8 ? "0" : "G29", CultureInfo.InvariantCulture)));

        private void ResizeRows()
        {
            foreach (var row in rows.Values) row.Panel.Width = Math.Max(610, settings.ClientSize.Width - 38);
        }

        private void Submit(string name, string text)
        {
            if (updating || loading || closed || session == null) return;
            try
            {
                session.SetValue(name, text, DateTime.UtcNow);
                rows[name].Submitted = true;
            }
            catch (Exception ex) { session.Errors[name] = ex.Message; }
            UpdateRows();
        }

        private void Poll()
        {
            if (closed || loading) return;
            try
            {
                while (incoming.TryDequeue(out var message))
                {
                    if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_INFORMATION)
                    {
                        var info = (MAVLink.mavlink_camera_information_t)message.data;
                        var uri = Encoding.UTF8.GetString(info.cam_definition_uri ?? new byte[0]).Split('\0')[0].Trim();
                        if (!loading && definitionUri != null && (uri != definitionUri || info.cam_definition_version != definitionVersion))
                        {
                            var reload = ReloadAsync();
                            return;
                        }
                    }
                    else if (!loading) session?.Receive((MAVLink.MAVLINK_MSG_ID)message.msgid, message.data, DateTime.UtcNow);
                }
                if (!loading && session != null)
                {
                    session.Tick(DateTime.UtcNow);
                    UpdateRows();
                }
            }
            catch (Exception ex) { status.Text = "Camera settings: " + ex.Message; }
        }

        private void UpdateRows()
        {
            updating = true;
            try
            {
                var controls = session.Definition.Controls(session.Values);
                foreach (var pair in rows)
                {
                    var row = pair.Value;
                    var p = session.Definition.Parameters[pair.Key];
                    row.Panel.Visible = controls.TryGetValue(p.Name, out var options);
                    bool enabled = session.CanEdit(p.Name, controls);
                    row.Editor.Enabled = enabled;
                    if (row.Apply != null) row.Apply.Enabled = enabled;
                    session.Values.TryGetValue(p.Name, out var value);
                    if (row.Editor is ComboBox combo && options != null && !combo.DroppedDown)
                    {
                        var choices = options.ToList();
                        var selected = choices.FindIndex(o => Equals(o.Value, value));
                        if (selected < 0)
                        {
                            choices.Insert(0, new CameraParameter.Option { Label = value == null ? "Waiting for camera" : "Current: " + CameraParameter.Format(value) });
                            selected = 0;
                        }
                        if (!combo.Items.Cast<CameraParameter.Option>().Select(o => (o.Label, o.Value)).SequenceEqual(choices.Select(o => (o.Label, o.Value))))
                        {
                            combo.Items.Clear();
                            combo.Items.AddRange(choices.Cast<object>().ToArray());
                        }
                        combo.SelectedIndex = selected;
                    }
                    else if (row.Editor is SettingsSlider slider && !slider.Adjusting && !session.IsPending(p.Name))
                    {
                        if (value != null)
                        {
                            if (decimal.TryParse(CameraParameter.Format(value), NumberStyles.Float, CultureInfo.InvariantCulture, out var numeric))
                            {
                                numeric = Math.Max(row.SliderMinimum, Math.Min(row.SliderMinimum + slider.Maximum * row.SliderStep, numeric));
                                slider.Value = (int)decimal.Round((numeric - row.SliderMinimum) / row.SliderStep);
                            }
                            else slider.Value = Convert.ToDouble(value) < 0 ? slider.Minimum : slider.Maximum;
                        }
                        row.SliderValue.Text = value == null ? "" : CameraParameter.Format(value);
                    }
                    else if (row.Editor is TextBox text && !session.IsPending(p.Name) && (row.Submitted || !text.Modified))
                    {
                        text.Text = CameraParameter.Format(value);
                        text.Modified = false;
                        row.Submitted = false;
                    }
                    row.Value = value;
                    session.Errors.TryGetValue(p.Name, out var error);
                    string limits = (p.Minimum != null ? " Min: " + CameraParameter.Format(p.Minimum) : "")
                        + (p.Maximum != null ? " Max: " + CameraParameter.Format(p.Maximum) : "") + (p.Step != null ? " Step: " + p.Step : "");
                    row.Detail.Text = error ?? (session.IsPending(p.Name) ? "Applying…" : value == null && !p.WriteOnly ? "Waiting for camera" : p.Name + limits);
                    row.Detail.ForeColor = error == null ? ForeColor : Color.Red;
                }
                var readable = session.Definition.Parameters.Values.Where(p => !p.WriteOnly).ToList();
                status.Text = session.Status + " — " + readable.Count(p => session.Values.ContainsKey(p.Name)) + "/" + readable.Count + " settings read";
            }
            finally { updating = false; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !closed)
            {
                closed = true;
                timer.Stop();
                timer.Dispose();
                download?.Cancel();
                target.parent.OnPacketReceived -= PacketReceived;
            }
            base.Dispose(disposing);
        }
    }
}
