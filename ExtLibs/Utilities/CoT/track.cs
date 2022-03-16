using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    public class track
    {
        [XmlAttribute] public string course;
        [XmlAttribute] public string speed;
    }
}