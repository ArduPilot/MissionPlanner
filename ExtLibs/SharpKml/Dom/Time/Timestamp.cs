using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a single moment in time.</summary>
    /// <remarks>OGC KML 2.2 Section 15.3</remarks>
    [KmlElement("TimeStamp")]
    public sealed class Timestamp : TimePrimitive
    {
        /// <summary>Gets or sets the moment in time.</summary>
        [KmlElement("when", 1)]
        public DateTime? When { get; set; }
    }
}
