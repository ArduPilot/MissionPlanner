using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	[TestFixture]
	public class DigestRandomNumberTest
		: SimpleTest
	{
		private static readonly byte[] ZERO_SEED = { 0, 0, 0, 0, 0, 0, 0, 0 };

		private static readonly byte[] TEST_SEED = Hex.Decode("81dcfafc885914057876");

		private static readonly byte[] expected0SHA1 = Hex.Decode("95bca677b3d4ff793213c00892d2356ec729ee02");
		private static readonly byte[] noCycle0SHA1 = Hex.Decode("d57ccd0eb12c3938d59226412bc1268037b6b846");
		private static readonly byte[] expected0SHA256 = Hex.Decode("587e2dfd597d086e47ddcd343eac983a5c913bef8c6a1a560a5c1bc3a74b0991");
		private static readonly byte[] noCycle0SHA256 = Hex.Decode("e5776c4483486ba7be081f4e1b9dafbab25c8fae290fd5474c1ceda2c16f9509");
		private static readonly byte[] expected100SHA1 = Hex.Decode("b9d924092546e0876cafd4937d7364ebf9efa4be");
		private static readonly byte[] expected100SHA256 = Hex.Decode("fbc4aa54b948b99de104c44563a552899d718bb75d1941cc62a2444b0506abaf");
		private static readonly byte[] expectedTestSHA1 = Hex.Decode("e9ecef9f5306daf1ac51a89a211a64cb24415649");
		private static readonly byte[] expectedTestSHA256 = Hex.Decode("bdab3ca831b472a2fa09bd1bade541ef16c96640a91fcec553679a136061de98");

		private static readonly byte[] sha1Xors = Hex.Decode("7edcc1216934f3891b03ffa65821611a3e2b1f79");
		private static readonly byte[] sha256Xors = Hex.Decode("5ec48189cc0aa71e79c707bc3c33ffd47bbba368a83d6cfebf3cd3969d7f3eed");

		public override string Name
		{
			get { return "DigestRandomNumber"; }
		}

		private void doExpectedTest(IDigest digest, int seed, byte[] expected)
		{
			doExpectedTest(digest, seed, expected, null);
		}
	    
		private void doExpectedTest(IDigest digest, int seed, byte[] expected, byte[] noCycle)
		{
			DigestRandomGenerator rGen = new DigestRandomGenerator(digest);
			byte[] output = new byte[digest.GetDigestSize()];

			rGen.AddSeedMaterial(seed);

			for (int i = 0; i != 1024; i++)
			{
				rGen.NextBytes(output);
			}

			if (noCycle != null)
			{
				if (Arrays.AreEqual(noCycle, output))
				{
					Fail("seed not being cycled!");
				}
			}

			if (!Arrays.AreEqual(expected, output))
			{
				Fail("expected output doesn't match");
			}
		}

		private void doExpectedTest(IDigest digest, byte[] seed, byte[] expected)
		{
			DigestRandomGenerator rGen = new DigestRandomGenerator(digest);
			byte[] output = new byte[digest.GetDigestSize()];

			rGen.AddSeedMaterial(seed);

			for (int i = 0; i != 1024; i++)
			{
				rGen.NextBytes(output);
			}

			if (!Arrays.AreEqual(expected, output))
			{
				Fail("expected output doesn't match");
			}
		}

		private void doCountTest(IDigest digest, byte[] seed, byte[] expectedXors)
		{
			DigestRandomGenerator rGen = new DigestRandomGenerator(digest);
			byte[] output = new byte[digest.GetDigestSize()];
			int[] averages = new int[digest.GetDigestSize()];
			byte[] ands = new byte[digest.GetDigestSize()];
			byte[] xors = new byte[digest.GetDigestSize()];
			byte[] ors = new byte[digest.GetDigestSize()];

			rGen.AddSeedMaterial(seed);

			for (int i = 0; i != 1000000; i++)
			{
				rGen.NextBytes(output);
				for (int j = 0; j != output.Length; j++)
				{
					averages[j] += output[j] & 0xff;
					ands[j] &= output[j];
					xors[j] ^= output[j];
					ors[j] |= output[j];
				}
			}

			for (int i = 0; i != output.Length; i++)
			{
				if ((averages[i] / 1000000) != 127)
				{
					Fail("average test failed for " + digest.AlgorithmName);
				}
				if (ands[i] != 0)
				{
					Fail("and test failed for " + digest.AlgorithmName);
				}
				if ((ors[i] & 0xff) != 0xff)
				{
					Fail("or test failed for " + digest.AlgorithmName);
				}
				if (xors[i] != expectedXors[i])
				{
					Fail("xor test failed for " + digest.AlgorithmName);
				}
			}
		}

		public override void PerformTest()
		{
			doExpectedTest(new Sha1Digest(), 0, expected0SHA1, noCycle0SHA1);
			doExpectedTest(new Sha256Digest(), 0, expected0SHA256, noCycle0SHA256);

			doExpectedTest(new Sha1Digest(), 100, expected100SHA1);
			doExpectedTest(new Sha256Digest(), 100, expected100SHA256);

			doExpectedTest(new Sha1Digest(), ZERO_SEED, expected0SHA1);
			doExpectedTest(new Sha256Digest(), ZERO_SEED, expected0SHA256);

			doExpectedTest(new Sha1Digest(), TEST_SEED, expectedTestSHA1);
			doExpectedTest(new Sha256Digest(), TEST_SEED, expectedTestSHA256);

			doCountTest(new Sha1Digest(), TEST_SEED, sha1Xors);
			doCountTest(new Sha256Digest(), TEST_SEED, sha256Xors);
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DigestRandomNumberTest());
		}
	}
}
