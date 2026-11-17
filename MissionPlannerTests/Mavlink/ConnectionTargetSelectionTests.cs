using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Mavlink;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class ConnectionTargetSelectionTests
    {
        private MAVLinkInterface link;
        private MAVList Targets => link.MAVlist;

        [TestInitialize]
        public void Initialize()
        {
            link = new MAVLinkInterface();
        }

        [TestCleanup]
        public void Cleanup()
        {
            link?.Dispose();
        }

        private static MAVLink.MAVLinkMessage Heartbeat(uint system, byte component = 1,
            MAVLink.MAV_AUTOPILOT autopilot = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
            MAVLink.MAV_TYPE type = MAVLink.MAV_TYPE.HEXAROTOR, byte sequence = 0, bool highLatency = false)
        {
            object body = highLatency
                ? (object)new MAVLink.mavlink_high_latency2_t { type = (byte)type, autopilot = (byte)autopilot }
                : new MAVLink.mavlink_heartbeat_t { type = (byte)type, autopilot = (byte)autopilot, mavlink_version = 3 };
            return new MAVLink.MAVLinkMessage { buffer = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                highLatency ? MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY2 : MAVLink.MAVLINK_MSG_ID.HEARTBEAT,
                body, false, system, component, sequence) };
        }

        private void Observe(MAVLink.MAVLinkMessage message, int milliseconds = 0)
        {
            Targets.ObserveHeartbeat(message, TimeSpan.FromMilliseconds(milliseconds));
        }

        private MAVLink.MAVLinkMessage Select(int milliseconds = 0, bool deadlineReached = false)
        {
            return Targets.SelectConnectionTarget(TimeSpan.FromMilliseconds(milliseconds), deadlineReached);
        }

        [TestMethod]
        public void AutopilotClassesHaveTheExpectedPriority()
        {
            foreach (var autopilot in new[] { MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                MAVLink.MAV_AUTOPILOT.PX4, MAVLink.MAV_AUTOPILOT.SLUGS })
                Assert.AreEqual(ConnectionTargetSelector.Priority.SpecificAutopilot,
                    ConnectionTargetSelector.GetPriority(autopilot), autopilot.ToString());

            foreach (var autopilot in new[] { MAVLink.MAV_AUTOPILOT.GENERIC,
                MAVLink.MAV_AUTOPILOT.GENERIC_WAYPOINTS_ONLY,
                MAVLink.MAV_AUTOPILOT.GENERIC_WAYPOINTS_AND_SIMPLE_NAVIGATION_ONLY,
                MAVLink.MAV_AUTOPILOT.GENERIC_MISSION_FULL })
                Assert.AreEqual(ConnectionTargetSelector.Priority.GenericAutopilot,
                    ConnectionTargetSelector.GetPriority(autopilot), autopilot.ToString());

            foreach (var autopilot in new[] { MAVLink.MAV_AUTOPILOT.INVALID,
                MAVLink.MAV_AUTOPILOT.RESERVED, (MAVLink.MAV_AUTOPILOT)255 })
                Assert.AreEqual(ConnectionTargetSelector.Priority.Other,
                    ConnectionTargetSelector.GetPriority(autopilot), autopilot.ToString());
        }

        [TestMethod]
        public void RankingUsesAutopilotThenComponentThenFirstArrival()
        {
            Observe(Heartbeat(1, component: 100, autopilot: MAVLink.MAV_AUTOPILOT.INVALID));
            Observe(Heartbeat(2, component: 1, autopilot: MAVLink.MAV_AUTOPILOT.INVALID));
            Observe(Heartbeat(3, component: 100, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC));
            Observe(Heartbeat(4, component: 2, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC));
            Observe(Heartbeat(uint.MaxValue, component: 42));
            Observe(Heartbeat(9, component: 1));
            Observe(Heartbeat(6, component: 1, autopilot: MAVLink.MAV_AUTOPILOT.PX4));

            // More packets and a smaller system ID must not displace an earlier peer.
            for (int i = 0; i < 10; i++)
                Observe(Heartbeat(6, component: 1, autopilot: MAVLink.MAV_AUTOPILOT.PX4));
            Observe(Heartbeat(9, component: 1));

            CollectionAssert.AreEqual(new uint[] { 9, 6, uint.MaxValue, 4, 3, 2, 1 },
                Targets.GetRankedStates().Select(m => m.sysid).ToArray());
        }

        [TestMethod]
        public void SpecificAutopilotOnComponentOneIsSelectedAfterOneMessage()
        {
            foreach (var highLatency in new[] { false, true })
            {
                Targets.Clear();
                var heartbeat = Heartbeat(7, highLatency: highLatency);
                Observe(heartbeat, 5000);
                Assert.AreSame(heartbeat, Select(5000), "A single FCU message must select immediately.");
            }
        }

        [TestMethod]
        public void FallbackWaitsThreeSecondsFromFirstCandidateUnlessDeadlineReached()
        {
            Assert.IsNull(Select(10000, deadlineReached: true), "No traffic means no target or discovery window.");
            Observe(Heartbeat(11, component: 100, autopilot: MAVLink.MAV_AUTOPILOT.INVALID), 15000);
            Assert.IsNull(Select(17900), "A peripheral must wait for discovery.");

            var better = Heartbeat(12, component: 42);
            Observe(better, 17900);
            Assert.IsNull(Select(17999), "A nonstandard FCU component must also wait for discovery.");
            Assert.AreSame(better, Select(17999, deadlineReached: true),
                "A shorter connection deadline must use the best available single message.");
            Assert.AreSame(better, Select(18000),
                "Discovery lasts exactly three seconds from the first candidate; later arrivals must not extend it.");
        }

        [TestMethod]
        public void ExcludedDevicesDoNotQualifyOrStartDiscovery()
        {
            Observe(Heartbeat(255, type: MAVLink.MAV_TYPE.GCS));
            Observe(Heartbeat(0));
            Observe(Heartbeat(1, component: 0));
            Targets.Create(2, 100);
            Targets[2, 100].CANNode = true;
            CollectionAssert.AreEqual(new uint[] { 2 }, Targets.GetRankedStates().Select(m => m.sysid).ToArray(),
                "The heartbeat-free CAN node must remain in the dropdown; GCS and broadcast entries must not.");
            Assert.IsNull(Select(10000, deadlineReached: true));

            var heartbeat = Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC);
            Observe(heartbeat, 15000);
            Assert.IsNull(Select(15000), "Excluded devices must not start the clock or give generic FCUs an immediate path.");
            Assert.AreSame(heartbeat, Select(15000, deadlineReached: true));
        }

        [TestMethod]
        public void HeartbeatAndHighLatencyUpdatesSharePriorityAndPreserveFirstArrival()
        {
            Observe(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC, highLatency: true));
            Observe(Heartbeat(1, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC));
            Observe(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC));
            CollectionAssert.AreEqual(new uint[] { 11, 1 }, Targets.GetRankedStates().Select(m => m.sysid).ToArray(),
                "Changing message format must not duplicate a device or reset its arrival order.");

            Observe(Heartbeat(1, highLatency: true));
            CollectionAssert.AreEqual(new uint[] { 1, 11 }, Targets.GetRankedStates().Select(m => m.sysid).ToArray());
            Observe(Heartbeat(1, autopilot: MAVLink.MAV_AUTOPILOT.INVALID));
            CollectionAssert.AreEqual(new uint[] { 11, 1 }, Targets.GetRankedStates().Select(m => m.sysid).ToArray(),
                "The latest heartbeat must update priority in either direction, regardless of format.");
        }

        [TestMethod]
        public void ReconnectClearsCandidatesAndRestartsDiscoveryAndArrivalOrder()
        {
            var old = Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC);
            Observe(old);
            Assert.AreSame(old, Select(10000, deadlineReached: true));
            Targets.Clear();
            Assert.AreEqual(0, Targets.GetRankedStates().Length);
            Assert.IsNull(Select(10000, deadlineReached: true));

            var first = Heartbeat(1, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC);
            Observe(first, 10000);
            Observe(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC), 11000);
            Assert.IsNull(Select(11000), "The old connection's discovery window must not be reused.");
            Assert.AreSame(first, Select(11000, deadlineReached: true),
                "Arrival order must be determined afresh on reconnect.");
        }

        private static Task ReplyWithVersion(MAVLinkInterface connection, CommsInjection wire, uint target, byte component = 1)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(20); // getVersion installs its subscription after sending.
                var version = new MAVLink.mavlink_autopilot_version_t {
                    flight_custom_version = new byte[8], middleware_custom_version = new byte[8],
                    os_custom_version = new byte[8], uid2 = new byte[18] };
                wire.AppendBuffer(new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                    MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION, version, false, target, component, 2));
                while (wire.BytesToRead > 0) await connection.readPacketAsync();
            });
        }

        [TestMethod]
        public void OpenUsesDiscoveryFromAnotherReaderAndPreservesManualSelection()
        {
            var wire = new CommsInjection();
            link.BaseStream = wire;
            link.CONNECT_TIMEOUT_SECONDS = 5;
            int heartbeatRequests = 0, backgroundReads = 0, startedOtherReader = 0;
            var versionTargets = new ConcurrentBag<uint>();
            var backgroundTasks = new ConcurrentBag<Task>();
            link.OnPacketReceived += (sender, message) =>
            {
                if (message.msgid != (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT || message.sysid != 11 ||
                    Interlocked.Exchange(ref startedOtherReader, 1) != 0) return;
                var otherReader = Task.Run(() =>
                {
                    for (int i = 0; i < 10; i++)
                        link.readPacketAsync().GetAwaiter().GetResult();
                    Interlocked.Add(ref backgroundReads, 10);
                });
                backgroundTasks.Add(otherReader);
                Assert.IsTrue(otherReader.Wait(1000));
            };
            wire.WriteCallback += (sender, bytes) =>
            {
                var sent = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                {
                    if (Interlocked.Increment(ref heartbeatRequests) == 1)
                    {
                        // Ten companion messages must not outweigh one FCU message,
                        // including when the normal reader consumes the FCU packet.
                        for (byte sequence = 0; sequence < 10; sequence++)
                            wire.AppendBuffer(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC,
                                sequence: sequence).buffer);
                        wire.AppendBuffer(Heartbeat(1, sequence: 0).buffer);
                    }
                    else wire.AppendBuffer(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC, sequence: 10).buffer);
                }
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST)
                {
                    uint target = sent.GetTargetSystem().Value;
                    versionTargets.Add(target);
                    backgroundTasks.Add(ReplyWithVersion(link, wire, target));
                }
            };
            link.Open(false, skipconnectedcheck: true, showui: false);
            Assert.IsTrue(Task.WaitAll(backgroundTasks.ToArray(), 3000));
            Assert.AreEqual(10, backgroundReads);
            Assert.AreEqual(1u, link.sysidcurrent);
            Assert.IsTrue(versionTargets.Count > 0 && versionTargets.All(id => id == 1));

            link.sysidcurrent = 11; // An explicit user choice after connection.
            wire.AppendBuffer(Heartbeat(7, autopilot: MAVLink.MAV_AUTOPILOT.PX4).buffer);
            link.readPacketAsync().GetAwaiter().GetResult();
            Assert.AreEqual(11u, link.sysidcurrent, "Later discovery must not steal manual selection.");
            CollectionAssert.AreEqual(new uint[] { 1, 7, 11 }, Targets.GetRankedStates().Select(m => m.sysid).ToArray(),
                "The dropdown's ranked list must keep discovering devices after the handshake.");
        }

        [TestMethod]
        public void OpenSelectsAQuietSingleHeartbeatPeripheralWhenTheWindowExpires()
        {
            var wire = new CommsInjection();
            link.BaseStream = wire;
            link.CONNECT_TIMEOUT_SECONDS = 5;
            int heartbeatRequests = 0;
            var clock = Stopwatch.StartNew();
            TimeSpan? firstSeen = null, selectedAt = null;
            var backgroundTasks = new ConcurrentBag<Task>();
            link.OnPacketReceived += (sender, message) =>
            {
                if (message.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT && !firstSeen.HasValue)
                    firstSeen = clock.Elapsed;
            };
            wire.WriteCallback += (sender, bytes) =>
            {
                var sent = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.STATUSTEXT && !selectedAt.HasValue)
                    selectedAt = clock.Elapsed;
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT &&
                    (Interlocked.Increment(ref heartbeatRequests) == 2 || selectedAt.HasValue))
                    wire.AppendBuffer(Heartbeat(11, component: 100, autopilot: MAVLink.MAV_AUTOPILOT.INVALID).buffer);
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST)
                    backgroundTasks.Add(ReplyWithVersion(link, wire, 11, 100));
            };
            link.Open(false, skipconnectedcheck: true, showui: false);
            Assert.IsTrue(Task.WaitAll(backgroundTasks.ToArray(), 3000));
            Assert.AreEqual(11u, link.sysidcurrent);
            Assert.AreEqual(100, link.compidcurrent);
            Assert.IsTrue(firstSeen.HasValue && selectedAt.HasValue);
            double elapsed = (selectedAt.Value - firstSeen.Value).TotalSeconds;
            Assert.IsTrue(elapsed >= 2.9 && elapsed < 3.8,
                $"Selection took {elapsed:F3}s; it must not wait for another heartbeat or a blocking read timeout.");
        }

        [TestMethod]
        public void OpenDrainsQueuedTelemetryBeforeChoosingGenericCompanion()
        {
            var wire = new CommsInjection();
            link.BaseStream = wire;
            link.CONNECT_TIMEOUT_SECONDS = 5;
            int heartbeatRequests = 0;
            var backgroundTasks = new ConcurrentBag<Task>();
            wire.WriteCallback += (sender, bytes) =>
            {
                var sent = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                {
                    if (Interlocked.Increment(ref heartbeatRequests) == 1)
                    {
                        wire.AppendBuffer(Heartbeat(11, autopilot: MAVLink.MAV_AUTOPILOT.GENERIC).buffer);
                        var parser = new MAVLink.MavlinkParse();
                        // At 10 ms per packet, this backlog would outlast discovery.
                        for (int i = 0; i < 400; i++)
                            wire.AppendBuffer(parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.ATTITUDE,
                                new MAVLink.mavlink_attitude_t(), false, 11, 1, i + 1));
                        wire.AppendBuffer(Heartbeat(1).buffer);
                    }
                    else wire.AppendBuffer(Heartbeat(1, sequence: 1).buffer);
                }
                if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST)
                    backgroundTasks.Add(ReplyWithVersion(link, wire, sent.GetTargetSystem().Value));
            };
            link.Open(false, skipconnectedcheck: true, showui: false);
            Assert.IsTrue(Task.WaitAll(backgroundTasks.ToArray(), 3000));
            Assert.AreEqual(1u, link.sysidcurrent, "Queued telemetry must not delay discovery of the FCU.");
        }

        [TestMethod]
        public void OpenWithNoCandidatesHonorsTheConnectionTimeout()
        {
            var wire = new CommsInjection();
            link.BaseStream = wire;
            link.CONNECT_TIMEOUT_SECONDS = 0.1;
            var clock = Stopwatch.StartNew();
            link.Open(false, skipconnectedcheck: true, showui: false);
            Assert.IsFalse(wire.IsOpen);
            Assert.IsTrue(clock.Elapsed < TimeSpan.FromSeconds(1), "An empty read must not block the connection deadline.");
        }
    }
}
