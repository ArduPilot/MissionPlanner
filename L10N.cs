using MissionPlanner.Properties;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionPlanner
{
    public class L10N
    {
        public static CultureInfo ConfigLang;

        static L10N()
        {
            ConfigLang = GetConfigLang();
            Strings.Culture = ConfigLang;
            //In .NET 4.5,System.Globalization.CultureInfo.DefaultThreadCurrentCulture & DefaultThreadCurrentUICulture is avaiable
        }

        public static CultureInfo GetConfigLang()
        {
            CultureInfo ci = CultureInfoEx.GetCultureInfo(MainV2.getConfig("language"));
            if (ci != null)
            {
                return ci;
            }
            else
            {
                return System.Globalization.CultureInfo.CurrentUICulture;
            }
        }

        public static string ReplaceMirrorUrl(ref string url)
        {
            switch (ConfigLang.Name)
            {
                case "zh-CN":
                case "zh-Hans":
                    if (url.Contains("firmware.diydrones.com"))
                    {
                        url = url.Replace("firmware.diydrones.com", "firmware.diywrj.com");
                    }
                    else if (url.Contains("raw.github.com"))
                    {
                        url = url.Replace("raw.github.com", "githubraw.diywrj.com");
                    }
                    /*
                    else if (url.Contains("raw.githubusercontent.com"))
                    {
                        url = url.Replace("raw.githubusercontent.com", "githubraw.diywrj.com");
                    }
                    else if (url.Contains("github.com"))
                    {
                        url = url.Replace("github.com", "github.diywrj.com");
                    }
                    */
                    break;
                default:
                    break;
            }

            return url;
        }
    }
}