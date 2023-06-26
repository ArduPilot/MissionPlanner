using GeoJSON.Net;
using GMap.NET;
using GMap.NET.WindowsForms;
using Ionic.Zip;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace MissionPlanner.NoFly
{
    public class NoFly
    {
        private const int proximity = 100000;

        static GMapOverlay kmlpolygonsoverlay = new GMapOverlay();

        private static string directory = Settings.GetRunningDirectory() + "NoFly";

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
            if (!Settings.Instance.GetBoolean("ShowNoFly", true))
                return;

            var files = Directory.GetFiles(directory, "*.kmz");

            foreach (var file in files)
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

                    Directory.Delete(outputDirectory, true);
                }
                catch
                {
                }
            }

            try
            {
                Utilities.nfz.HK.ConfirmNoFly += () =>
                {
                    return CustomMessageBox.Show("Show Hong Kong No fly zones?", "NoFly Zones", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes;
                };

                var nfzinfo = Utilities.nfz.HK.LoadNFZ().Result;

                if (nfzinfo != null)
                    UpdateNoFlyZoneEvent += (sender, args) =>
                    {
                        if (nfzinfo.Type == GeoJSONObjectType.FeatureCollection)
                        {
                            foreach (var item in nfzinfo.Features)
                            {
                                if (item.Type == GeoJSONObjectType.Feature)
                                {
                                    if (item.Geometry.Type == GeoJSONObjectType.Polygon)
                                    {
                                        var poly = (GeoJSON.Net.Geometry.Polygon)item.Geometry;
                                        var coordinates =
                                            poly.Coordinates[0].Coordinates.OfType<GeoJSON.Net.Geometry.Position>()
                                                .Select(c => new PointLatLng(c.Latitude, c.Longitude))
                                                .ToList();

                                        var close = coordinates.Any(a => a.ToPLLA(0).GetDistance(args) < 100000);
                                        if (!close)
                                            continue;

                                        var name = item.Properties["name"];
                                        var desc = item.Properties["description"];

                                        GMapPolygon nfzpolygon = new GMapPolygon(coordinates, "HK"+name.ToString());
                                        nfzpolygon.Tag = item;

                                        nfzpolygon.Stroke.Color = Color.Purple;

                                        nfzpolygon.Fill = new SolidBrush(Color.FromArgb(30, Color.Blue));

                                        MainV2.instance.BeginInvoke(new Action(() =>
                                        {
                                            if (kmlpolygonsoverlay.Polygons.Any(a => a.Name == "HK" + name.ToString()))
                                                return;
                                            kmlpolygonsoverlay.Polygons.Add(nfzpolygon);
                                        }));
                                    }
                                }
                            }
                        }
                    };
            }
            catch
            {
            }

            try
            {
                Utilities.nfz.EU.ConfirmNoFly += () =>
                {
                    return CustomMessageBox.Show("Show European Union No fly zones?", "NoFly Zones", CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes;
                };

                var nfzinfo = Utilities.nfz.EU.LoadNFZ().Result;

                if (nfzinfo != null)
                    UpdateNoFlyZoneEvent += (sender, args) =>
                    {
                        foreach (var feat in nfzinfo.Features)
                        {
                            foreach (var item in feat.Geometry)
                            {
                                if (item.HorizontalProjection?.Type == "Polygon")
                                {
                                    //if (item.LowerVerticalReference == "AGL" && item.UomDimensions == "M" && item.LowerLimit > 300)
                                    //continue;

                                    var coordinates = item.HorizontalProjection.Coordinates[0].Select(c => new PointLatLng(c[1], c[0])).ToList();

                                    var close = coordinates.Any(a => a.ToPLLA(item.LowerLimit).GetDistance(args) < 100000);
                                    if (!close)
                                        continue;

                                    GMapPolygon nfzpolygon = new GMapPolygon(coordinates, feat.Name);

                                    nfzpolygon.Tag = item;

                                    nfzpolygon.Stroke.Color = Color.Purple;

                                    nfzpolygon.Fill = new SolidBrush(Color.FromArgb(30, Color.Blue));

                                    nfzpolygon.IsHitTestVisible = true;
                                    /*
                                    nfzpolygon.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                    nfzpolygon.ToolTipText = feat.Name + "\r\n" + feat.Message;
                                    */
                                    if (kmlpolygonsoverlay.Control.IsMouseOverPolygon) { 

                                    }
                                    MainV2.instance.BeginInvoke(new Action(() =>
                                    {
                                        if (kmlpolygonsoverlay.Polygons.Any(a => a.Name == feat.Name))
                                            return;
                                        kmlpolygonsoverlay.Polygons.Add(nfzpolygon);
                                    }));
                                }
                                else if (item.HorizontalProjection?.Type == "Circle")
                                {
                                    var coordinates = new PointLatLng(item.HorizontalProjection.Center[1], item.HorizontalProjection.Center[0]);

                                    var close = coordinates.ToPLLA(item.LowerLimit).GetDistance(args) < proximity;
                                    if (!close)
                                        continue;

                                    GMapMarkerAirport nfzcircle = new GMapMarkerAirport(coordinates);
                                    nfzcircle.wprad = (int)(item.HorizontalProjection.Radius ?? 0);
                                    nfzcircle.Tag = feat;
                                    nfzcircle.IsHitTestVisible = true;
                                    nfzcircle.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                    nfzcircle.ToolTipText = feat.Name +"\r\n"+ feat.Message;

                                    MainV2.instance.BeginInvoke(new Action(() =>
                                    {
                                        if (kmlpolygonsoverlay.Markers.Any(a => ((Utilities.nfz.Feature)a.Tag).Name == feat.Name))
                                            return;
                                        kmlpolygonsoverlay.Markers.Add(nfzcircle);
                                    }));
                                }
                            }

                        }
                    };
            }
            catch
            {
            }

            if (NoFlyEvent != null)
                NoFlyEvent(null, new NoFlyEventArgs(kmlpolygonsoverlay));
        }

        static PointLatLngAlt lastUpdateLocation = PointLatLngAlt.Zero;
        public static void UpdateNoFlyZone(object sender, PointLatLngAlt plla)
        {
            if (plla.GetDistance(lastUpdateLocation) > 100)
            {
                UpdateNoFlyZoneEvent?.Invoke(sender, plla);
                lastUpdateLocation = plla;
            }
        }

        public static event EventHandler<PointLatLngAlt> UpdateNoFlyZoneEvent;

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
            MultipleGeometry geom = Element as MultipleGeometry;

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
                GMapPolygon kmlpolygon = new GMapPolygon(new List<PointLatLng>(), polygon.Id);

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
            else if (geom != null)
            {
                foreach (var geometry in geom.Geometry)
                {
                    processKML(geometry);
                }
            }
        }
    }
}