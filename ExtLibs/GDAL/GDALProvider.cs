﻿using GMap.NET.WindowsForms;
using System.Collections.Generic;
using System.IO;

namespace GDAL
{
    using GMap.NET;
    using GMap.NET.MapProviders;
    using GMap.NET.Projections;
    using System;
    using System.Drawing.Imaging;
    using System.Drawing;
    using System.Reflection;

    /// <summary>
    /// GDAL Custom
    /// </summary>
    public class GDALProvider : GMapProvider
    {
        public static readonly GDALProvider Instance;

        public double opacity = 1.0;

        public GDALProvider()
        {
            MaxZoom = 24;
            BypassCache = true;
        }

        static GDALProvider()
        {
            Instance = new GDALProvider();

            Type mytype = typeof(GMapProviders);
            FieldInfo field = mytype.GetField("DbHash", BindingFlags.Static | BindingFlags.NonPublic);
            Dictionary<int, GMapProvider> list = (Dictionary<int, GMapProvider>)field.GetValue(Instance);

            list.Add(Instance.DbId, Instance);

            //GMap.NET.MapProviders.GMapProviders.List.Add(Instance);
        }

        readonly Guid id = new Guid("4574218D-B552-4CAF-89AE-F20941BBDB2B");

        public override Guid Id
        {
            get { return id; }
        }

        readonly string name = "GDAL Custom";

        public override string Name
        {
            get { return name; }
        }

        GMapProvider[] overlays;

        public override GMapProvider[] Overlays
        {
            get
            {
                if (overlays == null)
                {
                    overlays = new GMapProvider[] { GoogleSatelliteMapProvider.Instance, this };
                }
                return overlays;
            }
        }

        public override PureProjection Projection
        {
            get { return MercatorProjection.Instance; }
        }

        public override PureImage GetTileImage(GPoint pos, int zoom)
        {
            var px1 = GDALProvider.Instance.Projection.FromTileXYToPixel(pos);
            var px2 = px1;

            px1.Offset(0, 0);
            PointLatLng p1 = GDALProvider.Instance.Projection.FromPixelToLatLng(px1, zoom);

            px2.Offset(GDALProvider.Instance.Projection.TileSize.Width, -GDALProvider.Instance.Projection.TileSize.Height);
            PointLatLng p2 = GDALProvider.Instance.Projection.FromPixelToLatLng(px2, zoom);

            var bmp = GDAL.GetBitmap(p1.Lng, p1.Lat, p2.Lng, p2.Lat, GDALProvider.Instance.Projection.TileSize.Width, GDALProvider.Instance.Projection.TileSize.Height);

            if (bmp == null)
            {
                return null;
            }

            if (opacity < 1 && opacity > 0)
                bmp = ChangeImageOpacity(bmp, opacity);

            var ms = new MemoryStream();

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return new GMapImage() { Img = bmp, Data = ms };
        }

        private const int bytesPerPixel = 4;

        /// <summary>
        /// Change the opacity of an image
        /// </summary>
        /// <param name="originalImage">The original image</param>
        /// <returns>The changed image</returns>
        public static Bitmap ChangeImageOpacity(Bitmap originalImage, double opacity)
        {
            if ((originalImage.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return originalImage;
            }

            Bitmap bmp = (Bitmap)originalImage.Clone();

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + bytesPerPixel - 1] == 0)
                    continue;

                int pos = 0;
                pos++; // B value
                pos++; // G value
                pos++; // R value

                argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }
}