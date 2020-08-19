namespace System.Drawing.Drawing2D
{
    public sealed class PathGradientBrush : Brush
    {
        public PathGradientBrush(GraphicsPath ellipsePath)
        {
        }

        public PointF CenterPoint { get; set; }

        public Color CenterColor { get; set; }

        public Color[] SurroundColors { get; set; }

        public PointF FocusScales { get; set; }
    }
}