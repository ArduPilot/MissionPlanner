using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Simple always-on-top toast that shows parameter write confirmations and auto-dismisses.
    /// </summary>
    public class ParamWriteToast : Form
    {
        private readonly Timer _timer;
        private readonly Form _owner;
        private const double DisplayDurationMs = 2000;
        private const double FadeDurationMs = 500;
        private readonly double _initialOpacity = 0.75;
        private double _remainingMs = DisplayDurationMs;

        public ParamWriteToast(string message, Form owner)
        {
            _owner = owner;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            AutoSize = false;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(34, 34, 34);
            ForeColor = Color.White;
            Padding = Padding.Empty;
            Opacity = _initialOpacity;

            DoubleBuffered = true;

            var label = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(360, 0),
                Text = message,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = Padding.Empty
            };

            var padding = 12;
            label.Location = new Point(padding, padding);

            Controls.Add(label);

            // Size the toast to fit the label with consistent padding.
            var preferred = label.PreferredSize;
            ClientSize = new Size(preferred.Width + padding * 2, preferred.Height + padding * 2);

            _timer = new Timer { Interval = 50 };
            _timer.Tick += (s, e) =>
            {
                _remainingMs -= _timer.Interval;

                if (_remainingMs <= 0)
                {
                    _timer.Stop();
                    Close();
                    return;
                }

                if (_remainingMs <= FadeDurationMs)
                {
                    var fraction = Math.Max(0, _remainingMs / FadeDurationMs);
                    Opacity = _initialOpacity * fraction;
                }
            };
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                // Tool window style keeps it out of alt-tab.
                cp.ExStyle |= 0x80;
                // WS_EX_NOACTIVATE prevents focus stealing.
                cp.ExStyle |= 0x08000000;
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            PositionToast();
            _timer.Start();
        }

        private void PositionToast()
        {
            const int margin = 16;
            Rectangle bounds;

            if (_owner != null && !_owner.IsDisposed)
            {
                bounds = _owner.Bounds;
            }
            else
            {
                bounds = Screen.PrimaryScreen?.WorkingArea ?? Screen.PrimaryScreen.Bounds;
            }

            // Place near bottom-right of the owning form/screen.
            var x = bounds.Right - Width - margin;
            var y = bounds.Bottom - Height - margin;
            Location = new Point(Math.Max(margin, x), Math.Max(margin, y));
        }
    }
}
