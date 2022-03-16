using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Tests
{
	[TestFixture]
	public class MqvTest
		: SimpleTest
	{
		public override string Name
		{
			get { return "MQV"; }
		}

		public override void PerformTest()
		{
			TestECMqv();
		}

		[Test]
		public void TestECMqv()
		{
			IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("ECMQV");

            X9ECParameters x9 = ECNamedCurveTable.GetByName("prime239v1");
            ECDomainParameters ecSpec = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H);

            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));
			
			//
			// U side
			//
			AsymmetricCipherKeyPair U1 = g.GenerateKeyPair();
			AsymmetricCipherKeyPair U2 = g.GenerateKeyPair();
			
			IBasicAgreement uAgree = AgreementUtilities.GetBasicAgreement("ECMQV");
			uAgree.Init(new MqvPrivateParameters(
				(ECPrivateKeyParameters)U1.Private,
				(ECPrivateKeyParameters)U2.Private,
				(ECPublicKeyParameters)U2.Public));
			
			//
			// V side
			//
			AsymmetricCipherKeyPair V1 = g.GenerateKeyPair();
			AsymmetricCipherKeyPair V2 = g.GenerateKeyPair();

			IBasicAgreement vAgree = AgreementUtilities.GetBasicAgreement("ECMQV");
			vAgree.Init(new MqvPrivateParameters(
				(ECPrivateKeyParameters)V1.Private,
				(ECPrivateKeyParameters)V2.Private,
				(ECPublicKeyParameters)V2.Public));
			
			//
			// agreement
			//
			BigInteger ux = uAgree.CalculateAgreement(new MqvPublicParameters(
				(ECPublicKeyParameters)V1.Public,
				(ECPublicKeyParameters)V2.Public));
			BigInteger vx = vAgree.CalculateAgreement(new MqvPublicParameters(
				(ECPublicKeyParameters)U1.Public,
				(ECPublicKeyParameters)U2.Public));

			if (!ux.Equals(vx))
			{
				Fail("Agreement failed");
			}
		}

		public static void Main(
			string[] args)
		{
			RunTest(new MqvTest());
		}
	}
}
