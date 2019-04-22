namespace MissionPlanner.Utilities.Drawing
{
    // System.Drawing.Imaging.ColorMatrix
    using System;
    using System.Runtime.InteropServices;

    /// <summary>Defines a 5 x 5 matrix that contains the coordinates for the RGBAW space. Several methods of the <see cref="T:System.Drawing.Imaging.ImageAttributes" /> class adjust image colors by using a color matrix. This class cannot be inherited.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ColorMatrix
    {
        private float matrix00;

        private float matrix01;

        private float matrix02;

        private float matrix03;

        private float matrix04;

        private float matrix10;

        private float matrix11;

        private float matrix12;

        private float matrix13;

        private float matrix14;

        private float matrix20;

        private float matrix21;

        private float matrix22;

        private float matrix23;

        private float matrix24;

        private float matrix30;

        private float matrix31;

        private float matrix32;

        private float matrix33;

        private float matrix34;

        private float matrix40;

        private float matrix41;

        private float matrix42;

        private float matrix43;

        private float matrix44;

        /// <summary>Gets or sets the element at the 0 (zero) row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the 0 row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix00
        {
            get
            {
                return matrix00;
            }
            set
            {
                matrix00 = value;
            }
        }

        /// <summary>Gets or sets the element at the 0 (zero) row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the 0 row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" /> .</returns>
        public float Matrix01
        {
            get
            {
                return matrix01;
            }
            set
            {
                matrix01 = value;
            }
        }

        /// <summary>Gets or sets the element at the 0 (zero) row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the 0 row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix02
        {
            get
            {
                return matrix02;
            }
            set
            {
                matrix02 = value;
            }
        }

        /// <summary>Gets or sets the element at the 0 (zero) row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />. Represents the alpha component.</summary>
        /// <returns>The element at the 0 row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix03
        {
            get
            {
                return matrix03;
            }
            set
            {
                matrix03 = value;
            }
        }

        /// <summary>Gets or sets the element at the 0 (zero) row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the 0 row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix04
        {
            get
            {
                return matrix04;
            }
            set
            {
                matrix04 = value;
            }
        }

        /// <summary>Gets or sets the element at the first row and 0 (zero) column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the first row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix10
        {
            get
            {
                return matrix10;
            }
            set
            {
                matrix10 = value;
            }
        }

        /// <summary>Gets or sets the element at the first row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the first row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix11
        {
            get
            {
                return matrix11;
            }
            set
            {
                matrix11 = value;
            }
        }

        /// <summary>Gets or sets the element at the first row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the first row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix12
        {
            get
            {
                return matrix12;
            }
            set
            {
                matrix12 = value;
            }
        }

        /// <summary>Gets or sets the element at the first row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />. Represents the alpha component.</summary>
        /// <returns>The element at the first row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix13
        {
            get
            {
                return matrix13;
            }
            set
            {
                matrix13 = value;
            }
        }

        /// <summary>Gets or sets the element at the first row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the first row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix14
        {
            get
            {
                return matrix14;
            }
            set
            {
                matrix14 = value;
            }
        }

        /// <summary>Gets or sets the element at the second row and 0 (zero) column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the second row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix20
        {
            get
            {
                return matrix20;
            }
            set
            {
                matrix20 = value;
            }
        }

        /// <summary>Gets or sets the element at the second row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the second row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix21
        {
            get
            {
                return matrix21;
            }
            set
            {
                matrix21 = value;
            }
        }

        /// <summary>Gets or sets the element at the second row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the second row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix22
        {
            get
            {
                return matrix22;
            }
            set
            {
                matrix22 = value;
            }
        }

        /// <summary>Gets or sets the element at the second row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the second row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix23
        {
            get
            {
                return matrix23;
            }
            set
            {
                matrix23 = value;
            }
        }

        /// <summary>Gets or sets the element at the second row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the second row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix24
        {
            get
            {
                return matrix24;
            }
            set
            {
                matrix24 = value;
            }
        }

        /// <summary>Gets or sets the element at the third row and 0 (zero) column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the third row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix30
        {
            get
            {
                return matrix30;
            }
            set
            {
                matrix30 = value;
            }
        }

        /// <summary>Gets or sets the element at the third row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the third row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix31
        {
            get
            {
                return matrix31;
            }
            set
            {
                matrix31 = value;
            }
        }

        /// <summary>Gets or sets the element at the third row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the third row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix32
        {
            get
            {
                return matrix32;
            }
            set
            {
                matrix32 = value;
            }
        }

        /// <summary>Gets or sets the element at the third row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />. Represents the alpha component.</summary>
        /// <returns>The element at the third row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix33
        {
            get
            {
                return matrix33;
            }
            set
            {
                matrix33 = value;
            }
        }

        /// <summary>Gets or sets the element at the third row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the third row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix34
        {
            get
            {
                return matrix34;
            }
            set
            {
                matrix34 = value;
            }
        }

        /// <summary>Gets or sets the element at the fourth row and 0 (zero) column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the fourth row and 0 column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix40
        {
            get
            {
                return matrix40;
            }
            set
            {
                matrix40 = value;
            }
        }

        /// <summary>Gets or sets the element at the fourth row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the fourth row and first column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix41
        {
            get
            {
                return matrix41;
            }
            set
            {
                matrix41 = value;
            }
        }

        /// <summary>Gets or sets the element at the fourth row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the fourth row and second column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix42
        {
            get
            {
                return matrix42;
            }
            set
            {
                matrix42 = value;
            }
        }

        /// <summary>Gets or sets the element at the fourth row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />. Represents the alpha component.</summary>
        /// <returns>The element at the fourth row and third column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix43
        {
            get
            {
                return matrix43;
            }
            set
            {
                matrix43 = value;
            }
        }

        /// <summary>Gets or sets the element at the fourth row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <returns>The element at the fourth row and fourth column of this <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</returns>
        public float Matrix44
        {
            get
            {
                return matrix44;
            }
            set
            {
                matrix44 = value;
            }
        }

        /// <summary>Gets or sets the element at the specified row and column in the <see cref="T:System.Drawing.Imaging.ColorMatrix" />.</summary>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <returns>The element at the specified row and column.</returns>
        public float this[int row, int column]
        {
            get
            {
                return GetMatrix()[row][column];
            }
            set
            {
                float[][] matrix = GetMatrix();
                matrix[row][column] = value;
                SetMatrix(matrix);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Imaging.ColorMatrix" /> class.</summary>
        public ColorMatrix()
        {
            matrix00 = 1f;
            matrix11 = 1f;
            matrix22 = 1f;
            matrix33 = 1f;
            matrix44 = 1f;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Imaging.ColorMatrix" /> class using the elements in the specified matrix <paramref name="newColorMatrix" />.</summary>
        /// <param name="newColorMatrix">The values of the elements for the new <see cref="T:System.Drawing.Imaging.ColorMatrix" />. </param>
        [CLSCompliant(false)]
        public ColorMatrix(float[][] newColorMatrix)
        {
            SetMatrix(newColorMatrix);
        }

        internal void SetMatrix(float[][] newColorMatrix)
        {
            matrix00 = newColorMatrix[0][0];
            matrix01 = newColorMatrix[0][1];
            matrix02 = newColorMatrix[0][2];
            matrix03 = newColorMatrix[0][3];
            matrix04 = newColorMatrix[0][4];
            matrix10 = newColorMatrix[1][0];
            matrix11 = newColorMatrix[1][1];
            matrix12 = newColorMatrix[1][2];
            matrix13 = newColorMatrix[1][3];
            matrix14 = newColorMatrix[1][4];
            matrix20 = newColorMatrix[2][0];
            matrix21 = newColorMatrix[2][1];
            matrix22 = newColorMatrix[2][2];
            matrix23 = newColorMatrix[2][3];
            matrix24 = newColorMatrix[2][4];
            matrix30 = newColorMatrix[3][0];
            matrix31 = newColorMatrix[3][1];
            matrix32 = newColorMatrix[3][2];
            matrix33 = newColorMatrix[3][3];
            matrix34 = newColorMatrix[3][4];
            matrix40 = newColorMatrix[4][0];
            matrix41 = newColorMatrix[4][1];
            matrix42 = newColorMatrix[4][2];
            matrix43 = newColorMatrix[4][3];
            matrix44 = newColorMatrix[4][4];
        }

        internal float[][] GetMatrix()
        {
            float[][] array = new float[5][];
            for (int i = 0; i < 5; i++)
            {
                array[i] = new float[5];
            }
            array[0][0] = matrix00;
            array[0][1] = matrix01;
            array[0][2] = matrix02;
            array[0][3] = matrix03;
            array[0][4] = matrix04;
            array[1][0] = matrix10;
            array[1][1] = matrix11;
            array[1][2] = matrix12;
            array[1][3] = matrix13;
            array[1][4] = matrix14;
            array[2][0] = matrix20;
            array[2][1] = matrix21;
            array[2][2] = matrix22;
            array[2][3] = matrix23;
            array[2][4] = matrix24;
            array[3][0] = matrix30;
            array[3][1] = matrix31;
            array[3][2] = matrix32;
            array[3][3] = matrix33;
            array[3][4] = matrix34;
            array[4][0] = matrix40;
            array[4][1] = matrix41;
            array[4][2] = matrix42;
            array[4][3] = matrix43;
            array[4][4] = matrix44;
            return array;
        }
    }
}
