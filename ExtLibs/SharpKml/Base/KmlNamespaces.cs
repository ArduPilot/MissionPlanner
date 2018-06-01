using System;
using System.Collections.Generic;
using System.Reflection;

namespace SharpKml.Base
{
    /// <summary>Represents XML namespace information.</summary>
    /// <remarks>
    /// These need to be constants for the use in Attributes, hence the
    /// XXXNamespace and XXXPrefix fields.
    /// </remarks>
    internal static class KmlNamespaces
    {
        /// <summary>Represents the Atom Publishing Protocol namespace.</summary>
        public const string AppNamespace = "http://www.w3.org/2007/app";

        /// <summary>Represents the default Atom Publishing Protocol prefix.</summary>
        public const string AppPrefix = "app";

        /// <summary>Represents the Atom Syndication Format namespace.</summary>
        public const string AtomNamespace = "http://www.w3.org/2005/Atom";

        /// <summary>Represents the default Atom Syndication Format prefix.</summary>
        public const string AtomPrefix = "atom";

        /// <summary>Represents the Google Data Batch Processing namespace.</summary>
        public const string BatchNamespace = "http://schemas.google.com/gdata/batch";

        /// <summary>Represents the default Google Data Batch Processing prefix.</summary>
        public const string BatchPrefix = "batch";

        /// <summary>Represents the Google Documents namespace.</summary>
        public const string DocsNamespace = "http://schemas.google.com/docs/2007";

        /// <summary>Represents the default Google Documents prefix.</summary>
        public const string DocsPrefix = "docs";

        /// <summary>Represents the Exchangeable Image File Format namespace.</summary>
        public const string ExifNamespace = "http://schemas.google.com/photos/exif/2007";

        /// <summary>Represents the default Exchangeable Image File Format prefix.</summary>
        public const string ExifPrefix = "exif";

        /// <summary>Represents the Google Data namespace.</summary>
        public const string GoogleDataNamespace = "http://schemas.google.com/g/2005";

        /// <summary>Represents the default Google Data prefix.</summary>
        public const string GoogleDataPrefix = "gd";

        /// <summary>Represents the GeoRSS Application namespace.</summary>
        public const string GeoRssNamespace = "http://www.georss.org/georss";

        /// <summary>Represents the default GeoRSS Application prefix.</summary>
        public const string GeoRssPrefix = "georss";

        /// <summary>Represents the Geography Markup Language namespace.</summary>
        public const string GmlNamespace = "http://www.opengis.net/gml";

        /// <summary>Represents the default Geography Markup Language prefix.</summary>
        public const string GmlPrefix = "gml";

        /// <summary>Represents the GPS Exchange Format namespace.</summary>
        public const string GpxNamespace = "http://www.topografix.com/GPX/1/0";

        /// <summary>Represents the default GPS Exchange Format prefix.</summary>
        public const string GpxPrefix = "gpx";

        /// <summary>Represents the Google Extensions namespace.</summary>
        public const string GX22Namespace = "http://www.google.com/kml/ext/2.2";

        /// <summary>Represents the default Google Extensions prefix.</summary>
        public const string GX22Prefix = "gx";

        /// <summary>Represents the Kml 2.2 namespace.</summary>
        public const string Kml22Namespace = "http://www.opengis.net/kml/2.2";

        /// <summary>Represents the default Kml 2.2 prefix.</summary>
        public const string Kml22Prefix = "kml";

        /// <summary>Represents the Media RSS namespace.</summary>
        public const string MediaNamespace = "http://search.yahoo.com/mrss/";

        /// <summary>Represents the default Media RSS prefix.</summary>
        public const string MediaPrefix = "media";

        /// <summary>Represents the OpenSearch 1.1 namespace.</summary>
        public const string OpenSearchNamespace = "http://a9.com/-/spec/opensearch/1.1/";

        /// <summary>Represents the default OpenSearch 1.1 prefix.</summary>
        public const string OpenSearchPrefix = "openSearch";

        /// <summary>Represents the Picasa Web Albums namespace.</summary>
        public const string PhotoNamespace = "http://schemas.google.com/photos/2007";

        /// <summary>Represents the default Picasa Web Albums prefix.</summary>
        public const string PhotoPrefix = "gphoto";

        /// <summary>Represents the Google Spreadsheets namespace.</summary>
        public const string SpreadSheetsNamespace = "http://schemas.google.com/spreadsheets/2006";

        /// <summary>Represents the default Google Spreadsheets prefix.</summary>
        public const string SpreadSheetsPrefix = "gs";

        /// <summary>Represents the Extensible Address Language namespace.</summary>
        public const string XalNamespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0";

        /// <summary>Represents the default Extensible Address Language prefix.</summary>
        public const string XalPrefix = "xal";

        /// <summary>Represents the XML namespace.</summary>
        public const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";

        /// <summary>Represents the default XML prefix.</summary>
        public const string XmlPrefix = "xml";

        /// <summary>Represents the XML Schema Datatypes namespace.</summary>
        public const string XsdNamespace = "http://www.w3.org/2001/XMLSchema";

        /// <summary>Represents the default XML Schema Datatypes prefix.</summary>
        public const string XsdPrefix = "xsd";

        /// <summary>Represents the XML Schema namespace.</summary>
        public const string XsiNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        /// <summary>Represents the default XML Schema prefix.</summary>
        public const string XsiPrefix = "xsi";

        private static Dictionary<string, string> _namespaces = GenerateNamespaces();

        /// <summary>Searches for the associated XML namespace for the specified prefix.</summary>
        /// <param name="prefix">The standard prefix to search for.</param>
        /// <returns>
        /// The XML namespace associated with the specified prefix if the prefix is known;
        /// otherwise, null.
        /// </returns>
        public static string FindNamespace(string prefix)
        {
            string output;
            if (_namespaces.TryGetValue(prefix ?? string.Empty, out output))
            {
                return output;
            }
            return null;
        }

        // FxCop doesn't like static constructors, so this is used to allow field initialization
        private static Dictionary<string, string> GenerateNamespaces()
        {
            var output = new Dictionary<string, string>();

            // Iterate over the static fields and add them to the dictionary
            foreach (var field in typeof(KmlNamespaces).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name.EndsWith("Namespace", StringComparison.Ordinal))
                {
                    string name = field.Name.Substring(0, field.Name.Length - 9); // Remove the "Namespace" part
                    var prefix = typeof(KmlNamespaces).GetField(name + "Prefix");

                    output.Add((string)prefix.GetValue(null), (string)field.GetValue(null));
                }
            }
            return output;
        }
    }
}
