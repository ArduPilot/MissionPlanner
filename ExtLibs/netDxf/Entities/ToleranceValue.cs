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

namespace netDxf.Entities
{
    /// <summary>
    /// Represents a tolerance, indicates the amount by which the geometric characteristic can deviate from a perfect form.
    /// </summary>
    public class ToleranceValue :
        ICloneable
    {
        #region private fields

        private bool showDiameterSymbol;
        private string tolerance;
        private ToleranceMaterialCondition materialCondition;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>ToleranceValue</c> class.
        /// </summary>
        public ToleranceValue()
        {
            this.showDiameterSymbol = false;
            this.tolerance = string.Empty;
            this.materialCondition = ToleranceMaterialCondition.None;
        }

        /// <summary>
        /// Initializes a new instance of the <c>ToleranceValue</c> class.
        /// </summary>
        /// <param name="showDiameterSymbol">Show a diameter symbol before the tolerance value.</param>
        /// <param name="value">Tolerance value.</param>
        /// <param name="materialCondition">Tolerance material condition.</param>
        public ToleranceValue(bool showDiameterSymbol, string value, ToleranceMaterialCondition materialCondition)
        {
            this.showDiameterSymbol = showDiameterSymbol;
            this.tolerance = value;
            this.materialCondition = materialCondition;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets if the tolerance diameter symbol will be shown.
        /// </summary>
        public bool ShowDiameterSymbol
        {
            get { return this.showDiameterSymbol; }
            set { this.showDiameterSymbol = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance value.
        /// </summary>
        public string Value
        {
            get { return this.tolerance; }
            set { this.tolerance = value; }
        }

        /// <summary>
        /// Gets or sets the tolerance material condition.
        /// </summary>
        public ToleranceMaterialCondition MaterialCondition
        {
            get { return this.materialCondition; }
            set { this.materialCondition = value; }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Creates a new ToleranceValue that is a copy of the current instance.
        /// </summary>
        /// <returns>A new ToleranceValue that is a copy of this instance.</returns>
        public object Clone()
        {
            return new ToleranceValue
            {
                ShowDiameterSymbol = this.showDiameterSymbol,
                Value = this.tolerance,
                MaterialCondition = this.materialCondition
            };
        }

        #endregion
    }
}