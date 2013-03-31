using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the projection mode for a shape.</summary>
    /// <remarks>OGC KML 2.2 Section 16.17</remarks>
    public enum Shape
    {
        /// <summary>Represents an ordinary photo.</summary>
        [KmlElement("rectangle")]
        Rectangle = 0,

        /// <summary>
        /// Represents panoramas, which can be either partial or full cylinders.
        /// </summary>
        [KmlElement("cylinder")]
        Cylinder,

        /// <summary>Represents spherical panoramas.</summary>
        [KmlElement("sphere")]
        Sphere
    }
}
