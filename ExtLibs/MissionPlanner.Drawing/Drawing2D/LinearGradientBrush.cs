using System.Drawing;

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

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}