using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using Mono.Unix;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DroneCAN;

namespace Ntrip
{
    class Program
    {
        private static bool stop = false;
        private static int totalread;
        private static int everyminute;

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
            Console.WriteLine("Listerning on 2101");

            tcp.Start();

            tcp.BeginAcceptTcpClient(processclient, tcp);

            var ubx = new Ubx();
            var rtcm = new rtcm3();
            var can = new DroneCAN.DroneCAN();
            Stream file = null;
            DateTime filetime = DateTime.MinValue;
            SerialPort port = null;
            everyminute = DateTime.Now.Minute;

            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            // setup allocator
            can.SourceNode = 127;
            can.SetupDynamicNodeAllocator();

            // feed the rtcm data into the rtcm parser if we get a can message
            can.MessageReceived += (frame, msg, id) =>
            {
                if (frame.MsgTypeID == (ushort)DroneCAN.DroneCAN.UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_ID)
                {
                    var rtcmcan = (DroneCAN.DroneCAN.uavcan_equipment_gnss_RTCMStream)msg;

                    for (int a = 0; a < rtcmcan.data_len; a++)
                    {
                        int seenmsg = -1;

                        if ((seenmsg = rtcm.Read(rtcmcan.data[a])) > 0)
                        {
                            Console.WriteLine("CANRTCM" + seenmsg);
                            gotRTCMData?.Invoke(rtcm.packet, rtcm.length);
                            file.Write(rtcm.packet, 0, rtcm.length);
                        }
                    }
                }
            };

            while (!stop)
            {
                try
                {
                    if (port == null || !port.IsOpen)
                    {
                        var comport = args[0];
                        if (comport.Contains("/dev/"))
                            comport = UnixPath.GetRealPath(comport);
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
                        totalread += btr;
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
                            if (by >= 0 && rtcm.Read((byte) by) > 0)
                            {
                                ubx.resetParser();
                                //Console.WriteLine(DateTime.Now + " new rtcm message");
                                gotRTCMData?.Invoke(rtcm.packet, rtcm.length);
                                file.Write(rtcm.packet, 0, rtcm.length);
                            }
                            if ((by >= 0 && can.ReadSLCAN(by) > 0))// can_rtcm
                            {
                                ubx.resetParser();
                            }
                        }
                    }

                    if (DateTime.Now.Minute != everyminute)
                    {
                        Console.WriteLine("{0} bps", totalread / 60);
                        if (totalread == 0)
                        {
                            try
                            {
                                port.Close();
                                port = null;
                            }
                            catch (Exception)
                            {
                                port = null;
                            }
                        }
                        totalread = 0;
                        everyminute = DateTime.Now.Minute;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(5000);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                Thread.Sleep(10);
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
                Program.DataEventHandler func = (data, length) =>
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
                    var request = new StreamReader(client.GetStream(), Encoding.ASCII).ReadLine();

                    if (request.Contains(" / "))
                    {
                        var data2 =
                            "SOURCETABLE 200 OK\r\nContent-Type: text/plain\r\n\r\nSTR;DEFAULT;Default;RTCM 3.2;;2;GPS+GLO+GLO+BDS;MP;;0.00;0.00;0;0;sNTRIP;none;N;N;0;none;\r\nENDSOURCETABLE\r\n\r\n"
                                .Select(a => (byte) a).ToArray();
                        client.GetStream().Write(data2, 0, data2.Length);
                        client.Close();
                        return;
                    }

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
