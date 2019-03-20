using System;
using System.Collections;
using System.Globalization;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Security.Tests
{
    [TestFixture]
    public class TestSignerUtilities
    {
        [Test]
        public void TestAlgorithms()
        {
            //
            // RSA parameters
            //
            BigInteger rsaMod = new BigInteger("a7295693155b1813bb84877fb45343556e0568043de5910872a3a518cc11e23e2db74eaf4545068c4e3d258a2718fbacdcc3eafa457695b957e88fbf110aed049a992d9c430232d02f3529c67a3419935ea9b569f85b1bcd37de6b899cd62697e843130ff0529d09c97d813cb15f293751ff56f943fbdabb63971cc7f4f6d5bff1594416b1f5907bde5a84a44f9802ef29b43bda1960f948f8afb8766c1ab80d32eec88ed66d0b65aebe44a6d0b3c5e0ab051aaa1b912fbcc17b8e751ddecc5365b6db6dab0020c3057db4013a51213a5798a3aab67985b0f4d88627a54a0f3f0285fbcb4afdfeb65cb153af66825656d43238b75503231500753f4e421e3c57", 16);
            BigInteger rsaPubExp = new BigInteger("10001", 16);

            BigInteger rsaPrivExp = new BigInteger("65dad56ac7df7abb434e4cb5eeadb16093aa6da7f0033aad3815289b04757d32bfee6ade7749c8e4a323b5050a2fb9e2a99e23469e1ed4ba5bab54336af20a5bfccb8b3424cc6923db2ffca5787ed87aa87aa614cd04cedaebc8f623a2d2063017910f436dff18bb06f01758610787f8b258f0a8efd8bd7de30007c47b2a1031696c7d6523bc191d4d918927a7e0b09584ed205bd2ff4fc4382678df82353f7532b3bbb81d69e3f39070aed3fb64fce032a089e8e64955afa5213a6eb241231bd98d702fba725a9b205952fda186412d9e0d9344d2998c455ad8c2bae85ee672751466d5288304032b5b7e02f7e558c7af82c7fbf58eea0bb4ef0f001e6cd0a9", 16);
            BigInteger rsaPrivP = new BigInteger("d4fd9ac3474fb83aaf832470643609659e511b322632b239b688f3cd2aad87527d6cf652fb9c9ca67940e84789444f2e99b0cb0cfabbd4de95396106c865f38e2fb7b82b231260a94df0e01756bf73ce0386868d9c41645560a81af2f53c18e4f7cdf3d51d80267372e6e0216afbf67f655c9450769cca494e4f6631b239ce1b", 16);
            BigInteger rsaPrivQ = new BigInteger("c8eaa0e2a1b3a4412a702bccda93f4d150da60d736c99c7c566fdea4dd1b401cbc0d8c063daaf0b579953d36343aa18b33dbf8b9eae94452490cc905245f8f7b9e29b1a288bc66731a29e1dd1a45c9fd7f8238ff727adc49fff73991d0dc096206b9d3a08f61e7462e2b804d78cb8c5eccdb9b7fbd2ad6a8fea46c1053e1be75", 16);
            BigInteger rsaPrivDP = new BigInteger("10edcb544421c0f9e123624d1099feeb35c72a8b34e008ac6fa6b90210a7543f293af4e5299c8c12eb464e70092805c7256e18e5823455ba0f504d36f5ccacac1b7cd5c58ff710f9c3f92646949d88fdd1e7ea5fed1081820bb9b0d2a8cd4b093fecfdb96dabd6e28c3a6f8c186dc86cddc89afd3e403e0fcf8a9e0bcb27af0b", 16);
            BigInteger rsaPrivDQ = new BigInteger("97fc25484b5a415eaa63c03e6efa8dafe9a1c8b004d9ee6e80548fefd6f2ce44ee5cb117e77e70285798f57d137566ce8ea4503b13e0f1b5ed5ca6942537c4aa96b2a395782a4cb5b58d0936e0b0fa63b1192954d39ced176d71ef32c6f42c84e2e19f9d4dd999c2151b032b97bd22aa73fd8c5bcd15a2dca4046d5acc997021", 16);
            BigInteger rsaPrivQinv = new BigInteger("4bb8064e1eff7e9efc3c4578fcedb59ca4aef0993a8312dfdcb1b3decf458aa6650d3d0866f143cbf0d3825e9381181170a0a1651eefcd7def786b8eb356555d9fa07c85b5f5cbdd74382f1129b5e36b4166b6cc9157923699708648212c484958351fdc9cf14f218dbe7fbf7cbd93a209a4681fe23ceb44bab67d66f45d1c9d", 16);

            RsaKeyParameters rsaPublic = new RsaKeyParameters(false, rsaMod, rsaPubExp);
            RsaPrivateCrtKeyParameters rsaPrivate = new RsaPrivateCrtKeyParameters(
                rsaMod, rsaPubExp, rsaPrivExp, rsaPrivP, rsaPrivQ, rsaPrivDP, rsaPrivDQ, rsaPrivQinv);

            //
            // ECDSA parameters
            //
            BigInteger ECParraGX = new BigInteger(Base64.Decode("D/qWPNyogWzMM7hkK+35BcPTWFc9Pyf7vTs8uaqv"));
            BigInteger ECParraGY = new BigInteger(Base64.Decode("AhQXGxb1olGRv6s1LPRfuatMF+cx3ZTGgzSE/Q5R"));
            BigInteger ECParraH = new BigInteger(Base64.Decode("AQ=="));
            BigInteger ECParraN = new BigInteger(Base64.Decode("f///////////////f///nl6an12QcfvRUiaIkJ0L"));
            BigInteger ECPubQX = new BigInteger(Base64.Decode("HWWi17Yb+Bm3PYr/DMjLOYNFhyOwX1QY7ZvqqM+l"));
            BigInteger ECPubQY = new BigInteger(Base64.Decode("JrlJfxu3WGhqwtL/55BOs/wsUeiDFsvXcGhB8DGx"));
            BigInteger ECPrivD = new BigInteger(Base64.Decode("GYQmd/NF1B+He1iMkWt3by2Az6Eu07t0ynJ4YCAo"));

            FpCurve curve = new FpCurve(
                new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
                new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
                new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16), // b
                ECParraN, ECParraH);

            ECDomainParameters ecDomain = new ECDomainParameters(
                curve,
                curve.ValidatePoint(ECParraGX, ECParraGY),
                ECParraN, ECParraH);

            ECPublicKeyParameters ecPub = new ECPublicKeyParameters(
                curve.ValidatePoint(ECPubQX, ECPubQY),
                ecDomain);

            ECPrivateKeyParameters ecPriv = new ECPrivateKeyParameters(ECPrivD, ecDomain);

            //
            // DSA parameters
            //
            BigInteger DSAParaG = new BigInteger(Base64.Decode("AL0fxOTq10OHFbCf8YldyGembqEu08EDVzxyLL29Zn/t4It661YNol1rnhPIs+cirw+yf9zeCe+KL1IbZ/qIMZM="));
            BigInteger DSAParaP = new BigInteger(Base64.Decode("AM2b/UeQA+ovv3dL05wlDHEKJ+qhnJBsRT5OB9WuyRC830G79y0R8wuq8jyIYWCYcTn1TeqVPWqiTv6oAoiEeOs="));
            BigInteger DSAParaQ = new BigInteger(Base64.Decode("AIlJT7mcKL6SUBMmvm24zX1EvjNx"));
            BigInteger DSAPublicY = new BigInteger(Base64.Decode("TtWy2GuT9yGBWOHi1/EpCDa/bWJCk2+yAdr56rAcqP0eHGkMnA9s9GJD2nGU8sFjNHm55swpn6JQb8q0agrCfw=="));
            BigInteger DsaPrivateX = new BigInteger(Base64.Decode("MMpBAxNlv7eYfxLTZ2BItJeD31A="));

            DsaParameters para = new DsaParameters(DSAParaP, DSAParaQ, DSAParaG);
            DsaPrivateKeyParameters dsaPriv = new DsaPrivateKeyParameters(DsaPrivateX, para);
            DsaPublicKeyParameters dsaPub = new DsaPublicKeyParameters(DSAPublicY, para);

            //
            // ECGOST3410 parameters
            //
            IAsymmetricCipherKeyPairGenerator ecGostKpg = GeneratorUtilities.GetKeyPairGenerator("ECGOST3410");
            ecGostKpg.Init(
                new ECKeyGenerationParameters(
                    CryptoProObjectIdentifiers.GostR3410x2001CryptoProA,
                    new SecureRandom()));

            AsymmetricCipherKeyPair ecGostPair = ecGostKpg.GenerateKeyPair();

            //
            // GOST3410 parameters
            //
            IAsymmetricCipherKeyPairGenerator gostKpg = GeneratorUtilities.GetKeyPairGenerator("GOST3410");
            gostKpg.Init(
                new Gost3410KeyGenerationParameters(
                    new SecureRandom(),
                    CryptoProObjectIdentifiers.GostR3410x94CryptoProA));

            AsymmetricCipherKeyPair gostPair = gostKpg.GenerateKeyPair();



            //
            // signer loop
            //
            byte[] shortMsg = new byte[] { 1, 4, 5, 6, 8, 8, 4, 2, 1, 3 };
            byte[] longMsg = new byte[100];
            new SecureRandom().NextBytes(longMsg);

            foreach (string algorithm in SignerUtilities.Algorithms)
            {
                ISigner signer = SignerUtilities.GetSigner(algorithm);

                string upper = algorithm.ToUpper(CultureInfo.InvariantCulture);
                int withPos = upper.LastIndexOf("WITH");

                string cipherName = withPos < 0
                    ?	upper
                    :	upper.Substring(withPos + "WITH".Length);

                ICipherParameters signParams = null, verifyParams = null;

                if (cipherName == "RSA" || cipherName == "RSAANDMGF1")
                {
                    signParams = rsaPrivate;
                    verifyParams = rsaPublic;
                }
                else if (cipherName == "ECDSA")
                {
                    signParams = ecPriv;
                    verifyParams = ecPub;
                }
                else if (cipherName == "DSA")
                {
                    signParams = dsaPriv;
                    verifyParams = dsaPub;
                }
                else if (cipherName == "ECGOST3410")
                {
                    signParams = ecGostPair.Private;
                    verifyParams = ecGostPair.Public;
                }
                else if (cipherName == "GOST3410")
                {
                    signParams = gostPair.Private;
                    verifyParams = gostPair.Public;
                }
                else
                {
                    Assert.Fail("Unknown algorithm encountered: " + cipherName);
                }

                signer.Init(true, signParams);
                foreach (byte b in shortMsg)
                {
                    signer.Update(b);
                }
                signer.BlockUpdate(longMsg, 0, longMsg.Length);
                byte[] sig = signer.GenerateSignature();

                signer.Init(false, verifyParams);
                foreach (byte b in shortMsg)
                {
                    signer.Update(b);
                }
                signer.BlockUpdate(longMsg, 0, longMsg.Length);

                Assert.IsTrue(signer.VerifySignature(sig), cipherName + " signer " + algorithm + " failed.");
            }
        }
    }
}
