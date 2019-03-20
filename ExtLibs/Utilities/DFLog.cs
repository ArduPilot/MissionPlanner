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
            public string msgtype;
            public DateTime time;
            public string[] items;
            public int timems;
            public int lineno;

            public DFLog parent;

            public DFItem(DFLog _parent, object[] _answer, int lineno) : this()
            {
                 this.parent = _parent;

                this.lineno = lineno;

                if (_answer.Length > 0)
                {
                    msgtype = _answer[0].ToString();
                    items = _answer.Select((a) =>
                    {
                        if (a.IsNumber())
                            return (((IConvertible) a).ToString(CultureInfo.InvariantCulture));
                        else
                            return a.ToString();
                    }).ToArray();
                    bool timeus = false;

                    if (_parent.logformat.ContainsKey(msgtype))
                    {
                        int indextimems = _parent.FindMessageOffset(msgtype, "TimeMS");

                        if (msgtype.StartsWith("GPS"))
                        {
                            indextimems = _parent.FindMessageOffset(msgtype, "T");

                            if (parent.gpsstarttime == DateTime.MinValue)
                            {
                                var time = parent.GetTimeGPS(msgtype + "," + String.Join(",", items));

                                if (time != DateTime.MinValue)
                                {
                                    parent.gpsstarttime = time;

                                    _parent.lasttime = parent.gpsstarttime;

                                    indextimems = _parent.FindMessageOffset(items[0], "T");

                                    if (indextimems != -1)
                                    {
                                        try
                                        {
                                            _parent.msoffset = int.Parse(items[indextimems]);
                                        }
                                        catch
                                        {
                                            _parent.gpsstarttime = DateTime.MinValue;
                                        }
                                    }

                                    int indextimeus = _parent.FindMessageOffset(items[0], "TimeUS");

                                    if (indextimeus != -1)
                                    {
                                        try
                                        {
                                            _parent.msoffset = long.Parse(items[indextimeus]) / 1000;
                                        }
                                        catch
                                        {
                                            _parent.gpsstarttime = DateTime.MinValue;
                                        }
                                    }
                                }
                            }


                        }

                        if (indextimems == -1)
                        {
                            indextimems = _parent.FindMessageOffset(msgtype, "TimeUS");
                            timeus = true;
                        }

                        if (indextimems != -1)
                        {
                            long ntime = 0;

                            if (long.TryParse(items[indextimems], out ntime))
                            {
                                if (timeus)
                                    ntime /= 1000;

                                timems = (int)ntime;

                                if (_parent.gpsstarttime != DateTime.MinValue)
                                {
                                    time = _parent.gpsstarttime.AddMilliseconds(timems - _parent.msoffset);
                                    _parent.lasttime = time;
                                }
                            }
                            else
                            {
                                time = _parent.lasttime;
                            }
                        }
                        else
                        {
                            time = _parent.lasttime;
                        }
                    }
                }
            }

            public string this[string item]
            {
                get
                {
                    var index = parent.FindMessageOffset(msgtype, item);
                    if (index == -1)
                        return null;
                    return items[index];
                }
            }
        }

        public enum error_subsystem
        {
            MAIN = 1,
            RADIO = 2,
            COMPASS = 3,
            OPTFLOW = 4,
            FAILSAFE_RADIO = 5,
            FAILSAFE_BATT = 6,
            FAILSAFE_GPS = 7,
            FAILSAFE_GCS = 8,
            FAILSAFE_FENCE = 9,
            FLIGHT_MODE = 10,
            GPS = 11,
            CRASH_CHECK = 12,
            ERROR_SUBSYSTEM_FLIP = 13,
            AUTOTUNE = 14,
            PARACHUTE = 15,
            EKF_CHECK = 16,
            FAILSAFE_EKF = 17,
            BARO = 18,
            CPU = 19,
            ADSB = 20,
            TERRAIN = 21,
            NAVIGATION = 22,
            FAILSAFE_TERRAIN = 23,
            EKF_PRIMARY = 24,
        }

        public enum events
        {
            DATA_MAVLINK_FLOAT = 1,
            DATA_MAVLINK_INT32 = 2,
            DATA_MAVLINK_INT16 = 3,
            DATA_MAVLINK_INT8 = 4,
            DATA_AP_STATE = 7,
            DATA_SYSTEM_TIME_SET = 8,
            DATA_INIT_SIMPLE_BEARING = 9,
            DATA_ARMED = 10,
            DATA_DISARMED = 11,
            DATA_AUTO_ARMED = 15,
            DATA_TAKEOFF = 16,
            DATA_LAND_COMPLETE_MAYBE = 17,
            DATA_LAND_COMPLETE = 18,
            DATA_NOT_LANDED = 28,
            DATA_LOST_GPS = 19,
            DATA_BEGIN_FLIP = 21,
            DATA_END_FLIP = 22,
            DATA_EXIT_FLIP = 23,
            DATA_SET_HOME = 25,
            DATA_SET_SIMPLE_ON = 26,
            DATA_SET_SIMPLE_OFF = 27,
            DATA_SET_SUPERSIMPLE_ON = 29,
            DATA_AUTOTUNE_INITIALISED = 30,
            DATA_AUTOTUNE_OFF = 31,
            DATA_AUTOTUNE_RESTART = 32,
            DATA_AUTOTUNE_COMPLETE = 33,
            DATA_AUTOTUNE_ABANDONED = 34,
            DATA_AUTOTUNE_REACHED_LIMIT = 35,
            DATA_AUTOTUNE_TESTING = 36,
            DATA_AUTOTUNE_SAVEDGAINS = 37,
            DATA_SAVE_TRIM = 38,
            DATA_SAVEWP_ADD_WP = 39,
            DATA_SAVEWP_CLEAR_MISSION_RTL = 40,
            DATA_FENCE_ENABLE = 41,
            DATA_FENCE_DISABLE = 42,
            DATA_ACRO_TRAINER_DISABLED = 43,
            DATA_ACRO_TRAINER_LEVELING = 44,
            DATA_ACRO_TRAINER_LIMITED = 45,
            DATA_GRIPPER_GRAB = 46,
            DATA_GRIPPER_RELEASE = 47,
            DATA_GRIPPER_NEUTRAL = 48,
            DATA_PARACHUTE_DISABLED = 49,
            DATA_PARACHUTE_ENABLED = 50,
            DATA_PARACHUTE_RELEASED = 51,
            DATA_LANDING_GEAR_DEPLOYED = 52,
            DATA_LANDING_GEAR_RETRACTED = 53,
            DATA_MOTORS_EMERGENCY_STOPPED = 54,
            DATA_MOTORS_EMERGENCY_STOP_CLEARED = 55,
            DATA_MOTORS_INTERLOCK_DISABLED = 56,
            DATA_MOTORS_INTERLOCK_ENABLED = 57,
            DATA_ROTOR_RUNUP_COMPLETE = 58, // Heli only
            DATA_ROTOR_SPEED_BELOW_CRITICAL = 59, // Heli only
            DATA_EKF_ALT_RESET = 60,
            DATA_LAND_CANCELLED_BY_PILOT = 61,
            DATA_EKF_YAW_RESET = 62,
            DATA_AVOIDANCE_ADSB_ENABLE = 63,
            DATA_AVOIDANCE_ADSB_DISABLE = 64,
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
                if (line.StartsWith("GPS") && gpsstarttime == DateTime.MinValue)
                {
                    var time = GetTimeGPS(line);

                    if (time != DateTime.MinValue)
                    {
                        gpsstarttime = time;

                        lasttime = gpsstarttime;

                        int indextimems = FindMessageOffset(items[0], "T");

                        if (indextimems != -1)
                        {
                            try
                            {
                                msoffset = int.Parse(items[indextimems]);
                            }
                            catch
                            {
                                gpsstarttime = DateTime.MinValue;
                            }
                        }

                        int indextimeus = FindMessageOffset(items[0], "TimeUS");

                        if (indextimeus != -1)
                        {
                            try
                            {
                                msoffset = long.Parse(items[indextimeus])/1000;
                            }
                            catch
                            {
                                gpsstarttime = DateTime.MinValue;
                            }
                        }
                    }
                }
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

                    items[items.Length - 2] = "" + (DFLog.error_subsystem) int.Parse(items[index]);
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

                    items[items.Length - 1] = "" + (DFLog.events) int.Parse(items[index]);
                }
                catch
                {
                }
            }
            else if (line.StartsWith("MAG"))
            {
            }

            DFItem item = new DFItem() {parent = this};
            try
            {
                item.lineno = lineno;

                if (items.Length > 0)
                {
                    item.msgtype = items[0];
                    item.items = items;
                    bool timeus = false;

                    if (logformat.ContainsKey(item.msgtype))
                    {
                        int indextimems = FindMessageOffset(item.msgtype, "TimeMS");

                        if (item.msgtype.StartsWith("GPS"))
                        {
                            indextimems = FindMessageOffset(item.msgtype, "T");
                        }

                        if (indextimems == -1)
                        {
                            indextimems = FindMessageOffset(item.msgtype, "TimeUS");
                            timeus = true;
                        }

                        if (indextimems != -1)
                        {
                            long ntime = 0;

                            if (long.TryParse(items[indextimems], out ntime))
                            {
                                if (timeus)
                                    ntime /= 1000;

                                item.timems = (int) ntime;

                                if (gpsstarttime != DateTime.MinValue)
                                {
                                    item.time = gpsstarttime.AddMilliseconds(item.timems - msoffset);
                                    lasttime = item.time;
                                }
                            }
                            else
                            {
                                item.time = lasttime;
                            }
                        }
                        else
                        {
                            item.time = lasttime;
                        }
                    }
                }
            }
            catch
            {
            }

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
                    return gpsTimeToTime(int.Parse(items[indexweek]), long.Parse(items[indextimems]) / 1000.0);
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

        public long GetLineNoFromTime(CollectionBuffer logdata, DateTime p1)
        {
            DateTime last = DateTime.MaxValue;

            foreach (var dfItem in logdata.GetEnumeratorType("GPS"))
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