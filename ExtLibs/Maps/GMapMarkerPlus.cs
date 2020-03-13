using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerPlus : GMapMarker
    {
        private static readonly Bitmap icong = new Bitmap(global::MissionPlanner.Maps.Resources.plus.ToBitmap(), new Size(20, 20));

        public GMapMarkerPlus(PointLatLng p)
            : base(p)
        {
            // used for hitarea
            Size = new Size(30, 30);
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            if (IsMouseOver)
            {
                g.RotateTransform(45);
            }

            g.DrawImageUnscaled(icong, icong.Width / -2, icong.Height / -2);

            g.Transform = temp;
        }
    }
}