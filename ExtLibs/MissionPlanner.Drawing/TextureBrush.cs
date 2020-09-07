using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

namespace System.Drawing
{
    public class TextureBrush : Brush
    {
        public TextureBrush()
        {
            nativeBrush = new SKPaint() { };
        }

        public TextureBrush(Image backgroundImage, WrapMode tile)
        {
            Image = backgroundImage;
            SKShader item;
            switch (tile)
            {
                case WrapMode.Clamp:
                    item = backgroundImage.ToSKImage().ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);
                    nativeBrush = new SKPaint() {Shader = item};
                    break;
                case WrapMode.Tile:
                    item = backgroundImage.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Repeat);
                    nativeBrush = new SKPaint() {Shader = item};
                    break;
                case WrapMode.TileFlipXY:
                    item = backgroundImage.ToSKImage().ToShader(SKShaderTileMode.Mirror, SKShaderTileMode.Mirror);
                    nativeBrush = new SKPaint() {Shader = item};
                    break;
                case WrapMode.TileFlipX:
                    item = backgroundImage.ToSKImage().ToShader(SKShaderTileMode.Mirror, SKShaderTileMode.Repeat);
                    nativeBrush = new SKPaint() {Shader = item};
                    break;
                case WrapMode.TileFlipY:
                    item = backgroundImage.ToSKImage().ToShader(SKShaderTileMode.Repeat, SKShaderTileMode.Mirror);
                    nativeBrush = new SKPaint() {Shader = item};
                    break;
            }
        }

        public Image Image;

        public Color Color
        {
            get
            {
                return Color.FromArgb(nativeBrush.Color.Alpha, nativeBrush.Color.Red, nativeBrush.Color.Green,
                    nativeBrush.Color.Blue);
            }
            set { nativeBrush.Color = value.ToSKColor(); }
        }

        public Brush Clone()
        {
            return new TextureBrush() {nativeBrush = nativeBrush?.Clone()};
        }

        public void ScaleTransform(float rectangleWidth, float rectangleHeight, MatrixOrder append)
        {
        }

        public void TranslateTransform(float rectangleLeft, float rectangleTop, MatrixOrder append)
        {
        }
    }
}