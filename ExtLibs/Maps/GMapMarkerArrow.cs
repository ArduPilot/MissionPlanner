using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Drawing;

namespace MissionPlanner.Maps
{
    public class GMapMarkerArrow: GMapMarker
    {
        public float Bearing = 0;

        public GMapMarkerArrow(PointLatLng p, float Bearing, Pen color)
           : base(p)
        {
            this.Bearing = Bearing;
            Offset = new Point(0, 0);
            this.Color = color;

        }

        static readonly Point[] Arrow = new Point[] { new Point(-7, 7), new Point(0, -7), new Point(7, 7)/*, new Point(0, 2)*/ };

        public Pen Color { get; set; }

        public override void OnRender(IGraphics g)
        {
            if (Math.Abs(LocalPosition.X) > 100000 || Math.Abs(LocalPosition.Y) > 100000)
                return;

            if(Overlay.Control.Zoom < 16)
                return;


            var old = g.Transform;

            g.TranslateTransform(this.LocalPosition.X - this.Offset.X, this.LocalPosition.Y - this.Offset.Y);
            g.RotateTransform(Bearing - Overlay.Control.Bearing);
            g.DrawLines(Color, Arrow);

            g.Transform = old;
        }
    }
}