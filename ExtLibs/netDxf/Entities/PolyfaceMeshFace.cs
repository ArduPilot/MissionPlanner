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
using System.Collections.Generic;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a polyface mesh face. 
    /// </summary>
    /// <remarks>
    /// The way the vertex indexes for a polyface mesh are defined follows the dxf documentation.
    /// The values of the vertex indexes specify one of the previously defined vertexes by the index in the list plus one.
    /// If the index is negative, the edge that begins with that vertex is invisible.
    /// For example if the vertex index in the list is 0 the vertex index for the face will be 1, and
    /// if the edge between the vertexes 0 and 1 is hidden the vertex index for the face will be -1.<br/>
    /// The maximum number of vertexes per face is 4.
    /// </remarks>
    public class PolyfaceMeshFace :
        DxfObject,
        ICloneable
    {
        #region private fields

        private readonly VertexTypeFlags flags;
        private readonly List<short> vertexIndexes;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>PolyfaceMeshFace</c> class.
        /// </summary>
        /// <remarks>
        /// By default the face is made up of four vertexes.
        /// </remarks>
        public PolyfaceMeshFace()
            : this(new short[4])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>PolyfaceMeshFace</c> class.
        /// </summary>
        /// <param name="vertexIndexes">Array of indexes to the vertex list of a polyface mesh that makes up the face.</param>
        public PolyfaceMeshFace(IEnumerable<short> vertexIndexes)
            : base(DxfObjectCode.Vertex)
        {
            if (vertexIndexes == null)
                throw new ArgumentNullException(nameof(vertexIndexes));
            this.flags = VertexTypeFlags.PolyfaceMeshVertex;
            this.vertexIndexes = new List<short>(vertexIndexes);
            if (this.vertexIndexes.Count > 4)
                throw new ArgumentOutOfRangeException(nameof(vertexIndexes), this.vertexIndexes.Count, "The maximum number of vertexes per face is 4");
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the list of indexes to the vertex list of a polyface mesh that makes up the face.
        /// </summary>
        public List<short> VertexIndexes
        {
            get { return this.vertexIndexes; }
        }

        #endregion

        #region internal properties

        /// <summary>
        /// Gets the vertex type.
        /// </summary>
        internal VertexTypeFlags Flags
        {
            get { return this.flags; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return "PolyfaceMeshFace";
        }

        /// <summary>
        /// Creates a new PolyfaceMeshFace that is a copy of the current instance.
        /// </summary>
        /// <returns>A new PolyfaceMeshFace that is a copy of this instance.</returns>
        public object Clone()
        {
            return new PolyfaceMeshFace(this.vertexIndexes);
        }

        #endregion
    }
}