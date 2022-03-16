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
            Size = icong.Size;
            Offset = new Point(-Size.Width / 2, -Size.Height / 2);
        }

        public override void OnRender(IGraphics g)
        {
            var temp = g.Transform;
            g.TranslateTransform(LocalPosition.X, LocalPosition.Y);

            if (IsMouseOver)
            {
                g.TranslateTransform(icong.Width / 2, icong.Height / -4);
                g.RotateTransform(45);
            }

            g.DrawImageUnscaled(icong, 0, 0);// icong.Width / -2, icong.Height / -2);

            g.Transform = temp;
        }
    }
}