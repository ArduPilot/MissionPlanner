using System;
using System.IO;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Bcpg
{
	/// <remarks>Generic literal data packet.</remarks>
    public class LiteralDataPacket
        : InputStreamPacket
	{
		private int		format;
        private byte[]	fileName;
        private long	modDate;

		internal LiteralDataPacket(
            BcpgInputStream bcpgIn)
			: base(bcpgIn)
        {
            format = bcpgIn.ReadByte();
            int len = bcpgIn.ReadByte();

			fileName = new byte[len];
			for (int i = 0; i != len; ++i)
            {
                fileName[i] = (byte)bcpgIn.ReadByte();
            }

			modDate = (((uint)bcpgIn.ReadByte() << 24)
				| ((uint)bcpgIn.ReadByte() << 16)
                | ((uint)bcpgIn.ReadByte() << 8)
				| (uint)bcpgIn.ReadByte()) * 1000L;
        }

		/// <summary>The format tag value.</summary>
        public int Format
		{
			get { return format; }
		}

		/// <summary>The modification time of the file in milli-seconds (since Jan 1, 1970 UTC)</summary>
        public long ModificationTime
		{
			get { return modDate; }
		}

		public string FileName
		{
			get { return Strings.FromUtf8ByteArray(fileName); }
		}

		public byte[] GetRawFileName()
		{
			return Arrays.Clone(fileName);
		}
	}
}
