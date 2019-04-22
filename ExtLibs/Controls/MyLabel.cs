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

        SolidBrush s = new SolidBrush(SystemColors.ControlText);
        SolidBrush b = new SolidBrush(SystemColors.Control);

        StringFormat stringFormat = new StringFormat();

        [System.ComponentModel.Browsable(true)]
        public bool resize { get { return autosize; } set { autosize = value; } }

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

            e.Surface.Canvas.DrawText(label, 0, Height-2, paint);
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
