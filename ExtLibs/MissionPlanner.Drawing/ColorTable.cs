using System.Collections.Generic;
using System.Reflection;

namespace System.Drawing
{
    internal static class ColorTable
    {
        private static readonly Lazy<Dictionary<string, Color>> s_colorConstants = new Lazy<Dictionary<string, Color>>((Func<Dictionary<string, Color>>)GetColors);

        internal static Dictionary<string, Color> Colors => s_colorConstants.Value;

        private static Dictionary<string, Color> GetColors()
        {
            Dictionary<string, Color> dictionary = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);
            FillConstants(dictionary, typeof(Color));
            FillConstants(dictionary, typeof(SystemColors));
            return dictionary;
        }

        private static void FillConstants(Dictionary<string, Color> colors, Type enumType)
        {
            PropertyInfo[] properties = enumType.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (propertyInfo.PropertyType == typeof(Color))
                {
                    colors[propertyInfo.Name] = (Color)propertyInfo.GetValue(null, null);
                }
            }
        }

        internal static bool TryGetNamedColor(string name, out Color result)
        {
            return Colors.TryGetValue(name, out result);
        }
    }
}