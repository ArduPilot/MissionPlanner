using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/**
	 * standard vector test for SHA-224 from RFC 3874 - only the last three are in
	 * the RFC.
	 */
	[TestFixture]
	public class Sha224DigestTest
		: DigestTest
	{
		private static string[] messages =
		{
			"",
			"a",
			"abc",
			"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"
		};

		private static string[] digests =
		{
			"d14a028c2a3a2bc9476102bb288234c415a2b01f828ea62ac5b3e42f",
			"abd37534c7d9a2efb9465de931cd7055ffdb8879563ae98078d6d6d5",
			"23097d223405d8228642a477bda255b32aadbce4bda0b3f7e36c9da7",
			"75388b16512776cc5dba5da1fd890150b0c6455cb4f58b1952522525"
		};

		// 1 million 'a'
		private static string million_a_digest = "20794655980c91d8bbb4c1ea97618a4bf03f42581948b2ee4ee7ad67";

		public Sha224DigestTest()
			: base(new Sha224Digest(), messages, digests)
		{
		}

		public override void PerformTest()
		{
			base.PerformTest();

			millionATest(million_a_digest);
		}

		protected override IDigest CloneDigest(IDigest digest)
		{
			return new Sha224Digest((Sha224Digest)digest);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new Sha224DigestTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
