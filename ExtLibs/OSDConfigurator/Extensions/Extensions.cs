using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSDConfigurator.Extensions
{
    public static class Extensions
    {
        public static bool TryParseScreenAndIndex(this string parameterName, out byte screen, out byte index)
        {
            var match = Regex.Match(parameterName, "OSD(\\d)\\S+(\\d)\\S+");

            if (match.Success && match.Groups.Count > 2)
            {
                screen = byte.Parse(match.Groups[1].Value);
                index = byte.Parse(match.Groups[2].Value);
                return true;
            }

            screen = index = 0;
            return false;
        }
    }
}
