using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerPlane : GMapMarker
    {
        private readonly Bitmap icon = global::MissionPlanner.Maps.Resources.planeicon;

        private readonly Bitmap icon1 = global::MissionPlanner.Maps.Resources.planeicon1;
        private readonly Bitmap icon2 = global::MissionPlanner.Maps.Resources.planeicon2;
        private readonly Bitmap icon3 = global::MissionPlanner.Maps.Resources.planeicon3;
        private readonly Bitmap icon4 = global::MissionPlanner.Maps.Resources.planeicon4;
        private readonly Bitmap icon5 = global::MissionPlanner.Maps.Resources.planeicon5;
        private readonly Bitmap icon6 = global::MissionPlanner.Maps.Resources.planeicon6;

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;
        float radius = -1;
        int which = 0;

        public GMapMarkerPlane(int which, PointLatLng p, float heading, float cog, float nav_bearing, float target, float radius)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            this.radius = radius;
            this.which = which;
            Size = icon.Size;
        }

        public override void OnRender(IGraphics g)
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

                var scaledradius = radius * (float)m2pixelwidth;

                if (radius < -1 && alpha < -1)
                {
                    // fixme 

                    float p1 = (float)Math.Cos((cog) * MathHelper.deg2rad) * scaledradius + scaledradius;

                    float p2 = (float)Math.Sin((cog) * MathHelper.deg2rad) * scaledradius + scaledradius;

                    g.DrawArc(new Pen(Color.HotPink, 2), p1, p2, Math.Abs(scaledradius) * 2, Math.Abs(scaledradius) * 2, cog, alpha);
                }

                else if (radius > 1 && alpha > 1)
                {
                    // correct

                    float p1 = (float)Math.Cos((cog - 180) * MathHelper.deg2rad) * scaledradius + scaledradius;

                    float p2 = (float)Math.Sin((cog - 180) * MathHelper.deg2rad) * scaledradius + scaledradius;

                    g.DrawArc(new Pen(Color.HotPink, 2), -p1, -p2, scaledradius * 2, scaledradius * 2, cog - 180, alpha);
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
            // 'which' variable simply selects different coloured plane icon/s from the resource library
            if (which == 0)
                g.DrawImageUnscaled(icon, icon.Width / -2, icon.Height / -2);
            if (which == 1)
                g.DrawImageUnscaled(icon1, icon1.Width / -2, icon1.Height / -2);
            if (which == 2)
                g.DrawImageUnscaled(icon2, icon2.Width / -2, icon2.Height / -2);
            if (which == 3)
                g.DrawImageUnscaled(icon3, icon3.Width / -2, icon3.Height / -2);
            if (which == 4)
                g.DrawImageUnscaled(icon4, icon4.Width / -2, icon4.Height / -2);
            if (which == 5)
                g.DrawImageUnscaled(icon5, icon5.Width / -2, icon5.Height / -2);
            if (which == 6)
                g.DrawImageUnscaled(icon6, icon6.Width / -2, icon6.Height / -2);

            

            g.Transform = temp;
        }
    }
}