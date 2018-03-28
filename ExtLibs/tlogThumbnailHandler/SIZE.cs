using System.Runtime.InteropServices;

namespace tlogThumbnailHandler
{
    [ComVisible(false)]
    public struct SIZE
    {
        public int cx;
        public int cy;

        public SIZE(int cx, int cy)
        {
            this.cx = cx;
            this.cy = cy;
        }
    }
}