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
using System.Xml;
using System.Text.RegularExpressions;
using System.Globalization;

namespace MissionPlanner
{
    public class Georefimage : Form 
    {
        enum PROCESSING_MODE
        { 
            TIME_OFFSET,
            CAM_MSG
        }

        public class PictureInformation : SingleLocation
        {
            string path;

            public string Path
            {
                get { return path; }
                set { path = value; }
            }

            DateTime shotTimeReportedByCamera;

            public DateTime ShotTimeReportedByCamera
            {
                get { return shotTimeReportedByCamera; }
                set { shotTimeReportedByCamera = value; }
            }

            int width;
            public int Width
            {
                get { return width; }
                set { width = value; }
            }

            int height;
            public int Height
            {
                get { return height; }
                set { height = value; }
            }

            public PictureInformation()
            {
                width = 3200;
                height = 2400;
            }
        }

        public class SingleLocation
        {
            
            DateTime time;

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            double lat;

            public double Lat
            {
                get { return lat; }
                set { lat = value; }
            }
            double lon;

            public double Lon
            {
                get { return lon; }
                set { lon = value; }
            }
            double altAMSL;

            public double AltAMSL
            {
                get { return altAMSL; }
                set { altAMSL = value; }
            }

            double relAlt;

            public double RelAlt
            {
                get { return relAlt; }
                set { relAlt = value; }
            }

            float roll;

            public float Roll
            {
                get { return roll; }
                set { roll = value; }
            }
            float pitch;

            public float Pitch
            {
                get { return pitch; }
                set { pitch = value; }
            }
            float yaw;

            public float Yaw
            {
                get { return yaw; }
                set { yaw = value; }
            }

            public double getAltitude(bool AMSL)
            {
                return (AMSL ? AltAMSL : RelAlt);
            }
        }

        public class VehicleLocation : SingleLocation
        {
            
        }

        private const string PHOTO_FILES_FILTER = "*.jpg";
        private const int JXL_ID_OFFSET = 10;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // CONSTS
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        // GPS Log positions
        //Status,Time,NSats,HDop,Lat,Lng,RelAlt,Alt,Spd,GCrs
        //GPS, 3, 122732, 10, 0.00, -35.3628880, 149.1621961, 808.90, 810.30, 23.30, 94.04
        //GPS, 3, 23524837, 1790, 10, 0.00, -35.3629379, 149.165085, 2.09, 585.41, 0.00, 129.86, 0, 4001
        // 0   1     2         3   4    5         6           7        8      9     10     11    12  13
		int gpsweekpos = 3, timepos = 2, latpos = 6, lngpos = 7, altpos = 8, altAMSLpos = 9;
        
        // ATT Msg Positions
        // ATT, 199361, 0.00, -0.40, 0.00, -3.01, 103.03, 103.03
        int pitchATT = 5, rollATT = 3, yawATT = 7;
        
        // CAM Log positions
        //CAM, 36028400, 1790, 37.4155135, -3.8520916, 69.93, -3.61, -3.82, 62.93
        int timeCAMpos = 1, weekCAMPos = 2, latCAMpos = 3, lngCAMpos = 4, altCAMpos = 5, pitchCAMATT = 6, rollCAMATT = 7, yawCAMATT = 8;

        #region GraphicalStuff
        private NumericUpDown NUM_CAM_Week;
        private Label label26;
        private Panel PANEL_SHUTTER_LAG;
        private TextBox TXT_shutterLag;
        private Label label27;
        private Label label6;
        private Panel panel2;
        private NumericUpDown NUM_ATT_Roll;
        private Label label25;
        private NumericUpDown NUM_ATT_Pitch;
        private Label label24;
        private CheckBox CHECK_AMSLAlt_Use;
        private Label label23;
        private NumericUpDown NUM_GPS_Week;
        private OpenFileDialog openFileDialog1;
        private Controls.MyButton BUT_browselog;
        private Controls.MyButton BUT_browsedir;
        private TextBox TXT_logfile;
        private TextBox TXT_jpgdir;
        private TextBox TXT_offsetseconds;
        private Controls.MyButton BUT_doit;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label1;
        private TextBox TXT_outputlog;
        private Controls.MyButton BUT_estoffset;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label20;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label label21;
        private Label label22;
        private NumericUpDown NUM_latpos;
        private NumericUpDown NUM_lngpos;
        private NumericUpDown NUM_altpos;
        private NumericUpDown NUM_ATT_Heading;
        private Controls.MyButton BUT_networklinkgeoref;
        private NumericUpDown NUM_time;
        private RadioButton RDIO_TimeOffset;
        private RadioButton RDIO_CAMMsgSynchro;
        private Panel PANEL_TIME_OFFSET;
        private NumericUpDown NUM_CAM_Alt;
        private NumericUpDown NUM_CAM_Lon;
        private NumericUpDown NUM_CAM_Lat;
        private Panel PANEL_CAM;
        private NumericUpDown num_camerarotation;
        private NumericUpDown num_hfov;
        private NumericUpDown num_vfov;
        private Panel panel3;
        private Controls.MyButton BUT_Geotagimages;
        private PROCESSING_MODE selectedProcessingMode;
        private NumericUpDown NUM_CAM_Pitch;
        private NumericUpDown NUM_CAM_Roll;
        private NumericUpDown NUM_CAM_Heading;
        private NumericUpDown NUM_GPS_AMSL_Alt;
        private NumericUpDown NUM_CAM_Time;
        private Panel panel1;
        #endregion
        
        // Key = path of file, Value = object with picture information
        Dictionary<string, PictureInformation> picturesInfo;

        // Key = time in milliseconds, Value = object with location info and attitude
        Dictionary<long, VehicleLocation> vehicleLocations;

        bool useAMSLAlt;
        int millisShutterLag = 0;

        Hashtable filedatecache = new Hashtable();
        List<int> JXL_StationIDs = new List<int>();

        internal Georefimage() {
            InitializeComponent();


            CHECK_AMSLAlt_Use.Checked = true;
            PANEL_TIME_OFFSET.Enabled = false;

            useAMSLAlt = CHECK_AMSLAlt_Use.Checked;

            JXL_StationIDs = new List<int>();

            selectedProcessingMode = PROCESSING_MODE.CAM_MSG;

            // Graphic init
            // GPS
            NUM_GPS_Week.Value = gpsweekpos;
            NUM_time.Value = timepos;
            NUM_latpos.Value = latpos;
            NUM_lngpos.Value = lngpos;
            NUM_altpos.Value = altpos;
            NUM_GPS_AMSL_Alt.Value = altAMSLpos;

            // ATT 
            NUM_ATT_Heading.Value = yawATT;
            NUM_ATT_Pitch.Value = pitchATT;
            NUM_ATT_Roll.Value = rollATT;

            // CAM
            NUM_CAM_Time.Value = timeCAMpos;
            NUM_CAM_Week.Value = weekCAMPos;
            NUM_CAM_Lat.Value = latCAMpos;
            NUM_CAM_Lon.Value = lngCAMpos;
            NUM_CAM_Alt.Value = altCAMpos;

            NUM_CAM_Heading.Value = yawCAMATT;
            NUM_CAM_Roll.Value = rollCAMATT;
            NUM_CAM_Pitch.Value = pitchCAMATT;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

        }

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

        // Return List with all GPS Messages splitted in string arrays
        Dictionary<long, VehicleLocation> readGPSMsgInLog(string fn)
        {
            Dictionary<long, VehicleLocation> vehiclePositionList = new Dictionary<long,VehicleLocation>();

            // Telemetry Log
            if (fn.ToLower().EndsWith("tlog"))
            {
                MAVLinkInterface mine = new MAVLinkInterface();
                mine.logplaybackfile = new BinaryReader(File.Open(fn, FileMode.Open, FileAccess.Read, FileShare.Read));
                mine.logreadmode = true;

                mine.MAV.packets.Initialize(); // clear

                CurrentState cs = new CurrentState();

                while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                {
                    byte[] packet = mine.readPacket();

                    cs.datetime = mine.lastlogread;

                    cs.UpdateCurrentSettings(null, true, mine);

                    VehicleLocation location = new VehicleLocation();
                    location.Time = cs.datetime;
                    location.Lat = cs.lat;
                    location.Lon = cs.lng;
                    location.RelAlt = cs.alt;
                    location.AltAMSL = cs.altasl;

                    location.Roll = cs.roll;
                    location.Pitch = cs.pitch;
                    location.Yaw = cs.yaw;

                    vehiclePositionList[ToMilliseconds(location.Time)] = location;
                    // 4 5 7
                    Console.Write((mine.logplaybackfile.BaseStream.Position * 100 / mine.logplaybackfile.BaseStream.Length) + "    \r");
                }
                mine.logplaybackfile.Close();
            }
            // DataFlash Log
            else
            {
                StreamReader sr = new StreamReader(fn);

                // Will hold the last seen Attitude information in order to incorporate them into the GPS Info
                float currentYaw = 0f;
                float currentRoll = 0f;
                float currentPitch = 0f;

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    // Look for GPS Messages. However GPS Messages do not have Roll, Pitch and Yaw
                    // So we have to look for one ATT message after having read a GPS one
                    if (line.ToLower().StartsWith("gps"))
                    {
                        VehicleLocation location = new VehicleLocation();

                        string[] gpsLineValues = line.Split(new char[] { ',', ':' });

                        location.Time = GetTimeFromGps(int.Parse(getValueFromStringArray(gpsLineValues, gpsweekpos), CultureInfo.InvariantCulture), int.Parse(getValueFromStringArray(gpsLineValues, timepos), CultureInfo.InvariantCulture));
                        location.Lat = double.Parse(getValueFromStringArray(gpsLineValues, latpos), CultureInfo.InvariantCulture);
                        location.Lon = double.Parse(getValueFromStringArray(gpsLineValues, lngpos), CultureInfo.InvariantCulture);
                        location.RelAlt = double.Parse(getValueFromStringArray(gpsLineValues, altpos), CultureInfo.InvariantCulture);
                        location.AltAMSL = double.Parse(getValueFromStringArray(gpsLineValues, altAMSLpos), CultureInfo.InvariantCulture);

                        location.Roll = currentRoll;
                        location.Pitch = currentPitch;
                        location.Yaw = currentYaw;

                        

                        long millis = ToMilliseconds(location.Time);

                        //System.Diagnostics.Debug.WriteLine("GPS MSG - UTCMillis = " + millis  + "  GPS Week = " + getValueFromStringArray(gpsLineValues, gpsweekpos) + "  TimeMS = " + getValueFromStringArray(gpsLineValues, timepos));

                        if (!vehiclePositionList.ContainsKey(millis))
                            vehiclePositionList[millis] = location;
                    }
                    else if (line.ToLower().StartsWith("att"))
                    {
                        string[] attLineValues = line.Split(new char[] { ',', ':' });

                        currentRoll = float.Parse(getValueFromStringArray(attLineValues, rollATT), CultureInfo.InvariantCulture);
                        currentPitch = float.Parse(getValueFromStringArray(attLineValues, pitchATT), CultureInfo.InvariantCulture);
                        currentYaw = float.Parse(getValueFromStringArray(attLineValues, yawATT), CultureInfo.InvariantCulture);

                    }


                }

                sr.Close();

            }

            return vehiclePositionList;
        }

        // Return List with all CAMs messages splitted in string arrays
        List<string[]> readCAMMsgInLog(string fn)
        {
            List<string[]> list = new List<string[]>();

            if (fn.ToLower().EndsWith("tlog"))
                return null;

            StreamReader sr = new StreamReader(fn);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                if (line.ToLower().StartsWith("cam"))
                {
                    string[] vals = line.Split(new char[] { ',', ':' });

                    list.Add(vals);
                }
            }

            sr.Close();
            return list;
        }

        #region HelperMethods
        
        // return a value in an array securely
        public string getValueFromStringArray(string[] array, int position)
        {
            string sResult = "-1";

            if (position < array.Length)
                sResult = array[position];

            return sResult;
        }
        public DateTime FromUTCTimeMilliseconds(long milliseconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(milliseconds);
        }

        public DateTime GetTimeFromGps(int weeknumber, int milliseconds)
        {
            int LEAP_SECONDS = 25;

            DateTime datum = new DateTime(1980,1,6,0,0,0);
            DateTime week = datum.AddDays(weeknumber * 7);
            DateTime time = week.AddMilliseconds(milliseconds);

            time.AddSeconds(-LEAP_SECONDS);

            return time;
        }
        public long ToMilliseconds(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        #endregion

        private double EstimateOffset(string logFile, string dirWithImages)
        {
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
                vehicleLocations = readGPSMsgInLog(logFile);

            if (vehicleLocations == null || vehicleLocations.Count <= 0)
                return -1;

            string[] files = Directory.GetFiles(dirWithImages, PHOTO_FILES_FILTER);

            if (files == null || files.Length == 0)
                return -1;

            Array.Sort(files, Comparer.DefaultInvariant);

            // First Photo time
            string firstPhoto = files[0];

            DateTime photoTime = getPhotoTime(firstPhoto);

            TXT_outputlog.AppendText("First Picture " + Path.GetFileNameWithoutExtension(firstPhoto) + " with DateTime: " + photoTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

            // First GPS Message in Log time
            List<long> times = new List<long>(vehicleLocations.Keys); 
            times.Sort();
            long firstTimeInGPSMsg = times[0];
            DateTime logTime = FromUTCTimeMilliseconds(firstTimeInGPSMsg);

            TXT_outputlog.AppendText("First GPS Log Msg: " + logTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

            return (double)(photoTime - logTime).TotalSeconds;
        }

        private void CreateReportFiles(Dictionary<string, PictureInformation> listPhotosWithInfo, string dirWithImages, float offset)
        {
            // Write report files
            Document kml = new Document();

            // Clear Stations IDs
            JXL_StationIDs.Clear();

            using (StreamWriter swlogloccsv = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "loglocation.csv")) 
            using (StreamWriter swlockml = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.kml"))
            using (StreamWriter swloctxt = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.txt"))
            using (StreamWriter swloctel = new StreamWriter(dirWithImages + Path.DirectorySeparatorChar + "location.tel"))
            using (XmlTextWriter swloctrim = new XmlTextWriter(dirWithImages + Path.DirectorySeparatorChar + "location.jxl", Encoding.ASCII))
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

                swloctxt.WriteLine("#name longitude/X latitude/Y height/Z yaw pitch roll");

                TXT_outputlog.AppendText("Start Processing\n");

                // Dont know why but it was 10 in the past so let it be. Used to generate jxl file simulating x100 from trimble
                int lastRecordN = JXL_ID_OFFSET; 

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
                            Name = filenameWithoutExt,
                            Geometry = new SharpKml.Dom.Point()
                            {
                                Coordinate = new Vector(picInfo.Lat, picInfo.Lon, picInfo.RelAlt)
                            },
                            Description = new Description()
                            {
                                Text = "<table><tr><td><img src=\"" + filename.ToLower() + "\" width=500 /></td></tr></table>"
                            },
                            StyleSelector = new Style()
                            {
                                Balloon = new BalloonStyle() { Text = "$[name]<br>$[description]" }
                            }
                        }
                    );

                    double lat = picInfo.Lat;
                    double lng = picInfo.Lon;
                    double alpha = picInfo.Yaw + (double)num_camerarotation.Value;;

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



                    swloctxt.WriteLine(filename + " " + picInfo.Lat + " " + picInfo.Lon + " " + picInfo.getAltitude(useAMSLAlt) + " " + picInfo.Yaw + " " + picInfo.Pitch + " " + picInfo.Roll);


                    swloctel.WriteLine(filename + "\t" + picInfo.Time.ToString("yyyy:MM:dd HH:mm:ss") + "\t" + picInfo.Lon + "\t" + picInfo.Lat + "\t" + picInfo.getAltitude(useAMSLAlt));
                    swloctel.Flush();
                    swloctxt.Flush();

                    lastRecordN = GenPhotoStationRecord(swloctrim, picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.getAltitude(useAMSLAlt), 0, 0, picInfo.Yaw, picInfo.Width, picInfo.Height, lastRecordN);

                    log.InfoFormat(filename + " " + picInfo.Lon + " " + picInfo.Lat + " " + picInfo.getAltitude(useAMSLAlt) + "           ");
                }

                Serializer serializer = new Serializer();
                serializer.Serialize(kml);
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

        private VehicleLocation LookForLocation(DateTime t, Dictionary<long, VehicleLocation> listLocations)
        {
            long time = ToMilliseconds(t);

            // Time at which the GPS position is actually search and found
            long actualTime = time; 
            int millisSTEP = 1; 

            // 2 seconds (2000 ms) in the log as absolute maximum
            int maxIteration = 2000; 

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
            Dictionary<string, PictureInformation> picturesInformationTemp = new Dictionary<string, PictureInformation>();

            //logFile = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM\2011-10-01 11-48 1.log";
            TXT_outputlog.AppendText("Reading log for GPS-ATT Messages\n");

            // Read Vehicle Locations from log. GPS Messages. Will have to do it anyway
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
                vehicleLocations = readGPSMsgInLog(logFile);

            if (vehicleLocations == null)
            {
                TXT_outputlog.AppendText("Log file problem. Aborting....\n");
                return null;
            }

            //dirWithImages = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM";

            TXT_outputlog.AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            TXT_outputlog.AppendText("Images read : " + files.Length + "\n");

            // Check that we have at least one picture
            if (files.Length <= 0)
            {
                TXT_outputlog.AppendText("Not enought files found.  Aborting..... \n");
                return null;
            }

            Array.Sort(files, Comparer.DefaultInvariant);

            // Each file corresponds to one CAM message
            // We assume that picture names are in ascending order in time
            for (int i = 0; i < files.Length; i++)
            {
                string filename = files[i];

                PictureInformation p = new PictureInformation();

                // Fill shot time in Picture
                p.ShotTimeReportedByCamera = getPhotoTime(filename);

                // Lookfor corresponding Location in vehicleLocationList
                DateTime correctedTime = p.ShotTimeReportedByCamera.AddSeconds(-offset);
                VehicleLocation shotLocation = LookForLocation(correctedTime, vehicleLocations);

                if (shotLocation == null)
                {
                    TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(filename) + " NOT PROCESSED. No GPS match in the log file. Please take care\n");
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

                    p.Time = shotLocation.Time;

                    p.Path = filename;


                    picturesInformationTemp.Add(filename, p);

                    TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(filename) + " PROCESSED with GPS position found " + (shotLocation.Time - correctedTime).Milliseconds + " ms away\n");
                }

            }

            return picturesInformationTemp;
        }


        private void GuessImageDimensions(string imagePath, out int imageWidth, out int imageHeight)
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(imagePath));

            Image picture = Image.FromStream(ms);

            imageHeight = picture.Height;
            imageWidth = picture.Width;
        }

        public Dictionary<string, PictureInformation> doworkCAM(string logFile, string dirWithImages)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp = new Dictionary<string,PictureInformation>();

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
            }


            //logFile = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM\2011-10-01 11-48 1.log";
            TXT_outputlog.AppendText("Reading log for CAM Messages\n");

            List<string[]> list = readCAMMsgInLog(logFile);

            if (list == null)
            {
                TXT_outputlog.AppendText("Log file problem. Aborting....\n");
                return null;
            }

            TXT_outputlog.AppendText("Log Read with - " + list.Count + " - CAM Messages found\n");

            //dirWithImages = @"C:\Users\hog\Pictures\farm 1-10-2011\100SSCAM";

            TXT_outputlog.AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            TXT_outputlog.AppendText("Images read : "  + files.Length + "\n");

            // Check that we have same number of CAMs than files
            if (files.Length != list.Count)
            {
                TXT_outputlog.AppendText("CAM Msgs and Files discrepancy. Check it!  Aborting..... \n");
                return null;
            }

            Array.Sort(files, Comparer.DefaultInvariant);

            // Each file corresponds to one CAM message
            // We assume that picture names are in ascending order in time
            for (int i = 0; i < list.Count; i++)
            {
                string[] currentCAM = list[i];

                PictureInformation p = new PictureInformation();

                DateTime dCAMMsgTime = GetTimeFromGps(int.Parse(getValueFromStringArray(currentCAM, weekCAMPos), CultureInfo.InvariantCulture), int.Parse(getValueFromStringArray(currentCAM, timeCAMpos), CultureInfo.InvariantCulture));

                if (millisShutterLag == 0)
                {
                    // Lets puts GPS time
                    p.Time = dCAMMsgTime;

                    p.Lat = double.Parse(getValueFromStringArray(currentCAM, latCAMpos), CultureInfo.InvariantCulture);
                    p.Lon = double.Parse(getValueFromStringArray(currentCAM, lngCAMpos), CultureInfo.InvariantCulture);
                    p.AltAMSL = double.Parse(getValueFromStringArray(currentCAM, altCAMpos), CultureInfo.InvariantCulture);
                    p.RelAlt = double.Parse(getValueFromStringArray(currentCAM, altCAMpos), CultureInfo.InvariantCulture);

                    VehicleLocation cameraLocationFromGPSMsg = null;

                    string logAltMsg = "RelAlt";

                    if (useAMSLAlt)
                    {
                        cameraLocationFromGPSMsg = LookForLocation(p.Time, vehicleLocations);
                        if (cameraLocationFromGPSMsg != null)
                        {
                            logAltMsg = "AMSL Alt " + (cameraLocationFromGPSMsg.Time - p.Time).Milliseconds + " ms away";
                            p.AltAMSL = cameraLocationFromGPSMsg.AltAMSL;
                        }
                        else
                            logAltMsg = "AMSL Alt NOT found";
                    }


                    p.Pitch = float.Parse(getValueFromStringArray(currentCAM, pitchCAMATT), CultureInfo.InvariantCulture);
                    p.Roll = float.Parse(getValueFromStringArray(currentCAM, rollCAMATT), CultureInfo.InvariantCulture);
                    p.Yaw = float.Parse(getValueFromStringArray(currentCAM, yawCAMATT), CultureInfo.InvariantCulture);

                    p.Path = files[i];

                    string picturePath = files[i];

                    picturesInformationTemp.Add(picturePath, p);

                    TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(picturePath) + " processed from CAM Msg with " + millisShutterLag +  " ms shutter lag. " + logAltMsg + "\n");

                }
                else
                {
                    // Look fot GPS Message ahead
                    DateTime dCorrectedWithLagPhotoTime = dCAMMsgTime;
                    dCorrectedWithLagPhotoTime = dCorrectedWithLagPhotoTime.AddMilliseconds(millisShutterLag);

                    VehicleLocation cameraLocationFromGPSMsg = LookForLocation(dCorrectedWithLagPhotoTime, vehicleLocations);


                    // Check which GPS Position is closer in time.
                    if (cameraLocationFromGPSMsg != null)
                    {
                        System.TimeSpan diffGPSTimeCAMTime = cameraLocationFromGPSMsg.Time - dCAMMsgTime;

                        if (diffGPSTimeCAMTime.Milliseconds > 2 * millisShutterLag)
                        {
                            // Stay with CAM Message as it is closer to CorrectedTime
                            p.Time = dCAMMsgTime;

                            p.Lat = double.Parse(getValueFromStringArray(currentCAM, latCAMpos), CultureInfo.InvariantCulture);
                            p.Lon = double.Parse(getValueFromStringArray(currentCAM, lngCAMpos), CultureInfo.InvariantCulture);
                            p.AltAMSL = double.Parse(getValueFromStringArray(currentCAM, altCAMpos), CultureInfo.InvariantCulture);
                            p.RelAlt = double.Parse(getValueFromStringArray(currentCAM, altCAMpos), CultureInfo.InvariantCulture);

                            string logAltMsg = "RelAlt";

                            cameraLocationFromGPSMsg = null;
                            if (useAMSLAlt)
                            {
                                cameraLocationFromGPSMsg = LookForLocation(p.Time, vehicleLocations);
                                if (cameraLocationFromGPSMsg != null)
                                {
                                    logAltMsg = "AMSL Alt " + (cameraLocationFromGPSMsg.Time - p.Time).Milliseconds + " ms away";
                                    p.AltAMSL = cameraLocationFromGPSMsg.AltAMSL;
                                }
                                else
                                    logAltMsg = "AMSL Alt NOT found";
                            }


                            TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) + " processed with CAM Msg. Shutter lag too small. "  + logAltMsg + "\n");
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

                            TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) + " processed with GPS Msg : " + diffGPSTimeCAMTime.Milliseconds + " ms ahead of CAM Msg. " + logAltMsg + "\n");

                        }


                        p.Pitch = float.Parse(getValueFromStringArray(currentCAM, pitchCAMATT), CultureInfo.InvariantCulture);
                        p.Roll = float.Parse(getValueFromStringArray(currentCAM, rollCAMATT), CultureInfo.InvariantCulture);
                        p.Yaw = float.Parse(getValueFromStringArray(currentCAM, yawCAMATT), CultureInfo.InvariantCulture);

                        p.Path = files[i];

                        string picturePath = files[i];

                        picturesInformationTemp.Add(picturePath, p);

                        

                    }
                    else
                    {
                        TXT_outputlog.AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) + " NOT Processed. Time not found in log. Too large Shutter Lag? Try setting it to 0\n");
                    }


                }

            }

            return picturesInformationTemp;
        }

        void GenFlightMission(XmlTextWriter swloctrim, int lastRecordN)
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

        int GenPhotoStationRecord(XmlTextWriter swloctrim, string imgname, double lat, double lng, double alt, double roll, double pitch, double yaw, int imgwidth, int imgheight, int lastRecordN)
        {
            Console.WriteLine("yaw {0}",yaw);

            int photoStationID = lastRecordN++;
            int pointRecordID = lastRecordN++;
            int imageRecordID = lastRecordN++;

            JXL_StationIDs.Add(photoStationID);

            // conver tto rads
            yaw = -yaw * deg2rad;

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

        private void writeGPX(string filename, Dictionary<string, PictureInformation> pictureList)
        {

            using (System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(filename) + ".gpx", Encoding.ASCII))
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
            this.BUT_Geotagimages = new MissionPlanner.Controls.MyButton();
            this.BUT_estoffset = new MissionPlanner.Controls.MyButton();
            this.BUT_doit = new MissionPlanner.Controls.MyButton();
            this.BUT_browsedir = new MissionPlanner.Controls.MyButton();
            this.BUT_browselog = new MissionPlanner.Controls.MyButton();
            this.NUM_latpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_lngpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_altpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_ATT_Heading = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BUT_networklinkgeoref = new MissionPlanner.Controls.MyButton();
            this.NUM_time = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.RDIO_TimeOffset = new System.Windows.Forms.RadioButton();
            this.RDIO_CAMMsgSynchro = new System.Windows.Forms.RadioButton();
            this.PANEL_TIME_OFFSET = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NUM_CAM_Alt = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.NUM_CAM_Lon = new System.Windows.Forms.NumericUpDown();
            this.NUM_CAM_Lat = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.PANEL_CAM = new System.Windows.Forms.Panel();
            this.NUM_CAM_Week = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.NUM_ATT_Roll = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.NUM_ATT_Pitch = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.NUM_GPS_Week = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NUM_CAM_Time = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.NUM_GPS_AMSL_Alt = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.NUM_CAM_Pitch = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.NUM_CAM_Roll = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.NUM_CAM_Heading = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.num_camerarotation = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.num_hfov = new System.Windows.Forms.NumericUpDown();
            this.num_vfov = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.CHECK_AMSLAlt_Use = new System.Windows.Forms.CheckBox();
            this.PANEL_SHUTTER_LAG = new System.Windows.Forms.Panel();
            this.TXT_shutterLag = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Heading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_time)).BeginInit();
            this.PANEL_TIME_OFFSET.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Alt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lat)).BeginInit();
            this.PANEL_CAM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Roll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Pitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_Week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_AMSL_Alt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Pitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Roll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Heading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).BeginInit();
            this.panel3.SuspendLayout();
            this.PANEL_SHUTTER_LAG.SuspendLayout();
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
            6,
            0,
            0,
            0});
            this.NUM_latpos.ValueChanged += new System.EventHandler(this.NUM_latpos_ValueChanged);
            // 
            // NUM_lngpos
            // 
            resources.ApplyResources(this.NUM_lngpos, "NUM_lngpos");
            this.NUM_lngpos.Name = "NUM_lngpos";
            this.NUM_lngpos.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUM_lngpos.ValueChanged += new System.EventHandler(this.NUM_lngpos_ValueChanged);
            // 
            // NUM_altpos
            // 
            resources.ApplyResources(this.NUM_altpos, "NUM_altpos");
            this.NUM_altpos.Name = "NUM_altpos";
            this.NUM_altpos.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.NUM_altpos.ValueChanged += new System.EventHandler(this.NUM_altpos_ValueChanged);
            // 
            // NUM_ATT_Heading
            // 
            resources.ApplyResources(this.NUM_ATT_Heading, "NUM_ATT_Heading");
            this.NUM_ATT_Heading.Name = "NUM_ATT_Heading";
            this.NUM_ATT_Heading.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Heading.ValueChanged += new System.EventHandler(this.NUM_ATT_Heading_ValueChanged);
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
            // BUT_networklinkgeoref
            // 
            resources.ApplyResources(this.BUT_networklinkgeoref, "BUT_networklinkgeoref");
            this.BUT_networklinkgeoref.Name = "BUT_networklinkgeoref";
            this.BUT_networklinkgeoref.UseVisualStyleBackColor = true;
            this.BUT_networklinkgeoref.Click += new System.EventHandler(this.BUT_networklinkgeoref_Click);
            // 
            // NUM_time
            // 
            resources.ApplyResources(this.NUM_time, "NUM_time");
            this.NUM_time.Name = "NUM_time";
            this.NUM_time.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUM_time.ValueChanged += new System.EventHandler(this.NUM_time_ValueChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // RDIO_TimeOffset
            // 
            resources.ApplyResources(this.RDIO_TimeOffset, "RDIO_TimeOffset");
            this.RDIO_TimeOffset.Name = "RDIO_TimeOffset";
            this.RDIO_TimeOffset.UseVisualStyleBackColor = true;
            this.RDIO_TimeOffset.CheckedChanged += new System.EventHandler(this.ProcessType_CheckedChanged);
            // 
            // RDIO_CAMMsgSynchro
            // 
            resources.ApplyResources(this.RDIO_CAMMsgSynchro, "RDIO_CAMMsgSynchro");
            this.RDIO_CAMMsgSynchro.Checked = true;
            this.RDIO_CAMMsgSynchro.Name = "RDIO_CAMMsgSynchro";
            this.RDIO_CAMMsgSynchro.TabStop = true;
            this.RDIO_CAMMsgSynchro.UseVisualStyleBackColor = true;
            this.RDIO_CAMMsgSynchro.CheckedChanged += new System.EventHandler(this.ProcessType_CheckedChanged);
            // 
            // PANEL_TIME_OFFSET
            // 
            this.PANEL_TIME_OFFSET.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_TIME_OFFSET.Controls.Add(this.BUT_estoffset);
            this.PANEL_TIME_OFFSET.Controls.Add(this.TXT_offsetseconds);
            this.PANEL_TIME_OFFSET.Controls.Add(this.label1);
            resources.ApplyResources(this.PANEL_TIME_OFFSET, "PANEL_TIME_OFFSET");
            this.PANEL_TIME_OFFSET.Name = "PANEL_TIME_OFFSET";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // NUM_CAM_Alt
            // 
            resources.ApplyResources(this.NUM_CAM_Alt, "NUM_CAM_Alt");
            this.NUM_CAM_Alt.Name = "NUM_CAM_Alt";
            this.NUM_CAM_Alt.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUM_CAM_Alt.ValueChanged += new System.EventHandler(this.NUM_CAM_Alt_ValueChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // NUM_CAM_Lon
            // 
            resources.ApplyResources(this.NUM_CAM_Lon, "NUM_CAM_Lon");
            this.NUM_CAM_Lon.Name = "NUM_CAM_Lon";
            this.NUM_CAM_Lon.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.NUM_CAM_Lon.ValueChanged += new System.EventHandler(this.NUM_CAM_Lon_ValueChanged);
            // 
            // NUM_CAM_Lat
            // 
            resources.ApplyResources(this.NUM_CAM_Lat, "NUM_CAM_Lat");
            this.NUM_CAM_Lat.Name = "NUM_CAM_Lat";
            this.NUM_CAM_Lat.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUM_CAM_Lat.ValueChanged += new System.EventHandler(this.NUM_CAM_Lat_ValueChanged);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // PANEL_CAM
            // 
            this.PANEL_CAM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Week);
            this.PANEL_CAM.Controls.Add(this.label26);
            this.PANEL_CAM.Controls.Add(this.panel2);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Roll);
            this.PANEL_CAM.Controls.Add(this.label25);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Pitch);
            this.PANEL_CAM.Controls.Add(this.label24);
            this.PANEL_CAM.Controls.Add(this.label6);
            this.PANEL_CAM.Controls.Add(this.label23);
            this.PANEL_CAM.Controls.Add(this.NUM_GPS_Week);
            this.PANEL_CAM.Controls.Add(this.panel1);
            this.PANEL_CAM.Controls.Add(this.NUM_latpos);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Time);
            this.PANEL_CAM.Controls.Add(this.NUM_lngpos);
            this.PANEL_CAM.Controls.Add(this.label22);
            this.PANEL_CAM.Controls.Add(this.label10);
            this.PANEL_CAM.Controls.Add(this.label21);
            this.PANEL_CAM.Controls.Add(this.NUM_altpos);
            this.PANEL_CAM.Controls.Add(this.NUM_GPS_AMSL_Alt);
            this.PANEL_CAM.Controls.Add(this.NUM_time);
            this.PANEL_CAM.Controls.Add(this.label20);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Heading);
            this.PANEL_CAM.Controls.Add(this.label2);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Pitch);
            this.PANEL_CAM.Controls.Add(this.label3);
            this.PANEL_CAM.Controls.Add(this.label19);
            this.PANEL_CAM.Controls.Add(this.label4);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Roll);
            this.PANEL_CAM.Controls.Add(this.label5);
            this.PANEL_CAM.Controls.Add(this.label18);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Heading);
            this.PANEL_CAM.Controls.Add(this.label17);
            this.PANEL_CAM.Controls.Add(this.label16);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Lat);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Lon);
            this.PANEL_CAM.Controls.Add(this.label15);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Alt);
            this.PANEL_CAM.Controls.Add(this.label14);
            this.PANEL_CAM.Controls.Add(this.label13);
            resources.ApplyResources(this.PANEL_CAM, "PANEL_CAM");
            this.PANEL_CAM.Name = "PANEL_CAM";
            // 
            // NUM_CAM_Week
            // 
            resources.ApplyResources(this.NUM_CAM_Week, "NUM_CAM_Week");
            this.NUM_CAM_Week.Name = "NUM_CAM_Week";
            this.NUM_CAM_Week.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUM_CAM_Week.ValueChanged += new System.EventHandler(this.NUM_CAM_Week_ValueChanged);
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // NUM_ATT_Roll
            // 
            resources.ApplyResources(this.NUM_ATT_Roll, "NUM_ATT_Roll");
            this.NUM_ATT_Roll.Name = "NUM_ATT_Roll";
            this.NUM_ATT_Roll.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Roll.ValueChanged += new System.EventHandler(this.NUM_ATT_Roll_ValueChanged);
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // NUM_ATT_Pitch
            // 
            resources.ApplyResources(this.NUM_ATT_Pitch, "NUM_ATT_Pitch");
            this.NUM_ATT_Pitch.Name = "NUM_ATT_Pitch";
            this.NUM_ATT_Pitch.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Pitch.ValueChanged += new System.EventHandler(this.NUM_ATT_Pitch_ValueChanged);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // NUM_GPS_Week
            // 
            resources.ApplyResources(this.NUM_GPS_Week, "NUM_GPS_Week");
            this.NUM_GPS_Week.Name = "NUM_GPS_Week";
            this.NUM_GPS_Week.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUM_GPS_Week.ValueChanged += new System.EventHandler(this.NUM_GPS_Week_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // NUM_CAM_Time
            // 
            resources.ApplyResources(this.NUM_CAM_Time, "NUM_CAM_Time");
            this.NUM_CAM_Time.Name = "NUM_CAM_Time";
            this.NUM_CAM_Time.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_CAM_Time.ValueChanged += new System.EventHandler(this.NUM_CAM_Time_ValueChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // NUM_GPS_AMSL_Alt
            // 
            resources.ApplyResources(this.NUM_GPS_AMSL_Alt, "NUM_GPS_AMSL_Alt");
            this.NUM_GPS_AMSL_Alt.Name = "NUM_GPS_AMSL_Alt";
            this.NUM_GPS_AMSL_Alt.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.NUM_GPS_AMSL_Alt.ValueChanged += new System.EventHandler(this.NUM_GPS_AMSL_Alt_ValueChanged);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // NUM_CAM_Pitch
            // 
            resources.ApplyResources(this.NUM_CAM_Pitch, "NUM_CAM_Pitch");
            this.NUM_CAM_Pitch.Name = "NUM_CAM_Pitch";
            this.NUM_CAM_Pitch.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.NUM_CAM_Pitch.ValueChanged += new System.EventHandler(this.NUM_CAM_Pitch_ValueChanged);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // NUM_CAM_Roll
            // 
            resources.ApplyResources(this.NUM_CAM_Roll, "NUM_CAM_Roll");
            this.NUM_CAM_Roll.Name = "NUM_CAM_Roll";
            this.NUM_CAM_Roll.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUM_CAM_Roll.ValueChanged += new System.EventHandler(this.NUM_CAM_Roll_ValueChanged);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // NUM_CAM_Heading
            // 
            resources.ApplyResources(this.NUM_CAM_Heading, "NUM_CAM_Heading");
            this.NUM_CAM_Heading.Name = "NUM_CAM_Heading";
            this.NUM_CAM_Heading.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUM_CAM_Heading.ValueChanged += new System.EventHandler(this.NUM_CAM_Heading_ValueChanged);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
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
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
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
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.CHECK_AMSLAlt_Use);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.num_vfov);
            this.panel3.Controls.Add(this.num_camerarotation);
            this.panel3.Controls.Add(this.num_hfov);
            this.panel3.Controls.Add(this.label7);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // CHECK_AMSLAlt_Use
            // 
            resources.ApplyResources(this.CHECK_AMSLAlt_Use, "CHECK_AMSLAlt_Use");
            this.CHECK_AMSLAlt_Use.Checked = true;
            this.CHECK_AMSLAlt_Use.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHECK_AMSLAlt_Use.Name = "CHECK_AMSLAlt_Use";
            this.CHECK_AMSLAlt_Use.UseVisualStyleBackColor = true;
            this.CHECK_AMSLAlt_Use.CheckedChanged += new System.EventHandler(this.CHECK_AMSLAlt_Use_CheckedChanged);
            // 
            // PANEL_SHUTTER_LAG
            // 
            this.PANEL_SHUTTER_LAG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_SHUTTER_LAG.Controls.Add(this.TXT_shutterLag);
            this.PANEL_SHUTTER_LAG.Controls.Add(this.label27);
            resources.ApplyResources(this.PANEL_SHUTTER_LAG, "PANEL_SHUTTER_LAG");
            this.PANEL_SHUTTER_LAG.Name = "PANEL_SHUTTER_LAG";
            // 
            // TXT_shutterLag
            // 
            resources.ApplyResources(this.TXT_shutterLag, "TXT_shutterLag");
            this.TXT_shutterLag.Name = "TXT_shutterLag";
            this.TXT_shutterLag.TextChanged += new System.EventHandler(this.TXT_shutterLag_TextChanged);
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // Georefimage
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.PANEL_SHUTTER_LAG);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.PANEL_CAM);
            this.Controls.Add(this.PANEL_TIME_OFFSET);
            this.Controls.Add(this.RDIO_CAMMsgSynchro);
            this.Controls.Add(this.RDIO_TimeOffset);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.BUT_networklinkgeoref);
            this.Controls.Add(this.BUT_Geotagimages);
            this.Controls.Add(this.TXT_outputlog);
            this.Controls.Add(this.BUT_doit);
            this.Controls.Add(this.TXT_jpgdir);
            this.Controls.Add(this.TXT_logfile);
            this.Controls.Add(this.BUT_browsedir);
            this.Controls.Add(this.BUT_browselog);
            this.Name = "Georefimage";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Heading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_time)).EndInit();
            this.PANEL_TIME_OFFSET.ResumeLayout(false);
            this.PANEL_TIME_OFFSET.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Alt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lat)).EndInit();
            this.PANEL_CAM.ResumeLayout(false);
            this.PANEL_CAM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Roll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Pitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_Week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_AMSL_Alt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Pitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Roll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Heading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.PANEL_SHUTTER_LAG.ResumeLayout(false);
            this.PANEL_SHUTTER_LAG.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void BUT_browselog_Click(object sender, EventArgs e)
        {
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
            try
            {
                folderBrowserDialog1.SelectedPath = Path.GetDirectoryName(TXT_logfile.Text);
            }
            catch { }

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
                    catch { }                    
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
                if (float.TryParse(TXT_offsetseconds.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds) == false)
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
                        {
                            picturesInfo = doworkCAM(logFilePath, dirPictures);
                            if (picturesInfo != null)
                                CreateReportFiles(picturesInfo, dirPictures, seconds);
                            break;
                        }
                }
            }
            catch (Exception ex) { TXT_outputlog.AppendText("Error " + ex.ToString()); }
            
            BUT_doit.Enabled = true;
            BUT_Geotagimages.Enabled = true;
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

            foreach (PictureInformation picInfo in picturesInfo.Values)
            {
                WriteCoordinatesToImage(picInfo.Path, picInfo.Lat, picInfo.Lon, picInfo.AltAMSL);
            }

            TXT_outputlog.AppendText("GeoTagging FINISHED \n\n");
            

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
                TXT_outputlog.AppendText("GeoTagging "+ Filename + "\n");
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

                    // Save file into Geotag folder
                    string rootFolder = TXT_jpgdir.Text;
                    string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

                    string outputfilename = geoTagFolder + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Filename) + "_geotag" + Path.GetExtension(Filename);

                    // Just in case
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
            else
            {
                selectedProcessingMode = PROCESSING_MODE.TIME_OFFSET;
                PANEL_TIME_OFFSET.Enabled = true;
                PANEL_SHUTTER_LAG.Enabled = false;
            }
        }

        #region GraphicalSetterEvents
        private void TXT_shutterLag_TextChanged(object sender, EventArgs e)
        {
            bool convertedOK = int.TryParse(TXT_shutterLag.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out millisShutterLag);

            if (!convertedOK)
                TXT_shutterLag.Text = "0";
        }
        private void CHECK_AMSLAlt_Use_CheckedChanged(object sender, EventArgs e)
        {
            useAMSLAlt = ((CheckBox)sender).Checked;
        }

        private void NUM_CAM_Time_ValueChanged(object sender, EventArgs e)
        {
            timeCAMpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Lat_ValueChanged(object sender, EventArgs e)
        {
            latCAMpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Lon_ValueChanged(object sender, EventArgs e)
        {
            lngCAMpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Alt_ValueChanged(object sender, EventArgs e)
        {
            altCAMpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Heading_ValueChanged(object sender, EventArgs e)
        {
            yawCAMATT = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Roll_ValueChanged(object sender, EventArgs e)
        {
            rollCAMATT = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_CAM_Pitch_ValueChanged(object sender, EventArgs e)
        {
            pitchCAMATT = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_time_ValueChanged(object sender, EventArgs e)
        {
            timepos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_GPS_Week_ValueChanged(object sender, EventArgs e)
        {
            gpsweekpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_GPS_AMSL_Alt_ValueChanged(object sender, EventArgs e)
        {
            altAMSLpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_latpos_ValueChanged(object sender, EventArgs e)
        {
            latpos  = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_lngpos_ValueChanged(object sender, EventArgs e)
        {
            lngpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_altpos_ValueChanged(object sender, EventArgs e)
        {
            altpos = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_ATT_Heading_ValueChanged(object sender, EventArgs e)
        {
            yawATT  = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_ATT_Roll_ValueChanged(object sender, EventArgs e)
        {
            rollATT = (int)((NumericUpDown)sender).Value;
        }

        private void NUM_ATT_Pitch_ValueChanged(object sender, EventArgs e)
        {
            pitchATT = (int)((NumericUpDown)sender).Value;
        }
       

        private void NUM_CAM_Week_ValueChanged(object sender, EventArgs e)
        {
            weekCAMPos = (int)((NumericUpDown)sender).Value;
        }
        #endregion


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
