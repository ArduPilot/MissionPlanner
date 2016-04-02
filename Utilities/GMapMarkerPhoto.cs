using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Properties;

namespace MissionPlanner.Utilities
{
    [Serializable]
    public class GMapMarkerPhoto : GMapMarker
    {
        static Bitmap localcache2 = new Bitmap(Resources.camera_icon, 32, 32);

        public double Alt { get; set; }

        public static double hfov = 63;
        public static double vfov = 43;

        public static double rolltrim = 0;
        public static double pitchtrim = 0;
        public static double yawtrim = 0;

        public bool drawfootprint = true;

        MAVLink.mavlink_camera_feedback_t local;

        GMapPolygon footprintpoly;

        public GMapMarkerPhoto(MAVLink.mavlink_camera_feedback_t mark)
            : base(new PointLatLng(mark.lat/1e7, mark.lng/1e7))
        {
            local = mark;
            Offset = new Point(-16, -16);
            Size = localcache2.Size;
            Alt = mark.alt_msl;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
            ToolTipText = "Photo" + "\nAlt: " + mark.alt_msl;

            Tag = mark.time_usec;

            var footprint = ImageProjection.calc(new PointLatLngAlt(Position.Lat, Position.Lng, Alt), local.roll - rolltrim,
                local.pitch - pitchtrim, local.yaw - yawtrim, hfov, vfov);

            footprintpoly = new GMapPolygon(footprint.ConvertAll(x => x.Point()), "FP"+mark.time_usec);

            footprintpoly.Fill = Brushes.Transparent;
        }

        public override void OnRender(Graphics g)
        {
            g.DrawImageUnscaled(localcache2, LocalPosition.X, LocalPosition.Y);

            if (drawfootprint || IsMouseOver)
            {
                Overlay.Control.UpdatePolygonLocalPosition(footprintpoly);
                footprintpoly.OnRender(g);
            }
        }
    }
}