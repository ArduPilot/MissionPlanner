using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using com.drew.imaging.jpg;
using com.drew.metadata;
using log4net;
using SharpKml.Base;
using SharpKml.Dom;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace ArdupilotMega
{
    public class Georefimage : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private OpenFileDialog openFileDialog1;
        private ArdupilotMega.Controls.MyButton BUT_browselog;
        private ArdupilotMega.Controls.MyButton BUT_browsedir;
        private TextBox TXT_logfile;
        private TextBox TXT_jpgdir;
        private TextBox TXT_offsetseconds;
        private ArdupilotMega.Controls.MyButton BUT_doit;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label1;
        private TextBox TXT_outputlog;
        private ArdupilotMega.Controls.MyButton BUT_estoffset;

        int latpos = 4, lngpos = 5, altpos = 7, cogpos = 9, pitchATT = 13, rollATT = 12, yawATT = 14;
        private NumericUpDown NUM_latpos;
        private NumericUpDown NUM_lngpos;
        private NumericUpDown NUM_altpos;
        private NumericUpDown NUM_headingpos;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Controls.MyButton BUT_networklinkgeoref;
        private NumericUpDown num_vfov;
        private NumericUpDown num_hfov;
        private Label label7;
        private Label label8;
        private NumericUpDown num_camerarotation;
        private Label label9;
        private ArdupilotMega.Controls.MyButton BUT_Geotagimages;

        internal Georefimage() {
            InitializeComponent();
        }

        Hashtable filedatecache = new Hashtable();
        Hashtable photocoords = new Hashtable();

        Hashtable timecoordcache = new Hashtable();
        Hashtable imagetotime = new Hashtable();

        List<string[]> logcache = new List<string[]>();

        DateTime getPhotoTime(string fn)
        {
            DateTime dtaken = DateTime.MinValue;

            if (filedatecache.ContainsKey(fn))
            {
                return (DateTime)filedatecache[fn];
            }

            try
            {
                
                Metadata lcMetadata = null;
                try
                {
                    FileInfo lcImgFile = new FileInfo(fn);
                    // Loading all meta data
                    lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
                }
                catch (JpegProcessingException e)
                {
                    log.InfoFormat(e.Message);
                    return dtaken;
                }

                foreach (AbstractDirectory lcDirectory in lcMetadata)
                {

                    if (lcDirectory.ContainsTag(0x9003))
                    {
                        dtaken = lcDirectory.GetDate(0x9003);
                        log.InfoFormat("does " + lcDirectory.GetTagName(0x9003) + " " + dtaken);

                        filedatecache[fn] = dtaken;

                        break;
                    }

                }


                


                ////// old method, works, just slow
                /*
                Image myImage = Image.FromFile(fn);
                PropertyItem propItem = myImage.GetPropertyItem(36867); // 36867  // 306

                //Convert date taken metadata to a DateTime object 
                string sdate = Encoding.UTF8.GetString(propItem.Value).Trim();
                string secondhalf = sdate.Substring(sdate.IndexOf(" "), (sdate.Length - sdate.IndexOf(" ")));
                string firsthalf = sdate.Substring(0, 10);
                firsthalf = firsthalf.Replace(":", "-");
                sdate = firsthalf + secondhalf;
                dtaken = DateTime.Parse(sdate);

                myImage.Dispose();
                 */
            }
            catch { }

            return dtaken;
        }

        List<string[]> readLog(string fn)
        {
            if (logcache.Count > 0)
                return logcache;

            List<string[]> list = new List<string[]>();

            if (fn.ToLower().EndsWith("tlog"))
            {
                MAVLink mine = new MAVLink();
                mine.logplaybackfile = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read));
                mine.logreadmode = true;

                mine.MAV.packets.Initialize(); // clear

                CurrentState cs = new CurrentState();

                string[] oldvalues = {""};

                while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                {

                    byte[] packet = mine.readPacket();

                    cs.datetime = mine.lastlogread;

                    cs.UpdateCurrentSettings(null, true, mine);

                    //		line	"GPS: 82686250, 1, 8, -34.1406480, 118.5441900, 0.0000, 309.1900, 315.9500, 0.0000, 279.1200"	string


                    string[] vals = new string[] { "GPS", (cs.datetime.ToUniversalTime() - new DateTime(cs.datetime.Year,cs.datetime.Month,cs.datetime.Day,0,0,0,DateTimeKind.Utc)).TotalMilliseconds.ToString(), "1",
                    cs.satcount.ToString(),cs.lat.ToString(),cs.lng.ToString(),"0.0",cs.alt.ToString(),cs.alt.ToString(),cs.groundspeed.ToString(),cs.yaw.ToString()};

                    if (oldvalues.Length > 2 && oldvalues[latpos] == vals[latpos]
                        && oldvalues[lngpos] == vals[lngpos]
                        && oldvalues[altpos] == vals[altpos])
                        continue;

                    oldvalues = vals;

                    list.Add(vals);
                    // 4 5 7
                    Console.Write((mine.logplaybackfile.BaseStream.Position * 100 / mine.logplaybackfile.BaseStream.Length) + "    \r");
                    
                }

                mine.logplaybackfile.Close();

                logcache = list;

                return list;
            }

            StreamReader sr = new StreamReader(fn);

            string lasttime = "0";

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                if (line.ToLower().StartsWith("gps"))
                {
                    if (!sr.EndOfStream){
                        string line2 = sr.ReadLine();
                        if (line2.ToLower().StartsWith("att")){
                            string[] vals = string.Concat(line, ",", line2).Split(new char[] { ',', ':' });
                            if (lasttime == vals[1])
                                continue;
                            lasttime = vals[1];
                            list.Add(vals);
                        }
                    }
                }
            }

            sr.Close();
            sr.Dispose();

            logcache = list;

            return list;
        }

        public void dowork(string logFile, string dirWithImages, float offsetseconds, bool dooffset)
        {
            DateTime localmin = DateTime.MaxValue;
            DateTime localmax = DateTime.MinValue;

            DateTime startTime = DateTime.MinValue;

            photocoords = new Hashtable();
            timecoordcache = new Hashtable();
            imagetotime = new Hashtable();

            //logFile = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM\2011-10-01 11-48 1.log";
            TXT_outputlog.AppendText("Read Log\n");

            List<string[]> list = readLog(logFile);

            TXT_outputlog.AppendText("Log Read\n");

            //dirWithImages = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM";

            TXT_outputlog.AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages);

            TXT_outputlog.AppendText("images read\n");

            Document kml = new Document();

            using (StreamWriter swlogloccsv = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "loglocation.csv"))
            using (StreamWriter swlockml = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.kml"))
            using (StreamWriter swloctxt = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.txt"))
            using (StreamWriter swloctel = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.tel"))
            {
                swloctel.WriteLine("version=1");

                swloctel.WriteLine("#seconds offset - " + TXT_offsetseconds.Text);
                swloctel.WriteLine("#longitude and latitude - in degrees");
                swloctel.WriteLine("#name	utc	longitude	latitude	height");

                swloctxt.WriteLine("#name longitude/X latitude/Y height/Z yaw pitch roll");
                swloctxt.WriteLine("#seconds_offset: " + TXT_offsetseconds.Text);

                int first = 0;
                int matchs = 0;

                int lastmatchindex = 0;

                TXT_outputlog.AppendText("start Processing\n");

                foreach (string filename in files)
                {
                    if (filename.ToLower().EndsWith(".jpg") && !filename.ToLower().Contains("_geotag"))
                    {
                        DateTime photodt = getPhotoTime(filename);

                        // get log min and max time
                        if (startTime == DateTime.MinValue)
                        {
                            startTime = new DateTime(photodt.Year, photodt.Month, photodt.Day, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

                            foreach (string[] arr in list)
                            {
                                DateTime crap = startTime.AddMilliseconds(int.Parse(arr[1])).AddSeconds(offsetseconds);

                                if (localmin > crap)
                                    localmin = crap;
                                if (localmax < crap)
                                    localmax = crap;
                            }

                            log.InfoFormat("min " + localmin + " max " + localmax);
                            TXT_outputlog.AppendText("Log min " + localmin + " max " + localmax + "\r\n");
                        }

                        TXT_outputlog.AppendText("Photo  " + Path.GetFileNameWithoutExtension(filename) + " time  " + photodt + "\r\n");
                        //Application.DoEvents();


                        int a = 0;

                        foreach (string[] arr in list)
                        {
                            a++;

                            if (lastmatchindex > (a))
                                continue;

                            if (a % 1000 == 0)
                                Application.DoEvents();

                            DateTime logdt = startTime.AddMilliseconds(int.Parse(arr[1])).AddSeconds(offsetseconds);

                            if (first == 0)
                            {
                                TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(filename) + " " + photodt + " vs Log " + logdt + "\r\n");

                                TXT_outputlog.AppendText("offset should be about " + (photodt - logdt).TotalSeconds + "\r\n");

                                if (dooffset)
                                {
                                    swlockml.Close();
                                    swloctel.Close();
                                    swloctxt.Close();
                                    swlogloccsv.Close();
                                    return;
                                }

                                first++;
                            }

                            // time has past, logs are in time order
                            if (photodt < logdt.AddSeconds(-1))
                            {
                                lastmatchindex = a;
                                break;
                            }

                            //Console.Write("ph " + dt + " log " + crap + "         \r");



               


                            timecoordcache[(long)(logdt.AddSeconds(-offsetseconds) - DateTime.MinValue).TotalSeconds] = new double[] {
                            double.Parse(arr[latpos]), double.Parse(arr[lngpos]), double.Parse(arr[altpos]) 
                        };

                            swlogloccsv.WriteLine("ph " + filename + " " + photodt + " log " + logdt);

                            if (photodt.ToString("yyyy-MM-ddTHH:mm:ss") == logdt.ToString("yyyy-MM-ddTHH:mm:ss"))
                            {
                                lastmatchindex = a;

                                TXT_outputlog.AppendText("MATCH Photo " + Path.GetFileNameWithoutExtension(filename) + " " + photodt + "\r\n");

                                matchs++;

                               // int fixme;
                               // if (matchs > 50)
                                //    break; ;

                                SharpKml.Dom.Timestamp tstamp = new SharpKml.Dom.Timestamp();

                                tstamp.When = photodt;

                                kml.AddFeature(
                                    new Placemark()
                                    {
                                        Time = tstamp,
                                        Name = Path.GetFileNameWithoutExtension(filename),
                                        Geometry = new SharpKml.Dom.Point()
                                        {
                                            Coordinate = new Vector(double.Parse(arr[latpos]), double.Parse(arr[lngpos]), double.Parse(arr[altpos]))
                                        },
                                        Description = new Description()
                                        {
                                            Text = "<table><tr><td><img src=\"" + Path.GetFileName(filename).ToLower() + "\" width=500 /></td></tr></table>"
                                        },
                                        StyleSelector = new Style()
                                        {
                                            Balloon = new BalloonStyle() { Text = "$[name]<br>$[description]" }
                                        }
                                    }
                                );

                                double lat = double.Parse(arr[latpos]) ;
                                double lng = double.Parse(arr[lngpos]) ;
                                double alpha = ((double.Parse(arr[yawATT]) / 100.0) + 180) + (double)num_camerarotation.Value;

                                RectangleF rect = getboundingbox(lat, lng, alpha, (double)num_hfov.Value, (double)num_vfov.Value);

                                Console.WriteLine(rect);

                                //http://en.wikipedia.org/wiki/World_file
                               /* using (StreamWriter swjpw = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".jgw"))
                                {
                                    swjpw.WriteLine((rect.Height / 2448.0).ToString("0.00000000000000000"));
                                    swjpw.WriteLine((0).ToString("0.00000000000000000")); // 
                                    swjpw.WriteLine((0).ToString("0.00000000000000000")); //
                                    swjpw.WriteLine((rect.Width / -3264.0).ToString("0.00000000000000000")); // distance per pixel
                                    swjpw.WriteLine((rect.Left).ToString("0.00000000000000000"));
                                    swjpw.WriteLine((rect.Top).ToString("0.00000000000000000"));

                                    swjpw.Close();
                                }*/

                                kml.AddFeature(
                                 new GroundOverlay()
                                 {
                                     Name = Path.GetFileNameWithoutExtension(filename),
                                     Visibility = false,
                                     Time = tstamp,
                                     AltitudeMode = AltitudeMode.ClampToGround,
                                     Bounds = new LatLonBox()
                                     {
                                         Rotation = -alpha % 360,
                                         North = rect.Bottom,
                                         East = rect.Right,
                                         West = rect.Left,
                                         South = rect.Top,
                                     },
                                     Icon = new SharpKml.Dom.Icon()
                                     {
                                         Href = new Uri(Path.GetFileName(filename).ToLower(), UriKind.Relative),
                                     },
                                 }
                                );



                                photocoords[filename] = new double[] { double.Parse(arr[latpos]), double.Parse(arr[lngpos]), double.Parse(arr[altpos]), double.Parse(arr[cogpos]) };

                                imagetotime[filename] = (long)(logdt.AddSeconds(-offsetseconds) - DateTime.MinValue).TotalSeconds;

                                swloctxt.WriteLine(Path.GetFileName(filename) + " " + arr[lngpos] + " " + arr[latpos] + " " + arr[altpos] + " " + ((double.Parse(arr[yawATT]) / 100.0) + 180) % 360 + " " + ((double.Parse(arr[pitchATT]) / 100.0)) + " " + (-double.Parse(arr[rollATT]) / 100.0));
                                swloctel.WriteLine(Path.GetFileName(filename) + "\t" + logdt.ToString("yyyy:MM:dd HH:mm:ss") + "\t" + arr[lngpos] + "\t" + arr[latpos] + "\t" + arr[altpos]);
                                swloctel.Flush();
                                swloctxt.Flush();
                                log.InfoFormat(Path.GetFileName(filename) + " " + arr[lngpos] + " " + arr[latpos] + " " + arr[altpos] + "           ");
                                break;
                            }
                            //Console.WriteLine(crap);
                        }
                    }


                }

                Serializer serializer = new Serializer();
                serializer.Serialize(kml);
                swlockml.Write(serializer.Xml);
                swlockml.Close();

                MainV2.instance.georefkml = serializer.Xml;
                MainV2.instance.georefimagepath = dirWithImages + Path.DirectorySeparatorChar;

                writeGPX(dirWithImages + Path.DirectorySeparatorChar + "location.gpx");

                swlogloccsv.Close();

                swloctxt.Close();
                swloctel.Close();

                TXT_outputlog.AppendText("Done " + matchs + " matchs");

            }
        }

 

        RectangleF getboundingbox(double centery, double centerx, double angle, double width, double height)
        {
            double lat = centery;
            double lng = centerx;
            double alpha = angle;

            double ang = degrees(Math.Atan((width / 2.0)/(height / 2.0)));

            double hyplength = Math.Sqrt(Math.Pow((double)num_vfov.Value / 2.0, 2) + Math.Pow((double)num_hfov.Value / 2.0, 2));

            double lat1 = lat;
            double lng1 = lng;

            newpos(ref lat1, ref lng1, alpha + ang, hyplength);

            double lat2 = lat;
            double lng2 = lng;

            newpos(ref lat2, ref lng2, alpha + 180 - ang, hyplength);

            double lat3 = lat;
            double lng3 = lng;

            newpos(ref lat3, ref lng3, alpha + 180 + ang, hyplength);

            double lat4 = lat;
            double lng4 = lng;

            newpos(ref lat4, ref lng4, alpha + 360 - ang, hyplength);

            double minx = 999, miny = 999, maxx = -999, maxy = -999;



            maxx = Math.Max(maxx, lat1);
            maxx = Math.Max(maxx, lat2);
            maxx = Math.Max(maxx, lat3);
            maxx = Math.Max(maxx, lat4);

            minx = Math.Min(minx, lat1);
            minx = Math.Min(minx, lat2);
            minx = Math.Min(minx, lat3);
            minx = Math.Min(minx, lat4);

            miny = Math.Min(miny, lng1);
            miny = Math.Min(miny, lng2);
            miny = Math.Min(miny, lng3);
            miny = Math.Min(miny, lng4);

            maxy = Math.Max(maxy, lng1);
            maxy = Math.Max(maxy, lng2);
            maxy = Math.Max(maxy, lng3);
            maxy = Math.Max(maxy, lng4);

            Console.WriteLine("{0} {1} {2} {3}", minx, maxx, miny, maxy);
            Console.WriteLine("{0} {1} {2} {3}", lat1, lat2, lat3, lat4);
            Console.WriteLine("{0} {1} {2} {3}", lng1, lng2, lng3, lng4);

            return new RectangleF((float)miny, (float)minx, (float)(maxy - miny), (float)(maxx - minx));
        }

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public static double radians(double val)
        {
            return val * deg2rad;
        }
        public static double degrees(double val)
        {
            return val * rad2deg;
        }

        void newpos(ref double lat, ref double lon, double bearing, double distance)
        {
            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            double radius_of_earth = 6378100.0;//# in meters

            double lat1 = radians(lat);
            double lon1 = radians(lon);
            double brng = radians((bearing+360) % 360);
            double dr = distance / radius_of_earth;

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dr) +
                        Math.Cos(lat1) * Math.Sin(dr) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dr) * Math.Cos(lat1),
                                Math.Cos(dr) - Math.Sin(lat1) * Math.Sin(lat2));

            lat = degrees(lat2);
            lon = degrees(lon2);
            //return (degrees(lat2), degrees(lon2));
        }

        private void writeGPX(string filename)
        {

            using (System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".gpx", Encoding.ASCII))
            {

                xw.WriteStartElement("gpx");

                xw.WriteStartElement("trk");

                xw.WriteStartElement("trkseg");

                List<string> items = new List<string>();

                foreach (string photo in photocoords.Keys)
                {
                    items.Add(photo);
                }

                items.Sort();

                foreach (string photo in items)
                {


                    xw.WriteStartElement("trkpt");
                    xw.WriteAttributeString("lat", ((double[])photocoords[photo])[0].ToString(new System.Globalization.CultureInfo("en-US")));
                    xw.WriteAttributeString("lon", ((double[])photocoords[photo])[1].ToString(new System.Globalization.CultureInfo("en-US")));

                    // must stay as above

                    xw.WriteElementString("time", ((DateTime)filedatecache[photo]).ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    xw.WriteElementString("ele", ((double[])photocoords[photo])[2].ToString(new System.Globalization.CultureInfo("en-US")));
                    xw.WriteElementString("course", ((double[])photocoords[photo])[3].ToString(new System.Globalization.CultureInfo("en-US")));

                    xw.WriteElementString("compass", ((double[])photocoords[photo])[3].ToString(new System.Globalization.CultureInfo("en-US")));

                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();

                xw.Close();
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Georefimage));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TXT_logfile = new System.Windows.Forms.TextBox();
            this.TXT_jpgdir = new System.Windows.Forms.TextBox();
            this.TXT_offsetseconds = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.TXT_outputlog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BUT_Geotagimages = new ArdupilotMega.Controls.MyButton();
            this.BUT_estoffset = new ArdupilotMega.Controls.MyButton();
            this.BUT_doit = new ArdupilotMega.Controls.MyButton();
            this.BUT_browsedir = new ArdupilotMega.Controls.MyButton();
            this.BUT_browselog = new ArdupilotMega.Controls.MyButton();
            this.NUM_latpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_lngpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_altpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_headingpos = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.BUT_networklinkgeoref = new ArdupilotMega.Controls.MyButton();
            this.num_vfov = new System.Windows.Forms.NumericUpDown();
            this.num_hfov = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.num_camerarotation = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_headingpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).BeginInit();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // TXT_logfile
            // 
            resources.ApplyResources(this.TXT_logfile, "TXT_logfile");
            this.TXT_logfile.Name = "TXT_logfile";
            this.TXT_logfile.TextChanged += new System.EventHandler(this.TXT_logfile_TextChanged);
            // 
            // TXT_jpgdir
            // 
            resources.ApplyResources(this.TXT_jpgdir, "TXT_jpgdir");
            this.TXT_jpgdir.Name = "TXT_jpgdir";
            // 
            // TXT_offsetseconds
            // 
            resources.ApplyResources(this.TXT_offsetseconds, "TXT_offsetseconds");
            this.TXT_offsetseconds.Name = "TXT_offsetseconds";
            // 
            // TXT_outputlog
            // 
            resources.ApplyResources(this.TXT_outputlog, "TXT_outputlog");
            this.TXT_outputlog.Name = "TXT_outputlog";
            this.TXT_outputlog.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // BUT_Geotagimages
            // 
            resources.ApplyResources(this.BUT_Geotagimages, "BUT_Geotagimages");
            this.BUT_Geotagimages.Name = "BUT_Geotagimages";
            this.BUT_Geotagimages.UseVisualStyleBackColor = true;
            this.BUT_Geotagimages.Click += new System.EventHandler(this.BUT_Geotagimages_Click);
            // 
            // BUT_estoffset
            // 
            resources.ApplyResources(this.BUT_estoffset, "BUT_estoffset");
            this.BUT_estoffset.Name = "BUT_estoffset";
            this.BUT_estoffset.UseVisualStyleBackColor = true;
            this.BUT_estoffset.Click += new System.EventHandler(this.BUT_estoffset_Click);
            // 
            // BUT_doit
            // 
            resources.ApplyResources(this.BUT_doit, "BUT_doit");
            this.BUT_doit.Name = "BUT_doit";
            this.BUT_doit.UseVisualStyleBackColor = true;
            this.BUT_doit.Click += new System.EventHandler(this.BUT_doit_Click);
            // 
            // BUT_browsedir
            // 
            resources.ApplyResources(this.BUT_browsedir, "BUT_browsedir");
            this.BUT_browsedir.Name = "BUT_browsedir";
            this.BUT_browsedir.UseVisualStyleBackColor = true;
            this.BUT_browsedir.Click += new System.EventHandler(this.BUT_browsedir_Click);
            // 
            // BUT_browselog
            // 
            resources.ApplyResources(this.BUT_browselog, "BUT_browselog");
            this.BUT_browselog.Name = "BUT_browselog";
            this.BUT_browselog.UseVisualStyleBackColor = true;
            this.BUT_browselog.Click += new System.EventHandler(this.BUT_browselog_Click);
            // 
            // NUM_latpos
            // 
            resources.ApplyResources(this.NUM_latpos, "NUM_latpos");
            this.NUM_latpos.Name = "NUM_latpos";
            this.NUM_latpos.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // NUM_lngpos
            // 
            resources.ApplyResources(this.NUM_lngpos, "NUM_lngpos");
            this.NUM_lngpos.Name = "NUM_lngpos";
            this.NUM_lngpos.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // NUM_altpos
            // 
            resources.ApplyResources(this.NUM_altpos, "NUM_altpos");
            this.NUM_altpos.Name = "NUM_altpos";
            this.NUM_altpos.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            // 
            // NUM_headingpos
            // 
            resources.ApplyResources(this.NUM_headingpos, "NUM_headingpos");
            this.NUM_headingpos.Name = "NUM_headingpos";
            this.NUM_headingpos.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // BUT_networklinkgeoref
            // 
            resources.ApplyResources(this.BUT_networklinkgeoref, "BUT_networklinkgeoref");
            this.BUT_networklinkgeoref.Name = "BUT_networklinkgeoref";
            this.BUT_networklinkgeoref.UseVisualStyleBackColor = true;
            this.BUT_networklinkgeoref.Click += new System.EventHandler(this.BUT_networklinkgeoref_Click);
            // 
            // num_vfov
            // 
            resources.ApplyResources(this.num_vfov, "num_vfov");
            this.num_vfov.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.num_vfov.Name = "num_vfov";
            this.num_vfov.Value = new decimal(new int[] {
            130,
            0,
            0,
            0});
            // 
            // num_hfov
            // 
            resources.ApplyResources(this.num_hfov, "num_hfov");
            this.num_hfov.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.num_hfov.Name = "num_hfov";
            this.num_hfov.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // num_camerarotation
            // 
            resources.ApplyResources(this.num_camerarotation, "num_camerarotation");
            this.num_camerarotation.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.num_camerarotation.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.num_camerarotation.Name = "num_camerarotation";
            this.num_camerarotation.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // Georefimage
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label9);
            this.Controls.Add(this.num_camerarotation);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.num_hfov);
            this.Controls.Add(this.num_vfov);
            this.Controls.Add(this.BUT_networklinkgeoref);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NUM_headingpos);
            this.Controls.Add(this.NUM_altpos);
            this.Controls.Add(this.NUM_lngpos);
            this.Controls.Add(this.NUM_latpos);
            this.Controls.Add(this.BUT_Geotagimages);
            this.Controls.Add(this.BUT_estoffset);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TXT_outputlog);
            this.Controls.Add(this.BUT_doit);
            this.Controls.Add(this.TXT_offsetseconds);
            this.Controls.Add(this.TXT_jpgdir);
            this.Controls.Add(this.TXT_logfile);
            this.Controls.Add(this.BUT_browsedir);
            this.Controls.Add(this.BUT_browselog);
            this.Name = "Georefimage";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_headingpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BUT_browselog_Click(object sender, EventArgs e)
        {
            logcache.Clear();

            openFileDialog1.Filter = "Logs|*.log;*.tlog";
            openFileDialog1.ShowDialog();

            if (File.Exists(openFileDialog1.FileName))
            {
                TXT_logfile.Text = openFileDialog1.FileName;
                TXT_jpgdir.Text = Path.GetDirectoryName(TXT_logfile.Text);
            }
        }

        private void BUT_browsedir_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                TXT_jpgdir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void BUT_doit_Click(object sender, EventArgs e)
        {
            if (!File.Exists(TXT_logfile.Text))
                return;
            if (!Directory.Exists(TXT_jpgdir.Text))
                return;
            float seconds;
            if (float.TryParse(TXT_offsetseconds.Text, out seconds) == false)
                return;

            BUT_doit.Enabled = false;
            try
            {
                dowork(TXT_logfile.Text, TXT_jpgdir.Text, seconds, false);
            }
            catch (Exception ex) { TXT_outputlog.AppendText("Error " + ex.ToString()); }
            BUT_doit.Enabled = true;
            BUT_Geotagimages.Enabled = true;
        }

        private void BUT_estoffset_Click(object sender, EventArgs e)
        {
            dowork(TXT_logfile.Text, TXT_jpgdir.Text, 0, true);
        }

        private void BUT_Geotagimages_Click(object sender, EventArgs e)
        {

            foreach (string file in photocoords.Keys)
            {
                WriteCoordinatesToImage(file, ((double[])photocoords[file])[0], ((double[])photocoords[file])[1], ((double[])photocoords[file])[2]);
            }

        }

        byte[] coordtobytearray(double coordin)
        {
            double coord = Math.Abs(coordin);

            byte[] output = new byte[sizeof(double) * 3];

            int d = (int)coord;
            int m = (int)((coord - d) * 60);
            double s = ((((coord - d) * 60) - m) * 60);
            /*
21 00 00 00 01 00 00 00--> 33/1
18 00 00 00 01 00 00 00--> 24/1
06 02 00 00 0A 00 00 00--> 518/10
*/

            Array.Copy(BitConverter.GetBytes((uint)d), 0, output, 0, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1), 0, output, 4, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)m), 0, output, 8, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1), 0, output, 12, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)(s * 10)), 0, output, 16, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)10), 0, output, 20, sizeof(uint));

            return output;
        }

        void WriteCoordinatesToImage(string Filename, double dLat, double dLong, double alt)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Filename)))
            {
                TXT_outputlog.AppendText("GeoTagging "+Filename + "\n");
                Application.DoEvents();

                using (Image Pic = Image.FromStream(ms))
                {
                    PropertyItem[] pi = Pic.PropertyItems;

                    pi[0].Id = 0x0004;
                    pi[0].Type = 5;
                    pi[0].Len = sizeof(ulong) * 3;
                    pi[0].Value = coordtobytearray(dLong);
                    Pic.SetPropertyItem(pi[0]);

                    pi[0].Id = 0x0002;
                    pi[0].Type = 5;
                    pi[0].Len = sizeof(ulong) * 3;
                    pi[0].Value = coordtobytearray(dLat);
                    Pic.SetPropertyItem(pi[0]);

                    pi[0].Id = 0x0006;
                    pi[0].Type = 5;
                    pi[0].Len = 8;
                    pi[0].Value = new Rational(alt).GetBytes();
                    Pic.SetPropertyItem(pi[0]);

                    pi[0].Id = 1;
                    pi[0].Len = 2;
                    pi[0].Type = 2;
                    if (dLat < 0)
                    {
                        pi[0].Value = new byte[] { (byte)'S', 0 };
                    }
                    else
                    {
                        pi[0].Value = new byte[] { (byte)'N', 0 };
                    }
                    Pic.SetPropertyItem(pi[0]);

                    pi[0].Id = 3;
                    pi[0].Len = 2;
                    pi[0].Type = 2;
                    if (dLong < 0)
                    {
                        pi[0].Value = new byte[] { (byte)'W', 0 };
                    }
                    else
                    {
                        pi[0].Value = new byte[] { (byte)'E', 0 };
                    }
                    Pic.SetPropertyItem(pi[0]);



                    string outputfilename = Path.GetDirectoryName(Filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Filename) + "_geotag" + Path.GetExtension(Filename);

                    File.Delete(outputfilename);

                    Pic.Save(outputfilename);
                }
            }
        }

        private void BUT_networklinkgeoref_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "m3u" + Path.DirectorySeparatorChar + "GeoRefnetworklink.kml");
        }

        private void TXT_logfile_TextChanged(object sender, EventArgs e)
        {
            logcache.Clear();
        }
    }

    public class Rational
    {
        uint dem = 0;
        uint num = 0;

        public Rational(double input)
        {
            Value = input;
        }

        public byte[] GetBytes()
        {
            byte[] answer = new byte[8];

            Array.Copy(BitConverter.GetBytes((uint)num), 0, answer, 0, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)dem), 0, answer, 4, sizeof(uint));

            return answer;
        }

        public double Value { 
            get { 
                return num / dem;
            } 
            set {
                if ((value % 1.0) != 0)
                {
                    dem = 100; num = (uint)(value * dem);
                }
                else
                {
                    dem = 1; num = (uint)(value);
                }
            } 
        }
    }
}
