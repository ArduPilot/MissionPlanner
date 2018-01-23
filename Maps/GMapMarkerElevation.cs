using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;

namespace MissionPlanner.Maps
{
    public class GMapMarkerElevation : GMapMarker
    {
        static Bitmap elevation;

        int Width = 0;
        int Height = 0;

        public GMapMarkerElevation(byte [,] imageData, int width, int height, PointLatLng currentloc)
        : base(currentloc)
        {

            Width = width;
            Height = height;

            //elevation = PureImage.FromArray(imageData);

            //now we have to convert the 2 dimensional array into a one dimensional byte-array for use with 8bpp bitmaps
            byte[] pixels = new byte[imageData.GetLength(0) * imageData.GetLength(1)];
            for (int y = 0; y < imageData.GetLength(1); y++)
            {
                for (int x = 0; x < imageData.GetLength(0); x++)
                {
                    pixels[y * imageData.GetLength(0) + x] = imageData[x, y];
                }
            }

            //create a new Bitmap
            Bitmap bmp = new Bitmap(imageData.GetLength(0), imageData.GetLength(1), System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            System.Drawing.Imaging.ColorPalette pal = bmp.Palette;

            /*
            //create grayscale palette
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = Color.FromArgb((int)200, i, i, i);
            }

            //assign to bmp
            bmp.Palette = pal;
            */
            //lock it to get the BitmapData Object
            System.Drawing.Imaging.BitmapData bmData =
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //copy the bytes
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmData.Scan0, bmData.Stride * bmData.Height);

            //never forget to unlock the bitmap
            bmp.UnlockBits(bmData);

            //display
            elevation = bmp;
            
        }

        public override void OnRender(Graphics g)
        {
            base.OnRender(g);
            elevation.MakeTransparent();
            GPoint loc = new GPoint(LocalPosition.X, LocalPosition.Y);

            g.DrawImageUnscaled(elevation, (int)loc.X-Width/2, (int)loc.Y-Height/2);

        }
    }
}
