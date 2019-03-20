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

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a <see cref="Polyline">polyline</see> vertex.
    /// </summary>
    public class PolylineVertex :
        DxfObject, ICloneable
    {
        #region private fields

        private VertexTypeFlags flags;
        private Vector3 position;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>PolylineVertex</c> class.
        /// </summary>
        public PolylineVertex()
            : this(Vector3.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>PolylineVertex</c> class.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public PolylineVertex(double x, double y, double z)
            : this(new Vector3(x, y, z))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>PolylineVertex</c> class.
        /// </summary>
        /// <param name="position">Polyline <see cref="Vector3">vertex</see> coordinates.</param>
        public PolylineVertex(Vector3 position)
            : base(DxfObjectCode.Vertex)
        {
            this.flags = VertexTypeFlags.Polyline3dVertex;
            this.position = position;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the vertex <see cref="Vector3">position</see>.
        /// </summary>
        public Vector3 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        /// <summary>
        /// Gets the vertex type.
        /// </summary>
        internal VertexTypeFlags Flags
        {
            get { return this.flags; }
            set { this.flags = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}: ({1})", "PolylineVertex", this.position);
        }

        /// <summary>
        /// Creates a new PolylineVertex that is a copy of the current instance.
        /// </summary>
        /// <returns>A new PolylineVertex that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PolylineVertex(this.position);
        }

        #endregion
    }
}