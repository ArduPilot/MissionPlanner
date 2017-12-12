using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerPlane : GMapMarker
    {
        private readonly Bitmap icon = global::MissionPlanner.Properties.Resources.planeicon;

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;
        float radius = -1;

        public GMapMarkerPlane(PointLatLng p, float heading, float cog, float nav_bearing, float target, float radius)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            this.radius = radius;
            Size = icon.Size;
        }

        public override void OnRender(Graphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            int length = 500;
            // anti NaN
            try
            {
                g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f, (float) Math.Cos((heading - 90)*MathHelper.deg2rad)*length,
                    (float) Math.Sin((heading - 90)*MathHelper.deg2rad)*length);
            }
            catch
            {
            }
            g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float) Math.Cos((nav_bearing - 90)*MathHelper.deg2rad)*length,
                (float) Math.Sin((nav_bearing - 90)*MathHelper.deg2rad)*length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*MathHelper.deg2rad)*length,
                (float) Math.Sin((cog - 90)*MathHelper.deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*MathHelper.deg2rad)*length,
                (float) Math.Sin((target - 90)*MathHelper.deg2rad)*length);
            // anti NaN
            try
            {
                float desired_lead_dist = 100;

                double width =
                (Overlay.Control.MapProvider.Projection.GetDistance(Overlay.Control.FromLocalToLatLng(0, 0),
                     Overlay.Control.FromLocalToLatLng(Overlay.Control.Width, 0))*1000.0);
                double m2pixelwidth = Overlay.Control.Width/width;

                float alpha = (float)(((desired_lead_dist * (float)m2pixelwidth) / radius) * MathHelper.rad2deg);

                if (radius < -1 && alpha > 1)
                {
                    // fixme 

                    float p1 = (float)Math.Cos((cog) * MathHelper.deg2rad) * radius + radius;

                    float p2 = (float)Math.Sin((cog) * MathHelper.deg2rad) * radius + radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(radius) * 2, Math.Abs(radius) * 2, cog, alpha);
                }

                else if (radius > 1 && alpha > 1)
                {
                    // correct

                    float p1 = (float)Math.Cos((cog - 180) * MathHelper.deg2rad) * radius + radius;

                    float p2 = (float)Math.Sin((cog - 180) * MathHelper.deg2rad) * radius + radius;

                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, radius * 2, radius * 2, cog - 180, alpha);
                }
            }
            catch
            {
            }

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImageUnscaled(icon, icon.Width/-2, icon.Height/-2);

            g.Transform = temp;
        }
    }
}