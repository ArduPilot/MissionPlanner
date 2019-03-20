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

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a shape linetype segment.
    /// </summary>
    public class LinetypeShapeSegment :
        LinetypeSegment
    {
        #region private fields

        private string name;
        private ShapeStyle style;
        private Vector2 offset;
        private LinetypeSegmentRotationType rotationType;
        private double rotation;
        private double scale;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>LinetypeShapeSegment</c> class.
        /// </summary>
        /// <param name="name">Shape name of the linetype segment.</param>
        /// <param name="style">File where the shape of the linetype segment is defined.</param>
        /// <remarks>
        /// The shape must be defined in the .shx shape definitions file.<br />
        /// The dxf instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
        /// it stores the shape number. Therefore when saving a dxf file the shape number will be obtained reading the .shp file.<br />
        /// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
        /// </remarks>
        public LinetypeShapeSegment(string name, ShapeStyle style) : this(name, style, 0.0, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinetypeShapeSegment</c> class.
        /// </summary>
        /// <param name="name">Shape name of the linetype segment.</param>
        /// <param name="style">File where the shape of the linetype segment is defined.</param>
        /// <param name="length">Dash, dot, or space length of the linetype segment.</param>
        /// <remarks>
        /// The shape must be defined in the .shx shape definitions file.<br />
        /// The dxf instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
        /// it stores the shape number. Therefore when saving a dxf file the shape number will be obtained reading the .shp file.<br />
        /// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
        /// </remarks>
        public LinetypeShapeSegment(string name, ShapeStyle style, double length) : this(name, style, length, Vector2.Zero, LinetypeSegmentRotationType.Relative, 0.0, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinetypeShapeSegment</c> class.
        /// </summary>
        /// <param name="name">Shape name of the linetype segment.</param>
        /// <param name="style">File where the shape of the linetype segment is defined.</param>
        /// <param name="length">Dash, dot, or space length of the linetype segment.</param>
        /// <param name="offset">Shift of the shape along the line.</param>
        /// <param name="rotationType">Type of rotation defined by the rotation value.</param>
        /// <param name="rotation">Rotation of the shape.</param>
        /// <param name="scale">Scale of the shape.</param>
        /// <remarks>
        /// The shape must be defined in the .shx shape definitions file.<br />
        /// The dxf instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
        /// it stores the shape number. Therefore when saving a dxf file the shape number will be obtained reading the .shp file.<br />
        /// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
        /// </remarks>
        public LinetypeShapeSegment(string name, ShapeStyle style, double length, Vector2 offset, LinetypeSegmentRotationType rotationType, double rotation, double scale) : base(LinetypeSegmentType.Shape, length)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The linetype shape name should be at least one character long.");
            this.name = name;
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.style = style;
            this.offset = offset;
            this.rotationType = rotationType;
            this.rotation = MathHelper.NormalizeAngle(rotation);
            if (scale <= 0)
                throw new ArgumentOutOfRangeException(nameof(scale), scale, "The LinetypeShepeSegment scale must be greater than zero.");
            this.scale = scale;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the name of the shape.
        /// </summary>
        /// <remarks>
        /// The shape must be defined in the .shx shape definitions file.<br />
        /// The dxf instead of saving the shape name, as the Shape entity or the shape linetype segments definition in a .lin file,
        /// it stores the shape number. Therefore when saving a dxf file the shape number will be obtained reading the .shp file.
        /// </remarks>
        public string Name
        {
            get { return this.name; }
            internal set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException(nameof(value), "The linetype shape name should be at least one character long.");
                this.name = value;
            }
        }

        /// <summary>
        /// Gets the shape style.
        /// </summary>
        /// <remarks>
        /// It is required that the equivalent .shp file to be also present in the same folder or one of the support folders defined in the DxfDocument.
        /// </remarks>
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
        /// Gets or sets the shift of the shape along the line.
        /// </summary>
        public Vector2 Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets or sets the type of rotation defined by the rotation value upright, relative, or absolute.
        /// </summary>
        public LinetypeSegmentRotationType RotationType
        {
            get { return this.rotationType; }
            set { this.rotationType = value; }
        }

        /// <summary>
        /// Gets or sets the angle in degrees of the shape.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the scale of the shape relative to the scale of the line type.
        /// </summary>
        /// <remarks>
        /// If the size of the shape style is 0, the scale value alone is used as the size.
        /// </remarks>
        public double Scale
        {
            get { return this.scale; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The linetype shape segment scale must be greater than zero.");
                this.scale = value;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new <c>LinetypeShapeSegment</c> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <c>LinetypeShapeSegment</c> that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new LinetypeShapeSegment(this.name, (ShapeStyle) this.style.Clone(), this.Length)
            {
                Offset = this.offset,
                RotationType = this.rotationType,
                Rotation = this.rotation,
                Scale = this.scale
            };
        }

        #endregion
    }
}