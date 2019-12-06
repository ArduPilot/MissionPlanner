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
using System.Globalization;

namespace netDxf
{
    /// <summary>
    /// Represents the transparency of a layer or an entity.
    /// </summary>
    /// <remarks>
    /// When the transparency of an entity is ByLayer the code 440 will not appear in the dxf,
    /// but for comparison purposes the ByLayer transparency is assigned a value of -1.
    /// </remarks>
    public class Transparency :
        ICloneable,
        IEquatable<Transparency>
    {
        #region private fields

        private short transparency;

        #endregion

        #region constants

        /// <summary>
        /// Gets the ByLayer transparency.
        /// </summary>
        public static Transparency ByLayer
        {
            get { return new Transparency {transparency = -1}; }
        }

        /// <summary>
        /// Gets the ByBlock transparency.
        /// </summary>
        public static Transparency ByBlock
        {
            get { return new Transparency {transparency = 100}; }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Transparency</c> class.
        /// </summary>
        public Transparency()
        {
            this.transparency = -1;
        }

        /// <summary>
        /// Initializes a new instance of the <c>Transparency</c> class.
        /// </summary>
        /// <param name="value">Alpha value range from 0 to 90.</param>
        /// <remarks>
        /// Accepted transparency values range from 0 (opaque) to 90 (almost transparent), the reserved values -1 and 100 represents ByLayer and ByBlock transparency.
        /// </remarks>
        public Transparency(short value)
        {
            if (value < 0 || value > 90)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted transparency values range from 0 to 90.");
            this.transparency = value;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Defines if the transparency is defined by layer.
        /// </summary>
        public bool IsByLayer
        {
            get { return this.transparency == -1; }
        }

        /// <summary>
        /// Defines if the transparency is defined by block.
        /// </summary>
        public bool IsByBlock
        {
            get { return this.transparency == 100; }
        }

        /// <summary>
        /// Gets or sets the transparency value range from 0 to 90.
        /// </summary>
        /// <remarks>
        /// Accepted transparency values range from 0 to 90, the reserved values -1 and 100 represents ByLayer and ByBlock.
        /// </remarks>
        public short Value
        {
            get { return this.transparency; }
            set
            {
                if (value < 0 || value > 90)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Accepted transparency values range from 0 to 90.");
                this.transparency = value;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Gets the transparency value from a <see cref="Transparency">transparency</see> object.
        /// </summary>
        /// <param name="transparency">A <see cref="Transparency">transparency</see>.</param>
        /// <returns>A transparency value.</returns>
        public static int ToAlphaValue(Transparency transparency)
        {
            if (transparency == null)
                throw new ArgumentNullException(nameof(transparency));

            byte alpha = (byte) (255*(100 - transparency.Value)/100.0);
            byte[] bytes = {alpha, 0, 0, 2};
            if (transparency.IsByBlock)
                bytes[3] = 1;
            return BitConverter.ToInt32(bytes, 0);
        }

        /// <summary>
        /// Gets the <see cref="Transparency">transparency</see> object from a transparency value.
        /// </summary>
        /// <param name="value">A transparency value.</param>
        /// <returns>A <see cref="Transparency">transparency</see></returns>
        public static Transparency FromAlphaValue(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            short alpha = (short) (100 - (bytes[0]/255.0)*100);
            return FromCadIndex(alpha);
        }

        #endregion

        #region private methods

        private static Transparency FromCadIndex(short alpha)
        {
            if (alpha == -1)
                return ByLayer;
            if (alpha == 100)
                return ByBlock;

            return new Transparency(alpha);
        }

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a new transparency that is a copy of the current instance.
        /// </summary>
        /// <returns>A new transparency that is a copy of this instance.</returns>
        public object Clone()
        {
            return FromCadIndex(this.transparency);
        }

        #endregion

        #region implements IEquatable

        /// <summary>
        /// Check if the components of two transparencies are equal.
        /// </summary>
        /// <param name="other">Another transparency to compare to.</param>
        /// <returns>True if their indexes are equal or false in any other case.</returns>
        public bool Equals(Transparency other)
        {
            if (other == null)
                return false;

            return other.transparency == this.transparency;
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.transparency == -1)
                return "ByLayer";
            if (this.transparency == 100)
                return "ByBlock";

            return this.transparency.ToString(CultureInfo.CurrentCulture);
        }

        #endregion
    }
}