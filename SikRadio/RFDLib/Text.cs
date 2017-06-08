using System;

namespace RFDLib
{
    public static class Text
    {
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
    }
}