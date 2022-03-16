using System;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Rfc7748.Tests
{
    [TestFixture]
    public class X448Test
    {
        private static readonly SecureRandom Random = new SecureRandom();

        [SetUp]
        public void SetUp()
        {
            X448.Precompute();
        }

        [Test]
        public void TestConsistency()
        {
            byte[] u = new byte[56];    u[0] = 5;
            byte[] k = new byte[56];
            byte[] rF = new byte[56];
            byte[] rV = new byte[56];

            for (int i = 1; i <= 100; ++i)
            {
                Random.NextBytes(k);
                X448.ScalarMultBase(k, 0, rF, 0);
                X448.ScalarMult(k, 0, u, 0, rV, 0);
                Assert.IsTrue(Arrays.AreEqual(rF, rV), "Consistency #" + i);
            }
        }

        [Test]
        public void TestECDH()
        {
            byte[] kA = new byte[56];
            byte[] kB = new byte[56];
            byte[] qA = new byte[56];
            byte[] qB = new byte[56];
            byte[] sA = new byte[56];
            byte[] sB = new byte[56];

            for (int i = 1; i <= 100; ++i)
            {
                // Each party generates an ephemeral private key, ...
                Random.NextBytes(kA);
                Random.NextBytes(kB);

                // ... publishes their public key, ...
                X448.ScalarMultBase(kA, 0, qA, 0);
                X448.ScalarMultBase(kB, 0, qB, 0);

                // ... computes the shared secret, ...
                X448.ScalarMult(kA, 0, qB, 0, sA, 0);
                X448.ScalarMult(kB, 0, qA, 0, sB, 0);

                // ... which is the same for both parties.
                Assert.IsTrue(Arrays.AreEqual(sA, sB), "ECDH #" + i);
            }
        }

        [Test]
        public void TestECDHVector1()
        {
            CheckECDHVector(
                "9a8f4925d1519f5775cf46b04b5800d4ee9ee8bae8bc5565d498c28dd9c9baf574a9419744897391006382a6f127ab1d9ac2d8c0a598726b",
                "9b08f7cc31b7e3e67d22d5aea121074a273bd2b83de09c63faa73d2c22c5d9bbc836647241d953d40c5b12da88120d53177f80e532c41fa0",
                "1c306a7ac2a0e2e0990b294470cba339e6453772b075811d8fad0d1d6927c120bb5ee8972b0d3e21374c9c921b09d1b0366f10b65173992d",
                "3eb7a829b0cd20f5bcfc0b599b6feccf6da4627107bdb0d4f345b43027d8b972fc3e34fb4232a13ca706dcb57aec3dae07bdc1c67bf33609",
                "07fff4181ac6cc95ec1c16a94a0f74d12da232ce40a77552281d282bb60c0b56fd2464c335543936521c24403085d59a449a5037514a879d",
                "ECDH Vector #1");
        }

        [Test]
        public void TestX448Iterated()
        {
            CheckIterated(1000);
        }

        //[Test, Explicit]
        //public void TestX448IteratedFull()
        //{
        //    CheckIterated(1000000);
        //}

        [Test]
        public void TestX448Vector1()
        {
            CheckX448Vector(
                "3d262fddf9ec8e88495266fea19a34d28882acef045104d0d1aae121700a779c984c24f8cdd78fbff44943eba368f54b29259a4f1c600ad3",
                "06fce640fa3487bfda5f6cf2d5263f8aad88334cbd07437f020f08f9814dc031ddbdc38c19c6da2583fa5429db94ada18aa7a7fb4ef8a086",
                "ce3e4ff95a60dc6697da1db1d85e6afbdf79b50a2412d7546d5f239fe14fbaadeb445fc66a01b0779d98223961111e21766282f73dd96b6f",
                "Vector #1");
        }

        [Test]
        public void TestX448Vector2()
        {
            CheckX448Vector(
                "203d494428b8399352665ddca42f9de8fef600908e0d461cb021f8c538345dd77c3e4806e25f46d3315c44e0a5b4371282dd2c8d5be3095f",
                "0fbcc2f993cd56d3305b0b7d9e55d4c1a8fb5dbb52f8e9a1e9b6201b165d015894e56c4d3570bee52fe205e28a78b91cdfbde71ce8d157db",
                "884a02576239ff7a2f2f63b2db6a9ff37047ac13568e1e30fe63c4a7ad1b3ee3a5700df34321d62077e63633c575c1c954514e99da7c179d",
                "Vector #2");
        }

        private static void CheckECDHVector(string sA, string sAPub, string sB, string sBPub, string sK, string text)
        {
            byte[] a = Hex.Decode(sA);
            byte[] b = Hex.Decode(sB);

            byte[] aPub = new byte[56];
            X448.ScalarMultBase(a, 0, aPub, 0);
            CheckValue(aPub, text, sAPub);

            byte[] bPub = new byte[56];
            X448.ScalarMultBase(b, 0, bPub, 0);
            CheckValue(bPub, text, sBPub);

            byte[] aK = new byte[56];
            X448.ScalarMult(a, 0, bPub, 0, aK, 0);
            CheckValue(aK, text, sK);

            byte[] bK = new byte[56];
            X448.ScalarMult(b, 0, aPub, 0, bK, 0);
            CheckValue(bK, text, sK);
        }

        private static void CheckIterated(int count)
        {
            byte[] k = new byte[56]; k[0] = 5;
            byte[] u = new byte[56]; u[0] = 5;
            byte[] r = new byte[56];

            int iterations = 0;
            while (iterations < count)
            {
                X448.ScalarMult(k, 0, u, 0, r, 0);

                Array.Copy(k, 0, u, 0, 56);
                Array.Copy(r, 0, k, 0, 56);

                switch (++iterations)
                {
                case 1:
                    CheckValue(k, "Iterated @1",
                        "3f482c8a9f19b01e6c46ee9711d9dc14fd4bf67af30765c2ae2b846a4d23a8cd0db897086239492caf350b51f833868b9bc2b3bca9cf4113");
                    break;
                case 1000:
                    CheckValue(k, "Iterated @1000",
                        "aa3b4749d55b9daf1e5b00288826c467274ce3ebbdd5c17b975e09d4af6c67cf10d087202db88286e2b79fceea3ec353ef54faa26e219f38");
                    break;
                case 1000000:
                    CheckValue(k, "Iterated @1000000",
                        "077f453681caca3693198420bbe515cae0002472519b3e67661a7e89cab94695c8f4bcd66e61b9b9c946da8d524de3d69bd9d9d66b997e37");
                    break;
                default:
                    break;
                }
            }
        }

        private static void CheckValue(byte[] n, String text, String se)
        {
            byte[] e = Hex.Decode(se);
            Assert.IsTrue(Arrays.AreEqual(e, n), text);
        }

        private static void CheckX448Vector(String sk, String su, String se, String text)
        {
            byte[] k = Hex.Decode(sk);
            byte[] u = Hex.Decode(su);
            byte[] r = new byte[56];
            X448.ScalarMult(k, 0, u, 0, r, 0);
            CheckValue(r, text, se);
        }
    }
}
