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
	public class TeaTest
		: CipherTest
	{
		private static readonly SimpleTest[] tests =
		{
			new BlockCipherVectorTest(0, new TeaEngine(),
				new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
				"0000000000000000","41ea3a0a94baa940"),
			new BlockCipherVectorTest(1, new TeaEngine(),
				new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
				"0102030405060708", "6a2f9cf3fccf3c55"),
			new BlockCipherVectorTest(2, new TeaEngine(),
				new KeyParameter(Hex.Decode("0123456712345678234567893456789A")),
				"0000000000000000", "34e943b0900f5dcb"),
			new BlockCipherVectorTest(3, new TeaEngine(),
				new KeyParameter(Hex.Decode("0123456712345678234567893456789A")),
				"0102030405060708", "773dc179878a81c0"),
		};

		public TeaTest()
			: base(tests, new TeaEngine(), new KeyParameter(new byte[16]))
		{
		}

		public override string Name
		{
			get { return "TEA"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new TeaTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
