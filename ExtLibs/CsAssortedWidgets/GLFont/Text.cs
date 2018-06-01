
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
using System.Drawing;

using AssortedWidgets.Graphics;
using AssortedWidgets.Math3D;

using OpenTK.Graphics.OpenGL;

namespace AssortedWidgets.GLFont
{
    public class Text : Mesh
    {
        string _TextValue;
        int numVertices = 0;
        int adjust = 0;
        protected TextFontTexture texFont;
        VectorStream<Vector3f> vertices;
        VectorStream<Vector2f> coordTex;
        Vector3f auxVertice = new Vector3f();
        Vector2f auxCoordTex = new Vector2f();
        int defMaxChars = 0;
        public Texture textura;

        const int DEFAULT_MAX_CHARS = 50;

        Color ColorTexto_ = Color.OrangeRed;
        Color ColorSombra_ = Color.DarkGray;
        public int PosX = 10;
        public int PosY = 10;

        /// <summary>
        /// Define una instancia de texto con un número máximo de caracteres igual a 'DEFAULT_MAX_CHARS'
        /// </summary>
        /// <param Name_="texFuente"></param>
        public Text(string nombre, TextFontTexture pTexFont)
            : this(nombre, pTexFont, DEFAULT_MAX_CHARS, "")
        {
        }
        public Text(string nombre, TextFontTexture pTexFont, String textoInicial)
            : this(nombre, pTexFont, textoInicial.Length, textoInicial)
        {
        }

        /// <summary>
        /// Define una instancia de texto con un número máximo de caracteres igual a maxChars
        /// </summary>
        /// <param Name_="texFuente"></param>
        /// <param Name_="maxChars"></param>
        public Text(string nombre, TextFontTexture pTexFont, int pMaxChars)
            : this(nombre, pTexFont, pMaxChars, "")
        {
        }

        /// <summary>
        /// Define una nueva instancia de texto con un número máximo de caracteres igual a maxChars, valor
        /// inicial del texto igual a textoInicial
        /// </summary>
        /// <param Name_="textoInicial"></param>
        /// <param Name_="texFuente"></param>
        public Text(string nombre, TextFontTexture texFuente, int maxChars, string textoInicial)
            : base(nombre)
        {
            if (textoInicial == null)
                throw new ArgumentNullException("textoInicial", "Argumento del constructor de TextInstance(), nulo.");
            if (texFuente == null)
                throw new ArgumentNullException("texFuente", "Argumento del constructor de TextInstance(), nulo.");

            defMaxChars = maxChars;
            _TextValue = textoInicial;
            numVertices = maxChars * 4;

            this.PositionStream_ = new VectorStream<Vector3f>(numVertices);
            this.texCoordStream_ = new VectorStream<Vector2f>(numVertices);
            this.primitiveType_ = OpenTK.Graphics.OpenGL.BeginMode.Quads;
            texFont = texFuente;
            textura = TextureManager.Singleton.CreateTexture2D(texFuente);
            PerformLayout();
        }

        public Color TextColor
        {
            set 
            {
                ColorTexto_ = value; 
            }
            get { return ColorTexto_; }
        }

        public Color ColorSombra
        {
            set 
            {
                ColorSombra_ = value; 
            }
            get { return ColorSombra_; }
        }

        /// <summary>
        /// Nuevo texto a mostrar.
        /// </summary>
        public string TextValue
        {
            set
            {
                _TextValue = value;
                PerformLayout();
            }
            get
            {
                return _TextValue;
            }
        }

        void PerformLayout()
        {
            // 0-----3
            // |     |
            // |     |
            // 1-----2

            float coordTextWidth;
            float coordTextHeight;

            float xtemp = 0;
            float ytemp = 0;

            Glyph unGlyph;

            if (_TextValue.Length > 8192)
                throw new ArgumentOutOfRangeException("_TextValue", _TextValue.Length, "Text length must be between 1 and 8192 characters");

            _TextValue = texFont.TextFilter(_TextValue);

            PositionStream_.CurrentOffset = 0;
            texCoordStream_.CurrentOffset = 0;

            if (_TextValue.Length > defMaxChars)
                PositionStream_.VariableVertices = defMaxChars * 4;
            else
                PositionStream_.VariableVertices = _TextValue.Length * 4;

            vertices = PositionStream_;
            coordTex = texCoordStream_;

            coordTextHeight = (float)texFont.Font.Height / texFont.TextureDim;

            if (texFont.Font.Size >= 32)
                adjust = 20;
            else
                if (texFont.Font.Size >= 10)
                    adjust = 6;
                else
                    adjust = 2;

            int min = Math.Min(defMaxChars, _TextValue.Length);
            for (int cont = 0; cont < min; cont++)
            {
                char c = _TextValue[cont];
                unGlyph = texFont[(int)c];
                coordTextWidth = (float)(texFont.Font.Size + adjust) / texFont.TextureDim;
                //0
                auxCoordTex.X = unGlyph.coordTex.X;
                auxCoordTex.Y = unGlyph.coordTex.Y;
                coordTex += auxCoordTex;
                auxVertice.X = xtemp;
                auxVertice.Y = ytemp;
                vertices += auxVertice;
                //1
                auxCoordTex.X = unGlyph.coordTex.X;
                auxCoordTex.Y = unGlyph.coordTex.Y - coordTextHeight;
                coordTex += auxCoordTex;
                auxVertice.X = xtemp;
                auxVertice.Y = ytemp + texFont.HeightFont;
                vertices += auxVertice;
                //2
                auxCoordTex.X = unGlyph.coordTex.X + coordTextWidth;
                auxCoordTex.Y = unGlyph.coordTex.Y - coordTextHeight;
                coordTex += auxCoordTex;
                auxVertice.X = xtemp + texFont.Font.Size + adjust;
                auxVertice.Y = ytemp + texFont.HeightFont;
                vertices += auxVertice;
                //3
                auxCoordTex.X = unGlyph.coordTex.X + coordTextWidth;
                auxCoordTex.Y = unGlyph.coordTex.Y;
                coordTex += auxCoordTex;
                auxVertice.X = xtemp + texFont.Font.Size + adjust;
                auxVertice.Y = ytemp;
                vertices += auxVertice;

                xtemp = xtemp + unGlyph.width;
            }
        }

        private void Render(Color pColor, Color pShadowColor)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            GL.LoadIdentity();
            GL.Translate(2.5f, 2, 0);

            Render(pShadowColor);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            Render(pColor);
        }

        private void Render(Color pColor)
        {
            GL.Color4(pColor.R, pColor.G, pColor.B, pColor.A);

            base.Render(true);
        }

        public override void Render(bool setStreams)
        {
            if (textura != null)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, textura.TextureId);
            }
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();

            GL.LoadIdentity();
            GL.Translate(PosX, PosY, 0);
            /*
            if (sombraPlana)
                Render(ColorTexto_, ColorSombra_);
            else
                Render(ColorTexto_);
            */
            base.Render(true);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();

            GL.Disable(EnableCap.Texture2D);
        }
    }
}
