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

        /// <summary>
        /// Update the given original dictionary with new values from NewValues.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="Original"></param>
        /// <param name="NewValues"></param>
        public static void Update<K, V>(Dictionary<K, V> Original, Dictionary<K, V> NewValues)
        {
            foreach (var kvp in NewValues)
            {
                Original[kvp.Key] = kvp.Value;
            }
        }
    }

}