using System.Globalization;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Numbers typed by the user. Both '.' and ',' are the decimal separator whatever
    /// the Windows regional format, so 12.34 is never read as 1234 on a comma locale.
    /// </summary>
    public static class DecimalInput
    {
        public static bool TryParse(string text, out float value)
        {
            return float.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        public static float Parse(string text)
        {
            return float.Parse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// True for text like "5.000" or "5,000": a thousands separator to many users,
        /// but read as 5 here.
        /// </summary>
        public static bool LooksLikeThousands(string text)
        {
            return Regex.IsMatch(text.Trim(), @"^\d+[.,]\d{3}$");
        }
    }
}
