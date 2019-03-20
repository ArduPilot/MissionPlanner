using System;
using System.IO;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

using M = Org.BouncyCastle.Math;

namespace Org.BouncyCastle.Utilities.Test
{
	public class FixedSecureRandom
		: SecureRandom
	{
        private static readonly M.BigInteger REGULAR = new M.BigInteger("01020304ffffffff0506070811111111", 16);
        private static readonly M.BigInteger ANDROID = new M.BigInteger("1111111105060708ffffffff01020304", 16);
        private static readonly M.BigInteger CLASSPATH = new M.BigInteger("3020104ffffffff05060708111111", 16);

        private static readonly bool isAndroidStyle;
        private static readonly bool isClasspathStyle;
        private static readonly bool isRegularStyle;

        static FixedSecureRandom()
        {
            M.BigInteger check1 = new M.BigInteger(128, new RandomChecker());
            M.BigInteger check2 = new M.BigInteger(120, new RandomChecker());

            isAndroidStyle = check1.Equals(ANDROID);
            isRegularStyle = check1.Equals(REGULAR);
            isClasspathStyle = check2.Equals(CLASSPATH);
        }

		private byte[]       _data;
		private int          _index;

        /**
         * Base class for sources of fixed "Randomness"
         */
        public class Source
        {
            internal byte[] data;

            internal Source(byte[] data)
            {
                this.data = data;
            }
        }

        /**
         * Data Source - in this case we just expect requests for byte arrays.
         */
        public class Data
            : Source
        {
            public Data(byte[] data)
                : base(data)
            {
            }
        }

        /**
         * BigInteger Source - in this case we expect requests for data that will be used
         * for BigIntegers. The FixedSecureRandom will attempt to compensate for platform differences here.
         */
        public class BigInteger
            : Source
        {
            public BigInteger(byte[] data)
                : base(data)
            {
            }

            public BigInteger(int bitLength, byte[] data)
                : base(ExpandToBitLength(bitLength, data))
            {
            }

            public BigInteger(string hexData)
                : this(Hex.Decode(hexData))
            {
            }

            public BigInteger(int bitLength, string hexData)
                : base(ExpandToBitLength(bitLength, Hex.Decode(hexData)))
            {
            }
        }

        protected FixedSecureRandom(
			byte[] data)
		{
			_data = data;
		}

		public static FixedSecureRandom From(
			params byte[][] values)
		{
			MemoryStream bOut = new MemoryStream();

			for (int i = 0; i != values.Length; i++)
			{
				try
				{
					byte[] v = values[i];
					bOut.Write(v, 0, v.Length);
				}
				catch (IOException)
				{
					throw new ArgumentException("can't save value array.");
				}
			}

			return new FixedSecureRandom(bOut.ToArray());
		}

        public FixedSecureRandom(
            Source[] sources)
        {
            MemoryStream bOut = new MemoryStream();

            if (isRegularStyle)
            {
                if (isClasspathStyle)
                {
                    for (int i = 0; i != sources.Length; i++)
                    {
                        try
                        {
                            if (sources[i] is BigInteger)
                            {
                                byte[] data = sources[i].data;
                                int len = data.Length - (data.Length % 4);
                                for (int w = data.Length - len - 1; w >= 0; w--)
                                {
                                    bOut.WriteByte(data[w]);
                                }
                                for (int w = data.Length - len; w < data.Length; w += 4)
                                {
                                    bOut.Write(data, w, 4);
                                }
                            }
                            else
                            {
                                bOut.Write(sources[i].data, 0, sources[i].data.Length);
                            }
                        }
                        catch (IOException e)
                        {
                            throw new ArgumentException("can't save value source.");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i != sources.Length; i++)
                    {
                        try
                        {
                            bOut.Write(sources[i].data, 0, sources[i].data.Length);
                        }
                        catch (IOException e)
                        {
                            throw new ArgumentException("can't save value source.");
                        }
                    }
                }
            }
            else if (isAndroidStyle)
            {
                for (int i = 0; i != sources.Length; i++)
                {
                    try
                    {
                        if (sources[i] is BigInteger)
                        {
                            byte[] data = sources[i].data;
                            int len = data.Length - (data.Length % 4);
                            for (int w = 0; w < len; w += 4)
                            {
                                bOut.Write(data, data.Length - (w + 4), 4);
                            }
                            if (data.Length - len != 0)
                            {
                                for (int w = 0; w != 4 - (data.Length - len); w++)
                                {
                                    bOut.WriteByte(0);
                                }
                            }
                            for (int w = 0; w != data.Length - len; w++)
                            {
                                bOut.WriteByte(data[len + w]);
                            }
                        }
                        else
                        {
                            bOut.Write(sources[i].data, 0, sources[i].data.Length);
                        }
                    }
                    catch (IOException e)
                    {
                        throw new ArgumentException("can't save value source.");
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Unrecognized BigInteger implementation");
            }

            _data = bOut.ToArray();
        }

        public override byte[] GenerateSeed(int numBytes)
        {
            return SecureRandom.GetNextBytes(this, numBytes);
        }

        public override void NextBytes(
			byte[] buf)
		{
			Array.Copy(_data, _index, buf, 0, buf.Length);

			_index += buf.Length;
		}

		public override void NextBytes(
			byte[]	buf,
			int		off,
			int		len)
		{
			Array.Copy(_data, _index, buf, off, len);

			_index += len;
		}

		public bool IsExhausted
		{
			get { return _index == _data.Length; }
		}

        private class RandomChecker
            : SecureRandom
        {
            byte[] data = Hex.Decode("01020304ffffffff0506070811111111");
            int    index = 0;

            public override void NextBytes(byte[] bytes)
            {
                Array.Copy(data, index, bytes, 0, bytes.Length);

                index += bytes.Length;
            }
        }

        private static byte[] ExpandToBitLength(int bitLength, byte[] v)
        {
            if ((bitLength + 7) / 8 > v.Length)
            {
                byte[] tmp = new byte[(bitLength + 7) / 8];

                Array.Copy(v, 0, tmp, tmp.Length - v.Length, v.Length);
                if (isAndroidStyle)
                {
                    if (bitLength % 8 != 0)
                    {
                        uint i = BE_To_UInt32(tmp, 0);
                        UInt32_To_BE(i << (8 - (bitLength % 8)), tmp, 0);
                    }
                }

                return tmp;
            }
            else
            {
                if (isAndroidStyle && bitLength < (v.Length * 8))
                {
                    if (bitLength % 8 != 0)
                    {
                        uint i = BE_To_UInt32(v, 0);
                        UInt32_To_BE(i << (8 - (bitLength % 8)), v, 0);
                    }
                }
            }

            return v;
        }

        internal static uint BE_To_UInt32(byte[] bs, int off)
        {
            return (uint)bs[off] << 24
                | (uint)bs[off + 1] << 16
                | (uint)bs[off + 2] << 8
                | (uint)bs[off + 3];
        }

        internal static void UInt32_To_BE(uint n, byte[] bs, int off)
        {
            bs[off] = (byte)(n >> 24);
            bs[off + 1] = (byte)(n >> 16);
            bs[off + 2] = (byte)(n >> 8);
            bs[off + 3] = (byte)(n);
        }
	}
}
