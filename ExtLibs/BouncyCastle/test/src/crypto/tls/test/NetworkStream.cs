using System;
using System.IO;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class NetworkStream
        :   Stream
    {
        private readonly Stream mInner;
        private bool mClosed = false;

        internal NetworkStream(Stream inner)
        {
            this.mInner = inner;
        }

        internal virtual bool IsClosed
        {
            get { lock (this) return mClosed; }
        }

        public override bool CanRead
        {
            get { return mInner.CanRead; }
        }

        public override bool CanSeek
        {
            get { return mInner.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return mInner.CanWrite; }
        }

        public override void Close()
        {
            lock (this) mClosed = true;
        }

        public override void Flush()
        {
            mInner.Flush();
        }

        public override long Length
        {
            get { return mInner.Length; }
        }

        public override long Position
        {
            get { return mInner.Position; }
            set { mInner.Position = value; }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return mInner.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            mInner.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckNotClosed();
            return mInner.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            CheckNotClosed();
            return mInner.ReadByte();
        }

        public override void Write(byte[] buf, int off, int len)
        {
            CheckNotClosed();
            mInner.Write(buf, off, len);
        }

        public override void WriteByte(byte value)
        {
            CheckNotClosed();
            mInner.WriteByte(value);
        }

        private void CheckNotClosed()
        {
            lock (this)
            {
                if (mClosed)
                    throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}
