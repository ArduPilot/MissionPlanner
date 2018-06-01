using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the unit of a value.</summary>
    /// <remarks>OGC KML 2.2 Section 16.20</remarks>
    public enum Unit
    {
        /// <summary>Value is a fraction of the icon.</summary>
        [KmlElement("fraction")]
        Fraction = 0,

        /// <summary>Value is a specific pixel size.</summary>
        [KmlElement("pixels")]
        Pixel,

        /// <summary>
        /// Value is an offset in pixels from the upper right corner of the icon.
        /// </summary>
        [KmlElement("insetPixels")]
        InsetPixel
    }
}
