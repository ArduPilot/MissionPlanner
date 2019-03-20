using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Math.EC.Tests
{
    [TestFixture]
    public class ECAlgorithmsTest
    {
        private const int Scale = 4;
        private static readonly SecureRandom Random = new SecureRandom();

        [Test]
        public void TestSumOfMultiplies()
        {
            X9ECParameters x9 = CustomNamedCurves.GetByName("secp256r1");
            Assert.NotNull(x9);
            DoTestSumOfMultiplies(x9);
        }

        [Test, Explicit]
        public void TestSumOfMultipliesComplete()
        {
            foreach (X9ECParameters x9 in GetTestCurves())
            {
                DoTestSumOfMultiplies(x9);
            }
        }

        [Test]
        public void TestSumOfTwoMultiplies()
        {
            X9ECParameters x9 = CustomNamedCurves.GetByName("secp256r1");
            Assert.NotNull(x9);
            DoTestSumOfTwoMultiplies(x9);
        }

        [Test, Explicit]
        public void TestSumOfTwoMultipliesComplete()
        {
            foreach (X9ECParameters x9 in GetTestCurves())
            {
                DoTestSumOfTwoMultiplies(x9);
            }
        }

        private void DoTestSumOfMultiplies(X9ECParameters x9)
        {
            ECPoint[] points = new ECPoint[Scale];
            BigInteger[] scalars = new BigInteger[Scale];
            for (int i = 0; i < Scale; ++i)
            {
                points[i] = GetRandomPoint(x9);
                scalars[i] = GetRandomScalar(x9);
            }

            ECPoint u = x9.Curve.Infinity;
            for (int i = 0; i < Scale; ++i)
            {
                u = u.Add(points[i].Multiply(scalars[i]));

                ECPoint v = ECAlgorithms.SumOfMultiplies(CopyPoints(points, i + 1), CopyScalars(scalars, i + 1));

                ECPoint[] results = new ECPoint[] { u, v };
                x9.Curve.NormalizeAll(results);

                AssertPointsEqual("ECAlgorithms.SumOfMultiplies is incorrect", results[0], results[1]);
            }
        }

        private void DoTestSumOfTwoMultiplies(X9ECParameters x9)
        {
            ECPoint p = GetRandomPoint(x9);
            BigInteger a = GetRandomScalar(x9);

            for (int i = 0; i < Scale; ++i)
            {
                ECPoint q = GetRandomPoint(x9);
                BigInteger b = GetRandomScalar(x9);

                ECPoint u = p.Multiply(a).Add(q.Multiply(b));
                ECPoint v = ECAlgorithms.ShamirsTrick(p, a, q, b);
                ECPoint w = ECAlgorithms.SumOfTwoMultiplies(p, a, q, b);

                ECPoint[] results = new ECPoint[] { u, v, w };
                x9.Curve.NormalizeAll(results);

                AssertPointsEqual("ECAlgorithms.ShamirsTrick is incorrect", results[0], results[1]);
                AssertPointsEqual("ECAlgorithms.SumOfTwoMultiplies is incorrect", results[0], results[2]);

                p = q;
                a = b;
            }
        }

        private void AssertPointsEqual(string message, ECPoint a, ECPoint b)
        {
            Assert.AreEqual(a, b, message);
        }

        private ECPoint[] CopyPoints(ECPoint[] ps, int len)
        {
            ECPoint[] result = new ECPoint[len];
            Array.Copy(ps, 0, result, 0, len);
            return result;
        }

        private BigInteger[] CopyScalars(BigInteger[] ks, int len)
        {
            BigInteger[] result = new BigInteger[len];
            Array.Copy(ks, 0, result, 0, len);
            return result;
        }

        private ECPoint GetRandomPoint(X9ECParameters x9)
        {
            return x9.G.Multiply(GetRandomScalar(x9));
        }

        private BigInteger GetRandomScalar(X9ECParameters x9)
        {
            return new BigInteger(x9.N.BitLength, Random);
        }

        private IList GetTestCurves()
        {
            ArrayList x9s = new ArrayList();
            ISet names = new HashSet(ECNamedCurveTable.Names);
            names.AddAll(CustomNamedCurves.Names);

            foreach (string name in names)
            {
                X9ECParameters x9 = ECNamedCurveTable.GetByName(name);
                if (x9 != null)
                {
                    AddTestCurves(x9s, x9);
                }

                x9 = CustomNamedCurves.GetByName(name);
                if (x9 != null)
                {
                    AddTestCurves(x9s, x9);
                }
            }
            return x9s;
        }

        private void AddTestCurves(IList x9s, X9ECParameters x9)
        {
            ECCurve curve = x9.Curve;

            int[] coords = ECCurve.GetAllCoordinateSystems();
            for (int i = 0; i < coords.Length; ++i)
            {
                int coord = coords[i];
                if (curve.CoordinateSystem == coord)
                {
                    x9s.Add(x9);
                }
                else if (curve.SupportsCoordinateSystem(coord))
                {
                    ECCurve c = curve.Configure().SetCoordinateSystem(coord).Create();
                    x9s.Add(new X9ECParameters(c, c.ImportPoint(x9.G), x9.N, x9.H));
                }
            }
        }
    }
}
