using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;

namespace MissionPlannerTests.Mavlink
{
    /// <summary>
    /// Encoding and decoding of MAVLink frames, checked against frames packed by pymavlink
    /// rather than against MissionPlanner's own encoder.
    /// </summary>
    [TestClass]
    public class MavlinkCodecTests
    {
        // Packed by pymavlink 2.4 (ardupilotmega dialect) with srcSystem=1, srcComponent=1 and the
        // sequence numbers given. The script that made them is in MissionPlannerTests/TestData/README.md.
        const string V2Heartbeat = "FD0900000501010000000400000002035103030C84";
        const string V2ParamValue = "FD1900000601011600000080BB44B0042A0052544C5F414C5400000000000000000009B589";
        const string V2CommandLong = "FD2000000701014C00000000803F00000000000000000000000000000000000000000000000090010101A00F";
        const string V2MissionCount = "FD0500000801012C00000700010102C303";
        const string V2GpsRawInt = "FD34000009010118000015CD5B07000000009EFEEBEACCCEE85840E908007900FFFF0000FFFF030AC0270900E8030000D0070000B80B0000A00F00009F8C76C7";
        const string V2StatusText = "FD1600000A0101FD00000641726475436F707465722056342E372E302D646576CE88";
        const string V1Heartbeat = "FE090B010100040000000203510303B98D";
        const string V1Attitude = "FE1C0C01011EE8030000CDCCCC3DCDCC4CBE000040400AD7233C0AD7A33C8FC2F5BC858D";
        // key 00..1f, link id 3, timestamp 0x0102030405
        const string V2SignedHeartbeat = "FD0901000D01010000000400000002035103032F2C03050403020100713739292783";

        static byte[] Hex(string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }

        static MAVLink.MAVLinkMessage Read(byte[] frame, bool timestamped = false)
        {
            return new MAVLink.MavlinkParse(timestamped).ReadPacket(new MemoryStream(frame));
        }

        static string Text(byte[] chars)
        {
            return Encoding.ASCII.GetString(chars).TrimEnd('\0');
        }

        [TestMethod]
        public void DecodesPymavlinkFrames()
        {
            var hb = Read(Hex(V2Heartbeat));
            Assert.IsNotNull(hb);
            Assert.IsTrue(hb.ismavlink2);
            Assert.AreEqual(1u, hb.sysid);
            Assert.AreEqual((byte)1, hb.compid);
            Assert.AreEqual((byte)5, hb.seq);
            Assert.AreEqual((uint)MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb.msgid);
            Assert.AreEqual("HEARTBEAT", hb.msgtypename);
            var heartbeat = hb.ToStructure<MAVLink.mavlink_heartbeat_t>();
            Assert.AreEqual((byte)2, heartbeat.type);
            Assert.AreEqual((byte)3, heartbeat.autopilot);
            Assert.AreEqual((byte)81, heartbeat.base_mode);
            Assert.AreEqual(4u, heartbeat.custom_mode);
            Assert.AreEqual((byte)3, heartbeat.system_status);
            Assert.AreEqual((byte)3, heartbeat.mavlink_version);

            var param = Read(Hex(V2ParamValue)).ToStructure<MAVLink.mavlink_param_value_t>();
            Assert.AreEqual("RTL_ALT", Text(param.param_id));
            Assert.AreEqual(1500f, param.param_value);
            Assert.AreEqual((byte)MAVLink.MAV_PARAM_TYPE.REAL32, param.param_type);
            Assert.AreEqual((ushort)1200, param.param_count);
            Assert.AreEqual((ushort)42, param.param_index);

            var command = Read(Hex(V2CommandLong));
            Assert.IsTrue(command.payloadlength < 33, "pymavlink trims the zero tail of COMMAND_LONG");
            var commandLong = command.ToStructure<MAVLink.mavlink_command_long_t>();
            Assert.AreEqual((ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, commandLong.command);
            Assert.AreEqual(1f, commandLong.param1);
            Assert.AreEqual(0f, commandLong.param7);
            Assert.AreEqual((byte)1, commandLong.target_system);
            Assert.AreEqual((byte)1, commandLong.target_component);
            Assert.AreEqual((byte)0, commandLong.confirmation);

            // an extension field
            var count = Read(Hex(V2MissionCount)).ToStructure<MAVLink.mavlink_mission_count_t>();
            Assert.AreEqual((ushort)7, count.count);
            Assert.AreEqual((byte)MAVLink.MAV_MISSION_TYPE.RALLY, count.mission_type);

            var gps = Read(Hex(V2GpsRawInt)).ToStructure<MAVLink.mavlink_gps_raw_int_t>();
            Assert.AreEqual(123456789ul, gps.time_usec);
            Assert.AreEqual(-353632610, gps.lat);
            Assert.AreEqual(1491652300, gps.lon);
            Assert.AreEqual(584000, gps.alt);
            Assert.AreEqual((ushort)65535, gps.epv);
            Assert.AreEqual((byte)10, gps.satellites_visible);
            Assert.AreEqual(600000, gps.alt_ellipsoid);
            Assert.AreEqual(4000u, gps.hdg_acc);
            Assert.AreEqual((ushort)35999, gps.yaw);

            var text = Read(Hex(V2StatusText)).ToStructure<MAVLink.mavlink_statustext_t>();
            Assert.AreEqual((byte)MAVLink.MAV_SEVERITY.INFO, text.severity);
            Assert.AreEqual("ArduCopter V4.7.0-dev", Text(text.text));

            var v1 = Read(Hex(V1Heartbeat));
            Assert.IsFalse(v1.ismavlink2);
            Assert.AreEqual((byte)11, v1.seq);
            Assert.AreEqual(4u, v1.ToStructure<MAVLink.mavlink_heartbeat_t>().custom_mode);

            var attitude = Read(Hex(V1Attitude)).ToStructure<MAVLink.mavlink_attitude_t>();
            Assert.AreEqual(1000u, attitude.time_boot_ms);
            Assert.AreEqual(0.1f, attitude.roll);
            Assert.AreEqual(-0.2f, attitude.pitch);
            Assert.AreEqual(3.0f, attitude.yaw);
            Assert.AreEqual(-0.03f, attitude.yawspeed);
        }

        [TestMethod]
        public void EncodesTheSameBytesAsPymavlink()
        {
            var parser = new MAVLink.MavlinkParse();
            var heartbeat = new MAVLink.mavlink_heartbeat_t
            {
                type = 2, autopilot = 3, base_mode = 81, custom_mode = 4, system_status = 3, mavlink_version = 3
            };
            CollectionAssert.AreEqual(Hex(V2Heartbeat),
                parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, heartbeat, false, 1, 1, 5));
            CollectionAssert.AreEqual(Hex(V1Heartbeat),
                parser.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, heartbeat, 1, 1, 11));

            var param = new MAVLink.mavlink_param_value_t(1500f, 1200, 42, Encoding.ASCII.GetBytes("RTL_ALT\0\0\0\0\0\0\0\0\0"),
                (byte)MAVLink.MAV_PARAM_TYPE.REAL32);
            CollectionAssert.AreEqual(Hex(V2ParamValue),
                parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.PARAM_VALUE, param, false, 1, 1, 6));

            var command = new MAVLink.mavlink_command_long_t
            {
                param1 = 1, command = (ushort)MAVLink.MAV_CMD.COMPONENT_ARM_DISARM, target_system = 1, target_component = 1
            };
            CollectionAssert.AreEqual(Hex(V2CommandLong),
                parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG, command, false, 1, 1, 7));

            var count = new MAVLink.mavlink_mission_count_t
            {
                target_system = 1, target_component = 1, count = 7, mission_type = (byte)MAVLink.MAV_MISSION_TYPE.RALLY
            };
            CollectionAssert.AreEqual(Hex(V2MissionCount),
                parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_COUNT, count, false, 1, 1, 8));

            var attitude = new MAVLink.mavlink_attitude_t(1000, 0.1f, -0.2f, 3.0f, 0.01f, 0.02f, -0.03f);
            CollectionAssert.AreEqual(Hex(V1Attitude),
                parser.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.ATTITUDE, attitude, 1, 1, 12));
        }

        [TestMethod]
        public void SignedPymavlinkFrameVerifiesWithTheSameKey()
        {
            byte[] frame = Hex(V2SignedHeartbeat);
            var message = Read(frame);
            Assert.IsNotNull(message);
            Assert.AreEqual((byte)MAVLink.MAVLINK_IFLAG_SIGNED, message.incompat_flags);
            Assert.AreEqual((byte)3, message.sigLinkid);
            Assert.AreEqual(0x0102030405ul, message.sigTimestamp);

            byte[] key = Enumerable.Range(0, 32).Select(a => (byte)a).ToArray();
            using (var sha = SHA256.Create())
            {
                var signed = key.Concat(frame.Take(frame.Length - 6)).ToArray();
                CollectionAssert.AreEqual(frame.Skip(frame.Length - 6).ToArray(), sha.ComputeHash(signed).Take(6).ToArray());
            }
        }

        [TestMethod]
        public void EveryMessageTypeRoundTripsThroughV2()
        {
            var random = new Random(1234);
            var parser = new MAVLink.MavlinkParse();
            int checkedTypes = 0;
            foreach (var info in MAVLink.MAVLINK_MESSAGE_INFOS)
            {
                if (info.type == null)
                    continue;

                byte[] payload = RandomPayload(random, info);
                object message = MavlinkUtil.ByteArrayToStructureGC(payload, info.type, 0, (byte)payload.Length);

                byte[] frame = parser.GenerateMAVLinkPacket20((MAVLink.MAVLINK_MSG_ID)info.msgid, message, false, 1, 1);
                var decoded = Read(frame);

                Assert.IsNotNull(decoded, info.name);
                Assert.AreEqual(info.msgid, decoded.msgid, info.name);
                Assert.AreEqual(info.name, decoded.msgtypename);
                Assert.IsTrue(decoded.payloadlength <= info.length, info.name);
                Assert.IsTrue(decoded.payloadlength >= 1, info.name);
                byte[] trimmed = (byte[])payload.Clone();
                MavlinkUtil.trim_payload(ref trimmed);
                CollectionAssert.AreEqual(trimmed, decoded.buffer.Skip(decoded.headerlength).Take(decoded.payloadlength).ToArray(), info.name);
                CollectionAssert.AreEqual(payload, MavlinkUtil.StructureToByteArray(decoded.data).Take(payload.Length).ToArray(), info.name);
                checkedTypes++;
            }

            Assert.IsTrue(checkedTypes > 200, "only " + checkedTypes + " message types");
        }

        [TestMethod]
        public void MessagesBelow256RoundTripThroughV1()
        {
            var random = new Random(4321);
            var parser = new MAVLink.MavlinkParse();
            foreach (var info in MAVLink.MAVLINK_MESSAGE_INFOS.Where(a => a.msgid < 256 && a.type != null))
            {
                byte[] payload = RandomPayload(random, info);
                object message = MavlinkUtil.ByteArrayToStructureGC(payload, info.type, 0, (byte)payload.Length);
                byte[] frame = parser.GenerateMAVLinkPacket10((MAVLink.MAVLINK_MSG_ID)info.msgid, message, 7, 9);
                var decoded = Read(frame);

                Assert.IsNotNull(decoded, info.name);
                Assert.IsFalse(decoded.ismavlink2);
                Assert.AreEqual(7u, decoded.sysid);
                Assert.AreEqual((byte)9, decoded.compid);
                Assert.AreEqual(info.msgid, decoded.msgid, info.name);
                CollectionAssert.AreEqual(payload, MavlinkUtil.StructureToByteArray(decoded.data).Take(payload.Length).ToArray(), info.name);
            }
        }

        /// <summary>
        /// a payload of the message's full length, ending in a non zero byte so the length of the
        /// trimmed form is known
        /// </summary>
        static byte[] RandomPayload(Random random, MAVLink.message_info info)
        {
            int size = Math.Max((int)info.length, Marshal.SizeOf(info.type));
            var payload = new byte[size];
            random.NextBytes(payload);
            // the floats and doubles stay finite: NaN payloads need not survive a float copy
            foreach (var field in info.type.GetFields())
            {
                int offset = (int)Marshal.OffsetOf(info.type, field.Name);
                Type type = field.FieldType.IsArray ? field.FieldType.GetElementType() : field.FieldType;
                int count = 1;
                if (field.FieldType.IsArray)
                {
                    var marshal = (MarshalAsAttribute)field.GetCustomAttributes(typeof(MarshalAsAttribute), false).Single();
                    count = marshal.SizeConst;
                }

                for (int i = 0; i < count; i++)
                {
                    if (type == typeof(float))
                        payload[offset + i * 4 + 3] &= 0x3f;
                    else if (type == typeof(double))
                        payload[offset + i * 8 + 7] &= 0x3f;
                }
            }

            Array.Resize(ref payload, (int)info.length);
            if (payload[payload.Length - 1] == 0)
                payload[payload.Length - 1] = 1;
            return payload;
        }

        [TestMethod]
        public void ZeroPayloadIsSentAsOneByteAndDecodesAsZero()
        {
            var parser = new MAVLink.MavlinkParse();
            byte[] frame = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.ATTITUDE, new MAVLink.mavlink_attitude_t());
            var decoded = Read(frame);
            Assert.AreEqual((byte)1, decoded.payloadlength);
            var attitude = decoded.ToStructure<MAVLink.mavlink_attitude_t>();
            Assert.AreEqual(0u, attitude.time_boot_ms);
            Assert.AreEqual(0f, attitude.yawspeed);
        }

        [TestMethod]
        public void ParserSkipsNoiseAndBadFramesAndKeepsStreamAligned()
        {
            var frames = new List<byte[]>
            {
                Hex(V2Heartbeat), Hex(V1Heartbeat), Hex(V2ParamValue), Hex(V2SignedHeartbeat), Hex(V2GpsRawInt)
            };
            byte[] badCrc = Hex(V2StatusText);
            badCrc[badCrc.Length - 1] ^= 0x55;

            var stream = new MemoryStream();
            var random = new Random(99);
            var noise = new byte[37];
            random.NextBytes(noise);
            // noise without a start byte in it
            for (int i = 0; i < noise.Length; i++)
                if (noise[i] == MAVLink.MAVLINK_STX || noise[i] == MAVLink.MAVLINK_STX_MAVLINK1)
                    noise[i] = 0x55;
            stream.Write(noise, 0, noise.Length);
            stream.Write(frames[0], 0, frames[0].Length);
            stream.Write(badCrc, 0, badCrc.Length);
            foreach (var frame in frames.Skip(1))
                stream.Write(frame, 0, frame.Length);
            stream.Position = 0;

            var parser = new MAVLink.MavlinkParse();
            var read = new List<MAVLink.MAVLinkMessage>();
            while (stream.Position < stream.Length)
                read.Add(parser.ReadPacket(stream));

            Assert.AreEqual(1, parser.badCRC);
            var good = read.Where(a => a != null).ToList();
            Assert.AreEqual(frames.Count, good.Count);
            for (int i = 0; i < frames.Count; i++)
                CollectionAssert.AreEqual(frames[i], good[i].buffer);
        }

        [TestMethod]
        public void TruncatedFramesAtTheEndOfAStreamReturnNull()
        {
            byte[] frame = Hex(V2GpsRawInt);
            for (int length = 1; length < frame.Length; length++)
            {
                var stream = new MemoryStream(frame, 0, length);
                Assert.IsNull(new MAVLink.MavlinkParse().ReadPacket(stream), "length " + length);
            }
        }

        [TestMethod]
        public void SingleBitErrorsAreRejected()
        {
            byte[] frame = Hex(V2ParamValue);
            // the start byte, the length and the flags change the framing and are covered elsewhere
            for (int i = 4; i < frame.Length; i++)
            {
                byte[] corrupt = (byte[])frame.Clone();
                corrupt[i] ^= 0x10;
                var decoded = new MAVLink.MavlinkParse().ReadPacket(new MemoryStream(corrupt));
                Assert.IsNull(decoded, "corrupt byte " + i);
            }
        }

        [TestMethod]
        public void TlogTimestampsAreReadWithEachFrame()
        {
            var start = new DateTime(2024, 5, 6, 7, 8, 9, DateTimeKind.Utc);
            var frames = new[] { Hex(V2Heartbeat), Hex(V1Attitude), Hex(V2GpsRawInt) };
            var tlog = new MemoryStream();
            for (int i = 0; i < frames.Length; i++)
            {
                ulong micros = (ulong)((start.AddMilliseconds(250 * i) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10);
                var stamp = BitConverter.GetBytes(micros).Reverse().ToArray();
                tlog.Write(stamp, 0, 8);
                tlog.Write(frames[i], 0, frames[i].Length);
            }
            tlog.Position = 0;

            var parser = new MAVLink.MavlinkParse(true);
            for (int i = 0; i < frames.Length; i++)
            {
                var message = parser.ReadPacket(tlog);
                CollectionAssert.AreEqual(frames[i], message.buffer);
                Assert.AreEqual(start.AddMilliseconds(250 * i), message.rxtime.ToUniversalTime());
            }
            Assert.AreEqual(tlog.Length, tlog.Position);
        }

        [TestMethod]
        public void TlogPlaybackUpdatesVehicleState()
        {
            var parser = new MAVLink.MavlinkParse();
            var tlog = new MemoryStream();
            var time = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            void Write(MAVLink.MAVLINK_MSG_ID id, object message, int i)
            {
                ulong micros = (ulong)((time.AddSeconds(i) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks / 10);
                tlog.Write(BitConverter.GetBytes(micros).Reverse().ToArray(), 0, 8);
                var frame = parser.GenerateMAVLinkPacket20(id, message, false, 1, 1);
                tlog.Write(frame, 0, frame.Length);
            }

            for (int i = 0; i < 3; i++)
            {
                Write(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, new MAVLink.mavlink_heartbeat_t
                {
                    type = (byte)MAVLink.MAV_TYPE.QUADROTOR, autopilot = (byte)MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                    custom_mode = 4, base_mode = (byte)MAVLink.MAV_MODE_FLAG.CUSTOM_MODE_ENABLED, mavlink_version = 3
                }, i);
                Write(MAVLink.MAVLINK_MSG_ID.ATTITUDE, new MAVLink.mavlink_attitude_t { roll = 0.5f, pitch = -0.25f, yaw = 1 }, i);
                Write(MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, new MAVLink.mavlink_global_position_int_t
                {
                    lat = -353632610, lon = 1491652300, alt = 600000, relative_alt = 16000
                }, i);
            }
            tlog.Position = 0;

            using (var port = new MAVLinkInterface(tlog))
            {
                int count = 0;
                while (tlog.Position < tlog.Length)
                {
                    var message = port.readPacket();
                    if (message != MAVLink.MAVLinkMessage.Invalid)
                        count++;
                }

                Assert.AreEqual(9, count);
                var mav = port.MAVlist[1, 1];
                Assert.AreEqual(MAVLink.MAV_TYPE.QUADROTOR, mav.aptype);
                Assert.AreEqual(MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA, mav.apname);
                Assert.IsNotNull(mav.getPacket((uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE));
                Assert.AreEqual(-353632610, mav.getPacket((uint)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                    .ToStructure<MAVLink.mavlink_global_position_int_t>().lat);
                Assert.AreEqual(time.AddSeconds(2), port.lastlogread.ToUniversalTime());
            }
        }
    }
}
