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
    public partial class DistanceBar : UserControl
    {
        Brush brushbar = new SolidBrush(Color.FromArgb(50, Color.White));

        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.marker_05;

        public float totaldist { get; set; }
        public float traveleddist { get; set; }

        bool parentpaint = false;

        private List<float> wpdist = new List<float>();

        public void AddWPDist(float dist)
        {
            lock (wpdist)
                wpdist.Add(dist);

            totaldist = wpdist.Sum();
        }

        public void ClearWPDist()
        {
            lock (wpdist)
                wpdist.Clear();

            wpdist.Add(0);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parms = base.CreateParams;
                parms.ExStyle |= 0x20;
                return parms;
            }
        }

        public DistanceBar()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.Opaque, true);

            InitializeComponent();

            totaldist = 100;

            

            this.BackColor = Color.Transparent;

            ClearWPDist();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (this.Parent != null)
            {
                //Parent.Invalidate(this.Bounds, true);
            }



            if (totaldist <= 0)
                totaldist = 100;

            // bar

            RectangleF bar = new RectangleF(4, this.Height - this.Height / 5, this.Width - 8, this.Height / 5);

            e.Graphics.FillRectangle(brushbar, bar);

            // draw bar traveled

            RectangleF bartrav = new RectangleF(bar.X, bar.Y, bar.Width * (traveleddist / totaldist) , bar.Height);

            e.Graphics.FillRectangle(brushbar, bartrav);
            e.Graphics.FillRectangle(brushbar, bartrav);
            e.Graphics.FillRectangle(brushbar, bartrav);
            e.Graphics.FillRectangle(brushbar, bartrav);
            e.Graphics.FillRectangle(brushbar, bartrav);

            // draw dist traveled

            string dist = traveleddist.ToString("0");

            e.Graphics.DrawString(dist, this.Font, new SolidBrush(this.ForeColor), bartrav.Right, bartrav.Top - this.Font.Height);

            // draw wp dist

            lock (wpdist)
            {
                float iconwidth = this.Height / 4;
                float trav = 0;
                foreach (var disttrav in wpdist)
                {
                    trav += disttrav;

                    e.Graphics.DrawImage(icon, (bar.X + bar.Width * (trav / totaldist)) - iconwidth/2, 1, iconwidth, (this.Height / 5) * 2.5f);
                }
            }

            
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
           // base.OnParentBackColorChanged(e);
        }
    }
}
