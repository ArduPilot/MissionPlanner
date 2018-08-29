using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Anssi;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class NamedCurveTest
        : SimpleTest
    {
//		private static readonly Hashtable CurveNames = new Hashtable();
//		private static readonly Hashtable CurveAliases = new Hashtable();
//
//		static NamedCurveTest()
//		{
//			CurveNames.Add("prime192v1", "prime192v1"); // X9.62
//			CurveNames.Add("sect571r1", "sect571r1"); // sec
//			CurveNames.Add("secp224r1", "secp224r1");
//			CurveNames.Add("B-409", SecNamedCurves.GetName(NistNamedCurves.GetOid("B-409")));   // nist
//			CurveNames.Add("P-521", SecNamedCurves.GetName(NistNamedCurves.GetOid("P-521")));
//			CurveNames.Add("brainpoolp160r1", "brainpoolp160r1");         // TeleTrusT
//
//			CurveAliases.Add("secp192r1", "prime192v1");
//			CurveAliases.Add("secp256r1", "prime256v1");
//		}

        private static ECDomainParameters GetCurveParameters(
            string name)
        {
            ECDomainParameters ecdp = ECGost3410NamedCurves.GetByName(name);

            if (ecdp != null)
                return ecdp;

            X9ECParameters ecP = ECNamedCurveTable.GetByName(name);

            if (ecP == null)
                throw new Exception("unknown curve name: " + name);

            return new ECDomainParameters(ecP.Curve, ecP.G, ecP.N, ecP.H, ecP.GetSeed());
        }

        public void doTestCurve(
            string name)
        {
//			ECGenParameterSpec ecSpec = new ECGenParameterSpec(name);
            ECDomainParameters ecSpec = GetCurveParameters(name);

            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("ECDH");

//			g.initialize(ecSpec, new SecureRandom());
            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom())); 

            //
            // a side
            //
            AsymmetricCipherKeyPair aKeyPair = g.GenerateKeyPair();

//			KeyAgreement aKeyAgree = KeyAgreement.getInstance("ECDHC");
            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement("ECDHC");

            aKeyAgree.Init(aKeyPair.Private);

            //
            // b side
            //
            AsymmetricCipherKeyPair bKeyPair = g.GenerateKeyPair();

//			KeyAgreement bKeyAgree = KeyAgreement.getInstance("ECDHC");
            IBasicAgreement bKeyAgree = AgreementUtilities.GetBasicAgreement("ECDHC");

            bKeyAgree.Init(bKeyPair.Private);

            //
            // agreement
            //
//			aKeyAgree.doPhase(bKeyPair.Public, true);
//			bKeyAgree.doPhase(aKeyPair.Public, true);
//
//			BigInteger k1 = new BigInteger(aKeyAgree.generateSecret());
//			BigInteger k2 = new BigInteger(bKeyAgree.generateSecret());
            BigInteger k1 = aKeyAgree.CalculateAgreement(bKeyPair.Public);
            BigInteger k2 = bKeyAgree.CalculateAgreement(aKeyPair.Public);

            if (!k1.Equals(k2))
            {
                Fail("2-way test failed");
            }

            //
            // public key encoding test
            //
//			byte[]              pubEnc = aKeyPair.Public.getEncoded();
            byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(aKeyPair.Public).GetDerEncoded();

//			KeyFactory          keyFac = KeyFactory.getInstance("ECDH");
//			X509EncodedKeySpec  pubX509 = new X509EncodedKeySpec(pubEnc);
//			ECPublicKey         pubKey = (ECPublicKey)keyFac.generatePublic(pubX509);
            ECPublicKeyParameters pubKey = (ECPublicKeyParameters) PublicKeyFactory.CreateKey(pubEnc);

//			if (!pubKey.getW().Equals(((ECPublicKey)aKeyPair.Public).getW()))
            if (!pubKey.Q.Equals(((ECPublicKeyParameters)aKeyPair.Public).Q))
            {
                Fail("public key encoding (Q test) failed");
            }

            // TODO Put back in?
//			if (!(pubKey.getParams() is ECNamedCurveSpec))
//			{
//				Fail("public key encoding not named curve");
//			}

            //
            // private key encoding test
            //
//			byte[]              privEnc = aKeyPair.Private.getEncoded();
            byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(aKeyPair.Private).GetDerEncoded();

//			PKCS8EncodedKeySpec privPKCS8 = new PKCS8EncodedKeySpec(privEnc);
//			ECPrivateKey        privKey = (ECPrivateKey)keyFac.generatePrivate(privPKCS8);
            ECPrivateKeyParameters privKey = (ECPrivateKeyParameters) PrivateKeyFactory.CreateKey(privEnc);

//			if (!privKey.getS().Equals(((ECPrivateKey)aKeyPair.Private).getS()))
            if (!privKey.D.Equals(((ECPrivateKeyParameters)aKeyPair.Private).D))
            {
                Fail("private key encoding (S test) failed");
            }

            // TODO Put back in?
//			if (!(privKey.getParams() is ECNamedCurveSpec))
//			{
//				Fail("private key encoding not named curve");
//			}
//
//			ECNamedCurveSpec privSpec = (ECNamedCurveSpec)privKey.getParams();
//			if (!(privSpec.GetName().Equals(name) || privSpec.GetName().Equals(CurveNames.get(name))))
//			{
//				Fail("private key encoding wrong named curve. Expected: "
//					+ CurveNames[name] + " got " + privSpec.GetName());
//			}
        }

        public void doTestECDsa(
            string name)
        {
//			ECGenParameterSpec ecSpec = new ECGenParameterSpec(name);
            ECDomainParameters ecSpec = GetCurveParameters(name);

            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("ECDSA");

//			g.initialize(ecSpec, new SecureRandom());
            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom())); 

            ISigner sgr = SignerUtilities.GetSigner("ECDSA");
            AsymmetricCipherKeyPair pair = g.GenerateKeyPair();
            AsymmetricKeyParameter sKey = pair.Private;
            AsymmetricKeyParameter vKey = pair.Public;

            sgr.Init(true, sKey);

            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            sgr.BlockUpdate(message, 0, message.Length);

            byte[] sigBytes = sgr.GenerateSignature();

            sgr.Init(false, vKey);

            sgr.BlockUpdate(message, 0, message.Length);

            if (!sgr.VerifySignature(sigBytes))
            {
                Fail(name + " verification failed");
            }

            //
            // public key encoding test
            //
//			byte[]              pubEnc = vKey.getEncoded();
            byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(vKey).GetDerEncoded();

//			KeyFactory          keyFac = KeyFactory.getInstance("ECDH");
//			X509EncodedKeySpec  pubX509 = new X509EncodedKeySpec(pubEnc);
//			ECPublicKey         pubKey = (ECPublicKey)keyFac.generatePublic(pubX509);
            ECPublicKeyParameters pubKey = (ECPublicKeyParameters) PublicKeyFactory.CreateKey(pubEnc);

//			if (!pubKey.getW().Equals(((ECPublicKey)vKey).getW()))
            if (!pubKey.Q.Equals(((ECPublicKeyParameters)vKey).Q))
            {
                Fail("public key encoding (Q test) failed");
            }

            // TODO Put back in?
//			if (!(pubKey.Parameters is ECNamedCurveSpec))
//			{
//				Fail("public key encoding not named curve");
//			}

            //
            // private key encoding test
            //
//			byte[]              privEnc = sKey.getEncoded();
            byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(sKey).GetDerEncoded();

//			PKCS8EncodedKeySpec privPKCS8 = new PKCS8EncodedKeySpec(privEnc);
//			ECPrivateKey        privKey = (ECPrivateKey)keyFac.generatePrivate(privPKCS8);
            ECPrivateKeyParameters privKey = (ECPrivateKeyParameters) PrivateKeyFactory.CreateKey(privEnc);

//			if (!privKey.getS().Equals(((ECPrivateKey)sKey).getS()))
            if (!privKey.D.Equals(((ECPrivateKeyParameters)sKey).D))
            {
                Fail("private key encoding (D test) failed");
            }

            // TODO Put back in?
//			if (!(privKey.Parameters is ECNamedCurveSpec))
//			{
//				Fail("private key encoding not named curve");
//			}
//
//			ECNamedCurveSpec privSpec = (ECNamedCurveSpec)privKey.getParams();
//			if (!privSpec.GetName().EqualsIgnoreCase(name)
//				&& !privSpec.GetName().EqualsIgnoreCase((string) CurveAliases[name]))
//			{
//				Fail("private key encoding wrong named curve. Expected: " + name + " got " + privSpec.GetName());
//			}
        }

        public void doTestECGost(string name)
        {
            ISigner sgr;
            string keyAlgorithm;

            if (name.IndexOf("Tc26-Gost-3410") == 0)
            {
                // TODO Implement ECGOST3410-2012 in SignerUtilies/GeneratorUtilities etc.
                // Current test cases don't work for GOST34.10 2012
                return;

                keyAlgorithm = "ECGOST3410-2012";
                if (name.IndexOf("256") > 0)
                {
                    sgr = SignerUtilities.GetSigner("ECGOST3410-2012-256");
                }
                else
                {
                    sgr = SignerUtilities.GetSigner("ECGOST3410-2012-512");
                }
            }
            else
            {
                keyAlgorithm = "ECGOST3410";

                sgr = SignerUtilities.GetSigner("ECGOST3410");
            }

            ECDomainParameters ecSpec = GetCurveParameters(name);

            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator(keyAlgorithm);
            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom())); 

            AsymmetricCipherKeyPair pair = g.GenerateKeyPair();
            AsymmetricKeyParameter sKey = pair.Private;
            AsymmetricKeyParameter vKey = pair.Public;

            sgr.Init(true, sKey);

            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            sgr.BlockUpdate(message, 0, message.Length);

            byte[] sigBytes = sgr.GenerateSignature();

            sgr.Init(false, vKey);

            sgr.BlockUpdate(message, 0, message.Length);

            if (!sgr.VerifySignature(sigBytes))
            {
                Fail(name + " verification failed");
            }

            // TODO Get this working?
//			//
//			// public key encoding test
//			//
////			byte[]              pubEnc = vKey.getEncoded();
//			byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(vKey).GetDerEncoded();
//
////			KeyFactory          keyFac = KeyFactory.getInstance(keyAlgorithm);
////			X509EncodedKeySpec  pubX509 = new X509EncodedKeySpec(pubEnc);
////			ECPublicKey         pubKey = (ECPublicKey)keyFac.generatePublic(pubX509);
//			ECPublicKeyParameters pubKey = (ECPublicKeyParameters) PublicKeyFactory.CreateKey(pubEnc);
//
////			if (!pubKey.getW().equals(((ECPublicKey)vKey).getW()))
//			if (!pubKey.Q.Equals(((ECPublicKeyParameters)vKey).Q))
//			{
//				Fail("public key encoding (Q test) failed");
//			}

            // TODO Put back in?
//			if (!(pubKey.Parameters is ECNamedCurveSpec))
//			{
//				Fail("public key encoding not named curve");
//			}

            // TODO Get this working?
//			//
//			// private key encoding test
//			//
////			byte[]              privEnc = sKey.getEncoded();
//			byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(sKey).GetDerEncoded();
//
////			PKCS8EncodedKeySpec privPKCS8 = new PKCS8EncodedKeySpec(privEnc);
////			ECPrivateKey        privKey = (ECPrivateKey)keyFac.generatePrivate(privPKCS8);
//			ECPrivateKeyParameters privKey = (ECPrivateKeyParameters) PrivateKeyFactory.CreateKey(privEnc);
//
////			if (!privKey.getS().Equals(((ECPrivateKey)sKey).getS()))
//			if (!privKey.D.Equals(((ECPrivateKeyParameters)sKey).D))
//			{
//				Fail("GOST private key encoding (D test) failed");
//			}

            // TODO Put back in?
//			if (!(privKey.Parameters is ECNamedCurveSpec))
//			{
//				Fail("GOST private key encoding not named curve");
//			}
//
//			ECNamedCurveSpec privSpec = (ECNamedCurveSpec)privKey.getParams();
//			if (!privSpec.getName().equalsIgnoreCase(name)
//				&& !privSpec.getName().equalsIgnoreCase((String)CURVE_ALIASES[name]))
//			{
//				Fail("GOST private key encoding wrong named curve. Expected: " + name + " got " + privSpec.getName());
//			}
        }

        public override string Name
        {
            get { return "NamedCurve"; }
        }
        
        public override void PerformTest()
        {
            doTestCurve("prime192v1"); // X9.62
            doTestCurve("sect571r1"); // sec
            doTestCurve("secp224r1");
            doTestCurve("B-409");   // nist
            doTestCurve("P-521");
            doTestCurve("brainpoolp160r1");    // TeleTrusT

            foreach (string name in X962NamedCurves.Names)
            {
                doTestECDsa(name);
            }

            foreach (string name in SecNamedCurves.Names)
            {
                doTestECDsa(name);
            }

            foreach (string name in TeleTrusTNamedCurves.Names)
            {
                doTestECDsa(name);
            }

            foreach (string name in AnssiNamedCurves.Names)
            {
                doTestECDsa(name);
            }

            foreach (string name in GMNamedCurves.Names)
            {
                doTestECDsa(name);
            }

            foreach (string name in ECGost3410NamedCurves.Names)
            {
                doTestECGost(name);
            }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new NamedCurveTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
