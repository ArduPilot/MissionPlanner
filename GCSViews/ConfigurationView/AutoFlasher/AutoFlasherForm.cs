using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView.AutoFlasher
{
    /// <summary>
    /// Single-purpose dialog that drives the AutoFlasherService. Built entirely
    /// in code so it requires no Designer.cs or .resx pipeline.
    /// </summary>
    public sealed class AutoFlasherForm : Form
    {
        private readonly Button _btnPickFirmware;
        private readonly TextBox _txtFirmware;
        private readonly Label _lblStatus;
        private readonly ProgressBar _progress;
        private readonly TextBox _log;
        private readonly Button _btnCancel;

        private AutoFlasherService _service;
        private string _firmwarePath;

        public AutoFlasherForm()
        {
            Text = "Auto-Flash Firmware (USB hot-plug)";
            ClientSize = new Size(720, 420);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(640, 360);

            var top = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 88,
                ColumnCount = 3,
                RowCount = 2,
                Padding = new Padding(10)
            };
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            var lblFw = new Label
            {
                Text = "Firmware :",
                Anchor = AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
            };
            _txtFirmware = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
            };
            _btnPickFirmware = new Button
            {
                Text = "Parcourir…",
                Dock = DockStyle.Fill,
            };
            _btnPickFirmware.Click += BtnPickFirmware_Click;

            _lblStatus = new Label
            {
                Text = "Sélectionne un firmware (.apj de préférence) puis branche la carte USB.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true,
            };
            top.Controls.Add(lblFw, 0, 0);
            top.Controls.Add(_txtFirmware, 1, 0);
            top.Controls.Add(_btnPickFirmware, 2, 0);
            top.Controls.Add(_lblStatus, 0, 1);
            top.SetColumnSpan(_lblStatus, 3);

            _progress = new ProgressBar
            {
                Dock = DockStyle.Top,
                Height = 22,
                Minimum = 0,
                Maximum = 100,
            };

            _log = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font(FontFamily.GenericMonospace, 8.5f),
                BackColor = SystemColors.Window,
            };

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
            };
            _btnCancel = new Button { Text = "Fermer", Width = 100 };
            _btnCancel.Click += (s, e) => Close();
            bottom.Controls.Add(_btnCancel);

            Controls.Add(_log);
            Controls.Add(_progress);
            Controls.Add(bottom);
            Controls.Add(top);

            FormClosed += (s, e) =>
            {
                try { _service?.Dispose(); } catch { }
            };
        }

        private void BtnPickFirmware_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Title = "Choisir le firmware ArduPilot",
                Filter = "ArduPilot firmware (*.apj;*.px4;*.hex)|*.apj;*.px4;*.hex|All files (*.*)|*.*",
            })
            {
                if (ofd.ShowDialog(this) != DialogResult.OK) return;
                if (!File.Exists(ofd.FileName)) return;

                _firmwarePath = ofd.FileName;
                _txtFirmware.Text = _firmwarePath;
                StartListening();
            }
        }

        private void StartListening()
        {
            try
            {
                try { _service?.Dispose(); } catch { }

                _service = new AutoFlasherService(_firmwarePath);
                _service.Status += s => InvokeIfNeeded(() =>
                {
                    _lblStatus.Text = s;
                    AppendLog(s);
                });
                _service.Progress += pct => InvokeIfNeeded(() =>
                {
                    var v = (int)Math.Max(0, Math.Min(100, pct));
                    _progress.Value = v;
                });
                _service.Completed += (ok, msg) => InvokeIfNeeded(() =>
                {
                    _lblStatus.Text = msg;
                    AppendLog((ok ? "[OK] " : "[FAIL] ") + msg);
                    _progress.Value = ok ? 100 : 0;
                    _btnPickFirmware.Enabled = true;
                });

                _btnPickFirmware.Enabled = false;
                _service.Start();
                AppendLog("Listener USB armé. Branche la carte maintenant.");
            }
            catch (Exception ex)
            {
                AppendLog("Erreur : " + ex.Message);
                _btnPickFirmware.Enabled = true;
            }
        }

        private void InvokeIfNeeded(Action a)
        {
            if (IsDisposed) return;
            if (InvokeRequired) BeginInvoke((Action)(() => { try { a(); } catch { } }));
            else { try { a(); } catch { } }
        }

        private void AppendLog(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            var stamped = DateTime.Now.ToString("HH:mm:ss.fff") + "  " + line + Environment.NewLine;
            if (_log.IsDisposed) return;
            _log.AppendText(stamped);
        }
    }
}
