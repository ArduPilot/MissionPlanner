using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Parameters
{
    public class ECDomainParameters
    {
        internal ECCurve     curve;
        internal byte[]      seed;
        internal ECPoint     g;
        internal BigInteger  n;
        internal BigInteger  h;
        internal BigInteger  hInv;

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n)
            : this(curve, g, n, BigInteger.One, null)
        {
        }

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n,
            BigInteger  h)
            : this(curve, g, n, h, null)
        {
        }

        public ECDomainParameters(
            ECCurve     curve,
            ECPoint     g,
            BigInteger  n,
            BigInteger  h,
            byte[]      seed)
        {
            if (curve == null)
                throw new ArgumentNullException("curve");
            if (g == null)
                throw new ArgumentNullException("g");
            if (n == null)
                throw new ArgumentNullException("n");
            // we can't check for h == null here as h is optional in X9.62 as it is not required for ECDSA

            this.curve = curve;
            this.g = Validate(curve, g);
            this.n = n;
            this.h = h;
            this.seed = Arrays.Clone(seed);
        }

        public ECCurve Curve
        {
            get { return curve; }
        }

        public ECPoint G
        {
            get { return g; }
        }

        public BigInteger N
        {
            get { return n; }
        }

        public BigInteger H
        {
            get { return h; }
        }

        public BigInteger HInv
        {
            get
            {
                lock (this)
                {
                    if (hInv == null)
                    {
                        hInv = h.ModInverse(n);
                    }
                    return hInv;
                }
            }
        }

        public byte[] GetSeed()
        {
            return Arrays.Clone(seed);
        }

        public override bool Equals(
            object obj)
        {
            if (obj == this)
                return true;

            ECDomainParameters other = obj as ECDomainParameters;

            if (other == null)
                return false;

            return Equals(other);
        }

        protected virtual bool Equals(
            ECDomainParameters other)
        {
            return curve.Equals(other.curve)
                &&	g.Equals(other.g)
                &&	n.Equals(other.n)
                &&  h.Equals(other.h);
        }

        public override int GetHashCode()
        {
            int hc = curve.GetHashCode();
            hc *= 37;
            hc ^= g.GetHashCode();
            hc *= 37;
            hc ^= n.GetHashCode();
            hc *= 37;
            hc ^= h.GetHashCode();
            return hc;
        }

        internal static ECPoint Validate(ECCurve c, ECPoint q)
        {
            if (q == null)
                throw new ArgumentException("Point has null value", "q");

            q = ECAlgorithms.ImportPoint(c, q).Normalize();

            if (q.IsInfinity)
                throw new ArgumentException("Point at infinity", "q");

            if (!q.IsValid())
                throw new ArgumentException("Point not on curve", "q");

            return q;
        }
    }
}
