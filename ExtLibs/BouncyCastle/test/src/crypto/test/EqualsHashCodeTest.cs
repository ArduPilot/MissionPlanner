using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{
	internal class DHTestKeyParameters
		: DHKeyParameters
	{
		public DHTestKeyParameters(
			bool			isPrivate,
			DHParameters	parameters)
			: base(isPrivate, parameters)
		{
		}
	}

	internal class ElGamalTestKeyParameters
		: ElGamalKeyParameters
	{
		public ElGamalTestKeyParameters(
			bool				isPrivate,
			ElGamalParameters	parameters)
			: base(isPrivate, parameters)
		{
		}
	}

	[TestFixture]
	public class EqualsHashCodeTest
		: SimpleTest
	{
		private static object Other = new object();

		public override string Name
		{
			get { return "EqualsHashCode"; }
		}

		private void doTest(
			object	a,
			object	equalsA,
			object	notEqualsA)
		{
			if (a.Equals(null))
			{
				Fail("a equaled null");
			}

			if (!a.Equals(equalsA) || !equalsA.Equals(a))
			{
				Fail("equality failed");
			}

			if (a.Equals(Other))
			{
				Fail("other inequality failed");
			}

			if (a.Equals(notEqualsA) || notEqualsA.Equals(a))
			{
				Fail("inequality failed");
			}

			if (a.GetHashCode() != equalsA.GetHashCode())
			{
				Fail("hashCode equality failed");
			}
		}

		[Test]
		public void TestDH()
		{
			BigInteger g512 = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
			BigInteger p512 = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

			DHParameters dhParams = new DHParameters(p512, g512);
			DHKeyGenerationParameters parameters = new DHKeyGenerationParameters(new SecureRandom(), dhParams);         DHKeyPairGenerator          kpGen = new DHKeyPairGenerator();

			kpGen.Init(parameters);

			AsymmetricCipherKeyPair pair = kpGen.GenerateKeyPair();
			DHPublicKeyParameters pu1 = (DHPublicKeyParameters)pair.Public;
			DHPrivateKeyParameters pv1 = (DHPrivateKeyParameters)pair.Private;

			DHPublicKeyParameters pu2 = new DHPublicKeyParameters(pu1.Y, pu1.Parameters);
			DHPrivateKeyParameters pv2 = new DHPrivateKeyParameters(pv1.X, pv1.Parameters);
			DHPublicKeyParameters pu3 = new DHPublicKeyParameters(pv1.X, pu1.Parameters);
			DHPrivateKeyParameters pv3 = new DHPrivateKeyParameters(pu1.Y, pu1.Parameters);

			doTest(pu1, pu2, pu3);
			doTest(pv1, pv2, pv3);

			DHParameters pr1 = pu1.Parameters;
			DHParameters pr2 = new DHParameters(
				pr1.P, pr1.G, pr1.Q, pr1.M, pr1.L, pr1.J, pr1.ValidationParameters);
			DHParameters pr3 = new DHParameters(
				pr1.P.Add(BigInteger.Two), pr1.G, pr1.Q, pr1.M, pr1.L, pr1.J, pr1.ValidationParameters);

			doTest(pr1, pr2, pr3);

			pr3 = new DHParameters(
				pr1.P, pr1.G.Add(BigInteger.One), pr1.Q, pr1.M, pr1.L, pr1.J, pr1.ValidationParameters);

			doTest(pr1, pr2, pr3);

			pu2 = new DHPublicKeyParameters(pu1.Y, pr2);
			pv2 = new DHPrivateKeyParameters(pv1.X, pr2);

			doTest(pu1, pu2, pu3);
			doTest(pv1, pv2, pv3);

			DHValidationParameters vp1 = new DHValidationParameters(new byte[20], 1024);
			DHValidationParameters vp2 = new DHValidationParameters(new byte[20], 1024);
			DHValidationParameters vp3 = new DHValidationParameters(new byte[24], 1024);

			doTest(vp1, vp1, vp3);
			doTest(vp1, vp2, vp3);

			byte[] bytes = new byte[20];
			bytes[0] = 1;

			vp3 = new DHValidationParameters(bytes, 1024);

			doTest(vp1, vp2, vp3);

			vp3 = new DHValidationParameters(new byte[20], 2048);

			doTest(vp1, vp2, vp3);

			DHTestKeyParameters k1 = new DHTestKeyParameters(false, null);
			DHTestKeyParameters k2 = new DHTestKeyParameters(false, null);
			DHTestKeyParameters k3 = new DHTestKeyParameters(false, pu1.Parameters);

			doTest(k1, k2, k3);
		}

		[Test]
		public void TestElGamal()
		{
			BigInteger g512 = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
			BigInteger p512 = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

			ElGamalParameters dhParams = new ElGamalParameters(p512, g512);
			ElGamalKeyGenerationParameters parameters = new ElGamalKeyGenerationParameters(new SecureRandom(), dhParams);         ElGamalKeyPairGenerator kpGen = new ElGamalKeyPairGenerator();

			kpGen.Init(parameters);

			AsymmetricCipherKeyPair pair = kpGen.GenerateKeyPair();
			ElGamalPublicKeyParameters pu1 = (ElGamalPublicKeyParameters)pair.Public;
			ElGamalPrivateKeyParameters pv1 = (ElGamalPrivateKeyParameters)pair.Private;

			ElGamalPublicKeyParameters pu2 = new ElGamalPublicKeyParameters(pu1.Y, pu1.Parameters);
			ElGamalPrivateKeyParameters pv2 = new ElGamalPrivateKeyParameters(pv1.X, pv1.Parameters);
			ElGamalPublicKeyParameters pu3 = new ElGamalPublicKeyParameters(pv1.X, pu1.Parameters);
			ElGamalPrivateKeyParameters pv3 = new ElGamalPrivateKeyParameters(pu1.Y, pu1.Parameters);

			doTest(pu1, pu2, pu3);
			doTest(pv1, pv2, pv3);

			ElGamalParameters pr1 = pu1.Parameters;
			ElGamalParameters pr2 = new ElGamalParameters(pr1.P, pr1.G);
			ElGamalParameters pr3 = new ElGamalParameters(pr1.G, pr1.P);

			doTest(pr1, pr2, pr3);

			pu2 = new ElGamalPublicKeyParameters(pu1.Y, pr2);
			pv2 = new ElGamalPrivateKeyParameters(pv1.X, pr2);

			doTest(pu1, pu2, pu3);
			doTest(pv1, pv2, pv3);

			ElGamalTestKeyParameters k1 = new ElGamalTestKeyParameters(false, null);
			ElGamalTestKeyParameters k2 = new ElGamalTestKeyParameters(false, null);
			ElGamalTestKeyParameters k3 = new ElGamalTestKeyParameters(false, pu1.Parameters);

			doTest(k1, k2, k3);
		}

		[Test]
		public void TestDsa()
		{
			BigInteger a = BigInteger.ValueOf(1), b = BigInteger.ValueOf(2), c = BigInteger.ValueOf(3);

			DsaParameters dsaP1 = new DsaParameters(a, b, c);
			DsaParameters dsaP2 = new DsaParameters(a, b, c);
			DsaParameters dsaP3 = new DsaParameters(b, c, a);

			doTest(dsaP1, dsaP2, dsaP3);

			DsaValidationParameters vp1 = new DsaValidationParameters(new byte[20], 1024);
			DsaValidationParameters vp2 = new DsaValidationParameters(new byte[20], 1024);
			DsaValidationParameters vp3 = new DsaValidationParameters(new byte[24], 1024);

			doTest(vp1, vp1, vp3);
			doTest(vp1, vp2, vp3);

			byte[] bytes = new byte[20];
			bytes[0] = 1;

			vp3 = new DsaValidationParameters(bytes, 1024);

			doTest(vp1, vp2, vp3);

			vp3 = new DsaValidationParameters(new byte[20], 2048);

			doTest(vp1, vp2, vp3);
		}

		[Test]
		public void TestGost3410()
		{
			BigInteger a = BigInteger.ValueOf(1), b = BigInteger.ValueOf(2), c = BigInteger.ValueOf(3);

			Gost3410Parameters g1 = new Gost3410Parameters(a, b, c);
			Gost3410Parameters g2 = new Gost3410Parameters(a, b, c);
			Gost3410Parameters g3 = new Gost3410Parameters(a, c, c);

			doTest(g1, g2, g3);

			Gost3410ValidationParameters v1 = new Gost3410ValidationParameters(100, 1);
			Gost3410ValidationParameters v2 = new Gost3410ValidationParameters(100, 1);
			Gost3410ValidationParameters v3 = new Gost3410ValidationParameters(101, 1);

			doTest(v1, v2, v3);

			v3 = new Gost3410ValidationParameters(100, 2);

			doTest(v1, v2, v3);

			v1 = new Gost3410ValidationParameters(100L, 1L);
			v2 = new Gost3410ValidationParameters(100L, 1L);
			v3 = new Gost3410ValidationParameters(101L, 1L);

			doTest(v1, v2, v3);

			v3 = new Gost3410ValidationParameters(100L, 2L);

			doTest(v1, v2, v3);
		}

		public override void PerformTest()
		{
			TestDH();
			TestElGamal();
			TestGost3410();
			TestDsa();
		}

		public static void Main(
			string[] args)
		{
			RunTest(new EqualsHashCodeTest());
		}
	}
}
