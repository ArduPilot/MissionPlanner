using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using MissionPlanner.Utilities.Drawing;

namespace GMap.NET.Drawing
{
    public static class Extensions
    {
        public static Bitmap ToBitmap(this byte[] input)
        {
            return (Bitmap)Image.FromStream(new MemoryStream(input));
        }
    }
}
