using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BitMiracle.LibTiff.Classic;
using GMap.NET;
using log4net;

namespace MissionPlanner.Utilities
{
    public class DTED
    {
        private static readonly ILog log =
LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //MIL-PRF-89020B
        /*
3.10.7.4.1 DTED Level 1.
Record starts at byte
a. User Header Label (UHL: 80 bytes) 1
b. Data Set Identification Record (DSI: 648 bytes) 81
c. Accuracy Record (ACC: 2700 bytes) 729
d. Data Records (Each data record is 2414 bytes)* 3429
5843
8257
etc
3.10.7.4.2 DTED Level 2.
Record starts at byte
a. User Header Label (UHL: 80 bytes) 1
b. Data Set Identification Record ( DSI: 648 bytes) 81
c. Accuracy Record (ACC: 2700 bytes) 729
d. Data Records (Each data record is 7214 bytes)* 3429
10643
17857
etc
         */

        private static Dictionary<string, short[,]> cache = new Dictionary<string, short[,]>();

        private static List<DTEDdata> index = new List<DTEDdata>();

        static List<String> _customDirectorys = new List<string>();

        public class DTEDdata: IComparer<DTEDdata>, IComparable
        {
            // 80 bytes
            static Regex user_header_label = new Regex("^(UHL)(.)(.{8})(.{8})(.{4})(.{4})(.{4})(.{3})(.{12})(.{4})(.{4})(.{1})(.{24})");
            // 648 bytes
            static Regex data_set_identification = new Regex("^(DSI)(.{1})(.{2})(.{27})(.{26})(.{5})(.{15})(.{8})(.{2})(.{1})(.{4})(.{4})(.{4})(.{8})(.{16})(.{9})(.{2})(.{4})(.{3})(.{5})(.{10})(.{4})(.{22})(.{9})(.{10})(.{7})(.{8})(.{7})(.{8})(.{7})(.{8})(.{7})(.{8})(.{9})(.{4})(.{4})(.{4})(.{4})(.{2})(.{101})(.{100})(.{156})");
            // 2700 bytes
            static Regex accuracy_description = new Regex("^(ACC)(.{4})(.{4})(.{4})(.{4})(.{4})(.{1})(.{31})(.{2}).*(.{18})(.{69})");

            public void LoadFile(string filename)
            {
                log.InfoFormat("DTED {0}", filename);

                var exists = index.Where(a => { return a.FileName.ToLower() == filename.ToLower(); });

                if (exists.Count() > 0)
                {
                    log.InfoFormat("DTED already loaded {0}", filename);
                    return;
                }

                using (var stream = File.OpenRead(filename))
                {
                    byte[] buffer = new byte[80];
                    stream.Read(buffer, 0, buffer.Length);

                    var UHL = user_header_label.Match(ASCIIEncoding.ASCII.GetString(buffer));

                    if (!UHL.Success)
                    {
                        log.ErrorFormat("DTED invalid header {0} - {1}", filename, ASCIIEncoding.ASCII.GetString(buffer).TrimUnPrintable());
                        return;
                    }

                    buffer = new byte[648];
                    stream.Read(buffer, 0, buffer.Length);

                    var DSI = data_set_identification.Match(ASCIIEncoding.ASCII.GetString(buffer));

                    buffer = new byte[2700];
                    stream.Read(buffer, 0, buffer.Length);

                    var ACC = accuracy_description.Match(ASCIIEncoding.ASCII.GetString(buffer));

                    width = int.Parse(UHL.Groups[10].Value);
                    height = int.Parse(UHL.Groups[11].Value);

                    log.InfoFormat("Size ({0},{1})", width, height);

                    // lower left corner
                    x = DDDMMSSH2DD(UHL.Groups[3].Value);
                    y = DDDMMSSH2DD(UHL.Groups[4].Value);

                    log.InfoFormat("Start Point ({0},{1})", x, y);

                    // scales
                    xscale = SSSS2DD(UHL.Groups[5].Value);
                    yscale = SSSS2DD(UHL.Groups[6].Value);

                    log.InfoFormat("Scale ({0},{1})", xscale, yscale);

                    // switch top for bottom
                    y += height * yscale;

                    Area = new RectLatLng(y, x, width * xscale, height * yscale);

                    log.InfoFormat("Coverage {0}", Area.ToString());

                    FileName = filename;
                    index.Add(this);
                }
            }

            public string FileName;
            public int width;
            public int height;
            public double x;
            public double y;
            public double xscale;
            public double yscale;
            public RectLatLng Area;

            double DDDMMSSH2DD(string input)
            {
                var D = int.Parse(input.Substring(0, 3));
                var M = double.Parse(input.Substring(3, 2)) / 60;
                var S = double.Parse(input.Substring(5, 2)) / 60 / 60;
                var H = input.Substring(7, 1);

                var answer = D + M + S;

                if (H == "W" || H == "S")
                    answer *= -1;

                return answer;
            }

            double SSSS2DD(string input)
            {
                // 10ths of seconds
                var S = double.Parse(input) / 60 / 60 / 10;

                return S;
            }

            public int Compare(DTEDdata x, DTEDdata y)
            {
                return Path.GetExtension(y.FileName).CompareTo(Path.GetExtension(x.FileName));
            }

            public int CompareTo(object obj)
            {
                return Compare(this, obj as DTEDdata);
            }
        }

        static DTED()
        {
            AddCustomDirectory(srtm.datadirectory);
        }

        public delegate void Progress(double percent, string message);

        public static event Progress OnProgress;

        public static void AddCustomDirectory(string dir)
        {
            if(!_customDirectorys.Contains(dir))
                _customDirectorys.Add(dir);
            generateIndex();
        }

        private static void generateIndex()
        {
            List<string> files = new List<string>();

            foreach (var dir in _customDirectorys)
            {
                if (!Directory.Exists(dir))
                    continue;

                files.AddRange(Directory.GetFiles(dir, "*.dt2",SearchOption.AllDirectories));
                files.AddRange(Directory.GetFiles(dir, "*.dt1", SearchOption.AllDirectories));
                files.AddRange(Directory.GetFiles(dir, "*.dt0", SearchOption.AllDirectories));
            }

            int i = 0;
            foreach (var file in files)
            {
                i++;
                try
                {
                    if (OnProgress != null)
                        OnProgress((i - 1) / (double)files.Count, file);

                    DTEDdata dtedfile = new DTEDdata();

                    dtedfile.LoadFile(file);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            index.Sort();
        }

        public static srtm.altresponce getAltitude(double lat, double lng, double zoom = 16)
        {
            if (index.Count == 0)
                return srtm.altresponce.Invalid;

            var answer = new srtm.altresponce();

            foreach (var DTEDdata in index)
            {
                if (DTEDdata.Area.Contains(lat, lng))
                {
                    // add to cache
                    if (!cache.ContainsKey(DTEDdata.FileName))
                    {
                        short[,] altdata = new short[DTEDdata.width, DTEDdata.height];

                        using (var stream = File.OpenRead(DTEDdata.FileName))
                        {
                            stream.Seek(3428, SeekOrigin.Begin);

                            for (int b = 0; b < (DTEDdata.height); b++)
                            {
                                var buffer = new byte[DTEDdata.height*2 + 12]; // header + checksum + data(shorts)
                                stream.Read(buffer, 0, buffer.Length);

                                if (buffer[0] == 0xaa)
                                {
                                    int blockno = ((int) buffer[1] << 16) + ((int) buffer[2] << 8) + buffer[3];
                                    int longcount = ((int) buffer[4] << 8) + buffer[5];
                                    int latcount = ((int) buffer[6] << 8) + buffer[7];

                                    for (int a = 0; a < DTEDdata.width; a++)
                                    {
                                        altdata[longcount, latcount + a] =
                                            (short) (((int) buffer[8 + a*2] << 8) + buffer[8 + a*2 + 1]);
                                    }
                                }
                            }
                        }
                        cache[DTEDdata.FileName] = altdata;
                    }

                    // get answer
                    var xf = map(lng, DTEDdata.Area.Left, DTEDdata.Area.Right-DTEDdata.xscale, 0, DTEDdata.width - 1);
                    var yf = map(lat, DTEDdata.Area.Bottom,DTEDdata.Area.Top-DTEDdata.yscale, 0, DTEDdata.height - 1);

                    int x_int = (int)xf;
                    double x_frac = xf - x_int;

                    int y_int = (int)yf;
                    double y_frac = yf - y_int;

                    double alt00 = GetAlt(DTEDdata.FileName, x_int, y_int);
                    double alt10 = GetAlt(DTEDdata.FileName, x_int + 1, y_int);
                    double alt01 = GetAlt(DTEDdata.FileName, x_int, y_int + 1);
                    double alt11 = GetAlt(DTEDdata.FileName, x_int + 1, y_int + 1);

                    double v1 = avg(alt00, alt10, x_frac);
                    double v2 = avg(alt01, alt11, x_frac);
                    double v = avg(v1, v2, y_frac);

                    if (v > -1000)
                        answer.currenttype = srtm.tiletype.valid;
                    answer.alt = v;
                    answer.altsource = "DTED";
                    return answer;
                }
            }

            return srtm.altresponce.Invalid;
        }

        private static double GetAlt(string filename, int x, int y)
        {
            return cache[filename][x, y];
        }

        private static double avg(double v1, double v2, double weight)
        {
            return v2 * weight + v1 * (1 - weight);
        }

        private static double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }
    }
}
