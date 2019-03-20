using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace MissionPlanner.Comms
{
    public class CommsInjection: ICommsSerial
    {
        private CircularBuffer<byte> _buffer = new CircularBuffer<byte>(1024 * 100);

        public void Close()
        {
            _buffer.Clear();
        }

        public void DiscardInBuffer()
        {
            _buffer.Clear();
        }

        public void Open()
        {
            _buffer.Clear();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int counttimeout = 0;
            while (this.BytesToRead == 0)
            {
                System.Threading.Thread.Sleep(1);
                if (counttimeout > ReadTimeout)
                    throw new Exception("CommsInjection Timeout on read");
                counttimeout++;
            }

            var read = Math.Min(count, _buffer.Length());
            for (int i = 0; i < read; i++)
            {
                buffer[offset + i] = _buffer.Read();
            }

            return read;
        }

        public int ReadByte()
        {
            byte[] buffer = new byte[1];
            Read(buffer, 0, 1);
            return buffer[0];
        }

        public int ReadChar()
        {
            return ReadByte();
        }

        public string ReadExisting()
        {
            byte[] data = new byte[0];
            if (data.Length > 0)
                Read(data, 0, data.Length);

            string line = Encoding.ASCII.GetString(data, 0, data.Length);

            return line;
        }

        public string ReadLine()
        {
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
                }
                else
                {
                    timeout++;
                    System.Threading.Thread.Sleep(5);
                }
            }

            Array.Resize<byte>(ref temp, count + 1);

            return Encoding.ASCII.GetString(temp, 0, temp.Length);
        }

        public void WriteLine(string line)
        {
            line = line + "\n";
            Write(line);
        }

        public void Write(string line)
        {
            byte[] data = new System.Text.ASCIIEncoding().GetBytes(line);
            Write(data, 0, data.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            foreach (var b in buffer.Skip(offset).Take(count))
            {
                _buffer.Add(b);
            }
        }


        public void toggleDTR()
        {
            
        }

        public Stream BaseStream { get; }
        public int BaudRate { get; set; }
        public int BytesToRead
        {
            get { return _buffer.Length(); }
        }
        public int BytesToWrite { get; }
        public int DataBits { get; set; }
        public bool DtrEnable { get; set; }
        public bool IsOpen
        {
            get { return true; }
        }
        public Parity Parity { get; set; }
        public string PortName { get; set; }
        public int ReadBufferSize { get; set; }
        public int ReadTimeout { get; set; }
        public bool RtsEnable { get; set; }
        public StopBits StopBits { get; set; }
        public int WriteBufferSize { get; set; }
        public int WriteTimeout { get; set; }
    }
}