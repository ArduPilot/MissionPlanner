using System;
using SharpKml.Base;

namespace SharpKml.Engine
{
    /// <summary>Represents a simple geographic bounding box.</summary>
    /// <remarks>
    /// The antemeridian is not considered and the validity of the values are
    /// not checked.
    /// </remarks>
    public sealed class BoundingBox
    {
        /// <summary>
        /// Initializes a new instance of the BoundingBox class.
        /// </summary>
        public BoundingBox()
        {
            this.East = -180;
            this.North = -180;
            this.South = 180;
            this.West = 180;
        }

        /// <summary>
        /// Initializes a new instance of the BoundingBox class with the
        /// specified values;
        /// </summary>
        /// <param name="north">The value for <see cref="North"/>.</param>
        /// <param name="south">The value for <see cref="South"/>.</param>
        /// <param name="east">The value for <see cref="East"/>.</param>
        /// <param name="west">The value for <see cref="West"/>.</param>
        public BoundingBox(double north, double south, double east, double west)
        {
            this.East = east;
            this.North = north;
            this.South = south;
            this.West = west;
        }

        /// <summary>Gets the center of this instance.</summary>
        public Vector Center
        {
            get
            {
                double latitude = (this.North + this.South) / 2.0;
                double longitude = (this.East + this.West) / 2.0;
                return new Vector(latitude, longitude);
            }
        }

        /// <summary>
        /// Gets or sets the longitude of the east edge of the bounding box,
        /// in decimal degrees from 0 to ±180.
        /// </summary>
        public double East { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty or not;
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return (this.East == -180) &&
                       (this.North == -180) &&
                       (this.South == 180) &&
                       (this.West == 180);
            }
        }

        /// <summary>
        /// Gets or sets the latitude of the north edge of the bounding box,
        /// in decimal degrees from 0 to ±90.
        /// </summary>
        public double North { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the south edge of the bounding box,
        /// in decimal degrees from 0 to ±90.
        /// </summary>
        public double South { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the west edge of the bounding box,
        /// in decimal degrees from 0 to ±180.
        /// </summary>
        public double West { get; set; }

        /// <summary>
        /// Aligns this instance within the specified quadrant tree down to the
        /// maximum level specified.
        /// </summary>
        /// <param name="quadTree">The quadrant tree to align to.</param>
        /// <param name="maxDepth">The maximum depth to iterate for.</param>
        /// <exception cref="ArgumentNullException">quadTree is null.</exception>
        public void Align(BoundingBox quadTree, int maxDepth)
        {
            if (quadTree == null)
            {
                throw new ArgumentNullException("quadTree");
            }

            Vector center = quadTree.Center;
            if (this.ContainedBy(new BoundingBox(quadTree.North, center.Latitude, quadTree.East, center.Longitude)))
            {
                quadTree.South = center.Latitude;
                quadTree.West = center.Longitude;
            }
            else if (this.ContainedBy(new BoundingBox(quadTree.North, center.Latitude, center.Longitude, quadTree.West)))
            {
                quadTree.South = center.Latitude;
                quadTree.East = center.Longitude;
            }
            else if (this.ContainedBy(new BoundingBox(center.Latitude, quadTree.South, quadTree.East, center.Longitude)))
            {
                quadTree.North = center.Latitude;
                quadTree.West = center.Longitude;
            }
            else if (this.ContainedBy(new BoundingBox(center.Latitude, quadTree.South, center.Longitude, quadTree.West)))
            {
                quadTree.North = center.Latitude;
                quadTree.East = center.Longitude;
            }
            else
            {
                return;  // Target not contained by any child quadrant of quadTree.
            }

            // Fall through from above and recurse.
            if (maxDepth > 0)
            {
                this.Align(quadTree, maxDepth - 1);
            }
        }

        /// <summary>
        /// Determines if this instance is inside the specified bounds.
        /// </summary>
        /// <param name="box">The BoundingBox to compare.</param>
        /// <returns>
        /// true if the specified value parameter contains this instance;
        /// otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">box is null.</exception>
        public bool ContainedBy(BoundingBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }

            return (box.North >= this.North) &&
                   (box.South <= this.South) &&
                   (box.East >= this.East) &&
                   (box.West <= this.West);
        }

        /// <summary>
        /// Determines the specified coordinates are inside this instance.
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate.</param>
        /// <param name="longitude">The longitude of the coordinate.</param>
        /// <returns>
        /// true if the specified point is inside the bounds of this instance;
        /// otherwise, false.
        /// </returns>
        public bool Contains(double latitude, double longitude)
        {
            return (this.North >= latitude) &&
                   (this.South <= latitude) &&
                   (this.East >= longitude) &&
                   (this.West <= longitude);
        }

        /// <summary>
        /// Expands the bounding box to conatin the specified bounds.
        /// </summary>
        /// <param name="box">The bounds to contain by this instance.</param>
        /// <exception cref="ArgumentNullException">box is null.</exception>
        public void Expand(BoundingBox box)
        {
            if (box == null)
            {
                throw new ArgumentNullException("box");
            }

            this.ExpandLatitude(box.North);
            this.ExpandLatitude(box.South);
            this.ExpandLongitude(box.East);
            this.ExpandLongitude(box.West);
        }

        /// <summary>
        /// Expands the bounding box to contain the specified coordinates.
        /// </summary>
        /// <param name="latitude">
        /// The latitude the new bounds will contain.
        /// </param>
        /// <param name="longitude">
        /// The longitude the new bounds will contain.
        /// </param>
        public void Expand(double latitude, double longitude)
        {
            this.ExpandLatitude(latitude);
            this.ExpandLongitude(longitude);
        }

        /// <summary>
        /// Expands the bounding box to include the specified latitude.
        /// </summary>
        /// <param name="latitude">
        /// The latitude the new bounds will contain.
        /// </param>
        public void ExpandLatitude(double latitude)
        {
            if (latitude > this.North)
            {
                this.North = latitude;
            }
            if (latitude < this.South)
            {
                this.South = latitude;
            }
        }

        /// <summary>
        /// Expands the bounding box to include the specified longitude.
        /// </summary>
        /// <param name="longitude">
        /// The longitude the new bounds will contain.
        /// </param>
        public void ExpandLongitude(double longitude)
        {
            if (longitude > this.East)
            {
                this.East = longitude;
            }
            if (longitude < this.West)
            {
                this.West = longitude;
            }
        }
    }
}
