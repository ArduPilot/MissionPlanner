using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Specifies how the description balloon for a <see cref="Feature"/> is drawn.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 12.6</remarks>
    [KmlElement("BalloonStyle")]
    public sealed class BalloonStyle : SubStyle
    {
        /// <summary>The default value that should be used for <see cref="BackgroundColor"/>.</summary>
        public static readonly Color32 DefaultBackground = new Color32(255, 255, 255, 255);

        /// <summary>The default value that should be used for <see cref="TextColor"/>.</summary>
        public static readonly Color32 DefaultText = new Color32(255, 0, 0, 0);

        /// <summary>
        /// Gets or sets the background color of the graphic element.
        /// </summary>
        [KmlElement("bgColor", 1)]
        public Color32? BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets whether the balloon is displayed or hidden.
        /// </summary>
        [KmlElement("displayMode", 4)]
        public DisplayMode? DisplayMode { get; set; }

        /// <summary>Gets or sets the text displayed in the balloon.</summary>
        /// <remarks>
        /// The text may include HTML content. Text shall support entity
        /// substitution, as defined in Section 6.5 Entity Replacement.
        /// </remarks>
        [KmlElement("text", 3)]
        public string Text { get; set; }

        /// <summary>Gets or sets the foreground color of the text.</summary>
        [KmlElement("textColor", 2)]
        public Color32? TextColor { get; set; }
    }
}
