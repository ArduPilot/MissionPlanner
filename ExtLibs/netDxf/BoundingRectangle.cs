#region netDxf library, Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2016 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;

namespace netDxf
{
    /// <summary>
    /// Represents an axis aligned bounding rectangle.
    /// </summary>
    public class BoundingRectangle
    {
        #region private fields

        private Vector2 min;
        private Vector2 max;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle from a rotated ellipse.
        /// </summary>
        /// <param name="center">Center of the ellipse.</param>
        /// <param name="majorAxis">Major axis of the ellipse.</param>
        /// <param name="minorAxis">Minor axis of the ellipse.</param>
        /// <param name="rotation">Rotation in degrees of the ellipse.</param>
        public BoundingRectangle(Vector2 center, double majorAxis, double minorAxis, double rotation)
        {
            double rot = rotation*MathHelper.DegToRad;
            double a = majorAxis*0.5*Math.Cos(rot);
            double b = minorAxis*0.5*Math.Sin(rot);
            double c = majorAxis*0.5*Math.Sin(rot);
            double d = minorAxis*0.5*Math.Cos(rot);

            double width = Math.Sqrt(a*a + b*b)*2;
            double height = Math.Sqrt(c*c + d*d)*2;
            this.min = new Vector2(center.X - width*0.5, center.Y - height*0.5);
            this.max = new Vector2(center.X + width*0.5, center.Y + height*0.5);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle from a circle.
        /// </summary>
        /// <param name="center">Center of the bounding rectangle.</param>
        /// <param name="radius">Radius of the circle.</param>
        public BoundingRectangle(Vector2 center, double radius)
        {
            this.min = new Vector2(center.X - radius, center.Y - radius);
            this.max = new Vector2(center.X + radius, center.Y + radius);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="center">Center of the bounding rectangle.</param>
        /// <param name="width">Width of the bounding rectangle.</param>
        /// <param name="height">Height of the bounding rectangle.</param>
        public BoundingRectangle(Vector2 center, double width, double height)
        {
            this.min = new Vector2(center.X - width*0.5, center.Y - height*0.5);
            this.max = new Vector2(center.X + width*0.5, center.Y + height*0.5);
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="min">Lower-left corner.</param>
        /// <param name="max">Upper-right corner.</param>
        public BoundingRectangle(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Initializes a new axis aligned bounding rectangle.
        /// </summary>
        /// <param name="points">A list of Vector2.</param>
        public BoundingRectangle(IEnumerable<Vector2> points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            bool any = false;
            foreach (Vector2 point in points)
            {
                any = true;
                if (minX > point.X)
                    minX = point.X;
                if (minY > point.Y)
                    minY = point.Y;
                if (maxX < point.X)
                    maxX = point.X;
                if (maxY < point.Y)
                    maxY = point.Y;
            }
            if (any)
            {
                this.min = new Vector2(minX, minY);
                this.max = new Vector2(maxX, maxY);
            }
            else
            {
                this.min = new Vector2(double.MinValue, double.MinValue);
                this.max = new Vector2(double.MaxValue, double.MaxValue);
            }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the bounding rectangle lower-left corner.
        /// </summary>
        public Vector2 Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        /// <summary>
        /// Gets or sets the bounding rectangle upper-right corner.
        /// </summary>
        public Vector2 Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        /// <summary>
        /// Gets the bounding rectangle center.
        /// </summary>
        public Vector2 Center
        {
            get { return (this.min + this.max)*0.5; }
        }

        /// <summary>
        /// Gets the radius of the circle that contains the bounding rectangle.
        /// </summary>
        public double Radius
        {
            get { return Vector2.Distance(this.min, this.max)*0.5; }
        }

        /// <summary>
        /// Gets the bounding rectangle width.
        /// </summary>
        public double Width
        {
            get { return this.max.X - this.min.X; }
        }

        /// <summary>
        /// Gets the bounding rectangle height.
        /// </summary>
        public double Height
        {
            get { return this.max.Y - this.min.Y; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Checks if a point is inside the bounding rectangle.
        /// </summary>
        /// <param name="point">Vector2 to check.</param>
        /// <returns>True if the point is inside the bounding rectangle, false otherwise.</returns>
        /// <remarks></remarks>
        public bool PointInside(Vector2 point)
        {
            return point.X >= this.min.X && point.X <= this.max.X && point.Y >= this.min.Y && point.Y <= this.max.Y;
        }

        /// <summary>
        /// Obtains the union between two bounding rectangles.
        /// </summary>
        /// <param name="aabr1">A bounding rectangle.</param>
        /// <param name="aabr2">A bounding rectangle.</param>
        /// <returns>The resulting bounding rectangle.</returns>
        public static BoundingRectangle Union(BoundingRectangle aabr1, BoundingRectangle aabr2)
        {
            if (aabr1 == null && aabr2 == null)
                return null;
            if (aabr1 == null)
                return aabr2;
            if (aabr2 == null)
                return aabr1;

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();
            for (int i = 0; i < 2; i++)
            {
                if (aabr1.Min[i] <= aabr2.Min[i])
                    min[i] = aabr1.Min[i];
                else
                    min[i] = aabr2.Min[i];

                if (aabr1.Max[i] >= aabr2.Max[i])
                    max[i] = aabr1.Max[i];
                else
                    max[i] = aabr2.Max[i];
            }
            return new BoundingRectangle(min, max);
        }

        /// <summary>
        /// Obtains the union of a bounding rectangles list .
        /// </summary>
        /// <param name="rectangles">A list of bounding rectangles.</param>
        /// <returns>The resulting bounding rectangle.</returns>
        public static BoundingRectangle Union(IEnumerable<BoundingRectangle> rectangles)
        {
            if (rectangles == null)
                throw new ArgumentNullException(nameof(rectangles));

            BoundingRectangle rtnAABR = null;
            foreach (BoundingRectangle aabr in rectangles)
                rtnAABR = Union(rtnAABR, aabr);

            return rtnAABR;
        }

        #endregion
    }
}