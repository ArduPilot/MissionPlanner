using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* TEA tester - based on C implementation results from http://www.simonshepherd.supanet.com/tea.htm
	*/
	[TestFixture]
	public class XteaTest
		: CipherTest
	{
		private static readonly SimpleTest[] tests =
		{
			new BlockCipherVectorTest(0, new XteaEngine(),
				new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
				"0000000000000000", "dee9d4d8f7131ed9"),
			new BlockCipherVectorTest(1, new XteaEngine(),
				new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
				"0102030405060708", "065c1b8975c6a816"),
			new BlockCipherVectorTest(2, new XteaEngine(),
				new KeyParameter(Hex.Decode("0123456712345678234567893456789A")),
				"0000000000000000", "1ff9a0261ac64264"),
			new BlockCipherVectorTest(3, new XteaEngine(),
				new KeyParameter(Hex.Decode("0123456712345678234567893456789A")),
				"0102030405060708", "8c67155b2ef91ead"),
		};

		public XteaTest()
			: base(tests, new XteaEngine(), new KeyParameter(new byte[16]))
		{
		}

		public override string Name
		{
			get { return "XTEA"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new XteaTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
