using System;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>Basic test class for the AES cipher vectors from FIPS-197</remarks>
	[TestFixture]
	public class AesTest
		: BaseBlockCipherTest
	{
		internal static readonly string[] cipherTests =
		{
			"128",
			"000102030405060708090a0b0c0d0e0f",
			"00112233445566778899aabbccddeeff",
			"69c4e0d86a7b0430d8cdb78070b4c55a",
			"192",
			"000102030405060708090a0b0c0d0e0f1011121314151617",
			"00112233445566778899aabbccddeeff",
			"dda97ca4864cdfe06eaf70a0ec0d7191",
			"256",
			"000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f",
			"00112233445566778899aabbccddeeff",
			"8ea2b7ca516745bfeafc49904b496089",
		};

		public AesTest()
			: base("AES")
		{
		}

		[Test]
		public void TestCiphers()
		{
			for (int i = 0; i != cipherTests.Length; i += 4)
			{
				doCipherTest(int.Parse(cipherTests[i]),
					Hex.Decode(cipherTests[i + 1]),
					Hex.Decode(cipherTests[i + 2]),
					Hex.Decode(cipherTests[i + 3]));
			}
		}

		[Test]
		public void TestOids()
		{
			string[] oids = {
				NistObjectIdentifiers.IdAes128Ecb.Id,
				NistObjectIdentifiers.IdAes128Cbc.Id,
				NistObjectIdentifiers.IdAes128Ofb.Id,
				NistObjectIdentifiers.IdAes128Cfb.Id,
				NistObjectIdentifiers.IdAes192Ecb.Id,
				NistObjectIdentifiers.IdAes192Cbc.Id,
				NistObjectIdentifiers.IdAes192Ofb.Id,
				NistObjectIdentifiers.IdAes192Cfb.Id,
				NistObjectIdentifiers.IdAes256Ecb.Id,
				NistObjectIdentifiers.IdAes256Cbc.Id,
				NistObjectIdentifiers.IdAes256Ofb.Id,
				NistObjectIdentifiers.IdAes256Cfb.Id
			};

			string[] names = {
				"AES/ECB/PKCS7Padding",
				"AES/CBC/PKCS7Padding",
				"AES/OFB/NoPadding",
				"AES/CFB/NoPadding",
				"AES/ECB/PKCS7Padding",
				"AES/CBC/PKCS7Padding",
				"AES/OFB/NoPadding",
				"AES/CFB/NoPadding",
				"AES/ECB/PKCS7Padding",
				"AES/CBC/PKCS7Padding",
				"AES/OFB/NoPadding",
				"AES/CFB/NoPadding"
			};

			oidTest(oids, names, 4);
		}

		[Test]
		public void TestWrap()
		{
			byte[] kek1 = Hex.Decode("000102030405060708090a0b0c0d0e0f");
			byte[] in1 = Hex.Decode("00112233445566778899aabbccddeeff");
			byte[] out1 = Hex.Decode("1fa68b0a8112b447aef34bd8fb5a7b829d3e862371d2cfe5");

			wrapTest(1, "AESWrap", kek1, in1, out1);
		}

		[Test]
		public void TestWrapOids()
		{
			string[] wrapOids =
			{
				NistObjectIdentifiers.IdAes128Wrap.Id,
				NistObjectIdentifiers.IdAes192Wrap.Id,
				NistObjectIdentifiers.IdAes256Wrap.Id
			};

			wrapOidTest(wrapOids, "AESWrap");
		}

		private void doCipherTest(
			int		strength,
			byte[]	keyBytes,
			byte[]	input,
			byte[]	output)
		{
			KeyParameter key = ParameterUtilities.CreateKeyParameter("AES", keyBytes);

			IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");

			try
			{
				outCipher.Init(true, key);
			}
			catch (Exception e)
			{
				Fail("AES failed initialisation - " + e, e);
			}

			try
			{
				inCipher.Init(false, key);
			}
			catch (Exception e)
			{
				Fail("AES failed initialisation - " + e, e);
			}

			//
			// encryption pass
			//
			MemoryStream bOut = new MemoryStream();

			CipherStream cOut = new CipherStream(bOut, null, outCipher);

			try
			{
				for (int i = 0; i != input.Length / 2; i++)
				{
					cOut.WriteByte(input[i]);
				}
				cOut.Write(input, input.Length / 2, input.Length - input.Length / 2);
				cOut.Close();
			}
			catch (IOException e)
			{
				Fail("AES failed encryption - " + e, e);
			}

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("AES failed encryption - expected "
					+ Hex.ToHexString(output) + " got "
					+ Hex.ToHexString(bytes));
			}

			//
			// decryption pass
			//
			MemoryStream bIn = new MemoryStream(bytes, false);

			CipherStream cIn = new CipherStream(bIn, inCipher, null);

			try
			{
//				DataInputStream dIn = new DataInputStream(cIn);
				BinaryReader dIn = new BinaryReader(cIn);

				bytes = new byte[input.Length];

				for (int i = 0; i != input.Length / 2; i++)
				{
//					bytes[i] = (byte)dIn.read();
					bytes[i] = dIn.ReadByte();
				}

				int remaining = bytes.Length - input.Length / 2;
//				dIn.readFully(bytes, input.Length / 2, remaining);
				byte[] extra = dIn.ReadBytes(remaining);
				if (extra.Length < remaining)
					throw new EndOfStreamException();
				extra.CopyTo(bytes, input.Length / 2);
			}
			catch (Exception e)
			{
				Fail("AES failed encryption - " + e, e);
			}

			if (!AreEqual(bytes, input))
			{
				Fail("AES failed decryption - expected "
					+ Hex.ToHexString(input) + " got "
					+ Hex.ToHexString(bytes));
			}
		}

		[Test]
		public void TestEax()
		{
			byte[] K = Hex.Decode("233952DEE4D5ED5F9B9C6D6FF80FF478");
			byte[] N = Hex.Decode("62EC67F9C3A4A407FCB2A8C49031A8B3");
			byte[] P = Hex.Decode("68656c6c6f20776f726c642121");
			byte[] C = Hex.Decode("2f9f76cb7659c70e4be11670a3e193ae1bc6b5762a");

			KeyParameter key = ParameterUtilities.CreateKeyParameter("AES", K);
			IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/EAX/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("AES/EAX/NoPadding");

			inCipher.Init(true, new ParametersWithIV(key, N));

			byte[] enc = inCipher.DoFinal(P);
			if (!AreEqual(enc, C))
			{
				Fail("ciphertext doesn't match in EAX");
			}

			outCipher.Init(false, new ParametersWithIV(key, N));

			byte[] dec = outCipher.DoFinal(C);
			if (!AreEqual(dec, P))
			{
				Fail("plaintext doesn't match in EAX");
			}

			try
			{
				inCipher = CipherUtilities.GetCipher("AES/EAX/PKCS5Padding");

				Fail("bad padding missed in EAX");
			}
			catch (SecurityUtilityException)
			{
				// expected
			}
		}

		[Test]
		public void TestCcm()
		{
			byte[] K = Hex.Decode("404142434445464748494a4b4c4d4e4f");
			byte[] N = Hex.Decode("10111213141516");
			byte[] P = Hex.Decode("68656c6c6f20776f726c642121");
			byte[] C = Hex.Decode("39264f148b54c456035de0a531c8344f46db12b388");

			KeyParameter key = ParameterUtilities.CreateKeyParameter("AES", K);

			IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/CCM/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("AES/CCM/NoPadding");

			inCipher.Init(true, new ParametersWithIV(key, N));

			byte[] enc = inCipher.DoFinal(P);
			if (!AreEqual(enc, C))
			{
				Fail("ciphertext doesn't match in CCM");
			}

			outCipher.Init(false, new ParametersWithIV(key, N));

			byte[] dec = outCipher.DoFinal(C);
			if (!AreEqual(dec, P))
			{
				Fail("plaintext doesn't match in CCM");
			}

			try
			{
				inCipher = CipherUtilities.GetCipher("AES/CCM/PKCS5Padding");

				Fail("bad padding missed in CCM");
			}
			catch (SecurityUtilityException)
			{
				// expected
			}
		}

		[Test]
		public void TestGcm()
		{
			// Test Case 15 from McGrew/Viega
			byte[] K = Hex.Decode(
				  "feffe9928665731c6d6a8f9467308308"
				+ "feffe9928665731c6d6a8f9467308308");
			byte[] P = Hex.Decode(
				  "d9313225f88406e5a55909c5aff5269a"
				+ "86a7a9531534f7da2e4c303d8a318a72"
				+ "1c3c0c95956809532fcf0e2449a6b525"
				+ "b16aedf5aa0de657ba637b391aafd255");
			byte[] N = Hex.Decode("cafebabefacedbaddecaf888");
			string T = "b094dac5d93471bdec1a502270e3cc6c";
			byte[] C = Hex.Decode(
				  "522dc1f099567d07f47f37a32a84427d"
				+ "643a8cdcbfe5c0c97598a2bd2555d1aa"
				+ "8cb08e48590dbb3da7b08b1056828838"
				+ "c5f61e6393ba7a0abcc9f662898015ad"
				+ T);

			KeyParameter key = ParameterUtilities.CreateKeyParameter("AES", K);
			IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/GCM/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("AES/GCM/NoPadding");

			inCipher.Init(true, new ParametersWithIV(key, N));

			byte[] enc = inCipher.DoFinal(P);
			if (!AreEqual(enc, C))
			{
				Fail("ciphertext doesn't match in GCM");
			}

			outCipher.Init(false, new ParametersWithIV(key, N));

			byte[] dec = outCipher.DoFinal(C);
			if (!AreEqual(dec, P))
			{
				Fail("plaintext doesn't match in GCM");
			}

			try
			{
				inCipher = CipherUtilities.GetCipher("AES/GCM/PKCS5Padding");

				Fail("bad padding missed in GCM");
			}
			catch (SecurityUtilityException)
			{
				// expected
			}
		}

		public override void PerformTest()
		{
			TestCiphers();
			TestWrap();
			TestOids();
			TestWrapOids();
			TestEax();
			TestCcm();
			TestGcm();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new AesTest());
		}
	}
}
