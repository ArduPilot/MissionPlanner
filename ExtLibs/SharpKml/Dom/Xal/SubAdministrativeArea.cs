using SharpKml.Base;

namespace SharpKml.Dom.Xal
{
    /// <summary>Represents information about a sub-administrative area.</summary>
    /// <remarks>
    /// An example of a sub-administrative areas is a county. There are two places
    /// where the name of an administrative area can be specified and in this case,
    /// one becomes sub-administrative area.
    /// </remarks>
    [KmlElement("SubAdministrativeArea", KmlNamespaces.XalNamespace)]
    public sealed class SubAdministrativeArea : Element
    {
        private Locality _locality;

        /// <summary>
        /// Gets or sets the <see cref="Locality"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Locality Locality
        {
            get { return _locality; }
            set { this.UpdatePropertyChild(value, ref _locality); }
        }

        /// <summary>
        /// Gets or sets the name of the sub-administrative area.
        /// </summary>
        [KmlElement("SubAdministrativeAreaName", KmlNamespaces.XalNamespace)]
        public string Name { get; set; }
    }
}
