using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Comms;
using Xunit;

namespace MissionPlanner.ArduPilot.Tests
{
    public class GetLogTests
    {
        const byte VehicleSysid = 1;
        const byte VehicleCompid = 1;
        const ushort LogId = 3;
        const int BlockSize = 90;

        /// <summary>
        /// Simulated vehicle end of the MAVLink log download protocol, connected
        /// to a MAVLinkInterface through an in-memory CommsInjection link.
        /// </summary>
        sealed class FakeLogVehicle : IDisposable
        {
            public readonly CommsInjection Link = new CommsInjection();
            public readonly MAVLinkInterface Mav = new MAVLinkInterface();
            public readonly List<MAVLink.mavlink_log_request_data_t> Requests = new List<MAVLink.mavlink_log_request_data_t>();
            public Action<MAVLink.mavlink_log_request_data_t> OnRequest;
            int _endRequests;

            public int EndRequests => Volatile.Read(ref _endRequests);

            readonly MAVLink.MavlinkParse _parse = new MAVLink.MavlinkParse();
            readonly byte[] _log;
            readonly CancellationTokenSource _stop = new CancellationTokenSource();
            readonly Task _pump;

            public FakeLogVehicle(byte[] log)
            {
                _log = log;

                Link.WriteCallback += (sender, outbytes) =>
                {
                    MAVLink.MAVLinkMessage msg;
                    try
                    {
                        msg = new MAVLink.MAVLinkMessage(outbytes.ToArray());
                    }
                    catch
                    {
                        return;
                    }

                    if (msg.msgid == (uint)MAVLink.MAVLINK_MSG_ID.LOG_REQUEST_END)
                    {
                        Interlocked.Increment(ref _endRequests);
                        return;
                    }

                    if (msg.msgid != (uint)MAVLink.MAVLINK_MSG_ID.LOG_REQUEST_DATA)
                        return;

                    var req = msg.ToStructure<MAVLink.mavlink_log_request_data_t>();
                    if (req.id != LogId)
                        return;

                    lock (Requests)
                        Requests.Add(req);
                    OnRequest?.Invoke(req);
                };

                Mav.BaseStream = Link;

                // real-time protocol timeouts scaled down so the suite stays fast
                Mav.LogDataTimeoutMs = 500;
                Mav.LogDataRefetchMs = 100;

                // GetLog consumes packets via OnPacketReceived, which fires from the
                // receive loop - pump it the same way MainV2.SerialReader does
                _pump = Task.Run(async () =>
                {
                    while (!_stop.IsCancellationRequested)
                    {
                        try
                        {
                            await Mav.readPacketAsync().ConfigureAwait(false);
                        }
                        catch
                        {
                            try
                            {
                                await Task.Delay(1, _stop.Token).ConfigureAwait(false);
                            }
                            catch (OperationCanceledException)
                            {
                            }
                        }
                    }
                });
            }

            public void SendBlock(uint ofs, int count)
            {
                var payload = new byte[BlockSize];
                Array.Copy(_log, ofs, payload, 0, count);
                Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA, new MAVLink.mavlink_log_data_t(ofs, LogId, (byte)count, payload));
            }

            public void SendEndMarker(uint ofs)
            {
                Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA, new MAVLink.mavlink_log_data_t(ofs, LogId, 0, new byte[BlockSize]));
            }

            public void Send(MAVLink.MAVLINK_MSG_ID msgid, object indata)
            {
                Link.AppendBuffer(_parse.GenerateMAVLinkPacket20(msgid, indata, false, VehicleSysid, VehicleCompid));
            }

            /// <summary>
            /// Serve a LOG_REQUEST_DATA the way ArduPilot does: 90 byte blocks with
            /// a short block at end of log, or a zero count marker when the log ends
            /// on an exact block boundary. skipBlocks are dropped, but only the
            /// first time each is requested.
            /// </summary>
            public void Serve(MAVLink.mavlink_log_request_data_t req, HashSet<uint> skipBlocks = null)
            {
                var end = Math.Min((ulong)req.ofs + req.count, (ulong)_log.Length);
                for (var ofs = (ulong)req.ofs; ofs < end; ofs += BlockSize)
                {
                    var block = (uint)(ofs / BlockSize);
                    if (skipBlocks != null && skipBlocks.Remove(block))
                        continue;
                    SendBlock((uint)ofs, (int)Math.Min(BlockSize, end - ofs));
                }

                if (end == (ulong)_log.Length && (ulong)req.ofs + req.count > end && _log.Length % BlockSize == 0)
                    SendEndMarker((uint)end);
            }

            public void Dispose()
            {
                _stop.Cancel();
                Link.Close();
                try
                {
                    _pump.Wait(2000);
                }
                catch
                {
                }
            }
        }

        static byte[] MakeLog(int length)
        {
            var data = new byte[length];
            new Random(42).NextBytes(data);
            return data;
        }

        /// <summary>A private directory owned by a single test, removed on dispose.</summary>
        sealed class TestDir : IDisposable
        {
            public string Root { get; } =
                Path.Combine(Path.GetTempPath(), "GetLogTests-" + Guid.NewGuid().ToString("N"));

            public TestDir()
            {
                Directory.CreateDirectory(Root);
            }

            public string File(string name) => Path.Combine(Root, name);

            public void Dispose()
            {
                try
                {
                    Directory.Delete(Root, true);
                }
                catch
                {
                }
            }
        }

        static async Task<byte[]> Download(FakeLogVehicle vehicle, CancellationToken cancel)
        {
            using (var dir = new TestDir())
            {
                var file = await vehicle.Mav.GetLogInternal(VehicleSysid, VehicleCompid, LogId,
                    dir.File("log.bin"), cancel);
                return File.ReadAllBytes(file);
            }
        }

        [Fact]
        public async Task DownloadsCompleteLogInOrder()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                vehicle.OnRequest = req => vehicle.Serve(req);

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                Assert.True(vehicle.EndRequests > 0,
                    "LOG_REQUEST_END not sent after a completed download");
            }
        }

        [Fact]
        public async Task StaleDuplicatesDoNotMultiplyFillRequests()
        {
            var log = MakeLog(1000);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var drop = new HashSet<uint> { 3 };
                var fillRequests = 0;
                vehicle.OnRequest = req =>
                {
                    if (Interlocked.Increment(ref fillRequests) > 1)
                    {
                        // before serving the gap, flood stale duplicates of blocks the
                        // download already holds - they must not each trigger another
                        // fill request on a duplicating link
                        for (var i = 0; i < 30; i++)
                        {
                            vehicle.SendBlock(8 * BlockSize, BlockSize);
                            vehicle.SendBlock(9 * BlockSize, BlockSize);
                        }
                    }

                    vehicle.Serve(req, drop);
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                lock (vehicle.Requests)
                    Assert.True(vehicle.Requests.Count <= 8,
                        $"{vehicle.Requests.Count} requests for one dropped block - stale " +
                        "duplicates are multiplying fill requests");
            }
        }

        [Fact]
        public async Task CorruptFarOffsetDoesNotBreakEndDetection()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var corruptSent = false;
                vehicle.OnRequest = req =>
                {
                    var end = Math.Min((ulong)req.ofs + req.count, (ulong)log.Length);
                    for (var ofs = (ulong)req.ofs; ofs < end; ofs += BlockSize)
                    {
                        // a corrupt-but-valid-looking packet at a far offset must neither
                        // poison end-of-log detection nor lengthen the returned file
                        if (ofs == 5 * BlockSize && !corruptSent)
                        {
                            corruptSent = true;
                            vehicle.Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA,
                                new MAVLink.mavlink_log_data_t(1_000_000, LogId, BlockSize,
                                    new byte[BlockSize]));
                        }

                        vehicle.SendBlock((uint)ofs, (int)Math.Min(BlockSize, end - ofs));
                    }
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.True(result.Length == log.Length,
                    $"corrupt far-offset packet changed the file length: {result.Length} != {log.Length}");
                Assert.Equal(log, result);
            }
        }

        [Fact]
        public async Task CorruptShortFarPacketDoesNotEndTheDownload()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var corruptSent = false;
                vehicle.OnRequest = req =>
                {
                    var end = Math.Min((ulong)req.ofs + req.count, (ulong)log.Length);
                    for (var ofs = (ulong)req.ofs; ofs < end; ofs += BlockSize)
                    {
                        // a packet that is both short and at a far offset clears the
                        // end-of-log bar trivially - it must not terminate the stream
                        // at a phantom length
                        if (ofs == 5 * BlockSize && !corruptSent)
                        {
                            corruptSent = true;
                            vehicle.Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA,
                                new MAVLink.mavlink_log_data_t(1_000_000, LogId, 40,
                                    new byte[BlockSize]));
                        }

                        vehicle.SendBlock((uint)ofs, (int)Math.Min(BlockSize, end - ofs));
                    }
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.True(result.Length == log.Length,
                    $"corrupt short far packet ended the download at a phantom length: " +
                    $"{result.Length} != {log.Length}");
                Assert.Equal(log, result);
            }
        }

        [Fact]
        public async Task CorruptFarPacketDoesNotHijackRetryOffset()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var first = true;
                vehicle.OnRequest = req =>
                {
                    if (!first)
                    {
                        vehicle.Serve(req);
                        return;
                    }

                    // stream five blocks, inject a corrupt far packet, then go silent -
                    // the streaming retry must resume from the true stream position,
                    // not from past the corrupt offset
                    first = false;
                    for (var ofs = 0u; ofs < 5 * BlockSize; ofs += BlockSize)
                        vehicle.SendBlock(ofs, BlockSize);
                    vehicle.Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA,
                        new MAVLink.mavlink_log_data_t(1_000_000, LogId, BlockSize,
                            new byte[BlockSize]));
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                lock (vehicle.Requests)
                {
                    Assert.True(vehicle.Requests.Any(r => r.ofs == 5 * BlockSize),
                        "retry did not resume from the true stream position");
                    Assert.False(vehicle.Requests.Any(r => r.ofs > (uint)log.Length),
                        "retry requested data past the corrupt offset");
                }
            }
        }

        [Fact]
        public async Task RepeatedStaleDataDoesNotKeepTheStreamAliveForever()
        {
            var log = MakeLog(1000);
            using (var vehicle = new FakeLogVehicle(log))
            using (var dir = new TestDir())
            {
                var first = true;
                vehicle.OnRequest = req =>
                {
                    if (first)
                    {
                        // stream only the first three blocks, then answer every retry
                        // with a stale duplicate - it brings no new data, so it must
                        // not keep resetting the retry budget forever
                        first = false;
                        for (var ofs = 0u; ofs < 3 * BlockSize; ofs += BlockSize)
                            vehicle.SendBlock(ofs, BlockSize);
                        return;
                    }

                    vehicle.SendBlock(0, BlockSize);
                };

                using (var guard = new CancellationTokenSource(30000))
                using (var linked = CancellationTokenSource.CreateLinkedTokenSource(
                    guard.Token, TestContext.Current.CancellationToken))
                {
                    var path = dir.File("log.bin");

                    await Assert.ThrowsAsync<TimeoutException>(
                        () => vehicle.Mav.GetLogInternal(VehicleSysid, VehicleCompid, LogId,
                            path, linked.Token));

                    Assert.False(guard.IsCancellationRequested,
                        "download looped instead of timing out");
                }
            }
        }

        [Fact]
        public async Task EndMarkerBeyondStalledFrontierCompletesViaFillin()
        {
            // an early drop stalls the contiguous frontier while the log is large
            // enough that the genuine end packet sits far past its trust window -
            // the end must still hand over to the fill-in phase, not force the
            // whole stream to be sent again
            var log = MakeLog(12000);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var drop = new HashSet<uint> { 3 };
                vehicle.OnRequest = req => vehicle.Serve(req, drop);

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                lock (vehicle.Requests)
                    Assert.True(vehicle.Requests.Count <= 4,
                        $"{vehicle.Requests.Count} requests for one dropped block - the " +
                        "far end marker is forcing full re-streams");
            }
        }

        [Fact]
        public async Task IgnoresPacketWithOversizedCount()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                vehicle.OnRequest = req =>
                {
                    // count larger than the 90-byte payload array must be skipped,
                    // not abort the download
                    vehicle.Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA,
                        new MAVLink.mavlink_log_data_t(0, LogId, 200, new byte[BlockSize]));
                    vehicle.Serve(req);
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
            }
        }

        [Fact]
        public async Task BogusFillinOffsetDoesNotExtendFile()
        {
            var log = MakeLog(1000);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var drop = new HashSet<uint> { 3 };
                var bogusSent = false;
                vehicle.OnRequest = req =>
                {
                    if (req.ofs == 3 * BlockSize && !bogusSent)
                    {
                        // a fill-in response beyond the end of the log must not
                        // grow the file past the length the end marker established
                        bogusSent = true;
                        vehicle.Send(MAVLink.MAVLINK_MSG_ID.LOG_DATA,
                            new MAVLink.mavlink_log_data_t((uint)(log.Length + 10 * BlockSize),
                                LogId, BlockSize, new byte[BlockSize]));
                    }

                    vehicle.Serve(req, drop);
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.True(result.Length == log.Length,
                    $"bogus fill-in offset changed the file length: {result.Length} != {log.Length}");
                Assert.Equal(log, result);
            }
        }

        [Fact]
        public async Task FillinTimesOutWhenVehicleGoesSilent()
        {
            var log = MakeLog(1000);
            using (var vehicle = new FakeLogVehicle(log))
            using (var dir = new TestDir())
            {
                // serve the streaming phase with a block missing, then never answer
                // the fill-in requests - the download must fail, not hang forever
                var first = true;
                vehicle.OnRequest = req =>
                {
                    if (!first)
                        return;
                    first = false;
                    vehicle.Serve(req, new HashSet<uint> { 3 });
                };

                var path = dir.File("log.bin");

                await Assert.ThrowsAsync<TimeoutException>(
                    () => vehicle.Mav.GetLogInternal(VehicleSysid, VehicleCompid, LogId, path,
                        TestContext.Current.CancellationToken));

                Assert.False(File.Exists(path), "partial file left behind after fill-in timeout");
            }
        }

        [Fact]
        public async Task HandlesLogSizeExactMultipleOfBlockSize()
        {
            var log = MakeLog(900);
            using (var vehicle = new FakeLogVehicle(log))
            {
                vehicle.OnRequest = req => vehicle.Serve(req);

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                // the fill in phase must not ask for a phantom block past end of log
                lock (vehicle.Requests)
                    Assert.False(vehicle.Requests.Skip(1).Any(r => r.ofs >= 900),
                        "requested data past the end of the log");
            }
        }

        [Fact]
        public async Task RefetchesDroppedBlocks()
        {
            var log = MakeLog(1000);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var drop = new HashSet<uint> { 3, 7 };
                vehicle.OnRequest = req => vehicle.Serve(req, drop);

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Equal(log, result);
                lock (vehicle.Requests)
                    Assert.True(vehicle.Requests.Any(r => r.ofs == 3 * BlockSize),
                        "first dropped block was never re-requested");
            }
        }

        [Fact]
        public async Task IgnoresStrayShortPacketBeforeEndOfLog()
        {
            var log = MakeLog(1234);
            using (var vehicle = new FakeLogVehicle(log))
            {
                var first = true;
                vehicle.OnRequest = req =>
                {
                    if (!first)
                    {
                        vehicle.Serve(req);
                        return;
                    }

                    first = false;
                    var end = Math.Min((ulong)req.ofs + req.count, (ulong)log.Length);
                    for (var ofs = (ulong)req.ofs; ofs < end; ofs += BlockSize)
                    {
                        // a stale short retransmit mid stream must not truncate the log
                        if (ofs == 8 * BlockSize)
                            vehicle.SendBlock(3 * BlockSize, 40);
                        vehicle.SendBlock((uint)ofs, (int)Math.Min(BlockSize, end - ofs));
                    }
                };

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.True(log.Length == result.Length, "stray short packet truncated the download");
                Assert.Equal(log, result);
            }
        }

        [Fact]
        public async Task EmptyLogProducesEmptyFile()
        {
            using (var vehicle = new FakeLogVehicle(new byte[0]))
            {
                vehicle.OnRequest = req => vehicle.SendEndMarker(0);

                var result = await Download(vehicle, TestContext.Current.CancellationToken);

                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task CancelStopsDownloadAndDeletesFile()
        {
            var log = MakeLog(100 * BlockSize);
            using (var vehicle = new FakeLogVehicle(log))
            using (var dir = new TestDir())
            {
                // stream a few blocks, then go silent so the download hangs mid way
                vehicle.OnRequest = req =>
                {
                    for (uint block = 0; block < 5; block++)
                        vehicle.SendBlock(block * BlockSize, BlockSize);
                };

                var path = dir.File("log.bin");

                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(TestContext.Current.CancellationToken))
                {
                    cts.CancelAfter(300);
                    await Assert.ThrowsAnyAsync<OperationCanceledException>(
                        () => vehicle.Mav.GetLogInternal(VehicleSysid, VehicleCompid, LogId, path, cts.Token));
                }

                Assert.False(File.Exists(path), "partial file left behind after cancel");
                Assert.True(vehicle.EndRequests > 0,
                    "LOG_REQUEST_END not sent - the vehicle would keep streaming LOG_DATA");
            }
        }

        [Fact(Timeout = 15000)]
        public async Task TimesOutWhenVehicleNeverResponds()
        {
            using (var vehicle = new FakeLogVehicle(MakeLog(10)))
            using (var dir = new TestDir())
            {
                var path = dir.File("log.bin");

                // no OnRequest wired - total silence; LogDataTimeoutMs with 3 retries
                await Assert.ThrowsAsync<TimeoutException>(
                    () => vehicle.Mav.GetLogInternal(VehicleSysid, VehicleCompid, LogId, path,
                        TestContext.Current.CancellationToken));

                Assert.False(File.Exists(path), "partial file left behind after timeout");
            }
        }
    }
}
