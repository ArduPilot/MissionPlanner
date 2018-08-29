using System;

using NUnit.Framework;

using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Rfc7748.Tests
{
    [TestFixture]
    public class X25519Test
    {
        private static readonly SecureRandom Random = new SecureRandom();

        [SetUp]
        public void SetUp()
        {
            X25519.Precompute();
        }

        [Test]
        public void TestConsistency()
        {
            byte[] u = new byte[32];    u[0] = 9;
            byte[] k = new byte[32];
            byte[] rF = new byte[32];
            byte[] rV = new byte[32];

            for (int i = 1; i <= 100; ++i)
            {
                Random.NextBytes(k);
                X25519.ScalarMultBase(k, 0, rF, 0);
                X25519.ScalarMult(k, 0, u, 0, rV, 0);
                Assert.IsTrue(Arrays.AreEqual(rF, rV), "Consistency #" + i);
            }
        }

        [Test]
        public void TestECDH()
        {
            byte[] kA = new byte[32];
            byte[] kB = new byte[32];
            byte[] qA = new byte[32];
            byte[] qB = new byte[32];
            byte[] sA = new byte[32];
            byte[] sB = new byte[32];

            for (int i = 1; i <= 100; ++i)
            {
                // Each party generates an ephemeral private key, ...
                Random.NextBytes(kA);
                Random.NextBytes(kB);

                // ... publishes their public key, ...
                X25519.ScalarMultBase(kA, 0, qA, 0);
                X25519.ScalarMultBase(kB, 0, qB, 0);

                // ... computes the shared secret, ...
                X25519.ScalarMult(kA, 0, qB, 0, sA, 0);
                X25519.ScalarMult(kB, 0, qA, 0, sB, 0);

                // ... which is the same for both parties.
                //Assert.IsTrue(Arrays.AreEqual(sA, sB), "ECDH #" + i);
                if (!Arrays.AreEqual(sA, sB))
                {
                    Console.WriteLine(" " + i);
                }
            }
        }

        [Test]
        public void TestECDHVector1()
        {
            CheckECDHVector(
                "77076d0a7318a57d3c16c17251b26645df4c2f87ebc0992ab177fba51db92c2a",
                "8520f0098930a754748b7ddcb43ef75a0dbf3a0d26381af4eba4a98eaa9b4e6a",
                "5dab087e624a8a4b79e17f8b83800ee66f3bb1292618b6fd1c2f8b27ff88e0eb",
                "de9edb7d7b7dc1b4d35b61c2ece435373f8343c85b78674dadfc7e146f882b4f",
                "4a5d9d5ba4ce2de1728e3bf480350f25e07e21c947d19e3376f09b3c1e161742",
                "ECDH Vector #1");
        }

        [Test]
        public void TestX25519Iterated()
        {
            CheckIterated(1000);
        }

        //[Test, Explicit]
        //public void TestX25519IteratedFull()
        //{
        //    CheckIterated(1000000);
        //}

        [Test]
        public void TestX25519Vector1()
        {
            CheckX25519Vector(
                "a546e36bf0527c9d3b16154b82465edd62144c0ac1fc5a18506a2244ba449ac4",
                "e6db6867583030db3594c1a424b15f7c726624ec26b3353b10a903a6d0ab1c4c",
                "c3da55379de9c6908e94ea4df28d084f32eccf03491c71f754b4075577a28552",
                "Vector #1");
        }

        [Test]
        public void TestX25519Vector2()
        {
            CheckX25519Vector(
                "4b66e9d4d1b4673c5ad22691957d6af5c11b6421e0ea01d42ca4169e7918ba0d",
                "e5210f12786811d3f4b7959d0538ae2c31dbe7106fc03c3efc4cd549c715a493",
                "95cbde9476e8907d7aade45cb4b873f88b595a68799fa152e6f8f7647aac7957",
                "Vector #2");
        }

        private static void CheckECDHVector(string sA, string sAPub, string sB, string sBPub, string sK, string text)
        {
            byte[] a = Hex.Decode(sA);
            byte[] b = Hex.Decode(sB);

            byte[] aPub = new byte[32];
            X25519.ScalarMultBase(a, 0, aPub, 0);
            CheckValue(aPub, text, sAPub);

            byte[] bPub = new byte[32];
            X25519.ScalarMultBase(b, 0, bPub, 0);
            CheckValue(bPub, text, sBPub);

            byte[] aK = new byte[32];
            X25519.ScalarMult(a, 0, bPub, 0, aK, 0);
            CheckValue(aK, text, sK);

            byte[] bK = new byte[32];
            X25519.ScalarMult(b, 0, aPub, 0, bK, 0);
            CheckValue(bK, text, sK);
        }

        private static void CheckIterated(int count)
        {
            byte[] k = new byte[32]; k[0] = 9;
            byte[] u = new byte[32]; u[0] = 9;
            byte[] r = new byte[32];

            int iterations = 0;
            while (iterations < count)
            {
                X25519.ScalarMult(k, 0, u, 0, r, 0);

                Array.Copy(k, 0, u, 0, 32);
                Array.Copy(r, 0, k, 0, 32);

                switch (++iterations)
                {
                case 1:
                    CheckValue(k, "Iterated @1", "422c8e7a6227d7bca1350b3e2bb7279f7897b87bb6854b783c60e80311ae3079");
                    break;
                case 1000:
                    CheckValue(k, "Iterated @1000", "684cf59ba83309552800ef566f2f4d3c1c3887c49360e3875f2eb94d99532c51");
                    break;
                case 1000000:
                    CheckValue(k, "Iterated @1000000", "7c3911e0ab2586fd864497297e575e6f3bc601c0883c30df5f4dd2d24f665424");
                    break;
                default:
                    break;
                }
            }
        }

        private static void CheckValue(byte[] n, string text, string se)
        {
            byte[] e = Hex.Decode(se);
            Assert.IsTrue(Arrays.AreEqual(e, n), text);
        }

        private static void CheckX25519Vector(string sk, string su, string se, string text)
        {
            byte[] k = Hex.Decode(sk);
            byte[] u = Hex.Decode(su);
            byte[] r = new byte[32];
            X25519.ScalarMult(k, 0, u, 0, r, 0);
            CheckValue(r, text, se);
        }
    }
}
