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
    /// Specifies the knot parameterization, computational methods that determines how the component curves between successive fit points within a spline are blended.
    /// </summary>
    public enum SplineKnotParameterization
    {
        /// <summary>
        /// Chord-Length method.
        /// </summary>
        /// <remarks>
        /// Spaces the knots connecting each component curve to be proportional to the distances between each associated pair of fit points.
        /// </remarks>
        FitChord = 32,

        /// <summary>
        /// Centripetal method.
        /// </summary>
        /// <remarks>
        /// Spaces the knots connecting each component curve to be proportional to the square root of the distance between each associated pair of fit points.
        /// This method usually produces "gentler" curves.
        /// </remarks>
        FitSqrtChord = 64,

        /// <summary>
        /// Equidistant method.
        /// </summary>
        /// <remarks>
        /// Spaces the knots of each component curve to be equal, regardless of the spacing of the fit points.
        /// This method often produces curves that overshoot the fit points.
        /// </remarks>
        FitUniform = 128,

        /// <summary>
        /// Only applicable when a spline has been converted from the control points  to the fit point creation method.
        /// </summary>
        FitCustom = 256
    }
}