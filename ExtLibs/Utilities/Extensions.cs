using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public static class Extensions
    {
        public static string TrimUnPrintable(this string input)
        {
            return Regex.Replace(input, @"[^\u0020-\u007E]", string.Empty);
        }

        public static async void Async(this Action function)
        {
            await Task.Run(() => { function(); });
        }
    }
}
