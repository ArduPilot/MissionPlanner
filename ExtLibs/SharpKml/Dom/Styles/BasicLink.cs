using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a resource location.</summary>
    /// <remarks>OGC KML 2.2 Section 12.9</remarks>
    public abstract class BasicLink : KmlObject
    {
        /// <summary>Gets or sets the resource location.</summary>
        /// <remarks>
        /// The URL may contain a fragment component that allows indirect
        /// identification of some portion or subset of a resource.
        /// </remarks>
        [KmlElement("href", 1)]
        public Uri Href { get; set; }
    }
}
