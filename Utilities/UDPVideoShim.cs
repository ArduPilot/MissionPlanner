using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class UDPVideoShim
    {
        private static UdpClient client;
        private static TcpClient tcpclient;

        static UDPVideoShim()
        {
            try
            {
                client = new UdpClient(5600, AddressFamily.InterNetwork);
                client.BeginReceive(clientdata, client);
            }
            catch { }
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
            catch { }

            try
            {
                if (tcpclient != null)
                    tcpclient.Close();
                tcpclient = null;
            }
            catch { }

            try
            {
                GStreamer.Stop();
            }
            catch { }
        }

        private static void clientdata(IAsyncResult ar)
        {
            if(client != null)
                client.Close();

            if (!File.Exists(GStreamer.gstlaunch))
            {
                if (CustomMessageBox.Show("A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to config it now","GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (GStreamer.getGstLaunchExe())
                    {

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            GStreamer.Start();
        }

        public static void Start()
        {
            ThreadPool.QueueUserWorkItem(tcpsolo);
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
            catch (Exception)
            {
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
            catch
            {
                return false;
            }
        }
    }
}