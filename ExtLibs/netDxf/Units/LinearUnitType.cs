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

namespace netDxf.Units
{
    /// <summary>
    /// Linear units format for creating objects.
    /// </summary>
    public enum LinearUnitType
    {
        /// <summary>
        /// Scientific.
        /// </summary>
        Scientific = 1,

        /// <summary>
        /// Decimal.
        /// </summary>
        Decimal = 2,

        /// <summary>
        /// Engineering.
        /// </summary>
        Engineering = 3,

        /// <summary>
        /// Architectural.
        /// </summary>
        Architectural = 4,

        /// <summary>
        /// Fractional.
        /// </summary>
        Fractional = 5,

        /// <summary>
        /// Microsoft Windows Desktop (decimal format using Control Panel settings for decimal separator and number grouping symbols).
        /// </summary>
        WindowsDesktop = 6
    }
}