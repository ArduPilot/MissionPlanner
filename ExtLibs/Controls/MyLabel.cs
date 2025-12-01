using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// profiling showed that the built in Label function was using alot of call time.
    /// </summary>
    public  class MyLabel : SkiaSharp.Views.Desktop.SKControl
    {
        string label = "";
        int noofchars = 0;
        bool autosize = false;
        int _leftPadding = 0;
        ContentAlignment _textAlign = ContentAlignment.MiddleLeft;

        SolidBrush s = new SolidBrush(SystemColors.ControlText);
        SolidBrush b = new SolidBrush(SystemColors.Control);

        StringFormat stringFormat = new StringFormat();

        [System.ComponentModel.Browsable(true)]
        public bool resize { get { return autosize; } set { autosize = value; } }

        [System.ComponentModel.Browsable(true)]
        public int LeftPadding { get { return _leftPadding; } set { _leftPadding = value; this.Invalidate(); } }

        [System.ComponentModel.Browsable(true)]
        public ContentAlignment TextAlign { get { return _textAlign; } set { _textAlign = value; this.Invalidate(); } }

        public MyLabel()
        {
            stringFormat.Alignment = StringAlignment.Near;
            stringFormat.LineAlignment = StringAlignment.Center;

            PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            e.Surface.Canvas.DrawRect(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width,
                ClientRectangle.Height, new SKPaint() {Color = BackColor.SKColor(), Style = SKPaintStyle.Fill});

            var paint = Font.SKPaint();
            paint.Color = ForeColor.SKColor();

            if (noofchars <= label.Length && resize)
            {
                noofchars = label.Length;
                float textSize = paint.MeasureText(label);
                this.Width = (int)textSize;
            }

            float x = _leftPadding;
            if (_textAlign == ContentAlignment.MiddleCenter || _textAlign == ContentAlignment.TopCenter || _textAlign == ContentAlignment.BottomCenter)
            {
                float textWidth = paint.MeasureText(label);
                x = (Width - textWidth) / 2;
            }
            else if (_textAlign == ContentAlignment.MiddleRight || _textAlign == ContentAlignment.TopRight || _textAlign == ContentAlignment.BottomRight)
            {
                float textWidth = paint.MeasureText(label);
                x = Width - textWidth - _leftPadding;
            }

            e.Surface.Canvas.DrawText(label, x, Height-2, paint);
        }

        public override string Text
        {
            get
            {
                return label;
            }
            set
            {

                if (value == null)
                    return;

                if (label == value)
                    return;

                label = value;



                if (this.Visible && this.ThisReallyVisible())
                    this.Invalidate();
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                b = new SolidBrush(value);
                this.Invalidate();
            }
        }


        public override System.Drawing.Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                s = new SolidBrush(value);
                this.Invalidate();
            }
        }
        
        protected override void WndProc(ref Message m) // seems to crash here on linux... so try ignore it
        {
            try
            {
                base.WndProc(ref m);
            }
            catch { }
        }
    }
}
