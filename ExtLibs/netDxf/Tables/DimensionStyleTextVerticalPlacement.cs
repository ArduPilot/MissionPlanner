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
    /// Controls the placement of dimension text.
    /// </summary>
    public enum DimensionStyleTextVerticalPlacement
    {
        /// <summary>
        /// Centers the dimension text between the two parts of the dimension line.
        /// </summary>
        Centered = 0,

        /// <summary>
        /// Places the dimension text above the dimension line.
        /// </summary>
        Above = 1,

        /// <summary>
        /// Places the dimension text on the side of the dimension line farthest away from the first defining point.
        /// </summary>
        Outside = 2,

        /// <summary>
        /// Places the dimension text to conform to a Japanese Industrial Standards (JIS) representation.
        /// </summary>
        JIS = 3,

        /// <summary>
        /// Places the dimension text under the dimension line.
        /// </summary>
        Below = 4
    }
}