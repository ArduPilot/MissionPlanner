using System.Collections.Generic;
using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    [XmlRoot(Namespace = "")]
    public class @event
    {
        [XmlAttribute] public string version = "2.0";
        [XmlAttribute] public string uid;
        [XmlAttribute] public string type;
        [XmlAttribute] public string time;
        [XmlAttribute] public string start;
        [XmlAttribute] public string stale;
        [XmlAttribute] public string how;
        public detail detail;
        public point point;
    }
}