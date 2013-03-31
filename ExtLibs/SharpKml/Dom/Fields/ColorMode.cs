using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies the color mode for a graphic element.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 16.7</para>
    /// <para>If the value is <see cref="Random"/> then the color components are
    /// modified as follows:</para>
    /// <list type="bullet">
    /// <item><description>If a single color component is specified (for example
    /// <see cref="Color32.Red"/> equals 255), random color values for that one
    /// component will be selected. In this case, the values would range from 0
    /// (black) to 255 (full red).</description></item>
    /// <item><description>If values for two or for all three color components
    /// are specified, a random linear scale is applied to each color component,
    /// with results ranging from black to the maximum values specified for each
    /// component.</description></item>
    /// <item><description>The opacity of a color (<see cref="Color32.Alpha"/>)
    /// is never randomized.</description></item>
    /// </list>
    /// </remarks>
    public enum ColorMode
    {
        /// <summary>Use a single color value.</summary>
        [KmlElement("normal")]
        Normal = 0,

        /// <summary>Use a random color value.</summary>
        [KmlElement("random")]
        Random
    }
}
