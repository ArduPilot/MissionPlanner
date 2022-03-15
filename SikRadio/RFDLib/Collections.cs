using System;
using System.Collections.Generic;

namespace RFDLib
{
    public static class Collections
    {
        public static Dictionary<K, V2> Translate<K, V1, V2>(Dictionary<K, V1> x, Func<V1, V2> TransLateFn)
        {
            Dictionary<K, V2> Result = new Dictionary<K, V2>();

            foreach (var kvp in x)
            {
                Result[kvp.Key] = TransLateFn(kvp.Value);
            }

            return Result;
        }
    }

}