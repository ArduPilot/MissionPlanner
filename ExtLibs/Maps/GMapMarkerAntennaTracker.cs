using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerAntennaTracker : GMapMarker
    {
        private readonly Bitmap icon = global::MissionPlanner.Maps.Resources.Antenna_Tracker_01;

        float heading = 0;
        private float target = 0;

        public GMapMarkerAntennaTracker(PointLatLng p, float heading, float target)
            : base(p)
        {
            Size = icon.Size;
            this.heading = heading;
            this.target = target;
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            int length = 500;

            try
            {
                // heading
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*MathHelper.deg2rad)*length,
                    (float) Math.Sin((heading - 90)*MathHelper.deg2rad)*length);

                // target
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*MathHelper.deg2rad)*length,
                    (float) Math.Sin((target - 90)*MathHelper.deg2rad)*length);
            }
            catch
            {
            }

            g.DrawImage(icon, -20, -20, 40, 40);

            g.Transform = temp;
        }
    }
}