using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    [Serializable]
    public class GMapMarkerAirport : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.White, 2);

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

        public GMapMarker InnerMarker;

        // m
        public int wprad = 5559;

        public void ResetColor()
        {
            if (initcolor.HasValue)
                Color = initcolor.Value;
            else
                Color = Color.White;
        }

        public GMapMarkerAirport(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width/2, -Size.Height/2);
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

            //wprad = 300;

            // undo autochange in mouse over
            //if (Pen.Color == Color.Blue)
            //  Pen.Color = Color.White;

            double width =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0))*1000.0);
            double height =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0))*1000.0);
            double m2pixelwidth = Overlay.Control.Width/width;
            double m2pixelheight = Overlay.Control.Height/height;

            GPoint loc = new GPoint((int) (LocalPosition.X - (m2pixelwidth*wprad*2)), LocalPosition.Y);
                // MainMap.FromLatLngToLocal(wpradposition);


            int x = LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X)/2);
            int y = LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X)/2;
            int widtharc = (int) Math.Abs(loc.X - LocalPosition.X);
            int heightarc = (int) Math.Abs(loc.X - LocalPosition.X);

            if (widtharc > 0 && Overlay.Control.Zoom > 3)
            {
                //g.DrawArc(Pen, new System.Drawing.Rectangle(x, y, widtharc, heightarc), 0, 360);

                g.FillPie(new SolidBrush(Color.FromArgb(25, Color.Red)), x, y, widtharc, heightarc, 0, 360);
            }
        }
    }
}