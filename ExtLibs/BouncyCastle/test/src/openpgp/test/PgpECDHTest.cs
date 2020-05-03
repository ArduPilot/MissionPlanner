using System;
using System.Collections;
using System.IO;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Bcpg.OpenPgp.Tests
{
    [TestFixture]
    public class PgpECDHTest
        : SimpleTest
    {
        private static readonly byte[] testPubKey =
            Base64.Decode(
                "mFIEUb4GwBMIKoZIzj0DAQcCAwS8p3TFaRAx58qCG63W+UNthXBPSJDnVDPTb/sT" +
                "iXePaAZ/Gh1GKXTq7k6ab/67MMeVFp/EdySumqdWLtvceFKstFBUZXN0IEVDRFNB" +
                "LUVDREggKEtleSBhbmQgc3Via2V5IGFyZSAyNTYgYml0cyBsb25nKSA8dGVzdC5l" +
                "Y2RzYS5lY2RoQGV4YW1wbGUuY29tPoh6BBMTCAAiBQJRvgbAAhsDBgsJCAcDAgYV" +
                "CAIJCgsEFgIDAQIeAQIXgAAKCRD3wDlWjFo9U5O2AQDi89NO6JbaIObC63jMMWsi" +
                "AaQHrBCPkDZLibgNv73DLgD/faouH4YZJs+cONQBPVnP1baG1NpWR5ppN3JULFcr" +
                "hcq4VgRRvgbAEggqhkjOPQMBBwIDBLtY8Nmfz0zSEa8C1snTOWN+VcT8pXPwgJRy" +
                "z6kSP4nPt1xj1lPKj5zwPXKWxMkPO9ocqhKdg2mOh6/rc1ObIoMDAQgHiGEEGBMI" +
                "AAkFAlG+BsACGwwACgkQ98A5VoxaPVN8cgEAj4dMNMNwRSg2ZBWunqUAHqIedVbS" +
                "dmwmbysD192L3z4A/ReXEa0gtv8OFWjuALD1ovEK8TpDORLUb6IuUb5jUIzY");

        private static readonly byte[] testPrivKey =
            Base64.Decode(
                "lKUEUb4GwBMIKoZIzj0DAQcCAwS8p3TFaRAx58qCG63W+UNthXBPSJDnVDPTb/sT" +
                "iXePaAZ/Gh1GKXTq7k6ab/67MMeVFp/EdySumqdWLtvceFKs/gcDAo11YYCae/K2" +
                "1uKGJ/uU4b4QHYnPIsAdYpuo5HIdoAOL/WwduRa8C6vSFrtMJLDqPK3BUpMz3CXN" +
                "GyMhjuaHKP5MPbBZkIfgUGZO5qvU9+i0UFRlc3QgRUNEU0EtRUNESCAoS2V5IGFu" +
                "ZCBzdWJrZXkgYXJlIDI1NiBiaXRzIGxvbmcpIDx0ZXN0LmVjZHNhLmVjZGhAZXhh" +
                "bXBsZS5jb20+iHoEExMIACIFAlG+BsACGwMGCwkIBwMCBhUIAgkKCwQWAgMBAh4B" +
                "AheAAAoJEPfAOVaMWj1Tk7YBAOLz007oltog5sLreMwxayIBpAesEI+QNkuJuA2/" +
                "vcMuAP99qi4fhhkmz5w41AE9Wc/VtobU2lZHmmk3clQsVyuFyg==");

        private static readonly byte[] testMessage =
            Base64.Decode(
                "hH4Dp5+FdoujIBwSAgMErx4BSvgXY3irwthgxU8zPoAoR+8rhmxdpwbw6ZJAO2GX" +
                "azWJ85JNcobHKDeGeUq6wkTFu+g6yG99gIX8J5xJAjBRhyCRcaFgwbdDV4orWTe3" +
                "iewiT8qs4BQ23e0c8t+thdKoK4thMsCJy7wSKqY0sJTSVAELroNbCOi2lcO15YmW" +
                "6HiuFH7VKWcxPUBjXwf5+Z3uOKEp28tBgNyDrdbr1BbqlgYzIKq/pe9zUbUXfitn" +
                "vFc6HcGhvmRQreQ+Yw1x3x0HJeoPwg==");

        private void Generate()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA1PRNG");

            //
            // Generate a master key
            //
            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("ECDSA");
            keyGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, random));

            AsymmetricCipherKeyPair kpSign = keyGen.GenerateKeyPair();

            PgpKeyPair ecdsaKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.ECDsa, kpSign, DateTime.UtcNow);

            //
            // Generate an encryption key
            //
            keyGen = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            keyGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, random));

            AsymmetricCipherKeyPair kpEnc = keyGen.GenerateKeyPair();

            PgpKeyPair ecdhKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.ECDH, kpEnc, DateTime.UtcNow);

            //
            // Generate a key ring
            //
            char[] passPhrase = "test".ToCharArray();
            PgpKeyRingGenerator keyRingGen = new PgpKeyRingGenerator(PgpSignature.PositiveCertification, ecdsaKeyPair,
                "test@bouncycastle.org", SymmetricKeyAlgorithmTag.Aes256, passPhrase, true, null, null, random);
            keyRingGen.AddSubKey(ecdhKeyPair);

            PgpPublicKeyRing pubRing = keyRingGen.GeneratePublicKeyRing();

            // TODO: add check of KdfParameters
            DoBasicKeyRingCheck(pubRing);

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

            PgpPrivateKey pgpPrivKey = secRing.GetSecretKey().ExtractPrivateKey(passPhrase);
        }

        private void TestDecrypt(PgpSecretKeyRing secretKeyRing)
        {
            PgpObjectFactory pgpF = new PgpObjectFactory(testMessage);

            PgpEncryptedDataList encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            PgpPublicKeyEncryptedData encP = (PgpPublicKeyEncryptedData)encList[0];

            PgpSecretKey secretKey = secretKeyRing.GetSecretKey(); // secretKeyRing.GetSecretKey(encP.KeyId);

    //        PgpPrivateKey pgpPrivKey = secretKey.extractPrivateKey(new JcePBESecretKeyEncryptorBuilder());

    //        clear = encP.getDataStream(pgpPrivKey, "BC");
    //
    //        bOut.reset();
    //
    //        while ((ch = clear.read()) >= 0)
    //        {
    //            bOut.write(ch);
    //        }
    //
    //        out = bOut.toByteArray();
    //
    //        if (!AreEqual(out, text))
    //        {
    //            fail("wrong plain text in Generated packet");
    //        }
        }

        private void EncryptDecryptTest()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA1PRNG");

            byte[] text = Encoding.ASCII.GetBytes("hello world!");

            IAsymmetricCipherKeyPairGenerator keyGen = GeneratorUtilities.GetKeyPairGenerator("ECDH");
            keyGen.Init(new ECKeyGenerationParameters(SecObjectIdentifiers.SecP256r1, random));

            AsymmetricCipherKeyPair kpEnc = keyGen.GenerateKeyPair();

            PgpKeyPair ecdhKeyPair = new PgpKeyPair(PublicKeyAlgorithmTag.ECDH, kpEnc, DateTime.UtcNow);

            PgpLiteralDataGenerator lData = new PgpLiteralDataGenerator();
            MemoryStream ldOut = new MemoryStream();
            Stream pOut = lData.Open(ldOut, PgpLiteralDataGenerator.Utf8, PgpLiteralData.Console, text.Length, DateTime.UtcNow);

            pOut.Write(text, 0, text.Length);

            pOut.Close();

            byte[] data = ldOut.ToArray();

            MemoryStream cbOut = new MemoryStream();

            PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5, random);
            cPk.AddMethod(ecdhKeyPair.PublicKey);

            Stream cOut = cPk.Open(new UncloseableStream(cbOut), data.Length);

            cOut.Write(data, 0, data.Length);

            cOut.Close();

            PgpObjectFactory pgpF = new PgpObjectFactory(cbOut.ToArray());

            PgpEncryptedDataList encList = (PgpEncryptedDataList)pgpF.NextPgpObject();

            PgpPublicKeyEncryptedData encP = (PgpPublicKeyEncryptedData)encList[0];

            Stream clear = encP.GetDataStream(ecdhKeyPair.PrivateKey);

            pgpF = new PgpObjectFactory(clear);

            PgpLiteralData ld = (PgpLiteralData)pgpF.NextPgpObject();

            clear = ld.GetInputStream();
            MemoryStream bOut = new MemoryStream();

            int ch;
            while ((ch = clear.ReadByte()) >= 0)
            {
                bOut.WriteByte((byte)ch);
            }

            byte[] output = bOut.ToArray();

            if (!AreEqual(output, text))
            {
                Fail("wrong plain text in Generated packet");
            }
        }

        public override void PerformTest()
        {
            //
            // Read the public key
            //
            PgpPublicKeyRing pubKeyRing = new PgpPublicKeyRing(testPubKey);

            DoBasicKeyRingCheck(pubKeyRing);

            //
            // Read the private key
            //
            PgpSecretKeyRing secretKeyRing = new PgpSecretKeyRing(testPrivKey);

            TestDecrypt(secretKeyRing);

            EncryptDecryptTest();

            Generate();
        }

        private void DoBasicKeyRingCheck(PgpPublicKeyRing pubKeyRing)
        {
            foreach (PgpPublicKey pubKey in pubKeyRing.GetPublicKeys())
            {
                if (pubKey.IsMasterKey)
                {
                    if (pubKey.IsEncryptionKey)
                    {
                        Fail("master key showed as encryption key!");
                    }
                }
                else
                {
                    if (!pubKey.IsEncryptionKey)
                    {
                        Fail("sub key not encryption key!");
                    }

                    foreach (PgpSignature certification in pubKeyRing.GetPublicKey().GetSignatures())
                    {
                        certification.InitVerify(pubKeyRing.GetPublicKey());

                        if (!certification.VerifyCertification((string)First(pubKeyRing.GetPublicKey().GetUserIds()), pubKeyRing.GetPublicKey()))
                        {
                            Fail("subkey certification does not verify");
                        }
                    }
                }
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
            get { return "PgpECDHTest"; }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new PgpECDHTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
