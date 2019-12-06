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
    /// Controls the vertical placement of dimension text in relation to the dimension line.
    /// </summary>
    public enum DimensionStyleTextHorizontalPlacement
    {
        /// <summary>
        /// Centers the dimension text along the dimension line between the extension lines.
        /// </summary>
        Centered = 0,

        /// <summary>
        /// Left-justifies the text with the first extension line along the dimension line.
        /// </summary>
        AtExtLines1 = 1,

        /// <summary>
        /// Right-justifies the text with the second extension line along the dimension line.
        /// </summary>
        AtExtLine2 = 2,

        /// <summary>
        /// Positions the text over or along the first extension line.
        /// </summary>
        OverExtLine1 = 3,

        /// <summary>
        /// Positions the text over or along the second extension line.
        /// </summary>
        OverExtLine2 = 4
    }
}