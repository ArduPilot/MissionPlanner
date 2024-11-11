using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MissionPlanner.GCSViews
{
    public class GMapMarkerKMLLabel : GMapMarker
    {
        private PointLatLng pointLatLng;
        private string text;
        private Font font;
        static Dictionary<string, Bitmap> fontBitmaps = new Dictionary<string, Bitmap>();
        private SizeF txtsize;

        public GMapMarkerKMLLabel(PointLatLng pointLatLng, string text): base(pointLatLng)
        {
            this.pointLatLng = pointLatLng;
            this.text = text;

            if (font == null)
                font = SystemFonts.DefaultFont;

            if (!fontBitmaps.ContainsKey(text))
            {
                Bitmap temp = new Bitmap(100, 40, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    txtsize = g.MeasureString(text, font);

                    g.DrawString(text, font, Brushes.White, new PointF(0, 0));
                }
                fontBitmaps[text] = temp;
            }
            IsHitTestVisible = false;
        }

        public override void OnRender(IGraphics g)
        {
            base.OnRender(g);

            var midw = LocalPosition.X + 10;
            var midh = LocalPosition.Y + 3;

            if (txtsize.Width > 15)
                midw -= 4;
            
            g.DrawImageUnscaled(fontBitmaps[text], midw, midh);
        }
    }
}