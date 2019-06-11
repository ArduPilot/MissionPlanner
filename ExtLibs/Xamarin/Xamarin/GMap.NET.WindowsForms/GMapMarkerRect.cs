using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities.Drawing;


namespace GMap.NET.WindowsForms
{
    /// <summary>
    /// used to override the drawing of the waypoint box bounding
    /// </summary>
    [Serializable]
    public class GMapMarkerRect : GMapMarker
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

        public int wprad = 0;

        public void ResetColor()
        {
            if (initcolor.HasValue)
                Color = initcolor.Value;
            else
                Color = Color.White;
        }

        public GMapMarkerRect(PointLatLng p)
            : base(p)
        {
            Pen.DashStyle = DashStyle.Dash;

            // do not forget set Size of the marker
            // if so, you shall have no event on it ;}
            Size = new System.Drawing.Size(50, 50);
            Offset = new System.Drawing.Point(-Size.Width/2, -Size.Height/2 - 20);
        }

        public override void OnRender(IGraphics g)
        {
            base.OnRender(g);

            if (wprad == 0 || Overlay.Control == null)
                return;

            // if we have drawn it, then keep that color
            if (!initcolor.HasValue)
                Color = Color.White;

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

            if (m2pixelheight > 0.5 && !double.IsInfinity(m2pixelheight))
                g.DrawArc(Pen,
                    new System.Drawing.Rectangle(
                        LocalPosition.X - Offset.X - (int) (Math.Abs(loc.X - LocalPosition.X)/2),
                        LocalPosition.Y - Offset.Y - (int) Math.Abs(loc.X - LocalPosition.X)/2,
                        (int) Math.Abs(loc.X - LocalPosition.X), (int) Math.Abs(loc.X - LocalPosition.X)), 0, 360);
        }
    }
}