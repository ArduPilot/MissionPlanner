using System.Drawing;

namespace System.Drawing
{
    public class ColorTranslator
    {
        public static int ToWin32(Color foreColor)
        {
            return foreColor.ToArgb();
        }
    }
}
