using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;

namespace MissionPlanner.Controls
{
    public partial class QuickView : SkiaSharp.Views.Desktop.SKControl
    {
        [System.ComponentModel.Browsable(true)]
        public string desc
        {
            get { return _desc; } set { if (_desc == value) return; _desc = value; Invalidate(); }
        }

        private double _number = 0;
        private bool _isConnected = false;

        /// <summary>
        /// Set to true when connected to a device. When false, the view is dimmed.
        /// </summary>
        [System.ComponentModel.Browsable(true)]
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected == value)
                    return;
                _isConnected = value;
                Invalidate();
            }
        }

        // Optionally set this value to force the text size to fit this many "0" characters 
        // in the view. This allows for matching the text size of adjacent views.
        public int charWidth = -1;

        // Optional offset and scale for unit conversions
        public double offset = 0;
        public double scale = 1;

        // Gauge mode properties
        public bool isGauge = false;
        public double gaugeMin = 0;
        public double gaugeMax = 100;

        // Blink state for out-of-range values
        private Timer _blinkTimer;
        private bool _blinkVisible = true;
        private bool _isOutOfRange = false;

        [System.ComponentModel.Browsable(true)]
        public double number
        {
            get { return _number; }
            set
            {
                lock (this)
                {
                    if (_number.Equals(value))
                        return;
                    _number = value;
                    Invalidate();
                }
            }
        }

        string _numberformat = "0.00";
        private string _desc = "";
        private Color _numbercolor;

        [System.ComponentModel.Browsable(true)]
        public string numberformat
        {
            get
            {
                return _numberformat;
            }
            set
            {
                if (_numberformat.Equals(value))
                    return;
                _numberformat = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.Browsable(true)]
        public Color numberColor { get { return _numbercolor; } set { if (_numbercolor == value) return; _numbercolor = value; Invalidate(); } }

        //We use this property as a backup store for the numberColor, so it is possible to change numberColor temporary.
        public Color numberColorBackup { get; set; }

        public QuickView()
        {
            InitializeComponent();

            PaintSurface+= OnPaintSurface;

            DoubleBuffered = true;

            // Initialize blink timer (2Hz = 250ms interval for on/off cycle)
            _blinkTimer = new Timer();
            _blinkTimer.Interval = 250;
            _blinkTimer.Tick += (s, e) =>
            {
                _blinkVisible = !_blinkVisible;
                Invalidate();
            };
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            var canvas = e2.Surface.Canvas;
            canvas.Clear();

            // Dim the view when not connected
            if (!_isConnected)
            {
                using (var paint = new SkiaSharp.SKPaint())
                {
                    paint.Color = paint.Color.WithAlpha(128);
                    canvas.SaveLayer(paint);
                }
            }

            // Calculate description font size that fits the available width
            float descFontSize = this.Font.Size;
            Size descExtent;
            Font descFont = this.Font;

            // Scale down description font if it doesn't fit
            descExtent = e.MeasureString(desc, descFont).ToSize();
            while (descExtent.Width > this.Width * 0.95f && descFontSize > 6)
            {
                descFontSize -= 1;
                descFont = new Font(this.Font.FontFamily, descFontSize, this.Font.Style);
                descExtent = e.MeasureString(desc, descFont).ToSize();
            }

            int descHeight = descExtent.Height + 10; // 5px padding top and bottom

            // Draw description centered at top
            var descMid = descExtent.Width / 2;
            e.DrawString(desc, descFont, new SolidBrush(this.ForeColor), this.Width / 2 - descMid, 5);

            // Calculate available space for content (below description)
            int contentAreaTop = descHeight;
            int contentAreaHeight = this.Height - contentAreaTop;

            if (contentAreaHeight <= 0)
                return;

            // Format the number with scale/offset and TimeSpan support
            string numb;
            double scaled_number = scale * number + offset;

            try
            {
                if (numberformat.Contains(":"))
                {
                    numb = TimeSpan.FromSeconds(scaled_number).ToString(numberformat);
                }
                else
                {
                    numb = scaled_number.ToString(numberformat);
                }
            }
            catch (FormatException)
            {
                numberformat = "0.00";
                numb = scaled_number.ToString(numberformat);
            }

            if (isGauge)
            {
                // Draw gauge mode (180-degree half circle)
                DrawGauge(canvas, e, contentAreaTop, contentAreaHeight, scaled_number, numb);
            }
            else
            {
                // Draw number mode (original behavior)
                DrawNumber(e, contentAreaTop, contentAreaHeight, numb);
            }

            if (!_isConnected)
            {
                canvas.Restore();
            }
        }

        private void DrawNumber(SkiaGraphics e, int numberAreaTop, int numberAreaHeight, string numb)
        {
            // Use a reference string for consistent sizing (prevents jumping when digits change)
            // charWidth allows forcing consistent sizing across multiple QuickViews
            var refCharWidth = Math.Max(this.charWidth, numb.Length) + 1;
            string refString = "0".PadLeft(Math.Max(refCharWidth, 5), '0');

            // Start with a base font size and calculate the optimal size
            float fontSize = 8f;
            float maxFontSize = 200f;
            float optimalSize = fontSize;

            // Binary search for optimal font size that fits
            while (fontSize <= maxFontSize)
            {
                using (var testFont = new Font(this.Font.FontFamily, fontSize, this.Font.Style))
                {
                    Size testExtent = e.MeasureString(refString, testFont).ToSize();

                    if (testExtent.Width <= this.Width * 0.95f && testExtent.Height <= numberAreaHeight * 0.9f)
                    {
                        optimalSize = fontSize;
                        fontSize += 2f;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Round down to nearest 2 for stability
            optimalSize = Math.Max(8f, optimalSize - (optimalSize % 2));

            using (var numberFont = new Font(this.Font.FontFamily, optimalSize, this.Font.Style))
            {
                Size extent = e.MeasureString(numb, numberFont).ToSize();

                // Center the number in the available space below the description
                float x = this.Width / 2 - extent.Width / 2;
                float y = numberAreaTop + (numberAreaHeight / 2 - extent.Height / 2);

                e.DrawString(numb, numberFont, new SolidBrush(this.numberColor), x, y);
            }
        }

        private void DrawGauge(SkiaSharp.SKCanvas canvas, SkiaGraphics e, int contentAreaTop, int contentAreaHeight, double value, string formattedValue)
        {
            // Check if value is out of range and manage blink timer
            bool outOfRange = value < gaugeMin || value > gaugeMax;
            if (outOfRange != _isOutOfRange)
            {
                _isOutOfRange = outOfRange;
                if (outOfRange)
                {
                    _blinkVisible = true;
                    _blinkTimer.Start();
                }
                else
                {
                    _blinkTimer.Stop();
                    _blinkVisible = true;
                }
            }

            // 8px padding on all sides
            float padding = 8;
            float availableWidth = this.Width - padding * 2;
            float availableHeight = contentAreaHeight - padding * 2;

            // For 270-degree arc: the arc spans from 135° (bottom-left) to 405° (bottom-right)
            // The gap is at the bottom (45° on each side)
            // Height needed = radius + radius * sin(45°) = radius * (1 + 0.707) ≈ 1.707 * radius
            // Width needed = 2 * radius

            // Calculate radius that fits in available space
            float radiusFromWidth = availableWidth / 2;
            float radiusFromHeight = availableHeight / 1.707f;
            float arcRadius = Math.Min(radiusFromWidth, radiusFromHeight);

            float centerX = this.Width / 2;
            // Position center so the arc is vertically centered in available space
            // The arc extends from -radius (top) to radius * sin(45°) ≈ 0.707 * radius (bottom)
            float arcTopExtent = arcRadius;
            float arcBottomExtent = arcRadius * 0.707f;
            float totalArcHeight = arcTopExtent + arcBottomExtent;
            float centerY = contentAreaTop + padding + arcTopExtent + (availableHeight - totalArcHeight) / 2;

            // Clamp value to min/max range for arc drawing
            double clampedValue = Math.Max(gaugeMin, Math.Min(gaugeMax, value));

            // Calculate sweep angle (270 degrees total, starting from 135°)
            double range = gaugeMax - gaugeMin;
            double normalizedValue = range > 0 ? (clampedValue - gaugeMin) / range : 0;

            float arcStrokeWidth = Math.Max(6, arcRadius * 0.12f);
            float startAngle = 135; // Bottom-left (45° below horizontal)
            float totalSweep = 270; // 270 degrees

            // Draw arc background (gray track)
            using (var arcPaint = new SkiaSharp.SKPaint())
            {
                arcPaint.Style = SkiaSharp.SKPaintStyle.Stroke;
                arcPaint.StrokeWidth = arcStrokeWidth;
                arcPaint.Color = new SkiaSharp.SKColor(80, 80, 80);
                arcPaint.IsAntialias = true;
                arcPaint.StrokeCap = SkiaSharp.SKStrokeCap.Round;

                var arcRect = new SkiaSharp.SKRect(
                    centerX - arcRadius,
                    centerY - arcRadius,
                    centerX + arcRadius,
                    centerY + arcRadius);

                using (var path = new SkiaSharp.SKPath())
                {
                    path.AddArc(arcRect, startAngle, totalSweep);
                    canvas.DrawPath(path, arcPaint);
                }
            }

            // Draw colored arc (value indicator)
            using (var valuePaint = new SkiaSharp.SKPaint())
            {
                valuePaint.Style = SkiaSharp.SKPaintStyle.Stroke;
                valuePaint.StrokeWidth = arcStrokeWidth;
                valuePaint.Color = new SkiaSharp.SKColor(numberColor.R, numberColor.G, numberColor.B);
                valuePaint.IsAntialias = true;
                valuePaint.StrokeCap = SkiaSharp.SKStrokeCap.Round;

                var arcRect = new SkiaSharp.SKRect(
                    centerX - arcRadius,
                    centerY - arcRadius,
                    centerX + arcRadius,
                    centerY + arcRadius);

                float sweepAngle = (float)(normalizedValue * totalSweep);
                using (var path = new SkiaSharp.SKPath())
                {
                    path.AddArc(arcRect, startAngle, sweepAngle);
                    canvas.DrawPath(path, valuePaint);
                }
            }

            // Draw tick marks along the 270-degree arc
            using (var tickPaint = new SkiaSharp.SKPaint())
            {
                tickPaint.Style = SkiaSharp.SKPaintStyle.Stroke;
                tickPaint.Color = new SkiaSharp.SKColor(ForeColor.R, ForeColor.G, ForeColor.B);
                tickPaint.IsAntialias = true;

                float tickOuterRadius = arcRadius + arcStrokeWidth / 2 + 2;

                for (int i = 0; i <= 20; i++)
                {
                    // Map tick position to angle: 0 = 135° (min), 20 = 405° (max)
                    float tickAngle = startAngle + (i * totalSweep / 20);
                    float angleRad = tickAngle * (float)Math.PI / 180;

                    // Major ticks at 0, 5, 10, 15, 20 (every 5th) - longer and thicker
                    bool isMajor = (i % 5 == 0);
                    float tickLength = isMajor ? arcStrokeWidth + 4 : arcStrokeWidth / 2 + 2;
                    tickPaint.StrokeWidth = isMajor ? 3 : 1.5f;

                    float tickInnerRadius = tickOuterRadius - tickLength;

                    float outerX = centerX + tickOuterRadius * (float)Math.Cos(angleRad);
                    float outerY = centerY + tickOuterRadius * (float)Math.Sin(angleRad);
                    float innerX = centerX + tickInnerRadius * (float)Math.Cos(angleRad);
                    float innerY = centerY + tickInnerRadius * (float)Math.Sin(angleRad);

                    canvas.DrawLine(innerX, innerY, outerX, outerY, tickPaint);
                }
            }

            // Draw value centered in the middle of the arc - blinks when out of range
            // Scale font to fit within the inner arc area
            float maxValueWidth = arcRadius * 1.4f; // Allow value to span most of the inner circle
            float valueFontSize = Math.Max(12, arcRadius * 0.40f);
            Font valueFont = new Font(this.Font.FontFamily, valueFontSize, this.Font.Style);
            Size extent = e.MeasureString(formattedValue, valueFont).ToSize();

            // Scale down if value doesn't fit
            while (extent.Width > maxValueWidth && valueFontSize > 8)
            {
                valueFontSize -= 1;
                valueFont = new Font(this.Font.FontFamily, valueFontSize, this.Font.Style);
                extent = e.MeasureString(formattedValue, valueFont).ToSize();
            }

            float x = centerX - extent.Width / 2;
            // Position value slightly above center to give more visual padding
            float y = centerY - extent.Height / 2 - arcRadius * 0.05f;

            if (_blinkVisible)
            {
                e.DrawString(formattedValue, valueFont, new SolidBrush(this.numberColor), x, y);
            }
        }

        public override void Refresh()
        {
            if (this.Visible)
                base.Refresh();
        }

        protected override void WndProc(ref Message m) // seems to crash here on linux... so try ignore it
        {
            try
            {
                base.WndProc(ref m);
            }
            catch { }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.Visible && this.ThisReallyVisible())
                base.OnInvalidated(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
