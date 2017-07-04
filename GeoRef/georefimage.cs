using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using com.drew.imaging.jpg;
using com.drew.imaging.tiff;
using com.drew.metadata;
using log4net;
using SharpKml.Base;
using SharpKml.Dom;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Log;
using MissionPlanner.Utilities;
using Placemark = SharpKml.Dom.Placemark;

namespace MissionPlanner.GeoRef
{
    partial class Georefimage : Form
    {
        private enum PROCESSING_MODE
        {
            TIME_OFFSET,
            CAM_MSG, // via digital feedback
            TRIG // camera was triggered
        }

        private const string PHOTO_FILES_FILTER = "*.jpg;*.tif";
        private const int JXL_ID_OFFSET = 10;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Key = path of file, Value = object with picture information
        private Dictionary<string, PictureInformation> picturesInfo;

        // Key = time in milliseconds, Value = object with location info and attitude
        private Dictionary<long, VehicleLocation> vehicleLocations;

        private bool useAMSLAlt;
        private int millisShutterLag = 0;

        private Hashtable filedatecache = new Hashtable();
        private CheckBox chk_cammsg;
        private TextBox txt_basealt;
        private Label label28;
        private List<int> JXL_StationIDs = new List<int>();

        public Georefimage()
        {
            InitializeComponent();


            CHECK_AMSLAlt_Use.Checked = true;
            PANEL_TIME_OFFSET.Enabled = false;

            useAMSLAlt = CHECK_AMSLAlt_Use.Checked;

            JXL_StationIDs = new List<int>();

            selectedProcessingMode = PROCESSING_MODE.CAM_MSG;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

            myGMAP1.MapProvider = MainV2.instance.FlightData.gMapControl1.MapProvider;
            myGMAP1.MinZoom = MainV2.instance.FlightData.gMapControl1.MinZoom;
            myGMAP1.MaxZoom = MainV2.instance.FlightData.gMapControl1.MaxZoom;

            GMapOverlay overlay = new GMapOverlay();
            myGMAP1.Overlays.Add(overlay);

            myGMAP1.OnMarkerClick += MyGMAP1_OnMarkerClick;
        }

        private void MyGMAP1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            foreach (var pictureInformation in picturesInfo)
            {
                if (item.ToolTipText == Path.GetFileName(pictureInformation.Value.Path))
                {
                    //pictureBox1.Image = new Bitmap(pictureInformation.Value.Path);
                    pictureBox1.ImageLocation = pictureInformation.Value.Path;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    return;
                }
            }
        }

        /// <summary>
        /// Get a photos shutter time
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private DateTime getPhotoTime(string fn)
        {
            DateTime dtaken = DateTime.MinValue;

            if (filedatecache.ContainsKey(fn))
            {
                return (DateTime) filedatecache[fn];
            }

            try
            {
                Metadata lcMetadata = null;
                try
                {
                    FileInfo lcImgFile = new FileInfo(fn);
                    // Loading all meta data
                    if (fn.ToLower().EndsWith(".jpg"))
                    {
                        lcMetadata = JpegMetadataReader.ReadMetadata(lcImgFile);
                    }
                    else if (fn.ToLower().EndsWith(".tif"))
                    {
                        lcMetadata = TiffMetadataReader.ReadMetadata(lcImgFile);
                    }
                }
                catch (JpegProcessingException e)
                {
                    log.InfoFormat(e.Message);
                    return dtaken;
                }
                catch (TiffProcessingException e)
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

                    if (lcDirectory.ContainsTag(0x9004))
                    {
                        dtaken = lcDirectory.GetDate(0x9004);
                        log.InfoFormat("does " + lcDirectory.GetTagName(0x9004) + " " + dtaken);

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
            catch
            {
            }

            return dtaken;
        }

        public string UseGpsorGPS2()
        {
            if (chk_usegps2.Checked)
                return "GPS2";

            return "GPS";
        }

        /// <summary>
        /// Return a list of all gps messages with there timestamp from the log
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private Dictionary<long, VehicleLocation> readGPSMsgInLog(string fn)
        {
            Dictionary<long, VehicleLocation> vehiclePositionList = new Dictionary<long, VehicleLocation>();

            // Telemetry Log
            if (fn.ToLower().EndsWith("tlog"))
            {
                using (MAVLinkInterface mine = new MAVLinkInterface())
                {
                    mine.logplaybackfile =
                        new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read));
                    mine.logreadmode = true;

                    CurrentState cs = new CurrentState();

                    while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                    {
                        MAVLink.MAVLinkMessage packet = mine.readPacket();

                        cs.datetime = mine.lastlogread;

                        cs.UpdateCurrentSettings(null, true, mine);

                        // check for valid lock
                        if (!(cs.gpsstatus >=3 || cs.gpsstatus2 >= 3))
                            continue;

                        VehicleLocation location = new VehicleLocation();
                        location.Time = cs.datetime;
                        location.Lat = cs.lat;
                        location.Lon = cs.lng;
                        location.RelAlt = cs.alt;
                        location.AltAMSL = cs.altasl;

                        location.Roll = cs.roll;
                        location.Pitch = cs.pitch;
                        location.Yaw = cs.yaw;

                        location.SAlt = cs.sonarrange;

                        vehiclePositionList[ToMilliseconds(location.Time)] = location;
                        // 4 5 7
                        Console.Write((mine.logplaybackfile.BaseStream.Position*100/
                                       mine.logplaybackfile.BaseStream.Length) + "    \r");
                    }
                    mine.logplaybackfile.Close();
                }
            }
            // DataFlash Log
            else
            {
                using (var sr = new CollectionBuffer(File.OpenRead(fn)))
                {
                    // Will hold the last seen Attitude information in order to incorporate them into the GPS Info
                    float currentYaw = 0f;
                    float currentRoll = 0f;
                    float currentPitch = 0f;
                    float currentSAlt = 0f;

                    foreach (var item in sr.GetEnumeratorType(new string[] { "GPS", "GPS2", "ATT", "CTUN", "RFND" }))
                    {
                        // Look for GPS Messages. However GPS Messages do not have Roll, Pitch and Yaw
                        // So we have to look for one ATT message after having read a GPS one

                        var gpstouse = UseGpsorGPS2();

                        if (item.msgtype == gpstouse)
                        {
                            if (!sr.dflog.logformat.ContainsKey(gpstouse))
                                continue;

                            int latindex = sr.dflog.FindMessageOffset(gpstouse, "Lat");
                            int lngindex = sr.dflog.FindMessageOffset(gpstouse, "Lng");
                            int altindex = sr.dflog.FindMessageOffset(gpstouse, "Alt");
                            int statusindex = sr.dflog.FindMessageOffset(gpstouse, "Status");
                            int raltindex = sr.dflog.FindMessageOffset(gpstouse, "RAlt");
                            if (raltindex == -1)
                                raltindex = sr.dflog.FindMessageOffset(gpstouse, "RelAlt");

                            VehicleLocation location = new VehicleLocation();

                            try
                            {
                                location.Time = item.time;

                                if (statusindex != -1)
                                {
                                    // check for valid lock
                                    if (double.Parse(item.items[statusindex], CultureInfo.InvariantCulture) < 3)
                                        continue;
                                }
                                if (latindex != -1)
                                    location.Lat = double.Parse(item.items[latindex], CultureInfo.InvariantCulture);
                                if (lngindex != -1)
                                    location.Lon = double.Parse(item.items[lngindex], CultureInfo.InvariantCulture);
                                if (raltindex != -1)
                                    location.RelAlt = double.Parse(item.items[raltindex], CultureInfo.InvariantCulture);
                                if (altindex != -1)
                                    location.AltAMSL = double.Parse(item.items[altindex], CultureInfo.InvariantCulture);

                                location.Roll = currentRoll;
                                location.Pitch = currentPitch;
                                location.Yaw = currentYaw;

                                location.SAlt = currentSAlt;

                                long millis = ToMilliseconds(location.Time);

                                //System.Diagnostics.Debug.WriteLine("GPS MSG - UTCMillis = " + millis  + "  GPS Week = " + getValueFromStringArray(gpsLineValues, gpsweekpos) + "  TimeMS = " + getValueFromStringArray(gpsLineValues, timepos));

                                if (!vehiclePositionList.ContainsKey(millis) && (location.Time != DateTime.MinValue))
                                    vehiclePositionList[millis] = location;
                            }
                            catch
                            {
                                Console.WriteLine("Bad "+gpstouse+" Line");
                            }
                        }
                        else if (item.msgtype == "ATT")
                        {
                            int Rindex = sr.dflog.FindMessageOffset("ATT", "Roll");
                            int Pindex = sr.dflog.FindMessageOffset("ATT", "Pitch");
                            int Yindex = sr.dflog.FindMessageOffset("ATT", "Yaw");

                            if (Rindex != -1)
                                currentRoll = float.Parse(item.items[Rindex], CultureInfo.InvariantCulture);
                            if (Pindex != -1)
                                currentPitch = float.Parse(item.items[Pindex], CultureInfo.InvariantCulture);
                            if (Yindex != -1)
                                currentYaw = float.Parse(item.items[Yindex], CultureInfo.InvariantCulture);
                        }
                        else if (item.msgtype == "CTUN")
                        {
                            int SAltindex = sr.dflog.FindMessageOffset("CTUN", "SAlt");

                            if (SAltindex != -1)
                            {
                                currentSAlt = float.Parse(item.items[SAltindex], CultureInfo.InvariantCulture);
                            }
                        }
                        else if (item.msgtype == "RFND")
                        {
                            int SAltindex = sr.dflog.FindMessageOffset("RFND", "Dist1");

                            if (SAltindex != -1)
                            {
                                currentSAlt = float.Parse(item.items[SAltindex], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }

            return vehiclePositionList;
        }

        /// <summary>
        /// Return a list of all cam messages in a log with timestamp
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        private Dictionary<long, VehicleLocation> readCAMMsgInLog(string fn)
        {
            Dictionary<long, VehicleLocation> list = new Dictionary<long, VehicleLocation>();

            // Telemetry Log
            if (fn.ToLower().EndsWith("tlog"))
            {
                TXT_outputlog.AppendText("Warning: tlogs are not fully supported when using CAM Messages\n");

                using (MAVLinkInterface mine = new MAVLinkInterface())
                {
                    mine.logplaybackfile =
                        new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read));
                    mine.logreadmode = true;

                    CurrentState cs = new CurrentState();

                    while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                    {
                        MAVLink.MAVLinkMessage packet = mine.readPacket();

                        cs.datetime = mine.lastlogread;
                        cs.UpdateCurrentSettings(null, true, mine);

                        if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_FEEDBACK)
                        {
                            var msg = (MAVLink.mavlink_camera_feedback_t)packet.data;

                            VehicleLocation location = new VehicleLocation();
                            location.Time = FromUTCTimeMilliseconds((long)(msg.time_usec / 1000));// cs.datetime;
                            location.Lat = msg.lat;
                            location.Lon = msg.lng;
                            location.RelAlt = msg.alt_rel;
                            location.AltAMSL = msg.alt_msl;

                            location.Roll = msg.roll;
                            location.Pitch = msg.pitch;
                            location.Yaw = msg.yaw;

                            location.SAlt = cs.sonarrange;

                            list[ToMilliseconds(location.Time)] = location;

                            Console.Write((mine.logplaybackfile.BaseStream.Position*100/
                                           mine.logplaybackfile.BaseStream.Length) + "    \r");
                        }
                    }
                    mine.logplaybackfile.Close();
                }
            }
            // DataFlash Log
            else
            {
                float currentSAlt = 0;
                using (var sr = new CollectionBuffer(File.OpenRead(fn)))
                {
                    foreach (var item in sr.GetEnumeratorType(new string[] { "CAM", "RFND" }))
                    {
                        if (item.msgtype == "CAM")
                        {
                            int latindex = sr.dflog.FindMessageOffset("CAM", "Lat");
                            int lngindex = sr.dflog.FindMessageOffset("CAM", "Lng");
                            int altindex = sr.dflog.FindMessageOffset("CAM", "Alt");
                            int raltindex = sr.dflog.FindMessageOffset("CAM", "RelAlt");

                            int rindex = sr.dflog.FindMessageOffset("CAM", "Roll");
                            int pindex = sr.dflog.FindMessageOffset("CAM", "Pitch");
                            int yindex = sr.dflog.FindMessageOffset("CAM", "Yaw");

                            int gtimeindex = sr.dflog.FindMessageOffset("CAM", "GPSTime");
                            int gweekindex = sr.dflog.FindMessageOffset("CAM", "GPSWeek");

                            VehicleLocation p = new VehicleLocation();

                            p.Time = GetTimeFromGps(int.Parse(item.items[gweekindex], CultureInfo.InvariantCulture),
                                int.Parse(item.items[gtimeindex], CultureInfo.InvariantCulture));

                            p.Lat = double.Parse(item.items[latindex], CultureInfo.InvariantCulture);
                            p.Lon = double.Parse(item.items[lngindex], CultureInfo.InvariantCulture);
                            p.AltAMSL = double.Parse(item.items[altindex], CultureInfo.InvariantCulture);
                            if (raltindex != -1)
                                p.RelAlt = double.Parse(item.items[raltindex], CultureInfo.InvariantCulture);

                            p.Pitch = float.Parse(item.items[pindex], CultureInfo.InvariantCulture);
                            p.Roll = float.Parse(item.items[rindex], CultureInfo.InvariantCulture);
                            p.Yaw = float.Parse(item.items[yindex], CultureInfo.InvariantCulture);

                            p.SAlt = currentSAlt;

                            list[ToMilliseconds(p.Time)] = p;
                        }
                        else if (item.msgtype == "RFND")
                        {
                            int SAltindex = sr.dflog.FindMessageOffset("RFND", "Dist1");

                            if (SAltindex != -1)
                            {
                                currentSAlt = float.Parse(item.items[SAltindex], CultureInfo.InvariantCulture);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private Dictionary<long, VehicleLocation> readTRIGMsgInLog(string fn)
        {
            Dictionary<long, VehicleLocation> list = new Dictionary<long, VehicleLocation>();


            float currentSAlt = 0;
            using (var sr = new CollectionBuffer(File.OpenRead(fn)))
            {
                foreach (var item in sr.GetEnumeratorType(new string[] { "TRIG", "RFND" }))
                {
                    if (item.msgtype == "TRIG")
                    {
                        int latindex = sr.dflog.FindMessageOffset("TRIG", "Lat");
                        int lngindex = sr.dflog.FindMessageOffset("TRIG", "Lng");
                        int altindex = sr.dflog.FindMessageOffset("TRIG", "Alt");
                        int raltindex = sr.dflog.FindMessageOffset("TRIG", "RelAlt");

                        int rindex = sr.dflog.FindMessageOffset("TRIG", "Roll");
                        int pindex = sr.dflog.FindMessageOffset("TRIG", "Pitch");
                        int yindex = sr.dflog.FindMessageOffset("TRIG", "Yaw");

                        int gtimeindex = sr.dflog.FindMessageOffset("TRIG", "GPSTime");
                        int gweekindex = sr.dflog.FindMessageOffset("TRIG", "GPSWeek");

                        VehicleLocation p = new VehicleLocation();

                        p.Time = GetTimeFromGps(int.Parse(item.items[gweekindex], CultureInfo.InvariantCulture),
                            int.Parse(item.items[gtimeindex], CultureInfo.InvariantCulture));

                        p.Lat = double.Parse(item.items[latindex], CultureInfo.InvariantCulture);
                        p.Lon = double.Parse(item.items[lngindex], CultureInfo.InvariantCulture);
                        p.AltAMSL = double.Parse(item.items[altindex], CultureInfo.InvariantCulture);
                        if (raltindex != -1)
                            p.RelAlt = double.Parse(item.items[raltindex], CultureInfo.InvariantCulture);

                        p.Pitch = float.Parse(item.items[pindex], CultureInfo.InvariantCulture);
                        p.Roll = float.Parse(item.items[rindex], CultureInfo.InvariantCulture);
                        p.Yaw = float.Parse(item.items[yindex], CultureInfo.InvariantCulture);

                        p.SAlt = currentSAlt;

                        list[ToMilliseconds(p.Time)] = p;
                    }
                    else if (item.msgtype == "RFND")
                    {
                        int SAltindex = sr.dflog.FindMessageOffset("RFND", "Dist1");

                        if (SAltindex != -1)
                        {
                            currentSAlt = float.Parse(item.items[SAltindex], CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            return list;
        }


        public DateTime FromUTCTimeMilliseconds(long milliseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(milliseconds);
        }

        public DateTime GetTimeFromGps(int weeknumber, int milliseconds)
        {
            int LEAP_SECONDS = 17;

            DateTime datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            DateTime week = datum.AddDays(weeknumber*7);
            DateTime time = week.AddMilliseconds(milliseconds);

            return time.AddSeconds(-LEAP_SECONDS);
        }

        public long ToMilliseconds(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        private double EstimateOffset(string logFile, string dirWithImages)
        {
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
            {
                if (chk_cammsg.Checked)
                {
                    vehicleLocations = readCAMMsgInLog(logFile);
                }
                else
                {
                    vehicleLocations = readGPSMsgInLog(logFile);
                }
            }

            if (vehicleLocations == null || vehicleLocations.Count <= 0)
                return -1;

            List<string> filelist = new List<string>();
            string[] exts = PHOTO_FILES_FILTER.Split(';');
            foreach (var ext in exts)
            {
                filelist.AddRange(Directory.GetFiles(dirWithImages, ext));
            }

            string[] files = filelist.ToArray();

            if (files == null || files.Length == 0)
                return -1;

            Array.Sort(files, compareFileByPhotoTime);

            double ans = 0;

            TXT_outputlog.Clear();

            for (int a = 0; a < 4; a++)
            {
                // First Photo time
                string firstPhoto = files[a];

                DateTime photoTime = getPhotoTime(firstPhoto);

                TXT_outputlog.AppendText((a + 1) + " Picture " + Path.GetFileNameWithoutExtension(firstPhoto) +
                                         " with DateTime: " + photoTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

                // First GPS Message in Log time
                List<long> times = new List<long>(vehicleLocations.Keys);
                times.Sort();
                long firstTimeInGPSMsg = times[a];
                DateTime logTime = FromUTCTimeMilliseconds(firstTimeInGPSMsg);

                TXT_outputlog.AppendText((a + 1) + " GPS Log Msg: " + logTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

                TXT_outputlog.AppendText((a + 1) + " Est: " + (double) (photoTime - logTime).TotalSeconds + "\n");

                if (ans == 0)
                    ans = (double) (photoTime - logTime).TotalSeconds;
                else
                    ans = ans*0.5 + (photoTime - logTime).TotalSeconds*0.5;
            }

            return ans;
        }

        private void CreateReportFiles(Dictionary<string, PictureInformation> listPhotosWithInfo, string dirWithImages,
            float offset)
        {
            // Write report files
            Document kmlroot = new Document();
            Folder kml = new Folder("Pins");

            Folder overlayfolder = new Folder("Overlay");

            // add root folder to document
            kmlroot.AddFeature(kml);
            kmlroot.AddFeature(overlayfolder);

            // Clear Stations IDs
            JXL_StationIDs.Clear();

            using (
                StreamWriter swlogloccsv =
                    new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "loglocation.csv"))
            using (
                StreamWriter swloccsv =
                    new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.csv"))
            using (
                StreamWriter swlockml = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.kml"))
            using (
                StreamWriter swloctxt = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.txt"))
            using (
                StreamWriter swloctel = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.tel"))
            using (
                XmlTextWriter swloctrim = new XmlTextWriter(
                    dirWithImages + Path.DirectorySeparatorChar + "location.jxl", Encoding.ASCII))
            {
                swloctrim.Formatting = Formatting.Indented;
                swloctrim.WriteStartDocument(false);
                swloctrim.WriteStartElement("JOBFile");
                swloctrim.WriteAttributeString("jobName", "MPGeoRef");
                swloctrim.WriteAttributeString("product", "Gatewing");
                swloctrim.WriteAttributeString("productVersion", "1.0");
                swloctrim.WriteAttributeString("version", "5.6");
                // enviro
                swloctrim.WriteStartElement("Environment");
                swloctrim.WriteStartElement("CoordinateSystem");
                swloctrim.WriteElementString("SystemName", "Default");
                swloctrim.WriteElementString("ZoneName", "Default");
                swloctrim.WriteElementString("DatumName", "WGS 1984");
                swloctrim.WriteStartElement("Ellipsoid");
                swloctrim.WriteElementString("EarthRadius", "6378137");
                swloctrim.WriteElementString("Flattening", "0.00335281067183");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("Projection");
                swloctrim.WriteElementString("Type", "NoProjection");
                swloctrim.WriteElementString("Scale", "1");
                swloctrim.WriteElementString("GridOrientation", "IncreasingNorthEast");
                swloctrim.WriteElementString("SouthAzimuth", "false");
                swloctrim.WriteElementString("ApplySeaLevelCorrection", "true");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("LocalSite");
                swloctrim.WriteElementString("Type", "Grid");
                swloctrim.WriteElementString("ProjectLocationLatitude", "");
                swloctrim.WriteElementString("ProjectLocationLongitude", "");
                swloctrim.WriteElementString("ProjectLocationHeight", "");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("Datum");
                swloctrim.WriteElementString("Type", "ThreeParameter");
                swloctrim.WriteElementString("GridName", "WGS 1984");
                swloctrim.WriteElementString("Direction", "WGS84ToLocal");
                swloctrim.WriteElementString("EarthRadius", "6378137");
                swloctrim.WriteElementString("Flattening", "0.00335281067183");
                swloctrim.WriteElementString("TranslationX", "0");
                swloctrim.WriteElementString("TranslationY", "0");
                swloctrim.WriteElementString("TranslationZ", "0");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("HorizontalAdjustment");
                swloctrim.WriteElementString("Type", "NoAdjustment");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("VerticalAdjustment");
                swloctrim.WriteElementString("Type", "NoAdjustment");
                swloctrim.WriteEndElement();
                swloctrim.WriteStartElement("CombinedScaleFactor");
                swloctrim.WriteStartElement("Location");
                swloctrim.WriteElementString("Latitude", "");
                swloctrim.WriteElementString("Longitude", "");
                swloctrim.WriteElementString("Height", "");
                swloctrim.WriteEndElement();
                swloctrim.WriteElementString("Scale", "");
                swloctrim.WriteEndElement();

                swloctrim.WriteEndElement();
                swloctrim.WriteEndElement();

                // fieldbook
                swloctrim.WriteStartElement("FieldBook");

                swloctrim.WriteRaw(@"   <CameraDesignRecord ID='00000001'>
                                      <Type>GoPro   </Type>
                                      <HeightPixels>2400</HeightPixels>
                                      <WidthPixels>3200</WidthPixels>
                                      <PixelSize>0.0000022</PixelSize>
                                      <LensModel>Rectilinear</LensModel>
                                      <NominalFocalLength>0.002</NominalFocalLength>
                                    </CameraDesignRecord>
                                    <CameraRecord2 ID='00000002'>
                                      <CameraDesignID>00000001</CameraDesignID>
                                      <CameraPosition>01</CameraPosition>
                                      <Optics>
                                        <IdealAngularMagnification>1.0</IdealAngularMagnification>
                                        <AngleSymmetricDistortion>
                                          <Order3>-0.35</Order3>
                                          <Order5>0.15</Order5>
                                          <Order7>-0.033</Order7>
                                          <Order9> 0</Order9>
                                        </AngleSymmetricDistortion>
                                        <AngleDecenteringDistortion>
                                          <Column>0</Column>
                                          <Row>0</Row>
                                        </AngleDecenteringDistortion>
                                      </Optics>
                                      <Geometry>
                                        <PerspectiveCenterPixels>
                                          <PrincipalPointColumn>-1615.5</PrincipalPointColumn>
                                          <PrincipalPointRow>-1187.5</PrincipalPointRow>
                                          <PrincipalDistance>-2102</PrincipalDistance>
                                        </PerspectiveCenterPixels>
                                        <VectorOffset>
                                          <X>0</X>
                                          <Y>0</Y>
                                          <Z>0</Z>
                                        </VectorOffset>
                                        <BiVectorAngle>
                                          <XX>0</XX>
                                          <YY>0</YY>
                                          <ZZ>-1.5707963268</ZZ>
                                        </BiVectorAngle>
                                      </Geometry>
                                    </CameraRecord2>");

                // 2mm fl
                // res 2400 * 3200 = 7,680,000
                // sensor size = 1/2.5" - 5.70 × 4.28 mm
                // 2.2 μm
                // fl in pixels = fl in mm * res / sensor size

                swloctrim.WriteStartElement("PhotoInstrumentRecord");
                swloctrim.WriteAttributeString("ID", "0000000E");
                swloctrim.WriteElementString("Type", "Aerial");
                swloctrim.WriteElementString("Model", "X100");
                swloctrim.WriteElementString("Serial", "000-000");
                swloctrim.WriteElementString("FirmwareVersion", "v0.0");
                swloctrim.WriteElementString("UserDefinedName", "Prototype");
                swloctrim.WriteEndElement();

                swloctrim.WriteStartElement("AtmosphereRecord");
                swloctrim.WriteAttributeString("ID", "0000000F");
                swloctrim.WriteElementString("Pressure", "");
                swloctrim.WriteElementString("Temperature", "");
                swloctrim.WriteElementString("PPM", "");
                swloctrim.WriteElementString("ApplyEarthCurvatureCorrection", "false");
                swloctrim.WriteElementString("ApplyRefractionCorrection", "false");
                swloctrim.WriteElementString("RefractionCoefficient", "0");
                swloctrim.WriteElementString("PressureInputMethod", "ReadFromInstrument");
                swloctrim.WriteEndElement();

                swloctel.WriteLine("version=1");

                swloctel.WriteLine("#seconds offset - " + offset);
                swloctel.WriteLine("#longitude and latitude - in degrees");
                swloctel.WriteLine("#name	utc	longitude	latitude	height");

                swloctxt.WriteLine("#name latitude/Y longitude/X height/Z yaw pitch roll SAlt");

                TXT_outputlog.AppendText("Start Processing\n");

                // Dont know why but it was 10 in the past so let it be. Used to generate jxl file simulating x100 from trimble
                int lastRecordN = JXL_ID_OFFSET;

                // path
                CoordinateCollection coords = new CoordinateCollection();

                foreach (var item in vehicleLocations.Values)
                {
                    if (item != null)
                        coords.Add(new SharpKml.Base.Vector(item.Lat, item.Lon, item.AltAMSL));
                }

                var ls = new LineString() { Coordinates = coords, AltitudeMode = AltitudeMode.Absolute };

                SharpKml.Dom.Placemark pm = new SharpKml.Dom.Placemark() { Geometry = ls, Name = "path" };

                kml.AddFeature(pm);

                foreach (PictureInformation picInfo in listPhotosWithInfo.Values)
                {
                    string filename = Path.GetFileName(picInfo.Path);
                    string filenameWithoutExt = Path.GetFileNameWithoutExtension(picInfo.Path);

                    SharpKml.Dom.Timestamp tstamp = new SharpKml.Dom.Timestamp();

                    tstamp.When = picInfo.Time;

                    kml.AddFeature(
                        new Placemark()
                        {
                            Time = tstamp,
                            Visibility = true,
                            Name = filenameWithoutExt,
                            Geometry = new SharpKml.Dom.Point()
                            {
                                Coordinate = new Vector(picInfo.Lat, picInfo.Lon, picInfo.AltAMSL),
                                AltitudeMode = AltitudeMode.Absolute
                            },
                            Description = new Description()
                            {
                                Text =
                                    "<table><tr><td><img src=\"" + filename.ToLower() +
                                    "\" width=500 /></td></tr></table>"
                            },
                            StyleSelector = new Style()
                            {
                                Balloon = new BalloonStyle() { Text = "$[name]<br>$[description]" }
                            }
                        }
                        );

                    double lat = picInfo.Lat;
                    double lng = picInfo.Lon;
                    double alpha = picInfo.Yaw + (double)num_camerarotation.Value;

                    RectangleF rect = getboundingbox(picInfo.Lat, picInfo.Lon, picInfo.AltAMSL, alpha, (double)num_hfov.Value, (double)num_vfov.Value);

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

                    overlayfolder.AddFeature(
                        new GroundOverlay()
                        {
                            Name = filenameWithoutExt,
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
                                Href = new Uri(filename.ToLower(), UriKind.Relative),
                            },
                        }
                        );

                    swloctxt.WriteLine(filename + " " + picInfo.Lat.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Lon.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.getAltitude(useAMSLAlt).ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Yaw.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Pitch.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Roll.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.SAlt.ToString(CultureInfo.InvariantCulture));

                    swloccsv.WriteLine(filename + "," + picInfo.Lat.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Lon.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.getAltitude(useAMSLAlt).ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Yaw.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Pitch.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Roll.ToString(CultureInfo.InvariantCulture));

                    swloctel.WriteLine(filename + "\t" + picInfo.Time.ToString("yyyy:MM:dd HH:mm:ss") + "\t" +
                                       picInfo.Lon + "\t" + picInfo.Lat + "\t" + picInfo.getAltitude(useAMSLAlt));
                    swloctel.Flush();
                    swloctxt.Flush();

                    lastRecordN = GenPhotoStationRecord(swloctrim, picInfo.Path, picInfo.Lat, picInfo.Lon,
                        picInfo.getAltitude(useAMSLAlt), 0, 0, picInfo.Yaw, picInfo.Width, picInfo.Height, lastRecordN);

                    log.InfoFormat(filename + " " + picInfo.Lon + " " + picInfo.Lat + " " +
                                   picInfo.getAltitude(useAMSLAlt) + "           ");
                }

                Serializer serializer = new Serializer();
                serializer.Serialize(kmlroot);
                swlockml.Write(serializer.Xml);

                Utilities.httpserver.georefkml = serializer.Xml;
                Utilities.httpserver.georefimagepath = dirWithImages + Path.DirectorySeparatorChar;

                writeGPX(dirWithImages + Path.DirectorySeparatorChar + "location.gpx", listPhotosWithInfo);

                // flightmission
                GenFlightMission(swloctrim, lastRecordN);

                swloctrim.WriteEndElement(); // fieldbook
                swloctrim.WriteEndElement(); // job
                swloctrim.WriteEndDocument();

                TXT_outputlog.AppendText("Done \n\n");
            }
        }

        private VehicleLocation LookForLocation(DateTime t, Dictionary<long, VehicleLocation> listLocations,
            int offsettime = 2000)
        {
            long time = ToMilliseconds(t);

            // Time at which the GPS position is actually search and found
            long actualTime = time;
            int millisSTEP = 1;

            // 2 seconds (2000 ms) in the log as absolute maximum
            int maxIteration = offsettime;

            bool found = false;
            int iteration = 0;
            VehicleLocation location = null;

            while (!found && iteration < maxIteration)
            {
                found = listLocations.ContainsKey(actualTime);
                if (found)
                {
                    location = listLocations[actualTime];
                }
                else
                {
                    actualTime += millisSTEP;
                    iteration++;
                }
            }

            /*if (location == null)
                TXT_outputlog.AppendText("Time not found in log: " + time  + "\n");
            else
                TXT_outputlog.AppendText("GPS position found " + (actualTime - time) + " ms away\n");*/

            return location;
        }

        public Dictionary<string, PictureInformation> doworkGPSOFFSET(string logFile, string dirWithImages, float offset)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            // Read Vehicle Locations from log. GPS Messages. Will have to do it anyway
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
            {
                if (chk_cammsg.Checked)
                {
                    TXT_outputlog.AppendText("Reading log for CAM Messages\n");

                    vehicleLocations = readCAMMsgInLog(logFile);
                }
                else
                {
                    TXT_outputlog.AppendText("Reading log for GPS-ATT Messages\n");

                    vehicleLocations = readGPSMsgInLog(logFile);
                }
            }

            if (vehicleLocations == null)
            {
                TXT_outputlog.AppendText("Log file problem. Aborting....\n");
                return null;
            }

            TXT_outputlog.AppendText("Log locations : " + vehicleLocations.Count + "\n");

            TXT_outputlog.AppendText("Read images\n");

            List<string> filelist = new List<string>();
            string[] exts = PHOTO_FILES_FILTER.Split(';');
            foreach (var ext in exts)
            {
                filelist.AddRange(Directory.GetFiles(dirWithImages, ext));
            }

            string[] files = filelist.ToArray();

            TXT_outputlog.AppendText("Images read : " + files.Length + "\n");

            // Check that we have at least one picture
            if (files.Length <= 0)
            {
                TXT_outputlog.AppendText("Not enought files found.  Aborting..... \n");
                return null;
            }

            // load all image info
            Parallel.ForEach(files, filename => { getPhotoTime(filename); });

            // sort
            Array.Sort(files, compareFileByPhotoTime);

            // Each file corresponds to one CAM message
            // We assume that picture names are in ascending order in time
            Parallel.ForEach(files, filename =>
            {
                PictureInformation p = new PictureInformation();

                // Fill shot time in Picture
                p.ShotTimeReportedByCamera = getPhotoTime(filename);

                // Lookfor corresponding Location in vehicleLocationList
                DateTime correctedTime = p.ShotTimeReportedByCamera.AddSeconds(-offset);
                VehicleLocation shotLocation = LookForLocation(correctedTime, vehicleLocations, 5000);

                if (shotLocation == null)
                {
                    AppendText("Photo " + Path.GetFileNameWithoutExtension(filename) +
                                             " NOT PROCESSED. No GPS match in the log file. Please take care\n");
                }
                else
                {
                    p.Lat = shotLocation.Lat;
                    p.Lon = shotLocation.Lon;
                    p.AltAMSL = shotLocation.AltAMSL;

                    p.RelAlt = shotLocation.RelAlt;

                    p.Pitch = shotLocation.Pitch;
                    p.Roll = shotLocation.Roll;
                    p.Yaw = shotLocation.Yaw;

                    p.SAlt = shotLocation.SAlt;

                    p.Time = shotLocation.Time;

                    p.Path = filename;

                    lock(this)
                        picturesInformationTemp.Add(filename, p);

                    AppendText("Photo " + Path.GetFileNameWithoutExtension(filename) +
                                             " PROCESSED with GPS position found " +
                                             (shotLocation.Time - correctedTime).Milliseconds + " ms away\n");
                }
            });

            return picturesInformationTemp;
        }

        private void AppendText(string text)
        {
            var inv = new MethodInvoker(delegate { TXT_outputlog.AppendText(text); });

            this.BeginInvoke(inv);
        }

        private int compareFileByPhotoTime(string x, string y)
        {
            // compare times
            var ans1 = getPhotoTime(x).CompareTo(getPhotoTime(y));
            // if times are the same, compare filenames
            return ans1 != 0 ? ans1 : x.CompareTo(y);
        }

        public Dictionary<string, PictureInformation> doworkCAM(string logFile, string dirWithImages)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            TXT_outputlog.AppendText("Using AMSL Altitude " + useAMSLAlt + "\n");

            // If we are required to use AMSL then GPS messages should be used until CAM messages includes AMSL in the coming AC versions
            // Or if the user enter shutter lag and thus we have to look for GPS messages ahead in time
            if (useAMSLAlt || millisShutterLag > 0)
            {
                TXT_outputlog.AppendText("Reading log for GPS Messages in order to get AMSL Altitude\n");
                if (vehicleLocations == null || vehicleLocations.Count <= 0)
                {
                    vehicleLocations = readGPSMsgInLog(logFile);

                    if (vehicleLocations == null || vehicleLocations.Count <= 0)
                    {
                        TXT_outputlog.AppendText("Log file problem. Aborting....\n");
                        return null;
                    }
                }
                TXT_outputlog.AppendText("Log Read for GPS Messages\n");
                TXT_outputlog.AppendText("Log locations : " + vehicleLocations.Count + "\n");

            }

            TXT_outputlog.AppendText("Reading log for CAM Messages\n");

            var list = readCAMMsgInLog(logFile);

            if (list == null)
            {
                TXT_outputlog.AppendText("Log file problem. No CAM messages. Aborting....\n");
                return null;
            }

            TXT_outputlog.AppendText("Log Read with - " + list.Count + " - CAM Messages found\n");

            TXT_outputlog.AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            TXT_outputlog.AppendText("Images read : " + files.Length + "\n");

            // Check that we have same number of CAMs than files
            if (files.Length != list.Count)
            {
                TXT_outputlog.AppendText(string.Format("CAM Msgs and Files discrepancy. Check it! files: {0} vs CAM msg: {1}\n",files.Length,list.Count));
                return null;
            }

            Array.Sort(files, compareFileByPhotoTime);

            // Each file corresponds to one CAM message
            // We assume that picture names are in ascending order in time
            int i = -1;
            foreach (var currentCAM in list.Values)
            {
                i++;
                PictureInformation p = new PictureInformation();

                // Fill shot time in Picture
                p.ShotTimeReportedByCamera = getPhotoTime(files[i]);

                DateTime dCAMMsgTime = currentCAM.Time;

                if (millisShutterLag == 0)
                {
                    // Lets puts GPS time
                    p.Time = dCAMMsgTime;

                    p.Lat = currentCAM.Lat;
                    p.Lon = currentCAM.Lon;
                    p.AltAMSL = currentCAM.AltAMSL;
                    p.RelAlt = currentCAM.RelAlt;

                    VehicleLocation cameraLocationFromGPSMsg = null;

                    string logAltMsg = "RelAlt";

                    if (useAMSLAlt)
                    {
                        cameraLocationFromGPSMsg = LookForLocation(p.Time, vehicleLocations);
                        if (cameraLocationFromGPSMsg != null)
                        {
                            logAltMsg = "AMSL Alt " + (cameraLocationFromGPSMsg.Time - p.Time).Milliseconds + " ms away" +
                                        " offset: " + (p.ShotTimeReportedByCamera - dCAMMsgTime).TotalSeconds;
                            p.AltAMSL = cameraLocationFromGPSMsg.AltAMSL;
                        }
                        else
                            logAltMsg = "AMSL Alt NOT found";
                    }


                    p.Pitch = currentCAM.Pitch;
                    p.Roll = currentCAM.Roll;
                    p.Yaw = currentCAM.Yaw;

                    p.SAlt = currentCAM.SAlt;

                    p.Path = files[i];

                    string picturePath = files[i];

                    picturesInformationTemp.Add(picturePath, p);

                    TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(picturePath) +
                                             " processed from CAM Msg with " + millisShutterLag + " ms shutter lag. " +
                                             logAltMsg + "\n");
                }
                else
                {
                    // Look fot GPS Message ahead
                    DateTime dCorrectedWithLagPhotoTime = dCAMMsgTime;
                    dCorrectedWithLagPhotoTime = dCorrectedWithLagPhotoTime.AddMilliseconds(millisShutterLag);

                    VehicleLocation cameraLocationFromGPSMsg = LookForLocation(dCorrectedWithLagPhotoTime,
                        vehicleLocations);

                    // Check which GPS Position is closer in time.
                    if (cameraLocationFromGPSMsg != null)
                    {
                        System.TimeSpan diffGPSTimeCAMTime = cameraLocationFromGPSMsg.Time - dCAMMsgTime;

                        if (diffGPSTimeCAMTime.Milliseconds > 2*millisShutterLag)
                        {
                            // Stay with CAM Message as it is closer to CorrectedTime
                            p.Time = dCAMMsgTime;

                            p.Lat = currentCAM.Lat;
                            p.Lon = currentCAM.Lon;
                            p.AltAMSL = currentCAM.AltAMSL;
                            p.RelAlt = currentCAM.RelAlt;

                            string logAltMsg = "RelAlt";

                            cameraLocationFromGPSMsg = null;
                            if (useAMSLAlt)
                            {
                                cameraLocationFromGPSMsg = LookForLocation(p.Time, vehicleLocations);
                                if (cameraLocationFromGPSMsg != null)
                                {
                                    logAltMsg = "AMSL Alt " + (cameraLocationFromGPSMsg.Time - p.Time).Milliseconds +
                                                " ms away";
                                    p.AltAMSL = cameraLocationFromGPSMsg.AltAMSL;
                                }
                                else
                                    logAltMsg = "AMSL Alt NOT found";
                            }


                            TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
                                                     " processed with CAM Msg. Shutter lag too small. " + logAltMsg +
                                                     "\n");
                        }
                        else
                        {
                            // Get GPS Time as it is closer to CorrectedTime
                            // Lets puts GPS time
                            p.Time = cameraLocationFromGPSMsg.Time;

                            p.Lat = cameraLocationFromGPSMsg.Lat;
                            p.Lon = cameraLocationFromGPSMsg.Lon;
                            p.AltAMSL = cameraLocationFromGPSMsg.AltAMSL;
                            p.RelAlt = cameraLocationFromGPSMsg.RelAlt;

                            string logAltMsg = useAMSLAlt ? "AMSL Alt" : "RelAlt";

                            TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
                                                     " processed with GPS Msg : " + diffGPSTimeCAMTime.Milliseconds +
                                                     " ms ahead of CAM Msg. " + logAltMsg + "\n");
                        }

                        p.Pitch = currentCAM.Pitch;
                        p.Roll = currentCAM.Roll;
                        p.Yaw = currentCAM.Yaw;

                        p.SAlt = currentCAM.SAlt;

                        p.Path = files[i];

                        string picturePath = files[i];

                        picturesInformationTemp.Add(picturePath, p);
                    }
                    else
                    {
                        TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
                                                 " NOT Processed. Time not found in log. Too large Shutter Lag? Try setting it to 0\n");
                    }
                }
            }

            return picturesInformationTemp;
        }

        private void GenFlightMission(XmlTextWriter swloctrim, int lastRecordN)
        {
            swloctrim.WriteStartElement("FlightMissionRecord");
            swloctrim.WriteAttributeString("ID", (lastRecordN++).ToString("0000000"));
            swloctrim.WriteElementString("Name", "MP");
            swloctrim.WriteStartElement("FlightBlock");
            swloctrim.WriteStartElement("FlightPlan");
            swloctrim.WriteAttributeString("height", "100");
            swloctrim.WriteAttributeString("percentForwardOverlap", "75");
            swloctrim.WriteAttributeString("percentLateralOverlap", "75");
            //swloctrim.WriteElementString("Node", "");
            //swloctrim.WriteElementString("Node", "");
            //swloctrim.WriteElementString("Node", "");
            //swloctrim.WriteElementString("Node", "");
            swloctrim.WriteEndElement();
            swloctrim.WriteStartElement("StationList");
            foreach (int station in JXL_StationIDs)
            {
                swloctrim.WriteElementString("StationID", station.ToString("0000000"));
            }
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();
        }

        private int GenPhotoStationRecord(XmlTextWriter swloctrim, string imgname, double lat, double lng, double alt,
            double roll, double pitch, double yaw, int imgwidth, int imgheight, int lastRecordN)
        {
            Console.WriteLine("yaw {0}", yaw);

            int photoStationID = lastRecordN++;
            int pointRecordID = lastRecordN++;
            int imageRecordID = lastRecordN++;

            JXL_StationIDs.Add(photoStationID);

            // conver tto rads
            yaw = -yaw*MathHelper.deg2rad;

            swloctrim.WriteStartElement("PhotoStationRecord");
            swloctrim.WriteAttributeString("ID", (photoStationID).ToString("0000000"));

            swloctrim.WriteElementString("StationName", Path.GetFileNameWithoutExtension(imgname));
            swloctrim.WriteElementString("InstrumentHeight", "");

            swloctrim.WriteStartElement("RawInstrumentHeight");
            swloctrim.WriteElementString("MeasurementMethod", "TrueHeight");
            swloctrim.WriteElementString("MeasuredHeight", "0");
            swloctrim.WriteElementString("HorizontalOffset", "0");
            swloctrim.WriteElementString("VerticalOffset", "0");
            swloctrim.WriteEndElement();

            swloctrim.WriteElementString("InstrumentID", "0000000E");
            swloctrim.WriteElementString("AtmosphereID", "0000000F");
            swloctrim.WriteElementString("StationType", "RawSensorValues");

            swloctrim.WriteStartElement("DeviceAxisOrientationData");
            swloctrim.WriteStartElement("DeviceAxisOrientation");
            swloctrim.WriteStartElement("BiVector");
            swloctrim.WriteElementString("XX", roll.ToString());
            swloctrim.WriteElementString("YY", pitch.ToString());
            swloctrim.WriteElementString("ZZ", yaw.ToString());
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();
            // end PhotoStationRecord

            // pointrecord

            swloctrim.WriteStartElement("PointRecord");
            swloctrim.WriteAttributeString("ID", (pointRecordID).ToString("0000000"));

            swloctrim.WriteElementString("Name", Path.GetFileNameWithoutExtension(imgname));
            swloctrim.WriteElementString("Code", "");
            swloctrim.WriteElementString("Method", "Coordinates");
            swloctrim.WriteElementString("SurveyMethod", "Autonomous");
            swloctrim.WriteElementString("Classification", "Normal");
            swloctrim.WriteElementString("Deleted", "false");
            swloctrim.WriteStartElement("WGS84");
            swloctrim.WriteElementString("Latitude", lat.ToString());
            swloctrim.WriteElementString("Longitude", lng.ToString());
            swloctrim.WriteElementString("Height", alt.ToString());
            swloctrim.WriteEndElement();
            swloctrim.WriteEndElement();

            // end point record

            // imagerecord
            swloctrim.WriteStartElement("ImageRecord");
            swloctrim.WriteAttributeString("ID", (imageRecordID).ToString("0000000"));
            swloctrim.WriteElementString("StationID", (photoStationID).ToString("0000000"));
            swloctrim.WriteElementString("BackBearingID", "");
            swloctrim.WriteElementString("CameraID", "00000002");
            swloctrim.WriteElementString("PointRecordID", "");
            swloctrim.WriteElementString("FileName", Path.GetFileName(imgname));
            swloctrim.WriteElementString("HorizontalAngle", "");
            swloctrim.WriteElementString("VerticalAngle", "");
            swloctrim.WriteElementString("Width", imgwidth.ToString());
            swloctrim.WriteElementString("Height", imgheight.ToString());
            swloctrim.WriteElementString("SourceX", "0");
            swloctrim.WriteElementString("SourceY", "0");
            swloctrim.WriteElementString("SourceWidth", imgwidth.ToString());
            swloctrim.WriteElementString("SourceHeight", imgheight.ToString());
            swloctrim.WriteEndElement();
            /*
    <ImageRecord ID="0000056" TimeStamp="2013-04-12T10:22:21">
      <StationID>0000010</StationID>
      <BackBearingID/>
      <CameraID>00000002</CameraID>
      <PointRecordID/>
      <FileName>R0011726.JPG</FileName>
      <HorizontalAngle/>
      <VerticalAngle/>
      <Width>3648</Width>
      <Height>2736</Height>
      <SourceX>0</SourceX>
      <SourceY>0</SourceY>
      <SourceWidth>3648</SourceWidth>
      <SourceHeight>2736</SourceHeight>
    </ImageRecord>
             * */

            return lastRecordN;
        }

        private RectangleF getboundingbox(double centery, double centerx, double alt, double angle, double width, double height)
        {
            double lat = centery;
            double lng = centerx;
            double alpha = angle;

            var rect = ImageProjection.calc(new PointLatLngAlt(lat, lng, alt), 0, 0, alpha, width, height);



            double minx = 999, miny = 999, maxx = -999, maxy = -999;

            foreach (var pnt in rect)
            {
                maxx = Math.Max(maxx, pnt.Lat);

                minx = Math.Min(minx, pnt.Lat);

                miny = Math.Min(miny, pnt.Lng);

                maxy = Math.Max(maxy, pnt.Lng);
            }

            Console.WriteLine("{0} {1} {2} {3}", minx, maxx, miny, maxy);

            return new RectangleF((float) miny, (float) minx, (float) (maxy - miny), (float) (maxx - minx));
        }

        public static double radians(double val)
        {
            return val*MathHelper.deg2rad;
        }

        public static double degrees(double val)
        {
            return val*MathHelper.rad2deg;
        }

        private void newpos(ref double lat, ref double lon, double bearing, double distance)
        {
            // '''extrapolate latitude/longitude given a heading and distance 
            //   thanks to http://www.movable-type.co.uk/scripts/latlong.html
            //  '''
            // from math import sin, asin, cos, atan2, radians, degrees
            double radius_of_earth = 6378100.0; //# in meters

            double lat1 = radians(lat);
            double lon1 = radians(lon);
            double brng = radians((bearing + 360)%360);
            double dr = distance/radius_of_earth;

            double lat2 = Math.Asin(Math.Sin(lat1)*Math.Cos(dr) +
                                    Math.Cos(lat1)*Math.Sin(dr)*Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng)*Math.Sin(dr)*Math.Cos(lat1),
                Math.Cos(dr) - Math.Sin(lat1)*Math.Sin(lat2));

            lat = degrees(lat2);
            lon = degrees(lon2);
            //return (degrees(lat2), degrees(lon2));
        }

        private void writeGPX(string filename, Dictionary<string, PictureInformation> pictureList)
        {
            using (
                System.Xml.XmlTextWriter xw =
                    new System.Xml.XmlTextWriter(
                        Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar +
                        Path.GetFileNameWithoutExtension(filename) + ".gpx", Encoding.ASCII))
            {
                xw.WriteStartElement("gpx");

                xw.WriteStartElement("trk");

                xw.WriteStartElement("trkseg");

                foreach (PictureInformation p in pictureList.Values)
                {
                    xw.WriteStartElement("trkpt");
                    xw.WriteAttributeString("lat", p.Lat.ToString(new System.Globalization.CultureInfo("en-US")));
                    xw.WriteAttributeString("lon", p.Lon.ToString(new System.Globalization.CultureInfo("en-US")));

                    // must stay as above

                    xw.WriteElementString("time", p.Time.ToString("yyyy-MM-ddTHH:mm:ssZ"));

                    xw.WriteElementString("ele", p.RelAlt.ToString(new System.Globalization.CultureInfo("en-US")));
                    xw.WriteElementString("course", p.Yaw.ToString(new System.Globalization.CultureInfo("en-US")));

                    xw.WriteElementString("compass", p.Yaw.ToString(new System.Globalization.CultureInfo("en-US")));

                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndElement();
                xw.WriteEndElement();
            }
        }

        private void BUT_browselog_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Logs|*.log;*.tlog;*.bin";
            openFileDialog1.ShowDialog();

            if (File.Exists(openFileDialog1.FileName))
            {
                TXT_logfile.Text = openFileDialog1.FileName;
                TXT_jpgdir.Text = Path.GetDirectoryName(TXT_logfile.Text);
            }
        }

        private void BUT_browsedir_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(TXT_logfile.Text);
            }
            catch
            {
            }

            folderBrowserDialog1.ShowDialog();

            if (folderBrowserDialog1.SelectedPath != "")
            {
                TXT_jpgdir.Text = folderBrowserDialog1.SelectedPath;

                string file = folderBrowserDialog1.SelectedPath + Path.DirectorySeparatorChar + "location.txt";

                if (File.Exists(file))
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            string cotent = sr.ReadToEnd();

                            Match match = Regex.Match(cotent, "seconds_offset: ([0-9]+)");

                            if (match.Success)
                            {
                                TXT_offsetseconds.Text = match.Groups[1].Value;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void BUT_doit_Click(object sender, EventArgs e)
        {
            string dirPictures = TXT_jpgdir.Text;
            string logFilePath = TXT_logfile.Text;

            if (!File.Exists(logFilePath))
                return;
            if (!Directory.Exists(dirPictures))
                return;

            float seconds = 0;
            if (selectedProcessingMode == PROCESSING_MODE.TIME_OFFSET)
            {
                if (
                    float.TryParse(TXT_offsetseconds.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds) ==
                    false)
                {
                    TXT_outputlog.AppendText("Offset number not in correct format. Use . as decimal separator\n");
                    return;
                }
            }

            BUT_doit.Enabled = false;
            TXT_outputlog.Clear();

            try
            {
                switch (selectedProcessingMode)
                {
                    case PROCESSING_MODE.TIME_OFFSET:
                        picturesInfo = doworkGPSOFFSET(logFilePath, dirPictures, seconds);
                        if (picturesInfo != null)
                            CreateReportFiles(picturesInfo, dirPictures, seconds);
                        break;
                    case PROCESSING_MODE.CAM_MSG:
                        picturesInfo = doworkCAM(logFilePath, dirPictures);
                        if (picturesInfo != null)
                            CreateReportFiles(picturesInfo, dirPictures, seconds);
                        break;
                    case PROCESSING_MODE.TRIG:
                        picturesInfo = doworkTRIG(logFilePath, dirPictures);
                        if (picturesInfo != null)
                            CreateReportFiles(picturesInfo, dirPictures, seconds);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXT_outputlog.AppendText("Error " + ex.ToString());
            }


            GMapRoute route = new GMapRoute("vehicle");
            if (vehicleLocations != null)
            {
                foreach (var vehicleLocation in vehicleLocations)
                {
                    route.Points.Add(new PointLatLngAlt(vehicleLocation.Value.Lat, vehicleLocation.Value.Lon,
                        vehicleLocation.Value.AltAMSL));
                }
            }

            myGMAP1.Overlays[0].Markers.Clear();
            if (picturesInfo != null)
            {
                foreach (var pictureLocation in picturesInfo)
                {
                    myGMAP1.Overlays[0].Markers.Add(
                        new GMarkerGoogle(new PointLatLngAlt(pictureLocation.Value.Lat, pictureLocation.Value.Lon,
                            pictureLocation.Value.AltAMSL), GMarkerGoogleType.green)
                        {
                            IsHitTestVisible = true,
                            ToolTipMode = MarkerTooltipMode.OnMouseOver,
                            ToolTipText = Path.GetFileName(pictureLocation.Value.Path)
                        });
                }
            }

            myGMAP1.Overlays[0].Routes.Clear();
            myGMAP1.Overlays[0].Routes.Add(route);

            myGMAP1.ZoomAndCenterRoutes(myGMAP1.Overlays[0].Id);

            BUT_doit.Enabled = true;
            BUT_Geotagimages.Enabled = true;
        }

        private Dictionary<string, PictureInformation> doworkTRIG(string logFile, string dirWithImages)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            TXT_outputlog.AppendText("Using AMSL Altitude " + useAMSLAlt + "\n");

            TXT_outputlog.AppendText("Reading log for TRIG Messages\n");

            vehicleLocations = readTRIGMsgInLog(logFile);

            if (vehicleLocations == null)
            {
                TXT_outputlog.AppendText("Log file problem. No TRIG messages. Aborting....\n");
                return null;
            }

            TXT_outputlog.AppendText("Log Read with - " + vehicleLocations.Count + " - TRIG Messages found\n");

            TXT_outputlog.AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            TXT_outputlog.AppendText("Images read : " + files.Length + "\n");

            // Check that we have same number of CAMs than files
            if (files.Length != vehicleLocations.Count)
            {
                TXT_outputlog.AppendText(string.Format("TRIG Msgs and Files discrepancy. Check it! files: {0} vs TRIG msg: {1}\n", files.Length, vehicleLocations.Count));
                return null;
            }

            Array.Sort(files, compareFileByPhotoTime);

            // Each file corresponds to one CAM message
            // We assume that picture names are in ascending order in time
            int i = -1;
            foreach (var currentTRIG in vehicleLocations.Values)
            {
                i++;
                PictureInformation p = new PictureInformation();

                // Fill shot time in Picture
                p.ShotTimeReportedByCamera = getPhotoTime(files[i]);

                DateTime dCAMMsgTime = currentTRIG.Time;

                // Lets puts GPS time
                p.Time = dCAMMsgTime;

                p.Lat = currentTRIG.Lat;
                p.Lon = currentTRIG.Lon;
                p.AltAMSL = currentTRIG.AltAMSL;
                p.RelAlt = currentTRIG.RelAlt;


                p.Pitch = currentTRIG.Pitch;
                p.Roll = currentTRIG.Roll;
                p.Yaw = currentTRIG.Yaw;

                p.SAlt = currentTRIG.SAlt;

                p.Path = files[i];

                string picturePath = files[i];

                picturesInformationTemp.Add(picturePath, p);

                TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(picturePath) +
                                         " processed from CAM Msg with " + millisShutterLag + " ms shutter lag. " +
                                         "\n");

            }

            return picturesInformationTemp;
        }

        private void BUT_estoffset_Click(object sender, EventArgs e)
        {
            //doworkLegacy(TXT_logfile.Text, TXT_jpgdir.Text, 0, true);
            double offset = EstimateOffset(TXT_logfile.Text, TXT_jpgdir.Text);

            TXT_outputlog.AppendText("Offset around :  " + offset.ToString(CultureInfo.InvariantCulture) + "\n\n");
        }

        private void BUT_Geotagimages_Click(object sender, EventArgs e)
        {
            // Save file into Geotag folder
            string rootFolder = TXT_jpgdir.Text;
            string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

            bool isExists = System.IO.Directory.Exists(geoTagFolder);

            // delete old files and folder
            if (isExists)
                Directory.Delete(geoTagFolder, true);

            // create it again
            System.IO.Directory.CreateDirectory(geoTagFolder);

            if (picturesInfo == null)
            {
                TXT_outputlog.AppendText("no valid matchs");
                return;
            }

            foreach (PictureInformation picInfo in picturesInfo.Values)
            {
                if (useAMSLAlt)
                {
                    WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon,
                        double.Parse(txt_basealt.Text) + picInfo.AltAMSL);
                }
                else
                {
                    WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.RelAlt);
                }
            }

            TXT_outputlog.AppendText("GeoTagging FINISHED \n\n");
        }

        private byte[] coordtobytearray(double coordin)
        {
            double coord = Math.Abs(coordin);

            byte[] output = new byte[sizeof (double)*3];

            int d = (int) coord;
            int m = (int) ((coord - d)*60);
            double s = ((((coord - d)*60) - m)*60);
            /*
21 00 00 00 01 00 00 00--> 33/1
18 00 00 00 01 00 00 00--> 24/1
06 02 00 00 0A 00 00 00--> 518/10
*/
            
            Array.Copy(BitConverter.GetBytes((uint) d), 0, output, 0, sizeof (uint));
            Array.Copy(BitConverter.GetBytes((uint) 1), 0, output, 4, sizeof (uint));
            Array.Copy(BitConverter.GetBytes((uint) m), 0, output, 8, sizeof (uint));
            Array.Copy(BitConverter.GetBytes((uint) 1), 0, output, 12, sizeof (uint));
            Array.Copy(BitConverter.GetBytes((uint)(s * 1.0e7)), 0, output, 16, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1.0e7), 0, output, 20, sizeof(uint));
            /*
            Array.Copy(BitConverter.GetBytes((uint)d * 1.0e7), 0, output, 0, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1.0e7), 0, output, 4, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)0), 0, output, 8, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1), 0, output, 12, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)0), 0, output, 16, sizeof(uint));
            Array.Copy(BitConverter.GetBytes((uint)1), 0, output, 20, sizeof(uint));
            */
            return output;
        }

        public void WriteCoordinatesToImage(string Filename, double dLat, double dLong, double alt)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Filename)))
            {
                TXT_outputlog.AppendText("GeoTagging " + Filename + "\n");
                TXT_outputlog.Refresh();
                try
                {
                    using (Image Pic = Image.FromStream(ms))
                    {
                        PropertyItem[] pi = Pic.PropertyItems;

                        pi[0].Id = 0x0004;
                        pi[0].Type = 5;
                        pi[0].Len = sizeof (ulong)*3;
                        pi[0].Value = coordtobytearray(dLong);
                        Pic.SetPropertyItem(pi[0]);

                        pi[0].Id = 0x0002;
                        pi[0].Type = 5;
                        pi[0].Len = sizeof (ulong)*3;
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
                            pi[0].Value = new byte[] {(byte) 'S', 0};
                        }
                        else
                        {
                            pi[0].Value = new byte[] {(byte) 'N', 0};
                        }

                        Pic.SetPropertyItem(pi[0]);

                        pi[0].Id = 3;
                        pi[0].Len = 2;
                        pi[0].Type = 2;
                        if (dLong < 0)
                        {
                            pi[0].Value = new byte[] {(byte) 'W', 0};
                        }
                        else
                        {
                            pi[0].Value = new byte[] {(byte) 'E', 0};
                        }
                        Pic.SetPropertyItem(pi[0]);

                        // Save file into Geotag folder
                        string rootFolder = TXT_jpgdir.Text;
                        string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

                        string outputfilename = geoTagFolder + Path.DirectorySeparatorChar +
                                                Path.GetFileNameWithoutExtension(Filename) + "_geotag" +
                                                Path.GetExtension(Filename);

                        // Just in case
                        if (File.Exists(outputfilename))
                            File.Delete(outputfilename);

                        ImageCodecInfo ici = GetImageCodec("image/jpeg");
                        EncoderParameters eps = new EncoderParameters(1);
                        eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

                        Pic.Save(outputfilename);
                    }
                }
                catch (Exception ex)
                {
                    TXT_outputlog.AppendText("There was a problem with image " + Filename);
                    this.LogError(ex);
                }
            }
        }

        static ImageCodecInfo GetImageCodec(string mimetype)
        {
            foreach (ImageCodecInfo ici in ImageCodecInfo.GetImageEncoders())
            {
                if (ici.MimeType == mimetype) return ici;
            }
            return null;
        }

        private void BUT_networklinkgeoref_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Settings.GetRunningDirectory() + "m3u" + Path.DirectorySeparatorChar +
                                             "GeoRefnetworklink.kml");
        }

        private void TXT_logfile_TextChanged(object sender, EventArgs e)
        {
            if (vehicleLocations != null)
                vehicleLocations.Clear();
            if (picturesInfo != null)
                picturesInfo.Clear();

            BUT_Geotagimages.Enabled = false;
        }

        private void ProcessType_CheckedChanged(object sender, EventArgs e)
        {
            if (RDIO_CAMMsgSynchro.Checked)
            {
                selectedProcessingMode = PROCESSING_MODE.CAM_MSG;
                PANEL_TIME_OFFSET.Enabled = false;
                PANEL_SHUTTER_LAG.Enabled = true;
            }
            else if (RDIO_trigmsg.Checked)
            {
                selectedProcessingMode = PROCESSING_MODE.TRIG;
                PANEL_TIME_OFFSET.Enabled = false;
                PANEL_SHUTTER_LAG.Enabled = false;
            }
            else
            {
                selectedProcessingMode = PROCESSING_MODE.TIME_OFFSET;
                PANEL_TIME_OFFSET.Enabled = true;
                PANEL_SHUTTER_LAG.Enabled = false;
            }
        }


        private void TXT_shutterLag_TextChanged(object sender, EventArgs e)
        {
            bool convertedOK = int.TryParse(TXT_shutterLag.Text, NumberStyles.Integer, CultureInfo.InvariantCulture,
                out millisShutterLag);

            if (!convertedOK)
                TXT_shutterLag.Text = "0";
        }

        private void CHECK_AMSLAlt_Use_CheckedChanged(object sender, EventArgs e)
        {
            useAMSLAlt = ((CheckBox) sender).Checked;

            txt_basealt.Enabled = !useAMSLAlt;
        }

        private void chk_cammsg_CheckedChanged(object sender, EventArgs e)
        {
            if (vehicleLocations != null)
                vehicleLocations.Clear();
        }

        private void chk_usegps2_CheckedChanged(object sender, EventArgs e)
        {
            if (vehicleLocations != null)
                vehicleLocations.Clear();
        }
    }
}