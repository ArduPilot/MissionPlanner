using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;
using Org.BouncyCastle.X509.Extension;

namespace Org.BouncyCastle.Tests
{
    [TestFixture]
    public class Pkcs10CertRequestTest
        : SimpleTest
    {
        private static readonly byte[] gost3410EC_A = Base64.Decode(
              "MIIBOzCB6wIBADB/MQ0wCwYDVQQDEwR0ZXN0MRUwEwYDVQQKEwxEZW1vcyBDbyBMdGQxHjAcBgNV"
            +"BAsTFUNyeXB0b2dyYXBoeSBkaXZpc2lvbjEPMA0GA1UEBxMGTW9zY293MQswCQYDVQQGEwJydTEZ"
            +"MBcGCSqGSIb3DQEJARYKc2RiQGRvbC5ydTBjMBwGBiqFAwICEzASBgcqhQMCAiMBBgcqhQMCAh4B"
            +"A0MABEBYx0P2D7YuuZo5HgdIAUKAXcLBDZ+4LYFgbKjrfStVfH59lc40BQ2FZ7M703hLpXK8GiBQ"
            +"GEYpKaAuQZnMIpByoAAwCAYGKoUDAgIDA0EAgXMcTrhdOY2Er2tHOSAgnMezqrYxocZTWhxmW5Rl"
            +"JY6lbXH5rndCn4swFzXU+YhgAsJv1wQBaoZEWRl5WV4/nA==");

        private static readonly byte[] gost3410EC_B = Base64.Decode(
              "MIIBPTCB7QIBADCBgDENMAsGA1UEAxMEdGVzdDEWMBQGA1UEChMNRGVtb3MgQ28gTHRkLjEeMBwG"
            +"A1UECxMVQ3J5cHRvZ3JhcGh5IGRpdmlzaW9uMQ8wDQYDVQQHEwZNb3Njb3cxCzAJBgNVBAYTAnJ1"
            +"MRkwFwYJKoZIhvcNAQkBFgpzZGJAZG9sLnJ1MGMwHAYGKoUDAgITMBIGByqFAwICIwIGByqFAwIC"
            +"HgEDQwAEQI5SLoWT7dZVilbV9j5B/fyIDuDs6x4pjqNC2TtFYbpRHrk/Wc5g/mcHvD80tsm5o1C7"
            +"7cizNzkvAVUM4VT4Dz6gADAIBgYqhQMCAgMDQQAoT5TwJ8o+bSrxckymyo3diwG7ZbSytX4sRiKy"
            +"wXPWRS9LlBvPO2NqwpS2HUnxSU8rzfL9fJcybATf7Yt1OEVq");

        private static readonly byte[] gost3410EC_C = Base64.Decode(
              "MIIBRDCB9AIBADCBhzEVMBMGA1UEAxMMdGVzdCByZXF1ZXN0MRUwEwYDVQQKEwxEZW1vcyBDbyBM"
            +"dGQxHjAcBgNVBAsTFUNyeXB0b2dyYXBoeSBkaXZpc2lvbjEPMA0GA1UEBxMGTW9zY293MQswCQYD"
            +"VQQGEwJydTEZMBcGCSqGSIb3DQEJARYKc2RiQGRvbC5ydTBjMBwGBiqFAwICEzASBgcqhQMCAiMD"
            +"BgcqhQMCAh4BA0MABEBcmGh7OmR4iqqj+ycYo1S1fS7r5PhisSQU2Ezuz8wmmmR2zeTZkdMYCOBa"
            +"UTMNms0msW3wuYDho7nTDNscHTB5oAAwCAYGKoUDAgIDA0EAVoOMbfyo1Un4Ss7WQrUjHJoiaYW8"
            +"Ime5LeGGU2iW3ieAv6es/FdMrwTKkqn5dhd3aL/itFg5oQbhyfXw5yw/QQ==");

        private static readonly byte[] gost3410EC_ExA = Base64.Decode(
              "MIIBOzCB6wIBADB/MQ0wCwYDVQQDEwR0ZXN0MRUwEwYDVQQKEwxEZW1vcyBDbyBMdGQxHjAcBgNV"
            + "BAsTFUNyeXB0b2dyYXBoeSBkaXZpc2lvbjEPMA0GA1UEBxMGTW9zY293MQswCQYDVQQGEwJydTEZ"
            + "MBcGCSqGSIb3DQEJARYKc2RiQGRvbC5ydTBjMBwGBiqFAwICEzASBgcqhQMCAiQABgcqhQMCAh4B"
            + "A0MABEDkqNT/3f8NHj6EUiWnK4JbVZBh31bEpkwq9z3jf0u8ZndG56Vt+K1ZB6EpFxLT7hSIos0w"
            + "weZ2YuTZ4w43OgodoAAwCAYGKoUDAgIDA0EASk/IUXWxoi6NtcUGVF23VRV1L3undB4sRZLp4Vho"
            + "gQ7m3CMbZFfJ2cPu6QyarseXGYHmazoirH5lGjEo535c1g==");

        private static readonly byte[] gost3410EC_ExB = Base64.Decode(
              "MIIBPTCB7QIBADCBgDENMAsGA1UEAxMEdGVzdDEWMBQGA1UEChMNRGVtb3MgQ28gTHRkLjEeMBwG"
            + "A1UECxMVQ3J5cHRvZ3JhcGh5IGRpdmlzaW9uMQ8wDQYDVQQHEwZNb3Njb3cxCzAJBgNVBAYTAnJ1"
            + "MRkwFwYJKoZIhvcNAQkBFgpzZGJAZG9sLnJ1MGMwHAYGKoUDAgITMBIGByqFAwICJAEGByqFAwIC"
            + "HgEDQwAEQMBWYUKPy/1Kxad9ChAmgoSWSYOQxRnXo7KEGLU5RNSXA4qMUvArWzvhav+EYUfTbWLh"
            + "09nELDyHt2XQcvgQHnSgADAIBgYqhQMCAgMDQQAdaNhgH/ElHp64mbMaEo1tPCg9Q22McxpH8rCz"
            + "E0QBpF4H5mSSQVGI5OAXHToetnNuh7gHHSynyCupYDEHTbkZ");

        public override string Name
        {
            get { return "PKCS10CertRequest"; }
        }

        private void generationTest(
            int		keySize,
            string	keyName,
            string	sigName)
        {
            IAsymmetricCipherKeyPairGenerator kpg = GeneratorUtilities.GetKeyPairGenerator(keyName);

//			kpg.initialize(keySize);
            kpg.Init(new KeyGenerationParameters(new SecureRandom(), keySize));

            AsymmetricCipherKeyPair kp = kpg.GenerateKeyPair();

            IDictionary attrs = new Hashtable();
            attrs.Add(X509Name.C, "AU");
            attrs.Add(X509Name.O, "The Legion of the Bouncy Castle");
            attrs.Add(X509Name.L, "Melbourne");
            attrs.Add(X509Name.ST, "Victoria");
            attrs.Add(X509Name.EmailAddress, "feedback-crypto@bouncycastle.org");

            IList order = new ArrayList();
            order.Add(X509Name.C);
            order.Add(X509Name.O);
            order.Add(X509Name.L);
            order.Add(X509Name.ST);
            order.Add(X509Name.EmailAddress);

            X509Name subject = new X509Name(order, attrs);

            Pkcs10CertificationRequest req1 = new Pkcs10CertificationRequest(
                sigName,
                subject,
                kp.Public,
                null,
                kp.Private);

            byte[] bytes = req1.GetEncoded();

            Pkcs10CertificationRequest req2 = new Pkcs10CertificationRequest(bytes);

            if (!req2.Verify())
            {
                Fail(sigName + ": Failed Verify check.");
            }

            if (!req2.GetPublicKey().Equals(req1.GetPublicKey()))
            {
                Fail(keyName + ": Failed public key check.");
            }
        }

        /*
         * we generate a self signed certificate for the sake of testing - SHA224withECDSA
         */
        private void createECRequest(
            string				algorithm,
            DerObjectIdentifier	algOid)
        {
            X9ECParameters x9 = ECNamedCurveTable.GetByName("secp521r1");
            ECCurve curve = x9.Curve;
            ECDomainParameters spec = new ECDomainParameters(curve, x9.G, x9.N, x9.H);

            ECPrivateKeyParameters privKey = new ECPrivateKeyParameters(
                new BigInteger("5769183828869504557786041598510887460263120754767955773309066354712783118202294874205844512909370791582896372147797293913785865682804434049019366394746072023"), // d
                spec);

            ECPublicKeyParameters pubKey = new ECPublicKeyParameters(
//				curve.DecodePoint(Hex.Decode("026BFDD2C9278B63C92D6624F151C9D7A822CC75BD983B17D25D74C26740380022D3D8FAF304781E416175EADF4ED6E2B47142D2454A7AC7801DD803CF44A4D1F0AC")), // Q
                curve.DecodePoint(Hex.Decode("02006BFDD2C9278B63C92D6624F151C9D7A822CC75BD983B17D25D74C26740380022D3D8FAF304781E416175EADF4ED6E2B47142D2454A7AC7801DD803CF44A4D1F0AC")), // Q
                spec);

//			//
//			// set up the keys
//			//
//			AsymmetricKeyParameter privKey;
//			AsymmetricKeyParameter pubKey;
//
//			KeyFactory fact = KeyFactory.getInstance("ECDSA");
//
//			privKey = fact.generatePrivate(privKeySpec);
//			pubKey = fact.generatePublic(pubKeySpec);

            Pkcs10CertificationRequest req = new Pkcs10CertificationRequest(
                algorithm, new X509Name("CN=XXX"), pubKey, null, privKey);
            if (!req.Verify())
            {
                Fail("Failed Verify check EC.");
            }

            req = new Pkcs10CertificationRequest(req.GetEncoded());
            if (!req.Verify())
            {
                Fail("Failed Verify check EC encoded.");
            }

            //
            // try with point compression turned off
            //
//			((ECPointEncoder)pubKey).setPointFormat("UNCOMPRESSED");
            ECPoint q = pubKey.Q.Normalize();
            pubKey = new ECPublicKeyParameters(
                pubKey.AlgorithmName,
                q.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()),
                pubKey.Parameters);

            req = new Pkcs10CertificationRequest(
                algorithm, new X509Name("CN=XXX"), pubKey, null, privKey);
            if (!req.Verify())
            {
                Fail("Failed Verify check EC uncompressed.");
            }

            req = new Pkcs10CertificationRequest(req.GetEncoded());
            if (!req.Verify())
            {
                Fail("Failed Verify check EC uncompressed encoded.");
            }

            if (!req.SignatureAlgorithm.Algorithm.Equals(algOid))
            {
                Fail("ECDSA oid incorrect.");
            }

            if (req.SignatureAlgorithm.Parameters != null)
            {
                Fail("ECDSA parameters incorrect.");
            }

            ISigner sig = SignerUtilities.GetSigner(algorithm);

            sig.Init(false, pubKey);

            byte[] b = req.GetCertificationRequestInfo().GetEncoded();
            sig.BlockUpdate(b, 0, b.Length);

            if (!sig.VerifySignature(req.GetSignatureOctets()))
            {
                Fail("signature not mapped correctly.");
            }
        }

        private void createECGostRequest()
        {
            string algorithm = "GOST3411withECGOST3410";
            IAsymmetricCipherKeyPairGenerator ecGostKpg = GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");

            ecGostKpg.Init(
                new ECKeyGenerationParameters(
                    CryptoProObjectIdentifiers.GostR3410x2001CryptoProA,
                    new SecureRandom()));

            //
            // set up the keys
            //
            AsymmetricCipherKeyPair pair = ecGostKpg.GenerateKeyPair();
            AsymmetricKeyParameter privKey = pair.Private;
            AsymmetricKeyParameter pubKey = pair.Public;

            Pkcs10CertificationRequest req = new Pkcs10CertificationRequest(
                algorithm, new X509Name("CN=XXX"), pubKey, null, privKey);

            if (!req.Verify())
            {
                Fail("Failed Verify check EC.");
            }

            req = new Pkcs10CertificationRequest(req.GetEncoded());
            if (!req.Verify())
            {
                Fail("Failed Verify check EC encoded.");
            }

            if (!req.SignatureAlgorithm.Algorithm.Equals(CryptoProObjectIdentifiers.GostR3411x94WithGostR3410x2001))
            {
                Fail("ECGOST oid incorrect.");
            }

            if (req.SignatureAlgorithm.Parameters != null)
            {
                Fail("ECGOST parameters incorrect.");
            }

            ISigner sig = SignerUtilities.GetSigner(algorithm);

            sig.Init(false, pubKey);

            byte[] b = req.GetCertificationRequestInfo().GetEncoded();
            sig.BlockUpdate(b, 0, b.Length);

            if (!sig.VerifySignature(req.GetSignatureOctets()))
            {
                Fail("signature not mapped correctly.");
            }
        }

        private void createPssTest(
            string algorithm)
        {
//			RSAPublicKeySpec pubKeySpec = new RSAPublicKeySpec(
            RsaKeyParameters pubKey = new RsaKeyParameters(false,
                new BigInteger("a56e4a0e701017589a5187dc7ea841d156f2ec0e36ad52a44dfeb1e61f7ad991d8c51056ffedb162b4c0f283a12a88a394dff526ab7291cbb307ceabfce0b1dfd5cd9508096d5b2b8b6df5d671ef6377c0921cb23c270a70e2598e6ff89d19f105acc2d3f0cb35f29280e1386b6f64c4ef22e1e1f20d0ce8cffb2249bd9a2137",16),
                new BigInteger("010001",16));

//			RSAPrivateCrtKeySpec privKeySpec = new RSAPrivateCrtKeySpec(
            RsaPrivateCrtKeyParameters privKey = new RsaPrivateCrtKeyParameters(
                new BigInteger("a56e4a0e701017589a5187dc7ea841d156f2ec0e36ad52a44dfeb1e61f7ad991d8c51056ffedb162b4c0f283a12a88a394dff526ab7291cbb307ceabfce0b1dfd5cd9508096d5b2b8b6df5d671ef6377c0921cb23c270a70e2598e6ff89d19f105acc2d3f0cb35f29280e1386b6f64c4ef22e1e1f20d0ce8cffb2249bd9a2137",16),
                new BigInteger("010001",16),
                new BigInteger("33a5042a90b27d4f5451ca9bbbd0b44771a101af884340aef9885f2a4bbe92e894a724ac3c568c8f97853ad07c0266c8c6a3ca0929f1e8f11231884429fc4d9ae55fee896a10ce707c3ed7e734e44727a39574501a532683109c2abacaba283c31b4bd2f53c3ee37e352cee34f9e503bd80c0622ad79c6dcee883547c6a3b325",16),
                new BigInteger("e7e8942720a877517273a356053ea2a1bc0c94aa72d55c6e86296b2dfc967948c0a72cbccca7eacb35706e09a1df55a1535bd9b3cc34160b3b6dcd3eda8e6443",16),
                new BigInteger("b69dca1cf7d4d7ec81e75b90fcca874abcde123fd2700180aa90479b6e48de8d67ed24f9f19d85ba275874f542cd20dc723e6963364a1f9425452b269a6799fd",16),
                new BigInteger("28fa13938655be1f8a159cbaca5a72ea190c30089e19cd274a556f36c4f6e19f554b34c077790427bbdd8dd3ede2448328f385d81b30e8e43b2fffa027861979",16),
                new BigInteger("1a8b38f398fa712049898d7fb79ee0a77668791299cdfa09efc0e507acb21ed74301ef5bfd48be455eaeb6e1678255827580a8e4e8e14151d1510a82a3f2e729",16),
                new BigInteger("27156aba4126d24a81f3a528cbfb27f56886f840a9f6e86e17a44b94fe9319584b8e22fdde1e5a2e3bd8aa5ba8d8584194eb2190acf832b847f13a3d24a79f4d",16));

//			KeyFactory  fact = KeyFactory.getInstance("RSA", "BC");
//
//			PrivateKey privKey = fact.generatePrivate(privKeySpec);
//			PublicKey pubKey = fact.generatePublic(pubKeySpec);

            Pkcs10CertificationRequest req = new Pkcs10CertificationRequest(
                algorithm, new X509Name("CN=XXX"), pubKey, null, privKey);

            if (!req.Verify())
            {
                Fail("Failed verify check PSS.");
            }

            req = new Pkcs10CertificationRequest(req.GetEncoded());
            if (!req.Verify())
            {
                Fail("Failed verify check PSS encoded.");
            }

            if (!req.SignatureAlgorithm.Algorithm.Equals(PkcsObjectIdentifiers.IdRsassaPss))
            {
                Fail("PSS oid incorrect.");
            }

            if (req.SignatureAlgorithm.Parameters == null)
            {
                Fail("PSS parameters incorrect.");
            }

            ISigner sig = SignerUtilities.GetSigner(algorithm);

            sig.Init(false, pubKey);

            byte[] encoded = req.GetCertificationRequestInfo().GetEncoded();
            sig.BlockUpdate(encoded, 0, encoded.Length);

            if (!sig.VerifySignature(req.GetSignatureOctets()))
            {
                Fail("signature not mapped correctly.");
            }
        }

        // previous code found to cause a NullPointerException
        private void nullPointerTest()
        {
            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            keyGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));

            AsymmetricCipherKeyPair pair = keyGen.GenerateKeyPair();

            IList oids = new ArrayList();
            IList values = new ArrayList();
            oids.Add(X509Extensions.BasicConstraints);
            values.Add(new X509Extension(true, new DerOctetString(new BasicConstraints(true))));
            oids.Add(X509Extensions.KeyUsage);
            values.Add(new X509Extension(true, new DerOctetString(
                new KeyUsage(KeyUsage.KeyCertSign | KeyUsage.CrlSign))));
            SubjectKeyIdentifier subjectKeyIdentifier = new SubjectKeyIdentifierStructure(pair.Public);
            X509Extension ski = new X509Extension(false, new DerOctetString(subjectKeyIdentifier));
            oids.Add(X509Extensions.SubjectKeyIdentifier);
            values.Add(ski);

            AttributePkcs attribute = new AttributePkcs(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest,
                new DerSet(new X509Extensions(oids, values)));

            Pkcs10CertificationRequest p1 = new Pkcs10CertificationRequest(
                "SHA1WithRSA", new X509Name("cn=csr"), pair.Public, new DerSet(attribute), pair.Private);
            Pkcs10CertificationRequest p2 = new Pkcs10CertificationRequest(
                "SHA1WithRSA", new X509Name("cn=csr"), pair.Public, new DerSet(attribute), pair.Private);

            if (!p1.Equals(p2))
            {
                Fail("cert request comparison failed");
            }
        }

        public override void PerformTest()
        {
            generationTest(512, "RSA", "SHA1withRSA");
            generationTest(512, "GOST3410", "GOST3411withGOST3410");

            //		if (Security.getProvider("SunRsaSign") != null)
            //        {
            //            generationTest(512, "RSA", "SHA1withRSA", "SunRsaSign");
            //        }

            // elliptic curve GOST A parameter set
            Pkcs10CertificationRequest req = new Pkcs10CertificationRequest(gost3410EC_A);
            if (!req.Verify())
            {
                Fail("Failed Verify check gost3410EC_A.");
            }

            // elliptic curve GOST B parameter set
            req = new Pkcs10CertificationRequest(gost3410EC_B);
            if (!req.Verify())
            {
                Fail("Failed Verify check gost3410EC_B.");
            }

            // elliptic curve GOST C parameter set
            req = new Pkcs10CertificationRequest(gost3410EC_C);
            if (!req.Verify())
            {
                Fail("Failed Verify check gost3410EC_C.");
            }

            // elliptic curve GOST ExA parameter set
            req = new Pkcs10CertificationRequest(gost3410EC_ExA);
            if (!req.Verify())
            {
                Fail("Failed Verify check gost3410EC_ExA.");
            }

            // elliptic curve GOST ExB parameter set
            req = new Pkcs10CertificationRequest(gost3410EC_ExB);
            if (!req.Verify())
            {
                Fail("Failed Verify check gost3410EC_ExA.");
            }

            // elliptic curve openSSL
            IAsymmetricCipherKeyPairGenerator g = GeneratorUtilities.GetKeyPairGenerator("ECDSA");

            X9ECParameters x9 = ECNamedCurveTable.GetByName("prime239v1");
            ECCurve curve = x9.Curve;
            ECDomainParameters ecSpec = new ECDomainParameters(curve, x9.G, x9.N, x9.H);

//			g.initialize(ecSpec, new SecureRandom());
            g.Init(new ECKeyGenerationParameters(ecSpec, new SecureRandom()));

            AsymmetricCipherKeyPair kp = g.GenerateKeyPair();

            req = new Pkcs10CertificationRequest(
                "ECDSAWITHSHA1", new X509Name("CN=XXX"), kp.Public, null, kp.Private);

            if (!req.Verify())
            {
                Fail("Failed Verify check EC.");
            }

            createECRequest("SHA1withECDSA", X9ObjectIdentifiers.ECDsaWithSha1);
            createECRequest("SHA224withECDSA", X9ObjectIdentifiers.ECDsaWithSha224);
            createECRequest("SHA256withECDSA", X9ObjectIdentifiers.ECDsaWithSha256);
            createECRequest("SHA384withECDSA", X9ObjectIdentifiers.ECDsaWithSha384);
            createECRequest("SHA512withECDSA", X9ObjectIdentifiers.ECDsaWithSha512);

            createECGostRequest();

            // TODO The setting of parameters for MGF algorithms is not implemented
//			createPssTest("SHA1withRSAandMGF1");
//			createPssTest("SHA224withRSAandMGF1");
//			createPssTest("SHA256withRSAandMGF1");
//			createPssTest("SHA384withRSAandMGF1");

            nullPointerTest();
        }

        public static void Main(
            string[] args)
        {
            RunTest(new Pkcs10CertRequestTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
