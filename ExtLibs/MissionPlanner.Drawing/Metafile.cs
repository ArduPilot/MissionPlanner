using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing
{
    public class Metafile : Image
    {
        public Metafile(IntPtr hdc, EmfType emfPlusOnly)
        {
        }

        public Metafile(Stream stream, IntPtr emfPlusOnly, RectangleF rect, MetafileFrameUnit pixel,
            EmfType emfPlusDual)
        {
        }

        public IntPtr GetHenhmetafile()
        {
            return IntPtr.Zero;
        }
    }
}