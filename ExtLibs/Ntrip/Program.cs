using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ntrip
{
    class Program
    {
        static void Main(string[] args)
        {
            var tcp = TcpListener.Create(2101);

            tcp.Start();

            tcp.BeginAcceptTcpClient(processclient, tcp);

            var comport = args[0];
            var port = new SerialPort(comport, 115200);

            var ubx = new Ubx();
            var rtcm = new rtcm3();

            while (true)
            {
                try
                {
                    if (!port.IsOpen)
                    {
                        port.Open();
                        ubx.SetupM8P(port, true, false);

                        ubx.SetupBasePos(port, PointLatLngAlt.Zero, 60, 2);
                    }

                    var btr = port.BytesToRead;
                    if (btr > 0)
                    {
                        var buffer = new byte[btr];
                        port.Read(buffer, 0, btr);

                        foreach (byte by in buffer)
                        {
                            btr--;

                            if (ubx.Read((byte)by) > 0)
                            {
                                rtcm.resetParser();
                                //Console.WriteLine(DateTime.Now + " new ubx message");
                            }
                            else if (by >= 0 && rtcm.Read((byte)by) > 0)
                            {
                                ubx.resetParser();
                                //Console.WriteLine(DateTime.Now + " new rtcm message");
                                gotRTCMData?.Invoke(rtcm.packet, rtcm.length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(100);
            }
        }

        private static void processclient(IAsyncResult ar)
        {
            var tcplist = ar.AsyncState as TcpListener;
            tcplist.BeginAcceptTcpClient(processclient, tcplist);
            var client = tcplist.EndAcceptTcpClient(ar);

            Task.Run(() =>
            {
                Program.DataEventHandler func = async (data, length) =>
                {
                    try
                    {
                        //Console.WriteLine("write data " + length);
                        client.GetStream().Write(data, 0, length);
                        client.GetStream().Flush();
                    }
                    catch { }
                };
                try
                {
                    Program.gotRTCMData += func;

                    var data = "ICY 200 OK\r\n\r\n".Select(a => (byte)a).ToArray();
                    client.GetStream().Write(data, 0, data.Length);

                    while (client.Connected)
                    {
                        Thread.Sleep(100);
                    }

                }
                catch { }
                finally
                {
                    Program.gotRTCMData -= func;
                }
            });
        }

        public delegate void DataEventHandler(byte[] data, int length);

        public static event DataEventHandler gotRTCMData;
    }
}
