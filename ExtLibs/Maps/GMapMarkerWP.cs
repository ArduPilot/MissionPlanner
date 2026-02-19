using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MissionPlanner.Maps
{
    [Serializable]
    public class GMapMarkerWP : GMarkerGoogle
    {
        string wpno = "";
        public bool selected = false;
        static Dictionary<string, Bitmap> fontBitmaps = new Dictionary<string, Bitmap>();
        static Dictionary<string, SizeF> fontSizes = new Dictionary<string, SizeF>();
        static Font font;

        public GMapMarkerWP(PointLatLng p, string wpno, GMarkerGoogleType type = GMarkerGoogleType.green)
            : base(p, type)
        {
            this.wpno = wpno;
            if (font == null)
                font = SystemFonts.DefaultFont;

            if (!fontBitmaps.ContainsKey(wpno))
            {
                Bitmap temp = new Bitmap(100,40, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(temp))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    fontSizes[wpno] = g.MeasureString(wpno, font);

                    g.DrawString(wpno, font, Brushes.Black, new PointF(0, 0));
                }
                fontBitmaps[wpno] = temp;
            }
        }

        public override void OnRender(IGraphics g)
        {
            if (selected)
            {
                g.FillEllipse(Brushes.Red, new Rectangle(this.LocalPosition, this.Size));
                g.DrawArc(Pens.Red, new Rectangle(this.LocalPosition, this.Size), 0, 360);
            }
            
            base.OnRender(g);

            var midw = LocalPosition.X + 15;
            var midh = LocalPosition.Y + 3;

            var offset = (int)(fontSizes[wpno].Width / 2);
            // For really long WP numbers, better to at least have the left 2.5
            // digits on the marker rather than having both sides hang off.
            offset = Math.Min(offset, 10);
            midw -= offset;

            if (Overlay.Control.Zoom> 16 || IsMouseOver)
                g.DrawImageUnscaled(fontBitmaps[wpno], midw,midh);
        }
    }
}