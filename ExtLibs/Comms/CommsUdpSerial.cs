using System.Reflection;
using System.Text;
using System.Net; // dns, ip address
using System.Net.Sockets; // tcplistner
using log4net;
using System.IO.Ports;
using System.IO;
using System;
using System.Collections.Generic;

namespace MissionPlanner.Comms
{
    public class UdpSerial : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UdpClient client = new UdpClient();
        /// <summary>
        /// this is the remote endpoint we send messages too. this class does not support multiple remote endpoints.
        /// </summary>
        public IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

        private List<IPEndPoint> EndPointList = new List<IPEndPoint>();

        byte[] rbuffer = new byte[0];
        int rbufferread = 0;

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream { get { return Stream.Null; } }

        public UdpSerial()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Port = "14550";
            ReadTimeout = 500;
        }

        public UdpSerial(UdpClient client)
        {
            this.client = client;
            _isopen = true;
            ReadTimeout = 500;
        }

        public void toggleDTR()
        {
        }

        public string Port { get; set; }

        public int ReadTimeout
        {
            get;// { return client.ReceiveTimeout; }
            set;// { client.ReceiveTimeout = value; }
        }

        public int ReadBufferSize {get;set;}

        public int BaudRate { get; set; }
        public StopBits StopBits { get; set; }
        public  Parity Parity { get; set; }
        public  int DataBits { get; set; }

        public string PortName {
            get { return "UDP" + Port; }
            set { }
        }

        public  int BytesToRead
        {
            get { return client.Available + rbuffer.Length - rbufferread; }
        }

        public int BytesToWrite { get {return 0;} }

        private bool _isopen = false;
        public bool IsOpen { get { if (client.Client == null) return false; return _isopen; } set { _isopen = value; } }

        public bool DtrEnable
        {
            get;
            set;
        }

        public bool CancelConnect = false;

        public void Open()
        {
            if (client.Client.Connected || IsOpen)
            {
                log.Info("UDPSerial socket already open");
                return;
            }

            client.Close();

            string dest = Port;

            dest = OnSettings("UDP_port", dest);

            if (inputboxreturn.Cancel == OnInputBoxShow("Listern Port", "Enter Local port (ensure remote end is already sending)", ref dest))
            {
                return;
            }
            Port = dest;

            OnSettings("UDP_port", Port, true);

            //######################################

            try
            {
                if (client != null)
                {
                    client.Close();
                }
            }
            catch { }

            client = new UdpClient(int.Parse(Port));

            while (true)
            {
                System.Threading.Thread.Sleep(500);

                if (CancelConnect)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
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
                if (client != null && client.Client.Connected)
                {
                    client.Close();
                }
                log.Info(ex.ToString());
                //CustomMessageBox.Show("Please check your Firewall settings\nPlease try running this command\n1.    Run the following command in an elevated command prompt to disable Windows Firewall temporarily:\n    \nNetsh advfirewall set allprofiles state off\n    \nNote: This is just for test; please turn it back on with the command 'Netsh advfirewall set allprofiles state on'.\n", "Error");
                throw new Exception("The socket/UDPSerial is closed " + ex);
            }
        }

        void VerifyConnected()
        {
            if (client == null || !IsOpen)
            {
                this.Close();
                throw new Exception("The socket/serialproxy is closed");
            }
        }

        public int Read(byte[] readto,int offset,int length)
        {
            VerifyConnected();
            try
            {
                if (length < 1) { return 0; }

                // check if we are at the end of our current allocation
                if (rbufferread == rbuffer.Length)
                {
                    DateTime deadline = DateTime.Now.AddMilliseconds(this.ReadTimeout);

                    MemoryStream r = new MemoryStream();
                    do
                    {
                        // read more
                        while (client.Available > 0 && r.Length < (1024 * 1024))
                        {
                            var currentRemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                            // assumes the udp packets are mavlink aligned, if we are receiving from more than one source
                            Byte[] b = client.Receive(ref currentRemoteIpEndPoint);
                            r.Write(b, 0, b.Length);

                            if(!EndPointList.Contains(currentRemoteIpEndPoint))
                                EndPointList.Add(currentRemoteIpEndPoint);
                        }
                        // copy mem stream to byte array.
                        rbuffer = r.ToArray();
                        // reset head.
                        rbufferread = 0;
                    } while (rbuffer.Length < length && DateTime.Now < deadline);
                }

                // prevent read past end of array
                if ((rbuffer.Length - rbufferread) < length)
                {
                    length = (rbuffer.Length - rbufferread);
                }

                Array.Copy(rbuffer, rbufferread, readto, offset, length);

                rbufferread += length;

                return length;
            }
            catch { throw; }
        }

        public  int ReadByte()
        {
            VerifyConnected();
            int count = 0;
            while (this.BytesToRead == 0)
            {
                System.Threading.Thread.Sleep(1);
                if (count > ReadTimeout)
                    throw new Exception("NetSerial Timeout on read");
                count++;
            }
            byte[] buffer = new byte[1];
            Read(buffer, 0, 1);
            return buffer[0];
        }

        public  int ReadChar()
        {
            return ReadByte();
        }

        public  string ReadExisting() 
        {
            VerifyConnected();
            byte[] data = new byte[client.Available];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            string line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public  void WriteLine(string line)
        {
            VerifyConnected();
            line = line + "\n";
            Write(line);
        }

        public  void Write(string line)
        {
            VerifyConnected();
            byte[] data = new System.Text.ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public  void Write(byte[] write, int offset, int length)
        {
            VerifyConnected();
            // this is not ideal. but works
            foreach (var ipEndPoint in EndPointList)
            {
                try
                {
                    client.Send(write, length, ipEndPoint);
                }
                catch { }//throw new Exception("Comport / Socket Closed"); }
            }
        }

        public  void DiscardInBuffer()
        {
            VerifyConnected();
            int size = client.Available;
            byte[] crap = new byte[size];
            log.InfoFormat("UdpSerial DiscardInBuffer {0}",size);
            Read(crap, 0, size);
        }

        public  string ReadLine() {
            byte[] temp = new byte[4000];
            int count = 0;
            int timeout = 0;

            while (timeout <= 100)
            {
                if (!this.IsOpen) { break; }
                if (this.BytesToRead > 0)
                {
                    byte letter = (byte)this.ReadByte();

                    temp[count] = letter;

                    if (letter == '\n') // normal line
                    {
                        break;
                    }


                    count++;
                    if (count == temp.Length)
                        break;
                    timeout = 0;
                } else {
                    timeout++;
                    System.Threading.Thread.Sleep(5);
                }
            }

            Array.Resize<byte>(ref temp, count + 1);

            return Encoding.ASCII.GetString(temp, 0, temp.Length);
        }

        public void Close()
        {
            _isopen = false;
            if (client != null)
            {
                client.Close();
            }

            client = new UdpClient();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                this.Close();
                client = null;
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
