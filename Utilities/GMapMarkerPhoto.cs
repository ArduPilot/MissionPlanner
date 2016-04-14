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
        static Bitmap localcache1 = new Bitmap(Resources.camera_icon_G, 20, 20);
        static Bitmap localcache2 = new Bitmap(Resources.camera_icon, 20, 20);

        public double Alt { get; set; }

        public static double hfov = 63;
        public static double vfov = 43;

        public static double rolltrim = 0;
        public static double pitchtrim = 0;
        public static double yawtrim = 0;

        public bool drawfootprint = true;

        MAVLink.mavlink_camera_feedback_t local;

        public GMapPolygon footprintpoly;
        private bool shotBellowMinInterval;

        public GMapMarkerPhoto(MAVLink.mavlink_camera_feedback_t mark, bool shotBellowMinInterval = false)
            : base(new PointLatLng(mark.lat/1e7, mark.lng/1e7))
        {
            local = mark;
            this.shotBellowMinInterval = shotBellowMinInterval;
            Offset = new Point(-localcache1.Width/2, -localcache1.Height/2);
            Size = localcache1.Size;
            Alt = mark.alt_msl;
            ToolTipMode = MarkerTooltipMode.OnMouseOver;
            ToolTipText = "Photo" + "\nAlt: " + mark.alt_msl + "\nNo: "+ mark.img_idx;

            Tag = mark.time_usec;

            var footprint = ImageProjection.calc(new PointLatLngAlt(Position.Lat, Position.Lng, Alt), local.roll - rolltrim,
                local.pitch - pitchtrim, local.yaw - yawtrim, hfov, vfov);

            footprintpoly = new GMapPolygon(footprint.ConvertAll(x => x.Point()), "FP"+mark.time_usec);

            footprintpoly.Fill = Brushes.Transparent;
        }

        public override void OnRender(Graphics g)
        {
            if (shotBellowMinInterval)
                g.DrawImageUnscaled(localcache2, LocalPosition.X, LocalPosition.Y);
            else
                g.DrawImageUnscaled(localcache1, LocalPosition.X, LocalPosition.Y);

            if (drawfootprint || IsMouseOver)
            {
                Overlay.Control.UpdatePolygonLocalPosition(footprintpoly);
                footprintpoly.OnRender(g);
            }
        }
    }
}