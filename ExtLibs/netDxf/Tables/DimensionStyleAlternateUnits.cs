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

using System;
using netDxf.Units;

namespace netDxf.Tables
{
    /// <summary>
    /// Represents the way alternate units are formatted in dimension entities.
    /// </summary>
    /// <remarks>Alternative units are not applicable for angular dimensions.</remarks>
    public class DimensionStyleAlternateUnits :
        ICloneable
    {
        #region private fields

        private bool dimalt;
        private LinearUnitType dimaltu;
        private bool stackedUnits;
        private short dimaltd;
        private double dimaltf;
        private double dimaltrnd;
        private string dimPrefix;
        private string dimSuffix;
        private bool suppressLinearLeadingZeros;
        private bool suppressLinearTrailingZeros;
        private bool suppressZeroFeet;
        private bool suppressZeroInches;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>DimensionStyleUnitsFormat</c> class.
        /// </summary>
        public DimensionStyleAlternateUnits()
        {
            this.dimalt = false;
            this.dimaltd = 2;
            this.dimPrefix = string.Empty;
            this.dimSuffix = string.Empty;
            this.dimaltf = 25.4;
            this.dimaltu = LinearUnitType.Decimal;
            this.stackedUnits = false;
            this.suppressLinearLeadingZeros = false;
            this.suppressLinearTrailingZeros = false;
            this.suppressZeroFeet = true;
            this.suppressZeroInches = true;
            this.dimaltrnd = 0.0;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets if the alternate measurement units are added to the dimension text.  (DIMALT)
        /// </summary>
        public bool Enabled
        {
            get { return this.dimalt; }
            set { this.dimalt = value; }
        }

        /// <summary>
        /// Sets the number of decimal places displayed for the alternate units of a dimension. (DIMALTD)
        /// </summary>
        /// <remarks>
        /// Default: 4<br/>
        /// It is recommended to use values in the range 0 to 8.<br/>
        /// For architectural and fractional the precision used for the minimum fraction is 1/2^LinearDecimalPlaces.
        /// </remarks>
        public short LengthPrecision
        {
            get { return this.dimaltd; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The length precision must be equals or greater than zero.");
                this.dimaltd = value;
            }
        }

        /// <summary>
        /// Specifies the text prefix for the dimension. (DIMAPOST)
        /// </summary>
        public string Prefix
        {
            get { return this.dimPrefix; }
            set { this.dimPrefix = value ?? string.Empty; }
        }

        /// <summary>
        /// Specifies the text suffix for the dimension. (DIMAPOST)
        /// </summary>
        public string Suffix
        {
            get { return this.dimSuffix; }
            set { this.dimSuffix = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the multiplier used as the conversion factor between primary and alternate units. (DIMALTF)
        /// </summary>
        /// <remarks>
        /// to convert inches to millimeters, enter 25.4.
        /// The value has no effect on angular dimensions, and it is not applied to the rounding value or the plus or minus tolerance values. 
        /// </remarks>
        public double Multiplier
        {
            get { return this.dimaltf; }
            set
            {
                if (value <= 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The multiplier for alternate units must be greater than zero0.");
                this.dimaltf = value;
            }
        }

        /// <summary>
        /// Gets or sets the alternate units for all dimension types except angular. (DIMALTU)
        /// </summary>
        /// <remarks>
        /// Scientific<br/>
        /// Decimal<br/>
        /// Engineering<br/>
        /// Architectural<br/>
        /// Fractional
        /// </remarks>
        public LinearUnitType LengthUnits
        {
            get { return this.dimaltu; }
            set { this.dimaltu = value; }
        }

        /// <summary>
        /// Gets or set if the Architectural or Fractional linear units will be shown stacked or not. (DIMALTU)
        /// </summary>
        /// <remarks>
        /// This value only is applicable if the <c>DimLengthUnits</c> property has been set to Architectural or Fractional,
        /// for any other value this parameter is not applicable.
        /// </remarks>
        public bool StackUnits
        {
            get { return this.stackedUnits; }
            set { this.stackedUnits = value; }
        }

        /// <summary>
        /// Suppresses leading zeros in linear decimal alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressLinearLeadingZeros
        {
            get { return this.suppressLinearLeadingZeros; }
            set { this.suppressLinearLeadingZeros = value; }
        }

        /// <summary>
        /// Suppresses trailing zeros in linear decimal alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressLinearTrailingZeros
        {
            get { return this.suppressLinearTrailingZeros; }
            set { this.suppressLinearTrailingZeros = value; }
        }

        /// <summary>
        /// Suppresses zero feet in architectural alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressZeroFeet
        {
            get { return this.suppressZeroFeet; }
            set { this.suppressZeroFeet = value; }
        }

        /// <summary>
        /// Suppresses zero inches in architectural alternate units. (DIMALTZ)
        /// </summary>
        /// <remarks>This value is part of the DIMALTZ variable.</remarks>
        public bool SuppressZeroInches
        {
            get { return this.suppressZeroInches; }
            set { this.suppressZeroInches = value; }
        }

        /// <summary>
        /// Gets or sets the value to round all dimensioning distances. (DIMALTRND)
        /// </summary>
        /// <remarks>
        /// Default: 0 (no rounding off).<br/>
        /// If DIMRND is set to 0.25, all distances round to the nearest 0.25 unit.
        /// If you set DIMRND to 1.0, all distances round to the nearest integer.
        /// Note that the number of digits edited after the decimal point depends on the precision set by DIMDEC.
        /// DIMRND does not apply to angular dimensions.
        /// </remarks>
        public double Roundoff
        {
            get { return this.dimaltrnd; }
            set
            {
                if (value < 0.000001 && !MathHelper.IsZero(value, double.Epsilon)) // ToDo check range of values
                    throw new ArgumentOutOfRangeException(nameof(value), value, "The nearest value to round all distances must be equal or greater than 0.000001 or zero (no rounding off).");
                this.dimaltrnd = value;
            }
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new <c>DimensionStyle.DimensionStyleAlternateUnits</c> that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <c>DimensionStyle.DimensionStyleAlternateUnits</c> that is a copy of this instance.</returns>
        public object Clone()
        {
            DimensionStyleAlternateUnits copy = new DimensionStyleAlternateUnits()
            {
                Enabled = this.dimalt,
                LengthUnits = this.dimaltu,
                StackUnits = this.stackedUnits,
                LengthPrecision = this.dimaltd,
                Multiplier = this.dimaltf,
                Roundoff = this.dimaltrnd,
                Prefix = this.dimPrefix,
                Suffix = this.dimSuffix,
                SuppressLinearLeadingZeros = this.suppressLinearLeadingZeros,
                SuppressLinearTrailingZeros = this.suppressLinearTrailingZeros,
                SuppressZeroFeet = this.suppressZeroFeet,
                SuppressZeroInches = this.suppressZeroInches
            };

            return copy;
        }

        #endregion
    }
}