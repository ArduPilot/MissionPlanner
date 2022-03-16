using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Agreement.Kdf;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	/// <remarks>DHKEK Generator tests - from RFC 2631.</remarks>
	[TestFixture]
	public class DHKekGeneratorTest
		: SimpleTest
	{
		private byte[] seed1 = Hex.Decode("000102030405060708090a0b0c0d0e0f10111213");
		private DerObjectIdentifier alg1 = PkcsObjectIdentifiers.IdAlgCms3DesWrap;
		private byte[] result1 = Hex.Decode("a09661392376f7044d9052a397883246b67f5f1ef63eb5fb");

		private byte[] seed2 = Hex.Decode("000102030405060708090a0b0c0d0e0f10111213");
		private DerObjectIdentifier alg2 = PkcsObjectIdentifiers.IdAlgCmsRC2Wrap;
		private byte[] partyAInfo = Hex.Decode(
				"0123456789abcdeffedcba9876543201"
			+ "0123456789abcdeffedcba9876543201"
			+ "0123456789abcdeffedcba9876543201"
			+ "0123456789abcdeffedcba9876543201");
		private byte[] result2 = Hex.Decode("48950c46e0530075403cce72889604e0");

		public DHKekGeneratorTest()
		{
		}

		public override void PerformTest()
		{
			CheckMask(1, new DHKekGenerator(new Sha1Digest()), new DHKdfParameters(alg1, 192, seed1), result1);
			CheckMask(2, new DHKekGenerator(new Sha1Digest()), new DHKdfParameters(alg2, 128, seed2, partyAInfo), result2);
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
				Fail("DHKekGenerator failed generator test " + count);
			}
		}

		public override string Name
		{
			get { return "DHKekGenerator"; }
		}

		public static void Main(
			string[] args)
		{
			RunTest(new DHKekGeneratorTest());
		}

		[Test]
		public void TestFunction()
		{
			string resultText = Perform().ToString();

			Assert.AreEqual(Name + ": Okay", resultText);
		}
	}
}
