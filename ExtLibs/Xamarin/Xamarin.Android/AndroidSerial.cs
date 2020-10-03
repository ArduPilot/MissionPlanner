using Hoho.Android.UsbSerial.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MissionPlanner.Comms
{
    public class AndroidSerial : Stream, ICommsSerial
    {
        MemoryStream readbuffer = new MemoryStream(1024*10);
        private SerialInputOutputManager serialIoManager;
        private string _portName;

        public AndroidSerial(SerialInputOutputManager serialIoManager)
        {
            this.serialIoManager = serialIoManager;

            serialIoManager.DataReceived += (sender, e) =>
            {
                lock(readbuffer)
                {
                    if (readbuffer.Position == readbuffer.Length && readbuffer.Length > 0)
                        readbuffer.SetLength(0);
                    var pos = readbuffer.Position;
                    // goto end
                    readbuffer.Seek(0, SeekOrigin.End);
                    //write
                    readbuffer.Write(e.Data);
                    // seek back to readpos
                    readbuffer.Seek(pos, SeekOrigin.Begin);
                    BytesToRead += e.Data.Length;
                }
            };
            serialIoManager.ErrorReceived += (sender, e) => {  };
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;

        public Stream BaseStream => this;

        public int BaudRate
        {
            get { return serialIoManager.BaudRate;}
            set { serialIoManager.BaudRate = value; }
        }

        public int BytesToRead { get; internal set; }

        public int BytesToWrite => 0;

        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }

        public bool IsOpen => serialIoManager.IsOpen;

        public string PortName
        {
            get => _portName;
            set => _portName = value;
        }

        public int ReadBufferSize { get; set; }
        public override int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public int WriteBufferSize { get; set; }
        public override int WriteTimeout { get; set; }

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Close()
        {
            serialIoManager.Close();
        }

        public void DiscardInBuffer()
        {
            lock (readbuffer)
            {
                readbuffer.SetLength(0);
                BytesToRead = 0;
            }
        }

        public new void Dispose()
        {
            serialIoManager.Dispose();
        }

        public void Open()
        {
            serialIoManager.Open();
        }

        public override int ReadByte()
        {
            var ans = new byte[] { 0 };
            var count = Read(ans, 0, 1);
            return count > 0 ? ans[0] : -1;
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            StringBuilder build = new StringBuilder();
            for (int a = 0; a < BytesToRead; a++)
                build.Append((char) ReadByte());
            return build.ToString();
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

            return Encoding.ASCII.GetString(temp, 0, count + 1);
        }

        public void toggleDTR()
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            serialIoManager.Write(buffer.Skip(offset).Take(count).ToArray(), 250);
        }

        public void Write(string text)
        {
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public void WriteLine(string text)
        {
            text += '\n';
            Write(ASCIIEncoding.ASCII.GetBytes(text), 0, text.Length);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);
            do
            {
                Thread.Sleep(1);
            } while (BytesToRead < count && DateTime.Now < deadline);

            var read = Math.Min(count, BytesToRead);

            lock (readbuffer)
            {
                read = readbuffer.Read(buffer, offset, read);
                BytesToRead -= read;
            }

            return read;
        }

        public override void Flush()
        {
            Thread.Sleep(1);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}