using SkiaSharp;
using System;
using System.Drawing;
using System.IO;
using SKImage = SkiaSharp.SKImage;

namespace MissionPlanner.Utilities.Drawing
{
    public class Bitmap : Image
    {
        public Bitmap(int width, int height, int stride, SKColorType bgra8888 = (SKColorType.Bgra8888),
            IntPtr data = default(IntPtr))
        {
            nativeSkBitmap = new SKBitmap(new SKImageInfo(width, height, bgra8888));
            nativeSkBitmap.SetPixels(data);
        }

        public Bitmap(int width, int height, SKColorType colorType = (SKColorType.Bgra8888))
        {
            nativeSkBitmap = new SKBitmap(new SKImageInfo(width, height, colorType));
            nativeSkBitmap.Erase(SKColor.Empty);
        }

        public Bitmap(Stream stream)
        {
            nativeSkBitmap = SKBitmap.Decode(stream);
        }

        internal Bitmap()
        {
        }

        public static implicit operator SKImage(Bitmap input)
        {
            return SKImage.FromBitmap(input.nativeSkBitmap);
        }

        public Bitmap(int clientSizeWidth, int clientSizeHeight, Graphics realDc)
        {
            nativeSkBitmap = new SKBitmap(new SKImageInfo(clientSizeWidth, clientSizeHeight, SKColorType.Bgra8888));
            nativeSkBitmap.Erase(SKColor.Empty);
            //nativeSkBitmap.SetPixels(realDc._surface.);
        }

        public Bitmap(string filename)
        {
            using (var f = File.OpenRead(filename))
                nativeSkBitmap = SKBitmap.Decode(f);
        }

        public Bitmap(Image image)
        {
            nativeSkBitmap = image.nativeSkBitmap.Copy();
        }

        public Bitmap(Image largeIconsImage, Size clientSizeHeight)
        {
            SKBitmap ans = new SKBitmap(clientSizeHeight.Width, clientSizeHeight.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            largeIconsImage.nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            nativeSkBitmap = ans;
        }

        public Bitmap(Image largeIconsImage, int x, int y)
        {
            SKBitmap ans = new SKBitmap(x, y, SKColorType.Bgra8888, SKAlphaType.Premul);
            largeIconsImage.nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            nativeSkBitmap = ans;
        }

        public Bitmap(int width, int height, PixelFormat format4BppIndexed): this(width, height, SKColorType.Bgra8888)
        {
        }

        public Bitmap(Type clientSizeWidth, string propertygridCategorizedPng)
        {
            // no idea
        }

        public Bitmap(Bitmap bmp, Size size): this(bmp, size.Width,size.Height)
        {
         
        }

        public SKColorType PixelFormat
        {
            get { return nativeSkBitmap.ColorType; }

            set { }
        }

        public ColorPalette Palette { get; set; } = new ColorPalette(256);

        public BitmapData LockBits(Rectangle rectangle, object writeOnly, SKColorType imgPixelFormat)
        {
            return new BitmapData()
            {
                Scan0 = nativeSkBitmap.GetPixels(),
                Stride = nativeSkBitmap.RowBytes,
                Width = rectangle.Width,
                Height = rectangle.Height
            };
        }

        public void UnlockBits(BitmapData bmpData)
        {
            bmpData = null;
        }
        public void MakeTransparent()
        {
            if (nativeSkBitmap.IsEmpty)
                nativeSkBitmap.Erase(SKColor.Empty);
        }
        public void MakeTransparent(Color transparent)
        {
            if(nativeSkBitmap.IsEmpty)
                nativeSkBitmap.Erase(SKColor.Empty);
        }

        public Color GetPixel(int c2, int c1)
        {
            return nativeSkBitmap.GetPixel(c2, c1).ToColor();
        }

        public void SetPixel(int i, int i1, Color transparent)
        {
            nativeSkBitmap.SetPixel(i,i1, transparent.ToSKColor());
        }

        public Image GetThumbnailImage(int v1, int v2, GetThumbnailImageAbort myCallback, IntPtr zero)
        {
            SKBitmap ans = new SKBitmap(v1, v2, SKColorType.Bgra8888, SKAlphaType.Premul);
            nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            return new Bitmap() {nativeSkBitmap = ans, PixelFormat = SKColorType.Bgra8888};
        }

        public BitmapData LockBits(Rectangle rectangle, ImageLockMode readWrite, PixelFormat format32BppArgb)
        {
            return LockBits(rectangle, readWrite, SKColorType.Rgba8888);
        }

        public void RotateFlip(RotateFlipType rotateNoneFlipX)
        {
            //
        }
    }
}