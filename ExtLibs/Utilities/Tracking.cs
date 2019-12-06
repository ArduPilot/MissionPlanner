﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
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
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", version),
                new KeyValuePair<string, string>("tid", tid),
                new KeyValuePair<string, string>("cid", cid.ToString()),
                new KeyValuePair<string, string>("t", "event"),
                new KeyValuePair<string, string>("an", productName),
                new KeyValuePair<string, string>("av", productVersion),

                new KeyValuePair<string, string>("cd", currentscreen),
                new KeyValuePair<string, string>("dp", currentscreen),

                new KeyValuePair<string, string>("ec", cat),
                new KeyValuePair<string, string>("ea", action),
                new KeyValuePair<string, string>("el", label)
            };
            if (value != "")
                param.Add(new KeyValuePair<string, string>("ev", value));


            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", currentCultureName));
            param.Add(new KeyValuePair<string, string>("sd", primaryScreenBitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", boundsWidth + "x" + boundsHeight));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static string productVersion
        {
            get; set;
        }

        public static string productName
        {
            get; set;
        }

        public static int boundsHeight
        {
            get; set;
        }

        public static int boundsWidth
        {
            get; set;
        }

        public static int primaryScreenBitsPerPixel
        {
            get; set;
        }

        public static string currentCultureName { get; set; }

        public static void AddPage(string page, string title)
        {
            // check if we are already here
            if (currentscreen == page)
                return;

            currentscreen = page;

            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", version),
                new KeyValuePair<string, string>("tid", tid),
                new KeyValuePair<string, string>("cid", cid.ToString()),
                new KeyValuePair<string, string>("t", "appview"),
                new KeyValuePair<string, string>("an", productName),
                new KeyValuePair<string, string>("av", productVersion),

                new KeyValuePair<string, string>("cd", page),
                new KeyValuePair<string, string>("dp", page),
                new KeyValuePair<string, string>("dt", title)
            };

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", currentCultureName));
            param.Add(new KeyValuePair<string, string>("sd", primaryScreenBitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", boundsWidth + "x" + boundsHeight));

            Console.WriteLine("Open "+page + " " + title);

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddException(Exception ex)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", version),
                new KeyValuePair<string, string>("tid", tid),
                new KeyValuePair<string, string>("cid", cid.ToString()),
                new KeyValuePair<string, string>("t", "exception"),
                new KeyValuePair<string, string>("an", productName),
                new KeyValuePair<string, string>("av", productVersion),
            };

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

            param.Add(new KeyValuePair<string, string>("ul", currentCultureName));
            param.Add(new KeyValuePair<string, string>("sd", primaryScreenBitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", boundsWidth + "x" + boundsHeight));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddFW(string name, string board)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", version),
                new KeyValuePair<string, string>("tid", tid),
                new KeyValuePair<string, string>("cid", cid.ToString()),
                new KeyValuePair<string, string>("t", "event"),
                new KeyValuePair<string, string>("an", productName),
                new KeyValuePair<string, string>("av", productVersion),

                new KeyValuePair<string, string>("cd", currentscreen),
                new KeyValuePair<string, string>("dp", currentscreen),

                new KeyValuePair<string, string>("ec", "Firmware Upload"),
                new KeyValuePair<string, string>("ea", board),
                new KeyValuePair<string, string>("el", name),

                new KeyValuePair<string, string>("cd2", name),
                new KeyValuePair<string, string>("cd3", board)
            };

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", currentCultureName));
            param.Add(new KeyValuePair<string, string>("sd", primaryScreenBitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", boundsWidth + "x" + boundsHeight));

            System.Threading.ThreadPool.QueueUserWorkItem(track, param);
        }

        public static void AddTiming(string cat, string name, double timeinms, string label)
        {
            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("v", version),
                new KeyValuePair<string, string>("tid", tid),
                new KeyValuePair<string, string>("cid", cid.ToString()),
                new KeyValuePair<string, string>("t", "timing"),
                new KeyValuePair<string, string>("an", productName),
                new KeyValuePair<string, string>("av", productVersion),

                new KeyValuePair<string, string>("cd", currentscreen),
                new KeyValuePair<string, string>("dp", currentscreen),

                new KeyValuePair<string, string>("utc", cat),
                new KeyValuePair<string, string>("utv", name),
                new KeyValuePair<string, string>("utt", ((int)timeinms).ToString()),
                new KeyValuePair<string, string>("utl", label)
            };

            if (sessionstart == false)
            {
                param.Add(new KeyValuePair<string, string>("sc", "start"));
                sessionstart = true;
            }

            param.Add(new KeyValuePair<string, string>("ul", currentCultureName));
            param.Add(new KeyValuePair<string, string>("sd", primaryScreenBitsPerPixel + "-bits"));
            param.Add(new KeyValuePair<string, string>("sr", boundsWidth + "x" + boundsHeight));

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
                httpWebRequest.UserAgent = productName + " " + productVersion + " ("+ Environment.OSVersion.VersionString +")";
                //httpWebRequest.ContentType = "text/plain";
                httpWebRequest.Method = "POST";

                string data = "";

                List<KeyValuePair<string, string>> data1 = (List<KeyValuePair<string, string>>)temp;

                data1.Add(new KeyValuePair<string, string>("cd1", Environment.OSVersion.VersionString));
                data1.Add(new KeyValuePair<string, string>("cd4", Environment.ProcessorCount.ToString()));

                foreach (KeyValuePair<string, string> item in data1)
                {
                    data += "&" + item.Key + "=" + WebUtility.UrlEncode(item.Value);
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

                    using (var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse())
                    {
                        if (httpResponse.StatusCode >= HttpStatusCode.OK && (int) httpResponse.StatusCode < 300)
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
            }
            catch { }
        }
    }
}
