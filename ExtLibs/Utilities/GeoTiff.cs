using BitMiracle.LibTiff.Classic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using GMap.NET;
using log4net;

namespace MissionPlanner.Utilities
{
    public class GeoTiff
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, float[,]> cache = new Dictionary<string, float[,]>();

        public static List<geotiffdata> index = new List<geotiffdata>();

        public class geotiffdata
        {
            public bool LoadFile(string filename)
            {
                FileName = filename;

                log.InfoFormat("GeoTiff {0}", filename);

                using (Tiff tiff = Tiff.Open(filename, "r"))
                {
                    width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                    height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                    bits = tiff.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();

                    var modelscale = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
                    var tiepoint = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

                    i = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0);
                    j = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0 + 8);
                    k = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0 + 16);
                    x = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0 + 24);
                    y = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0 + 32);
                    z = BitConverter.ToDouble(tiepoint[1].ToByteArray(), 0 + 40);

                    log.InfoFormat("Tie Point ({0},{1},{2}) --> ({3},{4},{5})", i, j, k, x, y, z);

                    xscale = BitConverter.ToDouble(modelscale[1].ToByteArray(), 0);
                    yscale = BitConverter.ToDouble(modelscale[1].ToByteArray(), 0 + 8);
                    zscale = BitConverter.ToDouble(modelscale[1].ToByteArray(), 0 + 16);

                    log.InfoFormat("Scale ({0},{1},{2})", xscale, yscale, zscale);

                    Area = new RectLatLng(y, x, width*xscale, height*yscale);

                    log.InfoFormat("Coverage {0}", Area.ToString());

                    log.InfoFormat("CacheAble {0}", cacheable.ToString());

                    // starts from top left so x + y -
                    x += xscale / 2.0;
                    y -= yscale / 2.0;

                    log.InfoFormat("Start Point ({0},{1},{2}) --> ({3},{4},{5})", i, j, k, x, y, z);

                    GeoTiff.index.Add(this);

                    /*

                short numberOfDirectories = tiff.NumberOfDirectories();
                for (short d = 0; d < numberOfDirectories; ++d)
                {
                    tiff.SetDirectory((short)d);

                    for (ushort t = ushort.MinValue; t < ushort.MaxValue; ++t)
                    {
                        TiffTag tag = (TiffTag)t;
                        FieldValue[] value = tiff.GetField(tag);
                        if (value != null)
                        {
                            for (int j2 = 0; j2 < value.Length; j2++)
                            {
                                Console.WriteLine("{0} : {1} : {2}", tag.ToString(), value[j2].Value.GetType().ToString(), value[j2].ToString());
                            }
                        }
                    }
                }
                     */
                }

                return true;
            }

            public bool cacheable {get { return new FileInfo(FileName).Length < 1024*1024*1000; }}

            public string FileName;
            public int width;
            public int height;
            public int bits;
            public RectLatLng Area;
            public double i;
            public double j;
            public double k;
            public double x;
            public double y;
            public double z;
            public double xscale;
            public double yscale;
            public double zscale;
        }

        static GeoTiff()
        {
            generateIndex();
        }

        private static void generateIndex()
        {
            if (!Directory.Exists(srtm.datadirectory))
                return;

            var files = Directory.GetFiles(srtm.datadirectory, "*.tif");

            foreach (var file in files)
            {
                try
                {
                    geotiffdata tif = new geotiffdata();

                    tif.LoadFile(file);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public static srtm.altresponce getAltitude(double lat, double lng, double zoom = 16)
        {
            if (index.Count == 0)
                return srtm.altresponce.Invalid;

            var answer = new srtm.altresponce();

            foreach (var geotiffdata in index)
            {
                if (geotiffdata.Area.Contains(lat, lng))
                {
                    // add to cache
                    if (!cache.ContainsKey(geotiffdata.FileName) && geotiffdata.cacheable)
                    {
                        float[,] altdata = new float[geotiffdata.height, geotiffdata.width];

                        using (Tiff tiff = Tiff.Open(geotiffdata.FileName, "r"))
                        {
                            byte[] scanline = new byte[tiff.ScanlineSize()];

                            for (int row = 0; row < geotiffdata.height; row++)
                            {
                                tiff.ReadScanline(scanline, row);

                                for (int col = 0; col < geotiffdata.width; col++)
                                {
                                    if (geotiffdata.bits == 16)
                                    {
                                        altdata[row, col] = (short) ((scanline[col*2 + 1] << 8) + scanline[col*2]);
                                    }
                                    else if (geotiffdata.bits == 32)
                                    {
                                        altdata[row, col] = (float) BitConverter.ToSingle(scanline, col*4);
                                    }
                                }
                            }
                        }

                        cache[geotiffdata.FileName] = altdata;
                    }

                    // get answer
                    var xf = map(lat, geotiffdata.Area.Top, geotiffdata.Area.Bottom, 0, geotiffdata.height-1);
                    var yf = map(lng, geotiffdata.Area.Left, geotiffdata.Area.Right, 0, geotiffdata.width-1);

                    int x_int = (int) xf;
                    double x_frac = xf - x_int;

                    int y_int = (int) yf;
                    double y_frac = yf - y_int;

                    // y_int = (geotiffdata.height - 2) - y_int;

                    double alt00 = GetAlt(geotiffdata, x_int, y_int);
                    double alt10 = GetAlt(geotiffdata, x_int + 1, y_int);
                    double alt01 = GetAlt(geotiffdata, x_int, y_int + 1);
                    double alt11 = GetAlt(geotiffdata, x_int + 1, y_int + 1);

                    double v1 = avg(alt00, alt10, x_frac);
                    double v2 = avg(alt01, alt11, x_frac);
                    double v = avg(v1, v2, y_frac);

                    if (v > -1000)
                        answer.currenttype = srtm.tiletype.valid;
                    answer.alt = v;
                    answer.altsource = "GeoTiff";
                    return answer;
                }
            }

            return srtm.altresponce.Invalid;
        }

        private static double GetAltNoCache(geotiffdata geotiffdata, int x, int y)
        {
            using (Tiff tiff = Tiff.Open(geotiffdata.FileName, "r"))
            {
                byte[] scanline = new byte[tiff.ScanlineSize()];

                for (int row = 0; row < geotiffdata.height; row++)
                {
                    tiff.ReadScanline(scanline, x);

                    if (geotiffdata.bits == 16)
                    {
                        return (short)((scanline[y * 2 + 1] << 8) + scanline[y * 2]);
                    }
                    else if (geotiffdata.bits == 32)
                    {
                        return BitConverter.ToSingle(scanline, y * 4);
                    }
                }
            }

            throw new Exception("GetAltNoCache: Invalid geotiff coord");
        }

        private static double GetAlt(geotiffdata geotiffdata, int x, int y)
        {
            // if the image is to large use the direct to file approach
            if (!geotiffdata.cacheable)
                return GetAltNoCache(geotiffdata, x, y);

            // use our cache
            return cache[geotiffdata.FileName][x, y];
        }

        private static double avg(double v1, double v2, double weight)
        {
            return v2*weight + v1*(1 - weight);
        }

        private static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min)*(out_max - out_min)/(in_max - in_min) + out_min;
        }
    }
}
