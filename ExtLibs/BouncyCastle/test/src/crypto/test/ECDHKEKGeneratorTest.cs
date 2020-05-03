using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks>ECDHKEK Generator tests.</remarks>
	[TestFixture]
	public class ECDHKekGeneratorTest
		: SimpleTest
	{
		private byte[] seed1 = Hex.Decode("db4a8daba1f98791d54e940175dd1a5f3a0826a1066aa9b668d4dc1e1e0790158dcad1533c03b44214d1b61fefa8b579");
		private DerObjectIdentifier alg1 = NistObjectIdentifiers.IdAes256Wrap;
		private byte[] result1 = Hex.Decode("8ecc6d85caf25eaba823a7d620d4ab0d33e4c645f2");

		private byte[] seed2 = Hex.Decode("75d7487b5d3d2bfb3c69ce0365fe64e3bfab5d0d63731628a9f47eb8fddfa28c65decaf228a0b38f0c51c6a3356d7c56");
		private DerObjectIdentifier alg2 = NistObjectIdentifiers.IdAes128Wrap;
		private byte[] result2 = Hex.Decode("042be1faca3a4a8fc859241bfb87ba35");

		private byte[] seed3 = Hex.Decode("fdeb6d809f997e8ac174d638734dc36d37aaf7e876e39967cd82b1cada3de772449788461ee7f856bad9305627f8e48b");
		private DerObjectIdentifier alg3 = PkcsObjectIdentifiers.IdAlgCms3DesWrap;
		private byte[] result3 = Hex.Decode("bcd701fc92109b1b9d6f3b6497ad5ca9627fa8a597010305");

		public ECDHKekGeneratorTest()
		{
		}

		public override void PerformTest()
		{
			CheckMask(1, new ECDHKekGenerator(new Sha1Digest()), new DHKdfParameters(alg1, 256, seed1), result1);
			CheckMask(2, new ECDHKekGenerator(new Sha1Digest()), new DHKdfParameters(alg2, 128, seed2), result2);
			CheckMask(3, new ECDHKekGenerator(new Sha1Digest()), new DHKdfParameters(alg3, 192, seed3), result3);
		}

		private void CheckMask(
			int						count,
			IDerivationFunction		kdf,
			IDerivationParameters	parameters,
			byte[]					result)
		{
			byte[] data = new byte[result.Length];

			kdf.Init(parameters);

			kdf.GenerateBytes(data, 0, data.Length);

			if (!AreEqual(result, data))
			{
				Fail("ECDHKekGenerator failed generator test " + count);
			}
		}

		public override string Name
		{
			get { return "ECDHKekGenerator"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new ECDHKekGeneratorTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
