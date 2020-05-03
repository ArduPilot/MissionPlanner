using System;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* General Padding tests.
	*/
	[TestFixture]
	public class PaddingTest : SimpleTest
	{
		public PaddingTest()
		{
		}

		private void blockCheck(
			PaddedBufferedBlockCipher   cipher,
			IBlockCipherPadding          padding,
			KeyParameter                key,
			byte[]                      data)
		{
			byte[]  outBytes = new byte[data.Length + 8];
			byte[]  dec = new byte[data.Length];

			try
			{
				cipher.Init(true, key);

				int    len = cipher.ProcessBytes(data, 0, data.Length, outBytes, 0);

				len += cipher.DoFinal(outBytes, len);

				cipher.Init(false, key);

				int    decLen = cipher.ProcessBytes(outBytes, 0, len, dec, 0);

				decLen += cipher.DoFinal(dec, decLen);

				if (!AreEqual(data, dec))
				{
					Fail("failed to decrypt - i = " + data.Length + ", padding = " + padding.PaddingName);
				}
			}
			catch (Exception e)
			{
				Fail("Exception - " + e.ToString(), e);
			}
		}

		public void doTestPadding(
			IBlockCipherPadding  padding,
			SecureRandom        rand,
			byte[]              ffVector,
			byte[]              ZeroVector)
		{
			PaddedBufferedBlockCipher    cipher = new PaddedBufferedBlockCipher(new DesEngine(), padding);
			KeyParameter                 key = new KeyParameter(Hex.Decode("0011223344556677"));

			//
			// ff test
			//
			byte[]    data = { (byte)0xff, (byte)0xff, (byte)0xff, (byte)0, (byte)0, (byte)0, (byte)0, (byte)0 };

			if (ffVector != null)
			{
				padding.AddPadding(data, 3);

				if (!AreEqual(data, ffVector))
				{
					Fail("failed ff test for " + padding.PaddingName);
				}
			}

			//
			// zero test
			//
			if (ZeroVector != null)
			{
				data = new byte[8];
				padding.AddPadding(data, 4);

				if (!AreEqual(data, ZeroVector))
				{
					Fail("failed zero test for " + padding.PaddingName);
				}
			}

			for (int i = 1; i != 200; i++)
			{
				data = new byte[i];

				rand.NextBytes(data);

				blockCheck(cipher, padding, key, data);
			}
		}

		public override void PerformTest()
		{
            SecureRandom rand = SecureRandom.GetInstance("SHA1PRNG");

            doTestPadding(new Pkcs7Padding(), rand,
				Hex.Decode("ffffff0505050505"),
				Hex.Decode("0000000004040404"));

			Pkcs7Padding padder = new Pkcs7Padding();
			try
			{
				padder.PadCount(new byte[8]);

				Fail("invalid padding not detected");
			}
			catch (InvalidCipherTextException e)
			{
				if (!"pad block corrupted".Equals(e.Message))
				{
					Fail("wrong exception for corrupt padding: " + e);
				}
			} 

			doTestPadding(new ISO10126d2Padding(), rand,
				null,
				null);

			doTestPadding(new X923Padding(), rand,
				null,
				null);

			doTestPadding(new TbcPadding(), rand,
				Hex.Decode("ffffff0000000000"),
				Hex.Decode("00000000ffffffff"));

			doTestPadding(new ZeroBytePadding(), rand,
				Hex.Decode("ffffff0000000000"),
				null);

			doTestPadding(new ISO7816d4Padding(), rand,
				Hex.Decode("ffffff8000000000"),
				Hex.Decode("0000000080000000"));
		}

		public override string Name
		{
			get { return "PaddingTest"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new PaddingTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
