using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ntrip
{
    class Program
    {
        private static bool stop = false;

        static void Main(string[] args)
        {
            Console.WriteLine("Usage: Ntrip.exe comport");
            Console.WriteLine("Usage: Ntrip.exe comport lat lng alt");
            Console.CancelKeyPress += Console_CancelKeyPress;
            AppDomain.CurrentDomain.DomainUnload += (sender, eventArgs) =>
            {
                stop = true;
                Thread.Sleep(1000);
            };
            var tcp = TcpListener.Create(2101);

            tcp.Start();

            tcp.BeginAcceptTcpClient(processclient, tcp);

            var ubx = new Ubx();
            var rtcm = new rtcm3();
            Stream file = null;
            DateTime filetime = DateTime.MinValue;
            SerialPort port = null;

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            
            while (!stop)
            {
                try
                {
                    if (port == null || !port.IsOpen)
                    {
                        var comport = args[0];
                        if (comport.Contains("/dev/"))
                            comport = Mono.Unix.UnixPath.GetRealPath(comport);
                        Console.WriteLine("Port: " + comport);
                        port = new SerialPort(comport, 115200);

                        port.Open();
                        ubx.SetupM8P(port, true, false);

                        if (args.Length == 4)
                        {
                            ubx.SetupBasePos(port,
                                new PointLatLngAlt(double.Parse(args[1]), double.Parse(args[2]), double.Parse(args[3])),
                                60, 2);
                        }
                        else
                        {
                            ubx.SetupBasePos(port, PointLatLngAlt.Zero, 60, 2);
                        }
                    }

                    if (file == null || !file.CanWrite || DateTime.UtcNow.Day != filetime.Day)
                    {
                        if (file != null)
                            file.Close();
                        string fn = "";
                        int no = 0;
                        while (File.Exists((fn = DateTime.UtcNow.ToString("yyyy-MM-dd") + "-" + no + ".rtcm.Z")))
                        {
                            no++;
                        }

                        file = new GZipStream(new BufferedStream(new FileStream(fn,
                            FileMode.OpenOrCreate)), CompressionMode.Compress);
                        filetime = DateTime.UtcNow;
                    }

                    var btr = port.BytesToRead;
                    if (btr > 0)
                    {
                        var buffer = new byte[btr];
                        btr = port.Read(buffer, 0, btr);

                        foreach (byte by in buffer)
                        {
                            btr--;

                            if (ubx.Read((byte) by) > 0)
                            {
                                rtcm.resetParser();
                                //Console.WriteLine(DateTime.Now + " new ubx message");
                            }
                            else if (by >= 0 && rtcm.Read((byte) by) > 0)
                            {
                                ubx.resetParser();
                                //Console.WriteLine(DateTime.Now + " new rtcm message");
                                gotRTCMData?.Invoke(rtcm.packet, rtcm.length);
                                file.Write(new ReadOnlySpan<byte>(rtcm.packet, 0, rtcm.length));
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(100);
            }

            if (file != null)
                file.Close();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            stop = true;
            Thread.Sleep(1000);
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
