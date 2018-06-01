using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies how altitude components are interpreted.</summary>
    /// <remarks>OGC KML 2.2 Section 16.1</remarks>
    public enum AltitudeMode
    {
        /// <summary>Ignore the altitude specification.</summary>
        [KmlElement("clampToGround")]
        ClampToGround = 0,

        /// <summary>
        /// Interpret the altitude in meters relative to the terrain elevation.
        /// </summary>
        [KmlElement("relativeToGround")]
        RelativeToGround,

        /// <summary>
        /// Interpret the altitude as a value in meters relative to the
        /// vertical datum.
        /// </summary>
        [KmlElement("absolute")]
        Absolute
    }
}
