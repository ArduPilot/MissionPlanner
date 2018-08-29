using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Modes.Gcm;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class GcmReorderTest
	{
		private static readonly byte[] H;
		private static readonly SecureRandom random = new SecureRandom(); 
		private static readonly IGcmMultiplier mul = new Tables64kGcmMultiplier();
		private static readonly IGcmExponentiator exp = new Tables1kGcmExponentiator();
		private static readonly byte[] Empty = new byte[0];

		static GcmReorderTest()
		{
			H = new byte[16];
			random.NextBytes(H);
			mul.Init(Arrays.Clone(H));
			exp.Init(Arrays.Clone(H));
		}

		[Test]
		public void TestCombine()
		{
			for (int count = 0; count < 10; ++count)
			{
				byte[] A = randomBytes(1000);
				byte[] C = randomBytes(1000);

				byte[] ghashA_ = GHASH(A, Empty);
				byte[] ghash_C = GHASH(Empty, C);
				byte[] ghashAC = GHASH(A, C);

				byte[] ghashCombine = combine_GHASH(ghashA_, (long)A.Length * 8, ghash_C, (long)C.Length * 8);

				Assert.IsTrue(Arrays.AreEqual(ghashAC, ghashCombine));
			}
		}

		[Test]
		public void TestConcatAuth()
		{
			for (int count = 0; count < 10; ++count)
			{
				byte[] P = randomBlocks(100);
				byte[] A = randomBytes(1000);
				byte[] PA = Arrays.Concatenate(P, A);

				byte[] ghashP_ = GHASH(P, Empty);
				byte[] ghashA_ = GHASH(A, Empty);
				byte[] ghashPA_val = GHASH(PA, Empty);
				byte[] ghashConcat = concatAuth_GHASH(ghashP_, (long)P.Length * 8, ghashA_, (long)A.Length * 8);

				Assert.IsTrue(Arrays.AreEqual(ghashPA_val, ghashConcat));
			}
		}

        [Test]
		public void TestConcatCrypt()
		{
			for (int count = 0; count < 10; ++count)
			{
				byte[] P = randomBlocks(100);
				byte[] A = randomBytes(1000);
                byte[] PA = Arrays.Concatenate(P, A);

				byte[] ghash_P = GHASH(Empty, P);
				byte[] ghash_A = GHASH(Empty, A);
				byte[] ghash_PA = GHASH(Empty, PA);
				byte[] ghashConcat = concatCrypt_GHASH(ghash_P, (long)P.Length * 8, ghash_A, (long)A.Length * 8);

				Assert.IsTrue(Arrays.AreEqual(ghash_PA, ghashConcat));
			}
		}

		[Test]
		public void TestExp()
		{
			{
				byte[] buf1 = new byte[16];
				buf1[0] = 0x80;

				byte[] buf2 = new byte[16];

				for (int pow = 0; pow != 100; ++pow)
				{
					exp.ExponentiateX(pow, buf2);

					Assert.IsTrue(Arrays.AreEqual(buf1, buf2));

					mul.MultiplyH(buf1);
				}
			}

			long[] testPow = new long[]{ 10, 1, 8, 17, 24, 13, 2, 13, 2, 3 };
			byte[][] testData = new byte[][]{
				Hex.Decode("9185848a877bd87ba071e281f476e8e7"),
				Hex.Decode("697ce3052137d80745d524474fb6b290"),
				Hex.Decode("2696fc47198bb23b11296e4f88720a17"),
				Hex.Decode("01f2f0ead011a4ae0cf3572f1b76dd8e"),
				Hex.Decode("a53060694a044e4b7fa1e661c5a7bb6b"),
				Hex.Decode("39c0392e8b6b0e04a7565c85394c2c4c"),
				Hex.Decode("519c362d502e07f2d8b7597a359a5214"),
				Hex.Decode("5a527a393675705e19b2117f67695af4"),
				Hex.Decode("27fc0901d1d332a53ba4d4386c2109d2"),
				Hex.Decode("93ca9b57174aabedf8220e83366d7df6"),
			};

			for (int i = 0; i != 10; ++i)
			{
				long pow = testPow[i];
				byte[] data = Arrays.Clone(testData[i]);

				byte[] expected = Arrays.Clone(data);
				for (int j = 0; j < pow; ++j)
				{
					mul.MultiplyH(expected);
				}

				byte[] H_a = new byte[16];
				exp.ExponentiateX(pow, H_a);
				byte[] actual = multiply(data, H_a);

				Assert.IsTrue(Arrays.AreEqual(expected, actual));
			}
		}

		[Test]
		public void TestMultiply()
		{
			byte[] expected = Arrays.Clone(H);
			mul.MultiplyH(expected);

			Assert.IsTrue(Arrays.AreEqual(expected, multiply(H, H)));

			for (int count = 0; count < 10; ++count)
			{
				byte[] a = new byte[16];
				random.NextBytes(a);

				byte[] b = new byte[16];
				random.NextBytes(b);

				expected = Arrays.Clone(a);
				mul.MultiplyH(expected);
				Assert.IsTrue(Arrays.AreEqual(expected, multiply(a, H)));
				Assert.IsTrue(Arrays.AreEqual(expected, multiply(H, a)));
				
				expected = Arrays.Clone(b);
				mul.MultiplyH(expected);
				Assert.IsTrue(Arrays.AreEqual(expected, multiply(b, H)));
				Assert.IsTrue(Arrays.AreEqual(expected, multiply(H, b)));
				
				Assert.IsTrue(Arrays.AreEqual(multiply(a, b), multiply(b, a)));
			}
		}

		private byte[] randomBlocks(int upper)
		{
			byte[] bs = new byte[16 * random.Next(upper)];
			random.NextBytes(bs);
			return bs;
		}

		private byte[] randomBytes(int upper)
		{
			byte[] bs = new byte[random.Next(upper)];
			random.NextBytes(bs);
			return bs;
		}

        private byte[] combine_GHASH(byte[] ghashA_, long bitlenA, byte[] ghash_C, long bitlenC)
		{
			// Note: bitlenA must be aligned to the block size

			long c = (bitlenC + 127) >> 7;

			byte[] H_c = new byte[16];
			exp.ExponentiateX(c, H_c);

			byte[] tmp1 = lengthBlock(bitlenA, 0);
			mul.MultiplyH(tmp1);

			byte[] ghashAC = Arrays.Clone(ghashA_);
			xor(ghashAC, tmp1);
			ghashAC = multiply(ghashAC, H_c);
			// No need to touch the len(C) part (second 8 bytes)
			xor(ghashAC, tmp1);
			xor(ghashAC, ghash_C);

			return ghashAC;
		}

		private byte[] concatAuth_GHASH(byte[] ghashP, long bitlenP, byte[] ghashA, long bitlenA)
		{
			// Note: bitlenP must be aligned to the block size

			long a = (bitlenA + 127) >> 7;

			byte[] tmp1 = lengthBlock(bitlenP, 0);
			mul.MultiplyH(tmp1);

			byte[] tmp2 = lengthBlock(bitlenA ^ (bitlenP + bitlenA), 0);
			mul.MultiplyH(tmp2);

			byte[] H_a = new byte[16];
			exp.ExponentiateX(a, H_a);

			byte[] ghashC = Arrays.Clone(ghashP);
			xor(ghashC, tmp1);
			ghashC = multiply(ghashC, H_a);
			xor(ghashC, tmp2);
			xor(ghashC, ghashA);
			return ghashC;
		}

		private byte[] concatCrypt_GHASH(byte[] ghashP, long bitlenP, byte[] ghashA, long bitlenA)
		{
			// Note: bitlenP must be aligned to the block size
			
			long a = (bitlenA + 127) >> 7;

			byte[] tmp1 = lengthBlock(0, bitlenP);
			mul.MultiplyH(tmp1);

			byte[] tmp2 = lengthBlock(0, bitlenA ^ (bitlenP + bitlenA));
			mul.MultiplyH(tmp2);

			byte[] H_a = new byte[16];
			exp.ExponentiateX(a, H_a);

			byte[] ghashC = Arrays.Clone(ghashP);
			xor(ghashC, tmp1);
			ghashC = multiply(ghashC, H_a);
			xor(ghashC, tmp2);
			xor(ghashC, ghashA);
			return ghashC;
		}

		private byte[] GHASH(byte[] A, byte[] C)
		{
			byte[] X = new byte[16];

			{
				for (int pos = 0; pos < A.Length; pos += 16)
				{
					byte[] tmp = new byte[16];
					int num = System.Math.Min(A.Length - pos, 16);
					Array.Copy(A, pos, tmp, 0, num);
					xor(X, tmp);
					mul.MultiplyH(X);
				}
			}

			{
				for (int pos = 0; pos < C.Length; pos += 16)
				{
					byte[] tmp = new byte[16];
					int num = System.Math.Min(C.Length - pos, 16);
					Array.Copy(C, pos, tmp, 0, num);
					xor(X, tmp);
					mul.MultiplyH(X);
				}
			}

			{
				xor(X, lengthBlock((long)A.Length * 8, (long)C.Length * 8));
				mul.MultiplyH(X);
			}

			return X;
		}

		private static byte[] lengthBlock(long bitlenA, long bitlenC)
		{
			byte[] tmp = new byte[16];
			UInt64_To_BE((ulong)bitlenA, tmp, 0);
			UInt64_To_BE((ulong)bitlenC, tmp, 8);
			return tmp;
		}

		private static void xor(byte[] block, byte[] val)
		{
			for (int i = 15; i >= 0; --i)
			{
				block[i] ^= val[i];
			}
		}

		private static void UInt64_To_BE(ulong n, byte[] bs, int off)
		{
			UInt32_To_BE((uint)(n >> 32), bs, off);
			UInt32_To_BE((uint)(n), bs, off + 4);
		}

		private static void UInt32_To_BE(uint n, byte[] bs, int off)
		{
			bs[  off] = (byte)(n >> 24);
			bs[++off] = (byte)(n >> 16);
			bs[++off] = (byte)(n >>  8);
			bs[++off] = (byte)(n      );
		}

		private static byte[] multiply(byte[] a, byte[] b)
		{
			byte[] c = new byte[16];
			byte[] tmp = Arrays.Clone(b);

			for (int i = 0; i < 16; ++i)
			{
				byte bits = a[i];
				for (int j = 7; j >= 0; --j)
				{
					if ((bits & (1 << j)) != 0)
					{
						xor(c, tmp);
					}

					bool lsb = (tmp[15] & 1) != 0;
					shiftRight(tmp);
					if (lsb)
					{
						// R = new byte[]{ 0xe1, ... };
						//GcmUtilities.Xor(tmp, R);
						tmp[0] ^= (byte)0xe1;
					}
				}
			}

			return c;
		}

		private static void shiftRight(byte[] block)
		{
			int i = 0;
			byte bit = 0;
			for (;;)
			{
				byte b = block[i];
				block[i] = (byte)((b >> 1) | bit);
				if (++i == 16) break;
				bit = (byte)(b << 7);
			}
		}
	}
}
