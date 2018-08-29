using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	 * RIPEMD128 Digest Test
	 */
	[TestFixture]
	public class RipeMD128DigestTest
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
			"cdf26213a150dc3ecb610f18f6b38b46",
			"86be7afa339d0fc7cfc785e72f578d33",
			"c14a12199c66e4ba84636b0f69144c77",
			"9e327b3d6e523062afc1132d7df9d1b8",
			"fd2aa607f71dc8f510714922b371834e",
			"a1aa0689d0fafa2ddc22e88b49133a06",
			"d1e959eb179c911faea4624c60c5c702",
			"3f45ef194732c2dbb2c4a2c769795fa3"
		};

		readonly static String million_a_digest = "4a7f5723f954eba1216c9d8f6320431f";

		public RipeMD128DigestTest()
			: base(new RipeMD128Digest(), messages, digests)
		{
		}

		public override void PerformTest()
		{
			base.PerformTest();

			millionATest(million_a_digest);
		}

		protected override IDigest CloneDigest(IDigest digest)
		{
			return new RipeMD128Digest((RipeMD128Digest)digest);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new RipeMD128DigestTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
