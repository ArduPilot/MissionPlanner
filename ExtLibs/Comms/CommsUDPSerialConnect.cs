using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using log4net;

// dns, ip address
// tcplistner

namespace MissionPlanner.Comms
{
    public class UdpSerialConnect : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UdpSerialConnect));
        public UdpClient client = new UdpClient();
        private byte[] rbuffer = new byte[0];
        private int rbufferread;
        public IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        private int retrys = 3;

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

        public int BytesToRead => client.Available + rbuffer.Length - rbufferread;

        public int BytesToWrite => 0;

        public bool IsOpen
        {
            get
            {
                try
                {
                    return client.Client.Connected;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool DtrEnable { get; set; }

        public void Open()
        {
            if (client.Client.Connected)
            {
                log.Warn("UdpSerialConnect socket already open");
                return;
            }

            log.Info("UDP Open");

            var dest = Port;
            var host = "127.0.0.1";

            dest = OnSettings("UDP_port", dest);

            host = OnSettings("UDP_host", host);

            //if (!MainV2.MONO)
            {
                if (inputboxreturn.Cancel == OnInputBoxShow("remote host",
                        "Enter host name/ip (ensure remote end is already started)", ref host))
                    throw new Exception("Canceled by request");
                if (inputboxreturn.Cancel == OnInputBoxShow("remote Port", "Enter remote port", ref dest))
                    throw new Exception("Canceled by request");
            }

            Port = dest;

            OnSettings("UDP_port", Port, true);
            OnSettings("UDP_host", host, true);

            client = new UdpClient(host, int.Parse(Port));

            client.Connect(host, int.Parse(Port));

            VerifyConnected();
        }

        public int Read(byte[] readto, int offset, int length)
        {
            VerifyConnected();
            if (length < 1) return 0;

            // check if we are at the end of our current allocation
            if (rbufferread == rbuffer.Length)
            {
                var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);

                var r = new MemoryStream();
                do
                {
                    // read more
                    while (client.Available > 0 && r.Length < 1024 * 1024)
                    {
                        var b = client.Receive(ref RemoteIpEndPoint);
                        r.Write(b, 0, b.Length);
                    }

                    // copy mem stream to byte array.
                    rbuffer = r.ToArray();
                    // reset head.
                    rbufferread = 0;
                } while (rbuffer.Length < length && DateTime.Now < deadline);
            }

            // prevent read past end of array
            if (rbuffer.Length - rbufferread < length) length = rbuffer.Length - rbufferread;

            Array.Copy(rbuffer, rbufferread, readto, offset, length);

            rbufferread += length;

            return length;
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
            var data = new ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] write, int offset, int length)
        {
            VerifyConnected();
            try
            {
                client.Client.Send(write, length, SocketFlags.None);
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
            if (!IsOpen)
            {
                try
                {
                    client.Dispose();
                }
                catch
                {
                }

                // this should only happen if we have established a connection in the first place
                if (client != null && retrys > 0)
                {
                    log.Info("udp reconnect");
                    client = new UdpClient();
                    client.Connect(OnSettings("UDP_host", ""), int.Parse(OnSettings("UDP_port", "")));
                    retrys--;
                }

                throw new Exception("The UdpSerialConnect is closed");
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