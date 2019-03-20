using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class UDPVideoShim
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UdpClient client;
        private static UdpClient client2;
        private static UdpClient client3;
        private static TcpClient tcpclient;

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
        }

        private static void clientdata(IAsyncResult ar)
        {
            var client = ((UdpClient) ar.AsyncState);

            if (client == null || client.Client == null)
                return;

            var port = ((IPEndPoint) client.Client.LocalEndPoint).Port;

            if (client != null)
                client.Close();

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();

            if (!File.Exists(GStreamer.gstlaunch))
            {
                if (CustomMessageBox.Show(
                        "A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to install/config it now?",
                        "GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) ==
                    (int) System.Windows.Forms.DialogResult.Yes)
                {
                    DownloadGStreamer();
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


            GStreamer.UdpPort = port;
            GStreamer.StartA("udpsrc port=" + port +
                             " buffer-size=300000 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink");
        }

        public static void DownloadGStreamer()
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            ThemeManager.ApplyThemeTo(prd);
            prd.DoWork += sender =>
            {
                GStreamer.DownloadGStreamer(((i, s) =>
                {
                    prd.UpdateProgressAndStatus(i, s);
                    if (prd.doWorkArgs.CancelRequested) throw new Exception("User Request");
                }));
            };
            prd.RunBackgroundOperationAsync();

            GStreamer.gstlaunch = GStreamer.LookForGstreamer();
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
                    log.Info("Detected a solo IP");
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
                    log.Info("Detected a SkyViper IP");

                    bool skyviper = false;

                    // skyviper rtsp
                    try
                    {
                        var rtspclient = new TcpClient("192.168.99.1", 554);
                        rtspclient.Close();
                        skyviper = true;
                    }
                    catch
                    {
                    }

                    // skyviper video
                    try
                    {
                        var test = new WebClient().DownloadString("http://192.168.99.1/");
                        if (test.Contains("SkyViper"))
                            skyviper = true;
                    }
                    catch
                    {
                    }

                    if (skyviper)
                    {
                        log.Info("Detected a SkyViper");

                        GStreamer.gstlaunch = GStreamer.LookForGstreamer();

                        if (!File.Exists(GStreamer.gstlaunch))
                        {
                            if (CustomMessageBox.Show(
                                    "A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to install/config it now?",
                                    "GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) ==
                                (int) System.Windows.Forms.DialogResult.Yes)
                            {
                                DownloadGStreamer();
                            }
                        }


                        //slave to sender clock and Pipeline clock time
                        GStreamer.StartA(
                            "rtspsrc location=rtsp://192.168.99.1/media/stream2 debug=false buffer-mode=1 latency=100 ntp-time-source=3 ! application/x-rtp ! rtph264depay ! avdec_h264 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink");
                        /*
                        string url = "http://192.168.99.1/ajax/video.mjpg";

                        Settings.Instance["mjpeg_url"] = url;

                        CaptureMJPEG.Stop();

                        CaptureMJPEG.URL = url;

                        CaptureMJPEG.OnNewImage += FlightData.instance.CaptureMJPEG_OnNewImage;

                        CaptureMJPEG.runAsync();
                        */
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