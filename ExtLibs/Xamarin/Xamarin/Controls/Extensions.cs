using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GMap.NET.Drawing
{
    public static class Extensions
    {
        public static Bitmap ToBitmap99(this byte[] input)
        {
            return (Bitmap)Image.FromStream(new MemoryStream(input));
        }
    }
}


public class ThemeManager
{
    public static Color ControlBGColor;

    public static void ApplyThemeTo(Control textControl)
    {
        
    }
}

namespace MissionPlanner.Controls
{


    public class MyLabel : Label
    {
        internal bool resize;
    }
}