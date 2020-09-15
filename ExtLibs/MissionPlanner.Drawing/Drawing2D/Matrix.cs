//
// System.Drawing.Drawing2D.Matrix.cs
//
// Authors:
//   Stefan Maierhofer <sm@cg.tuwien.ac.at>
//   Dennis Hayes (dennish@Raytek.com)
//   Duncan Mak (duncan@ximian.com)
//   Ravindra (rkumar@novell.com)
//
// (C) Ximian, Inc.  http://www.ximian.com
// Copyright (C) 2004, 2006 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace System.Drawing.Drawing2D
{
    public sealed class Matrix : MarshalByRefObject, IDisposable
    {
        public void Dispose()
        {
        }

        [Flags]
        internal enum MatrixTypes
        {
            TRANSFORM_IS_IDENTITY = 0x0,
            TRANSFORM_IS_TRANSLATION = 0x1,
            TRANSFORM_IS_SCALING = 0x2,
            TRANSFORM_IS_UNKNOWN = 0x4
        }

        public Matrix()
        {
            Reset();
        }

        public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            //_padding = 0;
            DeriveMatrixType();
        }

        private void DeriveMatrixType()
        {
            _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
            if (_m21 != 0.0 || _m12 != 0.0)
            {
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                return;
            }

            if (_m11 != 1.0 || _m22 != 1.0)
            {
                _type = MatrixTypes.TRANSFORM_IS_SCALING;
            }

            if (_offsetX != 0.0 || _offsetY != 0.0)
            {
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }

            if ((_type & (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING)) == 0)
            {
                _type = MatrixTypes.TRANSFORM_IS_IDENTITY;
            }
        }

        public void RotateAt(double angle, PointF center)
        {
            RotateAt(angle, center.X, center.Y);
        }

        public void RotateAt(double angle, double centerX, double centerY)
        {
            angle %= 360.0;
            var middle = this * CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY);
            this.SetMatrix(middle.M11, middle.M12, middle.M21, middle.M22, middle.OffsetX, middle.OffsetY,
                middle._type);
        }

        public void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle %= 360.0;
            var middle = CreateRotationRadians(angle * (Math.PI / 180.0), centerX, centerY) * this;
            this.SetMatrix(middle.M11, middle.M12, middle.M21, middle.M22, middle.OffsetX, middle.OffsetY,
                middle._type);
        }

        public void RotatePrepend(double angle)
        {
            angle %= 360.0;
            var middle = CreateRotationRadians(angle * (Math.PI / 180.0)) * this;
            this.SetMatrix(middle.M11, middle.M12, middle.M21, middle.M22, middle.OffsetX, middle.OffsetY,
                middle._type);
        }

        internal static Matrix CreateRotationRadians(double angle)
        {
            return CreateRotationRadians(angle, 0.0, 0.0);
        }


        internal static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            Matrix result = new Matrix();
            double num = Math.Sin(angle);
            double num2 = Math.Cos(angle);
            double offsetX = centerX * (1.0 - num2) + centerY * num;
            double offsetY = centerY * (1.0 - num2) - centerX * num;
            result.SetMatrix(num2, num, 0.0 - num, num2, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_UNKNOWN);
            return result;
        }

        internal static Matrix CreateTranslation(double offsetX, double offsetY)
        {
            Matrix result = new Matrix();
            result.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
            return result;
        }

        public void TranslatePrepend(double offsetX, double offsetY)
        {
            var middle = CreateTranslation(offsetX, offsetY) * this;
            this.SetMatrix(middle.M11, middle.M12, middle.M21, middle.M22, middle.OffsetX, middle.OffsetY,
                middle._type);
        }

        public void TransformPoints(Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var output = points[i];
                    var X = (double) output.X;
                    var Y = (double) output.Y;
                    MultiplyPoint(ref X, ref Y);
                    output.X = (int) X;
                    output.Y = (int) Y;
                }
            }
        }

        public void TransformPoints(PointF[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var output = points[i];
                    var X = (double) output.X;
                    var Y = (double) output.Y;
                    MultiplyPoint(ref X, ref Y);
                    output.X = (float) X;
                    output.Y = (float) Y;
                }
            }
        }

        public void TransformPoints(List<PointF> points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    var output = points[i];
                    var X = (double) output.X;
                    var Y = (double) output.Y;
                    MultiplyPoint(ref X, ref Y);
                    output.X = (float) X;
                    output.Y = (float) Y;
                }
            }
        }

        internal void MultiplyPoint(ref double x, ref double y)
        {
            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x += _offsetX;
                    y += _offsetY;
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    x *= _m11;
                    y *= _m22;
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
                    x *= _m11;
                    x += _offsetX;
                    y *= _m22;
                    y += _offsetY;
                    break;
                default:
                {
                    double num = y * _m21 + _offsetX;
                    double num2 = x * _m12 + _offsetY;
                    x *= _m11;
                    x += num;
                    y *= _m22;
                    y += num2;
                    break;
                }
            }
        }

        public static Matrix operator *(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        private bool IsDistinguishedIdentity => _type == MatrixTypes.TRANSFORM_IS_IDENTITY;

        public bool IsIdentity
        {
            get
            {
                if (_type != 0)
                {
                    if (_m11 == 1.0 && _m12 == 0.0 && _m21 == 0.0 && _m22 == 1.0 && _offsetX == 0.0)
                    {
                        return _offsetY == 0.0;
                    }

                    return false;
                }

                return true;
            }
        }

        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            // both are null
            if (matrix1 is null && matrix2 is null)
                return true;

            // only one is null
            if (matrix1 is null || matrix2 is null)
                return false;

            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return matrix1.IsIdentity == matrix2.IsIdentity;
            }

            if (matrix1.M11 == matrix2.M11 && matrix1.M12 == matrix2.M12 && matrix1.M21 == matrix2.M21 &&
                matrix1.M22 == matrix2.M22 && matrix1.OffsetX == matrix2.OffsetX)
            {
                return matrix1.OffsetY == matrix2.OffsetY;
            }

            return false;
        }

        internal static class MatrixUtil
        {

            internal static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
            {
                MatrixTypes type = matrix1._type;
                MatrixTypes type2 = matrix2._type;
                if (type2 == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return;
                }

                if (type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    matrix1 = matrix2;
                    return;
                }

                if (type2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    matrix1._offsetX += matrix2._offsetX;
                    matrix1._offsetY += matrix2._offsetY;
                    if (type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }

                    return;
                }

                if (type == MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    double offsetX = matrix1._offsetX;
                    double offsetY = matrix1._offsetY;
                    matrix1 = matrix2;
                    matrix1._offsetX = offsetX * matrix2._m11 + offsetY * matrix2._m21 + matrix2._offsetX;
                    matrix1._offsetY = offsetX * matrix2._m12 + offsetY * matrix2._m22 + matrix2._offsetY;
                    if (type2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                    }
                    else
                    {
                        matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
                    }

                    return;
                }

                switch (((int) type << 4) | (int) type2)
                {
                    case 34:
                        matrix1._m11 *= matrix2._m11;
                        matrix1._m22 *= matrix2._m22;
                        break;
                    case 35:
                        matrix1._m11 *= matrix2._m11;
                        matrix1._m22 *= matrix2._m22;
                        matrix1._offsetX = matrix2._offsetX;
                        matrix1._offsetY = matrix2._offsetY;
                        matrix1._type = (MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING);
                        break;
                    case 50:
                        matrix1._m11 *= matrix2._m11;
                        matrix1._m22 *= matrix2._m22;
                        matrix1._offsetX *= matrix2._m11;
                        matrix1._offsetY *= matrix2._m22;
                        break;
                    case 51:
                        matrix1._m11 *= matrix2._m11;
                        matrix1._m22 *= matrix2._m22;
                        matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                        matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                        break;
                    case 36:
                    case 52:
                    case 66:
                    case 67:
                    case 68:
                        matrix1 = new Matrix(matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                            matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,
                            matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                            matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,
                            matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                            matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                        break;
                }
            }

            internal static void PrependOffset(ref Matrix matrix, double offsetX, double offsetY)
            {
                if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    matrix = new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
                    matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    return;
                }

                matrix._offsetX += matrix._m11 * offsetX + matrix._m21 * offsetY;
                matrix._offsetY += matrix._m12 * offsetX + matrix._m22 * offsetY;
                if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }



        public void Translate(double offsetX, double offsetY)
        {
            if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else if (_type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
            }
            else
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
                _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }
        }

        private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY,
            MatrixTypes type)
        {
            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _type = type;
        }

        internal double _m11;

        internal double _m12;

        internal double _m21;

        internal double _m22;

        internal double _offsetX;

        internal double _offsetY;

        internal MatrixTypes _type;

        /// <summary>Gets or sets the value of the first row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
        /// <returns>The value of the first row and first column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 1.</returns>
        public float M11
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0f;
                }

                return (float) _m11;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
                    return;
                }

                _m11 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }

        /// <summary> Gets or sets the value of the first row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
        /// <returns>The value of the first row and second column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 0.</returns>
        public float M12
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0f;
                }

                return (float) _m12;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
                    return;
                }

                _m12 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }

        /// <summary> Gets or sets the value of the second row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure.</summary>
        /// <returns>The value of the second row and first column of this <see cref="T:System.Windows.Media.Matrix" />. The default value is 0.</returns>
        public float M21
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0f;
                }

                return (float) _m21;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
                    return;
                }

                _m21 = value;
                _type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
        }

        /// <summary>Gets or sets the value of the second row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
        /// <returns>The value of the second row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 1.</returns>
        public float M22
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0f;
                }

                return (float) _m22;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
                    return;
                }

                _m22 = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_SCALING;
                }
            }
        }

        /// <summary>Gets or sets the value of the third row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure.  </summary>
        /// <returns>The value of the third row and first column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 0.</returns>
        public float OffsetX
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0f;
                }

                return (float) _offsetX;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, MatrixTypes.TRANSFORM_IS_TRANSLATION);
                    return;
                }

                _offsetX = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }

        /// <summary>Gets or sets the value of the third row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. </summary>
        /// <returns>The value of the third row and second column of this <see cref="T:System.Windows.Media.Matrix" /> structure. The default value is 0.</returns>
        public float OffsetY
        {
            get
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0f;
                }

                return (float) _offsetY;
            }
            set
            {
                if (_type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, MatrixTypes.TRANSFORM_IS_TRANSLATION);
                    return;
                }

                _offsetY = value;
                if (_type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    _type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }

        private SKMatrix matrix;

        public float[] Data
        {
            get { return matrix.Values; }
            set
            {
                matrix = new SKMatrix() {Values = value};
                SetMatrix(matrix.ScaleX, matrix.SkewX,  matrix.SkewY,matrix.ScaleY, matrix.TransX, matrix.TransY,
                    MatrixTypes.TRANSFORM_IS_UNKNOWN);
            }
        }

        public double Determinant
        {
            get
            {
                switch (_type)
                {
                    case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return 1.0;
                    case MatrixTypes.TRANSFORM_IS_SCALING:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
                        return _m11 * _m22;
                    default:
                        return _m11 * _m22 - _m12 * _m21;
                }
            }
        }

        public static bool IsZero(double value)
        {
            return Math.Abs(value) < 2.2204460492503131E-15;
        }

        public void Invert()
        {
            double determinant = Determinant;
            if (IsZero(determinant))
            {
                throw new InvalidOperationException("Transform_NotInvertible");
            }

            switch (_type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    break;
                case MatrixTypes.TRANSFORM_IS_SCALING:
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    _offsetX = 0.0 - _offsetX;
                    _offsetY = 0.0 - _offsetY;
                    break;
                case MatrixTypes.TRANSFORM_IS_TRANSLATION | MatrixTypes.TRANSFORM_IS_SCALING:
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                    _offsetX = (0.0 - _offsetX) * _m11;
                    _offsetY = (0.0 - _offsetY) * _m22;
                    break;
                default:
                {
                    double num = 1.0 / determinant;
                    SetMatrix(_m22 * num, (0.0 - _m12) * num, (0.0 - _m21) * num, _m11 * num,
                        (_m21 * _offsetY - _offsetX * _m22) * num, (_offsetX * _m12 - _m11 * _offsetY) * num,
                        MatrixTypes.TRANSFORM_IS_UNKNOWN);
                    break;
                }
            }
        }

        public void Reset()
        {
            SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_IDENTITY);
        }

        public void Scale(double scaleX, double scaleY)
        {
            var middle = this * CreateScaling(scaleX, scaleY);
            this.SetMatrix(middle.M11, middle.M12, middle.M21, middle.M22, middle.OffsetX, middle.OffsetY,
                middle._type);
        }

        internal static Matrix CreateScaling(double scaleX, double scaleY)
        {
            Matrix result = new Matrix();
            result.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
            return result;
        }


        public void Scale(float scale, float f, MatrixOrder append)
        {
            Scale(scale, f);
        }

        public void Rotate(float elementRotation, MatrixOrder append)
        {
            RotateAt(elementRotation, 0, 0);
        }

        public void Rotate(float elementRotation)
        {
            RotateAt(elementRotation, 0, 0);
        }

        public void Translate(float elementTranslationX, float elementTranslationY, MatrixOrder append)
        {
            Translate(elementTranslationX, elementTranslationY);
        }
    }
}