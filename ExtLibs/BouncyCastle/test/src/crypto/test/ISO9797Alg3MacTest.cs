using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class ISO9797Alg3MacTest
		: SimpleTest
	{
		static byte[] keyBytes = Hex.Decode("7CA110454A1A6E570131D9619DC1376E");
		static byte[] ivBytes = Hex.Decode("0000000000000000");

		static byte[] input1 = Encoding.ASCII.GetBytes("Hello World !!!!");

		static byte[] output1 = Hex.Decode("F09B856213BAB83B");

		public ISO9797Alg3MacTest()
		{
		}

		public override void PerformTest()
		{
			KeyParameter key = new KeyParameter(keyBytes);
			IBlockCipher cipher = new DesEngine();
			IMac mac = new ISO9797Alg3Mac(cipher);

			//
			// standard DAC - zero IV
			//
			mac.Init(key);

			mac.BlockUpdate(input1, 0, input1.Length);

			byte[] outBytes = new byte[8];

			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output1))
			{
				Fail("Failed - expected " + Hex.ToHexString(output1) + " got " + Hex.ToHexString(outBytes));
			}

			//
			//  reset
			//
			mac.Reset();

			mac.Init(key);

			for (int i = 0; i != input1.Length / 2; i++)
			{
				mac.Update(input1[i]);
			}

			mac.BlockUpdate(input1, input1.Length / 2, input1.Length - (input1.Length / 2));

			mac.DoFinal(outBytes, 0);

			if (!AreEqual(outBytes, output1))
			{
				Fail("Reset failed - expected " + Hex.ToHexString(output1) + " got " + Hex.ToHexString(outBytes));
			}
		}

		public override string Name
		{
			get
			{
				return "ISO9797Alg3Mac";
			}
		}

		public static void Main(
			string[] args)
		{
			ISO9797Alg3MacTest test = new ISO9797Alg3MacTest();
			ITestResult result = test.Perform();

			Console.WriteLine(result);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
