using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>Controls changes during a tour to KML features.</summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("AnimatedUpdate", KmlNamespaces.GX22Namespace)]
    public sealed class AnimatedUpdate : TourPrimitive
    {
        private Update _update;

        /// <summary>Gets or sets the amount of time, in seconds.</summary>
        [KmlElement("duration", KmlNamespaces.GX22Namespace)]
        public double? Duration { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="Update"/> of this instance.
        /// </summary>
        [KmlElement(null)]
        public Update Update
        {
            get { return _update; }
            set { this.UpdatePropertyChild(value, ref _update); }
        }
    }
}
