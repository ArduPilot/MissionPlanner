using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Mavlink;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class Sysid32Tests
    {
        // Independently construct the proposal's wire format, without the production header/CRC helpers.
        static byte[] Frame(byte flags, uint source, uint target = 255,
            byte[] payload = null, uint message = 0, byte crcExtra = 50)
        {
            payload = payload ?? new byte[] { 0, 0, 0, 0, 2, 3, 81, 4, 3 };
            var bytes = new List<byte> { 253, (byte)payload.Length, flags, 0, 17 };
            if ((flags & 2) != 0) bytes.AddRange(BitConverter.GetBytes(source));
            else bytes.Add((byte)source);
            bytes.Add(1);
            bytes.Add((byte)message); bytes.Add((byte)(message >> 8)); bytes.Add((byte)(message >> 16));
            if ((flags & 4) != 0)
            {
                bytes.AddRange(BitConverter.GetBytes(target));
            }
            bytes.AddRange(payload);
            ushort crc = 0xffff;
            foreach (byte value in bytes.Skip(1).Concat(new[] { crcExtra }))
            {
                crc ^= value;
                for (int i = 0; i < 8; i++) crc = (ushort)((crc >> 1) ^ ((crc & 1) != 0 ? 0x8408 : 0));
            }
            bytes.Add((byte)crc); bytes.Add((byte)(crc >> 8));
            if ((flags & 1) != 0)
            {
                bytes.AddRange(new byte[] { 3, 1, 0, 0, 0, 0, 0 });
                using (var hash = SHA256.Create())
                    bytes.AddRange(hash.ComputeHash(Enumerable.Repeat((byte)42, 32).Concat(bytes).ToArray()).Take(6));
            }
            return bytes.ToArray();
        }

        static byte[] CommandPayload(uint target, byte component = 190)
        {
            var payload = new byte[33];
            payload[28] = 44; payload[29] = 1; // command 300
            payload[30] = target > 255 ? (byte)255 : (byte)target;
            payload[31] = component;
            payload[32] = 1; // retain the full payload after trimming
            return payload;
        }

        [TestMethod]
        public void IndependentFramesDecodeAllFlagCombinations()
        {
            for (byte flags = 0; flags < 8; flags++)
            foreach (uint id in new uint[] { 1, 255, 256, 0x7fffffff, 0x80000000, 0xffffffff })
            {
                if ((flags & 2) == 0 && id > 255) continue;
                uint target = (flags & 4) != 0 ? uint.MaxValue : 7u;
                var packet = Frame(flags, id, target, payload: CommandPayload(target), message: 76, crcExtra: 152);
                var parser = new MAVLink.MavlinkParse();
                var msg = parser.ReadPacket(new FragmentedStream(packet));
                Assert.IsNotNull(msg);
                Assert.AreEqual(id, msg.sysid);
                Assert.AreEqual((byte)1, msg.compid);
                Assert.AreEqual((byte)17, msg.seq);
                Assert.AreEqual(76u, msg.msgid);
                Assert.AreEqual((ushort)300, msg.ToStructure<MAVLink.mavlink_command_long_t>().command);
                Assert.AreEqual((uint?)target, msg.GetTargetSystem());
                Assert.AreEqual((byte?)190, msg.GetTargetComponent());
                Assert.AreEqual((flags & 1) != 0, msg.sig != null);
            }
        }

        [TestMethod]
        public void EncoderMatchesIndependentFramesAndSignatures()
        {
            var hb = new MAVLink.mavlink_heartbeat_t { type = 2, autopilot = 3, base_mode = 81, system_status = 4, mavlink_version = 3 };
            foreach (uint source in new uint[] { 1, 255, 256, 0xffffffff })
            foreach (uint? target in new uint?[] { null, 0, 7, 0x80000000, 0xffffffff })
            foreach (bool sign in new[] { false, true })
            {
                var parser = new MAVLink.MavlinkParse { signingKey = Enumerable.Repeat((byte)42, 32).ToArray() };
                var command = new MAVLink.mavlink_command_long_t { target_system = 7, target_component = 190, command = 300, confirmation = 1 };
                uint effectiveTarget = target ?? 7;
                byte[] actual = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, command, sign, source, 1, 17, target, 190);
                byte flags = (byte)((source > 255 ? 2 : 0) | (effectiveTarget > 255 ? 4 : 0) | (sign ? 1 : 0));
                byte[] expected = Frame(flags, source, effectiveTarget, payload: CommandPayload(effectiveTarget), message: 76, crcExtra: 152);
                int signedLength = sign ? actual.Length - 13 : actual.Length;
                CollectionAssert.AreEqual(expected.Take(signedLength).ToArray(), actual.Take(signedLength).ToArray());
                if (sign)
                {
                    using (var hash = SHA256.Create())
                        CollectionAssert.AreEqual(hash.ComputeHash(parser.signingKey.Concat(actual.Take(actual.Length - 6)).ToArray()).Take(6).ToArray(), actual.Skip(actual.Length - 6).ToArray());
                }
                var decoded = parser.ReadPacket(new MemoryStream(actual));
                Assert.IsNotNull(decoded);
                Assert.AreEqual((uint?)effectiveTarget, decoded.GetTargetSystem());
                Assert.AreEqual((byte)0, decoded.compat_flags);
            }
            Assert.ThrowsException<ArgumentException>(() => new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(
                MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, targetSystem: uint.MaxValue));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MAVLink.MavlinkParse().GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, 256));
        }

        [TestMethod]
        public void WideHeaderTargetKeepsPayloadComponent()
        {
            var payload = new byte[33]; // COMMAND_LONG, with conflicting legacy system target
            payload[30] = 99; payload[31] = 12;
            foreach (uint target in new uint[] { 256, 0x80000000, uint.MaxValue })
            {
                var msg = new MAVLink.MAVLinkMessage(Frame(4, 1, target, payload, 76, 152));
                Assert.AreEqual((uint?)target, msg.GetTargetSystem());
                Assert.AreEqual((byte?)12, msg.GetTargetComponent());
                Assert.IsTrue(msg.IsTargetedTo(target, 12));
                Assert.IsFalse(msg.IsTargetedTo(target, 190));
                Assert.AreEqual(target == 0, msg.IsTargetedTo(target == 1 ? 2u : 1u, 190));
                Assert.AreEqual((byte)99, msg.ToStructure<MAVLink.mavlink_command_long_t>().target_system);
            }
            var legacy = new MAVLink.MAVLinkMessage(Frame(0, 1, payload: payload, message: 76, crcExtra: 152));
            Assert.AreEqual((uint?)99, legacy.GetTargetSystem());
        }

        [TestMethod]
        public void RejectUnknownFlagsAndTruncatedOrCorruptFrames()
        {
            var parser = new MAVLink.MavlinkParse();
            foreach (byte flags in new byte[] { 128, 129, 134, 135 })
            {
                byte[] rejected = Frame(flags, 1, payload: Frame(0, 42));
                var stream = new FragmentedStream(rejected.Concat(Frame(2, uint.MaxValue)).ToArray());
                Assert.IsNull(parser.ReadPacket(stream));
                Assert.AreEqual(uint.MaxValue, parser.ReadPacket(stream).sysid);
            }
            byte[] valid = Frame(7, uint.MaxValue);
            for (int n = 0; n < valid.Length; n++)
                Assert.ThrowsException<InvalidDataException>(() => new MAVLink.MAVLinkMessage(valid.Take(n).ToArray()));
            valid[valid.Length - 14] ^= 1;
            Assert.IsNull(parser.ReadPacket(new MemoryStream(valid)));
            foreach (int length in new[] { 0, 255 })
            {
                var packet = Frame(7, uint.MaxValue, payload: new byte[length]);
                Assert.IsTrue(packet.Length <= MAVLink.MAVLINK_MAX_PACKET_LEN);
                if (length == 255) Assert.AreEqual(MAVLink.MAVLINK_MAX_PACKET_LEN, packet.Length);
                Assert.IsNotNull(parser.ReadPacket(new MemoryStream(packet)));
            }
        }

        [TestMethod]
        public void VehicleAndInspectorKeysDoNotAlias()
        {
            using (var port = new MAVLinkInterface())
            {
                var inspector = new PacketInspector<uint>();
                uint[] ids = { 1, 257, 0x80000001, 0xffffff01, uint.MaxValue };
                foreach (uint id in ids)
                {
                    port.MAVlist[id, 1].VersionString = id.ToString();
                    inspector.Add(id, 1, 0, id, 10);
                    Assert.AreEqual(id, MAVList.FromID(MAVList.GetID(id, 1)).sysid);
                }
                foreach (uint id in ids)
                {
                    Assert.AreEqual(id, port.MAVlist[id, 1].sysid);
                    Assert.AreEqual(id.ToString(), port.MAVlist[id, 1].VersionString);
                    Assert.AreEqual(id, inspector[id, 1].Single());
                }
                Assert.AreEqual(ids.Length, inspector.SeenSysid().Distinct().Count());
            }
        }

        [TestMethod]
        public void LiveLinkReadsFragmentedFramesAndTargetsWithoutAliasing()
        {
            uint oldGcs = MAVLinkInterface.gcssysid;
            try
            {
                MAVLinkInterface.gcssysid = 255;
                using (var port = new MAVLinkInterface())
                {
                    var serial = new TestSerial();
                    port.BaseStream = serial;
                    port.printbps = false;
                    int wideCalls = 0, narrowCalls = 0;
                    port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, m => { wideCalls++; return true; }, 0xffffff01, 1);
                    port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, m => { narrowCalls++; return true; }, 1, 1);
                    foreach (uint source in new uint[] { 1, 257, 0xffffff01, uint.MaxValue })
                    {
                        serial.Feed(Frame(2, source));
                        Assert.AreEqual(source, port.readPacket().sysid);
                    }
                    Assert.AreEqual(1, wideCalls); Assert.AreEqual(1, narrowCalls);
                    serial.Feed(Frame(6, 0xffffff01, 0xfffffffe, payload: CommandPayload(0xfffffffe), message: 76, crcExtra: 152));
                    Assert.AreSame(MAVLink.MAVLinkMessage.Invalid, port.readPacket());
                    Assert.AreEqual(1, wideCalls);
                    serial.Feed(Frame(2, 0xffffff01));
                    Assert.AreEqual(0xffffff01u, port.readPacket().sysid);
                    Assert.AreEqual(2, wideCalls);
                    port.sendPacket(new MAVLink.mavlink_command_long_t { command = 300, confirmation = 1 }, uint.MaxValue, 1);
                    var sent = new MAVLink.MAVLinkMessage(serial.Written.ToArray());
                    Assert.AreEqual((uint?)uint.MaxValue, sent.GetTargetSystem());
                    Assert.AreEqual(255u, sent.sysid);
                }
            }
            finally { MAVLinkInterface.gcssysid = oldGcs; }
        }

        [TestMethod]
        public void LegacyFramesAndTimestampedLogsKeepIdentity()
        {
            var parser = new MAVLink.MavlinkParse();
            var hb = new MAVLink.mavlink_heartbeat_t { type = 2, autopilot = 3, mavlink_version = 3 };
            var v1 = parser.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, 255, 1);
            Assert.AreEqual(255u, parser.ReadPacket(new FragmentedStream(v1)).sysid);
            var stamp = BitConverter.GetBytes(1700000000000000UL).Reverse().ToArray();
            byte[] frame = Frame(2, uint.MaxValue);
            byte[] log = stamp.Concat(frame).ToArray();
            var fromLog = new MAVLink.MavlinkParse(true).ReadPacket(new FragmentedStream(log));
            Assert.AreEqual(uint.MaxValue, fromLog.sysid);
            Assert.IsNull(fromLog.GetTargetSystem());
            Assert.AreEqual(2023, fromLog.rxtime.Year);
            using (var port = new MAVLinkInterface())
            {
                port.logreadmode = true;
                port.logplaybackfile = new BinaryReader(new MemoryStream(log));
                Assert.AreEqual(uint.MaxValue, port.readPacket().sysid);
                var item = new MAVLink.mavlink_mission_item_int_t { target_system = 255, target_component = 190, seq = 3, x = 123456, command = 16 };
                frame = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT, item, false, uint.MaxValue, 1, targetSystem: 255, targetComponent: 190);
                port.logplaybackfile = new BinaryReader(new MemoryStream(stamp.Concat(frame).ToArray()));
                Assert.AreEqual(uint.MaxValue, port.readPacket().sysid);
                Assert.IsTrue(port.MAVlist[uint.MaxValue, 1].wps.ContainsKey(3));
                Assert.IsFalse(port.MAVlist[255, 1].wps.ContainsKey(3));
            }
        }

        [TestMethod]
        public void LogPlaybackRecoversAfterUnknownIncompatibilityFlags()
        {
            byte[] stamp = BitConverter.GetBytes(1700000000000000UL).Reverse().ToArray();
            byte[] bad = Frame(2, 70000);
            bad[2] |= 0x80;
            byte[] good = Frame(2, uint.MaxValue);
            byte[] log = stamp.Concat(bad).Concat(stamp).Concat(bad).Concat(stamp).Concat(good).ToArray();
            using (var port = new MAVLinkInterface())
            {
                port.logreadmode = true;
                port.logplaybackfile = new BinaryReader(new MemoryStream(log));
                MAVLink.MAVLinkMessage result = MAVLink.MAVLinkMessage.Invalid;
                for (int attempt = 0; attempt < log.Length; attempt++)
                {
                    long before = port.logplaybackfile.BaseStream.Position;
                    result = port.readPacket();
                    if (result != MAVLink.MAVLinkMessage.Invalid)
                        break;
                    Assert.IsTrue(port.logplaybackfile.BaseStream.Position > before,
                        "Malformed headers must not prevent log playback from advancing");
                }
                Assert.AreEqual(uint.MaxValue, result.sysid);
                Assert.AreEqual(log.Length, port.logplaybackfile.BaseStream.Position);
            }
        }

        [TestMethod]
        public void SignedLiveFramesVerifyAndWideGcsUsesWideHeaders()
        {
            uint oldGcs = MAVLinkInterface.gcssysid;
            try
            {
                MAVLinkInterface.gcssysid = 0x80000001;
                using (var port = new MAVLinkInterface())
                {
                    var serial = new TestSerial();
                    port.BaseStream = serial;
                    byte[] key = Enumerable.Repeat((byte)42, 32).ToArray();
                    typeof(MAVState).GetField("signingKey", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(port.MAVlist[uint.MaxValue, 1], key);
                    byte[] packet = Frame(3, uint.MaxValue);
                    serial.Feed(packet);
                    Assert.AreEqual(uint.MaxValue, port.readPacket().sysid);
                    packet[packet.Length - 1] ^= 1;
                    serial.Feed(packet);
                    Assert.AreSame(MAVLink.MAVLinkMessage.Invalid, port.readPacket());
                    port.sendPacket(new MAVLink.mavlink_heartbeat_t { mavlink_version = 3 }, uint.MaxValue, 1);
                    var sent = new MAVLink.MavlinkParse().ReadPacket(new MemoryStream(serial.Written.ToArray()));
                    Assert.AreEqual(MAVLinkInterface.gcssysid, sent.sysid);
                    Assert.IsNull(sent.target_system); // regular heartbeats remain broadcast
                    Assert.IsNotNull(sent.sig);
                    using (var hash = SHA256.Create())
                        CollectionAssert.AreEqual(hash.ComputeHash(key.Concat(sent.buffer.Take(sent.Length - 6)).ToArray()).Take(6).ToArray(), sent.sig.Skip(7).ToArray());
                }
            }
            finally { MAVLinkInterface.gcssysid = oldGcs; }
        }

        [TestMethod]
        public void RcOverrideAndEquivalentTargetFieldKeepFullDestination()
        {
            foreach (uint target in new uint[] { 0, 255, 256, uint.MaxValue })
            using (var port = new MAVLinkInterface())
            {
                var serial = new TestSerial();
                port.BaseStream = serial;
                port.MAVlist[target, 1].mavlinkv2 = true;
                port.SendRCOverride(target, 1, 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500);
                var packet = new MAVLink.MavlinkParse().ReadPacket(new MemoryStream(serial.Written.ToArray()));
                Assert.AreEqual((uint?)target, packet.GetTargetSystem());
                Assert.AreEqual(target > 255, (packet.incompat_flags & MAVLink.MAVLINK_IFLAG_TARGET32) != 0);
                var manual = new MAVLink.mavlink_manual_control_t { target = 7, x = 1 };
                var bytes = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MANUAL_CONTROL,
                    manual, targetSystem: target);
                Assert.AreEqual((uint?)target, new MAVLink.MAVLinkMessage(bytes).GetTargetSystem());
            }
        }

        [TestMethod]
        public void LiveSmallDestinationPreservesPayloadBroadcast()
        {
            using (var port = new MAVLinkInterface())
            {
                var serial = new TestSerial();
                port.BaseStream = serial;
                port.MAVlist[42, 1].mavlinkv2 = true;
                port.sendPacket(new MAVLink.mavlink_command_long_t { target_system = 0, target_component = 0 }, 42, 1);
                var packet = new MAVLink.MavlinkParse().ReadPacket(new MemoryStream(serial.Written.ToArray()));
                Assert.AreEqual((uint?)0, packet.GetTargetSystem());
                Assert.AreEqual(0, packet.incompat_flags & MAVLink.MAVLINK_IFLAG_TARGET32);
            }
        }

        [TestMethod]
        public void SignedParameterStorageIsInterpretedAsUnsigned()
        {
            Assert.AreEqual(uint.MaxValue, MissionPlanner.Log.LogSort.ParseSystemId("-1"));
            Assert.AreEqual(0x80000000u, MissionPlanner.Log.LogSort.ParseSystemId("-2147483648"));
            Assert.AreEqual(0x80000000u, MissionPlanner.Log.LogSort.ParseSystemId("-2.147483648E9"));
            Assert.AreEqual(256u, MissionPlanner.Log.LogSort.ParseSystemId("256"));
            Assert.AreEqual(uint.MaxValue, MissionPlanner.Log.LogSort.ParseSystemId("4294967295"));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => MissionPlanner.Log.LogSort.ParseSystemId("4294967296"));
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => MissionPlanner.Log.LogSort.ParseSystemId("-2147483649"));
        }

        [TestMethod]
        public void AnonymiserHandlesMalformedTargetlessFrames()
        {
            string input = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tlog");
            string output = input + ".out";
            try
            {
                var stamp = BitConverter.GetBytes(1700000000000000UL).Reverse().ToArray();
                var payload = new byte[24]; // AHRS2, including latitude/longitude
                BitConverter.GetBytes(123456789).CopyTo(payload, 16);
                var frame = Frame(6, uint.MaxValue, 256, payload, 178, 47);
                File.WriteAllBytes(input, stamp.Concat(frame).ToArray());
                MissionPlanner.Utilities.Privacy.anonymise(input, output);
                using (var stream = File.OpenRead(output))
                {
                    var packet = new MAVLink.MavlinkParse(true).ReadPacket(stream);
                    Assert.AreEqual(uint.MaxValue, packet.sysid);
                    Assert.AreEqual(178u, packet.msgid);
                    Assert.IsNull(packet.target_system);
                    Assert.IsNotNull(packet.data);
                }
            }
            finally { File.Delete(input); File.Delete(output); }
        }

        [TestMethod]
        public void NonlocalTargetsAreLoggedWithoutProcessing()
        {
            using (var port = new MAVLinkInterface())
            using (var recording = new MemoryStream())
            {
                var serial = new TestSerial();
                port.BaseStream = serial;
                port.logfile = new BufferedStream(recording);
                int calls = 0;
                port.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG,
                    m => { calls++; return true; }, uint.MaxValue, 1);
                var frame = Frame(6, uint.MaxValue, 0x80000000,
                    CommandPayload(0x80000000), 76, 152);
                serial.Feed(frame);
                Assert.AreSame(MAVLink.MAVLinkMessage.Invalid, port.readPacket());
                Assert.AreEqual(0, calls);
                port.logfile.Flush();
                CollectionAssert.AreEqual(frame, recording.ToArray().Skip(8).ToArray());
            }
        }

        [TestMethod]
        public void LiveSendRejectsInvalidComponentsForEveryTargetWidth()
        {
            foreach (uint target in new uint[] { 42, 256, uint.MaxValue })
            using (var port = new MAVLinkInterface())
            {
                var serial = new TestSerial();
                port.BaseStream = serial;
                Assert.ThrowsException<OverflowException>(() =>
                    port.sendPacket(new MAVLink.mavlink_command_long_t(), target, 256));
                Assert.AreEqual(0L, serial.Written.Length);
            }
        }

        [TestMethod]
        public void StreamCombinerAllocatesPast255AndStopsAtTransportLimit()
        {
            var type = typeof(MissionPlanner.Utilities.StreamCombiner);
            var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
            var next = type.GetField("newsysid", flags);
            var allocate = type.GetMethod("AllocateSystemId", flags);
            var saved = next.GetValue(null);
            try
            {
                next.SetValue(null, 254u);
                foreach (uint expected in new uint[] { 254, 255, 256, 257 })
                    Assert.AreEqual(expected, allocate.Invoke(null, new object[] { 0xFFFFFFu }));
                next.SetValue(null, 256u);
                var legacyError = Assert.ThrowsException<System.Reflection.TargetInvocationException>(() =>
                    allocate.Invoke(null, new object[] { 255u }));
                Assert.IsInstanceOfType(legacyError.InnerException, typeof(InvalidOperationException));
                next.SetValue(null, 0xFFFFFFu);
                Assert.AreEqual(0xFFFFFFu, allocate.Invoke(null, new object[] { 0xFFFFFFu }));
                var error = Assert.ThrowsException<System.Reflection.TargetInvocationException>(() =>
                    allocate.Invoke(null, new object[] { 0xFFFFFFu }));
                Assert.IsInstanceOfType(error.InnerException, typeof(InvalidOperationException));
            }
            finally { next.SetValue(null, saved); }
        }

        sealed class FragmentedStream : MemoryStream
        {
            public FragmentedStream(byte[] data) : base(data) { }
            public override int Read(byte[] buffer, int offset, int count) { return base.Read(buffer, offset, Math.Min(1, count)); }
        }

        sealed class TestSerial : ICommsSerial
        {
            readonly Queue<byte> incoming = new Queue<byte>();
            public readonly MemoryStream Written = new MemoryStream();
            public void Feed(byte[] bytes) { foreach (byte b in bytes) incoming.Enqueue(b); }
            public Stream BaseStream { get { return Written; } }
            public int BaudRate { get; set; }
            public int BytesToRead { get { return incoming.Count; } }
            public int BytesToWrite { get { return 0; } }
            public int DataBits { get; set; }
            public bool DtrEnable { get; set; }
            public bool IsOpen { get; private set; } = true;
            public string PortName { get; set; } = "test";
            public int ReadBufferSize { get; set; }
            public int ReadTimeout { get; set; } = 100;
            public bool RtsEnable { get; set; }
            public int WriteBufferSize { get; set; }
            public int WriteTimeout { get; set; }
            public void Close() { IsOpen = false; }
            public void Open() { IsOpen = true; }
            public void DiscardInBuffer() { incoming.Clear(); }
            public int Read(byte[] buffer, int offset, int count)
            {
                int n = Math.Min(1, Math.Min(count, incoming.Count));
                for (int i = 0; i < n; i++) buffer[offset + i] = incoming.Dequeue();
                return n;
            }
            public int ReadByte() { return incoming.Dequeue(); }
            public int ReadChar() { return ReadByte(); }
            public string ReadExisting() { return ""; }
            public string ReadLine() { return ""; }
            public void Write(string text) { var bytes = Encoding.ASCII.GetBytes(text); Write(bytes, 0, bytes.Length); }
            public void Write(byte[] buffer, int offset, int count) { Written.Write(buffer, offset, count); }
            public void WriteLine(string text) { Write(text + "\n"); }
            public void toggleDTR() { }
            public void Dispose() { Close(); Written.Dispose(); }
        }
    }
}
