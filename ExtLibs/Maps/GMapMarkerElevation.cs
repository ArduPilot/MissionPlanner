using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.Maps
{
    public class GMapMarkerElevation : GMapMarker
    {
        Bitmap elevation;

        private RectLatLng rect;

        public GMapMarkerElevation(byte [,] imageData, int idx, int idy, RectLatLng rect, PointLatLng currentloc)
        : base(currentloc)
        {
            this.rect = rect;

            IsHitTestVisible = false;

            //create a new Bitmap
            Bitmap bmp = new Bitmap(idx,idy, PixelFormat.Format32bppArgb);
            
            //lock it to get the BitmapData Object
            BitmapData bmData =
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //now we have to convert the 2 dimensional array into a one dimensional byte-array for use with 8bpp bitmaps
            // use stride and height to prevent stride mod 4 issues
            int[] pixels = new int[(bmData.Stride/4) * bmData.Height];
            for (int y = 0; y < idy; y++)
            {
                for (int x = 0; x < idx; x++)
                {
                    pixels[(y * (bmData.Stride/4) + x)] = ConvertColor(imageData[x, y]);
                }
            }

            //copy the bytes
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmData.Scan0, (bmData.Stride/4) * bmData.Height);            

            //never forget to unlock the bitmap
            bmp.UnlockBits(bmData);

            bmp.MakeTransparent();

            //display
            elevation = bmp;
        }

        static GMapMarkerElevation()
        {
            var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            pal = bmp.Palette;
            //create grayscale palette
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] =
                    Rainbow(1.0f -
                            (i / 256.0f));
            }

            transparent = Color.Transparent.ToArgb();
        }

        public static Color Rainbow(float progress)
        {
            float div = (Math.Abs(progress % 1) * 5); // 5 not 6 - dont want back to red
            int ascending = (int)((div % 1) * 255);
            int descending = 255 - ascending;

            switch ((int)div)
            {
                case 0:
                    return Color.FromArgb(100, 255, ascending, 0);//red - yellow
                case 1:
                    return Color.FromArgb(100, descending, 255, 0); //yellow - green
                case 2:
                    return Color.FromArgb(100, 0, 255, ascending); //green - cyan
                case 3:
                    return Color.FromArgb(100, 0, descending, 255); //  cyan -  blue
                case 4:
                    return Color.FromArgb(100, ascending, 0, 255); // blue - pink
                default: // case 5:
                    return Color.FromArgb(100, 255, 0, descending); // pink - red
            }
        }

        private static ColorPalette pal;

        private static int transparent;

        int ConvertColor(byte incol)
        {
            if (incol == 0 || incol == 255)
                return transparent;

            return pal.Entries[incol].ToArgb();
        }

        public override void OnRender(IGraphics g)
        {
            base.OnRender(g);

            var tlll = Overlay.Control.FromLatLngToLocal(rect.LocationTopLeft);
            var brll = Overlay.Control.FromLatLngToLocal(rect.LocationRightBottom);            

            var old = g.Transform;

            g.ResetTransform();

            // maintain transperancy
            g.CompositingMode = CompositingMode.SourceOver;

            g.DrawImage(elevation, tlll.X, tlll.Y, brll.X - tlll.X, brll.Y - tlll.Y);

            g.Transform = old;
        }
    }
}
