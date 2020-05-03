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
    public class LinetypeSegmentChangeEventArgs :
        EventArgs
    {
        #region private fields

        private readonly LinetypeSegment item;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of <c>LinetypeSegmentChangeEventArgs</c>.
        /// </summary>
        /// <param name="item">The item that is being added or removed from the line type segment list.</param>
        public LinetypeSegmentChangeEventArgs(LinetypeSegment item)
        {
            this.item = item;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the item that is being added or removed from the line type segment list.
        /// </summary>
        public LinetypeSegment Item
        {
            get { return this.item; }
        }

        #endregion
    }
}