using System;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Defines a key/value pair that maps a mode
    /// (<see cref="Dom.StyleState"/>) to the predefined
    /// <see cref="StyleUrl"/> and/or a <see cref="StyleSelector"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 12.4</para>
    /// <para>If both <c>StyleUrl</c> and <see cref="Selector"/> exist then
    /// their styles shall be merged.</para>
    /// </remarks>
    [KmlElement("Pair")]
    public sealed class Pair : KmlObject
    {
        private StyleSelector _selector;

        /// <summary>
        /// Gets or sets the associated <see cref="StyleSelector"/> of this instance.
        /// </summary>
        [KmlElement(null, 3)]
        public StyleSelector Selector
        {
            get { return _selector; }
            set { this.UpdatePropertyChild(value, ref _selector); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Dom.StyleState"/> for the key.
        /// </summary>
        [KmlElement("key", 1)]
        public StyleState? State { get; set; }

        /// <summary>
        /// Gets or sets a reference to a <see cref="Style"/> or
        /// <see cref="StyleMapCollection"/>.
        /// </summary>
        /// <remarks>
        /// The value of the fragment shall be the id of a <c>Style</c> or
        /// <c>StyleMap</c> defined in a <see cref="Document"/>.
        /// </remarks>
        [KmlElement("styleUrl", 2)]
        public Uri StyleUrl { get; set; }
    }
}
