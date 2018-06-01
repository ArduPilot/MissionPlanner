
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

using OpenTK.Graphics.OpenGL;

namespace AssortedWidgets.Graphics
{
    public class Texture
    {
        protected int TextureId_ = -1;
        string _TextureName = String.Empty;
        bool _standardCoordSystem = false;
        public int SizeTexture = 2;
        public TextureEnvMode texEnvMode = TextureEnvMode.Modulate;

        public Texture(string fileNameWithoutExt)
        {
            _TextureName = fileNameWithoutExt;
        }
        public bool stdCoordSystem
        {
            get { return _standardCoordSystem; }
        }
        #region Propiedades

        public int TextureId
        {
            get { return TextureId_; }
        }

        public string TextureName
        {
            get { return _TextureName; }
        }
        #endregion Propiedades
        
        internal void LoadFromStream(Stream str)
        {
            Bitmap bitmap = new Bitmap(str);

            FromBitmap(bitmap);
        }

        public void LoadFromFile(string path)
        {
            Bitmap bitmap = new Bitmap(path + _TextureName);

            FromBitmap(bitmap);
        }

        public void FromBitmap(Bitmap bitmap)
        {
            GL.Enable(EnableCap.Texture2D);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            //if (volteaY)
            //bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            GL.GenTextures(1, out TextureId_);
            GL.BindTexture(TextureTarget.Texture2D, TextureId_);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat); // System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            SizeTexture = data.Width;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            bitmap = null;
        }
    }

    public class Texture2D : Texture
    {
        public Texture2D(string name)
            : base(name)
        {
        }
        public Texture2D(string name, TextFontTexture textFontTex)
            : base(name)
        {
            TextureId_ = textFontTex.TextureID;
            SizeTexture = textFontTex.TextureDim;
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, TextureId);
        }
    }
    /// <summary>
    /// Clase que maneja las fuentes de letras como texturas.
    /// El tamaño de la textura no puede ser superior a 48.
    /// (TextureFont)
    /// </summary>
    public class TextFontTexture
    {
        int TextureId_ = -1;
        Font _Font;
        protected int TextureDim_;
        System.Drawing.Graphics gfx;
        protected Bitmap bmp;
        //Form1 f1;
        int dim = 16;
        Glyph[] Glyphs = new Glyph[256];
        int cellDim = 0;

        string str = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNÑOPQRSTUVWXYZ[\\]_^`abcdefghijklmnñopqrstuvwxyz{|}~áéíóú¿¡";

        public TextFontTexture(Font pFont)
        {
            if (pFont == null)
                throw new ArgumentNullException("font", "Argument to TextureFont constructor cannot be null.");
            if (pFont.Size > 48)
                throw new ArgumentException("El tamaño de la textura no puede ser superior a 48.");

            this._Font = pFont;
            drawCharacters(str);
            CreateGLTexture();
        }

        /// <summary>
        /// Altura en puntos de la fuente
        /// </summary>
        public int HeightFont
        {
            get
            {
                return _Font.Height;
            }
        }

        /// <summary>
        /// id opengl de la textura
        /// </summary>
        public int TextureID
        {
            get
            {
                return TextureId_;
            }
        }

        /// <summary>
        /// Ancho de la fuente en puntos
        /// </summary>
        public Font Font
        {
            get
            {
                return _Font;
            }
        }

        /// <summary>
        /// Ancho = alto = TextureDim, en pixels de la textura
        /// </summary>
        public int TextureDim
        {
            get
            {
                return TextureDim_;
            }
        }

        internal Glyph this[int numGlyph]
        {
            get
            {
                if (numGlyph <= 255)
                {
                    return Glyphs[numGlyph];
                }
                else
                    throw new ArgumentOutOfRangeException("numGlyph, en TextureFont[int numGlyph]",
                        "numGlyph debe estar comprendido entre 0 alto 255");
            }
        }

        /// <summary>
        /// Rellena la textura con los dibujos de las letras
        /// </summary>
        /// <param Name_="pStr"></param>
        void drawCharacters(string pStr)
        {
            int xcont = 0;
            int ycont = 0;

            // We want a power-of-two Size
            // that is less than 1024 (supported in Geforce256-era cards), but large
            // enough to hold at least 256 (16*16) font glyphs.
            // TODO: Find the actual card limits, maybe?
            int Size = (int)((_Font.Size + 2) * dim);
            Size = (int)System.Math.Pow(2.0, System.Math.Ceiling(System.Math.Log((double)Size, 2.0)));
            if (Size > 1024)
                Size = 1024;

            TextureDim_ = Size;
            cellDim = TextureDim_ / dim; // TextureDim_ alto dim son potencias de 2, luego cellDim siempre será entero.

            bmp = new Bitmap(TextureDim_, TextureDim_);
            gfx = System.Drawing.Graphics.FromImage(bmp);
            gfx.Clear(System.Drawing.Color.Transparent);

            // Adjust font rendering mode. Small Sizes look blurry without gridfitting, so turn
            // that on. Increasing contrast also seems to help.
            if (_Font.Size <= 18.0f)
            {
                gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                gfx.TextContrast = 1;
            }
            else
            {
                gfx.TextRenderingHint = TextRenderingHint.AntiAlias;
                gfx.TextContrast = 0;
            }

            float xCoordTex = 0;
            float yCoordTex = 0;
            int xOrigen = 0;
            int yOrigen = 0;

            float auxWidth, auxHeight;

            foreach (char c in pStr)
            {
                if (xcont >= dim)
                {
                    xcont = 0;
                    ycont += 2;
                }
                xOrigen = xcont * cellDim;
                yOrigen = ycont * cellDim;

                xCoordTex = (float)xOrigen / TextureDim_;
                yCoordTex = (float)yOrigen / TextureDim_;

                MeasureString(c, out auxWidth, out auxHeight);
                Glyphs[(int)c] = new Glyph(xCoordTex, 1 - yCoordTex, auxWidth);
                gfx.DrawString(c.ToString(), _Font, System.Drawing.Brushes.White, xOrigen, yOrigen);
                xcont++;
            }
        }

        /// <summary>
        /// Calcula las dimensiones de la textura opengl alto le asigna el bitmap con las letras ya dibujadas
        /// </summary>
        protected void CreateGLTexture()
        {
            //f1.ClientSize = new Size(bmp.Width, bmp.HeightFont);
            //f1.pictureBox1.Image = new Bitmap(bmp);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.GenTextures(1, out TextureId_);
            GL.BindTexture(TextureTarget.Texture2D, TextureId_);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 0); // 0 = False

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Alpha, TextureDim_, TextureDim_, 0,
                          OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, bmpData.Scan0);
            //bmp.Save("texFont.png", ImageFormat.Png);
            bmp.UnlockBits(bmpData);
            bmp.Dispose();
            bmp = null;
        }

        /// <summary>
        /// Elimina los caracteres no imprimibles por este TextFont
        /// </summary>
        /// <param Name_="textoInicial"></param>
        /// <returns></returns>
        public string TextFilter(string pText)
        {
            char c;
            for (int cont = 0; cont < pText.Length; cont++)
            {
                c = pText[cont];
                if ((int)c > 255)
                {
                    pText = pText.Remove(cont, 1);
                    if (cont > 0)
                        cont--;
                }
            }
            return pText;
        }

        void MeasureString(char c, out float width, out float height)
        {
            MeasureString(c.ToString(), out width, out height);
        }

        /// <summary>
        /// Obtiene el ancho_ejeX alto alto en pixels de un caracter
        /// </summary>
        /// <param Name_="str"></param>
        /// <param Name_="width"></param>
        /// <param Name_="height"></param>
        public void MeasureString(string str, out float width, out float height)
        {
            System.Drawing.StringFormat format = System.Drawing.StringFormat.GenericTypographic;
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

            System.Drawing.SizeF Size = gfx.MeasureString(str, _Font, 16384, format);
            height = Size.Height;
            width = Size.Width;
        }
    }

    internal class Glyph
    {
        internal PointF coordTex = new PointF();
        internal float width = 0;

        internal Glyph(float x, float y, float width)
        {
            coordTex.X = x;
            coordTex.Y = y;
            this.width = width;
        }
    }

    public class TextureManager
    {
        private static volatile TextureManager instance;
        private static object syncRoot = new Object();

        List<Texture> textures = new List<Texture>();

        public Stream fileChecker;

        /// <summary>
        /// Unica instancia de <see cref="GEngine"/>.
        /// </summary>
        public static TextureManager Singleton
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new TextureManager();
                    }
                }

                return instance;
            }
        }

        TextureManager()
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                // Para ver los nombres de los recursos incrustados
                //String[] nombres = a.GetManifestResourceNames(); 
                fileChecker = a.GetManifestResourceStream("TestGui.Resources.Glass.png");
            }
            catch
            {
                throw new Exception("Error accessing resources!");
            }
        }

        public Texture2D CreateTexture2D(string name, string path)
        {
            Texture2D tex = new Texture2D(name);
            tex.LoadFromFile(path);
            textures.Add(tex);
            return tex;
        }

        public Texture2D CreateTexture2D(string name, Stream str)
        {
            Texture2D tex = new Texture2D(name);
            tex.LoadFromStream(str);
            textures.Add(tex);
            return tex;
        }
        public Texture2D CreateTexture2D(TextFontTexture textFontTex)
        {
            Texture2D tex = new Texture2D("TextFontTexture", textFontTex);
            return tex;
        }
        public void UnloadAll()
        {
            int texId = -1;

            foreach (Texture tex in textures)
            {
                texId = tex.TextureId;
                GL.DeleteTextures(1, ref texId);
            }
            textures.Clear();
        }
    }
}