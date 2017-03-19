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
    public class HorizontalProgressBar : ProgressBar
    {
        private string m_Text;
        int offset = 0;
        int _min = 0;
        int _max = 0;
        int _value = 0;
        bool ctladded = false;
        bool _drawlabel = true;

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
                if (_drawlabel == false)
                {
                    if (this.Parent != null && ctladded == true)
                    {
                        this.Parent.Controls.Remove(lbl);
                        this.Parent.Controls.Remove(lbl1);
                    }
                    ctladded = true;
                }
            }
        }
        System.Windows.Forms.Label lbl1 = new System.Windows.Forms.Label();
        System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();


        public HorizontalProgressBar()
            : base()
        {
            drawlbl();
            Maximum = 100;
            Minimum = 0;
        }

        public new int Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;

                _value = value;
                int ans = value + offset;
                if (ans <= base.Minimum)
                {
                    ans = base.Minimum + 1; // prevent an exception for the -1 hack
                }
                else if (ans >= base.Maximum)
                {
                    ans = base.Maximum;
                }
                base.Value = ans;
                //drawlbl();
                base.Value = ans - 1;
                //drawlbl();
                base.Value = ans;
                drawlbl();

                if (this.Parent != null && ctladded == false)
                {
                    this.Parent.Controls.Add(lbl);
                    this.Parent.Controls.Add(lbl1);
                    ctladded = true;
                }
            }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(0)]
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

                if (this.DesignMode) return;

                if (this.Parent != null && ctladded == false)
                {
                    this.Parent.Controls.Add(lbl);
                    this.Parent.Controls.Add(lbl1);
                    ctladded = true;
                }
            }
        }

        [System.ComponentModel.Browsable(true), DefaultValue(100)]
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
                    drawlbl();
                }
            }
        }

        private void drawlbl()
        {
            if (DrawLabel)
            {
                lbl.Location = new Point(this.Location.X, this.Location.Y + this.Height + 2);
                lbl.ClientSize = new Size(this.Width, 13);
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Text = m_Text;

                lbl1.Location = new Point(this.Location.X, this.Location.Y + this.Height + 15);
                lbl1.ClientSize = new Size(this.Width, 13);
                lbl1.TextAlign = ContentAlignment.MiddleCenter;
                lbl1.Text = Value.ToString();
            }

            if (minline != 0 || maxline != 0)
            {
                float range = this.Maximum - this.Minimum;
                float range2 = this.Width;
                using (Graphics e = this.CreateGraphics())
                {
                    Pen redPen = new Pen(Color.Red, 2);

                    if ((Type)this.GetType() == typeof(VerticalProgressBar))
                    {
                        range2 = this.Height;
                        if (minline != 0)
                            e.DrawLine(redPen, 0, (this.Maximum - minline) / range * range2 + 0, this.Width, (this.Maximum - minline) / range * range2 + 0);
                        if (maxline != 0)
                            e.DrawLine(redPen, 0, (this.Maximum - maxline) / range * range2 + 0, this.Width, (this.Maximum - maxline) / range * range2 + 0);
                        if (minline != 0)
                            e.DrawString(minline.ToString(), SystemFonts.DefaultFont, Brushes.Black, 5, (this.Maximum - minline) / range * range2 + 2);
                        if (maxline != 0)
                            e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, Brushes.Black, 5, (this.Maximum - maxline) / range * range2 - 15);
                    }
                    else
                    {
                        if (minline != 0)
                            e.DrawLine(redPen, (minline - this.Minimum) / range * range2 - 3, 0, (minline - this.Minimum) / range * range2 - 3, this.Height);
                        if (maxline != 0)
                            e.DrawLine(redPen, (maxline - this.Minimum) / range * range2 - 3, 0, (maxline - this.Minimum) / range * range2 - 3, this.Height);
                        if (minline != 0)
                            e.DrawString(minline.ToString(), SystemFonts.DefaultFont, Brushes.Black, (minline - this.Minimum) / range * range2 - 35, 5);
                        if (maxline != 0)
                            e.DrawString(maxline.ToString(), SystemFonts.DefaultFont, Brushes.Black, (maxline - this.Minimum) / range * range2 - 3, 5);
                    }
                }
            }
        }

        public int minline { get; set; }
        public int maxline { get; set; }

        protected new void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            drawlbl();
        }
    }
}
