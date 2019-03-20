using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Net; // dns, ip address
using System.Net.Sockets; // tcplistner
using log4net;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MissionPlanner.Comms
{
    public class CommsNTRIP : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CommsNTRIP));
        public TcpClient client = new TcpClient();
        IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private Uri remoteUri;

        public double lat = 0;
        public double lng = 0;
        public double alt = 0;

        int retrys = 3;

        private string host;

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }

        public Stream BaseStream
        {
            get { return client.GetStream(); }
        }

        public CommsNTRIP()
        {
            ReadTimeout = 500;
        }

        public void toggleDTR()
        {
        }

        public string Port { get; set; }

        public int ReadTimeout
        {
            get; // { return client.ReceiveTimeout; }
            set; // { client.ReceiveTimeout = value; }
        }

        public int ReadBufferSize { get; set; }

        public int BaudRate { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }

        public string PortName { get; set; }

        public int BytesToRead
        {
            get
            {
                /*Console.WriteLine(DateTime.Now.Millisecond + " tcp btr " + (client.Available + rbuffer.Length - rbufferread));*/
                SendNMEA();
                return (int) client.Available;
            }
        }

        public int BytesToWrite
        {
            get { return 0; }
        }

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
                log.Warn("ntrip socket already open");
                return;
            }

            log.Info("ntrip Open");

            string url = OnSettings("NTRIP_url", "");

            if (OnInputBoxShow("remote host", "Enter url (eg http://user:pass@host:port/mount)", ref url) ==
                inputboxreturn.Cancel)
            {
                throw new Exception("Canceled by request");
            }


            OnSettings("NTRIP_url", url, true);

            int count = url.Split('@').Length - 1;

            if (count > 1)
            {
                var regex = new Regex("@");
                url = regex.Replace(url, "%40", 1);
            }

            url = url.Replace("ntrip://", "http://");

            remoteUri = new Uri(url);

            doConnect();
        }

        private byte[] TcpKeepAlive(bool On_Off, uint KeepaLiveTime, uint KeepaLiveInterval)
        {
            byte[] InValue = new byte[12];

            Array.ConstrainedCopy(BitConverter.GetBytes(Convert.ToUInt32(On_Off)), 0, InValue, 0, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(KeepaLiveTime), 0, InValue, 4, 4);
            Array.ConstrainedCopy(BitConverter.GetBytes(KeepaLiveInterval), 0, InValue, 8, 4);

            return InValue;
        }

        private void doConnect()
        {
            string usernamePassword = remoteUri.UserInfo;
            string userpass2 = Uri.UnescapeDataString(usernamePassword);
            string auth = "Authorization: Basic " +
                          Convert.ToBase64String(new ASCIIEncoding().GetBytes(userpass2)) + "\r\n";

            if (usernamePassword == "")
                auth = "";

            host = remoteUri.Host;
            Port = remoteUri.Port.ToString();

            client = new TcpClient(host, int.Parse(Port));
            client.Client.IOControl(IOControlCode.KeepAliveValues, TcpKeepAlive(true, 36000000, 3000), null);

            NetworkStream ns = client.GetStream();

            StreamWriter sw = new StreamWriter(ns);
            StreamReader sr = new StreamReader(ns);

            string line = "GET " + remoteUri.PathAndQuery + " HTTP/1.0\r\n"
                          + "User-Agent: NTRIP MissionPlanner/1.0\r\n"
                          + auth
                          + "Connection: close\r\n\r\n";

            sw.Write(line);

            log.Info(line);

            sw.Flush();

            line = sr.ReadLine();

            log.Info(line);

            if (!line.Contains("200"))
            {
                client.Dispose();

                client = new TcpClient();

                throw new Exception("Bad ntrip Responce\n\n" + line);
            }

            // vrs may take up to 60+ seconds to respond
            SendNMEA();

            VerifyConnected();
        }

        DateTime _lastnmea = DateTime.MinValue;

        private void SendNMEA()
        {
            if (lat != 0 || lng != 0)
            {
                if (_lastnmea.AddSeconds(30) < DateTime.Now)
                {
                    double latdms = (int) lat + ((lat - (int) lat) * .6f);
                    double lngdms = (int) lng + ((lng - (int) lng) * .6f);

                    var line = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "$GP{0},{1:HHmmss.ff},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", "GGA",
                        DateTime.Now.ToUniversalTime(), Math.Abs(latdms * 100).ToString("0000.00", CultureInfo.InvariantCulture), lat < 0 ? "S" : "N",
                        Math.Abs(lngdms * 100).ToString("00000.00", CultureInfo.InvariantCulture), lng < 0 ? "W" : "E", 1, 10,
                        1,  alt.ToString("0.00", CultureInfo.InvariantCulture), "M", 0, "M", "0.0", "0");

                    string checksum = GetChecksum(line);
                    WriteLine(line + "*" + checksum);

                    log.Info(line + "*" + checksum);

                    _lastnmea = DateTime.Now;
                }
            }

        }

        // Calculates the checksum for a sentence
        string GetChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            int Checksum = 0;
            foreach (char Character in sentence.ToCharArray())
            {
                switch (Character)
                {
                    case '$':
                        // Ignore the dollar sign
                        break;
                    case '*':
                        // Stop processing before the asterisk
                        continue;
                    default:
                        // Is this the first value for the checksum?
                        if (Checksum == 0)
                        {
                            // Yes. Set the checksum to the value
                            Checksum = Convert.ToByte(Character);
                        }
                        else
                        {
                            // No. XOR the checksum with this character's value
                            Checksum = Checksum ^ Convert.ToByte(Character);
                        }
                        break;
                }
            }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }

        void VerifyConnected()
        {
            if (!IsOpen)
            {
                try
                {
                    client.Dispose();
                    client = new TcpClient();
                }
                catch { }

                // this should only happen if we have established a connection in the first place
                if (client != null && retrys > 0)
                {
                    log.Info("ntrip reconnect");
                    doConnect();
                    retrys--;
                }

                throw new Exception("The ntrip is closed");
            }
        }

        public  int Read(byte[] readto,int offset,int length)
        {
            VerifyConnected();

            SendNMEA();

            try
            {
                if (length < 1) { return 0; }

				return client.Client.Receive(readto, offset, length, SocketFlags.Partial);
/*
                byte[] temp = new byte[length];
                clientbuf.Read(temp, 0, length);

                temp.CopyTo(readto, offset);

                return length;*/
            }
            catch { throw new Exception("ntrip Socket Closed"); }
        }

        public  int ReadByte()
        {
            VerifyConnected();
            int count = 0;
            while (this.BytesToRead == 0)
            {
                System.Threading.Thread.Sleep(1);
                if (count > ReadTimeout)
                    throw new Exception("ntrip Timeout on read");
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
            line = line + "\r\n";
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
            log.InfoFormat("ntrip DiscardInBuffer {0}",size);
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
                    client.Client.Dispose();
                    client.Dispose();
                }
            }
            catch { }

            try
            {
                client.Dispose();
            }
            catch { }

            client = new TcpClient();
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
