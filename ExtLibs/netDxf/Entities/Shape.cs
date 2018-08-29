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
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a shape entity.
    /// </summary>
    public class Shape :
        EntityObject
    {
        #region private fields

        private string name;
        private ShapeStyle style;
        private Vector3 position;
        private double size;
        private double rotation;
        private double obliqueAngle;
        private double widthFactor;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="TextStyle">style</see>.</param>
        public Shape(string name, ShapeStyle style) : this(name, style, Vector3.Zero, 1.0, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Shape</c> class.
        /// </summary>
        /// <param name="name">Name of the shape which geometry is defined in the shape <see cref="ShapeStyle">style</see>.</param>
        /// <param name="style">Shape <see cref="TextStyle">style</see>.</param>
        /// <param name="position">Shape insertion point.</param>
        /// <param name="size">Shape size.</param>
        /// <param name="rotation">Shape rotation.</param>
        public Shape(string name, ShapeStyle style, Vector3 position, double size, double rotation) : base(EntityType.Shape, DxfObjectCode.Shape)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            this.name = name;
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.position = position;
            this.size = size;
            this.rotation = rotation;
            this.obliqueAngle = 0.0;
            this.widthFactor = 1.0;
            this.thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the shape name.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the <see cref="ShapeStyle">shape style</see>.
        /// </summary>
        public ShapeStyle Style
        {
            get { return this.style; }
            internal set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.style = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape <see cref="Vector3">insertion point</see> in world coordinates.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets or sets the size of the shape.
        /// </summary>
        /// <remarks>
        /// The shape size is relative to the actual size of the shape definition.
        /// The size value works as an scale value applied to the dimensions of the shape definition,
        /// it cannot be zero and, negative values will invert the shape in the local X and Y axis.<br />
        /// Values cannot be zero. Default: 1.0.
        /// </remarks>
        public double Size
        {
            get { return this.size; }
            set
            {
                if (MathHelper.IsZero(value))
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape cannot be zero.");
                this.size = value;
            }
        }

        /// <summary>
        /// Gets or sets the shape rotation in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape oblique angle in degrees.
        /// </summary>
        public double ObliqueAngle
        {
            get { return this.obliqueAngle; }
            set { this.obliqueAngle = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the shape width factor.
        /// </summary>
        /// <remarks>Valid values must be greater than zero. Default: 1.0.</remarks>
        public double WidthFactor
        {
            get { return this.widthFactor; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The shape width factor must be greater than zero.");
                this.widthFactor = value;
            }
        }

        /// <summary>
        /// Gets or set the shape thickness.
        /// </summary>
        public double Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region overrides

        public override object Clone()
        {
            Shape entity = new Shape(this.name, (ShapeStyle)this.style.Clone())
            {
                //EntityObject properties
                Layer = (Layer)this.Layer.Clone(),
                Linetype = (Linetype)this.Linetype.Clone(),
                Color = (AciColor)this.Color.Clone(),
                Lineweight = this.Lineweight,
                Transparency = (Transparency)this.Transparency.Clone(),
                LinetypeScale = this.LinetypeScale,
                Normal = this.Normal,
                IsVisible = this.IsVisible,
                //Shape properties
                Position = this.position,
                Size = this.size,
                Rotation = this.rotation,
                ObliqueAngle = this.obliqueAngle,
                Thickness = this.thickness
        };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData)data.Clone());

            return entity;
        }

        #endregion
    }
}
