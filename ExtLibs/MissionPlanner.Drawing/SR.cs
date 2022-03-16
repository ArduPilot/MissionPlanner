using System.Globalization;

namespace System.Drawing
{
    internal static class SR
    {
        internal static string Format(string resourceFormat, params object[] args)
        {
            if (args != null)
            {
                return string.Format(CultureInfo.InvariantCulture, resourceFormat, args);
            }
            return resourceFormat;
        }

        internal static string Format(string resourceFormat, object p1)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, resourceFormat, p1);
        }

        internal static string Format(string resourceFormat, object p1, object p2)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, resourceFormat, p1, p2);
        }

        internal static string Format(string resourceFormat, object p1, object p2, object p3)
        {
            return string.Format((IFormatProvider)CultureInfo.InvariantCulture, resourceFormat, p1, p2, p3);
        }
    }
}