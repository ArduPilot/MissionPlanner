using System;

namespace RFDLib
{
    public static class Text
    {
        /// <summary>
        /// Case-insensitive contains.
        /// </summary>
        /// <param name="Haystack"></param>
        /// <param name="Needle"></param>
        /// <returns></returns>
        public static bool Contains(string Haystack, string Needle)
        {
            return Haystack.IndexOf(Needle, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Returns whether the given character is an upper or lower case letter of the alphabet.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckIsLetter(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
        }

        /// <summary>
        /// Returns whether the given character is a decimal numeral.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool CheckIsNumeral(char c)
        {
            return (c >= '0' && c <= '9');
        }

        public static bool CheckIsHexNumeral(char c)
        {
            return CheckIsNumeral(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }
    }
}