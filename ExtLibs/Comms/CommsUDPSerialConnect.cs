using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

// dns, ip address
// tcplistner

namespace MissionPlanner.Comms
{
    public class UdpSerialConnect : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UdpSerialConnect));
        /// <summary>
        /// set hostEndPoint as well, if injecting
        /// </summary>
        public UdpClient client = new UdpClient();
        private MemoryStream rbuffer = new MemoryStream();
        public IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        private int retrys = 3;
        public IPEndPoint hostEndPoint;

        public string ConfigRef { get; set; } = "";

        public UdpSerialConnect()
        {
            Port = "14550";
            ReadTimeout = 500;
        }

        public string Port { get; set; }

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream => Stream.Null;

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
            get => "UDPCl" + Port;
            set { }
        }

        public int BytesToRead => (int)(client.Available + rbuffer.Length - rbuffer.Position);

        public int BytesToWrite => 0;

        public bool IsOpen { get; set; }

        public bool DtrEnable { get; set; }

        public void Open(string host, string port)
        {
            Port = port;

            OnSettings("UDP_port" + ConfigRef, Port, true);
            OnSettings("UDP_host" + ConfigRef, host, true);

            IPAddress addr;

            if (IPAddress.TryParse(host, out addr))
            {
                hostEndPoint = new IPEndPoint(addr, int.Parse(Port));
            }
            else
            {
                hostEndPoint = new IPEndPoint(Dns.GetHostEntry(host).AddressList.First(), int.Parse(Port));
            }

            if (IsInRange("224.0.0.0", "239.255.255.255", hostEndPoint.Address.ToString()))
            {
                log.Info($"UdpSerialConnect bind to port {Port}");

                client = new UdpClient();
                client.ExclusiveAddressUse = false;
                client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                client.Connect(hostEndPoint.Address, hostEndPoint.Port);

                IsOpen = true;

                Task.Run(() => {
                    while (IsOpen)
                    {
                        log.Info($"UdpSerialConnect join multicast group {host}");
                        try
                        {
                            client.JoinMulticastGroup(IPAddress.Parse(host));
                        } catch { return; }

                        Thread.Sleep(30 * 1000);
                    }
                });
                log.Info($"UdpSerialConnect default endpoint {hostEndPoint}");
            }
            else
            {
                client = new UdpClient(hostEndPoint.AddressFamily);
            }

            IsOpen = true;

            VerifyConnected();
        }

        public void Open()
        {
            if (IsOpen)
            {
                log.Warn("UdpSerialConnect socket already open");
                return;
            }

            log.Info("UDP Open");

            var dest = Port;
            var host = "127.0.0.1";

            dest = OnSettings("UDP_port" + ConfigRef, dest);

            host = OnSettings("UDP_host" + ConfigRef, host);

            //if (!MainV2.MONO)
            {
                if (inputboxreturn.Cancel == OnInputBoxShow("remote host",
                        "Enter host name/ip (ensure remote end is already started)", ref host))
                    throw new Exception("Canceled by request");
                if (inputboxreturn.Cancel == OnInputBoxShow("remote Port", "Enter remote port", ref dest))
                    throw new Exception("Canceled by request");
            }

            Open(host, dest);
        }

        public static bool IsInRange(string startIpAddr, string endIpAddr, string address)
        {
            long ipStart = BitConverter.ToInt32(IPAddress.Parse(startIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ipEnd = BitConverter.ToInt32(IPAddress.Parse(endIpAddr).GetAddressBytes().Reverse().ToArray(), 0);

            long ip = BitConverter.ToInt32(IPAddress.Parse(address).GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipStart && ip <= ipEnd;
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

                        RemoteIpEndPoint = currentRemoteIpEndPoint;
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
                    throw new Exception("UdpSerialConnect Timeout on read");
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
            var data = ASCIIEncoding.UTF8.GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] write, int offset, int length)
        {
            VerifyConnected();
            try
            {
                client.Client.SendTo(write, length, SocketFlags.None, hostEndPoint);
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
            log.InfoFormat("UdpSerialConnect DiscardInBuffer {0}", size);
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
            IsOpen = false;

            try
            {
                if (hostEndPoint != null)
                    client.DropMulticastGroup(hostEndPoint.Address);
            }
            catch
            {
            }

            try
            {
                if (client.Client != null && client.Client.Connected)
                {
                    client.Client.Dispose();
                    client.Dispose();
                }
            }
            catch
            {
            }

            try
            {
                client.Dispose();
            }
            catch
            {
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