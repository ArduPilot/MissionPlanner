using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>Check that cipher input/output streams are working correctly</remarks>
	[TestFixture]
	public class CipherStreamTest
		: SimpleTest
	{
		private static readonly byte[] RK = Hex.Decode("0123456789ABCDEF");
		private static readonly byte[] RIN = Hex.Decode("4e6f772069732074");
		private static readonly byte[] ROUT = Hex.Decode("3afbb5c77938280d");

		private static byte[] SIN = Hex.Decode(
			"00000000000000000000000000000000"
			+ "00000000000000000000000000000000"
			+ "00000000000000000000000000000000"
			+ "00000000000000000000000000000000");
		private static readonly byte[] SK = Hex.Decode("80000000000000000000000000000000");
		private static readonly byte[] SIV = Hex.Decode("0000000000000000");
		private static readonly byte[] SOUT = Hex.Decode(
			  "4DFA5E481DA23EA09A31022050859936"
			+ "DA52FCEE218005164F267CB65F5CFD7F"
			+ "2B4F97E0FF16924A52DF269515110A07"
			+ "F9E460BC65EF95DA58F740B7D1DBB0AA");

		private static readonly byte[] HCIN = new byte[64];
		private static readonly byte[] HCIV = new byte[32];

		private static readonly byte[] HCK256A = new byte[32];
		private static readonly byte[] HC256A = Hex.Decode(
			  "5B078985D8F6F30D42C5C02FA6B67951"
			+ "53F06534801F89F24E74248B720B4818"
			+ "CD9227ECEBCF4DBF8DBF6977E4AE14FA"
			+ "E8504C7BC8A9F3EA6C0106F5327E6981");

		private static readonly byte[] HCK128A = new byte[16];
		private static readonly byte[] HC128A = Hex.Decode(
			  "82001573A003FD3B7FD72FFB0EAF63AA"
			+ "C62F12DEB629DCA72785A66268EC758B"
			+ "1EDB36900560898178E0AD009ABF1F49"
			+ "1330DC1C246E3D6CB264F6900271D59C");

		private void doRunTest(
			string	name,
			int		ivLength)
		{
			string lCode = "ABCDEFGHIJKLMNOPQRSTUVWXY0123456789";

			string baseName = name;
			if (name.IndexOf('/') >= 0)
			{
				baseName = name.Substring(0, name.IndexOf('/'));
			}

			CipherKeyGenerator kGen = GeneratorUtilities.GetKeyGenerator(baseName);

			IBufferedCipher inCipher = CipherUtilities.GetCipher(name);
			IBufferedCipher outCipher = CipherUtilities.GetCipher(name);
			KeyParameter key = ParameterUtilities.CreateKeyParameter(baseName, kGen.GenerateKey());
			MemoryStream bIn = new MemoryStream(Encoding.ASCII.GetBytes(lCode), false);
			MemoryStream bOut = new MemoryStream();

			// In the Java build, this IV would be implicitly created and then retrieved with getIV()
			ICipherParameters cipherParams = key;
			if (ivLength > 0)
			{
				cipherParams = new ParametersWithIV(cipherParams, new byte[ivLength]);
			}

			inCipher.Init(true, cipherParams);

			// TODO Should we provide GetIV() method on IBufferedCipher?
			//if (inCipher.getIV() != null)
			//{
			//	outCipher.Init(false, new ParametersWithIV(key, inCipher.getIV()));
			//}
			//else
			//{
			//	outCipher.Init(false, key);
			//}
			outCipher.Init(false, cipherParams);

			CipherStream cIn = new CipherStream(bIn, inCipher, null);
			CipherStream cOut = new CipherStream(bOut, null, outCipher);

			int c;

			while ((c = cIn.ReadByte()) >= 0)
			{
				cOut.WriteByte((byte)c);
			}

			cIn.Close();

			cOut.Flush();
			cOut.Close();

			byte[] bs = bOut.ToArray();
			string res = Encoding.ASCII.GetString(bs, 0, bs.Length);

			if (!res.Equals(lCode))
			{
				Fail("Failed - decrypted data doesn't match.");
			}
		}

		private void doTestAlgorithm(
			string	name,
			byte[]	keyBytes,
			byte[]	iv,
			byte[]	plainText,
			byte[]	cipherText)
		{
			KeyParameter key = ParameterUtilities.CreateKeyParameter(name, keyBytes);

			IBufferedCipher inCipher = CipherUtilities.GetCipher(name);
			IBufferedCipher outCipher = CipherUtilities.GetCipher(name);

			if (iv != null)
			{
				inCipher.Init(true, new ParametersWithIV(key, iv));
				outCipher.Init(false, new ParametersWithIV(key, iv));
			}
			else
			{
				inCipher.Init(true, key);
				outCipher.Init(false, key);
			}

			byte[] enc = inCipher.DoFinal(plainText);
			if (!AreEqual(enc, cipherText))
			{
				Fail(name + ": cipher text doesn't match");
			}

			byte[] dec = outCipher.DoFinal(enc);

			if (!AreEqual(dec, plainText))
			{
				Fail(name + ": plain text doesn't match");
			}
		}

		private void doTestException(
			string	name,
			int		ivLength)
		{
			try
			{
				byte[] key128 = {
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143,
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143
				};

				byte[] key256 = {
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143,
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143,
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143,
					(byte)128, (byte)131, (byte)133, (byte)134,
					(byte)137, (byte)138, (byte)140, (byte)143 };

				byte[] keyBytes;
				if (name.Equals("HC256"))
				{
					keyBytes = key256;
				}
				else
				{
					keyBytes = key128;
				}

				KeyParameter cipherKey = ParameterUtilities.CreateKeyParameter(name, keyBytes);

				ICipherParameters cipherParams = cipherKey;
				if (ivLength > 0)
				{
					cipherParams = new ParametersWithIV(cipherParams, new byte[ivLength]);
				}

				IBufferedCipher ecipher = CipherUtilities.GetCipher(name);
				ecipher.Init(true, cipherParams);

				byte[] cipherText = new byte[0];
				try
				{
					// According specification Method engineUpdate(byte[] input,
					// int inputOffset, int inputLen, byte[] output, int
					// outputOffset)
					// throws ShortBufferException - if the given output buffer is
					// too
					// small to hold the result
					ecipher.ProcessBytes(new byte[20], 0, 20, cipherText, 0);

//					Fail("failed exception test - no ShortBufferException thrown");
					Fail("failed exception test - no DataLengthException thrown");
				}
//				catch (ShortBufferException e)
				catch (DataLengthException)
				{
					// ignore
				}

				// NB: The lightweight engine doesn't take public/private keys
//				try
//				{
//					IBufferedCipher c = CipherUtilities.GetCipher(name);
//
//					//                Key k = new PublicKey()
//					//                {
//					//
//					//                    public string getAlgorithm()
//					//                    {
//					//                        return "STUB";
//					//                    }
//					//
//					//                    public string getFormat()
//					//                    {
//					//                        return null;
//					//                    }
//					//
//					//                    public byte[] getEncoded()
//					//                    {
//					//                        return null;
//					//                    }
//					//
//					//                };
//					AsymmetricKeyParameter k = new AsymmetricKeyParameter(false);
//					c.Init(true, k);
//
//					Fail("failed exception test - no InvalidKeyException thrown for public key");
//				}
//				catch (InvalidKeyException)
//				{
//					// okay
//				}
//
//				try
//				{
//					IBufferedCipher c = CipherUtilities.GetCipher(name);
//
//					//				Key k = new PrivateKey()
//					//                {
//					//
//					//                    public string getAlgorithm()
//					//                    {
//					//                        return "STUB";
//					//                    }
//					//
//					//                    public string getFormat()
//					//                    {
//					//                        return null;
//					//                    }
//					//
//					//                    public byte[] getEncoded()
//					//                    {
//					//                        return null;
//					//                    }
//					//
//					//                };
//
//					AsymmetricKeyParameter k = new AsymmetricKeyParameter(true);
//					c.Init(false, k);
//
//					Fail("failed exception test - no InvalidKeyException thrown for private key");
//				}
//				catch (InvalidKeyException)
//				{
//					// okay
//				}
			}
			catch (Exception e)
			{
				Fail("unexpected exception.", e);
			}
		}

		[Test]
		public void TestRC4()
		{
			doRunTest("RC4", 0);
		}

		[Test]
		public void TestRC4Exception()
		{
			doTestException("RC4", 0);
		}

		[Test]
		public void TestRC4Algorithm()
		{
			doTestAlgorithm("RC4", RK, null, RIN, ROUT);
		}

		[Test]
		public void TestSalsa20()
		{
			doRunTest("Salsa20", 8);
		}

		[Test]
		public void TestSalsa20Exception()
		{
			doTestException("Salsa20", 8);
		}

		[Test]
		public void TestSalsa20Algorithm()
		{
			doTestAlgorithm("Salsa20", SK, SIV, SIN, SOUT);
		}

		[Test]
		public void TestHC128()
		{
			doRunTest("HC128", 16);
		}

		[Test]
		public void TestHC128Exception()
		{
			doTestException("HC128", 16);
		}

		[Test]
		public void TestHC128Algorithm()
		{
			doTestAlgorithm("HC128", HCK128A, HCIV, HCIN, HC128A);
		}

		[Test]
		public void TestHC256()
		{
			doRunTest("HC256", 32);
		}

		[Test]
		public void TestHC256Exception()
		{
			doTestException("HC256", 32);
		}

		[Test]
		public void TestHC256Algorithm()
		{
			doTestAlgorithm("HC256", HCK256A, HCIV, HCIN, HC256A);
		}

		[Test]
		public void TestVmpc()
		{
			doRunTest("VMPC", 16);
		}

		[Test]
		public void TestVmpcException()
		{
			doTestException("VMPC", 16);
		}

//		[Test]
//		public void TestVmpcAlgorithm()
//		{
//			doTestAlgorithm("VMPC", a, iv, in, a);
//		}

		[Test]
		public void TestVmpcKsa3()
		{
			doRunTest("VMPC-KSA3", 16);
		}

		[Test]
		public void TestVmpcKsa3Exception()
		{
			doTestException("VMPC-KSA3", 16);
		}

//		[Test]
//		public void TestVmpcKsa3Algorithm()
//		{
//			doTestAlgorithm("VMPC-KSA3", a, iv, in, a);
//		}

		[Test]
		public void TestDesEcbPkcs7()
		{
			doRunTest("DES/ECB/PKCS7Padding", 0);
		}

		[Test]
		public void TestDesCfbNoPadding()
		{
			doRunTest("DES/CFB8/NoPadding", 0);
		}

		public override void PerformTest()
		{
			TestRC4();
			TestRC4Exception();
			TestRC4Algorithm();
			TestSalsa20();
			TestSalsa20Exception();
			TestSalsa20Algorithm();
			TestHC128();
			TestHC128Exception();
			TestHC128Algorithm();
			TestHC256();
			TestHC256Exception();
			TestHC256Algorithm();
			TestVmpc();
			TestVmpcException();
//			TestVmpcAlgorithm();
			TestVmpcKsa3();
			TestVmpcKsa3Exception();
//			TestVmpcKsa3Algorithm();
			TestDesEcbPkcs7();
			TestDesCfbNoPadding();
		}

		public override string Name
		{
			get { return "CipherStreamTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new CipherStreamTest());
		}
	}
}
