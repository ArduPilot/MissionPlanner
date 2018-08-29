using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	 * GOST 28147 MAC tester
	 */
	[TestFixture]
	public class Gost28147MacTest
		:	ITest
	{
		//
		// these GOSTMac for testing.
		//
		static byte[]   gkeyBytes1 = Hex.Decode("6d145dc993f4019e104280df6fcd8cd8e01e101e4c113d7ec4f469ce6dcd9e49");
		static byte[]   gkeyBytes2 = Hex.Decode("6d145dc993f4019e104280df6fcd8cd8e01e101e4c113d7ec4f469ce6dcd9e49");

		static byte[]   input3 = Hex.Decode("7768617420646f2079612077616e7420666f72206e6f7468696e673f");
		static byte[]   input4 = Hex.Decode("7768617420646f2079612077616e7420666f72206e6f7468696e673f");

		static byte[]   output7 = Hex.Decode("93468a46");
		static byte[]   output8 = Hex.Decode("93468a46");

		public Gost28147MacTest()
		{
		}

		public ITestResult Perform()
		{
			// test1
			IMac mac = new Gost28147Mac();
			KeyParameter key = new KeyParameter(gkeyBytes1);

			mac.Init(key);

			mac.BlockUpdate(input3, 0, input3.Length);

			byte[] outBytes = new byte[4];

			mac.DoFinal(outBytes, 0);

			if (!Arrays.AreEqual(outBytes, output7))
			{
				return new SimpleTestResult(false, Name + ": Failed test 1 - expected "
					+ Hex.ToHexString(output7)
					+ " got " + Hex.ToHexString(outBytes));
			}

			// test2
			key = new KeyParameter(gkeyBytes2);

			ParametersWithSBox gparam = new ParametersWithSBox(key, Gost28147Engine.GetSBox("E-A"));

			mac.Init(gparam);

			mac.BlockUpdate(input4, 0, input4.Length);

			outBytes = new byte[4];

			mac.DoFinal(outBytes, 0);

			if (!Arrays.AreEqual(outBytes, output8))
			{
				return new SimpleTestResult(false, Name + ": Failed test 2 - expected "
					+ Hex.ToHexString(output8)
					+ " got " + Hex.ToHexString(outBytes));
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public string Name
		{
			get { return "Gost28147Mac"; }
		}

		public static void Main(
			string[] args)
		{
			ITest test = new Gost28147MacTest();
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
