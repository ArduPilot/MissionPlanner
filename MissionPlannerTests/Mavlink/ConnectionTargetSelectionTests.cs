using System;
using System.Collections.Concurrent;
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
        private static MAVLink.MAVLinkMessage Heartbeat(uint system, byte component = 1, bool autopilot = true,
            MAVLink.MAV_TYPE type = MAVLink.MAV_TYPE.HEXAROTOR, byte sequence = 0, bool highLatency = false)
        {
            byte stack = (byte)(autopilot ? MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA : MAVLink.MAV_AUTOPILOT.INVALID);
            object body = highLatency
                ? (object)new MAVLink.mavlink_high_latency2_t { type = (byte)type, autopilot = stack }
                : new MAVLink.mavlink_heartbeat_t { type = (byte)type, autopilot = stack, mavlink_version = 3 };
            return new MAVLink.MAVLinkMessage { buffer = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                highLatency ? MAVLink.MAVLINK_MSG_ID.HIGH_LATENCY2 : MAVLink.MAVLINK_MSG_ID.HEARTBEAT,
                body, false, system, component, sequence) };
        }

        private static void Observe(ConnectionTargetSelector selector, MAVLink.MAVLinkMessage message, int count, int milliseconds = 0)
        {
            for (int i = 0; i < count; i++) selector.Observe(message, TimeSpan.FromMilliseconds(milliseconds));
        }

        [TestMethod]
        public void FastCompanionCannotWinBeforeTheFlightControllerIsDiscovered()
        {
            var selector = new ConnectionTargetSelector();
            Observe(selector, Heartbeat(11, autopilot: false, type: MAVLink.MAV_TYPE.ONBOARD_CONTROLLER), 20);
            Assert.IsNull(selector.Select(TimeSpan.FromMilliseconds(20)));
            Observe(selector, Heartbeat(1), 1, 22);
            Assert.IsNull(selector.Select(TimeSpan.FromMilliseconds(22)), "Still require two FCU heartbeats.");
            Observe(selector, Heartbeat(1), 1, 1000);
            var selected = selector.Select(TimeSpan.FromSeconds(1));
            Assert.AreEqual(1u, selected.Message.sysid);
            Assert.IsTrue(selected.IsAutopilot);
        }

        [TestMethod]
        public void AutopilotRoleWinsRegardlessOfSystemOrComponentNumber()
        {
            foreach (uint system in new uint[] { 11, 256, uint.MaxValue })
            {
                var selector = new ConnectionTargetSelector();
                Observe(selector, Heartbeat(1, autopilot: false, type: MAVLink.MAV_TYPE.ONBOARD_CONTROLLER), 50);
                Observe(selector, Heartbeat(system, component: 42), 2);
                var selected = selector.Select(TimeSpan.Zero);
                Assert.AreEqual(system, selected.Message.sysid);
                Assert.AreEqual((byte)42, selected.Message.compid);
            }
        }

        [TestMethod]
        public void AutopilotOnlyConnectionsDoNotWaitForTheFallbackDelay()
        {
            var selector = new ConnectionTargetSelector();
            Observe(selector, Heartbeat(7), 2);
            Assert.AreEqual(7u, selector.Select(TimeSpan.Zero).Message.sysid);
        }

        [TestMethod]
        public void CompanionOnlyConnectionHasABoundedFallback()
        {
            var selector = new ConnectionTargetSelector();
            Observe(selector, Heartbeat(11, autopilot: false, type: MAVLink.MAV_TYPE.ONBOARD_CONTROLLER), 2);
            Assert.IsNull(selector.Select(TimeSpan.FromMilliseconds(2999)));
            Assert.AreEqual(11u, selector.Select(TimeSpan.FromSeconds(3)).Message.sysid);
        }

        [TestMethod]
        public void OtherPeripheralComponentsStillRequireFourConfirmations()
        {
            var selector = new ConnectionTargetSelector();
            var camera = Heartbeat(11, 100, false, MAVLink.MAV_TYPE.CAMERA);
            Observe(selector, camera, 3);
            Assert.IsNull(selector.Select(TimeSpan.FromSeconds(5)));
            Observe(selector, camera, 1);
            Assert.AreEqual((byte)100, selector.Select(TimeSpan.FromSeconds(5)).Message.compid);
        }

        [TestMethod]
        public void ConnectionDeadlineAllowsFallbackButNotAnUnconfirmedDevice()
        {
            var selector = new ConnectionTargetSelector();
            var companion = Heartbeat(11, autopilot: false);
            Observe(selector, companion, 1);
            Assert.IsNull(selector.Select(TimeSpan.FromSeconds(1), allowFallback: true));
            Observe(selector, companion, 1);
            Assert.AreEqual(11u, selector.Select(TimeSpan.FromSeconds(1), allowFallback: true).Message.sysid);
        }

        [TestMethod]
        public void GroundStationsAndBroadcastAddressesAreNotConnectionTargets()
        {
            var selector = new ConnectionTargetSelector();
            Observe(selector, Heartbeat(255, type: MAVLink.MAV_TYPE.GCS), 5);
            Observe(selector, Heartbeat(0), 5);
            Observe(selector, Heartbeat(1, component: 0), 5);
            Assert.IsFalse(selector.HasCandidates);
            Assert.IsNull(selector.Select(TimeSpan.FromSeconds(10), allowFallback: true));
        }

        [TestMethod]
        public void HighLatencyFlightControllersHaveTheSameRolePreference()
        {
            var selector = new ConnectionTargetSelector();
            Observe(selector, Heartbeat(11, autopilot: false, highLatency: true), 10);
            Observe(selector, Heartbeat(7, highLatency: true), 2);
            Assert.AreEqual(7u, selector.Select(TimeSpan.Zero).Message.sysid);
        }

        [TestMethod]
        public void OpenIncludesHeartbeatsConsumedByAnotherReaderAndPreservesManualSelection()
        {
            using (var link = new MAVLinkInterface())
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.CONNECT_TIMEOUT_SECONDS = 5;
                int heartbeatRequests = 0, backgroundReads = 0;
                byte companionSequence = 2;
                var versionTargets = new ConcurrentBag<uint>();
                var backgroundTasks = new ConcurrentBag<Task>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var sent = new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() };
                    if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                    {
                        int request = Interlocked.Increment(ref heartbeatRequests);
                        if (request == 1)
                        {
                            foreach (var message in new[] {
                                Heartbeat(11, autopilot: false, sequence: 0), Heartbeat(11, autopilot: false, sequence: 1),
                                Heartbeat(1, sequence: 0), Heartbeat(1, sequence: 1) }) wire.AppendBuffer(message.buffer);
                        }
                        else if (request == 2)
                        {
                            // Model the normal telemetry reader taking the second
                            // companion HB and the FCU's first HB between calls.
                            var otherReader = Task.Run(() =>
                            {
                                link.readPacketAsync().GetAwaiter().GetResult();
                                link.readPacketAsync().GetAwaiter().GetResult();
                                Interlocked.Add(ref backgroundReads, 2);
                            });
                            backgroundTasks.Add(otherReader);
                            otherReader.Wait(1000);
                        }
                        else wire.AppendBuffer(Heartbeat(11, autopilot: false, sequence: companionSequence++).buffer);
                    }
                    if (sent.msgid == (uint)MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION_REQUEST)
                    {
                        uint target = sent.GetTargetSystem().Value;
                        versionTargets.Add(target);
                        backgroundTasks.Add(Task.Run(async () =>
                        {
                            await Task.Delay(20); // getVersion installs its subscription after sending.
                            var version = new MAVLink.mavlink_autopilot_version_t {
                                flight_custom_version = new byte[8], middleware_custom_version = new byte[8],
                                os_custom_version = new byte[8], uid2 = new byte[18] };
                            wire.AppendBuffer(new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                                MAVLink.MAVLINK_MSG_ID.AUTOPILOT_VERSION, version, false, target, 1, 2));
                            while (wire.BytesToRead > 0) await link.readPacketAsync();
                        }));
                    }
                };
                link.Open(false, skipconnectedcheck: true, showui: false);
                Assert.IsTrue(Task.WaitAll(backgroundTasks.ToArray(), 3000));
                Assert.AreEqual(2, backgroundReads);
                Assert.AreEqual(1u, link.sysidcurrent, "Open must select the FCU even when another reader received one confirmation.");
                Assert.IsTrue(versionTargets.Count > 0 && versionTargets.All(id => id == 1), "Connection-time version requests must target the selected FCU.");

                link.sysidcurrent = 11; // An explicit user choice after connection.
                wire.AppendBuffer(Heartbeat(1, sequence: 3).buffer);
                link.readPacketAsync().GetAwaiter().GetResult();
                Assert.AreEqual(11u, link.sysidcurrent, "Later FCU heartbeats must not steal manual selection.");
            }
        }
    }
}
