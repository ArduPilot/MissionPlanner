using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerADSBPlane : GMapMarker
    {
        private static readonly Bitmap icong = new Bitmap(global::MissionPlanner.Maps.Resources.FW_icons_2013_logos_01, new Size(40, 40));
        private static readonly Bitmap iconr = new Bitmap(global::MissionPlanner.Maps.Resources.FW_icons_2013_logos_011, new Size(40, 40));
        private static readonly Bitmap icono = new Bitmap(global::MissionPlanner.Maps.Resources.FW_icons_2013_logos_012, new Size(40, 40));

        public float heading = 0;
        public AlertLevelOptions AlertLevel = AlertLevelOptions.Green;

        public enum AlertLevelOptions
        {
            Green,
            Orange,
            Red
        }

        public GMapMarkerADSBPlane(PointLatLng p, float heading, AlertLevelOptions alert = AlertLevelOptions.Green)
            : base(p)
        {
            this.AlertLevel = alert;
            this.heading = heading;
            Size = icong.Size;
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }

            switch (AlertLevel)
            {
                case AlertLevelOptions.Green:
                    g.DrawImageUnscaled(icong, icong.Width/-2, icong.Height/-2);
                    break;
                case AlertLevelOptions.Orange:
                    g.DrawImageUnscaled(icono, icono.Width/-2, icono.Height/-2);
                    break;
                case AlertLevelOptions.Red:
                    g.DrawImageUnscaled(iconr, iconr.Width/-2, iconr.Height/-2);
                    break;
            }

            g.Transform = temp;
        }
    }
}