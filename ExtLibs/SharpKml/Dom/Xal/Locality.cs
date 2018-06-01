using SharpKml.Base;

namespace SharpKml.Dom.Xal
{
    /// <summary>Represents an area one level lower than administrative.</summary>
    /// <remarks>
    /// Typically this includes: cities, reservations and any other built-up areas.
    /// </remarks>
    [KmlElement("Locality", KmlNamespaces.XalNamespace)]
    public sealed class Locality : Element
    {
        private PostalCode _code;
        private Thoroughfare _thoroughfare;

        /// <summary>Gets or sets the name of the Locality.</summary>
        [KmlElement("LocalityName", KmlNamespaces.XalNamespace)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PostalCode"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public PostalCode PostalCode
        {
            get { return _code; }
            set { this.UpdatePropertyChild(value, ref _code); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Thoroughfare"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Thoroughfare Thoroughfare
        {
            get { return _thoroughfare; }
            set { this.UpdatePropertyChild(value, ref _thoroughfare); }
        }
    }
}
