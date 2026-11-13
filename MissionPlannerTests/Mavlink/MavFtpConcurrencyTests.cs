using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
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

        private static void SetNextSequence(MAVFtp ftp, ushort next)
        {
            var allocator = typeof(MAVFtp).GetField("sequence", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ftp);
            allocator.GetType().GetField("next", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(allocator, next);
        }

        private sealed class CallbackAppender : AppenderSkeleton
        {
            private readonly Action<LoggingEvent> callback;
            public CallbackAppender(Action<LoggingEvent> callback) { this.callback = callback; }
            protected override void Append(LoggingEvent loggingEvent) { callback(loggingEvent); }
        }

        // readPacketAsync releases its parser semaphore before dispatching packets,
        // so concurrent readers are allowed to enter this notification concurrently.
        private static void Reply(MAVLinkInterface link, byte[] data, int offset, int count, byte[] request, bool burstComplete = false, ushort? sequence = null)
        {
            var payload = new byte[251];
            uint block = request[3] == 15 ? ((uint)offset - BitConverter.ToUInt32(request, 8)) / request[4] : 0;
            BitConverter.GetBytes(sequence ?? (ushort)(BitConverter.ToUInt16(request, 0) + 1 + block)).CopyTo(payload, 0);
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
            Nak(link, request, MAVFtp.FTPErrorCode.kErrEOF, sequence);
        }

        private static void Nak(MAVLinkInterface link, byte[] request, MAVFtp.FTPErrorCode error,
            ushort? sequence = null, byte size = 1)
        {
            var payload = new byte[251];
            BitConverter.GetBytes(sequence ?? (ushort)(BitConverter.ToUInt16(request, 0) + 1)).CopyTo(payload, 0);
            payload[2] = request[2];
            payload[3] = 129;
            payload[4] = size;
            payload[5] = request[3];
            payload[12] = (byte)error;
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

        private static void DispatchPayload(MAVLinkInterface link, byte[] payload, byte system = 7, byte component = 1)
        {
            var frame = new MAVLink.MavlinkParse().GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.FILE_TRANSFER_PROTOCOL,
                new MAVLink.mavlink_file_transfer_protocol_t { payload = payload }, false, system, component);
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
        [DoNotParallelize] // Temporarily configures the MAVFtp logger to hit the completion boundary.
        public void CancellationAfterConfirmedEofReturnsTheCompletedFile()
        {
            foreach (int length in new[] { 0, 160 })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, length).Select(i => (byte)i).ToArray();
                string file = "completed-" + Guid.NewGuid().ToString("N") + ".bin";
                bool cancelledAfterEof = false, reportedComplete = false;
                var ftp = new MAVFtp(link, 7, 1);
                ftp.Progress += (name, percent) => { if (percent == 100) reportedComplete = true; };
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && length > 0 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        Reply(link, data, 0, 80, request);
                        Reply(link, data, 80, 80, request, burstComplete: true);
                    }
                    else ReadReply(link, data, request);
                };

                // This log is emitted synchronously after EOF establishes completion,
                // before the old post-loop cancellation check. No sleeps or race odds.
                var logger = (Logger)LogManager.GetLogger(typeof(MAVFtp).Assembly, typeof(MAVFtp)).Logger;
                var repository = (Hierarchy)logger.Repository;
                var oldLevel = logger.Level;
                var oldThreshold = repository.Threshold;
                bool oldConfigured = repository.Configured;
                var appender = new CallbackAppender(entry =>
                {
                    if (entry.RenderedMessage.StartsWith("Done " + file + ", confirmed EOF at ", StringComparison.Ordinal))
                    {
                        cancelledAfterEof = true;
                        cancel.Cancel();
                    }
                });
                appender.ActivateOptions();
                logger.AddAppender(appender);
                logger.Level = Level.Info;
                repository.Threshold = Level.All;
                repository.Configured = true;
                try
                {
                    using (var result = ftp.kCmdBurstReadFile(file, data.Length, cancel))
                    {
                        CollectionAssert.AreEqual(data, result.ToArray());
                        Assert.AreEqual(0L, result.Position, "Completed data must be returned ready to read.");
                    }
                    Assert.IsTrue(cancelledAfterEof, "The test must cancel after EOF has been accepted.");
                    Assert.IsTrue(cancel.IsCancellationRequested);
                    Assert.IsTrue(reportedComplete, "Late cancellation must not hide successful completion.");
                }
                finally
                {
                    logger.RemoveAppender(appender);
                    appender.Close();
                    logger.Level = oldLevel;
                    repository.Threshold = oldThreshold;
                    repository.Configured = oldConfigured;
                }
            }
        }

        [TestMethod]
        public void CancellationBeforeConfirmedEofStillCancelsTheTransfer()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource())
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = new byte[160];
                var ftp = new MAVFtp(link, 7, 1);
                bool receivedAllBytes = false;
                ftp.Progress += (file, percent) =>
                {
                    if (percent == 99) { receivedAllBytes = true; cancel.Cancel(); }
                };
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    Reply(link, data, 0, 80, request);
                    Reply(link, data, 80, 80, request, burstComplete: true);
                };
                Assert.ThrowsException<OperationCanceledException>(() => ftp.kCmdBurstReadFile("unconfirmed.bin", data.Length, cancel));
                Assert.IsTrue(receivedAllBytes, "Byte coverage alone must not establish successful completion.");
            }
        }

        [TestMethod]
        public void CancelledTransferPacketsCannotOverwriteTheNextFile()
        {
            // Reused session zero, including old replies straddling sequence wrap.
            foreach (ushort start in new ushort[] { 20, 65534 })
            using (var link = new MAVLinkInterface())
            using (var firstCancel = new CancellationTokenSource())
            using (var nextCancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var oldData = Enumerable.Repeat((byte)0xee, 320).ToArray();
                var newData = Enumerable.Range(0, 320).Select(i => (byte)i).ToArray();
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, start);
                ftp.Progress += (file, percent) => { if (file == "old.bin" && percent > 0) firstCancel.Cancel(); };
                byte[] oldRequest = null;
                bool firstTransfer = true;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (firstTransfer)
                    {
                        oldRequest = request;
                        Reply(link, oldData, 0, 80, request);
                    }
                    else if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        Reply(link, newData, 0, 80, request);
                        Reply(link, newData, 80, 80, request);
                        Reply(link, newData, 160, 80, request);
                        // Same session and opcode, arriving after the new bytes.
                        // Neither a reply behind nor equal to the new request is valid.
                        Reply(link, oldData, 0, 80, oldRequest);
                        Reply(link, oldData, 80, 80, oldRequest);
                        // Even replies beyond the last received old sequence
                        // must stay behind the new request's reserved range.
                        Reply(link, oldData, 160, 80, oldRequest);
                        Reply(link, newData, 240, 80, request, burstComplete: true);
                    }
                    else ReadReply(link, newData, request);
                };
                Assert.ThrowsException<OperationCanceledException>(() => ftp.kCmdBurstReadFile("old.bin", oldData.Length, firstCancel));
                firstTransfer = false;
                using (var result = ftp.kCmdBurstReadFile("new.bin", newData.Length, nextCancel))
                    CollectionAssert.AreEqual(newData, result.ToArray(), "Late packets from the cancelled file must not replace new data.");
            }
        }

        [TestMethod]
        public void CancelledBurstRangesRejectLateDataAcrossInstancesAndWrap()
        {
            CheckCancelledBurstReplay(null);
        }

        [TestMethod]
        public void CancelledBurstRangesRejectLateErrorsAndEofAcrossInstancesAndWrap()
        {
            CheckCancelledBurstReplay(MAVFtp.FTPErrorCode.kErrFail);
            CheckCancelledBurstReplay(MAVFtp.FTPErrorCode.kErrEOF);
        }

        private static void CheckCancelledBurstReplay(MAVFtp.FTPErrorCode? error)
        {
            foreach (ushort start in new ushort[] { 20, 65000, 65534 })
            foreach (bool freshInstance in new[] { false, true })
            foreach (bool reopenFile in new[] { false, true })
            using (var link = new MAVLinkInterface())
            using (var firstCancel = new CancellationTokenSource())
            using (var nextCancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var oldData = Enumerable.Repeat((byte)0xee, 80 * 500).ToArray();
                var newData = Enumerable.Range(0, 640).Select(i => (byte)i).ToArray();
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, start);
                ftp.Progress += (file, percent) => { if (file == "old.bin" && percent > 0) firstCancel.Cancel(); };
                byte[] oldRequest = null;
                bool firstTransfer = true;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    Assert.AreEqual((byte)0, request[2], "Session reuse must not provide the isolation in this test.");
                    if (firstTransfer)
                    {
                        oldRequest = request;
                        Reply(link, oldData, 0, 80, request);
                    }
                    else if (request[3] == 2) // ResetSessions in the full GetFile workflow
                        Reply(link, new byte[0], 0, 0, request);
                    else if (request[3] == 4) // OpenFileRO, reusing session zero
                        Reply(link, BitConverter.GetBytes(newData.Length), 0, 4, request);
                    else if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        int distance = unchecked((ushort)(BitConverter.ToUInt16(request, 0) - BitConverter.ToUInt16(oldRequest, 0)));
                        Assert.AreEqual(reopenFile ? 1028 : 1024, distance,
                            "Cancellation must reserve the entire old burst, even across instances and wrap.");
                        for (int offset = 0; offset <= 320; offset += 80)
                            Reply(link, newData, offset, 80, request);
                        if (error.HasValue)
                        {
                            Nak(link, oldRequest, error.Value, sequence: unchecked((ushort)(start + 5)));
                            // A delayed trailing error after all 500 ArduPilot ACKs.
                            Nak(link, oldRequest, error.Value, sequence: unchecked((ushort)(start + 501)));
                            Nak(link, oldRequest, MAVFtp.FTPErrorCode.kErrFail,
                                sequence: unchecked((ushort)(start + 501)), size: 0);
                        }
                        else
                        {
                            // Previously this plausible pair overwrote the new file at 320.
                            Reply(link, oldData, 320, 80, oldRequest);
                            Reply(link, oldData, 499 * 80, 80, oldRequest, burstComplete: true);
                        }
                        for (int offset = 400; offset < newData.Length; offset += 80)
                            Reply(link, newData, offset, 80, request, burstComplete: offset == 560);
                    }
                    else ReadReply(link, newData, request);
                };
                Assert.ThrowsException<OperationCanceledException>(() => ftp.kCmdBurstReadFile("old.bin", newData.Length, firstCancel));
                firstTransfer = false;
                var next = freshInstance ? new MAVFtp(link, 7, 1) : ftp;
                using (var result = reopenFile ? next.GetFile("new.bin", nextCancel) : next.kCmdBurstReadFile("new.bin", newData.Length, nextCancel))
                    CollectionAssert.AreEqual(newData, result.ToArray(), "Delayed old packets must neither alter data nor abort the new file.");
            }
        }

        [TestMethod]
        public void ReorderedRepliesCannotMoveTheNextTransferSequenceBackwards()
        {
            foreach (ushort start in new ushort[] { 100, 65534 })
            using (var link = new MAVLinkInterface())
            using (var firstCancel = new CancellationTokenSource())
            using (var nextCancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, start);
                int progress = 0;
                ftp.Progress += (file, percent) => { if (percent > 0 && ++progress == 3) firstCancel.Cancel(); };
                bool first = true;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (first)
                    {
                        var data = new byte[400];
                        Reply(link, data, 0, 80, request);   // start+1
                        Reply(link, data, 240, 80, request); // start+4
                        Reply(link, data, 80, 80, request);  // start+2
                    }
                    else
                    {
                        Assert.AreEqual(unchecked((ushort)(start + (request[3] == 15 ? 1024 : 2048))), BitConverter.ToUInt16(request, 0));
                        Eof(link, request);
                    }
                };
                Assert.ThrowsException<OperationCanceledException>(() => ftp.kCmdBurstReadFile("reordered.bin", 400, firstCancel));
                first = false;
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("empty.bin", 0, nextCancel))
                    Assert.AreEqual(0L, result.Length);
            }
        }

        [TestMethod]
        public void UnchangedBurstRetriesReuseTheirSequenceRange()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = new byte[80];
                var requests = new System.Collections.Generic.List<byte[]>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    requests.Add(request);
                    if (requests.Count == 1) return; // Drop the first response.
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                        Reply(link, data, 0, 80, request, burstComplete: true);
                    else Eof(link, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("retry.bin", 80, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                Assert.AreEqual(4, requests.Count);
                CollectionAssert.AreEqual(requests[0], requests[1], "An unchanged retry must preserve the entire request.");
                Assert.AreEqual((ushort)1024, BitConverter.ToUInt16(requests[2], 0));
                Assert.AreEqual((ushort)2048, BitConverter.ToUInt16(requests[3], 0));
            }
        }

        [TestMethod]
        public void SequenceRangesAreSharedOnlyForTheSameLinkAndPeer()
        {
            using (var firstLink = new MAVLinkInterface())
            using (var secondLink = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var starts = new System.Collections.Generic.List<ushort>();
                foreach (var link in new[] { firstLink, secondLink })
                {
                    var wire = new CommsInjection();
                    link.BaseStream = wire;
                    wire.WriteCallback += (sender, bytes) =>
                    {
                        var packet = (MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data;
                        var request = packet.payload;
                        if (request[3] == 15) starts.Add(BitConverter.ToUInt16(request, 0));
                        var response = new byte[251];
                        BitConverter.GetBytes(unchecked((ushort)(BitConverter.ToUInt16(request, 0) + 1))).CopyTo(response, 0);
                        response[3] = 129;
                        response[4] = 1;
                        response[5] = request[3];
                        response[12] = (byte)MAVFtp.FTPErrorCode.kErrEOF;
                        DispatchPayload(link, response, packet.target_system, packet.target_component);
                    };
                }
                foreach (var peer in new[] { (firstLink, (byte)7, (byte)1), (firstLink, (byte)7, (byte)1),
                    (firstLink, (byte)7, (byte)2), (firstLink, (byte)8, (byte)1), (secondLink, (byte)7, (byte)1) })
                {
                    peer.Item1.MAVlist[peer.Item2, peer.Item3].mavlinkv2 = true;
                    using (var result = new MAVFtp(peer.Item1, peer.Item2, peer.Item3).kCmdBurstReadFile("empty.bin", 0, cancel))
                        Assert.AreEqual(0L, result.Length);
                }
                CollectionAssert.AreEqual(new ushort[] { 0, 1026, 0, 0, 0 }, starts);
            }
        }

        [TestMethod]
        public void OrdinaryRequestsKeepTheirReplySequenceAcrossWrap()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var requests = new System.Collections.Generic.List<ushort>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    requests.Add(BitConverter.ToUInt16(request, 0));
                    Reply(link, BitConverter.GetBytes(0), 0, 4, request);
                };
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, 65535);
                Assert.IsTrue(ftp.kCmdOpenFileRO("empty.bin", out var size, cancel));
                Assert.IsTrue(new MAVFtp(link, 7, 1).kCmdOpenFileRO("empty.bin", out size, cancel));
                CollectionAssert.AreEqual(new ushort[] { 65535, 1 }, requests);
            }
        }

        [TestMethod]
        public void BurstErrorsOutsideTheReservedSequenceRangeAreIgnored()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = new byte[80];
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15)
                    {
                        Nak(link, request, MAVFtp.FTPErrorCode.kErrFail,
                            sequence: unchecked((ushort)(BitConverter.ToUInt16(request, 0) + 1024)), size: 0);
                        if (BitConverter.ToUInt32(request, 8) == 0)
                        {
                            Reply(link, data, 0, 80, request, burstComplete: true);
                            return;
                        }
                    }
                    Eof(link, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("range.bin", 80, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void BurstSequenceWrapPreservesReorderedPackets()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 240).Select(i => (byte)i).ToArray();
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, (ushort)65534);
                var singleReads = new System.Collections.Generic.List<uint>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        // Replies 0, 65535, 1 are all after request 65534.
                        Reply(link, data, 80, 80, request);
                        Reply(link, data, 0, 80, request);
                        Reply(link, data, 160, 80, request, burstComplete: true);
                    }
                    else
                    {
                        if (request[3] == 5) singleReads.Add(BitConverter.ToUInt32(request, 8));
                        ReadReply(link, data, request);
                    }
                };
                using (var result = ftp.kCmdBurstReadFile("wrap.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                CollectionAssert.AreEqual(new uint[] { 240 }, singleReads,
                    "Only the EOF probe is needed; reordered current-burst data must not be discarded.");
            }
        }

        [TestMethod]
        public void EachSentBurstRejectsRepliesFromEarlierRequests()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 320).Select(i => (byte)i).ToArray();
                var staleData = Enumerable.Repeat((byte)0xee, 320).ToArray();
                byte[] firstRequest = null;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    uint offset = BitConverter.ToUInt32(request, 8);
                    if (request[3] == 15 && offset == 0)
                    {
                        firstRequest = request;
                        Reply(link, data, 0, 80, request);
                        Reply(link, data, 80, 80, request, burstComplete: true);
                    }
                    else if (request[3] == 15 && offset == 160)
                    {
                        Reply(link, data, 160, 80, request);
                        Reply(link, staleData, 0, 80, firstRequest);
                        Reply(link, data, 240, 80, request, burstComplete: true);
                    }
                    else ReadReply(link, data, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("bursts.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray(), "The sequence boundary must advance when another burst request is sent.");
            }
        }

        [TestMethod]
        public void BurstRepliesWithImpossibleSequenceAndOffsetPairsAreIgnored()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 240).Select(i => (byte)i).ToArray();
                var staleData = Enumerable.Repeat((byte)0xee, 560).ToArray();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        ushort sequence = BitConverter.ToUInt16(request, 0);
                        Reply(link, data, 0, 80, request);
                        // A second reply cannot follow more than 239 bytes,
                        // and a third reply cannot start at the burst origin.
                        Reply(link, staleData, 480, 80, request, sequence: (ushort)(sequence + 2));
                        Reply(link, staleData, 0, 80, request, sequence: (ushort)(sequence + 3));
                        Reply(link, data, 80, 80, request);
                        Reply(link, data, 160, 80, request, burstComplete: true);
                    }
                    else ReadReply(link, data, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("offsets.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void LegacyPx4BurstChunksMayExceedTheRequestedSize()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 600).Select(i => (byte)i).ToArray();
                var singleReads = new System.Collections.Generic.List<uint>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        ushort sequence = BitConverter.ToUInt16(request, 0);
                        // Older PX4 ignores the requested 80-byte burst size.
                        Reply(link, data, 0, 239, request, sequence: (ushort)(sequence + 1));
                        Reply(link, data, 239, 239, request, sequence: (ushort)(sequence + 2));
                        Reply(link, data, 478, 122, request, burstComplete: true, sequence: (ushort)(sequence + 3));
                    }
                    else
                    {
                        if (request[3] == 5) singleReads.Add(BitConverter.ToUInt32(request, 8));
                        ReadReply(link, data, request);
                    }
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("px4.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                CollectionAssert.AreEqual(new uint[] { 600 }, singleReads, "Valid legacy PX4 data must not require gap recovery.");
            }
        }

        [TestMethod]
        public void VariableBurstChunksAllowReorderingWrapAndALostFirstPacket()
        {
            foreach (bool loseFirst in new[] { false, true })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 400).Select(i => (byte)i).ToArray();
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, (ushort)65534);
                var offsets = new[] { 0, 80, 119, 199, 216, 296, 376 };
                var sizes = new[] { 80, 39, 80, 17, 80, 80, 24 };
                var singleReads = new System.Collections.Generic.List<uint>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15 && BitConverter.ToUInt32(request, 8) == 0)
                    {
                        ushort sequence = BitConverter.ToUInt16(request, 0);
                        foreach (int i in new[] { 4, 1, 3, 0, 5, 2, 6 })
                        {
                            if (loseFirst && i == 0) continue;
                            Reply(link, data, offsets[i], sizes[i], request, burstComplete: i == 6,
                                sequence: (ushort)(sequence + 1 + i));
                        }
                    }
                    else
                    {
                        if (request[3] == 5) singleReads.Add(BitConverter.ToUInt32(request, 8));
                        ReadReply(link, data, request);
                    }
                };
                using (var result = ftp.kCmdBurstReadFile("variable.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                CollectionAssert.AreEqual(loseFirst ? new uint[] { 0, 400 } : new uint[] { 400 }, singleReads,
                    "Variable-sized and reordered packets should only need recovery for the actual missing bytes.");
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
                        Reply(link, data, offset, 40, request, burstComplete: offset == 120,
                            sequence: (ushort)(BitConverter.ToUInt16(request, 0) + 1 + offset / 40));
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
        public void StaleBurstErrorsAreIgnoredAcrossSequenceWrap()
        {
            foreach (ushort start in new ushort[] { 20, 0, 65535 })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, start);
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15)
                    {
                        ushort sequence = BitConverter.ToUInt16(request, 0);
                        Nak(link, request, MAVFtp.FTPErrorCode.kErrInvalidSession, (ushort)(sequence - 1));
                        Nak(link, request, MAVFtp.FTPErrorCode.kErrInvalidSession, sequence);
                        // Stale malformed replies must be filtered before decoding.
                        Nak(link, request, MAVFtp.FTPErrorCode.kErrFail, sequence, size: 0);
                        if (BitConverter.ToUInt32(request, 8) == 0)
                        {
                            Reply(link, data, 0, 80, request);
                            Reply(link, data, 80, 80, request, burstComplete: true);
                            return;
                        }
                    }
                    ReadReply(link, data, request);
                };
                using (var result = ftp.kCmdBurstReadFile("stale-errors.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
            }
        }

        [TestMethod]
        public void StaleBurstEofDoesNotStartRecovery()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var data = Enumerable.Range(0, 160).Select(i => (byte)i).ToArray();
                var singleReads = new System.Collections.Generic.List<uint>();
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    if (request[3] == 15)
                    {
                        ushort sequence = BitConverter.ToUInt16(request, 0);
                        Eof(link, request, (ushort)(sequence - 1));
                        Eof(link, request, sequence);
                        if (BitConverter.ToUInt32(request, 8) == 0)
                        {
                            Reply(link, data, 0, 80, request);
                            Reply(link, data, 80, 80, request, burstComplete: true);
                            return;
                        }
                    }
                    if (request[3] == 5) singleReads.Add(BitConverter.ToUInt32(request, 8));
                    ReadReply(link, data, request);
                };
                using (var result = new MAVFtp(link, 7, 1).kCmdBurstReadFile("stale-eof.bin", data.Length, cancel))
                    CollectionAssert.AreEqual(data, result.ToArray());
                CollectionAssert.AreEqual(new uint[] { 160 }, singleReads,
                    "Stale burst EOF must not discard current burst data or start recovery at offset zero.");
            }
        }

        [TestMethod]
        public void CurrentBurstErrorsFailWithoutRetryingOrReturningPartialData()
        {
            foreach (var error in new[] { MAVFtp.FTPErrorCode.kErrInvalidSession, MAVFtp.FTPErrorCode.kErrFail, MAVFtp.FTPErrorCode.kErrFileNotFound })
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                var ftp = new MAVFtp(link, 7, 1);
                SetNextSequence(ftp, (ushort)65535);
                int requests = 0;
                bool reportedComplete = false;
                ftp.Progress += (file, percent) => { if (percent == 100) reportedComplete = true; };
                wire.WriteCallback += (sender, bytes) =>
                {
                    requests++;
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    // The first reply wraps to zero, then a current burst error
                    // follows it at sequence one, beyond request+1.
                    Reply(link, new byte[80], 0, 80, request);
                    Nak(link, request, error, (ushort)(BitConverter.ToUInt16(request, 0) + 2));
                };
                var exception = Assert.ThrowsException<IOException>(() => ftp.kCmdBurstReadFile("failed.bin", 160, cancel));
                StringAssert.Contains(exception.Message, error.ToString());
                Assert.AreEqual(1, requests, "A current server error must not be retried or masked.");
                Assert.IsFalse(reportedComplete, "A failed partial transfer must not report completion.");
            }
        }

        [TestMethod]
        public void CurrentEmptyBurstNakIsReportedAsMalformed()
        {
            using (var link = new MAVLinkInterface())
            using (var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
            {
                var wire = new CommsInjection();
                link.BaseStream = wire;
                link.MAVlist[7, 1].mavlinkv2 = true;
                wire.WriteCallback += (sender, bytes) =>
                {
                    var request = ((MAVLink.mavlink_file_transfer_protocol_t)new MAVLink.MAVLinkMessage { buffer = bytes.ToArray() }.data).payload;
                    Nak(link, request, MAVFtp.FTPErrorCode.kErrFail, size: 0);
                };
                Assert.ThrowsException<InvalidDataException>(() => new MAVFtp(link, 7, 1).kCmdBurstReadFile("malformed.bin", 160, cancel));
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
