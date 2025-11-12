using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;

// dns, ip address
// tcplistner

namespace MissionPlanner.Comms
{
    public class UdpSerial : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public readonly List<IPEndPoint> EndPointList = new List<IPEndPoint>();

        private bool _isopen;

        public bool CancelConnect = false;
        /// <summary>
        /// add to EndPointList if need when injecting
        /// </summary>
        public UdpClient client = new UdpClient();

        private MemoryStream rbuffer = new MemoryStream();

        /// <summary>
        ///     this is the remote endpoint we send messages too. this class does not support multiple remote endpoints.
        /// </summary>
        public IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public string ConfigRef { get; set; } = "";

        public static bool IsInRange(string startIpAddr, string endIpAddr, string address)
        {
            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ip = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipStart && ip <= ipEnd;
        }

        public UdpSerial()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Port = "14550";
            ReadTimeout = 500;
            MCast = "0.0.0.0";
        }

        public UdpSerial(UdpClient client)
        {
            this.client = client;
            _isopen = true;
            ReadTimeout = 500;
        }

        public string Port { get; set; }
        public string MCast { get; set; }

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream => new UDPStream(this);

        public void toggleDTR()
        {
        }

        public int ReadTimeout
        {
            get; // { return client.ReceiveTimeout; }
            set; // { client.ReceiveTimeout = value; }
        }

        public int ReadBufferSize { get; set; }

        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public string PortName
        {
            get => "UDP" + Port;
            set { }
        }

        public int BytesToRead => (int)(client.Available + rbuffer.Length - rbuffer.Position);

        public int BytesToWrite => 0;

        public bool IsOpen
        {
            get
            {
                if (client?.Client == null) return false;
                return _isopen;
            }
            set => _isopen = value;
        }

        public bool DtrEnable { get; set; }

        public void Open()
        {
            if (client.Client.Connected || IsOpen)
            {
                log.Info("UDPSerial socket already open");
                return;
            }

            client.Close();

            var dest = Port;
            var multicast_source = MCast;

            dest = OnSettings("UDP_port" + ConfigRef, dest);
            multicast_source = OnSettings("Mcast_rx" + ConfigRef, multicast_source);

            if (inputboxreturn.Cancel == OnInputBoxShow("Listern Port",
                    "Enter Local port (ensure remote end is already sending)", ref dest)) return;
            Port = dest;

            if (inputboxreturn.Cancel == OnInputBoxShow("Listen Multicast?",
                    "Enter Multicast to Subscribe too", ref multicast_source)) return;
            MCast = multicast_source;


            OnSettings("UDP_port" + ConfigRef, Port, true);
            OnSettings("Mcast_rx" + ConfigRef, MCast, true);

            //######################################

            try
            {
                if (client != null) client.Close();
            }
            catch
            {
            }

            client = new UdpClient(int.Parse(Port));
            if (IsInRange("224.0.0.0", "239.255.255.255", MCast))
            {
                client.JoinMulticastGroup(IPAddress.Parse(MCast));
            }

            while (true)
            {
                Thread.Sleep(500);

                if (CancelConnect)
                {
                    try
                    {
                        client.Close();
                    }
                    catch
                    {
                    }

                    return;
                }

                if (BytesToRead > 0)
                    break;
            }

            if (BytesToRead == 0)
                return;

            try
            {
                // reset any previous connection
                RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                client.Receive(ref RemoteIpEndPoint);
                log.InfoFormat("UDPSerial connecting to {0} : {1}", RemoteIpEndPoint.Address, RemoteIpEndPoint.Port);
                EndPointList.Add(RemoteIpEndPoint);
                _isopen = true;
            }
            catch (Exception ex)
            {
                if (client != null && client.Client.Connected) client.Close();
                log.Info(ex.ToString());
                //CustomMessageBox.Show("Please check your Firewall settings\nPlease try running this command\n1.    Run the following command in an elevated command prompt to disable Windows Firewall temporarily:\n    \nNetsh advfirewall set allprofiles state off\n    \nNote: This is just for test; please turn it back on with the command 'Netsh advfirewall set allprofiles state on'.\n", "Error");
                throw new Exception("The socket/UDPSerial is closed " + ex);
            }
        }

        public int Read(byte[] readto, int offset, int length)
        {
            VerifyConnected();
            if (length < 1) return 0;

            var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);

            lock (rbuffer)
            {
                if (rbuffer.Position == rbuffer.Length)
                    rbuffer.SetLength(0);

                var position = rbuffer.Position;

                while ((rbuffer.Length - rbuffer.Position) < length && DateTime.Now < deadline)
                {
                    // read more
                    while (client.Available > 0 && (rbuffer.Length - rbuffer.Position) < length)
                    {
                        var currentRemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        // assumes the udp packets are mavlink aligned, if we are receiving from more than one source
                        var b = client.Receive(ref currentRemoteIpEndPoint);
                        rbuffer.Seek(0, SeekOrigin.End);
                        rbuffer.Write(b, 0, b.Length);
                        rbuffer.Seek(position, SeekOrigin.Begin);

                        if (!EndPointList.Contains(currentRemoteIpEndPoint))
                            EndPointList.Add(currentRemoteIpEndPoint);
                    }

                    Thread.Yield();
                }

                // prevent read past end of array
                if (rbuffer.Length - rbuffer.Position < length)
                    length = (int)(rbuffer.Length - rbuffer.Position);

                return rbuffer.Read(readto, offset, length);
            }
        }

        public int ReadByte()
        {
            VerifyConnected();
            var count = 0;
            while (BytesToRead == 0)
            {
                Thread.Sleep(1);
                if (count > ReadTimeout)
                    throw new Exception("NetSerial Timeout on read");
                count++;
            }

            var buffer = new byte[1];
            Read(buffer, 0, 1);
            return buffer[0];
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            VerifyConnected();
            var data = new byte[client.Available];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            var line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public void WriteLine(string line)
        {
            VerifyConnected();
            line = line + "\n";
            Write(line);
        }

        public void Write(string line)
        {
            VerifyConnected();
            var data = new ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] write, int offset, int length)
        {
            VerifyConnected();
            // this is not ideal. but works
            foreach (var ipEndPoint in EndPointList)
                try
                {
                    client.Send(write, length, ipEndPoint);
                }
                catch
                {
                } //throw new Exception("Comport / Socket Closed"); }
        }

        public void DiscardInBuffer()
        {
            VerifyConnected();
            var size = client.Available;
            var crap = new byte[size];
            log.InfoFormat("UdpSerial DiscardInBuffer {0}", size);
            Read(crap, 0, size);
        }

        public string ReadLine()
        {
            var temp = new byte[4000];
            var count = 0;
            var timeout = 0;

            while (timeout <= 100)
            {
                if (!IsOpen) break;
                if (BytesToRead > 0)
                {
                    var letter = (byte) ReadByte();

                    temp[count] = letter;

                    if (letter == '\n') // normal line
                        break;

                    count++;
                    if (count == temp.Length)
                        break;
                    timeout = 0;
                }
                else
                {
                    timeout++;
                    Thread.Sleep(5);
                }
            }

            Array.Resize(ref temp, count + 1);

            return Encoding.ASCII.GetString(temp, 0, temp.Length);
        }

        public void Close()
        {
            _isopen = false;
            if (client != null)
            {
                if (IsInRange("224.0.0.0", "239.255.255.255", MCast))
                {
                    client.DropMulticastGroup(IPAddress.Parse(MCast));
                }
                client.Close();
            }

            client = new UdpClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void VerifyConnected()
        {
            if (client == null || !IsOpen)
            {
                Close();
                throw new Exception("The socket/serialproxy is closed");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                Close();
                client = null;
            }

            // free native resources
        }
    }
}