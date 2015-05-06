using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using Core.ExtendedObjects;
using webapi;

namespace MissionPlanner.Utilities
{
    public class StreamCombiner
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static List<int> portlist = new EventList<int>() { 5760, 5770, 5780, 5790, 5800 };

        static TcpListener listener = new TcpListener(IPAddress.Loopback, 5750);

        private static TcpClient Server = null;

        public static Thread th = null;

        private static bool run = false;

        public static void Start()
        {
            if (run == true)
            {
                run = false;
                clients.Clear();
                return;
            }

            listener.Start();

            listener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, listener);

            foreach (var portno in  portlist)
            {
                TcpClient cl = new TcpClient();

                cl.BeginConnect(IPAddress.Loopback, portno, RequestCallback, cl);
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
        }

        private static void mainloop()
        {
            run = true;

            byte[] buffer = new byte[1024];

            while (run)
            {
                try
                {
                    if (Server.Connected && Server.Available > 0)
                    {
                        int read = Server.GetStream().Read(buffer, 0, buffer.Length);

                        // write to all clients
                        foreach (TcpClient client in clients)
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
                foreach (TcpClient client in clients)
                {
                    try
                    {
                        if (client.Connected && client.Available > 0)
                        {
                            int read = client.GetStream().Read(buffer, 0, buffer.Length);
                            if (Server != null && Server.Connected)
                                Server.GetStream().Write(buffer, 0, read);
                        }
                    } 
                    catch
                    {
                        client.Close();
                    }
                }
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
            TcpClient client = (TcpClient) ar.AsyncState;

            if (client.Connected)
                clients.Add(client);
        }

   
    }
}
