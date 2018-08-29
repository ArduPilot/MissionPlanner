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

namespace netDxf.Header
{
    /// <summary>
    /// Defines the shape of the point entities.
    /// </summary>
    public enum PointShape
    {
        /// <summary>
        /// A dot.
        /// </summary>
        Dot = 0,

        /// <summary>
        /// No shape.
        /// </summary>
        Empty = 1,

        /// <summary>
        /// Plus sign.
        /// </summary>
        Plus = 2,

        /// <summary>
        /// Cross sign.
        /// </summary>
        Cross = 3,

        /// <summary>
        /// A line going upwards.
        /// </summary>
        Line = 4,

        /// <summary>
        /// A circle and a dot.
        /// </summary>
        CircleDot = 32,

        /// <summary>
        /// Only a circle shape.
        /// </summary>
        CircleEmpty = 33,

        /// <summary>
        /// A circle and a plus sign.
        /// </summary>
        CirclePlus = 34,

        /// <summary>
        /// A circle and a cross sign.
        /// </summary>
        CircleCross = 35,

        /// <summary>
        /// A circle and a line.
        /// </summary>
        CircleLine = 36,

        /// <summary>
        /// A square and a dot.
        /// </summary>
        SquareDot = 64,

        /// <summary>
        /// Only a square shape.
        /// </summary>
        SquareEmpty = 65,

        /// <summary>
        /// A square and a plus sign.
        /// </summary>
        SquarePlus = 66,

        /// <summary>
        /// A square and a cross sign.
        /// </summary>
        SquareCross = 67,

        /// <summary>
        /// A square and a line.
        /// </summary>
        SquareLine = 68,

        /// <summary>
        /// A circle, a square, and a dot.
        /// </summary>
        CircleSquareDot = 96,

        /// <summary>
        /// A circle and a square.
        /// </summary>
        CircleSquareEmpty = 97,

        /// <summary>
        /// A circle, a square, and a plus sign.
        /// </summary>
        CircleSquarePlus = 98,

        /// <summary>
        /// A circle, a square, and a cross sign.
        /// </summary>
        CircleSquareCross = 99,

        /// <summary>
        /// A circle, a square, and a line.
        /// </summary>
        CircleSquareLine = 100
    }
}