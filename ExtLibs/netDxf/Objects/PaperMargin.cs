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

namespace netDxf.Objects
{
    /// <summary>
    /// Represents the unprintable margins of a paper. 
    /// </summary>
    public struct PaperMargin
    {
        #region private fields

        private double left;
        private double bottom;
        private double right;
        private double top;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of <c>PaperMargin</c>.
        /// </summary>
        /// <param name="left">Margin on left side of paper.</param>
        /// <param name="bottom">Margin on bottom side of paper.</param>
        /// <param name="right">Margin on right side of paper.</param>
        /// <param name="top">Margin on top side of paper.</param>
        public PaperMargin(double left, double bottom, double right, double top)
        {
            this.left = left;
            this.bottom = bottom;
            this.right = right;
            this.top = top;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on left side of paper.
        /// </summary>
        public double Left
        {
            get { return this.left; }
            set { this.left = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on bottom side of paper.
        /// </summary>
        public double Bottom
        {
            get { return this.bottom; }
            set { this.bottom = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on right side of paper.
        /// </summary>
        public double Right
        {
            get { return this.right; }
            set { this.right = value; }
        }

        /// <summary>
        /// Gets or set the size, in millimeters, of unprintable margin on top side of paper.
        /// </summary>
        public double Top
        {
            get { return this.top; }
            set { this.top = value; }
        }
        #endregion      
    }
}