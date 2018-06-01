using System;
using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>
    /// Encodes an instance of a user-defined data type defined by a referenced
    /// <see cref="Schema"/>.
    /// </summary>
    /// <remarks>OGC KML 2.2 Section 9.4</remarks>
    [KmlElement("SchemaData")]
    public sealed class SchemaData : KmlObject
    {
        /// <summary>Initializes a new instance of the SchemaData class.</summary>
        public SchemaData()
        {
            this.RegisterValidChild<SimpleData>();
            this.RegisterValidChild<GX.SimpleArrayData>();
        }

        /// <summary>
        /// Gets or sets a reference to a <see cref="KmlObject.Id"/> belonging
        /// to a <see cref="Schema"/>.
        /// </summary>
        /// <remarks>
        /// Reference can either be a full URL or a reference to a <c>Schema</c>
        /// defined in a KML resource file (both external or local).
        /// </remarks>
        [KmlAttribute("schemaUrl")]
        public Uri SchemaUrl { get; set; }

        /// <summary>Gets a collection of user-defined fields.</summary>
        public IEnumerable<SimpleData> SimpleData
        {
            get { return this.Children.OfType<SimpleData>(); }
        }

        /// <summary>
        /// Gets a collection of value arrays.
        /// [Google Extension]
        /// </summary>
        public IEnumerable<GX.SimpleArrayData> GXSimpleArray
        {
            get { return this.Children.OfType<GX.SimpleArrayData>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="SimpleData"/> to this instance.
        /// </summary>
        /// <param name="data">The <c>SimpleData</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">data is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// data belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddData(SimpleData data)
        {
            this.AddChild(data);
        }

        /// <summary>
        /// Adds the specified <see cref="GX.SimpleArrayData"/> to this instance.
        /// [Google Extension]
        /// </summary>
        /// <param name="array">The <c>SimpleArrayData</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">array is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// array belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddArray(GX.SimpleArrayData array)
        {
            this.AddChild(array);
        }
    }
}
