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
    /// Represents the hatch gradient pattern style.
    /// </summary>
    /// <remarks>
    /// Gradient patterns are only supported by AutoCad2004 and higher dxf versions. It will default to a solid pattern if saved as AutoCad2000.
    /// </remarks>
    public class HatchGradientPattern :
        HatchPattern
    {
        #region private fields

        private HatchGradientPatternType gradientType;
        private AciColor color1;
        private AciColor color2;
        private bool singleColor;
        private double tint;
        private bool centered;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a default linear gradient. 
        /// </summary>
        public HatchGradientPattern()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a default linear gradient. 
        /// </summary>
        /// <param name="description">Description of the pattern (optional, this information is not saved in the dxf file). By default it will use the supplied name.</param>
        public HatchGradientPattern(string description)
            : base("SOLID", description)
        {
            this.color1 = AciColor.Blue;
            this.color2 = AciColor.Yellow;
            this.singleColor = false;
            this.gradientType = HatchGradientPatternType.Linear;
            this.tint = 1.0;
            this.centered = true;
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a single color gradient. 
        /// </summary>
        /// <param name="color">Gradient <see cref="AciColor">color</see>.</param>
        /// <param name="tint">Gradient tint.</param>
        /// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
        public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type)
            : this(color, tint, type, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a single color gradient. 
        /// </summary>
        /// <param name="color">Gradient <see cref="AciColor">color</see>.</param>
        /// <param name="tint">Gradient tint.</param>
        /// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
        /// <param name="description">Description of the pattern (optional, this information is not saved in the dxf file). By default it will use the supplied name.</param>
        public HatchGradientPattern(AciColor color, double tint, HatchGradientPatternType type, string description)
            : base("SOLID", description)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(color));
            this.color1 = color;
            this.color2 = this.Color2FromTint(tint);
            this.singleColor = true;
            this.gradientType = type;
            this.tint = tint;
            this.centered = true;
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a two color gradient. 
        /// </summary>
        /// <param name="color1">Gradient <see cref="AciColor">color</see> 1.</param>
        /// <param name="color2">Gradient <see cref="AciColor">color</see> 2.</param>
        /// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
        public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type)
            : this(color1, color2, type, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>HatchGradientPattern</c> class as a two color gradient. 
        /// </summary>
        /// <param name="color1">Gradient <see cref="AciColor">color</see> 1.</param>
        /// <param name="color2">Gradient <see cref="AciColor">color</see> 2.</param>
        /// <param name="type">Gradient <see cref="HatchGradientPatternType">type</see>.</param>
        /// <param name="description">Description of the pattern (optional, this information is not saved in the dxf file). By default it will use the supplied name.</param>
        public HatchGradientPattern(AciColor color1, AciColor color2, HatchGradientPatternType type, string description)
            : base("SOLID", description)
        {
            if (color1 == null)
                throw new ArgumentNullException(nameof(color1));
            this.color1 = color1;
            if (color2 == null)
                throw new ArgumentNullException(nameof(color2));
            this.color2 = color2;
            this.singleColor = false;
            this.gradientType = type;
            this.tint = 1.0;
            this.centered = true;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or set the gradient pattern <see cref="HatchGradientPatternType">type</see>.
        /// </summary>
        public HatchGradientPatternType GradientType
        {
            get { return this.gradientType; }
            set { this.gradientType = value; }
        }

        /// <summary>
        /// Gets or sets the gradient <see cref="AciColor">color</see> 1.
        /// </summary>
        public AciColor Color1
        {
            get { return this.color1; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.color1 = value;
            }
        }

        /// <summary>
        /// Gets or sets the gradient <see cref="AciColor">color</see> 2.
        /// </summary>
        /// <remarks>
        /// If color 2 is defined, automatically the single color property will be set to false.  
        /// </remarks>
        public AciColor Color2
        {
            get { return this.color2; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.singleColor = false;
                this.color2 = value;
            }
        }

        /// <summary>
        /// Gets or sets the gradient pattern color type.
        /// </summary>
        public bool SingleColor
        {
            get { return this.singleColor; }
            set
            {
                if (value)
                    this.Color2 = this.Color2FromTint(this.tint);
                this.singleColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the gradient pattern tint.
        /// </summary>
        /// <remarks>It only applies to single color gradient patterns.</remarks>
        public double Tint
        {
            get { return this.tint; }
            set
            {
                if (this.singleColor)
                    this.Color2 = this.Color2FromTint(value);
                this.tint = value;
            }
        }

        /// <summary>
        /// Gets or sets if the pattern is centered or not.
        /// </summary>
        /// <remarks>
        /// Each gradient has two definitions, shifted and unsifted. A shift value describes the blend of the two definitions that should be used.
        /// A value of 0.0 (false) means only the unsifted version should be used, and a value of 1.0 (true) means that only the shifted version should be used.
        /// </remarks>
        public bool Centered
        {
            get { return this.centered; }
            set { this.centered = value; }
        }

        #endregion

        #region private methods

        private AciColor Color2FromTint(double value)
        {
            double h, s, l;
            AciColor.ToHsl(this.color1, out h, out s, out l);
            return AciColor.FromHsl(h, s, value);
        }

        #endregion

        #region ICloneable

        public override object Clone()
        {
            HatchGradientPattern copy = new HatchGradientPattern
            {
                // Pattern
                Fill = this.Fill,
                Type = this.Type,
                Origin = this.Origin,
                Angle = this.Angle,
                Scale = this.Scale,
                Style = this.Style,
                // GraientPattern
                GradientType = this.gradientType,
                Color1 = (AciColor) this.color1.Clone(),
                Color2 = (AciColor) this.color2.Clone(),
                SingleColor = this.singleColor,
                Tint = this.tint,
                Centered = this.centered
            };

            return copy;
        }

        #endregion
    }
}