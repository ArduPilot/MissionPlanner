using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AltitudeAngelWings
{
    public static class CollectionExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : class
        {
            TValue val;
            dict.TryGetValue(key, out val);

            return val;
        }

        public static string AsReadableList(this IEnumerable<string> items, string separator = ", ", string lastSeparator = " and ")
        {
            var list = items?.ToList();
            if (list == null || list.Count == 0)
            {
                return string.Empty;
            }

            if (list.Count == 1)
            {
                return list[0];
            }

            var builder = new StringBuilder();
            for (var index = 0; index < list.Count - 1; index++)
            {
                builder.Append(list[index]);
                if (index < list.Count - 2)
                {
                    builder.Append(separator);
                }
            }

            builder.Append(lastSeparator);
            builder.Append(list[list.Count - 1]);

            return builder.ToString();
        }
    }
}
