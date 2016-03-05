using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Net; // dns, ip address
using System.Net.Sockets; // tcplistner
using log4net;
using System.IO;
using MissionPlanner.Controls;

namespace MissionPlanner.Comms
{
    public class UdpSerialConnect : CommsBase,  ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UdpClient client = new UdpClient();
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        byte[] rbuffer = new byte[0];
        int rbufferread = 0;

        int retrys = 3;

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream { get { return this.BaseStream; } }

        public UdpSerialConnect()
        {
            Port = "14550";
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
            get { return "UDPCl" + Port; }
            set { }
        }

        public int BytesToRead
        {
            get { return client.Available + rbuffer.Length - rbufferread; }
        }

        public int BytesToWrite { get { return 0; } }

        public bool IsOpen { get { try { return client.Client.Connected; } catch { return false; } } }

        public bool DtrEnable
        {
            get;
            set;
        }

        public void Open()
        {
            if (client.Client.Connected)
            {
                log.Warn("UdpSerialConnect socket already open");
                return;
            }

            log.Info("UDP Open");

            string dest = Port;
            string host = "127.0.0.1";

            dest = OnSettings("UDP_port", dest);

            host = OnSettings("UDP_host", host);

            //if (!MainV2.MONO)
            {
                if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("remote host", "Enter host name/ip (ensure remote end is already started)", ref host))
                {
                    throw new Exception("Canceled by request");
                }
                if (System.Windows.Forms.DialogResult.Cancel == InputBox.Show("remote Port", "Enter remote port", ref dest))
                {
                    throw new Exception("Canceled by request");
                }
            }

            Port = dest;

            OnSettings("UDP_port", Port, true);
            OnSettings("UDP_host", host, true);

            client = new UdpClient(host, int.Parse(Port));

            client.Connect(host, int.Parse(Port));

            VerifyConnected();

            return;
        }

        void VerifyConnected()
        {
            if (!IsOpen)
            {
                try
                {
                    client.Close();
                }
                catch { }

                // this should only happen if we have established a connection in the first place
                if (client != null && retrys > 0)
                {
                    log.Info("udp reconnect");
                    client.Connect(OnSettings("UDP_host", ""), int.Parse(OnSettings("UDP_port", "")));
                    retrys--;
                }

                throw new Exception("The UdpSerialConnect is closed");
            }
        }

        public int Read(byte[] readto, int offset, int length)
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
                            Byte[] b = client.Receive(ref RemoteIpEndPoint);
                            r.Write(b, 0, b.Length);
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
                    throw new Exception("UdpSerialConnect Timeout on read");
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
            try
            {
                client.Client.Send(write, length,SocketFlags.None);
            }
            catch { }//throw new Exception("Comport / Socket Closed"); }
        }

        public  void DiscardInBuffer()
        {
            VerifyConnected();
            int size = (int)client.Available;
            byte[] crap = new byte[size];
            log.InfoFormat("UdpSerialConnect DiscardInBuffer {0}",size);
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
            try
            {
                if (client.Client != null && client.Client.Connected)
                {
                    client.Client.Close();
                    client.Close();
                }
            }
            catch { }

            try
            {
                client.Close();
            }
            catch { }

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
