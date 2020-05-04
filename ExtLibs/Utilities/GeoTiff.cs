using BitMiracle.LibTiff.Classic;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using log4net;

namespace MissionPlanner.Utilities
{
    public class GeoTiff
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string, float[,]> cache = new Dictionary<string, float[,]>();

        private static List<string> cacheloading = new List<string>();

        public static List<geotiffdata> index = new List<geotiffdata>();

        /// <summary>
        /// http://duff.ess.washington.edu/data/raster/drg/docs/geotiff.txt
        /// </summary>
        public class geotiffdata
        {
            enum modeltype
            {
                ModelTypeProjected = 1 ,  /* Projection Coordinate System         */
                ModelTypeGeographic = 2 , /* Geographic latitude-longitude System */
                ModelTypeGeocentric = 3   /* Geocentric (X,Y,Z) Coordinate System */
            }

            enum rastertype
            {
                RasterPixelIsArea = 1,
                RasterPixelIsPoint = 2
            }

            enum GKID
            {
                GTModelTypeGeoKey = 1024, /* Section 6.3.1.1 Codes       */
                GTRasterTypeGeoKey = 1025, /* Section 6.3.1.2 Codes       */
                GTCitationGeoKey = 1026, /* documentation */

                GeographicTypeGeoKey = 2048, /* Section 6.3.2.1 Codes     */
                GeogCitationGeoKey = 2049, /* documentation             */
                GeogGeodeticDatumGeoKey = 2050, /* Section 6.3.2.2 Codes     */
                GeogPrimeMeridianGeoKey = 2051, /* Section 6.3.2.4 codes     */
                GeogLinearUnitsGeoKey = 2052, /* Section 6.3.1.3 Codes     */
                GeogLinearUnitSizeGeoKey = 2053, /* meters                    */
                GeogAngularUnitsGeoKey = 2054, /* Section 6.3.1.4 Codes     */


                GeogAngularUnitSizeGeoKey = 2055, /* radians                   */
                GeogEllipsoidGeoKey = 2056, /* Section 6.3.2.3 Codes     */
                GeogSemiMajorAxisGeoKey = 2057, /* GeogLinearUnits           */
                GeogSemiMinorAxisGeoKey = 2058, /* GeogLinearUnits           */
                GeogInvFlatteningGeoKey = 2059, /* ratio                     */
                GeogAzimuthUnitsGeoKey = 2060, /* Section 6.3.1.4 Codes     */
                GeogPrimeMeridianLongGeoKey = 2061, /* GeogAngularUnit           */

                ProjectedCSTypeGeoKey = 3072, /* Section 6.3.3.1 codes   */
                PCSCitationGeoKey = 3073, /* documentation           */
                ProjectionGeoKey = 3074, /* Section 6.3.3.2 codes   */
                ProjCoordTransGeoKey = 3075, /* Section 6.3.3.3 codes   */
                ProjLinearUnitsGeoKey = 3076, /* Section 6.3.1.3 codes   */
                ProjLinearUnitSizeGeoKey = 3077, /* meters                  */
                ProjStdParallelGeoKey = 3078, /* GeogAngularUnit */
                ProjStdParallel2GeoKey = 3079, /* GeogAngularUnit */
                ProjOriginLongGeoKey = 3080, /* GeogAngularUnit */
                ProjOriginLatGeoKey = 3081, /* GeogAngularUnit */
                ProjFalseEastingGeoKey = 3082, /* ProjLinearUnits */
                ProjFalseNorthingGeoKey = 3083, /* ProjLinearUnits */
                ProjFalseOriginLongGeoKey = 3084, /* GeogAngularUnit */
                ProjFalseOriginLatGeoKey = 3085, /* GeogAngularUnit */
                ProjFalseOriginEastingGeoKey = 3086, /* ProjLinearUnits */
                ProjFalseOriginNorthingGeoKey = 3087, /* ProjLinearUnits */
                ProjCenterLongGeoKey = 3088, /* GeogAngularUnit */
                ProjCenterLatGeoKey = 3089, /* GeogAngularUnit */
                ProjCenterEastingGeoKey = 3090, /* ProjLinearUnits */
                ProjCenterNorthingGeoKey = 3091, /* ProjLinearUnits */
                ProjScaleAtOriginGeoKey = 3092, /* ratio   */
                ProjScaleAtCenterGeoKey = 3093, /* ratio   */
                ProjAzimuthAngleGeoKey = 3094, /* GeogAzimuthUnit */
                ProjStraightVertPoleLongGeoKey = 3095, /* GeogAngularUnit */

                VerticalCSTypeGeoKey = 4096, /* Section 6.3.4.1 codes   */
                VerticalCitationGeoKey = 4097, /* documentation */
                VerticalDatumGeoKey = 4098, /* Section 6.3.4.2 codes   */
                VerticalUnitsGeoKey = 4099, /* Section 6.3.1.3 codes   */
            }

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

                    var GeoKeyDirectoryTag = tiff.GetField((TiffTag)34735);

                    var KeyDirectoryVersion = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 0);
                    var KeyRevision= BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 2);
                    var MinorRevision= BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 4);
                    var NumberOfKeys = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 6);

                    var ProjectedCSTypeGeoKey = 0;

                    for (int i = 8; i < 8 + NumberOfKeys * 8;i+=8)
                    {
                        var KeyID = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i);
                        var TIFFTagLocation = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 2);
                        var Count = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 4);
                        var Value_Offset = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 6);

                        log.InfoFormat("GeoKeyDirectoryTag ID={0} TagLoc={1} Count={2} Value/offset={3}", (GKID)KeyID, TIFFTagLocation,
                            Count, Value_Offset);

                        if (KeyID == (int)GKID.ProjectedCSTypeGeoKey)
                            ProjectedCSTypeGeoKey = Value_Offset;

                        if (TIFFTagLocation != 0)
                        {
                            if (TIFFTagLocation == 34737)
                            {
                                var value = tiff.GetField((TiffTag) TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset)
                                    .Take(Count);
                                log.InfoFormat("GeoKeyDirectoryTag ID={0} Value={1}", (GKID) KeyID,
                                    Encoding.ASCII.GetString(value.ToArray()));
                            }
                            if (TIFFTagLocation == 34736)
                            {
                                /*
                                var value = tiff.GetField((TiffTag)TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset*8)
                                    .Take(Count*8);
                                log.InfoFormat("GeoKeyDirectoryTag ID={0} Value={1}", (GKID) KeyID, value);
                                */
                            }
                        }
                    }

                    var GeoAsciiParamsTag = tiff.GetField((TiffTag)34737);
                    if (GeoAsciiParamsTag != null && GeoAsciiParamsTag.Length == 2)
                        log.InfoFormat("GeoAsciiParamsTag {0}", GeoAsciiParamsTag[1]);

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

                    // wgs84 utm
                    if (ProjectedCSTypeGeoKey >= 32601 && ProjectedCSTypeGeoKey <= 32760)
                    {
                        if (ProjectedCSTypeGeoKey > 32700)
                        {
                            var pnt =PointLatLngAlt.FromUTM((ProjectedCSTypeGeoKey - 32700) * -1, x, y);
                            var pnt2 = PointLatLngAlt.FromUTM((ProjectedCSTypeGeoKey - 32700) * -1, x + width * xscale,
                                y + height * yscale);

                            y = pnt.Lat;
                            x = pnt.Lng;
                            xscale = (pnt2.Lng - pnt.Lng) / width;
                            yscale = (pnt2.Lat - pnt.Lat) / height;

                        }

                        if (ProjectedCSTypeGeoKey < 32700)
                        {
                            var pnt = PointLatLngAlt.FromUTM((ProjectedCSTypeGeoKey - 32600), x, y);
                            var pnt2 = PointLatLngAlt.FromUTM((ProjectedCSTypeGeoKey - 32600), x + width * xscale,
                                y + height * yscale);

                            y = pnt.Lat;
                            x = pnt.Lng;
                            xscale = (pnt2.Lng - pnt.Lng) / width;
                            yscale = (pnt2.Lat - pnt.Lat) / height;
                        }
                    }

                    Area = new RectLatLng(y, x, width*xscale, height*yscale);

                    log.InfoFormat("Coverage {0}", Area.ToString());

                    log.InfoFormat("CacheAble {0}", cacheable.ToString());

                    // starts from top left so x + y -
                    x += xscale / 2.0;
                    y -= yscale / 2.0;

                    log.InfoFormat("Start Point ({0},{1},{2}) --> ({3},{4},{5})", i, j, k, x, y, z);

                    lock (index)
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

            private long size = -1;

            public bool cacheable
            {
                get
                {
                    if (size == -1) size = new FileInfo(FileName).Length;
                    return size < 1024 * 1024 * 1000;
                }
            }

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
            lock (index)
                if (index.Count == 0)
                return srtm.altresponce.Invalid;

            var answer = new srtm.altresponce();

            foreach (var geotiffdata in index.ToArray())
            {
                if (geotiffdata.Area.Contains(lat, lng))
                {
                    // add to cache
                    if (!cache.ContainsKey(geotiffdata.FileName) && geotiffdata.cacheable)
                    {
                        if (!File.Exists(geotiffdata.FileName))
                            continue;

                        lock (cacheloading)
                        {
                            if (cacheloading.Contains(geotiffdata.FileName))
                                return srtm.altresponce.Invalid;

                            cacheloading.Add(geotiffdata.FileName);
                        }

                        Task.Run(() => { 
                        try
                        {
                            float[,] altdata = new float[geotiffdata.height, geotiffdata.width];

                            using (Tiff tiff = Tiff.Open(geotiffdata.FileName, "r"))
                            {
                                if (tiff.GetField(TiffTag.TILEWIDTH) != null &&
                                    tiff.GetField(TiffTag.TILEWIDTH).Length >= 1)
                                {
                                    FieldValue[] value = tiff.GetField(TiffTag.IMAGEWIDTH);
                                    int imageWidth = value[0].ToInt();

                                    value = tiff.GetField(TiffTag.IMAGELENGTH);
                                    int imageLength = value[0].ToInt();

                                    value = tiff.GetField(TiffTag.TILEWIDTH);
                                    int tileWidth = value[0].ToInt();

                                    value = tiff.GetField(TiffTag.TILELENGTH);
                                    int tileLength = value[0].ToInt();

                                    byte[] buf = new byte[tiff.TileSize()];
                                    for (int y = 0; y < imageLength; y += tileLength)
                                    {
                                        for (int x = 0; x < imageWidth; x += tileWidth)
                                        {
                                            tiff.ReadTile(buf, 0, x, y, 0, 0);

                                            for (int row = 0; row < tileLength; row++)
                                            {
                                                for (int col = 0; col < tileWidth; col++)
                                                {
                                                    if (x + col >= imageWidth || y + row >= imageLength)
                                                        break;

                                                    if (geotiffdata.bits == 16)
                                                    {
                                                        altdata[y + row, x + col] =
                                                            (short) ((buf[row * tileWidth * 2 + col * 2 + 1] << 8) +
                                                                     buf[row * tileWidth * 2 + col * 2]);
                                                    }
                                                    else if (geotiffdata.bits == 32)
                                                    {
                                                        altdata[y + row, x + col] =
                                                            (float) BitConverter.ToSingle(buf,
                                                                row * tileWidth * 4 + col * 4);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {

                                    byte[] scanline = new byte[tiff.ScanlineSize()];

                                    for (int row = 0; row < geotiffdata.height; row++)
                                    {
                                        tiff.ReadScanline(scanline, row);

                                        for (int col = 0; col < geotiffdata.width; col++)
                                        {
                                            if (geotiffdata.bits == 16)
                                            {
                                                altdata[row, col] =
                                                    (short) ((scanline[col * 2 + 1] << 8) + scanline[col * 2]);
                                            }
                                            else if (geotiffdata.bits == 32)
                                            {
                                                altdata[row, col] = (float) BitConverter.ToSingle(scanline, col * 4);
                                            }
                                        }
                                    }
                                }
                            }

                            cache[geotiffdata.FileName] = altdata;
                        }
                        finally
                        {
                            lock (cacheloading)
                            {
                                cacheloading.Remove(geotiffdata.FileName);
                            }
                        }
                        });

                        return srtm.altresponce.Invalid;
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
                    if(alt00 < -1000 || alt10 < -1000 || alt01 < -1000 || alt11 < -1000 )
                        answer.currenttype = srtm.tiletype.invalid;
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
