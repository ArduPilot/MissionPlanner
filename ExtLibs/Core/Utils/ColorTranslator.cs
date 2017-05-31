using System;
using System.Drawing;

namespace Core.Utils
{
    public class ColorTranslator
    {
        public static Color FromHtml(string htmlColor)
        {
            Color result = Color.Empty;
            if (htmlColor == null || htmlColor.Length == 0)
            {
                return result;
            }
            if (htmlColor[0] == '#' && (htmlColor.Length == 7 || htmlColor.Length == 4))
            {
                if (htmlColor.Length == 7)
                {
                    result = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                        Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                        Convert.ToInt32(htmlColor.Substring(5, 2), 16));
                }
                else
                {
                    string text = char.ToString(htmlColor[1]);
                    string text2 = char.ToString(htmlColor[2]);
                    string text3 = char.ToString(htmlColor[3]);
                    result = Color.FromArgb(Convert.ToInt32(text + text, 16), Convert.ToInt32(text2 + text2, 16),
                        Convert.ToInt32(text3 + text3, 16));
                }
            }

            return result;
        }

        public static string ToHtml(Color c)
        {
            var result = "#" + c.R.ToString("X2", null) + c.G.ToString("X2", null) + c.B.ToString("X2", null);

            return result;
        }
    }
}