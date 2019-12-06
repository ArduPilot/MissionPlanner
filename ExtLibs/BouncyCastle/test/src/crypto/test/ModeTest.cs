using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks> CFB/OFB Mode test of IV padding.</remarks>
	[TestFixture]
	public class ModeTest
		: ITest
	{
		public string Name
		{
			get { return "ModeTest"; }
		}

		public ModeTest()
		{
		}

		public virtual ITestResult Perform()
		{
			KeyParameter key = new KeyParameter(Hex.Decode("0011223344556677"));
			byte[] input = Hex.Decode("4e6f7720");
			byte[] out1 = new byte[4];
			byte[] out2 = new byte[4];


			IBlockCipher ofb = new OfbBlockCipher(new DesEngine(), 32);

			ofb.Init(true, new ParametersWithIV(key, Hex.Decode("1122334455667788")));

			ofb.ProcessBlock(input, 0, out1, 0);

			ofb.Init(false, new ParametersWithIV(key, Hex.Decode("1122334455667788")));
			ofb.ProcessBlock(out1, 0, out2, 0);

			if (!Arrays.AreEqual(out2, input))
			{
				return new SimpleTestResult(false, Name + ": test 1 - in != out");
			}

			ofb.Init(true, new ParametersWithIV(key, Hex.Decode("11223344")));

			ofb.ProcessBlock(input, 0, out1, 0);

			ofb.Init(false, new ParametersWithIV(key, Hex.Decode("0000000011223344")));
			ofb.ProcessBlock(out1, 0, out2, 0);

			if (!Arrays.AreEqual(out2, input))
			{
				return new SimpleTestResult(false, Name + ": test 2 - in != out");
			}

			IBlockCipher cfb = new CfbBlockCipher(new DesEngine(), 32);

			cfb.Init(true, new ParametersWithIV(key, Hex.Decode("1122334455667788")));

			cfb.ProcessBlock(input, 0, out1, 0);

			cfb.Init(false, new ParametersWithIV(key, Hex.Decode("1122334455667788")));
			cfb.ProcessBlock(out1, 0, out2, 0);

			if (!Arrays.AreEqual(out2, input))
			{
				return new SimpleTestResult(false, Name + ": test 3 - in != out");
			}

			cfb.Init(true, new ParametersWithIV(key, Hex.Decode("11223344")));

			cfb.ProcessBlock(input, 0, out1, 0);

			cfb.Init(false, new ParametersWithIV(key, Hex.Decode("0000000011223344")));
			cfb.ProcessBlock(out1, 0, out2, 0);

			if (!Arrays.AreEqual(out2, input))
			{
				return new SimpleTestResult(false, Name + ": test 4 - in != out");
			}

			return new SimpleTestResult(true, Name + ": Okay");
		}

		public static void Main(
			string[] args)
		{
			ITest test = new ModeTest();
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
