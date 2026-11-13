using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class MavlinkParamTests
    {
        static MAVLink.MAVLinkParam Param(string name, double value,
            MAVLink.MAV_PARAM_TYPE type = MAVLink.MAV_PARAM_TYPE.REAL32)
        {
            return new MAVLink.MAVLinkParam(name, value, type);
        }

        [TestMethod]
        public void ValuesKeepTheirWireType()
        {
            var cases = new (MAVLink.MAV_PARAM_TYPE type, double value, byte[] wire)[]
            {
                (MAVLink.MAV_PARAM_TYPE.UINT8, 200, new byte[] { 200, 0, 0, 0 }),
                (MAVLink.MAV_PARAM_TYPE.INT8, -100, new byte[] { 0x9c, 0, 0, 0 }),
                (MAVLink.MAV_PARAM_TYPE.UINT16, 65000, new byte[] { 0xe8, 0xfd, 0, 0 }),
                (MAVLink.MAV_PARAM_TYPE.INT16, -30000, new byte[] { 0xd0, 0x8a, 0, 0 }),
                (MAVLink.MAV_PARAM_TYPE.UINT32, 4000000000, BitConverter.GetBytes(4000000000u)),
                (MAVLink.MAV_PARAM_TYPE.INT32, -2000000000, BitConverter.GetBytes(-2000000000)),
                (MAVLink.MAV_PARAM_TYPE.REAL32, 0.8, BitConverter.GetBytes(0.8f)),
            };

            foreach (var item in cases)
            {
                var param = Param("P", item.value, item.type);
                CollectionAssert.AreEqual(item.wire, param.data, item.type.ToString());
                Assert.AreEqual(item.value, param.Value, item.type.ToString());

                // as received from the vehicle
                var received = new MAVLink.MAVLinkParam("P", item.wire, item.type, item.type);
                Assert.AreEqual(item.value, received.Value, item.type.ToString());
            }
        }

        [TestMethod]
        public void FloatsAreReportedAtSinglePrecision()
        {
            // 0.1f is 0.100000001490116 as a double, the user set 0.1
            Assert.AreEqual(0.1, Param("P", 0.1).Value);
            Assert.AreEqual(1234567, Param("P", 1234567).Value);
            Assert.AreEqual(-0.000123, Param("P", -0.000123).Value, 1e-12);
            Assert.AreEqual(0, Param("P", 0).Value);
            Assert.AreEqual("0.8", Param("P", 0.8).ToString());

            // exactly the value set, so it compares equal with a param file
            for (int i = -1000; i <= 1000; i++)
            {
                Assert.AreEqual((double)i, Param("P", i).Value, "integer " + i);
                Assert.AreEqual(i / 10.0, Param("P", i / 10.0).Value, "tenth " + i);
                Assert.AreEqual(i / 100.0, Param("P", i / 100.0).Value, "hundredth " + i);
            }
            Assert.AreEqual(1e-9, Param("P", 1e-9).Value);
            Assert.AreEqual(3.402823e38, Param("P", float.MaxValue).Value);
            Assert.IsTrue(double.IsNaN(Param("P", double.NaN).Value));
        }

        [TestMethod]
        public void IntegersAreStoredInFloatsByArduPilot()
        {
            // ArduPilot sends every param as REAL32 on the wire, TypeAP is the real type
            var param = new MAVLink.MAVLinkParam("SYSID_THISMAV", BitConverter.GetBytes(255f),
                MAVLink.MAV_PARAM_TYPE.REAL32, MAVLink.MAV_PARAM_TYPE.INT16);
            Assert.AreEqual(255, param.Value);
            Assert.AreEqual(MAVLink.MAV_PARAM_TYPE.INT16, param.TypeAP);
            Assert.AreEqual(MAVLink.MAV_PARAM_TYPE.REAL32, param.Type);
            Assert.AreEqual(255, (int)param);
        }

        [TestMethod]
        public void ListFindsParamsByName()
        {
            var list = new MAVLink.MAVLinkParamList();
            for (int i = 0; i < 100; i++)
                list.Add(Param("P" + i, i));

            Assert.AreEqual(100, list.Count);
            Assert.AreEqual(42, list["P42"].Value);
            Assert.IsTrue(list.ContainsKey("P99"));
            Assert.IsFalse(list.ContainsKey("P100"));
            Assert.IsNull(list["P100"]);
            Assert.IsFalse(list.ContainsKey(null));

            // adding a name again replaces it
            list.Add(Param("P42", 4242));
            Assert.AreEqual(100, list.Count);
            Assert.AreEqual(4242, list["P42"].Value);

            // a name not found before is found once added
            Assert.IsNull(list["NEW"]);
            list["NEW"] = Param("NEW", 1);
            Assert.AreEqual(1, list["NEW"].Value);
            Assert.AreEqual(101, list.Count);

            Dictionary<string, double> copy = list;
            Assert.AreEqual(101, copy.Count);
            Assert.AreEqual(4242, copy["P42"]);
            CollectionAssert.AreEquivalent(copy.Keys.ToList(), list.Keys.ToList());
        }

        [TestMethod]
        public void ListLookupsFollowChangesMadeThroughTheBaseList()
        {
            var list = new MAVLink.MAVLinkParamList();
            list.Add(Param("A", 1));
            list.Add(Param("B", 2));
            list.Add(Param("C", 3));
            Assert.AreEqual(2, list["B"].Value);

            List<MAVLink.MAVLinkParam> plain = list;
            plain.RemoveAt(0);
            Assert.IsNull(list["A"]);
            Assert.AreEqual(2, list["B"].Value);
            Assert.AreEqual(3, list["C"].Value);

            plain.Insert(0, Param("C", 30));
            Assert.AreEqual(30, list["C"].Value, "the first param of a name is found");

            plain.Sort((x, y) => string.CompareOrdinal(y.Name, x.Name));
            Assert.AreEqual(2, list["B"].Value);

            list.Clear();
            Assert.AreEqual(0, list.Count);
            Assert.AreEqual(0, list.TotalReported);
            Assert.IsNull(list["B"]);
            Assert.IsFalse(list.ContainsKey("C"));
        }

        [TestMethod]
        public void ListLookupsFollowRenamedParams()
        {
            var list = new MAVLink.MAVLinkParamList();
            var param = Param("OLD_NAME", 5);
            list.Add(param);
            list.Add(Param("OTHER", 6));
            Assert.AreSame(param, list["OLD_NAME"]);
            Assert.IsNull(list["NEW_NAME"]);

            param.Name = "NEW_NAME";

            Assert.IsNull(list["OLD_NAME"]);
            Assert.AreSame(param, list["NEW_NAME"]);
            Assert.AreEqual(6, list["OTHER"].Value);
        }

        [TestMethod]
        public void ListAddRangeAndNameArrays()
        {
            var list = new MAVLink.MAVLinkParamList();
            list.AddRange(new[] { Param("BATT_MONITOR", 4), Param("COMPASS_USE", 1) });
            Assert.AreEqual(4, list[new[] { "BATT_MONITOR", "BATT1_MONITOR" }].Value);
            Assert.IsNull(list[new[] { "NONE1", "NONE2" }]);

            // both of the names: ambiguous
            list.Add(Param("BATT1_MONITOR", 3));
            Assert.IsNull(list[new[] { "BATT_MONITOR", "BATT1_MONITOR" }]);
        }

        [TestMethod]
        public void ConcurrentReadsAndWritesKeepTheList()
        {
            var list = new MAVLink.MAVLinkParamList();
            var errors = new List<Exception>();
            var writers = Enumerable.Range(0, 4).Select(w => System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < 250; i++)
                        list.Add(Param("W" + w + "_" + i, i));
                }
                catch (Exception e) { lock (errors) errors.Add(e); }
            })).ToArray();
            var reader = System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    for (int i = 0; i < 2000; i++)
                    {
                        var found = list["W0_" + (i % 250)];
                        if (found != null)
                            Assert.AreEqual((double)(i % 250), found.Value);
                    }
                }
                catch (Exception e) { lock (errors) errors.Add(e); }
            });

            System.Threading.Tasks.Task.WaitAll(writers.Concat(new[] { reader }).ToArray());
            Assert.AreEqual(0, errors.Count, string.Join("\n", errors));
            Assert.AreEqual(1000, list.Count);
            for (int w = 0; w < 4; w++)
                Assert.AreEqual(249, list["W" + w + "_249"].Value);
        }
    }
}
