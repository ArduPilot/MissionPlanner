using System;

using Org.BouncyCastle.Math.Raw;
using Org.BouncyCastle.Utilities.Encoders;

namespace Org.BouncyCastle.Math.EC.Custom.Sec
{
    internal class SecP160R2Curve
        : AbstractFpCurve
    {
        public static readonly BigInteger q = new BigInteger(1,
            Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC73"));

        private const int SECP160R2_DEFAULT_COORDS = COORD_JACOBIAN;
        private const int SECP160R2_FE_INTS = 5;

        protected readonly SecP160R2Point m_infinity;

        public SecP160R2Curve()
            : base(q)
        {
            this.m_infinity = new SecP160R2Point(this, null, null);

            this.m_a = FromBigInteger(new BigInteger(1,
                Hex.Decode("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFEFFFFAC70")));
            this.m_b = FromBigInteger(new BigInteger(1,
                Hex.Decode("B4E134D3FB59EB8BAB57274904664D5AF50388BA")));
            this.m_order = new BigInteger(1, Hex.Decode("0100000000000000000000351EE786A818F3A1A16B"));
            this.m_cofactor = BigInteger.One;

            this.m_coord = SECP160R2_DEFAULT_COORDS;
        }

        protected override ECCurve CloneCurve()
        {
            return new SecP160R2Curve();
        }

        public override bool SupportsCoordinateSystem(int coord)
        {
            switch (coord)
            {
            case COORD_JACOBIAN:
                return true;
            default:
                return false;
            }
        }

        public virtual BigInteger Q
        {
            get { return q; }
        }

        public override ECPoint Infinity
        {
            get { return m_infinity; }
        }

        public override int FieldSize
        {
            get { return q.BitLength; }
        }

        public override ECFieldElement FromBigInteger(BigInteger x)
        {
            return new SecP160R2FieldElement(x);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, bool withCompression)
        {
            return new SecP160R2Point(this, x, y, withCompression);
        }

        protected internal override ECPoint CreateRawPoint(ECFieldElement x, ECFieldElement y, ECFieldElement[] zs, bool withCompression)
        {
            return new SecP160R2Point(this, x, y, zs, withCompression);
        }

        public override ECLookupTable CreateCacheSafeLookupTable(ECPoint[] points, int off, int len)
        {
            uint[] table = new uint[len * SECP160R2_FE_INTS * 2];
            {
                int pos = 0;
                for (int i = 0; i < len; ++i)
                {
                    ECPoint p = points[off + i];
                    Nat160.Copy(((SecP160R2FieldElement)p.RawXCoord).x, 0, table, pos); pos += SECP160R2_FE_INTS;
                    Nat160.Copy(((SecP160R2FieldElement)p.RawYCoord).x, 0, table, pos); pos += SECP160R2_FE_INTS;
                }
            }

            return new SecP160R2LookupTable(this, table, len);
        }

        private class SecP160R2LookupTable
            : ECLookupTable
        {
            private readonly SecP160R2Curve m_outer;
            private readonly uint[] m_table;
            private readonly int m_size;

            internal SecP160R2LookupTable(SecP160R2Curve outer, uint[] table, int size)
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
                uint[] x = Nat160.Create(), y = Nat160.Create();
                int pos = 0;

                for (int i = 0; i < m_size; ++i)
                {
                    uint MASK = (uint)(((i ^ index) - 1) >> 31);

                    for (int j = 0; j < SECP160R2_FE_INTS; ++j)
                    {
                        x[j] ^= m_table[pos + j] & MASK;
                        y[j] ^= m_table[pos + SECP160R2_FE_INTS + j] & MASK;
                    }

                    pos += (SECP160R2_FE_INTS * 2);
                }

                return m_outer.CreateRawPoint(new SecP160R2FieldElement(x), new SecP160R2FieldElement(y), false);
            }
        }
    }
}
