using MissionPlanner.Utilities.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    public static class L10NU
    {
        public static Dictionary<string, string> strings;

        static L10NU()
        {
            strings = new Dictionary<string, string>();
            string[] lines = null;

            string lang = System.Globalization.CultureInfo.CurrentUICulture.Name;
            switch (lang)
            {
                case "zh-CN":
                case "zh-Hans":
                    lines = Regex.Split(Resources.strings_zhHans, "\r\n|\r|\n");
                    break;
            }
            if (lines != null)
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    string[] kv = Regex.Split(line, "=");
                    if (kv.Length == 2)
                    {
                        strings[kv[0]] = kv[1];
                    }
                }
            }
        }

        public static string GetString(string key)
        {
            if (strings.ContainsKey(key))
            {
                return strings[key];
            }
            else
            {
                return key;
            }
        }
    }
}
