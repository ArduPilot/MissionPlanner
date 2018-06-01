using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpKml.Dom
{
    /// <summary>Represents a KML AbstractContainerGroup.</summary>
    /// <remarks>OGC KML 2.2 Section 9.6</remarks>
    public abstract class Container : Feature
    {
        /// <summary>Initializes a new instance of the Container class.</summary>
        internal Container()
        {
            // Cannot be inherited outside of this assembly because of the
            // need to register Feature as a valid child. This is because
            // in the Kml spec Features belongs in Document/Folder but the
            // C++ version has it here
        }

        /// <summary>
        /// Gets the <see cref="Feature"/>s contained by this instance.
        /// </summary>
        public IEnumerable<Feature> Features
        {
            get { return this.Children.OfType<Feature>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="Feature"/> to this instance.
        /// </summary>
        /// <param name="feature">The <c>Feature</c> to add to this instance.</param>
        /// <exception cref="ArgumentNullException">feature is null.</exception>
        /// <exception cref="InvalidOperationException">
        /// feature belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddFeature(Feature feature)
        {
            this.AddChild(feature);
        }

        /// <summary>
        /// Returns the first <see cref="Feature"/> found with the specified id.
        /// </summary>
        /// <param name="id">The id of the <c>Feature</c> to search for.</param>
        /// <returns>
        /// The first <c>Feature</c> matching the specified id, if any;
        /// otherwise, null.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public Feature FindFeature(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return this.Features.FirstOrDefault(f => string.Equals(f.Id, id, StringComparison.Ordinal));
        }

        /// <summary>
        /// Removes the specified <see cref="Feature"/> this instance.
        /// </summary>
        /// <param name="id">The Id of the <c>Feature</c> to remove.</param>
        /// <returns>
        /// true if the value parameter is successfully removed; otherwise,
        /// false. This method also returns false if item was not found in
        /// <see cref="Features"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">id is null.</exception>
        public bool RemoveFeature(string id)
        {
            Feature feature = this.FindFeature(id); // Will throw is id is null.
            if (feature != null)
            {
                return this.RemoveChild(feature);
            }
            return false;
        }
    }
}
