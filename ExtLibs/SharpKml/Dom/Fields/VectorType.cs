using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies an image coordinate system.</summary>
    /// <remarks>OGC KML 2.2 Section 16.21</remarks>
    public abstract class VectorType : Element
    {
        /// <summary>The default value that should be used for <see cref="X"/>.</summary>
        public const double DefaultX = 1.0;

        /// <summary>The default value that should be used for <see cref="Y"/>.</summary>
        public const double DefaultY = 1.0;

        /// <summary>Gets or sets the x component of a point.</summary>
        [KmlAttribute("x")]
        public double? X { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Unit"/> in which the x value is specified.
        /// </summary>
        [KmlAttribute("xunits")]
        public Unit? XUnits { get; set; }

        /// <summary>Gets or sets the y component of a point.</summary>
        [KmlAttribute("y")]
        public double? Y { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Unit"/> in which the y value is specified.
        /// </summary>
        [KmlAttribute("yunits")]
        public Unit? YUnits { get; set; }
    }
}
