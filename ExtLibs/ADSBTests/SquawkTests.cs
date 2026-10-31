// Squawk handling tests for MissionPlanner.Utilities.adsb, driven by recorded live data.
//
// Ground truth for the wire formats (verified against upstream source, 2026-09-29):
//   readsb / adsb.lol JSON   "squawk": "%04x" of the hex-coded Mode A code -> "1200"      (4 octal digits, string)
//   dump1090-fa SBS field 18  "%04x" of the same value, only on MSG,6 (DF5/DF21)   -> "1200"
//   readsb SBS field 18       "%04d" of squawkDec                                   -> "1200"
//   antirez dump1090 SBS      "%d" of a*1000+b*100+c*10+d, on every message          -> "1200", "504"
//   MAVLink ADSB_VEHICLE      "the code is in decimal: e.g. 7700 is encoded as 0b0001_1110_0001_0100"
//   ArduPilot                 SITL sends 1200 for VFR; adsb_send.lua documents "decimal (e.g. 1200 for VFR)"
// So every text source must become ushort 1200 for "1200", and must display back as "1200".
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using Newtonsoft.Json.Linq;

[assembly: DoNotParallelize]

namespace ADSBTests
{
    static class Fx
    {
        public static string PathOf(params string[] p) => Path.Combine(new[] { AppContext.BaseDirectory, "Fixtures" }.Concat(p).ToArray());
        public static JObject Json(params string[] p) => JObject.Parse(File.ReadAllText(PathOf(p)));
        public static byte[] Bytes(params string[] p) => File.ReadAllBytes(PathOf(p));
    }

    static class Sq
    {
        /// <summary>The value MAVLink and ArduPilot expect for a squawk written as these digits.</summary>
        public static ushort Expected(string digits) => ushort.Parse(digits);

        /// <summary>null when the model cannot express "no squawk" at all.</summary>
        public static bool? Valid(adsb.PointLatLngAltHdg p)
        {
            var pi = typeof(adsb.PointLatLngAltHdg).GetProperty("SquawkValid");
            return pi == null ? (bool?)null : (bool)pi.GetValue(p);
        }

        /// <summary>What the map tooltip shows. Falls back to FlightData.cs's current expression.</summary>
        public static string Display(adsb.PointLatLngAltHdg p)
        {
            var pi = typeof(adsb.PointLatLngAltHdg).GetProperty("SquawkString");
            return pi != null ? (string)pi.GetValue(p) : p.Squawk.ToString("X4");
        }

        public static void Report(List<string> failures, int checkedCount, string what)
        {
            if (failures.Count > 0)
                Assert.Fail($"{failures.Count} of {checkedCount} {what} wrong. First few:\n  " + string.Join("\n  ", failures.Take(8)));
            Assert.IsTrue(checkedCount > 0, "nothing was checked - " + what);
            Console.WriteLine($"{checkedCount} {what} checked, all correct");
        }
    }

    /// <summary>adsb.ReadMessage sets ReadTimeout, which a plain MemoryStream rejects.</summary>
    class TimeoutStream : MemoryStream
    {
        public TimeoutStream(byte[] b) : base(b) { }
        public override bool CanTimeout => true;
        public override int ReadTimeout { get; set; }
    }

    /// <summary>Feeds bytes through the real adsb.ReadMessage(Stream) (SBS / AVR / Beast autodetect).</summary>
    static class Feed
    {
        static readonly FieldInfo RunField = typeof(adsb).GetField("run", BindingFlags.NonPublic | BindingFlags.Static);
        static readonly FieldInfo PlanesField = typeof(adsb).GetField("Planes", BindingFlags.NonPublic | BindingFlags.Static);
        public static Hashtable Planes => (Hashtable)PlanesField.GetValue(null);

        public static List<adsb.PointLatLngAltHdg> Stream(byte[] data)
        {
            Planes.Clear();
            // no constructor: it would start the network thread
            var inst = (adsb)RuntimeHelpers.GetUninitializedObject(typeof(adsb));
            var got = new List<adsb.PointLatLngAltHdg>();
            EventHandler<adsb.PointLatLngAltHdg> h = (s, e) => { if (s == null || ReferenceEquals(s, inst)) lock (got) got.Add(e); };
            adsb.UpdatePlanePosition += h;
            RunField.SetValue(null, true);
            try { inst.ReadMessage(new TimeoutStream(data)); }
            finally { adsb.UpdatePlanePosition -= h; RunField.SetValue(null, false); }
            return got;
        }

        public static Dictionary<string, adsb.PointLatLngAltHdg> LastByIcao(IEnumerable<adsb.PointLatLngAltHdg> pts)
        {
            var d = new Dictionary<string, adsb.PointLatLngAltHdg>();
            foreach (var p in pts) d[Norm(p.Tag)] = p;
            return d;
        }

        /// <summary>The Mode S path keys planes with ToString("X5"), which drops a leading 0.</summary>
        public static string Norm(string icao) => (icao ?? "").Trim().ToUpperInvariant().PadLeft(6, '0');

        /// <summary>Reads a squawk the Mode S decoder stored on its internal Plane, if it stores one.</summary>
        public static (bool supported, bool valid, ushort value) PlaneSquawk(string icao)
        {
            object plane = null;
            foreach (DictionaryEntry e in Planes)
                if (Norm((string)e.Key) == Norm(icao)) plane = e.Value;
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var f = typeof(adsb.Plane).GetField("squawk", flags);
            var v = typeof(adsb.Plane).GetField("squawk_valid", flags);
            if (f == null || v == null) return (false, false, 0);
            if (plane == null) return (true, false, 0);
            return (true, (bool)v.GetValue(plane), (ushort)f.GetValue(plane));
        }
    }

    // ------------------------------------------------------------------------------------------
    // HTTP: the real adsb reader thread, pointed at a local server replaying live adsb.lol responses
    // ------------------------------------------------------------------------------------------
    [TestClass]
    public partial class HttpApi
    {
        static readonly string[] Files =
        {
            "lol_local.json", "lol_jfk.json", "lol_lhr.json", "lol_fra.json",
            "lol_sqk7700.json", "lol_sqk1200.json", "lol_icao_a34f90.json", "lol_icao_a37d8a.json"
        };
        static readonly Dictionary<string, Dictionary<string, adsb.PointLatLngAltHdg>> Seen = new();
        static readonly List<string> Urls = new();

        [ClassInitialize]
        public static void ReplayLiveResponses(TestContext _)
        {
            var probe = new TcpListener(IPAddress.Loopback, 0); probe.Start();
            int port = ((IPEndPoint)probe.LocalEndpoint).Port; probe.Stop();

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{port}/");
            listener.Start();

            var current = new ConcurrentDictionary<string, adsb.PointLatLngAltHdg>();
            EventHandler<adsb.PointLatLngAltHdg> h = (s, e) => { if (s is adsb) current[e.Tag] = e; };
            adsb.UpdatePlanePosition += h;
            adsb.server = $"http://127.0.0.1:{port}";
            adsb.CurrentPosition = new PointLatLngAlt(46.2, -119.1, 0);
            new adsb();   // starts the real TryConnect() thread

            try
            {
                for (int i = 0; i <= Files.Length; i++)
                {
                    var t = listener.GetContextAsync();
                    if (!t.Wait(30000)) Assert.Fail($"reader never requested response #{i}");
                    var ctx = t.Result;
                    Urls.Add(ctx.Request.RawUrl);
                    // the reader handles a whole response before asking again, so this is complete
                    if (i > 0)
                    {
                        Seen[Files[i - 1]] = new Dictionary<string, adsb.PointLatLngAltHdg>(current);
                        current.Clear();
                    }
                    byte[] body = i < Files.Length ? File.ReadAllBytes(Fx.PathOf("http", Files[i])) : Encoding.ASCII.GetBytes("{\"ac\":[]}");
                    ctx.Response.ContentType = "application/json";
                    ctx.Response.OutputStream.Write(body, 0, body.Length);
                    ctx.Response.Close();
                }
            }
            finally
            {
                adsb.UpdatePlanePosition -= h;
                adsb.server = "";
                listener.Stop();
            }
        }

        internal static IEnumerable<(string file, JObject ac, adsb.PointLatLngAltHdg p)> All()
        {
            foreach (var f in Files)
                foreach (JObject ac in Fx.Json("http", f)["ac"])
                {
                    Seen[f].TryGetValue(ac.Value<string>("hex").Trim().ToUpper(), out var p);
                    yield return (f, ac, p);
                }
        }

        [TestMethod]
        public void Requests_use_the_adsb_lol_point_url()
        {
            Assert.IsTrue(Urls.All(u => u.StartsWith("/v2/point/46.2/-119.1/")), string.Join(", ", Urls));
        }

        [TestMethod]
        public void Every_live_aircraft_is_delivered()
        {
            var lost = All().Where(x => x.p == null).Select(x => $"{x.file} {x.ac["hex"]}").ToList();
            Sq.Report(lost, All().Count(), "live aircraft delivered");
        }

        [TestMethod]
        public void Live_squawk_strings_parse_to_the_MAVLink_decimal_value()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                var s = ac.Value<string>("squawk");
                if (s == null || p == null) continue;
                n++;
                if (p.Squawk != Sq.Expected(s)) bad.Add($"{f} {ac["hex"]}: \"{s}\" -> {p.Squawk}, expected {Sq.Expected(s)}");
            }
            Sq.Report(bad, n, "live squawks");
        }

        [TestMethod]
        public void Live_squawks_display_as_the_digits_that_were_received()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                var s = ac.Value<string>("squawk");
                if (s == null || p == null) continue;
                n++;
                if (Sq.Display(p) != s) bad.Add($"{f} {ac["hex"]}: received \"{s}\", tooltip shows \"{Sq.Display(p)}\"");
            }
            Sq.Report(bad, n, "tooltip squawks");
        }

        [TestMethod]
        public void Aircraft_without_a_squawk_are_not_reported_as_0000()
        {
            var missing = All().Where(x => x.p != null && x.ac["squawk"] == null).ToList();
            Assert.IsTrue(missing.Count > 0, "live data had no aircraft without a squawk");
            var valid = Sq.Valid(missing[0].p);
            if (valid == null)
                Assert.Fail($"{missing.Count} live aircraft have no squawk, but PointLatLngAltHdg cannot say so: they get Squawk=0, " +
                            $"shown as \"{Sq.Display(missing[0].p)}\" and forwarded to the autopilot as squawk 0000");
            var bad = missing.Where(x => Sq.Valid(x.p) != false).Select(x => $"{x.file} {x.ac["hex"]}").ToList();
            Sq.Report(bad, missing.Count, "aircraft without a squawk marked unknown");
        }

        [TestMethod]
        public void Emergency_7700_is_forwarded_as_the_MAVLink_spec_example()
        {
            var (f, ac, p) = All().First(x => x.file == "lol_sqk7700.json");
            Assert.AreEqual("7700", ac.Value<string>("squawk"));
            // MAVLink common.xml: 7700 "is encoded as binary 0b0001_1110_0001_0100"
            Assert.AreEqual(0b0001_1110_0001_0100, p.Squawk);
            var parse = new MAVLink.MavlinkParse();
            var pkt = new MAVLink.mavlink_adsb_vehicle_t { squawk = p.Squawk, flags = (ushort)MAVLink.ADSB_FLAGS.VALID_SQUAWK, callsign = new byte[9] };
            var bytes = parse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.ADSB_VEHICLE, pkt);
            var back = parse.ReadPacket(new MemoryStream(bytes)).ToStructure<MAVLink.mavlink_adsb_vehicle_t>();
            Assert.AreEqual((ushort)7700, back.squawk);
        }
    }

    [TestClass]
    public class Parsing
    {
        [DataTestMethod]
        [DataRow("1200", (ushort)1200)]
        [DataRow("7700", (ushort)7700)]
        [DataRow("0000", (ushort)0)]
        [DataRow("0504", (ushort)504)]
        [DataRow("504", (ushort)504)]
        [DataRow(" 7000\r\n", (ushort)7000)]
        public void Accepts_octal_digit_squawks(string text, ushort want)
        {
            Assert.IsTrue(adsb.TryParseSquawk(text, out var got));
            Assert.AreEqual(want, got);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("8000")]   // 8 is not an octal digit
        [DataRow("1289")]
        [DataRow("12000")]
        [DataRow("04B0")]   // what the old hex formatting produced for 1200
        [DataRow("-1")]
        public void Rejects_anything_that_is_not_a_squawk(string text)
        {
            Assert.IsFalse(adsb.TryParseSquawk(text, out _));
        }

        [TestMethod]
        public void Unknown_squawk_displays_empty_and_known_zero_displays_0000()
        {
            var p = new adsb.PointLatLngAltHdg(0, 0, 0, 0, 0, "ABCDEF", DateTime.Now);
            Assert.AreEqual("", p.SquawkString);
            p.SquawkValid = true;
            Assert.AreEqual("0000", p.SquawkString);
        }
    }

    // ------------------------------------------------------------------------------------------
    // SBS-1 / BaseStation (port 30003)
    // ------------------------------------------------------------------------------------------
    [TestClass]
    public class Sbs
    {
        static JArray Golden => (JArray)Fx.Json("rf", "pymodes_df21.json")["frames"];

        [TestMethod]
        public void Dump1090fa_MSG6_squawk_reaches_the_position_report()
        {
            // dump1090-fa and readsb only put field 18 on MSG,6 (DF5/DF21); MSG,3 leaves it empty
            var got = Feed.LastByIcao(Feed.Stream(Fx.Bytes("sbs", "dump1090fa_df21.sbs")));
            var bad = new List<string>(); int n = 0;
            foreach (var icao in Golden.Select(g => (string)g["icao"]).Distinct())
            {
                var want = (string)Golden.Last(g => (string)g["icao"] == icao)["squawk"];
                n++;
                if (!got.TryGetValue(Feed.Norm(icao), out var p)) { bad.Add($"{icao}: no position emitted"); continue; }
                if (p.Squawk != Sq.Expected(want) || Sq.Display(p) != want)
                    bad.Add($"{icao}: MSG,6 said \"{want}\", position report carries {p.Squawk} (\"{Sq.Display(p)}\")");
            }
            Sq.Report(bad, n, "dump1090-fa SBS aircraft");
        }

        [TestMethod]
        public void Antirez_unpadded_squawk_on_MSG3_parses_and_displays_padded()
        {
            var got = Feed.LastByIcao(Feed.Stream(Fx.Bytes("sbs", "antirez_style.sbs")));
            var bad = new List<string>(); int n = 0;
            foreach (var g in Golden.Take(40))
            {
                var want = (string)g["squawk"];
                if (!got.TryGetValue(Feed.Norm((string)g["icao"]), out var p)) continue;
                n++;
                if (p.Squawk != Sq.Expected(want) || Sq.Display(p) != want)
                    bad.Add($"{g["icao"]}: \"{int.Parse(want)}\" -> {p.Squawk} (\"{Sq.Display(p)}\"), expected \"{want}\"");
            }
            Sq.Report(bad, n, "antirez-style SBS squawks");
        }

        [TestMethod]
        public void Live_positions_with_an_empty_squawk_field_are_unknown()
        {
            var pts = Feed.Stream(Fx.Bytes("sbs", "dump1090fa_live_airspy.sbs"));
            Assert.IsTrue(pts.Count > 0, "no live positions emitted");
            var v = Sq.Valid(pts[0]);
            if (v == null)
                Assert.Fail($"empty SBS squawk field becomes Squawk=0 (\"{Sq.Display(pts[0])}\") with no way to mark it unknown");
            Sq.Report(pts.Where(p => Sq.Valid(p) != false).Select(p => p.Tag).ToList(), pts.Count, "live SBS positions with no squawk");
        }
    }

    // ------------------------------------------------------------------------------------------
    // Raw Mode S: AVR (port 30002) and Beast binary (port 30005)
    // ------------------------------------------------------------------------------------------
    [TestClass]
    public partial class ModeS
    {
        static JObject Live => Fx.Json("rf", "live_airspy_frames.json");
        static JArray Golden => (JArray)Fx.Json("rf", "pymodes_df21.json")["frames"];

        void LiveTc28(string file)
        {
            var got = Feed.LastByIcao(Feed.Stream(Fx.Bytes(file.EndsWith(".avr") ? "avr" : "beast", file)));
            var bad = new List<string>(); int n = 0;
            foreach (var kv in (JObject)Live["adsb_lol_squawk"])
            {
                n++;
                if (!got.TryGetValue(Feed.Norm(kv.Key), out var p)) { bad.Add($"{kv.Key}: no position emitted"); continue; }
                var want = (string)kv.Value;
                if (p.Squawk != Sq.Expected(want) || Sq.Display(p) != want)
                    bad.Add($"{kv.Key}: broadcast (DF17 TC28) and adsb.lol both say \"{want}\", Mission Planner has {p.Squawk} (\"{Sq.Display(p)}\")");
            }
            Sq.Report(bad, n, "live over-the-air aircraft");
        }

        [TestMethod] public void Avr_live_air_squawk_matches_adsb_lol() => LiveTc28("live_airspy.avr");
        [TestMethod] public void Beast_live_air_squawk_matches_adsb_lol() => LiveTc28("live_airspy.bin");

        /// <summary>
        /// adsb.ReadMessage(Stream) only switches into AVR mode at the 4th '*', so the first three
        /// frames of every connection are dropped. That is existing behaviour, unrelated to squawks;
        /// three bare '*' let the fixture's first frame (the address seed) through.
        /// </summary>
        static byte[] AvrPrimed(byte[] b) => Encoding.ASCII.GetBytes("***\n").Concat(b).ToArray();

        void GoldenDf21(string dir, string file)
        {
            var bytes = Fx.Bytes(dir, file);
            Feed.Stream(dir == "avr" ? AvrPrimed(bytes) : bytes);
            var bad = new List<string>(); int n = 0;
            foreach (var icao in Golden.Select(g => (string)g["icao"]).Distinct())
            {
                var want = (string)Golden.Last(g => (string)g["icao"] == icao)["squawk"];
                var (supported, valid, value) = Feed.PlaneSquawk(icao);
                if (!supported) Assert.Fail("adsb.Plane does not store a squawk: DF5/DF21 identity replies and DF17 TC28 are never decoded");
                n++;
                if (!valid || value != Sq.Expected(want)) bad.Add($"{icao}: pyModeS \"{want}\", Mission Planner valid={valid} value={value}");
            }
            Sq.Report(bad, n, $"DF21 aircraft via {dir}");
        }

        [TestMethod] public void Avr_DF21_identity_replies_match_pyModeS() => GoldenDf21("avr", "pymodes_df21.avr");
        [TestMethod] public void Beast_DF21_identity_replies_match_pyModeS_including_escaped_0x1A() => GoldenDf21("beast", "pymodes_df21.bin");

        // DF5 short frames: the same identity fields as the pyModeS DF21 frames, parity recomputed for 56 bits
        [TestMethod] public void Avr_DF5_short_identity_replies_decode() => GoldenDf21("avr", "derived_df5.avr");
        [TestMethod] public void Beast_DF5_short_frames_decode() => GoldenDf21("beast", "derived_df5.bin");

        [TestMethod]
        public void Identity_reply_from_an_unheard_address_is_ignored()
        {
            // no DF11/DF17 seed first: the CRC residual cannot be trusted as an address
            var lines = string.Concat(Golden.Take(20).Select(g => "*" + (string)g["hex"] + ";\n"));
            Feed.Stream(Encoding.ASCII.GetBytes(lines));
            foreach (var g in Golden.Take(20))
            {
                var (supported, valid, _) = Feed.PlaneSquawk((string)g["icao"]);
                if (!supported) Assert.Fail("adsb.Plane does not store a squawk");
                Assert.IsFalse(valid, $"{g["icao"]} got a squawk from an unverified DF21");
            }
        }

        [TestMethod]
        public void Avr_DF17_TC28_reference_frame_decodes_6513()
        {
            // pyModeS tests/test_bds61.py: 8DA2C1B6E112B600000000760759 -> squawk 6513
            var lines = string.Concat(Enumerable.Repeat("*8DA2C1B6E112B600000000760759;\n", 5));
            Feed.Stream(Encoding.ASCII.GetBytes(lines));
            var (supported, valid, value) = Feed.PlaneSquawk("A2C1B6");
            if (!supported) Assert.Fail("adsb.Plane does not store a squawk");
            Assert.IsTrue(valid); Assert.AreEqual((ushort)6513, value);
        }
    }
}
