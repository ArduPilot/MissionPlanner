using System;
using System.IO;

namespace CircularBuffer
{
    public class CircularStream : Stream
    {
        private CircularBuffer<byte> buffer;

        public CircularStream(int bufferCapacity)
            : base()
        {
            buffer = new CircularBuffer<byte>(bufferCapacity);
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Capacity
        {
            get { return buffer.Capacity; }
            set { buffer.Capacity = value; }
        }

        public override long Length
        {
            get { return buffer.Size; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public byte[] GetBuffer()
        {
            return buffer.GetBuffer();
        }

        public byte[] ToArray()
        {
            return buffer.ToArray();
        }

        public override void Flush()
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.buffer.Put(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            this.buffer.Put(value);
        }
        
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.buffer.Get(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return this.buffer.Get();
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
