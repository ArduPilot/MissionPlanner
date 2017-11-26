///////////////////////////////////////////////////////////////////////////////
//
//  Matrix.cs
//
//  By Philip R. Braica (HoshiKata@aol.com, VeryMadSci@gmail.com)
//
//  Distributed under the The Code Project Open License (CPOL)
//  http://www.codeproject.com/info/cpol10.aspx
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Matrix math class, simple very basic.
    /// </summary>
    public class Matrix
    {
        #region Protected data.
        /// <summary>
        /// Columns
        /// </summary>
        protected int m_c = 0;

        /// <summary>
        /// Rows.
        /// </summary>
        protected int m_r = 0;
        #endregion

        /// <summary>
        /// Make an identity matrix
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public static Matrix MakeIdentity(int rank)
        {
            Matrix m = new Matrix(rank, rank);
            m.SetIdentity();
            return m;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Matrix()
        {
            Data = null;
        }

        /// <summary>
        /// Constructor with dimensions.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        public Matrix(int cols, int rows)
        {
            Resize(cols, rows);
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="m"></param>
        public Matrix(Matrix m)
        {
            Set(m);
        }

        /// <summary>
        /// Set this matric to m.
        /// </summary>
        /// <param name="m"></param>
        public void Set(Matrix m)
        {

            Resize(m.Columns, m.Rows);
            for (int i = 0; i < m.Data.Length; i++)
            {
                Data[i] = m.Data[i];
            }
        }

        /// <summary>
        /// Columns.
        /// </summary>
        public int Columns
        {
            get { return m_c; }
            set
            {
                Resize(value, m_r);                
            }
        }

        /// <summary>
        /// Rows
        /// </summary>
        public int Rows
        {
            get { return m_r; }
            set
            {
                Resize(m_c, value);                 
            }
        }

        /// <summary>
        /// Resize.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        public void Resize(int cols, int rows)
        {
            if ((m_c == cols) && (m_r == rows)) return;
            m_c = cols;
            m_r = rows;
            Data = new double[cols * rows];
            Zero();
        }

        /// <summary>
        /// Clone this matrix.
        /// </summary>
        /// <returns></returns>
        public Matrix Clone()
        {
            Matrix m = new Matrix();
            m.Resize(this.Columns, this.Rows);
            for (int i = 0; i < Data.Length; i++)
            {
                m.Data[i] = Data[i];
            }
            return m;
        }

        /// <summary>
        /// Get a value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double Get(int x, int y)
        {
            return Data[x + y * m_c];
        }


        /// <summary>
        /// Return the trace, same as Get(index, index);
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double Trace(int index)
        {
            return Get(index, index);
        }

        /// <summary>
        /// Set a value.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public void Set(int x, int y, double v)
        {
            Data[x + (y * m_c)] = v;
        }

        /// <summary>
        /// The raw data.
        /// </summary>
        public double[] Data { get; set; }

        /// <summary>
        /// In place scalar multiplication.
        /// this *= scalar.
        /// </summary>
        /// <param name="scalar"></param>
        public void Multiply(double scalar)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] *= scalar;
            }
        }

        /// <summary>
        /// Scalar multiplication.
        /// result = Multiply(matrix, scalar);
        /// </summary>
        /// <param name="scalar"></param>
        public static Matrix Multiply(Matrix m, double scalar)
        {
            Matrix rv = m.Clone();
            rv.Multiply(scalar);
            return rv;
        }

        /// <summary>
        /// Multiply two matrixes.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix Multiply(Matrix a, Matrix b)
        {
            Matrix rv = new Matrix(b.Columns, a.Rows);
            int min = a.Columns < b.Rows ? a.Columns : b.Rows;
            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < b.Columns; j++)
                {
                    double s = 0;
                    for (int k = 0; k < min; k++)
                    {
                        double av = a.Get(k, i);
                        double bv = b.Get(j, k);
                        s += av * bv;
                    }
                    rv.Set(j, i, s);
                }
            }
            return rv;
        }

        /// <summary>
        /// Multiply in place this * b.
        /// </summary>
        /// <param name="b"></param>
        public void Multiply(Matrix b)
        {
            Matrix tmp = Matrix.Multiply(this, b);
            this.Set(tmp);
        }

        /// <summary>
        /// Result = a*b*a^T.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Matrix MultiplyABAT(Matrix a, Matrix b)
        {
            Matrix rv = Multiply(a, b);
            Matrix t = Matrix.Transpose(a);
            rv.Multiply(t);
            return rv;
        }


        /// <summary>
        /// Add scalar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix a, double scalar)
        {
            Matrix rv = new Matrix(a);
            rv.Add(scalar);
            return rv;
        }

        /// <summary>
        /// Add scalar in place
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public void Add(double scalar)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] += scalar;
            }
        }

        /// <summary>
        /// Add matrix.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Add(Matrix a, Matrix b)
        {
            Matrix rv = new Matrix(a);
            rv.Add(b);
            return rv;
        }

        /// <summary>
        /// Add matrix in place
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public void Add(Matrix a)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] += a.Data[i];
            }
        }

        /// <summary>
        /// Add scalar.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix a, double scalar)
        {
            Matrix rv = new Matrix(a);
            rv.Subtract(scalar);
            return rv;
        }

        /// <summary>
        /// Add scalar in place
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public void Subtract(double scalar)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] -= scalar;
            }
        }

        /// <summary>
        /// Add matrix.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix Subtract(Matrix a, Matrix b)
        {
            Matrix rv = new Matrix(a);
            rv.Subtract(b);
            return rv;
        }

        /// <summary>
        /// Add matrix in place
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public void Subtract(Matrix a)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] -= a.Data[i];
            }
        }


        /// <summary>
        /// Transpose matrix m.
        /// </summary>
        public static Matrix Transpose(Matrix m)
        {
            Matrix rv = new Matrix(m.m_r, m.m_c);
            for (int i = 0; i < m.m_c; i++)
            {
                for (int j = 0; j < m.m_r; j++)
                {
                    rv.Set(j, i, m.Get(i, j));
                }
            }
            return rv;
        }

        /// <summary>
        /// Transpose this matrix in place.
        /// </summary>
        public void Transpose()
        {
            Matrix rv = new Matrix(this.m_r, this.m_c);
            for (int i = 0; i < m_c; i++)
            {
                for (int j = 0; j < m_r; j++)
                {
                    rv.Set(j, i, this.Get(i, j));
                }
            }
            this.Set(rv);
        }

        /// <summary>
        /// Test if this is an identity matrix.
        /// </summary>
        /// <returns></returns>
        public bool IsIdentity()
        {
            if (m_c != m_r) return false;
            int check = m_c + 1;
            int j = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                if (j == check)
                {
                    j = 0;
                    if (Data[i] != 1) return false;
                }
                else
                {
                    if (Data[i] != 0) return false;
                }
                j++;
            }
            return true;
        }

        /// <summary>
        /// Test if this is an identity matrix.
        /// </summary>
        /// <returns></returns>
        public void SetIdentity()
        {
            if (m_c != m_r) return;
            int check = m_c + 1;
            int j = 0;
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = (j == check) ? 1 : 0;
                j = j == check ? 1 : j + 1;
            }
        }

        /// <summary>
        /// Zero.
        /// </summary>
        public void Zero()
        {
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = 0;
            }
        }

        /// <summary>
        /// Determinant.
        /// </summary>
        public double Determinant
        {
            get
            {
                if (m_c != m_r) return 0;

                if (m_c == 0) return 0;
                if (m_c == 1) return Data[0];
                if (m_c == 2) return (Data[0] * Data[3]) - (Data[1] * Data[2]);
                if (m_c == 3) return
                    (Data[0] * ((Data[8] * Data[4]) - (Data[7] * Data[5]))) -
                    (Data[3] * ((Data[8] * Data[1]) - (Data[7] * Data[2]))) +
                    (Data[6] * ((Data[5] * Data[1]) - (Data[4] * Data[2])));

                // only supporting 1x1, 2x2 and 3x3
                return 0;
            }
        }

        /// <summary>
        /// Invert.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix Invert(Matrix m)
        {
            if (m.m_c != m.m_r) return null;
            double det = m.Determinant;
            if (det == 0) return null;

            Matrix rv = new Matrix(m);
            if (m.m_c == 1) rv.Data[0] = 1 / rv.Data[0];
            det = 1 / det;
            if (m.m_c == 2)
            {
                rv.Data[0] = det * m.Data[3];
                rv.Data[3] = det * m.Data[0];
                rv.Data[1] = -det * m.Data[2];
                rv.Data[2] = -det * m.Data[1];
            }
            if (m.m_c == 3)
            {
                rv.Data[0] = det * (m.Data[8] * m.Data[4]) - (m.Data[7] * m.Data[5]);
                rv.Data[1] = -det * (m.Data[8] * m.Data[1]) - (m.Data[7] * m.Data[2]);
                rv.Data[2] = det * (m.Data[5] * m.Data[1]) - (m.Data[4] * m.Data[2]);

                rv.Data[3] = -det * (m.Data[8] * m.Data[3]) - (m.Data[6] * m.Data[5]);
                rv.Data[4] = det * (m.Data[8] * m.Data[0]) - (m.Data[6] * m.Data[2]);
                rv.Data[5] = -det * (m.Data[5] * m.Data[0]) - (m.Data[3] * m.Data[2]);

                rv.Data[6] = det * (m.Data[7] * m.Data[3]) - (m.Data[6] * m.Data[4]);
                rv.Data[7] = -det * (m.Data[7] * m.Data[0]) - (m.Data[6] * m.Data[2]);
                rv.Data[8] = det * (m.Data[4] * m.Data[0]) - (m.Data[3] * m.Data[1]);
            }
            return rv;
        }
    }
}
