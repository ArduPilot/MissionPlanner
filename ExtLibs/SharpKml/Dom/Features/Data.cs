using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents an untyped name/value pair.</summary>
    /// <remarks>OGC KML 2.2 Section 9.3</remarks>
    [KmlElement("Data")]
    public sealed class Data : KmlObject
    {
        /// <summary>Gets or sets an alternate display name.</summary>
        [KmlElement("displayName", 1)]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the name of the data pair.</summary>
        /// <remarks>
        /// The value shall be unique within the context of its <see cref="Element.Parent"/>.
        /// </remarks>
        [KmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the value of the data pair.</summary>
        [KmlElement("value", 2)]
        public string Value { get; set; }
    }
}
