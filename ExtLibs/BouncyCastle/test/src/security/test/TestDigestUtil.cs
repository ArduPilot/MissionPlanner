using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace Org.BouncyCastle.Security.Tests
{
    [TestFixture]
    public class TestDigestUtilities
    {
		private static readonly byte[] TestBytes = new byte[100];

		static TestDigestUtilities()
		{
			new SecureRandom().NextBytes(TestBytes);
		}

		[Test]
        public void TestAlgorithms()
        {
			CheckAlgorithm("GOST3411", new Gost3411Digest());
			CheckAlgorithm("MD2", new MD2Digest());
            CheckAlgorithm("MD4", new MD4Digest());
            CheckAlgorithm("MD5", new MD5Digest());
			CheckAlgorithm("RipeMD128", new RipeMD128Digest());
			CheckAlgorithm("RipeMD160", new RipeMD160Digest());
			CheckAlgorithm("RipeMD256", new RipeMD256Digest());
			CheckAlgorithm("RipeMD320", new RipeMD320Digest());
			CheckAlgorithm("SHA-1", new Sha1Digest());
            CheckAlgorithm("SHA-224", new Sha224Digest());
            CheckAlgorithm("SHA-256", new Sha256Digest());
            CheckAlgorithm("SHA-384", new Sha384Digest());
            CheckAlgorithm("SHA-512", new Sha512Digest());
			CheckAlgorithm("Tiger", new TigerDigest());
			CheckAlgorithm("Whirlpool", new WhirlpoolDigest());
		}

		private void CheckAlgorithm(
            string	name,
            IDigest	digest)
        {
            byte[] hash1 = MakeTestHash(digest);
			byte[] hash2 = MakeTestHash(DigestUtilities.GetDigest(name));

			Assert.AreEqual(hash1, hash2, name);
        }

		private byte[] MakeTestHash(
			IDigest digest)
		{
			for (int i = 0; i < digest.GetDigestSize(); ++i)
			{
				digest.Update((byte) i);
			}

			digest.BlockUpdate(TestBytes, 0, TestBytes.Length);

			return DigestUtilities.DoFinal(digest);
		}
    }
}
