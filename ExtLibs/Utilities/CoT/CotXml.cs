using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MissionPlanner.Utilities.CoT
{
    /// <summary>
    /// The text of a Cursor on Target event as Mission Planner sends it: the XML declaration, then
    /// the event without namespaces, either on one line or indented with one attribute per line.
    /// </summary>
    public static class CotXml
    {
        public const string Declaration = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>";

        static readonly XmlSerializer serializer = new XmlSerializer(typeof(@event));

        public static string Serialize(@event cotevent, bool indent)
        {
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = indent,
                NewLineOnAttributes = indent,
                Encoding = Encoding.UTF8
            };

            using (var text = new StringWriter())
            {
                using (var writer = XmlWriter.Create(text, settings))
                    serializer.Serialize(writer, cotevent, namespaces);

                return Declaration + "\r\n" + text;
            }
        }
    }
}
