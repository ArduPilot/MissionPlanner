using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MissionPlanner.Controls.Tests
{
    [TestClass]
    public class CotSenderTests
    {
        class FakeTransport : ICotTransport
        {
            public readonly List<string> Messages = new List<string>();
            public Exception OpenFails;
            public bool Disposed;

            public void Open()
            {
                if (OpenFails != null)
                    throw OpenFails;
            }

            public bool Send(byte[] message)
            {
                lock (Messages)
                    Messages.Add(Encoding.UTF8.GetString(message));
                return true;
            }

            public int Count
            {
                get
                {
                    lock (Messages)
                        return Messages.Count;
                }
            }

            public string Status
            {
                get { return "fake"; }
            }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        static IEnumerable<CotVehicle> OneCopter()
        {
            return new[]
            {
                new CotVehicle
                {
                    Connection = "link", SysId = 1, CompId = 1, Autopilot = MAVLink.MAV_AUTOPILOT.ARDUPILOTMEGA,
                    Lat = -35.363262, Lng = 149.165237, AltMslM = 684, LastPacketUtc = DateTime.UtcNow
                }
            };
        }

        static CotSettings EverySecond()
        {
            return new CotSettings
            {
                IntervalSeconds = 1,
                Rows = new List<CotRow> { new CotRow { SysId = "1", Uid = "MP-UAS-1", Callsign = "UAS-1" } }
            };
        }

        static void WaitFor(Func<bool> condition, int ms = 5000)
        {
            var end = DateTime.UtcNow.AddMilliseconds(ms);
            while (!condition() && DateTime.UtcNow < end)
                Thread.Sleep(20);
        }

        [TestCleanup]
        public void StopAny()
        {
            var running = CotSender.Current;
            if (running != null)
                running.Stop();
        }

        [TestMethod]
        public void StopThenStart_InsideOneInterval_LeavesOneSender()
        {
            var first = new FakeTransport();
            var a = CotSender.Start(first, EverySecond(), OneCopter);
            WaitFor(() => first.Count >= 1);

            a.Stop();
            Assert.IsNull(CotSender.Current);
            Assert.IsTrue(first.Disposed);
            var firstTotal = first.Count;
            Assert.AreEqual(3, firstTotal, "one round, then the track's ending twice");
            StringAssert.Contains(first.Messages[1], "uid=\"MP-UAS-1\"");

            var second = new FakeTransport();
            var b = CotSender.Start(second, EverySecond(), OneCopter);
            Assert.AreSame(b, CotSender.Current);
            Thread.Sleep(2600);

            Assert.AreEqual(firstTotal, first.Count, "the stopped sender stays stopped");
            Assert.IsTrue(second.Count >= 2 && second.Count <= 4,
                "one message a second, not two: " + second.Count);
        }

        [TestMethod]
        public void Starting_StopsTheOneRunning()
        {
            var first = new FakeTransport();
            CotSender.Start(first, EverySecond(), OneCopter);
            WaitFor(() => first.Count >= 1);

            var second = new FakeTransport();
            CotSender.Start(second, EverySecond(), OneCopter);

            Assert.IsTrue(first.Disposed);
            var firstTotal = first.Count;
            Thread.Sleep(1500);
            Assert.AreEqual(firstTotal, first.Count);
        }

        [TestMethod]
        public void Stop_SendsTheTracksAlreadyStale()
        {
            var transport = new FakeTransport();
            var sender = CotSender.Start(transport, EverySecond(), OneCopter);
            WaitFor(() => transport.Count >= 1);

            sender.Stop();

            var last = transport.Messages.Last();
            var time = Attribute(last, "time");
            Assert.AreEqual(time, Attribute(last, "stale"));
            Assert.IsTrue(last.EndsWith("</event>\n"), "a line feed ends each message");
            Assert.IsFalse(last.Contains("\r"));
        }

        [TestMethod]
        public void Stop_EndsEveryVehicleTwice_HalfASecondApart_AfterItsLastUpdate()
        {
            var transport = new FakeTransport();
            Func<IEnumerable<CotVehicle>> two = () => OneCopter().Concat(OneCopter().Select(v =>
            {
                v.SysId = 2;
                return v;
            })).ToList();
            var settings = EverySecond();
            settings.Rows.Add(new CotRow { SysId = "2", Uid = "MP-UAS-2" });

            var sender = CotSender.Start(transport, settings, two);
            WaitFor(() => transport.Count >= 2);
            var stopping = DateTime.UtcNow;
            sender.Stop();
            var took = DateTime.UtcNow - stopping;

            var messages = transport.Messages.ToList();
            foreach (var uid in new[] { "MP-UAS-1", "MP-UAS-2" })
            {
                var mine = messages.Where(m => m.Contains("uid=\"" + uid + "\"")).ToList();
                Assert.AreEqual(2, mine.Count(m => Attribute(m, "time") == Attribute(m, "stale")), uid + " ends twice");
                Assert.IsTrue(mine.Skip(mine.Count - 2).All(m => Attribute(m, "time") == Attribute(m, "stale")),
                    uid + " ends after its last update");
            }

            Assert.IsTrue(took.TotalSeconds >= 0.45 && took.TotalSeconds < 3, "the second ending half a second later: " + took);
        }

        [TestMethod]
        public void NewSettings_ReachTheNextRound()
        {
            var transport = new FakeTransport();
            var sender = CotSender.Start(transport, EverySecond(), OneCopter);
            WaitFor(() => transport.Count >= 1);

            var renamed = EverySecond();
            renamed.Rows[0].Uid = "RENAMED";
            sender.Settings = renamed;
            WaitFor(() => transport.Messages.ToList().Any(m => m.Contains("uid=\"RENAMED\"")));

            Assert.IsTrue(transport.Messages.ToList().Any(m => m.Contains("uid=\"RENAMED\"")));
        }

        [TestMethod]
        public void AFailedStart_SaysWhyAndStops()
        {
            var transport = new FakeTransport { OpenFails = new InvalidOperationException("port 14551 is in use by another program") };
            var sender = CotSender.Start(transport, EverySecond(), OneCopter);
            WaitFor(() => CotSender.Current == null);

            Assert.IsNull(CotSender.Current);
            StringAssert.Contains(sender.Status, "port 14551 is in use by another program");
            Assert.AreEqual(0, transport.Count);
        }

        [TestMethod]
        public void Identity_FromTheRowWithThatSysid()
        {
            var settings = new CotSettings
            {
                Rows = new List<CotRow>
                {
                    new CotRow { SysId = "2", Uid = "TWO" },
                    new CotRow { SysId = "1", Uid = "ONE", Callsign = "UAS-1", Takv = true }
                }
            };

            var one = settings.IdentityFor(new CotVehicle { SysId = 1 });
            Assert.AreEqual("ONE", one.Uid);
            Assert.AreEqual("UAS-1", one.Callsign);
            Assert.IsTrue(one.Takv);
            Assert.AreEqual("NOsysid3", settings.IdentityFor(new CotVehicle { SysId = 3 }).Uid);
        }

        [TestMethod]
        public void Identity_TypeFromTheRow_ElseTheSettings_AutoByKind()
        {
            var settings = new CotSettings
            {
                Type = CotSettings.Auto,
                Rows = new List<CotRow>
                {
                    new CotRow { SysId = "1", Uid = "PLANE" },
                    new CotRow { SysId = "2", Uid = "ROVER", Type = "a-f-G-U-C" },
                    new CotRow { SysId = "3", Uid = "COPTER", Type = "AUTO" }
                }
            };

            Assert.AreEqual("a-f-A-M-F-Q", settings.IdentityFor(new CotVehicle { SysId = 1, Type = MAVLink.MAV_TYPE.FIXED_WING }).Type);
            Assert.AreEqual("a-f-G-U-C", settings.IdentityFor(new CotVehicle { SysId = 2, Type = MAVLink.MAV_TYPE.GROUND_ROVER }).Type);
            Assert.AreEqual("a-f-A-M-H-Q", settings.IdentityFor(new CotVehicle { SysId = 3, Type = MAVLink.MAV_TYPE.QUADROTOR }).Type);

            settings.Type = "a-f-A-M-F-Q";
            Assert.AreEqual("a-f-A-M-F-Q", settings.IdentityFor(new CotVehicle { SysId = 9, Type = MAVLink.MAV_TYPE.QUADROTOR }).Type);
            Assert.AreEqual(CotSettings.Auto, new CotSettings().Type, "Auto is the default");
        }

        [TestMethod]
        public void ASysidOnTwoConnections_IsSaidInTheStatus()
        {
            var transport = new FakeTransport();
            Func<IEnumerable<CotVehicle>> two = () => OneCopter().Concat(OneCopter().Select(v =>
            {
                v.Connection = "another link";
                return v;
            })).ToList();

            CotSender.Start(transport, EverySecond(), two);
            WaitFor(() => transport.Count >= 2);
            WaitFor(() => CotSender.Current.Status.Contains("MAV_SYSID"));

            StringAssert.Contains(CotSender.Current.Status, "system ID 1");
            Assert.IsTrue(transport.Messages.ToList().Any(m => m.Contains("uid=\"MP-UAS-1-2\"")));
        }

        [TestMethod]
        public void Udp_SendsToTheAddress()
        {
            using (var listener = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0)))
            {
                var port = ((IPEndPoint)listener.Client.LocalEndPoint).Port;
                listener.Client.ReceiveTimeout = 2000;
                using (var transport = new CotUdpTransport("127.0.0.1", port))
                {
                    transport.Open();
                    Assert.IsTrue(transport.Send(Encoding.UTF8.GetBytes("hello\n")));
                    var from = new IPEndPoint(IPAddress.Any, 0);
                    Assert.AreEqual("hello\n", Encoding.UTF8.GetString(listener.Receive(ref from)));
                    StringAssert.Contains(transport.Status, "127.0.0.1:" + port);
                }
            }
        }

        [TestMethod]
        public void Multicast_LeavesTheGroupPortToOtherPrograms()
        {
            var adapter = CotNetwork.DefaultAdapter(CotNetwork.Adapters());
            if (adapter == null)
                Assert.Inconclusive("no network adapter is up");

            var group = IPAddress.Parse(CotMulticastTransport.SaGroup);
            const int port = 46969;

            // another program holds the port without sharing it, the way WinTAK holds 6969 (on
            // loopback here, so the test raises no firewall prompt); the old output bound the
            // group's port itself and could not start
            using (var holder = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                holder.ExclusiveAddressUse = true;
                holder.Bind(new IPEndPoint(IPAddress.Loopback, port));

                using (var transport = new CotMulticastTransport(group, port, adapter))
                {
                    transport.Open();
                    Assert.IsTrue(transport.Send(Encoding.UTF8.GetBytes("<event/>\n")));
                    StringAssert.Contains(transport.Status, adapter.Address.ToString());
                    StringAssert.Contains(transport.Status, "239.2.3.1:" + port);
                }
            }
        }

        [TestMethod]
        public void Multicast_DefaultAdapter_HasAGatewayWhenOneDoes()
        {
            var adapters = new List<CotAdapter>
            {
                new CotAdapter { Name = "VirtualBox Host-Only", Address = IPAddress.Parse("192.0.2.1"), Physical = true },
                new CotAdapter { Name = "VPN", Address = IPAddress.Parse("198.51.100.7"), HasGateway = true },
                new CotAdapter { Name = "Wi-Fi", Address = IPAddress.Parse("203.0.113.25"), HasGateway = true, Physical = true }
            };

            Assert.AreEqual("Wi-Fi", CotNetwork.DefaultAdapter(adapters).Name);
            Assert.AreEqual("VPN", CotNetwork.DefaultAdapter(adapters.Take(2).ToList()).Name);
            Assert.IsNull(CotNetwork.DefaultAdapter(new List<CotAdapter>()));
        }

        [TestMethod]
        public void TcpHost_SendsToEveryClient()
        {
            var port = FreeTcpPort();
            using (var transport = new CotTcpServerTransport(port, true))
            {
                transport.Open();
                StringAssert.Contains(transport.Status, port.ToString());

                using (var one = new TcpClient("127.0.0.1", port))
                using (var two = new TcpClient("127.0.0.1", port))
                {
                    WaitFor(() => transport.Status.Contains("2 client"));
                    Assert.IsTrue(transport.Send(Encoding.UTF8.GetBytes("<event/>\n")));

                    foreach (var client in new[] { one, two })
                    {
                        client.ReceiveTimeout = 2000;
                        var buffer = new byte[64];
                        var read = client.GetStream().Read(buffer, 0, buffer.Length);
                        Assert.AreEqual("<event/>\n", Encoding.UTF8.GetString(buffer, 0, read));
                    }
                }
            }
        }

        [TestMethod]
        public void TcpHost_APortInUse_IsNamed()
        {
            var port = FreeTcpPort();
            var holder = new TcpListener(IPAddress.Loopback, port);
            holder.ExclusiveAddressUse = true;
            holder.Start();
            try
            {
                using (var transport = new CotTcpServerTransport(port, true))
                {
                    var ex = Assert.ThrowsException<InvalidOperationException>(() => transport.Open());
                    StringAssert.Contains(ex.Message, port.ToString());
                }
            }
            finally
            {
                holder.Stop();
            }
        }

        [TestMethod]
        public void TcpClient_NobodyListening_FailsToStart()
        {
            var transport = new CotTcpClientTransport("127.0.0.1", FreeTcpPort());
            Assert.ThrowsException<AggregateException>(() => transport.Open());
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [TestMethod]
        public void Window_ATypePickedFromTheList_ReachesTheRunningOutput()
        {
            const int CB_SETCURSEL = 0x014E, WM_COMMAND = 0x0111, CBN_SELCHANGE = 1;
            // the window reports a page view from a pool thread; leave that off for the rest of the run
            MissionPlanner.Utilities.Tracking.OptOut = true;
            var sender = CotSender.Start(new FakeTransport(), EverySecond(), OneCopter);

            using (var window = new SerialOutputCoT())
            {
                var type = window.Controls.Find("CMB_type", true).OfType<System.Windows.Forms.ComboBox>().Single();

                // a pick made from outside the window (a UI test driver): the selection and the box's
                // text change first, then the notice reaches the parent, which hands it to the box
                SendMessage(type.Handle, CB_SETCURSEL, (IntPtr)1, IntPtr.Zero);
                SendMessage(window.Handle, WM_COMMAND, (IntPtr)(CBN_SELCHANGE << 16), type.Handle);
                Assert.AreEqual("a-f-A-M-F-Q", sender.Settings.Type, "a type picked from the list");

                type.Text = "a-f-G-U-C";
                Assert.AreEqual("a-f-G-U-C", sender.Settings.Type, "a type typed in the box");
            }
        }

        static int FreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        static string Attribute(string xml, string name)
        {
            var start = xml.IndexOf(name + "=\"", StringComparison.Ordinal) + name.Length + 2;
            return xml.Substring(start, xml.IndexOf('"', start) - start);
        }
    }
}
