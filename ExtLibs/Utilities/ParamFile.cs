using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using log4net;

namespace MissionPlanner.Utilities
{
    public class ParamFile
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string FileMask = "Parameter File|*.param;*.parm|All Files|*.*";

        public static Dictionary<string, double> loadParamFile(string Filename)
        {
            Dictionary<string, double> param = new Dictionary<string, double>();

            using (StreamReader sr = new StreamReader(Filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith("#"))
                        continue;

                    string[] items = line.Split(new char[] {' ', ',', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length != 2)
                        continue;

                    string name = items[0];
                    double value = 0;
                    try
                    {
                        value = double.Parse(items[1], CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        throw new FormatException("Invalid number on param " + name + " : " + items[1].ToString());
                    }

                    if (name == "SYSID_SW_MREV")
                        continue;
                    if (name == "WP_TOTAL")
                        continue;
                    if (name == "CMD_TOTAL")
                        continue;
                    if (name == "FENCE_TOTAL")
                        continue;
                    if (name == "SYS_NUM_RESETS")
                        continue;
                    if (name == "ARSPD_OFFSET")
                        continue;
                    if (name == "GND_ABS_PRESS")
                        continue;
                    if (name == "GND_TEMP")
                        continue;
                    if (name == "CMD_INDEX")
                        continue;
                    if (name == "LOG_LASTFILE")
                        continue;
                    if (name == "FORMAT_VERSION")
                        continue;

                    param[name] = value;
                }
            }

            return param;
        }

        public static void SaveParamFile(string fn, Hashtable paramlist)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fn, FileMode.Create)))
            {
                var list = new SortedList(paramlist);

                foreach (var item in list.Keys)
                {
                    double value = double.Parse(paramlist[item].ToString());

                    string valueasstring = value.ToString(CultureInfo.InvariantCulture);

                    if (valueasstring.Contains("."))
                    {
                        sw.WriteLine(item + "," +
                                     (value).ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        sw.WriteLine(item + "," + valueasstring);
                    }
                }
            }
        }
    }
}