using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// read log and extract log
    /// </summary>
    public class DFLog
    {
        internal readonly DFLogBuffer _dfLogBuffer;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public struct Label
        {
            public int Id;
            public string Format;
            public List<string> FieldNames;

            public int Length;
            public string Name;
        }

        public struct DFItem
        {
            public string msgtype
            {
                get
                {
                    if(raw.Length>0)
                        return raw[0].ToString();
                    return "";
                }
            }
            public DateTime time
            {
                get
                {
                    // always return a time relative to the gps start time
                    var time = parent.gpsstarttime.AddMilliseconds(timems - parent.msoffset);
                    parent.lasttime = time;
                    return time;
                }
            }

            public string instance
            {
                get
                {
                    var typeno = parent.logformat[msgtype].Id;

                    if (!parent._dfLogBuffer.InstanceType.ContainsKey(typeno))
                        return "";

                    var unittypes = parent._dfLogBuffer.FMTU[typeno].Item1;

                    int colinst = unittypes.IndexOf("#") + 1;
                    return raw[colinst].ToString();
                }
            }

            string[] _items;
            public string[] items
            {
                get
                {
                    if (_items == null)
                    {
                        _items = raw.Select((a) =>
                        {
                            if (a.IsNumber())
                                return (((IConvertible)a).ToString(CultureInfo.InvariantCulture));
                            else
                                return a?.ToString();
                        }).ToArray();
                    }
                    return _items;
                }
            }

            int _timems;
            public int timems
            {
                get
                {
                    if (_timems != 0)
                        return _timems;

                    if (parent.logformat.ContainsKey(msgtype))
                    {
                        int index;

                        index = parent.FindMessageOffset(msgtype, "TimeMS");
                        if (index >= 0)
                        {
                            _timems = int.Parse(raw[index].ToString());
                            return _timems;
                        }
                        index = parent.FindMessageOffset(msgtype, "TimeUS");
                        if (index >= 0)
                        {
                            _timems = (int)(long.Parse(raw[index].ToString()) / 1000);
                            return _timems;
                        }
                        index = parent.FindMessageOffset(msgtype, "T");
                        if (index >= 0)
                        {
                            _timems = int.Parse(raw[index].ToString());
                            return _timems;
                        }
                    }

                    return _timems;
                }
            }

            public int lineno;
            private object[] _raw;

            public object[] raw
            {
                get { return _raw; }
                set
                {
                    _raw = value;
                    _items = null;
                }
            }

            public DFLog parent;

            public DFItem(DFLog _parent, object[] _answer, int lineno) : this()
            {
                this.parent = _parent;

                this.lineno = lineno;

                this.raw = _answer;

                // check we have data
                if (_answer.Length > 0)
                {
                    // check this is a gps message and we dont have the current gpsstarttime
                    if (parent.gpsstarttime == DateTime.MinValue && msgtype.StartsWith("GPS"))
                    {
                        if (parent.logformat.ContainsKey(msgtype))
                        {
                            var indextimems = _parent.FindMessageOffset(msgtype, "T");
                            var time = parent.GetTimeGPS(String.Join(",", items));

                            if (time != DateTime.MinValue)
                            {
                                parent.gpsstarttime = time;

                                parent.lasttime = parent.gpsstarttime;

                                indextimems = parent.FindMessageOffset(items[0], "T");

                                if (indextimems != -1)
                                {
                                    try
                                    {
                                        parent.msoffset = int.Parse(items[indextimems]);
                                    }
                                    catch
                                    {
                                        parent.gpsstarttime = DateTime.MinValue;
                                    }
                                }

                                int indextimeus = parent.FindMessageOffset(items[0], "TimeUS");

                                if (indextimeus != -1)
                                {
                                    try
                                    {
                                        parent.msoffset = long.Parse(items[indextimeus]) / 1000;
                                    }
                                    catch
                                    {
                                        parent.gpsstarttime = DateTime.MinValue;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public string this[string item]
            {
                get
                {
                    var index = parent.FindMessageOffset(msgtype, item);
                    if (index == -1 || index >= items.Length)
                        return null;
                    return items[index];
                }
            }

            public Dictionary<string, object> ToDictionary()
            {
                var ans = new Dictionary<string, object>();

                int a = 1;
                foreach (var fieldName in parent.logformat[msgtype].FieldNames)
                {
                    if (a.IsNumber())
                        ans[fieldName] = (IConvertible) raw[a];
                    else
                        ans[fieldName] = items[a];
                    a++;
                }

                return ans;
            }

            public object GetRaw(string item)
            {
                var index = parent.FindMessageOffset(msgtype, item);
                if (index == -1)
                    return null;
                return raw[index];
            }

            public T GetRaw<T>(string item)
            {
                var index = parent.FindMessageOffset(msgtype, item);
                if (index == -1)
                    return default(T);
                return (T) raw[index];
            }
        }

        //https://github.com/ArduPilot/ardupilot/blob/master/libraries/AP_Logger/AP_Logger.h
        public enum Log_Event : byte
        {
            AP_STATE = 7,
            // SYSTEM_TIME_SET = 8,
            INIT_SIMPLE_BEARING = 9,

            ARMED = 10,
            DISARMED = 11,
            AUTO_ARMED = 15,
            LAND_COMPLETE_MAYBE = 17,
            LAND_COMPLETE = 18,
            NOT_LANDED = 28,
            LOST_GPS = 19,
            FLIP_START = 21,
            FLIP_END = 22,
            SET_HOME = 25,
            SET_SIMPLE_ON = 26,
            SET_SIMPLE_OFF = 27,
            SET_SUPERSIMPLE_ON = 29,
            AUTOTUNE_INITIALISED = 30,
            AUTOTUNE_OFF = 31,
            AUTOTUNE_RESTART = 32,
            AUTOTUNE_SUCCESS = 33,
            AUTOTUNE_FAILED = 34,
            AUTOTUNE_REACHED_LIMIT = 35,
            AUTOTUNE_PILOT_TESTING = 36,
            AUTOTUNE_SAVEDGAINS = 37,
            SAVE_TRIM = 38,
            SAVEWP_ADD_WP = 39,
            FENCE_ENABLE = 41,
            FENCE_DISABLE = 42,
            ACRO_TRAINER_OFF = 43,
            ACRO_TRAINER_LEVELING = 44,
            ACRO_TRAINER_LIMITED = 45,
            GRIPPER_GRAB = 46,
            GRIPPER_RELEASE = 47,
            PARACHUTE_DISABLED = 49,
            PARACHUTE_ENABLED = 50,
            PARACHUTE_RELEASED = 51,
            LANDING_GEAR_DEPLOYED = 52,
            LANDING_GEAR_RETRACTED = 53,
            MOTORS_EMERGENCY_STOPPED = 54,
            MOTORS_EMERGENCY_STOP_CLEARED = 55,
            MOTORS_INTERLOCK_DISABLED = 56,
            MOTORS_INTERLOCK_ENABLED = 57,
            ROTOR_RUNUP_COMPLETE = 58, // Heli only
            ROTOR_SPEED_BELOW_CRITICAL = 59, // Heli only
            EKF_ALT_RESET = 60,
            LAND_CANCELLED_BY_PILOT = 61,
            EKF_YAW_RESET = 62,
            AVOIDANCE_ADSB_ENABLE = 63,
            AVOIDANCE_ADSB_DISABLE = 64,
            AVOIDANCE_PROXIMITY_ENABLE = 65,
            AVOIDANCE_PROXIMITY_DISABLE = 66,
            GPS_PRIMARY_CHANGED = 67,
            WINCH_RELAXED = 68,
            WINCH_LENGTH_CONTROL = 69,
            WINCH_RATE_CONTROL = 70,
            ZIGZAG_STORE_A = 71,
            ZIGZAG_STORE_B = 72,
            LAND_REPO_ACTIVE = 73,
            STANDBY_ENABLE = 74,
            STANDBY_DISABLE = 75,

            SURFACED = 163,
            NOT_SURFACED = 164,
            BOTTOMED = 165,
            NOT_BOTTOMED = 166,
        }

        public enum LogErrorSubsystem : byte
        {
            MAIN = 1,
            RADIO = 2,
            COMPASS = 3,
            OPTFLOW = 4,   // not used
            FAILSAFE_RADIO = 5,
            FAILSAFE_BATT = 6,
            FAILSAFE_GPS = 7,   // not used
            FAILSAFE_GCS = 8,
            FAILSAFE_FENCE = 9,
            FLIGHT_MODE = 10,
            GPS = 11,
            CRASH_CHECK = 12,
            FLIP = 13,
            AUTOTUNE = 14,  // not used
            PARACHUTES = 15,
            EKFCHECK = 16,
            FAILSAFE_EKFINAV = 17,
            BARO = 18,
            CPU = 19,
            FAILSAFE_ADSB = 20,
            TERRAIN = 21,
            NAVIGATION = 22,
            FAILSAFE_TERRAIN = 23,
            EKF_PRIMARY = 24,
            THRUST_LOSS_CHECK = 25,
            FAILSAFE_SENSORS = 26,
            FAILSAFE_LEAK = 27,
            PILOT_INPUT = 28,
            FAILSAFE_VIBE = 29,
        }

        // bizarrely this enumeration has lots of duplicate values, offering
        // very little in the way of typesafety
        public enum LogErrorCode : byte
        {
            // general error codes
            ERROR_RESOLVED = 0,
            FAILED_TO_INITIALISE = 1,
            UNHEALTHY = 4,
            // subsystem specific error codes -- radio
            RADIO_LATE_FRAME = 2,
            // subsystem specific error codes -- failsafe_thr, batt, gps
            FAILSAFE_RESOLVED = 0,
            FAILSAFE_OCCURRED = 1,
            // subsystem specific error codes -- main
            MAIN_INS_DELAY = 1,
            // subsystem specific error codes -- crash checker
            CRASH_CHECK_CRASH = 1,
            CRASH_CHECK_LOSS_OF_CONTROL = 2,
            // subsystem specific error codes -- flip
            FLIP_ABANDONED = 2,
            // subsystem specific error codes -- terrain
            MISSING_TERRAIN_DATA = 2,
            // subsystem specific error codes -- navigation
            FAILED_TO_SET_DESTINATION = 2,
            RESTARTED_RTL = 3,
            FAILED_CIRCLE_INIT = 4,
            DEST_OUTSIDE_FENCE = 5,

            // parachute failed to deploy because of low altitude or landed
            PARACHUTE_TOO_LOW = 2,
            PARACHUTE_LANDED = 3,
            // EKF check definitions
            EKFCHECK_BAD_VARIANCE = 2,
            EKFCHECK_VARIANCE_CLEARED = 0,
            // Baro specific error codes
            BARO_GLITCH = 2,
            BAD_DEPTH = 3, // sub-only
                           // GPS specific error coces
            GPS_GLITCH = 2,
        }


        public Dictionary<string, Label> logformat = new Dictionary<string, Label>();

        public void Clear()
        {
            logformat.Clear();

            GC.Collect();

            // current gps time
            gpstime = DateTime.MinValue;
            // last time of message
            lasttime = DateTime.MinValue;
            // first valid gpstime
            gpsstarttime = DateTime.MinValue;
        }

        public DateTime GetFirstGpsTime(string fn)
        {
            using (StreamReader sr = new StreamReader(fn))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith("FMT"))
                    {
                        FMTLine(line);
                    }
                    else if (line.StartsWith("GPS"))
                    {
                        DateTime answer = GetTimeGPS(line);
                        if (answer != DateTime.MinValue)
                            return answer;
                    }
                }
            }

            return DateTime.MinValue;
        }

        public List<DFItem> ReadLog(string fn)
        {
            List<DFItem> answer = new List<DFItem>();

            using (Stream st = File.OpenRead(fn))
            {
                answer = ReadLog(st);
            }

            return answer;
        }

        // current gps time
        DateTime gpstime = DateTime.MinValue;
        // last time of message
        DateTime lasttime = DateTime.MinValue;
        // first valid gpstime
        DateTime gpsstarttime = DateTime.MinValue;

        long msoffset = 0;

        public DFLog(DFLogBuffer dfLogBuffer)
        {
            _dfLogBuffer = dfLogBuffer;
        }

        public List<DFItem> ReadLog(Stream fn)
        {
            Clear();
            GC.Collect();

            List<DFItem> answer = new List<DFItem>();

            // current gps time
            gpstime = DateTime.MinValue;
            // last time of message
            lasttime = DateTime.MinValue;
            // first valid gpstime
            gpsstarttime = DateTime.MinValue;

            int lineno = 0;
            msoffset = 0;

            log.Info("loading log " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            using (StreamReader sr = new StreamReader(fn))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string line = sr.ReadLine();

                        lineno++;

                        DFItem newitem = GetDFItemFromLine(line, lineno);

                        answer.Add(newitem);
                    }
                    catch (OutOfMemoryException ex)
                    {
                        log.Error(ex);
                        return answer;
                    }
                    catch
                    {
                    }
                }
            }

            log.Info("loaded log " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            return answer;
        }

        public DFItem GetDFItemFromLine(string line, int lineno)
        {
            //line = line.Replace(",", ",");
            //line = line.Replace(":", ":");

            string[] items = line.Trim().Split(new char[] {',', ':'}, StringSplitOptions.RemoveEmptyEntries);

            if (line.StartsWith("FMT"))
            {
                FMTLine(line);
            }
            else if (line.StartsWith("GPS"))
            {
               
            }
            else if (line.StartsWith("ERR"))
            {
                Array.Resize(ref items, items.Length + 2);
                try
                {
                    int index = FindMessageOffset("ERR", "Subsys");
                    if (index == -1)
                    {
                        throw new ArgumentNullException();
                    }

                    int index2 = FindMessageOffset("ERR", "ECode");
                    if (index2 == -1)
                    {
                        throw new ArgumentNullException();
                    }

                    items[items.Length - 2] = "" + (DFLog.LogErrorSubsystem) int.Parse(items[index]);
                }
                catch
                {
                }
            }
            else if (line.StartsWith("EV"))
            {
                Array.Resize(ref items, items.Length + 1);
                try
                {
                    int index = FindMessageOffset("EV", "Id");
                    if (index == -1)
                    {
                        throw new ArgumentNullException();
                    }

                    items[items.Length - 1] = "" + (DFLog.Log_Event) int.Parse(items[index]);
                }
                catch
                {
                }
            }
            else if (line.StartsWith("MAG"))
            {
            }

            DFItem item = new DFItem(this, items, lineno);

            return item;
        }

        public void FMTLine(string strLine)
        {
            try
            {
                if (strLine.StartsWith("FMT"))
                {
                    strLine = strLine.Replace(", ", ",");
                    strLine = strLine.Replace(": ", ":");

                    string[] items = strLine.Trim().Split(',', ':');

                    string[] names = new string[items.Length - 5];
                    Array.ConstrainedCopy(items, 5, names, 0, names.Length);

                    Label lbl = new Label()
                    {
                        Name = items[3],
                        Id = int.Parse(items[1]),
                        Format = items[4],
                        Length = int.Parse(items[2]),
                        FieldNames = names.ToList()
                    };

                    logformat[lbl.Name] = lbl;
                }
            }
            catch
            {
            }
        }

        //FMT, 130, 45, GPS, BIHBcLLeeEefI, Status,TimeMS,Week,NSats,HDop,Lat,Lng,RelAlt,Alt,Spd,GCrs,VZ,T
        //GPS, 3, 130040903, 1769, 10, 0.00, -35.3547178, 149.1696673, 885.52, 870.45, 24.56, 321.44, 2.450000, 127615
        public DateTime GetTimeGPS(string gpsline)
        {
            if (gpsline.StartsWith("GPS") && logformat.Count > 0)
            {
                string strLine = gpsline.Replace(", ", ",");
                strLine = strLine.Replace(": ", ":");

                string[] items = strLine.Split(',', ':');

                // check its a valid lock
                int indexstatus = FindMessageOffset("GPS", "Status");

                if (indexstatus != -1)
                {
                    // 3d lock or better
                    if (items[indexstatus].Trim() == "0" || items[indexstatus].Trim() == "1" ||
                        items[indexstatus].Trim() == "2")
                        return DateTime.MinValue;
                }

                // get time since start of week
                int indextimems = FindMessageOffset("GPS", "TimeMS");

                if (indextimems == -1)
                {
                    indextimems = FindMessageOffset("GPS", "GMS");
                }

                // get week number
                int indexweek = FindMessageOffset("GPS", "Week");

                if (indexweek == -1)
                    indexweek = FindMessageOffset("GPS", "GWk");

                if (indextimems == -1 || indexweek == -1)
                    return DateTime.MinValue;

                try
                {
                    var week = int.Parse(items[indexweek]);
                    var sec = long.Parse(items[indextimems]) / 1000.0;

                    if(week > 5000 || week < 0 || sec > 60*60*24*7 || sec < 0)
                        return DateTime.MinValue;

                    return gpsTimeToTime(week, sec);
                }
                catch 
                {
                    return DateTime.MinValue;
                }
            }

            return DateTime.MinValue;
        }

        public static DateTime gpsTimeToTime(int week, double sec)
        {
            int leap = rtcm3.StaticUtils.LeapSecondsGPS(DateTime.Now.Year, DateTime.Now.Month);

            var basetime = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            basetime = basetime.AddDays(week*7);
            basetime = basetime.AddSeconds((sec - leap));

            return basetime.ToLocalTime();
        }

        public int FindMessageOffset(string linetype, string find)
        {
            if (linetype == null || find == null)
                return -1;

            if (logformat.ContainsKey(linetype.ToUpper()))
            {
                var ans = logformat[linetype].FieldNames.IndexOf(find);
                if (ans == -1)
                    return -1;
                // + type
                return ans + 1;
            }

            return -1;
        }

        public long GetLineNoFromTime(DFLogBuffer logdata, DateTime p1)
        {
            DateTime last = DateTime.MaxValue;

            foreach (var dfItem in logdata.GetEnumeratorType(new string[] { "GPS","GPS2"}))
            {
                // always forwards
                if (dfItem.time >= p1)
                    return dfItem.lineno;

                last = dfItem.time;
            }

            if (last != DateTime.MaxValue)
                return long.MaxValue;

            return 0;
        }
    }
}