using System;

using NUnit.Framework;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Math.Tests
{
    [TestFixture]
    public class PrimesTest
    {
        private const int ITERATIONS = 10;
        private const int PRIME_BITS = 256;
        private const int PRIME_CERTAINTY = 100;

        private static readonly BigInteger Two = BigInteger.Two;

        private static readonly SecureRandom R = new SecureRandom();

        [Test]
        public void TestHasAnySmallFactors()
        {
            for (int iterations = 0; iterations < ITERATIONS; ++iterations)
            {
                BigInteger prime = RandomPrime();
                Assert.False(Primes.HasAnySmallFactors(prime));

                // NOTE: Loop through ALL small values to be sure no small primes are missing
                for (int smallFactor = 2; smallFactor <= Primes.SmallFactorLimit; ++smallFactor)
                {
                    BigInteger nonPrimeWithSmallFactor = BigInteger.ValueOf(smallFactor).Multiply(prime);
                    Assert.True(Primes.HasAnySmallFactors(nonPrimeWithSmallFactor));
                }
            }
        }

        [Test]
        public void TestEnhancedMRProbablePrime()
        {
            int mrIterations = (PRIME_CERTAINTY + 1) / 2;
            for (int iterations = 0; iterations < ITERATIONS; ++iterations)
            {
                BigInteger prime = RandomPrime();
                Primes.MROutput mr = Primes.EnhancedMRProbablePrimeTest(prime, R, mrIterations);
                Assert.False(mr.IsProvablyComposite);
                Assert.False(mr.IsNotPrimePower);
                Assert.Null(mr.Factor);

                BigInteger primePower = prime;
                for (int i = 0; i <= (iterations % 8); ++i)
                {
                    primePower = primePower.Multiply(prime);
                }

                Primes.MROutput mr2 = Primes.EnhancedMRProbablePrimeTest(primePower, R, mrIterations);
                Assert.True(mr2.IsProvablyComposite);
                Assert.False(mr2.IsNotPrimePower);
                Assert.AreEqual(mr2.Factor, prime);

                BigInteger nonPrimePower = RandomPrime().Multiply(prime);
                Primes.MROutput mr3 = Primes.EnhancedMRProbablePrimeTest(nonPrimePower, R, mrIterations);
                Assert.True(mr3.IsProvablyComposite);
                Assert.True(mr3.IsNotPrimePower);
                Assert.Null(mr.Factor);
            }
        }

        [Test]
        public void TestMRProbablePrime()
        {
            int mrIterations = (PRIME_CERTAINTY + 1) / 2;
            for (int iterations = 0; iterations < ITERATIONS; ++iterations)
            {
                BigInteger prime = RandomPrime();
                Assert.True(Primes.IsMRProbablePrime(prime, R, mrIterations));

                BigInteger nonPrime = RandomPrime().Multiply(prime);
                Assert.False(Primes.IsMRProbablePrime(nonPrime, R, mrIterations));
            }
        }

        [Test]
        public void TestMRProbablePrimeToBase()
        {
            int mrIterations = (PRIME_CERTAINTY + 1) / 2;
            for (int iterations = 0; iterations < ITERATIONS; ++iterations)
            {
                BigInteger prime = RandomPrime();
                Assert.True(ReferenceIsMRProbablePrime(prime, mrIterations));

                BigInteger nonPrime = RandomPrime().Multiply(prime);
                Assert.False(ReferenceIsMRProbablePrime(nonPrime, mrIterations));
            }
        }

        [Test]
        public void TestSTRandomPrime()
        {
            IDigest[] digests = new IDigest[] { new Sha1Digest(), new Sha256Digest() };
            for (int digestIndex = 0; digestIndex < digests.Length; ++digestIndex)
            {
                int coincidenceCount = 0;

                IDigest digest = digests[digestIndex];
                for (int iterations = 0; iterations < ITERATIONS; ++iterations)
                {
                    try
                    {
                        byte[] inputSeed = new byte[16];
                        R.NextBytes(inputSeed);
    
                        Primes.STOutput st = Primes.GenerateSTRandomPrime(digest, PRIME_BITS, inputSeed);
                        Assert.True(IsPrime(st.Prime));

                        Primes.STOutput st2 = Primes.GenerateSTRandomPrime(digest, PRIME_BITS, inputSeed);
                        Assert.AreEqual(st.Prime, st2.Prime);
                        Assert.AreEqual(st.PrimeGenCounter, st2.PrimeGenCounter);
                        Assert.True(Arrays.AreEqual(st.PrimeSeed, st2.PrimeSeed));

                        for (int i = 0; i < inputSeed.Length; ++i)
                        {
                            inputSeed[i] ^= 0xFF;
                        }

                        Primes.STOutput st3 = Primes.GenerateSTRandomPrime(digest, PRIME_BITS, inputSeed);
                        Assert.True(!st.Prime.Equals(st3.Prime));
                        Assert.False(Arrays.AreEqual(st.PrimeSeed, st3.PrimeSeed));

                        if (st.PrimeGenCounter == st3.PrimeGenCounter)
                        {
                            ++coincidenceCount;
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        if (e.Message.StartsWith("Too many iterations"))
                        {
                            --iterations;
                            continue;
                        }

                        throw e;
                    }
                }

                Assert.True(coincidenceCount * coincidenceCount < ITERATIONS);
            }
        }

        private static bool ReferenceIsMRProbablePrime(BigInteger x, int numBases)
        {
            BigInteger xSubTwo = x.Subtract(Two);

            for (int i = 0; i < numBases; ++i)
            {
                BigInteger b = BigIntegers.CreateRandomInRange(Two, xSubTwo, R);
                if (!Primes.IsMRProbablePrimeToBase(x, b))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsPrime(BigInteger x)
        {
            return x.IsProbablePrime(PRIME_CERTAINTY);
        }

        private static BigInteger RandomPrime()
        {
            return new BigInteger(PRIME_BITS, PRIME_CERTAINTY, R);
        }
    }
}
