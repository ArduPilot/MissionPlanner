using GMap.NET;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Org.Gdal.Gdal;
using Org.Gdal.Gdalconst;
using Org.Gdal.Ogr;
using Org.Gdal.Osr;
using System.Linq;
using GMap.NET.MapProviders;
using MissionPlanner.Utilities;

namespace GDAL
{
    public class GDAL: IGDAL
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static List<GeoBitmap> _cache = new List<GeoBitmap>();

        static GDAL()
        {
            log.InfoFormat("GDAL static ctor");
            //GdalConfiguration.ConfigureGdal();
            Gdal.AllRegister();
        }

        public delegate void Progress(double percent, string message);

        public static event Progress OnProgress;

        public static void ScanDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            int i = 0;
            foreach (var file in files)
            {
                i++;
                try
                {
                    // 1kb file check
                    if (new FileInfo(file).Length < 1024 * 1)
                        continue;

                    if (OnProgress != null)
                        OnProgress((i - 1) / (double)files.Length, file);

                    var info = GDAL.LoadImageInfo(file);

                    _cache.Add(info);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            // lowest res first
            _cache.Sort((a, b) => { return b.Resolution.CompareTo(a.Resolution); });
        }

        public GMapProvider GetProvider()
        {
            return GDALProvider.Instance;
        }

        public static GeoBitmap LoadImageInfo(string file)
        {
            using (var ds = Gdal.Open(file, GdalconstJNI.GA_ReadOnly_get()))
            {
                log.InfoFormat("Raster dataset parameters:");
                log.InfoFormat("  Projection: " + ds.ProjectionRef);
                log.InfoFormat("  RasterCount: " + ds.RasterCount);
                log.InfoFormat("  RasterSize (" + ds.RasterXSize + "," + ds.RasterYSize + ")");

                Org.Gdal.Gdal.Driver drv = ds.Driver;

                log.InfoFormat("Using driver " + drv.LongName);


                log.InfoFormat("Corner Coordinates:");
                log.InfoFormat("  Upper Left (" + GDALInfoGetPosition(ds, 0.0, 0.0) + ")");
                log.InfoFormat("  Lower Left (" + GDALInfoGetPosition(ds, 0.0, ds.RasterYSize) + ")");
                log.InfoFormat("  Upper Right (" + GDALInfoGetPosition(ds, ds.RasterXSize, 0.0) + ")");
                log.InfoFormat("  Lower Right (" + GDALInfoGetPosition(ds, ds.RasterXSize, ds.RasterYSize) + ")");
                log.InfoFormat("  Center (" + GDALInfoGetPosition(ds, ds.RasterXSize / 2, ds.RasterYSize / 2) + ")");
                log.InfoFormat("");

                string projection = ds.ProjectionRef;
                if (projection != null)
                {
                    SpatialReference srs = new SpatialReference(null);
                    if (srs.ImportFromWkt(projection) == 0)
                    {
                        string[] wkt = new string[1];
                        srs.ExportToPrettyWkt(wkt, 0);
                        log.InfoFormat("Coordinate System is:");
                        log.InfoFormat(wkt[0]);
                    }
                    else
                    {
                        log.InfoFormat("Coordinate System is:");
                        log.InfoFormat(projection);
                    }
                }

                var TL = GDALInfoGetPositionDouble(ds, 0.0, 0.0);
                var BR = GDALInfoGetPositionDouble(ds, ds.RasterXSize, ds.RasterYSize);
                var resolution = Math.Abs(BR[0] - TL[0]) / ds.RasterXSize;

                if (resolution == 1)
                    throw new Exception("Invalid coords");

                return new GeoBitmap(file, resolution, ds.RasterXSize, ds.RasterYSize, TL[0], TL[1], BR[0], BR[1]);
            }
        }

        static object locker = new object();

        public static Bitmap GetBitmap(double lng1, double lat1, double lng2, double lat2, long width, long height)
        {
            if (_cache.Count == 0)
                return null;

            Bitmap output = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);

            int a = 0;

            using (Graphics g = Graphics.FromImage(output))
            {
                g.Clear(Color.Transparent);

                RectLatLng request = new RectLatLng(lat1, lng1, lng2 - lng1, lat2 - lat1);

                //g.DrawString(request.ToString(), new Font("Arial", 12), Brushes.Wheat, 0, 0);

                bool cleared = false;

                foreach (var image in _cache.ToArray())
                {
                    // calc the pixel coord within the image rect
                    var ImageTop = (float)map(request.Top, image.Rect.Top, image.Rect.Bottom, 0, image.RasterYSize);
                    var ImageLeft = (float)map(request.Left, image.Rect.Left, image.Rect.Right, 0, image.RasterXSize);

                    var ImageBottom = (float)map(request.Bottom, image.Rect.Top, image.Rect.Bottom, 0, image.RasterYSize);
                    var ImageRight = (float)map(request.Right, image.Rect.Left, image.Rect.Right, 0, image.RasterXSize);

                    RectangleF rect = new RectangleF(ImageLeft, ImageTop, ImageRight - ImageLeft, ImageBottom - ImageTop);

                    var res = (request.Right - request.Left) / width;


                    if (rect.Left <= image.RasterXSize && rect.Top <= image.RasterYSize && rect.Right >= 0 && rect.Bottom >= 0)
                    {
                        if (!cleared)
                        {
                            //g.FillRectangle(Brushes.Green, rect.X, rect.Y, rect.Width, rect.Height);
                            cleared = true;
                        }

                        //if (image.Resolution < (res / 3))
                            //continue;

                        //Console.WriteLine("{0} <= {1} && {2} <= {3} || {4} >= {5} && {6} >= {7} ", rect.Left, image.RasterXSize, rect.Top, image.RasterYSize, rect.Right, 0, rect.Bottom, 0);

                        try
                        {
                            lock (locker)
                            {
                                if (image.Bitmap == null)
                                    continue;

                                // this is wrong
                                g.DrawImage(image.Bitmap, new RectangleF(0, 0, width, height), rect, GraphicsUnit.Pixel);

                            }
                            a++;

                            if (a >= 50)
                                return output;
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                            //throw new Exception("Bad Image "+image.File);
                        }
                    }
                }

                if (a == 0)
                {
                    return null;
                }

                return output;
            }
        }

        private static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static Bitmap LoadImage(string file)
        {
            lock (locker)
            {
                using (var ds = Gdal.Open(file, GdalconstJNI.GA_ReadOnly_get()))
                {
                    // 8bit geotiff - single band
                    if (ds.RasterCount == 1)
                    {
                        Band band = ds.GetRasterBand(1);
                        if (band == null)
                            return null;

                        ColorTable ct = band.GetRasterColorTable();

                        PixelFormat format = PixelFormat.Format8bppIndexed;

                        // Create a Bitmap to store the GDAL image in
                        Bitmap bitmap = new Bitmap(ds.RasterXSize, ds.RasterYSize, format);

                        // Obtaining the bitmap buffer
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, ds.RasterXSize, ds.RasterYSize),
                            ImageLockMode.ReadWrite, format);
                        try
                        {
                            if (ct != null)
                            {
                                int iCol = ct.GetCount();
                                ColorPalette pal = bitmap.Palette;
                                for (int i = 0; i < iCol; i++)
                                {
                                    ColorEntry ce = ct.GetColorEntry(i);
                                    pal.Entries[i] = Color.FromArgb(ce.c4, ce.c1, ce.c2, ce.c3);
                                }

                                bitmap.Palette = pal;
                            }
                            else
                            {

                            }

                            int stride = bitmapData.Stride;
                            IntPtr buf = bitmapData.Scan0;

                            var buffer = new byte[ds.RasterXSize * ds.RasterYSize];

                            band.ReadRaster(0, 0, ds.RasterXSize, ds.RasterYSize, ds.RasterXSize, ds.RasterYSize,
                                    (int) DataType.GDT_Byte, buffer);

                            Marshal.Copy(buffer, 0, buf, buffer.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }


                        return bitmap;
                    }

                    {
                        Bitmap bitmap = new Bitmap(ds.RasterXSize, ds.RasterYSize, PixelFormat.Format32bppArgb);
                        if (ds.RasterCount == 3)
                        {
                            // when we load a 24bit bitmap, we need to set the alpha channel else we get nothing
                            using (var tmp=Graphics.FromImage(bitmap))
                            {
                                tmp.Clear(Color.White);
                            }
                        }

                        for (int a = 1; a <= ds.RasterCount; a++)
                        {
                            // Get the GDAL Band objects from the Dataset
                            Band band = ds.GetRasterBand(a);
                            if (band == null)
                                return null;

                            var cint = (ColorInterp)band.GetColorInterpretation();

                
                            // Obtaining the bitmap buffer
                            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, ds.RasterXSize, ds.RasterYSize),
                                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                            try
                            {

                                int stride = bitmapData.Stride;
                                IntPtr buf = bitmapData.Scan0;
                                var buffer = new byte[ds.RasterXSize * ds.RasterYSize];

                                band.ReadRaster(0, 0, ds.RasterXSize, ds.RasterYSize, ds.RasterXSize,
                                    ds.RasterYSize, (int) DataType.GDT_Byte, buffer);

                                int c = 0;
                                if (cint == ColorInterp.GCI_AlphaBand)
                                    foreach (var b in buffer)
                                    {
                                        Marshal.WriteByte(buf, 3 + c * 4, (byte)b);
                                        c++;
                                    }
                                else if (cint == ColorInterp.GCI_RedBand)
                                    foreach (var b in buffer)
                                    {
                                        Marshal.WriteByte(buf, 2 + c * 4, (byte)b);
                                        c++;
                                    }
                                else if (cint == ColorInterp.GCI_GreenBand)
                                    foreach (var b in buffer)
                                    {
                                        Marshal.WriteByte(buf, 1 + c * 4, (byte)b);
                                        c++;
                                    }
                                else if (cint == ColorInterp.GCI_BlueBand)
                                    foreach (var b in buffer)
                                    {
                                        Marshal.WriteByte(buf, 0 + c * 4, (byte)b);
                                        c++;
                                    }
                                else
                                {

                                }
                            }
                            finally
                            {
                                bitmap.UnlockBits(bitmapData);
                            }
                        }

                        //bitmap.Save("gdal.png", ImageFormat.Png);
                        return bitmap;
                    }
                }
            }

            return null;
        }

        private static string GDALInfoGetPosition(Dataset ds, double x, double y)
        {
            double[] adfGeoTransform = new double[6];
            double dfGeoX, dfGeoY;
            ds.GetGeoTransform(adfGeoTransform);

            dfGeoX = adfGeoTransform[0] + adfGeoTransform[1] * x + adfGeoTransform[2] * y;
            dfGeoY = adfGeoTransform[3] + adfGeoTransform[4] * x + adfGeoTransform[5] * y;

            return dfGeoX.ToString() + ", " + dfGeoY.ToString();
        }

        private static double[] GDALInfoGetPositionDouble(Dataset ds, double x, double y)
        {
            double[] adfGeoTransform = new double[6];
            double dfGeoX, dfGeoY;
            ds.GetGeoTransform(adfGeoTransform);

            dfGeoX = adfGeoTransform[0] + adfGeoTransform[1] * x + adfGeoTransform[2] * y;
            dfGeoY = adfGeoTransform[3] + adfGeoTransform[4] * x + adfGeoTransform[5] * y;

            return new double[] { dfGeoX, dfGeoY };
        }

        public class GeoBitmap
        {
            Bitmap _bitmap = null;
            // load on demand
            public Bitmap Bitmap
            {
                get
                {
                    lock (this)
                    {
                        if (_bitmap == null) _bitmap = LoadImage(File);
                        return _bitmap;
                    }
                }
            }

            public GMap.NET.RectLatLng Rect;
            public string File;
            public int RasterXSize;
            public int RasterYSize;
            public double Resolution;

            public GeoBitmap(string file, double resolution, int rasterXSize, int rasterYSize, double Left, double Top, double Right, double Bottom)
            {
                this.File = file;
                this.Resolution = resolution;
                this.RasterXSize = rasterXSize;
                this.RasterYSize = rasterYSize;
                Rect = new GMap.NET.RectLatLng(Top, Left, Right - Left, Top - Bottom);
            }
        }

        void IGDAL.ScanDirectory(string s)
        {
            ScanDirectory(s);
        }
    }
}