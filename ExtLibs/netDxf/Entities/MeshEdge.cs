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
using System.Threading;

namespace netDxf.Entities
{
    /// <summary>
    /// Represents an edge of a <see cref="EntityObject">mesh</see> entity.
    /// </summary>
    public class MeshEdge :
        ICloneable
    {
        #region private fields

        private int startVertexIndex;
        private int endVertexIndex;
        private double crease;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <c>MeshEdge</c> class.
        /// </summary>
        /// <param name="startVertexIndex">The edge start vertex index.</param>
        /// <param name="endVertexIndex">The edge end vertex index.</param>
        public MeshEdge(int startVertexIndex, int endVertexIndex)
            : this(startVertexIndex, endVertexIndex, 0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MeshEdge</c> class.
        /// </summary>
        /// <param name="startVertexIndex">The edge start vertex index.</param>
        /// <param name="endVertexIndex">The edge end vertex index.</param>
        /// <param name="crease">The highest smoothing level at which the crease is retained  (default: 0.0).</param>
        public MeshEdge(int startVertexIndex, int endVertexIndex, double crease)
        {
            if (startVertexIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startVertexIndex), startVertexIndex, "The vertex index must be positive.");
            this.startVertexIndex = startVertexIndex;

            if (endVertexIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(endVertexIndex), endVertexIndex, "The vertex index must be positive.");
            this.endVertexIndex = endVertexIndex;
            this.crease = crease < 0.0 ? -1.0 : crease;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the edge start vertex index.
        /// </summary>
        /// <remarks>
        /// This value must be positive represent the position of the vertex in the mesh vertex list.
        /// </remarks>
        public int StartVertexIndex
        {
            get { return this.startVertexIndex; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex index must be must be equals or greater than zero.");
                this.startVertexIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the edge end vertex index.
        /// </summary>
        public int EndVertexIndex
        {
            get { return this.endVertexIndex; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The vertex index must be must be equals or greater than zero.");
                this.endVertexIndex = value;
            }
        }

        /// <summary>
        /// Get or set the highest smoothing level at which the crease is retained. If the smoothing level exceeds this value, the crease is also smoothed.
        /// </summary>
        /// <remarks>
        /// Enter a value of 0 to remove an existing crease (no edge sharpening).<br/>
        /// Enter a value of -1 (any negative number will be reset to -1) to specify that the crease is always retained, even if the object or sub-object is smoothed or refined.
        /// </remarks>
        public double Crease
        {
            get { return this.crease; }
            set { this.crease = value < 0 ? -1 : value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the mesh edge.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0}: ({1}{4} {2}) crease={3}", "SplineVertex", this.startVertexIndex, this.endVertexIndex, this.crease, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Obtains a string that represents the mesh edge.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0}: ({1}{4} {2}) crease={3}", "SplineVertex", this.startVertexIndex.ToString(provider), this.endVertexIndex.ToString(provider), this.crease.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Creates a new MeshEdge that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MeshEdge that is a copy of this instance.</returns>
        public object Clone()
        {
            return new MeshEdge(this.startVertexIndex, this.endVertexIndex, this.crease);
        }

        #endregion
    }
}