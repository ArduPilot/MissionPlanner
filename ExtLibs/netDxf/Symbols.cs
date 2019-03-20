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

namespace netDxf
{
    /// <summary>
    /// Symbols for dxf text strings.
    /// </summary>
    /// <remarks>
    /// These special strings translates to symbols in AutoCad. They are obsolete since Unicode characters are supported.
    /// </remarks>
    public static class Symbols
    {
        /// <summary>
        /// Text string that shows as a diameter 'Ø' character.
        /// </summary>
        public const string Diameter = "%%c";

        /// <summary>
        /// Text string that shows as a degree '°' character.
        /// </summary>
        public const string Degree = "%%d";

        /// <summary>
        /// Text string that shows as a plus-minus '±' character.
        /// </summary>
        public const string PlusMinus = "%%p";
    }
}