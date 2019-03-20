using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	/// <remarks>Basic test class for the GOST28147 cipher</remarks>
	[TestFixture]
	public class Gost28147Test
		: SimpleTest
	{
		private static string[] cipherTests =
		{
			"256",
			"0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
			"4e6f77206973207468652074696d6520666f7220616c6c20",
			"281630d0d5770030068c252d841e84149ccc1912052dbc02",

			"256",
			"0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
			"4e6f77206973207468652074696d65208a920c6ed1a804f5",
			"88e543dfc04dc4f764fa7b624741cec07de49b007bf36065"
		};

		public override string Name
		{
			get { return "GOST28147"; }
		}

		private void doTestEcb(
			int		strength,
			byte[]	keyBytes,
			byte[]	input,
			byte[]	output)
		{
			IBufferedCipher inCipher, outCipher;
			CipherStream cIn, cOut;
			MemoryStream bIn, bOut;

			KeyParameter key = ParameterUtilities.CreateKeyParameter("GOST28147", keyBytes);

			inCipher = CipherUtilities.GetCipher("GOST28147/ECB/NoPadding");
			outCipher = CipherUtilities.GetCipher("GOST28147/ECB/NoPadding");
			outCipher.Init(true, key);
			inCipher.Init(false, key);

			//
			// encryption pass
			//
			bOut = new MemoryStream();
			cOut = new CipherStream(bOut, null, outCipher);

			for (int i = 0; i != input.Length / 2; i++)
			{
				cOut.WriteByte(input[i]);
			}
			cOut.Write(input, input.Length / 2, input.Length - input.Length / 2);
			cOut.Close();

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("GOST28147 failed encryption - expected "
					+ Hex.ToHexString(output) + " got " + Hex.ToHexString(bytes));
			}

			//
			// decryption pass
			//
			bIn = new MemoryStream(bytes, false);
			cIn = new CipherStream(bIn, inCipher, null);

			BinaryReader dIn = new BinaryReader(cIn);

			bytes = new byte[input.Length];

			for (int i = 0; i != input.Length / 2; i++)
			{
				bytes[i] = dIn.ReadByte();
			}

			int remaining = bytes.Length - input.Length / 2;
			byte[] extra = dIn.ReadBytes(remaining);
			if (extra.Length < remaining)
				throw new EndOfStreamException();
			extra.CopyTo(bytes, input.Length / 2);

			if (!AreEqual(bytes, input))
			{
				Fail("GOST28147 failed decryption - expected " + Hex.ToHexString(input) + " got " + Hex.ToHexString(bytes));
			}
		}

		private void doTestCfb(
			int         strength,
			byte[]      keyBytes,
			byte[]      input,
			byte[]      output)
		{
			IBufferedCipher inCipher, outCipher;
			CipherStream cIn, cOut;
			MemoryStream bIn, bOut;

			KeyParameter key = ParameterUtilities.CreateKeyParameter("GOST28147", keyBytes);

			inCipher = CipherUtilities.GetCipher("GOST28147/CFB8/NoPadding");
			outCipher = CipherUtilities.GetCipher("GOST28147/CFB8/NoPadding");
			byte[] iv = {1,2,3,4,5,6,7,8};

			outCipher.Init(true, new ParametersWithIV(key, iv));
			inCipher.Init(false, new ParametersWithIV(key, iv));

			//
			// encryption pass
			//
			bOut = new MemoryStream();
			cOut = new CipherStream(bOut, null, outCipher);

			for (int i = 0; i != input.Length / 2; i++)
			{
				cOut.WriteByte(input[i]);
			}
			cOut.Write(input, input.Length / 2, input.Length - input.Length / 2);
			cOut.Close();

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("GOST28147 failed encryption - expected " + Hex.ToHexString(output) + " got " + Hex.ToHexString(bytes));
			}

			//
			// decryption pass
			//
			bIn = new MemoryStream(bytes, false);
			cIn = new CipherStream(bIn, inCipher, null);

			BinaryReader dIn = new BinaryReader(cIn);

			bytes = new byte[input.Length];

			for (int i = 0; i != input.Length / 2; i++)
			{
				bytes[i] = dIn.ReadByte();
			}

			int remaining = bytes.Length - input.Length / 2;
			byte[] extra = dIn.ReadBytes(remaining);
			if (extra.Length < remaining)
				throw new EndOfStreamException();
			extra.CopyTo(bytes, input.Length / 2);

			if (!AreEqual(bytes, input))
			{
				Fail("GOST28147 failed decryption - expected " + Hex.ToHexString(input) + " got " + Hex.ToHexString(bytes));
			}
		}

		private void doOidTest()
		{
			string[] oids = {
					CryptoProObjectIdentifiers.GostR28147Cbc.Id,
			};

			string[] names = {
				"GOST28147/CBC/PKCS7Padding"
			};

			try
			{
				byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
//				IvParameterSpec ivSpec = new IvParameterSpec(new byte[8]);
				byte[] iv = new byte[8];
			    
				for (int i = 0; i != oids.Length; i++)
				{
					IBufferedCipher c1 = CipherUtilities.GetCipher(oids[i]);
					IBufferedCipher c2 = CipherUtilities.GetCipher(names[i]);

//					KeyGenerator kg = KeyGenerator.getInstance(oids[i]);
//					SecretKey k = kg.generateKey();
					CipherKeyGenerator kg = GeneratorUtilities.GetKeyGenerator(oids[i]);
					KeyParameter k = ParameterUtilities.CreateKeyParameter(oids[i], kg.GenerateKey());

					c1.Init(true, new ParametersWithIV(k, iv));
					c2.Init(false, new ParametersWithIV(k, iv));

					byte[] result = c2.DoFinal(c1.DoFinal(data));

					if (!AreEqual(data, result))
					{
						Fail("failed OID test");
					}
				}
			}
			catch (Exception ex)
			{
				Fail("failed exception " + ex.ToString(), ex);
			}
		}

		public override void PerformTest() 
		{
			for (int i = 0; i != cipherTests.Length; i += 8)
			{
				doTestEcb(int.Parse(cipherTests[i]),
					Hex.Decode(cipherTests[i + 1]),
					Hex.Decode(cipherTests[i + 2]),
					Hex.Decode(cipherTests[i + 3]));

				doTestCfb(int.Parse(cipherTests[i + 4]),
					Hex.Decode(cipherTests[i + 4 + 1]),
					Hex.Decode(cipherTests[i + 4 + 2]),
					Hex.Decode(cipherTests[i + 4 + 3]));

				doOidTest();
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new Gost28147Test());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}