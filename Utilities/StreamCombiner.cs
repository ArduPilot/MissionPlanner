using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Core.ExtendedObjects;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class StreamCombiner
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static List<int> portlist = new EventList<int>() {5760, 5770, 5780, 5790, 5800};

        static TcpListener listener = new TcpListener(IPAddress.Loopback, 5750);

        private static TcpClient Server = null;

        public static Thread th = null;

        private static bool run = false;

        static byte newsysid = 1;

        public static void Start()
        {
            if (run == true)
            {
                Stop();

                return;
            }

            newsysid = 1;

            listener.Start();

            listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);

            foreach (var portno in portlist)
            {
                TcpClient cl = new TcpClient();

                cl.BeginConnect(IPAddress.Loopback, portno, RequestCallback, cl);

                System.Threading.Thread.Sleep(500);
            }

            th = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "stream combiner"
            };
            th.Start();
        }

        public static void Stop()
        {
            run = false;
            foreach (var client in clients)
            {
                try
                {
                    client.Close();
                }
                catch
                {
                }
            }

            clients.Clear();
        }

        private static void mainloop()
        {
            run = true;

            byte[] buffer = new byte[1024];

            MAVLink.MavlinkParse mav = new MAVLink.MavlinkParse();

            while (run)
            {
                try
                {
                    while (Server.Connected && Server.Available > 0)
                    {
                        int read = Server.GetStream().Read(buffer, 0, buffer.Length);

                        // write to all clients
                        foreach (var client in clients)
                        {
                            if (client.Connected)
                                client.GetStream().Write(buffer, 0, read);
                        }
                    }
                }
                catch
                {
                }

                // read from all clients
                foreach (var client in clients)
                {
                    try
                    {
                        while (client.Connected && client.Available > 0)
                        {
                            byte[] packet = mav.ReadPacket(client.GetStream());
                            if (packet == null)
                                continue;
                            if (Server != null && Server.Connected)
                                Server.GetStream().Write(packet, 0, packet.Length);
                        }
                    }
                    catch
                    {
                        client.Close();
                    }
                }

                System.Threading.Thread.Sleep(1);
            }
        }

        static void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener) ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            Server = client;

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }

        private static void RequestCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient) ar.AsyncState;

            byte localsysid = newsysid++;

            if (client.Connected)
            {
                MAVLinkInterface mav = new MAVLinkInterface();

                mav.BaseStream = new TcpSerial() {client = client};

                try
                {
                    mav.GetParam("SYSID_THISMAV");
                }
                catch
                {
                }

                var ans = mav.setParam("SYSID_THISMAV", localsysid);

                Console.WriteLine("this mav set " + ans);

                mav = null;

                clients.Add(client);
            }
        }
    }
}