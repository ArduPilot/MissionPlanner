using MissionPlanner.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public class L10N
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static CultureInfo ConfigLang;
        public static bool isMirrorAvailable = true;
        public static bool isMirrorAvailableChecked = false;

        static L10N()
        {
            ConfigLang = GetConfigLang();
            Strings.Culture = ConfigLang;
            //In .NET 4.5,System.Globalization.CultureInfo.DefaultThreadCurrentCulture & DefaultThreadCurrentUICulture is avaiable
        }

        public static CultureInfo GetConfigLang()
        {
            CultureInfo ci = CultureInfoEx.GetCultureInfo(Settings.Instance["language"]);
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
                    if (!isMirrorAvailableChecked) CheckMirror();
                    if (isMirrorAvailable)
                    {
                        log.InfoFormat("old url {0}", url);
                        if (url.Contains("firmware.ardupilot.org"))
                        {
                            url = url.Replace("firmware.ardupilot.org", "firmware.diywrj.com");
                        }
                        else if (url.Contains("firmware.diydrones.com"))
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
                        log.InfoFormat("updated url {0}", url);
                    }
                    break;
                default:
                    break;
            }

            return url;
        }

        public static void CheckMirror()
        {
            switch (ConfigLang.Name)
            {
                case "zh-CN":
                case "zh-Hans":
                    bool isDIYWRJ = CheckHTTP("http://firmware.diywrj.com");
                    bool isDIYDRONES = CheckHTTP("http://firmware.ardupilot.org");
                    bool isGITHUB = Ping("raw.github.com");
                    if (!isDIYWRJ)
                    {
                        string notice =
                            String.Format(
                                "[✘] 奠基网国内镜像\r\n\r\n{0} diydrones官网服务器\r\n\r\n{1} GitHub服务器\r\n\r\n已切换到官网服务器，\r\n您的固件下载和软件更新可能会受到影响。",
                                (isDIYDRONES ? "[✔]" : "[✘]"), (isGITHUB ? "[✔]" : "[✘]"));
                        CustomMessageBox.Show(notice, "服务器连通性检查");
                        isMirrorAvailable = false;
                    }
                    break;
            }
            isMirrorAvailableChecked = true;
        }

        public static bool Ping(string ip)
        {
            try
            {
                System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
                System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
                options.DontFragment = true;
                string data = "MissionPlanner";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 500;
                System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckHTTP(string url)
        {
            try
            {
                HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
                req.Timeout = 500;
                HttpWebResponse response = (HttpWebResponse) req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}