using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class Gost3410Test
        : SimpleTest
    {
        private void ecGOST3410Test()
        {
            BigInteger r = new BigInteger("29700980915817952874371204983938256990422752107994319651632687982059210933395");
            BigInteger s = new BigInteger("46959264877825372965922731380059061821746083849389763294914877353246631700866");

            byte[] kData = new BigInteger("53854137677348463731403841147996619241504003434302020712960838528893196233395").ToByteArrayUnsigned();

            SecureRandom k = FixedSecureRandom.From(kData);

            BigInteger mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564821041");
            BigInteger mod_q = new BigInteger("57896044618658097711785492504343953927082934583725450622380973592137631069619");

            ECCurve curve = new FpCurve(
                mod_p,
                new BigInteger("7"), // a
                new BigInteger("43308876546767276905765904595650931995942111794451039583252968842033849580414"), // b
                mod_q, BigInteger.One);

            ECDomainParameters spec = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("2"),
                    new BigInteger("4018974056539037503335449422937059775635739389905545080690979365213431566280")),
                mod_q, BigInteger.One);

            ECPrivateKeyParameters sKey = new ECPrivateKeyParameters(
                "ECGOST3410",
                new BigInteger("55441196065363246126355624130324183196576709222340016572108097750006097525544"), // d
                spec);

            ECPublicKeyParameters vKey = new ECPublicKeyParameters(
                "ECGOST3410",
                curve.CreatePoint(
                    new BigInteger("57520216126176808443631405023338071176630104906313632182896741342206604859403"),
                    new BigInteger("17614944419213781543809391949654080031942662045363639260709847859438286763994")),
                spec);

            ISigner sgr = SignerUtilities.GetSigner("ECGOST3410");

            sgr.Init(true, new ParametersWithRandom(sKey, k));

            byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

            sgr.BlockUpdate(message, 0, message.Length);

            byte[] sigBytes = sgr.GenerateSignature();

            sgr.Init(false, vKey);

            sgr.BlockUpdate(message, 0, message.Length);

            if (!sgr.VerifySignature(sigBytes))
            {
                Fail("ECGOST3410 verification failed");
            }

            BigInteger[] sig = decode(sigBytes);

            if (!r.Equals(sig[0]))
            {
                Fail(
                    ": r component wrong." + SimpleTest.NewLine
                    + " expecting: " + r + SimpleTest.NewLine
                    + " got      : " + sig[0]);
            }

            if (!s.Equals(sig[1]))
            {
                Fail(
                    ": s component wrong." + SimpleTest.NewLine
                    + " expecting: " + s + SimpleTest.NewLine
                    + " got      : " + sig[1]);
            }
        }

        private void generationTest()
        {
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            ISigner s = SignerUtilities.GetSigner("GOST3410");

            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("GOST3410");
            g.Init(
                new Gost3410KeyGenerationParameters(
                    new SecureRandom(),
                    CryptoProObjectIdentifiers.GostR3410x94CryptoProA));

            AsymmetricCipherKeyPair p = g.GenerateKeyPair();

            AsymmetricKeyParameter sKey = p.Private;
            AsymmetricKeyParameter vKey = p.Public;

            s.Init(true, sKey);

            s.BlockUpdate(data, 0, data.Length);

            byte[] sigBytes = s.GenerateSignature();

            s = SignerUtilities.GetSigner("GOST3410");

            s.Init(false, vKey);

            s.BlockUpdate(data, 0, data.Length);

            if (!s.VerifySignature(sigBytes))
            {
                Fail("GOST3410 verification failed");
            }

            //
            // default initialisation test
            //
            s = SignerUtilities.GetSigner("GOST3410");
            g = GeneratorUtilities.GetKeyPairGenerator("GOST3410");

            // TODO This is supposed to be a 'default initialisation' test, but don't have a factory
            // These values are defaults from JCE provider
            g.Init(
                new Gost3410KeyGenerationParameters(
                    new SecureRandom(),
                    CryptoProObjectIdentifiers.GostR3410x94CryptoProA));

            p = g.GenerateKeyPair();

            sKey = p.Private;
            vKey = p.Public;

            s.Init(true, sKey);

            s.BlockUpdate(data, 0, data.Length);

            sigBytes = s.GenerateSignature();

            s = SignerUtilities.GetSigner("GOST3410");

            s.Init(false, vKey);

            s.BlockUpdate(data, 0, data.Length);

            if (!s.VerifySignature(sigBytes))
            {
                Fail("GOST3410 verification failed");
            }

            //
            // encoded test
            //
            //KeyFactory f = KeyFactory.getInstance("GOST3410");
            //X509EncodedKeySpec  x509s = new X509EncodedKeySpec(vKey.GetEncoded());
            //Gost3410PublicKeyParameters k1 = (Gost3410PublicKeyParameters)f.generatePublic(x509s);
            byte[] vKeyEnc = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(vKey).GetDerEncoded();
            Gost3410PublicKeyParameters k1 = (Gost3410PublicKeyParameters)
                PublicKeyFactory.CreateKey(vKeyEnc);

            if (!k1.Y.Equals(((Gost3410PublicKeyParameters)vKey).Y))
            {
                Fail("public number not decoded properly");
            }

            //PKCS8EncodedKeySpec  pkcs8 = new PKCS8EncodedKeySpec(sKey.GetEncoded());
            //Gost3410PrivateKeyParameters k2 = (Gost3410PrivateKeyParameters)f.generatePrivate(pkcs8);
            byte[] sKeyEnc = PrivateKeyInfoFactory.CreatePrivateKeyInfo(sKey).GetDerEncoded();
            Gost3410PrivateKeyParameters k2 = (Gost3410PrivateKeyParameters)
                PrivateKeyFactory.CreateKey(sKeyEnc);

            if (!k2.X.Equals(((Gost3410PrivateKeyParameters)sKey).X))
            {
                Fail("private number not decoded properly");
            }

            //
            // ECGOST3410 generation test
            //
            s = SignerUtilities.GetSigner("ECGOST3410");
            g = GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");

            BigInteger mod_p = new BigInteger("57896044618658097711785492504343953926634992332820282019728792003956564821041");
            BigInteger mod_q = new BigInteger("57896044618658097711785492504343953927082934583725450622380973592137631069619");

            ECCurve curve = new FpCurve(
                mod_p,
                new BigInteger("7"), // a
                new BigInteger("43308876546767276905765904595650931995942111794451039583252968842033849580414"), // b
                mod_q, BigInteger.One);

            ECDomainParameters ecSpec = new ECDomainParameters(
                curve,
                curve.CreatePoint(
                    new BigInteger("2"),
                    new BigInteger("4018974056539037503335449422937059775635739389905545080690979365213431566280")),
                mod_q, BigInteger.One);

            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

            p = g.GenerateKeyPair();

            sKey = p.Private;
            vKey = p.Public;

            s.Init(true, sKey);

            s.BlockUpdate(data, 0, data.Length);

            sigBytes = s.GenerateSignature();

            s = SignerUtilities.GetSigner("ECGOST3410");

            s.Init(false, vKey);

            s.BlockUpdate(data, 0, data.Length);

            if (!s.VerifySignature(sigBytes))
            {
                Fail("ECGOST3410 verification failed");
            }
        }

        private void keyStoreTest(
            AsymmetricKeyParameter	sKey,
            AsymmetricKeyParameter	vKey)
//	        throws KeyStoreException, IOException, NoSuchAlgorithmException, CertificateException, NoSuchProviderException, SignatureException, InvalidKeyException, UnrecoverableKeyException
        {
            //
            // keystore test
            //
//			KeyStore ks = KeyStore.GetInstance("JKS");
//			ks.Load(null, null);
            Pkcs12StoreBuilder ksBuilder = new Pkcs12StoreBuilder();
            Pkcs12Store ks = ksBuilder.Build();
    
            //
            // create the certificate - version 3
            //
            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
    
            certGen.SetSerialNumber(BigInteger.One);
            certGen.SetIssuerDN(new X509Name("CN=Test"));
            certGen.SetNotBefore(DateTime.UtcNow.AddSeconds(-50));
            certGen.SetNotAfter(DateTime.UtcNow.AddSeconds(50));
            certGen.SetSubjectDN(new X509Name("CN=Test"));
            certGen.SetPublicKey(vKey);
            certGen.SetSignatureAlgorithm("GOST3411withGOST3410");
    
            X509Certificate cert = certGen.Generate(sKey);
            X509CertificateEntry certEntry = new X509CertificateEntry(cert);

//	        ks.SetKeyEntry("gost", sKey, "gost".ToCharArray(), new X509Certificate[] { cert });
            ks.SetKeyEntry("gost", new AsymmetricKeyEntry(sKey), new X509CertificateEntry[] { certEntry });
    
            MemoryStream bOut = new MemoryStream();
    
            ks.Save(bOut, "gost".ToCharArray(), new SecureRandom());
    
//	        ks = KeyStore.getInstance("JKS");
            ks = ksBuilder.Build();

            ks.Load(new MemoryStream(bOut.ToArray(), false), "gost".ToCharArray());

//	        AsymmetricKeyParameter gKey = (AsymmetricKeyParameter)ks.GetKey("gost", "gost".ToCharArray());
//	        AsymmetricKeyEntry gKeyEntry = (AsymmetricKeyEntry)
                ks.GetKey("gost");
        }
        
        private void parametersTest()
        {
//	        AlgorithmParameterGenerator a = AlgorithmParameterGenerator.getInstance("GOST3410");
//	        a.init(512, random);
//	        AlgorithmParameters params = a.generateParameters();
//
//	        byte[] encodeParams = params.getEncoded();
//
//	        AlgorithmParameters a2 = AlgorithmParameters.getInstance("GOST3410");
//	        a2.init(encodeParams);
//
//	        // a and a2 should be equivalent!
//	        byte[] encodeParams_2 = a2.getEncoded();
//
//	        if (!arrayEquals(encodeParams, encodeParams_2))
//	        {
//	            Fail("encode/decode parameters failed");
//	        }

//			GOST3410ParameterSpec gost3410P = new GOST3410ParameterSpec(
//				CryptoProObjectIdentifiers.gostR3410_94_CryptoPro_B.getId());
//			g.initialize(gost3410P, new SecureRandom());
            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("GOST3410");
            g.Init(
                new Gost3410KeyGenerationParameters(
                    new SecureRandom(),
                    CryptoProObjectIdentifiers.GostR3410x94CryptoProB));

            AsymmetricCipherKeyPair p = g.GenerateKeyPair();

            AsymmetricKeyParameter sKey = p.Private;
            AsymmetricKeyParameter vKey = p.Public;

            ISigner s = SignerUtilities.GetSigner("GOST3410");
            byte[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };

            s.Init(true, sKey);

            s.BlockUpdate(data, 0, data.Length);

            byte[] sigBytes = s.GenerateSignature();

            s = SignerUtilities.GetSigner("GOST3410");

            s.Init(false, vKey);

            s.BlockUpdate(data, 0, data.Length);

            if (!s.VerifySignature(sigBytes))
            {
                Fail("GOST3410 verification failed");
            }

            keyStoreTest(sKey, vKey);
        }

        private BigInteger[] decode(
            byte[]  encoding)
        {
            byte[] r = new byte[32];
            byte[] s = new byte[32];

            Array.Copy(encoding, 0, s, 0, 32);
            Array.Copy(encoding, 32, r, 0, 32);

            BigInteger[] sig = new BigInteger[2];

            sig[0] = new BigInteger(1, r);
            sig[1] = new BigInteger(1, s);

            return sig;
        }

        public override string Name
        {
            get { return "GOST3410/ECGOST3410"; }
        }

        public override void PerformTest()
        {
            ecGOST3410Test();
            generationTest();
            parametersTest();
        }

        public static void Main(
            string[]	args)
        {
            RunTest(new Gost3410Test());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
