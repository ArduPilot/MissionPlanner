using System;
using System.Text;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Security.Tests
{
    [TestFixture]
    public class SecureRandomTest
    {
#if !(NETCF_1_0 || PORTABLE)
        [Test]
        public void TestCryptoApi()
        {
            SecureRandom random = new SecureRandom(
                new CryptoApiRandomGenerator());

            CheckSecureRandom(random);
        }
#endif

        [Test]
        public void TestDefault()
        {
            SecureRandom random = new SecureRandom();

            CheckSecureRandom(random);
        }

        [Test]
        public void TestSha1Prng()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA1PRNG");

            CheckSecureRandom(random);
        }

        [Test]
        public void TestSha1PrngBackward()
        {
            byte[] seed = Encoding.ASCII.GetBytes("backward compatible");

            SecureRandom sx = new SecureRandom(seed);
            SecureRandom sy = SecureRandom.GetInstance("SHA1PRNG", false); sy.SetSeed(seed);

            byte[] bx = new byte[128]; sx.NextBytes(bx);
            byte[] by = new byte[128]; sy.NextBytes(by);

            Assert.IsTrue(Arrays.AreEqual(bx, by));
        }

        [Test]
        public void TestSha256Prng()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA256PRNG");

            CheckSecureRandom(random);
        }

        [Test]
        public void TestSP800Ctr()
        {
            SecureRandom random = new SP800SecureRandomBuilder().BuildCtr(new AesEngine(), 256, new byte[32], false);

            CheckSecureRandom(random);
        }

        [Test]
        public void TestSP800Hash()
        {
            SecureRandom random = new SP800SecureRandomBuilder().BuildHash(new Sha256Digest(), new byte[32], false);

            CheckSecureRandom(random);
        }

        [Test]
        public void TestSP800HMac()
        {
            SecureRandom random = new SP800SecureRandomBuilder().BuildHMac(new HMac(new Sha256Digest()), new byte[32], false);

            CheckSecureRandom(random);
        }

        [Test]
        public void TestThreadedSeed()
        {
            SecureRandom random = SecureRandom.GetInstance("SHA1PRNG", false);
            random.SetSeed(new ThreadedSeedGenerator().GenerateSeed(20, false));

            CheckSecureRandom(random);
        }

        [Test]
        public void TestVmpcPrng()
        {
            SecureRandom random = new SecureRandom(new VmpcRandomGenerator());
            random.SetSeed(random.GenerateSeed(32));

            CheckSecureRandom(random);
        }

        [Test]
        public void TestX931()
        {
            SecureRandom random = new X931SecureRandomBuilder().Build(new AesEngine(), new KeyParameter(new byte[16]), false);

            CheckSecureRandom(random);
        }


        private static void CheckSecureRandom(SecureRandom random)
        {
            // Note: This will periodically (< 1e-6 probability) give a false alarm.
            // That's randomness for you!
            Assert.IsTrue(RunChiSquaredTests(random), "Chi2 test detected possible non-randomness");
        }

        private static bool RunChiSquaredTests(SecureRandom random)
        {
            int passes = 0;

            for (int tries = 0; tries < 100; ++tries)
            {
                double chi2 = MeasureChiSquared(random, 1000);

                // 255 degrees of freedom in test => Q ~ 10.0% for 285
                if (chi2 < 285.0)
                {
                    ++passes;
                }
            }

            return passes > 75;
        }

        private static double MeasureChiSquared(SecureRandom random, int rounds)
        {
            byte[] opts = random.GenerateSeed(2);
            int[] counts = new int[256];

            byte[] bs = new byte[256];
            for (int i = 0; i < rounds; ++i)
            {
                random.NextBytes(bs);

                for (int b = 0; b < 256; ++b)
                {
                    ++counts[bs[b]];
                }
            }

            byte mask = opts[0];
            for (int i = 0; i < rounds; ++i)
            {
                random.NextBytes(bs);

                for (int b = 0; b < 256; ++b)
                {
                    ++counts[bs[b] ^ mask];
                }

                ++mask;
            }

            byte shift = opts[1];
            for (int i = 0; i < rounds; ++i)
            {
                random.NextBytes(bs);

                for (int b = 0; b < 256; ++b)
                {
                    ++counts[(byte)(bs[b] + shift)];
                }

                ++shift;
            }

            int total = 3 * rounds;

            double chi2 = 0;
            for (int k = 0; k < counts.Length; ++k)
            {
                double diff = ((double) counts[k]) - total;
                double diff2 = diff * diff;

                chi2 += diff2;
            }

            chi2 /= total;

            return chi2;
        }
    }
}
