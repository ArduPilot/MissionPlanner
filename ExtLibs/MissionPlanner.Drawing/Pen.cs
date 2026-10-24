using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace System.Drawing
{
    public class Pen : ICloneable, IDisposable
    {
        private SKPaint _nativePen;

        internal SKPaint nativePen
        {
            get => _nativePen;
            set
            {
                _nativePen = value;
                Color = Color.FromArgb(_nativePen.Color.Alpha, _nativePen.Color.Red, _nativePen.Color.Green,
                    _nativePen.Color.Blue);
                Brush = new SolidBrush(Color);
            }
        }

        internal Pen()
        {
        }

        public Pen(Color color) : this(color.ToSKColor())
        {
        }

        public Pen(SKColor color, float width = 1)
        {
            Width = width;
            Color = Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
            
            try
            {
                var loader = SKTypeface.Default;

                Brush = new SolidBrush(Color);

                nativePen = new SKPaint()
                {
                    Color = color,
                    StrokeWidth = Width,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    BlendMode = SKBlendMode.SrcOver,
                    FilterQuality = SKFilterQuality.High
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public Pen(Color brush, float width) : this(brush.ToSKColor(), width)
        {
        }

        public Pen(Brush brush, float width) : this(brush._color, width)
        {
        }

        public Pen(Brush brush) : this(brush, 1)
        {
        }

        public LineJoin LineJoin { get; set; }
        public float Width { get; set; } = 2;
        public LineCap StartCap { get; set; }
        public DashStyle DashStyle { get; set; }
        public Color Color { get; set; } = Color.Black;
        public Brush Brush { get; set; }
        public float[] DashPattern { get; set; } = new float[] {1, 0};
        public PenAlignment Alignment { get; set; }

        public int MiterLimit { get; set; }

        public LineCap EndCap { get; set; }
        public CustomLineCap CustomStartCap { get; set; }
        public CustomLineCap CustomEndCap { get; set; }

        public object Clone()
        {
            return new Pen(nativePen.Color, Width);
        }

        private SKPathEffect _dashEffect;
        private DashStyle _dashEffectStyle;
        private float _dashEffectWidth;
        private float[] _dashEffectPattern;

        // set while nativePen carries a dash effect, so going back to Solid can clear it
        internal bool dashApplied;

        // GDI+ dash patterns, in multiples of the pen width
        private static readonly float[] DashIntervals = {3, 1};
        private static readonly float[] DotIntervals = {1, 1};
        private static readonly float[] DashDotIntervals = {3, 1, 1, 1};
        private static readonly float[] DashDotDotIntervals = {3, 1, 1, 1, 1, 1};

        /// <summary>
        /// Dash effect for DashStyle (DashPattern for Custom), scaled by the pen width as GDI+ does. Rebuilt
        /// only when the style, width or pattern changes.
        /// </summary>
        internal SKPathEffect GetDashEffect()
        {
            var style = DashStyle;
            var width = Math.Max(Width, 1);
            if (_dashEffect != null && _dashEffectStyle == style && _dashEffectWidth == width &&
                (style != DashStyle.Custom || SamePattern(DashPattern, _dashEffectPattern)))
                return _dashEffect;

            float[] unit;
            switch (style)
            {
                case DashStyle.Dash:
                    unit = DashIntervals;
                    break;
                case DashStyle.Dot:
                    unit = DotIntervals;
                    break;
                case DashStyle.DashDot:
                    unit = DashDotIntervals;
                    break;
                case DashStyle.DashDotDot:
                    unit = DashDotDotIntervals;
                    break;
                default:
                    unit = DashPattern;
                    break;
            }

            var intervals = new float[unit.Length];
            for (int i = 0; i < unit.Length; i++)
                intervals[i] = unit[i] * width;

            _dashEffect = SKPathEffect.CreateDash(intervals, 0);
            _dashEffectStyle = style;
            _dashEffectWidth = width;
            _dashEffectPattern = (float[]) DashPattern?.Clone();
            return _dashEffect;
        }

        private static bool SamePattern(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return a == b;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i])
                    return false;
            return true;
        }

        // set for the shared instances returned by Pens
        internal bool isSystemPen;

        public void Dispose()
        {
            if (isSystemPen)
                return;
            nativePen?.Dispose();
        }
    }
}