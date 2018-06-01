using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the drawing style for a line geometry.</summary>
    /// <remarks>OGC KML 2.2 Section 12.11</remarks>
    [KmlElement("LineStyle")]
    public sealed class LineStyle : ColorStyle
    {
        /// <summary>The default value that should be used for <see cref="Width"/>.</summary>
        public const double DefaultWidth = 1.0;

        /// <summary>Gets or sets the width of the line, in pixels.</summary>
        [KmlElement("width", 1)]
        public double? Width { get; set; }

		public LineStyle() {}

        public LineStyle(Color32 color, int width)
        {
            this.Color = color;
            this.Width = width;
        }
    }
}
