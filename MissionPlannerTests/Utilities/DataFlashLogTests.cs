using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Utilities;
using Newtonsoft.Json.Linq;

namespace MissionPlannerTests.Utilities
{
    [TestClass]
    public class DataFlashLogTests
    {
        static readonly short[] Shorts = Enumerable.Range(1, 32).Select(a => (short)(a * (a % 2 == 0 ? 1 : -100))).ToArray();

        /// <summary>
        /// a log with one message of every format character, GPS, battery instances and units
        /// </summary>
        static DataFlashLogWriter SampleLog()
        {
            var log = new DataFlashLogWriter().DefineUnits();
            log.Define(200, "TSTA", "QbBhHiIqQfd", "TimeUS,b,B,h,H,i,I,q,Q,f,d");
            log.Define(201, "TSTB", "QcCeELnNZMa", "TimeUS,c,C,e,E,L,n,N,Z,M,a");
            log.Define(202, "GPS", "QBBIHBcLLeffffB", "TimeUS,I,Status,GMS,GWk,NSats,HDop,Lat,Lng,Alt,Spd,GCrs,VZ,Yaw,U");
            log.Define(203, "BAT", "QBf", "TimeUS,Inst,Volt");
            log.Define(204, "ATT", "QccC", "TimeUS,Roll,Pitch,Yaw");
            log.Write("FMTU", 0L, (byte)203, "s#v", "F-0");
            log.Write("FMTU", 0L, (byte)204, "sddd", "FBBB");
            log.Write("FMTU", 0L, (byte)202, "s#-------------", "F--------------");

            log.Write("TSTA", 1000L, (sbyte)-5, (byte)250, (short)-30000, (ushort)65000, -2000000000, 4000000000u,
                -9000000000000000000L, ulong.MaxValue, 1.5f, 2.25);
            log.Write("TSTB", 2000L, (short)-12345, (ushort)54321, -1234567, 4000000000u, -353632610,
                "ABCD", "sixteen_chars_ok", "hello world", (byte)5, Shorts);
            // no fix yet: not used for the log's time
            log.Write("GPS", 9000000L, (byte)0, (byte)1, 0u, (ushort)0, (byte)3, (short)121, 0, 0, 0, 0f, 0f, 0f, 0f, (byte)1);
            log.Write("GPS", 10000000L, (byte)0, (byte)3, 100000u, (ushort)2300, (byte)10, (short)121, -353632610, 1491652300, 58400, 0f, 0f, 0f, 0f, (byte)1);
            for (int i = 0; i < 4; i++)
            {
                log.Write("BAT", 10000000L + i * 100000, (byte)(i % 2), 12.5f + i);
                log.Write("ATT", 10000000L + i * 100000 + 50000, (short)(i * 100), (short)-150, (ushort)35999);
            }
            return log;
        }

        static DFLogBuffer Open(byte[] log)
        {
            return new DFLogBuffer(new MemoryStream(log));
        }

        [TestMethod]
        public void DecodesEveryFormatCharacter()
        {
            using (var buffer = Open(SampleLog().ToArray()))
            {
                var a = buffer.GetEnumeratorType("TSTA").Single();
                Assert.AreEqual("TSTA", a.msgtype);
                Assert.AreEqual(1000ul, a.GetRaw<ulong>("TimeUS"));
                Assert.AreEqual((sbyte)-5, a.GetRaw<sbyte>("b"));
                Assert.AreEqual((byte)250, a.GetRaw<byte>("B"));
                Assert.AreEqual((short)-30000, a.GetRaw<short>("h"));
                Assert.AreEqual((ushort)65000, a.GetRaw<ushort>("H"));
                Assert.AreEqual(-2000000000, a.GetRaw<int>("i"));
                Assert.AreEqual(4000000000u, a.GetRaw<uint>("I"));
                Assert.AreEqual(-9000000000000000000L, a.GetRaw<long>("q"));
                Assert.AreEqual(ulong.MaxValue, a.GetRaw<ulong>("Q"));
                Assert.AreEqual(1.5f, a.GetRaw<float>("f"));
                Assert.AreEqual(2.25, a.GetRaw<double>("d"));
                Assert.AreEqual("18446744073709551615", a["Q"]);
                Assert.AreEqual("-5", a["b"]);
                Assert.AreEqual(1.0, a.timems);

                var b = buffer.GetEnumeratorType("TSTB").Single();
                Assert.AreEqual(-123.45, (double)b.GetRaw("c"), 1e-9);
                Assert.AreEqual(543.21, (double)b.GetRaw("C"), 1e-9);
                Assert.AreEqual(-12345.67, (double)b.GetRaw("e"), 1e-9);
                Assert.AreEqual(40000000.0, (double)b.GetRaw("E"), 1e-9);
                Assert.AreEqual(-35.363261, (double)b.GetRaw("L"), 1e-12);
                Assert.AreEqual("ABCD", b["n"]);
                Assert.AreEqual("sixteen_chars_ok", b["N"]);
                Assert.AreEqual("hello world", b["Z"]);
                Assert.AreEqual("5", b["M"]);
                var array = (BinaryLog.UnionArray)b.GetRaw("a");
                CollectionAssert.AreEqual(Shorts, array.Shorts.ToArray());
                Assert.AreEqual("[" + string.Join(" ", Shorts) + "]", b["a"]);
                Assert.IsNull(b["NoSuchField"]);

                var dictionary = a.ToDictionary();
                Assert.AreEqual(11, dictionary.Count);
                Assert.AreEqual(2.25, Convert.ToDouble(dictionary["d"]));
            }
        }

        [TestMethod]
        public void IndexesMessagesAndFormats()
        {
            var log = SampleLog();
            using (var buffer = Open(log.ToArray()))
            {
                // FMT x9, UNIT x7, MULT x4, FMTU x3, TSTA, TSTB, GPS x2, BAT x4, ATT x4
                Assert.AreEqual(9 + 7 + 4 + 3 + 1 + 1 + 2 + 4 + 4, buffer.Count);
                Assert.AreEqual(9, buffer.GetEnumeratorType("FMT").Count());
                CollectionAssert.IsSubsetOf(new[] { "FMT", "UNIT", "MULT", "FMTU", "TSTA", "TSTB", "GPS", "BAT", "ATT" },
                    buffer.SeenMessageTypes);
                Assert.AreEqual(4, buffer.GetEnumeratorType("ATT").Count());
                Assert.AreEqual(0, buffer.GetEnumeratorType("NONE").Count());

                Assert.AreEqual("TimeUS,Inst,Volt", buffer.FMT[203].columns);
                Assert.AreEqual("QBf", buffer.FMT[203].format);
                Assert.AreEqual(DataFlashLogWriter.MessageLength("QBf"), buffer.FMT[203].length);
                Assert.AreEqual("BAT", buffer.FMT[203].name);
                Assert.AreEqual(3, buffer.dflog.FindMessageOffset("BAT", "Volt"));
                Assert.AreEqual(-1, buffer.dflog.FindMessageOffset("BAT", "Nope"));

                // lines are in file order, the line numbers of a type index the whole log
                var lines = buffer.GetEnumeratorType(new[] { "ATT", "BAT" }).ToList();
                Assert.AreEqual(8, lines.Count);
                CollectionAssert.AreEqual(lines.Select(a => a.lineno).OrderBy(a => a).ToList(), lines.Select(a => a.lineno).ToList());
                CollectionAssert.AreEqual(new[] { "BAT", "ATT", "BAT", "ATT", "BAT", "ATT", "BAT", "ATT" }, lines.Select(a => a.msgtype).ToArray());
                foreach (var line in lines)
                    Assert.AreEqual(line.msgtype, buffer[(long)line.lineno].msgtype);
            }
        }

        [TestMethod]
        public void SelectsInstances()
        {
            using (var buffer = Open(SampleLog().ToArray()))
            {
                Assert.AreEqual(2, buffer.getInstanceIndex("BAT"));
                Assert.AreEqual(-1, buffer.getInstanceIndex("NONE"));
                CollectionAssert.AreEquivalent(new[] { "0", "1" }, buffer.InstanceType[203].value);

                var first = buffer.GetEnumeratorType("BAT[0]").ToList();
                CollectionAssert.AreEqual(new[] { 12.5f, 14.5f }, first.Select(a => a.GetRaw<float>("Volt")).ToArray());
                Assert.IsTrue(first.All(a => a.instance == "0"));

                var second = buffer.GetEnumeratorType("BAT[1]").ToList();
                CollectionAssert.AreEqual(new[] { 13.5f, 15.5f }, second.Select(a => a.GetRaw<float>("Volt")).ToArray());
                // BAT2 is the second instance, BAT the lot
                CollectionAssert.AreEqual(second.Select(a => a.lineno).ToList(), buffer.GetEnumeratorType("BAT2").Select(a => a.lineno).ToList());
                Assert.AreEqual(4, buffer.GetEnumeratorType("BAT").Count());
                Assert.AreEqual(4, buffer.GetEnumeratorType(new[] { "BAT[1]", "GPS[0]" }).Count());
            }
        }

        [TestMethod]
        public void InstanceColumnIsTheOneOfEachLog()
        {
            // the same name, with the instance in another column
            var other = new DataFlashLogWriter().DefineUnits();
            other.Define(203, "BAT", "QfB", "TimeUS,Volt,Inst");
            other.Write("FMTU", 0L, (byte)203, "sv#", "F0-");
            other.Write("BAT", 1L, 11.1f, (byte)3);

            using (var buffer = Open(SampleLog().ToArray()))
                Assert.AreEqual("1", buffer.GetEnumeratorType("BAT").ElementAt(1).instance);
            using (var buffer = Open(other.ToArray()))
            {
                Assert.AreEqual("3", buffer.GetEnumeratorType("BAT").Single().instance);
                Assert.AreEqual(1, buffer.GetEnumeratorType("BAT[3]").Count());
            }
        }

        [TestMethod]
        public void ReadsUnitsAndMultipliers()
        {
            using (var buffer = Open(SampleLog().ToArray()))
            {
                Assert.AreEqual("V", buffer.Unit['v']);
                Assert.AreEqual("1E-06", buffer.Mult['F']);
                Assert.AreEqual(Tuple.Create("V", 1.0), buffer.GetUnit("BAT", "Volt"));
                Assert.AreEqual(Tuple.Create("s", 1e-6), buffer.GetUnit("BAT", "TimeUS"));
                // c and C are already scaled when read
                Assert.AreEqual(Tuple.Create("deg", 1.0), buffer.GetUnit("ATT", "Roll"));
                Assert.AreEqual(Tuple.Create("", 1.0), buffer.GetUnit("TSTA", "b"));
            }
        }

        [TestMethod]
        public void TimesAreRelativeToTheFirstGpsFix()
        {
            using (var buffer = Open(SampleLog().ToArray()))
            {
                var fix = DFLog.gpsTimeToTime(2300, 100);
                Assert.AreEqual(fix, buffer.dflog.gpsstarttime);

                var gps = buffer.GetEnumeratorType("GPS").Last();
                Assert.AreEqual(fix, gps.time);
                var att = buffer.GetEnumeratorType("ATT").ToList();
                for (int i = 0; i < att.Count; i++)
                    Assert.AreEqual(fix.AddMilliseconds(i * 100 + 50), att[i].time);

                // the line of the first position at or after a time: the GPS without a fix is a second earlier
                Assert.AreEqual(gps.lineno, buffer.dflog.GetLineNoFromTime(buffer, fix.AddMilliseconds(-500)));
            }
        }

        [TestMethod]
        public void SkipsNoiseAndBadBytesBetweenMessages()
        {
            var clean = SampleLog();
            var noisy = new DataFlashLogWriter().DefineUnits();
            noisy.Define(203, "BAT", "QBf", "TimeUS,Inst,Volt");
            noisy.WriteBytes(0, 1, 2, 0xA3, 0x00, 0xA3, 0xA3, 0x95, 0x00);
            noisy.Write("BAT", 1L, (byte)0, 1f);
            // a lone first header byte just before a message
            noisy.WriteBytes(0x55, 0x66, 0xA3);
            // the body holds the header bytes, it is not taken for a message
            noisy.Write("BAT", 2L, (byte)0, BitConverter.ToSingle(new byte[] { 0xA3, 0x95, 0x80, 0x41 }, 0));
            noisy.WriteBytes(0xA3, 0x95, 0x07, 9, 9, 9);
            noisy.Write("BAT", 3L, (byte)1, 3f);
            // cut short by the end of the log
            noisy.WriteBytes(0xA3, 0x95, 203, 4, 0, 0);

            using (var buffer = Open(noisy.ToArray()))
            {
                var bat = buffer.GetEnumeratorType("BAT").Take(3).ToList();
                CollectionAssert.AreEqual(new ulong[] { 1, 2, 3 }, bat.Select(a => a.GetRaw<ulong>("TimeUS")).ToArray());
                Assert.AreEqual(1f, bat[0].GetRaw<float>("Volt"));
                CollectionAssert.AreEqual(new byte[] { 0xA3, 0x95, 0x80, 0x41 }, BitConverter.GetBytes(bat[1].GetRaw<float>("Volt")));
                Assert.AreEqual((byte)1, bat[2].GetRaw<byte>("Inst"));

                // every line can be read, none throws
                for (long i = 0; i < buffer.Count; i++)
                    Assert.IsNotNull(buffer[i].raw);
            }
            Assert.IsTrue(clean.Length > 0);
        }

        [TestMethod]
        public void RandomAccessMatchesFileOrderInALargeLog()
        {
            var log = new DataFlashLogWriter().DefineUnits();
            log.Define(204, "ATT", "QffI", "TimeUS,Roll,Pitch,Seq");
            log.Define(205, "IMU", "QBfffZ", "TimeUS,I,AccX,AccY,AccZ,Pad");
            log.Write("FMTU", 0L, (byte)205, "s#ooo-", "F-----");
            const int count = 12000;
            for (int i = 0; i < count; i++)
            {
                log.Write("ATT", (long)i * 2500, i * 0.5f, -i * 0.25f, (uint)i);
                if (i % 3 == 0)
                    log.Write("IMU", (long)i * 2500 + 1, (byte)(i % 2), i * 1f, i * 2f, i * 3f, "x");
            }
            Assert.IsTrue(log.Length > 2 * 256 * 1024, "spans several read windows");

            using (var buffer = Open(log.ToArray()))
            {
                var forward = buffer.GetEnumeratorType("ATT").ToList();
                Assert.AreEqual(count, forward.Count);
                for (int i = 0; i < count; i++)
                {
                    Assert.AreEqual((uint)i, forward[i].GetRaw<uint>("Seq"));
                    Assert.AreEqual(i * 0.5f, forward[i].GetRaw<float>("Roll"));
                }

                // backwards, and jumping about, decode the same
                var random = new Random(5);
                foreach (int i in Enumerable.Range(0, count).Reverse().Concat(Enumerable.Range(0, 2000).Select(a => random.Next(count))))
                {
                    var line = buffer[(long)forward[i].lineno];
                    Assert.AreEqual((uint)i, line.GetRaw<uint>("Seq"));
                    Assert.AreEqual(-i * 0.25f, line.GetRaw<float>("Pitch"));
                }

                var imu = buffer.GetEnumeratorType("IMU").ToList();
                Assert.AreEqual(count / 3, imu.Count);
                Assert.IsTrue(imu.All(a => a.GetRaw<float>("AccZ") == 3 * a.GetRaw<float>("AccX")));
                Assert.AreEqual(count / 6, buffer.GetEnumeratorType("IMU[1]").Count());

                var both = buffer.GetEnumeratorType(new[] { "ATT", "IMU" }).ToList();
                Assert.AreEqual(count + count / 3, both.Count);
                for (int i = 1; i < both.Count; i++)
                    Assert.IsTrue(both[i].timems >= both[i - 1].timems);
            }
        }

        [TestMethod]
        public void TextConversionReadsBackTheSameValues()
        {
            string bin = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".bin");
            string text = Path.ChangeExtension(bin, ".log");
            try
            {
                File.WriteAllBytes(bin, SampleLog().ToArray());
                BinaryLog.ConvertBin(bin, text);

                using (var binary = new DFLogBuffer(bin))
                using (var converted = new DFLogBuffer(text))
                {
                    Assert.AreEqual(binary.Count, converted.Count);
                    CollectionAssert.AreEquivalent(binary.SeenMessageTypes, converted.SeenMessageTypes);
                    for (long i = 0; i < binary.Count; i++)
                    {
                        var expected = binary[i];
                        var actual = converted[i];
                        Assert.AreEqual(expected.msgtype, actual.msgtype);
                        // FMT columns are one field in the binary form
                        if (expected.msgtype == "FMT")
                            continue;
                        // the text form loses a trailing empty field, the UNIT with no label
                        var fields = expected.items.Reverse().SkipWhile(string.IsNullOrEmpty).Reverse().ToArray();
                        CollectionAssert.AreEqual(fields, actual.items.Select(a => a.Trim()).ToArray(), "line " + i);
                    }

                    var gps = converted.GetEnumeratorType("GPS").Last();
                    Assert.AreEqual("-35.363261", gps["Lat"].Trim());
                    Assert.AreEqual(binary.dflog.gpsstarttime, converted.dflog.gpsstarttime);
                    Assert.AreEqual(2, converted.GetEnumeratorType("BAT[1]").Count());
                }
            }
            finally
            {
                File.Delete(bin);
                File.Delete(text);
            }
        }

        static string TestData(string name)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", name);
        }

        /// <summary>
        /// A log written by ArduCopter SITL, decoded by pymavlink for the expected values. See
        /// TestData/README.md.
        /// </summary>
        [TestMethod]
        public void ArduCopterLogMatchesPymavlink()
        {
            var expected = JObject.Parse(File.ReadAllText(TestData("copter-sitl.expected.json")));
            using (var buffer = new DFLogBuffer(TestData("copter-sitl.bin")))
            {
                int total = 0;
                foreach (var count in (JObject)expected["counts"])
                {
                    var items = buffer.GetEnumeratorType(count.Key).ToList();
                    Assert.AreEqual((int)count.Value, items.Count, count.Key);
                    total += items.Count;

                    CompareMessage(count.Key + " first", (JObject)expected["first"][count.Key], items.First());
                    CompareMessage(count.Key + " last", (JObject)expected["last"][count.Key], items.Last());
                }
                Assert.AreEqual(total, buffer.Count);

                // the version, from MSG
                Assert.IsTrue(buffer.GetEnumeratorType("MSG").Any(a => a["Message"].StartsWith("ArduCopter V")));
                Assert.IsTrue(buffer.GetEnumeratorType("PARM").Any(a => a["Name"] == "LOG_DISARMED" && a["Value"] == "1"));
                Assert.AreNotEqual(DateTime.MinValue, buffer.dflog.gpsstarttime);

                // ArduPilot's own units, its multipliers are single precision
                Assert.AreEqual(Tuple.Create("V", 1.0), buffer.GetUnit("BAT", "Volt"));
                Assert.AreEqual("s", buffer.GetUnit("BAT", "TimeUS").Item1);
                Assert.AreEqual(1e-6, buffer.GetUnit("BAT", "TimeUS").Item2, 1e-13);
                Assert.AreEqual("m/s/s", buffer.GetUnit("IMU", "AccX").Item1);

                var imus = buffer.InstanceType[buffer.dflog.logformat["IMU"].Id].value;
                Assert.IsTrue(imus.Count > 1, "SITL has more than one IMU");
                foreach (var imu in imus)
                    Assert.IsTrue(buffer.GetEnumeratorType("IMU[" + imu + "]").All(a => a["I"] == imu));
                Assert.AreEqual(buffer.GetEnumeratorType("IMU").Count(),
                    imus.Sum(imu => buffer.GetEnumeratorType("IMU[" + imu + "]").Count()));
            }
        }

        static void CompareMessage(string name, JObject expected, DFLog.DFItem actual)
        {
            foreach (var field in expected)
            {
                var raw = actual.GetRaw(field.Key);
                string what = name + "." + field.Key;
                switch (field.Value.Type)
                {
                    case JTokenType.Integer:
                    case JTokenType.Float:
                        double want = (double)field.Value;
                        double have = Convert.ToDouble(raw, CultureInfo.InvariantCulture);
                        if (double.IsNaN(want))
                        {
                            Assert.IsTrue(double.IsNaN(have), what);
                            break;
                        }
                        Assert.AreEqual(want, have, Math.Max(1e-9, Math.Abs(want) * 1e-7), what);
                        break;
                    case JTokenType.String:
                        string text = raw is byte[] bytes ? Encoding.ASCII.GetString(bytes).TrimEnd('\0') : raw?.ToString();
                        if (actual.msgtype == "FMT" && field.Key == "Columns")
                            text = text.Replace(" ", "");
                        Assert.AreEqual((string)field.Value, text, what);
                        break;
                    case JTokenType.Array:
                        CollectionAssert.AreEqual(field.Value.Select(a => (short)a).ToArray(),
                            ((BinaryLog.UnionArray)raw).Shorts.ToArray(), what);
                        break;
                    default:
                        Assert.Fail(what + " " + field.Value.Type);
                        break;
                }
            }
        }
    }
}
