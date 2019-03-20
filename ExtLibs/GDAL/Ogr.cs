using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OSGeo.OGR;
using OSGeo.OSR;

namespace GDAL
{
    [StructLayout(LayoutKind.Explicit, Size = (3*8))]
    public struct point
    {
        [FieldOffset(0)]
        public double x;
        [FieldOffset(8)]
        public double y;
        [FieldOffset(16)]
        public double z;

        public point(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public point(double[] pnt)
        {
            this.x = pnt[0];
            this.y = pnt[1];
            this.z = pnt[2];
        }

        public static implicit operator point(double[] a)
        {
            return new point(a);
        }

        public static implicit operator double[](point a)
        {
            return new[] {a.x, a.y, a.z};
        }

        public override string ToString()
        {
            return String.Format("point ({0},{1},{2})", x, y, z);
        }
    }

    public class OGR: IDisposable
    {
        DataSource dataSource;

        public delegate void singlepoint(point pnt);

        public delegate void linestring(List<point> ls);

        public delegate void polygon(List<point> ls);

        public event singlepoint NewPoint;
        public event linestring NewLineString;
        public event polygon NewPolygon;

        public static OGR Open(string inputFile)
        {
            if (!File.Exists(inputFile))
                return null;

            GdalConfiguration.ConfigureOgr();
            GdalConfiguration.ConfigureGdal();

            //Reading the vector data  
            DataSource dataSource = Ogr.Open(inputFile, 0);

            Driver drv = dataSource.GetDriver();
            Console.WriteLine("Using driver " + drv.name);

            var count = dataSource.GetLayerCount();

            for (var a = 0; a < count; a++)
            {
                Layer layer = dataSource.GetLayerByIndex(a);
                var litems = layer.GetFeatureCount(0);
                //var feature1 = layer.GetFeature(0);
                var geomtype = layer.GetGeomType();
                var refcnt = layer.GetRefCount();
                var layerdef = layer.GetLayerDefn();
                var spatref = layer.GetSpatialRef();
                var spatfilter = layer.GetSpatialFilter();
                var lname = layer.GetName();
                Envelope envelope = new Envelope();
                layer.GetExtent(envelope, 0);

                ReportLayer(layer);
            }

            var ret = new OGR();
            ret.dataSource = dataSource;
            return ret;
        }

        public void Process()
        {
            var count = dataSource.GetLayerCount();

            for (var a = 0; a < count; a++)
            {
                Layer layer = dataSource.GetLayerByIndex(a);
                var fcount = layer.GetFeatureCount(0);
                for (var f = 0; f <= fcount; f++)
                {
                    var feature = layer.GetFeature(f);
                    if (feature == null)
                        continue;
                    var geom = feature.GetGeometryRef();

                    ProcessGeometry(geom);
                }
            }
        }

        private void ProcessGeometry(Geometry geom)
        {
            if (geom == null)
                return;

            var GeometryType = geom.GetGeometryType();

            switch ((wkbGeometryType) ((int) GeometryType & 0xffff))
            {
                case wkbGeometryType.wkbUnknown:
                    break;
                case wkbGeometryType.wkbPoint:
                    var pnt = new double[3];
                    geom.GetPoint(0, pnt);
                    NewPoint?.Invoke(new point(pnt));
                    break;
                case wkbGeometryType.wkbLineString:
                {
                    List<point> ls = new List<point>();
                    var pointcount = geom.GetPointCount();
                    for (int p = 0; p < pointcount; p++)
                    {
                        double[] pnt2 = new double[3];
                        geom.GetPoint(p, pnt2);
                        ls.Add(pnt2);
                    }

                    NewLineString?.Invoke(ls);
                    break;
                }
                case wkbGeometryType.wkbPolygon:
                {
                    List<point> poly = new List<point>();
                    for (int i = 0; i < geom.GetGeometryCount(); i++)
                    {
                        var geom2 = geom.GetGeometryRef(i);
                        var pointcount1 = geom2.GetPointCount();
                        for (int p = 0; p < pointcount1; p++)
                        {
                            double[] pnt2 = new double[3];
                            geom2.GetPoint(p, pnt2);
                            poly.Add(pnt2);
                        }

                        NewPolygon?.Invoke(poly);
                    }

                    break;
                }
                case wkbGeometryType.wkbMultiPoint:
                case wkbGeometryType.wkbMultiLineString:
                case wkbGeometryType.wkbMultiPolygon:
                case wkbGeometryType.wkbGeometryCollection:
                case wkbGeometryType.wkbLinearRing:
                {
                    Geometry sub_geom;
                    for (int i = 0; i < geom.GetGeometryCount(); i++)
                    {
                        sub_geom = geom.GetGeometryRef(i);
                        ProcessGeometry(sub_geom);
                    }

                    break;
                }
                case wkbGeometryType.wkbNone:

                    break;
            }
        }

        public static void ReportLayer(Layer layer)
        {
            FeatureDefn def = layer.GetLayerDefn();
            Console.WriteLine("Layer name: " + def.GetName());
            Console.WriteLine("Feature Count: " + layer.GetFeatureCount(1));
            Envelope ext = new Envelope();
            layer.GetExtent(ext, 1);
            Console.WriteLine("Extent: " + ext.MinX + "," + ext.MaxX + "," +
                ext.MinY + "," + ext.MaxY);

            /* -------------------------------------------------------------------- */
            /*      Reading the spatial reference                                   */
            /* -------------------------------------------------------------------- */
            OSGeo.OSR.SpatialReference sr = layer.GetSpatialRef();
            string srs_wkt;
            if (sr != null)
            {
                sr.ExportToPrettyWkt(out srs_wkt, 1);
            }
            else
                srs_wkt = "(unknown)";


            Console.WriteLine("Layer SRS WKT: " + srs_wkt);

            /* -------------------------------------------------------------------- */
            /*      Reading the fields                                              */
            /* -------------------------------------------------------------------- */
            Console.WriteLine("Field definition:");
            for (int iAttr = 0; iAttr < def.GetFieldCount(); iAttr++)
            {
                FieldDefn fdef = def.GetFieldDefn(iAttr);

                Console.WriteLine(fdef.GetNameRef() + ": " +
                    fdef.GetFieldTypeName(fdef.GetFieldType()) + " (" +
                    fdef.GetWidth() + "." +
                    fdef.GetPrecision() + ")");
            }

            /* -------------------------------------------------------------------- */
            /*      Reading the shapes                                              */
            /* -------------------------------------------------------------------- */
            Console.WriteLine("");
            Feature feat;
            while ((feat = layer.GetNextFeature()) != null)
            {
                ReportFeature(feat, def);
                feat.Dispose();
            }
        }

        public static void ReportFeature(Feature feat, FeatureDefn def)
        {
            Console.WriteLine("Feature(" + def.GetName() + "): " + feat.GetFID());
            for (int iField = 0; iField < feat.GetFieldCount(); iField++)
            {
                FieldDefn fdef = def.GetFieldDefn(iField);

                Console.Write(fdef.GetNameRef() + " (" +
                    fdef.GetFieldTypeName(fdef.GetFieldType()) + ") = ");

                if (feat.IsFieldSet(iField))
                {
                    if (fdef.GetFieldType() == FieldType.OFTStringList)
                    {
                        string[] sList = feat.GetFieldAsStringList(iField);
                        foreach (string s in sList)
                        {
                            Console.Write("\"" + s + "\" ");
                        }
                        Console.WriteLine();
                    }
                    else if (fdef.GetFieldType() == FieldType.OFTIntegerList)
                    {
                        int count;
                        int[] iList = feat.GetFieldAsIntegerList(iField, out count);
                        for (int i = 0; i < count; i++)
                        {
                            Console.Write(iList[i] + " ");
                        }
                        Console.WriteLine();
                    }
                    else if (fdef.GetFieldType() == FieldType.OFTRealList)
                    {
                        int count;
                        double[] iList = feat.GetFieldAsDoubleList(iField, out count);
                        for (int i = 0; i < count; i++)
                        {
                            Console.Write(iList[i].ToString() + " ");
                        }
                        Console.WriteLine();
                    }
                    else
                        Console.WriteLine(feat.GetFieldAsString(iField));
                }
                else
                    Console.WriteLine("(null)");

            }

            if (feat.GetStyleString() != null)
                Console.WriteLine("  Style = " + feat.GetStyleString());

            Geometry geom = feat.GetGeometryRef();
            if (geom != null)
            {
                Console.WriteLine("  " + geom.GetGeometryName() +
                    "(" + geom.GetGeometryType() + ")");
                Geometry sub_geom;
                for (int i = 0; i < geom.GetGeometryCount(); i++)
                {
                    sub_geom = geom.GetGeometryRef(i);
                    if (sub_geom != null)
                    {
                        Console.WriteLine("  subgeom" + i + ": " + sub_geom.GetGeometryName() +
                            "(" + sub_geom.GetGeometryType() + ")");
                    }
                }
                Envelope env = new Envelope();
                geom.GetEnvelope(env);
                Console.WriteLine("   ENVELOPE: " + env.MinX + "," + env.MaxX + "," +
                    env.MinY + "," + env.MaxY);

                string geom_wkt;
                geom.ExportToWkt(out geom_wkt);
                Console.WriteLine("  " + geom_wkt);
            }

            Console.WriteLine("");
        }

        public void Dispose()
        {
            dataSource.Dispose();
        }
    }
}
