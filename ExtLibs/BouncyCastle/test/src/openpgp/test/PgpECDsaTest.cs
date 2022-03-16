using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpECDsaTest
        : SimpleTest
    {
        private static readonly byte[] testPubKey =
            Base64.Decode(
                "mFIEUb4HqBMIKoZIzj0DAQcCAwSQynmjwsGJHYJakAEVYxrm3tt/1h8g9Uksx32J" +
                "zG/ZH4RwaD0PbjzEe5EVBmCwSErRZxt/5AxXa0TEHWjya8FetDVFQ0RTQSAoS2V5" +
                "IGlzIDI1NiBiaXRzIGxvbmcpIDx0ZXN0LmVjZHNhQGV4YW1wbGUuY29tPoh6BBMT" +
                "CAAiBQJRvgeoAhsDBgsJCAcDAgYVCAIJCgsEFgIDAQIeAQIXgAAKCRDqO46kgPLi" +
                "vN1hAP4n0UApR36ziS5D8KUt7wEpBujQE4G3+efATJ+DMmY/SgEA+wbdDynFf/V8" +
                "pQs0+FtCYQ9schzIur+peRvol7OrNnc=");

        private static readonly byte[] testPrivKey =
            Base64.Decode(
                "lKUEUb4HqBMIKoZIzj0DAQcCAwSQynmjwsGJHYJakAEVYxrm3tt/1h8g9Uksx32J" +
                "zG/ZH4RwaD0PbjzEe5EVBmCwSErRZxt/5AxXa0TEHWjya8Fe/gcDAqTWSUiFpEno" +
                "1n8izmLaWTy8GYw5/lK4R2t6D347YGgTtIiXfoNPOcosmU+3OibyTm2hc/WyG4fL" +
                "a0nxFtj02j0Bt/Fw0N4VCKJwKL/QJT+0NUVDRFNBIChLZXkgaXMgMjU2IGJpdHMg" +
                "bG9uZykgPHRlc3QuZWNkc2FAZXhhbXBsZS5jb20+iHoEExMIACIFAlG+B6gCGwMG" +
                "CwkIBwMCBhUIAgkKCwQWAgMBAh4BAheAAAoJEOo7jqSA8uK83WEA/ifRQClHfrOJ" +
                "LkPwpS3vASkG6NATgbf558BMn4MyZj9KAQD7Bt0PKcV/9XylCzT4W0JhD2xyHMi6" +
                "v6l5G+iXs6s2dw==");

        private static readonly char[] testPasswd = "test".ToCharArray();

        private static readonly byte[] sExprKey =
            Base64.Decode(
                "KDIxOnByb3RlY3RlZC1wcml2YXRlLWtleSgzOmVjYyg1OmN1cnZlMTU6YnJh"
                    + "aW5wb29sUDM4NHIxKSgxOnE5NzoEi29XCqkugtlRvONnpAVMQgfecL+Gk86O"
                    + "t8LnUizfHG2TqRrtqlMg1DdU8Z8dJWmhJG84IUOURCyjt8nE4BeeCfRIbTU5"
                    + "7CB13OqveBdNIRfK45UQnxHLO2MPVXf4GMdtKSg5OnByb3RlY3RlZDI1Om9w"
                    + "ZW5wZ3AtczJrMy1zaGExLWFlcy1jYmMoKDQ6c2hhMTg6itLEzGV4Cfg4OjEy"
                    + "OTA1NDcyKTE2OgxmufENKFTZUB72+X7AwkgpMTEyOvMWNLZgaGdlTN8XCxa6"
                    + "8ia0Xqqb9RvHgTh+iBf0RgY5Tx5hqO9fHOi76LTBMfxs9VC4f1rTketjEUKR"
                    + "f5amKb8lrJ67kKEsny4oRtP9ejkNzcvHFqRdxmHyL10ui8M8rJN9OU8ArqWf"
                    + "g22dTcKu02cpKDEyOnByb3RlY3RlZC1hdDE1OjIwMTQwNjA4VDE2MDg1MCkp"
                    + "KQ==");

        private void GenerateAndSign()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA1PRNG");

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            keyGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, random));

            AsymmetricCipherKeyPair kpSign = keyGen.GenerateKeyPair();

            PgpKeyPair ecdsaKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.ECDsa, kpSign, DateTime.UtcNow);

            byte[] msg = Encoding.ASCII.GetBytes("hello world!");

            //
            // try a signature
            //
            PgpSignatureGenerator signGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.ECDsa, HashAlgorithmTag.Sha256);
            signGen.InitSign(PgpSignature.BinaryDocument, ecdsaKeyPair.PrivateKey);

            signGen.Update(msg);

            PgpSignature sig = signGen.Generate();

            sig.InitVerify(ecdsaKeyPair.PublicKey);
            sig.Update(msg);

            if (!sig.Verify())
            {
                Fail("signature failed to verify!");
            }

            //
            // generate a key ring
            //
            char[] passPhrase = "test".ToCharArray();
            PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(PgpSignature.PositiveCertification, ecdsaKeyPair,
                "test@bouncycastle.org", SymmetricKeyAlgorithmTag.Aes256, passPhrase, true, null, null, random);

            PgpPublicKeyRing pubRing = keyRingGen.GeneratePublicKeyRing();
            PgpSecretKeyRing secRing = keyRingGen.GenerateSecretKeyRing();

            PgpPublicKeyRing pubRingEnc = new PgpPublicKeyRing(pubRing.GetEncoded());
            if (!Arrays.AreEqual(pubRing.GetEncoded(), pubRingEnc.GetEncoded()))
            {
                Fail("public key ring encoding failed");
            }

            PgpSecretKeyRing secRingEnc = new PgpSecretKeyRing(secRing.GetEncoded());
            if (!Arrays.AreEqual(secRing.GetEncoded(), secRingEnc.GetEncoded()))
            {
                Fail("secret key ring encoding failed");
            }


            //
            // try a signature using encoded key
            //
            signGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.ECDsa, HashAlgorithmTag.Sha256);
            signGen.InitSign(PgpSignature.BinaryDocument, secRing.GetSecretKey().ExtractPrivateKey(passPhrase));
            signGen.Update(msg);

            sig = signGen.Generate();
            sig.InitVerify(secRing.GetSecretKey().PublicKey);
            sig.Update(msg);

            if (!sig.Verify())
            {
                Fail("re-encoded signature failed to verify!");
            }
        }

        public override void PerformTest()
        {
            //
            // Read the public key
            //
            PgpPublicKeyRing pubKeyRing = new PgpPublicKeyRing(testPubKey);
            foreach (PgpSignature certification in pubKeyRing.GetPublicKey().GetSignatures())
            {
                certification.InitVerify(pubKeyRing.GetPublicKey());

                if (!certification.VerifyCertification((string)First(pubKeyRing.GetPublicKey().GetUserIds()), pubKeyRing.GetPublicKey()))
                {
                    Fail("self certification does not verify");
                }
            }

            if (pubKeyRing.GetPublicKey().BitStrength != 256)
            {
                Fail("incorrect bit strength returned");
            }

            //
            // Read the private key
            //
            PgpSecretKeyRing secretKeyRing = new PgpSecretKeyRing(testPrivKey);

            PgpPrivateKey privKey = secretKeyRing.GetSecretKey().ExtractPrivateKey(testPasswd);

            GenerateAndSign();

            //
            // sExpr
            //
            byte[] msg = Encoding.ASCII.GetBytes("hello world!");

            PgpSecretKey key = PgpSecretKey.ParseSecretKeyFromSExpr(new MemoryStream(sExprKey, false), "test".ToCharArray());

            PgpSignatureGenerator signGen = new PgpSignatureGenerator(PublicKeyAlgorithmTag.ECDsa, HashAlgorithmTag.Sha256);
            signGen.InitSign(PgpSignature.BinaryDocument, key.ExtractPrivateKey(null));
            signGen.Update(msg);

            PgpSignature sig = signGen.Generate();
            sig.InitVerify(key.PublicKey);
            sig.Update(msg);

            if (!sig.Verify())
            {
                Fail("signature failed to verify!");
            }
        }

        private static object First(IEnumerable e)
        {
            IEnumerator n = e.GetEnumerator();
            Assert.IsTrue(n.MoveNext());
            return n.Current;
        }

        public override string Name
        {
            get { return "PgpECDsaTest"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new PgpECDsaTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
