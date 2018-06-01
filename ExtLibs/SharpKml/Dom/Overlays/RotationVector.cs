using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a point about which a rotation occurs.</summary>
    /// <remarks>OGC KML 2.2 Section 11.7.3.3</remarks>
    [KmlElement("rotationXY")]
    public sealed class RotationVector : VectorType
    {
        // Intentionally left blank - this is a simple concrete implementation of VectorType.
    }
}
