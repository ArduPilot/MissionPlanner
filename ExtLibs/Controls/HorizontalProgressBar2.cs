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
    public class HorizontalProgressBar2 : BSE.Windows.Forms.ProgressBar
    {
        private string m_Text;
        int offset = 0;
        int _min = 0;
        int _max = 0;
        int _value = 0;
        System.Windows.Forms.Label lbl1 = new System.Windows.Forms.Label();
        System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
        public bool reverse = false;
        int displayvalue = 0;

        public HorizontalProgressBar2()
            : base()
        {
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

                if (this.DesignMode) return;

                if (this.Parent != null)
                {
                    this.Parent.Controls.Add(lbl);
                    this.Parent.Controls.Add(lbl1);
                }
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

        private void drawlbl(Graphics e)
        {
            lbl.Location = new Point(this.Location.X, this.Location.Y + this.Height + 2);
            lbl.ClientSize = new Size(this.Width, 13);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Text = m_Text;

            lbl1.Location = new Point(this.Location.X, this.Location.Y + this.Height + 15);
            lbl1.ClientSize = new Size(this.Width, 13);
            lbl1.TextAlign = ContentAlignment.MiddleCenter;
            lbl1.Text = displayvalue.ToString();

            if (minline != 0 && maxline != 0)
            {
                float range = this.Maximum - this.Minimum;
                float range2 = this.Width;
                Pen redPen = new Pen(Color.Red, 2);

                SolidBrush mybrush = new SolidBrush(Color.FromArgb(0x40, 0x57, 0x04));

                if ((Type)this.GetType() == typeof(VerticalProgressBar2))
                {
                    e.ResetTransform();
                    range2 = this.Height;
                    if (reverse)
                    {
                        e.DrawLine(redPen, 0, (maxline - this.Minimum) / range * range2 + 0, this.Width, (maxline - this.Minimum) / range * range2 + 0);
                        e.DrawLine(redPen, 0, (minline - this.Minimum) / range * range2 + 6, this.Width, (minline - this.Minimum) / range * range2 + 6);
                        e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, mybrush, 5, (maxline - this.Minimum) / range * range2 + 2);
                        e.DrawString(minline.ToString(), SystemFonts.DefaultFont, Brushes.White, 5, (minline - this.Minimum) / range * range2 - 10);
                    }
                    else
                    {
                        e.DrawLine(redPen, 0, (this.Maximum - minline) / range * range2 + 0, this.Width, (this.Maximum - minline) / range * range2 + 0);
                        e.DrawLine(redPen, 0, (this.Maximum - maxline) / range * range2 + 6, this.Width, (this.Maximum - maxline) / range * range2 + 6);
                        e.DrawString(minline.ToString(), SystemFonts.DefaultFont, mybrush, 5, (this.Maximum - minline) / range * range2 + 2);
                        e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, Brushes.White, 5, (this.Maximum - maxline) / range * range2 - 10);
                    }
                }
                else
                {
                    if (reverse)
                    {
                        e.DrawLine(redPen, (this.Maximum - minline) / range * range2 - 0, 0, (this.Maximum - minline) / range * range2 - 0, this.Height);
                        e.DrawLine(redPen, (this.Maximum - maxline) / range * range2 - 0, 0, (this.Maximum - maxline) / range * range2 - 0, this.Height);
                        e.DrawString(minline.ToString(), SystemFonts.DefaultFont, mybrush, (this.Maximum - minline) / range * range2 - 30, 5);
                        e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, Brushes.White, (this.Maximum - maxline) / range * range2 - 0, 5);
                    }
                    else
                    {
                        e.DrawLine(redPen, (minline - this.Minimum) / range * range2 - 0, 0, (minline - this.Minimum) / range * range2 - 0, this.Height);
                        e.DrawLine(redPen, (maxline - this.Minimum) / range * range2 - 0, 0, (maxline - this.Minimum) / range * range2 - 0, this.Height);
                        e.DrawString(minline.ToString(), SystemFonts.DefaultFont, mybrush, (minline - this.Minimum) / range * range2 - 30, 5);
                        e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, Brushes.White, (maxline - this.Minimum) / range * range2 - 0, 5);
                    }
                }
            }
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
