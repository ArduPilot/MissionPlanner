using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace MissionPlanner.Log
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
            public string[] FieldNames;

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
        }

        public static Dictionary<string, Label> logformat = new Dictionary<string, Label>();

        public static void Clear()
        {
            logformat.Clear();
        }

        public static DateTime GetFirstGpsTime(string fn)
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

        public static List<DFItem> ReadLog(string fn)
        {
            List<DFItem> answer = new List<DFItem>();

            using (Stream st = File.OpenRead(fn))
            {
                answer = ReadLog(st);
            }

            return answer;
        }

        public static List<DFItem> ReadLog(Stream fn)
        {
            Clear();

            List<DFItem> answer = new List<DFItem>();
            DateTime gpstime = DateTime.MinValue;

            int lineno = 0;
            int msoffset = 0;
            DateTime lasttime = DateTime.MinValue;

            log.Info("loading log");
           

            using (StreamReader sr = new StreamReader(fn))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        string line = sr.ReadLine();

                        lineno++;

                        line = line.Replace(",", ",");
                        line = line.Replace(":", ":");

                        string[] items = line.Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (line.StartsWith("FMT"))
                        {
                            FMTLine(line);
                        }
                        else if (line.StartsWith("GPS"))
                        {
                            gpstime = GetTimeGPS(line);
                            lasttime = gpstime;

                            int indextimems = FindInArray(logformat["GPS"].FieldNames, "T");

                            if (indextimems != -1)
                            {
                                try
                                {
                                    msoffset = int.Parse(items[indextimems]);
                                }
                                catch { }
                            }
                        }

                        DFItem item = new DFItem();
                        try
                        {
                            item.lineno = lineno;

                            if (items.Length > 0)
                            {
                                item.msgtype = items[0];
                                item.items = items;

                                if (line.StartsWith("GPS"))
                                {
                                    item.time = gpstime;
                                }
                                else
                                {

                                    int indextimems = FindInArray(logformat[item.msgtype].FieldNames, "TimeMS");

                                    if (indextimems != -1)
                                    {
                                        item.timems = int.Parse(items[indextimems]);

                                        item.time = gpstime.AddMilliseconds(item.timems - msoffset);

                                        lasttime = item.time;
                                    }
                                    else
                                    {
                                        item.time = lasttime;
                                    }
                                }
                            }
                        }
                        catch { }

                        answer.Add(item);
                    }
                    catch { }
                }
            }

            log.Info("loaded log");

            return answer;
        }

        public static void FMTLine(string strLine)
        {
            try
            {
                if (strLine.StartsWith("FMT"))
                {
                    strLine = strLine.Replace(", ", ",");
                    strLine = strLine.Replace(": ", ":");

                    string[] items = strLine.Split(',', ':');

                    string[] names = new string[items.Length - 5];
                    Array.ConstrainedCopy(items, 5, names, 0, names.Length);

                    Label lbl = new Label() { Name = items[3], Id = int.Parse(items[1]), Format = items[4], Length = int.Parse(items[2]), FieldNames = names };

                    logformat[lbl.Name] = lbl;
                }
            }
            catch { }
        }

        //FMT, 130, 45, GPS, BIHBcLLeeEefI, Status,TimeMS,Week,NSats,HDop,Lat,Lng,RelAlt,Alt,Spd,GCrs,VZ,T
        //GPS, 3, 130040903, 1769, 10, 0.00, -35.3547178, 149.1696673, 885.52, 870.45, 24.56, 321.44, 2.450000, 127615
        public static DateTime GetTimeGPS(string gpsline)
        {
            if (gpsline.StartsWith("GPS") && logformat.Count > 0)
            {
                string strLine = gpsline.Replace(", ", ",");
                strLine = strLine.Replace(": ", ":");

                string[] items = strLine.Split(',', ':');

                // check its a valid lock
                int indexstatus = FindInArray(logformat["GPS"].FieldNames, "Status");

                if (indexstatus != -1)
                {
                    if (items[indexstatus].Trim() != "3" && items[indexstatus].Trim() != "2")
                        return DateTime.MinValue;
                }

                // get time since start of week
                int indextimems = FindInArray(logformat["GPS"].FieldNames, "TimeMS");

                // get week number
                int indexweek = FindInArray(logformat["GPS"].FieldNames, "Week");

                if (indextimems == -1 || indexweek == -1)
                    return DateTime.MinValue;

                return gpsTimeToTime(int.Parse(items[indexweek]), int.Parse(items[indextimems]) / 1000.0);
            }

            return DateTime.MinValue;
        }

        public static DateTime gpsTimeToTime(int week, double sec)
        {
            int leap = 16;

            // not correct for leap seconds                   day   days  weeks  seconds
            var basetime = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Local);
            basetime = basetime.AddDays(week * 7);
            basetime = basetime.AddSeconds((sec - leap));

            return basetime;
        }

        public static int FindInArray(string[] array, string find)
        {
            int a = 1;
            foreach (string item in array)
            {
                if (item == find)
                {
                    return a;
                }
                a++;
            }
            return -1;
        }
    }
}
