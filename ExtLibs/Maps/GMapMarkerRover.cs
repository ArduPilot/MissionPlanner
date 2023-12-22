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
            this.Heading = heading;
            this.Cog = cog;
            this.Target = target;
            this.Nav_bearing = nav_bearing;
            Size = SizeSt;
        }

        public float Heading { get => heading; set => heading = value; }
        public float Cog { get => cog; set => cog = value; }
        public float Target { get => target; set => target = value; }
        public float Nav_bearing { get => nav_bearing; set => nav_bearing = value; }

        public override void OnRender(IGraphics g)
        {
            if(IsHidden)
            {
                return;
            }

            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            g.RotateTransform(-Overlay.Control.Bearing);

            // anti NaN
            try
            {
                if (DisplayHeading)
                    g.DrawLine(new Pen(Color.Red, 2), 0.0f, 0.0f,
                        (float) Math.Cos((Heading - 90) * MathHelper.deg2rad) * length,
                        (float) Math.Sin((Heading - 90) * MathHelper.deg2rad) * length);
            }
            catch
            {
            }

            if (DisplayNavBearing)
                g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f,
                    (float) Math.Cos((Nav_bearing - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((Nav_bearing - 90) * MathHelper.deg2rad) * length);
            if (DisplayCOG)
                g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f,
                    (float) Math.Cos((Cog - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((Cog - 90) * MathHelper.deg2rad) * length);
            if (DisplayTarget)
                g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f,
                    (float) Math.Cos((Target - 90) * MathHelper.deg2rad) * length,
                    (float) Math.Sin((Target - 90) * MathHelper.deg2rad) * length);
            // anti NaN

            try
            {
                g.RotateTransform(Heading);
            }
            catch
            {
            }
#if NET472_OR_GREATER
            var img = Resources.rover;
            var ia = new System.Drawing.Imaging.ImageAttributes();
            if (IsTransparent)
            {
                // Draw image with transparency using a color matrix
                var cm = new System.Drawing.Imaging.ColorMatrix { Matrix33 = 0.39f };
                ia.SetColorMatrix(cm, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
            }
            g.DrawImage(img, new Rectangle(-img.Width / 2, -img.Width / 2, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);
#else
            g.DrawImageUnscaled(global::MissionPlanner.Maps.Resources.rover,
                Size.Width / -2,
                Size.Height / -2);
#endif

            g.Transform = temp;
        }
    }
}