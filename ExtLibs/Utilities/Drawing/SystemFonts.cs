using System.Geometry.Text;
using SkiaSharp;

namespace MissionPlanner.Utilities.Drawing
{
    public class SystemFonts
    {
        public static SKPaint DefaultFont { get; set; } = new SKPaint() {Typeface = SKTypeface.Default, TextSize = 12}; // points
    }
}