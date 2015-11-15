using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;

namespace MissionPlanner.NoFly
{
    public class NoFly
    {
        static GMapOverlay kmlpolygonsoverlay = new GMapOverlay();

        private static string directory = Application.StartupPath + Path.DirectorySeparatorChar + "NoFly";

        public static event EventHandler<NoFlyEventArgs> NoFlyEvent;

        public class NoFlyEventArgs : EventArgs
        {
            public NoFlyEventArgs(GMapOverlay overlay)
            {
                NoFlyZones = overlay;
            }

            public GMapOverlay NoFlyZones { get; set; }
        }

        public static void Scan()
        {
            var files = Directory.GetFiles(directory, "*.kmz");

            foreach (var file in  files)
            {
                try
                {
                    // get a temp dir
                    var outputDirectory = Path.GetTempPath() + Path.DirectorySeparatorChar + "mpkml" +
                                          DateTime.Now.Ticks;
                    using (var zip = ZipFile.Read(File.OpenRead(file)))
                    {
                        zip.ExtractAll(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }

                    var kmls = Directory.GetFiles(outputDirectory, "*.kml");
                    foreach (var kml in kmls)
                    {
                        LoadNoFly(kml);
                    }
                }
                catch
                {
                }
            }

            if (NoFlyEvent != null)
                NoFlyEvent(null, new NoFlyEventArgs(kmlpolygonsoverlay));
        }


        public static void LoadNoFly(string file)
        {
            string kml = "";

            using (var sr = new StreamReader(File.OpenRead(file)))
            {
                kml = sr.ReadToEnd();
                sr.Close();
            }

            kml = kml.Replace("<Snippet/>", "");

            var parser = new SharpKml.Base.Parser();

            parser.ElementAdded += parser_ElementAdded;
            parser.ParseString(kml, false);
        }

        static void parser_ElementAdded(object sender, SharpKml.Base.ElementEventArgs e)
        {
            processKML(e.Element);
        }

        private static void processKML(SharpKml.Dom.Element Element)
        {
            try
            {
                //  log.Info(Element.ToString() + " " + Element.Parent);
            }
            catch
            {
            }

            SharpKml.Dom.Document doc = Element as SharpKml.Dom.Document;
            SharpKml.Dom.Placemark pm = Element as SharpKml.Dom.Placemark;
            SharpKml.Dom.Folder folder = Element as SharpKml.Dom.Folder;
            SharpKml.Dom.Polygon polygon = Element as SharpKml.Dom.Polygon;
            SharpKml.Dom.LineString ls = Element as SharpKml.Dom.LineString;

            if (doc != null)
            {
                foreach (var feat in doc.Features)
                {
                    //Console.WriteLine("feat " + feat.GetType());
                    //processKML((Element)feat);
                }
            }
            else if (folder != null)
            {
                foreach (SharpKml.Dom.Feature feat in folder.Features)
                {
                    //Console.WriteLine("feat "+feat.GetType());
                    //processKML(feat);
                }
            }
            else if (pm != null)
            {
            }
            else if (polygon != null)
            {
                GMapPolygon kmlpolygon = new GMapPolygon(new List<PointLatLng>(), "kmlpolygon");

                kmlpolygon.Stroke.Color = Color.Purple;

                kmlpolygon.Fill = new SolidBrush(Color.FromArgb(30, Color.Blue));

                foreach (var loc in polygon.OuterBoundary.LinearRing.Coordinates)
                {
                    kmlpolygon.Points.Add(new PointLatLng(loc.Latitude, loc.Longitude));
                }

                kmlpolygonsoverlay.Polygons.Add(kmlpolygon);
            }
            else if (ls != null)
            {
                GMapRoute kmlroute = new GMapRoute(new List<PointLatLng>(), "kmlroute");

                kmlroute.Stroke.Color = Color.Purple;

                foreach (var loc in ls.Coordinates)
                {
                    kmlroute.Points.Add(new PointLatLng(loc.Latitude, loc.Longitude));
                }

                kmlpolygonsoverlay.Routes.Add(kmlroute);
            }
        }
    }
}