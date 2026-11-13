using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Utilities;

namespace MissionPlannerTests.Sitl
{
    /// <summary>
    /// A DataFlash log written by ArduCopter SITL, downloaded and read by MissionPlanner. See README.md.
    /// </summary>
    [TestClass]
    [TestCategory("SITL")]
    public class SitlDataFlashTests
    {
        static SitlVehicle sitl;
        // the log, as SITL wrote it and as downloaded
        static string onDisk;
        static string downloaded;
        static MAVLink.mavlink_log_entry_t entry;

        const uint Sys = SitlVehicle.SysId;
        const byte Comp = SitlVehicle.CompId;

        [ClassInitialize]
        public static void Start(TestContext context)
        {
            if (!SitlVehicle.IsConfigured)
                return;

            // log from boot, without arming
            sitl = SitlVehicle.Start(parameters: new Dictionary<string, double> { { "LOG_DISARMED", 1 } });

            // long enough for a GPS fix
            sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT,
                m => m.ToStructure<MAVLink.mavlink_gps_raw_int_t>().fix_type >= 3, TimeSpan.FromSeconds(60));
            Thread.Sleep(2000);

            // stop logging so the file is complete
            Assert.IsTrue(sitl.Mav.setParam(Sys, Comp, "LOG_DISARMED", 0));
            Thread.Sleep(2000);

#pragma warning disable CS0612
            var logs = sitl.Mav.GetLogEntry().GetAwaiter().GetResult();
#pragma warning restore CS0612
            Assert.IsTrue(logs.Count > 0, "the vehicle lists its logs");
            entry = logs.Values.OrderBy(a => a.id).Last();

            onDisk = Path.Combine(sitl.WorkDir, "logs", entry.id.ToString("00000000") + ".BIN");
            Assert.IsTrue(File.Exists(onDisk), onDisk);

            downloaded = sitl.Mav.GetLog(Sys, Comp, entry.id).GetAwaiter().GetResult();
        }

        [ClassCleanup]
        public static void Stop()
        {
            sitl?.Dispose();
            sitl = null;
            if (downloaded != null)
                File.Delete(downloaded);
        }

        [TestInitialize]
        public void RequireSitl()
        {
            if (sitl == null)
                Assert.Inconclusive(SitlVehicle.NotConfigured);
        }

        [TestMethod]
        public void DownloadedLogIsTheFileTheVehicleWrote()
        {
            byte[] expected = File.ReadAllBytes(onDisk);
            Assert.IsTrue(expected.Length > 50000, "a log of " + expected.Length + " bytes");
            Assert.AreEqual((uint)expected.Length, entry.size, "LOG_ENTRY size");
            CollectionAssert.AreEqual(expected, File.ReadAllBytes(downloaded));
        }

        [TestMethod]
        public void LogDownloadsOverMavftpToo()
        {
            var ftp = new MAVFtp(sitl.Mav, Sys, Comp);
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(120)))
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                // burst reads of the largest packets: ArduPilot sends a fixed number of packets a second, the
                // default 80 bytes is three times slower over TCP
                using (var file = ftp.GetFile("logs/" + Path.GetFileName(onDisk), cancel, true, 239))
                {
                    Console.WriteLine("MAVFTP read " + file.Length + " bytes in " + timer.Elapsed);
                    CollectionAssert.AreEqual(File.ReadAllBytes(onDisk), file.ToArray());
                }
            }
        }

        [TestMethod]
        public void LogParses()
        {
            using (var log = new DFLogBuffer(downloaded))
            {
                var types = log.SeenMessageTypes;
                CollectionAssert.IsSubsetOf(new[] { "FMT", "FMTU", "UNIT", "MULT", "PARM", "MSG", "ATT", "IMU", "GPS", "BARO", "BAT" }, types);

                // every message is one of a type the log defines
                Assert.AreEqual(log.Count, types.Sum(a => log.GetEnumeratorType(a).Count()));

                Assert.IsTrue(log.GetEnumeratorType("MSG").Any(a => a["Message"].StartsWith("ArduCopter V")));
                var logDisarmed = log.GetEnumeratorType("PARM").First(a => a["Name"] == "LOG_DISARMED");
                Assert.AreEqual("1", logDisarmed["Value"]);

                foreach (var type in new[] { "ATT", "IMU[0]", "GPS[0]", "BARO[0]" })
                {
                    var times = log.GetEnumeratorType(type).Select(a => a.GetRaw<ulong>("TimeUS")).ToList();
                    Assert.IsTrue(times.Count > 5, type);
                    for (int i = 1; i < times.Count; i++)
                        Assert.IsTrue(times[i] > times[i - 1], type + " times go forward");
                }

                // the log has a fix, it sets the log's clock
                Assert.IsTrue(log.GetEnumeratorType("GPS").Any(a => int.Parse(a["Status"]) >= 3));
                var start = log.dflog.gpsstarttime;
                Assert.AreNotEqual(DateTime.MinValue, start);
                Assert.IsTrue(Math.Abs((start.ToUniversalTime() - DateTime.UtcNow).TotalDays) < 2,
                    "SITL's GPS time is now, not " + start);

                var gps = log.GetEnumeratorType("GPS").Last();
                Assert.AreEqual(-35.363261, double.Parse(gps["Lat"], System.Globalization.CultureInfo.InvariantCulture), 0.001);
                Assert.AreEqual(149.165230, double.Parse(gps["Lng"], System.Globalization.CultureInfo.InvariantCulture), 0.001);

                Assert.AreEqual(Tuple.Create("V", 1.0), log.GetUnit("BAT", "Volt"));
            }
        }

        [TestMethod]
        public void LogConvertsToText()
        {
            string text = Path.ChangeExtension(downloaded, ".log");
            try
            {
                BinaryLog.ConvertBin(downloaded, text);
                using (var binary = new DFLogBuffer(downloaded))
                using (var converted = new DFLogBuffer(text))
                {
                    Assert.AreEqual(binary.Count, converted.Count);
                    // MSG is left out: the text form splits its text at ':' and ','
                    foreach (var type in new[] { "PARM", "GPS", "ATT", "IMU" })
                    {
                        var expected = binary.GetEnumeratorType(type).ToList();
                        var actual = converted.GetEnumeratorType(type).ToList();
                        Assert.AreEqual(expected.Count, actual.Count, type);
                        for (int i = 0; i < expected.Count; i++)
                            CollectionAssert.AreEqual(expected[i].items, actual[i].items.Select(a => a.Trim()).ToArray(), type + " " + i);
                    }
                }
            }
            finally
            {
                File.Delete(text);
            }
        }
    }
}
