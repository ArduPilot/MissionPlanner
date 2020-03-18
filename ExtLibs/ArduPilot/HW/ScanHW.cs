using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Flurl.Util;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissionPlanner.Utilities.HW
{
    public class ScanHW
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static List<List<string>> spidevlist = new List<List<string>>();
        static List<List<string>> i2cdevlist = new List<List<string>>();

        static Regex spidev =
            new Regex(@"^(SPIDEV)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)\s+([^\s]+)", RegexOptions.Multiline);

        static Regex i2cdev = new Regex(@"^(BARO|IMU|COMPASS)\s+([^\s]+)\s+([^\s:]+):([^\s:]+):([^\s]+)", RegexOptions.Multiline);

        public static void Generate(string dir)
        {
            var files = Directory.GetFiles(dir, "hwdef*.dat", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var content = File.ReadAllText(file);

                var spimatches = spidev.Matches(content);
                var i2cmatches = i2cdev.Matches(content);

                foreach (Match spimatch in spimatches)
                {
                    //new hwdefspi()
                    var devname = spimatch.Groups[2].Value;
                    var spibus = spimatch.Groups[3].Value;
                    var devid = spimatch.Groups[4].Value;
                    var devcs = spimatch.Groups[5].Value;
                    var devmode = spimatch.Groups[6].Value;
                    var devspeed1 = spimatch.Groups[7].Value;
                    var devspeed2 = spimatch.Groups[8].Value;

                    Console.WriteLine("{0}", devname);
                    
                    spidevlist.Add(spimatch.Groups.Cast<Group>().Select(a => a.Value).ToList());
                }

                foreach (Match i2Cmatch in i2cmatches)
                {
                    var devtype = i2Cmatch.Groups[1].Value;
                    var devname = i2Cmatch.Groups[2].Value;
                    var bus = i2Cmatch.Groups[3].Value;
                    var busno = i2Cmatch.Groups[4].Value;
                    var i2cadd = i2Cmatch.Groups[5].Value;

                    Console.WriteLine("{0} {1} {2}:{3}", devtype, devname, busno, i2cadd);

                    i2cdevlist.Add(i2Cmatch.Groups.Cast<Group>().Select(a => a.Value).ToList());
                }
            }

            File.WriteAllLines(Settings.GetUserDataDirectory() + "spilist.txt",
                spidevlist.GroupBy(a => a[2]).Select(a => a.First().ToJSON()));

            File.WriteAllLines(Settings.GetUserDataDirectory() + "i2clist.txt",
                i2cdevlist.GroupBy(a => a[2]+a[4]+a[5]).Select(a => a.First().ToJSON()));

            spidevlist= spidevlist.GroupBy(a => a[2]).Select(a => a.First()).ToList();
            i2cdevlist = i2cdevlist.GroupBy(a => a[2] + a[4] + a[5]).Select(a => a.First()).ToList();
        }

        public static void Scan(MAVLinkInterface mav)
        {
            // look for valid devnames
            foreach (var spidev in spidevlist)
            {
                byte[] outBytes = new byte[0];
                var obj = spidev;//JsonConvert.DeserializeObject<List<string>>(spidev);
                if (mav.device_op(mav.MAV.sysid, mav.MAV.compid, out outBytes, MAVLink.DEVICE_OP_BUSTYPE.SPI, obj[2], 0,
                        0, 0xff, 0) == (byte) device_op_error.BadResponse)
                {
                    log.Info(obj[2] + " spi driver available");
                }
            }

            foreach (var i2cdev in i2cdevlist)
            {
                byte[] outBytes = new byte[0];
                var obj = i2cdev;//JsonConvert.DeserializeObject<List<string>>(i2cdev);
                try
                {
                    if (mav.device_op(mav.MAV.sysid, mav.MAV.compid, out outBytes, MAVLink.DEVICE_OP_BUSTYPE.I2C, "",
                            Convert.ToByte(obj[4]),
                            Convert.ToByte(obj[5], 16), 0xff, 0) == (byte) device_op_error.BadResponse)
                    {
                        log.Info(obj[2] + " i2c driver available");
                    }
                }
                catch
                {

                }
            }

        }
    }

    public enum device_op_error
    {
        OK = 0,
        BadBus = 1,
        BadDev = 2,
        BadSemaphore = 3,
        BadResponse = 4,
    }

    public class hwdefspi
    {
        public string SPIDEV { get; }
        public string devname { get; }
        public string spibus { get; }
        public string devid { get; }
        public string devcs { get; }

        public string devmode { get; }

        public string devspeedinit { get; }
        public string devspeedrun { get; }
    }

    public class hwdefi2c
    {

    }

    public class Query
    {
        public string data { get; set; }
        public int length { get; set; }
    }

    public class Scan
    {
        public string type { get; set; }
        public string name { get; set; }
        public string device { get; set; }
        public Query query { get; set; }
        public List<string> response { get; set; }
    }

    public class RootObject
    {
        public string name { get; set; }
        public List<Scan> scan { get; set; }
    }
}
