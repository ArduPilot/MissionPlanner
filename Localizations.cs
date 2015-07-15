using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MissionPlanner
{
    public static class Localizations
    {
        public static CultureInfo ConfigLang = CultureInfo.GetCultureInfo("en-US");

        public static CultureInfo GetConfigLang()
        {
            CultureInfo ci = CultureInfoEx.GetCultureInfo((string)MainV2.config["language"]);
            if (ci != null)
            {
                return ci;
            }
            else
            {
                return CultureInfo.GetCultureInfo("en-US");
            }
        }

        public static string ReplaceMirrorUrl(ref string url)
        {
            switch (ConfigLang.Name)
            {
                case "zh-CN":
                case "zh-Hans":
                    if (url.Contains("raw.github.com"))
                    {
                        url = url.Replace("raw.github.com", "githubraw.diywrj.com");
                    }
                    else if (url.Contains("firmware.diydrones.com"))
                    {
                        url = url.Replace("firmware.diydrones.com", "firmware.diywrj.com");
                    }
                    else if (url.Contains("github.com"))
                    {
                        url = url.Replace("github.com", "github.diywrj.com");
                    }
                    else
                    {
                    }
                    break;
                default:
                    break;
            }

            return url;
        }
    }
}
