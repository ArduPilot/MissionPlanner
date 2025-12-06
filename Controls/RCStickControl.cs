using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// A visual representation of an RC transmitter stick showing two axes
    /// </summary>
    public class RCStickControl : UserControl
    {
        private int _horizontalValue = 1500;
        private int _verticalValue = 1500;
        private int _minimum = 800;
        private int _maximum = 2200;
        private string _horizontalLabel = "Roll";
        private string _verticalLabel = "Pitch";

        // Calibration min/max lines
        private int _horizontalMinLine = 0;
        private int _horizontalMaxLine = 0;
        private int _verticalMinLine = 0;
        private int _verticalMaxLine = 0;

        [Browsable(true), Category("Data")]
        [DefaultValue(1500)]
        public int HorizontalValue
        {
            get => _horizontalValue;
            set { _horizontalValue = value; Invalidate(); }
        }

        [Browsable(true), Category("Data")]
        [DefaultValue(1500)]
        public int VerticalValue
        {
            get => _verticalValue;
            set { _verticalValue = value; Invalidate(); }
        }

        [Browsable(true), Category("Data")]
        [DefaultValue(800)]
        public int Minimum
        {
            get => _minimum;
            set { _minimum = value; Invalidate(); }
        }

        [Browsable(true), Category("Data")]
        [DefaultValue(2200)]
        public int Maximum
        {
            get => _maximum;
            set { _maximum = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance")]
        public string HorizontalLabel
        {
            get => _horizontalLabel;
            set { _horizontalLabel = value; Invalidate(); }
        }

        [Browsable(true), Category("Appearance")]
        public string VerticalLabel
        {
            get => _verticalLabel;
            set { _verticalLabel = value; Invalidate(); }
        }

        // Calibration min/max line properties
        [Browsable(true), Category("Calibration")]
        public int HorizontalMinLine
        {
            get => _horizontalMinLine;
            set { _horizontalMinLine = value; Invalidate(); }
        }

        [Browsable(true), Category("Calibration")]
        public int HorizontalMaxLine
        {
            get => _horizontalMaxLine;
            set { _horizontalMaxLine = value; Invalidate(); }
        }

        [Browsable(true), Category("Calibration")]
        public int VerticalMinLine
        {
            get => _verticalMinLine;
            set { _verticalMinLine = value; Invalidate(); }
        }

        [Browsable(true), Category("Calibration")]
        public int VerticalMaxLine
        {
            get => _verticalMaxLine;
            set { _verticalMaxLine = value; Invalidate(); }
        }

        public RCStickControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            Size = new Size(150, 150);
            MinimumSize = new Size(100, 100);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Get theme colors
            Color bgColor = ThemeManager.ControlBGColor;
            Color borderColor = ThemeManager.BannerColor1;
            Color stickColor = ThemeManager.HorizontalPBValueColor;
            Color textColor = ThemeManager.TextColor;
            Color crosshairColor = Color.FromArgb(100, textColor);

            int padding = 25;
            Rectangle stickArea = new Rectangle(padding, padding,
                Width - 2 * padding, Height - 2 * padding);

            // Draw background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, stickArea);
            }

            // Draw border
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.DrawRectangle(borderPen, stickArea);
            }

            // Draw crosshairs
            using (var crossPen = new Pen(crosshairColor, 1))
            {
                crossPen.DashStyle = DashStyle.Dash;
                // Vertical center line
                g.DrawLine(crossPen,
                    stickArea.X + stickArea.Width / 2, stickArea.Y,
                    stickArea.X + stickArea.Width / 2, stickArea.Y + stickArea.Height);
                // Horizontal center line
                g.DrawLine(crossPen,
                    stickArea.X, stickArea.Y + stickArea.Height / 2,
                    stickArea.X + stickArea.Width, stickArea.Y + stickArea.Height / 2);
            }

            // Draw calibration range indicators
            if (_horizontalMinLine > 0 || _horizontalMaxLine > 0 || _verticalMinLine > 0 || _verticalMaxLine > 0)
            {
                using (var rangePen = new Pen(Color.Red, 2))
                {
                    // Horizontal min/max lines (vertical bars)
                    if (_horizontalMinLine > 0)
                    {
                        float xMin = MapValue(_horizontalMinLine, stickArea.X, stickArea.X + stickArea.Width);
                        g.DrawLine(rangePen, xMin, stickArea.Y, xMin, stickArea.Y + stickArea.Height);
                    }
                    if (_horizontalMaxLine > 0)
                    {
                        float xMax = MapValue(_horizontalMaxLine, stickArea.X, stickArea.X + stickArea.Width);
                        g.DrawLine(rangePen, xMax, stickArea.Y, xMax, stickArea.Y + stickArea.Height);
                    }
                    // Vertical min/max lines (horizontal bars)
                    if (_verticalMinLine > 0)
                    {
                        float yMax = MapValueInverted(_verticalMinLine, stickArea.Y, stickArea.Y + stickArea.Height);
                        g.DrawLine(rangePen, stickArea.X, yMax, stickArea.X + stickArea.Width, yMax);
                    }
                    if (_verticalMaxLine > 0)
                    {
                        float yMin = MapValueInverted(_verticalMaxLine, stickArea.Y, stickArea.Y + stickArea.Height);
                        g.DrawLine(rangePen, stickArea.X, yMin, stickArea.X + stickArea.Width, yMin);
                    }
                }
            }

            // Calculate stick position
            float stickX = MapValue(_horizontalValue, stickArea.X, stickArea.X + stickArea.Width);
            float stickY = MapValueInverted(_verticalValue, stickArea.Y, stickArea.Y + stickArea.Height);

            // Draw stick position indicator
            int stickRadius = 12;
            using (var stickBrush = new SolidBrush(stickColor))
            {
                g.FillEllipse(stickBrush, stickX - stickRadius, stickY - stickRadius,
                    stickRadius * 2, stickRadius * 2);
            }
            using (var stickOutlinePen = new Pen(textColor, 2))
            {
                g.DrawEllipse(stickOutlinePen, stickX - stickRadius, stickY - stickRadius,
                    stickRadius * 2, stickRadius * 2);
            }

            // Draw labels
            using (var font = new Font("Segoe UI", 9, FontStyle.Bold))
            using (var labelBrush = new SolidBrush(textColor))
            {
                // Horizontal label (below)
                var hLabelSize = g.MeasureString(_horizontalLabel, font);
                g.DrawString(_horizontalLabel, font, labelBrush,
                    stickArea.X + (stickArea.Width - hLabelSize.Width) / 2,
                    stickArea.Y + stickArea.Height + 3);

                // Vertical label (rotated on the left)
                var state = g.Save();
                g.TranslateTransform(stickArea.X - 5, stickArea.Y + stickArea.Height / 2);
                g.RotateTransform(-90);
                var vLabelSize = g.MeasureString(_verticalLabel, font);
                g.DrawString(_verticalLabel, font, labelBrush, -vLabelSize.Width / 2, -vLabelSize.Height);
                g.Restore(state);
            }
        }

        private float MapValue(int value, float minOut, float maxOut)
        {
            float normalized = (float)(value - _minimum) / (_maximum - _minimum);
            normalized = Math.Max(0, Math.Min(1, normalized));
            return minOut + normalized * (maxOut - minOut);
        }

        private float MapValueInverted(int value, float minOut, float maxOut)
        {
            float normalized = (float)(value - _minimum) / (_maximum - _minimum);
            normalized = Math.Max(0, Math.Min(1, normalized));
            return maxOut - normalized * (maxOut - minOut);
        }
    }
}
