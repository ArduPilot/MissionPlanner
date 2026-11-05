// Unit-conversion tests for adsb.PointLatLngAltHdg: Alt in metres, Speed and VerticalSpeed in cm/s,
// Heading in degrees. Exact factors: 1 ft = 0.3048 m, 1 kt = 51.4444 cm/s, 1 ft/min = 0.508 cm/s.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using Newtonsoft.Json.Linq;

namespace ADSBTests
{
    static class U
    {
        public const double FT = 0.3048, KT_CMS = 51.44444, FPM_CMS = 0.508;
        public static bool Near(double got, double want, double relTol, double absTol) =>
            Math.Abs(got - want) <= Math.Max(absTol, Math.Abs(want) * relTol);
        public static int? AltBaroFt(JObject ac) => ac["alt_baro"]?.Type == JTokenType.Integer ? (int?)ac.Value<int>("alt_baro") : null;
    }

    public partial class HttpApi
    {
        [TestMethod]
        public void Http_altitude_falls_back_to_alt_baro_in_metres()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                var ft = U.AltBaroFt(ac);
                if (ft == null || p == null || ac["alt_geom"] != null) continue;
                n++;
                if (!U.Near(p.Alt, ft.Value * U.FT, 0.001, 0.5)) bad.Add($"{ac["hex"]}: {ft} ft -> {p.Alt:F1} m, expected {ft * U.FT:F1}");
            }
            Sq.Report(bad, n, "live altitudes (within 0.1%)");
        }

        [TestMethod]
        public void Http_ground_speed_is_knots_converted_to_cm_per_s()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                if (ac["gs"] == null || p == null) continue;
                n++;
                double want = ac.Value<double>("gs") * U.KT_CMS;
                if (!U.Near(p.Speed, want, 0.001, 1)) bad.Add($"{ac["hex"]}: {ac["gs"]} kt -> {p.Speed:F0} cm/s, expected {want:F0}");
            }
            Sq.Report(bad, n, "live ground speeds");
        }

        [TestMethod]
        public void Http_heading_is_track_in_degrees()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                if (ac["track"] == null || p == null) continue;
                n++;
                if (Math.Abs(p.Heading - ac.Value<double>("track")) > 0.01) bad.Add($"{ac["hex"]}: track {ac["track"]} -> {p.Heading}");
            }
            Sq.Report(bad, n, "live headings");
        }

        [TestMethod]
        public void Http_vertical_speed_is_baro_rate_converted_to_cm_per_s()
        {
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                if (ac["baro_rate"] == null || p == null) continue;
                n++;
                double want = ac.Value<double>("baro_rate") * U.FPM_CMS;
                if (!U.Near(p.VerticalSpeed, want, 0.001, 0.5)) bad.Add($"{ac["hex"]}: {ac["baro_rate"]} ft/min -> {p.VerticalSpeed:F1} cm/s, expected {want:F1}");
            }
            Sq.Report(bad, n, "live vertical speeds from baro_rate");
        }

        [TestMethod]
        public void Http_vertical_speed_falls_back_to_geom_rate()
        {
            // readsb sends baro_rate or geom_rate depending on what the aircraft broadcasts
            var bad = new List<string>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                if (ac["baro_rate"] != null || ac["geom_rate"] == null || p == null) continue;
                n++;
                double want = ac.Value<double>("geom_rate") * U.FPM_CMS;
                if (!U.Near(p.VerticalSpeed, want, 0.01, 1)) bad.Add($"{f} {ac["hex"]}: geom_rate {ac["geom_rate"]} ft/min, no baro_rate -> VerticalSpeed {p.VerticalSpeed:F0} cm/s");
            }
            Sq.Report(bad, n, "live aircraft reporting only geom_rate");
        }

        [TestMethod]
        public void Http_altitude_forwarded_as_GEOMETRIC_matches_alt_geom()
        {
            // MainV2.ADSBRunner sends Alt with ADSB_ALTITUDE_TYPE.GEOMETRIC, but the HTTP path fills Alt from alt_baro
            var diffs = new List<(string hex, double ft)>(); int n = 0;
            foreach (var (f, ac, p) in All())
            {
                if (ac["alt_geom"] == null || p == null || U.AltBaroFt(ac) == null) continue;
                n++;
                double errFt = p.Alt / U.FT - ac.Value<double>("alt_geom");
                if (Math.Abs(errFt) > 100) diffs.Add(((string)ac["hex"], errFt));
            }
            if (diffs.Count > 0)
            {
                var abs = diffs.Select(d => Math.Abs(d.ft)).OrderBy(x => x).ToList();
                Assert.Fail($"{diffs.Count} of {n} aircraft are off from their geometric altitude by more than 100 ft " +
                            $"(median {abs[abs.Count / 2]:F0} ft, worst {abs.Last():F0} ft). Examples: " +
                            string.Join(", ", diffs.OrderByDescending(d => Math.Abs(d.ft)).Take(4).Select(d => $"{d.hex} {d.ft:+0;-0} ft")));
            }
        }

        [TestMethod]
        public void Http_aircraft_without_altitude_are_not_placed_at_sea_level()
        {
            var none = All().Where(x => x.p != null && x.ac["alt_baro"] == null && x.ac["alt_geom"] == null).ToList();
            if (none.Count == 0) Assert.Inconclusive("live data had no aircraft without an altitude");
            var bad = none.Where(x => x.p.AltValid).Select(x => $"{x.file} {x.ac["hex"]}: no altitude, but AltValid with Alt {x.p.Alt} m").ToList();
            Sq.Report(bad, none.Count, "live aircraft without an altitude marked unknown");
            // and everything that did send one is still valid
            var lost = All().Where(x => x.p != null && (U.AltBaroFt(x.ac) != null || x.ac["alt_geom"] != null) && !x.p.AltValid).ToList();
            Assert.AreEqual(0, lost.Count, "aircraft with an altitude marked unknown");
        }
    }

    [TestClass]
    public class SbsUnits
    {
        [TestMethod]
        public void Sbs_live_MSG4_and_MSG3_convert_to_metres_and_cm_per_s()
        {
            var lines = System.IO.File.ReadAllLines(Fx.PathOf("sbs", "dump1090fa_live_airspy_with_velocity.sbs"));
            var pts = Feed.Stream(Fx.Bytes("sbs", "dump1090fa_live_airspy_with_velocity.sbs"));
            Assert.IsTrue(pts.Count > 0, "no positions emitted");
            // replay the expectation: each MSG,3 carries the most recent MSG,4 values for that aircraft
            var last4 = new Dictionary<string, string[]>();
            var expected = new List<(string icao, double altM, double? spd, double? hdg, double? vs)>();
            foreach (var l in lines)
            {
                var a = l.Split(',');
                if (a[1] == "4") { last4[a[4]] = a; continue; }
                if (a[1] != "3" || a[14] == "") continue;
                last4.TryGetValue(a[4], out var v);
                expected.Add((a[4], double.Parse(a[11]) * U.FT,
                    v == null ? (double?)null : double.Parse(v[12]) * U.KT_CMS,
                    v == null ? (double?)null : double.Parse(v[13]),
                    v == null || v[16] == "" ? (double?)null : double.Parse(v[16]) * U.FPM_CMS));
            }
            Assert.AreEqual(expected.Count, pts.Count);
            var bad = new List<string>();
            for (int i = 0; i < pts.Count; i++)
            {
                var (icao, altM, spd, hdg, vs) = expected[i]; var p = pts[i];
                if (!U.Near(p.Alt, altM, 0.001, 1)) bad.Add($"{icao} alt {p.Alt:F1} m, expected {altM:F1}");
                if (spd != null && !U.Near(p.Speed, spd.Value, 0.001, 52)) bad.Add($"{icao} speed {p.Speed:F0} cm/s, expected {spd:F0}");
                if (hdg != null && Math.Abs(p.Heading - hdg.Value) > 1) bad.Add($"{icao} heading {p.Heading}, expected {hdg}");
                if (vs != null && !U.Near(p.VerticalSpeed, vs.Value, 0.001, 1)) bad.Add($"{icao} vs {p.VerticalSpeed:F1} cm/s, expected {vs:F1}");
            }
            Sq.Report(bad, pts.Count, "live SBS position reports");
        }
    }

    public partial class ModeS
    {
        static JArray Decoded => (JArray)Fx.Json("rf", "live_airspy_decoded.json")["frames"];

        void LiveUnits(string dir, string file, bool checkSpeed, bool checkVs)
        {
            var pts = Feed.Stream(Fx.Bytes(dir, file));
            Assert.IsTrue(pts.Count > 0, "no positions emitted");
            var bad = new List<string>(); int n = 0;
            foreach (var p in pts)
            {
                var mine = Decoded.Where(d => (string)d["icao"] == Feed.Norm(p.Tag)).ToList();
                var vel = mine.Where(d => d["gs_kt"] != null).Select(d => (JObject)d).ToList();
                var alts = mine.Where(d => d["alt_ft"] != null && d["alt_ft"].Type != JTokenType.Null).Select(d => (double)d["alt_ft"] * U.FT).ToList();
                n++;
                if (alts.Count > 0 && alts.Min(a => Math.Abs(a - p.Alt)) > 8)
                    bad.Add($"{p.Tag}: alt {p.Alt:F0} m is not any decoded altitude");
                if (vel.Count == 0) continue;
                // Speed must be one of the independently decoded ground speeds (raw-1 knots)
                if (checkSpeed && vel.Min(v => Math.Abs((double)v["gs_kt"] * U.KT_CMS - p.Speed)) > 0.5 * U.KT_CMS)
                    bad.Add($"{p.Tag}: speed {p.Speed / U.KT_CMS:F1} kt, independent decode has " +
                            string.Join("/", vel.Select(v => ((double)v["gs_kt"]).ToString("F1")).Distinct().Take(4)) + " kt");
                if (checkVs && vel.Any(v => v["vrate_fpm"] != null && v["vrate_fpm"].Type != JTokenType.Null && (int)v["vrate_fpm"] != 0)
                    && vel.Min(v => Math.Abs((double)(v["vrate_fpm"] ?? 0) * U.FPM_CMS - p.VerticalSpeed)) > 64 * U.FPM_CMS)
                    bad.Add($"{p.Tag}: vertical speed {p.VerticalSpeed / U.FPM_CMS:F0} ft/min, aircraft broadcast " +
                            string.Join("/", vel.Select(v => (string)v["vrate_fpm"]).Distinct().Take(4)) + " ft/min");
            }
            Sq.Report(bad.Distinct().ToList(), n, $"live {dir} positions");
        }

        [TestMethod] public void Avr_live_altitude_is_metres() => LiveUnits("avr", "live_airspy.avr", false, false);
        [TestMethod] public void Avr_live_ground_speed_matches_independent_decode() => LiveUnits("avr", "live_airspy.avr", true, false);
        [TestMethod] public void Avr_live_vertical_speed_is_decoded() => LiveUnits("avr", "live_airspy.avr", false, true);
        [TestMethod] public void Beast_live_ground_speed_matches_independent_decode() => LiveUnits("beast", "live_airspy.bin", true, false);
    }

    [TestClass]
    public class ThreatInputs
    {
        static readonly adsb.ThreatThresholds Warn = new adsb.ThreatThresholds { TimeHorizon = 30, DistanceXY = 1000, DistanceZ = 300 };
        static readonly adsb.ThreatThresholds Crit = new adsb.ThreatThresholds { TimeHorizon = 30, DistanceXY = 300, DistanceZ = 100 };

        /// <summary>
        /// MainV2.ADSBRunner reads CurrentState getters, which return display units (altasl * multiplieralt,
        /// groundspeed/climbrate * multiplierspeed), and passes them with the multipliers to Ownship.FromDisplayUnits.
        /// Same geometry, two GCS display settings.
        /// </summary>
        [TestMethod]
        public void Threat_level_does_not_depend_on_GCS_display_units()
        {
            // ownship 400 m AMSL flying north at 20 m/s; traffic 1500 m north, 450 m AMSL, heading south at 60 m/s
            var traffic = new adsb.PointLatLngAltHdg(46.2 + 1500 / 111320.0, -119.1, 450, 180, 6000, "ABCDEF", DateTime.Now);

            MAVLink.MAV_COLLISION_THREAT_LEVEL Level(double altMult, double spdMult) =>
                adsb.AssessThreat(adsb.Ownship.FromDisplayUnits(46.2, -119.1, 400 * altMult, 20 * spdMult, 0, 0, altMult, spdMult),
                    traffic, Warn, Crit).Level;

            var metric = Level(1, 1);
            var imperial = Level(3.28084, 1.943844);   // feet + knots, as set in Config > Planner
            Assert.AreEqual(MAVLink.MAV_COLLISION_THREAT_LEVEL.HIGH, metric, "sanity: head-on at 50 m vertical separation");
            Assert.AreEqual(metric, imperial, $"same encounter is {metric} with metric display and {imperial} with feet/knots display");
        }

        [TestMethod]
        public void Traffic_with_no_altitude_is_judged_on_distance_alone()
        {
            var us = adsb.Ownship.FromDisplayUnits(46.2, -119.1, 120, 0, 0, 0, 1, 1);
            var traffic = new adsb.PointLatLngAltHdg(46.2 + 200 / 111320.0, -119.1, 0, 180, 0, "ABCDEF", DateTime.Now) { AltValid = false };
            Assert.AreEqual(MAVLink.MAV_COLLISION_THREAT_LEVEL.HIGH, adsb.AssessThreat(us, traffic, Warn, Crit).Level,
                "200 m away with unknown altitude must not be cleared as 120 m below us");
        }
    }
}
