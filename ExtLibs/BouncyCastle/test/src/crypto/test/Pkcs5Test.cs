using System;
using System.IO;

using NUnit.Framework;

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Tests
{

    /// <summary> A test class for Pkcs5 PbeS2 with PBKDF2 (Pkcs5 v2.0) using
    /// test vectors provider at
    /// <a href="http://www.rsasecurity.com/rsalabs/pkcs/pkcs-5/index.html">
    /// RSA's Pkcs5 Page</a>
    /// <br/>
    /// The vectors are Base 64 encoded and encrypted using the password "password"
    /// (without quotes). They should all yield the same PrivateKeyInfo object.
    /// </summary>
    [TestFixture]
    public class Pkcs5Test
        : SimpleTest
    {
        public override string Name
        {
            get { return "Pkcs5Test"; }
        }

        /// <summary> encrypted using des-cbc.</summary>
        internal static byte[] sample1;

        /// <summary> encrypted using des-ede3-cbc.</summary>
        internal static byte[] sample2;

        /// <summary> encrypted using rc2-cbc.</summary>
        internal static byte[] sample3;

        internal static byte[] result;

        private class PbeTest
            : SimpleTest
        {
            private Pkcs5Test enclosingInstance;

            private void InitBlock(
                Pkcs5Test enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public override string Name
            {
                get { return cipher.AlgorithmName + " Pkcs5S2 Test " + id; }
            }

            public Pkcs5Test Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            internal int id;
            internal BufferedBlockCipher cipher;
            internal byte[] sample;
            internal int keySize;

            internal PbeTest(
                Pkcs5Test			enclosingInstance,
                int					id,
                BufferedBlockCipher	cipher,
                byte[]				sample,
                int					keySize)
            {
                InitBlock(enclosingInstance);

                this.id = id;
                this.cipher = cipher;
                this.sample = sample;
                this.keySize = keySize;
            }

            public override void PerformTest()
            {
                char[] password = "password".ToCharArray();
                PbeParametersGenerator generator = new Pkcs5S2ParametersGenerator();

                EncryptedPrivateKeyInfo info = null;
                try
                {
                    info = EncryptedPrivateKeyInfo.GetInstance(Asn1Object.FromByteArray(sample));
                }
                catch (System.Exception e)
                {
                    Fail("failed construction - exception " + e.ToString(), e);
                }

                PbeS2Parameters alg = PbeS2Parameters.GetInstance(info.EncryptionAlgorithm.Parameters);
                Pbkdf2Params func = Pbkdf2Params.GetInstance(alg.KeyDerivationFunc.Parameters);
                EncryptionScheme scheme = alg.EncryptionScheme;

                if (func.KeyLength != null)
                {
                    keySize = func.KeyLength.IntValue * 8;
                }

                int iterationCount = func.IterationCount.IntValue;
                byte[] salt = func.GetSalt();

                generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password), salt, iterationCount);

                DerObjectIdentifier algOid = scheme.Algorithm;

                byte[] iv;
                if (algOid.Equals(PkcsObjectIdentifiers.RC2Cbc))
                {
                    RC2CbcParameter rc2Params = RC2CbcParameter.GetInstance(scheme.Asn1Object);
                    iv = rc2Params.GetIV();
                }
                else
                {
                    iv = ((Asn1OctetString) scheme.Asn1Object).GetOctets();
                }

                ICipherParameters param = new ParametersWithIV(
                    generator.GenerateDerivedParameters(algOid.Id, keySize), iv);

                cipher.Init(false, param);

                byte[] data = info.GetEncryptedData();
                byte[] outBytes = new byte[cipher.GetOutputSize(data.Length)];
                int len = cipher.ProcessBytes(data, 0, data.Length, outBytes, 0);

                try
                {
                    len += cipher.DoFinal(outBytes, len);
                }
                catch (Exception e)
                {
                    Fail("failed DoFinal - exception " + e.ToString());
                }

                if (result.Length != len)
                {
                    Fail("failed length");
                }

                for (int i = 0; i != len; i++)
                {
                    if (outBytes[i] != result[i])
                    {
                        Fail("failed comparison");
                    }
                }
            }
        }

        public override void PerformTest()
        {
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEngine()));
            SimpleTest test = new PbeTest(this, 0, cipher, sample1, 64);

            test.PerformTest();

            cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new DesEdeEngine()));
            test = new PbeTest(this, 1, cipher, sample2, 192);

            test.PerformTest();

            cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new RC2Engine()));
            test = new PbeTest(this, 2, cipher, sample3, 0);

            test.PerformTest();

            //
            // RFC 3211 tests
            //
            char[] password = "password".ToCharArray();
            PbeParametersGenerator generator = new Pkcs5S2ParametersGenerator();

            byte[] salt = Hex.Decode("1234567878563412");

            generator.Init(
                PbeParametersGenerator.Pkcs5PasswordToBytes(password),
                salt,
                5);

            if (!AreEqual(((KeyParameter)generator.GenerateDerivedParameters("DES", 64)).GetKey(),
                Hex.Decode("d1daa78615f287e6")))
            {
                Fail("64 test failed");
            }

            password = "All n-entities must communicate with other n-entities via n-1 entiteeheehees".ToCharArray();

            generator.Init(
                PbeParametersGenerator.Pkcs5PasswordToBytes(password),
                salt,
                500);

            if (!AreEqual(((KeyParameter)generator.GenerateDerivedParameters("DESEDE", 192)).GetKey(),
                Hex.Decode("6a8970bf68c92caea84a8df28510858607126380cc47ab2d")))
            {
                Fail("192 test failed");
            }

            generator.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password), salt, 60000);
            if (!AreEqual(((KeyParameter)generator.GenerateDerivedParameters("DESEDE", 192)).GetKey(),
                Hex.Decode("29aaef810c12ecd2236bbcfb55407f9852b5573dc1c095bb")))
            {
                Fail("192 (60000) test failed");
            }
        }

        public static void Main(
            string[] args)
        {
            RunTest(new Pkcs5Test());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        static Pkcs5Test()
        {
            sample1 = Base64.Decode("MIIBozA9BgkqhkiG9w0BBQ0wMDAbBgkqhkiG9w0BBQwwDgQIfWBDXwLp4K4CAggA" + "MBEGBSsOAwIHBAiaCF/AvOgQ6QSCAWDWX4BdAzCRNSQSANSuNsT5X8mWYO27mr3Y" + "9c9LoBVXGNmYWKA77MI4967f7SmjNcgXj3xNE/jmnVz6hhsjS8E5VPT3kfyVkpdZ" + "0lr5e9Yk2m3JWpPU7++v5zBkZmC4V/MwV/XuIs6U+vykgzMgpxQg0oZKS9zgmiZo" + "f/4dOCL0UtCDnyOSvqT7mCVIcMDIEKu8QbVlgZYBop08l60EuEU3gARUo8WsYQmO" + "Dz/ldx0Z+znIT0SXVuOwc+RVItC5T/Qx+aijmmpt+9l14nmaGBrEkmuhmtdvU/4v" + "aptewGRgmjOfD6cqK+zs0O5NrrJ3P/6ZSxXj91CQgrThGfOv72bUncXEMNtc8pks" + "2jpHFjGMdKufnadAD7XuMgzkkaklEXZ4f5tU6heIIwr51g0GBEGF96gYPFnjnSQM" + "75JE02Clo+DfcfXpcybPTwwFg2jd6JTTOfkdf6OdSlA/1XNK43FA");
            sample2 = Base64.Decode("MIIBpjBABgkqhkiG9w0BBQ0wMzAbBgkqhkiG9w0BBQwwDgQIeFeOWl1jywYCAggA" + "MBQGCCqGSIb3DQMHBAjUJ5eGBhQGtQSCAWBrHrRgqO8UUMLcWzZEtpk1l3mjxiF/" + "koCMkHsFwowgyWhEbgIkTgbSViK54LVK8PskekcGNLph+rB6bGZ7pPbL5pbXASJ8" + "+MkQcG3FZdlS4Ek9tTJDApj3O1UubZGFG4uvTlJJFbF1BOJ3MkY3XQ9Gl1qwv7j5" + "6e103Da7Cq9+oIDKmznza78XXQYrUsPo8mJGjUxPskEYlzwvHjKubRnYm/K6RKhi" + "5f4zX4BQ/Dt3H812ZjRXrsjAJP0KrD/jyD/jCT7zNBVPH1izBds+RwizyQAHwfNJ" + "BFR78TH4cgzB619X47FDVOnT0LqQNVd0O3cSwnPrXE9XR3tPayE+iOB15llFSmi8" + "z0ByOXldEpkezCn92Umk++suzIVj1qfsK+bv2phZWJPbLEIWPDRHUbYf76q5ArAr" + "u4xtxT/hoK3krEs/IN3d70qjlUJ36SEw1UaZ82PWhakQbdtu39ZraMJB");
            sample3 = Base64.Decode("MIIBrjBIBgkqhkiG9w0BBQ0wOzAeBgkqhkiG9w0BBQwwEQQIrHyQPBZqWLUCAggA" + "AgEQMBkGCCqGSIb3DQMCMA0CAToECEhbh7YZKiPSBIIBYCT1zp6o5jpFlIkgwPop" + "7bW1+8ACr4exqzkeb3WflQ8cWJ4cURxzVdvxUnXeW1VJdaQZtjS/QHs5GhPTG/0f" + "wtvnaPfwrIJ3FeGaZfcg2CrYhalOFmEb4xrE4KyoEQmUN8tb/Cg94uzd16BOPw21" + "RDnE8bnPdIGY7TyL95kbkqH23mK53pi7h+xWIgduW+atIqDyyt55f7WMZcvDvlj6" + "VpN/V0h+qxBHL274WA4dj6GYgeyUFpi60HdGCK7By2TBy8h1ZvKGjmB9h8jZvkx1" + "MkbRumXxyFsowTZawyYvO8Um6lbfEDP9zIEUq0IV8RqH2MRyblsPNSikyYhxX/cz" + "tdDxRKhilySbSBg5Kr8OfcwKp9bpinN96nmG4xr3Tch1bnVvqJzOQ5+Vva2WwVvH" + "2JkWvYm5WaANg4Q6bRxu9vz7DuhbJjQdZbxFezIAgrJdSe92B00jO/0Kny1WjiVO" + "6DA=");
            result = Hex.Decode("30820155020100300d06092a864886f70d01010105000482013f3082013b020100024100" + "debbfc2c09d61bada2a9462f24224e54cc6b3cc0755f15ce318ef57e79df17026b6a85cc" + "a12428027245045df2052a329a2f9ad3d17b78a10572ad9b22bf343b020301000102402d" + "90a96adcec472743527bc023153d8f0d6e96b40c8ed228276d467d843306429f8670559b" + "f376dd41857f6397c2fc8d95e0e53ed62de420b855430ee4a1b8a1022100ffcaf0838239" + "31e073ff534f06a5d415b3d414bc614a4544a3dff7ed271817eb022100deea30242117db" + "2d3b8837f58f1da530ff83cf9283680da33683ec4e583610f1022100e6026381adb0a683" + "f16a8f4c096b462979b9e4277cc89f3ed8a905b46fa9ff9f02210097c146d4d1d2b3dbaf" + "53a504ff51674c5c271800de84d003f4f10ac6ab36e38102202bfa141f10bda874e1017d" + "845e82767c1c38e82745daf421f0c8cd09d7652387");
        }
    }
}
