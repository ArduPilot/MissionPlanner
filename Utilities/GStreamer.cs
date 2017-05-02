using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities
{
    public class GStreamer
    {
        private static readonly ILog log =
    LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static List<Process> processList = new List<Process>();

        static object _lock = new object();

        //static NamedPipeServerStream pipeServer = new NamedPipeServerStream("gstreamer", PipeDirection.In);

        ~GStreamer()
        {
            StopAll();
        }

        static GStreamer()
        {
            UdpPort = 5600;
            OutputPort = 1235;
        }

        //gst-launch-1.0.exe  videotestsrc pattern=ball ! video/x-raw,width=640,height=480 ! clockoverlay ! x264enc ! rtph264pay ! udpsink host=127.0.0.1 port=5600
        //gst-launch-1.0.exe -v udpsrc port=5600 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue leaky=2 ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false

        //gst-launch-1.0.exe -v videotestsrc !  video/x-raw,format=BGRA,framerate=25/1 ! videoconvert ! autovideosink

        //gst-launch-1.0 videotestsrc pattern=ball ! x264enc ! rtph264pay ! udpsink host=127.0.0.1 port=5600
        //gst-launch-1.0 udpsrc port=5600 caps='application/x-rtp, media=(string)video, clock-rate=(int)90000, encoding-name=(string)H264' ! rtph264depay ! avdec_h264 ! autovideosink fps-update-interval=1000 sync=false

        //gst-launch-1.0.exe videotestsrc ! video/x-raw, width=1280, height=720, framerate=25/1 ! x264enc ! rtph264pay ! udpsink port=1234 host=192.168.0.1
        //gst-launch-1.0.exe -v udpsrc port=1234 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false

        // list plugins
        // gst-inspect-1.0

        public static string gstlaunch
        {
            get { return Settings.Instance["gstlaunchexe"]; }
            set { Settings.Instance["gstlaunchexe"] = value; }
        }

        public static bool checkGstLaunchExe()
        {
            if (File.Exists(gstlaunch))
                return true;

            return getGstLaunchExe();
        }

        public static string LookForGstreamer()
        {
            List<string> dirs = new List<string>();

            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.IsReady && d.DriveType == DriveType.Fixed)
                {
                    dirs.Add(d.RootDirectory.Name+ "gstreamer");
                    dirs.Add(d.RootDirectory.Name + "Program Files"+Path.DirectorySeparatorChar+"gstreamer");
                    dirs.Add(d.RootDirectory.Name + "Program Files (x86)" + Path.DirectorySeparatorChar + "gstreamer");
                }
            }

            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    var ans = Directory.GetFiles(dir, "gst-launch-1.0.exe",SearchOption.AllDirectories);

                    if (ans.Length > 0)
                    {
                        log.Info("Found gstreamer "+ans.First());
                        return ans.First();
                    }
                }
            }

            return "";
        }

        public static bool getGstLaunchExe()
        {
            if (gstlaunch == "")
                gstlaunch = LookForGstreamer();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "gst-launch|gst-launch-1.0.exe";
                ofd.FileName = gstlaunch;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(ofd.FileName))
                    {
                        gstlaunch = ofd.FileName;
                        return true;
                    }
                }
            }

            return false;
        }

        public static int UdpPort { get; set; }

        public static int OutputPort { get; set; }

        private static bool isrunning
        {
            get { return processList != null && processList.Count > 0 && !processList.Any(a => a.HasExited); }
        }

        public static Process Start(string custompipelinesrc = "", bool externalpipeline = false, bool allowmultiple = false)
        {
            // prevent starting 2 process's
            lock (_lock)
            {
                if (!allowmultiple && isrunning)
                {
                    log.Info("already running");
                    return null;
                }

                if (File.Exists(gstlaunch))
                {
                    ProcessStartInfo psi = new ProcessStartInfo(gstlaunch,
                        String.Format(
                            //"-v udpsrc port={0} buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! queue leaky=2 ! rtpvrawpay ! tcpserversink host=127.0.0.1 port={1} sync=false",
                            "-v udpsrc port={0} buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue leaky=2 ! avenc_mjpeg ! queue leaky=2 ! tcpserversink host=127.0.0.1 port={1} sync=false",
                            //"-v udpsrc port={0} buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! glimagesink",
                            UdpPort, OutputPort));

                    if (custompipelinesrc != "")
                    {
                        psi.Arguments = custompipelinesrc;

                        if (!externalpipeline)
                        {
                            psi.Arguments += String.Format(
                                " ! queue leaky=2 ! tcpserversink host=127.0.0.1 port={0} sync=false",
                                OutputPort);
                        }
                        else
                        {
                            psi.Arguments += " ! decodebin ! queue leaky=2 ! autovideosink";
                        }
                    }

                    //"-v udpsrc port=5600 buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! queue ! rtpvrawpay ! giosink location=\\\\\\\\.\\\\pipe\\\\gstreamer");

                    //avenc_mjpeg

                    psi.UseShellExecute = false;

                    log.Info("Starting " + psi.FileName + " " + psi.Arguments);

                    psi.RedirectStandardOutput = true;

                    var process = Process.Start(psi);
                    GStreamer.processList.Add(process);

                    var th = new Thread((() =>
                    {
                        using (StreamReader sr = process.StandardOutput)
                        {
                            try
                            {
                                while (process != null && !process.HasExited)
                                {
                                    log.Info(sr.ReadLine());
                                }
                            }
                            catch { }
                        }
                    }));
                    th.IsBackground = true;
                    th.Start();

                    process.Exited += delegate (object sender, EventArgs args) { Stop(process); };

                    //pipeServer.WaitForConnection();

                    //NamedPipeConnect(pipeServer);

                    System.Threading.ThreadPool.QueueUserWorkItem(_Start);

                    return process;
                }
            }
            return null;
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void NamedPipeConnect(NamedPipeServerStream pipeServer)
        {
            // 1080 * 1920 * 4(int) = 8294400 * 30fps = buffer 1/3 sec
            using (var stream = new BufferedStream(pipeServer,1024*1024*9*10))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    while (pipeServer.IsConnected)
                    {
                        //readJPGData(stream, ms);

                        readRTPData(stream, ms);
                     
                        System.Threading.Thread.Sleep(0);
                    }

                    //cleanup on disconnect
                    FlightData.myhud.bgimage = null;
                }
            }
        }

        static void _Start(object nothing)
        {
            try
            {
                var deadline = DateTime.Now.AddSeconds(20);

                log.Info("_Start");

                while (DateTime.Now < deadline)
                {
                    try
                    {
                        TcpClient client = new TcpClient("127.0.0.1", OutputPort);
                        Console.WriteLine("Port open");
                        client.Close();
                        break;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Port closed");
                    }
                }

                using (TcpClient client = new TcpClient("127.0.0.1", OutputPort))
                {
                    client.ReceiveBufferSize = 1024*1024*5; // 5mb

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (var stream = new BufferedStream(client.GetStream(), 1024*1024*5))
                        {
                            DateTime lastdata = DateTime.Now;
                            while (client.Client.Connected)
                            {
                                int bytestoread = client.Available;

                                while (bytestoread > 0)
                                {
                                    //bytestoread -= readRTPData(stream, ms);
                                    bytestoread -= readJPGData(stream, ms);
                                    lastdata = DateTime.Now;
                                }
                                // up to 100 fps or 50 with 10ms process time
                                System.Threading.Thread.Sleep(10);

                                if (lastdata.AddSeconds(5) < DateTime.Now)
                                {
                                    client.Client.Send(new byte[0]);
                                }
                            }
                            //cleanup on disconnect
                            FlightData.myhud.bgimage = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FlightData.myhud.bgimage = null;
                log.Error(ex);
            }
        }

        class rtpheader
        {
            // 2 bits
            public byte version { get; set; }
            // 1 bit
            public byte padding { get; set; }
            // 1 bit
            public byte extension { get; set; }
            // 4 bits
            public byte csrccount { get; set; }
            // 1 bits
            public byte marker { get; set; }
            // 7 bits
            public byte payloadtype { get; set; }
            // 16 bits
            public ushort sequencenumber { get; set; }
            // 32 bits
            public uint timestamp { get; set; }
            // 32 bits
            public uint ssrc { get; set; }

            public ushort extsequencenumber { get; set; }

            public ushort length { get; set; }
            public byte F { get; set; }
            public ushort lineno { get; set; }
            public byte C { get; set; }
            public ushort offset { get; set; }

            // only is C is set above;
            public ushort length2 { get; set; }
            public byte F2 { get; set; }
            public ushort lineno2 { get; set; }
            public byte C2 { get; set; }
            public ushort offset2 { get; set; }

            // rfc 4175 - https://tools.ietf.org/html/rfc4175
            public rtpheader(byte[] buffer)
            {
                version = (byte) ((buffer[0] >> 6) & 3);
                padding = (byte) ((buffer[0] >> 5) & 1);
                extension = (byte)((buffer[0] >> 4) & 1);
                csrccount = (byte)((buffer[0] >> 0) & 15);

                marker = (byte) ((buffer[1] >> 7) & 1);
                payloadtype = (byte) ((buffer[1] >> 0) & 127);

                sequencenumber = (ushort)(buffer[2]<<8 + buffer[3]);
                timestamp = (uint)(buffer[4] << 24 + buffer[5] << 26 + buffer[6] << 8 + buffer[7]);
                ssrc = (uint)(buffer[8] << 24 + buffer[9] << 26 + buffer[10] << 8 + buffer[11]);

                // 2 bytes https://fossies.org/linux/gst-plugins-good/gst/rtp/gstrtpvrawpay.c #384   
                // always 0    
                extsequencenumber = (ushort)(buffer[12] << 8 + buffer[13]);

                // 6 byte - https://fossies.org/linux/gst-plugins-good/gst/rtp/gstrtpvrawpay.c #467
                // (pixels * pgroup) / xinc   ;    pgroup = 4/32bit BGRA   xinc = 1 for BGRA
                length = (ushort)((buffer[14] << 8) + buffer[15]);
                F = (byte)(buffer[16] >> 7);
                // height line number
                lineno = (ushort)(((buffer[16] & 127) << 8) + buffer[17]);
                // there is more than one height here
                C = (byte) (buffer[18] >> 7);
                offset = (ushort) (((buffer[18] & 127) << 8) + buffer[19]);

                if (buffer.Length >= 26)
                {
                    length2 = (ushort)((buffer[20] << 8) + buffer[21]);
                    F2 = (byte)(buffer[22] >> 7);
                    // height line number
                    lineno2 = (ushort)(((buffer[22] & 127) << 8) + buffer[23]);
                    // there is more than one height here
                    C2 = (byte)(buffer[24] >> 7);
                    offset2 = (ushort)(((buffer[24] & 127) << 8) + buffer[25]);

                    return;
                }

                var actoffset = offset*4;

                //Console.WriteLine("rtp {0} - mark {1} {2} {3} {4} {5} {6}={7} {8}", payloadtype, marker, sequencenumber, length, C, lineno, offset, actoffset,F);
            }
        }

        static byte rtpbyte1 = 0x80;
        // mark bit notset
        static byte rtpbyte2 = 0x60;
        // mark bit set
        static byte rtpbyte2_2 = 0xe0;
        // image width
        static int width = 0;

        private static byte[] buffer;
        private static Bitmap img;

        public static int readRTPData(Stream stream, MemoryStream ms)
        {
            int readamount = 0;

            var ch1 = stream.ReadByte();
            readamount++;
            if (ch1 == rtpbyte1)
            {
                var ch2 = stream.ReadByte();
                readamount++;
                // handle 2 rtpbyte1's in a row
                if (ch2 == rtpbyte1)
                {
                    ch1 = ch2;
                    ch2 = stream.ReadByte();
                    readamount++;
                }

                if (ch2 == rtpbyte2 || ch2 == rtpbyte2_2)
                {
                    byte[] headerBytes = new byte[4*5];
                    headerBytes[0] = (byte) ch1;
                    headerBytes[1] = (byte) ch2;

                    readamount += stream.Read(headerBytes, 2, headerBytes.Length - 2);

                    GStreamer.rtpheader header = new rtpheader(headerBytes);

                    // this check is the same as rtpbyte1 and rtpbyte2/rtpbyte2_2
                    if (header.version == 2 && header.payloadtype == 96 && header.extsequencenumber == 0)
                    {
                        // read additial C
                        if (header.C > 0)
                        {
                            var oldsize = headerBytes.Length;
                            Array.Resize(ref headerBytes, headerBytes.Length + 6);
                            readamount += stream.Read(headerBytes, oldsize, 6);

                            header = new rtpheader(headerBytes);
                        }

                        var pixels = header.length/4;
                        if (header.C > 0 && header.lineno == 0)
                        {
                            width = header.offset + pixels;
                        }

                        //p0 + (lin * ystride) + (offs * pgroup), length
                        var fileoffset = (header.lineno)*width*4 + header.offset*4;
                        if (fileoffset != ms.Position)
                        {
                        }
                        ms.Seek(fileoffset, SeekOrigin.Begin);

                        int neededbytes = header.length + header.length2;

                        if (buffer == null || buffer.Length < (neededbytes))
                            buffer = new byte[neededbytes];

                        var read = stream.Read(buffer, 0, neededbytes);
                        ms.Write(buffer, 0, read);
                        readamount += read;

                        if (header.marker > 0 && width != 0)
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            try
                            {
                                if (img == null || img.Width < width || img.Height < header.lineno + 1)
                                    img = new Bitmap(width, header.lineno + 1, PixelFormat.Format32bppArgb);

                                lock (img)
                                {
                                    BitmapData bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height),
                                        ImageLockMode.WriteOnly, img.PixelFormat);

                                    IntPtr ptr = bmpData.Scan0;

                                    Marshal.Copy(ms.ToArray(), 0, ptr, (int) img.Width*img.Height*4);

                                    img.UnlockBits(bmpData);
                                }

                                //img.Save("test.bmp");

                                FlightData.myhud.bgimage = img;

                                tempno++;
                                persecond++;

                                if (lastsecond.Second != DateTime.Now.Second)
                                {
                                    Console.WriteLine("image {0}x{1} size {2} miss {3} ps {4}",
                                        img.Width,
                                        img.Height, 0, miss, persecond);
                                    persecond = 0;
                                    lastsecond = DateTime.Now;
                                    miss = 0;
                                }

                                ms.SetLength(0);
                            }
                            catch
                            {
                                ms.SetLength(0);
                            }
                        }
                    }
                    else
                    {
                        miss++;
                        Console.WriteLine("packet failed header check ");
                    }
                }
                else
                {
                    miss++;
                    Console.WriteLine("out of sync2 {0:X}", ch1);
                }
            }
            else
            {
                miss++;
                Console.WriteLine("out of sync1 {0:X}", ch1);
            }

            return readamount;
        }

        static int tempno = 0;
        static int miss = 0;
        static int persecond = 0;
        static DateTime lastsecond = DateTime.Now;

        private static int readJPGData(Stream stream, MemoryStream ms)
        {
            var bytesread = 0;

            // start header
            byte ch3 = (byte) stream.ReadByte();
            bytesread++;
            if (ch3 == 0xff)
            {
                byte ch4 = (byte) stream.ReadByte();
                bytesread++;
                if (ch4 == 0xd8)
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.WriteByte(ch3);
                    ms.WriteByte(ch4);
                    int last = 0;
                    do
                    {
                        int datach = stream.ReadByte();
                        bytesread++;
                        if (datach < 0)
                            break;

                        ms.WriteByte((byte) datach);

                        if (last == 0xff)
                        {
                            if (datach == 0xd9)
                                break;
                        }
                        last = datach;
                    } while (true);

                    ms.Seek(0, SeekOrigin.Begin);
                    try
                    {
                        var temp = Image.FromStream(ms);

                        //File.WriteAllBytes(tempno + ".bmp", ms.ToArray());

                        FlightData.myhud.bgimage = temp;

                        tempno++;
                        persecond++;

                        if (lastsecond.Second != DateTime.Now.Second)
                        {
                            Console.WriteLine("image {0}x{1} size {2} miss {3} ps {4}",
                                temp.Width,
                                temp.Height, 0, miss, persecond);
                            persecond = 0;
                            lastsecond = DateTime.Now;
                            miss = 0;
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    miss++;
                }
            }
            else
            {
                miss++;
            }

            return bytesread;
        }

        public static void Stop(Process run)
        {
            try
            {
                log.Info("Stop");

                if (run != null)
                {
                    run.Kill();
                    run.Close();
                }
            }
            catch { }

            run = null;
        }

        public static void StopAll()
        {
            foreach (var process in processList)
            {
                Stop(process);
            }
        }
    }
}