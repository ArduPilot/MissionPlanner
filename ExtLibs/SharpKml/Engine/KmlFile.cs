using System;
using System.Collections.Generic;
using System.IO;
using SharpKml.Base;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>Represents an instance of a KML file.</summary>
    /// <remarks>
    /// A KmlFile manages an XML id domain and includes an internal map of all
    /// KmlObject Id's, shared styles, named Schemas and a list of all links.
    /// </remarks>
    public sealed class KmlFile
    {
        private Dictionary<string, KmlObject> _objects = new Dictionary<string, KmlObject>();
        private Dictionary<string, StyleSelector> _styles = new Dictionary<string, StyleSelector>();
        private bool _strict;

        // Use the Create methods
        private KmlFile()
        {
        }

        /// <summary>Gets the root <see cref="Element"/> of the file.</summary>
        public Element Root { get; private set; }

        /// <summary>
        /// Gets the <see cref="StyleSelector"/>s which have their Id set.
        /// </summary>
        public IEnumerable<StyleSelector> Styles
        {
            get { return _styles.Values; }
        }

        /// <summary>
        /// Gets the internal map of the styles found during the parse.
        /// </summary>
        internal IDictionary<string, StyleSelector> StyleMap
        {
            get { return _styles; }
        }

        /// <summary>
        /// Creates a KmlFie from the specified <see cref="Element"/> hierarchy.
        /// </summary>
        /// <param name="root">The root <c>Element</c> of the file.</param>
        /// <param name="duplicates">
        /// true to allow duplicate <see cref="KmlObject.Id"/> values (newer
        /// values will overwrite existing values); false to throw an exception
        /// for duplicate values.
        /// </param>
        /// <returns>
        /// A new KmlFile with the specified <c>Element</c> as the root.
        /// </returns>
        /// <exception cref="ArgumentNullException">root is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// A object has already been registered with the same Id and the
        /// duplicates argument is set to false.
        /// </exception>
        public static KmlFile Create(Element root, bool duplicates)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            KmlFile file = new KmlFile();
            file._strict = !duplicates;

            foreach (var element in ElementWalker.Walk(root))
            {
                file.OnElementAdded(element);
            }

            file.Root = root;
            return file;
        }

        /// <summary>
        /// Loads a KmlFile using the specified KML data.
        /// </summary>
        /// <param name="input">The stream containing the KML data.</param>
        /// <returns>A KmlFile representing the specified information.</returns>
        /// <remarks>
        /// This method checks for duplicate Id's in the file and throws an
        /// exception if duplicate Id's are found. To enable duplicate Id's
        /// use the <see cref="Parser"/> class and pass the root element
        /// to <see cref="Create"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">input is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// Duplicate Id's were found or the XML is nested too deeply.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The stream does not support reading.
        /// </exception>
        /// <exception cref="ObjectDisposedException">
        /// The stream was closed.
        /// </exception>
        /// <exception cref="System.Xml.XmlException">
        /// An error occurred while parsing the KML.
        /// </exception>
        public static KmlFile Load(Stream input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            KmlFile file = new KmlFile();
            using (var reader = new StreamReader(input))
            {
                file.Parse(reader);
            }
            return file;
        }

        /// <summary>
        /// Loads a KmlFile using the specified KML data at the specified path.
        /// </summary>
        /// <param name="path">
        /// The URI for the file containing the XML data.
        /// </param>
        /// <returns>A KmlFile representing the specified file.</returns>
        /// <remarks>
        /// This method checks for duplicate Id's in the file and throws an
        /// exception if duplicate Id's are found. To enable duplicate Id's
        /// use the <see cref="Parser"/> class and pass the root element
        /// to <see cref="Create"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Xml.XmlException">
        /// An error occurred while parsing the KML.
        /// </exception>
        public static KmlFile Load(string path)
        {
            using (var stream = FileHandler.OpenRead(new Uri(path, UriKind.RelativeOrAbsolute)))
            {
                return Load(stream);
            }
        }

        /// <summary>
        /// Loads a KmlFile using the specified KMZ data.
        /// </summary>
        /// <param name="kmz">The KmzFile containing the KML data.</param>
        /// <returns>
        /// A KmlFile representing the default KML file in the specified KMZ archive
        /// or null if no KML data was found.
        /// </returns>
        /// <remarks>
        /// This method checks for duplicate Id's in the file and throws an
        /// exception if duplicate Id's are found. To enable duplicate Id's
        /// use the <see cref="Parser"/> class and pass the root element
        /// to <see cref="Create"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">kmz is null.</exception>
        /// <exception cref="ObjectDisposedException">
        /// <see cref="KmzFile.Dispose"/> has been called on kmz.
        /// </exception>
        /// <exception cref="System.Xml.XmlException">
        /// An error occurred while parsing the KML.
        /// </exception>
        public static KmlFile LoadFromKmz(KmzFile kmz)
        {
            if (kmz == null)
            {
                throw new ArgumentNullException("kmz");
            }

            string kml = kmz.ReadKml();
            if (kml != null)
            {
                KmlFile instance = new KmlFile();
                using (var stream = new StringReader(kml))
                {
                    instance.Parse(stream);
                }
                return instance;
            }
            return null;
        }

        /// <summary>
        /// Searches for a <see cref="KmlObject"/> with the specified
        /// <see cref="KmlObject.Id"/>.
        /// </summary>
        /// <param name="id">The id of the <c>KmlObject</c> to find.</param>
        /// <returns>
        /// A <c>KmlObject</c> with the specified id if one was found;
        /// otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public KmlObject FindObject(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            KmlObject obj;
            if (_objects.TryGetValue(id, out obj))
            {
                return obj;
            }
            return null;
        }

        /// <summary>
        /// Searches for a <see cref="StyleSelector"/> with the specified
        /// <see cref="KmlObject.Id"/>.
        /// </summary>
        /// <param name="id">The id of the <c>StyleSelector</c> to find.</param>
        /// <returns>
        /// A <c>StyleSelector</c> with the specified id if one was found;
        /// otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public StyleSelector FindStyle(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            StyleSelector style;
            if (_styles.TryGetValue(id, out style))
            {
                return style;
            }
            return null;
        }

        /// <summary>Saves this instance to the specified stream.</summary>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentException">stream is not writable.</exception>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="NotSupportedException">
        /// The contents of the buffer cannot be written to the underlying fixed
        /// size stream because the end the stream has been reached.
        /// </exception>
        public void Save(Stream stream)
        {
            Serializer serializer = new Serializer();
            serializer.Serialize(this.Root);

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(serializer.Xml);
            }
        }

        /// <summary>Saves this instance to the specified path.</summary>
        /// <param name="path">The complete file path to write to.</param>
        /// <remarks>
        /// If the specified file exists in the specified path then it will be
        /// overwritten; otherwise, a new file will be created.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// path is a zero-length string, contains only white space, or contains
        /// one or more invalid characters as defined by
        /// <see cref="Path.GetInvalidPathChars"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">
        /// The specified path is invalid.
        /// </exception>
        /// <exception cref="IOException">
        /// An I/O error occurred or path includes an incorrect or invalid syntax
        /// for file name, directory name or volume label.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// path is in an invalid format.
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// The specified path, file name, or both exceed the system-defined
        /// maximum length.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The caller does not have the required permission or path specified a
        /// file that is read-only.
        /// </exception>
        public void Save(string path)
        {
            using (var file = File.Create(path))
            {
                this.Save(file);
            }
        }

        /// <summary>Adds the feature to the KmlFile.</summary>
        /// <param name="feature">The feature to add.</param>
        /// <remarks>This is used for Update operations.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Duplicate Id's were found.
        /// </exception>
        internal void AddFeature(Feature feature)
        {
            if (feature.Id != null)
            {
                this.AddToDictionary(feature, _objects);
            }
        }

        /// <summary>Removes the feature from the KmlFile.</summary>
        /// <param name="feature">The feature to remove.</param>
        /// <remarks>This is used for Update operations.</remarks>
        internal void RemoveFeature(Feature feature)
        {
            if (feature.Id != null)
            {
                _objects.Remove(feature.Id);
            }
        }

        private void AddToDictionary<T>(T element, Dictionary<string, T> dictionary) where T : KmlObject
        {
            if (_strict && dictionary.ContainsKey(element.Id))
            {
                throw new InvalidOperationException("Duplicate Object id found.");
            }
            dictionary[element.Id] = element; // Add or overwrite existing.
        }

        private void OnElementAdded(Element element)
        {
            // Map KmlObjects and related descendents
            KmlObject obj = element as KmlObject;
            if ((obj != null) && (obj.Id != null))
            {
                this.AddToDictionary(obj, _objects);

                // Also map Styles, as StyleSelector inherits from KmlObject.
                // A shared style is defined to be a StyleSelector with an
                // id that is a child of a Document.
                StyleSelector style = element as StyleSelector;
                if (style != null)
                {
                    Document document = element.Parent as Document;
                    if (document != null)
                    {
                        this.AddToDictionary(style, _styles);
                    }
                }
            }
        }

        private void Parse(TextReader input)
        {
            Parser parser = new Parser();
            parser.ElementAdded += (s, e) => this.OnElementAdded(e.Element);
            parser.Parse(input);
            this.Root = parser.Root;
        }
    }
}
