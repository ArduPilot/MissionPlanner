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
    /// AutoCAD units for inserting images.
    /// </summary>
    /// <remarks>This is what one AutoCAD unit is equal to for the purpose of inserting and scaling images with an associated resolution.</remarks>
    public enum ImageUnits
    {
        /// <summary>
        /// No units.
        /// </summary>
        Unitless = 0,

        /// <summary>
        /// Millimeters.
        /// </summary>
        Millimeters = 1,

        /// <summary>
        /// Centimeters.
        /// </summary>
        Centimeters = 2,

        /// <summary>
        /// Meters.
        /// </summary>
        Meters = 3,

        /// <summary>
        /// Kilometers.
        /// </summary>
        Kilometers = 4,

        /// <summary>
        /// Inches.
        /// </summary>
        Inches = 5,

        /// <summary>
        /// Feet.
        /// </summary>
        Feet = 6,

        /// <summary>
        /// Yards.
        /// </summary>
        Yards = 7,

        /// <summary>
        /// Miles.
        /// </summary>
        Miles = 8
    }
}