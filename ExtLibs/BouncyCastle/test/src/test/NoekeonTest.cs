using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Tests
{
	/**
	* basic test class for SEED
	*/
	[TestFixture]
	public class NoekeonTest
		: BaseBlockCipherTest
	{
		private static readonly string[] cipherTests =
		{
			"128",
			"b1656851699e29fa24b70148503d2dfc",
			"2a78421b87c7d0924f26113f1d1349b2",
			"e2f687e07b75660ffc372233bc47532c"
		};

		public NoekeonTest()
			: base("Noekeon")
		{
		}

		public void DoTest(
			int		strength,
			byte[]	keyBytes,
			byte[]	input,
			byte[]	output)
		{
			KeyParameter key = ParameterUtilities.CreateKeyParameter("Noekeon", keyBytes);

			IBufferedCipher inCipher = CipherUtilities.GetCipher("Noekeon/ECB/NoPadding");
			IBufferedCipher outCipher = CipherUtilities.GetCipher("Noekeon/ECB/NoPadding");

			try
			{
				outCipher.Init(true, key);
			}
			catch (Exception e)
			{
				Fail("Noekeon failed initialisation - " + e.Message, e);
			}

			try
			{
				inCipher.Init(false, key);
			}
			catch (Exception e)
			{
				Fail("Noekeoen failed initialisation - " + e.Message, e);
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
				Fail("Noekeon failed encryption - " + e.Message, e);
			}

			byte[] bytes = bOut.ToArray();

			if (!AreEqual(bytes, output))
			{
				Fail("Noekeon failed encryption - expected "
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
				Fail("Noekeon failed encryption - " + e.Message, e);
			}

			if (!AreEqual(bytes, input))
			{
				Fail("Noekeon failed decryption - expected "
					+ Hex.ToHexString(input) + " got "
					+ Hex.ToHexString(bytes));
			}
		}

		public override void PerformTest()
		{
			for (int i = 0; i != cipherTests.Length; i += 4)
			{
				DoTest(int.Parse(cipherTests[i]),
					Hex.Decode(cipherTests[i + 1]),
					Hex.Decode(cipherTests[i + 2]),
					Hex.Decode(cipherTests[i + 3]));
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new NoekeonTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
