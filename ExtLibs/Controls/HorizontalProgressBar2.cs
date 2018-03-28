using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AGaugeApp;
using System.IO.Ports;
using System.Threading;


using System.Security.Cryptography.X509Certificates;

using System.Net;
using System.Net.Sockets;
using System.Xml; // config file
using System.Runtime.InteropServices; // dll imports
using log4net;

using MissionPlanner;
using System.Reflection;

using System.IO;

using System.Drawing.Drawing2D;

namespace MissionPlanner.Controls
{
    public class HorizontalProgressBar2:  BSE.Windows.Forms.ProgressBar
    {
        private string m_Text;
        int offset = 0;
        int _min = 0;
        int _max = 0;
        int _value = 0;
        public bool reverse = false;
        int displayvalue = 0;
        bool _drawlabel = true;

        //BSE.Windows.Forms.ProgressBar basepb = new BSE.Windows.Forms.ProgressBar();

        [System.ComponentModel.Browsable(true),
System.ComponentModel.Category("Mine"),
System.ComponentModel.Description("draw text under Bar")]
        public bool DrawLabel
        {
            get
            {
                return _drawlabel;
            }
            set
            {
                _drawlabel = value;
            }
        }

        internal class proxyvpb: BSE.Windows.Forms.ProgressBar
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.TranslateTransform(0, e.Graphics.ClipBounds.Height);
                e.Graphics.RotateTransform(270);
                e.Graphics.ScaleTransform((float)this.Height / (float)this.Width, (float)this.Width / (float)this.Height);
                base.OnPaint(e);
            }
        }

        public HorizontalProgressBar2()
        {
            if ((Type) this.GetType() == typeof (VerticalProgressBar2))
            {
                //basepb = new proxyvpb();
            }

            this.SetStyle(
    ControlStyles.ResizeRedraw |
    ControlStyles.OptimizedDoubleBuffer |
    ControlStyles.AllPaintingInWmPaint |
    ControlStyles.SupportsTransparentBackColor |
    ControlStyles.UserPaint, true);
        }

        public new int Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                _value = value;
                displayvalue = _value;

                if (reverse)
                {
                    int dif = _value - Minimum;
                    _value = Maximum - dif;
                }

                int ans = _value + offset;
                if (ans <= base.Minimum)
                {
                    ans = base.Minimum + 1; // prevent an exception for the -1 hack
                }
                else if (ans >= base.Maximum)
                {
                    ans = base.Maximum;
                }

                base.Value = ans;

                this.Invalidate();

                if (this.DesignMode) return;
            }
        }

        public new int Minimum
        {
            get { return _min; }
            set
            {
                _min = value;
                if (_min < 0)
                {
                    base.Minimum = 0; offset = (_max - value) / 2; base.Maximum = _max - value;
                }
                else
                {
                    base.Minimum = value;
                }
            }
        }

        public new int Maximum { get { return _max; } set { _max = value; base.Maximum = value; } }

        [System.ComponentModel.Browsable(true),
System.ComponentModel.Category("Mine"),
System.ComponentModel.Description("Text under Bar")]
        public string Label
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                }
            }
        }

             [System.ComponentModel.Browsable(true),
System.ComponentModel.Category("Mine"),
System.ComponentModel.Description("values scaled for display")]
        public float DisplayScale
        {
            get { return _displayscale; }
            set { _displayscale = value; }
        }

        private float _displayscale = 1;

        StringFormat drawFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        private void drawlbl(Graphics e)
        {
            if (DrawLabel)
            {
                var rect = new RectangleF(0, 0, Width, Height);
                var tran = e.Transform;
                e.ResetTransform();
                if (Width < Height)
                {
                    drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                }
                else
                {
                    drawFormat.FormatFlags = 0;
                }

                e.DrawString((m_Text + "  " + (Value * _displayscale).ToString()+" ").Trim(), this.Font, new SolidBrush(this.ForeColor), rect, drawFormat);
                e.Transform = tran;
            }

            if (minline != 0 && maxline != 0)
            {
                float range = this.Maximum - this.Minimum;
                float range2 = this.Width;
                Pen redPen = new Pen(Color.Red, 2);

                SolidBrush mybrush = new SolidBrush(Color.FromArgb(0x40, 0x57, 0x04));

                if ((Type)this.GetType() == typeof(VerticalProgressBar2))
                {
                    e.ResetTransform();
                    range2 = base.Height;
                    if (reverse)
                    {
                        e.DrawLine(redPen, 0, (maxline - this.Minimum) / range * range2 + 0, this.Width, (maxline - this.Minimum) / range * range2 + 0);
                        e.DrawLine(redPen, 0, (minline - this.Minimum) / range * range2 + 6, this.Width, (minline - this.Minimum) / range * range2 + 6);
                        e.DrawString((maxline * _displayscale).ToString(), SystemFonts.DefaultFont, mybrush, 5, (maxline - this.Minimum) / range * range2 + 2);
                        e.DrawString((minline * _displayscale).ToString(), SystemFonts.DefaultFont, Brushes.White, 5, (minline - this.Minimum) / range * range2 - 10);
                    }
                    else
                    {
                        e.DrawLine(redPen, 0, (this.Maximum - minline) / range * range2 + 0, this.Width, (this.Maximum - minline) / range * range2 + 0);
                        e.DrawLine(redPen, 0, (this.Maximum - maxline) / range * range2 + 6, this.Width, (this.Maximum - maxline) / range * range2 + 6);
                        e.DrawString((minline * _displayscale).ToString(), SystemFonts.DefaultFont, mybrush, 5, (this.Maximum - minline) / range * range2 + 2);
                        e.DrawString((maxline * _displayscale).ToString(), SystemFonts.DefaultFont, Brushes.White, 5, (this.Maximum - maxline) / range * range2 - 10);
                    }
                }
                else
                {
                    if (reverse)
                    {
                        e.DrawLine(redPen, (this.Maximum - minline) / range * range2 - 0, 0, (this.Maximum - minline) / range * range2 - 0, this.Height);
                        e.DrawLine(redPen, (this.Maximum - maxline) / range * range2 - 0, 0, (this.Maximum - maxline) / range * range2 - 0, this.Height);
                        e.DrawString((minline * _displayscale).ToString(), SystemFonts.DefaultFont, mybrush, (this.Maximum - minline) / range * range2 - 30, 5);
                        e.DrawString((maxline * _displayscale).ToString(), SystemFonts.DefaultFont, Brushes.White, (this.Maximum - maxline) / range * range2 - 0, 5);
                    }
                    else
                    {
                        e.DrawLine(redPen, (minline - this.Minimum) / range * range2 - 0, 0, (minline - this.Minimum) / range * range2 - 0, this.Height);
                        e.DrawLine(redPen, (maxline - this.Minimum) / range * range2 - 0, 0, (maxline - this.Minimum) / range * range2 - 0, this.Height);
                        e.DrawString((minline * _displayscale).ToString(), SystemFonts.DefaultFont, mybrush, (minline - this.Minimum) / range * range2 - 30, 5);
                        e.DrawString((maxline * _displayscale).ToString(), SystemFonts.DefaultFont, Brushes.White, (maxline - this.Minimum) / range * range2 - 0, 5);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            //base.Width = this.Width;
            //base.Height = this.Height - 30;
            base.OnResize(e);
        }

        public int minline { get; set; }
        public int maxline { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            drawlbl(e.Graphics);
        }
    }
}
