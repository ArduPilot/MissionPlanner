using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// profiling showed that the built in Label function was using alot of call time.
    /// </summary>
    public partial class MyLabel : Control //: Label
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

                if (noofchars <= label.Length && resize)
                {
                    noofchars = label.Length;
                    using (Graphics g = Graphics.FromHwnd(this.Handle))
                    {
                        SizeF textSize = TextRenderer.MeasureText(g, value, this.Font);
                        this.Width = (int)textSize.Width;
                    }                    
                }

                if (this.Visible && this.ThisReallyVisible())
                    this.Invalidate();
            }
        }


        public override void Refresh()
        {
            base.Refresh();
        }

        protected override void OnParentBindingContextChanged(EventArgs e)
        {
            base.OnParentBindingContextChanged(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            try
            {
                base.OnVisibleChanged(e);
            }
            catch (Exception)
            {
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

           // TextRenderer.DrawText(e.Graphics, label, this.Font, new Point(0, 0), ForeColor);

            e.Graphics.DrawString(label, this.Font, s, new PointF(0, this.Height / 2.0f), stringFormat);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.FillRectangle(b, this.ClientRectangle);

            base.OnPaintBackground(pevent);
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
