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
    /// Dimension style override types.
    /// </summary>
    /// <remarks>
    /// There is one dimension style override type for each property of the <see cref="DimensionStyle">DimensionStyle</see> class.
    /// The dimension style properties DIMBLK and DIMSAH are not available.
    /// The overrides always make use of the DIMBLK1 and DIMBLK2 setting the DIMSAH to true even when both arrow ends are the same.
    /// </remarks>
    public enum DimensionStyleOverrideType
    {
        /// <summary>
        /// Assigns colors to dimension lines, arrowheads, and dimension leader lines.
        /// </summary>
        DimLineColor,

        /// <summary>
        /// Linetype of the dimension line.
        /// </summary>
        DimLineLinetype,

        /// <summary>
        /// Lineweight to dimension lines.
        /// </summary>
        DimLineLineweight,

        /// <summary>
        /// Suppresses display of the first dimension line.
        /// </summary>
        DimLine1Off,

        /// <summary>
        /// Suppresses display of the second dimension line.
        /// </summary>
        DimLine2Off,

        /// <summary>
        /// Distance the dimension line extends beyond the extension line when oblique, architectural tick, integral, or no marks are drawn for arrowheads.
        /// </summary>
        DimLineExtend,

        /// <summary>
        /// Colors to extension lines, center marks, and centerlines.
        /// </summary>
        ExtLineColor,

        /// <summary>
        /// Linetype of the first extension line.
        /// </summary>
        ExtLine1Linetype,

        /// <summary>
        /// Linetype of the second extension line.
        /// </summary>
        ExtLine2Linetype,

        /// <summary>
        /// Lineweight to extension lines.
        /// </summary>
        ExtLineLineweight,

        /// <summary>
        /// Suppresses display of the first extension line.
        /// </summary>
        ExtLine1Off,

        /// <summary>
        /// Suppresses display of the second extension line.
        /// </summary>
        ExtLine2Off,

        /// <summary>
        /// Specifies how far extension lines are offset from origin points.
        /// </summary>
        ExtLineOffset,

        /// <summary>
        /// Specifies how far to extend the extension line beyond the dimension line.
        /// </summary>
        ExtLineExtend,

        /// <summary>
        /// Enables fixed length extension lines.
        /// </summary>
        ExtLineFixed,

        /// <summary>
        /// Total length of the extension lines starting from the dimension line toward the dimension origin.
        /// </summary>
        ExtLineFixedLength,

        /// <summary>
        /// Arrowhead block for the first end of the dimension line.
        /// </summary>
        DimArrow1,

        /// <summary>
        /// Arrowhead block for the second end of the dimension line.
        /// </summary>
        DimArrow2,

        /// <summary>
        /// Arrowhead block for leaders.
        /// </summary>
        LeaderArrow,

        /// <summary>
        /// Size of dimension line and leader line arrowheads. Also controls the size of hook lines.
        /// </summary>
        ArrowSize,

        /// <summary>
        /// Drawing of circle or arc center marks and centerlines.
        /// </summary>
        CenterMarkSize,

        /// <summary>
        /// Text style of the dimension.
        /// </summary>
        TextStyle,

        /// <summary>
        /// Color of dimension text.
        /// </summary>
        TextColor,

        /// <summary>
        /// Color of background dimension text.
        /// </summary>
        TextFillColor,

        /// <summary>
        /// Height of dimension text, unless the current text style has a fixed height.
        /// </summary>
        TextHeight,

        /// <summary>
        /// Vertical position of text in relation to the dimension line.
        /// </summary>
        TextVerticalPlacement,

        /// <summary>
        /// Horizontal positioning of dimension text.
        /// </summary>
        TextHorizontalPlacement,

        /// <summary>
        /// Positioning of the dimension text inside extension lines.
        /// </summary>
        TextInsideAlign,

        /// <summary>
        /// Positioning of the dimension text outside extension lines.
        /// </summary>
        TextOutsideAlign,

        /// <summary>
        /// Distance around the dimension text when the dimension line breaks to accommodate dimension text.
        /// </summary>
        TextOffset,

        /// <summary>
        /// Specifies the direction of the dimension text.
        /// </summary>
        TextDirection,

        /// <summary>
        /// Controls the scale of fractions relative to dimension text height.
        /// </summary>
        /// <remarks>
        /// This value is only applicable to Architectural and Fractional units, and also
        /// controls the height factor applied to the tolerance text in relation with the dimension text height.
        /// </remarks>
        TextFractionHeightScale,

        /// <summary>
        /// Controls the drawing of the dimension lines even when the text are placed outside the extension lines.
        /// </summary>
        FitDimLineForce,

        /// <summary>
        /// Controls the drawing of dimension lines outside extension lines.
        /// </summary>
        FitDimLineInside,

        /// <summary>
        /// Overall scale factor applied to dimensioning variables that specify sizes, distances, or offsets.
        /// </summary>
        DimScaleOverall,

        /// <summary>
        /// Controls the placement of text and arrowheads based on the space available between the extension lines.
        /// </summary>
        FitOptions,

        /// <summary>
        /// Controls the drawing of text between the extension lines.
        /// </summary>
        FitTextInside,

        /// <summary>
        /// Controls the position of the text when it's moved either manually or automatically.
        /// </summary>
        FitTextMove,

        /// <summary>
        /// Number of precision places displayed in angular dimensions.
        /// </summary>
        AngularPrecision,

        /// <summary>
        /// Number of decimal places displayed for the primary units of a dimension.
        /// </summary>
        LengthPrecision,

        /// <summary>
        /// Specifies the text prefix for the dimension.
        /// </summary>
        DimPrefix,

        /// <summary>
        /// Specifies the text suffix for the dimension.
        /// </summary>
        DimSuffix,

        /// <summary>
        /// Single-character decimal separator to use when creating dimensions whose unit format is decimal.
        /// </summary>
        DecimalSeparator,

        /// <summary>
        /// Scale factor for linear dimension measurements
        /// </summary>
        DimScaleLinear,

        /// <summary>
        /// Units for all dimension types except angular.
        /// </summary>
        DimLengthUnits,

        /// <summary>
        /// Units format for angular dimensions.
        /// </summary>
        DimAngularUnits,

        /// <summary>
        /// Fraction format when DIMLUNIT is set to Architectural or Fractional.
        /// </summary>
        FractionalType,

        /// <summary>
        /// Suppresses leading zeros in linear decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        SuppressLinearLeadingZeros,

        /// <summary>
        /// Suppresses trailing zeros in linear decimal dimensions (for example, 12.5000 becomes 12.5).
        /// </summary>
        SuppressLinearTrailingZeros,

        /// <summary>
        /// Suppresses leading zeros in angular decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        SuppressAngularLeadingZeros,

        /// <summary>
        /// Suppresses trailing zeros in angular decimal dimensions (for example, 12.5000 becomes 12.5).
        /// </summary>
        SuppressAngularTrailingZeros,

        /// <summary>
        /// Suppresses zero feet in architectural dimensions.
        /// </summary>
        SuppressZeroFeet,

        /// <summary>
        /// Suppresses zero inches in architectural dimensions.
        /// </summary>
        SuppressZeroInches,

        /// <summary>
        /// Value to round all dimensioning distances.
        /// </summary>
        DimRoundoff,

        /// <summary>
        /// Controls the display of the alternate units.
        /// </summary>
        AltUnitsEnabled ,

        /// <summary>
        /// Alternate units for all dimension types except angular.
        /// </summary>
        AltUnitsLengthUnits,

        /// <summary>
        /// Controls if the Architectural or Fractional linear units will be shown stacked or not.
        /// </summary>
        AltUnitsStackedUnits,

        /// <summary>
        /// Number of decimal places displayed for the alternate units of a dimension.
        /// </summary>
        AltUnitsLengthPrecision,

        /// <summary>
        /// Multiplier used as the conversion factor between primary and alternate units.
        /// </summary>
        AltUnitsMultiplier,

        /// <summary>
        /// Value to round all alternate units of a dimension  except angular.
        /// </summary>
        AltUnitsRoundoff,

        /// <summary>
        /// Specifies the alternate units text prefix for the dimension.
        /// </summary>
        AltUnitsPrefix,

        /// <summary>
        /// Specifies the alternate units text suffix for the dimension.
        /// </summary>
        AltUnitsSuffix,

        /// <summary>
        /// Suppresses alternate units leading zeros in linear decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        AltUnitsSuppressLinearLeadingZeros,

        /// <summary>
        /// Suppresses alternate units trailing zeros in linear decimal dimensions (for example, 0.5000 becomes .5000).
        /// </summary>
        AltUnitsSuppressLinearTrailingZeros,

        /// <summary>
        /// Suppresses alternate units zero feet in architectural dimensions.
        /// </summary>
        AltUnitsSuppressZeroFeet,

        /// <summary>
        /// Suppresses alternate units zero inches in architectural dimensions.
        /// </summary>
        AltUnitsSuppressZeroInches,

        /// <summary>
        /// Method for calculating the tolerance.
        /// </summary>
        TolerancesDisplayMethod,

        /// <summary>
        /// Maximum or upper tolerance value. When you select Symmetrical in DisplayMethod, this value is used for the tolerance.
        /// </summary>
        TolerancesUpperLimit,

        /// <summary>
        /// Minimum or lower tolerance value.
        /// </summary>
        TolerancesLowerLimit,

        /// <summary>
        /// Text vertical placement for symmetrical and deviation tolerances.
        /// </summary>
        TolerancesVerticalPlacement,

        /// <summary>
        /// Gets or sets the number of decimal places.
        /// </summary>
        TolerancesPrecision,

        /// <summary>
        /// Suppresses leading zeros in linear decimal tolerance units (for example, 0.5000 becomes .5000).
        /// </summary>
        TolerancesSuppressLinearLeadingZeros,

        /// <summary>
        /// Suppresses trailing zeros in linear decimal tolerance units (for example, 12.5000 becomes 12.5).
        /// </summary>
        TolerancesSuppressLinearTrailingZeros,

        /// <summary>
        /// Suppresses zero feet in architectural tolerance units.
        /// </summary>
        TolerancesSuppressZeroFeet,

        /// <summary>
        /// Suppresses zero inches in architectural tolerance units.
        /// </summary>
        TolerancesSuppressZeroInches,
    
        /// <summary>
        /// Number of decimal places of the tolerance alternate units.
        /// </summary>
        TolerancesAlternatePrecision,

        /// <summary>
        /// Suppresses leading zeros in linear decimal alternate tolerance units (for example, 0.5000 becomes .5000).
        /// </summary>
        TolerancesAltSuppressLinearLeadingZeros,

        /// <summary>
        /// Suppresses trailing zeros in linear decimal alternate tolerance units (for example, 12.5000 becomes 12.5).
        /// </summary>
        TolerancesAltSuppressLinearTrailingZeros,

        /// <summary>
        /// Suppresses zero feet in architectural alternate tolerance units.
        /// </summary>
        TolerancesAltSuppressZeroFeet,

        /// <summary>
        /// Suppresses zero inches in architectural alternate tolerance units.
        /// </summary>
        TolerancesAltSuppressZeroInches
    }
}