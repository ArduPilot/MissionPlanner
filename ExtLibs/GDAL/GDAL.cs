using System;
using OSGeo.GDAL;
using System.Drawing.Imaging;
using System.Drawing;
using log4net;
using System.Reflection;
using OSGeo.OSR;
using log4net.Layout;
using log4net.Appender;
using log4net.Config;
using System.IO;
using System.Collections.Generic;
using GMap.NET;
using GMap.NET.MapProviders;
using System.Drawing.Drawing2D;
using OSGeo.OGR;

namespace GDAL
{
    public static class GDAL
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static List<GeoBitmap> _cache = new List<GeoBitmap>();

        static GDAL()
        {
            log.InfoFormat("GDAL static ctor");
            GdalConfiguration.ConfigureGdal();
        }

        public delegate void Progress(double percent, string message);

        public static event Progress OnProgress;

        public static void ScanDirectory(string path)
        {
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

        public static GeoBitmap LoadImageInfo(string file)
        {
            using (var ds = OSGeo.GDAL.Gdal.Open(file, OSGeo.GDAL.Access.GA_ReadOnly))
            {
                log.InfoFormat("Raster dataset parameters:");
                log.InfoFormat("  Projection: " + ds.GetProjectionRef());
                log.InfoFormat("  RasterCount: " + ds.RasterCount);
                log.InfoFormat("  RasterSize (" + ds.RasterXSize + "," + ds.RasterYSize + ")");

                OSGeo.GDAL.Driver drv = ds.GetDriver();

                log.InfoFormat("Using driver " + drv.LongName);

                string[] metadata = ds.GetMetadata("");
                if (metadata.Length > 0)
                {
                    log.InfoFormat("  Metadata:");
                    for (int iMeta = 0; iMeta < metadata.Length; iMeta++)
                    {
                        // log.InfoFormat("    " + iMeta + ":  " + metadata[iMeta]);
                    }
                    log.InfoFormat("");
                }

                metadata = ds.GetMetadata("IMAGE_STRUCTURE");
                if (metadata.Length > 0)
                {
                    log.InfoFormat("  Image Structure Metadata:");
                    for (int iMeta = 0; iMeta < metadata.Length; iMeta++)
                    {
                        log.InfoFormat("    " + iMeta + ":  " + metadata[iMeta]);
                    }
                    log.InfoFormat("");
                }

                metadata = ds.GetMetadata("SUBDATASETS");
                if (metadata.Length > 0)
                {
                    log.InfoFormat("  Subdatasets:");
                    for (int iMeta = 0; iMeta < metadata.Length; iMeta++)
                    {
                        log.InfoFormat("    " + iMeta + ":  " + metadata[iMeta]);
                    }
                    log.InfoFormat("");
                }

                metadata = ds.GetMetadata("GEOLOCATION");
                if (metadata.Length > 0)
                {
                    log.InfoFormat("  Geolocation:");
                    for (int iMeta = 0; iMeta < metadata.Length; iMeta++)
                    {
                        log.InfoFormat("    " + iMeta + ":  " + metadata[iMeta]);
                    }
                    log.InfoFormat("");
                }

                log.InfoFormat("Corner Coordinates:");
                log.InfoFormat("  Upper Left (" + GDALInfoGetPosition(ds, 0.0, 0.0) + ")");
                log.InfoFormat("  Lower Left (" + GDALInfoGetPosition(ds, 0.0, ds.RasterYSize) + ")");
                log.InfoFormat("  Upper Right (" + GDALInfoGetPosition(ds, ds.RasterXSize, 0.0) + ")");
                log.InfoFormat("  Lower Right (" + GDALInfoGetPosition(ds, ds.RasterXSize, ds.RasterYSize) + ")");
                log.InfoFormat("  Center (" + GDALInfoGetPosition(ds, ds.RasterXSize / 2, ds.RasterYSize / 2) + ")");
                log.InfoFormat("");

                string projection = ds.GetProjectionRef();
                if (projection != null)
                {
                    SpatialReference srs = new SpatialReference(null);
                    if (srs.ImportFromWkt(ref projection) == 0)
                    {
                        string wkt;
                        srs.ExportToPrettyWkt(out wkt, 0);
                        log.InfoFormat("Coordinate System is:");
                        log.InfoFormat(wkt);
                    }
                    else
                    {
                        log.InfoFormat("Coordinate System is:");
                        log.InfoFormat(projection);
                    }
                }

                if (ds.GetGCPCount() > 0)
                {
                    log.InfoFormat("GCP Projection: ", ds.GetGCPProjection());
                    GCP[] GCPs = ds.GetGCPs();
                    for (int i = 0; i < ds.GetGCPCount(); i++)
                    {
                        log.InfoFormat("GCP[" + i + "]: Id=" + GCPs[i].Id + ", Info=" + GCPs[i].Info);
                        log.InfoFormat("          (" + GCPs[i].GCPPixel + "," + GCPs[i].GCPLine + ") -> ("
                                     + GCPs[i].GCPX + "," + GCPs[i].GCPY + "," + GCPs[i].GCPZ + ")");
                        log.InfoFormat("");
                    }
                    log.InfoFormat("");

                    double[] transform = new double[6];
                    Gdal.GCPsToGeoTransform(GCPs, transform, 0);
                    log.InfoFormat("GCP Equivalent geotransformation parameters: ", ds.GetGCPProjection());
                    for (int i = 0; i < 6; i++)
                        log.InfoFormat("t[" + i + "] = " + transform[i].ToString());
                    log.InfoFormat("");
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

                //g.DrawString(request.ToString(), Control.DefaultFont, Brushes.Wheat, 0, 0);

                bool cleared = false;

                foreach (var image in _cache)
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
                            //g.Clear(Color.Red);
                            cleared = true;
                        }

                        if (image.Resolution < (res / 3))
                            continue;

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
                using (var ds = OSGeo.GDAL.Gdal.Open(file, OSGeo.GDAL.Access.GA_ReadOnly))
                {
                    // Get the GDAL Band objects from the Dataset
                    Band band = ds.GetRasterBand(1);
                    if (band == null)
                        return null;
                    ColorTable ct = band.GetRasterColorTable();
                    // Create a Bitmap to store the GDAL image in
                    Bitmap bitmap = new Bitmap(ds.RasterXSize, ds.RasterYSize, PixelFormat.Format8bppIndexed);
                    // Obtaining the bitmap buffer
                    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, ds.RasterXSize, ds.RasterYSize),
                        ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
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

                        int stride = bitmapData.Stride;
                        IntPtr buf = bitmapData.Scan0;

                        band.ReadRaster(0, 0, ds.RasterXSize, ds.RasterYSize, buf, ds.RasterXSize, ds.RasterYSize,
                            DataType.GDT_Byte, 1, stride);
                    }
                    finally
                    {
                        bitmap.UnlockBits(bitmapData);
                    }

                    return bitmap;
                }
            }
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

        //http://www.gisremotesensing.com/2015/09/vector-to-raster-conversion-using-gdal-c.html
        public static void Rasterize(string inputFeature, string outRaster, string fieldName, int cellSize)
        {
            // Define pixel_size and NoData value of new raster  
            int rasterCellSize = cellSize;
            const double noDataValue = -9999;
            string outputRasterFile = outRaster;
            //Register the vector drivers  
            Ogr.RegisterAll();
            //Reading the vector data  
            DataSource dataSource = Ogr.Open(inputFeature, 0);
            var count = dataSource.GetLayerCount();
            Layer layer = dataSource.GetLayerByIndex(0);
            var litems = layer.GetFeatureCount(0);
            var lname= layer.GetName();
            Envelope envelope = new Envelope();
            layer.GetExtent(envelope, 0);
            //Compute the out raster cell resolutions  
            int x_res = Convert.ToInt32((envelope.MaxX - envelope.MinX) / rasterCellSize);
            int y_res = Convert.ToInt32((envelope.MaxY - envelope.MinY) / rasterCellSize);
            Console.WriteLine("Extent: " + envelope.MaxX + " " + envelope.MinX + " " + envelope.MaxY + " " + envelope.MinY);
            Console.WriteLine("X resolution: " + x_res);
            Console.WriteLine("X resolution: " + y_res);
            //Register the raster drivers  
            Gdal.AllRegister();
            //Check if output raster exists & delete (optional)  
            if (File.Exists(outputRasterFile))
            {
                File.Delete(outputRasterFile);
            }
            //Create new tiff   
            OSGeo.GDAL.Driver outputDriver = Gdal.GetDriverByName("GTiff");
            Dataset outputDataset = outputDriver.Create(outputRasterFile, x_res, y_res, 1, DataType.GDT_Float64, null);
            //Extrac srs from input feature   
            string inputShapeSrs;
            SpatialReference spatialRefrence = layer.GetSpatialRef();
            spatialRefrence.ExportToWkt(out inputShapeSrs);
            //Assign input feature srs to outpur raster  
            outputDataset.SetProjection(inputShapeSrs);
            //Geotransform  
            double[] argin = new double[] { envelope.MinX, rasterCellSize, 0, envelope.MaxY, 0, -rasterCellSize };
            outputDataset.SetGeoTransform(argin);
            //Set no data  
            Band band = outputDataset.GetRasterBand(1);
            band.SetNoDataValue(noDataValue);
            //close tiff  
            outputDataset.FlushCache();
            outputDataset.Dispose();
            //Feature to raster rasterize layer options  
            //No of bands (1)  
            int[] bandlist = new int[] { 1 };
            //Values to be burn on raster (10.0)  
            double[] burnValues = new double[] { 10.0 };
            Dataset myDataset = Gdal.Open(outputRasterFile, Access.GA_Update);
            //additional options  
            string[] rasterizeOptions;
            //rasterizeOptions = new string[] { "ALL_TOUCHED=TRUE", "ATTRIBUTE=" + fieldName }; //To set all touched pixels into raster pixel  
            rasterizeOptions = new string[] { "ATTRIBUTE=" + fieldName };
            //Rasterize layer  
            //Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, null, null, null); // To burn the given burn values instead of feature attributes  
            Gdal.RasterizeLayer(myDataset, 1, bandlist, layer, IntPtr.Zero, IntPtr.Zero, 1, burnValues, rasterizeOptions, new Gdal.GDALProgressFuncDelegate(ProgressFunc), "Raster conversion");
        }

        private static int ProgressFunc(double complete, IntPtr message, IntPtr data)
        {
            Console.Write("Processing ... " + complete * 100 + "% Completed.");
            if (message != IntPtr.Zero)
            {
                Console.Write(" Message:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message));
            }
            if (data != IntPtr.Zero)
            {
                Console.Write(" Data:" + System.Runtime.InteropServices.Marshal.PtrToStringAnsi(data));
            }
            Console.WriteLine("");
            return 1;
        }
    }
}