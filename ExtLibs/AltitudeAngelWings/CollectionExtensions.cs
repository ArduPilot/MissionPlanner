using System.Collections.Generic;

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
    }
}
