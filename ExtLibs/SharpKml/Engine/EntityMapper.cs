using System;
using System.Collections.Generic;
using System.Text;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>Handles replaceable entities in <see cref="Feature"/>s.</summary>
    public sealed class EntityMapper
    {
        private const string DisplayNamePostfix = "/displayName";

        private Dictionary<string, string> _map = new Dictionary<string, string>();
        private Dictionary<string, string> _fieldMap = new Dictionary<string, string>();
        private List<Tuple<string, string>> _markup = new List<Tuple<string, string>>();
        private KmlFile _file;

        /// <summary>
        /// Initializes a new instance of the EntityMapper class.
        /// </summary>
        /// <param name="file">The kml information to parse.</param>
        /// <exception cref="ArgumentNullException">file is null.</exception>
        public EntityMapper(KmlFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            _file = file;
        }

        /// <summary>
        /// Gets the entities found during the most recent parse.
        /// </summary>
        /// <remarks>This property is for unit testing only.</remarks>
        internal IDictionary<string, string> Entities
        {
            get { return _map; }
        }

        /// <summary>
        /// Gets the alternative markup found during the most recent parse.
        /// </summary>
        /// <remarks>This property is for unit testing only.</remarks>
        internal IList<Tuple<string, string>> Markup
        {
            get { return _markup; }
        }

        /// <summary>
        /// Creates a balloon text for the specified feature, calling
        /// <see cref="ParseEntityFields"/> to gather information.
        /// </summary>
        /// <param name="feature">The feature to create a balloon text for.</param>
        /// <returns>
        /// The expanded version of the feature's balloon text, if specified;
        /// otherwise an automatically generated HTML summary of the feature.
        /// </returns>
        /// <exception cref="ArgumentNullException">feature is null.</exception>
        /// <remarks>
        /// By default this method will not resolve any external styles.
        /// </remarks>
        public string CreateBalloonText(Feature feature)
        {
            this.ParseEntityFields(feature); // Will throw is feature is null

            Style style = StyleResolver.CreateResolvedStyle(feature, _file, StyleState.Normal, false);
            if ((style.Balloon != null) && (style.Balloon.Text != null))
            {
                return this.ExpandEntities(style.Balloon.Text);
            }

            // If the Balloon doesn't exist or doesn't have any text then we'll
            // make an HTML default table.
            StringBuilder html = new StringBuilder();
            if (feature.Name != null)
            {
                html.Append("<h3>");
                html.Append(feature.Name);
                html.Append("</h3><br/><br/>");
            }

            if ((feature.Description != null) && (feature.Description.Text != null))
            {
                html.Append(this.ExpandEntities(feature.Description.Text));
            }

            // Now a table of the extended data
            if (_markup.Count != 0)
            {
                html.AppendLine("\n<table border=\"1\">");
                foreach (var data in _markup)
                {
                    html.Append("<tr><td>");
                    html.Append(data.Item1);
                    html.Append("</td><td>");
                    html.Append(data.Item2);
                    html.AppendLine("</tr>");
                }
            }
            return html.ToString();
        }

        /// <summary>
        /// Replaces known entities in the specified string with their
        /// corresponding data from the most recently parsed <see cref="Feature"/>.
        /// </summary>
        /// <param name="input">The string containing replaceable entities.</param>
        /// <returns>A string with the entity placeholders replaced.</returns>
        /// <remarks>
        /// This method searches for $[entityName] in the input string and
        /// replaces it with data found during the most recent call to
        /// <see cref="ParseEntityFields"/>.
        /// </remarks>
        public string ExpandEntities(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (var entity in _map)
            {
                sb.Replace("$[" + entity.Key + "]", entity.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Maps all replaceable entities in the specified <see cref="Feature"/>.
        /// </summary>
        /// <param name="feature">
        /// The <c>Feature</c> to search for replaceable entities.
        /// </param>
        /// <exception cref="ArgumentNullException">feature is null.</exception>
        public void ParseEntityFields(Feature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            _fieldMap.Clear();
            _map.Clear();
            _markup.Clear();

            this.GetFeatureFields(feature);
            this.GetExtendedDataFields(feature);
        }

        private static void AddtoDictionary(Dictionary<string, string> dictionary, string name, string value)
        {
            if (value != null)
            {
                dictionary[name] = value;
            }
        }

        private static void AddtoDictionary(Dictionary<string, string> dictionary, string name, IHtmlContent value)
        {
            if (value != null)
            {
                dictionary[name] = value.Text;
            }
        }

        private void GetExtendedDataFields(Feature feature)
        {
            if (feature.ExtendedData != null)
            {
                foreach (var data in feature.ExtendedData.Data)
                {
                    this.GatherDataFields(data);
                }

                foreach (var schema in feature.ExtendedData.SchemaData)
                {
                    this.GatherSchemaDataFields(schema);
                }
            }
        }

        private void GetFeatureFields(Feature feature)
        {
            // KmlObject's fields
            AddtoDictionary(_map, "id", feature.Id);
            AddtoDictionary(_map, "targetId", feature.TargetId);

            // Feature's fields
            // TODO: OGC KML 2.2 does not single out specific elements -
            // any simple field or attribute of Feature is an entity candidate,
            // however, these are the select few chosen by the C++ version
            AddtoDictionary(_map, "name", feature.Name);
            AddtoDictionary(_map, "address", feature.Address);
            AddtoDictionary(_map, "Snippet", feature.Snippet);
            AddtoDictionary(_map, "description", feature.Description);
        }

        private void GatherDataFields(Data data)
        {
            if (data.Name != null)
            {
                _map[data.Name] = data.Value;
                if (data.DisplayName != null)
                {
                    _map[data.Name + DisplayNamePostfix] = data.DisplayName;
                    _markup.Add(Tuple.Create(data.DisplayName, data.Value));
                }
                else
                {
                    _markup.Add(Tuple.Create(data.Name, data.Value));
                }
            }
        }

        private void GatherSchemaDataFields(SchemaData schemaData)
        {
            string prefix = "/";
            if (schemaData.SchemaUrl != null)
            {
                string id = schemaData.SchemaUrl.GetFragment();
                if (id != null) // Make sure a fragment was found
                {
                    Schema schema = _file.FindObject(id) as Schema;
                    if (schema != null) // Make sure it was found and the object is a Schema
                    {
                        foreach (var field in schema.Fields)
                        {
                            this.GatherSimpleFieldFields(field, schema);
                        }
                        this.PopulateSimpleFieldNameMap(schema);
                        prefix = schema.Name + prefix;
                    }
                }
            }

            foreach (var data in schemaData.SimpleData)
            {
                this.GatherSimpleDataFields(data, prefix);
            }
        }

        private void GatherSimpleDataFields(SimpleData data, string prefix)
        {
            if (data.Name != null)
            {
                _map[prefix + data.Name] = data.Text;

                string name;
                if (!_fieldMap.TryGetValue(data.Name, out name))
                {
                    name = data.Name; // Fallback to it's name
                }
                _markup.Add(Tuple.Create(name, data.Text));
            }
        }

        private void GatherSimpleFieldFields(SimpleField field, Schema schema)
        {
            if ((field.Name != null) && (field.DisplayName != null))
            {
                string name = schema.Name + "/" + field.Name + DisplayNamePostfix;
                _map[name] = field.DisplayName;
            }
        }

        private void PopulateSimpleFieldNameMap(Schema schema)
        {
            foreach (var field in schema.Fields)
            {
                if (field.Name != null)
                {
                    string name = field.DisplayName ?? field.Name; // If no display name then use the regular name.
                    string value = schema.Name + ":" + name;
                    _fieldMap[field.Name] = value;
                }
            }
        }
    }
}
