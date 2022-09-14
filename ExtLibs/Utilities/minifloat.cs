using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Minifloat
    /// 1.4.3.−2 minifloat
    /// </summary>
    public struct minifloat : IComparable, IConvertible, IFormattable
    {
        const bool IEEE_754_STANDARD = true;                                                                            //standard: true
        const bool SIGN_BIT = IEEE_754_STANDARD || true;                                                                //standard: true
        const int BITS = 8 * sizeof(byte);                                                                              //standard: 8
        const int EXPONENT_BITS = 4 + (SIGN_BIT ? 0 : 1);                                                               //standard: 4
        const int MANTISSA_BITS = BITS - EXPONENT_BITS - (SIGN_BIT ? 1 : 0);                                            //standard: 3
        const int EXPONENT_BIAS = -(((1 << BITS) - 1) >> (BITS - (EXPONENT_BITS - 1)));                                 //standard: -7
        const int MAX_EXPONENT = EXPONENT_BIAS + ((1 << EXPONENT_BITS) - 1) - (IEEE_754_STANDARD ? 1 : 0);              //standard: 7
        const int SIGNALING_EXPONENT = (MAX_EXPONENT - EXPONENT_BIAS + (IEEE_754_STANDARD ? 1 : 0)) << MANTISSA_BITS;   //standard: 0b0111_0000                     

        private byte _value;

        public minifloat(double d)
        {
            var sign = (d < 0) ? (byte) 1 : (byte) 0;
            var exponent = (int) Math.Log(Math.Abs(d), 2) + 1;
            var significand = Math.Abs(d - (0.0625 * Math.Pow(2, exponent) * 8)) / (0.0625 * Math.Pow(2, exponent));
            if (d > -1 && d < 1)
            {
                exponent = 0;
                significand = Math.Abs(d) * 8;
            }

            if (double.IsInfinity(d))
            {
                exponent = SIGNALING_EXPONENT >> 3;
            }

            if (double.IsNaN(d))
            {
                exponent = (SIGNALING_EXPONENT) >> 3;
                significand = 1;
            }

            _value = (byte) ((sign << 7) + (((int) exponent << 3) & 0x78) + ((int) significand & 0x7));
        }

        public float Value => ToFloat(this);

        static minifloat Epsilon => new minifloat { _value = 1 };
        static minifloat MaxValue => new minifloat { _value = (byte)(SIGNALING_EXPONENT - 1) };
        static minifloat NaN => new minifloat { _value = (byte)(SIGNALING_EXPONENT | 1) };
        static minifloat PositiveInfinity => new minifloat { _value = (byte)SIGNALING_EXPONENT };

        static uint asuint(float f) => BitConverter.ToUInt32(BitConverter.GetBytes(f),0);// *(uint*)&f;
        static float asfloat(uint u) => BitConverter.ToSingle(BitConverter.GetBytes(u),0);//*(float*)&u;
        static byte tobyte(bool b) => b ? (byte)1 : (byte)0;//*(byte*)&b;
        
        public static float ToFloat(minifloat q)
        {
            var sign = (q._value & 0x80) >> 7;
            var exponent = (q._value & 0x78) >> 3;
            var significand = q._value & 0x7;

            if (exponent == 0xf)
            {
                if (significand == 0 && sign == 0)
                    return float.PositiveInfinity;
                if (significand == 0 && sign == 1)
                    return float.NegativeInfinity;
                return float.NaN;
            }

            if (sign == 0)
                sign = 1;
            else
                sign = -1;

            if (exponent == 0)
            {
                var part1 = 0.125f * significand;
                return part1 * sign;
            }
            else
            {
                return (float) (0.0625 * (significand + 8) * Math.Pow(2, exponent)) * sign;
            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            minifloat otherobj = (minifloat) obj;
            if (obj is minifloat)
                return this._value.CompareTo(otherobj._value);
            else
                throw new ArgumentException("Object is not a quarter");
        }

        public TypeCode GetTypeCode()
        {
            return (TypeCode) 19;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            if (_value != 0)
                return true;
            return false;
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)ToFloat(this);
        }

        public char ToChar(IFormatProvider provider)
        {
            return (char)ToFloat(this);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return (decimal)ToFloat(this);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return (double)ToFloat(this);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (Int16)ToFloat(this);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (Int32)ToFloat(this);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return (Int64)ToFloat(this);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)ToFloat(this);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ToFloat(this);
        }
        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return ToFloat(this).ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return (ushort) ToFloat(this);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return (uint) ToFloat(this);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return (ulong) ToFloat(this);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ToString(formatProvider);
        }

        public static void test()
        {
            for (int a = 0; a <= byte.MaxValue; a++)
            {
                var fl = new minifloat() { _value = (byte)a };
                Console.Write(ToFloat(fl) + "\t");
                if ((a+1) % 8 == 0)
                    Console.WriteLine();

                minifloat fl2 = fl.Value;

                if(fl._value != fl2._value)
                    Debugger.Break();
            }

            minifloat f0 = 0.299;
            minifloat f1 = 1;
            minifloat f2 = 22.2; // 43    2b
            minifloat f3 = 333.3; // 74   4a
            minifloat f4 = 3.14159265359;

            {
                var a1 = f0 + f1;
                var a2 = f0 / f1;
                var a3 = f0 - f1;
                var a4 = f0 * f1;
                var a5 = f0 % f1;
            }
            {
                var a1 = f0 + 1.1;
                var a2 = f0 / 1.1;
                var a3 = f0 - 1.1;
                var a4 = f0 * 1.1;
                var a5 = f0 % 1.1;
            }
        }

        public static implicit operator minifloat(double v)
        {
            return new minifloat(v);
        }

        public static minifloat operator +(minifloat a, minifloat b) => a.Value + b.Value;
        public static minifloat operator -(minifloat a, minifloat b) => a.Value - b.Value;
        public static minifloat operator /(minifloat a, minifloat b) => a.Value / b.Value;
        public static minifloat operator *(minifloat a, minifloat b) => a.Value * b.Value;
        public static minifloat operator %(minifloat a, minifloat b) => a.Value % b.Value;

        public static minifloat operator ++(minifloat a) => a._value++;

        public static minifloat operator --(minifloat a) => a._value--;

    }
}
