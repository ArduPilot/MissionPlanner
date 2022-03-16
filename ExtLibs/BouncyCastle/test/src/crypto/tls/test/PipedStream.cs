using System;
using System.IO;
using System.Threading;

namespace Org.BouncyCastle.Crypto.Tls.Tests
{
    internal class PipedStream
        :   Stream
    {
        private readonly MemoryStream mBuf = new MemoryStream();
        private bool mClosed = false;

        private PipedStream mOther = null;
        private long mReadPos = 0;

        internal PipedStream()
        {
        }

        internal PipedStream(PipedStream other)
        {
            lock (other)
            {
                this.mOther = other;
                other.mOther = this;
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Close()
        {
            lock (this)
            {
                mClosed = true;
                Monitor.PulseAll(this);
            }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (mOther)
            {
                WaitForData();
                int len = (int)System.Math.Min(count, mOther.mBuf.Position - mReadPos);
                Array.Copy(mOther.mBuf.GetBuffer(), mReadPos, buffer, offset, len);
                mReadPos += len;
                return len;
            }
        }

        public override int ReadByte()
        {
            lock (mOther)
            {
                WaitForData();
                bool eof = (mReadPos >= mOther.mBuf.Position);
                return eof ? -1 : mOther.mBuf.GetBuffer()[mReadPos++];
            }
        }

        public override void Write(byte[] buf, int off, int len)
        {
            lock (this)
            {
                CheckOpen();
                mBuf.Write(buf, off, len);
                Monitor.PulseAll(this);
            }
        }

        public override void WriteByte(byte value)
        {
            lock (this)
            {
                CheckOpen();
                mBuf.WriteByte(value);
                Monitor.PulseAll(mBuf);
            }
        }

        private void CheckOpen()
        {
            if (mClosed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

        private void WaitForData()
        {
            while (mReadPos >= mOther.mBuf.Position && !mOther.mClosed)
            {
                Monitor.Wait(mOther);
            }
        }
    }
}
