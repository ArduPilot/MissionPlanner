using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>Enables controlled flights through geospatial data.</summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Tour", KmlNamespaces.GX22Namespace)]
    public sealed class Tour : Feature
    {
        private Playlist _playlist;

        /// <summary>
        /// Gets or sets the associated <see cref="Playlist"/> of this instance.
        /// </summary>
        [KmlElement(null)]
        public Playlist Playlist
        {
            get { return _playlist; }
            set { this.UpdatePropertyChild(value, ref _playlist); }
        }
    }
}
