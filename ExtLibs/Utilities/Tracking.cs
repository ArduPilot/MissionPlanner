using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Windows.Forms;
using log4net;

namespace MissionPlanner.Utilities
{
    public class Tracking
    {
        private static readonly ILog log =
     LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static string currentscreen = "";

        //https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters


        // cd1 = os
        // cd2 = fw version
        // cd3 = board
        // cd4 = processors
        // cd5 = exception detail

        enum type
        {
            pageview,
            event_,
            transaction,
            item,
            social,
            exception,
            timing,

            appview      ,

        }

        public static bool OptOut = false;

        static string version = "1";
        static string tid = "UA-43098846-1";
        public static Guid cid = new Guid();

        static bool sessionstart = false;

        private static readonly Uri trackingEndpoint = new Uri("http://www.google-analytics.com/collect");
        private static readonly Uri secureTrackingEndpoint = new Uri("https://ssl.google-analytics.com/collect");

        public static void AddEvent(string cat, string action, string label, string value)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("v", version));
            param.Add(new KeyValuePair<string, string>("tid", tid));
            param.Add(new KeyValuePair<string, string>("cid", cid.ToString()));
            param.Add(new KeyValuePair<string, string>("t", "event"));
            param.Add(new KeyValuePair<string, string>("an", Application.ProductName));
            param.Add(new KeyValuePair<string, string>("av", Application.ProductVersion));

            param.Add(new KeyValuePair<string, string>("cd", currentscreen));
            param.Add(new KeyValuePair<string, string>("dp", currentscreen));

            param.Add(new KeyValuePair<string, string>("ec", cat));
            param.Add(new KeyValuePair<string, string>("ea", action));
            param.Add(new KeyValuePair<string, string>("el", label));
            if (value != "")
                param.Add(new KeyValuePair<string, string>("ev", value));


            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", Application.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, string>("sd", Screen.PrimaryScreen.BitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddPage(string page, string title)
        {
            currentscreen = page;

            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("v", version));
            param.Add(new KeyValuePair<string, string>("tid", tid));
            param.Add(new KeyValuePair<string, string>("cid", cid.ToString()));
            param.Add(new KeyValuePair<string, string>("t", "appview"));
            param.Add(new KeyValuePair<string, string>("an", Application.ProductName));
            param.Add(new KeyValuePair<string, string>("av", Application.ProductVersion));

            param.Add(new KeyValuePair<string, string>("cd", page));
            param.Add(new KeyValuePair<string, string>("dp", page));
            param.Add(new KeyValuePair<string, string>("dt", title));

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", Application.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, string>("sd", Screen.PrimaryScreen.BitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height));

            Console.WriteLine("Open "+page + " " + title);

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddException(Exception ex)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("v", version));
            param.Add(new KeyValuePair<string, string>("tid", tid));
            param.Add(new KeyValuePair<string, string>("cid", cid.ToString()));
            param.Add(new KeyValuePair<string, string>("t", "exception"));
            param.Add(new KeyValuePair<string, string>("an", Application.ProductName));
            param.Add(new KeyValuePair<string, string>("av", Application.ProductVersion));

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("cd", currentscreen));
            param.Add(new KeyValuePair<string, string>("dp", currentscreen));

            try
            {
                string reportline = "";

                if (ex.StackTrace != null)
                {
                    string[] lines = ex.StackTrace.Split(new char[] {'\n'});

                    foreach (string line in lines)
                    {
                        if (line.Contains(":line"))
                        {
                            reportline = line;
                            break;
                        }
                    }
                    // 150 bytes

                    reportline =
                        reportline.Replace(@"c:\Users\hog\Documents\Visual Studio 2010\Projects\MissionPlanner.", "");
                }

                param.Add(new KeyValuePair<string, string>("exd", ex.Message + reportline));
            }
            catch 
            { 
                param.Add(new KeyValuePair<string, string>("exd", ex.Message));
            }
            param.Add(new KeyValuePair<string, string>("exf", "0"));
            try
            {
                param.Add(new KeyValuePair<string, string>("cd5", ex.ToString().Substring(0, 140)));
            }
            catch
            {
            }

            param.Add(new KeyValuePair<string, string>("ul", Application.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, string>("sd", Screen.PrimaryScreen.BitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddFW(string name, string board)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("v", version));
            param.Add(new KeyValuePair<string, string>("tid", tid));
            param.Add(new KeyValuePair<string, string>("cid", cid.ToString()));
            param.Add(new KeyValuePair<string, string>("t", "event"));
            param.Add(new KeyValuePair<string, string>("an", Application.ProductName));
            param.Add(new KeyValuePair<string, string>("av", Application.ProductVersion));

            param.Add(new KeyValuePair<string, string>("cd", currentscreen));
            param.Add(new KeyValuePair<string, string>("dp", currentscreen));

            param.Add(new KeyValuePair<string, string>("ec", "Firmware Upload"));
            param.Add(new KeyValuePair<string, string>("ea", board));
            param.Add(new KeyValuePair<string, string>("el", name));

            param.Add(new KeyValuePair<string, string>("cd2", name));
            param.Add(new KeyValuePair<string, string>("cd3", board));

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", Application.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, string>("sd", Screen.PrimaryScreen.BitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddTiming(string cat, string name, double timeinms, string label)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();

            param.Add(new KeyValuePair<string, string>("v", version));
            param.Add(new KeyValuePair<string, string>("tid", tid));
            param.Add(new KeyValuePair<string, string>("cid", cid.ToString()));
            param.Add(new KeyValuePair<string, string>("t", "timing"));
            param.Add(new KeyValuePair<string, string>("an", Application.ProductName));
            param.Add(new KeyValuePair<string, string>("av", Application.ProductVersion));

            param.Add(new KeyValuePair<string, string>("cd", currentscreen));
            param.Add(new KeyValuePair<string, string>("dp", currentscreen));

            param.Add(new KeyValuePair<string, string>("utc", cat));
            param.Add(new KeyValuePair<string, string>("utv", name));
            param.Add(new KeyValuePair<string, string>("utt", ((int)timeinms).ToString()));
            param.Add(new KeyValuePair<string, string>("utl", label));

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", Application.CurrentCulture.Name));
            param.Add(new KeyValuePair<string, string>("sd", Screen.PrimaryScreen.BitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        static void track(object temp)
        {
            if (OptOut)
                return;

            try
            {

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(secureTrackingEndpoint);
                httpWebRequest.ServicePoint.Expect100Continue = false;
                httpWebRequest.UserAgent = Application.ProductName + " " + Application.ProductVersion + " ("+ Environment.OSVersion.VersionString +")";
                //httpWebRequest.ContentType = "text/plain";
                httpWebRequest.Method = "POST";

                string data = "";

                List<KeyValuePair<string, string>> data1 = (List<KeyValuePair<string, string>>)temp;

                data1.Add(new KeyValuePair<string, string>("cd1", Environment.OSVersion.VersionString));
                data1.Add(new KeyValuePair<string, string>("cd4", Environment.ProcessorCount.ToString()));

                foreach (KeyValuePair<string, string> item in data1)
                {
                    data += "&" + item.Key + "=" + HttpUtility.UrlEncode(item.Value);
                }

                data = data.TrimStart(new char[] {'&'});

                Random random = new Random();

                data += "&z=" + random.Next().ToString(CultureInfo.InvariantCulture);

                httpWebRequest.ContentLength = data.Length;

                log.Debug(data);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    streamWriter.Write(data);
                    streamWriter.Flush();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpResponse.StatusCode >= HttpStatusCode.OK && (int)httpResponse.StatusCode < 300)
                    {
                        // response is a gif file
                        log.Debug(httpResponse.StatusCode);
                    }
                    else
                    {
                        log.Debug(httpResponse.StatusCode);
                    }
                }
            }
            catch { }
        }
    }
}
