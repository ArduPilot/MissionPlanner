#region netDxf library, Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2018 Daniel Carvajal (haplokuon@gmail.com)
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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a circular arc <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Arc :
        EntityObject
    {
        #region private fields

        private Vector3 center;
        private double radius;
        private double startAngle;
        private double endAngle;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        public Arc()
            : this(Vector3.Zero, 1.0, 0.0, 180.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        /// <param name="center">Arc <see cref="Vector2">center</see> in world coordinates.</param>
        /// <param name="radius">Arc radius.</param>
        /// <param name="startAngle">Arc start angle in degrees.</param>
        /// <param name="endAngle">Arc end angle in degrees.</param>
        public Arc(Vector2 center, double radius, double startAngle, double endAngle)
            : this(new Vector3(center.X, center.Y, 0.0), radius, startAngle, endAngle)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Arc</c> class.
        /// </summary>
        /// <param name="center">Arc <see cref="Vector3">center</see> in world coordinates.</param>
        /// <param name="radius">Arc radius.</param>
        /// <param name="startAngle">Arc start angle in degrees.</param>
        /// <param name="endAngle">Arc end angle in degrees.</param>
        public Arc(Vector3 center, double radius, double startAngle, double endAngle)
            : base(EntityType.Arc, DxfObjectCode.Arc)
        {
            this.center = center;
            if (radius <= 0)
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "The circle radius must be greater than zero.");
            this.radius = radius;
            this.startAngle = MathHelper.NormalizeAngle(startAngle);
            this.endAngle = MathHelper.NormalizeAngle(endAngle);
            this.thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the arc <see cref="Vector3">center</see> in world coordinates.
        /// </summary>
        public Vector3 Center
        {
            get { return this.center; }
            set { this.center = value; }
        }

        /// <summary>
        /// Gets or sets the arc radius.
        /// </summary>
        public double Radius
        {
            get { return this.radius; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The arc radius must be greater than zero.");
                this.radius = value;
            }
        }

        /// <summary>
        /// Gets or sets the arc start angle in degrees.
        /// </summary>
        public double StartAngle
        {
            get { return this.startAngle; }
            set { this.startAngle = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the arc end angle in degrees.
        /// </summary>
        public double EndAngle
        {
            get { return this.endAngle; }
            set { this.endAngle = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the arc thickness.
        /// </summary>
        public double Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Converts the arc in a list of vertexes.
        /// </summary>
        /// <param name="precision">Number of divisions.</param>
        /// <returns>A list vertexes that represents the arc expressed in object coordinate system.</returns>
        public List<Vector2> PolygonalVertexes(int precision)
        {
            if (precision < 2)
                throw new ArgumentOutOfRangeException(nameof(precision), precision, "The arc precision must be greater or equal to three");

            List<Vector2> ocsVertexes = new List<Vector2>();
            double start = this.startAngle*MathHelper.DegToRad;
            double end = this.endAngle*MathHelper.DegToRad;
            if (end < start) end += MathHelper.TwoPI;
            double delta = (end - start)/precision;
            for (int i = 0; i <= precision; i++)
            {
                double angle = start + delta*i;
                double sine = this.radius*Math.Sin(angle);
                double cosine = this.radius*Math.Cos(angle);
                ocsVertexes.Add(new Vector2(cosine, sine));
            }

            return ocsVertexes;
        }

        /// <summary>
        /// Converts the arc in a Polyline.
        /// </summary>
        /// <param name="precision">Number of divisions.</param>
        /// <returns>A new instance of <see cref="LwPolyline">LightWeightPolyline</see> that represents the arc.</returns>
        public LwPolyline ToPolyline(int precision)
        {
            IEnumerable<Vector2> vertexes = this.PolygonalVertexes(precision);
            Vector3 ocsCenter = MathHelper.Transform(this.center, this.Normal, CoordinateSystem.World, CoordinateSystem.Object);

            LwPolyline poly = new LwPolyline
            {
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                Elevation = ocsCenter.Z,
                Thickness = this.Thickness,
                IsClosed = false
            };
            foreach (Vector2 v in vertexes)
            {
                poly.Vertexes.Add(new LwPolylineVertex(v.X + ocsCenter.X, v.Y + ocsCenter.Y));
            }
            return poly;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Arc that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Arc that is a copy of this instance.</returns>
        public override object Clone()
        {
            Arc entity = new Arc
            {
                //EntityObject properties
                Layer = (Layer) this.Layer.Clone(),
                Linetype = (Linetype) this.Linetype.Clone(),
                Color = (AciColor) this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency) this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Arc properties
                Center = this.center,
                Radius = this.radius,
                StartAngle = this.startAngle,
                EndAngle = this.endAngle,
                Thickness = this.thickness
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}