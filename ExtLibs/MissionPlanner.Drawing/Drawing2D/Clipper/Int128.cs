using System;

namespace ClipperLib
{
    internal struct Int128
    {
        private Int64 hi;
        private UInt64 lo;

        public Int128(Int64 _lo)
        {
            lo = (UInt64)_lo;
            if (_lo < 0) hi = -1;
            else hi = 0;
        }

        public Int128(Int64 _hi, UInt64 _lo)
        {
            lo = _lo;
            hi = _hi;
        }

        public Int128(Int128 val)
        {
            hi = val.hi;
            lo = val.lo;
        }

        public bool IsNegative()
        {
            return hi < 0;
        }

        public static bool operator ==(Int128 val1, Int128 val2)
        {
            if ((object)val1 == (object)val2) return true;
            else if ((object)val1 == null || (object)val2 == null) return false;
            return (val1.hi == val2.hi && val1.lo == val2.lo);
        }

        public static bool operator !=(Int128 val1, Int128 val2)
        {
            return !(val1 == val2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null || !(obj is Int128))
                return false;
            Int128 i128 = (Int128)obj;
            return (i128.hi == hi && i128.lo == lo);
        }

        public override int GetHashCode()
        {
            return hi.GetHashCode() ^ lo.GetHashCode();
        }

        public static bool operator >(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi)
                return val1.hi > val2.hi;
            else
                return val1.lo > val2.lo;
        }

        public static bool operator <(Int128 val1, Int128 val2)
        {
            if (val1.hi != val2.hi)
                return val1.hi < val2.hi;
            else
                return val1.lo < val2.lo;
        }

        public static Int128 operator +(Int128 lhs, Int128 rhs)
        {
            lhs.hi += rhs.hi;
            lhs.lo += rhs.lo;
            if (lhs.lo < rhs.lo) lhs.hi++;
            return lhs;
        }

        public static Int128 operator -(Int128 lhs, Int128 rhs)
        {
            return lhs + -rhs;
        }

        public static Int128 operator -(Int128 val)
        {
            if (val.lo == 0)
                return new Int128(-val.hi, 0);
            else
                return new Int128(~val.hi, ~val.lo + 1);
        }

        //nb: Constructing two new Int128 objects every time we want to multiply longs  
        //is slow. So, although calling the Int128Mul method doesn't look as clean, the 
        //code runs significantly faster than if we'd used the * operator.

        public static Int128 Int128Mul(Int64 lhs, Int64 rhs)
        {
            bool negate = (lhs < 0) != (rhs < 0);
            if (lhs < 0) lhs = -lhs;
            if (rhs < 0) rhs = -rhs;
            UInt64 int1Hi = (UInt64)lhs >> 32;
            UInt64 int1Lo = (UInt64)lhs & 0xFFFFFFFF;
            UInt64 int2Hi = (UInt64)rhs >> 32;
            UInt64 int2Lo = (UInt64)rhs & 0xFFFFFFFF;

            //nb: see comments in clipper.pas
            UInt64 a = int1Hi * int2Hi;
            UInt64 b = int1Lo * int2Lo;
            UInt64 c = int1Hi * int2Lo + int1Lo * int2Hi;

            UInt64 lo;
            Int64 hi;
            hi = (Int64)(a + (c >> 32));

            unchecked { lo = (c << 32) + b; }
            if (lo < b) hi++;
            Int128 result = new Int128(hi, lo);
            return negate ? -result : result;
        }

        public static Int128 operator /(Int128 lhs, Int128 rhs)
        {
            if (rhs.lo == 0 && rhs.hi == 0)
                throw new ClipperException("Int128: divide by zero");

            bool negate = (rhs.hi < 0) != (lhs.hi < 0);
            if (lhs.hi < 0) lhs = -lhs;
            if (rhs.hi < 0) rhs = -rhs;

            if (rhs < lhs)
            {
                Int128 result = new Int128(0);
                Int128 cntr = new Int128(1);
                while (rhs.hi >= 0 && !(rhs > lhs))
                {
                    rhs.hi <<= 1;
                    if ((Int64)rhs.lo < 0) rhs.hi++;
                    rhs.lo <<= 1;

                    cntr.hi <<= 1;
                    if ((Int64)cntr.lo < 0) cntr.hi++;
                    cntr.lo <<= 1;
                }
                rhs.lo >>= 1;
                if ((rhs.hi & 1) == 1)
                    rhs.lo |= 0x8000000000000000;
                rhs.hi = (Int64)((UInt64)rhs.hi >> 1);

                cntr.lo >>= 1;
                if ((cntr.hi & 1) == 1)
                    cntr.lo |= 0x8000000000000000;
                cntr.hi >>= 1;

                while (cntr.hi != 0 || cntr.lo != 0)
                {
                    if (!(lhs < rhs))
                    {
                        lhs -= rhs;
                        result.hi |= cntr.hi;
                        result.lo |= cntr.lo;
                    }
                    rhs.lo >>= 1;
                    if ((rhs.hi & 1) == 1)
                        rhs.lo |= 0x8000000000000000;
                    rhs.hi >>= 1;

                    cntr.lo >>= 1;
                    if ((cntr.hi & 1) == 1)
                        cntr.lo |= 0x8000000000000000;
                    cntr.hi >>= 1;
                }
                return negate ? -result : result;
            }
            else if (rhs == lhs)
                return new Int128(1);
            else
                return new Int128(0);
        }

        public double ToDouble()
        {
            const double shift64 = 18446744073709551616.0; //2^64
            if (hi < 0)
            {
                if (lo == 0)
                    return (double)hi * shift64;
                else
                    return -(double)(~lo + ~hi * shift64);
            }
            else
                return (double)(lo + hi * shift64);
        }

    };
}