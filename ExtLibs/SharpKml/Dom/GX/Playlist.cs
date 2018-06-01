using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Contains any number of <see cref="TourPrimitive"/> elements.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("Playlist", KmlNamespaces.GX22Namespace)]
    public sealed class Playlist : KmlObject
    {
        /// <summary>Initializes a new instance of the Playlist class.</summary>
        public Playlist()
        {
            this.RegisterValidChild<TourPrimitive>();
        }

        /// <summary>
        /// Gets the <see cref="TourPrimitive"/>s stored by this instance.
        /// </summary>
        public IEnumerable<TourPrimitive> Values
        {
            get { return this.Children.OfType<TourPrimitive>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="TourPrimitive"/> to this instance.
        /// </summary>
        /// <param name="tour">
        /// The <c>TourPrimitive</c> to add to this instance.
        /// </param>
        /// <exception cref="System.ArgumentNullException">tour is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// tour belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddTourPrimitive(TourPrimitive tour)
        {
            this.AddChild(tour);
        }
    }
}
