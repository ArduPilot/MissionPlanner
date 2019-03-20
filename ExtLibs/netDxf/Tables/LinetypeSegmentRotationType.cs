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
    /// Define the type of rotation specified by the rotation value of the text or shape linetype segment.
    /// </summary>
    public enum LinetypeSegmentRotationType
    {
        /// <summary>
        /// Specifies upright or easy-to-read text.
        /// </summary>
        /// <remarks>The dxf seems to store the rotation value only as relative or absolute, therefore upright and relative will be considered equivalent.</remarks>
        Upright,

        /// <summary>
        /// Specifies relative or tangential rotation with respect to the line.
        /// </summary>
        Relative,

        /// <summary>
        /// Specifies absolute rotation of the text with respect to the origin; that is, all text has the same rotation regardless of its position relative to the line.
        /// </summary>
        Absolute
    }
}