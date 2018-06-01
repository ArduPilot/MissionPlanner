using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies where to begin numbering the tiles in a layer of an
    /// <see cref="ImagePyramid"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 16.12</remarks>
    public enum GridOrigin
    {
        /// <summary>
        /// Begin numbering the tiles in a layer from the lower left corner.
        /// </summary>
        [KmlElement("lowerLeft")]
        LowerLeft = 0,

        /// <summary>
        /// Begin numbering the tiles in a layer from the upper left corner.
        /// </summary>
        [KmlElement("upperLeft")]
        UpperLeft
    }
}
