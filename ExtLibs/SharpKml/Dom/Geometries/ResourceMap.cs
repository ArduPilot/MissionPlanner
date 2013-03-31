using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom
{
    /// <summary>Represents a collection of resource aliases.</summary>
    /// <remarks>
    /// <para>OGC KML 2.2 Section 10.13</para>
    /// <para>This element allows texture files to be moved and renamed without
    /// having to update the original textured 3D object file that references
    /// those textures. One ResourceMap element can contain multiple mappings
    /// from different source textured object files into the same target
    /// resource.</para>
    /// </remarks>
    [KmlElement("ResourceMap")]
    public sealed class ResourceMap : KmlObject
    {
        /// <summary>Initializes a new instance of the ResourceMap class.</summary>
        public ResourceMap()
        {
            this.RegisterValidChild<Alias>();
        }

        /// <summary>Gets a collection of untyped name/value pairs.</summary>
        public IEnumerable<Alias> Aliases
        {
            get { return this.Children.OfType<Alias>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="Alias"/> to this instance.
        /// </summary>
        /// <param name="alias">The <c>Alias</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">alias is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// alias belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddAlias(Alias alias)
        {
            this.AddChild(alias);
        }
    }
}
