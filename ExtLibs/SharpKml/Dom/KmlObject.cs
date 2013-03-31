using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents an AbstractObjectGroup.</summary>
    /// <remarks>OGC KML 2.2 Section 8.1</remarks>
    public abstract class KmlObject : Element
    {
        /// <summary>Gets or sets the KML id attribute.</summary>
        /// <remarks>
        /// May be used to specify a unique identifier for the
        /// KmlObject within the KML document instance.
        /// </remarks>
        [KmlAttribute("id")]
        public string Id { get; set; }

        /// <summary>Gets or sets the KML targetId attribute.</summary>
        /// <remarks>
        /// The optional targetId attribute may be used to encode
        /// the id value of another KmlObject.
        /// </remarks>
        [KmlAttribute("targetId")]
        public string TargetId { get; set; }
    }
}
