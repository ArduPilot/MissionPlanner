using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class StreamCombiner
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static TcpListener listener = new TcpListener(IPAddress.Loopback, 5750);

        private static TcpClient Server = null;

        public static Thread th = null;

        private static bool run = false;

        static uint newsysid = 1;

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

            foreach (var portno in Range(5760, 10, 100))
            {
                TcpClient cl = new TcpClient();

                cl.BeginConnect(IPAddress.Loopback, portno, RequestCallback, cl);

                System.Threading.Thread.Sleep(100);
            }

            th = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "stream combiner"
            };
            th.Start();

            //MainV2.comPort.BaseStream = new TcpSerial() {client = new TcpClient("127.0.0.1", 5750) };

            //MainV2.instance.doConnect(MainV2.comPort, "preset", "5750");
        }

        private static IEnumerable<int> Range(int start, int inc, int count)
        {
            List<int> ans = new List<int>();

            for (int a = 0; a < count; a++)
            {
                ans.Add(start + inc * a);
            }

            return ans;
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
                    if (Server == null)
                    {
                        System.Threading.Thread.Sleep(1);
                        continue;
                    }

                    while (Server.Connected && Server.Available > 0)
                    {
                        int read = Server.GetStream().Read(buffer, 0, buffer.Length);

                        // write to all clients
                        foreach (var client in clients.ToArray())
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
                foreach (var client in clients.ToArray())
                {
                    try
                    {
                        while (client.Connected && client.Available > 0)
                        {
                            var packet = mav.ReadPacket(client.GetStream());
                            if (packet == null)
                                continue;
                            if (Server != null && Server.Connected)
                                Server.GetStream().Write(packet.buffer, 0, packet.Length);
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

        static object locker = new object();

        private static uint AllocateSystemId(uint maximum)
        {
            lock (locker)
            {
                if (newsysid > maximum)
                    throw new InvalidOperationException("No representable system IDs remain");
                return newsysid++;
            }
        }

        private static void RequestCallback(IAsyncResult ar)
        {
            TcpClient client = (TcpClient) ar.AsyncState;

            if (client.Connected)
            {
                MAVLinkInterface mav = new MAVLinkInterface();
                mav.BaseStream = new TcpSerial() {client = client};
                uint localsysid;
                try
                {
                    var heartbeat = mav.getHeartBeat();
                    if (heartbeat == MAVLink.MAVLinkMessage.Invalid)
                        throw new TimeoutException("No vehicle heartbeat");
                    mav.sysidcurrent = heartbeat.sysid;
                    mav.compidcurrent = heartbeat.compid;
                    string parameter = "MAV_SYSID";
                    try
                    {
                        mav.GetParam(mav.sysidcurrent, (byte)mav.compidcurrent, parameter);
                    }
                    catch (TimeoutException)
                    {
                        parameter = "SYSID_THISMAV";
                        mav.GetParam(mav.sysidcurrent, (byte)mav.compidcurrent, parameter);
                    }
                    // PARAM_SET uses float32; only allocate consecutive exact IDs.
                    localsysid = AllocateSystemId(parameter == "MAV_SYSID" ? 0xFFFFFFU : 255U);
                    mav.sendPacket(new MAVLink.mavlink_param_set_t
                    {
                        param_id = System.Text.Encoding.ASCII.GetBytes(parameter.PadRight(16, '\0')),
                        param_value = localsysid,
                        param_type = (byte)mav.MAV.param_types[parameter],
                        target_system = (byte)mav.sysidcurrent,
                        target_component = (byte)mav.compidcurrent
                    }, mav.sysidcurrent, mav.compidcurrent);
                    // ArduPilot changes its source ID immediately. Read back at
                    // the new address instead of waiting for an ACK at the old one.
                    mav.sysidcurrent = localsysid;
                    if (mav.GetParam(parameter) != localsysid)
                        throw new InvalidOperationException("Vehicle rejected its allocated system ID");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cannot assign vehicle system ID: " + ex.Message);
                    client.Close();
                    return;
                }

                Connect?.Invoke(mav, localsysid.ToString());
            }

        }

        public static event Action<MAVLinkInterface, string> Connect;
    }
}