namespace SharpKml.Dom
{
    /// <summary>Represents a KML AbstractTimePrimitiveGroup.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 15.1</para>
    /// <para>All DateTime instances in TimePrimitive and its descendants should
    /// be in the UTC form (i.e. have their <see cref="System.DateTime.Kind"/>
    /// set to <see cref="System.DateTimeKind.Utc"/>).</para>
    /// </remarks>
    public abstract class TimePrimitive : KmlObject
    {
        // Intentionally left blank - this is a base class only
    }
}
