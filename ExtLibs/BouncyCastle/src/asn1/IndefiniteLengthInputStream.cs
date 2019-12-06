using System;
using System.IO;

namespace Org.BouncyCastle.Asn1
{
	class IndefiniteLengthInputStream
		: LimitedInputStream
	{
        private int _lookAhead;
        private bool _eofOn00 = true;

		internal IndefiniteLengthInputStream(
			Stream	inStream,
			int		limit)
			: base(inStream, limit)
		{
            _lookAhead = RequireByte();
            CheckForEof();
		}

		internal void SetEofOn00(
			bool eofOn00)
		{
			_eofOn00 = eofOn00;
            if (_eofOn00)
            {
                CheckForEof();
            }
        }

        private bool CheckForEof()
		{
            if (_lookAhead == 0x00)
            {
                int extra = RequireByte();
                if (extra != 0)
                {
                    throw new IOException("malformed end-of-contents marker");
                }

                _lookAhead = -1;
                SetParentEofDetect(true);
                return true;
            }
            return _lookAhead < 0;
        }

		public override int Read(
			byte[]	buffer,
			int		offset,
			int		count)
		{
			// Only use this optimisation if we aren't checking for 00
			if (_eofOn00 || count <= 1)
				return base.Read(buffer, offset, count);

			if (_lookAhead < 0)
				return 0;

			int numRead = _in.Read(buffer, offset + 1, count - 1);

			if (numRead <= 0)
			{
				// Corrupted stream
				throw new EndOfStreamException();
			}

			buffer[offset] = (byte)_lookAhead;
			_lookAhead = RequireByte();

			return numRead + 1;
		}

		public override int ReadByte()
		{
            if (_eofOn00 && CheckForEof())
				return -1;

            int result = _lookAhead;
            _lookAhead = RequireByte();
            return result;
		}

        private int RequireByte()
        {
            int b = _in.ReadByte();
            if (b < 0)
            {
                // Corrupted stream
                throw new EndOfStreamException();
            }
            return b;
        }
	}
}

//using System;
//using System.IO;

//namespace Org.BouncyCastle.Asn1
//{
//    class IndefiniteLengthInputStream
//        : LimitedInputStream
//    {
//        private bool _eofReached = false;
//        private bool _eofOn00 = true;

//        internal IndefiniteLengthInputStream(
//            Stream	inStream,
//            int		limit)
//            : base(inStream, limit)
//        {
//        }

//        internal void SetEofOn00(
//            bool eofOn00)
//        {
//            _eofOn00 = eofOn00;
//        }

//        public override int Read(
//            byte[]	buffer,
//            int		offset,
//            int		count)
//        {
//            if (_eofReached)
//                return 0;

//            if (_eofOn00)
//                return base.Read(buffer, offset, count);

//            int numRead = _in.Read(buffer, offset, count);

//            if (numRead <= 0)
//                throw new EndOfStreamException();

//            return numRead;
//        }

//        public override int ReadByte()
//        {
//            if (_eofReached)
//                return -1;

//            int b1 = _in.ReadByte();

//            if (b1 < 0)
//                throw new EndOfStreamException();

//            if (b1 == 0 && _eofOn00)
//            {
//                int b2 = _in.ReadByte();

//                if (b2 < 0)
//                    throw new EndOfStreamException();

//                if (b2 == 0)
//                {
//                    _eofReached = true;
//                    SetParentEofDetect(true);
//                    return -1;
//                }

//                throw new InvalidDataException();
//            }

//            return b1;
//        }
//    }
//}
