using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how to display an image specified by <see cref="Icon"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 11.1</remarks>
    public abstract class Overlay : Feature
    {
        /// <summary>The default value that should be used for <see cref="Color"/>.</summary>
        public static readonly Color32 DefaultColor = new Color32(255, 255, 255, 255);

        private Icon _icon;

        /// <summary>Gets or sets the color of the graphic element.</summary>
        [KmlElement("color", 1)]
        public Color32? Color { get; set; }

        /// <summary>
        /// Gets or sets the stacking order, relative to the
        /// <see cref="AbstractView"/>, for overlapping Overlay elements.
        /// </summary>
        /// <remarks>
        /// Overlay elements with higher DrawOrder values are drawn on top of
        /// overlays with lower DrawOrder values.
        /// </remarks>
        [KmlElement("drawOrder", 2)]
        public int? DrawOrder { get; set; }

        /// <summary>Gets or sets the associated image of this instance.</summary>
        /// <remarks>
        /// If no image is specified or located, a rectangle is drawn using the
        /// color and size defined by the ground or screen overlay.
        /// </remarks>
        [KmlElement(null, 3)]
        public Icon Icon
        {
            get { return _icon; }
            set { this.UpdatePropertyChild(value, ref _icon); }
        }
    }
}
