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
        static List<MAVLinkInterface> clients = new List<MAVLinkInterface>();

        static List<int> portlist = new EventList<int>() { 5760, 5770, 5780, 5790, 5800 };

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
                            if (client.BaseStream.IsOpen)
                                client.BaseStream.Write(buffer, 0, read);
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
                        while (client.BaseStream.IsOpen && client.BaseStream.BytesToRead > 0)
                        {
                            byte[] packet = client.readPacket();
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
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            Server = client;

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }

        private static void RequestCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient)ar.AsyncState;

            byte localsysid = newsysid++;

            if (client.Connected)
            {
                MAVLinkInterface mav = new MAVLinkInterface();

                mav.BaseStream = new TcpSerial() { client = client };

                //byte[] packet = mav.getHeartBeat();

                //Console.WriteLine("HB " + packet.Length);

                try
                {
                    mav.GetParam("SYSID_THISMAV");
                }
                catch { }

                var ans = mav.setParam("SYSID_THISMAV", localsysid);

                Console.WriteLine("this mav set " + ans);

                clients.Add(mav);
            }
        }


    }
}
