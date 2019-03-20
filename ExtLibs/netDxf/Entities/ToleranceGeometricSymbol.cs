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
    /// Defines the geometric characteristic symbols for location, orientation, form, profile, and runout.
    /// </summary>
    public enum ToleranceGeometricSymbol
    {
        /// <summary>
        /// No geometric symbol.
        /// </summary>
        None,

        /// <summary>
        /// Position, type location.
        /// </summary>
        Position,

        /// <summary>
        /// Concentricity or coaxiality, type location.
        /// </summary>
        Concentricity,

        /// <summary>
        /// Symmetry, type location.
        /// </summary>
        Symmetry,

        /// <summary>
        /// Parallelism, type orientation.
        /// </summary>
        Parallelism,

        /// <summary>
        /// Perpendicularity, type orientation.
        /// </summary>
        Perpendicularity,

        /// <summary>
        /// Angularity, type orientation.
        /// </summary>
        Angularity,

        /// <summary>
        /// Cylindricity, type form.
        /// </summary>
        Cylindricity,

        /// <summary>
        /// Flatness, type form.
        /// </summary>
        Flatness,

        /// <summary>
        /// Circularity or roundness, type form.
        /// </summary>
        Roundness,

        /// <summary>
        /// Straightness, type form.
        /// </summary>
        Straightness,

        /// <summary>
        /// Profile of a surface, type profile.
        /// </summary>
        ProfileSurface,

        /// <summary>
        /// Profile of a line, type profile.
        /// </summary>
        ProfileLine,

        /// <summary>
        /// Circular runout, type runout.
        /// </summary>
        CircularRunout,

        /// <summary>
        /// Total runout, type runout.
        /// </summary>
        TotalRunOut
    }
}