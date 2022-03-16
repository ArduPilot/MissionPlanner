using System;
using System.Text;
using System;
using System.ComponentModel;
using System.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Numerics.Hashing;

namespace System.Drawing
{
    // System.Drawing.Color


    [Serializable]
    [TypeConverter(typeof(ColorConverter))]
    public struct Color : IEquatable<Color>
    {
        public static readonly Color Empty;

        private readonly string name;

        private readonly long value;

        private readonly short knownColor;

        private readonly short state;

        public static Color Transparent => new Color(KnownColor.Transparent);

        public static Color AliceBlue => new Color(KnownColor.AliceBlue);

        public static Color Aqua => new Color(KnownColor.Aqua);

        public static Color Black => new Color(KnownColor.Black);

        public static Color BlanchedAlmond => new Color(KnownColor.BlanchedAlmond);

        public static Color Blue => new Color(KnownColor.Blue);

        public static Color BlueViolet => new Color(KnownColor.BlueViolet);

        public static Color Brown => new Color(KnownColor.Brown);

        public static Color BurlyWood => new Color(KnownColor.BurlyWood);

        public static Color CornflowerBlue => new Color(KnownColor.CornflowerBlue);

        public static Color Cyan => new Color(KnownColor.Cyan);

        public static Color DarkBlue => new Color(KnownColor.DarkBlue);

        public static Color DarkGray => new Color(KnownColor.DarkGray);

        public static Color DarkOrange => new Color(KnownColor.DarkOrange);

        public static Color DarkRed => new Color(KnownColor.DarkRed);

        public static Color DimGray => new Color(KnownColor.DimGray);

        public static Color DodgerBlue => new Color(KnownColor.DodgerBlue);

        public static Color Fuchsia => new Color(KnownColor.Fuchsia);

        public static Color Gainsboro => new Color(KnownColor.Gainsboro);

        public static Color Gray => new Color(KnownColor.Gray);

        public static Color Green => new Color(KnownColor.Green);

        public static Color HotPink => new Color(KnownColor.HotPink);

        public static Color Indigo => new Color(KnownColor.Indigo);

        public static Color LemonChiffon => new Color(KnownColor.LemonChiffon);

        public static Color LightBlue => new Color(KnownColor.LightBlue);

        public static Color LightGreen => new Color(KnownColor.LightGreen);

        public static Color LightGray => new Color(KnownColor.LightGray);

        public static Color LightSteelBlue => new Color(KnownColor.LightSteelBlue);

        public static Color Lime => new Color(KnownColor.Lime);

        public static Color LimeGreen => new Color(KnownColor.LimeGreen);

        public static Color Magenta => new Color(KnownColor.Magenta);

        public static Color Maroon => new Color(KnownColor.Maroon);

        public static Color MediumBlue => new Color(KnownColor.MediumBlue);

        public static Color MidnightBlue => new Color(KnownColor.MidnightBlue);

        public static Color Navy => new Color(KnownColor.Navy);

        public static Color Olive => new Color(KnownColor.Olive);

        public static Color OliveDrab => new Color(KnownColor.OliveDrab);

        public static Color Orange => new Color(KnownColor.Orange);

        public static Color PaleVioletRed => new Color(KnownColor.PaleVioletRed);

        public static Color PeachPuff => new Color(KnownColor.PeachPuff);

        public static Color Pink => new Color(KnownColor.Pink);

        public static Color Purple => new Color(KnownColor.Purple);

        public static Color Red => new Color(KnownColor.Red);

        public static Color RoyalBlue => new Color(KnownColor.RoyalBlue);

        public static Color SandyBrown => new Color(KnownColor.SandyBrown);

        public static Color SeaGreen => new Color(KnownColor.SeaGreen);

        public static Color Silver => new Color(KnownColor.Silver);

        public static Color Teal => new Color(KnownColor.Teal);

        public static Color Violet => new Color(KnownColor.Violet);

        public static Color Wheat => new Color(KnownColor.Wheat);

        public static Color White => new Color(KnownColor.White);

        public static Color WhiteSmoke => new Color(KnownColor.WhiteSmoke);

        public static Color Yellow => new Color(KnownColor.Yellow);

        public byte R => (byte) ((Value >> 16) & 0xFF);

        public byte G => (byte) ((Value >> 8) & 0xFF);

        public byte B => (byte) (Value & 0xFF);

        public byte A => (byte) ((Value >> 24) & 0xFF);

        public bool IsKnownColor => (state & 1) != 0;

        public bool IsEmpty => state == 0;

        public bool IsNamedColor
        {
            get
            {
                if ((state & 8) == 0)
                {
                    return IsKnownColor;
                }

                return true;
            }
        }

        public bool IsSystemColor
        {
            get
            {
                if (IsKnownColor)
                {
                    if (knownColor > 26)
                    {
                        return knownColor > 167;
                    }

                    return true;
                }

                return false;
            }
        }

        public string Name
        {
            get
            {
                if ((state & 8) != 0)
                {
                    return name;
                }

                if (IsKnownColor)
                {
                    return KnownColorTable.KnownColorToName((KnownColor) knownColor);
                }

                return Convert.ToString(value, 16);
            }
        }

        private long Value
        {
            get
            {
                if ((state & 2) != 0)
                {
                    return value;
                }

                if (IsKnownColor)
                {
                    return KnownColorTable.KnownColorToArgb((KnownColor) knownColor);
                }

                return 0L;
            }
        }

        internal Color(KnownColor knownColor)
        {
            value = 0L;
            state = 1;
            name = null;
            this.knownColor = (short) knownColor;
        }

        private Color(long value, short state, string name, KnownColor knownColor)
        {
            this.value = value;
            this.state = state;
            this.name = name;
            this.knownColor = (short) knownColor;
        }

        private static void CheckByte(int value, string name)
        {
            if (value < 0 || value > 255)
            {
                throw new ArgumentException(SR.Format(
                    "Value of '{1}' is not valid for '{0}'. '{0}' should be greater than or equal to {2} and less than or equal to {3}.",
                    name, value, 0, 255));
            }
        }

        private static long MakeArgb(byte alpha, byte red, byte green, byte blue)
        {
            return (long) (uint) ((red << 16) | (green << 8) | blue | (alpha << 24)) & 4294967295L;
        }

        public static Color FromArgb(int argb)
        {
            return new Color(argb & uint.MaxValue, 2, null, (KnownColor) 0);
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new Color(MakeArgb((byte) alpha, (byte) red, (byte) green, (byte) blue), 2, null, (KnownColor) 0);
        }

        public static Color FromArgb(int alpha, Color baseColor)
        {
            CheckByte(alpha, "alpha");
            return new Color(MakeArgb((byte) alpha, baseColor.R, baseColor.G, baseColor.B), 2, null, (KnownColor) 0);
        }

        public static Color FromArgb(int red, int green, int blue)
        {
            return FromArgb(255, red, green, blue);
        }

        public static Color FromName(string name)
        {
            if (ColorTable.TryGetNamedColor(name, out Color result))
            {
                return result;
            }

            return new Color(0L, 8, name, (KnownColor) 0);
        }

        public float GetBrightness()
        {
            float num = (float) (int) R / 255f;
            float num2 = (float) (int) G / 255f;
            float num3 = (float) (int) B / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            else if (num2 < num5)
            {
                num5 = num2;
            }

            if (num3 > num4)
            {
                num4 = num3;
            }
            else if (num3 < num5)
            {
                num5 = num3;
            }

            return (num4 + num5) / 2f;
        }

        public float GetHue()
        {
            if (R == G && G == B)
            {
                return 0f;
            }

            float num = (float) (int) R / 255f;
            float num2 = (float) (int) G / 255f;
            float num3 = (float) (int) B / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            else if (num2 < num5)
            {
                num5 = num2;
            }

            if (num3 > num4)
            {
                num4 = num3;
            }
            else if (num3 < num5)
            {
                num5 = num3;
            }

            float num6 = num4 - num5;
            float num7 = (num == num4)
                ? ((num2 - num3) / num6)
                : ((num2 != num4) ? (4f + (num - num2) / num6) : (2f + (num3 - num) / num6));
            num7 *= 60f;
            if (num7 < 0f)
            {
                num7 += 360f;
            }

            return num7;
        }

        public float GetSaturation()
        {
            float num = (float) (int) R / 255f;
            float num2 = (float) (int) G / 255f;
            float num3 = (float) (int) B / 255f;
            float result = 0f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            else if (num2 < num5)
            {
                num5 = num2;
            }

            if (num3 > num4)
            {
                num4 = num3;
            }
            else if (num3 < num5)
            {
                num5 = num3;
            }

            if (num4 != num5)
            {
                result = ((!((double) ((num4 + num5) / 2f) <= 0.5))
                    ? ((num4 - num5) / (2f - num4 - num5))
                    : ((num4 - num5) / (num4 + num5)));
            }

            return result;
        }

        public int ToArgb()
        {
            return (int) Value;
        }

        public override string ToString()
        {
            if ((state & 8) != 0 || (state & 1) != 0)
            {
                return "Color [" + Name + "]";
            }

            if ((state & 2) != 0)
            {
                return "Color [A=" + A.ToString() + ", R=" + R.ToString() + ", G=" + G.ToString() + ", B=" +
                       B.ToString() + "]";
            }

            return "Color [Empty]";
        }

        public static bool operator ==(Color left, Color right)
        {
            if (left.value == right.value && left.state == right.state && left.knownColor == right.knownColor)
            {
                return left.name == right.name;
            }

            return false;
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Color)
            {
                return Equals((Color) obj);
            }

            return false;
        }

        public bool Equals(Color other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            if ((name != null) & !IsKnownColor)
            {
                return name.GetHashCode();
            }

            return System.Numerics.Hashing.HashHelpers.Combine(
                System.Numerics.Hashing.HashHelpers.Combine(value.GetHashCode(), state.GetHashCode()),
                knownColor.GetHashCode());
        }
    }
    // System.Drawing.ColorConverter
}