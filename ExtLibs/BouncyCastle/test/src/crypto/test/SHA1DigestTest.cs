using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks>Standard vector test for SHA-1 from "Handbook of Applied Cryptography", page 345.</remarks>
	[TestFixture]
	public class Sha1DigestTest
		: DigestTest
	{
		private static string[] messages =
		{
			"",
			"a",
			"abc",
			"abcdefghijklmnopqrstuvwxyz"
		};

		private static string[] digests =
		{
			"da39a3ee5e6b4b0d3255bfef95601890afd80709",
			"86f7e437faa5a7fce15d1ddcb9eaeaea377667b8",
			"a9993e364706816aba3e25717850c26c9cd0d89d",
			"32d10c7b8cf96570ca04ce37f2a19d84240d3a89"
		};

		public Sha1DigestTest()
			: base(new Sha1Digest(), messages, digests)
		{
		}

		protected override IDigest CloneDigest(IDigest digest)
		{
			return new Sha1Digest((Sha1Digest)digest);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new Sha1DigestTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
