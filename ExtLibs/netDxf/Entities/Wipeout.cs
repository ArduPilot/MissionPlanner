#region netDxf library, Copyright (C) 2009-2017 Daniel Carvajal (haplokuon@gmail.com)

//                        netDxf library
// Copyright (C) 2009-2017 Daniel Carvajal (haplokuon@gmail.com)
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
    /// Represents a wipeout <see cref="EntityObject">entity</see>.
    /// </summary>
    /// <remarks>
    /// The Wipeout dxf definition includes three variables for brightness, contrast, and fade but those variables have no effect; in AutoCad you cannot even change them.<br/>
    /// The Wipeout entity is related with the system variable WIPEOUTFRAME but this variable is not saved in a dxf.
    /// </remarks>
    public class Wipeout :
        EntityObject
    {
        #region private fields

        private ClippingBoundary clippingBoundary;
        private double elevation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Wipeout</c> class as a rectangular wipeout.
        /// </summary>
        /// <param name="x">Rectangle x-coordinate of the bottom-left corner in local coordinates.</param>
        /// <param name="y">Rectangle y-coordinate of the bottom-left corner in local coordinates.</param>
        /// <param name="width">Rectangle width in local coordinates.</param>
        /// <param name="height">Rectangle height in local coordinates.</param>
        public Wipeout(double x, double y, double width, double height)
            : this(new ClippingBoundary(x, y, width, height))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Wipeout</c> class as a rectangular wipeout from two opposite corners.
        /// </summary>
        /// <param name="firstCorner">Rectangle firstCorner in local coordinates.</param>
        /// <param name="secondCorner">Rectangle secondCorner in local coordinates.</param>
        public Wipeout(Vector2 firstCorner, Vector2 secondCorner)
            : this(new ClippingBoundary(firstCorner, secondCorner))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Wipeout</c> class as a polygonal wipeout.
        /// </summary>
        /// <param name="vertexes">The list of vertexes of the wipeout.</param>
        public Wipeout(IEnumerable<Vector2> vertexes)
            : this(new ClippingBoundary(vertexes))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Wipeout</c> class.
        /// </summary>
        /// <param name="clippingBoundary">The wipeout clipping boundary.</param>
        public Wipeout(ClippingBoundary clippingBoundary)
            : base(EntityType.Wipeout, DxfObjectCode.Wipeout)
        {
            if (clippingBoundary == null)
                throw new ArgumentNullException(nameof(clippingBoundary));
            this.clippingBoundary = clippingBoundary;
            this.elevation = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the wipeout clipping boundary.
        /// </summary>
        public ClippingBoundary ClippingBoundary
        {
            get { return this.clippingBoundary; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.clippingBoundary = value;
            }
        }

        /// <summary>
        /// Gets or sets the wipeout elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the wipeout boundary.</remarks>
        public double Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Wipeout that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Wipeout that is a copy of this instance.</returns>
        public override object Clone()
        {
            Wipeout entity = new Wipeout((ClippingBoundary) this.ClippingBoundary.Clone())
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
                //Wipeout properties
                Elevation = this.elevation
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}