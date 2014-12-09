using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    public class KIndex
    {
        static string kindexurl = "http://www.swpc.noaa.gov/ftpdir/latest/wwv.txt";

        static Regex kregex = new Regex("K-index at [.]+ was ([0-9]+)");

        public static event EventHandler KIndexEvent;

        public static void GetKIndex()
        {
            var request = WebRequest.Create(kindexurl);

            request.BeginGetResponse(kindexcallback, request);
        }

        private static void kindexcallback(IAsyncResult ar)
        {
            try
            {
                // Set the State of request to asynchronous.
                WebRequest myWebRequest1 = (WebRequest)ar.AsyncState;

                WebResponse response = myWebRequest1.EndGetResponse(ar);

                var st = response.GetResponseStream();

                StreamReader sr = new StreamReader(st);

                string content = sr.ReadToEnd();

                Match match = kregex.Match(content);

                if (match.Success) 
                {
                    string number = match.Groups[1].Value;

                    int kno = int.Parse(number);

                    if (KIndexEvent != null)
                        KIndexEvent(kno, null);

                    return;
                }
            }
            catch 
            {

            }

            if (KIndexEvent != null)
                KIndexEvent(-1, null);
        }
    }
}
