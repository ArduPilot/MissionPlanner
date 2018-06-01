using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace MissionPlanner.Comms
{
    public class CommsStream : Stream
    {
        public ICommsSerial CommsSerial { get; set; }

        long _len = 0;

        public CommsStream(ICommsSerial comm, long len)
        {
            CommsSerial = comm;
            SetLength(len);
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var read = CommsSerial.Read(buffer, offset, count);
            Position += read;
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Position;
        }

        public override void SetLength(long value)
        {
            _len = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CommsSerial.Write(buffer,offset,count);
        }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;

        public override long Length
        {
            get { return _len; }
        }

        public override long Position { get; set; }
    }
}
