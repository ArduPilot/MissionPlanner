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
    /// Helper methods for unit conversion.
    /// </summary>
    public static class UnitHelper
    {
        #region constants

        private static readonly double[,] UnitFactors =
        {
            {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
            },
            {
                1, 1, 0.0833333333333333, 1.57828282828283E-05, 25.4, 2.54, 0.0254, 2.54E-05, 1000000, 1000, 0.0277777777777778, 254000000, 25400000, 25400, 0.254, 0.00254, 0.000254, 2.54E-11, 1.697885129158E-13, 8.231579395684E-19
            },
            {
                1, 12, 1, 0.000189393939393939, 304.8, 30.48, 0.3048, 0.0003048, 12000000, 12000, 0.333333333333333, 3048000000, 304800000, 304800, 3.048, 0.03048, 0.003048, 3.048E-10, 2.0374621549896E-12, 9.8778952748208E-18
            },
            {
                1, 63360, 5280, 1, 1609344, 160934.4, 1609.344, 1.609344, 63360000000, 63360000, 1760, 16093440000000, 1609344000000, 1609344000, 16093.44, 160.9344, 16.09344, 1.609344E-06, 1.07578001783451E-08, 5.21552870510538E-14
            },
            {
                1, 0.0393700787401575, 0.00328083989501312, 6.21371192237334E-07, 1, 0.1, 0.001, 1E-06, 39370.0787401575, 39.3700787401575, 0.00109361329833771, 10000000, 1000000, 1000, 0.01, 0.0001, 1E-05, 1E-12, 6.68458712266929E-15, 3.24077928963937E-20
            },
            {
                1, 0.393700787401575, 0.0328083989501312, 6.21371192237334E-06, 10, 1, 0.01, 1E-05, 393700.787401575, 393.700787401575, 0.0109361329833771, 100000000, 10000000, 10000, 0.1, 0.001, 0.0001, 1E-11, 6.68458712266929E-14, 3.24077928963937E-19
            },
            {
                1, 39.3700787401575, 3.28083989501312, 0.000621371192237334, 1000, 100, 1, 0.001, 39370078.7401575, 39370.0787401575, 1.09361329833771, 10000000000, 1000000000, 1000000, 10, 0.1, 0.01, 1E-09, 6.68458712266929E-12, 3.24077928963937E-17
            },
            {
                1, 39370.0787401575, 3280.83989501312, 0.621371192237334, 1000000, 100000, 1000, 1, 39370078740.1575, 39370078.7401575, 1093.61329833771, 10000000000000, 1000000000000, 1000000000, 10000, 100, 10, 1E-06, 6.68458712266929E-09, 3.24077928963937E-14
            },
            {
                1, 1E-06, 8.33333333333333E-08, 1.57828282828283E-11, 2.54E-05, 2.54E-06, 2.54E-08, 2.54E-11, 1, 0.001, 2.77777777777778E-08, 254, 25.4, 0.0254, 2.54E-07, 2.54E-09, 2.54E-10, 2.54E-17, 1.697885129158E-19, 8.231579395684E-25
            },
            {
                1, 0.001, 8.33333333333333E-05, 1.57828282828283E-08, 0.0254, 0.00254, 2.54E-05, 2.54E-08, 1000, 1, 2.77777777777778E-05, 254000, 25400, 25.4, 0.000254, 2.54E-06, 2.54E-07, 2.54E-14, 1.697885129158E-16, 8.231579395684E-22
            },
            {
                1, 36, 3, 0.000568181818181818, 914.4, 91.44, 0.9144, 0.0009144, 36000000, 36000, 1, 9144000000, 914400000, 914400, 9.144, 0.09144, 0.009144, 9.144E-10, 6.1123864649688E-12, 2.96336858244624E-17
            },
            {
                1, 3.93700787401575E-09, 3.28083989501312E-10, 6.21371192237334E-14, 1E-07, 1E-08, 1E-10, 1E-13, 0.00393700787401575, 3.93700787401575E-06, 1.09361329833771E-10, 1, 0.1, 0.0001, 1E-09, 1E-11, 1E-12, 1E-19, 6.68458712266929E-22, 3.24077928963937E-27
            },
            {
                1, 3.93700787401575E-08, 3.28083989501312E-09, 6.21371192237334E-13, 1E-06, 1E-07, 1E-09, 1E-12, 0.0393700787401575, 3.93700787401575E-05, 1.09361329833771E-09, 10, 1, 0.001, 1E-08, 1E-10, 1E-11, 1E-18, 6.68458712266929E-21, 3.24077928963937E-26
            },
            {
                1, 3.93700787401575E-05, 3.28083989501312E-06, 6.21371192237334E-10, 0.001, 0.0001, 1E-06, 1E-09, 39.3700787401575, 0.0393700787401575, 1.09361329833771E-06, 10000, 1000, 1, 1E-05, 1E-07, 1E-08, 1E-15, 6.68458712266929E-18, 3.24077928963937E-23
            },
            {
                1, 3.93700787401575, 0.328083989501312, 6.21371192237334E-05, 100, 10, 0.1, 0.0001, 3937007.87401575, 3937.00787401575, 0.109361329833771, 1000000000, 100000000, 100000, 1, 0.01, 0.001, 1E-10, 6.68458712266929E-13, 3.24077928963937E-18
            },
            {
                1, 393.700787401575, 32.8083989501312, 0.00621371192237334, 10000, 1000, 10, 0.01, 393700787.401575, 393700.787401575, 10.9361329833771, 100000000000, 10000000000, 10000000, 100, 1, 0.1, 1E-08, 6.68458712266929E-11, 3.24077928963937E-16
            },
            {
                1, 3937.00787401575, 328.083989501312, 0.0621371192237334, 100000, 10000, 100, 0.1, 3937007874.01575, 3937007.87401575, 109.361329833771, 1000000000000, 100000000000, 100000000, 1000, 10, 1, 1E-07, 6.68458712266929E-10, 3.24077928963937E-15
            },
            {
                1, 39370078740.1575, 3280839895.01312, 621371.192237334, 1000000000000, 100000000000, 1000000000, 1000000, 3.93700787401575E+16, 39370078740157.5, 1093613298.33771, 1E+19, 1E+18, 1E+15, 10000000000, 100000000, 10000000, 1, 0.00668458712266929, 3.24077928963937E-08
            },
            {
                1, 5889679948465.72, 490806662372.143, 92955807.2674514, 149597870691029, 14959787069102.9, 149597870691.029, 149597870.691029, 5.88967994846572E+18, 5.88967994846572E+15, 163602220790.714, 1.49597870691029E+21, 1.49597870691029E+20, 1.49597870691029E+17, 1495978706910.29, 14959787069.1029, 1495978706.91029, 149.597870691029, 1, 4.84813681109636E-06
            },
            {
                1, 1.21483369342744E+18, 1.01236141118953E+17, 19173511575559.3, 3.08567758130569E+19, 3.08567758130569E+18, 3.08567758130569E+16, 30856775813056.9, 1.21483369342744E+24, 1.21483369342744E+21, 3.37453803729844E+16, 3.08567758130569E+26, 3.0856775813057E+25, 3.0856775813057E+22, 3.08567758130569E+17, 3.0856775813057E+15, 308567758130570, 30856775.813057, 206264.806247054, 1
            }
        };

        #endregion

        #region public methods

        /// <summary>
        /// Converts a value from one drawing unit to another.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <param name="from">Original drawing units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The converted value to the new drawing units.</returns>
        public static double ConvertUnit(double value, DrawingUnits from, DrawingUnits to)
        {
            return value*ConversionFactor(from, to);
        }

        /// <summary>
        /// Gets the conversion factor between drawing units.
        /// </summary>
        /// <param name="from">Original drawing units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The conversion factor between the drawing units.</returns>
        public static double ConversionFactor(DrawingUnits from, DrawingUnits to)
        {
            return UnitFactors[(int) from, (int) to];
        }

        /// <summary>
        /// Gets the conversion factor between image and drawing units.
        /// </summary>
        /// <param name="from">Original image units.</param>
        /// <param name="to">Destination drawing units.</param>
        /// <returns>The conversion factor between the units.</returns>
        public static double ConversionFactor(ImageUnits from, DrawingUnits to)
        {
            // more on the dxf format none sense, they don't even use the same integers for the drawing and the image units
            int rasterUnits = 0;
            switch (from)
            {
                case ImageUnits.Unitless:
                    rasterUnits = 0;
                    break;
                case ImageUnits.Millimeters:
                    rasterUnits = 4;
                    break;
                case ImageUnits.Centimeters:
                    rasterUnits = 5;
                    break;
                case ImageUnits.Meters:
                    rasterUnits = 6;
                    break;
                case ImageUnits.Kilometers:
                    rasterUnits = 7;
                    break;
                case ImageUnits.Inches:
                    rasterUnits = 1;
                    break;
                case ImageUnits.Feet:
                    rasterUnits = 2;
                    break;
                case ImageUnits.Yards:
                    rasterUnits = 10;
                    break;
                case ImageUnits.Miles:
                    rasterUnits = 3;
                    break;
            }
            return UnitFactors[rasterUnits, (int) to];
        }

        /// <summary>
        /// Gets the conversion factor between units.
        /// </summary>
        /// <param name="from">Original value units.</param>
        /// <param name="to">Destination value units.</param>
        /// <returns>The conversion factor between the passed units.</returns>
        public static double ConversionFactor(DrawingUnits from, ImageUnits to)
        {
            return 1/ConversionFactor(to, from);
        }

        #endregion
    }
}