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
using System.Text;
using System.Threading;

namespace netDxf
{
    /// <summary>
    /// Represents a 3x3 double precision matrix.
    /// </summary>
    public struct Matrix3 :
        IEquatable<Matrix3>
    {
        #region private fields

        private double m11;
        private double m12;
        private double m13;
        private double m21;
        private double m22;
        private double m23;
        private double m31;
        private double m32;
        private double m33;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of Matrix3.
        /// </summary>
        /// <param name="m11">Element [0,0].</param>
        /// <param name="m12">Element [0,1].</param>
        /// <param name="m13">Element [0,1].</param>
        /// <param name="m21">Element [1,0].</param>
        /// <param name="m22">Element [1,1].</param>
        /// <param name="m23">Element [1,2].</param>
        /// <param name="m31">Element [2,0].</param>
        /// <param name="m32">Element [2,1].</param>
        /// <param name="m33">Element [2,2].</param>
        public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;

            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;

            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        /// <summary>
        /// Initializes a new instance of Matrix3.
        /// </summary>
        /// <param name="array">Array of nine components.</param>
        public Matrix3(double[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (array.Length != 9)
                throw new ArgumentException("The array must contain 9 elements.");
            this.m11 = array[0];
            this.m12 = array[1];
            this.m13 = array[2];

            this.m21 = array[3];
            this.m22 = array[4];
            this.m23 = array[5];

            this.m31 = array[6];
            this.m32 = array[7];
            this.m33 = array[8];
        }

        #endregion

        #region constants

        /// <summary>
        /// Gets the zero matrix.
        /// </summary>
        public static Matrix3 Zero
        {
            get { return new Matrix3(0, 0, 0, 0, 0, 0, 0, 0, 0); }
        }

        /// <summary>
        /// Gets the identity matrix.
        /// </summary>
        public static Matrix3 Identity
        {
            get { return new Matrix3(1, 0, 0, 0, 1, 0, 0, 0, 1); }
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets or sets the matrix element [0,0].
        /// </summary>
        public double M11
        {
            get { return this.m11; }
            set { this.m11 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [0,1].
        /// </summary>
        public double M12
        {
            get { return this.m12; }
            set { this.m12 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [0,2].
        /// </summary>
        public double M13
        {
            get { return this.m13; }
            set { this.m13 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,0].
        /// </summary>
        public double M21
        {
            get { return this.m21; }
            set { this.m21 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,1].
        /// </summary>
        public double M22
        {
            get { return this.m22; }
            set { this.m22 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [1,2].
        /// </summary>
        public double M23
        {
            get { return this.m23; }
            set { this.m23 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,0].
        /// </summary>
        public double M31
        {
            get { return this.m31; }
            set { this.m31 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,1].
        /// </summary>
        public double M32
        {
            get { return this.m32; }
            set { this.m32 = value; }
        }

        /// <summary>
        /// Gets or sets the matrix element [2,2].
        /// </summary>
        public double M33
        {
            get { return this.m33; }
            set { this.m33 = value; }
        }

        #endregion

        #region overloaded operators

        /// <summary>
        /// Matrix addition.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator +(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33);
        }

        /// <summary>
        /// Matrix addition.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 Add(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 + b.M11, a.M12 + b.M12, a.M13 + b.M13, a.M21 + b.M21, a.M22 + b.M22, a.M23 + b.M23, a.M31 + b.M31, a.M32 + b.M32, a.M33 + b.M33);
        }

        /// <summary>
        /// Matrix subtraction.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator -(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33);
        }

        /// <summary>
        /// Matrix subtraction.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 Subtract(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11 - b.M11, a.M12 - b.M12, a.M13 - b.M13, a.M21 - b.M21, a.M22 - b.M22, a.M23 - b.M23, a.M31 - b.M31, a.M32 - b.M32, a.M33 - b.M33);
        }

        /// <summary>
        /// Product of two matrices.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11*b.M11 + a.M12*b.M21 + a.M13*b.M31, a.M11*b.M12 + a.M12*b.M22 + a.M13*b.M32, a.M11*b.M13 + a.M12*b.M23 + a.M13*b.M33, a.M21*b.M11 + a.M22*b.M21 + a.M23*b.M31,
                a.M21*b.M12 + a.M22*b.M22 + a.M23*b.M32, a.M21*b.M13 + a.M22*b.M23 + a.M23*b.M33, a.M31*b.M11 + a.M32*b.M21 + a.M33*b.M31, a.M31*b.M12 + a.M32*b.M22 + a.M33*b.M32,
                a.M31*b.M13 + a.M32*b.M23 + a.M33*b.M33);
        }

        /// <summary>
        /// Product of two matrices.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 Multiply(Matrix3 a, Matrix3 b)
        {
            return new Matrix3(a.M11*b.M11 + a.M12*b.M21 + a.M13*b.M31, a.M11*b.M12 + a.M12*b.M22 + a.M13*b.M32, a.M11*b.M13 + a.M12*b.M23 + a.M13*b.M33, a.M21*b.M11 + a.M22*b.M21 + a.M23*b.M31,
                a.M21*b.M12 + a.M22*b.M22 + a.M23*b.M32, a.M21*b.M13 + a.M22*b.M23 + a.M23*b.M33, a.M31*b.M11 + a.M32*b.M21 + a.M33*b.M31, a.M31*b.M12 + a.M32*b.M22 + a.M33*b.M32,
                a.M31*b.M13 + a.M32*b.M23 + a.M33*b.M33);
        }

        /// <summary>
        /// Product of a matrix with a vector.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="u">Vector3d.</param>
        /// <returns>Matrix3.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Vector3 operator *(Matrix3 a, Vector3 u)
        {
            return new Vector3(a.M11*u.X + a.M12*u.Y + a.M13*u.Z, a.M21*u.X + a.M22*u.Y + a.M23*u.Z, a.M31*u.X + a.M32*u.Y + a.M33*u.Z);
        }

        /// <summary>
        /// Product of a matrix with a vector.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="u">Vector3d.</param>
        /// <returns>Matrix3.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Vector3 Multiply(Matrix3 a, Vector3 u)
        {
            return new Vector3(a.M11*u.X + a.M12*u.Y + a.M13*u.Z, a.M21*u.X + a.M22*u.Y + a.M23*u.Z, a.M31*u.X + a.M32*u.Y + a.M33*u.Z);
        }

        /// <summary>
        /// Product of a matrix with a scalar.
        /// </summary>
        /// <param name="m">Matrix3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 operator *(Matrix3 m, double a)
        {
            return new Matrix3(m.M11*a, m.M12*a, m.M13*a, m.M21*a, m.M22*a, m.M23*a, m.M31*a, m.M32*a, m.M33*a);
        }

        /// <summary>
        /// Product of a matrix with a scalar.
        /// </summary>
        /// <param name="m">Matrix3.</param>
        /// <param name="a">Scalar.</param>
        /// <returns>Matrix3.</returns>
        public static Matrix3 Multiply(Matrix3 m, double a)
        {
            return new Matrix3(m.M11*a, m.M12*a, m.M13*a, m.M21*a, m.M22*a, m.M23*a, m.M31*a, m.M32*a, m.M33*a);
        }

        /// <summary>
        /// Check if the components of two matrices are equal.
        /// </summary>
        /// <param name="u">Matrix3.</param>
        /// <param name="v">Matrix3.</param>
        /// <returns>True if the matrix components are equal or false in any other case.</returns>
        public static bool operator ==(Matrix3 u, Matrix3 v)
        {
            return Equals(u, v);
        }

        /// <summary>
        /// Check if the components of two matrices are different.
        /// </summary>
        /// <param name="u">Matrix3.</param>
        /// <param name="v">Matrix3.</param>
        /// <returns>True if the matrix components are different or false in any other case.</returns>
        public static bool operator !=(Matrix3 u, Matrix3 v)
        {
            return !Equals(u, v);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Calculate the determinant of the actual matrix.
        /// </summary>
        /// <returns>Determinant.</returns>
        public double Determinant()
        {
            return this.m11*this.m22*this.m33 +
                   this.m12*this.m23*this.m31 +
                   this.m13*this.m21*this.m32 -
                   this.m13*this.m22*this.m31 -
                   this.m11*this.m23*this.m32 -
                   this.m12*this.m21*this.m33;
        }

        /// <summary>
        /// Calculates the inverse matrix.
        /// </summary>
        /// <returns>Inverse Matrix3.</returns>
        public Matrix3 Inverse()
        {
            double det = this.Determinant();
            if (MathHelper.IsZero(det))
                throw new ArithmeticException("The matrix is not invertible.");

            det = 1/det;

            return new Matrix3(
                det*(this.m22*this.m33 - this.m23*this.m32),
                det*(this.m13*this.m32 - this.m12*this.m33),
                det*(this.m12*this.m23 - this.m13*this.m22),
                det*(this.m23*this.m31 - this.m21*this.m33),
                det*(this.m11*this.m33 - this.m13*this.m31),
                det*(this.m13*this.m21 - this.m11*this.m23),
                det*(this.m21*this.m32 - this.m22*this.m31),
                det*(this.m12*this.m31 - this.m11*this.m32),
                det*(this.m11*this.m22 - this.m12*this.m21)
                );
        }

        /// <summary>
        /// Obtains the transpose matrix.
        /// </summary>
        /// <returns>Transpose matrix.</returns>
        public Matrix3 Transpose()
        {
            return new Matrix3(this.m11, this.m21, this.m31, this.m12, this.m22, this.m32, this.m13, this.m23, this.m33);
        }

        #endregion

        #region static methods

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationX(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(1, 0, 0, 0, cos, -sin, 0, sin, cos);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationY(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(cos, 0, sin, 0, 1, 0, -sin, 0, cos);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting Matrix3 instance.</returns>
        /// <remarks>Matrix3 adopts the convention of using column vectors to represent three dimensional points.</remarks>
        public static Matrix3 RotationZ(double angle)
        {
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            return new Matrix3(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="value">Single scale factor for x, y, and z axis.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(double value)
        {
            return Scale(value, value, value);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="value">Scale factors for x, y, and z axis.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(Vector3 value)
        {
            return Scale(value.X, value.Y, value.Z);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="x">Scale factor for x-axis.</param>
        /// <param name="y">Scale factor for y-axis.</param>
        /// <param name="z">Scale factor for z-axis.</param>
        /// <returns>A scaling matrix.</returns>
        public static Matrix3 Scale(double x, double y, double z)
        {
            return new Matrix3(x, 0, 0, 0, y, 0, 0, 0, z);
        }

        #endregion

        #region comparison methods

        /// <summary>
        /// Check if the components of two matrices are approximate equal.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <returns>True if the matrix components are almost equal or false in any other case.</returns>
        public static bool Equals(Matrix3 a, Matrix3 b)
        {
            return a.Equals(b, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two matrices are approximate equal.
        /// </summary>
        /// <param name="a">Matrix3.</param>
        /// <param name="b">Matrix3.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the matrix components are almost equal or false in any other case.</returns>
        public static bool Equals(Matrix3 a, Matrix3 b, double threshold)
        {
            return a.Equals(b, threshold);
        }

        /// <summary>
        /// Check if the components of two matrices are approximate equal.
        /// </summary>
        /// <param name="other">Matrix3.</param>
        /// <returns>True if the matrix components are almost equal or false in any other case.</returns>
        public bool Equals(Matrix3 other)
        {
            return this.Equals(other, MathHelper.Epsilon);
        }

        /// <summary>
        /// Check if the components of two matrices are approximate equal.
        /// </summary>
        /// <param name="obj">Matrix3.</param>
        /// <param name="threshold">Maximum tolerance.</param>
        /// <returns>True if the matrix components are almost equal or false in any other case.</returns>
        public bool Equals(Matrix3 obj, double threshold)
        {
            return
                MathHelper.IsEqual(obj.M11, this.M11, threshold) &&
                MathHelper.IsEqual(obj.M12, this.M12, threshold) &&
                MathHelper.IsEqual(obj.M13, this.M13, threshold) &&
                MathHelper.IsEqual(obj.M21, this.M21, threshold) &&
                MathHelper.IsEqual(obj.M22, this.M22, threshold) &&
                MathHelper.IsEqual(obj.M23, this.M23, threshold) &&
                MathHelper.IsEqual(obj.M31, this.M31, threshold) &&
                MathHelper.IsEqual(obj.M32, this.M32, threshold) &&
                MathHelper.IsEqual(obj.M33, this.M33, threshold);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>True if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix3)
                return this.Equals((Matrix3) obj);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return
                this.M11.GetHashCode() ^ this.M12.GetHashCode() ^ this.M13.GetHashCode() ^
                this.M21.GetHashCode() ^ this.M22.GetHashCode() ^ this.M23.GetHashCode() ^
                this.M31.GetHashCode() ^ this.M32.GetHashCode() ^ this.M33.GetHashCode();
        }

        #endregion

        #region overrides

        /// <summary>
        /// Obtains a string that represents the matrix.
        /// </summary>
        /// <returns>A string text.</returns>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, this.m11, this.m12, this.m13, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, this.m21, this.m22, this.m23, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            s.Append(string.Format("|{0}{3} {1}{3} {2}|" + Environment.NewLine, this.m31, this.m32, this.m33, Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator));
            return s.ToString();
        }

        /// <summary>
        /// Obtains a string that represents the matrix.
        /// </summary>
        /// <param name="provider">An IFormatProvider interface implementation that supplies culture-specific formatting information. </param>
        /// <returns>A string text.</returns>
        public string ToString(IFormatProvider provider)
        {
            string separator = Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
            StringBuilder s = new StringBuilder();
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.m11.ToString(provider), this.m12.ToString(provider), this.m13.ToString(provider), separator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.m21.ToString(provider), this.m22.ToString(provider), this.m23.ToString(provider), separator));
            s.Append(string.Format("|{0}{3}{1}{3}{2}|" + Environment.NewLine, this.m31.ToString(provider), this.m32.ToString(provider), this.m33.ToString(provider), separator));
            return s.ToString();
        }

        #endregion
    }
}