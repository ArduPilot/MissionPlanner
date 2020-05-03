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
    /// Represents the line weight of a layer or an entity.
    /// </summary>
    /// <remarks>The enum numeric value correspond to 1/100 mm.</remarks>
    public enum Lineweight
    {
        /// <summary>
        /// Default lineweight.
        /// </summary>
        Default = -3,

        /// <summary>
        /// Lineweight defined by block.
        /// </summary>
        ByBlock = -2,

        /// <summary>
        /// Lineweight defined by layer.
        /// </summary>
        ByLayer = -1,

        /// <summary>
        /// Lineweight value 0.00 mm (hairline).
        /// </summary>
        W0 = 0,

        /// <summary>
        /// Lineweight value 0.05 mm.
        /// </summary>
        W5 = 5,

        /// <summary>
        /// Lineweight value 0.09 mm.
        /// </summary>
        W9 = 9,

        /// <summary>
        /// Lineweight value 0.13 mm.
        /// </summary>
        W13 = 13,

        /// <summary>
        /// Lineweight value 0.15 mm.
        /// </summary>
        W15 = 15,

        /// <summary>
        /// Lineweight value 0.18 mm.
        /// </summary>
        W18 = 18,

        /// <summary>
        /// Lineweight value 0.20 mm.
        /// </summary>
        W20 = 20,

        /// <summary>
        /// Lineweight value 0.25 mm.
        /// </summary>
        W25 = 25,

        /// <summary>
        /// Lineweight value 0.30 mm.
        /// </summary>
        W30 = 30,

        /// <summary>
        /// Lineweight value 0.35 mm.
        /// </summary>
        W35 = 35,

        /// <summary>
        /// Lineweight value 0.40 mm.
        /// </summary>
        W40 = 40,

        /// <summary>
        /// Lineweight value 0.50 mm.
        /// </summary>
        W50 = 50,

        /// <summary>
        /// Lineweight value 0.53 mm.
        /// </summary>
        W53 = 53,

        /// <summary>
        /// Lineweight value 0.60 mm.
        /// </summary>
        W60 = 60,

        /// <summary>
        /// Lineweight value 0.70 mm.
        /// </summary>
        W70 = 70,

        /// <summary>
        /// Lineweight value 0.80 mm.
        /// </summary>
        W80 = 80,

        /// <summary>
        /// Lineweight value 0.90 mm.
        /// </summary>
        W90 = 90,

        /// <summary>
        /// Lineweight value 1.00 mm.
        /// </summary>
        W100 = 100,

        /// <summary>
        /// Lineweight value 1.06 mm.
        /// </summary>
        W106 = 106,

        /// <summary>
        /// Lineweight value 1.20 mm.
        /// </summary>
        W120 = 120,

        /// <summary>
        /// Lineweight value 1.40 mm.
        /// </summary>
        W140 = 140,

        /// <summary>
        /// Lineweight value 1.58 mm.
        /// </summary>
        W158 = 158,

        /// <summary>
        /// Lineweight value 2.00 mm.
        /// </summary>
        W200 = 200,

        /// <summary>
        /// Lineweight value 2.11 mm.
        /// </summary>
        W211 = 211
    }
}