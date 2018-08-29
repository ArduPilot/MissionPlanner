using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.OpenSsl.Tests
{
    /**
    * basic class for reading test.pem - the password is "secret"
    */
    [TestFixture]
    public class ReaderTest
        : SimpleTest
    {
        private class Password
            : IPasswordFinder
        {
            private readonly char[] password;

            public Password(
                char[] word)
            {
                this.password = (char[]) word.Clone();
            }

            public char[] GetPassword()
            {
                return (char[]) password.Clone();
            }
        }

        public override string Name
        {
            get { return "PEMReaderTest"; }
        }

        public override void PerformTest()
        {
            IPasswordFinder pGet = new Password("secret".ToCharArray());
            PemReader pemRd = OpenPemResource("test.pem", pGet);
            AsymmetricCipherKeyPair pair;

            object o;
            while ((o = pemRd.ReadObject()) != null)
            {
//				if (o is AsymmetricCipherKeyPair)
//				{
//					ackp = (AsymmetricCipherKeyPair)o;
//
//					Console.WriteLine(ackp.Public);
//					Console.WriteLine(ackp.Private);
//				}
//				else
//				{
//					Console.WriteLine(o.ToString());
//				}
            }

            //
            // pkcs 7 data
            //
            pemRd = OpenPemResource("pkcs7.pem", null);

            ContentInfo d = (ContentInfo)pemRd.ReadObject();    
    
            if (!d.ContentType.Equals(CmsObjectIdentifiers.EnvelopedData))
            {
                Fail("failed envelopedData check");
            }

            /*
            {
                //
                // ECKey
                //
                pemRd = OpenPemResource("eckey.pem", null);
    
                // TODO Resolve return type issue with EC keys and fix PemReader to return parameters
//				ECNamedCurveParameterSpec spec = (ECNamedCurveParameterSpec)pemRd.ReadObject();
    
                pair = (AsymmetricCipherKeyPair)pemRd.ReadObject();
                ISigner sgr = SignerUtilities.GetSigner("ECDSA");
    
                sgr.Init(true, pair.Private);
    
                byte[] message = new byte[] { (byte)'a', (byte)'b', (byte)'c' };
    
                sgr.BlockUpdate(message, 0, message.Length);
    
                byte[] sigBytes = sgr.GenerateSignature();
    
                sgr.Init(false, pair.Public);
    
                sgr.BlockUpdate(message, 0, message.Length);
    
                if (!sgr.VerifySignature(sigBytes))
                {
                    Fail("EC verification failed");
                }

                // TODO Resolve this issue with the algorithm name, study Java version
//				if (!((ECPublicKeyParameters) pair.Public).AlgorithmName.Equals("ECDSA"))
//				{
//					Fail("wrong algorithm name on public got: " + ((ECPublicKeyParameters) pair.Public).AlgorithmName);
//				}
//	
//				if (!((ECPrivateKeyParameters) pair.Private).AlgorithmName.Equals("ECDSA"))
//				{
//					Fail("wrong algorithm name on private got: " + ((ECPrivateKeyParameters) pair.Private).AlgorithmName);
//				}
            }
            */

            //
            // writer/parser test
            //
            IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            kpGen.Init(
                new RsaKeyGenerationParameters(
                BigInteger.ValueOf(0x10001),
                new SecureRandom(),
                768,
                25));

            pair = kpGen.GenerateKeyPair();

            keyPairTest("RSA", pair);

//			kpGen = KeyPairGenerator.getInstance("DSA");
//			kpGen.initialize(512, new SecureRandom());
            DsaParametersGenerator pGen = new DsaParametersGenerator();
            pGen.Init(512, 80, new SecureRandom());

            kpGen = GeneratorUtilities.GetKeyPairGenerator("DSA");
            kpGen.Init(
                new DsaKeyGenerationParameters(
                    new SecureRandom(),
                    pGen.GenerateParameters()));

            pair = kpGen.GenerateKeyPair();

            keyPairTest("DSA", pair);

            //
            // PKCS7
            //
            MemoryStream bOut = new MemoryStream();
            PemWriter pWrt = new PemWriter(new StreamWriter(bOut));

            pWrt.WriteObject(d);
            pWrt.Writer.Close();

            pemRd = new PemReader(new StreamReader(new MemoryStream(bOut.ToArray(), false)));
            d = (ContentInfo)pemRd.ReadObject();    

            if (!d.ContentType.Equals(CmsObjectIdentifiers.EnvelopedData))
            {
                Fail("failed envelopedData recode check");
            }


            // OpenSSL test cases (as embedded resources)
            doOpenSslDsaTest("unencrypted");
            doOpenSslRsaTest("unencrypted");

            doOpenSslTests("aes128");
            doOpenSslTests("aes192");
            doOpenSslTests("aes256");
            doOpenSslTests("blowfish");
            doOpenSslTests("des1");
            doOpenSslTests("des2");
            doOpenSslTests("des3");
            doOpenSslTests("rc2_128");

            doOpenSslDsaTest("rc2_40_cbc");
            doOpenSslRsaTest("rc2_40_cbc");
            doOpenSslDsaTest("rc2_64_cbc");
            doOpenSslRsaTest("rc2_64_cbc");

            doDudPasswordTest("7fd98", 0, "Corrupted stream - out of bounds length found");
            doDudPasswordTest("ef677", 1, "Corrupted stream - out of bounds length found");
            doDudPasswordTest("800ce", 2, "unknown tag 26 encountered");
            doDudPasswordTest("b6cd8", 3, "DEF length 81 object truncated by 56");
            doDudPasswordTest("28ce09", 4, "DEF length 110 object truncated by 28");
            doDudPasswordTest("2ac3b9", 5, "DER length more than 4 bytes: 11");
            doDudPasswordTest("2cba96", 6, "DEF length 100 object truncated by 35");
            doDudPasswordTest("2e3354", 7, "DEF length 42 object truncated by 9");
            doDudPasswordTest("2f4142", 8, "DER length more than 4 bytes: 14");
            doDudPasswordTest("2fe9bb", 9, "DER length more than 4 bytes: 65");
            doDudPasswordTest("3ee7a8", 10, "DER length more than 4 bytes: 57");
            doDudPasswordTest("41af75", 11, "unknown tag 16 encountered");
            doDudPasswordTest("1704a5", 12, "corrupted stream detected");
            doDudPasswordTest("1c5822", 13, "extra data found after object");
            doDudPasswordTest("5a3d16", 14, "corrupted stream detected");
            doDudPasswordTest("8d0c97", 15, "corrupted stream detected");
            doDudPasswordTest("bc0daf", 16, "corrupted stream detected");
            doDudPasswordTest("aaf9c4d",17, "Corrupted stream - out of bounds length found");

            // encrypted private key test
            pGet = new Password("password".ToCharArray());
            pemRd = OpenPemResource("enckey.pem", pGet);

            RsaPrivateCrtKeyParameters privKey = (RsaPrivateCrtKeyParameters)pemRd.ReadObject();

            if (!privKey.PublicExponent.Equals(new BigInteger("10001", 16)))
            {
                Fail("decryption of private key data check failed");
            }

            // general PKCS8 test
            pGet = new Password("password".ToCharArray());
            pemRd = OpenPemResource("pkcs8test.pem", pGet);

            while ((privKey = (RsaPrivateCrtKeyParameters)pemRd.ReadObject()) != null)
            {
                if (!privKey.PublicExponent.Equals(new BigInteger("10001", 16)))
                {
                    Fail("decryption of private key data check failed");
                }
            }
        }

        private void keyPairTest(
            string					name,
            AsymmetricCipherKeyPair	pair) 
        {
            MemoryStream bOut = new MemoryStream();
            PemWriter pWrt = new PemWriter(new StreamWriter(bOut));

            pWrt.WriteObject(pair.Public);
            pWrt.Writer.Close();

            PemReader pemRd = new PemReader(new StreamReader(new MemoryStream(bOut.ToArray(), false)));

            AsymmetricKeyParameter pubK = (AsymmetricKeyParameter) pemRd.ReadObject();
            if (!pubK.Equals(pair.Public))
            {
                Fail("Failed public key read: " + name);
            }

            bOut = new MemoryStream();
            pWrt = new PemWriter(new StreamWriter(bOut));

            pWrt.WriteObject(pair.Private);
            pWrt.Writer.Close();

            pemRd = new PemReader(new StreamReader(new MemoryStream(bOut.ToArray(), false)));

            AsymmetricCipherKeyPair kPair = (AsymmetricCipherKeyPair) pemRd.ReadObject();
            if (!kPair.Private.Equals(pair.Private))
            {
                Fail("Failed private key read: " + name);
            }
            
            if (!kPair.Public.Equals(pair.Public))
            {
                Fail("Failed private key public read: " + name);
            }
        }

        private void doOpenSslTests(
            string baseName)
        {
            doOpenSslDsaModesTest(baseName);
            doOpenSslRsaModesTest(baseName);
        }

        private void doOpenSslDsaModesTest(
            string baseName)
        {
            doOpenSslDsaTest(baseName + "_cbc");
            doOpenSslDsaTest(baseName + "_cfb");
            doOpenSslDsaTest(baseName + "_ecb");
            doOpenSslDsaTest(baseName + "_ofb");
        }

        private void doOpenSslRsaModesTest(
            string baseName)
        {
            doOpenSslRsaTest(baseName + "_cbc");
            doOpenSslRsaTest(baseName + "_cfb");
            doOpenSslRsaTest(baseName + "_ecb");
            doOpenSslRsaTest(baseName + "_ofb");
        }

        private void doOpenSslDsaTest(
            string name)
        {
            string fileName = "dsa.openssl_dsa_" + name + ".pem";

            doOpenSslTestFile(fileName, typeof(DsaPrivateKeyParameters));
        }

        private void doOpenSslRsaTest(
            string name)
        {
            string fileName = "rsa.openssl_rsa_" + name + ".pem";

            doOpenSslTestFile(fileName, typeof(RsaPrivateCrtKeyParameters));
        }

        private void doOpenSslTestFile(
            string	fileName,
            Type	expectedPrivKeyType)
        {
            PemReader pr = OpenPemResource(fileName, new Password("changeit".ToCharArray()));
            AsymmetricCipherKeyPair kp = pr.ReadObject() as AsymmetricCipherKeyPair;
            pr.Reader.Close();

            if (kp == null)
            {
                Fail("Didn't find OpenSSL key");
            }

            if (!expectedPrivKeyType.IsInstanceOfType(kp.Private))
            {
                Fail("Returned key not of correct type");
            }
        }

        private void doDudPasswordTest(string password, int index, string message)
        {
            // illegal state exception check - in this case the wrong password will
            // cause an underlying class cast exception.
            try
            {
                IPasswordFinder pGet = new Password(password.ToCharArray());
                PemReader pemRd = OpenPemResource("test.pem", pGet);

                Object o;
                while ((o = pemRd.ReadObject()) != null)
                {
                }

                Fail("issue not detected: " + index);
            }
            catch (Exception e)
            {
                if (e.Message.IndexOf(message) < 0)
                {
                    Console.Error.WriteLine(message);
                    Console.Error.WriteLine(e.Message);
                    Fail("issue " + index + " exception thrown, but wrong message");
                }
            }
        }

        private static PemReader OpenPemResource(
            string			fileName,
            IPasswordFinder	pGet)
        {
            Stream data = GetTestDataAsStream("openssl." + fileName);
            TextReader tr = new StreamReader(data);
            return new PemReader(tr, pGet);
        }

        public static void Main(
            string[] args)
        {
            RunTest(new ReaderTest());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }
    }
}
