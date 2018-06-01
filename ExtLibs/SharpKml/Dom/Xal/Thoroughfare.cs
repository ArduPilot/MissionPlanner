using SharpKml.Base;

namespace SharpKml.Dom.Xal
{
    /// <summary>Contains information of a thoroughfare.</summary>
    /// <remarks>A thoroughfare could be a road, street, canal, river, etc.</remarks>
    [KmlElement("Thoroughfare", KmlNamespaces.XalNamespace)]
    public sealed class Thoroughfare : Element
    {
        /// <summary>Gets or sets the name of the thoroughfare.</summary>
        [KmlElement("ThoroughfareName", KmlNamespaces.XalNamespace)]
        public string Name { get; set; }

        /// <summary>Gets or sets the number of the thoroughfare.</summary>
        [KmlElement("ThoroughfareNumber", KmlNamespaces.XalNamespace)]
        public string Number { get; set; }
    }
}
