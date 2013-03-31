using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utils
{
    public class MiscUtils
    {
        public static T ParseEnum<T>(string name) {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static void Swap<T>(ref T a, ref T b) {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
