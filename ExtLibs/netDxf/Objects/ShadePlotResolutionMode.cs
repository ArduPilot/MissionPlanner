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
    /// Defines the shade plot resolution mode.
    /// </summary>
    public enum ShadePlotResolutionMode
    {
        /// <summary>
        /// Draft.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Preview.
        /// </summary>
        Preview = 1,

        /// <summary>
        /// Normal.
        /// </summary>
        Normal = 2,

        /// <summary>
        /// Presentation.
        /// </summary>
        Presentation = 3,

        /// <summary>
        /// Maximum
        /// </summary>
        Maximum = 4,

        /// <summary>
        /// Custom as specified by the shade plot DPI.
        /// </summary>
        Custom = 5
    }
}