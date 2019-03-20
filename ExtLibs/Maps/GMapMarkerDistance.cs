using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerDistance : GMapMarker
    {
        public Pen Pen = new Pen(Brushes.Green, 3);
        public Pen Pen2 = new Pen(Brushes.Red, 3);

        // m
        public int wprad = 9000;
        public double tolerance;


        public GMapMarkerDistance(PointLatLng p,double km,double tol)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;
            Pen2.DashStyle = DashStyle.Dash;
            
            wprad = (int)(km * 1000);

            tolerance = tol;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width / 2, -Size.Height / 2);
        }

        public override void OnRender(IGraphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            double width =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0)) * 1000.0);
            double height =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                    Overlay.Control.FromLocalToLatLng(Overlay.Control.Height, 0)) * 1000.0);
            double m2pixelwidth = Overlay.Control.Width / width;
            double m2pixelheight = Overlay.Control.Height / height;

            GPoint loc = new GPoint((int)(LocalPosition.X - (m2pixelwidth * wprad * 2)), LocalPosition.Y);
            GPoint loc2 = new GPoint((int)(LocalPosition.X - (m2pixelwidth * wprad * tolerance * 2)), LocalPosition.Y);

            int x = LocalPosition.X - Offset.X - (int)(Math.Abs(loc.X - LocalPosition.X) / 2);
            int y = LocalPosition.Y - Offset.Y - (int)Math.Abs(loc.X - LocalPosition.X) / 2;
            int widtharc = (int)Math.Abs(loc.X - LocalPosition.X);
            int heightarc = (int)Math.Abs(loc.X - LocalPosition.X);

            int x2 = LocalPosition.X - Offset.X - (int)(Math.Abs(loc2.X - LocalPosition.X) / 2);
            int y2 = LocalPosition.Y - Offset.Y - (int)Math.Abs(loc2.X - LocalPosition.X) / 2;
            int widtharc2 = (int)Math.Abs(loc2.X - LocalPosition.X);
            int heightarc2 = (int)Math.Abs(loc2.X - LocalPosition.X);

            if (Overlay.Control.Zoom > 3)
            {
                if(widtharc > 0 && widtharc < 10000)
                    g.DrawArc(Pen, new System.Drawing.Rectangle(x, y, widtharc, heightarc), 0, 360);
                if(widtharc2 > 0 && widtharc2 < 10000 && tolerance != 0)
                    g.DrawArc(Pen2, new System.Drawing.Rectangle(x2, y2, widtharc2, heightarc2), 0, 360);
            }
        }
    }
}