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

namespace netDxf.Entities
{
    /// <summary>
    /// Dimension type.
    /// </summary>
    [Flags]
    internal enum DimensionTypeFlags
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
        Ordinate = 6,

        /// <summary>
        /// Indicates that the block reference (group code 2) is referenced by this dimension only.
        /// </summary>
        BlockReference = 32,

        /// <summary>
        /// Ordinate type. This is a bit value (bit 7) used only with integer value 6. If set, ordinate is X-type; if not set, ordinate is Y-type.
        /// </summary>
        OrdinateType = 64,

        /// <summary>
        /// This is a bit value (bit 8) added to the other group 70 values if the dimension text has been positioned at a user-defined location rather than at the default location.
        /// </summary>
        UserTextPosition = 128
    }
}