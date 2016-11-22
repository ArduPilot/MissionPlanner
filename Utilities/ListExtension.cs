using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public static class ListExtension
    {
        public static void SortENABLE(this List<string> list)
        {
            try
            {
                list.Sort((a, b) =>
                {
                    return sorter(a, b);
                });
            } catch { }
        }

        static int sorter(string a, string b)
        {
            if (a == null || b == null) return 0;
            if (a.EndsWith("ENABLE") && b.EndsWith("ENABLE")) return a.CompareTo(b);
            if (a.EndsWith("ENABLE")) return -1;
            if (b.EndsWith("ENABLE")) return 1;
            return a.CompareTo(b);
        }
    }
}
