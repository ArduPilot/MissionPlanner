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

namespace netDxf.Tables
{
    /// <summary>
    /// Represents a simple linetype segment.
    /// </summary>
    public class LinetypeSimpleSegment :
        LinetypeSegment
    {
        #region private fields

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>LinetypeSìmpleSegment</c> class.
        /// </summary>
        public LinetypeSimpleSegment() : this(0.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>LinetypeSìmpleSegment</c> class.
        /// </summary>
        /// <param name="length">Dash or space length of the segment.</param>
        public LinetypeSimpleSegment(double length) : base (LinetypeSegmentType.Simple, length)
        {
        }

        #endregion

        #region public properties

        #endregion

        #region overrides

        /// <summary>
        /// Creates a new <c>LinetypeSìmpleSegment</c> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <c>LinetypeSìmpleSegment</c> that is a copy of this instance.</returns>
        public override object Clone()
        {
            return new LinetypeSimpleSegment(this.Length);
        }

        #endregion
    }
}