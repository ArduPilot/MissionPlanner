using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Math.EC.Tests
{
    /**
     * Test class for {@link org.bouncycastle.math.ec.ECPoint ECPoint}. All
     * literature values are taken from "Guide to elliptic curve cryptography",
     * Darrel Hankerson, Alfred J. Menezes, Scott Vanstone, 2004, Springer-Verlag
     * New York, Inc.
     */
    [TestFixture]
    public class ECPointTest
    {
        /**
         * Random source used to generate random points
         */
        private SecureRandom Random = new SecureRandom();

        /**
         * Nested class containing sample literature values for <code>Fp</code>.
         */
        public class Fp
        {
            internal static readonly BigInteger q = new BigInteger("29");

            internal static readonly BigInteger a = new BigInteger("4");

            internal static readonly BigInteger b = new BigInteger("20");

            internal static readonly BigInteger n = new BigInteger("38");

            internal static readonly BigInteger h = new BigInteger("1");

            internal static readonly ECCurve curve = new FpCurve(q, a, b, n, h);

            internal static readonly ECPoint infinity = curve.Infinity;

            internal static readonly int[] pointSource = { 5, 22, 16, 27, 13, 6, 14, 6 };

            internal static ECPoint[] p = new ECPoint[pointSource.Length / 2];

            /**
             * Creates the points on the curve with literature values.
             */
            internal static void CreatePoints()
            {
                for (int i = 0; i < pointSource.Length / 2; i++)
                {
                    p[i] = curve.CreatePoint(
                        new BigInteger(pointSource[2 * i].ToString()),
                        new BigInteger(pointSource[2 * i + 1].ToString()));
                }
            }
        }

        /**
         * Nested class containing sample literature values for <code>F2m</code>.
         */
        public class F2m
        {
            // Irreducible polynomial for TPB z^4 + z + 1
            internal const int m = 4;

            internal const int k1 = 1;

            // a = z^3
            internal static readonly BigInteger aTpb = new BigInteger("1000", 2);

            // b = z^3 + 1
            internal static readonly BigInteger bTpb = new BigInteger("1001", 2);

            internal static readonly BigInteger n = new BigInteger("23");

            internal static readonly BigInteger h = new BigInteger("1");

            internal static readonly ECCurve curve = new F2mCurve(m, k1, aTpb, bTpb, n, h);

            internal static readonly ECPoint infinity = curve.Infinity;

            internal static readonly String[] pointSource = { "0010", "1111", "1100", "1100",
                    "0001", "0001", "1011", "0010" };

            internal static readonly ECPoint[] p = new ECPoint[pointSource.Length / 2];

            /**
             * Creates the points on the curve with literature values.
             */
            internal static void CreatePoints()
            {
                for (int i = 0; i < pointSource.Length / 2; i++)
                {
                    p[i] = curve.CreatePoint(
                        new BigInteger(pointSource[2 * i], 2),
                        new BigInteger(pointSource[2 * i + 1], 2));
                }
            }
        }

        [SetUp]
        public void SetUp()
        {
            Fp.CreatePoints();
            F2m.CreatePoints();
        }

        /**
         * Tests, if inconsistent points can be created, i.e. points with exactly
         * one null coordinate (not permitted).
         */
        [Test]
        public void TestPointCreationConsistency()
        {
            try
            {
                ECPoint bad = Fp.curve.CreatePoint(BigInteger.ValueOf(12), null);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Expected
            }

            try
            {
                ECPoint bad = Fp.curve.CreatePoint(null, BigInteger.ValueOf(12));
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Expected
            }

            try
            {
                ECPoint bad = F2m.curve.CreatePoint(new BigInteger("1011"), null);
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Expected
            }

            try
            {
                ECPoint bad = F2m.curve.CreatePoint(null, new BigInteger("1011"));
                Assert.Fail();
            }
            catch (ArgumentException)
            {
                // Expected
            }
        }

        /**
         * Tests <code>ECPoint.add()</code> against literature values.
         *
         * @param p
         *            The array of literature values.
         * @param infinity
         *            The point at infinity on the respective curve.
         */
        private void ImplTestAdd(ECPoint[] p, ECPoint infinity)
        {
            AssertPointsEqual("p0 plus p1 does not equal p2", p[2], p[0].Add(p[1]));
            AssertPointsEqual("p1 plus p0 does not equal p2", p[2], p[1].Add(p[0]));
            for (int i = 0; i < p.Length; i++)
            {
                AssertPointsEqual("Adding infinity failed", p[i], p[i].Add(infinity));
                AssertPointsEqual("Adding to infinity failed", p[i], infinity.Add(p[i]));
            }
        }

        /**
         * Calls <code>implTestAdd()</code> for <code>Fp</code> and
         * <code>F2m</code>.
         */
        [Test]
        public void TestAdd()
        {
            ImplTestAdd(Fp.p, Fp.infinity);
            ImplTestAdd(F2m.p, F2m.infinity);
        }

        /**
         * Tests <code>ECPoint.twice()</code> against literature values.
         *
         * @param p
         *            The array of literature values.
         */
        private void ImplTestTwice(ECPoint[] p)
        {
            AssertPointsEqual("Twice incorrect", p[3], p[0].Twice());
            AssertPointsEqual("Add same point incorrect", p[3], p[0].Add(p[0]));
        }

        /**
         * Calls <code>implTestTwice()</code> for <code>Fp</code> and
         * <code>F2m</code>.
         */
        [Test]
        public void TestTwice()
        {
            ImplTestTwice(Fp.p);
            ImplTestTwice(F2m.p);
        }

        private void ImplTestThreeTimes(ECPoint[] p)
        {
            ECPoint P = p[0];
            ECPoint _3P = P.Add(P).Add(P);
            AssertPointsEqual("ThreeTimes incorrect", _3P, P.ThreeTimes());
            AssertPointsEqual("TwicePlus incorrect", _3P, P.TwicePlus(P));
        }

        /**
         * Calls <code>implTestThreeTimes()</code> for <code>Fp</code> and
         * <code>F2m</code>.
         */
        [Test]
        public void TestThreeTimes()
        {
            ImplTestThreeTimes(Fp.p);
            ImplTestThreeTimes(F2m.p);
        }

        /**
         * Goes through all points on an elliptic curve and checks, if adding a
         * point <code>k</code>-times is the same as multiplying the point by
         * <code>k</code>, for all <code>k</code>. Should be called for points
         * on very small elliptic curves only.
         *
         * @param p
         *            The base point on the elliptic curve.
         * @param infinity
         *            The point at infinity on the elliptic curve.
         */
        private void ImplTestAllPoints(ECPoint p, ECPoint infinity)
        {
            ECPoint adder = infinity;
            ECPoint multiplier = infinity;

            BigInteger i = BigInteger.One;
            do
            {
                adder = adder.Add(p);
                multiplier = p.Multiply(i);
                AssertPointsEqual("Results of Add() and Multiply() are inconsistent " + i, adder, multiplier);
                i = i.Add(BigInteger.One);
            }
            while (!(adder.Equals(infinity)));
        }

        /**
         * Calls <code>implTestAllPoints()</code> for the small literature curves,
         * both for <code>Fp</code> and <code>F2m</code>.
         */
        [Test]
        public void TestAllPoints()
        {
            for (int i = 0; i < Fp.p.Length; i++)
            {
                ImplTestAllPoints(Fp.p[0], Fp.infinity);
            }

            for (int i = 0; i < F2m.p.Length; i++)
            {
                ImplTestAllPoints(F2m.p[0], F2m.infinity);
            }
        }

        /**
         * Checks, if the point multiplication algorithm of the given point yields
         * the same result as point multiplication done by the reference
         * implementation given in <code>multiply()</code>. This method chooses a
         * random number by which the given point <code>p</code> is multiplied.
         *
         * @param p
         *            The point to be multiplied.
         * @param numBits
         *            The bitlength of the random number by which <code>p</code>
         *            is multiplied.
         */
        private void ImplTestMultiply(ECPoint p, int numBits)
        {
            BigInteger k = new BigInteger(numBits, Random);
            ECPoint reff = ECAlgorithms.ReferenceMultiply(p, k);
            ECPoint q = p.Multiply(k);
            AssertPointsEqual("ECPoint.Multiply is incorrect", reff, q);
        }

        /**
         * Checks, if the point multiplication algorithm of the given point yields
         * the same result as point multiplication done by the reference
         * implementation given in <code>multiply()</code>. This method tests
         * multiplication of <code>p</code> by every number of bitlength
         * <code>numBits</code> or less.
         *
         * @param p
         *            The point to be multiplied.
         * @param numBits
         *            Try every multiplier up to this bitlength
         */
        private void ImplTestMultiplyAll(ECPoint p, int numBits)
        {
            BigInteger bound = BigInteger.One.ShiftLeft(numBits);
            BigInteger k = BigInteger.Zero;

            do
            {
                ECPoint reff = ECAlgorithms.ReferenceMultiply(p, k);
                ECPoint q = p.Multiply(k);
                AssertPointsEqual("ECPoint.Multiply is incorrect", reff, q);
                k = k.Add(BigInteger.One);
            }
            while (k.CompareTo(bound) < 0);
        }

        /**
         * Tests <code>ECPoint.add()</code> and <code>ECPoint.subtract()</code>
         * for the given point and the given point at infinity.
         *
         * @param p
         *            The point on which the tests are performed.
         * @param infinity
         *            The point at infinity on the same curve as <code>p</code>.
         */
        private void ImplTestAddSubtract(ECPoint p, ECPoint infinity)
        {
            AssertPointsEqual("Twice and Add inconsistent", p.Twice(), p.Add(p));
            AssertPointsEqual("Twice p - p is not p", p, p.Twice().Subtract(p));
            AssertPointsEqual("TwicePlus(p, -p) is not p", p, p.TwicePlus(p.Negate()));
            AssertPointsEqual("p - p is not infinity", infinity, p.Subtract(p));
            AssertPointsEqual("p plus infinity is not p", p, p.Add(infinity));
            AssertPointsEqual("infinity plus p is not p", p, infinity.Add(p));
            AssertPointsEqual("infinity plus infinity is not infinity ", infinity, infinity.Add(infinity));
            AssertPointsEqual("Twice infinity is not infinity ", infinity, infinity.Twice());
        }

        /**
         * Calls <code>implTestAddSubtract()</code> for literature values, both
         * for <code>Fp</code> and <code>F2m</code>.
         */
        [Test]
        public void TestAddSubtractMultiplySimple()
        {
            int fpBits = Fp.curve.Order.BitLength;
            for (int iFp = 0; iFp < Fp.pointSource.Length / 2; iFp++)
            {
                ImplTestAddSubtract(Fp.p[iFp], Fp.infinity);

                ImplTestMultiplyAll(Fp.p[iFp], fpBits);
                ImplTestMultiplyAll(Fp.infinity, fpBits);
            }

            int f2mBits = F2m.curve.Order.BitLength;
            for (int iF2m = 0; iF2m < F2m.pointSource.Length / 2; iF2m++)
            {
                ImplTestAddSubtract(F2m.p[iF2m], F2m.infinity);

                ImplTestMultiplyAll(F2m.p[iF2m], f2mBits);
                ImplTestMultiplyAll(F2m.infinity, f2mBits);
            }
        }

        /**
         * Test encoding with and without point compression.
         *
         * @param p
         *            The point to be encoded and decoded.
         */
        private void ImplTestEncoding(ECPoint p)
        {
            // Not Point Compression
            byte[] unCompBarr = p.GetEncoded(false);
            ECPoint decUnComp = p.Curve.DecodePoint(unCompBarr);
            AssertPointsEqual("Error decoding uncompressed point", p, decUnComp);

            // Point compression
            byte[] compBarr = p.GetEncoded(true);
            ECPoint decComp = p.Curve.DecodePoint(compBarr);
            AssertPointsEqual("Error decoding compressed point", p, decComp);
        }

        private void ImplAddSubtractMultiplyTwiceEncodingTest(ECCurve curve, ECPoint q, BigInteger n)
        {
            // Get point at infinity on the curve
            ECPoint infinity = curve.Infinity;

            ImplTestAddSubtract(q, infinity);
            ImplTestMultiply(q, n.BitLength);
            ImplTestMultiply(infinity, n.BitLength);

            ECPoint p = q;
            for (int i = 0; i < 10; ++i)
            {
                ImplTestEncoding(p);
                p = p.Twice();
            }
        }

        private void ImplSqrtTest(ECCurve c)
        {
            if (ECAlgorithms.IsFpCurve(c))
            {
                BigInteger p = c.Field.Characteristic;
                BigInteger pMinusOne = p.Subtract(BigInteger.One);
                BigInteger legendreExponent = p.ShiftRight(1);

                int count = 0;
                while (count < 10)
                {
                    BigInteger nonSquare = BigIntegers.CreateRandomInRange(BigInteger.Two, pMinusOne, Random);
                    if (!nonSquare.ModPow(legendreExponent, p).Equals(BigInteger.One))
                    {
                        ECFieldElement root = c.FromBigInteger(nonSquare).Sqrt();
                        Assert.IsNull(root);
                        ++count;
                    }
                }
            }
            else if (ECAlgorithms.IsF2mCurve(c))
            {
                int m = c.FieldSize;
                BigInteger x = new BigInteger(m, Random);
                ECFieldElement fe = c.FromBigInteger(x);
                for (int i = 0; i < 100; ++i)
                {
                    ECFieldElement sq = fe.Square();
                    ECFieldElement check = sq.Sqrt();
                    Assert.AreEqual(fe, check);
                    fe = sq;
                }
            }
        }

        private void ImplValidityTest(ECCurve c, ECPoint g)
        {
            Assert.IsTrue(g.IsValid());

            BigInteger h = c.Cofactor;
            if (h != null && h.CompareTo(BigInteger.One) > 0)
            {
                if (ECAlgorithms.IsF2mCurve(c))
                {
                    ECPoint order2 = c.CreatePoint(BigInteger.Zero, c.B.Sqrt().ToBigInteger());
                    ECPoint bad = g.Add(order2);
                    Assert.IsFalse(bad.IsValid());
                }
            }
        }

        private void ImplAddSubtractMultiplyTwiceEncodingTestAllCoords(X9ECParameters x9ECParameters)
        {
            BigInteger n = x9ECParameters.N;
            ECPoint G = x9ECParameters.G;
            ECCurve C = x9ECParameters.Curve;

            int[] coords = ECCurve.GetAllCoordinateSystems();
            for (int i = 0; i < coords.Length; ++i)
            {
                int coord = coords[i];
                if (C.SupportsCoordinateSystem(coord))
                {
                    ECCurve c = C;
                    ECPoint g = G;

                    if (c.CoordinateSystem != coord)
                    {
                        c = C.Configure().SetCoordinateSystem(coord).Create();
                        g = c.ImportPoint(G);
                    }

                    // The generator is multiplied by random b to get random q
                    BigInteger b = new BigInteger(n.BitLength, Random);
                    ECPoint q = g.Multiply(b).Normalize();

                    ImplAddSubtractMultiplyTwiceEncodingTest(c, q, n);

                    ImplSqrtTest(c);

                    ImplValidityTest(c, g);
                }
            }
        }

        /**
         * Calls <code>implTestAddSubtract()</code>,
         * <code>implTestMultiply</code> and <code>implTestEncoding</code> for
         * the standard elliptic curves as given in <code>SecNamedCurves</code>.
         */
        [Test]
        public void TestAddSubtractMultiplyTwiceEncoding()
        {
            ArrayList names = new ArrayList();
            CollectionUtilities.AddRange(names, ECNamedCurveTable.Names);
            CollectionUtilities.AddRange(names, CustomNamedCurves.Names);

            ISet uniqNames = new HashSet(names);

            foreach (string name in uniqNames)
            {
                X9ECParameters x9A = ECNamedCurveTable.GetByName(name);
                X9ECParameters x9B = CustomNamedCurves.GetByName(name);

                if (x9A != null && x9B != null)
                {
                    Assert.AreEqual(x9A.Curve.Field, x9B.Curve.Field);
                    Assert.AreEqual(x9A.Curve.A.ToBigInteger(), x9B.Curve.A.ToBigInteger());
                    Assert.AreEqual(x9A.Curve.B.ToBigInteger(), x9B.Curve.B.ToBigInteger());
                    AssertOptionalValuesAgree(x9A.Curve.Cofactor, x9B.Curve.Cofactor);
                    AssertOptionalValuesAgree(x9A.Curve.Order, x9B.Curve.Order);

                    AssertPointsEqual("Custom curve base-point inconsistency", x9A.G, x9B.G);

                    Assert.AreEqual(x9A.H, x9B.H);
                    Assert.AreEqual(x9A.N, x9B.N);
                    AssertOptionalValuesAgree(x9A.GetSeed(), x9B.GetSeed());

                    BigInteger k = new BigInteger(x9A.N.BitLength, Random);
                    ECPoint pA = x9A.G.Multiply(k);
                    ECPoint pB = x9B.G.Multiply(k);
                    AssertPointsEqual("Custom curve multiplication inconsistency", pA, pB);
                }

                if (x9A != null)
                {
                    ImplAddSubtractMultiplyTwiceEncodingTestAllCoords(x9A);
                }

                if (x9B != null)
                {
                    ImplAddSubtractMultiplyTwiceEncodingTestAllCoords(x9B);
                }
            }
        }

        private void AssertPointsEqual(string message, ECPoint a, ECPoint b)
        {
            // NOTE: We intentionally test points for equality in both directions
            Assert.AreEqual(a, b, message);
            Assert.AreEqual(b, a, message);
        }

        private void AssertOptionalValuesAgree(object a, object b)
        {
            if (a != null && b != null)
            {
                Assert.AreEqual(a, b);
            }
        }

        private void AssertOptionalValuesAgree(byte[] a, byte[] b)
        {
            if (a != null && b != null)
            {
                Assert.IsTrue(Arrays.AreEqual(a, b));
            }
        }
    }
}
