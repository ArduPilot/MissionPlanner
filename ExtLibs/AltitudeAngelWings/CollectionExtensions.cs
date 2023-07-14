using System.Collections.Generic;
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

        public static string AsReadableList(this IEnumerable<string> list)
        {
            var builder = new StringBuilder();

            foreach (var item in list)
            {
                builder.Append(item);
                builder.Append(" ");
            }

            if (builder.Length == 0)
            {
                return string.Empty;
            }

            builder.Length -= " ".Length;
            return builder.ToString();
        }
    }
}
