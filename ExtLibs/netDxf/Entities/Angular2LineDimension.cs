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
    /// Represents a 3 point angular dimension <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Angular2LineDimension :
        Dimension
    {
        #region private fields

        private double offset;
        private Vector2 startFirstLine;
        private Vector2 endFirstLine;
        private Vector2 startSecondLine;
        private Vector2 endSecondLine;
        private Vector2 arcDefinitionPoint;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        public Angular2LineDimension()
            : this(Vector2.Zero, Vector2.UnitX, Vector2.Zero, Vector2.UnitY, 0.1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset)
            : this(firstLine, secondLine, offset, Vector3.UnitZ, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="normal">Normal vector of the plane where the dimension is defined.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal)
            : this(firstLine, secondLine, offset, normal, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, DimensionStyle style)
            : this(firstLine, secondLine, offset, Vector3.UnitZ, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="firstLine">First <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="secondLine">Second <see cref="Line">line</see> that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Line firstLine, Line secondLine, double offset, Vector3 normal, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            if (firstLine == null)
                throw new ArgumentNullException(nameof(firstLine));
            if (secondLine == null)
                throw new ArgumentNullException(nameof(secondLine));

            if (Vector3.AreParallel(firstLine.Direction, secondLine.Direction))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            IList<Vector3> ocsPoints =
                MathHelper.Transform(
                    new[]
                    {
                        firstLine.StartPoint,
                        firstLine.EndPoint,
                        secondLine.StartPoint,
                        secondLine.EndPoint
                    },
                    normal, CoordinateSystem.World, CoordinateSystem.Object);

            this.startFirstLine = new Vector2(ocsPoints[0].X, ocsPoints[0].Y);
            this.endFirstLine = new Vector2(ocsPoints[1].X, ocsPoints[1].Y);
            this.startSecondLine = new Vector2(ocsPoints[2].X, ocsPoints[2].Y);
            this.endSecondLine = new Vector2(ocsPoints[3].X, ocsPoints[3].Y);
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.Style = style;
            this.Normal = normal;
            this.Elevation = ocsPoints[0].Z;
            this.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset)
            : this(startFirstLine, endFirstLine, startSecondLine, endSecondLine, offset, DimensionStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Angular2LineDimension</c> class.
        /// </summary>
        /// <param name="startFirstLine">Start <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="endFirstLine">End <see cref="Vector2">point</see> of the first line that defines the angle to measure.</param>
        /// <param name="startSecondLine">Start <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="endSecondLine">End <see cref="Vector2">point</see> of the second line that defines the angle to measure.</param>
        /// <param name="offset">Distance between the center point and the dimension line.</param>
        /// <param name="style">The <see cref="DimensionStyle">style</see> to use with the dimension.</param>
        public Angular2LineDimension(Vector2 startFirstLine, Vector2 endFirstLine, Vector2 startSecondLine, Vector2 endSecondLine, double offset, DimensionStyle style)
            : base(DimensionType.Angular)
        {
            Vector2 dir1 = endFirstLine - startFirstLine;
            Vector2 dir2 = endSecondLine - startSecondLine;
            if (Vector2.AreParallel(dir1, dir2))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            this.startFirstLine = startFirstLine;
            this.endFirstLine = endFirstLine;
            this.startSecondLine = startSecondLine;
            this.endSecondLine = endSecondLine;

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "The offset value must be equal or greater than zero.");
            this.offset = offset;

            if (style == null)
                throw new ArgumentNullException(nameof(style));
            this.Style = style;
            this.Update();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the center <see cref="Vector2">point</see> of the measured arc in local coordinates.
        /// </summary>
        public Vector2 CenterPoint
        {
            get
            {
                return MathHelper.FindIntersection(
                    this.startFirstLine, this.endFirstLine - this.startFirstLine,
                    this.startSecondLine, this.endSecondLine - this.startSecondLine);
            }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 StartFirstLine
        {
            get { return this.startFirstLine; }
            set { this.startFirstLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the first line that defines the angle to measure in local coordinates.
        /// </summary>
        public Vector2 EndFirstLine
        {
            get { return this.endFirstLine; }
            set { this.endFirstLine = value; }
        }

        /// <summary>
        /// Start <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 StartSecondLine
        {
            get { return this.startSecondLine; }
            set { this.startSecondLine = value; }
        }

        /// <summary>
        /// End <see cref="Vector2">point</see> of the second line that defines the angle to measure in OCS (object coordinate system).
        /// </summary>
        public Vector2 EndSecondLine
        {
            get { return this.endSecondLine; }
            set { this.endSecondLine = value; }
        }

        /// <summary>
        /// Gets the location of the dimension line arc.
        /// </summary>
        public Vector2 ArcDefinitionPoint
        {
            get { return this.arcDefinitionPoint; }
            internal set { this.arcDefinitionPoint = value; }
        }

        /// <summary>
        /// Gets or sets the distance between the center point and the dimension line.
        /// </summary>
        /// <remarks>
        /// Offset values cannot be negative and, even thought, zero values are allowed, they are not recommended.
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
        /// Actual measurement.
        /// </summary>
        /// <remarks>The dimension is always measured in the plane defined by the normal.</remarks>
        public override double Measurement
        {
            get
            {
                Vector2 dirRef1 = this.endFirstLine - this.startFirstLine;
                Vector2 dirRef2 = this.endSecondLine - this.startSecondLine;
                return Vector2.AngleBetween(dirRef1, dirRef2)*MathHelper.RadToDeg;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculates the dimension offset from a point along the dimension line.
        /// </summary>
        /// <param name="point">Point along the dimension line.</param>
        /// <remarks>
        /// The start and end points of the reference lines will be modified,
        /// the angle measurement is always made from the direction of the first line to the direction of the second line.
        /// </remarks>
        public void SetDimensionLinePosition(Vector2 point)
        {
            this.SetDimensionLinePosition(point, true);
        }

        #endregion

        #region private methods

        private void SetDimensionLinePosition(Vector2 point, bool updateRefs)
        {
            Vector2 center = this.CenterPoint;

            if (Vector2.IsNaN(center))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            if (updateRefs)
            {
                double cross = Vector2.CrossProduct(this.EndFirstLine - this.StartFirstLine, this.EndSecondLine - this.StartSecondLine);
                if (cross < 0)
                {
                    Vector2 temp1 = this.startFirstLine;
                    Vector2 temp2 = this.endFirstLine;

                    this.startFirstLine = this.startSecondLine;
                    this.endFirstLine = this.endSecondLine;

                    this.startSecondLine = temp1;
                    this.endSecondLine = temp2;
                }

                Vector2 ref1Start = this.StartFirstLine;
                Vector2 ref1End = this.EndFirstLine;
                Vector2 ref2Start = this.StartSecondLine;
                Vector2 ref2End = this.EndSecondLine;
                Vector2 dirRef1 = ref1End - ref1Start;
                Vector2 dirRef2 = ref2End - ref2Start;

                Vector2 dirOffset = point - center;
                double crossStart = Vector2.CrossProduct(dirRef1, dirOffset);
                double crossEnd = Vector2.CrossProduct(dirRef2, dirOffset);

                if (crossStart >= 0 && crossEnd >= 0)
                {
                    this.StartFirstLine = ref2Start;
                    this.EndFirstLine = ref2End;
                    this.StartSecondLine = ref1End;
                    this.EndSecondLine = ref1Start;
                }
                else if (crossStart < 0 && crossEnd >= 0)
                {
                    this.StartFirstLine = ref1End;
                    this.EndFirstLine = ref1Start;
                    this.StartSecondLine = ref2End;
                    this.EndSecondLine = ref2Start;
                }
                else if (crossStart < 0 && crossEnd < 0)
                {
                    this.StartFirstLine = ref2End;
                    this.EndFirstLine = ref2Start;
                    this.StartSecondLine = ref1Start;
                    this.EndSecondLine = ref1End;
                }
            }

            this.offset = Vector2.Distance(center, point);
            this.defPoint = this.endSecondLine;

            double measure = this.Measurement * MathHelper.DegToRad;
            double startAngle = Vector2.Angle(center, this.endFirstLine);
            double midRot = startAngle + measure * 0.5;
            Vector2 midDim = Vector2.Polar(center, this.offset, midRot);
            this.arcDefinitionPoint = midDim;

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
                this.textRefPoint = midDim + gap * Vector2.Normalize(midDim - center);
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

            double measure = this.Measurement * MathHelper.DegToRad ;
            Vector2 center = this.CenterPoint;

            if (Vector2.IsNaN(center))
                throw new ArgumentException("The two lines that define the dimension are parallel.");

            double startAngle = Vector2.Angle(center, this.endFirstLine);
            double midRot = startAngle + measure * 0.5;
            Vector2 midDim = Vector2.Polar(center, this.offset, midRot);

            this.defPoint = this.endSecondLine;
            this.arcDefinitionPoint = midDim;


            if (this.TextPositionManuallySet)
            {
                DimensionStyleFitTextMove moveText = this.Style.FitTextMove;
                if (this.StyleOverrides.TryGetValue(DimensionStyleOverrideType.FitTextMove, out styleOverride))
                {
                    moveText = (DimensionStyleFitTextMove)styleOverride.Value;
                }

                if (moveText == DimensionStyleFitTextMove.BesideDimLine)
                {
                    this.SetDimensionLinePosition(this.textRefPoint, false);
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
                this.textRefPoint = midDim + gap * Vector2.Normalize(midDim-center);
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
        /// Creates a new Angular2LineDimension that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Angular2LineDimension that is a copy of this instance.</returns>
        public override object Clone()
        {
            Angular2LineDimension entity = new Angular2LineDimension
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
                Elevation = this.Elevation,
                //Angular2LineDimension properties
                StartFirstLine = this.startFirstLine,
                EndFirstLine = this.endFirstLine,
                StartSecondLine = this.startSecondLine,
                EndSecondLine = this.endSecondLine,
                Offset = this.offset,
                arcDefinitionPoint = this.arcDefinitionPoint
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