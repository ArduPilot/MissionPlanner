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
using System.Threading;

namespace netDxf
{
    /// <summary>
    /// Represent a two component vector of double precision.
    /// </summary>
    public struct Vector2 :
        IEquatable<Vector2>
    {
        #region private fields

        private double x;
        private double y;
        private bool isNormalized;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="value">X, Y component.</param>
        public Vector2(double value)
        {
            this.x = value;
            this.y = value;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector2.
        /// </summary>
        /// <param name="array">Array of two elements that represents the vector.</param>
        public Vector2(double[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length != 2)
                throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The dimension of the array must be two");
            this.x = array[0];
            this.y = array[1];
            this.isNormalized = false;
        }

        #endregion

        #region constants

        /// <summary>
        /// Zero vector.
        /// </summary>
        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }

        /// <summary>
        /// Unit X vector.
        /// </summary>
        public static Vector2 UnitX
        {
            get { return new Vector2(1, 0) {isNormalized = true}; }
        }

        /// <summary>
        /// Unit Y vector.
        /// </summary>
        public static Vector2 UnitY
        {
            get { return new Vector2(0, 1) {isNormalized = true}; }
        }

        /// <summary>
        /// Represents a vector with not a number components.
        /// </summary>
        public static Vector2 NaN
        {
            get { return new Vector2(double.NaN, double.NaN); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the X component.
        /// </summary>
        public double X
        {
            get { return this.x; }
            set
            {
                this.isNormalized = false;
                this.x = value;
            }
        }

        /// <summary>
        /// Gets or sets the Y component.
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set
            {
                this.isNormalized = false;
                this.y = value;
            }
        }

        /// <summary>
        /// Gets or sets a vector element defined by its index.
        /// </summary>
        /// <param name="index">Index of the element.</param>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;
                    case 1:
                        return this.y;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
            set
            {
                this.isNormalized = false;
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>
        /// Gets if the vector has been normalized.
        /// </summary>
        public bool IsNormalized
        {
            get { return this.isNormalized; }
        }

        #endregion

        #region static methods

        /// <summary>
        /// Returns a value indicating if any component of the specified vector evaluates to a value that is not a number <see cref="System.Double.NaN"/>.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Returns true if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, false.</returns>
        public static bool IsNaN(Vector2 u)
        {
            return double.IsNaN(u.X) || double.IsNaN(u.Y);
        }

        /// <summary>
        /// Obtains the dot product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector2 u, Vector2 v)
        {
            return u.X*v.X + u.Y*v.Y;
        }

        /// <summary>
        /// Obtains the cross product of two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static double CrossProduct(Vector2 u, Vector2 v)
        {
            return u.X*v.Y - u.Y*v.X;
        }

        /// <summary>
        /// Obtains the counter clockwise perpendicular vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Perpendicular(Vector2 u)
        {
            return new Vector2(-u.Y, u.X) {isNormalized = u.IsNormalized};
        }

        /// <summary>
        /// Rotates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="angle">Rotation angles in radians.</param>
        /// <returns></returns>
        public static Vector2 Rotate(Vector2 u, double angle)
        {
            if (MathHelper.IsZero(angle))
                return u;
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return new Vector2(u.X*cos - u.Y*sin, u.X*sin + u.Y*cos) {isNormalized = u.IsNormalized};
        }

        /// <summary>
        /// Obtains a the polar point of another point. 
        /// </summary>
        /// <param name="u">Reference point.</param>
        /// <param name="distance">Distance from point u.</param>
        /// <param name="angle">Angle in radians.</param>
        /// <returns>The polar point of the specified point.</returns>
        public static Vector2 Polar(Vector2 u, double distance, double angle)
        {
            Vector2 dir = new Vector2(Math.Cos(angle), Math.Sin(angle));
            return u + dir*distance;
        }

        /// <summary>
        /// Obtains the distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Distance.</returns>
        public static double Distance(Vector2 u, Vector2 v)
        {
            return Math.Sqrt((u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y));
        }

        /// <summary>
        /// Obtains the square distance between two points.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Square distance.</returns>
        public static double SquareDistance(Vector2 u, Vector2 v)
        {
            return (u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y);
        }

        /// <summary>
        /// Obtains the angle of a vector.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u)
        {
            double angle = Math.Atan2(u.Y, u.X);
            if (angle < 0)
                return MathHelper.TwoPI + angle;
            return angle;
        }

        /// <summary>
        /// Obtains the angle of a line defined by two points.
        /// </summary>
        /// <param name="u">A Vector2.</param>
        /// <param name="v">A Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double Angle(Vector2 u, Vector2 v)
        {
            Vector2 dir = v - u;
            return Angle(dir);
        }

        /// <summary>
        /// Obtains the angle between two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Angle in radians.</returns>
        public static double AngleBetween(Vector2 u, Vector2 v)
        {
            double cos = DotProduct(u, v)/(u.Modulus()*v.Modulus());
            if (cos >= 1.0)
                return 0.0;
            if (cos <= -1.0)
                return Math.PI;

            return Math.Acos(cos);
        }

        /// <summary>
        /// Obtains the midpoint.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 MidPoint(Vector2 u, Vector2 v)
        {
            return new Vector2((v.X + u.X)*0.5, (v.Y + u.Y)*0.5);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector2 u, Vector2 v)
        {
            return ArePerpendicular(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector2 u, Vector2 v, double threshold)
        {
            return MathHelper.IsZero(DotProduct(u, v), threshold);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector2 u, Vector2 v)
        {
            return AreParallel(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector2 u, Vector2 v, double threshold)
        {
            return MathHelper.IsZero(CrossProduct(u, v), threshold);
        }

        /// <summary>
        /// Rounds the components of a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="numDigits">Number of decimal places in the return value.</param>
        /// <returns>Vector2.</returns>
        public static Vector2 Round(Vector2 u, int numDigits)
        {
            return new Vector2(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="u">Vector to normalize</param>
        /// <returns>A normalized vector.</returns>
        public static Vector2 Normalize(Vector2 u)
        {
            if (u.isNormalized) return u;

            double mod = u.Modulus();
            if (MathHelper.IsZero(mod))
                return NaN;
            double modInv = 1/mod;
            return new Vector2(u.x*modInv, u.y*modInv) {isNormalized = true};
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Check if the components of two vectors are equal.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are equal or false in any other case.</returns>
        public static bool operator ==(Vector2 u, Vector2 v)
        {
            return Equals(u, v);
        }

        /// <summary>
        /// Check if the components of two vectors are different.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>True if the two components are different or false in any other case.</returns>
        public static bool operator !=(Vector2 u, Vector2 v)
        {
            return !Equals(u, v);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector2 operator +(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector2 Add(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector2 operator -(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector2 Subtract(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector2 operator -(Vector2 u)
        {
            return new Vector2(-u.X, -u.Y) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector2 Negate(Vector2 u)
        {
            return new Vector2(-u.X, -u.Y) {isNormalized = u.IsNormalized};
        }

        /// <summary>
        /// Multiplies a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 operator *(Vector2 u, double a)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Multiplies a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 Multiply(Vector2 u, double a)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 operator *(double a, Vector2 u)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector2 Multiply(double a, Vector2 u)
        {
            return new Vector2(u.X*a, u.Y*a);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 operator *(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X * v.X, u.Y * v.Y);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 Multiply(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X * v.X, u.Y * v.Y);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector2.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 operator /(double a, Vector2 u)
        {
            return new Vector2(a* u.X, a* u.Y);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector2.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 Divide(double a, Vector2 u)
        {
            return new Vector2(a * u.X, a * u.Y);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 operator /(Vector2 u, double a)
        {
            double invEscalar = 1/a;
            return new Vector2(u.X*invEscalar, u.Y*invEscalar);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector2 Divide(Vector2 u, double a)
        {
            double invEscalar = 1/a;
            return new Vector2(u.X*invEscalar, u.Y*invEscalar);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 operator /(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X / v.X, u.Y / v.Y);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector2 Divide(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X / v.X, u.Y / v.Y);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        public void Normalize()
        {
            if (this.isNormalized) return;

            double mod = this.Modulus();
            if (MathHelper.IsZero(mod))
                this = NaN;
            else
            {
                double modInv = 1/mod;
                this.x *= modInv;
                this.y *= modInv;
            }
            this.isNormalized = true;
        }

        /// <summary>
        /// Obtains the modulus of the vector.
        /// </summary>
        /// <returns>Vector modulus.</returns>
        public double Modulus()
        {
            return Math.Sqrt(DotProduct(this, this));
        }

        /// <summary>
        /// Returns an array that represents the vector.
        /// </summary>
        /// <returns>Array.</returns>
        public double[] ToArray()
        {
            return new[] {this.x, this.y};
        }

        #endregion

        #region comparison methods

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector2.</param>
        /// <param name="b">Vector2.</param>
        /// <returns>True if the two components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector2 a, Vector2 b)
        {
            return a.Equals(b, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector2.</param>
        /// <param name="b">Vector2.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the two components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector2 a, Vector2 b, double threshold)
        {
            return a.Equals(b, threshold);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="other">Another Vector2 to compare to.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 other)
        {
            return this.Equals(other, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equals.
        /// </summary>
        /// <param name="other">Another Vector2 to compare to.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector2 other, double threshold)
        {
            return MathHelper.IsEqual(other.X, this.x, threshold) && MathHelper.IsEqual(other.Y, this.y, threshold);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is Vector2)
                return this.Equals((Vector2) other);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0}{2} {1}", this.x, this.y, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0}{2} {1}", this.x.ToString(provider), this.y.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        #endregion
    }
}