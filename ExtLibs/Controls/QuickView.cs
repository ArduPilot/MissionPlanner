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

        [System.ComponentModel.Browsable(true)]
        public double number { get { return _number; } set { if (_number == value) return; _number = value; Invalidate(); } }

        string _numberformat = "0.00";
        private string _desc;
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
                _numberformat = value;
                this.Invalidate();
            }
        }

        [System.ComponentModel.Browsable(true)]
        public Color numberColor { get { return _numbercolor; } set { if (_numbercolor == value) return; _numbercolor = value; Invalidate(); } }

        public QuickView()
        {
            InitializeComponent();

            PaintSurface+= OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            e2.Surface.Canvas.Clear();
            int y = 0;
            {
                Size extent = e.MeasureString(desc, this.Font).ToSize();

                var mid = extent.Width / 2;

                e.DrawString(desc, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - mid, 0);

                y = extent.Height;
            }
            //
            {
                var numb = number.ToString(numberformat);

                Size extent = e.MeasureString(numb, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                float hRatio = (this.Height - y) / (float)(extent.Height);
                float wRatio = this.Width / (float)extent.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize = (int)(newSize * ratio);// * 0.75f; // pixel to points

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
