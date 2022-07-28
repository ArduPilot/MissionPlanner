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
            e2.Surface.Canvas.Clear();

            // Calculate description area - fixed height based on font
            Size descExtent = e.MeasureString(desc, this.Font).ToSize();
            int descHeight = descExtent.Height + 10; // 5px padding top and bottom

            // Draw description centered at top
            var descMid = descExtent.Width / 2;
            e.DrawString(desc, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - descMid, 5);

            // Calculate available space for number (below description)
            int numberAreaTop = descHeight;
            int numberAreaHeight = this.Height - numberAreaTop;

            if (numberAreaHeight <= 0)
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
