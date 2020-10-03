using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.Drawing
{
    public class ColorTranslator
    {
        public static int ToWin32(Color foreColor)
        {
            return foreColor.ToArgb();
        }

        public static Color FromHtml(string htmlColor)
        {
            Color value = Color.Empty;
            if (htmlColor == null || htmlColor.Length == 0)
            {
                return value;
            }

            if (htmlColor[0] == '#' && (htmlColor.Length == 7 || htmlColor.Length == 4))
            {
                if (htmlColor.Length == 7)
                {
                    value = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                        Convert.ToInt32(htmlColor.Substring(3, 2), 16), Convert.ToInt32(htmlColor.Substring(5, 2), 16));
                }
                else
                {
                    string text = char.ToString(htmlColor[1]);
                    string text2 = char.ToString(htmlColor[2]);
                    string text3 = char.ToString(htmlColor[3]);
                    value = Color.FromArgb(Convert.ToInt32(text + text, 16), Convert.ToInt32(text2 + text2, 16),
                        Convert.ToInt32(text3 + text3, 16));
                }
            }

            if (value.IsEmpty && string.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
            {
                value = Color.LightGray;
            }

            if (value.IsEmpty)
            {
                if (s_htmlSysColorTable == null)
                {
                    InitializeHtmlSysColorTable();
                }

                s_htmlSysColorTable.TryGetValue(htmlColor.ToLower(CultureInfo.InvariantCulture), out value);
            }

            if (value.IsEmpty)
            {
                try
                {
                    return typeof(SystemColors).GetProperties(BindingFlags.Public | BindingFlags.Static)
                        .Where(a => a.PropertyType == typeof(Color) && a.Name.ToLower() == htmlColor.ToLower())
                        .Select(a => (Color) a.GetValue(null)).First();

                }
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message, "htmlColor", ex);
                }
            }

            return value;
        }

        public static Color FromOle(int oleColor)
        {
            if ((int) (oleColor & 4278190080u) == int.MinValue && (oleColor & 0xFFFFFF) <= 24)
            {
                switch (oleColor)
                {
                }
            }

            return Color.FromArgb((byte) (oleColor & 0xFF), (byte) ((oleColor >> 8) & 0xFF),
                (byte) ((oleColor >> 16) & 0xFF));
        }

        public static Color FromWin32(int win32Color)
        {
            return FromOle(win32Color);
        }

        public static string ToHtml(Color c)
        {
            string result = string.Empty;
            if (c.IsEmpty)
            {
                return result;
            }

            return result;
        }

        private static Dictionary<string, Color> s_htmlSysColorTable;


        private static void InitializeHtmlSysColorTable()
        {
            s_htmlSysColorTable = new Dictionary<string, Color>(27);
        }
    }
}