using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Controls;
using OpenTK;
using SvgNet.SvgGdi;

namespace MissionPlanner.Swarm
{
    public partial class Grid: MyUserControl
    {
        int xdist = 40;
        int ydist = 40;
        float xline;
        float yline;

        public List<icon> icons = new List<icon>();
        icon mouseover = null;
        bool ismousedown = false;

        public float BGImagex = 0;
        public float BGImagey = 0;
        public float BGImagew = 1;
        public float BGImageh = 1;
        public float BGImageStepSize = 1;
        public Image BGImage;

        public float centerx = 0;
        public float centery = 0;
        private MouseEventArgs mousedownlocation;

        public delegate void UpdateOffsetsEvent(MAVState mav, float x, float y, float z, icon ico);

        public event UpdateOffsetsEvent UpdateOffsets;

        public bool Vertical { get; set; }

        public void setScale(int dist)
        {
            if (dist > 6)
            {
                ydist = (int)(dist * (this.Height/(double)this.Width));
                xdist = dist;
                if (ydist % 2 == 1)
                    ydist++;
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

            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.Resize += Grid_Resize;
        }
        private void Grid_Resize(object sender, EventArgs e)
        {
            setScale(getScale());
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            OnPaint(new PaintEventArgsI(new GdiGraphics(e.Graphics), e.ClipRectangle));
        }

        void OnPaint(PaintEventArgsI e)
        { 
            xline = (this.Width - 1)/(float) xdist;

            yline = (this.Height - 1)/(float) ydist;

            var pen = new Pen(Color.Silver);
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            if (BGImage != null)
            {
                e.Graphics.DrawImage(BGImage, BGImagex * xline, BGImagey * yline, BGImagew * xline, BGImageh * yline);
            }

            //lines
            for (float x = 0; x <= xdist; x++)
            { 
                // middle
                if (x == xdist/2)
                {
                }
                else if (x % 2 == 0)
                {
                    e.Graphics.DrawLine(Pens.Silver, x * xline, 0, x * xline, this.Height);
                }
                else
                {
                    e.Graphics.DrawLine(pen, x * xline, 0, x * xline, this.Height);
                }
            }

            for (float y = 0; y <= ydist; y++)
            {
                // middle
                if (y == ydist/2.0f)
                {
                }
                else if (y % 2 == 0)
                {
                    e.Graphics.DrawLine(Pens.Silver, 0, y * yline, this.Width, y * yline);
                }
                else
                {
                    e.Graphics.DrawLine(pen, 0, y * yline, this.Width, y * yline);
                }
            }

            // draw the middle lines
            e.Graphics.DrawLine(Pens.Green, xdist/2*xline, 0, xdist/2*xline, this.Height);
            e.Graphics.DrawLine(Pens.Green, 0, ydist/2*yline, this.Width, ydist/2*yline);

            //text
            for (float x = 1; x <= xdist; x++)
            {
                if (x%2 == 0)
                    e.Graphics.DrawString(((xdist/-2) + x + centerx).ToString("0.0"), SystemFonts.DefaultFont, Brushes.Red, x*xline,
                        0.0f, StringFormat.GenericDefault);
            }

            for (float y = 0; y <= ydist; y++)
            {
                if (y%2 == 0)
                    e.Graphics.DrawString((((ydist/-2) + y - centery) *-1).ToString("0.0"), SystemFonts.DefaultFont, Brushes.Red, 0.0f,
                        y*yline, StringFormat.GenericDefault);
            }

            //icons
            foreach (icon ico in icons)
            {
                //if (ico.interf.parent.BaseStream.IsOpen)
                {
                    if (Vertical)
                    {
                        ico.OnPaintVertical(e, xdist, ydist, this.Width, this.Height, centerx, centery);
                    }
                    else
                    {
                        ico.OnPaint(e, xdist, ydist, this.Width, this.Height, centerx, centery);
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

        public void UpdateIcon(MAVState mav, float x, float y, float z, bool movable)
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
            this.Invalidate();
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
                        mouseover.z += centery;
                    }
                    else
                    {
                        mouseover.x = xdist/-2 + e.X/xline;
                        mouseover.y = (ydist/-2 + e.Y/yline)*-1;
                        mouseover.x += centerx;
                        mouseover.y += centery;
                    }
                    /*
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
                    */
                    if (UpdateOffsets != null)
                    {
                            UpdateOffsets(mouseover.interf, mouseover.x, mouseover.y, mouseover.z ,
                                mouseover);
                    }

                    this.Invalidate();
                }
                

                return;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left && ismousedown == true && mouseover == null)
            {
                // drag
                var deltax = e.X - mousedownlocation.X;
                var deltay = e.Y - mousedownlocation.Y;

                centerx -= (deltax / (float)xline);
                centery += (deltay / (float)yline);

                mousedownlocation = e;

                this.Invalidate();
            }

            mouseover = null;

            if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                foreach (icon ico in icons)
                {
                    if (e.X > ico.bounds.Left && e.X < ico.bounds.Right
                                              && e.Y > ico.bounds.Top && e.Y < ico.bounds.Bottom)
                    {
                        mouseover = ico;
                    }
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ismousedown = true;

                mousedownlocation = e;

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
            public MAVState interf = null;
            public bool Movable = true;

            public void OnPaint(PaintEventArgsI e, int xdist, int ydist, int width, int height, float centerx, float centery)
            {
                bounds.X = width / 2 + width / xdist * (x - centerx) - icosize / 2;
                bounds.Y = height / 2 - height / ydist * (y - centery) - icosize / 2;
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

            public void OnPaintVertical(PaintEventArgsI e, int xdist, int ydist, int width, int height, float centerx, float centery)
            {
                bounds.X = width / 2 + width / xdist * (x - centerx) - icosize / 2;
                bounds.Y = height / 2 - height / ydist * (z - centery) - icosize / 2;
                bounds.Width = icosize;
                bounds.Height = icosize;


                e.Graphics.DrawPie(new Pen(Color), bounds, 0, 359);

                e.Graphics.DrawString(Name, SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Top,
                    StringFormat.GenericDefault);
                //e.Graphics.DrawString(z.ToString(), SystemFonts.DefaultFont, Brushes.Red, bounds.Right, bounds.Bottom, StringFormat.GenericDefault);
                // e.ClipRectangle.Width / 2 + e.ClipRectangle.Width / xdist * x - icosize / 2, e.ClipRectangle.Height / 2 + e.ClipRectangle.Height / ydist * y - icosize / 2, icosize, icosize                
            }

            public void MouseOver(PaintEventArgsI e)
            {
            }
        }

        public void Clear()
        {
            icons.Clear();
        }

        private void changeAltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mouseover == null)
                return;

            var mouseoverlocal = mouseover;

            string output = mouseover.z.ToString();
            if (DialogResult.OK == InputBox.Show("Alt", "Enter New Alt", ref output))
            {
                mouseoverlocal.z = float.Parse(output);

                if (UpdateOffsets != null)
                    UpdateOffsets(mouseoverlocal.interf, mouseoverlocal.x, mouseoverlocal.y, mouseoverlocal.z, mouseoverlocal);

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