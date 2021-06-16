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

        static double DoubleParser(string Name, string Text)
        {
            try
            {
                return double.Parse(Text, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new FormatException("Invalid number on param " + Name + " : " + Text.ToString());
            }
        }

        /// <summary>
        /// Load a param file, throw exception if any of the params cannot parse as a double value.  
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static Dictionary<string, double> loadParamFile(string Filename)
        {
            return loadParamFile(Filename, DoubleParser, true);
        }

        static object DoubleOrStringParser(string Name, string Text)
        {
            double D;

            if (double.TryParse(Text, out D))
            {
                return D;
            }
            else
            {
                return Text;
            }
        }

        /// <summary>
        /// Load a param file such that all text values for the value field are accepted, and if they parse as double, they
        /// are conveted to double.
        /// </summary>
        /// <param name="Filename">The file path.</param>
        /// <returns></returns>
        public static Dictionary<string, object> loadParamFiledoubleorstring(string Filename)
        {
            return loadParamFile(Filename, DoubleOrStringParser, false);
        }

        /// <summary>
        /// Load a param file.
        /// </summary>
        /// <typeparam name="T">The type of parameter value.</typeparam>
        /// <param name="Filename">the file path.</param>
        /// <param name="Parser">The parser to use to convert name and value text to param type T</param>
        /// <param name="CheckForMultiSeparator">true to skip a line in the param file if there are multiple separators ' ', ',', or '\t'</param>
        /// <returns>The parameters, with key as name.</returns>
        public static Dictionary<string, T> loadParamFile<T>(string Filename, Func<string, string, T> Parser,
            bool CheckForMultiSeparator)
        {
            Dictionary<string, T> param = new Dictionary<string, T>();

            using (StreamReader sr = new StreamReader(Filename))
            {
                while (!sr.EndOfStream)
                {
                    char[] Separators = new char[] { ' ', ',', '\t' };
                    string line = sr.ReadLine();

                    if (line.StartsWith("#"))
                        continue;

                    int SeparatorIndex = line.IndexOfAny(Separators);
                    if (SeparatorIndex <= 0)
                    {
                        continue;
                    }
                    if (CheckForMultiSeparator && line.Split(Separators).Length > 2)
                    {
                        continue;
                    }

                    string name = line.Substring(0, SeparatorIndex);

                    T value = Parser(name, line.Substring(SeparatorIndex + 1));

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

        /// <summary>
        /// Save the given parameters to file with the given path.  Assumes all parameters are reals (i.e. ints, floats, doubles, etc).
        /// </summary>
        /// <param name="fn">File path</param>
        /// <param name="paramlist">Param list</param>
        public static void SaveParamFile(string fn, Hashtable paramlist)
        {
            SaveParamFile(fn, paramlist, true);
        }

        /// <summary>
        /// Save the given parameters to file with the given path.  
        /// </summary>
        /// <param name="fn">File path</param>
        /// <param name="paramlist">Param list</param>
        /// <param name="AllReals">true if all parameters are reals (i.e. ints, floats, doubles, etc)</param>
        public static void SaveParamFile(string fn, Hashtable paramlist, bool AllReals)
        {
            using (StreamWriter sw = new StreamWriter(File.Open(fn, FileMode.Create)))
            {
                var list = new SortedList(paramlist);

                foreach (var item in list.Keys)
                {
                    if (AllReals)
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
                    else
                    {
                        sw.WriteLine(item + "," + paramlist[item].ToString());
                    }
                }
            }
        }
    }
}