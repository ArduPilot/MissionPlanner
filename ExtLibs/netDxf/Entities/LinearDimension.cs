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
using netDxf.Blocks;
using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a linear or rotated dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class LinearDimension :
        Dimension
    {
        #region private fields

        private Vector2 firstRefPoint;
        private Vector2 secondRefPoint;
        private double offset;
        private double rotation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        public LinearDimension()
            : this(Vector2.Zero, Vector2.UnitX, 0.1, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation)
            : this(referenceLine, offset, rotation, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation, DimensionStyle style)
            : this(referenceLine, offset, rotation, Vector3.UnitZ, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation, Vector3 normal)
            : this(referenceLine, offset, rotation, normal, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="referenceLine">Reference <see cref="Line">line</see> of the dimension.</param>
        /// <param name="offset">Distance between the reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference line define the distance to be measure.</remarks>
        public LinearDimension(Line referenceLine, double offset, double rotation, Vector3 normal, DimensionStyle style)
            : base(DimensionType.Linear)
        {
            if (referenceLine == null)
                throw new ArgumentNullException(nameof(referenceLine));

            IList<Vector3> ocsPoints = MathHelper.Transform(
                new List<Vector3> {referenceLine.StartPoint, referenceLine.EndPoint}, normal, CoordinateSystem.World, CoordinateSystem.Object);
            this.firstRefPoint = new Vector2(ocsPoints[0].X, ocsPoints[0].Y);
            this.secondRefPoint = new Vector2(ocsPoints[1].X, ocsPoints[1].Y);

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            this.offset = offset;
            this.rotation = MathHelper.NormalizeAngle(rotation);

            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.Style = style;
            this.Normal = normal;
            this.Elevation = ocsPoints[0].Z;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation)
            : this(firstPoint, secondPoint, offset, rotation, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinearDimension</c> class.
        /// </summary>
        /// <param name="firstPoint">First reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="secondPoint">Second reference <see cref="Vector2">point</see> of the dimension.</param>
        /// <param name="offset">Distance between the mid point reference line and the dimension line.</param>
        /// <param name="rotation">Rotation in degrees of the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        /// <remarks>The reference points define the distance to be measure.</remarks>
        public LinearDimension(Vector2 firstPoint, Vector2 secondPoint, double offset, double rotation, DimensionStyle style)
            : base(DimensionType.Linear)
        {
            this.firstRefPoint = firstPoint;
            this.secondRefPoint = secondPoint;
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            this.offset = offset;
            this.rotation = MathHelper.NormalizeAngle(rotation);
            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.Style = style;
            this.Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first definition point of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 FirstReferencePoint
        {
            get { return this.firstRefPoint; }
            set { this.firstRefPoint = value; }
        }

        /// <summary>
        /// Gets or sets the second definition point of the dimension in OCS (object coordinate system).
        /// </summary>
        public Vector2 SecondReferencePoint
        {
            get { return this.secondRefPoint; }
            set { this.secondRefPoint = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line.
        /// </summary>
        public Vector2 DimLinePosition
        {
            get { return this.defPoint; }
        }

        /// <summary>
        /// Gets or sets the rotation of the dimension line.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = MathHelper.NormalizeAngle(value); }
        }

        /// <summary>
        /// Gets or sets the distance between the mid point of the reference line and the dimension line.
        /// </summary>
        /// <remarks>
        /// The offset value must be equal or greater than zero.<br />
        /// The side at which the dimension line is drawn depends of its rotation.
        /// </remarks>
        public double Offset
        {
            get { return this.offset; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "The offset value must be equal or greater than zero.");
                this.offset = value;
            }
        }

        /// <summary>
        /// Gets the actual measurement.
        /// </summary>
        /// <remarks>The dimension is always measured in the plane defined by the normal.</remarks>
        public override double Measurement
        {
            get
            {
                double refRot = Vector2.Angle(this.firstRefPoint, this.secondRefPoint);
                return Math.Abs(Vector2.Distance(this.firstRefPoint, this.secondRefPoint)*Math.Cos(this.rotation*MathHelper.DegToRad - refRot));
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        public void SetDimensionLinePosition(Vector2 point)
        {
            Vector2 ref1 = this.firstRefPoint;
            Vector2 ref2 = this.secondRefPoint;
            Vector2 midRef = Vector2.MidPoint(ref1, ref2);
            //Vector2 dimRef = this.DefinitionPoint;

            Vector2 refDir = Vector2.Normalize(this.secondRefPoint - this.firstRefPoint);
            double dimRotation = this.rotation * MathHelper.DegToRad;
            Vector2 dimDir = new Vector2(Math.Cos(dimRotation), Math.Sin(dimRotation));
            
            double cross = Vector2.CrossProduct(refDir, point - this.firstRefPoint);
            if (cross < 0)
            {
                Vector2 tmp = this.firstRefPoint;
                this.firstRefPoint = this.secondRefPoint;
                this.secondRefPoint = tmp;
                this.Rotation += 180;
                dimRotation = this.rotation*MathHelper.DegToRad;
            }

            this.offset = MathHelper.PointLineDistance(midRef, point, dimDir);

            Vector2 offsetDir = Vector2.Rotate(Vector2.UnitY, dimRotation);
            Vector2 midDimLine = midRef + this.offset* offsetDir;

            this.defPoint = midDimLine - this.Measurement * 0.5 * Vector2.Perpendicular(Vector2.Normalize(offsetDir));

            if (!this.TextPositionManuallySet)
            {
                DimensionStyleOverride styleOverride;
                double textGap = this.Style.TextOffset;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (double)styleOverride.Value;
                }
                double scale = this.Style.DimScaleOverall;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (double)styleOverride.Value;
                }

                double gap = textGap * scale;
                if (dimRotation > MathHelper.HalfPI && dimRotation <= MathHelper.ThreeHalfPI)
                {
                    gap = -gap;
                }
                this.textRefPoint = midDimLine + gap * offsetDir;
            }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Calculate the dimension reference points.
        /// </summary>
        protected override void CalculteReferencePoints()
        {
            DimensionStyleOverride styleOverride;

            double measure = this.Measurement;
            Vector2 ref1 = this.firstRefPoint;
            Vector2 ref2 = this.secondRefPoint;
            Vector2 midRef = Vector2.MidPoint(ref1, ref2);
            double dimRotation = this.Rotation * MathHelper.DegToRad;

            Vector2 vec = Vector2.Normalize(Vector2.Rotate(Vector2.UnitY, dimRotation));
            Vector2 midDimLine = midRef + this.offset * vec;
            double cross = Vector2.CrossProduct(ref2 - ref1, vec);
            if (cross < 0)
            {
                this.firstRefPoint = ref2;
                this.secondRefPoint = ref1;
            }
            this.defPoint = midDimLine - measure * 0.5 * Vector2.Perpendicular(vec);

            if (this.TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    this.SetDimensionLinePosition(this.textRefPoint);
                }
            }
            else
            {
                double textGap = this.Style.TextOffset;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.TextOffset, out styleOverride))
                {
                    textGap = (double)styleOverride.Value;
                }
                double scale = this.Style.DimScaleOverall;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.DimScaleOverall, out styleOverride))
                {
                    scale = (double)styleOverride.Value;
                }

                double gap = textGap * scale;
                if (dimRotation > MathHelper.HalfPI && dimRotation <= MathHelper.ThreeHalfPI)
                    gap = -gap;

                this.textRefPoint = midDimLine + gap * vec;
            }
        }

        /// <summary>
        /// Gets the block that contains the entities that make up the dimension picture.
        /// </summary>
        /// <param name="name">Name to be assigned to the generated block.</param>
        /// <returns>The block that represents the actual dimension.</returns>
        protected override Block BuildBlock(string name)
        {
            return DimensionBlock.Build(this, name);
        }

        /// <summary>
        /// Creates a new LinearDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new LinearDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            LinearDimension entity = new LinearDimension
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
                //Dimension properties
                Style = (DimensionStyle) this.Style.Clone(),
                DefinitionPoint = this.DefinitionPoint,
                TextReferencePoint = this.TextReferencePoint,
                TextPositionManuallySet = this.TextPositionManuallySet,
                TextRotation = this.TextRotation,
                AttachmentPoint = this.AttachmentPoint,
                LineSpacingStyle = this.LineSpacingStyle,
                LineSpacingFactor = this.LineSpacingFactor,
                UserText = this.UserText,
                //LinearDimension properties
                FirstReferencePoint = this.firstRefPoint,
                SecondReferencePoint = this.secondRefPoint,
                Rotation = this.rotation,
                Offset = this.offset,
                Elevation = this.Elevation
            };

            foreach (DimensionStyleOverride styleOverride in this.StyleOverrides.Values)
            {
                object copy;
                ICloneable value = styleOverride.Value as ICloneable;
                copy = value != null ? value.Clone() : styleOverride.Value;

                entity.StyleOverrides.Add(new DimensionStyleOverride(styleOverride.Type, copy));
            }

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}