using System.Drawing;
using SkiaSharp;

namespace System.Drawing.Drawing2D
{
    public class LinearGradientBrush : Brush
    {
        internal LinearGradientMode _gradMode;

        public LinearGradientBrush(RectangleF bg, Color skyColor1, Color skyColor2, LinearGradientMode gradMode)
        {
            _gradMode = gradMode;
            Rectangle = bg;
            LinearColors = new[] {skyColor1, skyColor2};
        }

        public LinearGradientBrush(Point TL, Point BR, Color skyColor1, Color skyColor2)
        {
            _gradMode = LinearGradientMode.Vertical;
            Rectangle = new RectangleF(TL, new SizeF(BR.X, BR.Y));
            LinearColors = new[] {skyColor1, skyColor2};
        }

        public LinearGradientBrush(Rectangle bg, Color skyColor1, Color skyColor2, float gradMode)
        {
            _gradMode = LinearGradientMode.Vertical;
            Rectangle = bg;
            LinearColors = new[] {skyColor1, skyColor2};
        }

        public RectangleF Rectangle { get; set; }
        public Color[] LinearColors { get; set; }

        public WrapMode WrapMode { get; set; }
        public ColorBlend InterpolationColors { get; set; }

        public Blend Blend { get; set; }
        public Matrix Transform { get; set; }

        private LinearGradientMode _paintMode;
        private RectangleF _paintRect;
        private Color _paintColor1, _paintColor2;

        /// <summary>
        /// Horizontal/vertical gradient paint, built once and rebuilt only when the mode, rectangle or
        /// colours change (Extension.ToSKPaint used to build a new paint and shader per fill). Kept in
        /// nativeBrush so Dispose() frees it.
        /// </summary>
        internal SKPaint GetPaint()
        {
            var rect = Rectangle;
            var color1 = LinearColors[0];
            var color2 = LinearColors[1];
            // Handle is zero once Dispose() has freed the paint; build a new one if drawn again
            if (nativeBrush != null && nativeBrush.Handle != IntPtr.Zero && _paintMode == _gradMode &&
                _paintRect == rect && _paintColor1 == color1 && _paintColor2 == color2)
                return nativeBrush;

            var end = _gradMode == LinearGradientMode.Horizontal
                ? new SKPoint(rect.Right, rect.Y)
                : new SKPoint(rect.X, rect.Bottom);
            var old = nativeBrush;
            nativeBrush = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Shader = SKShader.CreateLinearGradient(new SKPoint(rect.X, rect.Y), end,
                    new[] {color1.ToSKColor(), color2.ToSKColor()}, null, SKShaderTileMode.Clamp, SKMatrix.Identity)
            };
            _paintMode = _gradMode;
            _paintRect = rect;
            _paintColor1 = color1;
            _paintColor2 = color2;
            if (old != null && old.Handle != IntPtr.Zero)
                old.Dispose();
            return nativeBrush;
        }

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}