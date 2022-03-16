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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an extension line <see cref="EntityObject">entity</see> (aka construction line).
    /// </summary>
    /// <remarks>An extension line is a line in three-dimensional space that starts in the specified origin and extends to infinity in both directions.</remarks>
    public class XLine :
        EntityObject
    {
        #region private fields

        private Vector3 origin;
        private Vector3 direction;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        public XLine()
            : this(Vector3.Zero, Vector3.UnitX)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        /// <param name="origin">XLine <see cref="Vector2">origin.</see></param>
        /// <param name="direction">XLine <see cref="Vector2">direction.</see></param>
        public XLine(Vector2 origin, Vector2 direction)
            : this(new Vector3(origin.X, origin.Y, 0.0), new Vector3(direction.X, direction.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>XLine</c> class.
        /// </summary>
        /// <param name="origin">XLine <see cref="Vector3">origin.</see></param>
        /// <param name="direction">XLine <see cref="Vector3">direction.</see></param>
        public XLine(Vector3 origin, Vector3 direction)
            : base(EntityType.XLine, DxfObjectCode.XLine)
        {
            this.origin = origin;
            this.direction = Vector3.Normalize(direction);
            if (Vector3.IsNaN(this.direction))
                throw new ArgumentException("The direction can not be the zero vector.", nameof(direction));
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the extension line <see cref="netDxf.Vector3">origin</see>.
        /// </summary>
        public Vector3 Origin
        {
            get { return this.origin; }
            set { this.origin = value; }
        }

        /// <summary>
        /// Gets or sets the extension line <see cref="netDxf.Vector3">direction</see>.
        /// </summary>
        public Vector3 Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = Vector3.Normalize(value);
                if (Vector3.IsNaN(this.direction))
                    throw new ArgumentException("The direction can not be the zero vector.", nameof(value));
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new XLine that is a copy of the current instance.
        /// </summary>
        /// <returns>A new XLine that is a copy of this instance.</returns>
        public override object Clone()
        {
            XLine entity = new XLine
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
                //XLine properties
                Origin = this.origin,
                Direction = this.direction,
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}