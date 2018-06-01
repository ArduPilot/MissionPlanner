using System;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>
    /// Provides extension methods for <see cref="Geometry"/> objects.
    /// </summary>
    public static class GeometryExtensions
    {
        /// <summary>
        /// Calculates the coordinates of the bounds of the <see cref="Geometry"/>.
        /// </summary>
        /// <param name="geometry">The geometry instance.</param>
        /// <returns>
        /// A <c>BoundingBox</c> containing the coordinates of the bounds of the
        /// geometry or null if the bounds could not be calculated.
        /// </returns>
        /// <exception cref="ArgumentNullException">geometry is null.</exception>
        public static BoundingBox CalculateBounds(this Geometry geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }

            BoundingBox box = new BoundingBox();
            ExpandBox(geometry, box);

            return box.IsEmpty ? null : box;
        }

        /// <summary>
        /// Expands the BoundingBox to include the specified Geometry.
        /// </summary>
        /// <param name="geometry">The Geometry to expand the box to include.</param>
        /// <param name="box">The BoundingBox to expand.</param>
        internal static void ExpandBox(Geometry geometry, BoundingBox box)
        {
            MultipleGeometry multiple = geometry as MultipleGeometry;
            if (multiple != null)
            {
                foreach (var geo in multiple.Geometry)
                {
                    ExpandBox(geo, box);
                }
            }
            else
            {
                // Try it as an IBoundsInformation, doesn't matter if the conversion fails
                ExpandBox(geometry as IBoundsInformation, box);
            }
        }

        /// <summary>
        /// Expands the BoundingBox to include the specified coordinates.
        /// </summary>
        /// <param name="bounds">A collection of coordinates to expand the box with.</param>
        /// <param name="box">The BoundingBox to expand.</param>
        internal static void ExpandBox(IBoundsInformation bounds, BoundingBox box)
        {
            if (bounds != null)
            {
                foreach (var coord in bounds.Coordinates)
                {
                    box.Expand(coord.Latitude, coord.Longitude);
                }
            }
        }        
    }
}
