using System.Drawing;

namespace System.Drawing.Drawing2D
{
    public static class ExtensionsDrawing
    {
        public static PointF[] ToFloat(this Point[] p)
        {
            var pf = new PointF[p.Length];
            for (var i = 0; i < p.Length; ++i) pf[i] = new PointF(p[i].X, p[i].Y);

            return pf;
        }
    }
}