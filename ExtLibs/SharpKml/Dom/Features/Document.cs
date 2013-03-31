using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Represents a container for KML features, shared styles and user-defined schemas.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.7</remarks>
    [KmlElement("Document")]
    public sealed class Document : Container
    {
        /// <summary>Initializes a new instance of the Document class.</summary>
        public Document()
        {
            this.RegisterValidChild<StyleSelector>();
            this.RegisterValidChild<Schema>();
            this.RegisterValidChild<Feature>();
        }

        /// <summary>
        /// Gets a collection of <see cref="Schema"/> contained by this instance.
        /// </summary>
        public IEnumerable<Schema> Schemas
        {
            get { return this.Children.OfType<Schema>(); }
        }

        /// <summary>
        /// Gets a collection of <see cref="StyleSelector"/> contained by this instance.
        /// </summary>
        public IEnumerable<StyleSelector> Styles
        {
            get { return this.Children.OfType<StyleSelector>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="Schema"/> to this instance.
        /// </summary>
        /// <param name="schema">The <c>Schema</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">schema is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// schema belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddSchema(Schema schema)
        {
            this.AddChild(schema);
        }

        /// <summary>
        /// Adds the specified <see cref="StyleSelector"/> to this instance.
        /// </summary>
        /// <param name="style">The <c>StyleSelector</c> to add to this instance.
        /// </param>
        /// <exception cref="System.ArgumentNullException">style is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// style belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddStyle(StyleSelector style)
        {
            this.AddChild(style);
        }
    }
}
