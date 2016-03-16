using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class ParamFile
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Hashtable loadParamFile(string Filename)
        {
            Hashtable param = new Hashtable();

            using (StreamReader sr = new StreamReader(Filename))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Contains("NOTE:"))
                    {
                        CustomMessageBox.Show(line, "Saved Note");
                        continue;
                    }

                    if (line.StartsWith("#"))
                        continue;

                    string[] items = line.Split(new char[] {' ', ',', '\t'}, StringSplitOptions.RemoveEmptyEntries);

                    if (items.Length != 2)
                        continue;

                    string name = items[0];
                    double value = 0;
                    try
                    {
                        value = double.Parse(items[1], System.Globalization.CultureInfo.InvariantCulture);
                            // new System.Globalization.CultureInfo("en-US"));
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
                string input = DateTime.Now + " Frame : ";
                if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                {
                    input = DateTime.Now + " Plane: Skywalker";
                }
                InputBox.Show("Custom Note", "Enter your Notes/Frame Type etc", ref input);
                if (input != "")
                    sw.WriteLine("#NOTE: " + input.Replace(',', '|'));

                var list = new SortedList(paramlist);

                foreach (var item in list.Keys)
                {
                    double value = double.Parse(paramlist[item].ToString());

                    string valueasstring = value.ToString(new System.Globalization.CultureInfo("en-US"));

                    if (valueasstring.Contains("."))
                    {
                        sw.WriteLine(item + "," +
                                     ((float) value).ToString(new System.Globalization.CultureInfo("en-US")));
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