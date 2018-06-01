using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies the style state inside a <see cref="Pair"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.18</remarks>
    public enum StyleState
    {
        /// <summary>
        /// Specifies a normal style for a <see cref="Placemark"/>.
        /// </summary>
        [KmlElement("normal")]
        Normal = 0,

        /// <summary>
        /// Specifies a highlighted style for a <see cref="Placemark"/>.
        /// </summary>
        [KmlElement("highlight")]
        Highlight
    }
}
