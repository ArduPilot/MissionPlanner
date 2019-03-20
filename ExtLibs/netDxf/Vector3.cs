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
    /// Represent a three component vector of double precision.
    /// </summary>
    public struct Vector3 :
        IEquatable<Vector3>
    {
        #region private fields

        private double x;
        private double y;
        private double z;
        private bool isNormalized;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="value">X, Y, Z component.</param>
        public Vector3(double value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="x">X component.</param>
        /// <param name="y">Y component.</param>
        /// <param name="z">Z component.</param>
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.isNormalized = false;
        }

        /// <summary>
        /// Initializes a new instance of Vector3.
        /// </summary>
        /// <param name="array">Array of three elements that represents the vector.</param>
        public Vector3(double[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length != 3)
                throw new ArgumentOutOfRangeException(nameof(array), array.Length, "The dimension of the array must be three.");
            this.x = array[0];
            this.y = array[1];
            this.z = array[2];
            this.isNormalized = false;
        }

        #endregion

        #region constants

        /// <summary>
        /// Zero vector.
        /// </summary>
        public static Vector3 Zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        /// <summary>
        /// Unit X vector.
        /// </summary>
        public static Vector3 UnitX
        {
            get { return new Vector3(1, 0, 0) {isNormalized = true}; }
        }

        /// <summary>
        /// Unit Y vector.
        /// </summary>
        public static Vector3 UnitY
        {
            get { return new Vector3(0, 1, 0) {isNormalized = true}; }
        }

        /// <summary>
        /// Unit Z vector.
        /// </summary>
        public static Vector3 UnitZ
        {
            get { return new Vector3(0, 0, 1) {isNormalized = true}; }
        }

        /// <summary>
        /// Represents a vector with not a number components.
        /// </summary>
        public static Vector3 NaN
        {
            get { return new Vector3(double.NaN, double.NaN, double.NaN); }
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
        /// Gets or sets the Z component.
        /// </summary>
        public double Z
        {
            get { return this.z; }
            set
            {
                this.isNormalized = false;
                this.z = value;
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
                    case 2:
                        return this.z;
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
                    case 2:
                        this.z = value;
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
        ///  Returns a value indicating if any component of the specified vector evaluates to a value that is not a number <see cref="System.Double.NaN"/>.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <returns>Returns true if any component of the specified vector evaluates to <see cref="System.Double.NaN"/>; otherwise, false.</returns>
        public static bool IsNaN(Vector3 u)
        {
            return double.IsNaN(u.X) || double.IsNaN(u.Y) || double.IsNaN(u.Z);
        }

        /// <summary>
        /// Obtains the dot product of two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The dot product.</returns>
        public static double DotProduct(Vector3 u, Vector3 v)
        {
            return u.X*v.X + u.Y*v.Y + u.Z*v.Z;
        }

        /// <summary>
        /// Obtains the cross product of two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 CrossProduct(Vector3 u, Vector3 v)
        {
            double a = u.Y*v.Z - u.Z*v.Y;
            double b = u.Z*v.X - u.X*v.Z;
            double c = u.X*v.Y - u.Y*v.X;
            return new Vector3(a, b, c);
        }

        /// <summary>
        /// Obtains the distance between two points.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>Distance.</returns>
        public static double Distance(Vector3 u, Vector3 v)
        {
            return Math.Sqrt((u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y) + (u.Z - v.Z)*(u.Z - v.Z));
        }

        /// <summary>
        /// Obtains the square distance between two points.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>Square distance.</returns>
        public static double SquareDistance(Vector3 u, Vector3 v)
        {
            return (u.X - v.X)*(u.X - v.X) + (u.Y - v.Y)*(u.Y - v.Y) + (u.Z - v.Z)*(u.Z - v.Z);
        }

        /// <summary>
        /// Obtains the angle between two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>Angle in radians.</returns>
        public static double AngleBetween(Vector3 u, Vector3 v)
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
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 MidPoint(Vector3 u, Vector3 v)
        {
            return new Vector3((v.X + u.X)*0.5, (v.Y + u.Y)*0.5, (v.Z + u.Z)*0.5);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector3 u, Vector3 v)
        {
            return ArePerpendicular(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are perpendicular.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are perpendicular or false in any other case.</returns>
        public static bool ArePerpendicular(Vector3 u, Vector3 v, double threshold)
        {
            return MathHelper.IsZero(DotProduct(u, v), threshold);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector3 u, Vector3 v)
        {
            return AreParallel(u, v, MathHelper.Epsilon);
        }

        /// <summary>
        /// Checks if two vectors are parallel.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <param name="threshold">Tolerance used.</param>
        /// <returns>True if are parallel or false in any other case.</returns>
        public static bool AreParallel(Vector3 u, Vector3 v, double threshold)
        {
            Vector3 cross = CrossProduct(u, v);

            if (!MathHelper.IsZero(cross.X, threshold))
                return false;
            if (!MathHelper.IsZero(cross.Y, threshold))
                return false;
            if (!MathHelper.IsZero(cross.Z, threshold))
                return false;
            return true;
        }

        /// <summary>
        /// Rounds the components of a vector.
        /// </summary>
        /// <param name="u">Vector to round.</param>
        /// <param name="numDigits">Number of decimal places in the return value.</param>
        /// <returns>The rounded vector.</returns>
        public static Vector3 Round(Vector3 u, int numDigits)
        {
            return new Vector3(Math.Round(u.X, numDigits), Math.Round(u.Y, numDigits), Math.Round(u.Z, numDigits));
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <param name="u">Vector to normalize</param>
        /// <returns>A normalized vector.</returns>
        public static Vector3 Normalize(Vector3 u)
        {
            if (u.isNormalized) return u;

            double mod = u.Modulus();
            if (MathHelper.IsZero(mod))
                return NaN;
            double modInv = 1/mod;
            Vector3 vec = new Vector3(u.x*modInv, u.y*modInv, u.z*modInv);
            vec.isNormalized = true;
            return vec;
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Check if the components of two vectors are equal.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>True if the three components are equal or false in any other case.</returns>
        public static bool operator ==(Vector3 u, Vector3 v)
        {
            return Equals(u, v);
        }

        /// <summary>
        /// Check if the components of two vectors are different.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>True if the three components are different or false in any other case.</returns>
        public static bool operator !=(Vector3 u, Vector3 v)
        {
            return !Equals(u, v);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector3 operator +(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The addition of u plus v.</returns>
        public static Vector3 Add(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector3 operator -(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The subtraction of u minus v.</returns>
        public static Vector3 Subtract(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector3 operator -(Vector3 u)
        {
            return new Vector3(-u.X, -u.Y, -u.Z) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <returns>The negative vector of u.</returns>
        public static Vector3 Negate(Vector3 u)
        {
            return new Vector3(-u.X, -u.Y, -u.Z) { isNormalized = u.IsNormalized };
        }

        /// <summary>
        /// Multiplies a vector with an scalar (same as a*u, commutative property).
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector3 operator *(Vector3 u, double a)
        {
            return new Vector3(u.X*a, u.Y*a, u.Z*a);
        }

        /// <summary>
        /// Multiplies a vector with an scalar (same as a*u, commutative property).
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector3 Multiply(Vector3 u, double a)
        {
            return new Vector3(u.X*a, u.Y*a, u.Z*a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector (same as u*a, commutative property).
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector3 operator *(double a, Vector3 u)
        {
            return new Vector3(u.X*a, u.Y*a, u.Z*a);
        }

        /// <summary>
        /// Multiplies a scalar with a vector (same as u*a, commutative property).
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The multiplication of u times a.</returns>
        public static Vector3 Multiply(double a, Vector3 u)
        {
            return new Vector3(u.X*a, u.Y*a, u.Z*a);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector3 operator *(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);
        }

        /// <summary>
        /// Multiplies two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector3 Multiply(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X * v.X, u.Y * v.Y, u.Z * v.Z);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector3 operator /(double a, Vector3 u)
        {
            return new Vector3(a * u.X, a * u.Y, a * u.Z);
        }

        /// <summary>
        /// Divides a scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector3 Divide(double a, Vector3 u)
        {
            return new Vector3(a * u.X, a * u.Y, a * u.Z);
        }

        /// <summary>
        /// Divides an scalar with a vector.
        /// </summary>
        /// <param name="a">Scalar.</param>
        /// <param name="u">Vector3.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector3 operator /(Vector3 u, double a)
        {
            double invEscalar = 1/a;
            return new Vector3(u.X*invEscalar, u.Y*invEscalar, u.Z*invEscalar);
        }

        /// <summary>
        /// Divides a vector with an scalar.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>The division of u times a.</returns>
        public static Vector3 Divide(Vector3 u, double a)
        {
            double invEscalar = 1/a;
            return new Vector3(u.X*invEscalar, u.Y*invEscalar, u.Z*invEscalar);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector3.</param>
        /// <param name="v">Vector3.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector3 operator /(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);
        }

        /// <summary>
        /// Divides two vectors component by component.
        /// </summary>
        /// <param name="u">Vector2.</param>
        /// <param name="v">Vector2.</param>
        /// <returns>The multiplication of u times v.</returns>
        public static Vector3 Divide(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X / v.X, u.Y / v.Y, u.Z / v.Z);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Normalizes the current vector.
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
                this.z *= modInv;
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
            return new[] {this.x, this.y, this.z};
        }

        #endregion

        #region comparison methods

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector3.</param>
        /// <param name="b">Vector3.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector3 a, Vector3 b)
        {
            return a.Equals(b, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="a">Vector3.</param>
        /// <param name="b">Vector3.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public static bool Equals(Vector3 a, Vector3 b, double threshold)
        {
            return a.Equals(b, threshold);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="other">Vector3.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector3 other)
        {
            return this.Equals(other, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two vectors are approximate equal.
        /// </summary>
        /// <param name="other">Vector3.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the three components are almost equal or false in any other case.</returns>
        public bool Equals(Vector3 other, double threshold)
        {
            return MathHelper.IsEqual(other.X, this.x, threshold) && MathHelper.IsEqual(other.Y, this.y, threshold) && MathHelper.IsEqual(other.Z, this.z, threshold);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other is Vector3)
                return this.Equals((Vector3) other);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            return string.Format("{0}{3} {1}{3} {2}", this.x, this.y, this.z, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        /// <summary>
        /// Obtains a string that represents the vector.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            return string.Format("{0}{3} {1}{3} {2}", this.x.ToString(provider), this.y.ToString(provider), this.z.ToString(provider), Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator);
        }

        #endregion
    }
}