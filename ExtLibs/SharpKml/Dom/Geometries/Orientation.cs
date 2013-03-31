using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the orientation of the model coordinate axes relative to a
    /// local earth-fixed reference frame.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 10.11</remarks>
    [KmlElement("Orientation")]
    public sealed class Orientation : KmlObject
    {
        /// <summary>Gets or sets the rotation about the z axis.</summary>
        /// <remarks>
        /// A value of 0 equals North. A positive rotation is counter clockwise
        /// around the positive z axis, looking along the z-axis away from the
        /// origin, and specified in decimal degrees from 0 to ±180.
        /// </remarks>
        [KmlElement("heading", 1)]
        public double? Heading { get; set; }

        /// <summary>Gets or sets the rotation about the y axis.</summary>
        /// <remarks>
        /// A positive rotation is counter clockwise around the positive y axis
        /// and specified in decimal degrees from 0 to ±180.
        /// </remarks>
        [KmlElement("roll", 3)]
        public double? Roll { get; set; }

        /// <summary>Gets or sets the rotation about the x axis.</summary>
        /// <remarks>
        /// A positive rotation is counter clockwise around the positive x axis
        /// and specified in decimal degrees from 0 to ±180.
        /// </remarks>
        [KmlElement("tilt", 2)]
        public double? Tilt { get; set; }
    }
}
