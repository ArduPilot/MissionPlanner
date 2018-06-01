using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Contains a mapping from SourceHref to TargetHref.</summary>
    /// <remarks>OGC KML 2.2 Section 10.14</remarks>
    [KmlElement("Alias")]
    public sealed class Alias : KmlObject
    {
        /// <summary>
        /// Gets or sets the path for the texture file within the textured 3D object.
        /// </summary>
        [KmlElement("sourceHref", 2)]
        public Uri SourceHref { get; set; }

        /// <summary>
        /// Gets or sets the textured 3D object file to be fetched by an earth browser.
        /// </summary>
        /// <remarks>
        /// This reference can be a relative reference to an image file within
        /// a KMZ file, or it can be an absolute reference to the file (e.g. a URL).
        /// </remarks>
        [KmlElement("targetHref", 1)]
        public Uri TargetHref { get; set; }
    }
}
