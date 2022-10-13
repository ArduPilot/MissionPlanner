using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    public class contact
    {
        [XmlAttribute] public string callsign;
        [XmlAttribute] public string endpoint;
    }
}