using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies a point relative to the screen origin that an image is mapped to.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.7.3.2</remarks>
    [KmlElement("screenXY")]
    public sealed class ScreenVector : VectorType
    {
        // Intentionally left blank - this is a simple concrete implementation of VectorType.
    }
}
