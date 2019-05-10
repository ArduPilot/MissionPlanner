using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MissionPlanner.Utilities.Drawing
{
    public class ColorTranslator
    {
        public static int ToWin32(Color foreColor)
        {
            return foreColor.ToArgb();
        }
    }
}
