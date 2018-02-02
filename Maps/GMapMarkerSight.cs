using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerSight : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 5);

        public List<PointLatLng> po = new List<PointLatLng>();

        public float shiftx;
        public float shifty;
        public PointLatLng xy;

        public bool use;

        public Color Color
        {
            get { return Pen.Color; }
            set
            {
                if (!initcolor.HasValue) initcolor = value;
                Pen.Color = value;
            }
        }

        Color? initcolor = null;

        public void ResetColor()
        {
            if (initcolor.HasValue)
                Color = initcolor.Value;
            else
                Color = Color.White;
        }

        public GMapMarkerSight(PointLatLng p, List<PointLatLng> pointslist, bool yes)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);

            po = pointslist;
            xy = p;

            use = yes;
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

            PointF[] point;
            List<PointF> points = new List<PointF>();

            foreach (PointLatLng loca in po)
            {
                points.Add(new PointF(Overlay.Control.FromLatLngToLocal(loca).X, Overlay.Control.FromLatLngToLocal(loca).Y));

            }

            point = points.ToArray();

            shiftx = Overlay.Control.FromLatLngToLocal(xy).X;// - point[180].X);
            shifty = Overlay.Control.FromLatLngToLocal(xy).Y;// - point[180].Y);

            if (use)
            {
                for (int i = 0; i < point.Count(); i++)
                {
                    float x = point[i].X - 869;// - shiftx - Offset.X;// - (int)(Math.Abs(loc.X - shiftx) / 2);
                    float y = point[i].Y - 431;// -shifty - Offset.Y;// - (int)Math.Abs(loc.X - shiftx) / 2;

                    point[i] = new PointF(x, y);
                }
            }

            else
            {
                for (int i = 0; i < point.Count(); i++)
                {
                    float x = point[i].X - 713;// - Overlay.Control.FromLatLngToLocal(xy).X;// - shiftx - Offset.X;// - (int)(Math.Abs(loc.X - shiftx) / 2);
                    float y = point[i].Y - 476; // - Overlay.Control.FromLatLngToLocal(xy).Y; // -shifty - Offset.Y;// - (int)Math.Abs(loc.X - shiftx) / 2;

                    point[i] = new PointF(x, y);
                }
            }
            
            if (Overlay.Control.Zoom > 3)
            {
                g.DrawPolygon( Pen, point);
            }
        }
    }
}
