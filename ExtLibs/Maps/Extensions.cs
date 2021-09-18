using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Maps
{
    public static class ExtensionsMaps
    {
        public static Color ColorFromHex(this string hex)    
        {
            var hash = hex.IndexOf("#");
            if (hash >= 0)
            {
                hex = hex.Substring(hash + 1);
            }
            return Color.FromArgb(Convert.ToByte(hex.Substring(0, 2), 16), Convert.ToByte(hex.Substring(2, 2), 16), Convert.ToByte(hex.Substring(4, 2), 16));
        }
    }
}
