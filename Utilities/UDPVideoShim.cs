using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;

namespace MissionPlanner.Utilities
{
    public class UDPVideoShim
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UdpClient client;
        private static UdpClient client2;
        private static UdpClient client3;
        private static TcpClient tcpclient;
        private static Process gst;

        static UDPVideoShim()
        {
            try
            {
                client = new UdpClient(5600, AddressFamily.InterNetwork);
                client.BeginReceive(clientdata, client);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            try
            {
                client2 = new UdpClient(5000, AddressFamily.InterNetwork);
                client2.BeginReceive(clientdata, client2);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            try
            {
                client3 = new UdpClient(5100, AddressFamily.InterNetwork);
                client3.BeginReceive(clientdata, client3);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        ~UDPVideoShim()
        {
            Stop();
        }

        public static void Stop()
        {
            try
            {
                if(client != null)
                    client.Close();
                client = null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                if (tcpclient != null)
                    tcpclient.Close();
                tcpclient = null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            try
            {
                GStreamer.Stop(gst);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void clientdata(IAsyncResult ar)
        {
            var client = ((UdpClient) ar.AsyncState);

            if (client == null || client.Client == null)
                return;

            var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;

            if (client != null)
                client.Close();

            //removeme
            GStreamer.LookForGstreamer();

            if (!File.Exists(GStreamer.gstlaunch))
            {
                var gstpath = GStreamer.LookForGstreamer();

                if (File.Exists(gstpath))
                {
                    GStreamer.gstlaunch = gstpath;
                }
                else
                {
                    if (CustomMessageBox.Show("A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to install/config it now?", "GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        CustomMessageBox.Show(
                            "Please download gstreamer 1.9.2 from [link;HERE;https://gstreamer.freedesktop.org/data/pkg/windows/1.9.2/gstreamer-1.0-x86-1.9.2.msi]\n And install it using the 'COMPLETE' option");

                        GStreamer.gstlaunch = GStreamer.LookForGstreamer();
                        if (!File.Exists(GStreamer.gstlaunch))
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            GStreamer.UdpPort = port;
            gst = GStreamer.Start();
        }

        public static void Start()
        {
            ThreadPool.QueueUserWorkItem(tcpsolo);
            ThreadPool.QueueUserWorkItem(skyviper);
        }

        private static void tcpsolo(object state)
        {
            try
            {

                if (Ping("10.1.1.1"))
                {
                    // solo video
                    tcpclient = new TcpClient("10.1.1.1", 5502);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void skyviper(object state)
        {
            try
            {
                if (Ping("192.168.99.1"))
                {
                    // skyviper video
                    var test = new WebClient().DownloadString("http://192.168.99.1/");

                    if (test.Contains("SkyViper"))
                    {
                        //slave to sender clock and Pipeline clock time
                        GStreamer.Start("rtspsrc location=rtsp://192.168.99.1/media/stream2 debug=false buffer-mode=1 latency=100 ntp-time-source=3 ! application/x-rtp ! rtph264depay ! avdec_h264 ! avenc_mjpeg ");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static bool Ping(string ip)
        {
            try
            {
                using (var p = new Ping())
                {
                    var options = new PingOptions();
                    options.DontFragment = true;
                    var data = "MissionPlanner";
                    var buffer = Encoding.ASCII.GetBytes(data);
                    var timeout = 2000;
                    var reply = p.Send(ip, timeout, buffer, options);
                    if (reply.Status == IPStatus.Success)
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
    }
}