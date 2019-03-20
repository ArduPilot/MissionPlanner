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

namespace netDxf.Objects
{
    /// <summary>
    /// Flags (bit-coded).
    /// </summary>
    [Flags]
    public enum MLineStyleFlags
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// Fill on.
        /// </summary>
        FillOn = 1,

        /// <summary>
        /// Display miters.
        /// </summary>
        DisplayMiters = 2,

        /// <summary>
        /// Start square end (line) cap.
        /// </summary>
        StartSquareEndCap = 16,

        /// <summary>
        /// Start inner arcs cap.
        /// </summary>
        StartInnerArcsCap = 32,

        /// <summary>
        /// Start round (outer arcs) cap.
        /// </summary>
        StartRoundCap = 64,

        /// <summary>
        /// End square (line) cap.
        /// </summary>
        EndSquareCap = 256,

        /// <summary>
        /// End inner arcs cap.
        /// </summary>
        EndInnerArcsCap = 512,

        /// <summary>
        /// End round (outer arcs) cap.
        /// </summary>
        EndRoundCap = 1024
    }
}