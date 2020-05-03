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
    /// Defines the multiline text attachment point.
    /// </summary>
    public enum MTextAttachmentPoint
    {
        /// <summary>
        /// Top left.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        /// Top center.
        /// </summary>
        TopCenter = 2,

        /// <summary>
        /// Top right.
        /// </summary>
        TopRight = 3,

        /// <summary>
        /// Middle left.
        /// </summary>
        MiddleLeft = 4,

        /// <summary>
        /// Middle center.
        /// </summary>
        MiddleCenter = 5,

        /// <summary>
        /// Middle right.
        /// </summary>
        MiddleRight = 6,

        /// <summary>
        /// Bottom left.
        /// </summary>
        BottomLeft = 7,

        /// <summary>
        /// Bottom center.
        /// </summary>
        BottomCenter = 8,

        /// <summary>
        /// Bottom right.
        /// </summary>
        BottomRight = 9,
    }
}