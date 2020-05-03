using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerRover : GMapMarkerBase
    {
        static readonly System.Drawing.Size SizeSt =
            new System.Drawing.Size(global::MissionPlanner.Maps.Resources.rover.Width,
                global::MissionPlanner.Maps.Resources.rover.Height);

        float heading = 0;
        float cog = -1;
        float target = -1;
        float nav_bearing = -1;

        public GMapMarkerRover(PointLatLng p, float heading, float cog, float nav_bearing, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            this.nav_bearing = nav_bearing;
            Size = SizeSt;
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            // anti NaN
            try
            {
                if (DisplayHeading)
                    g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f,
                        (float) Math.Cos((heading - 90) * MathHelper.deg2rad) * length,
                        (float) Math.Sin((heading - 90) * MathHelper.deg2rad) * length);
            }
            catch
            {
            }

            if (DisplayNavBearing)
                g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f,
                    (float) Math.Cos((nav_bearing - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((nav_bearing - 90) * MathHelper.deg2rad) * length);
            if (DisplayCOG)
                g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f,
                    (float) Math.Cos((cog - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((cog - 90) * MathHelper.deg2rad) * length);
            if (DisplayTarget)
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f,
                    (float) Math.Cos((target - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((target - 90) * MathHelper.deg2rad) * length);
            // anti NaN

            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }

            g.DrawImageUnscaled(global::MissionPlanner.Maps.Resources.rover,
                Size.Width / -2,
                Size.Height / -2);

            g.Transform = temp;
        }
    }
}