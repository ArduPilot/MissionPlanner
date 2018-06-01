using System;
using System.ComponentModel;
using System.Globalization;

namespace SharpKml.Base
{
    /// <summary>Represents a color, stored in the KML AABBGGRR format.</summary>
    [ImmutableObject(true)]
    public struct Color32 : IComparable<Color32>, IEquatable<Color32>
    {
        private readonly uint _abgr; // Stored in the standard aabbggrr KML format.

        /// <summary>
        /// Initializes a new instance of the Color32 struct to the specified
        /// ABGR value.
        /// </summary>
        /// <param name="abgr">An integer containing the ABGR color value.</param>
        public Color32(int abgr)
        {
            _abgr = (uint)abgr;
        }

        public Color32(System.Drawing.Color color) :
            this((byte)color.A, (byte)color.B, (byte)color.G, (byte)color.R)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Color32 struct to the specified
        /// color components.
        /// </summary>
        /// <param name="alpha">The alpha component value.</param>
        /// <param name="blue">The blue component value.</param>
        /// <param name="green">The green component value.</param>
        /// <param name="red">The red component value.</param>
        public Color32(byte alpha, byte blue, byte green, byte red)
        {
            _abgr = (uint)(alpha << 24) | (uint)(blue << 16) | (uint)(green << 8) | (uint)red;
        }

        /// <summary>Gets the Color32 as an integer in ABGR format.</summary>
        public int Abgr
        {
            get { return (int)_abgr; }
        }

        /// <summary>Gets the alpha component value.</summary>
        public byte Alpha
        {
            get { return (byte)(_abgr >> 24); }
            ////set { _abgr = (_abgr & 0x00FFFFFF) | ((uint)value << 24); }
        }

        /// <summary>Gets the Color32 as an integer in ARGB format.</summary>
        public int Argb
        {
            get
            {
                return (int)((_abgr & 0xFF000000) |
                       ((_abgr & 0x00FF0000) >> 16) |
                       (_abgr & 0x0000FF00) |
                       ((_abgr & 0x000000FF) << 16));
            }
        }

        /// <summary>Gets the blue component value.</summary>
        public byte Blue
        {
            get { return (byte)(_abgr >> 16); }
            ////set { _abgr = (_abgr & 0xFF00FFFF) | ((uint)value << 16); }
        }

        /// <summary>Gets the green component value.</summary>
        public byte Green
        {
            get { return (byte)(_abgr >> 8); }
            ////set { _abgr = (_abgr & 0xFFFF00FF) | ((uint)value << 8); }
        }

        /// <summary>Gets the red component value.</summary>
        public byte Red
        {
            get { return (byte)_abgr; }
            ////set { _abgr = (_abgr & 0xFFFFFF00) | (uint)value; }
        }

        /// <summary>
        /// Determines whether two specified Color32s have the same value.
        /// </summary>
        /// <param name="colorA">The first Color32 to compare.</param>
        /// <param name="colorB">The second Color32 to compare.</param>
        /// <returns>
        /// true if the value of the two colors is the same; otherwise, false.
        /// </returns>
        public static bool operator ==(Color32 colorA, Color32 colorB)
        {
            // Can't use (a == null) because that would call this method again!
            if (object.ReferenceEquals(colorA, null))
            {
                return object.ReferenceEquals(colorB, null);
            }
            return colorA.Equals(colorB);
        }

        /// <summary>
        /// Determines whether two specified Color32s have different values.
        /// </summary>
        /// <param name="colorA">The first Color32 to compare.</param>
        /// <param name="colorB">The second Color32 to compare.</param>
        /// <returns>
        /// true if the value of the colors is different; otherwise, false.
        /// </returns>
        public static bool operator !=(Color32 colorA, Color32 colorB)
        {
            return !(colorA == colorB);
        }

        /// <summary>
        /// Determines whether the first specified Color32 is less than the second.
        /// </summary>
        /// <param name="colorA">The first Color32 to compare.</param>
        /// <param name="colorB">The second Color32 to compare.</param>
        /// <returns>
        /// true is the value of colorA is less than the value of colorB;
        /// otherwise, false.
        /// </returns>
        public static bool operator <(Color32 colorA, Color32 colorB)
        {
            if (colorA == null)
            {
                return false;
            }
            return colorA.CompareTo(colorB) < 0;
        }

        /// <summary>
        /// Determines whether the first specified Color32 is greater than the second.
        /// </summary>
        /// <param name="colorA">The first Color32 to compare.</param>
        /// <param name="colorB">The second Color32 to compare.</param>
        /// <returns>
        /// true is the value of colorA is greater than the value of colorB;
        /// otherwise, false.
        /// </returns>
        public static bool operator >(Color32 colorA, Color32 colorB)
        {
            if (colorA == null)
            {
                return false;
            }
            return colorA.CompareTo(colorB) > 0;
        }

        /// <summary>
        /// Converts the string representation of a color hex value to a Color32.
        /// </summary>
        /// <param name="value">
        /// A string containing a hex color value to convert.
        /// </param>
        /// <returns>A Color32 representing the value parameter.</returns>
        public static Color32 Parse(string value)
        {
            if (value == null)
            {
                return new Color32(0);
            }
            uint converted = 0;
            int max = Math.Min(value.Length, 8); // We consider only the first eight characters significant.
            for (int i = 0; i < max; ++i)
            {
                // Always increase the color, even if the char isn't a valid number
                converted <<= 4; // Move along one hex - 2^4
                string letter = value[i].ToString();
                uint number;
                if (uint.TryParse(letter, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
                {
                    converted += number;
                }
            }
            return new Color32((int)converted);
        }

        /// <summary>Creates a copy of this instance.</summary>
        /// <returns>
        /// A new Color32 instance with the same value of this instance.
        /// </returns>
        public Color32 Clone()
        {
            return new Color32((int)_abgr);
        }

        /// <summary>
        /// Compares this instance with a specified Color32 and indicates
        /// whether this instance precedes, follows, or appears in the same
        /// position in the sort order as the specified Color32.
        /// </summary>
        /// <param name="other">The Color32 to compare with this instance.</param>
        /// <returns>
        /// <list type="table"><item>
        /// <term>Less than zero</term>
        /// <description>This instance proceeds the value parameter.</description>
        /// </item><item>
        /// <term>Zero</term>
        /// <description>
        /// This instance has the same position in the sort order as the
        /// value parameter.
        /// </description>
        /// </item><item>
        /// <term>Greater than zero</term>
        /// <description>
        /// This instance follows the value parameter or the value parameter is null.
        /// </description>
        /// </item></list>
        /// </returns>
        public int CompareTo(Color32 other)
        {
            if (other == null)
            {
                return 1;
            }
            return _abgr.CompareTo(other._abgr);
        }

        /// <summary>
        /// Determines whether this instance and the specified Color32 have the
        /// same value.
        /// </summary>
        /// <param name="other">The Color32 to compare to this instance.</param>
        /// <returns>
        /// true if the value of the value parameter is the same as this instance;
        /// otherwise, false.
        /// </returns>
        public bool Equals(Color32 other)
        {
            if (other == null)
            {
                return false;
            }
            return _abgr == other._abgr;
        }

        /// <summary>
        /// Determines whether this instance and the specified object have the
        /// same value.
        /// </summary>
        /// <param name="obj">
        /// An object, which must be a Color32, to compare to this instance.
        /// </param>
        /// <returns>
        /// true if the object is a Color32 and the value of the object is the
        /// same as this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            Color32? color = obj as Color32?;
            return color.HasValue && this.Equals(color.Value);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return _abgr.GetHashCode();
        }

        /// <summary>
        /// Converts this instance to a string representing the hex value in
        /// ABGR format.
        /// </summary>
        /// <returns>
        /// The string representation of this instance in ABGR format.
        /// </returns>
        public override string ToString()
        {
            return _abgr.ToString("x8", CultureInfo.InvariantCulture);
        }
    }
}
