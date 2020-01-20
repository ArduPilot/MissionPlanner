using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Exception = System.Exception;

namespace MissionPlanner.Comms
{
    public class WebSocket : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocket));
        public bool autoReconnect = true;
        public ClientWebSocket client = new ClientWebSocket();
        private DateTime lastReconnectTime = DateTime.MinValue;

        private bool reconnectnoprompt;

        public int retrys = 3;

        public WebSocket()
        {
            Port = "";
            ReadTimeout = 500;
        }

        public string Port { get; set; }

        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public Stream BaseStream => new CommsStream(this, 0);

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
                return "WS" ;
            }
            set { }
        }

        public int BytesToRead
        {
            get
            {
                lock (readMemoryStream)
                    return (int) (readMemoryStream.Length - readCursor);
            }
        }

        public int BytesToWrite => 0;

        public bool IsOpen
        {
            get
            {
                try
                {
                    return client.State == WebSocketState.Open;
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
            if (IsOpen)
            {
                log.Warn("websocket socket already open");
                return;
            }

            log.Info("websocket Open");

            var url = OnSettings("WS_url", "");

            if (OnInputBoxShow("remote host", "Enter url (eg http://user:pass@host:port/wspath)", ref url) ==
                inputboxreturn.Cancel)
                throw new Exception("Canceled by request");

            OnSettings("WS_url", url, true);

            Open(url);

            Task.Run(() =>
            {
                try
                {
                    RunReader();
                }
                catch
                {
                }
            });
        }
        
        MemoryStream readMemoryStream = new MemoryStream();
        private int readCursor = 0;
        SemaphoreSlim clientSemaphoreSlim = new SemaphoreSlim(1);

        private async void RunReader()
        {
            while (IsOpen)
            {
                try
                {
                    var buffer = new ArraySegment<byte>(new byte[1024*8]);
                    var data = await client.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);

                    if (data.MessageType == WebSocketMessageType.Binary)
                    {
                        lock (readMemoryStream)
                        {
                            readMemoryStream.Seek(0, SeekOrigin.End);
                            readMemoryStream.Write(buffer.Array, 0, data.Count);
                        }
                    }
                    else if (data.MessageType == WebSocketMessageType.Text)
                    {
                        var stringdata = ASCIIEncoding.ASCII.GetString(buffer.Array, 0, data.Count);
                        log.Info(stringdata);

                        if (stringdata[0] == '0')
                        {
                            Regex match = new Regex(@"""sid"":""([^""]+)""");
                            var mat = match.Match(stringdata);
                            if (mat.Success)
                            {
                                _url += "&sid=" + mat.Groups[1].Value;
                                //Open(_url);
                                socketio = true;
                            }
                        } 
                        else if (stringdata[0] == '3')
                        {
                            client.SendAsync(new ArraySegment<byte>("40/MAVControl,".ToCharArray().Select(a => (byte)a).ToArray()),
                                WebSocketMessageType.Text, true, CancellationToken.None);

                            client.SendAsync(new ArraySegment<byte>("5".ToCharArray().Select(a => (byte) a).ToArray()),
                                WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    else
                    {
                        log.Info("unknown websocket data");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);

                    if (autoReconnect)
                    {
                        Thread.Sleep(100);
                        Open(_url);
                    }
                }
            }
        }

        private bool socketio = false;
        private string _url;

        private async void Open(string url)
        {
            _url = url;
            client = new ClientWebSocket();
            client.ConnectAsync(new Uri(url), CancellationToken.None).GetAwaiter().GetResult();
            log.Info("WS opened " + client);
            {
                //https://github.com/socketio/engine.io-protocol
                // socket.io
                client.SendAsync(new ArraySegment<byte>("2probe".ToCharArray().Select(a => (byte) a).ToArray()),
                    WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public int Read(byte[] readto, int offset, int length)
        {
      
            try
            {
                if (length < 1) return 0;
                int read = 0;
                lock (readMemoryStream)
                {
                    readMemoryStream.Seek(readCursor, SeekOrigin.Begin);
                    read = readMemoryStream.Read(readto, offset, length);
                    readCursor += read;
                    if (BytesToRead == 0)
                    {
                        readMemoryStream.SetLength(0);
                        readCursor = 0;
                    }
                }

                return read;
            }
            catch
            {
                throw new Exception("Socket Closed");
            }
        }

        public int ReadByte()
        {
           
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
         
            var data = new byte[BytesToRead];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            var line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public void WriteLine(string line)
        {
         
            line = line + "\n";
            Write(line);
        }

        public void Write(string line)
        {
       
            var data = new ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] write, int offset, int length)
        {
    
            try
            {
                if (socketio)
                {
                    var temp = new byte[length + 1];
                    temp[0] = (byte)4;
                    Array.Copy(write, offset, temp, 1, length);
                    client.SendAsync(new ArraySegment<byte>(temp), WebSocketMessageType.Binary, true,
                        CancellationToken.None).GetAwaiter().GetResult();
                }
                else
                    client.SendAsync(new ArraySegment<byte>(write, offset, length), WebSocketMessageType.Binary, true,
                        CancellationToken.None).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                log.Error(ex);
            } //throw new Exception("Comport / Socket Closed"); }
        }

        public void DiscardInBuffer()
        {
        
            var size = BytesToRead;
            var crap = new byte[size];
            log.InfoFormat("WSSerial DiscardInBuffer {0}", size);
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
                autoReconnect = false;
                client.Dispose();
            }
            catch
            {
            }

            client = new ClientWebSocket();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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