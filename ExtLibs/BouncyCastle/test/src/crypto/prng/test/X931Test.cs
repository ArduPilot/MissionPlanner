using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Utilities.Test;

namespace Org.BouncyCastle.Crypto.Prng.Test
{
    /**
     * HMAC SP800-90 DRBG
     */
    [TestFixture]
    public class X931Test
        :   SimpleTest
    {
        public override string Name
        {
            get { return "X931"; }
        }

        public static void Main(string[] args)
        {
            RunTest(new X931Test());
        }

        [Test]
        public void TestFunction()
        {
            string resultText = Perform().ToString();

            Assert.AreEqual(Name + ": Okay", resultText);
        }

        private X931TestVector[] CreateTestVectorData()
        {
            return new X931TestVector[]
            {
                new X931TestVector(
                    new AesEngine(),
                    new Aes128EntropyProvider(),
                    "f7d36762b9915f1ed585eb8e91700eb2",
                    "259e67249288597a4d61e7c0e690afae",
                    false,
                    new string[]
                    {
                        "15f013af5a8e9df9a8e37500edaeac43",
                        "a9d74bb1c90a222adc398546d64879cf",
                        "0379e404042d58180764fb9e6c5d94bb",
                        "3c74603e036d28c79947ffb56fee4e51",
                        "e872101a4df81ebbe1e632fc87195d52",
                        "26a6b3d33b8e7e68b75d9630ec036314"
                    }),
                new X931TestVector(
                    new DesEdeEngine(),
                    new TDesEntropyProvider(),
                    "ef16ec643e5db5892cbc6eabba310b3410e6f8759e3e382c",
                    "55df103deaf68dc4",
                    false,
                    new string[]
                    {
                        "9c960bb9662ce6de",
                        "d9d0e527fd0931da",
                        "3e2db9994e9e6995",
                        "0e3868aef8218cf7",
                        "7b0b0ca137f8fd81",
                        "f657df270ad12265"
                    })
            };
        }

        public override void PerformTest()
        {
            X931TestVector[] vectors = CreateTestVectorData();

            for (int i = 0; i != vectors.Length; i++)
            {
                X931TestVector tv = vectors[i];
                X931SecureRandomBuilder bld = new X931SecureRandomBuilder(tv.EntropyProvider);

                bld.SetDateTimeVector(Hex.Decode(tv.DateTimeVector));

                SecureRandom rand = bld.Build(tv.Engine, new KeyParameter(Hex.Decode(tv.Key)), tv.IsPredictionResistant);

                for (int j = 0; j != tv.Expected.Length - 1; j++)
                {
                    byte[] expected = Hex.Decode(tv.Expected[j]);
                    byte[] res = new byte[expected.Length];

                    rand.NextBytes(res);

                    if (!Arrays.AreEqual(expected, res))
                    {
                        Fail("expected output wrong [" + j + "] got : " + Strings.FromByteArray(Hex.Encode(res)));
                    }
                }

                {
                    byte[] expected = Hex.Decode(tv.Expected[tv.Expected.Length - 1]);
                    byte[] res = new byte[expected.Length];

                    for (int j = tv.Expected.Length - 1; j != 10000; j++)
                    {
                        rand.NextBytes(res);
                    }

                    if (!Arrays.AreEqual(expected, res))
                    {
                        Fail("expected output wrong [" + 10000 + "] got : " + Strings.FromByteArray(Hex.Encode(res)));
                    }
                }
            }
        }

        private class Aes128EntropyProvider
            :   TestEntropySourceProvider
        {
            internal Aes128EntropyProvider()
                : base(Hex.Decode("35cc0ea481fc8a4f5f05c7d4667233b2"), true)
            {
            }
        }

        private class TDesEntropyProvider
            :   TestEntropySourceProvider
        {
            internal TDesEntropyProvider()
                : base(Hex.Decode("96d872b9122c5e74"), true)
            {
            }
        }
    }
}
