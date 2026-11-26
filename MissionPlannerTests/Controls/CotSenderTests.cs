using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Controls;

namespace MissionPlannerTests.Controls
{
    [TestClass]
    public class CotSenderTests
    {
        /// <summary>
        /// A transport whose sends block, like a TCP client that stopped reading. Closing it makes a
        /// blocked send fail, as closing a socket does, unless it ignores being closed.
        /// </summary>
        sealed class BlockingTransport : ICotTransport
        {
            readonly bool ignoresClose;
            readonly ManualResetEventSlim closed = new ManualResetEventSlim();
            public readonly ManualResetEventSlim Release = new ManualResetEventSlim();
            public readonly ManualResetEventSlim Sending = new ManualResetEventSlim();
            public int Opens, Sends, SendsAfterClose;

            public BlockingTransport(bool ignoresClose = false)
            {
                this.ignoresClose = ignoresClose;
            }

            public bool Closed => closed.IsSet;

            public void Open()
            {
                Interlocked.Increment(ref Opens);
            }

            public bool Send(byte[] message)
            {
                Interlocked.Increment(ref Sends);
                if (Closed)
                {
                    Interlocked.Increment(ref SendsAfterClose);
                    throw new ObjectDisposedException("transport");
                }

                Sending.Set();
                if (ignoresClose)
                    Release.Wait();
                else
                    WaitHandle.WaitAny(new[] { closed.WaitHandle, Release.WaitHandle });
                if (Closed && !ignoresClose)
                    throw new ObjectDisposedException("transport");
                return true;
            }

            public string Status => "test";

            public void Dispose()
            {
                closed.Set();
            }
        }

        static IEnumerable<CotVehicle> OneVehicle()
        {
            return new[]
            {
                new CotVehicle
                {
                    Connection = "test", SysId = 1, CompId = 1, Type = MAVLink.MAV_TYPE.QUADROTOR,
                    Autopilot = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA, Lat = -35.36, Lng = 149.16, AltMslM = 600,
                    LastPacketUtc = DateTime.UtcNow
                }
            };
        }

        static CotSettings Settings() => new CotSettings { IntervalSeconds = 1 };

        [TestCleanup]
        public void StopAll()
        {
            CotSender.Current?.Stop(TimeSpan.FromSeconds(2));
        }

        [TestMethod]
        public void StopInterruptsABlockedSendAndReleasesTheTransport()
        {
            var transport = new BlockingTransport();
            var sender = CotSender.Start(transport, Settings(), OneVehicle);
            Assert.IsTrue(transport.Sending.Wait(TimeSpan.FromSeconds(10)), "the sender sends");

            var timer = Stopwatch.StartNew();
            Assert.IsTrue(sender.Stop(TimeSpan.FromSeconds(1)), "stopped");
            Assert.IsTrue(timer.Elapsed < TimeSpan.FromSeconds(3), "within the waits: " + timer.Elapsed);

            Assert.IsNull(CotSender.Current);
            Assert.IsTrue(transport.Closed, "the transport is closed");
            // the endings are not sent into a transport closed under the run
            Assert.AreEqual(0, transport.SendsAfterClose);
            int sends = transport.Sends;
            Thread.Sleep(1500);
            Assert.AreEqual(sends, transport.Sends, "nothing is sent after Stop");

            // and another can start, on a transport of its own
            var next = new BlockingTransport();
            var started = CotSender.Start(next, Settings(), OneVehicle);
            Assert.AreSame(started, CotSender.Current);
            Assert.IsTrue(next.Sending.Wait(TimeSpan.FromSeconds(10)));
            next.Release.Set();
            Assert.IsTrue(started.Stop(TimeSpan.FromSeconds(5)));
            Assert.IsTrue(next.Closed);
        }

        [TestMethod]
        public void ASenderThatDoesNotStopIsNotReplaced()
        {
            var stuck = new BlockingTransport(ignoresClose: true);
            var sender = CotSender.Start(stuck, Settings(), OneVehicle);
            Assert.IsTrue(stuck.Sending.Wait(TimeSpan.FromSeconds(10)));

            Assert.IsFalse(sender.Stop(TimeSpan.FromMilliseconds(300)), "the run is still in its send");
            Assert.AreSame(sender, CotSender.Current, "it stays the output");
            Assert.IsTrue(stuck.Closed, "its transport was closed to end the send");

            var other = new BlockingTransport();
            Assert.ThrowsException<InvalidOperationException>(() =>
                CotSender.Start(other, Settings(), OneVehicle));
            Assert.AreEqual(0, other.Opens, "nothing started beside it");
            Assert.AreSame(sender, CotSender.Current);

            // once the send returns, Stop again ends it
            stuck.Release.Set();
            Assert.IsTrue(sender.Stop(TimeSpan.FromSeconds(5)));
            Assert.IsNull(CotSender.Current);
        }

        [TestMethod]
        public void StopSendsTheEndingsWhenTheTransportIsFine()
        {
            var transport = new BlockingTransport();
            transport.Release.Set();
            var sender = CotSender.Start(transport, Settings(), OneVehicle);
            Assert.IsTrue(transport.Sending.Wait(TimeSpan.FromSeconds(10)));
            Thread.Sleep(200);
            int before = transport.Sends;

            Assert.IsTrue(sender.Stop(TimeSpan.FromSeconds(5)));
            // the track's ending, twice
            Assert.AreEqual(before + 2, transport.Sends);
            Assert.AreEqual(0, transport.SendsAfterClose);
            Assert.IsTrue(transport.Closed);
            Assert.IsNull(CotSender.Current);
        }
    }
}
