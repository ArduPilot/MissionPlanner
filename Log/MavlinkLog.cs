﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Text.RegularExpressions;
//using KMLib;
//using KMLib.Feature;
//using KMLib.Geometry;
//using Core.Geometry;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Dom.GX;
using System.Reflection;
using System.Xml;
using log4net;
using ZedGraph; // Graphs
using MissionPlanner.Utilities;
using System.CodeDom.Compiler;
using MissionPlanner;
using MissionPlanner.Controls;

namespace MissionPlanner.Log
{
    public partial class MavlinkLog : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<CurrentState> flightdata = new List<CurrentState>();

        List<string> selection = new List<string>();
        List<string> options = new List<string>();

        Hashtable datappl = new Hashtable();
        Hashtable packetdata = new Hashtable();

        PointLatLngAlt homepos = new PointLatLngAlt();

        public MavlinkLog()
        {
            InitializeComponent();

            zg1.GraphPane.YAxis.Title.IsVisible = false;
            zg1.GraphPane.Title.IsVisible = true;
            zg1.GraphPane.Title.Text = "Mavlink Log Graph";
            zg1.GraphPane.XAxis.Title.Text = "Time (sec)";

            zg1.GraphPane.XAxis.Type = AxisType.Date;
            zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss";
            zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
            zg1.PointDateFormat = "HH:mm:ss";

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void writeKML(string filename, double basealt = 0)
        {
            SharpKml.Dom.AltitudeMode altmode = SharpKml.Dom.AltitudeMode.Absolute;

            Color[] colours =
            {
                Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo,
                Color.Violet, Color.Pink
            };

            Document kml = new Document();

            Tour tour = new Tour() {Name = "First Person View"};
            Playlist tourplaylist = new Playlist();

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

            // create sub folders
            Folder planes = new Folder();
            planes.Name = "Models";
            kml.AddFeature(planes);

            Folder points = new Folder();
            points.Name = "Points";
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
                progressBar1.Value = 50 + (int) ((float) a/(float) flightdata.Count*100.0f/2.0f);
                progressBar1.Refresh();

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

                    stylecolor = colours[c%(colours.Length - 1)];

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

                flyto.Duration = (cs.datetime - lasttime).TotalMilliseconds/1000.0;

                flyto.Mode = FlyToMode.Smooth;
                SharpKml.Dom.Camera cam = new SharpKml.Dom.Camera();
                cam.AltitudeMode = altmode;
                cam.Latitude = cs.lat;
                cam.Longitude = cs.lng;
                cam.Altitude = location.Altitude;
                cam.Heading = cs.yaw;
                cam.Roll = -cs.roll;
                cam.Tilt = (90 - (cs.pitch*-1));

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

            filename = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar +
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
            PropertyInfo property = typeof (Element).GetProperty(
                "Namespaces",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var namespaces = (XmlNamespaceManager) property.GetValue(element, null);
            namespaces.AddNamespace(prefix, uri);
        }

        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //   if (selectform != null)
                //      selectform.Close();
            }
            catch
            {
            }
        }

        private void BUT_redokml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string offsetalt = "0";

                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        InputBox.Show("Alt offset",
                            "Please enter your offset altitude",
                            ref offsetalt);

                        float temp = 0;
                        if (!float.TryParse(offsetalt, out temp))
                        {
                            CustomMessageBox.Show("Bad Offset", "Error");
                            return;
                        }
                    }

                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }
                            mine.logreadmode = true;

                            mine.MAV.packets.Initialize(); // clear

                            double oldlatlngsum = 0;

                            int appui = 0;

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float) mine.logplaybackfile.BaseStream.Position/
                                         (float) mine.logplaybackfile.BaseStream.Length*100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                byte[] packet = mine.readPacket();

                                mine.MAV.cs.datetime = mine.lastlogread;

                                mine.MAV.cs.UpdateCurrentSettings(null, true, mine);

                                if (appui != DateTime.Now.Second)
                                {
                                    // cant do entire app as mixes with flightdata timer
                                    this.Refresh();
                                    appui = DateTime.Now.Second;
                                }

                                try
                                {
                                    if (MainV2.speechEngine != null)
                                        MainV2.speechEngine.SpeakAsyncCancelAll();
                                }
                                catch
                                {
                                }
                                    // ignore because of this Exception System.PlatformNotSupportedException: No voice installed on the system or none available with the current security setting.

                                if ((mine.MAV.cs.lat + mine.MAV.cs.lng) != oldlatlngsum
                                    && mine.MAV.cs.lat != 0 && mine.MAV.cs.lng != 0)
                                {
                                    if (Math.Round(mine.MAV.cs.lat, 5) == 0 || Math.Round(mine.MAV.cs.lng, 5) == 0)
                                        continue;

                                    // Console.WriteLine(cs.lat + " " + cs.lng + " " + cs.alt + "   lah " + (float)(cs.lat + cs.lng) + "!=" + oldlatlngsum);
                                    CurrentState cs2 = (CurrentState) mine.MAV.cs.Clone();

                                    flightdata.Add(cs2);

                                    oldlatlngsum = (mine.MAV.cs.lat + mine.MAV.cs.lng);
                                }
                            }

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;

                            string basealtstring = "0";

                            if (mine.MAV.wps.ContainsKey(0))
                            {
                                basealtstring = (mine.MAV.wps[0].z + float.Parse(offsetalt)).ToString();
                            }
                            else
                            {
                                InputBox.Show("Relative Alt",
                                    "Please enter your home altitude, or press cancel to use absolute alt",
                                    ref basealtstring);
                            }

                            Application.DoEvents();

                            log.Info(mine.MAV.cs.firmware + " : " + logfile);

                            writeGPX(logfile);
                            writeKML(logfile + ".kml", double.Parse(basealtstring)/CurrentState.multiplierdist);

                            flightdata.Clear();

                            progressBar1.Value = 100;
                        }
                    }
                }
            }
        }

        private void writeGPX(string filename)
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

        private void BUT_humanreadable_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }

                            mine.logreadmode = true;

                            mine.MAV.packets.Initialize(); // clear

                            StreamWriter sw =
                                new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                 Path.GetFileNameWithoutExtension(logfile) + ".txt");

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float) mine.logplaybackfile.BaseStream.Position/
                                         (float) mine.logplaybackfile.BaseStream.Length*100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                byte[] packet = mine.readPacket();
                                string text = "";
                                mine.DebugPacket(packet, ref text);

                                sw.Write(mine.lastlogread + " " + text);
                            }

                            sw.Close();

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;
                        }
                    }
                }
            }
        }

        private void BUT_graphmavlog_Click(object sender, EventArgs e)
        {
            //http://devreminder.wordpress.com/net/net-framework-fundamentals/c-dynamic-math-expression-evaluation/
            //http://www.c-sharpcorner.com/UploadFile/mgold/CodeDomCalculator08082005003253AM/CodeDomCalculator.aspx

//string mathExpression = "(1+1)*3";
            //Console.WriteLine(String.Format("{0}={1}",mathExpression, Evaluate(mathExpression)));


            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                try
                {
                    //  openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"logs" + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist
                openFileDialog1.Filter = "Telemetry Log|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;


                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.Text = "Log - " + Path.GetFileName(openFileDialog1.FileName);

                    List<string> fields = GetLogFileValidFields(openFileDialog1.FileName);

                    zg1.GraphPane.CurveList.Clear();

                    //GetLogFileData(zg1, openFileDialog1.FileName, fields);

                    try
                    {
                        // fix new line types
                        ThemeManager.ApplyThemeTo(this);

                        zg1.Invalidate();
                        zg1.AxisChange();
                    }
                    catch
                    {
                    }
                }
            }
        }

        static int[] ColourValues = new int[]
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

        // Form selectform;

        private List<string> GetLogFileValidFields(string logfile)
        {
            // if (selectform != null && !selectform.IsDisposed)
            //     selectform.Close();

            // selectform = SelectDataToGraphForm();

            Hashtable seenIt = new Hashtable();

            selection = new List<string>();

            options = new List<string>();

            this.datappl.Clear();
            this.packetdata.Clear();

            colorStep = 0;

            using (MAVLinkInterface MavlinkInterface = new MAVLinkInterface())
            {
                try
                {
                    MavlinkInterface.logplaybackfile =
                        new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                catch (Exception ex)
                {
                    log.Debug(ex.ToString());
                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                    return options;
                }
                MavlinkInterface.logreadmode = true;

                MavlinkInterface.MAV.packets.Initialize(); // clear

                CurrentState cs = new CurrentState();

                // to get first packet time
                MavlinkInterface.getHeartBeat();
                MavlinkInterface.setAPType(MavlinkInterface.MAV.sysid, MavlinkInterface.MAV.compid);

                DateTime startlogtime = MavlinkInterface.lastlogread;

                while (MavlinkInterface.logplaybackfile.BaseStream.Position <
                       MavlinkInterface.logplaybackfile.BaseStream.Length)
                {
                    int percent =
                        (int)
                            ((float) MavlinkInterface.logplaybackfile.BaseStream.Position/
                             (float) MavlinkInterface.logplaybackfile.BaseStream.Length*100.0f);
                    if (progressBar1.Value != percent)
                    {
                        progressBar1.Value = percent;
                        progressBar1.Refresh();
                    }

                    byte[] packet = MavlinkInterface.readPacket();

                    cs.datetime = MavlinkInterface.lastlogread;

                    cs.UpdateCurrentSettings(null, true, MavlinkInterface);

                    object data = MavlinkInterface.DebugPacket(packet, false);

                    if (data == null)
                    {
                        log.Info("No info on packet");
                        continue;
                    }

                    if (data is MAVLink.mavlink_heartbeat_t)
                    {
                        if (((MAVLink.mavlink_heartbeat_t) data).type == (byte) MAVLink.MAV_TYPE.GCS)
                            continue;
                    }

                    Type test = data.GetType();


                    if (true)
                    {
                        string packetname = test.Name.Replace("mavlink_", "").Replace("_t", "").ToUpper();

                        if (!packetdata.ContainsKey(packetname))
                        {
                            packetdata[packetname] = new Dictionary<DateTime, object>();
                        }

                        Dictionary<DateTime, object> temp = (Dictionary<DateTime, object>) packetdata[packetname];

                        //double time = (MavlinkInterface.lastlogread - startlogtime).TotalMilliseconds / 1000.0;
                        DateTime time = MavlinkInterface.lastlogread;

                        temp[time] = data;
                    }

                    foreach (var field in test.GetFields())
                    {
                        // field.Name has the field's name.

                        object fieldValue = field.GetValue(data); // Get value

                        if (field.FieldType.IsArray)
                        {
                        }
                        else
                        {
                            if (!seenIt.ContainsKey(field.DeclaringType.Name + "." + field.Name))
                            {
                                seenIt[field.DeclaringType.Name + "." + field.Name] = 1;
                                //AddDataOption(selectform, field.Name + " " + field.DeclaringType.Name);
                                options.Add(field.DeclaringType.Name + "." + field.Name);
                            }

                            if (!this.datappl.ContainsKey(field.Name + " " + field.DeclaringType.Name))
                                this.datappl[field.Name + " " + field.DeclaringType.Name] = new PointPairList();

                            PointPairList list =
                                ((PointPairList) this.datappl[field.Name + " " + field.DeclaringType.Name]);

                            object value = fieldValue;
                            // seconds scale
                            //double time = (MavlinkInterface.lastlogread - startlogtime).TotalMilliseconds / 1000.0;

                            XDate time = new XDate(MavlinkInterface.lastlogread);

                            if (value.GetType() == typeof (Single))
                            {
                                list.Add(time, (Single) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (short))
                            {
                                list.Add(time, (short) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (ushort))
                            {
                                list.Add(time, (ushort) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (byte))
                            {
                                list.Add(time, (byte) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (sbyte))
                            {
                                list.Add(time, (sbyte) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (Int32))
                            {
                                list.Add(time, (Int32) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (UInt32))
                            {
                                list.Add(time, (UInt32) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (ulong))
                            {
                                list.Add(time, (ulong) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (long))
                            {
                                list.Add(time, (long) field.GetValue(data));
                            }
                            else if (value.GetType() == typeof (double))
                            {
                                list.Add(time, (double) field.GetValue(data));
                            }

                            else
                            {
                                Console.WriteLine("Unknown data type");
                            }
                        }
                    }
                }

                MavlinkInterface.logreadmode = false;
                MavlinkInterface.logplaybackfile.Close();
                MavlinkInterface.logplaybackfile = null;

                try
                {
                    dospecial("GPS_RAW_INT");
                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                }
                try
                {
                    addMagField();
                    addDistHome();
                    addIMUTime();
                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                }

                // custom sort based on packet name
                options.Sort(delegate(string c1, string c2) { return String.Compare(c1, c2); });
                //String.Compare(c1.Substring(0,c1.IndexOf('.')),c2.Substring(0,c2.IndexOf('.')))

                // this needs sorting
                /*    string lastitem = "";
                foreach (string item in options)
                {
                    var items = item.Split('.');
                    if (items[0] != lastitem)
                        AddHeader(selectform, items[0].Replace("mavlink_","").Replace("_t","").ToUpper());
                    AddDataOption(selectform, items[1] + " " + items[0]);
                    lastitem = items[0];
                }
                */
                // add new treeview
                ResetTreeView(options);

                //  selectform.Show();

                progressBar1.Value = 100;
            }

            return selection;
        }

        public static T Cast<T>(object o)
        {
            return (T) o;
        }

        void dospecial(string PacketName)
        {
            Dictionary<DateTime, object> temp = null;

            try
            {
                temp = (Dictionary<DateTime, object>) packetdata[PacketName];
            }
            catch
            {
                CustomMessageBox.Show("Bad PacketName");
                return;
            }

            string code = @"

        public double stage(object inp) {
            return getAltAboveHome((MAVLink.mavlink_gps_raw_int_t) inp);
        }

        public double getAltAboveHome(MAVLink.mavlink_gps_raw_int_t gps)
        {
            if (customforusenumber == -1 && gps.fix_type != 2)
                customforusenumber = gps.alt  / 1000.0f;

            return (gps.alt / 1000.0f) - customforusenumber;
        }
";

            // build the class using codedom
            CodeGen.BuildClass(code);

            // compile the class into an in-memory assembly.
            // if it doesn't compile, show errors in the window
            CompilerResults results = CodeGen.CompileAssembly();

            if (results != null && results.CompiledAssembly != null)
            {
                string field = "custom mavlink_custom_t"; // reverse bellow

                options.Add("mavlink_custom_t.custom");

                this.datappl[field] = new PointPairList();


                MethodInfo mi = RunCode(results);


                // from here
                PointPairList result = (PointPairList) this.datappl[field];

                try
                {
                    if (temp == null)
                        return;

                    object assemblyInstance = results.CompiledAssembly.CreateInstance("ExpressionEvaluator.Calculator");

                    foreach (DateTime time in temp.Keys)
                    {
                        XDate time2 = new XDate(time);
                        result.Add(time2, (double) mi.Invoke(assemblyInstance, new object[] {temp[time]}));
                    }
                }
                catch
                {
                }
            }
            else
            {
                CustomMessageBox.Show("Compile Failed");
                return;
            }

            object answer = CodeGen.runCode(code);

            Console.WriteLine(answer);
        }

        public MethodInfo RunCode(CompilerResults results)
        {
            Assembly executingAssembly = results.CompiledAssembly;
            try
            {
                //cant call the entry method if the assembly is null
                if (executingAssembly != null)
                {
                    object assemblyInstance = executingAssembly.CreateInstance("ExpressionEvaluator.Calculator");
                    //Use reflection to call the static Main function

                    Module[] modules = executingAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the entry point method
                    foreach (Type type in types)
                    {
                        MethodInfo[] mis = type.GetMethods();
                        foreach (MethodInfo mi in mis)
                        {
                            if (mi.Name == "stage")
                            {
                                return mi;
                                //object result = mi.Invoke(assemblyInstance, null);
                                //return result.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: An exception occurred while executing the script\n{0}", ex);
            }
            return null;
        }

        PointPairList GetValuesForField(string name)
        {
            // eg RAW_IMU.xmag to "xmag mavlink_raw_imu_t"

            string[] items = name.ToLower().Split(new char[] {'.', ' '});

            PointPairList list = ((PointPairList) this.datappl[items[1] + " mavlink_" + items[0] + "_t"]);

            return list;
        }

        void addMagField()
        {
            string field = "mag_field mavlink_custom_t";

            options.Add("mavlink_custom_t.mag_field");

            this.datappl[field] = new PointPairList();

            PointPairList list = ((PointPairList) this.datappl[field]);

            PointPairList listx = ((PointPairList) this.datappl["xmag mavlink_raw_imu_t"]);
            PointPairList listy = ((PointPairList) this.datappl["ymag mavlink_raw_imu_t"]);
            PointPairList listz = ((PointPairList) this.datappl["zmag mavlink_raw_imu_t"]);

            //(float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2));

            for (int a = 0; a < listx.Count; a++)
            {
                double ans = Math.Sqrt(Math.Pow(listx[a].Y, 2) + Math.Pow(listy[a].Y, 2) + Math.Pow(listz[a].Y, 2));

                //Console.WriteLine("{0} {1} {2} {3}", ans, listx[a].Y, listy[a].Y, listz[a].Y);

                list.Add(listx[a].X, ans);
            }
        }

        void addIMUTime()
        {
            string field = "sitltime mavlink_custom_t";

            options.Add("mavlink_custom_t.sitltime");

            this.datappl[field] = new PointPairList();

            PointPairList list = ((PointPairList) this.datappl[field]);

            PointPairList listtime = ((PointPairList) this.datappl["time_usec mavlink_raw_imu_t"]);

            double lastrealtime = listtime[0].X;
            double lastvalue = listtime[0].Y*1.0e-6;

            for (int a = 0; a < listtime.Count; a++)
            {
                double delta = ((listtime[a].Y*1.0e-6) - lastvalue);

                // convert to seconds
                list.Add(listtime[a].X, delta);

                lastvalue = listtime[a].Y*1.0e-6;
                lastrealtime = listtime[a].X;
            }
        }

        void addDistHome()
        {
            string field = "dist_home mavlink_custom_t";

            options.Add("mavlink_custom_t.dist_home");

            this.datappl[field] = new PointPairList();

            PointLatLngAlt home = new PointLatLngAlt();

            PointPairList list = ((PointPairList) this.datappl[field]);

            PointPairList listfix = ((PointPairList) this.datappl["fix_type mavlink_gps_raw_int_t"]);
            PointPairList listx = ((PointPairList) this.datappl["lat mavlink_gps_raw_int_t"]);
            PointPairList listy = ((PointPairList) this.datappl["lon mavlink_gps_raw_int_t"]);
            PointPairList listz = ((PointPairList) this.datappl["alt mavlink_gps_raw_int_t"]);

            for (int a = 0; a < listfix.Count; a++)
            {
                if (listfix[a].Y == 3)
                {
                    home = new PointLatLngAlt(listx[a].Y/10000000.0, listy[a].Y/10000000.0, listz[a].Y/1000.0, "Home");
                    break;
                }
            }

            //(float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2));

            for (int a = 0; a < listx.Count; a++)
            {
                double ans =
                    home.GetDistance(new PointLatLngAlt(listx[a].Y/10000000.0, listy[a].Y/10000000.0, listz[a].Y/1000.0,
                        "Point"));

                //Console.WriteLine("{0} {1} {2} {3}", ans, listx[a].Y, listy[a].Y, listz[a].Y);

                list.Add(listx[a].X, ans);
            }
        }

        private void ResetTreeView(List<string> seenmessagetypes)
        {
            treeView1.Nodes.Clear();

            Hashtable addedrootnodes = new Hashtable();
            TreeNode tn = treeView1.TopNode;

            foreach (var item in seenmessagetypes)
            {
                var items = item.Split('.');

                var item1text = items[0].Replace("mavlink_", "").Replace("_t", "").ToUpper();
                var item2text = items[1];

                if (!addedrootnodes.ContainsKey(item1text))
                {
                    tn = new TreeNode(item1text);
                    treeView1.Nodes.Add(tn);
                    addedrootnodes[item1text] = 1;
                }

                tn.Nodes.Add(item2text);
            }
        }

        private void AddHeader(Form selectform, string Name)
        {
            System.Windows.Forms.Label lbl_head = new System.Windows.Forms.Label();

            log.Info("Add Header " + Name);

            lbl_head.Text = Name;
            lbl_head.Name = Name;
            lbl_head.Location = new System.Drawing.Point(x, y);
            lbl_head.Size = new System.Drawing.Size(100, 20);

            selectform.Controls.Add(lbl_head);

            Application.DoEvents();

            x += 0;
            y += 20;

            if (y > selectform.Height - 60)
            {
                x += 100;
                y = 10;

                selectform.Width = x + 100;
            }
        }

        private void AddDataOption(Form selectform, string Name)
        {
            CheckBox chk_box = new CheckBox();

            log.Info("Add Option " + Name);

            chk_box.Text = Name;
            chk_box.Name = Name;
            chk_box.Location = new System.Drawing.Point(x, y);
            chk_box.Size = new System.Drawing.Size(100, 20);
            chk_box.CheckedChanged += new EventHandler(chk_box_CheckedChanged);
            chk_box.MouseUp += new MouseEventHandler(chk_box_MouseUp);

            selectform.Controls.Add(chk_box);

            Application.DoEvents();

            x += 0;
            y += 20;

            if (y > selectform.Height - 60)
            {
                x += 100;
                y = 10;

                selectform.Width = x + 100;
            }
        }

        void chk_box_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // dont action a already draw item
                if (!((CheckBox) sender).Checked)
                {
                    rightclick = true;
                    ((CheckBox) sender).Checked = true;
                }
                else
                {
                    ((CheckBox) sender).Checked = false;
                }
                rightclick = false;
            }
        }

        int colorStep = 0;
        bool rightclick = false;

        void chk_box_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox) sender).Checked)
            {
                selection.Add(((CheckBox) sender).Name);

                LineItem myCurve;

                int colorvalue = ColourValues[colorStep%ColourValues.Length];
                colorStep++;
                Console.WriteLine("Color " + colorvalue);

                myCurve = zg1.GraphPane.AddCurve(((CheckBox) sender).Name.Replace("mavlink_", ""),
                    (PointPairList) datappl[((CheckBox) sender).Name],
                    Color.FromArgb(unchecked(colorvalue + (int) 0xff000000)), SymbolType.None);

                myCurve.Tag = ((CheckBox) sender).Name;

                if (myCurve.Tag.ToString() == "roll mavlink_attitude_t" ||
                    myCurve.Tag.ToString() == "pitch mavlink_attitude_t" ||
                    myCurve.Tag.ToString() == "yaw mavlink_attitude_t")
                {
                    PointPairList ppl = new PointPairList((PointPairList) datappl[((CheckBox) sender).Name]);
                    for (int a = 0; a < ppl.Count; a++)
                    {
                        ppl[a].Y = ppl[a].Y*(180.0/Math.PI);
                    }

                    myCurve.Points = ppl;
                }

                double xMin, xMax, yMin, yMax;

                myCurve.GetRange(out xMin, out xMax, out yMin, out yMax, true, false, zg1.GraphPane);

                if (rightclick || (yMin > 850 && yMax < 2100 && yMin < 2100))
                {
                    myCurve.IsY2Axis = true;
                    myCurve.YAxisIndex = 0;
                    zg1.GraphPane.Y2Axis.IsVisible = true;

                    myCurve.Label.Text = myCurve.Label.Text + "-R";
                }
            }
            else
            {
                selection.Remove(((CheckBox) sender).Name);
                foreach (var item in zg1.GraphPane.CurveList)
                {
                    if (item.Tag == null)
                        continue;
                    if (item.Tag.ToString() == ((CheckBox) sender).Name)
                    {
                        zg1.GraphPane.CurveList.Remove(item);
                        break;
                    }
                }
            }

            try
            {
                // fix new line types
                ThemeManager.ApplyThemeTo(this);

                zg1.GraphPane.XAxis.AxisGap = 0;

                zg1.Invalidate();
                zg1.AxisChange();
            }
            catch
            {
            }
        }

        int x = 10;
        int y = 10;

        private Form SelectDataToGraphForm()
        {
            Form selectform = new Form()
            {
                Name = "select",
                Width = 50,
                Height = 500,
                Text = "Graph This",
                TopLevel = true
            };

            x = 10;
            y = 10;

            AddHeader(selectform, "Left Click");
            AddHeader(selectform, "Left Axis");
            AddHeader(selectform, "Right Click");
            AddHeader(selectform, "Right Axis");

            ThemeManager.ApplyThemeTo(selectform);

            return selectform;
        }

        private void BUT_convertcsv_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }
                            mine.logreadmode = true;

                            mine.MAV.packets.Initialize(); // clear

                            StreamWriter sw =
                                new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                 Path.GetFileNameWithoutExtension(logfile) + ".csv");

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float) mine.logplaybackfile.BaseStream.Position/
                                         (float) mine.logplaybackfile.BaseStream.Length*100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                byte[] packet = mine.readPacket();
                                string text = "";
                                mine.DebugPacket(packet, ref text, true, ",");

                                sw.Write(mine.lastlogread.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "," + text);
                            }

                            sw.Close();

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;
                        }
                    }
                }
            }
        }

        private void BUT_paramsfromlog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        try
                        {
                            using (MAVLinkInterface mine = new MAVLinkInterface())
                            {
                                try
                                {
                                    mine.logplaybackfile =
                                        new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read,
                                            FileShare.Read));
                                }
                                catch (Exception ex)
                                {
                                    log.Debug(ex.ToString());
                                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                    return;
                                }

                                mine.logreadmode = true;

                                mine.MAV.packets.Initialize(); // clear

                                StreamWriter sw =
                                    new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                     Path.GetFileNameWithoutExtension(logfile) + ".param");

                                int percent =
                                    (int)
                                        ((float) mine.logplaybackfile.BaseStream.Position/
                                         (float) mine.logplaybackfile.BaseStream.Length*100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                mine.getParamList();

                                foreach (string item in mine.MAV.param.Keys)
                                {
                                    sw.WriteLine(item + "\t" + mine.MAV.param[item]);
                                }

                                sw.Close();

                                progressBar1.Value = 100;

                                mine.logreadmode = false;
                                mine.logplaybackfile.Close();
                                mine.logplaybackfile = null;
                            }
                            CustomMessageBox.Show("File Saved with log file");
                        }
                        catch
                        {
                            CustomMessageBox.Show("Error Extracting params");
                        }
                    }
                }
            }
        }

        private void BUT_getwpsfromlog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        int wplists = 0;

                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }

                            mine.logreadmode = true;

                            mine.MAV.packets.Initialize(); // clear

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float) mine.logplaybackfile.BaseStream.Position/
                                         (float) mine.logplaybackfile.BaseStream.Length*100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                ushort count = 0;
                                try
                                {
                                    count = mine.getWPCount();
                                }
                                catch
                                {
                                }

                                if (count == 0)
                                {
                                    continue;
                                }


                                StreamWriter sw =
                                    new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                     Path.GetFileNameWithoutExtension(logfile) + "-" + wplists +
                                                     ".waypoints");

                                sw.WriteLine("QGC WPL 120");
                                try
                                {
                                    //get mission count info 
                                    var item =
                                        mine.MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.MISSION_COUNT]
                                            .ByteArrayToStructure<MAVLink.mavlink_mission_count_t>();
                                    mine.MAV.packets[(byte) MAVLink.MAVLINK_MSG_ID.MISSION_COUNT] = null;
                                    sw.WriteLine("# count packet sent to comp " + item.target_component + " sys " +
                                                 item.target_system);
                                }
                                catch
                                {
                                }
                                for (ushort a = 0; a < count; a++)
                                {
                                    Locationwp wp = mine.getWP(a);
                                    //sw.WriteLine(item + "\t" + mine.param[item]);
                                    byte mode = (byte) wp.id;

                                    sw.Write((a + 1)); // seq
                                    sw.Write("\t" + 0); // current
                                    sw.Write("\t" + (byte) MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT); //frame 
                                    sw.Write("\t" + mode);
                                    sw.Write("\t" +
                                             wp.p1.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             wp.p2.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             wp.p3.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             wp.p4.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             wp.lat.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             wp.lng.ToString("0.000000", new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" +
                                             (wp.alt/CurrentState.multiplierdist).ToString("0.000000",
                                                 new System.Globalization.CultureInfo("en-US")));
                                    sw.Write("\t" + 1);
                                    sw.WriteLine("");
                                }

                                sw.Close();
                                wplists++;
                            }

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;

                            if (wplists == 0)
                            {
                                CustomMessageBox.Show("No Waypoint found in file!");
                            }
                            else
                            {
                                CustomMessageBox.Show("File Saved with log file!");
                            }
                        }
                    }
                }
            }
        }

        private void BUT_droneshare_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Logs|*.tlog;*.log;*.bin";
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        try
                        {
                            Utilities.DroneApi.droneshare.doUpload(logfile);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show(ex.Message);
                        }
                    }
                }
            }
        }

        private void BUT_matlab_Click(object sender, EventArgs e)
        {
            MissionPlanner.Log.MatLab.ProcessTLog();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Parent != null)
            {
                // set the check if we right click
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    e.Node.Checked = !e.Node.Checked;
                }

                if (e.Node.Checked)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        GraphItem(e.Node.Parent.Text, e.Node.Text, false);
                    }
                    else
                    {
                        GraphItem(e.Node.Parent.Text, e.Node.Text, true);
                    }
                }
                else
                {
                    List<CurveItem> removeitems = new List<CurveItem>();

                    foreach (var item in zg1.GraphPane.CurveList)
                    {
                        if (item.Label.Text.StartsWith(e.Node.Text) &&
                            item.Label.Text.Contains(e.Node.Parent.Text.ToLower()))
                        {
                            removeitems.Add(item);
                            //break;
                        }
                    }

                    foreach (var item in removeitems)
                        zg1.GraphPane.CurveList.Remove(item);
                }

                zg1.Invalidate();
            }
            else if (e.Node != null && e.Node.Parent == null) // root nood ticked
            {
                if (e.Node.Checked)
                {
                    e.Node.Checked = false;
                    /* foreach (var child in e.Node.Nodes)
                    {
                        ((TreeNode)child).Checked = true;
                        var newe = new TreeNodeMouseClickEventArgs((TreeNode)child, e.Button, e.Clicks, e.X, e.Y);
                        treeView1_NodeMouseClick(child, newe);
                    }
                    */
                }
                else
                {
                }
            }
        }

        private void GraphItem(string parenttext, string text, bool leftaxis)
        {
            rightclick = !leftaxis;

            chk_box_CheckedChanged(
                new CheckBox() {Name = text + " mavlink_" + parenttext.ToLower() + "_t", Checked = true}, null);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
        }
    }
}