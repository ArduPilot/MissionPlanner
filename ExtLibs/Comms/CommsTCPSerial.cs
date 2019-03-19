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
    public class TcpSerial : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TcpSerial));
        public bool autoReconnect;
        public TcpClient client = new TcpClient();
        private bool inOpen;
        private DateTime lastReconnectTime = DateTime.MinValue;

        private bool reconnectnoprompt;
        private IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        public int retrys = 3;

        public TcpSerial()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Port = "5760";
            ReadTimeout = 500;
        }

        public string Port { get; set; }

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream => client.GetStream();

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
            get
            {
                if (client != null && client.Client != null && client.Client.RemoteEndPoint != null)
                    return "TCP" + ((IPEndPoint) client.Client.RemoteEndPoint).Port;
                return "TCP" + Port;
            }
            set { }
        }

        public int BytesToRead => client.Available;

        public int BytesToWrite => 0;

        public bool IsOpen
        {
            get
            {
                try
                {
                    if (client == null) return false;
                    if (client.Client == null) return false;

                    if (autoReconnect && client.Client.Connected == false && !inOpen)
                        doAutoReconnect();

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
            try
            {
                inOpen = true;

                if (client.Client.Connected)
                {
                    log.Warn("tcpserial socket already open");
                    return;
                }

                var dest = Port;
                var host = "127.0.0.1";

                dest = OnSettings("TCP_port", dest);

                host = OnSettings("TCP_host", host);

                if (!reconnectnoprompt)
                {
                    if (inputboxreturn.Cancel == OnInputBoxShow("remote host",
                            "Enter host name/ip (ensure remote end is already started)", ref host))
                        throw new Exception("Canceled by request");
                    if (inputboxreturn.Cancel == OnInputBoxShow("remote Port", "Enter remote port", ref dest))
                        throw new Exception("Canceled by request");
                }

                Port = dest;

                log.InfoFormat("TCP Open {0} {1}", host, Port);

                OnSettings("TCP_port", Port, true);
                OnSettings("TCP_host", host, true);

                client = new TcpClient(host, int.Parse(Port));

                client.NoDelay = true;
                client.Client.NoDelay = true;

                VerifyConnected();

                reconnectnoprompt = true;
            }
            catch
            {
                // disable if the first connect fails
                autoReconnect = false;
                throw;
            }
            finally
            {
                inOpen = false;
            }
        }

        public int Read(byte[] readto, int offset, int length)
        {
            VerifyConnected();
            try
            {
                if (length < 1) return 0;

                return client.Client.Receive(readto, offset, length, SocketFlags.None);
                /*
                                byte[] temp = new byte[length];
                                clientbuf.Read(temp, 0, length);

                                temp.CopyTo(readto, offset);

                                return length;*/
            }
            catch
            {
                throw new Exception("Socket Closed");
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
            log.InfoFormat("TcpSerial DiscardInBuffer {0}", size);
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

            client = new TcpClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void doAutoReconnect()
        {
            if (!autoReconnect)
                return;
            try
            {
                if (DateTime.Now > lastReconnectTime)
                {
                    try
                    {
                        client.Dispose();
                    }
                    catch
                    {
                    }

                    client = new TcpClient();

                    var host = OnSettings("TCP_host", "");
                    var port = int.Parse(OnSettings("TCP_port", ""));

                    log.InfoFormat("doAutoReconnect {0} {1}", host, port);

                    var task = client.ConnectAsync(host, port);

                    lastReconnectTime = DateTime.Now.AddSeconds(5);
                }
            }
            catch
            {
            }
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
                    log.Info("tcp reconnect");
                    client = new TcpClient();
                    client.Connect(OnSettings("TCP_host", ""), int.Parse(OnSettings("TCP_port", "")));
                    retrys--;
                }

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