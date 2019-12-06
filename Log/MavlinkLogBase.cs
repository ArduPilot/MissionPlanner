﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MissionPlanner.Utilities;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;

namespace MissionPlanner.Log
{
    public class MavlinkLogBase
    {
        public static int[] ColourValues = new int[]
        {
            0xFF0000, 0x00FF00, 0x0000FF, 0xFFFF00, 0xFF00FF, 0x00FFFF,
            0x800000, 0x008000, 0x000080, 0x808000, /*0x800080,0x008080,  */
            0xC00000, 0x00C000, 0x0000C0, 0xC0C000, 0xC000C0, 0x00C0C0,
            /*  0x400000,0x004000,0x000040,0x404000,0x400040,0x004040, 
           0x200000,0x002000,0x000020,0x202000,0x200020,0x002020, 
            0x600000,0x006000,0x000060,0x606000,0x600060,0x006060,  
            0xA00000,0x00A000,0x0000A0,0xA0A000,0xA000A0,0x00A0A0, 
            0xE00000,0x00E000,0x0000E0,0xE0E000,0xE000E0,0x00E0E0,  */
        };

        public static void writeGPX(string filename, List<CurrentState> flightdata)
        {
            System.Xml.XmlTextWriter xw =
                new System.Xml.XmlTextWriter(
                    Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar +
                    Path.GetFileNameWithoutExtension(filename) + ".gpx", Encoding.ASCII);

            xw.WriteStartElement("gpx");
            xw.WriteAttributeString("creator", MainV2.instance.Text);
            xw.WriteAttributeString("xmlns", "http://www.topografix.com/GPX/1/1");

            xw.WriteStartElement("trk");

            xw.WriteStartElement("trkseg");

            foreach (CurrentState cs in flightdata)
            {
                xw.WriteStartElement("trkpt");
                xw.WriteAttributeString("lat", cs.lat.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteAttributeString("lon", cs.lng.ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("ele", cs.altasl.ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("ele2", cs.alt.ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("time", cs.datetime.ToString("yyyy-MM-ddTHH:mm:sszzzzzz"));
                xw.WriteElementString("course", (cs.yaw).ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("roll", cs.roll.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("pitch", cs.pitch.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("mode", cs.mode.ToString(new System.Globalization.CultureInfo("en-US")));
                //xw.WriteElementString("speed", mod.model.Orientation.);
                //xw.WriteElementString("fix", cs.altitude);

                xw.WriteEndElement();
            }

            xw.WriteEndElement();
            xw.WriteEndElement();

            int a = 0;
            DateTime lastsample = DateTime.MinValue;
            foreach (CurrentState cs in flightdata)
            {
                if (cs.datetime.Second != lastsample.Second)
                {
                    lastsample = cs.datetime;
                }
                else
                {
                    //continue;
                }

                xw.WriteStartElement("wpt");
                xw.WriteAttributeString("lat", cs.lat.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteAttributeString("lon", cs.lng.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("name", (a++).ToString());
                xw.WriteElementString("time", cs.datetime.ToString("yyyy-MM-ddTHH:mm:sszzzzzz"));
                xw.WriteElementString("ele", cs.altasl.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteEndElement(); //wpt
            }

            xw.WriteEndElement();

            xw.Close();
        }
        public static Color HexStringToColor(string hexColor)
        {
            string hc = (hexColor);
            if (hc.Length != 8)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Color.Empty;
            }
            string a = hc.Substring(0, 2);
            string r = hc.Substring(6, 2);
            string g = hc.Substring(4, 2);
            string b = hc.Substring(2, 2);
            Color color = Color.Empty;
            try
            {
                int ai
                    = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                int ri
                    = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                    = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                    = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb(ai, ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Color.Empty;
            }
            return color;
        }

        public static void writeKML(string filename, Dictionary<int, List<CurrentState>> flightdatas, Action<double> progressBar1,double basealt = 0)
        {
            SharpKml.Dom.AltitudeMode altmode = SharpKml.Dom.AltitudeMode.Absolute;

            Color[] colours =
            {
                Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo,
                Color.Violet, Color.Pink
            };

            Document kml = new Document();

            AddNamespace(kml, "gx", "http://www.google.com/kml/ext/2.2");

            Style style = new Style();
            style.Id = "yellowLineGreenPoly";
            style.Line = new LineStyle(new Color32(HexStringToColor("7f00ffff")), 4);


            PolygonStyle pstyle = new PolygonStyle();
            pstyle.Color = new Color32(HexStringToColor("7f00ff00"));
            style.Polygon = pstyle;

            kml.AddStyle(style);

            Style stylet = new Style();
            stylet.Id = "track";
            SharpKml.Dom.IconStyle ico = new SharpKml.Dom.IconStyle();
            LabelStyle lst = new LabelStyle();
            lst.Scale = 0;
            stylet.Icon = ico;
            ico.Icon =
                new IconStyle.IconLink(
                    new Uri("http://earth.google.com/images/kml-icons/track-directional/track-none.png"));
            stylet.Icon.Scale = 0.5;
            stylet.Label = lst;

            kml.AddStyle(stylet);

            foreach (var flightdatai in flightdatas)
            {
                var sysid = flightdatai.Key;
                var flightdata = flightdatai.Value;

                Tour tour = new Tour() { Name = "First Person View" };
                Playlist tourplaylist = new Playlist();

                // create sub folders
                Folder planes = new Folder();
                planes.Name = "Models " + sysid;
                kml.AddFeature(planes);

                Folder points = new Folder();
                points.Name = "Points " + sysid;
                kml.AddFeature(points);

                // coords for line string
                CoordinateCollection coords = new CoordinateCollection();

                int a = 1;
                int c = -1;
                DateTime lasttime = DateTime.MaxValue;
                DateTime starttime = DateTime.MinValue;
                Color stylecolor = Color.AliceBlue;
                string mode = "";
                if (flightdata.Count > 0)
                {
                    mode = flightdata[0].mode;
                }

                foreach (CurrentState cs in flightdata)
                {
                    progressBar1(50 + (int) ((float) a / (float) flightdata.Count * 100.0f / 2.0f));

                    if (starttime == DateTime.MinValue)
                    {
                        starttime = cs.datetime;
                        lasttime = cs.datetime;
                    }

                    if (mode != cs.mode || flightdata.Count == a)
                    {
                        c++;

                        LineString ls = new LineString();
                        ls.AltitudeMode = altmode;
                        ls.Extrude = true;

                        ls.Coordinates = coords;

                        Placemark pm = new Placemark();

                        pm.Name = c + " Flight Path " + mode;
                        pm.StyleUrl = new Uri("#yellowLineGreenPoly", UriKind.Relative);
                        pm.Geometry = ls;

                        SharpKml.Dom.TimeSpan ts = new SharpKml.Dom.TimeSpan();
                        ts.Begin = starttime;
                        ts.End = cs.datetime;

                        pm.Time = ts;

                        // setup for next line
                        mode = cs.mode;
                        starttime = cs.datetime;

                        stylecolor = colours[c % (colours.Length - 1)];

                        Style style2 = new Style();
                        style2.Line = new LineStyle(new Color32(stylecolor), 4);

                        pm.StyleSelector = style2;

                        kml.AddFeature(pm);

                        coords = new CoordinateCollection();
                    }

                    Vector location = new Vector(cs.lat, cs.lng, cs.altasl);

                    if (basealt != 0)
                    {
                        location.Altitude = cs.alt + basealt;
                        coords.Add(location);
                    }
                    else
                    {
                        coords.Add(location);
                    }

                    SharpKml.Dom.Timestamp tstamp = new SharpKml.Dom.Timestamp();
                    tstamp.When = cs.datetime;

                    FlyTo flyto = new FlyTo();

                    flyto.Duration = (cs.datetime - lasttime).TotalMilliseconds / 1000.0;

                    flyto.Mode = FlyToMode.Smooth;
                    SharpKml.Dom.Camera cam = new SharpKml.Dom.Camera();
                    cam.AltitudeMode = altmode;
                    cam.Latitude = cs.lat;
                    cam.Longitude = cs.lng;
                    cam.Altitude = location.Altitude;
                    cam.Heading = cs.yaw;
                    cam.Roll = -cs.roll;
                    cam.Tilt = (90 - (cs.pitch * -1));

                    cam.GXTimePrimitive = tstamp;

                    flyto.View = cam;
                    //if (Math.Abs(flyto.Duration.Value) > 0.1)
                    {
                        tourplaylist.AddTourPrimitive(flyto);
                        lasttime = cs.datetime;
                    }


                    Placemark pmplane = new Placemark();
                    pmplane.Name = "Point " + a;


                    pmplane.Time = tstamp;

                    pmplane.Visibility = false;

                    SharpKml.Dom.Location loc = new SharpKml.Dom.Location();
                    loc.Latitude = cs.lat;
                    loc.Longitude = cs.lng;
                    loc.Altitude = location.Altitude;

                    if (loc.Altitude < 0)
                        loc.Altitude = 0.01;

                    SharpKml.Dom.Orientation ori = new SharpKml.Dom.Orientation();
                    ori.Heading = cs.yaw;
                    ori.Roll = -cs.roll;
                    ori.Tilt = -cs.pitch;

                    SharpKml.Dom.Scale sca = new SharpKml.Dom.Scale();

                    sca.X = 2;
                    sca.Y = 2;
                    sca.Z = 2;

                    Model model = new Model();
                    model.Location = loc;
                    model.Orientation = ori;
                    model.AltitudeMode = altmode;
                    model.Scale = sca;

                    try
                    {
                        Description desc = new Description();
                        desc.Text = @"<![CDATA[
              <table>
                <tr><td>Roll: " + model.Orientation.Roll.Value.ToString("0.00") + @" </td></tr>
                <tr><td>Pitch: " + model.Orientation.Tilt.Value.ToString("0.00") + @" </td></tr>
                <tr><td>Yaw: " + model.Orientation.Heading.Value.ToString("0.00") + @" </td></tr>
                <tr><td>Time: " + cs.datetime.ToString("HH:mm:sszzzzzz") + @" </td></tr>
              </table> ]]>";
                        //            ]]>";

                        pmplane.Description = desc;
                    }
                    catch
                    {
                    }

                    SharpKml.Dom.Link link = new SharpKml.Dom.Link();
                    link.Href = new Uri("block_plane_0.dae", UriKind.Relative);

                    model.Link = link;

                    pmplane.Geometry = model;

                    planes.AddFeature(pmplane);

                    ///

                    Placemark pmt = new Placemark();

                    SharpKml.Dom.Point pnt = new SharpKml.Dom.Point();
                    pnt.AltitudeMode = altmode;
                    pnt.Coordinate = location;

                    pmt.Name = "" + a;

                    pmt.Description = pmplane.Description;

                    pmt.Time = tstamp;

                    pmt.Geometry = pnt;
                    pmt.StyleUrl = new Uri("#track", UriKind.Relative);

                    points.AddFeature(pmt);

                    a++;
                }

                tour.Playlist = tourplaylist;

                kml.AddFeature(tour);
            }

            Serializer serializer = new Serializer();
            serializer.Serialize(kml);


            //Console.WriteLine(serializer.Xml);

            StreamWriter sw = new StreamWriter(filename);
            sw.Write(serializer.Xml);
            sw.Close();

            // create kmz - aka zip file

            FileStream fs = File.Open(filename.Replace(Path.GetExtension(filename), ".kmz"), FileMode.Create);
            ZipOutputStream zipStream = new ZipOutputStream(fs);
            zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
            zipStream.UseZip64 = UseZip64.Off; // older zipfile

            // entry 1
            string entryName = ZipEntry.CleanName(Path.GetFileName(filename));
            // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            File.Delete(filename);

            filename = Settings.GetRunningDirectory() +
                       "block_plane_0.dae";

            // entry 2
            entryName = ZipEntry.CleanName(Path.GetFileName(filename));
            // Removes drive from name and fixes slash direction
            newEntry = new ZipEntry(entryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();


            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        static void AddNamespace(Element element, string prefix, string uri)
        {
            // The Namespaces property is marked as internal.
            PropertyInfo property = typeof(Element).GetProperty(
                "Namespaces",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var namespaces = (XmlNamespaceManager)property.GetValue(element, null);
            namespaces.AddNamespace(prefix, uri);
        }
    }
}