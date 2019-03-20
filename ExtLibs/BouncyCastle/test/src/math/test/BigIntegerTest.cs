using System;

using NUnit.Framework;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.Tests
{
    [TestFixture]
    public class BigIntegerTest
    {
        private static Random random = new Random();

        [Test]
        public void MonoBug81857()
        {
            BigInteger b = new BigInteger("18446744073709551616");
            BigInteger exp = BigInteger.Two;
            BigInteger mod = new BigInteger("48112959837082048697");
            BigInteger expected = new BigInteger("4970597831480284165");

            BigInteger manual = b.Multiply(b).Mod(mod);
            Assert.AreEqual(expected, manual, "b * b % mod");
        }

        [Test]
        public void TestAbs()
        {
            Assert.AreEqual(zero, zero.Abs());

            Assert.AreEqual(one, one.Abs());
            Assert.AreEqual(one, minusOne.Abs());

            Assert.AreEqual(two, two.Abs());
            Assert.AreEqual(two, minusTwo.Abs());
        }

        [Test]
        public void TestAdd()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i + j),
                        val(i).Add(val(j)),
                        "Problem: " + i + ".Add(" + j + ") should be " + (i + j));
                }
            }
        }

        [Test]
        public void TestAnd()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i & j),
                        val(i).And(val(j)),
                        "Problem: " + i + " AND " + j + " should be " + (i & j));
                }
            }
        }

        [Test]
        public void TestAndNot()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i & ~j),
                        val(i).AndNot(val(j)),
                        "Problem: " + i + " AND NOT " + j + " should be " + (i & ~j));
                }
            }
        }

        [Test]
        public void TestBitCount()
        {
            Assert.AreEqual(0, zero.BitCount);
            Assert.AreEqual(1, one.BitCount);
            Assert.AreEqual(0, minusOne.BitCount);
            Assert.AreEqual(1, two.BitCount);
            Assert.AreEqual(1, minusTwo.BitCount);

            for (int i = 0; i < 100; ++i)
            {
                BigInteger pow2 = one.ShiftLeft(i);

                Assert.AreEqual(1, pow2.BitCount);
                Assert.AreEqual(i, pow2.Negate().BitCount);
            }

            for (int i = 0; i < 10; ++i)
            {
                BigInteger test = new BigInteger(128, 0, random);
                int bitCount = 0;

                for (int bit = 0; bit < test.BitLength; ++bit)
                {
                    if (test.TestBit(bit))
                    {
                        ++bitCount;
                    }
                }

                Assert.AreEqual(bitCount, test.BitCount);
            }
        }

        [Test]
        public void TestBitLength()
        {
            Assert.AreEqual(0, zero.BitLength);
            Assert.AreEqual(1, one.BitLength);
            Assert.AreEqual(0, minusOne.BitLength);
            Assert.AreEqual(2, two.BitLength);
            Assert.AreEqual(1, minusTwo.BitLength);

            for (int i = 0; i < 100; ++i)
            {
                int bit = i + random.Next(64);
                BigInteger odd = new BigInteger(bit, random).SetBit(bit + 1).SetBit(0);
                BigInteger pow2 = one.ShiftLeft(bit);

                Assert.AreEqual(bit + 2, odd.BitLength);
                Assert.AreEqual(bit + 2, odd.Negate().BitLength);
                Assert.AreEqual(bit + 1, pow2.BitLength);
                Assert.AreEqual(bit, pow2.Negate().BitLength);
            }
        }

        [Test]
        public void TestClearBit()
        {
            Assert.AreEqual(zero, zero.ClearBit(0));
            Assert.AreEqual(zero, one.ClearBit(0));
            Assert.AreEqual(two, two.ClearBit(0));

            Assert.AreEqual(zero, zero.ClearBit(1));
            Assert.AreEqual(one, one.ClearBit(1));
            Assert.AreEqual(zero, two.ClearBit(1));

            // TODO Tests for clearing bits in negative numbers

            // TODO Tests for clearing extended bits

            for (int i = 0; i < 10; ++i)
            {
                BigInteger n = new BigInteger(128, random);

                for (int j = 0; j < 10; ++j)
                {
                    int pos = random.Next(128);
                    BigInteger m = n.ClearBit(pos);
                    bool test = m.ShiftRight(pos).Remainder(two).Equals(one);

                    Assert.IsFalse(test);
                }
            }

            for (int i = 0; i < 100; ++i)
            {
                BigInteger pow2 = one.ShiftLeft(i);
                BigInteger minusPow2 = pow2.Negate();

                Assert.AreEqual(zero, pow2.ClearBit(i));
                Assert.AreEqual(minusPow2.ShiftLeft(1), minusPow2.ClearBit(i));

                BigInteger bigI = BigInteger.ValueOf(i);
                BigInteger negI = bigI.Negate();

                for (int j = 0; j < 10; ++j)
                {
                    string data = "i=" + i + ", j=" + j;
                    Assert.AreEqual(bigI.AndNot(one.ShiftLeft(j)), bigI.ClearBit(j), data);
                    Assert.AreEqual(negI.AndNot(one.ShiftLeft(j)), negI.ClearBit(j), data);
                }
            }
        }

        [Test]
        public void TestCompareTo()
        {
            Assert.AreEqual(0, minusTwo.CompareTo(minusTwo));
            Assert.AreEqual(-1, minusTwo.CompareTo(minusOne));
            Assert.AreEqual(-1, minusTwo.CompareTo(zero));
            Assert.AreEqual(-1, minusTwo.CompareTo(one));
            Assert.AreEqual(-1, minusTwo.CompareTo(two));

            Assert.AreEqual(1, minusOne.CompareTo(minusTwo));
            Assert.AreEqual(0, minusOne.CompareTo(minusOne));
            Assert.AreEqual(-1, minusOne.CompareTo(zero));
            Assert.AreEqual(-1, minusOne.CompareTo(one));
            Assert.AreEqual(-1, minusOne.CompareTo(two));

            Assert.AreEqual(1, zero.CompareTo(minusTwo));
            Assert.AreEqual(1, zero.CompareTo(minusOne));
            Assert.AreEqual(0, zero.CompareTo(zero));
            Assert.AreEqual(-1, zero.CompareTo(one));
            Assert.AreEqual(-1, zero.CompareTo(two));

            Assert.AreEqual(1, one.CompareTo(minusTwo));
            Assert.AreEqual(1, one.CompareTo(minusOne));
            Assert.AreEqual(1, one.CompareTo(zero));
            Assert.AreEqual(0, one.CompareTo(one));
            Assert.AreEqual(-1, one.CompareTo(two));

            Assert.AreEqual(1, two.CompareTo(minusTwo));
            Assert.AreEqual(1, two.CompareTo(minusOne));
            Assert.AreEqual(1, two.CompareTo(zero));
            Assert.AreEqual(1, two.CompareTo(one));
            Assert.AreEqual(0, two.CompareTo(two));
        }

        [Test]
        public void TestConstructors()
        {
            Assert.AreEqual(BigInteger.Zero, new BigInteger(new byte[]{ 0 }));
            Assert.AreEqual(BigInteger.Zero, new BigInteger(new byte[]{ 0, 0 }));

            for (int i = 0; i < 10; ++i)
            {
                Assert.IsTrue(new BigInteger(i + 3, 0, random).TestBit(0));
            }

            // TODO Other constructors
        }

        [Test]
        public void TestDivide()
        {
            for (int i = -5; i <= 5; ++i)
            {
                try
                {
                    val(i).Divide(zero);
                    Assert.Fail("expected ArithmeticException");
                }
                catch (ArithmeticException) {}
            }

            int product = 1 * 2 * 3 * 4 * 5 * 6 * 7 * 8 * 9;
            int productPlus = product + 1;

            BigInteger bigProduct = val(product);
            BigInteger bigProductPlus = val(productPlus);

            for (int divisor = 1; divisor < 10; ++divisor)
            {
                // Exact division
                BigInteger expected = val(product / divisor);

                Assert.AreEqual(expected, bigProduct.Divide(val(divisor)));
                Assert.AreEqual(expected.Negate(), bigProduct.Negate().Divide(val(divisor)));
                Assert.AreEqual(expected.Negate(), bigProduct.Divide(val(divisor).Negate()));
                Assert.AreEqual(expected, bigProduct.Negate().Divide(val(divisor).Negate()));

                expected = val((product + 1)/divisor);

                Assert.AreEqual(expected, bigProductPlus.Divide(val(divisor)));
                Assert.AreEqual(expected.Negate(), bigProductPlus.Negate().Divide(val(divisor)));
                Assert.AreEqual(expected.Negate(), bigProductPlus.Divide(val(divisor).Negate()));
                Assert.AreEqual(expected, bigProductPlus.Negate().Divide(val(divisor).Negate()));
            }

            for (int rep = 0; rep < 10; ++rep)
            {
                BigInteger a = new BigInteger(100 - rep, 0, random);
                BigInteger b = new BigInteger(100 + rep, 0, random);
                BigInteger c = new BigInteger(10 + rep, 0, random);
                BigInteger d = a.Multiply(b).Add(c);
                BigInteger e = d.Divide(a);

                Assert.AreEqual(b, e);
            }

            // Special tests for power of two since uses different code path internally
            for (int i = 0; i < 100; ++i)
            {
                int shift = random.Next(64);
                BigInteger a = one.ShiftLeft(shift);
                BigInteger b = new BigInteger(64 + random.Next(64), random);
                BigInteger bShift = b.ShiftRight(shift);

                string data = "shift=" + shift +", b=" + b.ToString(16);

                Assert.AreEqual(bShift, b.Divide(a), data);
                Assert.AreEqual(bShift.Negate(), b.Divide(a.Negate()), data);
                Assert.AreEqual(bShift.Negate(), b.Negate().Divide(a), data);
                Assert.AreEqual(bShift, b.Negate().Divide(a.Negate()), data);
            }

            // Regression
            {
                int shift = 63;
                BigInteger a = one.ShiftLeft(shift);
                BigInteger b = new BigInteger(1, Hex.Decode("2504b470dc188499"));
                BigInteger bShift = b.ShiftRight(shift);

                string data = "shift=" + shift +", b=" + b.ToString(16);

                Assert.AreEqual(bShift, b.Divide(a), data);
                Assert.AreEqual(bShift.Negate(), b.Divide(a.Negate()), data);
//				Assert.AreEqual(bShift.Negate(), b.Negate().Divide(a), data);
                Assert.AreEqual(bShift, b.Negate().Divide(a.Negate()), data);
            }
        }

        [Test]
        public void TestDivideAndRemainder()
        {
            // TODO More basic tests

            BigInteger n = new BigInteger(48, random);
            BigInteger[] qr = n.DivideAndRemainder(n);
            Assert.AreEqual(one, qr[0]);
            Assert.AreEqual(zero, qr[1]);
            qr = n.DivideAndRemainder(one);
            Assert.AreEqual(n, qr[0]);
            Assert.AreEqual(zero, qr[1]);

            for (int rep = 0; rep < 10; ++rep)
            {
                BigInteger a = new BigInteger(100 - rep, 0, random);
                BigInteger b = new BigInteger(100 + rep, 0, random);
                BigInteger c = new BigInteger(10 + rep, 0, random);
                BigInteger d = a.Multiply(b).Add(c);
                BigInteger[] es = d.DivideAndRemainder(a);

                Assert.AreEqual(b, es[0]);
                Assert.AreEqual(c, es[1]);
            }

            // Special tests for power of two since uses different code path internally
            for (int i = 0; i < 100; ++i)
            {
                int shift = random.Next(64);
                BigInteger a = one.ShiftLeft(shift);
                BigInteger b = new BigInteger(64 + random.Next(64), random);
                BigInteger bShift = b.ShiftRight(shift);
                BigInteger bMod = b.And(a.Subtract(one));

                string data = "shift=" + shift +", b=" + b.ToString(16);

                qr = b.DivideAndRemainder(a);
                Assert.AreEqual(bShift, qr[0], data);
                Assert.AreEqual(bMod, qr[1], data);

                qr = b.DivideAndRemainder(a.Negate());
                Assert.AreEqual(bShift.Negate(), qr[0], data);
                Assert.AreEqual(bMod, qr[1], data);

                qr = b.Negate().DivideAndRemainder(a);
                Assert.AreEqual(bShift.Negate(), qr[0], data);
                Assert.AreEqual(bMod.Negate(), qr[1], data);

                qr = b.Negate().DivideAndRemainder(a.Negate());
                Assert.AreEqual(bShift, qr[0], data);
                Assert.AreEqual(bMod.Negate(), qr[1], data);
            }
        }

        [Test]
        public void TestFlipBit()
        {
            for (int i = 0; i < 10; ++i)
            {
                BigInteger a = new BigInteger(128, 0, random);
                BigInteger b = a;

                for (int x = 0; x < 100; ++x)
                {
                    // Note: Intentionally greater than initial size
                    int pos = random.Next(256);

                    a = a.FlipBit(pos);
                    b = b.TestBit(pos) ? b.ClearBit(pos) : b.SetBit(pos);
                }

                Assert.AreEqual(a, b);
            }

            for (int i = 0; i < 100; ++i)
            {
                BigInteger pow2 = one.ShiftLeft(i);
                BigInteger minusPow2 = pow2.Negate();

                Assert.AreEqual(zero, pow2.FlipBit(i));
                Assert.AreEqual(minusPow2.ShiftLeft(1), minusPow2.FlipBit(i));

                BigInteger bigI = BigInteger.ValueOf(i);
                BigInteger negI = bigI.Negate();

                for (int j = 0; j < 10; ++j)
                {
                    string data = "i=" + i + ", j=" + j;
                    Assert.AreEqual(bigI.Xor(one.ShiftLeft(j)), bigI.FlipBit(j), data);
                    Assert.AreEqual(negI.Xor(one.ShiftLeft(j)), negI.FlipBit(j), data);
                }
            }
        }

        [Test]
        public void TestGcd()
        {
            for (int i = 0; i < 10; ++i)
            {
                BigInteger fac = new BigInteger(32, random).Add(two);
                BigInteger p1 = BigInteger.ProbablePrime(63, random);
                BigInteger p2 = BigInteger.ProbablePrime(64, random);

                BigInteger gcd = fac.Multiply(p1).Gcd(fac.Multiply(p2));

                Assert.AreEqual(fac, gcd);
            }
        }

        [Test]
        public void TestGetLowestSetBit()
        {
            for (int i = 1; i <= 100; ++i)
            {
                BigInteger test = new BigInteger(i + 1, 0, random).Add(one);
                int bit1 = test.GetLowestSetBit();
                Assert.AreEqual(test, test.ShiftRight(bit1).ShiftLeft(bit1));
                int bit2 = test.ShiftLeft(i + 1).GetLowestSetBit();
                Assert.AreEqual(i + 1, bit2 - bit1);
                int bit3 = test.ShiftLeft(3 * i).GetLowestSetBit();
                Assert.AreEqual(3 * i, bit3 - bit1);
            }
        }

        [Test]
        public void TestIntValue()
        {
            int[] tests = new int[]{ int.MinValue, -1234, -10, -1, 0, ~0, 1, 10, 5678, int.MaxValue };

            foreach (int test in tests)
            {
                Assert.AreEqual(test, val(test).IntValue);
            }

            // TODO Tests for large numbers
        }

        [Test]
        public void TestIsProbablePrime()
        {
            Assert.IsFalse(zero.IsProbablePrime(100));
            Assert.IsFalse(zero.IsProbablePrime(100));
            Assert.IsTrue(zero.IsProbablePrime(0));
            Assert.IsTrue(zero.IsProbablePrime(-10));
            Assert.IsFalse(minusOne.IsProbablePrime(100));
            Assert.IsTrue(minusTwo.IsProbablePrime(100));
            Assert.IsTrue(val(-17).IsProbablePrime(100));
            Assert.IsTrue(val(67).IsProbablePrime(100));
            Assert.IsTrue(val(773).IsProbablePrime(100));

            foreach (int p in firstPrimes)
            {
                Assert.IsTrue(val(p).IsProbablePrime(100));
                Assert.IsTrue(val(-p).IsProbablePrime(100));
            }

            foreach (int c in nonPrimes)
            {
                Assert.IsFalse(val(c).IsProbablePrime(100));
                Assert.IsFalse(val(-c).IsProbablePrime(100));
            }

            foreach (int e in mersennePrimeExponents)
            {
                Assert.IsTrue(mersenne(e).IsProbablePrime(100));
                Assert.IsTrue(mersenne(e).Negate().IsProbablePrime(100));
            }

            foreach (int e in nonPrimeExponents)
            {
                Assert.IsFalse(mersenne(e).IsProbablePrime(100));
                Assert.IsFalse(mersenne(e).Negate().IsProbablePrime(100));
            }

            // TODO Other examples of 'tricky' values?
        }

        [Test]
        public void TestLongValue()
        {
            long[] tests = new long[]{ long.MinValue, -1234, -10, -1, 0L, ~0L, 1, 10, 5678, long.MaxValue };

            foreach (long test in tests)
            {
                Assert.AreEqual(test, val(test).LongValue);
            }

            // TODO Tests for large numbers
        }

        [Test]
        public void TestMax()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(val(System.Math.Max(i, j)), val(i).Max(val(j)));
                }
            }
        }

        [Test]
        public void TestMin()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(val(System.Math.Min(i, j)), val(i).Min(val(j)));
                }
            }
        }

        [Test]
        public void TestMod()
        {
            // TODO Basic tests

            for (int rep = 0; rep < 100; ++rep)
            {
                int diff = random.Next(25);
                BigInteger a = new BigInteger(100 - diff, 0, random);
                BigInteger b = new BigInteger(100 + diff, 0, random);
                BigInteger c = new BigInteger(10 + diff, 0, random);

                BigInteger d = a.Multiply(b).Add(c);
                BigInteger e = d.Mod(a);
                Assert.AreEqual(c, e);

                BigInteger pow2 = one.ShiftLeft(random.Next(128));
                Assert.AreEqual(b.And(pow2.Subtract(one)), b.Mod(pow2));
            }
        }

        [Test]
        public void TestModInverse()
        {
            for (int i = 0; i < 10; ++i)
            {
                BigInteger p = BigInteger.ProbablePrime(64, random);
                BigInteger q = new BigInteger(63, random).Add(one);
                BigInteger inv = q.ModInverse(p);
                BigInteger inv2 = inv.ModInverse(p);

                Assert.AreEqual(q, inv2);
                Assert.AreEqual(one, q.Multiply(inv).Mod(p));
            }

            // ModInverse a power of 2 for a range of powers
            for (int i = 1; i <= 128; ++i)
            {
                BigInteger m = one.ShiftLeft(i);
                BigInteger d = new BigInteger(i, random).SetBit(0);
                BigInteger x = d.ModInverse(m);
                BigInteger check = x.Multiply(d).Mod(m);

                Assert.AreEqual(one, check);
            }
        }

        [Test]
        public void TestModPow()
        {
            try
            {
                two.ModPow(one, zero);
                Assert.Fail("expected ArithmeticException");
            }
            catch (ArithmeticException) {}

            Assert.AreEqual(zero, zero.ModPow(zero, one));
            Assert.AreEqual(one, zero.ModPow(zero, two));
            Assert.AreEqual(zero, two.ModPow(one, one));
            Assert.AreEqual(one, two.ModPow(zero, two));

            for (int i = 0; i < 100; ++i)
            {
                BigInteger m = BigInteger.ProbablePrime(10 + i, random);
                BigInteger x = new BigInteger(m.BitLength - 1, random);

                Assert.AreEqual(x, x.ModPow(m, m));
                if (x.SignValue != 0)
                {
                    Assert.AreEqual(zero, zero.ModPow(x, m));
                    Assert.AreEqual(one, x.ModPow(m.Subtract(one), m));
                }

                BigInteger y = new BigInteger(m.BitLength - 1, random);
                BigInteger n = new BigInteger(m.BitLength - 1, random);
                BigInteger n3 = n.ModPow(three, m);

                BigInteger resX = n.ModPow(x, m);
                BigInteger resY = n.ModPow(y, m);
                BigInteger res = resX.Multiply(resY).Mod(m);
                BigInteger res3 = res.ModPow(three, m);

                Assert.AreEqual(res3, n3.ModPow(x.Add(y), m));

                BigInteger a = x.Add(one); // Make sure it's not zero
                BigInteger b = y.Add(one); // Make sure it's not zero

                Assert.AreEqual(a.ModPow(b, m).ModInverse(m), a.ModPow(b.Negate(), m));
            }
        }

        [Test]
        public void TestMultiply()
        {
            BigInteger one = BigInteger.One;

            Assert.AreEqual(one, one.Negate().Multiply(one.Negate()));

            for (int i = 0; i < 100; ++i)
            {
                int aLen = 64 + random.Next(64);
                int bLen = 64 + random.Next(64);

                BigInteger a = new BigInteger(aLen, random).SetBit(aLen);
                BigInteger b = new BigInteger(bLen, random).SetBit(bLen);
                BigInteger c = new BigInteger(32, random);

                BigInteger ab = a.Multiply(b);
                BigInteger bc = b.Multiply(c);

                Assert.AreEqual(ab.Add(bc), a.Add(c).Multiply(b));
                Assert.AreEqual(ab.Subtract(bc), a.Subtract(c).Multiply(b));
            }

            // Special tests for power of two since uses different code path internally
            for (int i = 0; i < 100; ++i)
            {
                int shift = random.Next(64);
                BigInteger a = one.ShiftLeft(shift);
                BigInteger b = new BigInteger(64 + random.Next(64), random);
                BigInteger bShift = b.ShiftLeft(shift);

                Assert.AreEqual(bShift, a.Multiply(b));
                Assert.AreEqual(bShift.Negate(), a.Multiply(b.Negate()));
                Assert.AreEqual(bShift.Negate(), a.Negate().Multiply(b));
                Assert.AreEqual(bShift, a.Negate().Multiply(b.Negate()));

                Assert.AreEqual(bShift, b.Multiply(a));
                Assert.AreEqual(bShift.Negate(), b.Multiply(a.Negate()));
                Assert.AreEqual(bShift.Negate(), b.Negate().Multiply(a));
                Assert.AreEqual(bShift, b.Negate().Multiply(a.Negate()));
            }
        }

        [Test]
        public void TestNegate()
        {
            for (int i = -10; i <= 10; ++i)
            {
                Assert.AreEqual(val(-i), val(i).Negate());
            }
        }

        [Test]
        public void TestNextProbablePrime()
        {
            BigInteger firstPrime = BigInteger.ProbablePrime(32, random);
            BigInteger nextPrime = firstPrime.NextProbablePrime();

            Assert.IsTrue(firstPrime.IsProbablePrime(10));
            Assert.IsTrue(nextPrime.IsProbablePrime(10));

            BigInteger check = firstPrime.Add(one);

            while (check.CompareTo(nextPrime) < 0)
            {
                Assert.IsFalse(check.IsProbablePrime(10));
                check = check.Add(one);
            }
        }

        [Test]
        public void TestNot()
        {
            for (int i = -10; i <= 10; ++i)
            {
                Assert.AreEqual(
                    val(~i),
                    val(i).Not(),
                    "Problem: ~" + i + " should be " + ~i);
            }
        }

        [Test]
        public void TestOr()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i | j),
                        val(i).Or(val(j)),
                        "Problem: " + i + " OR " + j + " should be " + (i | j));
                }
            }
        }

        [Test]
        public void TestPow()
        {
            Assert.AreEqual(one, zero.Pow(0));
            Assert.AreEqual(zero, zero.Pow(123));
            Assert.AreEqual(one, one.Pow(0));
            Assert.AreEqual(one, one.Pow(123));

            Assert.AreEqual(two.Pow(147), one.ShiftLeft(147));
            Assert.AreEqual(one.ShiftLeft(7).Pow(11), one.ShiftLeft(77));

            BigInteger n = new BigInteger("1234567890987654321");
            BigInteger result = one;

            for (int i = 0; i < 10; ++i)
            {
                try
                {
                    val(i).Pow(-1);
                    Assert.Fail("expected ArithmeticException");
                }
                catch (ArithmeticException) {}

                Assert.AreEqual(result, n.Pow(i));

                result = result.Multiply(n);
            }
        }

        [Test]
        public void TestRemainder()
        {
            // TODO Basic tests

            for (int rep = 0; rep < 10; ++rep)
            {
                BigInteger a = new BigInteger(100 - rep, 0, random);
                BigInteger b = new BigInteger(100 + rep, 0, random);
                BigInteger c = new BigInteger(10 + rep, 0, random);
                BigInteger d = a.Multiply(b).Add(c);
                BigInteger e = d.Remainder(a);

                Assert.AreEqual(c, e);
            }
        }

        [Test]
        public void TestSetBit()
        {
            Assert.AreEqual(one, zero.SetBit(0));
            Assert.AreEqual(one, one.SetBit(0));
            Assert.AreEqual(three, two.SetBit(0));

            Assert.AreEqual(two, zero.SetBit(1));
            Assert.AreEqual(three, one.SetBit(1));
            Assert.AreEqual(two, two.SetBit(1));

            // TODO Tests for setting bits in negative numbers

            // TODO Tests for setting extended bits

            for (int i = 0; i < 10; ++i)
            {
                BigInteger n = new BigInteger(128, random);

                for (int j = 0; j < 10; ++j)
                {
                    int pos = random.Next(128);
                    BigInteger m = n.SetBit(pos);
                    bool test = m.ShiftRight(pos).Remainder(two).Equals(one);

                    Assert.IsTrue(test);
                }
            }

            for (int i = 0; i < 100; ++i)
            {
                BigInteger pow2 = one.ShiftLeft(i);
                BigInteger minusPow2 = pow2.Negate();

                Assert.AreEqual(pow2, pow2.SetBit(i));
                Assert.AreEqual(minusPow2, minusPow2.SetBit(i));

                BigInteger bigI = BigInteger.ValueOf(i);
                BigInteger negI = bigI.Negate();

                for (int j = 0; j < 10; ++j)
                {
                    string data = "i=" + i + ", j=" + j;
                    Assert.AreEqual(bigI.Or(one.ShiftLeft(j)), bigI.SetBit(j), data);
                    Assert.AreEqual(negI.Or(one.ShiftLeft(j)), negI.SetBit(j), data);
                }
            }
        }

        [Test]
        public void TestShiftLeft()
        {
            for (int i = 0; i < 100; ++i)
            {
                int shift = random.Next(128);

                BigInteger a = new BigInteger(128 + i, random).Add(one);
                int bits = a.BitCount; // Make sure nBits is set

                BigInteger negA = a.Negate();
                bits = negA.BitCount; // Make sure nBits is set

                BigInteger b = a.ShiftLeft(shift);
                BigInteger c = negA.ShiftLeft(shift);

                Assert.AreEqual(a.BitCount, b.BitCount);
                Assert.AreEqual(negA.BitCount + shift, c.BitCount);
                Assert.AreEqual(a.BitLength + shift, b.BitLength);
                Assert.AreEqual(negA.BitLength + shift, c.BitLength);

                int j = 0;
                for (; j < shift; ++j)
                {
                    Assert.IsFalse(b.TestBit(j));
                }

                for (; j < b.BitLength; ++j)
                {
                    Assert.AreEqual(a.TestBit(j - shift), b.TestBit(j));
                }
            }
        }

        [Test]
        public void TestShiftRight()
        {
            for (int i = 0; i < 10; ++i)
            {
                int shift = random.Next(128);
                BigInteger a = new BigInteger(256 + i, random).SetBit(256 + i);
                BigInteger b = a.ShiftRight(shift);

                Assert.AreEqual(a.BitLength - shift, b.BitLength);

                for (int j = 0; j < b.BitLength; ++j)
                {
                    Assert.AreEqual(a.TestBit(j + shift), b.TestBit(j));
                }
            }
        }

        [Test]
        public void TestSignValue()
        {
            for (int i = -10; i <= 10; ++i)
            {
                Assert.AreEqual(i < 0 ? -1 : i > 0 ? 1 : 0, val(i).SignValue);
            }
        }

        [Test]
        public void TestSubtract()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i - j),
                        val(i).Subtract(val(j)),
                        "Problem: " + i + ".Subtract(" + j + ") should be " + (i - j));
                }
            }
        }

        [Test]
        public void TestTestBit()
        {
            for (int i = 0; i < 10; ++i)
            {
                BigInteger n = new BigInteger(128, random);

                Assert.IsFalse(n.TestBit(128));
                Assert.IsTrue(n.Negate().TestBit(128));

                for (int j = 0; j < 10; ++j)
                {
                    int pos = random.Next(128);
                    bool test = n.ShiftRight(pos).Remainder(two).Equals(one);

                    Assert.AreEqual(test, n.TestBit(pos));
                }
            }
        }

        [Test]
        public void TestToByteArray()
        {
            byte[] z = BigInteger.Zero.ToByteArray();
            Assert.IsTrue(Arrays.AreEqual(new byte[1], z));

            for (int i = 16; i <= 48; ++i)
            {
                BigInteger x = BigInteger.ProbablePrime(i, random);
                byte[] b = x.ToByteArray();
                Assert.AreEqual((i / 8 + 1), b.Length);
                BigInteger y = new BigInteger(b);
                Assert.AreEqual(x, y);

                x = x.Negate();
                b = x.ToByteArray();
                Assert.AreEqual((i / 8 + 1), b.Length);
                y = new BigInteger(b);
                Assert.AreEqual(x, y);
            }
        }

        [Test]
        public void TestToByteArrayUnsigned()
        {
            byte[] z = BigInteger.Zero.ToByteArrayUnsigned();
            Assert.IsTrue(Arrays.AreEqual(new byte[0], z));

            for (int i = 16; i <= 48; ++i)
            {
                BigInteger x = BigInteger.ProbablePrime(i, random);
                byte[] b = x.ToByteArrayUnsigned();
                Assert.AreEqual((i + 7) / 8, b.Length);
                BigInteger y = new BigInteger(1, b);
                Assert.AreEqual(x, y);

                x = x.Negate();
                b = x.ToByteArrayUnsigned();
                Assert.AreEqual(i / 8 + 1, b.Length);
                y = new BigInteger(b);
                Assert.AreEqual(x, y);
            }
        }

        [Test]
        public void TestToString()
        {
            string s = "12345667890987654321";

            Assert.AreEqual(s, new BigInteger(s).ToString());
            Assert.AreEqual(s, new BigInteger(s, 10).ToString(10));
            Assert.AreEqual(s, new BigInteger(s, 16).ToString(16));

            for (int i = 0; i < 100; ++i)
            {
                BigInteger n = new BigInteger(i, random);

                Assert.AreEqual(n, new BigInteger(n.ToString(2), 2));
                Assert.AreEqual(n, new BigInteger(n.ToString(10), 10));
                Assert.AreEqual(n, new BigInteger(n.ToString(16), 16));
            }

            // Radix version
            int[] radices = new int[] { 2, 8, 10, 16 };
            int trials = 256;

            BigInteger[] tests = new BigInteger[trials];
            for (int i = 0; i < trials; ++i)
            {
                int len = random.Next(i + 1);
                tests[i] = new BigInteger(len, random);
            }

            foreach (int radix in radices)
            {
                for (int i = 0; i < trials; ++i)
                {
                    BigInteger n1 = tests[i];
                    string str = n1.ToString(radix);
                    BigInteger n2 = new BigInteger(str, radix);
                    Assert.AreEqual(n1, n2);
                }
            }
        }

        [Test]
        public void TestValueOf()
        {
            Assert.AreEqual(-1, BigInteger.ValueOf(-1).SignValue);
            Assert.AreEqual(0, BigInteger.ValueOf(0).SignValue);
            Assert.AreEqual(1, BigInteger.ValueOf(1).SignValue);

            for (long i = -5; i < 5; ++i)
            {
                Assert.AreEqual(i, BigInteger.ValueOf(i).IntValue);
            }
        }

        [Test]
        public void TestXor()
        {
            for (int i = -10; i <= 10; ++i)
            {
                for (int j = -10; j <= 10; ++j)
                {
                    Assert.AreEqual(
                        val(i ^ j),
                        val(i).Xor(val(j)),
                        "Problem: " + i + " XOR " + j + " should be " + (i ^ j));
                }
            }
        }

        private static BigInteger val(long n)
        {
            return BigInteger.ValueOf(n);
        }

        private static BigInteger mersenne(int e)
        {
            return two.Pow(e).Subtract(one);
        }

        private static readonly BigInteger minusTwo = BigInteger.Two.Negate();
        private static readonly BigInteger minusOne = BigInteger.One.Negate();
        private static readonly BigInteger zero = BigInteger.Zero;
        private static readonly BigInteger one = BigInteger.One;
        private static readonly BigInteger two = BigInteger.Two;
        private static readonly BigInteger three = BigInteger.Three;

        private static int[] firstPrimes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };
        private static int[] nonPrimes = { 0, 1, 4, 10, 20, 21, 22, 25, 26, 27 };

        private static int[] mersennePrimeExponents = { 2, 3, 5, 7, 13, 17, 19, 31, 61, 89 };
        private static int[] nonPrimeExponents = { 1, 4, 6, 9, 11, 15, 23, 29, 37, 41 };
    }
}
