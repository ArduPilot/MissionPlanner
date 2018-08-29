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

using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a line <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Line :
        EntityObject
    {
        #region private fields

        private Vector3 start;
        private Vector3 end;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Line</c> class.
        /// </summary>
        public Line()
            : this(Vector3.Zero, Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Line</c> class.
        /// </summary>
        /// <param name="startPoint">Line <see cref="Vector2">start point.</see></param>
        /// <param name="endPoint">Line <see cref="Vector2">end point.</see></param>
        public Line(Vector2 startPoint, Vector2 endPoint)
            : this(new Vector3(startPoint.X, startPoint.Y, 0.0), new Vector3(endPoint.X, endPoint.Y, 0.0))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Line</c> class.
        /// </summary>
        /// <param name="startPoint">Line start <see cref="Vector3">point.</see></param>
        /// <param name="endPoint">Line end <see cref="Vector3">point.</see></param>
        public Line(Vector3 startPoint, Vector3 endPoint)
            : base(EntityType.Line, DxfObjectCode.Line)
        {
            this.start = startPoint;
            this.end = endPoint;
            this.thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the line <see cref="Vector3">start point</see>.
        /// </summary>
        public Vector3 StartPoint
        {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Gets or sets the line <see cref="Vector3">end point</see>.
        /// </summary>
        public Vector3 EndPoint
        {
            get { return this.end; }
            set { this.end = value; }
        }

        /// <summary>
        /// Gets the direction of the line.
        /// </summary>
        public Vector3 Direction
        {
            get { return this.end - this.start; }
        }

        /// <summary>
        /// Gets or sets the line thickness.
        /// </summary>
        public double Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Switch the line direction.
        /// </summary>
        public void Reverse()
        {
            Vector3 tmp = this.start;
            this.start = this.end;
            this.end = tmp;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Line that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Line that is a copy of this instance.</returns>
        public override object Clone()
        {
            Line entity = new Line
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
                //Line properties
                StartPoint = this.start,
                EndPoint = this.end,
                Thickness = this.thickness
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}