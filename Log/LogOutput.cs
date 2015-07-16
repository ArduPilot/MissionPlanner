﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Core.Geometry;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using KMLib;
using KMLib.Feature;
using KMLib.Geometry;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    public class LogOutput
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string lastline = "";
        string[] ctunlast = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        string[] ntunlast = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        // wp list
        List<PointLatLngAlt> cmd = new List<PointLatLngAlt>();

        Point3D oldlastpos = new Point3D();
        Point3D lastpos = new Point3D();
        List<Data> flightdata = new List<Data>();
        List<string> gpsrawdata = new List<string>();
        Model runmodel = new Model();
        List<string> modelist = new List<string>();

        List<Core.Geometry.Point3D>[] position = new List<Core.Geometry.Point3D>[200];
        int positionindex = 0;

        private DateTime doevent = DateTime.Now;

        public struct Data
        {
            public Model model;
            public string[] ntun;
            public string[] ctun;
            public DateTime datetime;
            public string mode;
        }

        public void processLine(string line)
        {
            try
            {
                if (doevent.Second != DateTime.Now.Second)
                {
                    Application.DoEvents();
                    doevent = DateTime.Now;
                }

                if (line.Length == 0)
                    return;

                DateTime start = DateTime.Now;

                if (line.ToLower().Contains("ArduCopter"))
                {
                    MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;
                }
                if (line.ToLower().Contains("ArduPlane"))
                {
                    MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduPlane;
                }
                if (line.ToLower().Contains("ArduRover"))
                {
                    MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduRover;
                }
                

                line = line.Replace(", ", ",");
                line = line.Replace(": ", ":");

                string[] items = line.Split(',', ':');

                if (items[0].Contains("FMT"))
                {
                    try
                    {
                        DFLog.FMTLine(line);
                    }
                    catch { }
                }
                else if (items[0].Contains("CMD"))
                {
                    if (flightdata.Count == 0)
                    {
                        if (int.Parse(items[3]) <= (int)MAVLink.MAV_CMD.LAST) // wps
                        {
                            PointLatLngAlt temp = new PointLatLngAlt(double.Parse(items[7], new System.Globalization.CultureInfo("en-US")), double.Parse(items[8], new System.Globalization.CultureInfo("en-US")), double.Parse(items[6], new System.Globalization.CultureInfo("en-US")), items[2].ToString());
                            cmd.Add(temp);
                        }
                    }
                }
                else if (items[0].Contains("MOD"))
                {
                    positionindex++;

                    while (modelist.Count < positionindex + 1)
                        modelist.Add("");

                    if (items.Length == 4)
                    {
                        modelist[positionindex] = (items[2]);
                    }
                    else
                    {
                        modelist[positionindex] = (items[1]);
                    }
                }
                else if (items[0].Contains("GPS") && DFLog.logformat.ContainsKey("GPS"))
                {
                    if (items[0].Contains("GPS2"))
                        return;

                    if (items[DFLog.FindMessageOffset("GPS", "Status")] != "3")
                        return;

                    if (position[positionindex] == null)
                        position[positionindex] = new List<Point3D>();

                    //  if (double.Parse(items[4], new System.Globalization.CultureInfo("en-US")) == 0)
                    //     return;

                    // 7 agl
                    // 8 asl...
                    double alt = double.Parse(items[DFLog.FindMessageOffset("GPS", "Alt")], new System.Globalization.CultureInfo("en-US"));

                    position[positionindex].Add(new Point3D(double.Parse(items[DFLog.FindMessageOffset("GPS", "Lng")],
                        new System.Globalization.CultureInfo("en-US")),
                        double.Parse(items[DFLog.FindMessageOffset("GPS", "Lat")],
                        new System.Globalization.CultureInfo("en-US")), alt));
                    oldlastpos = lastpos;
                    lastpos = (position[positionindex][position[positionindex].Count - 1]);
                    lastline = line;
                }
                else if (items[0].Contains("GPS") && items[2] == "1" && items[4] != "0" && items[4] != "-1" && lastline != line) // check gps line and fixed status
                {
                    MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduPlane;

                    if (position[positionindex] == null)
                        position[positionindex] = new List<Point3D>();

                    if (double.Parse(items[4], new System.Globalization.CultureInfo("en-US")) == 0)
                        return;

                    double alt = double.Parse(items[6], new System.Globalization.CultureInfo("en-US"));

                    if (items.Length == 11 && items[6] == "0.0000")
                        alt = double.Parse(items[7], new System.Globalization.CultureInfo("en-US"));
                    if (items.Length == 11 && items[6] == "0")
                        alt = double.Parse(items[7], new System.Globalization.CultureInfo("en-US"));


                    position[positionindex].Add(new Point3D(double.Parse(items[5], new System.Globalization.CultureInfo("en-US")), double.Parse(items[4], new System.Globalization.CultureInfo("en-US")), alt));
                    oldlastpos = lastpos;
                    lastpos = (position[positionindex][position[positionindex].Count - 1]);
                    lastline = line;
                }
                else if (items[0].Contains("GPS") && items[4] != "0" && items[4] != "-1" && items.Length <= 9) // AC
                {
                    MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;

                    if (position[positionindex] == null)
                        position[positionindex] = new List<Point3D>();

                    if (double.Parse(items[4], new System.Globalization.CultureInfo("en-US")) == 0)
                        return;

                    double alt = double.Parse(items[5], new System.Globalization.CultureInfo("en-US"));

                    position[positionindex].Add(new Point3D(double.Parse(items[4], new System.Globalization.CultureInfo("en-US")), double.Parse(items[3], new System.Globalization.CultureInfo("en-US")), alt));
                    oldlastpos = lastpos;
                    lastpos = (position[positionindex][position[positionindex].Count - 1]);
                    lastline = line;

                }
                else if ((items[0].Contains("GPS") && items[1] == "3" && items[6] != "0" && items[6] != "-1" && lastline != line && items.Length == 12) ||
                        (items[0].Contains("GPS") && items[1] == "3" && items[6] != "0" && items[6] != "-1" && lastline != line && items.Length == 14))
                {
                    if (position[positionindex] == null)
                        position[positionindex] = new List<Point3D>();

                    //  if (double.Parse(items[4], new System.Globalization.CultureInfo("en-US")) == 0)
                    //     return;

                    // 8 agl
                    // 9 asl...
                    double alt = double.Parse(items[9], new System.Globalization.CultureInfo("en-US"));

                    position[positionindex].Add(new Point3D(double.Parse(items[7], new System.Globalization.CultureInfo("en-US")), double.Parse(items[6], new System.Globalization.CultureInfo("en-US")), alt));
                    oldlastpos = lastpos;
                    lastpos = (position[positionindex][position[positionindex].Count - 1]);
                    lastline = line;
                }
                else if (items[0].Contains("GPS") && items[1] == "3" && items[4] != "0" && items[4] != "-1" && lastline != line && items.Length == 11) // check gps line and fixed status
                {
                    if (position[positionindex] == null)
                        position[positionindex] = new List<Point3D>();

                    //  if (double.Parse(items[4], new System.Globalization.CultureInfo("en-US")) == 0)
                    //     return;

                    // 7 agl
                    // 8 asl...
                    double alt = double.Parse(items[8], new System.Globalization.CultureInfo("en-US"));

                    position[positionindex].Add(new Point3D(double.Parse(items[6], new System.Globalization.CultureInfo("en-US")), double.Parse(items[5], new System.Globalization.CultureInfo("en-US")), alt));
                    oldlastpos = lastpos;
                    lastpos = (position[positionindex][position[positionindex].Count - 1]);
                    lastline = line;
                }
                else if (items[0].Contains("GRAW"))
                {
                    gpsrawdata.Add(line);
                }
                else if (items[0].Contains("GRXH"))
                {
                    gpsrawdata.Add(line);
                }
                else if (items[0].Contains("GRXS"))
                {
                    gpsrawdata.Add(line);
                }
                else if (items[0].Contains("CTUN"))
                {
                    ctunlast = items;
                }
                else if (items[0].Contains("NTUN"))
                {
                    ntunlast = items;
                    try
                    {
                        // line = "ATT:" + double.Parse(ctunlast[3], new System.Globalization.CultureInfo("en-US")) * 100 + "," + double.Parse(ctunlast[6], new System.Globalization.CultureInfo("en-US")) * 100 + "," + double.Parse(items[1], new System.Globalization.CultureInfo("en-US")) * 100;
                        // items = line.Split(',', ':');
                    }
                    catch { }
                }
                else if (items[0].Contains("ATT"))
                {
                    try
                    {
                        if (lastpos.X != 0 && oldlastpos.X != lastpos.X && oldlastpos.Y != lastpos.Y)
                        {
                            Data dat = new Data();

                            try
                            {
                                dat.datetime = DFLog.GetTimeGPS(lastline);
                            }
                            catch { }

                            runmodel = new Model();

                            runmodel.Location.longitude = lastpos.X;
                            runmodel.Location.latitude = lastpos.Y;
                            runmodel.Location.altitude = lastpos.Z;

                            oldlastpos = lastpos;

                            runmodel.Orientation.roll = double.Parse(items[DFLog.FindMessageOffset("ATT", "Roll")], new System.Globalization.CultureInfo("en-US")) / -1;
                            runmodel.Orientation.tilt = double.Parse(items[DFLog.FindMessageOffset("ATT", "Pitch")], new System.Globalization.CultureInfo("en-US")) / -1;
                            runmodel.Orientation.heading = double.Parse(items[DFLog.FindMessageOffset("ATT", "Yaw")], new System.Globalization.CultureInfo("en-US")) / 1;

                            dat.model = runmodel;
                            dat.ctun = ctunlast;
                            dat.ntun = ntunlast;

                            flightdata.Add(dat);
                        }
                    }
                    catch (Exception ex) { log.Error(ex); }
                }

                if ((DateTime.Now - start).TotalMilliseconds > 5)
                {
                    Console.WriteLine(line);
                }
            }
            catch (Exception)
            {
                // if items is to short or parse fails.. ignore
            }
        }

        public void writeKMLFirstPerson(string filename)
        {
            StreamWriter stream = new StreamWriter(File.Open(filename, FileMode.Create));
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            string header = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><kml xmlns=\"http://www.opengis.net/kml/2.2\" xmlns:gx=\"http://www.google.com/kml/ext/2.2\" xmlns:kml=\"http://www.opengis.net/kml/2.2\" xmlns:atom=\"http://www.w3.org/2005/Atom\">\n     <Document>   <name>Paths</name>    <description>Path</description>\n    <Style id=\"yellowLineGreenPoly\">      <LineStyle>        <color>7f00ffff</color>        <width>4</width>      </LineStyle>      <PolyStyle>        <color>7f00ff00</color>      </PolyStyle>    </Style>\n  ";
            stream.Write(header);

            StringBuilder kml = new StringBuilder();
            StringBuilder data = new StringBuilder();

            double lastlat = 0;
            double lastlong = 0;
            int gpspackets = 0;
            int lastgpspacket = 0;

            foreach (Data mod in flightdata)
            {
                if (mod.model.Location.latitude == 0)
                    continue;

                gpspackets++;
                if (lastlat == mod.model.Location.latitude && lastlong == mod.model.Location.longitude)
                    continue;
                // double speed 0.05 - assumeing 10hz in log file
                // 1 speed = 0.1    10 / 1  = 0.1
                data.Append(@"
        <gx:FlyTo>
            <gx:duration>" + ((gpspackets - lastgpspacket) * 0.1) + @"</gx:duration>
            <gx:flyToMode>smooth</gx:flyToMode>
            <Camera>
                <longitude>" + mod.model.Location.longitude.ToString(new System.Globalization.CultureInfo("en-US")) + @"</longitude>
                <latitude>" + mod.model.Location.latitude.ToString(new System.Globalization.CultureInfo("en-US")) + @"</latitude>
                <altitude>" + mod.model.Location.altitude.ToString(new System.Globalization.CultureInfo("en-US")) + @"</altitude>
                <roll>" + mod.model.Orientation.roll.ToString(new System.Globalization.CultureInfo("en-US")) + @"</roll>
                <tilt>" + (90 - mod.model.Orientation.tilt).ToString(new System.Globalization.CultureInfo("en-US")) + @"</tilt>
                <heading>" + mod.model.Orientation.heading.ToString(new System.Globalization.CultureInfo("en-US")) + @"</heading>              
                <altitudeMode>absolute</altitudeMode>
            </Camera>
        </gx:FlyTo>
");
                lastlat = mod.model.Location.latitude;
                lastlong = mod.model.Location.longitude;
                lastgpspacket = gpspackets;
            }

            kml.Append(@"
        <Folder>
            <name>Flight</name> 
            <gx:Tour>
                <name>Flight Do</name> 
                <gx:Playlist>
                    " + data +
                @"</gx:Playlist> 
            </gx:Tour>
        </Folder>
    </Document>
</kml>
");

            stream.Write(kml.ToString());
            stream.Close();

            // create kmz - aka zip file

            FileStream fs = File.Open(filename.Replace(".log-fp.kml", "-fp.kmz"), FileMode.Create);
            ZipOutputStream zipStream = new ZipOutputStream(fs);
            zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
            zipStream.UseZip64 = UseZip64.Off; // older zipfile

            // entry 1
            string entryName = ZipEntry.CleanName(Path.GetFileName(filename)); // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            File.Delete(filename);

            filename = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "block_plane_0.dae";

            // entry 2
            entryName = ZipEntry.CleanName(Path.GetFileName(filename)); // Removes drive from name and fixes slash direction
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


            zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            zipStream.Close();

            positionindex = 0;
            modelist.Clear();
            flightdata.Clear();
            position = new List<Core.Geometry.Point3D>[200];
            cmd.Clear();
        }


        public void writeGPX(string filename)
        {
            System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".gpx", Encoding.ASCII);

            xw.WriteStartElement("gpx");
            xw.WriteAttributeString("creator", MainV2.instance.Text);
            xw.WriteAttributeString("xmlns", "http://www.topografix.com/GPX/1/1");

            xw.WriteStartElement("trk");

            xw.WriteStartElement("trkseg");

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            foreach (Data mod in flightdata)
            {
                xw.WriteStartElement("trkpt");
                xw.WriteAttributeString("lat", mod.model.Location.latitude.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteAttributeString("lon", mod.model.Location.longitude.ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("ele", mod.model.Location.altitude.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("time", mod.datetime.ToString("yyyy-MM-ddTHH:mm:sszzzzzz"));
                xw.WriteElementString("course", (mod.model.Orientation.heading).ToString(new System.Globalization.CultureInfo("en-US")));

                xw.WriteElementString("roll", mod.model.Orientation.roll.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("pitch", mod.model.Orientation.tilt.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("mode", mod.mode);

                //xw.WriteElementString("speed", mod.model.Orientation.);
                //xw.WriteElementString("fix", mod.model.Location.altitude);

                xw.WriteEndElement();
            }


            xw.WriteEndElement(); // trkseg
            xw.WriteEndElement(); // trk

            int a = 0;
            foreach (Data mod in flightdata)
            {
                xw.WriteStartElement("wpt");
                xw.WriteAttributeString("lat", mod.model.Location.latitude.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteAttributeString("lon", mod.model.Location.longitude.ToString(new System.Globalization.CultureInfo("en-US")));
                xw.WriteElementString("name", (a++).ToString());
                xw.WriteEndElement();//wpt
            }

            xw.WriteEndElement(); // gpx

            xw.Close();
        }

        public static DateTime GetFromGps(int weeknumber, double seconds)
        {
            DateTime datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            DateTime week = datum.AddDays(weeknumber * 7);
            DateTime time = week.AddSeconds(seconds);
            return time;
        }

        public void writeRinex(string filename)
        {
            if (gpsrawdata.Count == 0)
                return;

            string file = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".obs";

            var rinexoutput = new StreamWriter(file);

            // 60 chars

            string header = @"     3.02           OBSERVATION DATA    M: Mixed            RINEX VERSION / TYPE
                                                            MARKER NAME         
                                                            MARKER NUMBER       
                                                            MARKER TYPE         
                                                            OBSERVER / AGENCY   
                                                            REC # / TYPE / VERS 
                                                            ANT # / TYPE        
        0.0000        0.0000        0.0000                  APPROX POSITION XYZ 
        0.0000        0.0000        0.0000                  ANTENNA: DELTA H/E/N
G    4 C1C L1C D1C S1C                                      SYS / # / OBS TYPES 
S    4 C1C L1C D1C S1C                                      SYS / # / OBS TYPES 
R    4 C1C L1C D1C S1C                                      SYS / # / OBS TYPES 
E    4 C1C L1C D1C S1C                                      SYS / # / OBS TYPES 
G                                                           SYS / PHASE SHIFT   
                                                            END OF HEADER       ";

            rinexoutput.WriteLine(header);

            /*
            { LOG_GPS_RAW_MSG, sizeof(log_GPS_RAW), \  
 690         "GRAW", "QIHBBddfBbB", "TimeUS,WkMS,Week,numSV,sv,cpMes,prMes,doMes,mesQI,cno,lli" }, \  
       +    { LOG_GPS_RAWH_MSG, sizeof(log_GPS_RAWH), \  
 692  +      "GRXH", "QdHbBB", "TimeUS,rcvTime,week,leapS,numMeas,recStat" }, \  
 693  +    { LOG_GPS_RAWS_MSG, sizeof(log_GPS_RAWS), \  
 694  +      "GRXS", "QddfBBBHBBBBB", "TimeUS,prMes,cpMes,doMes,gnss,sv,freq,lock,cno,prD,cpD,doD,trk" }, \  

            */
            /*
GNSS Identifiers
gnssId GNSS Type
0 GPS
1 SBAS
2 Galileo
3 BeiDou
4 IMES
5 QZSS
6 GLONASS
            */
            //ftp://igs.org/pub/data/format/rinex302.pdf
            //https://www.u-blox.com/images/downloads/Product_Docs/u-bloxM8_ReceiverDescriptionProtocolSpec_(UBX-13003221)_Public.pdf

            DateTime lastgpstime = DateTime.MinValue;

            double weekms = 0, NSats = 0;
            int week = 0;

            foreach (string line in gpsrawdata)
            {
                string sattype = "G";

                string[] items = line.Split(',', ':');

                double sv = -1, cpMes=-1, prMes=-1, doMes=-1, mesQI=-1, cno=-1, lli=-1;
                int gnss = 0;

                if (items[0].StartsWith("GRAW"))
                {
                    weekms = double.Parse(items[DFLog.FindMessageOffset("GRAW", "WkMS")]);
                    week = int.Parse(items[DFLog.FindMessageOffset("GRAW", "Week")]);
                    NSats = double.Parse(items[DFLog.FindMessageOffset("GRAW", "numSV")]);
                    sv = double.Parse(items[DFLog.FindMessageOffset("GRAW", "sv")]);
                    cpMes = double.Parse(items[DFLog.FindMessageOffset("GRAW", "cpMes")]);
                    prMes = double.Parse(items[DFLog.FindMessageOffset("GRAW", "prMes")]);
                    doMes = double.Parse(items[DFLog.FindMessageOffset("GRAW", "doMes")]);
                    mesQI = double.Parse(items[DFLog.FindMessageOffset("GRAW", "mesQI")]);
                    cno = double.Parse(items[DFLog.FindMessageOffset("GRAW", "cno")]);
                    lli = double.Parse(items[DFLog.FindMessageOffset("GRAW", "lli")]);

                    if (sv > 32)
                        gnss = 1;
                }
                else if (items[0].StartsWith("GRXH"))
                {
                    weekms = double.Parse(items[DFLog.FindMessageOffset("GRXH", "rcvTime")]) * 1000.0;
                    week = int.Parse(items[DFLog.FindMessageOffset("GRXH", "week")]);
                    NSats = double.Parse(items[DFLog.FindMessageOffset("GRXH", "numMeas")]);
                    continue;
                }
                else if (items[0].StartsWith("GRXS"))
                {
                    sv = double.Parse(items[DFLog.FindMessageOffset("GRXS", "sv")]);
                    cpMes = double.Parse(items[DFLog.FindMessageOffset("GRXS", "cpMes")]);
                    prMes = double.Parse(items[DFLog.FindMessageOffset("GRXS", "prMes")]);
                    doMes = double.Parse(items[DFLog.FindMessageOffset("GRXS", "doMes")]);
                    gnss = int.Parse(items[DFLog.FindMessageOffset("GRXS", "gnss")]);
                    cno = double.Parse(items[DFLog.FindMessageOffset("GRXS", "cno")]);
                    double locktime = double.Parse(items[DFLog.FindMessageOffset("GRXS", "lock")]);
                    lli = 0; // OK or not known
                }

                DateTime gpstime = GetFromGps(week, weekms/1000.0);

                if (week == 0)
                    continue;

                if (lastgpstime != gpstime)
                {
                    rinexoutput.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2}{5,11}  {6,1}{7,3}", gpstime.Year, gpstime.Month,
                        gpstime.Day, gpstime.Hour, gpstime.Minute,
                        (gpstime.Second + (gpstime.Millisecond / 1000.0)).ToString("0.0000000",
                            System.Globalization.CultureInfo.InvariantCulture), 0, NSats);

                    lastgpstime = gpstime;
                }

                if (gnss == 0)
                {
                    //GPS
                    sattype = "G";
                }
                if (gnss == 1)
                {
                    //sbas
                    sattype = "S";
                    sv -= 100;
                }
                if (gnss == 2)
                {
                    //Galileo
                    sattype = "E";
                }
                if (gnss == 3)
                {
                    //BEIDOU
                    sattype = "C";
                }
                if (gnss == 4)
                {
                    //IMES
                    sattype = "I";
                }
                if (gnss == 5)
                {
                    //QZSS
                    sattype = "J";
                    sv -= 192; // check this (100 or 192)
                }
                if (gnss == 6)
                {
                    //GLONASS
                    sattype = "R";
                }

                // a1,i2.2,satcount*(f14.3,i1,i1)
                rinexoutput.WriteLine("{0}{1,2}{2,14}  {3,14}{4,1} {5,14}  {6,14}  ", sattype, (sv).ToString("00"),
                    (prMes).ToString("0.000",
                        System.Globalization.CultureInfo.InvariantCulture),
                    (cpMes).ToString("0.000", System.Globalization.CultureInfo.InvariantCulture), lli, doMes,
                    (cno).ToString("0.000",
                        System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// abgr
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
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

        public bool IsClockwise(IList<Point3D> vertices)
        {
            double sum = 0.0;
            for (int i = 0; i < vertices.Count; i++)
            {
                Point3D v1 = vertices[i];
                Point3D v2 = vertices[(i + 1) % vertices.Count];
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum > 0.0;
        }

        public void writeKML(string filename)
        {
            try
            {
                writeGPX(filename);

                writeRinex(filename);
            }
            catch { }

            Color[] colours = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet, Color.Pink };

            AltitudeMode altmode = AltitudeMode.absolute;

            KMLRoot kml = new KMLRoot();
            Folder fldr = new Folder("Log");

            Style style = new Style();
            style.Id = "yellowLineGreenPoly";
            style.Add(new LineStyle(HexStringToColor("7f00ffff"), 4));

            Style style1 = new Style();
            style1.Id = "spray";
            style1.Add(new LineStyle(HexStringToColor("4c0000ff"), 0));
            style1.Add(new PolyStyle() { Color = HexStringToColor("4c0000ff") });

            PolyStyle pstyle = new PolyStyle();
            pstyle.Color = HexStringToColor("7f00ff00");
            style.Add(pstyle);

            kml.Document.AddStyle(style);
            kml.Document.AddStyle(style1);

            int stylecode = 0xff;
            int g = -1;
            foreach (List<Point3D> poslist in position)
            {
                g++;
                if (poslist == null)
                    continue;

                /*
                List<PointLatLngAlt> pllalist = new List<PointLatLngAlt>();

                foreach (var point in poslist)
                {
                    pllalist.Add(new PointLatLngAlt(point.Y, point.X, point.Z, ""));
                }

                var ans = Utilities.LineOffset.GetPolygon(pllalist, 2);



                while (ans.Count > 0)
                {
                    var first = ans[0];
                    var second = ans[1];
                    var secondlast = ans[ans.Count - 2];
                    var last = ans[ans.Count-1];

                    ans.Remove(first);
                    ans.Remove(last);

                    var polycoords = new BoundaryIs();

                    polycoords.LinearRing = new LinearRing();

                    polycoords.LinearRing.Coordinates.Add(new Point3D(first.Lng, first.Lat, 1));
                    polycoords.LinearRing.Coordinates.Add(new Point3D(second.Lng, second.Lat, 1));
                    polycoords.LinearRing.Coordinates.Add(new Point3D(secondlast.Lng, secondlast.Lat, 1));
                    polycoords.LinearRing.Coordinates.Add(new Point3D(last.Lng, last.Lat, 1));
                    polycoords.LinearRing.Coordinates.Add(new Point3D(first.Lng, first.Lat, 1));

                    //if (!IsClockwise(polycoords.LinearRing.Coordinates))
                      //  polycoords.LinearRing.Coordinates.Reverse();

                    Polygon kmlpoly = new Polygon() { AltitudeMode = AltitudeMode.relativeToGround, Extrude = false, OuterBoundaryIs = polycoords };

                    Placemark pmpoly = new Placemark();
                    pmpoly.Polygon = kmlpoly;
                    pmpoly.name = g + " test";
                    pmpoly.styleUrl = "#spray";

                    fldr.Add(pmpoly);
                }
                */
                LineString ls = new LineString();
                ls.AltitudeMode = altmode;
                ls.Extrude = true;
                //ls.Tessellate = true;

                Coordinates coords = new Coordinates();
                coords.AddRange(poslist);

                ls.coordinates = coords;

                Placemark pm = new Placemark();

                string mode = "";
                if (g < modelist.Count)
                    mode = modelist[g];

                pm.name = g + " Flight Path " + mode;
                pm.styleUrl = "#yellowLineGreenPoly";
                pm.LineString = ls;

                stylecode = colours[g % (colours.Length - 1)].ToArgb();

                Style style2 = new Style();
                Color color = Color.FromArgb(0xff, (stylecode >> 16) & 0xff, (stylecode >> 8) & 0xff, (stylecode >> 0) & 0xff);
                log.Info("colour " + color.ToArgb().ToString("X") + " " + color.ToKnownColor().ToString());
                style2.Add(new LineStyle(color, 4));



                pm.AddStyle(style2);

                fldr.Add(pm);
            }

            Folder planes = new Folder();
            planes.name = "Planes";
            fldr.Add(planes);

            Folder waypoints = new Folder();
            waypoints.name = "Waypoints";
            fldr.Add(waypoints);


            LineString lswp = new LineString();
            lswp.AltitudeMode = AltitudeMode.relativeToGround;
            lswp.Extrude = true;

            Coordinates coordswp = new Coordinates();

            foreach (PointLatLngAlt p1 in cmd)
            {
                coordswp.Add(new Point3D(p1.Lng, p1.Lat, p1.Alt));
            }

            lswp.coordinates = coordswp;

            Placemark pmwp = new Placemark();

            pmwp.name = "Waypoints";
            //pm.styleUrl = "#yellowLineGreenPoly";
            pmwp.LineString = lswp;

            waypoints.Add(pmwp);

            int a = 0;
            int l = -1;

            Model lastmodel = null;

            foreach (Data mod in flightdata)
            {
                l++;
                if (mod.model.Location.latitude == 0)
                    continue;

                if (lastmodel != null)
                {
                    if (lastmodel.Location.Equals(mod.model.Location))
                    {
                        continue;
                    }
                }
                Placemark pmplane = new Placemark();
                pmplane.name = "Plane " + a;

                pmplane.visibility = false;

                Model model = mod.model;
                model.AltitudeMode = altmode;
                model.Scale.x = 2;
                model.Scale.y = 2;
                model.Scale.z = 2;

                try
                {
                    pmplane.description = @"<![CDATA[
              <table>
                <tr><td>Roll: " + model.Orientation.roll + @" </td></tr>
                <tr><td>Pitch: " + model.Orientation.tilt + @" </td></tr>
                <tr><td>Yaw: " + model.Orientation.heading + @" </td></tr>
                <tr><td>WP dist " + mod.ntun[2] + @" </td></tr>
				<tr><td>tar bear " + mod.ntun[3] + @" </td></tr>
				<tr><td>nav bear " + mod.ntun[4] + @" </td></tr>
				<tr><td>alt error " + mod.ntun[5] + @" </td></tr>
              </table>
            ]]>";
                }
                catch { }

                try
                {

                    pmplane.Point = new KmlPoint((float)model.Location.longitude, (float)model.Location.latitude, (float)model.Location.altitude);
                    pmplane.Point.AltitudeMode = altmode;

                    Link link = new Link();
                    link.href = "block_plane_0.dae";

                    model.Link = link;

                    pmplane.Model = model;

                    planes.Add(pmplane);
                }
                catch { } // bad lat long value

                lastmodel = mod.model;

                a++;
            }

            kml.Document.Add(fldr);

            kml.Save(filename);

            // create kmz - aka zip file

            FileStream fs = File.Open(filename.ToLower().Replace(".log.kml", ".kmz").Replace(".bin.kml", ".kmz"), FileMode.Create);
            ZipOutputStream zipStream = new ZipOutputStream(fs);
            zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
            zipStream.UseZip64 = UseZip64.Off; // older zipfile

            // entry 1
            string entryName = ZipEntry.CleanName(Path.GetFileName(filename)); // Removes drive from name and fixes slash direction
            ZipEntry newEntry = new ZipEntry(entryName);
            newEntry.DateTime = DateTime.Now;

            zipStream.PutNextEntry(newEntry);

            // Zip the file in buffered chunks
            // the "using" will close the stream even if an exception occurs
            byte[] buffer = new byte[4096];
            using (FileStream streamReader = File.OpenRead(filename))
            {
                StreamUtils.Copy(streamReader, zipStream, buffer);
            }
            zipStream.CloseEntry();

            File.Delete(filename);

            filename = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "block_plane_0.dae";

            // entry 2
            entryName = ZipEntry.CleanName(Path.GetFileName(filename)); // Removes drive from name and fixes slash direction
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


            zipStream.IsStreamOwner = true;	// Makes the Close also Close the underlying stream
            zipStream.Close();

            positionindex = 0;
            modelist.Clear();
            flightdata.Clear();
            position = new List<Core.Geometry.Point3D>[200];
            cmd.Clear();
        }

    }
}
