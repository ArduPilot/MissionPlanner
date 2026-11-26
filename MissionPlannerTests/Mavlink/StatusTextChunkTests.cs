using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.ArduPilot.Mavlink;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class StatusTextChunkTests
    {
        // 59 characters, so two chunks: "...Wait " and "or reboot"
        private const string Long = "PreArm: AHRS: EKF3 Yaw inconsistent 140 deg. Wait or reboot";
        private static readonly DateTime Start = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Split text the way ArduPilot's GCS::send_textv does: text that fits is sent with id 0,
        // anything longer as 50 byte chunks sharing an id, ending on a chunk that is not full.
        private static List<MAVLink.mavlink_statustext_t> Chunks(string text, ushort id, byte severity = 2)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            var chunks = new List<MAVLink.mavlink_statustext_t>();
            if (bytes.Length <= StatusTextAssembler.ChunkSize)
                id = 0;
            for (int offset = 0; ; offset += StatusTextAssembler.ChunkSize)
            {
                int n = Math.Min(StatusTextAssembler.ChunkSize, bytes.Length - offset);
                var chunk = new byte[StatusTextAssembler.ChunkSize];
                Array.Copy(bytes, offset, chunk, 0, n);
                chunks.Add(new MAVLink.mavlink_statustext_t(severity, chunk, id, (byte)chunks.Count));
                if (n < StatusTextAssembler.ChunkSize || id == 0)
                    return chunks;
            }
        }

        private static string Text(char c, int length) => new string(c, length);

        [TestMethod]
        public void SplitMessageIsReturnedWholeOnItsLastChunk()
        {
            var assembler = new StatusTextAssembler();
            var chunks = Chunks(Long, 7);
            Assert.AreEqual(2, chunks.Count);
            Assert.IsNull(assembler.Add(1, 1, chunks[0], Start));
            Assert.IsTrue(assembler.HasPending);
            Assert.AreEqual(Long, assembler.Add(1, 1, chunks[1], Start));
            Assert.IsFalse(assembler.HasPending);
            Assert.AreEqual(0, assembler.TakeExpired(Start.AddHours(1)).Count);
        }

        [TestMethod]
        public void UnchunkedMessagesPassStraightThrough()
        {
            var assembler = new StatusTextAssembler();
            foreach (string text in new[] { "", "PreArm: RC not found", Text('x', 50) })
            {
                var chunks = Chunks(text, 7);
                Assert.AreEqual(1, chunks.Count);
                Assert.AreEqual((ushort)0, chunks[0].id);
                Assert.AreEqual(text, assembler.Add(1, 1, chunks[0], Start));
                Assert.IsFalse(assembler.HasPending);
            }
            // a sender that sets an id on a message that fits in one packet
            var single = new MAVLink.mavlink_statustext_t(2, Encoding.UTF8.GetBytes("short".PadRight(50, '\0')), 9, 0);
            Assert.AreEqual("short", assembler.Add(1, 1, single, Start));
            Assert.IsFalse(assembler.HasPending);
        }

        [TestMethod]
        public void ExactMultipleOfChunkSizeEndsOnAnEmptyChunk()
        {
            var assembler = new StatusTextAssembler();
            string text = Text('a', 50) + Text('b', 50);
            var chunks = Chunks(text, 7);
            Assert.AreEqual(3, chunks.Count);
            Assert.IsNull(assembler.Add(1, 1, chunks[0], Start));
            Assert.IsNull(assembler.Add(1, 1, chunks[1], Start));
            Assert.AreEqual(text, assembler.Add(1, 1, chunks[2], Start));
        }

        [TestMethod]
        public void ChunksCanArriveOutOfOrderAndInterleavedBetweenSenders()
        {
            var assembler = new StatusTextAssembler();
            string a = Text('a', 60) + Text('b', 60), b = Text('c', 70);
            var first = Chunks(a, 7);
            // the same id is also in use by another system and by another component
            var second = Chunks(b, 7);
            Assert.AreEqual(3, first.Count);
            Assert.IsNull(assembler.Add(1, 1, first[2], Start));
            Assert.IsNull(assembler.Add(2, 1, second[1], Start));
            Assert.IsNull(assembler.Add(1, 2, second[0], Start));
            Assert.IsNull(assembler.Add(1, 1, first[0], Start));
            Assert.AreEqual(b, assembler.Add(2, 1, second[0], Start));
            Assert.AreEqual(b, assembler.Add(1, 2, second[1], Start));
            // a repeated chunk changes nothing
            Assert.IsNull(assembler.Add(1, 1, first[0], Start));
            Assert.AreEqual(a, assembler.Add(1, 1, first[1], Start));
            Assert.IsFalse(assembler.HasPending);
        }

        [TestMethod]
        public void CharacterSplitAcrossChunksIsDecodedOnce()
        {
            var assembler = new StatusTextAssembler();
            // the two bytes of the e-acute land either side of the chunk boundary
            string text = Text('a', 49) + "\u00e9 tail";
            var chunks = Chunks(text, 7);
            Assert.AreEqual(2, chunks.Count);
            Assert.IsNull(assembler.Add(1, 1, chunks[0], Start));
            Assert.AreEqual(text, assembler.Add(1, 1, chunks[1], Start));
        }

        [TestMethod]
        public void IncompleteMessagesAreReleasedAfterTheTimeout()
        {
            var assembler = new StatusTextAssembler();
            var chunks = Chunks(Long, 7, 4);
            var three = Chunks(Text('a', 50) + Text('b', 50) + Text('c', 10), 8, 6);

            // last chunk lost
            Assert.IsNull(assembler.Add(1, 1, chunks[0], Start));
            // first chunk lost, the wait runs from the most recent chunk
            Assert.IsNull(assembler.Add(2, 1, chunks[1], Start.AddSeconds(0.2)));
            // middle chunk lost
            Assert.IsNull(assembler.Add(3, 1, three[0], Start.AddSeconds(0.4)));
            Assert.IsNull(assembler.Add(3, 1, three[2], Start.AddSeconds(0.6)));

            Assert.AreEqual(0, assembler.TakeExpired(Start.AddSeconds(1)).Count);

            var expired = assembler.TakeExpired(Start.AddSeconds(1.1));
            Assert.AreEqual(1, expired.Count);
            Assert.AreEqual((1u, (byte)1, (byte)4, Long.Substring(0, 50)), expired[0]);
            Assert.IsTrue(assembler.HasPending);

            expired = assembler.TakeExpired(Start.AddSeconds(2));
            Assert.AreEqual(2, expired.Count);
            Assert.AreEqual((2u, (byte)1, (byte)4, " ... " + Long.Substring(50)), expired[0]);
            Assert.AreEqual((3u, (byte)1, (byte)6, Text('a', 50) + " ... " + Text('c', 10)), expired[1]);
            Assert.IsFalse(assembler.HasPending);
            Assert.AreEqual(0, assembler.TakeExpired(Start.AddHours(1)).Count);

            // a late chunk still completes its message while it is being held
            Assert.IsNull(assembler.Add(1, 1, chunks[1], Start));
            Assert.AreEqual(Long, assembler.Add(1, 1, chunks[0], Start.AddSeconds(0.9)));
        }

        private static readonly byte[] Stamp = BitConverter.GetBytes(1700000000000000UL).Reverse().ToArray();

        private static BinaryReader Tlog(params byte[][] frames)
        {
            return new BinaryReader(new MemoryStream(frames.SelectMany(frame => Stamp.Concat(frame)).ToArray()));
        }

        private static string[] Messages(MAVLinkInterface port, uint sysid, byte compid)
        {
            return port.MAVlist[sysid, compid].cs.messages.Select(a => a.message).ToArray();
        }

        [TestMethod]
        public void SplitStatusTextIsOneEntryInTheMessageList()
        {
            var parser = new MAVLink.MavlinkParse();
            var chunks = Chunks(Long, 7);
            using (var port = new MAVLinkInterface())
            {
                port.logreadmode = true;
                port.logplaybackfile = Tlog(
                    parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, Chunks("PreArm: RC not found", 0)[0], false, 1, 1, 0),
                    parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, chunks[0], false, 1, 1, 1),
                    parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, chunks[1], false, 1, 1, 2));
                Assert.AreEqual(1u, port.readPacket().sysid);
                Assert.AreEqual(1u, port.readPacket().sysid);
                // nothing is shown for the first half on its own
                CollectionAssert.AreEqual(new[] { "PreArm: RC not found" }, Messages(port, 1, 1));
                Assert.AreEqual(1u, port.readPacket().sysid);
                CollectionAssert.AreEqual(new[] { "PreArm: RC not found", Long }, Messages(port, 1, 1));
            }
        }

        // MAVLink 1 leaves the extension fields off the wire, so id and chunk_seq never arrive
        private static byte[] Mavlink1Frame(MAVLink.mavlink_statustext_t msg, byte seq)
        {
            var frame = new List<byte> { 254, 51, seq, 1, 1, 253, msg.severity };
            frame.AddRange(msg.text);
            ushort crc = MAVLink.MavlinkCRC.crc_calculate(frame.ToArray(), frame.Count);
            crc = MAVLink.MavlinkCRC.crc_accumulate(83, crc);
            frame.Add((byte)crc);
            frame.Add((byte)(crc >> 8));
            return frame.ToArray();
        }

        [TestMethod]
        public void Mavlink1StatusTextIsShownAsReceived()
        {
            // with nothing to join the chunks by they are shown as they arrive, as before
            var chunks = Chunks(Long, 7);
            using (var port = new MAVLinkInterface())
            {
                port.logreadmode = true;
                port.logplaybackfile = Tlog(Mavlink1Frame(chunks[0], 0), Mavlink1Frame(chunks[1], 1));
                port.readPacket();
                CollectionAssert.AreEqual(new[] { Long.Substring(0, 50) }, Messages(port, 1, 1));
                port.readPacket();
                CollectionAssert.AreEqual(new[] { Long.Substring(0, 50), Long.Substring(50) }, Messages(port, 1, 1));
            }
        }

        [TestMethod]
        public void IncompleteStatusTextIsShownOnceItTimesOut()
        {
            var parser = new MAVLink.MavlinkParse();
            var hb = new MAVLink.mavlink_heartbeat_t { type = 2, autopilot = 3, mavlink_version = 3 };
            var heartbeats = Enumerable.Range(0, 4).Select(seq =>
                parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, false, 1, 1, seq)).ToArray();
            byte[] half = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, Chunks(Long, 7)[0], false, 1, 1, 1);
            byte[] newer = parser.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, Chunks("PreArm: RC not found", 0)[0], false, 1, 1, 3);
            using (var released = new MAVLinkInterface())
            using (var overtaken = new MAVLinkInterface())
            {
                released.logreadmode = overtaken.logreadmode = true;
                released.logplaybackfile = Tlog(heartbeats[0], half, heartbeats[2], heartbeats[3]);
                overtaken.logplaybackfile = Tlog(heartbeats[0], half, heartbeats[2], newer);
                // the last chunk never arrives, nothing is shown while it could still turn up
                foreach (var port in new[] { released, overtaken })
                {
                    port.readPacket();
                    port.readPacket();
                    port.readPacket();
                    Assert.AreEqual(0, Messages(port, 1, 1).Length);
                }
                Thread.Sleep(StatusTextAssembler.Timeout + TimeSpan.FromMilliseconds(200));
                // then any packet releases what did arrive
                released.readPacket();
                CollectionAssert.AreEqual(new[] { Long.Substring(0, 50) }, Messages(released, 1, 1));
                // and it is listed ahead of a newer message
                overtaken.readPacket();
                CollectionAssert.AreEqual(new[] { Long.Substring(0, 50), "PreArm: RC not found" }, Messages(overtaken, 1, 1));
            }
        }
    }
}
