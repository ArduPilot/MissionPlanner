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
            int y = 0;
            {
                Size extent = e.MeasureString(desc, this.Font).ToSize();

                var mid = extent.Width / 2;

                e.DrawString(desc, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - mid, 5);

                y = extent.Height;
            }
            //
            {
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
                catch(FormatException)
                {
                    numberformat = "0.00";
                    numb = scaled_number.ToString(numberformat);
                }


                var charWidth = Math.Max(this.charWidth, numb.Length) + 1;
                Size extent = e.MeasureString("0".PadLeft(charWidth, '0'), new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                float hRatio = (this.Height - y) / (float)(extent.Height);
                float wRatio = this.Width / (float)extent.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize = (newSize * ratio);// * 0.75f; // pixel to points

                newSize -= newSize % 5;

                if (newSize < 8 || newSize > 999999)
                    newSize = 8;

                extent = e.MeasureString(numb, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                e.DrawString(numb, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style), new SolidBrush(this.numberColor), this.Width / 2 - extent.Width / 2, y + ((this.Height - y) / 2 - extent.Height / 2));
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

        float newSize = 8;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
