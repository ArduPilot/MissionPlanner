using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    public class point
    {
        [XmlAttribute] public string lat;
        [XmlAttribute] public string lon;
        [XmlAttribute] public string hae;
        [XmlAttribute] public string ce = "1.0";
        [XmlAttribute] public string le = "1.0";
    }
}