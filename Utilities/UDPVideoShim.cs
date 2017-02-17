﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
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
            catch { }
            try
            {
                client2 = new UdpClient(5000, AddressFamily.InterNetwork);
                client2.BeginReceive(clientdata, client2);
            }
            catch { }
            try
            {
                client3 = new UdpClient(5100, AddressFamily.InterNetwork);
                client3.BeginReceive(clientdata, client3);
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
                    if (CustomMessageBox.Show("A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to config it now?", "GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
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
            }

            GStreamer.UdpPort = port;
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