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

        public SKColorType PixelFormat
        {
            get { return nativeSkBitmap.ColorType; }

            set { }
        }

        public BitmapData LockBits(Rectangle rectangle, object writeOnly, SKColorType imgPixelFormat)
        {
            return new BitmapData() {Scan0 = nativeSkBitmap.GetPixels()};
        }

        public void UnlockBits(BitmapData bmpData)
        {
            bmpData = null;
        }

        public void MakeTransparent(Color transparent)
        {
            throw new NotImplementedException();
        }
    }
}