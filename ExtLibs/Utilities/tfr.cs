using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Globalization;
using GMap.NET;

namespace MissionPlanner.Utilities
{
    public class tfr
    {
        //https://www.jeppesen.com/download/fstarfmap/TFR_Protocol.pdf
        //http://www.jepptech.com/tfr/Query.asp?UserID=Public

        public static event EventHandler GotTFRs;

        public static string tfrurl = "http://www.jepptech.com/tfr/Query.asp?UserID=Public";

        public static List<tfritem> tfrs = new List<tfritem>();

        // center then radius
        Regex circle = new Regex(@"[R,B]*C[N,S][0-9\.]+[E,W][0-9\.]+[R][0-9\.]+");

        // center then start arc
        Regex arc = new Regex(@"[R,B]*A[\+,\-][N,S][0-9\.]+[E,W][0-9\.]+[N,S][0-9\.]+[E,W][0-9\.]+");

        // point
        Regex line = new Regex(@"[R,B]*L[N,S][0-9\.]+[E,W][0-9\.]+");

        static Regex all = new Regex(@"([R,B]*(L)([N,S])([0-9\.]+)([E,W])([0-9\.]+)|[R,B]*(A)([\+,\-])([N,S])([0-9\.]+)([E,W])([0-9\.]+)([N,S])([0-9\.]+)([E,W])([0-9\.]+)|[R,B]*(C)([N,S])([0-9\.]+)([E,W])([0-9\.]+)[R]([0-9\.]+))", RegexOptions.Multiline);

        // R is inclusive, B is exclusive

        public static string tfrcache = "tfr.xml";

        public static void GetTFRs()
        {
                var request = WebRequest.Create(tfrurl);

                request.BeginGetResponse(tfrcallback, request);
        }

        private static void tfrcallback(IAsyncResult ar)
        {
            try
            {
                string content = "";

                // check if cache exists and last write was today
                if (File.Exists(tfrcache) &&
                    new FileInfo(tfrcache).LastWriteTime.ToShortDateString() == DateTime.Now.ToShortDateString()) 
                {
                    content = File.ReadAllText(tfrcache);
                }
                else
                {
                    // Set the State of request to asynchronous.
                    WebRequest myWebRequest1 = (WebRequest)ar.AsyncState;

                    using (WebResponse response = myWebRequest1.EndGetResponse(ar))
                    {

                        var st = response.GetResponseStream();

                        StreamReader sr = new StreamReader(st);

                        content = sr.ReadToEnd();

                        File.WriteAllText(tfrcache, content);
                    }
                }

                XDocument xdoc = XDocument.Parse(content);

                tfritem currenttfr = new tfritem();

                for (int a = 1; a < 100; a++)
                {
                    var newtfrs = (from _item in xdoc.Element("TFRSET").Elements("TFR" + a)
                                   select new tfritem
                                   {
                                       ID = _item.Element("ID").Value,
                                       NID = _item.Element("NID").Value,
                                       VERIFIED = _item.Element("VERIFIED").Value,
                                       NAME = _item.Element("NAME").Value,
                                       COMMENT = _item.Element("COMMENT").Value,
                                       ACCESS = _item.Element("ACCESS").Value,
                                       APPEAR = _item.Element("APPEAR").Value,
                                       TYPE = _item.Element("TYPE").Value,
                                       MINALT = _item.Element("MINALT").Value,
                                       MAXALT = _item.Element("MAXALT").Value,
                                       SEGS = _item.Element("SEGS").Value,
                                       BOUND = _item.Element("BOUND").Value,
                                       SRC = _item.Element("SRC").Value,
                                       CREATED = _item.Element("CREATED").Value,
                                       MODIFIED = _item.Element("MODIFIED").Value,
                                       DELETED = _item.Element("DELETED").Value,
                                       ACTIVE = _item.Element("ACTIVE").Value,
                                       EXPIRES = _item.Element("EXPIRES").Value,
                                       SUBMITID = _item.Element("SUBMITID").Value,
                                       SUBMITHOST = _item.Element("SUBMITHOST").Value,
                                   }).ToList();

                    if (newtfrs == null || newtfrs.Count == 0)
                        break;

                    tfrs.AddRange(newtfrs);
                }

                if (GotTFRs != null)
                    GotTFRs(tfrs, null);
            }
            catch {  }
        }

        public class tfritem
        {
            public string ID;// Unique Identifier “42”
            public string DELETED;// Deleted Flag "False"
            public string VERIFIED;
            public string NID;// NOTAM ID "ZAN 1/1109"
            public string NAME;// Name "AK PAVD 1/1109 1 OF 2"
            public string COMMENT;// Comments "...contact your local FSS."
            public string ACCESS;// Access Specifier "Public"
            public string APPEAR;// Appearance "2;0000ff;1;0000ff;30180c060381c060"
            public string TYPE;// Type “Natural Disaster”
            public string MINALT;// Minimum Altitude “2000A”
            public string MAXALT;// Maximum Altitude “5000A”
            public string SEGS;// Segment Count “7”
            public string BOUND;// Boundary "CN38.837606W86.803R005.00"
            public string SRC;// Source Text "!FDC 2/2183 ZID..."
            public string CREATED;// Creation Date/Time "37461.7885756134" and "7/24/2002 6:55:33 PM"
            public string MODIFIED;// Modification Date/Time "37461.7885756134" and "7/24/2002 6:55:33 PM"
            public string ACTIVE;// Activation Date/Time "37461.7885756134" and "7/24/2002 6:55:33 PM"
            public string EXPIRES;// Expiration Date/Time "37461.7885756134" and "7/24/2002 6:55:33 PM"
            public string SUBMITID;// Submitter ID “Publisher”
            public string SUBMITHOST;// Submitter Host “1.2.3.4” or “enduser5.faa.gov”

            public List<List<PointLatLng>> GetPaths()
            {
                //RLN27.576944W97.108611LN27.468056W96.961111LN27.322222W97.050000LN27.345833W97.088889LN27.439167W97.186944RLN27.672778W97.212222LN27.576944W97.108611LN27.533333W97.133333LN27.638333W97.237222RCN27.686333W97.294667R007.00

                List<List<PointLatLng>> list = new List<List<PointLatLng>>();

                List<PointLatLng> pointlist = new List<PointLatLng>();

                var matches = all.Matches(BOUND);

                bool isarcterminate = false;
                bool iscircleterminate = false;
                int arcdir = 0;
                PointLatLngAlt pointcent = null;
                PointLatLngAlt pointstart = null;

                foreach (Match item in matches)
                {
                    try
                    {
                        if (item.Groups[0].Value.ToString().StartsWith("R") || item.Groups[0].Value.ToString().StartsWith("B"))
                        {
                            // start new element
                            if (pointlist.Count > 0)
                            {
                                list.Add(pointlist);
                                pointlist = new List<PointLatLng>();
                            }
                        }

                        if (item.Groups[2].Value == "L")
                        {
                            var point = new PointLatLngAlt(double.Parse(item.Groups[4].Value, CultureInfo.InvariantCulture), double.Parse(item.Groups[6].Value, CultureInfo.InvariantCulture));

                            if (item.Groups[3].Value == "S")
                                point.Lat *= -1;

                            if (item.Groups[5].Value == "W")
                                point.Lng *= -1;

                            if (isarcterminate)
                            {
                                double radius = pointcent.GetDistance(pointstart);

                                double startbearing = pointcent.GetBearing(pointstart);

                                double endbearing = pointcent.GetBearing(point);

                                if (arcdir > 0 && endbearing < startbearing)
                                    endbearing += 360;

                                if (arcdir < 0)
                                {
                                    for (double a = startbearing; a > endbearing; a += (10 * arcdir))
                                    {
                                        pointlist.Add(pointcent.newpos(a, radius));
                                    }
                                }
                                else
                                {
                                    for (double a = startbearing; a < endbearing; a += (10 * arcdir))
                                    {
                                        pointlist.Add(pointcent.newpos(a, radius));
                                    }
                                }

                                pointlist.Add(point);


                                isarcterminate = false;
                                iscircleterminate = false;

                                continue;
                            }

                            if (iscircleterminate)
                            {
                                iscircleterminate = false;
                                continue;
                            }

                            pointlist.Add(point);

                            continue;
                        }
                        else if (item.Groups[7].Value == "A")
                        {
                            pointcent = new PointLatLngAlt(double.Parse(item.Groups[10].Value, CultureInfo.InvariantCulture), double.Parse(item.Groups[12].Value, CultureInfo.InvariantCulture));

                            if (item.Groups[9].Value == "S")
                                pointcent.Lat *= -1;

                            if (item.Groups[11].Value == "W")
                                pointcent.Lng *= -1;

                            pointstart = new PointLatLngAlt(double.Parse(item.Groups[14].Value, CultureInfo.InvariantCulture), double.Parse(item.Groups[16].Value, CultureInfo.InvariantCulture));

                            if (item.Groups[13].Value == "S")
                                pointstart.Lat *= -1;

                            if (item.Groups[15].Value == "W")
                                pointstart.Lng *= -1;

                            arcdir = item.Groups[8].Value == "+" ? 1 : -1;

                            isarcterminate = true;

                            continue;
                        }
                        else if (item.Groups[17].Value == "C")
                        {
                            var point = new PointLatLngAlt(double.Parse(item.Groups[19].Value, CultureInfo.InvariantCulture), double.Parse(item.Groups[21].Value, CultureInfo.InvariantCulture));

                            if (item.Groups[18].Value == "S")
                                point.Lat *= -1;

                            if (item.Groups[20].Value == "W")
                                point.Lng *= -1;

                            // radius in m from nautical miles
                            double radius = double.Parse(item.Groups[22].Value, CultureInfo.InvariantCulture) * 1852;

                            for (int a = 0; a <= 360; a += 10)
                            {
                                pointlist.Add(point.newpos(a, radius));
                            }

                            list.Add(pointlist);
                            pointlist = new List<PointLatLng>();

                            iscircleterminate = true;

                            continue;
                        }
                    }
                    catch { }
                }

                if(pointlist.Count > 0)
                    list.Add(pointlist);

                return list;
            }
        }
    }

}
