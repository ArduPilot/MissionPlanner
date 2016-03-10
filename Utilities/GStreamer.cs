using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MissionPlanner.GCSViews;

namespace MissionPlanner.Utilities
{
    public class GStreamer
    {

        //gst-launch-1.0.exe videotestsrc ! video/x-raw, width=1280, height=720, framerate=25/1 ! x264enc ! rtph264pay ! udpsink port=1234 host=192.168.0.1
        //gst-launch-1.0.exe -v udpsrc port=1234 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false

        public static string gstlaunch {
            get { return Settings.Instance["gstlaunchexe"]; }
            set { Settings.Instance["gstlaunchexe"] = value; }
        }

        public static bool checkGstLaunchExe()
        {
            if (File.Exists(gstlaunch))
                return true;

            return getGstLaunchExe();
        } 

        public static bool getGstLaunchExe()
        {
            OpenFileDialog ofd = new OpenFileDialog();
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

            return false;
        }

        public static void Start()
        {
            if (File.Exists(gstlaunch))
            {
                System.Diagnostics.Process.Start(gstlaunch,
                    "-v udpsrc port=1234 buffer-size=60000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! queue ! avenc_mjpeg ! tcpserversink host=127.0.0.1 port=1235 sync=false");

                System.Threading.ThreadPool.QueueUserWorkItem(_Start);
            }
        }

        static void _Start(object nothing)
        {
            TcpClient client = new TcpClient("127.0.0.1", 1235);
            client.ReceiveBufferSize = 1024 * 1024 * 1;

            MemoryStream ms = new MemoryStream();

            int tempno = 0;
            int miss = 0;
            int persecond = 0;
            DateTime lastsecond = DateTime.Now;

            var stream = client.GetStream();

            bool lastvalid = false;

            while (client.Client.Connected)
            {
                while (client.Available > 0)
                {
                    // start header
                    byte ch3 = (byte)stream.ReadByte();
                    if (ch3 == 0xff)
                    {
                        byte ch4 = (byte)stream.ReadByte();
                        if (ch4 == 0xd8)
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                            ms.WriteByte(ch3);
                            ms.WriteByte(ch4);
                            int last = 0;
                            do
                            {
                                int datach = stream.ReadByte();
                                if (datach < 0)
                                    break;

                                ms.WriteByte((byte)datach);

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
                                    Console.WriteLine("image {0}x{1} size {2} miss {3} ps {4}", temp.Width, temp.Height, 0, miss, persecond);
                                    persecond = 0;
                                    lastsecond = DateTime.Now;
                                    miss = 0;
                                }


                            }
                            catch { }
                        }
                        else { miss++; }
                    }
                    else { miss++; }
                }
                System.Threading.Thread.Sleep(2);

            }
        }
    }
}