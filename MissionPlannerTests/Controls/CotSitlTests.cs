using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MissionPlanner.Comms;

namespace MissionPlanner.Controls.Tests
{
    /// <summary>
    /// The CoT output end to end against SITL vehicles, one connection each, with a listener outside
    /// the test reading what arrives. Inconclusive unless MP_COT_SITL is set:
    ///   MP_COT_SITL=127.0.0.1:5760          the SITLs' MAVLink ports, comma separated
    ///                                       (127.0.0.1:5760,127.0.0.1:5770 for two vehicles)
    ///   MP_COT_OUT=udp:127.0.0.1:4242       or mcast:239.2.3.1:6969[:adapter ip], or tcphost:14551
    ///   MP_COT_SECONDS=40                   how long to send; MP_COT_DROP_AFTER=20 stops reading the
    ///                                       last link after that many seconds, as when telemetry goes
    ///                                       quiet (closing it would make the SITL exit), to see the
    ///                                       stale event while the other vehicles carry on
    /// Nothing reads or writes Mission Planner's settings.
    /// </summary>
    [TestClass]
    public class CotSitlTests
    {
        [TestMethod]
        public void Sitl_SendsTheVehicle()
        {
            var sitl = Environment.GetEnvironmentVariable("MP_COT_SITL");
            if (string.IsNullOrEmpty(sitl))
                Assert.Inconclusive("set MP_COT_SITL=host:port of a SITL vehicle");

            var seconds = int.Parse(Environment.GetEnvironmentVariable("MP_COT_SECONDS") ?? "40");
            var dropAfter = int.Parse(Environment.GetEnvironmentVariable("MP_COT_DROP_AFTER") ?? "0");
            var output = Environment.GetEnvironmentVariable("MP_COT_OUT") ?? "udp:127.0.0.1:4242";

            var links = sitl.Split(',').Select(address =>
            {
                var parts = address.Trim().Split(':');
                var link = new MAVLinkInterface();
                link.BaseStream = new TcpSerial { client = new TcpClient(parts[0], int.Parse(parts[1])) };
                return link;
            }).ToList();

            var stop = new CancellationTokenSource();
            var quiet = false;
            var readers = links.Select(link => Task.Run(() =>
            {
                while (!stop.IsCancellationRequested)
                {
                    try
                    {
                        if (!(quiet && link == links.Last()) && link.BaseStream.IsOpen)
                            link.readPacket();
                        else
                            Thread.Sleep(100);
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(50);
                    }
                }
            })).ToList();

            Func<MAVLinkInterface, MAVState> vehicleOn = link => link.MAVlist.FirstOrDefault(mav =>
                mav.apname != MAVLink.MAV_AUTOPILOT.INVALID && mav.cs.lat != 0);

            try
            {
                var deadline = DateTime.UtcNow.AddSeconds(30);
                while (DateTime.UtcNow < deadline && links.Any(link => vehicleOn(link) == null))
                    Thread.Sleep(200);
                Assert.IsTrue(links.All(link => vehicleOn(link) != null), "no heartbeat with a position from every SITL");

                var settings = new CotSettings
                {
                    IntervalSeconds = 2,
                    Rows = links.Select(link => vehicleOn(link).sysid.ToString()).Select(sysid =>
                        new CotRow { SysId = sysid, Uid = "MP-UAS-" + sysid, Callsign = "UAS-" + sysid, Takv = true }).ToList()
                };

                var sender = CotSender.Start(Transport(output), settings,
                    () => links.SelectMany(link => link.MAVlist.Select(mav => CotVehicles.Read(mav, link))).ToList());
                var start = DateTime.UtcNow;
                var dropped = false;
                while ((DateTime.UtcNow - start).TotalSeconds < seconds)
                {
                    if (dropAfter > 0 && !dropped && (DateTime.UtcNow - start).TotalSeconds > dropAfter)
                    {
                        // the last link goes quiet: no more packets, as when the telemetry drops
                        quiet = true;
                        dropped = true;
                        Console.WriteLine("{0:HH:mm:ss.fff} last link quiet", DateTime.Now);
                    }

                    Console.WriteLine("{0:HH:mm:ss} {1}", DateTime.Now, sender.Status);
                    Thread.Sleep(2000);
                }

                foreach (var link in links)
                    Console.WriteLine("components heard: " + string.Join(", ",
                        link.MAVlist.Select(mav => mav.sysid + "/" + mav.compid + " " + mav.apname)));
                Console.WriteLine("{0:HH:mm:ss.fff} stopping", DateTime.Now);
                sender.Stop();
                Console.WriteLine("{0:HH:mm:ss.fff} stopped: {1}, {2} sent", DateTime.Now, sender.Status, sender.Sent);
                Assert.IsTrue(sender.Sent > 0, sender.Status);
            }
            finally
            {
                stop.Cancel();
                Task.WaitAll(readers.ToArray(), 2000);
                foreach (var link in links)
                {
                    try
                    {
                        link.BaseStream.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        static ICotTransport Transport(string output)
        {
            var parts = output.Split(':');
            switch (parts[0])
            {
                case "mcast":
                    var adapter = parts.Length > 3
                        ? CotNetwork.Adapters().First(a => a.Address.ToString() == parts[3])
                        : CotNetwork.DefaultAdapter(CotNetwork.Adapters());
                    return new CotMulticastTransport(IPAddress.Parse(parts[1]), int.Parse(parts[2]), adapter);
                case "tcphost":
                    return new CotTcpServerTransport(int.Parse(parts[1]), true);
                case "tcp":
                    return new CotTcpClientTransport(parts[1], int.Parse(parts[2]));
                default:
                    return new CotUdpTransport(parts[1], int.Parse(parts[2]));
            }
        }
    }
}
