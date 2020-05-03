using System;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class DHTest
        : SimpleTest
    {
        private static readonly BigInteger g512 = new BigInteger("153d5d6172adb43045b68ae8e1de1070b6137005686d29d3d73a7749199681ee5b212c9b96bfdcfa5b20cd5e3fd2044895d609cf9b410b7a0f12ca1cb9a428cc", 16);
        private static readonly BigInteger p512 = new BigInteger("9494fec095f3b85ee286542b3836fc81a5dd0a0349b4c239dd38744d488cf8e31db8bcb7d33b41abb9e5a33cca9144b1cef332c94bf0573bf047a3aca98cdf3b", 16);

        private static readonly BigInteger g768 = new BigInteger("7c240073c1316c621df461b71ebb0cdcc90a6e5527e5e126633d131f87461c4dc4afc60c2cb0f053b6758871489a69613e2a8b4c8acde23954c08c81cbd36132cfd64d69e4ed9f8e51ed6e516297206672d5c0a69135df0a5dcf010d289a9ca1", 16);
        private static readonly BigInteger p768 = new BigInteger("8c9dd223debed1b80103b8b309715be009d48860ed5ae9b9d5d8159508efd802e3ad4501a7f7e1cfec78844489148cd72da24b21eddd01aa624291c48393e277cfc529e37075eccef957f3616f962d15b44aeab4039d01b817fde9eaa12fd73f", 16);

        private static readonly BigInteger g1024 = new BigInteger("1db17639cdf96bc4eabba19454f0b7e5bd4e14862889a725c96eb61048dcd676ceb303d586e30f060dbafd8a571a39c4d823982117da5cc4e0f89c77388b7a08896362429b94a18a327604eb7ff227bffbc83459ade299e57b5f77b50fb045250934938efa145511166e3197373e1b5b1e52de713eb49792bedde722c6717abf", 16);
        private static readonly BigInteger p1024 = new BigInteger("a00e283b3c624e5b2b4d9fbc2653b5185d99499b00fd1bf244c6f0bb817b4d1c451b2958d62a0f8a38caef059fb5ecd25d75ed9af403f5b5bdab97a642902f824e3c13789fed95fa106ddfe0ff4a707c85e2eb77d49e68f2808bcea18ce128b178cd287c6bc00efa9a1ad2a673fe0dceace53166f75b81d6709d5f8af7c66bb7", 16);

        // public key with mismatched oid/parameters
        private byte[] oldPubEnc = Base64.Decode(
            "MIIBnzCCARQGByqGSM4+AgEwggEHAoGBAPxSrN417g43VAM9sZRf1dt6AocAf7D6" +
            "WVCtqEDcBJrMzt63+g+BNJzhXVtbZ9kp9vw8L/0PHgzv0Ot/kOLX7Khn+JalOECW" +
            "YlkyBhmOVbjR79TY5u2GAlvG6pqpizieQNBCEMlUuYuK1Iwseil6VoRuA13Zm7uw" +
            "WO1eZmaJtY7LAoGAQaPRCFKM5rEdkMrV9FNzeSsYRs8m3DqPnnJHpuySpyO9wUcX" +
            "OOJcJY5qvHbDO5SxHXu/+bMgXmVT6dXI5o0UeYqJR7fj6pR4E6T0FwG55RFr5Ok4" +
            "3C4cpXmaOu176SyWuoDqGs1RDGmYQjwbZUi23DjaaTFUly9LCYXMliKrQfEDgYQA" +
            "AoGAQUGCBN4TaBw1BpdBXdTvTfCU69XDB3eyU2FOBE3UWhpx9D8XJlx4f5DpA4Y6" +
            "6sQMuCbhfmjEph8W7/sbMurM/awR+PSR8tTY7jeQV0OkmAYdGK2nzh0ZSifMO1oE" +
            "NNhN2O62TLs67msxT28S4/S89+LMtc98mevQ2SX+JF3wEVU=");

        // bogus key with full PKCS parameter set
        private byte[] oldFullParams = Base64.Decode(
            "MIIBIzCCARgGByqGSM4+AgEwggELAoGBAP1/U4EddRIpUt9KnC7s5Of2EbdSPO9E" +
            "AMMeP4C2USZpRV1AIlH7WT2NWPq/xfW6MPbLm1Vs14E7gB00b/JmYLdrmVClpJ+f" +
            "6AR7ECLCT7up1/63xhv4O1fnxqimFQ8E+4P208UewwI1VBNaFpEy9nXzrith1yrv" +
            "8iIDGZ3RSAHHAoGBAPfhoIXWmz3ey7yrXDa4V7l5lK+7+jrqgvlXTAs9B4JnUVlX" +
            "jrrUWU/mcQcQgYC0SRZxI+hMKBYTt88JMozIpuE8FnqLVHyNKOCjrh4rs6Z1kW6j" +
            "fwv6ITVi8ftiegEkO8yk8b6oUZCJqIPf4VrlnwaSi2ZegHtVJWQBTDv+z0kqAgFk" +
            "AwUAAgIH0A==");

        private byte[] samplePubEnc = Base64.Decode(
            "MIIBpjCCARsGCSqGSIb3DQEDATCCAQwCgYEA/X9TgR11EilS30qcLuzk5/YRt1I8" +
            "70QAwx4/gLZRJmlFXUAiUftZPY1Y+r/F9bow9subVWzXgTuAHTRv8mZgt2uZUKWk" +
            "n5/oBHsQIsJPu6nX/rfGG/g7V+fGqKYVDwT7g/bTxR7DAjVUE1oWkTL2dfOuK2HX" +
            "Ku/yIgMZndFIAccCgYEA9+GghdabPd7LvKtcNrhXuXmUr7v6OuqC+VdMCz0HgmdR" +
            "WVeOutRZT+ZxBxCBgLRJFnEj6EwoFhO3zwkyjMim4TwWeotUfI0o4KOuHiuzpnWR" +
            "bqN/C/ohNWLx+2J6ASQ7zKTxvqhRkImog9/hWuWfBpKLZl6Ae1UlZAFMO/7PSSoC" +
            "AgIAA4GEAAKBgEIiqxoUW6E6GChoOgcfNbVFclW91ITf5MFSUGQwt2R0RHoOhxvO" +
            "lZhNs++d0VPATLAyXovjfgENT9SGCbuZttYcqqLdKTbMXBWPek+rfnAl9E4iEMED" +
            "IDd83FJTKs9hQcPAm7zmp0Xm1bGF9CbUFjP5G02265z7eBmHDaT0SNlB");

        private byte[] samplePrivEnc = Base64.Decode(
            "MIIBZgIBADCCARsGCSqGSIb3DQEDATCCAQwCgYEA/X9TgR11EilS30qcLuzk5/YR" +
            "t1I870QAwx4/gLZRJmlFXUAiUftZPY1Y+r/F9bow9subVWzXgTuAHTRv8mZgt2uZ" +
            "UKWkn5/oBHsQIsJPu6nX/rfGG/g7V+fGqKYVDwT7g/bTxR7DAjVUE1oWkTL2dfOu" +
            "K2HXKu/yIgMZndFIAccCgYEA9+GghdabPd7LvKtcNrhXuXmUr7v6OuqC+VdMCz0H" +
            "gmdRWVeOutRZT+ZxBxCBgLRJFnEj6EwoFhO3zwkyjMim4TwWeotUfI0o4KOuHiuz" +
            "pnWRbqN/C/ohNWLx+2J6ASQ7zKTxvqhRkImog9/hWuWfBpKLZl6Ae1UlZAFMO/7P" +
            "SSoCAgIABEICQAZYXnBHazxXUUdFP4NIf2Ipu7du0suJPZQKKff81wymi2zfCfHh" +
            "uhe9gQ9xdm4GpzeNtrQ8/MzpTy+ZVrtd29Q=");

        public override string Name
        {
            get { return "DH"; }
        }

        private void doTestGP(
            string		algName,
            int         size,
            int         privateValueSize,
            BigInteger  g,
            BigInteger  p)
        {
            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator(algName);

            DHParameters dhParams = new DHParameters(p, g, null, privateValueSize);
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), dhParams);

            keyGen.Init(kgp);

            //
            // a side
            //
            AsymmetricCipherKeyPair aKeyPair = keyGen.GenerateKeyPair();

            IBasicAgreement aKeyAgreeBasic = AgreementUtilities.GetBasicAgreement(algName);

            checkKeySize(privateValueSize, aKeyPair);

            aKeyAgreeBasic.Init(aKeyPair.Private);

            //
            // b side
            //
            AsymmetricCipherKeyPair bKeyPair = keyGen.GenerateKeyPair();

            IBasicAgreement bKeyAgreeBasic = AgreementUtilities.GetBasicAgreement(algName);

            checkKeySize(privateValueSize, bKeyPair);

            bKeyAgreeBasic.Init(bKeyPair.Private);

            //
            // agreement
            //
//			aKeyAgreeBasic.doPhase(bKeyPair.Public, true);
//			bKeyAgreeBasic.doPhase(aKeyPair.Public, true);
//
//			BigInteger  k1 = new BigInteger(aKeyAgreeBasic.generateSecret());
//			BigInteger  k2 = new BigInteger(bKeyAgreeBasic.generateSecret());
            BigInteger k1 = aKeyAgreeBasic.CalculateAgreement(bKeyPair.Public);
            BigInteger k2 = bKeyAgreeBasic.CalculateAgreement(aKeyPair.Public);

            if (!k1.Equals(k2))
            {
                Fail(size + " bit 2-way test failed");
            }

            //
            // public key encoding test
            //
//			byte[]              pubEnc = aKeyPair.Public.GetEncoded();
            byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(aKeyPair.Public).GetDerEncoded();

//			KeyFactory          keyFac = KeyFactory.getInstance(algName);
//			X509EncodedKeySpec  pubX509 = new X509EncodedKeySpec(pubEnc);
//			DHPublicKey         pubKey = (DHPublicKey)keyFac.generatePublic(pubX509);
            DHPublicKeyParameters pubKey = (DHPublicKeyParameters) PublicKeyFactory.CreateKey(pubEnc);
//			DHParameterSpec     spec = pubKey.Parameters;
            DHParameters spec = pubKey.Parameters;

            if (!spec.G.Equals(dhParams.G) || !spec.P.Equals(dhParams.P))
            {
                Fail(size + " bit public key encoding/decoding test failed on parameters");
            }

            if (!((DHPublicKeyParameters)aKeyPair.Public).Y.Equals(pubKey.Y))
            {
                Fail(size + " bit public key encoding/decoding test failed on y value");
            }

            //
            // public key serialisation test
            //
            // TODO Put back in
//			MemoryStream bOut = new MemoryStream();
//			ObjectOutputStream oOut = new ObjectOutputStream(bOut);
//
//			oOut.WriteObject(aKeyPair.Public);
//
//			MemoryStream bIn = new MemoryStream(bOut.ToArray(), false);
//			ObjectInputStream oIn = new ObjectInputStream(bIn);
//
//			pubKey = (DHPublicKeyParameters)oIn.ReadObject();
            spec = pubKey.Parameters;

            if (!spec.G.Equals(dhParams.G) || !spec.P.Equals(dhParams.P))
            {
                Fail(size + " bit public key serialisation test failed on parameters");
            }

            if (!((DHPublicKeyParameters)aKeyPair.Public).Y.Equals(pubKey.Y))
            {
                Fail(size + " bit public key serialisation test failed on y value");
            }

            //
            // private key encoding test
            //
//			byte[] privEnc = aKeyPair.Private.GetEncoded();
            byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(aKeyPair.Private).GetDerEncoded();
//			PKCS8EncodedKeySpec privPKCS8 = new PKCS8EncodedKeySpec(privEnc);
//			DHPrivateKeyParameters privKey = (DHPrivateKey)keyFac.generatePrivate(privPKCS8);
            DHPrivateKeyParameters privKey = (DHPrivateKeyParameters) PrivateKeyFactory.CreateKey(privEnc);

            spec = privKey.Parameters;

            if (!spec.G.Equals(dhParams.G) || !spec.P.Equals(dhParams.P))
            {
                Fail(size + " bit private key encoding/decoding test failed on parameters");
            }

            if (!((DHPrivateKeyParameters)aKeyPair.Private).X.Equals(privKey.X))
            {
                Fail(size + " bit private key encoding/decoding test failed on y value");
            }

            //
            // private key serialisation test
            //
            // TODO Put back in
//			bOut = new MemoryStream();
//			oOut = new ObjectOutputStream(bOut);
//
//			oOut.WriteObject(aKeyPair.Private);
//
//			bIn = new MemoryStream(bOut.ToArray(), false);
//			oIn = new ObjectInputStream(bIn);
//
//			privKey = (DHPrivateKeyParameters)oIn.ReadObject();
            spec = privKey.Parameters;

            if (!spec.G.Equals(dhParams.G) || !spec.P.Equals(dhParams.P))
            {
                Fail(size + " bit private key serialisation test failed on parameters");
            }

            if (!((DHPrivateKeyParameters)aKeyPair.Private).X.Equals(privKey.X))
            {
                Fail(size + " bit private key serialisation test failed on y value");
            }

            //
            // three party test
            //
            IAsymmetricCipherKeyPairGenerator aPairGen = GeneratorUtilities.GetKeyPairGenerator(algName);
            aPairGen.Init(new DHKeyGenerationParameters(new SecureRandom(), spec));
            AsymmetricCipherKeyPair aPair = aPairGen.GenerateKeyPair();

            IAsymmetricCipherKeyPairGenerator bPairGen = GeneratorUtilities.GetKeyPairGenerator(algName);
            bPairGen.Init(new DHKeyGenerationParameters(new SecureRandom(), spec));
            AsymmetricCipherKeyPair bPair = bPairGen.GenerateKeyPair();

            IAsymmetricCipherKeyPairGenerator cPairGen = GeneratorUtilities.GetKeyPairGenerator(algName);
            cPairGen.Init(new DHKeyGenerationParameters(new SecureRandom(), spec));
            AsymmetricCipherKeyPair cPair = cPairGen.GenerateKeyPair();


            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement(algName);
            aKeyAgree.Init(aPair.Private);

            IBasicAgreement bKeyAgree = AgreementUtilities.GetBasicAgreement(algName);
            bKeyAgree.Init(bPair.Private);

            IBasicAgreement cKeyAgree = AgreementUtilities.GetBasicAgreement(algName);
            cKeyAgree.Init(cPair.Private);

//			Key ac = aKeyAgree.doPhase(cPair.Public, false);
//			Key ba = bKeyAgree.doPhase(aPair.Public, false);
//			Key cb = cKeyAgree.doPhase(bPair.Public, false);
//
//			aKeyAgree.doPhase(cb, true);
//			bKeyAgree.doPhase(ac, true);
//			cKeyAgree.doPhase(ba, true);
//
//			BigInteger aShared = new BigInteger(aKeyAgree.generateSecret());
//			BigInteger bShared = new BigInteger(bKeyAgree.generateSecret());
//			BigInteger cShared = new BigInteger(cKeyAgree.generateSecret());

            DHPublicKeyParameters ac = new DHPublicKeyParameters(aKeyAgree.CalculateAgreement(cPair.Public), spec);
            DHPublicKeyParameters ba = new DHPublicKeyParameters(bKeyAgree.CalculateAgreement(aPair.Public), spec);
            DHPublicKeyParameters cb = new DHPublicKeyParameters(cKeyAgree.CalculateAgreement(bPair.Public), spec);

            BigInteger aShared = aKeyAgree.CalculateAgreement(cb);
            BigInteger bShared = bKeyAgree.CalculateAgreement(ac);
            BigInteger cShared = cKeyAgree.CalculateAgreement(ba);

            if (!aShared.Equals(bShared))
            {
                Fail(size + " bit 3-way test failed (a and b differ)");
            }

            if (!cShared.Equals(bShared))
            {
                Fail(size + " bit 3-way test failed (c and b differ)");
            }
        }

        private void doTestExplicitWrapping(
            int			size,
            int			privateValueSize,
            BigInteger	g,
            BigInteger	p)
        {
            DHParameters dhParams = new DHParameters(p, g, null, privateValueSize);

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");

            keyGen.Init(new DHKeyGenerationParameters(new SecureRandom(), dhParams));

            //
            // a side
            //
            AsymmetricCipherKeyPair aKeyPair = keyGen.GenerateKeyPair();

            IBasicAgreement aKeyAgree = AgreementUtilities.GetBasicAgreement("DH");

            checkKeySize(privateValueSize, aKeyPair);

            aKeyAgree.Init(aKeyPair.Private);

            //
            // b side
            //
            AsymmetricCipherKeyPair bKeyPair = keyGen.GenerateKeyPair();

            IBasicAgreement bKeyAgree = AgreementUtilities.GetBasicAgreement("DH");

            checkKeySize(privateValueSize, bKeyPair);

            bKeyAgree.Init(bKeyPair.Private);

            //
            // agreement
            //
//			aKeyAgree.doPhase(bKeyPair.Public, true);
//			bKeyAgree.doPhase(aKeyPair.Public, true);
//
//			SecretKey k1 = aKeyAgree.generateSecret(PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id);
//			SecretKey k2 = bKeyAgree.generateSecret(PkcsObjectIdentifiers.IdAlgCms3DesWrap.Id);

            // TODO Does this really test the same thing as the above code?
            BigInteger b1 = aKeyAgree.CalculateAgreement(bKeyPair.Public);
            BigInteger b2 = bKeyAgree.CalculateAgreement(aKeyPair.Public);

            if (!b1.Equals(b2))
            {
                Fail("Explicit wrapping test failed");
            }
        }

        private void checkKeySize(
            int						privateValueSize,
            AsymmetricCipherKeyPair	aKeyPair)
        {
            if (privateValueSize != 0)
            {
                DHPrivateKeyParameters key = (DHPrivateKeyParameters)aKeyPair.Private;

                if (key.X.BitLength != privateValueSize)
                {
                    Fail("limited key check failed for key size " + privateValueSize);
                }
            }
        }

// TODO Put back in
//    private void doTestRandom(
//        int         size)
//    {
//        AlgorithmParameterGenerator a = AlgorithmParameterGenerator.getInstance("DH");
//        a.init(size, new SecureRandom());
//        AlgorithmParameters parameters = a.generateParameters();
//
//        byte[] encodeParams = parameters.GetEncoded();
//
//        AlgorithmParameters a2 = AlgorithmParameters.getInstance("DH");
//        a2.init(encodeParams);
//
//        // a and a2 should be equivalent!
//        byte[] encodeParams_2 = a2.GetEncoded();
//
//        if (!areEqual(encodeParams, encodeParams_2))
//        {
//            Fail("encode/Decode parameters failed");
//        }
//
//        DHParameterSpec dhP = (DHParameterSpec)parameters.getParameterSpec(DHParameterSpec.class);
//
//        doTestGP("DH", size, 0, dhP.G, dhP.P);
//    }

        [Test]
        public void TestECDH()
        {
            DoTestECDH("ECDH");
        }

        [Test]
        public void TestECDHC()
        {
            DoTestECDH("ECDHC");
        }

        private void DoTestECDH(string algorithm)
        {
            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator(algorithm);

            X9ECParameters x9 = ECNamedCurveTable.GetByName("prime239v1");
            ECDomainParameters ecSpec = new ECDomainParameters(x9.Curve, x9.G, x9.N, x9.H);

            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

            //
            // a side
            //
            AsymmetricCipherKeyPair aKeyPair = g.GenerateKeyPair();

            IBasicAgreement aKeyAgreeBasic = AgreementUtilities.GetBasicAgreement(algorithm);

            aKeyAgreeBasic.Init(aKeyPair.Private);

            //
            // b side
            //
            AsymmetricCipherKeyPair bKeyPair = g.GenerateKeyPair();

            IBasicAgreement bKeyAgreeBasic = AgreementUtilities.GetBasicAgreement(algorithm);

            bKeyAgreeBasic.Init(bKeyPair.Private);

            //
            // agreement
            //
            BigInteger k1 = aKeyAgreeBasic.CalculateAgreement(bKeyPair.Public);
            BigInteger k2 = bKeyAgreeBasic.CalculateAgreement(aKeyPair.Public);

            if (!k1.Equals(k2))
            {
                Fail(algorithm + " 2-way test failed");
            }

            //
            // public key encoding test
            //
//			byte[] pubEnc = aKeyPair.Public.GetEncoded();
            byte[] pubEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(aKeyPair.Public).GetDerEncoded();

//			KeyFactory keyFac = KeyFactory.getInstance(algorithm);
//			X509EncodedKeySpec pubX509 = new X509EncodedKeySpec(pubEnc);
//			ECPublicKey pubKey = (ECPublicKey)keyFac.generatePublic(pubX509);
            ECPublicKeyParameters pubKey = (ECPublicKeyParameters) PublicKeyFactory.CreateKey(pubEnc);

            ECDomainParameters ecDP = pubKey.Parameters;

//			if (!pubKey.getW().Equals(((ECPublicKeyParameters)aKeyPair.Public).getW()))
            ECPoint pq1 = pubKey.Q.Normalize(), pq2 = ((ECPublicKeyParameters)aKeyPair.Public).Q.Normalize();
            if (!pq1.Equals(pq2))
            {
//				Console.WriteLine(" expected " + pubKey.getW().getAffineX() + " got " + ((ECPublicKey)aKeyPair.Public).getW().getAffineX());
//				Console.WriteLine(" expected " + pubKey.getW().getAffineY() + " got " + ((ECPublicKey)aKeyPair.Public).getW().getAffineY());
//				Fail(algorithm + " public key encoding (W test) failed");
                Console.WriteLine(" expected " + pq1.AffineXCoord.ToBigInteger()
                    + " got " + pq2.AffineXCoord.ToBigInteger());
                Console.WriteLine(" expected " + pq1.AffineYCoord.ToBigInteger()
                    + " got " + pq2.AffineYCoord.ToBigInteger());
                Fail(algorithm + " public key encoding (Q test) failed");
            }

//			if (!pubKey.Parameters.getGenerator().Equals(((ECPublicKeyParameters)aKeyPair.Public).Parameters.getGenerator()))
            if (!pubKey.Parameters.G.Equals(((ECPublicKeyParameters)aKeyPair.Public).Parameters.G))
            {
                Fail(algorithm + " public key encoding (G test) failed");
            }

            //
            // private key encoding test
            //
//			byte[] privEnc = aKeyPair.Private.GetEncoded();
            byte[] privEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(aKeyPair.Private).GetDerEncoded();

//			PKCS8EncodedKeySpec privPKCS8 = new PKCS8EncodedKeySpec(privEnc);
//			ECPrivateKey        privKey = (ECPrivateKey)keyFac.generatePrivate(privPKCS8);
            ECPrivateKeyParameters privKey = (ECPrivateKeyParameters) PrivateKeyFactory.CreateKey(privEnc);

//			if (!privKey.getS().Equals(((ECPrivateKey)aKeyPair.Private).getS()))
            if (!privKey.D.Equals(((ECPrivateKeyParameters)aKeyPair.Private).D))
            {
//				Fail(algorithm + " private key encoding (S test) failed");
                Fail(algorithm + " private key encoding (D test) failed");
            }

//			if (!privKey.Parameters.getGenerator().Equals(((ECPrivateKey)aKeyPair.Private).Parameters.getGenerator()))
            if (!privKey.Parameters.G.Equals(((ECPrivateKeyParameters)aKeyPair.Private).Parameters.G))
            {
                Fail(algorithm + " private key encoding (G test) failed");
            }
        }

        [Test]
        public void TestExceptions()
        {
            DHParameters dhParams = new DHParameters(p512, g512);

            try
            {
                IBasicAgreement aKeyAgreeBasic = AgreementUtilities.GetBasicAgreement("DH");

//				aKeyAgreeBasic.generateSecret("DES");
                aKeyAgreeBasic.CalculateAgreement(null);
            }
            catch (InvalidOperationException)
            {
                // okay
            }
            catch (Exception e)
            {
                Fail("Unexpected exception: " + e, e);
            }
        }

        private void doTestDesAndDesEde(
            BigInteger	g,
            BigInteger	p)
        {
            DHParameters dhParams = new DHParameters(p, g, null, 256);

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");

            keyGen.Init(new DHKeyGenerationParameters(new SecureRandom(), dhParams));

            AsymmetricCipherKeyPair kp = keyGen.GenerateKeyPair();

            IBasicAgreement keyAgreement = AgreementUtilities.GetBasicAgreement("DH");

            keyAgreement.Init(kp.Private);
            BigInteger agreed = keyAgreement.CalculateAgreement(kp.Public);
            byte[] agreedBytes = agreed.ToByteArrayUnsigned();

            // TODO Figure out where the magic happens of choosing the right
            // bytes from 'agreedBytes' for each key type - need C# equivalent?
            // (see JCEDHKeyAgreement.engineGenerateSecret)

//			SecretKey key = keyAgreement.generateSecret("DES");
//
//			if (key.getEncoded().length != 8)
//			{
//				Fail("DES length wrong");
//			}
//
//			if (!DESKeySpec.isParityAdjusted(key.getEncoded(), 0))
//			{
//				Fail("DES parity wrong");
//			}
//
//			key = keyAgreement.generateSecret("DESEDE");
//
//			if (key.getEncoded().length != 24)
//			{
//				Fail("DESEDE length wrong");
//			}
//
//			if (!DESedeKeySpec.isParityAdjusted(key.getEncoded(), 0))
//			{
//				Fail("DESEDE parity wrong");
//			}
//
//			key = keyAgreement.generateSecret("Blowfish");
//
//			if (key.getEncoded().length != 16)
//			{
//				Fail("Blowfish length wrong");
//			}
        }

        [Test]
        public void TestFunction()
        {
            doTestGP("DH", 512, 0, g512, p512);
            doTestGP("DiffieHellman", 768, 0, g768, p768);
            doTestGP("DIFFIEHELLMAN", 1024, 0, g1024, p1024);
            doTestGP("DH", 512, 64, g512, p512);
            doTestGP("DiffieHellman", 768, 128, g768, p768);
            doTestGP("DIFFIEHELLMAN", 1024, 256, g1024, p1024);
            doTestExplicitWrapping(512, 0, g512, p512);
            doTestDesAndDesEde(g768, p768);

            // TODO Put back in
            //doTestRandom(256);
        }

        [Test]
        public void TestEnc()
        {
//			KeyFactory kFact = KeyFactory.getInstance("DH", "BC");
//			
//			Key k = kFact.generatePrivate(new PKCS8EncodedKeySpec(samplePrivEnc));
            AsymmetricKeyParameter k = PrivateKeyFactory.CreateKey(samplePrivEnc);
            byte[] encoded = PrivateKeyInfoFactory.CreatePrivateKeyInfo(k).GetEncoded();

            if (!Arrays.AreEqual(samplePrivEnc, encoded))
            {
                Fail("private key re-encode failed");
            }

//			k = kFact.generatePublic(new X509EncodedKeySpec(samplePubEnc));
            k = PublicKeyFactory.CreateKey(samplePubEnc);
            encoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(k).GetEncoded();

            if (!Arrays.AreEqual(samplePubEnc, encoded))
            {
                Fail("public key re-encode failed");
            }

//			k = kFact.generatePublic(new X509EncodedKeySpec(oldPubEnc));
            k = PublicKeyFactory.CreateKey(oldPubEnc);
            encoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(k).GetEncoded();

            if (!Arrays.AreEqual(oldPubEnc, encoded))
            {
                Fail("old public key re-encode failed");
            }

//			k = kFact.generatePublic(new X509EncodedKeySpec(oldFullParams));
            k = PublicKeyFactory.CreateKey(oldFullParams);
            encoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(k).GetEncoded();

            if (!Arrays.AreEqual(oldFullParams, encoded))
            {
                Fail("old full public key re-encode failed");
            }
        }

        internal static readonly string Message = "Hello";

        internal static readonly SecureRandom rand = new SecureRandom();

        public static DHParameters Ike2048()
        {
            BigInteger p = new BigInteger(
                "ffffffffffffffffc90fdaa22168c234c4c6628b80dc1cd129024e088a67cc74"
                    + "020bbea63b139b22514a08798e3404ddef9519b3cd3a431b302b0a6df25f1437"
                    + "4fe1356d6d51c245e485b576625e7ec6f44c42e9a637ed6b0bff5cb6f406b7ed"
                    + "ee386bfb5a899fa5ae9f24117c4b1fe649286651ece45b3dc2007cb8a163bf05"
                    + "98da48361c55d39a69163fa8fd24cf5f83655d23dca3ad961c62f356208552bb"
                    + "9ed529077096966d670c354e4abc9804f1746c08ca18217c32905e462e36ce3b"
                    + "e39e772c180e86039b2783a2ec07a28fb5c55df06f4c52c9de2bcbf695581718"
                    + "3995497cea956ae515d2261898fa051015728e5a8aacaa68ffffffffffffffff", 16);
            BigInteger g = new BigInteger("2");
            return new DHParameters(p, g);
        }

        internal class DHWeakPubKey
            : DHPublicKeyParameters
        {
            private readonly BigInteger weakY;

            public DHWeakPubKey(BigInteger weakY, DHParameters parameters)
			    : base(BigInteger.Two, parameters)
            {
                this.weakY = weakY;
            }

            public override BigInteger Y
            {
                get { return weakY; }
            }
        }

        /**
         * Tests whether a provider accepts invalid public keys that result in predictable shared secrets.
         * This test is based on RFC 2785, Section 4 and NIST SP 800-56A,
         * If an attacker can modify both public keys in an ephemeral-ephemeral key agreement scheme then
         * it may be possible to coerce both parties into computing the same predictable shared key.
         * <p/>
         * Note: the test is quite whimsical. If the prime p is not a safe prime then the provider itself
         * cannot prevent all small-subgroup attacks because of the missing parameter q in the
         * Diffie-Hellman parameters. Implementations must add additional countermeasures such as the ones
         * proposed in RFC 2785.
         */
        [Test]
        public void TestSubgroupConfinement()
        {
            DHParameters parameters = Ike2048();
            BigInteger p = parameters.P, g = parameters.G;

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");

            //keyGen.initialize(params);
            keyGen.Init(new DHKeyGenerationParameters(new SecureRandom(), parameters));

            AsymmetricCipherKeyPair kp = keyGen.GenerateKeyPair();
            AsymmetricKeyParameter priv = kp.Private;

            IBasicAgreement ka = AgreementUtilities.GetBasicAgreement("DH");

            BigInteger[] weakPublicKeys = { BigInteger.Zero, BigInteger.One, p.Subtract(BigInteger.One), p,
                p.Add(BigInteger.One), BigInteger.One.Negate() };

            foreach (BigInteger weakKey in weakPublicKeys)
            {
                try
                {
                    new DHPublicKeyParameters(weakKey, parameters);
                    Fail("Generated weak public key");
                }
                catch (ArgumentException ex)
                {
                    IsTrue("wrong message (constructor)", ex.Message.StartsWith("invalid DH public key"));
                }

                ka.Init(priv);

                try
                {
                    ka.CalculateAgreement(new DHWeakPubKey(weakKey, parameters));
                    Fail("Generated secrets with weak public key");
                }
                catch (ArgumentException ex)
                {
                    IsTrue("wrong message (CalculateAgreement)", "Diffie-Hellman public key is weak".Equals(ex.Message));
                }
            }
        }

        public override void PerformTest()
        {
            TestEnc();
            TestFunction();
            TestECDH();
            TestECDHC();
            TestExceptions();
            TestSubgroupConfinement();
        }

        public static void Main(
            string[] args)
        {
            RunTest(new DHTest());
        }
    }
}
