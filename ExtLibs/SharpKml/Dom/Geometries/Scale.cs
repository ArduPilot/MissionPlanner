using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Scales a <see cref="Model"/> along the x, y, and z axes in the
    /// <c>Model</c>'s coordinate space.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.12</remarks>
    [KmlElement("Scale")]
    public sealed class Scale : KmlObject
    {
        /// <summary>The default value that should be used for <see cref="X"/>.</summary>
        public const double DefaultX = 1.0;

        /// <summary>The default value that should be used for <see cref="Y"/>.</summary>
        public const double DefaultY = 1.0;

        /// <summary>The default value that should be used for <see cref="Z"/>.</summary>
        public const double DefaultZ = 1.0;

        /// <summary>Gets or sets the scale factor along x axis.</summary>
        [KmlElement("x", 1)]
        public double? X { get; set; }

        /// <summary>Gets or sets the scale factor along y axis.</summary>
        [KmlElement("y", 2)]
        public double? Y { get; set; }

        /// <summary>Gets or sets the scale factor along z axis.</summary>
        [KmlElement("z", 3)]
        public double? Z { get; set; }
    }
}
