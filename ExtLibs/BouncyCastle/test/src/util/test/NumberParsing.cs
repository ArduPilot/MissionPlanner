using System;
using System.Globalization;

namespace Org.BouncyCastle.Utilities.Test
{
    /**
    * Parsing
    */
    public sealed class NumberParsing
    {
        private NumberParsing()
        {
            // Hide constructor
        }

		public static long DecodeLongFromHex(
			string longAsString)
        {
            if ((longAsString[1] == 'x')
                || (longAsString[1] == 'X'))
            {
                longAsString = longAsString.Substring(2);
            }

			return long.Parse(longAsString, NumberStyles.HexNumber);
        }

		public static int DecodeIntFromHex(
			string intAsString)
        {
            if ((intAsString[1] == 'x')
                || (intAsString[1] == 'X'))
            {
                intAsString = intAsString.Substring(2);
            }

			return int.Parse(intAsString, NumberStyles.HexNumber);
        }
    }
}
