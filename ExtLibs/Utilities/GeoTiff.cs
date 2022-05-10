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
using Microsoft.Extensions.Caching.Memory;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using GeoAPI.CoordinateSystems;
using DotSpatial.Projections;

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
            enum GTModelTypeGeoKeyEnum
            {
                ModelTypeProjected = 1 ,  /* Projection Coordinate System         */
                ModelTypeGeographic = 2 , /* Geographic latitude-longitude System */
                ModelTypeGeocentric = 3   /* Geocentric (X,Y,Z) Coordinate System */
            }

            enum GTRasterTypeGeoKeyEnum
            {
                RasterPixelIsArea = 1,
                RasterPixelIsPoint = 2
            }

            public enum GKID
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
       

                ModelPixelScaleTag = 33550,
                ModelTiepointTag = 33922,
                GeoKeyDirectoryTag = 34735,
                GeoDoubleParamsTag = 34736,
                GeoAsciiParamsTag = 34737,
                GDAL_METADATA = 42112,
                GDAL_NODATA = 42113
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
                    //https://www.awaresystems.be/imaging/tiff/tifftags/sampleformat.html
                    type = tiff.GetField(TiffTag.SAMPLEFORMAT)[0].ToInt();

                    modelscale = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);
                    tiepoint = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

                    for (int i = 0; i < tiff.GetTagListCount(); i += 1)
                    {
                        var tagno = tiff.GetTagListEntry(i);
                        var tag = (TiffTag)tagno;
                        var info = tiff.GetField((TiffTag)tagno);

                        log.InfoFormat("tiff ID={0} ? {1} len={2}", tag, (GKID)tagno, info.Length);
                    }

                    var GeoKeyDirectoryTag = tiff.GetField((TiffTag)34735);

                    var KeyDirectoryVersion = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 0);
                    var KeyRevision= BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 2);
                    var MinorRevision= BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 4);
                    var NumberOfKeys = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), 6);

                    ProjectedCSTypeGeoKey = 0;

                    for (int i = 8; i < 8 + NumberOfKeys * 8;i+=8)
                    {
                        var KeyID = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i);
                        var TIFFTagLocation = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 2);
                        var Count = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 4);
                        var Value_Offset = BitConverter.ToUInt16(GeoKeyDirectoryTag[1].ToByteArray(), i + 6);

                        log.InfoFormat("GeoKeyDirectoryTag ID={0} TagLoc={1} Count={2} Value/offset={3}", (GKID)KeyID, TIFFTagLocation,
                            Count, Value_Offset);

                        // save it
                        if (TIFFTagLocation == 0)
                            GeoKeys[(GKID)KeyID] = Value_Offset;
                        else if (TIFFTagLocation == 34737)
                            GeoKeys[(GKID)KeyID] = Encoding.ASCII.GetString(tiff.GetField((TiffTag)TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset).Take(Count).ToArray());
                        else if (TIFFTagLocation == 34736)
                            GeoKeys[(GKID)KeyID] = BitConverter.ToDouble(tiff.GetField((TiffTag)TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset * 8).Take(Count * 8).ToArray(), 0);
                        else
                            GeoKeys[(GKID)KeyID] = Value_Offset;


                        if (KeyID == (int)GKID.ProjectedCSTypeGeoKey)
                            ProjectedCSTypeGeoKey = Value_Offset;

                        if (KeyID == (int)GKID.GTRasterTypeGeoKey)
                            GTRasterTypeGeoKey = Value_Offset;

                        if (KeyID == (int)GKID.ProjCoordTransGeoKey)
                            ProjCoordTransGeoKey = Value_Offset;

                        if (TIFFTagLocation != 0)
                        {
                            if (TIFFTagLocation == 34737) //ascii
                            {
                                var value = tiff.GetField((TiffTag) TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset)
                                    .Take(Count);
                                log.InfoFormat("GeoKeyDirectoryTag ID={0} Value={1}", (GKID) KeyID,
                                    Encoding.ASCII.GetString(value.ToArray()));
                            }
                            if (TIFFTagLocation == 34736) // double
                            {
                                
                                var value = tiff.GetField((TiffTag)TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset*8)                                    .Take(Count*8);
                                log.InfoFormat("GeoKeyDirectoryTag ID={0} Value={1}", (GKID)KeyID, BitConverter.ToDouble(value.ToArray(), 0));
                                
                            }
                          
                            if (KeyID == (int)GKID.PCSCitationGeoKey)
                            {
                                var value = tiff.GetField((TiffTag) TIFFTagLocation)[1].ToByteArray().Skip(Value_Offset)                                    .Take(Count);
                                PCSCitationGeoKey = Encoding.ASCII.GetString(value.ToArray());
                                log.InfoFormat("GeoKeyDirectoryTag ID={0} Value={1}", (GKID)KeyID, Encoding.ASCII.GetString(value.ToArray()));
                            }
                        }
                    }

                    GeoAsciiParamsTag = tiff.GetField((TiffTag)34737);
                    if (GeoAsciiParamsTag != null && GeoAsciiParamsTag.Length == 2)
                        log.InfoFormat("GeoAsciiParamsTag 34737 {0}", GeoAsciiParamsTag[1]);

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

                    if (GTRasterTypeGeoKey == 1)
                    {
                        // starts from top left so x + y -
                        x += xscale / 2.0;
                        y -= yscale / 2.0;
                    }

                    if (ProjectedCSTypeGeoKey == 32767 && ProjCoordTransGeoKey == 15)
                    { // user-defined
                        ProjectionInfo pStart = ProjectionInfo.FromProj4String($"+proj=stere +lat_ts={GeoKeys[GKID.ProjOriginLatGeoKey].ToString()} +lat_0=90 +lon_0={GeoKeys[GKID.ProjStraightVertPoleLongGeoKey].ToString()} +x_0=0 +y_0=0 +ellps={GeoKeys[GKID.GeogCitationGeoKey].ToString().Replace(" ", "").Replace("|", "")} +datum={GeoKeys[GKID.GeogCitationGeoKey].ToString().Replace(" ", "").Replace("|", "")} +units=m +no_defs ");
                        ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;

                        srcProjection = pStart;

                        double[] xyarray = { x,y,
                            x + width * xscale, y - height * yscale ,
                            x + width * xscale, y,
                            x, y - height * yscale };
                        Reproject.ReprojectPoints(xyarray, null, pStart, pESRIEnd, 0, xyarray.Length / 2);

                        ymin = Math.Min(Math.Min(Math.Min(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                        xmin = Math.Min(Math.Min(Math.Min(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                        ymax = Math.Max(Math.Max(Math.Max(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                        xmax = Math.Max(Math.Max(Math.Max(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                    }
                    else if (ProjectedCSTypeGeoKey != 32767 && ProjectedCSTypeGeoKey != 0)
                    {
                        try
                        {
                            srcProjection = ProjectionInfo.FromEpsgCode(ProjectedCSTypeGeoKey);

                            ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;

                            double[] xyarray = { x,y,
                            x + width * xscale, y - height * yscale ,
                            x + width * xscale, y,
                            x, y - height * yscale };
                            Reproject.ReprojectPoints(xyarray, null, srcProjection, pESRIEnd, 0, xyarray.Length / 2);

                            ymin = Math.Min(Math.Min(Math.Min(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                            xmin = Math.Min(Math.Min(Math.Min(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                            ymax = Math.Max(Math.Max(Math.Max(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                            xmax = Math.Max(Math.Max(Math.Max(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                        }
                        catch (Exception ex) { log.Error(ex); srcProjection = null; }
                    }
                    else 
                    {
                        try
                        {
                            srcProjection = ProjectionInfo.FromEsriString(GeoKeys[GKID.PCSCitationGeoKey].ToString());

                            ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;

                            double[] xyarray = { x,y,
                            x + width * xscale, y - height * yscale ,
                            x + width * xscale, y,
                            x, y - height * yscale };
                            Reproject.ReprojectPoints(xyarray, null, srcProjection, pESRIEnd, 0, xyarray.Length / 2);

                            ymin = Math.Min(Math.Min(Math.Min(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                            xmin = Math.Min(Math.Min(Math.Min(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                            ymax = Math.Max(Math.Max(Math.Max(xyarray[1], xyarray[3]), xyarray[5]), xyarray[7]);
                            xmax = Math.Max(Math.Max(Math.Max(xyarray[0], xyarray[2]), xyarray[4]), xyarray[6]);
                        }
                        catch (Exception ex) { log.Error(ex); srcProjection = null; }
                    }

                    if (srcProjection != null) {
                        
                    }
                    else
                    // wgs84 utm
                    if (ProjectedCSTypeGeoKey >= 32601 && ProjectedCSTypeGeoKey <= 32760)
                    {
                        if (ProjectedCSTypeGeoKey > 32700)
                        {                            
                            UTMZone = (ProjectedCSTypeGeoKey - 32700) * -1;
                            srcProjection = ProjectionInfo.FromProj4String($"+proj=utm +zone={UTMZone} +ellps=WGS84 +datum=WGS84 +units=m +no_defs ");
                            //tl
                            var pnt = PointLatLngAlt.FromUTM(UTMZone, x, y);
                            //br
                            var pnt2 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale,
                                y - height * yscale);
                            //tr
                            var pnt3 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale, y);
                            //bl
                            var pnt4 = PointLatLngAlt.FromUTM(UTMZone, x, y - height * yscale);

                            ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                            xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                            ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                            xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        }

                        if (ProjectedCSTypeGeoKey < 32700)
                        {
                            UTMZone = ProjectedCSTypeGeoKey - 32600;
                            srcProjection = ProjectionInfo.FromProj4String($"+proj=utm +zone={UTMZone} +ellps=WGS84 +datum=WGS84 +units=m +no_defs ");
                            var pnt = PointLatLngAlt.FromUTM(UTMZone, x, y);
                            var pnt2 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale,
                                y - height * yscale);
                            var pnt3 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale, y);
                            var pnt4 = PointLatLngAlt.FromUTM(UTMZone, x, y - height * yscale);

                            ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                            xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                            ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                            xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        }
                    }
                    else
                    // etrs89 utm
                    if (ProjectedCSTypeGeoKey >= 3038 && ProjectedCSTypeGeoKey <= 3051)
                    {
                        UTMZone = ProjectedCSTypeGeoKey - 3012;
                        srcProjection = ProjectionInfo.FromProj4String($"+proj=utm +zone={UTMZone} +ellps=GRS80 +units=m +no_defs ");
                        // 3038 - 26
                        var pnt = PointLatLngAlt.FromUTM(UTMZone, x, y);
                        var pnt2 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale,
                            y - height * yscale);
                        var pnt3 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale, y);
                        var pnt4 = PointLatLngAlt.FromUTM(UTMZone, x, y - height * yscale);

                        ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                    }
                    else

                    if (ProjectedCSTypeGeoKey >= 25828 && ProjectedCSTypeGeoKey <= 25838)
                    {
                        UTMZone = ProjectedCSTypeGeoKey - 25800;
                        // 3038 - 26
                        var pnt = PointLatLngAlt.FromUTM(UTMZone, x, y);
                        var pnt2 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale,
                            y - height * yscale);
                        var pnt3 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale, y);
                        var pnt4 = PointLatLngAlt.FromUTM(UTMZone, x, y - height * yscale);

                        ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                    }
                    else                 
                    /// gda94
                    if (ProjectedCSTypeGeoKey >= 28348 && ProjectedCSTypeGeoKey <= 28358)
                    {
                        UTMZone = (ProjectedCSTypeGeoKey - 28300) * -1;
                        var pnt = PointLatLngAlt.FromUTM(UTMZone, x, y);
                        var pnt2 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale,
                            y - height * yscale);
                        var pnt3 = PointLatLngAlt.FromUTM(UTMZone, x + width * xscale, y);
                        var pnt4 = PointLatLngAlt.FromUTM(UTMZone, x, y - height * yscale);

                        ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                    }
                    else

                    // geo lat/lng
                    if (ProjectedCSTypeGeoKey == 0 || ProjectedCSTypeGeoKey == 4326)
                    {
                        var pnt = new PointLatLngAlt(y, x);
                        var pnt2 = new PointLatLngAlt(y - height * yscale, x + width * xscale);
                        var pnt3 = new PointLatLngAlt(y, x + width * xscale);
                        var pnt4 = new PointLatLngAlt(y - height * yscale, x);

                        ymin = Math.Min(Math.Min(Math.Min(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmin = Math.Min(Math.Min(Math.Min(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                        ymax = Math.Max(Math.Max(Math.Max(pnt.Lat, pnt2.Lat), pnt3.Lat), pnt4.Lat);
                        xmax = Math.Max(Math.Max(Math.Max(pnt.Lng, pnt2.Lng), pnt3.Lng), pnt4.Lng);
                    }

                    Area = new RectLatLng(ymax, xmin, xmax - xmin, ymax - ymin);

                    log.InfoFormat("Coverage {0}", Area.ToString());


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

            internal Tiff Tiff;

            public string FileName;
            public int width;
            public int height;
            public int bits;
            public int type;
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
            private FieldValue[] modelscale;
            private FieldValue[] tiepoint;
            public int ProjectedCSTypeGeoKey;
            private FieldValue[] GeoAsciiParamsTag;
            public double ymin;
            public double xmin;
            public double ymax;
            public double xmax;
            public int UTMZone;
            /// <summary>
            /// 0= unknown, 1 = PixelIsArea, 2 = PixelIsPoint, 32767 = user def
            /// </summary>
            public ushort GTRasterTypeGeoKey;

            public string PCSCitationGeoKey;

            public ushort ProjCoordTransGeoKey;

            public Dictionary<GKID, object> GeoKeys = new Dictionary<GKID, object>();
            public ProjectionInfo srcProjection;
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
                    // get answer
                    var xf = map(lat, geotiffdata.Area.Top, geotiffdata.Area.Bottom, 0, geotiffdata.height-1);
                    var yf = map(lng, geotiffdata.Area.Left, geotiffdata.Area.Right, 0, geotiffdata.width-1);
                   
                    if (geotiffdata.srcProjection != null) 
                    {
                        ProjectionInfo pESRIEnd = KnownCoordinateSystems.Geographic.World.WGS1984;

                        double[] xyarray = { lng, lat };
                        Reproject.ReprojectPoints(xyarray, null, pESRIEnd, geotiffdata.srcProjection, 0, xyarray.Length / 2);

                        xf = map(xyarray[1], geotiffdata.y, geotiffdata.y - geotiffdata.height * geotiffdata.yscale, 0, geotiffdata.height - 1);
                        yf = map(xyarray[0], geotiffdata.x, geotiffdata.x + geotiffdata.width * geotiffdata.xscale, 0, geotiffdata.width - 1);
                    }           
                    //wgs84 && etrs89
                    else if (geotiffdata.ProjectedCSTypeGeoKey >= 3038 && geotiffdata.ProjectedCSTypeGeoKey <= 3051 ||
                        geotiffdata.ProjectedCSTypeGeoKey >= 32601 && geotiffdata.ProjectedCSTypeGeoKey <= 32760 ||
                        geotiffdata.ProjectedCSTypeGeoKey >= 25828 && geotiffdata.ProjectedCSTypeGeoKey <= 25838 ||
                        geotiffdata.ProjectedCSTypeGeoKey >= 28348 && geotiffdata.ProjectedCSTypeGeoKey <= 28358)
                    {
                        var pnt = PointLatLngAlt.ToUTM((geotiffdata.UTMZone) * 1, lat, lng);

                        xf = map(pnt[1], geotiffdata.y, geotiffdata.y - geotiffdata.height * geotiffdata.yscale, 0,
                            geotiffdata.height - 1);
                        yf = map(pnt[0], geotiffdata.x, geotiffdata.x + geotiffdata.width * geotiffdata.xscale, 0,
                            geotiffdata.width - 1);
                    }

                    int x_int = (int) xf;
                    double x_frac = xf - x_int;

                    int y_int = (int) yf;
                    double y_frac = yf - y_int;

                    
                    //could be on one of the other images
                    if (x_int < 0 || y_int < 0 || x_int >= geotiffdata.height -1  || y_int >= geotiffdata.width-1)
                        continue;

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

        private static MemoryCache cachescanlines =
            new MemoryCache(new MemoryCacheOptions()
            {
                /*SizeLimit = 1024 * 1024 * 500*/
            });

        private static double GetAltNoCache(geotiffdata geotiffdata, int x, int y)
        {
            byte[] scanline;
            lock (cachescanlines)
                scanline = cachescanlines.Get(geotiffdata.FileName.GetHashCode() ^ x.GetHashCode()) as byte[];
            if (scanline == null)
            {
                //Task.Run(() =>
                {
                    lock(geotiffdata)
                        if (geotiffdata.Tiff == null)
                            geotiffdata.Tiff = Tiff.Open(geotiffdata.FileName, "r");

                    lock (geotiffdata.Tiff)
                    {
                        lock (cachescanlines)
                            scanline = cachescanlines.Get(geotiffdata.FileName.GetHashCode() ^ x.GetHashCode()) as byte[];
                        if (scanline == null)
                        {
                            if (geotiffdata.Tiff.GetField(TiffTag.TILEWIDTH) != null &&
                                geotiffdata.Tiff.GetField(TiffTag.TILEWIDTH).Length >= 1)
                            {
                                //log.Info("read tile scanline " + x);
                                //return short.MinValue;

                                ExtractScanLineFromTile(geotiffdata, x);
                                lock (cachescanlines)
                                    scanline = cachescanlines.Get(geotiffdata.FileName.GetHashCode() ^ x.GetHashCode()) as byte[];
                            }
                            else
                            {
                                //log.Info("read scanline " + x);

                                //RowsPerStrip
                                //http://www.libtiff.org/man/TIFFReadScanline.3t.html   
                                var rps = geotiffdata.Tiff.GetField(TiffTag.ROWSPERSTRIP);
                                if (rps != null && rps.Length > 0 &&  (int)rps[0].Value > 1)
                                {                                    
                                    var start = x - (x % (int)rps[0].Value);
                                    for (int i = start; i < start + (int)rps[0].Value; i++)
                                    {
                                        scanline = new byte[geotiffdata.Tiff.ScanlineSize()];
                                        geotiffdata.Tiff.ReadScanline(scanline, i);
                                        AddToCache(geotiffdata, i, scanline);
                                    }
                                }
                                else
                                {
                                    scanline = new byte[geotiffdata.Tiff.ScanlineSize()];
                                    geotiffdata.Tiff.ReadScanline(scanline, x);
                                    AddToCache(geotiffdata, x, scanline);
                                }
                            }
                        }
                    }
                }
                //);
                //return short.MinValue;

                if (scanline == null)
                    return short.MinValue;
            }

            return ProcessScanLine(geotiffdata, y, scanline);
        }

        private static void AddToCache(geotiffdata geotiffdata, int x, byte[] scanline)
        {
            lock (cachescanlines)
            {
                if(cachescanlines.Get(geotiffdata.FileName.GetHashCode() ^ x.GetHashCode()) != null)
                    return;

                var ci = cachescanlines.CreateEntry(geotiffdata.FileName.GetHashCode() ^ x.GetHashCode());
                ci.Value = scanline;
                ci.Size = ((byte[]) ci.Value).Length;
                // evict after no access
                ci.SlidingExpiration = TimeSpan.FromMinutes(5);
                ci.Dispose();
            }
        }

        private static void ExtractScanLineFromTile(geotiffdata geotiffdata, int line)
        {
            FieldValue[] value = geotiffdata.Tiff.GetField(TiffTag.IMAGEWIDTH);
            int imageWidth = value[0].ToInt();

            value = geotiffdata.Tiff.GetField(TiffTag.IMAGELENGTH);
            int imageLength = value[0].ToInt();

            value = geotiffdata.Tiff.GetField(TiffTag.TILEWIDTH);
            int tileWidth = value[0].ToInt();

            value = geotiffdata.Tiff.GetField(TiffTag.TILELENGTH);
            int tileLength = value[0].ToInt();

            var bytespersample = geotiffdata.bits / 8;

            byte[][] scanlines = new byte[tileLength][];
            for (int i = 0; i < scanlines.Length; i++)
            {
                scanlines[i] = new byte[(imageWidth + tileWidth) * bytespersample];
            }

            int tiley = 0;
            byte[] buf = new byte[geotiffdata.Tiff.TileSize()];
            for (int y = 0; y < imageLength; y += tileLength)
            {
                for (int x = 0; x < imageWidth; x += tileWidth)
                {
                    if(y + tileLength < line || y > line)
                        break;

                    tiley = y;
                    geotiffdata.Tiff.ReadTile(buf, 0, x, y, 0, 0);

                    //int row = line % tileLength;

                    for (int i = 0; i < tileLength; i++)
                    {
                        Array.Copy(buf, i * bytespersample * tileLength, scanlines[i], x * bytespersample, tileWidth * bytespersample);
                        //Array.Copy(buf, row * bytespersample * tileLength, scanline, x * bytespersample, tileWidth * bytespersample);
                    }
                }
            }

            foreach (var scanline in scanlines)
            {
                AddToCache(geotiffdata, tiley, scanline);
                tiley++;
            }
        }

        private static double ProcessScanLine(geotiffdata geotiffdata, int y, byte[] scanline)
        {
            if (scanline == null)
            {

            }
            if (geotiffdata.bits == 8)
            {
                throw new Exception("ProcessScanLine: 8bit alt is invalid");
            } 
            else if (geotiffdata.bits == 16)
            {
                return (short) ((scanline[y * 2 + 1] << 8) + scanline[y * 2]);
            }
            else if (geotiffdata.bits == 32 && geotiffdata.type == 1)
            {
                return BitConverter.ToUInt32(scanline, y * 4);
            }
            else if (geotiffdata.bits == 32 && geotiffdata.type == 2)
            {
                return BitConverter.ToInt32(scanline, y * 4);
            }
            else if (geotiffdata.bits == 32 && geotiffdata.type == 3)
            {
                if (y * 4 > scanline.Length)
                    return short.MinValue;
                return BitConverter.ToSingle(scanline, y * 4);
            }
            else if (geotiffdata.bits == 64 && geotiffdata.type == 3)
            {
                if (y * 8 > scanline.Length)
                    return short.MinValue;
                return BitConverter.ToDouble(scanline, y * 8);
            }

            throw new Exception("ProcessScanLine: Invalid geotiff coord");
        }

        private static double GetAlt(geotiffdata geotiffdata, int x, int y)
        {
            if (x < 0 || y < 0 || x >= geotiffdata.height || y >= geotiffdata.width)
                return short.MinValue;
            // if the image is to large use the direct to file approach
            return GetAltNoCache(geotiffdata, x, y);
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
