using System.IO;

namespace MissionPlanner.Comms
{
    public class CommsStream : Stream
    {
        private long _len;

        public CommsStream(ICommsSerial comm, long len)
        {
            CommsSerial = comm;
            SetLength(len);
        }

        public ICommsSerial CommsSerial { get; set; }

        public override bool CanRead { get; } = true;
        public override bool CanSeek { get; } = false;
        public override bool CanWrite { get; } = true;

        public override long Length => _len;

        public override long Position { get; set; }

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
            CommsSerial.Write(buffer, offset, count);
        }
    }
}