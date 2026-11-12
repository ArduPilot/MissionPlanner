using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Comms;

namespace MissionPlannerTests.Mavlink
{
    [TestClass]
    public class MavFtpConcurrencyTests
    {
        private static readonly MethodInfo Dispatch = typeof(MAVLinkInterface).GetMethod("PacketReceived", BindingFlags.Instance | BindingFlags.NonPublic);

        // readPacketAsync releases its parser semaphore before dispatching packets,
        // so concurrent readers are allowed to enter this notification concurrently.
        private static void Reply(MAVLinkInterface link, byte[] data, int offset, int count, byte[] request, bool burstComplete = false)
        {
            var payload = new byte[251];
            uint block = request[3] == 15 ? ((uint)offset - BitConverter.ToUInt32(request, 8)) / request[4] : 0;
            BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(request, 0) + 1 + block)).CopyTo(payload, 0);
            payload[2] = request[2];
            payload[3] = 128; // ACK
            payload[4] = (byte)count;
            payload[5] = request[3];
            payload[6] = burstComplete ? (byte)1 : (byte)0;
            BitConverter.GetBytes((uint)offset).CopyTo(payload, 8);
            Array.Copy(data, offset, payload, 12, count);
            DispatchPayload(link, payload);
        }

        private static void Eof(MAVLinkInterface link, byte[] request, ushort? sequence = null)
        {
            var payload = new byte[251];
            BitConverter.GetBytes(sequence ?? (ushort)(BitConverter.ToUInt16(request, 0) + 1)).CopyTo(payload, 0);
            payload[2] = request[2];
            payload[3] = 129;
            payload[4] = 1;
            payload[5] = request[3];
            payload[12] = 6;
            // ArduPilot leaves offset zero on single-read NAKs. The matching
            // request, not this field, identifies the confirmed EOF position.
            DispatchPayload(link, payload);
        }

        private static void ReadReply(MAVLinkInterface link, byte[] data, byte[] request)
        {
            int offset = (int)BitConverter.ToUInt32(request, 8);
            if (offset >= data.Length) Eof(link, request);
            else Reply(link, data, offset, Math.Min(request[4], data.Length - offset), request);
        }

        private static void DispatchPayload(MAVLinkInterface link, byte[] payload)
        {
            var frame = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL,
                new MAVLink.mavlink_file_transfer_protocol_t { payload = payload }, false, 7, 1);
            Dispatch.Invoke(link, new object[] { new MAVLink.MAVLinkMessage { buffer = frame } });
        }

        [TestMethod]
        public void ConcurrentResponsesAreProcessedByTheTransferOwner()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 80 * 256).Select(i => (byte)(i * 37)).ToArray();
                var progressThreads = new ConcurrentBag<int>();
                var ftp = new MAVFtp(link, 7, 1);
                ftp.Progress += (file, percent) => progressThreads.Add(Thread.CurrentThread.ManagedThreadId);
                int requested = 0;
                Task responses = null;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] != 15) { ReadReply(link, data, request); return; }
                    if (Interlocked.Increment(ref requested) != 1) { Eof(link, request); return; }
                    responses = Task.Run(() =>
                    {
                        Parallel.For(0, 255, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                            i => Reply(link, data, i * 80, 80, request));
                        Reply(link, data, 255 * 80, 80, request, burstComplete: true);
                    });
                };
                int owner = Thread.CurrentThread.ManagedThreadId;
                using (var result = ftp.kCmdBurstReadFile("test.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray(), "Concurrent dispatch must not corrupt bytes or chunk coverage.");
                Assert.IsTrue(responses.Wait(TimeSpan.FromSeconds(5)));
                Assert.IsTrue(progressThreads.Count > 2);
                Assert.IsTrue(progressThreads.All(id => id == owner), "Transfer state and progress must have one owner, independent of packet-reader threads.");
            }
        }

        [TestMethod]
        public void BurstReadUsesTheOpenedSessionAndIgnoresOtherSessions()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 4) // OpenFileRO
                    {
                        var opened = new byte[251];
                        BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(request, 0) + 1)).CopyTo(opened, 0);
                        opened[2] = 9; // A server can allocate a nonzero session.
                        opened[3] = 128;
                        opened[4] = 4;
                        opened[5] = request[3];
                        BitConverter.GetBytes(data.Length).CopyTo(opened, 12);
                        DispatchPayload(link, opened);
                        return;
                    }
                    Assert.AreEqual((byte)9, request[2]);
                    var unrelated = new byte[251];
                    unrelated[2] = 8;
                    unrelated[3] = 129;
                    unrelated[4] = 1;
                    unrelated[5] = request[3];
                    unrelated[12] = 1; // Fail from another session must be ignored.
                    DispatchPayload(link, unrelated);
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        Reply(link, data, 0, 80, request);
                        Reply(link, data, 80, 80, request, burstComplete: true);
                    }
                    else ReadReply(link, data, request);
                };
                var ftp = new MAVFtp(link, 7, 1);
                Assert.IsTrue(ftp.kCmdOpenFileRO("session.bin", out var size, cancel));
                Assert.AreEqual(data.Length, size);
                using (var result = ftp.kCmdBurstReadFile("session.bin", size, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void CancelledBurstDoesNotReturnAnIncompleteFile()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource())
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                wire.WriteCallback += (sender, bytes) => cancel.Cancel();
                var ftp = new MAVFtp(link, 7, 1);
                Assert.ThrowsException<OperationCanceledException>(() => ftp.kCmdBurstReadFile("cancelled.bin", 160, cancel));
            }
        }

        [TestMethod]
        public void ShortPacketsDoNotEstablishEofBeforeTheServerConfirmsIt()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] != 15) { ReadReply(link, data, request); return; }
                    for (int offset = 0; offset < data.Length; offset += 40)
                        Reply(link, data, offset, 40, request, burstComplete: offset == 120);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("short.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void EndOfFileWithMissingTailRequestsTheMissingBytes()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 240).Select(i => (byte)i).ToArray();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15)
                    {
                        Reply(link, data, 0, 80, request);
                        Eof(link, request); // A burst EOF can arrive after lost data packets.
                    }
                    else if (request[3] == 5)
                        ReadReply(link, data, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("tail.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void GeneratedFileSizesAreEstimatesAndEofDoesNotLoop()
        {
            // Captured parameter transfer: open advertises 13116 bytes, actual
            // serialized data ends at 12832, and a read at 12832 returns EOF.
            foreach (int estimate in new[] { 13116, 12000, 12832, 0 })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 12832).Select(i => (byte)i).ToArray();
                int requests = 0;
                wire.WriteCallback += (sender, bytes) =>
                {
                    Assert.IsTrue(++requests <= 3, "EOF must not cause repeated identical recovery requests.");
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] != 15) { ReadReply(link, data, request); return; }
                    for (int offset = 0; offset < data.Length; offset += 110)
                    {
                        int count = Math.Min(110, data.Length - offset);
                        Reply(link, data, offset, count, request, burstComplete: offset + count == data.Length);
                    }
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("@PARAM/param.pck", estimate, cancel, 110))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void StaleEofCannotCompleteAnOutstandingRead()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15)
                    {
                        Reply(link, data, 0, 80, request);
                        Eof(link, request);
                    }
                    else
                    {
                        // This old response must not establish EOF at the new offset.
                        Eof(link, request, sequence: (ushort)(BitConverter.ToUInt16(request, 0) - 1));
                        ReadReply(link, data, request);
                    }
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("stale.bin", 240, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void DuplicateOverlapsDoNotLoseCoverageDuringGapRecovery()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 240).Select(i => (byte)i).ToArray();
                var reads = new System.Collections.Generic.List<uint>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        Reply(link, data, 0, 80, request);
                        Reply(link, data, 0, 40, request); // A short duplicate must not shrink coverage.
                        Reply(link, data, 160, 80, request, burstComplete: true);
                    }
                    else if (request[3] == 15) Eof(link, request);
                    else
                    {
                        reads.Add(BitConverter.ToUInt32(request, 8));
                        ReadReply(link, data, request);
                    }
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("gap.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                CollectionAssert.AreEqual(new uint[] { 80, 240 }, reads);
            }
        }

        [TestMethod]
        public void EofInsidePreviouslyReceivedDataFailsInsteadOfLooping()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = new byte[240];
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                        Reply(link, data, 160, 80, request, burstComplete: true);
                    else Eof(link, request);
                };
                Assert.ThrowsException<EndOfStreamException>(() => new MAVFtp(link, 7, 1).kCmdBurstReadFile("changed.bin", 240, cancel));
            }
        }
    }
}
