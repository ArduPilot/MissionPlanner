using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	* SEED tester - vectors http://www.ietf.org/rfc/rfc4009.txt
	*/
	[TestFixture]
	public class SeedTest
		: CipherTest
	{
		static SimpleTest[]  tests =
		{
			new BlockCipherVectorTest(0, new SeedEngine(),
				new KeyParameter(Hex.Decode("00000000000000000000000000000000")),
				"000102030405060708090a0b0c0d0e0f",
				"5EBAC6E0054E166819AFF1CC6D346CDB"),
			new BlockCipherVectorTest(0, new SeedEngine(),
				new KeyParameter(Hex.Decode("000102030405060708090a0b0c0d0e0f")),
				"00000000000000000000000000000000",
				"c11f22f20140505084483597e4370f43"),
			new BlockCipherVectorTest(0, new SeedEngine(),
				new KeyParameter(Hex.Decode("4706480851E61BE85D74BFB3FD956185")),
				"83A2F8A288641FB9A4E9A5CC2F131C7D",
				"EE54D13EBCAE706D226BC3142CD40D4A"),
			new BlockCipherVectorTest(0, new SeedEngine(),
				new KeyParameter(Hex.Decode("28DBC3BC49FFD87DCFA509B11D422BE7")),
				"B41E6BE2EBA84A148E2EED84593C5EC7",
				"9B9B7BFCD1813CB95D0B3618F40F5122"),
			new BlockCipherVectorTest(0, new SeedEngine(),
				new KeyParameter(Hex.Decode("0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E")),
				"0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E0E",
				"8296F2F1B007AB9D533FDEE35A9AD850"),
		};

		public SeedTest()
			: base(tests, new SeedEngine(), new KeyParameter(new byte[16]))
		{
		}

		public override string Name
		{
			get { return "SEED"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new SeedTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
