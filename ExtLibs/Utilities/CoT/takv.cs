using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    /// <summary>The software that sent the event, as TAK clients describe themselves.</summary>
    public class takv
    {
        [XmlAttribute] public string device;
        [XmlAttribute] public string platform;
        [XmlAttribute] public string os;
        [XmlAttribute] public string version;
    }
}