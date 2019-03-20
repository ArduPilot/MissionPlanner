// Copyright 2005 - 2009 - Morten Nielsen (www.sharpgis.net)
//
// This file is part of ProjNet.
// ProjNet is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// ProjNet is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with ProjNet; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems.Transformations
{
    /// <summary>
    /// Represents affine math transform which ttransform input coordinates to target using affine transformation matrix. Dimensionality might change.
    /// </summary>
    ///<remarks>If the transform's input dimension is M, and output dimension is N, then the matrix will have size [N+1][M+1].
    ///The +1 in the matrix dimensions allows the matrix to do a shift, as well as a rotation.
    ///The [M][j] element of the matrix will be the j'th ordinate of the moved origin.
    ///The [i][N] element of the matrix will be 0 for i less than M, and 1 for i equals M.</remarks>
    /// <seealso href="http://en.wikipedia.org/wiki/Affine_transformation"/>
#if HAS_SYSTEM_SERIALIZABLEATTRIBUTE
    [Serializable]
#endif
    public class AffineTransform : MathTransform
    {
        #region class variables
        /// <summary>
        /// Saved inverse transform
        /// </summary>
        private IMathTransform _inverse;

        /// <summary>
        /// Dimension of source points - it's related to number of transformation matrix rows
        /// </summary>
        private readonly int dimSource;

        /// <summary>
        /// Dimension of output points - it's related to number of columns
        /// </summary>
        private readonly int dimTarget;

        /// <summary>
        /// Represents transform matrix of this affine transformation from input points to output ones using dimensionality defined within the affine transform
        /// Number of rows = dimTarget + 1
        /// Number of columns = dimSource + 1
        /// </summary>
        private readonly double[,] transformMatrix;
        #endregion class variables

        #region constructors & finalizers
        /// <summary>
        /// Creates instance of 2D affine transform (source dimensionality 2, target dimensionality 2) using the specified values
        /// </summary>
        /// <param name="m00">Value for row 0, column 0 - AKA ScaleX</param>
        /// <param name="m01">Value for row 0, column 1 - AKA ShearX</param>
        /// <param name="m02">Value for row 0, column 2 - AKA Translate X</param>
        /// <param name="m10">Value for row 1, column 0 - AKA Shear Y</param>
        /// <param name="m11">Value for row 1, column 1 - AKA Scale Y</param>
        /// <param name="m12">Value for row 1, column 2 - AKA Translate Y</param>
        public AffineTransform (double m00, double m01, double m02, double m10, double m11, double m12)
            : base ()
        {
            //fill dimensionlity
            dimSource = 2;
            dimTarget = 2;
            //create matrix - 2D affine transform uses 3x3 matrix (3rd row is the special one)
            transformMatrix = new double[3, 3] {{m00, m01, m02}, {m10, m11, m12}, {0, 0, 1}};
        }

        /// <summary>
        /// Creates instance of affine transform using the specified matrix. 
        /// </summary>
        /// <remarks>If the transform's input dimension is M, and output dimension is N, then the matrix will have size [N+1][M+1].
        /// The +1 in the matrix dimensions allows the matrix to do a shift, as well as a rotation. The [M][j] element of the matrix will be the j'th ordinate of the moved origin. The [i][N] element of the matrix will be 0 for i less than M, and 1 for i equals M.</remarks>
        ///
        /// <param name="matrix">Matrix used to create afiine transform</param>
        public AffineTransform (double[,] matrix)
            : base ()
        {
            //check validity
            if (matrix == null)
            {
                throw new ArgumentNullException ("matrix");
            }
            if (matrix.GetLength (0) <= 1)
            {
                throw new ArgumentException ("Transformation matrix must have at least 2 rows.");
            }
            if (matrix.GetLength (1) <= 1)
            {
                throw new ArgumentException ("Transformation matrix must have at least 2 columns.");
            }

            //fill dimensionlity - dimension is M, and output dimension is N, then the matrix will have size [N+1][M+1].
            dimSource = matrix.GetLength (1) - 1;
            dimTarget = matrix.GetLength (0) - 1;
            //use specified matrix
            transformMatrix = matrix;
        }
        #endregion constructors & finalizers

        #region public properties
        /// <summary>
        /// Gets a Well-Known text representation of this affine math transformation.
        /// </summary>
        /// <value></value>
        public override string WKT
        {
            get
            {
                //PARAM_MT["Affine",
                //    PARAMETER["num_row",3],
                //    PARAMETER["num_col",3],
                //    PARAMETER["elt_0_1",1],
                //    PARAMETER["elt_0_2",2],
                //    PARAMETER["elt 1 2",3]]

                var sb = new StringBuilder ();

                sb.Append ("PARAM_MT[\"Affine\"");
                //append parameters
                foreach (ProjectionParameter param in this.GetParameterValues ())
                {
                    sb.Append (",");
                    sb.Append (param.WKT);
                }
                sb.Append ("]");
                return sb.ToString ();
            }
        }
        /// <summary>
        /// Gets an XML representation of this affine transformation.
        /// </summary>
        /// <value></value>
        public override string XML
        {
            get { throw new NotImplementedException ("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Gets the dimension of input points.
        /// </summary>
        public override int DimSource { get { return dimSource; } }

        /// <summary>
        /// Gets the dimension of output points.
        /// </summary>
        public override int DimTarget { get { return dimTarget; } }
        #endregion public properties

        #region private methods

        /// <summary>
        /// Return affine transformation matrix as group of parameter values that maiy be used for retrieving WKT of this affine transform
        /// </summary>
        /// <returns>List of string pairs NAME VALUE</returns>
        private IList<ProjectionParameter> GetParameterValues () 
        {
            int rowCnt =transformMatrix.GetLength (0);
            int colCnt =transformMatrix.GetLength (1);
            var pInfo = new List<ProjectionParameter> ();
            pInfo.Add (new ProjectionParameter ("num_row", rowCnt));
            pInfo.Add (new ProjectionParameter ("num_col", colCnt));
            //fill matrix values
            for (int row = 0; row < rowCnt; row++)
            {
                for (int col = 0; col < colCnt; col++)
                {
                    string name = string.Format (CultureInfo.InvariantCulture.NumberFormat, "elt_{0}_{1}", row, col);
                    pInfo.Add (new ProjectionParameter (name, transformMatrix[row, col]));
                }
            }
            return pInfo;
        }


        /// <summary>
        /// Given L,U,P and b solve for x.
        /// Input the L and U matrices as a single matrix LU.
        /// Return the solution as a double[].
        /// LU will be a n+1xm+1 matrix where the first row and columns are zero.
        /// This is for ease of computation and consistency with Cormen et al.
        /// pseudocode.
        /// The pi array represents the permutation matrix.
        /// </summary>
        /// <seealso href="http://www.rkinteractive.com/blogs/SoftwareDevelopment/post/2013/05/14/Algorithms-In-C-Solving-A-System-Of-Linear-Equations.aspx"/>
        /// <param name="LU"></param>
        /// <param name="pi"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double[] LUPSolve (double[,] LU, int[] pi, double[] b)
        {
            int n = LU.GetLength (0) - 1;
            double[] x = new double[n + 1];
            double[] y = new double[n + 1];
            double suml = 0;
            double sumu = 0;
            double lij = 0;

            /*
            * Solve for y using formward substitution
            * */
            for (int i = 0; i <= n; i++)
            {
                suml = 0;
                for (int j = 0; j <= i - 1; j++)
                {
                    /*
                    * Since we've taken L and U as a singular matrix as an input
                    * the value for L at index i and j will be 1 when i equals j, not LU[i][j], since
                    * the diagonal values are all 1 for L.
                    * */
                    if (i == j)
                    {
                        lij = 1;
                    }
                    else
                    {
                        lij = LU[i, j];
                    }
                    suml = suml + (lij * y[j]);
                }
                y[i] = b[pi[i]] - suml;
            }
            //Solve for x by using back substitution
            for (int i = n; i >= 0; i--)
            {
                sumu = 0;
                for (int j = i + 1; j <= n; j++)
                {
                    sumu = sumu + (LU[i, j] * x[j]);
                }
                x[i] = (y[i] - sumu) / LU[i, i];
            }
            return x;
        }

        /// <summary>
        /// Perform LUP decomposition on a matrix A.
        /// Return P as an array of ints and L and U are just in A, "in place".
        /// In order to make some of the calculations more straight forward and to 
        /// match Cormen's et al. pseudocode the matrix A should have its first row and first columns
        /// to be all 0.
        /// </summary>
        /// <seealso href="http://www.rkinteractive.com/blogs/SoftwareDevelopment/post/2013/05/07/Algorithms-In-C-LUP-Decomposition.aspx"/>
        /// <param name="A"></param>
        /// <returns></returns>
        private static int[] LUPDecomposition (double[,] A)
        {
            int n = A.GetLength (0) - 1;
            /*
            * pi represents the permutation matrix.  We implement it as an array
            * whose value indicates which column the 1 would appear.  We use it to avoid 
            * dividing by zero or small numbers.
            * */
            int[] pi = new int[n + 1];
            double p = 0;
            int kp = 0;
            int pik = 0;
            int pikp = 0;
            double aki = 0;
            double akpi = 0;

            //Initialize the permutation matrix, will be the identity matrix
            for (int j = 0; j <= n; j++)
            {
                pi[j] = j;
            }

            for (int k = 0; k <= n; k++)
            {
                /*
                * In finding the permutation matrix p that avoids dividing by zero
                * we take a slightly different approach.  For numerical stability
                * We find the element with the largest 
                * absolute value of those in the current first column (column k).  If all elements in
                * the current first column are zero then the matrix is singluar and throw an
                * error.
                * */
                p = 0;
                for (int i = k; i <= n; i++)
                {
                    if (Math.Abs (A[i, k]) > p)
                    {
                        p = Math.Abs (A[i, k]);
                        kp = i;
                    }
                }
                if (p == 0)
                {
                    throw new Exception ("singular matrix");
                }
                /*
                * These lines update the pivot array (which represents the pivot matrix)
                * by exchanging pi[k] and pi[kp].
                * */
                pik = pi[k];
                pikp = pi[kp];
                pi[k] = pikp;
                pi[kp] = pik;

                /*
                * Exchange rows k and kpi as determined by the pivot
                * */
                for (int i = 0; i <= n; i++)
                {
                    aki = A[k, i];
                    akpi = A[kp, i];
                    A[k, i] = akpi;
                    A[kp, i] = aki;
                }

                /*
                    * Compute the Schur complement
                    * */
                for (int i = k + 1; i <= n; i++)
                {
                    A[i, k] = A[i, k] / A[k, k];
                    for (int j = k + 1; j <= n; j++)
                    {
                        A[i, j] = A[i, j] - (A[i, k] * A[k, j]);
                    }
                }
            }
            return pi;
        }


        /// <summary>
        /// Given an nXn matrix A, solve n linear equations to find the inverse of A.
        /// </summary>
        /// <seealso href="http://www.rkinteractive.com/blogs/SoftwareDevelopment/post/2013/05/21/Algorithms-In-C-Finding-The-Inverse-Of-A-Matrix.aspx"/>
        /// <param name="A"></param>
        /// <returns></returns>
        private static double[,] InvertMatrix (double[,] A)
        {
            int n = A.GetLength (0);
            int m = A.GetLength (1);

            //x will hold the inverse matrix to be returned
            double[,] x = new double[n, m];

            /*
            * solve will contain the vector solution for the LUP decomposition as we solve
            * for each vector of x.  We will combine the solutions into the double[][] array x.
            * */
            double[] solve;

            //Get the LU matrix and P matrix (as an array)
            int[] P = LUPDecomposition (A);
            double[,] LU = A;

            /*
            * Solve AX = e for each column ei of the identity matrix using LUP decomposition
            * */
            for (int i = 0; i < n; i++)
            {
                //e will represent each column in the identity matrix
                double[] e = new double[m];
                e[i] = 1;
                solve = LUPSolve (LU, P, e);
                for (int j = 0; j < solve.Length; j++)
                {
                    x[j, i] = solve[j];
                }
            }
            return x;
        }
        #endregion private methods

        #region public methods
        /// <summary>
        /// Returns the inverse of this affine transformation.
        /// </summary>
        /// <returns>IMathTransform that is the reverse of the current affine transformation.</returns>
        public override IMathTransform Inverse ()
        {
            if (_inverse == null)
            {
                //find the inverse transformation matrix - use cloned matrix array
                //remarks about dimensionality: if input dimension is M, and output dimension is N, then the matrix will have size [N+1][M+1].
                var invMatrix = InvertMatrix ((double[,])transformMatrix.Clone ());
                _inverse = new AffineTransform (invMatrix);
            }

            return _inverse;
        }

        /// <summary>
        /// Transforms a coordinate point. The passed parameter point should not be modified.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double[] Transform (double[] point)
        {
            //check source dimensionality - alow coordinate clipping, if source dimensionality is greater then expected source dimensionality of affine transformation
            if (point.Length >= dimSource)
            {
                //use transformation matrix to create output points that has dimTarget dimensionality
                double[] transformed = new double[dimTarget];

                //count each target dimension using the apropriate row
                for (int row = 0; row < dimTarget; row++)
                {
                    //start with the last value which is in fact multiplied by 1
                    double dimVal = transformMatrix[row, dimSource];
                    for (int col = 0; col < dimSource; col++)
                    {
                        dimVal += transformMatrix[row, col] * point[col];
                    }
                    transformed[row] = dimVal;
                }
                return transformed;
            }

            //nepodporovane
            throw new NotSupportedException ("Dimensionality of point is not supported!");
        }

        /// <summary>
        /// Reverses the transformation
        /// </summary>
        public override void Invert ()
        {
            throw new NotImplementedException ("The method or operation is not implemented.");
        }


        /// <summary>
        /// Returns this affine transform as an affine transform matrix.
        /// </summary>
        /// <returns></returns>
        public double[,] GetMatrix ()
        {
            return (double[,])this.transformMatrix.Clone ();
        }
        #endregion public methods
    }
}
