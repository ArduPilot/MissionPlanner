using SharpKml.Base;

namespace SharpKml.Dom.Xal
{
    /// <summary>Represents a country.</summary>
    [KmlElement("Country", KmlNamespaces.XalNamespace)]
    public sealed class Country : Element
    {
        private AdministrativeArea _area;

        /// <summary>
        /// Gets or sets the <see cref="AdministrativeArea"/> associated with
        /// this instance.
        /// </summary>
        [KmlElement(null)]
        public AdministrativeArea AdministrativeArea
        {
            get { return _area; }
            set { this.UpdatePropertyChild(value, ref _area); }
        }

        /// <summary>Gets or sets a country code as per ISO 3166-1.</summary>
        [KmlElement("CountryNameCode", KmlNamespaces.XalNamespace)]
        public string NameCode { get; set; }
    }
}
