using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using log4net;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MissionPlanner.Utilities
{
    public class adsb
    {
        //http://www.lll.lu/~edward/edward/adsb/DecodingADSBposition.html
        //http://adsb.tc.faa.gov/WG3_Meetings/Meeting30/1090-WP30-21-Appendix_A%20Mods.pdf
        //http://adsb.tc.faa.gov/WG3_Meetings/Meeting9/1090-WP-9-14.pdf
        //*8D75804B580FF2CF7E9BA6F701D0
        //*8D75804B580FF6B283EB7A157117
        //https://www.adsbexchange.com/data/
        //https://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat=33.433638&lng=-112.008113&fDstL=0&fDstU=100

        private static readonly ILog log =        LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// When a plane position has been updated. you will need to age your own entries
        /// </summary>
        public static event EventHandler<MissionPlanner.Utilities.adsb.PointLatLngAltHdg> UpdatePlanePosition;

        public static PointLatLngAlt CurrentPosition = PointLatLngAlt.Zero;

        static bool run = false;
        static Thread thisthread;

        public static string server = "";
        public static int serverport = 0;

        private static int adsbexchangerange = 100;

        public adsb()
        {
            log.Info("adsb ctor");

            thisthread = new Thread(TryConnect);

            thisthread.Name = "ADSB reader thread";

            thisthread.IsBackground = true;

            thisthread.Start();
        }

        public static void Stop()
        {
            log.Info("adsb stop");
            run = false;

            if (thisthread != null)
            {
                thisthread.Abort();
                thisthread.Join();
                thisthread = null;
            }

            log.Info("adsb stopped");
        }

        void TryConnect()
        {
            run = true;

            while (run)
            {
                log.Info("adsb connect loop");
                //custom
                try
                {
                    if (!String.IsNullOrEmpty(server))
                    {
                        using (TcpClient cl = new TcpClient())
                        {
                            cl.Connect(server, serverport);

                            log.Info("Connected " + server + ":" + serverport);

                            ReadMessage(cl.GetStream());
                        }
                    }
                }
                catch (Exception ex) { log.Error(ex); }

                // dump1090 sbs
                try
                {
                    using (TcpClient cl = new TcpClient())
                    {

                        cl.Connect(System.Net.IPAddress.Loopback, 30003);

                        log.Info("Connected loopback:30003");

                        ReadMessage(cl.GetStream());
                    }
                }
                catch (Exception) {  }

                // dump1090 avr
                try
                {
                    using (TcpClient cl = new TcpClient())
                    {

                        cl.Connect(System.Net.IPAddress.Loopback, 30002);

                        log.Info("Connected loopback:30002");

                        ReadMessage(cl.GetStream());
                    }
                }
                catch (Exception) {  }


                // rtl1090 -sbs1
                try
                {
                    using (TcpClient cl = new TcpClient())
                    {

                        cl.Connect(System.Net.IPAddress.Loopback, 31004);

                        log.Info("Connected loopback:31004");

                        ReadMessage(cl.GetStream());
                    }
                }
                catch (Exception) { }

                // rtl1090 - avr
                try
                {
                    using (TcpClient cl = new TcpClient())
                    {

                        cl.Connect(System.Net.IPAddress.Loopback, 31001);

                        log.Info("Connected loopback:31001");

                        ReadMessage(cl.GetStream());
                    }
                }
                catch (Exception) {  }


                // adsb#
                try
                {
                    using (TcpClient cl = new TcpClient())
                    {

                        cl.Connect(System.Net.IPAddress.Loopback, 47806);

                        log.Info("Connected loopback:47806");

                        ReadMessage(cl.GetStream());
                    }
                }
                catch (Exception) {  }

                // adsbexchange
                try
                {

                    if (CurrentPosition != PointLatLngAlt.Zero)
                    {
                        string url =
                            "http://public-api.adsbexchange.com/VirtualRadar/AircraftList.json?lat={0}&lng={1}&fDstL=0&fDstU={2}";
                        string path = Settings.GetDataDirectory() + Path.DirectorySeparatorChar + "adsb.json";
                        var ans = Download.getFilefromNet(String.Format(url, CurrentPosition.Lat, CurrentPosition.Lng, adsbexchangerange),
                            path);

                        if (ans)
                        {
                            var result = JsonConvert.DeserializeObject<RootObject>(File.ReadAllText(path));

                            if (result.acList.Count < 1)
                                adsbexchangerange = Math.Min(adsbexchangerange + 10, 400);

                            foreach (var acList in result.acList)
                            {
                                var plane = new MissionPlanner.Utilities.adsb.PointLatLngAltHdg(acList.Lat, acList.Long,
                                    acList.Alt * 0.3048,
                                    (float) acList.Trak, acList.Icao, DateTime.Now);

                                UpdatePlanePosition(null, plane);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }

                // cleanup any sockets that might be outstanding.
                GC.Collect();
                System.Threading.Thread.Sleep(5000);
            }

            log.Info("adsb thread exit");
        }

        public class Feed
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool polarPlot { get; set; }
        }

        public class AcList
        {
            public int Id { get; set; }
            public int Rcvr { get; set; }
            public bool HasSig { get; set; }
            public int Sig { get; set; }
            public string Icao { get; set; }
            public bool Bad { get; set; }
            public string Reg { get; set; }
            public string FSeen { get; set; }
            public int TSecs { get; set; }
            public int CMsgs { get; set; }
            public int Alt { get; set; }
            public int GAlt { get; set; }
            public double InHg { get; set; }
            public int AltT { get; set; }
            public string Call { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
            public string PosTime { get; set; }
            public bool Mlat { get; set; }
            public bool Tisb { get; set; }
            public double Spd { get; set; }
            public double Trak { get; set; }
            public bool TrkH { get; set; }
            public string Type { get; set; }
            public string Mdl { get; set; }
            public string Man { get; set; }
            public string CNum { get; set; }
            public string Op { get; set; }
            public string OpIcao { get; set; }
            public string Sqk { get; set; }
            public int Vsi { get; set; }
            public int VsiT { get; set; }
            public double Dst { get; set; }
            public double Brng { get; set; }
            public int WTC { get; set; }
            public int Species { get; set; }
            public string Engines { get; set; }
            public int EngType { get; set; }
            public int EngMount { get; set; }
            public bool Mil { get; set; }
            public string Cou { get; set; }
            public bool HasPic { get; set; }
            public bool Interested { get; set; }
            public int FlightsCount { get; set; }
            public bool Gnd { get; set; }
            public int SpdTyp { get; set; }
            public bool CallSus { get; set; }
            public int Trt { get; set; }
            public string Year { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public bool? Help { get; set; }
        }

        public class RootObject
        {
            public int src { get; set; }
            public List<Feed> feeds { get; set; }
            public int srcFeed { get; set; }
            public bool showSil { get; set; }
            public bool showFlg { get; set; }
            public bool showPic { get; set; }
            public int flgH { get; set; }
            public int flgW { get; set; }
            public List<AcList> acList { get; set; }
            public int totalAc { get; set; }
            public string lastDv { get; set; }
            public int shtTrlSec { get; set; }
            public long stm { get; set; }
        }

        static Hashtable Planes = new Hashtable();

        public class Plane
        {
            internal ModeSMessage llaeven;
            internal ModeSMessage llaodd;

            public string ID;
            public string CallSign;

            static double[] _NLTable;
            const int NZ = 15;
            const int Nb = 17;
            const Double AirDlat0 = 6;
            const Double AirDlat1 = 360.0 / 59.0;

            double reflat = -34.988;
            double reflng = 117.8574;

            public double heading = 0;

            public Plane()
            {
                BuildNLTable();
            }

            public PointLatLngAlt plla()
            {
                if (llaodd == null || llaeven == null)
                    return PointLatLngAlt.Zero;

                if (Math.Abs((llaeven.recvtime - llaodd.recvtime).TotalSeconds) > 30)
                    return PointLatLngAlt.Zero;

                //A.1.7.7 Globally Unambiguous Airborne Position Decoding

                // coordinate2 = odd

                double j = Math.Floor((((59.0 * llaeven.lat) - (60.0 * llaodd.lat)) / 131072.0) + 0.5); // numerator

                double[] rlat = new double[2];

                rlat[0] = AirDlat0 * (modulo(j, 60) + llaeven.lat / 131072.0);
                rlat[1] = AirDlat1 * (modulo(j, 59) + llaodd.lat / 131072.0);

                if (rlat[0] >= 270)
                    rlat[0] -= 360;

                if (rlat[1] >= 270)
                    rlat[1] -= 360;

                int i1 = NL(rlat[0]);
                int i2 = NL(rlat[1]);

                if (i1 != i2)
                    return PointLatLngAlt.Zero;

                double inside = ((llaeven.lng * (i1 - 1)) - (llaodd.lng * i1)); // num12

                double M = Math.Floor((inside / 131072.0) + 0.5); // num13

                int ni0 = Math.Max(i1 - (0), 1);
                int ni1 = Math.Max(i1 - (1),1); // num14

                double dlon0 = 360.0 / ni0;
                double dlon1 = 360.0 / ni1; // num15   

                double[] rlng = new double[2];

                rlng[0] = dlon0 * (modulo(M, ni0) + (llaeven.lng / 131072.0)); 
                rlng[1] = dlon1 * (modulo(M, ni1) + (llaodd.lng / 131072.0)); // longitude

                rlng[0] = modulo(rlng[0] + 180.0, 360.0) - 180.0;
                rlng[1] = modulo(rlng[1] + 180.0, 360.0) - 180.0;

                // setup ref
                if (llaodd.recvtime < llaeven.recvtime)
                {
                    reflat = rlat[0];
                    reflng = rlng[0];
                }
                else
                {
                    reflat = rlat[1];
                    reflng = rlng[1];
                }

                return new PointLatLngAlt(reflat, reflng, llaodd.alt * 0.3048, ID);
            }

            public PointLatLngAlt pllalocal(ModeSMessage newmsg)
            {
                if (newmsg == null)
                    return PointLatLngAlt.Zero;

                int num = 131072;
                double denominator = (newmsg.OddFormat ? 6.101694915254237 : 6.0);
                double num3 =  360.0;
                double latitude = reflat;
                double num5 = ((double)newmsg.lat) / ((double)num);
                double num6 = Math.Floor((double)(latitude / denominator)) + Math.Floor((double)((0.5 + (this.modulo(latitude, denominator) / denominator)) - num5));
                double lat = denominator * (num6 + num5);
                int num7 = this.NL(newmsg.lat) - (newmsg.OddFormat ? 1 : 0);

                double numerator = reflng;
                double num9 = (num7 == 0) ? num3 : (num3 / ((double)num7));
                double num10 = ((double)newmsg.lng) / ((double)num);
                double num11 = Math.Floor((double)(numerator / num9)) + Math.Floor((double)((0.5 + (this.modulo(numerator, num9) / num9)) - num10));
                double lng = num9 * (num11 + num10);

                return new PointLatLngAlt(lat,lng);
            }
            /*
            internal int NL(double lat)
            {
                return (int)Math.Floor((2 * Math.PI) / (Math.Acos(1 - ((1 - Math.Cos(Math.PI / (2 * NZ))) / (Math.Pow(Math.Cos((Math.PI / 180) * Math.Abs(lat)), 2))))));
            }
            */
            private int NL(double latitude)
            {
                if (latitude < 0.0)
                {
                    latitude = -latitude;
                }
                if (latitude <= 87.0)
                {
                    for (int i = 0; i < _NLTable.Length; i++)
                    {
                        if (latitude <= _NLTable[i])
                        {
                            return (0x3b - i);
                        }
                    }
                }
                return 1;
            }
            
            internal double modulo(double numerator, double denominator)
            {
                double num = numerator - (denominator * Math.Floor((double)(numerator / denominator)));
                return ((num < 0.0) ? (num + denominator) : num);
            }

            private void BuildNLTable()
            {
                _NLTable = new double[0x3a];
                double num = 57.295779513082323;
                double num2 = 1.0 - Math.Cos(0.10471975511965977);
                int num3 = 0x3b;
                for (int i = 0; i < 0x3a; i++)
                {
                    double num5 = 1.0 - Math.Cos(6.2831853071795862 / ((double)num3));
                    double d = num2 / num5;
                    double num7 = Math.Sqrt(d);
                    _NLTable[i] = num * Math.Acos(num7);
                    num3--;
                }
            }

 

        }

        public class ModeSMessage
        {
            public byte DF; // 5 bits - downlink format
            public byte CA; // 3 bits - capability
            public uint AA; // 24 bits - icao24
            public byte TypeCode { get { return (byte)(adsbdata[0] >> 3); } }
            public byte[] adsbdata = new byte[7];
            public uint PI; // 24 bits

            public string Ident;

            public DateTime recvtime = DateTime.MinValue;

            // 17
            internal uint lat { get { byte[] data = getbits(this.adsbdata, 21, 17); return (uint)((data[2] << 16) + (data[1] << 8) + (data[0])); } }
            internal uint lng { get { byte[] data = getbits(this.adsbdata, 38, 17); return (uint)((data[2] << 16) + (data[1] << 8) + (data[0])); } }
            public bool Timeflag { get { return ((adsbdata[2] & 8) > 0); } }
            public bool Fcprformat { get { return ((adsbdata[2] & 4) > 0); } } // 0 = even, 1 = odd
            public bool OddFormat { get { return Fcprformat; } }

            public byte[] getbits(byte[] input, int startbit,int bitcount)
            {
                byte[] ans = new byte[bitcount / 8 + 1];

                BitArray ba = new BitArray(input);
                BitArray banswer = new BitArray(bitcount);

                int bitsdone = 0;

                for (int a = startbit + bitcount; a > startbit; a--) 
                {
                    int no = ((a / 8) * 8) + (7 - (a % 8));
                    bool bit = ba.Get(no);
                    //Console.SetCursorPosition(a, Console.CursorTop);
                    //Console.Write(bit ? "1" : "0");
                    banswer.Set(bitsdone, bit);
                    bitsdone++;
                }

                banswer.CopyTo(ans,0);

                return ans;
            }

            internal int alt
            {
                get
                {
                    // 8th bit... posibly wrong
                    int alt = (adsbdata[1] << 4) + (adsbdata[2]>>4);

                    int acCode = ((alt & 0xfe0) >> 1) | (alt & 15);

                    if ((alt & 0x10) != 0)
                    {
                        alt = (acCode * 25) - 1000;
                        //alt = 0;
                    }
                    else 
                    {
                        alt = LookupGillhamAltitude(acCode).Value;
                       // alt = 0;
                    }

                    return alt;
                }
            }

             int[] _ACCodeToIndexOffsets = new int[] { 
        0x40, 0x40, 0x40, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0xc0, 0xc0, 0x100, 0x100, 0x100, 0x100, 0x100, 
        0x100, 0x100, 320, 320, 0x180, 0x180, 0x1c0, 0x1c0, 0x200, 0x200, 0x240, 0x240, 640, 640, 0x2c0, 0x2c0
     };


             int[] _LookupTable = new int[] { 
        -1200, 0xf4ec, -300, 0xf168, 0x1eeec, 0xf550, 0x1eb68, 0xf8d4, 700, 0xed80, -200, 0xf104, 0x1e780, 0xfcbc, 0x1eb04, 0xf938, 
        0xa8c, 0xe5b0, 0x708, 0xe934, 0x1dfb0, 0x1048c, 0x1e334, 0x10108, 800, 0xed1c, 0x6a4, 0xe998, 0x1e71c, 0xfd20, 0x1e398, 0x100a4, 
        0x1a2c, 0xd610, 0x16a8, 0xd994, 0x1d010, 0x1142c, 0x1d394, 0x110a8, 0x12c0, 0xdd7c, 0x1644, 0xd9f8, 0x1d77c, 0x10cc0, 0x1d3f8, 0x11044, 
        0xaf0, 0xe54c, 0xe74, 0xe1c8, 0x1df4c, 0x104f0, 0x1dbc8, 0x10874, 0x125c, 0xdde0, 0xed8, 0xe164, 0x1d7e0, 0x10c5c, 0x1db64, 0x108d8, 
        0x396c, 0xb6d0, 0x35e8, 0xba54, 0x1b0d0, 0x1336c, 0x1b454, 0x12fe8, 0x3200, 0xbe3c, 0x3584, 0xbab8, 0x1b83c, 0x12c00, 0x1b4b8, 0x12f84, 
        0x2a30, 0xc60c, 0x2db4, 0xc288, 0x1c00c, 0x12430, 0x1bc88, 0x127b4, 0x319c, 0xbea0, 0x2e18, 0xc224, 0x1b8a0, 0x12b9c, 0x1bc24, 0x12818, 
        0x1a90, 0xd5ac, 0x1e14, 0xd228, 0x1cfac, 0x11490, 0x1cc28, 0x11814, 0x21fc, 0xce40, 0x1e78, 0xd1c4, 0x1c840, 0x11bfc, 0x1cbc4, 0x11878, 
        0x29cc, 0xc670, 0x2648, 0xc9f4, 0x1c070, 0x123cc, 0x1c3f4, 0x12048, 0x2260, 0xcddc, 0x25e4, 0xca58, 0x1c7dc, 0x11c60, 0x1c458, 0x11fe4, 
        -1000, 0xf424, -500, 0xf230, 0x1ee24, 0xf618, 0x1ec30, 0xf80c, 500, 0xee48, 0, 0xf03c, 0x1e848, 0xfbf4, 0x1ea3c, 0xfa00, 
        0x9c4, 0xe678, 0x7d0, 0xe86c, 0x1e078, 0x103c4, 0x1e26c, 0x101d0, 0x3e8, 0xec54, 0x5dc, 0xea60, 0x1e654, 0xfde8, 0x1e460, 0xffdc, 
        0x1964, 0xd6d8, 0x1770, 0xd8cc, 0x1d0d8, 0x11364, 0x1d2cc, 0x11170, 0x1388, 0xdcb4, 0x157c, 0xdac0, 0x1d6b4, 0x10d88, 0x1d4c0, 0x10f7c, 
        0xbb8, 0xe484, 0xdac, 0xe290, 0x1de84, 0x105b8, 0x1dc90, 0x107ac, 0x1194, 0xdea8, 0xfa0, 0xe09c, 0x1d8a8, 0x10b94, 0x1da9c, 0x109a0, 
        -1100, 0xf488, -400, 0xf1cc, 0x1ee88, 0xf5b4, 0x1ebcc, 0xf870, 600, 0xede4, -100, 0xf0a0, 0x1e7e4, 0xfc58, 0x1eaa0, 0xf99c, 
        0xa28, 0xe614, 0x76c, 0xe8d0, 0x1e014, 0x10428, 0x1e2d0, 0x1016c, 900, 0xecb8, 0x640, 0xe9fc, 0x1e6b8, 0xfd84, 0x1e3fc, 0x10040, 
        0x19c8, 0xd674, 0x170c, 0xd930, 0x1d074, 0x113c8, 0x1d330, 0x1110c, 0x1324, 0xdd18, 0x15e0, 0xda5c, 0x1d718, 0x10d24, 0x1d45c, 0x10fe0, 
        0xb54, 0xe4e8, 0xe10, 0xe22c, 0x1dee8, 0x10554, 0x1dc2c, 0x10810, 0x11f8, 0xde44, 0xf3c, 0xe100, 0x1d844, 0x10bf8, 0x1db00, 0x1093c, 
        0x38a4, 0xb798, 0x36b0, 0xb98c, 0x1b198, 0x132a4, 0x1b38c, 0x130b0, 0x32c8, 0xbd74, 0x34bc, 0xbb80, 0x1b774, 0x12cc8, 0x1b580, 0x12ebc, 
        0x2af8, 0xc544, 0x2cec, 0xc350, 0x1bf44, 0x124f8, 0x1bd50, 0x126ec, 0x30d4, 0xbf68, 0x2ee0, 0xc15c, 0x1b968, 0x12ad4, 0x1bb5c, 0x128e0, 
        0x1b58, 0xd4e4, 0x1d4c, 0xd2f0, 0x1cee4, 0x11558, 0x1ccf0, 0x1174c, 0x2134, 0xcf08, 0x1f40, 0xd0fc, 0x1c908, 0x11b34, 0x1cafc, 0x11940, 
        0x2904, 0xc738, 0x2710, 0xc92c, 0x1c138, 0x12304, 0x1c32c, 0x12110, 0x2328, 0xcd14, 0x251c, 0xcb20, 0x1c714, 0x11d28, 0x1c520, 0x11f1c, 
        0x3908, 0xb734, 0x364c, 0xb9f0, 0x1b134, 0x13308, 0x1b3f0, 0x1304c, 0x3264, 0xbdd8, 0x3520, 0xbb1c, 0x1b7d8, 0x12c64, 0x1b51c, 0x12f20, 
        0x2a94, 0xc5a8, 0x2d50, 0xc2ec, 0x1bfa8, 0x12494, 0x1bcec, 0x12750, 0x3138, 0xbf04, 0x2e7c, 0xc1c0, 0x1b904, 0x12b38, 0x1bbc0, 0x1287c, 
        0x1af4, 0xd548, 0x1db0, 0xd28c, 0x1cf48, 0x114f4, 0x1cc8c, 0x117b0, 0x2198, 0xcea4, 0x1edc, 0xd160, 0x1c8a4, 0x11b98, 0x1cb60, 0x118dc, 
        0x2968, 0xc6d4, 0x26ac, 0xc990, 0x1c0d4, 0x12368, 0x1c390, 0x120ac, 0x22c4, 0xcd78, 0x2580, 0xcabc, 0x1c778, 0x11cc4, 0x1c4bc, 0x11f80, 
        0x77ec, 0x7850, 0x7468, 0x7bd4, 0x17250, 0x171ec, 0x175d4, 0x16e68, 0x7080, 0x7fbc, 0x7404, 0x7c38, 0x179bc, 0x16a80, 0x17638, 0x16e04, 
        0x68b0, 0x878c, 0x6c34, 0x8408, 0x1818c, 0x162b0, 0x17e08, 0x16634, 0x701c, 0x8020, 0x6c98, 0x83a4, 0x17a20, 0x16a1c, 0x17da4, 0x16698, 
        0x5910, 0x972c, 0x5c94, 0x93a8, 0x1912c, 0x15310, 0x18da8, 0x15694, 0x607c, 0x8fc0, 0x5cf8, 0x9344, 0x189c0, 0x15a7c, 0x18d44, 0x156f8, 
        0x684c, 0x87f0, 0x64c8, 0x8b74, 0x181f0, 0x1624c, 0x18574, 0x15ec8, 0x60e0, 0x8f5c, 0x6464, 0x8bd8, 0x1895c, 0x15ae0, 0x185d8, 0x15e64, 
        0x39d0, 0xb66c, 0x3d54, 0xb2e8, 0x1b06c, 0x133d0, 0x1ace8, 0x13754, 0x413c, 0xaf00, 0x3db8, 0xb284, 0x1a900, 0x13b3c, 0x1ac84, 0x137b8, 
        0x490c, 0xa730, 0x4588, 0xaab4, 0x1a130, 0x1430c, 0x1a4b4, 0x13f88, 0x41a0, 0xae9c, 0x4524, 0xab18, 0x1a89c, 0x13ba0, 0x1a518, 0x13f24, 
        0x58ac, 0x9790, 0x5528, 0x9b14, 0x19190, 0x152ac, 0x19514, 0x14f28, 0x5140, 0x9efc, 0x54c4, 0x9b78, 0x198fc, 0x14b40, 0x19578, 0x14ec4, 
        0x4970, 0xa6cc, 0x4cf4, 0xa348, 0x1a0cc, 0x14370, 0x19d48, 0x146f4, 0x50dc, 0x9f60, 0x4d58, 0xa2e4, 0x19960, 0x14adc, 0x19ce4, 0x14758, 
        0x7724, 0x7918, 0x7530, 0x7b0c, 0x17318, 0x17124, 0x1750c, 0x16f30, 0x7148, 0x7ef4, 0x733c, 0x7d00, 0x178f4, 0x16b48, 0x17700, 0x16d3c, 
        0x6978, 0x86c4, 0x6b6c, 0x84d0, 0x180c4, 0x16378, 0x17ed0, 0x1656c, 0x6f54, 0x80e8, 0x6d60, 0x82dc, 0x17ae8, 0x16954, 0x17cdc, 0x16760, 
        0x59d8, 0x9664, 0x5bcc, 0x9470, 0x19064, 0x153d8, 0x18e70, 0x155cc, 0x5fb4, 0x9088, 0x5dc0, 0x927c, 0x18a88, 0x159b4, 0x18c7c, 0x157c0, 
        0x6784, 0x88b8, 0x6590, 0x8aac, 0x182b8, 0x16184, 0x184ac, 0x15f90, 0x61a8, 0x8e94, 0x639c, 0x8ca0, 0x18894, 0x15ba8, 0x186a0, 0x15d9c, 
        0x7788, 0x78b4, 0x74cc, 0x7b70, 0x172b4, 0x17188, 0x17570, 0x16ecc, 0x70e4, 0x7f58, 0x73a0, 0x7c9c, 0x17958, 0x16ae4, 0x1769c, 0x16da0, 
        0x6914, 0x8728, 0x6bd0, 0x846c, 0x18128, 0x16314, 0x17e6c, 0x165d0, 0x6fb8, 0x8084, 0x6cfc, 0x8340, 0x17a84, 0x169b8, 0x17d40, 0x166fc, 
        0x5974, 0x96c8, 0x5c30, 0x940c, 0x190c8, 0x15374, 0x18e0c, 0x15630, 0x6018, 0x9024, 0x5d5c, 0x92e0, 0x18a24, 0x15a18, 0x18ce0, 0x1575c, 
        0x67e8, 0x8854, 0x652c, 0x8b10, 0x18254, 0x161e8, 0x18510, 0x15f2c, 0x6144, 0x8ef8, 0x6400, 0x8c3c, 0x188f8, 0x15b44, 0x1863c, 0x15e00, 
        0x3a98, 0xb5a4, 0x3c8c, 0xb3b0, 0x1afa4, 0x13498, 0x1adb0, 0x1368c, 0x4074, 0xafc8, 0x3e80, 0xb1bc, 0x1a9c8, 0x13a74, 0x1abbc, 0x13880, 
        0x4844, 0xa7f8, 0x4650, 0xa9ec, 0x1a1f8, 0x14244, 0x1a3ec, 0x14050, 0x4268, 0xadd4, 0x445c, 0xabe0, 0x1a7d4, 0x13c68, 0x1a5e0, 0x13e5c, 
        0x57e4, 0x9858, 0x55f0, 0x9a4c, 0x19258, 0x151e4, 0x1944c, 0x14ff0, 0x5208, 0x9e34, 0x53fc, 0x9c40, 0x19834, 0x14c08, 0x19640, 0x14dfc, 
        0x4a38, 0xa604, 0x4c2c, 0xa410, 0x1a004, 0x14438, 0x19e10, 0x1462c, 0x5014, 0xa028, 0x4e20, 0xa21c, 0x19a28, 0x14a14, 0x19c1c, 0x14820, 
        0x3a34, 0xb608, 0x3cf0, 0xb34c, 0x1b008, 0x13434, 0x1ad4c, 0x136f0, 0x40d8, 0xaf64, 0x3e1c, 0xb220, 0x1a964, 0x13ad8, 0x1ac20, 0x1381c, 
        0x48a8, 0xa794, 0x45ec, 0xaa50, 0x1a194, 0x142a8, 0x1a450, 0x13fec, 0x4204, 0xae38, 0x44c0, 0xab7c, 0x1a838, 0x13c04, 0x1a57c, 0x13ec0, 
        0x5848, 0x97f4, 0x558c, 0x9ab0, 0x191f4, 0x15248, 0x194b0, 0x14f8c, 0x51a4, 0x9e98, 0x5460, 0x9bdc, 0x19898, 0x14ba4, 0x195dc, 0x14e60, 
        0x49d4, 0xa668, 0x4c90, 0xa3ac, 0x1a068, 0x143d4, 0x19dac, 0x14690, 0x5078, 0x9fc4, 0x4dbc, 0xa280, 0x199c4, 0x14a78, 0x19c80, 0x147bc, 
        -800, 0xf35c, -700, 0xf2f8, 0x1ed5c, 0xf6e0, 0x1ecf8, 0xf744, 300, 0xef10, 200, 0xef74, 0x1e910, 0xfb2c, 0x1e974, 0xfac8, 
        0x8fc, 0xe740, 0x898, 0xe7a4, 0x1e140, 0x102fc, 0x1e1a4, 0x10298, 0x4b0, 0xeb8c, 0x514, 0xeb28, 0x1e58c, 0xfeb0, 0x1e528, 0xff14, 
        0x189c, 0xd7a0, 0x1838, 0xd804, 0x1d1a0, 0x1129c, 0x1d204, 0x11238, 0x1450, 0xdbec, 0x14b4, 0xdb88, 0x1d5ec, 0x10e50, 0x1d588, 0x10eb4, 
        0xc80, 0xe3bc, 0xce4, 0xe358, 0x1ddbc, 0x10680, 0x1dd58, 0x106e4, 0x10cc, 0xdf70, 0x1068, 0xdfd4, 0x1d970, 0x10acc, 0x1d9d4, 0x10a68, 
        0x37dc, 0xb860, 0x3778, 0xb8c4, 0x1b260, 0x131dc, 0x1b2c4, 0x13178, 0x3390, 0xbcac, 0x33f4, 0xbc48, 0x1b6ac, 0x12d90, 0x1b648, 0x12df4, 
        0x2bc0, 0xc47c, 0x2c24, 0xc418, 0x1be7c, 0x125c0, 0x1be18, 0x12624, 0x300c, 0xc030, 0x2fa8, 0xc094, 0x1ba30, 0x12a0c, 0x1ba94, 0x129a8, 
        0x1c20, 0xd41c, 0x1c84, 0xd3b8, 0x1ce1c, 0x11620, 0x1cdb8, 0x11684, 0x206c, 0xcfd0, 0x2008, 0xd034, 0x1c9d0, 0x11a6c, 0x1ca34, 0x11a08, 
        0x283c, 0xc800, 0x27d8, 0xc864, 0x1c200, 0x1223c, 0x1c264, 0x121d8, 0x23f0, 0xcc4c, 0x2454, 0xcbe8, 0x1c64c, 0x11df0, 0x1c5e8, 0x11e54, 
        -900, 0xf3c0, -600, 0xf294, 0x1edc0, 0xf67c, 0x1ec94, 0xf7a8, 400, 0xeeac, 100, 0xefd8, 0x1e8ac, 0xfb90, 0x1e9d8, 0xfa64, 
        0x960, 0xe6dc, 0x834, 0xe808, 0x1e0dc, 0x10360, 0x1e208, 0x10234, 0x44c, 0xebf0, 0x578, 0xeac4, 0x1e5f0, 0xfe4c, 0x1e4c4, 0xff78, 
        0x1900, 0xd73c, 0x17d4, 0xd868, 0x1d13c, 0x11300, 0x1d268, 0x111d4, 0x13ec, 0xdc50, 0x1518, 0xdb24, 0x1d650, 0x10dec, 0x1d524, 0x10f18, 
        0xc1c, 0xe420, 0xd48, 0xe2f4, 0x1de20, 0x1061c, 0x1dcf4, 0x10748, 0x1130, 0xdf0c, 0x1004, 0xe038, 0x1d90c, 0x10b30, 0x1da38, 0x10a04, 
        0x3840, 0xb7fc, 0x3714, 0xb928, 0x1b1fc, 0x13240, 0x1b328, 0x13114, 0x332c, 0xbd10, 0x3458, 0xbbe4, 0x1b710, 0x12d2c, 0x1b5e4, 0x12e58, 
        0x2b5c, 0xc4e0, 0x2c88, 0xc3b4, 0x1bee0, 0x1255c, 0x1bdb4, 0x12688, 0x3070, 0xbfcc, 0x2f44, 0xc0f8, 0x1b9cc, 0x12a70, 0x1baf8, 0x12944, 
        0x1bbc, 0xd480, 0x1ce8, 0xd354, 0x1ce80, 0x115bc, 0x1cd54, 0x116e8, 0x20d0, 0xcf6c, 0x1fa4, 0xd098, 0x1c96c, 0x11ad0, 0x1ca98, 0x119a4, 
        0x28a0, 0xc79c, 0x2774, 0xc8c8, 0x1c19c, 0x122a0, 0x1c2c8, 0x12174, 0x238c, 0xccb0, 0x24b8, 0xcb84, 0x1c6b0, 0x11d8c, 0x1c584, 0x11eb8, 
        0x765c, 0x79e0, 0x75f8, 0x7a44, 0x173e0, 0x1705c, 0x17444, 0x16ff8, 0x7210, 0x7e2c, 0x7274, 0x7dc8, 0x1782c, 0x16c10, 0x177c8, 0x16c74, 
        0x6a40, 0x85fc, 0x6aa4, 0x8598, 0x17ffc, 0x16440, 0x17f98, 0x164a4, 0x6e8c, 0x81b0, 0x6e28, 0x8214, 0x17bb0, 0x1688c, 0x17c14, 0x16828, 
        0x5aa0, 0x959c, 0x5b04, 0x9538, 0x18f9c, 0x154a0, 0x18f38, 0x15504, 0x5eec, 0x9150, 0x5e88, 0x91b4, 0x18b50, 0x158ec, 0x18bb4, 0x15888, 
        0x66bc, 0x8980, 0x6658, 0x89e4, 0x18380, 0x160bc, 0x183e4, 0x16058, 0x6270, 0x8dcc, 0x62d4, 0x8d68, 0x187cc, 0x15c70, 0x18768, 0x15cd4, 
        0x3b60, 0xb4dc, 0x3bc4, 0xb478, 0x1aedc, 0x13560, 0x1ae78, 0x135c4, 0x3fac, 0xb090, 0x3f48, 0xb0f4, 0x1aa90, 0x139ac, 0x1aaf4, 0x13948, 
        0x477c, 0xa8c0, 0x4718, 0xa924, 0x1a2c0, 0x1417c, 0x1a324, 0x14118, 0x4330, 0xad0c, 0x4394, 0xaca8, 0x1a70c, 0x13d30, 0x1a6a8, 0x13d94, 
        0x571c, 0x9920, 0x56b8, 0x9984, 0x19320, 0x1511c, 0x19384, 0x150b8, 0x52d0, 0x9d6c, 0x5334, 0x9d08, 0x1976c, 0x14cd0, 0x19708, 0x14d34, 
        0x4b00, 0xa53c, 0x4b64, 0xa4d8, 0x19f3c, 0x14500, 0x19ed8, 0x14564, 0x4f4c, 0xa0f0, 0x4ee8, 0xa154, 0x19af0, 0x1494c, 0x19b54, 0x148e8, 
        0x76c0, 0x797c, 0x7594, 0x7aa8, 0x1737c, 0x170c0, 0x174a8, 0x16f94, 0x71ac, 0x7e90, 0x72d8, 0x7d64, 0x17890, 0x16bac, 0x17764, 0x16cd8, 
        0x69dc, 0x8660, 0x6b08, 0x8534, 0x18060, 0x163dc, 0x17f34, 0x16508, 0x6ef0, 0x814c, 0x6dc4, 0x8278, 0x17b4c, 0x168f0, 0x17c78, 0x167c4, 
        0x5a3c, 0x9600, 0x5b68, 0x94d4, 0x19000, 0x1543c, 0x18ed4, 0x15568, 0x5f50, 0x90ec, 0x5e24, 0x9218, 0x18aec, 0x15950, 0x18c18, 0x15824, 
        0x6720, 0x891c, 0x65f4, 0x8a48, 0x1831c, 0x16120, 0x18448, 0x15ff4, 0x620c, 0x8e30, 0x6338, 0x8d04, 0x18830, 0x15c0c, 0x18704, 0x15d38, 
        0x3afc, 0xb540, 0x3c28, 0xb414, 0x1af40, 0x134fc, 0x1ae14, 0x13628, 0x4010, 0xb02c, 0x3ee4, 0xb158, 0x1aa2c, 0x13a10, 0x1ab58, 0x138e4, 
        0x47e0, 0xa85c, 0x46b4, 0xa988, 0x1a25c, 0x141e0, 0x1a388, 0x140b4, 0x42cc, 0xad70, 0x43f8, 0xac44, 0x1a770, 0x13ccc, 0x1a644, 0x13df8, 
        0x5780, 0x98bc, 0x5654, 0x99e8, 0x192bc, 0x15180, 0x193e8, 0x15054, 0x526c, 0x9dd0, 0x5398, 0x9ca4, 0x197d0, 0x14c6c, 0x196a4, 0x14d98, 
        0x4a9c, 0xa5a0, 0x4bc8, 0xa474, 0x19fa0, 0x1449c, 0x19e74, 0x145c8, 0x4fb0, 0xa08c, 0x4e84, 0xa1b8, 0x19a8c, 0x149b0, 0x19bb8, 0x14884
     };


            public int? LookupGillhamAltitude(int acCode)
            {
                if ((acCode < 0x40) || (acCode > 0x7bf))
                {
                    return null;
                }
                return new int?(_LookupTable[acCode - _ACCodeToIndexOffsets[acCode / 0x40]]);
            }

 

        }

        public class Crc32ModeS
        {
            // Fields
            private uint[] _LookupTable = new uint[0x100];
            private const uint _Polynomial = 0xfffa0480;

            // Methods
            public Crc32ModeS()
            {
                for (uint i = 0; i < this._LookupTable.Length; i++)
                {
                    uint num2 = i << 0x18;
                    for (int j = 0; j < 8; j++)
                    {
                        if ((num2 & 0x80000000) != 0x80000000)
                        {
                            num2 = num2 << 1;
                        }
                        else
                        {
                            num2 = (uint)((num2 ^ -392064) << 1);
                        }
                    }
                    this._LookupTable[i] = num2;
                }
            }

            public uint ComputeChecksum(byte[] bytes, int offset, int length)
            {
                uint num = 0;
                for (int i = offset; i < (offset + length); i++)
                {
                    uint index = (uint)(((num & -16777216) >> 0x18) ^ bytes[i]);
                    num = (num << 8) ^ this._LookupTable[index];
                }
                return num;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes, int offset, int length, bool littleEndian = false)
            {
                return this.ConvertToByteArray(this.ComputeChecksum(bytes, offset, length), littleEndian);
            }

            public byte[] ComputeChecksumBytesTraditional32(byte[] bytes)
            {
                uint crc = (uint)((((bytes[0] << 0x18) + (bytes[1] << 0x10)) + (bytes[2] << 8)) + bytes[3]);
                for (int i = 0; i < 0x20; i++)
                {
                    if ((crc & 0x80000000) != 0)
                    {
                        crc ^= 0xfffa0480;
                    }
                    crc = crc << 1;
                }
                return this.ConvertToByteArray(crc, false);
            }

            public byte[] ComputeChecksumBytesTraditional88(byte[] bytes)
            {
                uint crc = (uint)((((bytes[0] << 0x18) + (bytes[1] << 0x10)) + (bytes[2] << 8)) + bytes[3]);
                uint num2 = (uint)((((bytes[4] << 0x18) + (bytes[5] << 0x10)) + (bytes[6] << 8)) + bytes[7]);
                uint num3 = (uint)(((bytes[8] << 0x18) + (bytes[9] << 0x10)) + (bytes[10] << 8));
                for (int i = 0; i < 0x58; i++)
                {
                    if ((crc & 0x80000000) != 0)
                    {
                        crc ^= 0xfffa0480;
                    }
                    crc = crc << 1;
                    if ((num2 & 0x80000000) != 0)
                    {
                        crc |= 1;
                    }
                    num2 = num2 << 1;
                    if ((num3 & 0x80000000) != 0)
                    {
                        num2 |= 1;
                    }
                    num3 = num3 << 1;
                }
                return this.ConvertToByteArray(crc, false);
            }

            private byte[] ConvertToByteArray(uint crc, bool littleEndian)
            {
                byte[] buffer = new byte[4];
                if (littleEndian)
                {
                    buffer[0] = (byte)(crc & 0xff);
                    buffer[1] = (byte)((crc >> 8) & 0xff);
                    buffer[2] = (byte)((crc >> 0x10) & 0xff);
                    buffer[3] = (byte)(crc >> 0x18);
                    return buffer;
                }
                buffer[0] = (byte)(crc >> 0x18);
                buffer[1] = (byte)((crc >> 0x10) & 0xff);
                buffer[2] = (byte)((crc >> 8) & 0xff);
                buffer[3] = (byte)(crc & 0xff);
                return buffer;
            }
        }

        public static void ReadMessage(Stream st1)
        {
            bool avr = false;
            bool binary = false;
            int avrcount = 0;

            st1.ReadTimeout = 10000;

            //st.ReadTimeout = 5000;

            while (run)
            {
                int by = st1.ReadByte();
                if (by == -1)
                    break;

                if (by == '*')
                {
                    avrcount++;
                    if (avrcount >= 4)
                        avr = true;

                    if (avr)
                    {
                        Plane plane = ReadMessage('*' + ReadLine(st1));                        
                        if (plane != null)
                        {
                            PointLatLngAltHdg plla = new PointLatLngAltHdg(plane.plla());
                            plla.Heading = (float)plane.heading;
                            if (plla.Lat == 0 && plla.Lng == 0)
                                continue;
                            if (UpdatePlanePosition != null && plla != null)
                                UpdatePlanePosition(null, plla);
                            //Console.WriteLine(plane.pllalocal(plane.llaeven));
                            Console.WriteLine(plane.ID + " " + plla);
                        }
                    }
                }
                else if ((by == 'M' || by == 'S' || by == 'A' || by == 'I' || by == 'C') && !binary) // msg clk sta air id sel
                {
                    string line = ((char)by) +ReadLine(st1);

                    if (line.StartsWith("MSG"))
                    {
                        string[] strArray = line.Split(new char[] { ',' });

                        if (strArray[1] == "3") // airborne pos
                        {
                            String session_id = strArray[2];// String. Database session record number. 
                            String aircraft_id = strArray[3];// String. Database aircraft record number. 
                            String hex_ident = strArray[4];//String. 24-bit ICACO ID, in hex. 
                            String flight_id = strArray[5];//String. Database flight record number. 
                            String generated_date = strArray[6];// String. Date the message was generated. 
                            String generated_time = strArray[7];//String. Time the message was generated. 
                            String logged_date = strArray[8];//String. Date the message was logged. 
                            String logged_time = strArray[9];//String. Time the message was logged. 
                            String callsign = strArray[10];//String. Eight character flight ID or callsign. 
                            int altitude = 0;
                            try
                            {
                                altitude = (int)double.Parse(strArray[11], CultureInfo.InvariantCulture);// Integer. Mode C Altitude relative to 1013 mb (29.92" Hg). 
                            }
                            catch { }
                           
                            double lat = 0;
                            try
                            {
                                lat = double.Parse(strArray[14], CultureInfo.InvariantCulture);//Float. Latitude. 
                            }
                            catch { }
                            double lon = 0;
                            try
                            {
                                lon = double.Parse(strArray[15], CultureInfo.InvariantCulture);//Float. Longitude 
                            }
                            catch { }

                            bool is_on_ground = strArray[21] != "0";//Boolean. Flag to indicate ground squat switch is active. 

                            if (Planes[hex_ident] == null)
                                Planes[hex_ident] = new Plane();

                            Plane plane = ((Plane)Planes[hex_ident]);

                            if (lat == 0 && lon == 0)
                                continue;

                            if (UpdatePlanePosition != null && plane != null)
                                UpdatePlanePosition(null, new PointLatLngAltHdg(lat, lon, altitude / 3.048, (float)plane.heading, hex_ident, DateTime.Now));
                        }
                        else if (strArray[1] == "4")
                        {
                            String session_id = strArray[2];// String. Database session record number. 
                            String aircraft_id = strArray[3];// String. Database aircraft record number. 
                            String hex_ident = strArray[4];//String. 24-bit ICACO ID, in hex. 
                            String flight_id = strArray[5];//String. Database flight record number. 
                            String generated_date = strArray[6];// String. Date the message was generated. 
                            String generated_time = strArray[7];//String. Time the message was generated. 
                            String logged_date = strArray[8];//String. Date the message was logged. 
                            String logged_time = strArray[9];//String. Time the message was logged. 

                            if (Planes[hex_ident] == null)
                                Planes[hex_ident] = new Plane();

                            try
                            {
                                int ground_speed = (int)double.Parse(strArray[12], CultureInfo.InvariantCulture);// Integer. Speed over ground. 
                            }
                            catch { }
                            try
                            {
                                ((Plane)Planes[hex_ident]).heading = (int)double.Parse(strArray[13], CultureInfo.InvariantCulture);//Integer. Ground track angle. 
                            }
                            catch { }

                        }
                        else if (strArray[1] == "1")
                        {
                            String session_id = strArray[2];// String. Database session record number. 
                            String aircraft_id = strArray[3];// String. Database aircraft record number. 
                            String hex_ident = strArray[4];//String. 24-bit ICACO ID, in hex. 
                            String flight_id = strArray[5];//String. Database flight record number. 
                            String generated_date = strArray[6];// String. Date the message was generated. 
                            String generated_time = strArray[7];//String. Time the message was generated. 
                            String logged_date = strArray[8];//String. Date the message was logged. 
                            String logged_time = strArray[9];//String. Time the message was logged. 
                            String callsign = strArray[10];//String. Eight character flight ID or callsign. 

                            if (Planes[hex_ident] == null)
                                Planes[hex_ident] = new Plane();
                            
                            ((Plane)Planes[hex_ident]).CallSign = callsign;
                        }
                    }
                    else
                    {
                        log.Info(line);

                    }
                }
                else if (by == 0x1a)
                {
                    avr = false;

                    byte[] buffer = new byte[24];
                    buffer[0] = (byte)by;

                    int type = st1.ReadByte();
                    buffer[1] = (byte)type;
                    st1.Read(buffer, 2, 7);

                    switch (type)
                    {
                        case '1': // mode-ac
                            // 2 bytes
                            st1.Read(buffer, 9, 2);
                            //log.Info("1");
                            break;
                        case '2': // mode-s short
                            st1.Read(buffer, 9, 7);
                            //log.Info("2");
                            break;
                        case '3': // mode-s long
                            st1.Read(buffer, 9, 14);
                            //log.Info("3");
                            Plane plane = ReadMessage(buffer);
                            if (plane != null)
                            {
                                binary = true;
                                PointLatLngAltHdg plla = new PointLatLngAltHdg(plane.plla());
                                if (plla == null)
                                    break;
                                if (plla.Lat == 0 && plla.Lng == 0)
                                    continue;
                                plla.Heading = (float)plane.heading;
                                if (UpdatePlanePosition != null && plla != null)
                                    UpdatePlanePosition(null, plla);
                                //Console.WriteLine(plane.pllalocal(plane.llaeven));
                                Console.WriteLine(plla);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    log.Info("bad sync 0x" + by.ToString("X2") + " " + (char)by);
                }
            }
        }

        private static string ReadLine(Stream st1)
        {
            string answer = "";
            char let;
            do
            {
                let = (char)st1.ReadByte();
                answer += let;
            } while (let != '\n');

            return answer;
        }

        /// <summary>
        /// Beast Binary data Format
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Plane ReadMessage(byte[] data)
        {
            /*Beast binary data format
<0x1a> <"1">  6 byte MLAT, 1 byte signal level, 2 byte Mode-AC
<0x1a> <"2">  6 byte MLAT, 1 byte signal level, 7 byte Mode-S short frame
<0x1a> <"3">  6 byte MLAT, 1 byte signal level, 14 byte Mode-S long frame
             */

            if (data[0] != 0x1a || data.Length < 0x17)
                return null;

            if (data[1] == '3')
            {
                StringBuilder sb = new StringBuilder();

                for (int a = 0; a < 14; a++)
                {
                    sb.Append(data[9 + a].ToString("X2"));
                }

                return ReadMessage("*"+sb);
            }

            return null;
        }

        /// <summary>
        /// AVR ASCii Format Input
        /// </summary>
        /// <param name="avrline"></param>
        /// <returns></returns>
        public static Plane ReadMessage(string avrline)
        {
            if (!avrline.StartsWith("*"))
                return null;

            log.Debug(avrline);

            avrline = avrline.Trim().TrimEnd(';');

            ModeSMessage adsbmess = new ModeSMessage();

            byte[] data = ConvertHexStringToByteArray(avrline.TrimStart('*'));

            if (data.Length < 13)
                return null;

            adsbmess.DF = (byte)(data[0] >> 3); // 5
            adsbmess.CA = (byte)(data[0] & 7); // 3
            // aircraft icao
            adsbmess.AA = (uint)((data[1] << 16) + (data[2] << 8) +(data[3])); // 24

            Array.Copy(data, 4, adsbmess.adsbdata, 0, 7); // 56 bytes

            // paraity
            adsbmess.PI = (uint)((data[11] << 16) + (data[12] << 8) + (data[13])); // 24

            adsbmess.recvtime = DateTime.Now;

            // check parity
            Crc32ModeS crc = new Crc32ModeS();
            byte[] pidata = crc.ComputeChecksumBytes(data, 0, data.Length - 3, false);

            if (adsbmess.PI != ((pidata[0] << 16) + (pidata[1] << 8) + pidata[2]))
            {
                Console.WriteLine("Parity Fail");
                return null;
            }

            // create the plane id 
            string planeid = adsbmess.AA.ToString("X5");
            if (!Planes.ContainsKey(planeid))
                Planes[planeid] = new Plane() { ID = planeid };

            if (adsbmess.DF == 17 && 
                (adsbmess.TypeCode >= 9 && adsbmess.TypeCode <= 18)
               || (adsbmess.TypeCode >= 0x14 && adsbmess.TypeCode <= 0x16)
                ) // airbornepos
            {
                // odd
                if (adsbmess.Fcprformat)
                {
                    ((Plane)Planes[adsbmess.AA.ToString("X5")]).llaodd  = adsbmess;

                    //Console.WriteLine("adsb " + planeid + " type " + adsbmess.DF + " odd");
                }
                else // even
                {
                    ((Plane)Planes[adsbmess.AA.ToString("X5")]).llaeven = adsbmess;

                    //Console.WriteLine("adsb " + planeid + " type " + adsbmess.DF + " even");
                }
            }
            else if (adsbmess.DF == 17 && adsbmess.TypeCode >= 1 && adsbmess.TypeCode <= 4) // ident
            {
                StringBuilder builder = new StringBuilder();

                int count = 8;
                for (int i = 0; i < count; i++)
                {
                    char ch = '\0';
                    byte[] char1 = adsbmess.getbits(adsbmess.adsbdata, 7 + i * 6, 6);

                    byte num2 = char1[0];

                    if ((num2 > 0) && (num2 < 0x1b))
                    {
                        ch = (char)(0x41 + (num2 - 1));
                    }
                    else if (num2 == 0x20)
                    {
                        ch = ' ';
                    }
                    else if ((num2 > 0x2f) && (num2 < 0x3a))
                    {
                        ch = (char)(0x30 + (num2 - 0x30));
                    }
                    if (ch != '\0')
                    {
                        builder.Append(ch);
                    }
                }

                adsbmess.Ident = builder.ToString();
                //Console.WriteLine("Ident " + builder.ToString());
            } 
            else if (adsbmess.DF == 17 && adsbmess.TypeCode == 0x13) // velocity
            {
                int subtype = adsbmess.adsbdata[0] & 7;
                int accuracy = (adsbmess.adsbdata[1] >> 3) & 15;

                switch (subtype)
                {
                    case 3:
                    case 4:
                        bool headingstatus = ((adsbmess.adsbdata[1] >> 2) & 1) > 0;
                        if (headingstatus)
                        {
                            ushort head = (ushort)(((adsbmess.adsbdata[1] & 3) << 8) + adsbmess.adsbdata[2]);
                            double heading = head * 0.3515625;
                            ((Plane)Planes[adsbmess.AA.ToString("X5")]).heading = heading;
                        }
                        break;
                    case 1:
                    case 2:
                    default:
                        bool westvel = ((adsbmess.adsbdata[1] >> 2) & 1) > 0;

                        int ewvel = (int)(((adsbmess.adsbdata[1] & 3) << 8) + adsbmess.adsbdata[2]);

                        bool southvel = ((adsbmess.adsbdata[3] >> 7) & 1) > 0;

                        int nsvel = (int)(((adsbmess.adsbdata[3] & 127) << 3) + (adsbmess.adsbdata[4] >> 5));

                        if (westvel)
                            ewvel *= -1;

                        if (southvel)
                            nsvel *= -1;

                        double cog = (Math.Atan2(ewvel, nsvel) * (180 / Math.PI));

                        Console.WriteLine("vel " + ewvel + " " + nsvel + " " + cog);

                        ((Plane)Planes[adsbmess.AA.ToString("X5")]).heading = (cog + 360) % 360;

                        break;
                }
            }
            else
            {
                Console.WriteLine("No processing type 0x" + adsbmess.TypeCode.ToString("X2") + " DF " + adsbmess.DF);
            }

            return ((Plane)Planes[adsbmess.AA.ToString("X5")]);
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        public class PointLatLngAltHdg : PointLatLngAlt
        {
            public PointLatLngAltHdg(PointLatLngAlt plla)
            {
                this.Lat = plla.Lat;
                this.Lng = plla.Lng;
                this.Alt = plla.Alt;
                this.Heading = -1;
                this.Tag = plla.Tag;
            }

            public PointLatLngAltHdg(double lat, double lng, double alt, float heading, string tag, DateTime time)
            {
                this.Lat = lat;
                this.Lng = lng;
                this.Alt = alt;
                this.Heading = heading;
                this.Tag = tag;
                this.Time = time;
            }

            public float Heading { get; set; }

            public MAVLink.MAV_COLLISION_THREAT_LEVEL ThreatLevel { get; set; }

            public DateTime Time { get; set; }

            public bool DisplayICAO { get; set; }

            public string CallSign { get; set; }
        }
    }
}
