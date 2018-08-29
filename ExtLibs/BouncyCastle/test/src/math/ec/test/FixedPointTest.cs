using System;
using System.Collections;

using NUnit.Framework;

using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.EC;
using Org.BouncyCastle.Math.EC.Multiplier;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Math.EC.Tests
{
    [TestFixture]
    public class FixedPointTest
    {
        private static readonly SecureRandom Random = new SecureRandom();

        private const int TestsPerCurve = 5;

        [Test]
        public void TestFixedPointMultiplier()
        {
            FixedPointCombMultiplier M = new FixedPointCombMultiplier();

            ArrayList names = new ArrayList();
            CollectionUtilities.AddRange(names, ECNamedCurveTable.Names);
            CollectionUtilities.AddRange(names, CustomNamedCurves.Names);

            ISet uniqNames = new HashSet(names);

            foreach (string name in uniqNames)
            {
                X9ECParameters x9A = ECNamedCurveTable.GetByName(name);
                X9ECParameters x9B = CustomNamedCurves.GetByName(name);

                X9ECParameters x9 = x9B != null ? x9B : x9A;

                for (int i = 0; i < TestsPerCurve; ++i)
                {
                    BigInteger k = new BigInteger(x9.N.BitLength, Random);
                    ECPoint pRef = ECAlgorithms.ReferenceMultiply(x9.G, k);

                    if (x9A != null)
                    {
                        ECPoint pA = M.Multiply(x9A.G, k);
                        AssertPointsEqual("Standard curve fixed-point failure", pRef, pA);
                    }

                    if (x9B != null)
                    {
                        ECPoint pB = M.Multiply(x9B.G, k);
                        AssertPointsEqual("Custom curve fixed-point failure", pRef, pB);
                    }
                }
            }
        }

        private void AssertPointsEqual(string message, ECPoint a, ECPoint b)
        {
            // NOTE: We intentionally test points for equality in both directions
            Assert.AreEqual(a, b, message);
            Assert.AreEqual(b, a, message);
        }
    }
}
