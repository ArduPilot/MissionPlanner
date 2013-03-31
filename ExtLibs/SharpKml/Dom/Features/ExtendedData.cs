using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents a KML AbstractFeatureGroup</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 9.2</para>
    /// <para>The scope of ExtendedData is restricted to its <see cref="Element.Parent"/>
    /// only. Child elements support entity substitution - see section 6.5 for details.</para>
    /// </remarks>
    [KmlElement("ExtendedData")]
    public sealed class ExtendedData : Element
    {
        /// <summary>Initializes a new instance of the ExtendedData class.</summary>
        public ExtendedData()
        {
            this.RegisterValidChild<Data>();
            this.RegisterValidChild<SchemaData>();
        }

        /// <summary>Gets a collection of untyped name/value pairs.</summary>
        public IEnumerable<Data> Data
        {
            get { return this.Children.OfType<Data>(); }
        }

        /// <summary>Gets a collection of <see cref="SchemaData"/> objects.</summary>
        public IEnumerable<SchemaData> SchemaData
        {
            get { return this.Children.OfType<SchemaData>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="Data"/> to this instance.
        /// </summary>
        /// <param name="data">The <c>Data</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">data is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// data belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddData(Data data)
        {
            this.AddChild(data);
        }

        /// <summary>
        /// Adds the specified <see cref="SchemaData"/> to this instance.
        /// </summary>
        /// <param name="data">The <c>SchemaData</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">data is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// data belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddSchemaData(SchemaData data)
        {
            this.AddChild(data);
        }
    }
}
