using MissionPlanner.Drawing;
using System.IO;

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
