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

        double _number = -9999;

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
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            var canvas = e2.Surface.Canvas;
            canvas.Clear();

            // Calculate description area - fixed height based on font
            Size descExtent = e.MeasureString(desc, this.Font).ToSize();
            int descHeight = descExtent.Height + 10; // 5px padding top and bottom

            // Draw description centered at top
            var descMid = descExtent.Width / 2;
            e.DrawString(desc, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - descMid, 5);

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
            // 8px padding on sides and bottom, top is anchored 8px below label
            float padding = 8;
            float valueGap = 12; // Gap between arc center and value text
            float availableWidth = this.Width - padding * 2;

            // Top of gauge area starts 8px below the label
            float gaugeTop = contentAreaTop + padding;
            // Bottom of gauge area is 8px from view bottom
            float gaugeBottom = this.Height - padding;
            float totalGaugeHeight = gaugeBottom - gaugeTop;

            // Estimate value text height to reserve space below the arc
            float estimatedValueFontSize = Math.Max(12, totalGaugeHeight * 0.15f);
            float valueTextHeight = estimatedValueFontSize * 1.4f;

            // The gauge content height = arc (radius) + valueGap + valueTextHeight
            // We need to fit this within totalGaugeHeight
            // Arc area height is what's left after value text and gap
            float arcAreaHeight = totalGaugeHeight - valueTextHeight - valueGap;

            // The arc should be a half-circle, so radius is constrained by both width and height
            // For a half-circle: height = radius, width = 2*radius
            float arcRadius = Math.Min(availableWidth / 2, arcAreaHeight);

            // Total content height (arc + gap + value text)
            float totalContentHeight = arcRadius + valueGap + valueTextHeight;

            // Center the content vertically within the gauge area
            float verticalOffset = (totalGaugeHeight - totalContentHeight) / 2;

            float centerX = this.Width / 2;
            float centerY = gaugeTop + verticalOffset + arcRadius;

            // Clamp value to min/max range
            double clampedValue = Math.Max(gaugeMin, Math.Min(gaugeMax, value));

            // Calculate needle angle (180 degrees = left to right, 0 = min, 180 = max)
            double range = gaugeMax - gaugeMin;
            double normalizedValue = range > 0 ? (clampedValue - gaugeMin) / range : 0;
            float needleAngle = (float)(180 - normalizedValue * 180); // 180 = left (min), 0 = right (max)

            float arcStrokeWidth = Math.Max(6, arcRadius * 0.12f);

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
                    path.AddArc(arcRect, 180, 180);
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

                float sweepAngle = (float)(normalizedValue * 180);
                using (var path = new SkiaSharp.SKPath())
                {
                    path.AddArc(arcRect, 180, sweepAngle);
                    canvas.DrawPath(path, valuePaint);
                }
            }

            // Draw 20 tick marks along the arc (on top of the dial) with major and minor ticks
            using (var tickPaint = new SkiaSharp.SKPaint())
            {
                tickPaint.Style = SkiaSharp.SKPaintStyle.Stroke;
                tickPaint.Color = new SkiaSharp.SKColor(ForeColor.R, ForeColor.G, ForeColor.B);
                tickPaint.IsAntialias = true;

                float tickOuterRadius = arcRadius + arcStrokeWidth / 2 + 2;

                for (int i = 0; i <= 20; i++)
                {
                    float tickAngle = 180 - (i * 180f / 20); // From 180 (left) to 0 (right)
                    float angleRad = (float)(tickAngle * Math.PI / 180);

                    // Major ticks at 0, 5, 10, 15, 20 (every 5th) - longer and thicker
                    bool isMajor = (i % 5 == 0);
                    float tickLength = isMajor ? arcStrokeWidth + 4 : arcStrokeWidth / 2 + 2;
                    tickPaint.StrokeWidth = isMajor ? 3 : 1.5f;

                    float tickInnerRadius = tickOuterRadius - tickLength;

                    float outerX = centerX + (float)(tickOuterRadius * Math.Cos(angleRad));
                    float outerY = centerY - (float)(tickOuterRadius * Math.Sin(angleRad));
                    float innerX = centerX + (float)(tickInnerRadius * Math.Cos(angleRad));
                    float innerY = centerY - (float)(tickInnerRadius * Math.Sin(angleRad));

                    canvas.DrawLine(innerX, innerY, outerX, outerY, tickPaint);
                }
            }

            // Draw needle
            using (var needlePaint = new SkiaSharp.SKPaint())
            {
                needlePaint.Style = SkiaSharp.SKPaintStyle.Stroke;
                needlePaint.StrokeWidth = 3;
                needlePaint.Color = new SkiaSharp.SKColor(numberColor.R, numberColor.G, numberColor.B);
                needlePaint.IsAntialias = true;
                needlePaint.StrokeCap = SkiaSharp.SKStrokeCap.Round;

                float needleLength = arcRadius * 0.75f;
                float angleRad = (float)(needleAngle * Math.PI / 180);
                float needleEndX = centerX + (float)(needleLength * Math.Cos(angleRad));
                float needleEndY = centerY - (float)(needleLength * Math.Sin(angleRad));

                canvas.DrawLine(centerX, centerY, needleEndX, needleEndY, needlePaint);

                // Draw center dot
                needlePaint.Style = SkiaSharp.SKPaintStyle.Fill;
                canvas.DrawCircle(centerX, centerY, 5, needlePaint);
            }

            // Draw min/max labels at arc endpoints
            using (var labelPaint = new SkiaSharp.SKPaint())
            {
                labelPaint.Color = new SkiaSharp.SKColor(ForeColor.R, ForeColor.G, ForeColor.B);
                labelPaint.IsAntialias = true;
                labelPaint.TextSize = Math.Max(10, arcRadius * 0.18f);

                string minLabel = gaugeMin.ToString("0.#");
                string maxLabel = gaugeMax.ToString("0.#");

                float labelY = centerY + labelPaint.TextSize + 6; // 6px padding from arc bottom

                // Min label (left side, hugging the arc endpoint)
                float minX = centerX - arcRadius - 3;
                canvas.DrawText(minLabel, minX, labelY, labelPaint);

                // Max label (right side, hugging the arc endpoint)
                float maxLabelWidth = labelPaint.MeasureText(maxLabel);
                float maxX = centerX + arcRadius - maxLabelWidth + 3;
                canvas.DrawText(maxLabel, maxX, labelY, labelPaint);
            }

            // Draw value below the arc, centered horizontally
            float valueFontSize = Math.Max(12, arcRadius * 0.35f);

            using (var valueFont = new Font(this.Font.FontFamily, valueFontSize, this.Font.Style))
            {
                Size extent = e.MeasureString(formattedValue, valueFont).ToSize();
                float x = centerX - extent.Width / 2;
                // Position value below the center point with 4px gap
                float y = centerY + valueGap;
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
