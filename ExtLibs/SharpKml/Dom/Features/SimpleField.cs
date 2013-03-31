using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Specifies a user-defined field.</summary>
    /// <remarks>OGC KML 2.2 Section 9.9</remarks>
    [KmlElement("SimpleField")]
    public sealed class SimpleField : Element
    {
        /// <summary>Gets or sets an alternate display name.</summary>
        [KmlElement("displayName", 1)]
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the type of the field.</summary>
        /// <remarks>
        /// The type content shall be formatted according to one of the following XML Schema types:
        /// <list type="bullet">
        /// <item>
        /// <term>xsd:boolean</term>
        /// <description>
        /// Compatible with System.Boolean
        /// Legal values are true, false, 1 (which indicates true), and 0 (which indicates false).
        /// </description>
        /// </item><item>
        /// <term>xsd:double</term>
        /// <description>Compatible with System.Double</description>
        /// </item><item>
        /// <term>xsd:int</term>
        /// <description>Compatible with System.Int32</description>
        /// </item><item>
        /// <term>xsd:float</term>
        /// <description>Compatible with System.Single</description>
        /// </item><item>
        /// <term>xsd:short</term>
        /// <description>Compatible with System.Int16</description>
        /// </item><item>
        /// <term>xsd:string</term>
        /// <description>Compatible with System.String</description>
        /// </item><item>
        /// <term>xsd:unsignedInt</term>
        /// <description>Compatible with System.UInt32</description>
        /// </item><item>
        /// <term>xsd:unsignedShort</term>
        /// <description>Compatible with System.UInt16</description>
        /// </item>
        /// </list>
        /// </remarks>
        [KmlAttribute("type")]
        public string FieldType { get; set; }

        /// <summary>Gets or sets a value acting as an identifier.</summary>
        [KmlAttribute("name")]
        public string Name { get; set; }
    }
}
