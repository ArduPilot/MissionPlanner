using SharpKml.Base;

namespace SharpKml.Dom.Xal
{
    /// <summary>Defines the details of an address.</summary>
    /// <remarks>
    /// Can define multiple addresses, including tracking address history.
    /// </remarks>
    [KmlElement("AddressDetails", KmlNamespaces.XalNamespace)]
    public sealed class AddressDetails : Element
    {
        private Country _country;

        /// <summary>
        /// Gets or sets the <see cref="Country"/> associated with this instance.
        /// </summary>
        [KmlElement(null)]
        public Country Country
        {
            get { return _country; }
            set { this.UpdatePropertyChild(value, ref _country); }
        }
    }
}
