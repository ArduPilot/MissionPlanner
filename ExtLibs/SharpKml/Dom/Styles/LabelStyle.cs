using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how the name of a <see cref="Feature"/> is drawn in the
    /// geographic view.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 12.10</remarks>
    [KmlElement("LabelStyle")]
    public sealed class LabelStyle : ColorStyle
    {
        /// <summary>The default value that should be used for <see cref="Scale"/>.</summary>
        public const double DefaultScale = 1.0;

        /// <summary>
        /// Gets or sets a scale factor to be applied to the label.
        /// </summary>
        [KmlElement("scale", 1)]
        public double? Scale { get; set; }
    }
}
