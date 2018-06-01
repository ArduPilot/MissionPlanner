
#region GPL License
/*
Copyright (c) 2010 Miguel Angel Guirado López

This file is part of CsAssortedWidgets.

    Trixion3D is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CsAssortedWidgets is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with CsAssortedWidgets.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion

namespace AssortedWidgets.Math3D
{
    public class Matrix4f
    {
        public float[] values = new float[16]{1, 0, 0, 0,   // Right        (values[0]..values[3]
                                              0, 1, 0, 0,   // Up           (values[4]..values[7]
                                              0, 0, 1, 0,   // Forward      (values[8]..values[11]
                                              0, 0, 0, 1};  // Translation  (values[12]..values[15]
        // Quaternion temporal
        //Quaternion4f tempQRot = new Quaternion4f();
        Vector3f tempV3 = new Vector3f();
        static Matrix4f auxMat4 = new Matrix4f();
        static Matrix4f resultadoMult = new Matrix4f();

        #region Constructor

        /// <summary>
        /// Crea una nueva instancia inicializada a la matriz identidad
        /// </summary>
        public Matrix4f() { }
        /// <summary>
        /// Crea una nueva instancia inicializados sus valores a los valores de la matriz parámetro
        /// </summary>
        /// <param Name="matrix"></param>
        public Matrix4f(Matrix4f matrix)
        {
            this.CopyFrom(matrix);
        }
        #endregion Constructor

        #region SetIdentity() & SetZero()

        public void SetIdentity()
        {
            values[00] = 1; values[01] = 0; values[02] = 0; values[03] = 0;
            values[04] = 0; values[05] = 1; values[06] = 0; values[07] = 0;
            values[08] = 0; values[09] = 0; values[10] = 1; values[11] = 0;
            values[12] = 0; values[13] = 0; values[14] = 0; values[15] = 1;
        }

        public void SetZero()
        {
            values[00] = 0; values[01] = 0; values[02] = 0; values[03] = 0;
            values[04] = 0; values[05] = 0; values[06] = 0; values[07] = 0;
            values[08] = 0; values[09] = 0; values[10] = 0; values[11] = 0;
            values[12] = 0; values[13] = 0; values[14] = 0; values[15] = 0;
        }
        #endregion SetIdentity() & SetZero()

        #region Propiedades

        #region Set-Get Vectores Unitarios (X, Y, Z)

        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector3f"/> que corresponde al vector unitario X de esta matriz
        /// Set: Establece los valores del vector unitario X de esta matriz
        /// </summary>
        public Vector3f Right3f
        {
            get
            {
                return new Vector3f(values[0], values[1], values[2]);
            }
            set
            {
                values[0] = value.X;
                values[1] = value.Y;
                values[2] = value.Z;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector3f"/> que corresponde al vector unitario Y de esta matriz
        /// Set: Establece los valores del vector unitario Y de esta matriz
        /// </summary>
        public Vector3f Up3f
        {
            get
            {
                return new Vector3f(values[4], values[5], values[6]);
            }
            set
            {
                values[4] = value.X;
                values[5] = value.Y;
                values[6] = value.Z;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector3f"/> que corresponde al vector unitario Z de esta matriz
        /// Set: Establece los valores del vector unitario Z de esta matriz
        /// </summary>
        public Vector3f Forward3f
        {
            get
            {
                return new Vector3f(values[8], values[9], values[10]);
            }
            set
            {
                values[8] = value.X;
                values[9] = value.Y;
                values[10] = value.Z;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector4f"/> que corresponde al vector unitario X de esta matriz
        /// Set: Establece los valores del vector unitario X de esta matriz
        /// </summary>
        public Vector4f Right4f
        {
            get
            {
                return new Vector4f(values[0], values[1], values[2], values[3]);
            }
            set
            {
                values[0] = value.X;
                values[1] = value.Y;
                values[2] = value.Z;
                values[3] = value.W;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector4f"/> que corresponde al vector unitario Y de esta matriz
        /// Set: Establece los valores del vector unitario Y de esta matriz
        /// </summary>
        public Vector4f Up4f
        {
            get
            {
                return new Vector4f(values[4], values[5], values[6], values[7]);
            }
            set
            {
                values[4] = value.X;
                values[5] = value.Y;
                values[6] = value.Z;
                values[7] = value.W;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector4f"/> que corresponde al vector unitario Z de esta matriz
        /// Set: Establece los valores del vector unitario Z de esta matriz
        /// </summary>
        public Vector4f Forward4f
        {
            get
            {
                return new Vector4f(values[8], values[9], values[10], values[11]);
            }
            set
            {
                values[8] = value.X;
                values[9] = value.Y;
                values[10] = value.Z;
                values[11] = value.W;
            }
        }
        #endregion Set-Get Vectores Unitarios (X, Y, Z)

        #region Set-Get Translation

        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector3f"/> que corresponde al vector de traslación de esta matriz
        /// Set: Establece los valores del vector traslación de esta matriz
        /// </summary>
        public Vector3f Translation3f
        {
            get
            {
                return new Vector3f(values[12], values[13], values[14]);
            }
            set
            {
                values[12] = value.X;
                values[13] = value.Y;
                values[14] = value.Z;
                values[15] = 1f;
            }
        }
        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector4f"/> que corresponde al vector de traslación de esta matriz
        /// Set: Establece los valores del vector traslación de esta matriz
        /// </summary>
        public Vector4f Translation4f
        {
            get
            {
                return new Vector4f(values[12], values[13], values[14], values[15]);
            }
            set
            {
                values[12] = value.X;
                values[13] = value.Y;
                values[14] = value.Z;
                values[15] = value.W;
            }
        }
        #endregion Set-Get Translation

        #region Set-Get SetScalation

        /// <summary>
        /// Get: Devuelve un nuevo <see cref="Vector3f"/> que corresponde al vector de escalado de esta matriz
        /// Set: Establece los valores del vector de escalado de esta matriz
        /// </summary>
        public Vector3f Scalation3f
        {
            get
            {
                return new Vector3f(values[0], values[5], values[10]);
            }
            set
            {
                values[0] = value.X;
                values[5] = value.Y;
                values[10] = value.Z;
            }
        }
        #endregion Set-Get SetScalation

        #endregion Propiedades
    
        /// <summary>
        /// Copia los valores de esta matriz desde la matriz pasada como parámetro
        /// </summary>
        /// <param Name="matrix"></param>
        public void CopyFrom(Matrix4f matrix)
        {
            matrix.values.CopyTo(values, 0);
        }
    }
}
