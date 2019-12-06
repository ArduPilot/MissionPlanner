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
    /// Defines the method for calculating the tolerance.
    /// </summary>
    /// <remarks>
    /// The Basic method for displaying tolerances in dimensions is not available,
    /// use a negative number for the <c>TextOffet</c> of the dimension style. The result is exactly the same.
    /// </remarks>
    public enum DimensionStyleTolerancesDisplayMethod
    {
        /// <summary>
        /// Does not add a tolerance.
        /// </summary>
        None,

        /// <summary>
        /// Adds a plus/minus expression of tolerance in which a single value of variation is applied to the dimension measurement.
        /// </summary>
        Symmetrical,

        /// <summary>
        /// Adds a plus/minus tolerance expression. Different plus and minus values of variation are applied to the dimension measurement.
        /// </summary>
        Deviation,

        /// <summary>
        /// Creates a limit dimension. A maximum and a minimum value are displayed, one over the other.
        /// </summary>
        Limits,
    }
}