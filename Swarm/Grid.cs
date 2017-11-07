using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner;

namespace MissionPlanner.Swarm
{
    public partial class Grid : UserControl
    {
        int xdist = 40;
        int ydist = 40;
        float xline;
        float yline;

        List<icon> icons = new List<icon>();
        icon mouseover = null;
        bool ismousedown = false;

        public delegate void UpdateOffsetsEvent(MAVLinkInterface mav, float x, float y, float z, icon ico);

        public event UpdateOffsetsEvent UpdateOffsets;

        public bool Vertical { get; set; }

        public void setScale(int dist)
        {
            if (dist > 6)
            {
                ydist = xdist = dist;
            }
            this.Invalidate();
        }

        public int getScale()
        {
            return xdist;
        }

        public Grid()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            xline = (this.Width - 1)/(float) xdist;

            yline = (this.Height - 1)/(float) ydist;

            //lines
            for (float x = 0; x <= xdist; x++)
            {
                // middle
                if (x == xdist/2)
                {
                }
                else
                {
                    e.Graphics.DrawLine(Pens.Silver, x*xline, 0, x*xline, this.Height);
                }
            }

            for (float y = 0; y <= ydist; y++)
            {
                // middle
                if (y == ydist/2.0f)
                {
                }
                else
                {
                    e.Graphics.DrawLine(Pens.Silver, 0, y*yline, this.Width, y*yline);
                }
            }

            // draw the middle lines
            e.Graphics.DrawLine(Pens.Green, xdist/2*xline, 0, xdist/2*xline, this.Height);
            e.Graphics.DrawLine(Pens.Green, 0, ydist/2*yline, this.Width, ydist/2*yline);

            //text
            for (float x = 1; x <= xdist; x++)
            {
                if (x%2 == 0)
                    e.Graphics.DrawString(((xdist/-2) + x).ToString(), SystemFonts.DefaultFont, Brushes.Red, x*xline,
                        0.0f, StringFormat.GenericDefault);
            }

            for (float y = 0; y <= ydist; y++)
            {
                if (y%2 == 0)
                    e.Graphics.DrawString((((ydist/-2) + y)*-1).ToString(), SystemFonts.DefaultFont, Brushes.Red, 0.0f,
                        y*yline, StringFormat.GenericDefault);
            }

            //icons
            foreach (icon ico in icons)
            {
                if (ico.interf.BaseStream.IsOpen)
                {
                    if (Vertical)
                    {
                        ico.OnPaintVertical(e, xdist, ydist, this.Width, this.Height);
                    }
                    else
                    {
                        ico.OnPaint(e, xdist, ydist, this.Width, this.Height);
                    }
                }
            }

            //
            if (mouseover != null)
                mouseover.MouseOver(e);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
            base.OnResize(e);
        }

        public void UpdateIcon(MAVLinkInterface mav, float x, float y, float z, bool movable)
        {
            foreach (var icon in icons)
            {
                if (icon.interf == mav)
                {
                    icon.Movable = movable;
                    if (!movable)
                    {
                        icon.x = 0;
                        icon.y = 0;
                        icon.z = 0;
                        icon.Color = Color.Blue;
                    }
                    else
                    {
                        icon.x = x;
                        icon.y = y;
                        icon.z = z;
                        icon.Color = Color.Red;
                    }
                    this.Invalidate();
                    return;
                }
            }

            Console.WriteLine("ADD MAV {0} {1} {2}", x, y, z);
            icons.Add(new icon() {interf = mav, y = y, z = z, x = x, Movable = movable, Name = mav.ToString()});
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left && ismousedown == true && mouseover != null)
            {
                if (mouseover.Movable)
                {
                    if (Vertical)
                    {
                        mouseover.z = (ydist/-2 + e.Y/yline)*-1;
                    }
                    else
                    {
                        mouseover.x = xdist/-2 + e.X/xline;
                        mouseover.y = (ydist/-2 + e.Y/yline)*-1;
                    }

                    if (mouseover.x < xdist/-2.0f)
                    {
                        mouseover.x = xdist/-2.0f;
                    }
                    if (mouseover.x > xdist/2.0f)
                    {
                        mouseover.x = xdist/2.0f;
                    }
                    if (mouseover.y < ydist/-2.0f)
                    {
                        mouseover.y = ydist/-2.0f;
                    }
                    if (mouseover.y > ydist/2.0f)
                    {
                        mouseover.y = ydist/2.0f;
                    }

                    if (UpdateOffsets != null)
                        UpdateOffsets(mouseover.interf, mouseover.x, mouseover.y, mouseover.z, mouseover);

                    this.Invalidate();
                }

                return;
            }

            mouseover = null;

            foreach (icon ico in icons)
            {
                if (e.X > ico.bounds.Left && e.X < ico.bounds.Right
                    && e.Y > ico.bounds.Top && e.Y < ico.bounds.Bottom)
                {
                    mouseover = ico;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ismousedown = true;

                foreach (icon ico in icons)
                {
                    if (e.X > ico.bounds.Left && e.X < ico.bounds.Right
                        && e.Y > ico.bounds.Top && e.Y < ico.bounds.Bottom)
                    {
                        mouseover = ico;
                    }
                }
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // context menu
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            ismousedown = false;
        }

        public class icon
        {
            public float x = 0;
            public float y = 0;
            public float z = 10;
            public int icosize = 20;
            public RectangleF bounds = new RectangleF();
            public Color Color = Color.Red;
            public String Name = "";
            public MAVLinkInterface interf = null;
            public bool Movable = true;

            public void OnPaint(PaintEventArgs e, int xdist, int ydist, int width, int height)
            {
                bounds.X = width/2 + width/xdist*x - icosize/2;
                bounds.Y = height/2 - height/ydist*y - icosize/2;
                bounds.Width = icosize;
                bounds.Height = icosize;


                e.Graphics.DrawPie(new Pen(Color), bounds, 0, 359);

                e.Graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Top,
                    StringFormat.GenericDefault);
                e.Graphics.DrawString(z.ToString(), SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Bottom,
                    StringFormat.GenericDefault);
                // e.ClipRectangle.Width / 2 + e.ClipRectangle.Width / xdist * x - icosize / 2, e.ClipRectangle.Height / 2 + e.ClipRectangle.Height / ydist * y - icosize / 2, icosize, icosize                
            }

            public bool MouseInside()
            {
                return false;
            }

            public void OnPaintVertical(PaintEventArgs e, int xdist, int ydist, int width, int height)
            {
                bounds.X = width/2 + width/xdist*x - icosize/2;
                bounds.Y = height/2 - height/ydist*z - icosize/2;
                bounds.Width = icosize;
                bounds.Height = icosize;


                e.Graphics.DrawPie(new Pen(Color), bounds, 0, 359);

                e.Graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Top,
                    StringFormat.GenericDefault);
                //e.Graphics.DrawString(z.ToString(), SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Bottom, StringFormat.GenericDefault);
                // e.ClipRectangle.Width / 2 + e.ClipRectangle.Width / xdist * x - icosize / 2, e.ClipRectangle.Height / 2 + e.ClipRectangle.Height / ydist * y - icosize / 2, icosize, icosize                
            }

            public void MouseOver(PaintEventArgs e)
            {
            }
        }

        private void changeAltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mouseover == null)
                return;

            string output = mouseover.z.ToString();
            if (DialogResult.OK == InputBox.Show("Alt", "Enter New Alt", ref output))
            {
                mouseover.z = float.Parse(output);

                if (UpdateOffsets != null)
                    UpdateOffsets(mouseover.interf, mouseover.x, mouseover.y, mouseover.z, mouseover);

                this.Invalidate();
            }
        }

        private void CHK_vertical_CheckedChanged(object sender, EventArgs e)
        {
            Vertical = CHK_vertical.Checked;
            this.Invalidate();
        }
    }
}