using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies a container of zero or more <see cref="ColorStyle"/> objects
    /// that can referenced from a <see cref="StyleMapCollection"/> or
    /// <see cref="Feature"/>.
    /// </summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 12.2</para>
    /// <para>Styles affect how a <see cref="Geometry"/> is presented in the
    /// geographic view and how a <c>Feature</c> appears in the list view.</para>
    /// </remarks>
    [KmlElement("Style")]
    public sealed class Style : StyleSelector
    {
        private BalloonStyle _balloon;
        private IconStyle _icon;
        private LabelStyle _label;
        private LineStyle _line;
        private ListStyle _list;
        private PolygonStyle _polygon;

        /// <summary>
        /// Gets or sets the associated <see cref="BalloonStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 5)]
        public BalloonStyle Balloon
        {
            get { return _balloon; }
            set { this.UpdatePropertyChild(value, ref _balloon); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="IconStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 1)]
        public IconStyle Icon
        {
            get { return _icon; }
            set { this.UpdatePropertyChild(value, ref _icon); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="LabelStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 2)]
        public LabelStyle Label
        {
            get { return _label; }
            set { this.UpdatePropertyChild(value, ref _label); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="LineStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 3)]
        public LineStyle Line
        {
            get { return _line; }
            set { this.UpdatePropertyChild(value, ref _line); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="ListStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 6)]
        public ListStyle List
        {
            get { return _list; }
            set { this.UpdatePropertyChild(value, ref _list); }
        }

        /// <summary>
        /// Gets or sets the associated <see cref="PolygonStyle"/> of this instance.
        /// </summary>
        [KmlElement(null, 4)]
        public PolygonStyle Polygon
        {
            get { return _polygon; }
            set { this.UpdatePropertyChild(value, ref _polygon); }
        }
    }
}
