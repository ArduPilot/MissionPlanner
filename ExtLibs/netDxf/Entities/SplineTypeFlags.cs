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

using System;

namespace netDxf.Entities
{
    /// <summary>
    /// Defines the spline type.
    /// </summary>
    /// <remarks>Bit flag.</remarks>
    [Flags]
    internal enum SplinetypeFlags
    {
        /// <summary>
        /// Default (open 3d spline).
        /// </summary>
        None = 0,

        /// <summary>
        /// Closed spline.
        /// </summary>
        Closed = 1,

        /// <summary>
        /// Periodic spline.
        /// </summary>
        Periodic = 2,

        /// <summary>
        /// Rational spline.
        /// </summary>
        Rational = 4,

        /// <summary>
        /// Planar.
        /// </summary>
        Planar = 8,

        /// <summary>
        /// Linear (planar bit is also set).
        /// </summary>
        Linear = 16,
        // in AutoCAD 2012 the flags can be greater than 70 despite the information that shows the dxf documentation these values are just a guess.
        FitChord = 32,
        FitSqrtChord = 64,
        FitUniform = 128,
        FitCustom = 256,
        Unknown2 = 512,

        /// <summary>
        /// Used by splines created by fit points.
        /// </summary>
        FitPointCreationMethod = 1024,

        /// <summary>
        /// Used for closed periodic splines.
        /// </summary>
        ClosedPeriodicSpline = 2048
    }
}