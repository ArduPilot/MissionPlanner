using System;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.Utilities;
using SvgNet.SvgGdi;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerHeli : GMapMarker_IconSelectable
    {
        private Bitmap icon;

        float heading = 0;
        float cog = -1;
        float target = -1;

        public GMapMarkerHeli(PointLatLng p, float heading, float cog, float target)
            : base(p)
        {
            this.heading = heading;
            this.cog = cog;
            this.target = target;
            icon = FrameIcon();// global::MissionPlanner.Maps.Resources.heli;
            Size = icon.Size;
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

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
            //g.DrawLine(new Pen(Color.Green, 2), 0.0f, 0.0f, (float)Math.Cos((nav_bearing - 90) * MathHelper.deg2rad) * length, (float)Math.Sin((nav_bearing - 90) * MathHelper.deg2rad) * length);
            g.DrawLine(new Pen(Color.Black, 2), 0.0f, 0.0f, (float) Math.Cos((cog - 90)*MathHelper.deg2rad)*length,
                (float) Math.Sin((cog - 90)*MathHelper.deg2rad)*length);
            g.DrawLine(new Pen(Color.Orange, 2), 0.0f, 0.0f, (float) Math.Cos((target - 90)*MathHelper.deg2rad)*length,
                (float) Math.Sin((target - 90)*MathHelper.deg2rad)*length);
            // anti NaN
            try
            {
                g.RotateTransform(heading);
            }
            catch
            {
            }
            g.DrawImage(icon, icon.Width/-2 + 2, icon.Height/-2, icon.Width, icon.Height );

            g.Transform = temp;
        }

        /// <summary>
        /// Defile default icon of Heli.
        /// </summary>
        /// <param name="icon_index">Not used in Heli.</param>
        /// <returns>default icon of Heli.</returns>
        protected override Bitmap DefaultIcon(int icon_index)
        {
            return global::MissionPlanner.Maps.Resources.heli;
        }
    }
}