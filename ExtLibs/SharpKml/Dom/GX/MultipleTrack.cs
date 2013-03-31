using System.Collections.Generic;
using System.Linq;
using SharpKml.Base;

namespace SharpKml.Dom.GX
{
    /// <summary>
    /// Used to combine multiple <see cref="Track"/>s into a single conceptual unit.
    /// </summary>
    /// <remarks>This is not part of the OGC KML 2.2 standard.</remarks>
    [KmlElement("MultiTrack", KmlNamespaces.GX22Namespace)]
    public sealed class MultipleTrack : Geometry
    {
        /// <summary>Initializes a new instance of the MultipleTrack class.</summary>
        public MultipleTrack()
        {
            this.RegisterValidChild<Track>();
        }

        /// <summary>
        /// Gets or sets whether to interpolate missing values between the end
        /// of the first track and the beginning of the next one.
        /// </summary>
        [KmlElement("interpolate", KmlNamespaces.GX22Namespace)]
        public bool? Interpolate { get; set; }

        /// <summary>
        /// Gets the <see cref="Track"/>s contained by this instance.
        /// </summary>
        public IEnumerable<Track> Tracks
        {
            get { return this.Children.OfType<Track>(); }
        }

        /// <summary>
        /// Adds the specified <see cref="Track"/> to this instance.
        /// </summary>
        /// <param name="track">The <c>Track</c> to add to this instance.</param>
        /// <exception cref="System.ArgumentNullException">track is null.</exception>
        /// <exception cref="System.InvalidOperationException">
        /// track belongs to another <see cref="Element"/>.
        /// </exception>
        public void AddTrack(Track track)
        {
            this.AddChild(track);
        }
    }
}
