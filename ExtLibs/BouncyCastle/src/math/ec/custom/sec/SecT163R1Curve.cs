using System;

using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecT163R1Curve
        : AbstractF2mCurve
    {
        private const int SECT163R1_DEFAULT_COORDS = COORD_LAMBDA_PROJECTIVE;
        private const int SECT163R1_FE_LONGS = 3;

        protected readonly SecT163R1Point m_infinity;

        public SecT163R1Curve()
            : base(163, 3, 6, 7)
        {
            this.m_infinity = new SecT163R1Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1, Hex.Decode("07B6882CAAEFA84F9554FF8428BD88E246D2782AE2")));
            this.m_b = FromBigInteger(new BigInteger(1, Hex.Decode("0713612DCDDCB40AAB946BDA29CA91F73AF958AFD9")));
            this.m_order = new BigInteger(1, Hex.Decode("03FFFFFFFFFFFFFFFFFFFF48AAB689C29CA710279B"));
            this.m_cofactor = BigInteger.Two;

            this.m_coord = SECT163R1_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecT163R1Curve();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
            case COORD_LAMBDA_PROJECTIVE:
                return true;
            default:
                return false;
            }
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return 163; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecT163FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecT163R1Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecT163R1Point(this, x, y, zs, withCompression);
        }

        public override bool IsKoblitz
        {
            get { return false; }
        }

        public virtual int M
        {
            get { return 163; }
        }

        public virtual bool IsTrinomial
        {
            get { return false; }
        }

        public virtual int K1
        {
            get { return 3; }
        }

        public virtual int K2
        {
            get { return 6; }
        }

        public virtual int K3
        {
            get { return 7; }
        }

        public override ECLookupTable CreateCacheSafeLookupTable(ECPoint[] points, int off, int len)
        {
            ulong[] table = new ulong[len * SECT163R1_FE_LONGS * 2];
            {
                int pos = 0;
                for (int i = 0; i < len; ++i)
                {
                    ECPoint p = points[off + i];
                    Nat192.Copy64(((SecT163FieldElement)p.RawXCoord).x, 0, table, pos); pos += SECT163R1_FE_LONGS;
                    Nat192.Copy64(((SecT163FieldElement)p.RawYCoord).x, 0, table, pos); pos += SECT163R1_FE_LONGS;
                }
            }

            return new SecT163R1LookupTable(this, table, len);
        }

        private class SecT163R1LookupTable
            : ECLookupTable
        {
            private readonly SecT163R1Curve m_outer;
            private readonly ulong[] m_table;
            private readonly int m_size;

            internal SecT163R1LookupTable(SecT163R1Curve outer, ulong[] table, int size)
            {
                this.m_outer = outer;
                this.m_table = table;
                this.m_size = size;
            }

            public virtual int Size
            {
                get { return m_size; }
            }

            public virtual ECPoint Lookup(int index)
            {
                ulong[] x = Nat192.Create64(), y = Nat192.Create64();
                int pos = 0;

                for (int i = 0; i < m_size; ++i)
                {
                    ulong MASK = (ulong)(long)(((i ^ index) - 1) >> 31);

                    for (int j = 0; j < SECT163R1_FE_LONGS; ++j)
                    {
                        x[j] ^= m_table[pos + j] & MASK;
                        y[j] ^= m_table[pos + SECT163R1_FE_LONGS + j] & MASK;
                    }

                    pos += (SECT163R1_FE_LONGS * 2);
                }

                return m_outer.CreateRawPoint(new SecT163FieldElement(x), new SecT163FieldElement(y), false);
            }
        }
    }
}
