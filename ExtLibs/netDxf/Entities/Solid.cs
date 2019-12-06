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

using netDxf.Tables;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a solid <see cref="EntityObject">entity</see>.
    /// </summary>
    public class Solid :
        EntityObject
    {
        #region private fields

        private Vector2 firstVertex;
        private Vector2 secondVertex;
        private Vector2 thirdVertex;
        private Vector2 fourthVertex;
        private double elevation;
        private double thickness;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        public Solid()
            : this(Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex)
            : this(new Vector2(firstVertex.X, firstVertex.Y),
                new Vector2(secondVertex.X, secondVertex.Y),
                new Vector2(thirdVertex.X, thirdVertex.Y),
                new Vector2(thirdVertex.X, thirdVertex.Y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>Solid</c> class.
        /// </summary>
        /// <param name="firstVertex">Solid <see cref="Vector2">first vertex</see> in OCS (object coordinate system).</param>
        /// <param name="secondVertex">Solid <see cref="Vector2">second vertex</see> in OCS (object coordinate system).</param>
        /// <param name="thirdVertex">Solid <see cref="Vector2">third vertex</see> in OCS (object coordinate system).</param>
        /// <param name="fourthVertex">Solid <see cref="Vector2">fourth vertex</see> in OCS (object coordinate system).</param>
        public Solid(Vector2 firstVertex, Vector2 secondVertex, Vector2 thirdVertex, Vector2 fourthVertex)
            : base(EntityType.Solid, DxfObjectCode.Solid)
        {
            this.firstVertex = firstVertex;
            this.secondVertex = secondVertex;
            this.thirdVertex = thirdVertex;
            this.fourthVertex = fourthVertex;
            this.elevation = 0.0;
            this.thickness = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the first solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FirstVertex
        {
            get { return this.firstVertex; }
            set { this.firstVertex = value; }
        }

        /// <summary>
        /// Gets or sets the second solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 SecondVertex
        {
            get { return this.secondVertex; }
            set { this.secondVertex = value; }
        }

        /// <summary>
        /// Gets or sets the third solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 ThirdVertex
        {
            get { return this.thirdVertex; }
            set { this.thirdVertex = value; }
        }

        /// <summary>
        /// Gets or sets the fourth solid <see cref="Vector2">vertex in OCS (object coordinate system).</see>.
        /// </summary>
        public Vector2 FourthVertex
        {
            get { return this.fourthVertex; }
            set { this.fourthVertex = value; }
        }

        /// <summary>
        /// Gets or sets the solid elevation.
        /// </summary>
        /// <remarks>This is the distance from the origin to the plane of the solid.</remarks>
        public double Elevation
        {
            get { return this.elevation; }
            set { this.elevation = value; }
        }

        /// <summary>
        /// Gets or sets the thickness of the solid.
        /// </summary>
        public double Thickness
        {
            get { return this.thickness; }
            set { this.thickness = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new Solid that is a copy of the current instance.
        /// </summary>
        /// <returns>A new Solid that is a copy of this instance.</returns>
        public override object Clone()
        {
            Solid entity = new Solid
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
                //Solid properties
                FirstVertex = this.firstVertex,
                SecondVertex = this.secondVertex,
                ThirdVertex = this.thirdVertex,
                FourthVertex = this.fourthVertex,
                Thickness = this.thickness
            };

            foreach (XData data in this.XData.Values)
                entity.XData.Add((XData) data.Clone());

            return entity;
        }

        #endregion
    }
}