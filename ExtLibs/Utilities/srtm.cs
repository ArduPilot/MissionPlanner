using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
using System.Collections;
using log4net;

namespace MissionPlanner
{
    public class srtm: IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public enum tiletype
        {
            valid,
            invalid,
            ocean
        }

        public class altresponce
        {
            public tiletype currenttype = tiletype.invalid;
            public double alt = 0;
        }

        public static string datadirectory = "./srtm/";

        static List<string> allhgts = new List<string>();

        static object objlock = new object();

        static Thread requestThread;

        static bool requestThreadrun = false;

        static List<string> queue = new List<string>();

        static Hashtable fnamecache = new Hashtable();

        static Hashtable filecache = new Hashtable();

        static List<string> oceantile = new List<string>();

        static Dictionary<string, short[,]> cache = new Dictionary<string, short[,]>();

        static srtm()
        {

        }

        public static altresponce getAltitude(double lat, double lng, double zoom = 16)
        {
            short alt = 0;
            var answer = new altresponce();

            var trytiff = Utilities.GeoTiff.getAltitude(lat, lng);

            if (trytiff.currenttype == tiletype.valid)
                return trytiff;

            //lat += 1 / 1199.0;
            //lng -= 1 / 1201f;

            // 		lat	-35.115676879882812	double
            //		lng	117.94178754638671	double
            // 		alt	70	short

            int x = (lng < 0) ? (int)(lng - 1) : (int)lng;//(int)Math.Floor(lng);
            int y = (lat < 0) ? (int)(lat - 1) : (int)lat; ;//(int)Math.Floor(lat);

            string ns;
            if (y > 0)
                ns = "N";
            else
                ns = "S";

            string ew;
            if (x > 0)
                ew = "E";
            else
                ew = "W";

            // running tostring at a high rate was costing cpu
            if (fnamecache[y] == null)
                fnamecache[y] = Math.Abs(y).ToString("00");
            if (fnamecache[1000 + x] == null)
                fnamecache[1000 + x] = Math.Abs(x).ToString("000");

            string filename = ns + fnamecache[y] + ew + fnamecache[1000 + x] + ".hgt";

            try
            {

                if (cache.ContainsKey(filename) || File.Exists(datadirectory + Path.DirectorySeparatorChar + filename))
                { // srtm hgt files

                    int size = -1;

                    // add to cache
                    if (!cache.ContainsKey(filename))
                    {
                        FileStream fs = new FileStream(datadirectory + Path.DirectorySeparatorChar + filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                        if (fs.Length == (1201 * 1201 * 2))
                        {
                            size = 1201;
                        }
                        else if (fs.Length == (3601 * 3601 * 2))
                        {
                            size = 3601;
                        }
                        else
                            return answer;

                        byte[] altbytes = new byte[2];
                        short[,] altdata = new short[size, size];


                        int altlat = 0;
                        int altlng = 0;

                        while (fs.Read(altbytes, 0, 2) != 0)
                        {
                            altdata[altlat, altlng] = (short)((altbytes[0] << 8) + altbytes[1]);

                            altlat++;
                            if (altlat >= size)
                            {
                                altlng++;
                                altlat = 0;
                            }
                        }

                        fs.Close();

                        cache[filename] = altdata;
                    }

                    if (cache[filename].Length == (1201 * 1201))
                    {
                        size = 1201;
                    }
                    else if (cache[filename].Length == (3601 * 3601))
                    {
                        size = 3601;
                    }
                    else
                        return answer;

                    // remove the base lat long
                    lat -= y;
                    lng -= x;

                    // values should be 0-1199, 1200 is for interpolation
                    double xf = lng * (size - 2);
                    double yf = lat * (size - 2);

                    int x_int = (int)xf;
                    double x_frac = xf - x_int;

                    int y_int = (int)yf;
                    double y_frac = yf - y_int;

                    y_int = (size - 2) - y_int;

                    double alt00 = GetAlt(filename, x_int, y_int);
                    double alt10 = GetAlt(filename, x_int + 1, y_int);
                    double alt01 = GetAlt(filename, x_int, y_int + 1);
                    double alt11 = GetAlt(filename, x_int + 1, y_int + 1);

                    double v1 = avg(alt00, alt10, x_frac);
                    double v2 = avg(alt01, alt11, x_frac);
                    double v = avg(v1, v2, -y_frac);

                    answer.currenttype = tiletype.valid;
                    answer.alt = v;
                    return answer;
                }

                string filename2 = "srtm_" + Math.Round((lng + 2.5 + 180) / 5, 0).ToString("00") + "_" + Math.Round((60 - lat + 2.5) / 5, 0).ToString("00") + ".asc";

                if (File.Exists(datadirectory + Path.DirectorySeparatorChar + filename2))
                {
                    StreamReader sr = new StreamReader(readFile(datadirectory + Path.DirectorySeparatorChar + filename2));

                    int nox = 0;
                    int noy = 0;
                    float left = 0;
                    float top = 0;
                    int nodata = -9999;
                    float cellsize = 0;

                    int rowcounter = 0;

                    float wantrow = 0;
                    float wantcol = 0;


                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();

                        if (line.StartsWith("ncols"))
                        {
                            nox = int.Parse(line.Substring(line.IndexOf(' ')));

                            //hgtdata = new int[nox * noy];
                        }
                        else if (line.StartsWith("nrows"))
                        {
                            noy = int.Parse(line.Substring(line.IndexOf(' ')));

                            //hgtdata = new int[nox * noy];
                        }
                        else if (line.StartsWith("xllcorner"))
                        {
                            left = float.Parse(line.Substring(line.IndexOf(' ')));
                        }
                        else if (line.StartsWith("yllcorner"))
                        {
                            top = float.Parse(line.Substring(line.IndexOf(' ')));
                        }
                        else if (line.StartsWith("cellsize"))
                        {
                            cellsize = float.Parse(line.Substring(line.IndexOf(' ')));
                        }
                        else if (line.StartsWith("NODATA_value"))
                        {
                            nodata = int.Parse(line.Substring(line.IndexOf(' ')));
                        }
                        else
                        {
                            string[] data = line.Split(new char[] { ' ' });

                            if (data.Length == (nox + 1))
                            {



                                wantcol = (float)((lng - Math.Round(left, 0)));

                                wantrow = (float)((lat - Math.Round(top, 0)));

                                wantrow = (int)(wantrow / cellsize);
                                wantcol = (int)(wantcol / cellsize);

                                wantrow = noy - wantrow;

                                if (rowcounter == wantrow)
                                {
                                    Console.WriteLine("{0} {1} {2} {3} ans {4} x {5}", lng, lat, left, top, data[(int)wantcol], (nox + wantcol * cellsize));

                                    answer.currenttype = tiletype.valid;
                                    answer.alt = int.Parse(data[(int)wantcol]);
                                    return answer;
                                }

                                rowcounter++;
                            }
                        }



                    }

                    //sr.Close();
                    answer.currenttype = tiletype.valid;
                    answer.alt = alt;
                    return answer;
                }
                else // get something
                {
                    if (filename.Contains("S00W000") || filename.Contains("S00W001") ||
                        filename.Contains("S01W000") || filename.Contains("S01W001"))
                    {
                        answer.currenttype = tiletype.ocean;
                        return answer;
                    }

                    if (oceantile.Contains(filename))
                        answer.currenttype = tiletype.ocean;

                    if (zoom >= 7)
                    {
                        if (!Directory.Exists(datadirectory))
                            Directory.CreateDirectory(datadirectory);

                        if (requestThread == null)
                        {
                            log.Info("Getting " + filename);
                            lock (objlock)
                            {
                                queue.Add(filename);
                            }

                            requestThread = new Thread(requestRunner);
                            requestThread.IsBackground = true;
                            requestThread.Name = "SRTM request runner";
                            requestThread.Start();
                        }
                        else
                        {
                            lock (objlock)
                            {
                                if (!queue.Contains(filename))
                                {
                                    log.Info("Getting " + filename);
                                    queue.Add(filename);
                                }
                            }
                        }
                    }
                }

            }
            catch { answer.alt = 0; answer.currenttype = tiletype.invalid; }

            return answer;
        }

        static double GetAlt(string filename, int x, int y)
        {
            return cache[filename][x, y];
        }

        static double avg(double v1, double v2, double weight)
        {
            return v2 * weight + v1 * (1 - weight);
        }

        static MemoryStream readFile(string filename)
        {
            if (filecache.ContainsKey(filename))
            {
                return (MemoryStream)filecache[filename];
            }
            else
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

                byte[] file = new byte[fs.Length];
                fs.Read(file, 0, (int)fs.Length);

                filecache[filename] = new MemoryStream(file);

                fs.Close();

                return (MemoryStream)filecache[filename];
            }
        }

        static void requestRunner()
        {
            requestThreadrun = true;

            while (requestThreadrun)
            {
                try
                {
                    string item = "";
                    lock (objlock)
                    {
                        if (queue.Count > 0)
                        {
                            item = queue[0];
                        }
                    }

                    if (item != "")
                    {
                        get3secfile(item);
                        lock (objlock)
                        {
                            queue.Remove(item);
                        }
                    }
                }
                catch (Exception ex)
                { 
                    log.Error(ex);
                }
                Thread.Sleep(1000);
            }
        }

        static void get3secfile(object name)
        {
            //string baseurl1sec = "http://dds.cr.usgs.gov/srtm/version2_1/SRTM1/";
            string baseurl = "http://firmware.ardupilot.org/SRTM/";

            // check file doesnt already exist
            if (File.Exists(datadirectory + Path.DirectorySeparatorChar + (string)name))
            {
                FileInfo fi = new FileInfo(datadirectory + Path.DirectorySeparatorChar + (string)name);
                if (fi.Length != 0)
                    return;
            }

            int checkednames = 0;
            List<string> list = new List<string>();

            // load 1 arc seconds first
            //list.AddRange(getListing(baseurl1sec));
            // load 3 arc second
            list.AddRange(getListing(baseurl));

            foreach (string item in list)
            {
                List<string> hgtfiles = new List<string>();

                hgtfiles = getListing(item);

                foreach (string hgt in hgtfiles)
                {
                    checkednames++;
                    if (hgt.Contains((string)name))
                    {
                        // get file

                        gethgt(hgt, (string)name);
                        return;
                    }
                }
            }

            // if there are no http exceptions, and the list is >= 20, then everything above is valid
            // 15760 is all srtm3 and srtm1
            if (list.Count >= 12 && checkednames > 14000 && !oceantile.Contains((string)name))
            {
                // we must be an ocean tile - no matchs
                oceantile.Add((string)name);
            }
        }

        static void gethgt(string url, string filename)
        {
            try
            {
                WebRequest req = HttpWebRequest.Create(url);

                log.Info("Get " + url);

                using (WebResponse res = req.GetResponse())
                using (Stream resstream = res.GetResponseStream())
                using (BinaryWriter bw = new BinaryWriter(File.Create(datadirectory + Path.DirectorySeparatorChar + filename + ".zip")))
                {
                    byte[] buf1 = new byte[1024];

                    int size = 0;

                    while (resstream.CanRead)
                    {

                        int len = resstream.Read(buf1, 0, 1024);
                        if (len == 0)
                            break;
                        bw.Write(buf1, 0, len);

                        size += len;
                    }

                    bw.Close();

                    log.Info("Got " + url + " " + size);

                    FastZip fzip = new FastZip();

                    fzip.ExtractZip(datadirectory + Path.DirectorySeparatorChar + filename + ".zip", datadirectory, "");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        static List<string> getListing(string url)
        {
            string name = new Uri(url).AbsolutePath;

            name = Path.GetFileName(name.TrimEnd('/'));

            List<string> list = new List<string>();

            if (File.Exists(datadirectory + Path.DirectorySeparatorChar + name))
            {
                using (StreamReader sr = new StreamReader(datadirectory + Path.DirectorySeparatorChar + name))
                {
                    while (!sr.EndOfStream)
                    {
                        list.Add(sr.ReadLine());
                    }

                    sr.Close();
                }
                return list;
            }

            try
            {
                log.Info("srtm req " + url);

                WebRequest req = HttpWebRequest.Create(url);

                using (WebResponse res = req.GetResponse())
                using (StreamReader resstream = new StreamReader(res.GetResponseStream()))
                {

                    string data = resstream.ReadToEnd();

                    Regex regex = new Regex("href=\"([^\"]+)\"", RegexOptions.IgnoreCase);
                    if (regex.IsMatch(data))
                    {
                        MatchCollection matchs = regex.Matches(data);
                        for (int i = 0; i < matchs.Count; i++)
                        {
                            if (matchs[i].Groups[1].Value.ToString().Contains(".."))
                                continue;
                            if (matchs[i].Groups[1].Value.ToString().Contains("http"))
                                continue;
                            if (matchs[i].Groups[1].Value.ToString().EndsWith("/srtm/version2_1/"))
                                continue;

                            list.Add(url.TrimEnd(new char[] {'/', '\\'}) + "/" + matchs[i].Groups[1].Value.ToString());
                        }
                    }
                }

                using (StreamWriter sw = new StreamWriter(datadirectory + Path.DirectorySeparatorChar + name))
                {
                    list.ForEach(x =>
                    {
                        sw.WriteLine((string)x);
                    });

                    sw.Close();
                }
            }
            catch (WebException ex)
            {
                log.Error(ex);
                throw;
            }

            return list;
        }

        public void Dispose()
        {
            requestThreadrun = false;
        }
    }
}