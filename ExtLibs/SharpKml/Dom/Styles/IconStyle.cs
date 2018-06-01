using System;
using System.Xml;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how icons for <see cref="Placemark"/> and <see cref="PhotoOverlay"/>
    /// with a Point geometry are drawn in an earth browser's list and geographic views.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 12.8</para>
    /// <para>The color specified in the <see cref="ColorStyle.Color"/> property
    /// is blended with the color of the icon.</para>
    /// </remarks>
    [KmlElement("IconStyle")]
    public sealed class IconStyle : ColorStyle
    {
        /// <summary>The default value that should be used for <see cref="Scale"/>.</summary>
        public const double DefaultScale = 1.0;

        private Hotspot _hotspot;
        private IconLink _icon;

        /// <summary>
        /// Gets or sets the direction (North, South, East, West), in decimal degrees.
        /// </summary>
        /// <remarks>Values range from 0 (North) to 360 degrees.</remarks>
        [KmlElement("heading", 2)]
        public double? Heading { get; set; }

        /// <summary>
        /// Gets or sets the position of the reference point on the icon that is
        /// anchored to the <see cref="Point"/> specified in
        /// <see cref="Placemark.Geometry"/>.
        /// </summary>
        /// <remarks>
        /// The origin of the image coordinate system is in the lower left
        /// corner of the icon.
        /// </remarks>
        [KmlElement(null, 4)]
        public Hotspot Hotspot
        {
            get { return _hotspot; }
            set { this.UpdatePropertyChild(value, ref _hotspot); }
        }

        /// <summary>Gets or sets the icon resource location.</summary>
        [KmlElement(null, 3)]
        public IconLink Icon
        {
            get { return _icon; }
            set { this.UpdatePropertyChild(value, ref _icon); }
        }

        /// <summary>
        /// Gets or sets the scale factor that shall be applied to the graphic element.
        /// </summary>
        [KmlElement("scale", 1)]
        public double? Scale { get; set; }

        /// <summary>
        /// Stores an invalid child <see cref="Element"/> for later serialization.
        /// </summary>
        /// <param name="orphan">The <c>Element</c> to store for serialization.</param>
        protected internal override void AddOrphan(Element orphan)
        {
            Icon icon = orphan as Icon;
            if (icon != null)
            {
                this.Icon = new IconLink(icon.Href);
            }
            else
            {
                base.AddOrphan(orphan);
            }
        }

        // This is declared like this in the XSD as a nested type in IconStyle

        /// <summary>Specifies an icon resource location.</summary>
        public sealed class IconLink : BasicLink, ICustomElement
        {
            /// <summary>Initializes a new instance of the IconLink class.</summary>
            /// <param name="href">The value for <see cref="BasicLink.Href"/></param>
            public IconLink(Uri href)
            {
                this.Href = href;
            }

            /// <summary>
            /// Gets or sets the height, in pixels, of the icon to use.
            /// [Google Extension]
            /// </summary>
            [KmlElement("h", KmlNamespaces.GX22Namespace, 4)]
            public double? Height { get; set; }

            /// <summary>
            /// Gets or sets the width, in pixels, of the icon to use.
            /// [Google Extension]
            /// </summary>
            [KmlElement("w", KmlNamespaces.GX22Namespace, 3)]
            public double? Width { get; set; }

            /// <summary>
            /// Gets or sets the offset, in pixels, from the left edge of the
            /// icon palette. [Google Extension]
            /// </summary>
            [KmlElement("x", KmlNamespaces.GX22Namespace, 1)]
            public double? X { get; set; }

            /// <summary>
            /// Gets or sets the offset, in pixels, from the bottom edge of the
            /// icon palette. [Google Extension]
            /// </summary>
            [KmlElement("y", KmlNamespaces.GX22Namespace, 2)]
            public double? Y { get; set; }

            /// <summary>
            /// Gets a value indicating whether to process the children of the Element.
            /// </summary>
            bool ICustomElement.ProcessChildren
            {
                get { return true; }
            }

            /// <summary>Writes the start of an XML element.</summary>
            /// <param name="writer">An <see cref="XmlWriter"/> to write to.</param>
            void ICustomElement.CreateStartElement(XmlWriter writer)
            {
                writer.WriteStartElement("Icon", KmlNamespaces.Kml22Namespace);
            }
        }
    }
}
