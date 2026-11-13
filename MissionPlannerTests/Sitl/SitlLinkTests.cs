using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Utilities;

namespace MissionPlannerTests.Sitl
{
    /// <summary>
    /// MissionPlanner's MAVLink operations against ArduCopter SITL. See README.md to run them.
    /// </summary>
    [TestClass]
    [TestCategory("SITL")]
    public class SitlLinkTests
    {
        static SitlVehicle sitl;

        [ClassInitialize]
        public static void Start(TestContext context)
        {
            if (SitlVehicle.IsConfigured)
                sitl = SitlVehicle.Start();
        }

        [ClassCleanup]
        public static void Stop()
        {
            sitl?.Dispose();
            sitl = null;
        }

        [TestInitialize]
        public void RequireSitl()
        {
            if (sitl == null)
                Assert.Inconclusive(SitlVehicle.NotConfigured);
            Assert.IsTrue(sitl.Mav.BaseStream.IsOpen, "the SITL link closed: " + sitl.ReadOutput());
        }

        const uint Sys = SitlVehicle.SysId;
        const byte Comp = SitlVehicle.CompId;

        static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        [TestMethod]
        public void IdentifiesTheVehicle()
        {
            Assert.AreEqual(Sys, sitl.Mav.sysidcurrent);
            Assert.AreEqual(Comp, (byte)sitl.Mav.compidcurrent);
            Assert.AreEqual(MAVLink.MAV_TYPE.QUADROTOR, sitl.State.aptype);
            Assert.AreEqual(MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA, sitl.State.apname);
            Assert.AreEqual(Firmwares.ArduCopter2, sitl.State.cs.firmware);

            Assert.IsTrue(sitl.Mav.getVersion(Sys, Comp));
            var version = sitl.State.getPacket((uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION)
                .ToStructure<MAVLink.mavlink_autopilot_version_t>();
            Assert.IsTrue((version.capabilities & (ulong)MAVLink.MAV_PROTOCOL_CAPABILITY.MAVLINK2) != 0);
            Assert.IsTrue((version.capabilities & (ulong)MAVLink.MAV_PROTOCOL_CAPABILITY.FTP) != 0);
            Assert.IsTrue(version.flight_sw_version >> 24 >= 4, "ArduPilot 4 or later");

            // telemetry asked for at connect arrives, and the link is MAVLink 2
            var attitude = sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.ATTITUDE, m => true, Timeout);
            Assert.IsTrue(attitude.ismavlink2);
            Assert.IsTrue(sitl.Mav.Mavlink2Count > 0);
        }

        [TestMethod]
        public void DownloadsTheParameterListTwoWays()
        {
            var list = sitl.Mav.getParamList(Sys, Comp);
            Assert.IsTrue(list.Count > 500, "only " + list.Count + " params");
            Assert.AreEqual(list.TotalReported, list.Count, "every param reported was received");
            Assert.AreEqual(list.Count, list.Keys.Distinct().Count(), "no duplicate names");
            Assert.AreEqual(1, list["SYSID_THISMAV"].Value);
            Assert.AreEqual(MAVLink.MAV_PARAM_TYPE.INT16, list["SYSID_THISMAV"].TypeAP);
            Assert.AreEqual(MAVLink.MAV_PARAM_TYPE.REAL32, list["WPNAV_SPEED"].TypeAP);
            Assert.AreEqual(list.Count, sitl.State.param.Count);

            var sent = new List<uint>();
            EventHandler<MAVLink.MAVLinkMessage> handler = (s, m) => sent.Add(m.msgid);
            sitl.Mav.OnPacketSent += handler;
            MAVLink.MAVLinkParamList ftp;
            try
            {
                ftp = sitl.Mav.getParamListMavftp(Sys, Comp);
            }
            finally
            {
                sitl.Mav.OnPacketSent -= handler;
            }

            Assert.IsNotNull(ftp, "MAVFTP param download");
            Assert.IsFalse(sent.Contains((uint)MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST), "MAVFTP did not fall back to PARAM_REQUEST_LIST");
            CollectionAssert.AreEquivalent(list.Keys.ToList(), ftp.Keys.ToList());
            foreach (var param in list)
            {
                // counters and learnt values move while the vehicle runs
                if (param.Name.StartsWith("STAT_") || param.Name.Contains("_OFS") || param.Name.Contains("GYROFFS"))
                    continue;
                Assert.AreEqual(param.Value, ftp[param.Name].Value, Math.Abs(param.Value) * 1e-6, param.Name);
                Assert.AreEqual(param.TypeAP, ftp[param.Name].TypeAP, param.Name);
            }
        }

        [TestMethod]
        public void SetsParametersOfEachType()
        {
            var cases = new (string name, double value)[]
            {
                ("FENCE_ENABLE", 1), // INT8
                ("ANGLE_MAX", 3456), // INT16
                ("RTL_ALT", 2345), // INT32
                ("WPNAV_SPEED", 987.5), // REAL32
                ("PSC_POSXY_P", 0.123), // REAL32 needing single precision rounding
            };

            var old = cases.ToDictionary(a => a.name, a => (double)sitl.Mav.GetParam(Sys, Comp, a.name));
            try
            {
                foreach (var item in cases)
                {
                    Assert.AreNotEqual(item.value, old[item.name], item.name + " already has the test value");
                    Assert.IsTrue(sitl.Mav.setParam(Sys, Comp, item.name, item.value), "set " + item.name);
                    Assert.AreEqual(item.value, sitl.State.param[item.name].Value, 1e-9, item.name + " in the param list");
                    Assert.AreEqual(item.value, sitl.Mav.GetParam(Sys, Comp, item.name), 1e-6, item.name + " read back");
                }

                // a name the vehicle does not have
                Assert.IsFalse(sitl.Mav.setParam(Sys, Comp, "NO_SUCH_PARAM", 1));
            }
            finally
            {
                foreach (var item in old)
                    sitl.Mav.setParam(Sys, Comp, item.Key, item.Value);
            }

            foreach (var item in old)
                Assert.AreEqual(item.Value, sitl.Mav.GetParam(Sys, Comp, item.Key), 1e-6, item.Key + " restored");
        }

        static Locationwp Item(MAVLink.MAV_CMD command, double lat, double lng, double alt,
            MAVLink.MAV_FRAME frame = MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT, float p1 = 0, float p2 = 0, float p3 = 0, float p4 = 0)
        {
            return new Locationwp
            {
                id = (ushort)command, lat = lat, lng = lng, alt = (float)alt, frame = (byte)frame, p1 = p1, p2 = p2, p3 = p3, p4 = p4
            };
        }

        static void AssertSameItems(List<Locationwp> expected, List<Locationwp> actual, int from)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = from; i < expected.Count; i++)
            {
                string what = "item " + i;
                Assert.AreEqual(expected[i].id, actual[i].id, what);
                // the frame of a command without a location is not kept
                if (Locationwp.isLocationCommand(expected[i].id))
                    Assert.AreEqual(expected[i].frame, actual[i].frame, what);
                // MISSION_ITEM_INT carries 1e-7 degrees
                Assert.AreEqual(expected[i].lat, actual[i].lat, 1e-7, what);
                Assert.AreEqual(expected[i].lng, actual[i].lng, 1e-7, what);
                Assert.AreEqual(expected[i].alt, actual[i].alt, 0.01, what);
                Assert.AreEqual(expected[i].p1, actual[i].p1, 1e-4, what);
                Assert.AreEqual(expected[i].p2, actual[i].p2, 1e-4, what);
                Assert.AreEqual(expected[i].p3, actual[i].p3, 1e-4, what);
            }
        }

        [TestMethod]
        public void MissionUploadsAndDownloads()
        {
            const double lat = -35.363261, lng = 149.165230;
            var mission = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.WAYPOINT, lat, lng, 584, MAVLink.MAV_FRAME.GLOBAL), // home
                Item(MAVLink.MAV_CMD.TAKEOFF, 0, 0, 20),
                Item(MAVLink.MAV_CMD.WAYPOINT, lat + 0.0012345, lng - 0.0006789, 30, p1: 2),
                // ArduPilot gives the loiter direction in param3, 1 for clockwise
                Item(MAVLink.MAV_CMD.LOITER_TIME, lat + 0.002, lng + 0.001, 25.5, p1: 10, p3: 1),
                Item(MAVLink.MAV_CMD.DO_CHANGE_SPEED, 0, 0, 0, MAVLink.MAV_FRAME.MISSION, p1: 1, p2: 7.5f, p3: -1),
                Item(MAVLink.MAV_CMD.WAYPOINT, lat - 0.0000001, lng + 0.0000001, 600, MAVLink.MAV_FRAME.GLOBAL),
                Item(MAVLink.MAV_CMD.SPLINE_WAYPOINT, lat - 0.001, lng - 0.001, 40),
                Item(MAVLink.MAV_CMD.RETURN_TO_LAUNCH, 0, 0, 0),
            };

            mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.MISSION, mission).GetAwaiter().GetResult();
            Assert.AreEqual(mission.Count, sitl.Mav.getWPCount(Sys, Comp, MAVLink.MAV_MISSION_TYPE.MISSION));

            var back = mav_mission.download(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.MISSION).GetAwaiter().GetResult();
            // the vehicle keeps its own home as item 0
            AssertSameItems(mission, back, 1);

            // a single item
            var item = sitl.Mav.getWP(Sys, Comp, 3);
            Assert.AreEqual((ushort)MAVLink.MAV_CMD.LOITER_TIME, item.id);
            Assert.AreEqual(10f, item.p1);

            // and a shorter mission replaces it
            mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.MISSION, mission.Take(3).ToList()).GetAwaiter().GetResult();
            Assert.AreEqual(3, sitl.Mav.getWPCount(Sys, Comp, MAVLink.MAV_MISSION_TYPE.MISSION));
        }

        [TestMethod]
        public void FenceAndRallyUploadAndDownload()
        {
            const double lat = -35.363261, lng = 149.165230;
            var fence = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, lat + 0.001, lng + 0.001, 0, MAVLink.MAV_FRAME.GLOBAL, p1: 4),
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, lat + 0.001, lng - 0.001, 0, MAVLink.MAV_FRAME.GLOBAL, p1: 4),
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, lat - 0.001, lng - 0.001, 0, MAVLink.MAV_FRAME.GLOBAL, p1: 4),
                Item(MAVLink.MAV_CMD.FENCE_POLYGON_VERTEX_INCLUSION, lat - 0.001, lng + 0.001, 0, MAVLink.MAV_FRAME.GLOBAL, p1: 4),
                Item(MAVLink.MAV_CMD.FENCE_CIRCLE_EXCLUSION, lat + 0.0002, lng, 0, MAVLink.MAV_FRAME.GLOBAL, p1: 15),
            };
            var rally = new List<Locationwp>
            {
                Item(MAVLink.MAV_CMD.RALLY_POINT, lat + 0.0005, lng, 30),
                Item(MAVLink.MAV_CMD.RALLY_POINT, lat, lng + 0.0005, 45),
            };

            try
            {
                mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.FENCE, fence).GetAwaiter().GetResult();
                mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.RALLY, rally).GetAwaiter().GetResult();

                AssertSameItems(fence, mav_mission.download(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.FENCE).GetAwaiter().GetResult(), 0);
                AssertSameItems(rally, mav_mission.download(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.RALLY).GetAwaiter().GetResult(), 0);
            }
            finally
            {
                mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.FENCE, new List<Locationwp>()).GetAwaiter().GetResult();
                mav_mission.upload(sitl.Mav, Sys, Comp, MAVLink.MAV_MISSION_TYPE.RALLY, new List<Locationwp>()).GetAwaiter().GetResult();
            }

            Assert.AreEqual(0, sitl.Mav.getWPCount(Sys, Comp, MAVLink.MAV_MISSION_TYPE.FENCE));
            Assert.AreEqual(0, sitl.Mav.getWPCount(Sys, Comp, MAVLink.MAV_MISSION_TYPE.RALLY));
        }

        static bool Armed(MAVLink.MAVLinkMessage heartbeat)
        {
            return (heartbeat.ToStructure<MAVLink.mavlink_heartbeat_t>().base_mode & (byte)MAVLink.MAV_MODE_FLAG.SAFETY_ARMED) != 0;
        }

        [TestMethod]
        public void ChangesModeAndArms()
        {
            sitl.Mav.setMode(Sys, Comp, "GUIDED");
            sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, m => m.ToStructure<MAVLink.mavlink_heartbeat_t>().custom_mode == 4, Timeout);

            // GUIDED wants a position to arm, STABILIZE does not
            sitl.Mav.setMode(Sys, Comp, "STABILIZE");
            sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, m => m.ToStructure<MAVLink.mavlink_heartbeat_t>().custom_mode == 0, Timeout);

            try
            {
                // forced: the EKF need not be ready
                Assert.IsTrue(sitl.Mav.doARM(Sys, Comp, true, true), "arm");
                sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, Armed, Timeout);
            }
            finally
            {
                Assert.IsTrue(sitl.Mav.doARM(Sys, Comp, false, true), "disarm");
            }
            sitl.WaitFor(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, m => !Armed(m), Timeout);

            // an unsupported command is refused, not timed out
            Assert.IsFalse(sitl.Mav.doCommand(Sys, Comp, MAVLink.MAV_CMD.DO_SET_ROI_NONE + 30000, 0, 0, 0, 0, 0, 0, 0));
        }

        [TestMethod]
        public void TransfersFilesOverMavftp()
        {
            var ftp = new MAVFtp(sitl.Mav, Sys, Comp);
            var content = new byte[5000];
            new Random(7).NextBytes(content);
            string name = "mptest" + Guid.NewGuid().ToString("N").Substring(0, 6) + ".bin";

            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
            {
                ftp.UploadFile(name, new MemoryStream(content), cancel);

                // the vehicle's file system is the SITL's directory
                CollectionAssert.AreEqual(content, File.ReadAllBytes(Path.Combine(sitl.WorkDir, name)));

                Assert.IsTrue(ftp.kCmdListDirectory(".", cancel).Any(a => a.Name == name && a.Size == (ulong)content.Length),
                    "listed in its directory");

                using (var back = ftp.GetFile(name, cancel))
                    CollectionAssert.AreEqual(content, back.ToArray());

                uint crc = 0;
                Assert.IsTrue(ftp.kCmdCalcFileCRC32(name, ref crc, cancel));
                Assert.AreEqual(MAVFtp.crc_crc32(0, content), crc);

                Assert.IsTrue(ftp.kCmdRemoveFile(name, cancel));
                Assert.IsFalse(File.Exists(Path.Combine(sitl.WorkDir, name)));
            }
        }

        [TestMethod]
        public void RecordsATlogThatReplays()
        {
            string tlog = Path.Combine(sitl.WorkDir, "session.tlog");
            int heartbeats = 0;
            EventHandler<MAVLink.MAVLinkMessage> count = (s, m) =>
            {
                if (m.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT && m.sysid == Sys)
                    Interlocked.Increment(ref heartbeats);
            };

            using (var file = File.Create(tlog))
            {
                var stream = new BufferedStream(file);
                sitl.Mav.logfile = stream;
                sitl.Mav.OnPacketReceived += count;
                // something sent is recorded as well
                sitl.Mav.GetParam(Sys, Comp, "SYSID_THISMAV");
                Thread.Sleep(4000);
                sitl.Mav.OnPacketReceived -= count;
                sitl.Mav.logfile = null;
                lock (stream)
                    stream.Flush();
            }

            Assert.IsTrue(heartbeats >= 3, heartbeats + " heartbeats");

            using (var port = new MAVLinkInterface(File.OpenRead(tlog)))
            {
                int replayed = 0, fromGcs = 0, total = 0;
                DateTime last = DateTime.MinValue;
                while (port.logplaybackfile.BaseStream.Position < port.logplaybackfile.BaseStream.Length)
                {
                    var message = port.readPacket();
                    if (message == MAVLink.MAVLinkMessage.Invalid)
                        continue;
                    total++;
                    if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT && message.sysid == Sys)
                        replayed++;
                    if (message.sysid == MAVLinkInterface.gcssysid)
                        fromGcs++;
                    Assert.IsTrue(port.lastlogread >= last, "tlog times only go forward");
                    last = port.lastlogread;
                }

                Assert.IsTrue(total > heartbeats * 5, "telemetry was recorded");
                Assert.AreEqual(heartbeats, replayed, 1, "heartbeats replayed");
                Assert.IsTrue(fromGcs > 0, "sent packets were recorded");
                Assert.AreEqual(MAVLink.MAV_TYPE.QUADROTOR, port.MAVlist[Sys, Comp].aptype);
                Assert.IsNotNull(port.MAVlist[Sys, Comp].getPacket((uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE));
            }
        }
    }
}
