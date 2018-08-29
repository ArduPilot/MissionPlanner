using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Kisa;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Tests
{
	/// <summary>Basic test class for SEED</summary>
	[TestFixture]
	public class SeedTest
		: BaseBlockCipherTest
	{
		internal static readonly string[] cipherTests =
		{
			"128",
			"28DBC3BC49FFD87DCFA509B11D422BE7",
			"B41E6BE2EBA84A148E2EED84593C5EC7",
			"9B9B7BFCD1813CB95D0B3618F40F5122"
		};

		public SeedTest()
			: base("SEED")
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
		public void TestWrap()
		{
			byte[] kek1 = Hex.Decode("000102030405060708090a0b0c0d0e0f");
			byte[] in1 = Hex.Decode("00112233445566778899aabbccddeeff");
			byte[] out1 = Hex.Decode("bf71f77138b5afea05232a8dad54024e812dc8dd7d132559");

			wrapTest(1, "SEEDWrap", kek1, in1, out1);
		}

		[Test]
		public void TestOids()
		{
			string[] oids = {
				KisaObjectIdentifiers.IdSeedCbc.Id
			};

			string[] names = {
				"SEED/CBC/PKCS7Padding"
			};

			oidTest(oids, names, 1);
		}

		[Test]
		public void TestWrapOids()
		{
			string[] wrapOids = {
				KisaObjectIdentifiers.IdNpkiAppCmsSeedWrap.Id
			};

			wrapOidTest(wrapOids, "SEEDWrap");
		}

		public void doCipherTest(
			int		strength,
			byte[]	keyBytes,
			byte[]	input,
			byte[]	output)
		{
			KeyParameter key = ParameterUtilities.CreateKeyParameter("SEED", keyBytes);

			IBufferedCipher inCipher = CipherUtilities.GetCipher("SEED/ECB/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("SEED/ECB/NoPadding");

			try
			{
				outCipher.Init(true, key);
			}
			catch (Exception e)
			{
				Fail("SEED failed initialisation - " + e.ToString(), e);
			}

			try
			{
				inCipher.Init(false, key);
			}
			catch (Exception e)
			{
				Fail("SEED failed initialisation - " + e.ToString(), e);
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
				Fail("SEED failed encryption - " + e.ToString(), e);
			}

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("SEED failed encryption - expected "
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
				Fail("SEED failed encryption - " + e.ToString(), e);
			}

			if (!AreEqual(bytes, input))
			{
				Fail("SEED failed decryption - expected "
					+ Hex.ToHexString(input) + " got "
					+ Hex.ToHexString(bytes));
			}
		}

		public override void PerformTest()
		{
			TestCiphers();
			TestWrap();
			TestOids();
			TestWrapOids();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new SeedTest());
		}
	}
}
