using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Where the Cursor on Target output sends its messages. The output only sends, so none of these
    /// read, bind a well-known port they do not serve, or join a group.
    /// </summary>
    public interface ICotTransport : IDisposable
    {
        /// <summary>Opens the socket or port. Throws, with a reason a user can read, when it cannot.</summary>
        void Open();

        /// <summary>Sends one message; false when it could not go anywhere this time.</summary>
        bool Send(byte[] message);

        /// <summary>Where the messages go and how that is going, for the status line.</summary>
        string Status { get; }
    }

    /// <summary>An IPv4 address of a network adapter that is up.</summary>
    public class CotAdapter
    {
        public string Name;
        public IPAddress Address;
        /// <summary>The adapter has a default gateway: the network the PC is on, unlike a VirtualBox host-only adapter.</summary>
        public bool HasGateway;
        public bool Physical;

        public override string ToString()
        {
            return Name + " (" + Address + ")";
        }
    }

    public static class CotNetwork
    {
        public static IPAddress Resolve(string host)
        {
            IPAddress address;
            if (IPAddress.TryParse(host, out address))
                return address;
            return Dns.GetHostAddresses(host).First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }

        public static bool IsMulticast(IPAddress address)
        {
            if (address.AddressFamily != AddressFamily.InterNetwork)
                return false;
            var first = address.GetAddressBytes()[0];
            return first >= 224 && first <= 239;
        }

        /// <summary>The IPv4 addresses of the adapters that are up and can send multicast, loopback left out.</summary>
        public static List<CotAdapter> Adapters()
        {
            var list = new List<CotAdapter>();
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    if (nic.OperationalStatus != OperationalStatus.Up || !nic.SupportsMulticast ||
                        nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        continue;

                    var properties = nic.GetIPProperties();
                    var gateway = properties.GatewayAddresses.Any(g =>
                        g.Address.AddressFamily == AddressFamily.InterNetwork && !g.Address.Equals(IPAddress.Any));
                    var physical = nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                   nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                                   nic.NetworkInterfaceType == NetworkInterfaceType.GigabitEthernet;

                    foreach (var unicast in properties.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily != AddressFamily.InterNetwork)
                            continue;
                        list.Add(new CotAdapter
                            { Name = nic.Name, Address = unicast.Address, HasGateway = gateway, Physical = physical });
                    }
                }
                catch (NetworkInformationException)
                {
                }
            }

            return list;
        }

        /// <summary>
        /// The adapter to offer first for multicast: a wired or wireless adapter with a default
        /// gateway, which is the network the PC is on. Windows on its own may pick a VirtualBox or VPN
        /// adapter for the group (seen on 2026-10-27), and the TAK devices never hear it.
        /// </summary>
        public static CotAdapter DefaultAdapter(IList<CotAdapter> adapters)
        {
            return adapters.FirstOrDefault(a => a.HasGateway && a.Physical) ??
                   adapters.FirstOrDefault(a => a.HasGateway) ??
                   adapters.FirstOrDefault();
        }

        /// <summary>The message of an exception, unwrapped from the AggregateException a task gives.</summary>
        public static string Reason(Exception ex)
        {
            var aggregate = ex as AggregateException;
            if (aggregate != null && aggregate.InnerException != null)
                ex = aggregate.InnerException;
            return ex.Message;
        }

        /// <summary>Another program holds the port: Windows says so by the error code, in any language.</summary>
        public static bool PortInUse(SocketException ex)
        {
            return ex.SocketErrorCode == SocketError.AddressAlreadyInUse || ex.SocketErrorCode == SocketError.AccessDenied;
        }
    }

    /// <summary>UDP datagrams to one address, from a port Windows picks.</summary>
    public sealed class CotUdpTransport : ICotTransport
    {
        readonly string host;
        readonly int port;
        UdpClient client;
        IPEndPoint target;

        public CotUdpTransport(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void Open()
        {
            target = new IPEndPoint(CotNetwork.Resolve(host), port);
            client = new UdpClient(AddressFamily.InterNetwork);
        }

        public bool Send(byte[] message)
        {
            client.Send(message, message.Length, target);
            return true;
        }

        public string Status
        {
            get { return string.Format(Strings.CotSendingUdp, target != null ? target.ToString() : host + ":" + port); }
        }

        public void Dispose()
        {
            if (client != null)
                client.Close();
        }
    }

    /// <summary>
    /// UDP multicast, by default the TAK situational awareness group 239.2.3.1:6969. Sends from a
    /// port Windows picks and never joins the group, so WinTAK and other TAK programs on this PC keep
    /// the group's port, and still hear the messages through loopback. Each adapter gets its own
    /// socket with the adapter set as the multicast interface; the time to live stays 1, so the
    /// messages stay on the local network as TAK's own do.
    /// </summary>
    public sealed class CotMulticastTransport : ICotTransport
    {
        public const string SaGroup = "239.2.3.1";
        public const int SaPort = 6969;

        readonly IPEndPoint target;
        readonly CotAdapter adapter;
        readonly List<KeyValuePair<CotAdapter, UdpClient>> clients = new List<KeyValuePair<CotAdapter, UdpClient>>();

        /// <param name="adapter">the adapter to send on; null for every adapter that is up</param>
        public CotMulticastTransport(IPAddress group, int port, CotAdapter adapter)
        {
            target = new IPEndPoint(group, port);
            this.adapter = adapter;
        }

        public void Open()
        {
            var adapters = adapter != null ? new List<CotAdapter> { adapter } : CotNetwork.Adapters();
            if (adapters.Count == 0)
                throw new InvalidOperationException(Strings.CotNoAdapter);

            foreach (var a in adapters)
            {
                var client = new UdpClient(AddressFamily.InterNetwork);
                client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface,
                    a.Address.GetAddressBytes());
                client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                client.Client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, true);
                clients.Add(new KeyValuePair<CotAdapter, UdpClient>(a, client));
            }
        }

        public bool Send(byte[] message)
        {
            var sent = false;
            foreach (var client in clients)
            {
                try
                {
                    client.Value.Send(message, message.Length, target);
                    sent = true;
                }
                catch (SocketException)
                {
                    // an adapter that went down since; the others still send
                }
            }

            return sent;
        }

        public string Status
        {
            get
            {
                var on = adapter != null ? adapter.ToString() : string.Join(", ", clients.Select(c => c.Key.ToString()));
                return string.Format(Strings.CotSendingMulticast, target, on);
            }
        }

        public void Dispose()
        {
            foreach (var client in clients)
                client.Value.Close();
            clients.Clear();
        }
    }

    /// <summary>A TCP connection to a TAK Server plain input or a TAK device. Tries again, at most every 5 s, after it drops.</summary>
    public sealed class CotTcpClientTransport : ICotTransport
    {
        readonly string host;
        readonly int port;
        TcpClient client;
        DateTime lastAttempt;
        string lastError;

        public CotTcpClientTransport(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        string Endpoint
        {
            get { return host + ":" + port; }
        }

        public void Open()
        {
            Connect();
        }

        void Connect()
        {
            lastAttempt = DateTime.UtcNow;
            var connecting = new TcpClient(AddressFamily.InterNetwork) { NoDelay = true, SendTimeout = 2000 };
            try
            {
                if (!connecting.ConnectAsync(CotNetwork.Resolve(host), port).Wait(TimeSpan.FromSeconds(5)))
                    throw new TimeoutException(string.Format(Strings.CotNoAnswer, Endpoint));
            }
            catch
            {
                connecting.Close();
                throw;
            }

            client = connecting;
            lastError = null;
        }

        public bool Send(byte[] message)
        {
            if (client == null)
            {
                if ((DateTime.UtcNow - lastAttempt).TotalSeconds < 5)
                    return false;
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    lastError = CotNetwork.Reason(ex);
                    return false;
                }
            }

            try
            {
                client.GetStream().Write(message, 0, message.Length);
                return true;
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                client.Close();
                client = null;
                return false;
            }
        }

        public string Status
        {
            get
            {
                return client != null
                    ? string.Format(Strings.CotSendingTcp, Endpoint)
                    : string.Format(Strings.CotReconnecting, Endpoint, lastError);
            }
        }

        public void Dispose()
        {
            if (client != null)
                client.Close();
        }
    }

    /// <summary>Listens for TCP connections and sends every message to each client connected.</summary>
    public sealed class CotTcpServerTransport : ICotTransport
    {
        readonly int port;
        readonly bool thisPcOnly;
        readonly List<TcpClient> clients = new List<TcpClient>();
        TcpListener listener;

        public CotTcpServerTransport(int port, bool thisPcOnly)
        {
            this.port = port;
            this.thisPcOnly = thisPcOnly;
        }

        public void Open()
        {
            listener = new TcpListener(thisPcOnly ? IPAddress.Loopback : IPAddress.Any, port);
            try
            {
                listener.Start();
            }
            catch (SocketException ex) when (CotNetwork.PortInUse(ex))
            {
                throw new InvalidOperationException(string.Format(Strings.CotPortInUse, port), ex);
            }

            Accept();
        }

        void Accept()
        {
            try
            {
                listener.BeginAcceptTcpClient(Accepted, null);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        void Accepted(IAsyncResult result)
        {
            try
            {
                var client = listener.EndAcceptTcpClient(result);
                client.NoDelay = true;
                client.SendTimeout = 2000;
                lock (clients)
                    clients.Add(client);
            }
            catch (ObjectDisposedException)
            {
                // stopped
                return;
            }
            catch (SocketException)
            {
            }

            Accept();
        }

        public bool Send(byte[] message)
        {
            TcpClient[] now;
            lock (clients)
                now = clients.ToArray();

            var sent = false;
            foreach (var client in now)
            {
                try
                {
                    client.GetStream().Write(message, 0, message.Length);
                    sent = true;
                }
                catch (Exception)
                {
                    lock (clients)
                        clients.Remove(client);
                    client.Close();
                }
            }

            return sent;
        }

        public string Status
        {
            get
            {
                string[] who;
                lock (clients)
                    who = clients.Select(c =>
                    {
                        try
                        {
                            return c.Client.RemoteEndPoint.ToString();
                        }
                        catch (Exception)
                        {
                            return "?";
                        }
                    }).ToArray();

                return who.Length == 0
                    ? string.Format(Strings.CotWaitingForClient, port)
                    : string.Format(Strings.CotSendingToClients, who.Length, port, string.Join(", ", who));
            }
        }

        public void Dispose()
        {
            if (listener != null)
                listener.Stop();
            lock (clients)
            {
                foreach (var client in clients)
                    client.Close();
                clients.Clear();
            }
        }
    }

    /// <summary>The text on a serial port, for a radio or modem that carries it.</summary>
    public sealed class CotSerialTransport : ICotTransport
    {
        readonly string name;
        readonly int baud;
        Comms.SerialPort port;

        public CotSerialTransport(string name, int baud)
        {
            this.name = name;
            this.baud = baud;
        }

        public void Open()
        {
            port = new Comms.SerialPort { PortName = name, BaudRate = baud };
            port.Open();
        }

        public bool Send(byte[] message)
        {
            port.Write(message, 0, message.Length);
            return true;
        }

        public string Status
        {
            get { return string.Format(Strings.CotSendingSerial, name, baud); }
        }

        public void Dispose()
        {
            if (port != null)
                port.Close();
        }
    }
}
