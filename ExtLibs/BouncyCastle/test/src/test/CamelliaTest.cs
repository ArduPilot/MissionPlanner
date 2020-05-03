using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Ntt;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Tests
{
	/// <summary>Basic test class for Camellia</summary>
	[TestFixture]
	public class CamelliaTest
		: BaseBlockCipherTest
	{
		internal static readonly string[] cipherTests =
		{
			"128",
			"0123456789abcdeffedcba9876543210",
			"0123456789abcdeffedcba9876543210",
			"67673138549669730857065648eabe43",
			"192",
			"0123456789abcdeffedcba98765432100011223344556677",
			"0123456789abcdeffedcba9876543210",
			"b4993401b3e996f84ee5cee7d79b09b9",
			"256",
			"0123456789abcdeffedcba987654321000112233445566778899aabbccddeeff",
			"0123456789abcdeffedcba9876543210",
			"9acc237dff16d76c20ef7c919e3a7509",
		};

		public CamelliaTest()
			: base("Camellia")
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
			byte[] out1 = Hex.Decode("635d6ac46eedebd3a7f4a06421a4cbd1746b24795ba2f708");

			wrapTest(1, "CamelliaWrap", kek1, in1, out1);
		}

		[Test]
		public void TestOids()
		{
			string[] oids = {
				NttObjectIdentifiers.IdCamellia128Cbc.Id,
				NttObjectIdentifiers.IdCamellia192Cbc.Id,
				NttObjectIdentifiers.IdCamellia256Cbc.Id
			};

			string[] names = {
				"Camellia/CBC/PKCS7Padding",
				"Camellia/CBC/PKCS7Padding",
				"Camellia/CBC/PKCS7Padding"
			};

			oidTest(oids, names, 1);
		}

		[Test]
		public void TestWrapOids()
		{
			string[] wrapOids = {
				NttObjectIdentifiers.IdCamellia128Wrap.Id,
				NttObjectIdentifiers.IdCamellia192Wrap.Id,
				NttObjectIdentifiers.IdCamellia256Wrap.Id
			};

			wrapOidTest(wrapOids, "CamelliaWrap");
		}

		public void doCipherTest(
			int		strength,
			byte[]	keyBytes,
			byte[]	input,
			byte[]	output)
		{
			KeyParameter key = ParameterUtilities.CreateKeyParameter("Camellia", keyBytes);

			IBufferedCipher inCipher = CipherUtilities.GetCipher("Camellia/ECB/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("Camellia/ECB/NoPadding");

			try
			{
				outCipher.Init(true, key);
			}
			catch (Exception e)
			{
				Fail("Camellia failed initialisation - " + e.ToString(), e);
			}

			try
			{
				inCipher.Init(false, key);
			}
			catch (Exception e)
			{
				Fail("Camellia failed initialisation - " + e.ToString(), e);
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
				Fail("Camellia failed encryption - " + e.ToString(), e);
			}

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("Camellia failed encryption - expected "
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
				Fail("Camellia failed encryption - " + e.ToString(), e);
			}

			if (!AreEqual(bytes, input))
			{
				Fail("Camellia failed decryption - expected "
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
			RunTest(new CamelliaTest());
		}
	}
}
