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
using netDxf.Tables;

namespace netDxf.Objects
{
    /// <summary>
    /// Represent each of the elements that make up a MLineStyle.
    /// </summary>
    public class MLineStyleElement :
        IComparable<MLineStyleElement>,
        ICloneable
    {
        #region delegates and events

        public delegate void LinetypeChangedEventHandler(MLineStyleElement sender, TableObjectChangedEventArgs<Linetype> e);

        public event LinetypeChangedEventHandler LinetypeChanged;

        protected virtual Linetype OnLinetypeChangedEvent(Linetype oldLinetype, Linetype newLinetype)
        {
            LinetypeChangedEventHandler ae = this.LinetypeChanged;
            if (ae != null)
            {
                TableObjectChangedEventArgs<Linetype> eventArgs = new TableObjectChangedEventArgs<Linetype>(oldLinetype, newLinetype);
                ae(this, eventArgs);
                return eventArgs.NewValue;
            }
            return newLinetype;
        }

        #endregion

        #region private fields

        private double offset;
        private AciColor color;
        private Linetype linetype;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyleElement</c> class.
        /// </summary>
        /// <param name="offset">Element offset.</param>
        public MLineStyleElement(double offset)
            : this(offset, AciColor.ByLayer, Linetype.ByLayer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <c>MLineStyleElement</c> class.
        /// </summary>
        /// <param name="offset">Element offset.</param>
        /// <param name="color">Element color.</param>
        /// <param name="linetype">Element line type.</param>
        public MLineStyleElement(double offset, AciColor color, Linetype linetype)
        {
            this.offset = offset;
            this.color = color;
            this.linetype = linetype;
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the element offset.
        /// </summary>
        public double Offset
        {
            get { return this.offset; }
            set { this.offset = value; }
        }

        /// <summary>
        /// Gets or sets the element color.
        /// </summary>
        /// <remarks>
        /// AutoCad2000 dxf version does not support true colors for MLineStyleElement color.
        /// </remarks>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the element line type.
        /// </summary>
        public Linetype Linetype
        {
            get { return this.linetype; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                this.linetype = this.OnLinetypeChangedEvent(this.linetype, value);
            }
        }

        #endregion

        #region implements IComparable

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared.
        /// The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.
        /// Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(MLineStyleElement other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            return this.offset.CompareTo(other.offset);
        }

        /// <summary>
        /// Check if two MLineStyleElement are equal.
        /// </summary>
        /// <param name="other">Another MLineStyleElement to compare to.</param>
        /// <returns>True if two MLineStyleElement are equal or false in any other case.</returns>
        /// <remarks>
        /// Two MLineStyleElement are considered equals if their offsets are the same.
        /// </remarks>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            if (this.GetType() != other.GetType())
                return false;

            return this.Equals((MLineStyleElement) other);
        }

        /// <summary>
        /// Check if two MLineStyleElement are equal.
        /// </summary>
        /// <param name="other">Another MLineStyleElement to compare to.</param>
        /// <returns>True if two MLineStyleElement are equal or false in any other case.</returns>
        /// <remarks>
        /// Two MLineStyleElement are considered equals if their offsets are the same.
        /// </remarks>
        public bool Equals(MLineStyleElement other)
        {
            if (other == null)
                return false;

            return MathHelper.IsEqual(this.offset, other.offset);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.Offset.GetHashCode();
        }

        ///// <summary>
        ///// Check if the two elements are equal.
        ///// </summary>
        ///// <param name="u">MLineStyleElement.</param>
        ///// <param name="v">MLineStyleElement.</param>
        ///// <returns>True if the two element offsets are equal, false in any other case.</returns>
        //public static bool operator ==(MLineStyleElement u, MLineStyleElement v)
        //{
        //    if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
        //        return true;

        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return MathHelper.IsEqual(u.Offset, v.Offset);
        //}

        ///// <summary>
        ///// Check if the elements are different.
        ///// </summary>
        ///// <param name="u">MLineStyleElement.</param>
        ///// <param name="v">MLineStyleElement.</param>
        ///// <returns>True if the two element offsets are different, false in any other case.</returns>
        //public static bool operator !=(MLineStyleElement u, MLineStyleElement v)
        //{
        //    if (ReferenceEquals(u, null) && ReferenceEquals(v, null))
        //        return false;

        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return true;

        //    return !MathHelper.IsEqual(u.offset, v.offset);
        //}

        ///// <summary>
        ///// Check if the first element is lesser than the second.
        ///// </summary>
        ///// <param name="u">MLineStyleElement.</param>
        ///// <param name="v">MLineStyleElement.</param>
        ///// <returns>True if the first element offset is lesser than the second, false in any other case.</returns>
        //public static bool operator <(MLineStyleElement u, MLineStyleElement v)
        //{
        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return u.offset.CompareTo(v.offset) < 0;
        //}

        ///// <summary>
        ///// Check if the first element is greater than the second.
        ///// </summary>
        ///// <param name="u">MLineStyleElement.</param>
        ///// <param name="v">MLineStyleElement.</param>
        ///// <returns>True if the first element offset is greater than the second, false in any other case.</returns>
        //public static bool operator >(MLineStyleElement u, MLineStyleElement v)
        //{
        //    if (ReferenceEquals(u, null) || ReferenceEquals(v, null))
        //        return false;

        //    return u.offset.CompareTo(v.offset) > 0;
        //}

        #endregion

        #region implements ICloneable

        /// <summary>
        /// Creates a MLineStyleElement that is a copy of the current instance.
        /// </summary>
        /// <returns>A new MLineStyleElement is a copy of this instance.</returns>
        public object Clone()
        {
            return new MLineStyleElement(this.offset)
            {
                Color = (AciColor) this.Color.Clone(),
                Linetype = (Linetype) this.linetype.Clone()
            };
        }

        #endregion

        #region overrides

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format("{0}, color:{1}, line type:{2}", this.offset, this.color, this.linetype);
        }

        #endregion
    }
}