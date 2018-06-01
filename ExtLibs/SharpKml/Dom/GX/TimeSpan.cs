using System;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Copy of <see cref="Dom.TimeSpan"/> in the extension namespace.
    /// </summary>
    /// <remarks>
    /// <para>This is not part of the OGC KML 2.2 standard.</para>
    /// <para>This class allows for the inclusion of time values in
    /// <see cref="AbstractView"/>.</para>
    /// </remarks>
    [KmlElement("TimeSpan", KmlNamespaces.GX22Namespace)]
    public sealed class TimeSpan : TimePrimitive
    {
        /// <summary>Gets or sets the beginning instant of a time period.</summary>
        [KmlElement("begin", 1)]
        public DateTime? Begin { get; set; }

        /// <summary>Gets or sets the ending instant of a time period.</summary>
        [KmlElement("end", 2)]
        public DateTime? End { get; set; }
    }
}
