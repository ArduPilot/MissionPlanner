using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Specifies a point in space to which the browser will fly during a tour.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("FlyTo", KmlNamespaces.GX22Namespace)]
    public sealed class FlyTo : TourPrimitive
    {
        private AbstractView _view;

        /// <summary>Gets or sets the amount of time, in seconds.</summary>
        [KmlElement("duration", KmlNamespaces.GX22Namespace)]
        public double? Duration { get; set; }

        /// <summary>Gets or sets the method of flight.</summary>
        [KmlElement("flyToMode", KmlNamespaces.GX22Namespace)]
        public FlyToMode? Mode { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="AbstractView"/> of this instance.
        /// </summary>
        [KmlElement(null)]
        public AbstractView View
        {
            get { return _view; }
            set { this.UpdatePropertyChild(value, ref _view); }
        }
    }
}
