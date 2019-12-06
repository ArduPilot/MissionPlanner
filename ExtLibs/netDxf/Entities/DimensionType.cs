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

namespace netDxf.Entities
{
    /// <summary>
    /// Dimension type.
    /// </summary>
    public enum DimensionType
    {
        /// <summary>
        /// Rotated, horizontal, or vertical.
        /// </summary>
        Linear = 0,

        /// <summary>
        /// Aligned.
        /// </summary>
        Aligned = 1,

        /// <summary>
        /// Angular 2 lines.
        /// </summary>
        Angular = 2,

        /// <summary>
        /// Diameter.
        /// </summary>
        Diameter = 3,

        /// <summary>
        /// Radius.
        /// </summary>
        Radius = 4,

        /// <summary>
        /// Angular 3 points.
        /// </summary>
        Angular3Point = 5,

        /// <summary>
        /// Ordinate.
        /// </summary>
        Ordinate = 6
    }
}