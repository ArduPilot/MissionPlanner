using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	 * RIPEMD256 Digest Test
	 */
	[TestFixture]
	public class RipeMD256DigestTest
		: DigestTest
	{
		readonly static string[] messages = {
			"",
			"a",
			"abc",
			"message digest",
			"abcdefghijklmnopqrstuvwxyz",
			"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq",
			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
			"12345678901234567890123456789012345678901234567890123456789012345678901234567890"
		};

		readonly static string[] digests = {
			"02ba4c4e5f8ecd1877fc52d64d30e37a2d9774fb1e5d026380ae0168e3c5522d",
			"f9333e45d857f5d90a91bab70a1eba0cfb1be4b0783c9acfcd883a9134692925",
			"afbd6e228b9d8cbbcef5ca2d03e6dba10ac0bc7dcbe4680e1e42d2e975459b65",
			"87e971759a1ce47a514d5c914c392c9018c7c46bc14465554afcdf54a5070c0e",
			"649d3034751ea216776bf9a18acc81bc7896118a5197968782dd1fd97d8d5133",
			"3843045583aac6c8c8d9128573e7a9809afb2a0f34ccc36ea9e72f16f6368e3f",
			"5740a408ac16b720b84424ae931cbb1fe363d1d0bf4017f1a89f7ea6de77a0b8",
			"06fdcc7a409548aaf91368c06a6275b553e3f099bf0ea4edfd6778df89a890dd"
		};

		readonly static string million_a_digest = "ac953744e10e31514c150d4d8d7b677342e33399788296e43ae4850ce4f97978";

		public RipeMD256DigestTest()
			: base(new RipeMD256Digest(), messages, digests)
		{
		}

		public override void PerformTest()
		{
			base.PerformTest();

			millionATest(million_a_digest);
		}

		protected override IDigest CloneDigest(IDigest digest)
		{
			return new RipeMD256Digest((RipeMD256Digest)digest);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new RipeMD256DigestTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
