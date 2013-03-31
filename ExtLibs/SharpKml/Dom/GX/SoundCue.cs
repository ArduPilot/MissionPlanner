using System;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Specifies a sound file to play, in MP3, M4A, or AAC format.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("SoundCue", KmlNamespaces.GX22Namespace)]
    public sealed class SoundCue : TourPrimitive
    {
        /// <summary>Gets or sets the location of the sound file.</summary>
        [KmlElement("href")]
        public Uri Href { get; set; }
    }
}
