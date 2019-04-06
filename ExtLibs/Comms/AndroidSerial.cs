using Hoho.Android.UsbSerial.Util;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MissionPlanner.Comms
{
    public class AndroidSerial : Stream, ICommsSerial
    {
        public CircularBuffer<byte> readbuffer = new CircularBuffer<byte>(1024 * 10);
        public CircularBuffer<byte> writebuffer = new CircularBuffer<byte>(1024 * 10);
        private SerialInputOutputManager serialIoManager;

        public AndroidSerial(SerialInputOutputManager serialIoManager)
        {
            this.serialIoManager = serialIoManager;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;

        public Stream BaseStream => this;

        public int BaudRate { get; set; }

        public int BytesToRead => readbuffer.Length();

        public int BytesToWrite => 0;

        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }

        public bool IsOpen => serialIoManager.IsOpen;

        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Close()
        {
            serialIoManager.Close();
        }

        public void DiscardInBuffer()
        {
            readbuffer.Clear();
        }

        public void Dispose()
        {
            serialIoManager.Dispose();
        }

        public void Open()
        {
            serialIoManager.Open();
        }

        public int ReadByte()
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
            for (int a = 0; a < readbuffer.Length(); a++)
                build.Append((char)readbuffer.Read());
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
            var read = Math.Min(count, readbuffer.Length());
            for (int a = 0; a < read; a++)
            {
                buffer[offset + a] = readbuffer.Read();
            }
            return read;
        }

        public override void Flush()
        {
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