using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// This should be the root element of a KML document instance.
    /// </summary>
    [KmlElement("kml")]
    public sealed class Kml : Element
    {
        private Feature _feature;
        private NetworkLinkControl _link;

        /// <summary>
        /// Gets or sets the associated <see cref="Feature"/> of this instance.
        /// </summary>
        [KmlElement(null, 2)]
        public Feature Feature
        {
            get { return _feature; }
            set { this.UpdatePropertyChild(value, ref _feature); }
        }

        /// <summary>
        /// Gets or sets information on how to process the KML document instance.
        /// </summary>
        [KmlAttribute("hint")]
        public string Hint { get; set; }

        /// <summary>
        /// Gets or sets the associated <see cref="NetworkLinkControl"/>
        /// of this instance.
        /// </summary>
        [KmlElement(null, 1)]
        public NetworkLinkControl NetworkLinkControl
        {
            get { return _link; }
            set { this.UpdatePropertyChild(value, ref _link); }
        }
    }
}
