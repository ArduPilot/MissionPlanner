using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using log4net;

using Fleck;
using System.Collections.Generic;


namespace MissionPlanner.Comms
{
    public class WebSocketServer : CommsBase, ICommsSerial, IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSocketServer));
        private bool inOpen;
        private bool closed;
        public IPEndPoint EndPoint = new IPEndPoint(IPAddress.Any, 0);

        public string ConfigRef { get; set; } = "";

        public WebSocketServer() { }

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
            get;
            set;
        }

        public int ReadBufferSize { get; set; }

        public int BaudRate { get; set; }

        public int DataBits { get; set; }

        public string PortName
        {
            get
            {
                return "WS " + EndPoint.ToString();
            }
            set{}
        }

        MemoryStream readMemoryStream = new MemoryStream();
        private int readCursor = 0;

        public int BytesToRead
        {
            get
            {
                lock (readMemoryStream)
                    return (int)(readMemoryStream.Length - readCursor);
            }
        }

        public int BytesToWrite => 0;

        public bool IsOpen
        {
            get
            {
                return allSockets.Count > 0;
            }
        }

        public bool DtrEnable { get; set; }
        public string Host { get; set; } = "";

        private List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();

        public void Open()
        {
            // Close any open connections
            allSockets.ForEach(s => s.Close());

            // New server
            var server = new Fleck.WebSocketServer("ws://" + EndPoint.ToString());

            // Open connection
            server.Start(socket => {
                socket.OnOpen = () => {
                    // Only allow single client for now
                    if (allSockets.Count != 0)
                    {
                        socket.Close();
                    }
                    else
                    {
                        allSockets.Add(socket);
                    }
                };
                socket.OnClose = () => {
                    allSockets.Remove(socket);
                };
                socket.OnBinary = b => {
                    // Add incoming data to buffer
                    lock (readMemoryStream)
                    {
                        readMemoryStream.Seek(0, SeekOrigin.End);
                        readMemoryStream.Write(b, 0, b.Length);
                    }
                };
            });
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
            var temp = new byte[length];
            Array.Copy(write, offset, temp, 0, length);
            allSockets.ForEach(s => s.Send(temp));
        }

        public void DiscardInBuffer()
        {
            var size = BytesToRead;
            var crap = new byte[size];
            log.InfoFormat("WSServer DiscardInBuffer {0}", size);
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
                    var letter = (byte)ReadByte();

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
            // Close any open connections
            allSockets.ForEach(s => s.Close());
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
            }

            // free native resources
        }
    }
}
