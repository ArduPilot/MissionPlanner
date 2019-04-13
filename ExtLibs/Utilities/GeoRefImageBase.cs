using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using com.drew.imaging.jpg;
using com.drew.imaging.tiff;
using com.drew.metadata;
using ExifLibrary;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Drawing;
using SharpKml.Base;
using SharpKml.Dom;

namespace MissionPlanner.GeoRef
{
    public enum PROCESSING_MODE
    {
        TIME_OFFSET,
        CAM_MSG, // via digital feedback
        TRIG // camera was triggered
    }

    public class GeoRefImageBase
    {
        public const string PHOTO_FILES_FILTER = "*.jpg;*.tif";
        public const int JXL_ID_OFFSET = 10;
        public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Dictionary<string, PictureInformation> picturesInfo = new Dictionary<string, PictureInformation>();
        public Dictionary<long, VehicleLocation> vehicleLocations = new Dictionary<long, VehicleLocation>();
        public Hashtable filedatecache = new Hashtable();
        public List<int> JXL_StationIDs = new List<int>();
        public bool useAMSLAlt;
        public int millisShutterLag = 0;

        /// <summary>
        /// Get a photos shutter time
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        protected DateTime getPhotoTime(string fn)
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

        /// <summary>
        /// Return a list of all gps messages with there timestamp from the log
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        public Dictionary<long, VehicleLocation> readGPSMsgInLog(string fn, string gpstouse = "GPS")
        {
            Dictionary<long, VehicleLocation> vehiclePositionList = new Dictionary<long, VehicleLocation>();

            // Telemetry Log
            if (fn.ToLower().EndsWith("tlog"))
            {
                using (CommsFile file = new CommsFile(fn))
                {
                    VehicleLocation location = new VehicleLocation();

                    foreach (var packet in file.GetMessageOfType(new[]
                    {
                        MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT,
                        MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT,
                        MAVLink.MAVLINK_MSG_ID.RANGEFINDER,
                        MAVLink.MAVLINK_MSG_ID.ATTITUDE
                    }, true))
                    {
                        var gps = packet.data as MAVLink.mavlink_gps_raw_int_t?;
                        if (gps.HasValue)
                        {
                            if (gps.Value.fix_type <= 2)
                                continue;
                        }

                        var range = packet.data as MAVLink.mavlink_rangefinder_t?;
                        if (range.HasValue)
                        {
                            location.SAlt = range.Value.distance;
                        }

                        var att = packet.data as MAVLink.mavlink_attitude_t?;
                        if (att.HasValue)
                        {
                            location.Roll = (float)degrees(att.Value.roll);
                            location.Pitch = (float)degrees(att.Value.pitch);
                            location.Yaw = (float)degrees(att.Value.yaw);
                        }

                        var pos = packet.data as MAVLink.mavlink_global_position_int_t?;
                        if (pos.HasValue)
                        {
                            location.Time = packet.rxtime;
                            location.Lat = pos.Value.lat / 1e7;
                            location.Lon = pos.Value.lon / 1e7;
                            location.RelAlt = pos.Value.relative_alt / 1000.0f;
                            location.AltAMSL = pos.Value.alt / 1000.0f;

                            vehiclePositionList[ToMilliseconds(location.Time)] = (VehicleLocation)location.Clone();
                        }
                    }
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
                                if (altindex != -1)
                                    location.GPSAlt = double.Parse(item.items[altindex], CultureInfo.InvariantCulture);

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
                                Console.WriteLine("Bad " + gpstouse + " Line");
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
        protected Dictionary<long, VehicleLocation> readCAMMsgInLog(string fn)
        {
            Dictionary<long, VehicleLocation> list = new Dictionary<long, VehicleLocation>();

            // Telemetry Log
            if (fn.ToLower().EndsWith("tlog"))
            {
                //TXT_outputlog.AppendText("Warning: tlogs are not fully supported when using CAM Messages\n");

                using (CommsFile file = new CommsFile(fn))
                {
                    VehicleLocation location = new VehicleLocation();

                    foreach (var packet in file.GetMessageOfType(new[]
                    {
                        MAVLink.MAVLINK_MSG_ID.RANGEFINDER,
                        MAVLink.MAVLINK_MSG_ID.CAMERA_FEEDBACK
                    }, true))
                    {
                        var range = packet.data as MAVLink.mavlink_rangefinder_t?;
                        if (range.HasValue)
                        {
                            location.SAlt = range.Value.distance;
                        }

                        if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_FEEDBACK)
                        {
                            var msg = (MAVLink.mavlink_camera_feedback_t)packet.data;

                            location.Time = FromUTCTimeMilliseconds((long)(msg.time_usec / 1000));
                            location.Lat = msg.lat / 1e7;
                            location.Lon = msg.lng / 1e7;
                            location.RelAlt = msg.alt_rel;
                            location.AltAMSL = msg.alt_msl;

                            location.Roll = msg.roll;
                            location.Pitch = msg.pitch;
                            location.Yaw = msg.yaw;

                            list[ToMilliseconds(location.Time)] = (VehicleLocation)location.Clone();
                        }
                    }
                }
            }
            // DataFlash Log
            else
            {
                float currentSAlt = 0;
                using (var sr = new CollectionBuffer(File.OpenRead(fn)))
                {
                    //FMT, 146, 43, CAM, QIHLLeeeccC, TimeUS,GPSTime,GPSWeek,Lat,Lng,Alt,RelAlt,GPSAlt,Roll,Pitch,Yaw
                    //FMT, 198, 17, RFND, QCBCB, TimeUS,Dist1,Orient1,Dist2,Orient2
                    foreach (var item in sr.GetEnumeratorType(new string[] { "CAM", "RFND" }))
                    {
                        if (item.msgtype == "CAM")
                        {
                            int latindex = sr.dflog.FindMessageOffset("CAM", "Lat");
                            int lngindex = sr.dflog.FindMessageOffset("CAM", "Lng");
                            int altindex = sr.dflog.FindMessageOffset("CAM", "Alt");
                            int raltindex = sr.dflog.FindMessageOffset("CAM", "RelAlt");
                            int galtindex = sr.dflog.FindMessageOffset("CAM", "GPSAlt");

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
                            if (galtindex != -1)
                                p.GPSAlt = double.Parse(item.items[galtindex], CultureInfo.InvariantCulture);

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

        public Dictionary<long, VehicleLocation> readTRIGMsgInLog(string fn)
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
                        int galtindex = sr.dflog.FindMessageOffset("TRIG", "GPSAlt");

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
                        if (galtindex != -1)
                            p.GPSAlt = double.Parse(item.items[galtindex], CultureInfo.InvariantCulture);

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
            DateTime week = datum.AddDays(weeknumber * 7);
            DateTime time = week.AddMilliseconds(milliseconds);

            return time.AddSeconds(-LEAP_SECONDS);
        }

        public long ToMilliseconds(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        public double EstimateOffset(string logFile, string dirWithImages, string UseGpsorGPS2, bool usecam, Action<string> AppendText)
        {
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
            {
                if (usecam)
                {
                    vehicleLocations = readCAMMsgInLog(logFile);
                }
                else
                {
                    vehicleLocations = readGPSMsgInLog(logFile, UseGpsorGPS2);
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

            for (int a = 0; a < 4; a++)
            {
                // First Photo time
                string firstPhoto = files[a];

                DateTime photoTime = getPhotoTime(firstPhoto);

                AppendText((a + 1) + " Picture " + Path.GetFileNameWithoutExtension(firstPhoto) +
                           " with DateTime: " + photoTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

                // First GPS Message in Log time
                List<long> times = new List<long>(vehicleLocations.Keys);
                times.Sort();
                long firstTimeInGPSMsg = times[a];
                DateTime logTime = FromUTCTimeMilliseconds(firstTimeInGPSMsg);

                AppendText((a + 1) + " GPS Log Msg: " + logTime.ToString("yyyy:MM:dd HH:mm:ss") + "\n");

                AppendText((a + 1) + " Est: " + (double) (photoTime - logTime).TotalSeconds + "\n");

                if (ans == 0)
                    ans = (double) (photoTime - logTime).TotalSeconds;
                else
                    ans = ans*0.5 + (photoTime - logTime).TotalSeconds*0.5;
            }

            return ans;
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

        public Dictionary<string, PictureInformation> doworkGPSOFFSET(string logFile, string dirWithImages, float offset, string UseGpsorGPS2, bool usecam, Action<string> AppendText)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            // Read Vehicle Locations from log. GPS Messages. Will have to do it anyway
            if (vehicleLocations == null || vehicleLocations.Count <= 0)
            {
                if (usecam)
                {
                    AppendText("Reading log for CAM Messages\n");

                    vehicleLocations = readCAMMsgInLog(logFile);
                }
                else
                {
                    AppendText("Reading log for GPS-ATT Messages\n");

                    vehicleLocations = readGPSMsgInLog(logFile, UseGpsorGPS2);
                }
            }

            if (vehicleLocations == null)
            {
                AppendText("Log file problem. Aborting....\n");
                return null;
            }

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(List<VehicleLocation>),
                    new[] {typeof(VehicleLocation)});

                using (var fl = File.OpenWrite(logFile + ".xml"))
                {
                    xmlSerializer.Serialize(fl, vehicleLocations.Values.ToList());
                }
            } catch { }

            AppendText("Log locations : " + vehicleLocations.Count + "\n");

            AppendText("Read images\n");

            List<string> filelist = new List<string>();
            string[] exts = PHOTO_FILES_FILTER.Split(';');
            foreach (var ext in exts)
            {
                filelist.AddRange(Directory.GetFiles(dirWithImages, ext));
            }

            string[] files = filelist.ToArray();

            AppendText("Images read : " + files.Length + "\n");

            // Check that we have at least one picture
            if (files.Length <= 0)
            {
                AppendText("Not enought files found.  Aborting..... \n");
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

        private int compareFileByPhotoTime(string x, string y)
        {
            // compare times
            var ans1 = getPhotoTime(x).CompareTo(getPhotoTime(y));
            // if times are the same, compare filenames
            return ans1 != 0 ? ans1 : x.CompareTo(y);
        }

        public Dictionary<string, PictureInformation> doworkCAM(string logFile, string dirWithImages, string UseGpsorGPS2, Action<string> AppendText)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            AppendText("Using AMSL Altitude " + useAMSLAlt + "\n");

            // If we are required to use AMSL then GPS messages should be used until CAM messages includes AMSL in the coming AC versions
            // Or if the user enter shutter lag and thus we have to look for GPS messages ahead in time
            if (useAMSLAlt || millisShutterLag > 0)
            {
                AppendText("Reading log for GPS Messages in order to get AMSL Altitude\n");
                if (vehicleLocations == null || vehicleLocations.Count <= 0)
                {
                    vehicleLocations = readGPSMsgInLog(logFile, UseGpsorGPS2);

                    if (vehicleLocations == null || vehicleLocations.Count <= 0)
                    {
                        AppendText("Log file problem. Aborting....\n");
                        return null;
                    }
                }
                AppendText("Log Read for GPS Messages\n");
                AppendText("Log locations : " + vehicleLocations.Count + "\n");

            }

            AppendText("Reading log for CAM Messages\n");

            var list = readCAMMsgInLog(logFile);

            if (list == null)
            {
                AppendText("Log file problem. No CAM messages. Aborting....\n");
                return null;
            }

            AppendText("Log Read with - " + list.Count + " - CAM Messages found\n");

            AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            AppendText("Images read : " + files.Length + "\n");

            // Check that we have same number of CAMs than files
            if (files.Length != list.Count)
            {
                AppendText(string.Format("CAM Msgs and Files discrepancy. Check it! files: {0} vs CAM msg: {1}\n",files.Length,list.Count));
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
                    p.GPSAlt = currentCAM.GPSAlt;

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

                    AppendText("Photo " + Path.GetFileNameWithoutExtension(picturePath) +
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
                            p.GPSAlt = currentCAM.GPSAlt;

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


                            AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
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
                            p.GPSAlt = cameraLocationFromGPSMsg.GPSAlt;

                            string logAltMsg = useAMSLAlt ? "AMSL Alt" : "RelAlt";

                            AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
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
                        AppendText("Photo " + Path.GetFileNameWithoutExtension(files[i]) +
                                   " NOT Processed. Time not found in log. Too large Shutter Lag? Try setting it to 0\n");
                    }
                }
            }

            return picturesInformationTemp;
        }

        public Dictionary<string, PictureInformation> doworkTRIG(string logFile, string dirWithImages, string UseGpsorGPS2, Action<string> AppendText)
        {
            // Lets start over 
            Dictionary<string, PictureInformation> picturesInformationTemp =
                new Dictionary<string, PictureInformation>();

            AppendText("Using AMSL Altitude " + useAMSLAlt + "\n");

            AppendText("Reading log for TRIG Messages\n");

            vehicleLocations = readTRIGMsgInLog(logFile);

            if (vehicleLocations == null)
            {
                AppendText("Log file problem. No TRIG messages. Aborting....\n");
                return null;
            }

            AppendText("Log Read with - " + vehicleLocations.Count + " - TRIG Messages found\n");

            AppendText("Read images\n");

            string[] files = Directory.GetFiles(dirWithImages, "*.jpg");

            AppendText("Images read : " + files.Length + "\n");

            // Check that we have same number of CAMs than files
            if (files.Length != vehicleLocations.Count)
            {
                AppendText(string.Format("TRIG Msgs and Files discrepancy. Check it! files: {0} vs TRIG msg: {1}\n", files.Length, vehicleLocations.Count));
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
                p.GPSAlt = currentTRIG.GPSAlt;

                p.Pitch = currentTRIG.Pitch;
                p.Roll = currentTRIG.Roll;
                p.Yaw = currentTRIG.Yaw;

                p.SAlt = currentTRIG.SAlt;

                p.Path = files[i];

                string picturePath = files[i];

                picturesInformationTemp.Add(picturePath, p);

                AppendText("Photo " + Path.GetFileNameWithoutExtension(picturePath) +
                           " processed from CAM Msg with " + millisShutterLag + " ms shutter lag. " +
                           "\n");

            }

            return picturesInformationTemp;
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

        protected void writeGPX(string filename, Dictionary<string, PictureInformation> pictureList)
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

        private string ByteArrayToString(byte[] data)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte b in data)
                s.AppendFormat("0x{0:X2} ", b);
            return s.ToString();
        }

        public void WriteCoordinatesToImage(string Filename, double dLat, double dLong, double alt, string rootFolder, Action<string> AppendText)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(Filename)))
            {
                AppendText("GeoTagging " + Filename + "\n");
                try
                {
                    var data = ImageFile.FromStream(ms);
                    foreach (ExifProperty item in data.Properties.ToArray())
                    {
                        var test = Enum.GetName(typeof(IFD), ExifTagFactory.GetTagIFD(item.Tag));
                        StringBuilder s = new StringBuilder();
                        s.AppendFormat("Tag: {0}{1}", item.Tag, Environment.NewLine);
                        string val = item.ToString();
                        //if (val.Length > 50) val = val.Substring(0, 50) + " ...";
                        s.AppendFormat("Value: {0}{1}", val, Environment.NewLine);
                        s.AppendFormat("IFD: {0}{1}", item.IFD, Environment.NewLine);
                        s.AppendFormat("Interop. TagID: {0} (0x{0:X2}){1}", item.Interoperability.TagID, Environment.NewLine);
                        s.AppendFormat("Interop. Type: {0} ({1}){2}", (ushort)item.Interoperability.TypeID, item.Interoperability.TypeID, Environment.NewLine);
                        s.AppendFormat("Interop. Count: {0} {1}", item.Interoperability.Count, Environment.NewLine);
                        s.AppendFormat("Interop. Data Length: {0}{1}", item.Interoperability.Data.Length, Environment.NewLine);
                        s.AppendFormat("Interop. Data: {0}", ByteArrayToString(item.Interoperability.Data), Environment.NewLine);

                        Console.WriteLine(s);

                        if (item.Tag == ExifTag.GPSLongitude || item.Tag == ExifTag.GPSLatitude ||
                            item.Tag == ExifTag.GPSAltitude || item.Tag == ExifTag.GPSLatitudeRef ||
                            item.Tag == ExifTag.GPSLongitudeRef)
                            data.Properties.Remove(item);

                    }
                   

                    var lon = dLong.toDMS();
                    data.Properties.Add(ExifTag.GPSLongitude, Math.Abs(lon.degrees), Math.Abs(lon.minutes), Math.Abs(lon.seconds));
                    var lat = dLat.toDMS();
                    data.Properties.Add(ExifTag.GPSLatitude, Math.Abs(lat.degrees), Math.Abs(lat.minutes), Math.Abs(lat.seconds));

                    data.Properties.Add(ExifTag.GPSAltitude, alt);

                    data.Properties.Add(ExifTag.GPSLatitudeRef, dLat < 0 ? "S" : "N");
                    data.Properties.Add(ExifTag.GPSLongitudeRef, dLong < 0 ? "W" : "E");

                    // Save file into Geotag folder
                    string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

                    string outputfilename = geoTagFolder + Path.DirectorySeparatorChar +
                                            Path.GetFileNameWithoutExtension(Filename) + "_geotag" +
                                            Path.GetExtension(Filename);
                    Directory.CreateDirectory(geoTagFolder);
                    // Just in case
                    if (File.Exists(outputfilename))
                        File.Delete(outputfilename);

                    data.Save(outputfilename);
                    /*
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
                        string geoTagFolder = rootFolder + Path.DirectorySeparatorChar + "geotagged";

                        string outputfilename = geoTagFolder + Path.DirectorySeparatorChar +
                                                Path.GetFileNameWithoutExtension(Filename) + "_geotag" +
                                                Path.GetExtension(Filename);

                        // Just in case
                        if (File.Exists(outputfilename))
                            File.Delete(outputfilename);

                        Pic.Save(outputfilename);
                    }
                   */
                }
                catch
                {
                    AppendText("There was a problem with image " + Filename);
                }
            }
        }



        public void CreateReportFiles(Dictionary<string, PictureInformation> listPhotosWithInfo, string dirWithImages,
            float offset, double num_camerarotation, double num_hfov, double num_vfov, Action<string> AppendText = null, Action<string> GeoRefKML = null, bool usegpsalt = false)
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

                AppendText?.Invoke("Start Processing\n");

                // Dont know why but it was 10 in the past so let it be. Used to generate jxl file simulating x100 from trimble
                int lastRecordN = JXL_ID_OFFSET;

                // path
                CoordinateCollection coords = new CoordinateCollection();

                foreach (var item in vehicleLocations.Values)
                {
                    if (item != null)
                        coords.Add(new SharpKml.Base.Vector(item.Lat, item.Lon, item.getAltitude(useAMSLAlt,usegpsalt)));
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
                                Coordinate = new Vector(picInfo.Lat, picInfo.Lon, picInfo.getAltitude(true,usegpsalt)),
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
                    double alpha = picInfo.Yaw + num_camerarotation;

                    RectangleF rect = getboundingbox(picInfo.Lat, picInfo.Lon, picInfo.getAltitude(true, usegpsalt), alpha, num_hfov, num_vfov);

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
                                       picInfo.getAltitude(useAMSLAlt, usegpsalt).ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Yaw.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Pitch.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.Roll.ToString(CultureInfo.InvariantCulture) + " " +
                                       picInfo.SAlt.ToString(CultureInfo.InvariantCulture));

                    swloccsv.WriteLine(filename + "," + picInfo.Lat.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Lon.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.getAltitude(useAMSLAlt, usegpsalt).ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Yaw.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Pitch.ToString(CultureInfo.InvariantCulture) + "," +
                                       picInfo.Roll.ToString(CultureInfo.InvariantCulture));

                    swloctel.WriteLine(filename + "\t" + picInfo.Time.ToString("yyyy:MM:dd HH:mm:ss") + "\t" +
                                       picInfo.Lon + "\t" + picInfo.Lat + "\t" + picInfo.getAltitude(useAMSLAlt, usegpsalt));
                    swloctel.Flush();
                    swloctxt.Flush();

                    lastRecordN = GenPhotoStationRecord(swloctrim, picInfo.Path, picInfo.Lat, picInfo.Lon,
                        picInfo.getAltitude(useAMSLAlt, usegpsalt), 0, 0, picInfo.Yaw, picInfo.Width, picInfo.Height, lastRecordN);

                    log.InfoFormat(filename + " " + picInfo.Lon + " " + picInfo.Lat + " " +
                                   picInfo.getAltitude(useAMSLAlt, usegpsalt) + "           ");
                }

                Serializer serializer = new Serializer();
                serializer.Serialize(kmlroot);
                swlockml.Write(serializer.Xml);

                GeoRefKML?.Invoke(serializer.Xml);

                writeGPX(dirWithImages + Path.DirectorySeparatorChar + "location.gpx", listPhotosWithInfo);

                // flightmission
                GenFlightMission(swloctrim, lastRecordN);

                swloctrim.WriteEndElement(); // fieldbook
                swloctrim.WriteEndElement(); // job
                swloctrim.WriteEndDocument();

                AppendText?.Invoke("Done \n\n");
            }
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
    }

}
