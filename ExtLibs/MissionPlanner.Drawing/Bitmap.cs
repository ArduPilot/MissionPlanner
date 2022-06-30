using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using SkiaSharp;


namespace System.Drawing
{
    [Serializable]
    public class Bitmap : Image, ISerializable, ICloneable, IDisposable
    {
        private object p;
        private Size size;

        public Bitmap(int width, int height, int stride, SKColorType bgra8888 = (SKColorType.Bgra8888),
            IntPtr data = default(IntPtr))
        {
            nativeSkBitmap = new SKBitmap(new SKImageInfo(width, height, bgra8888));
            nativeSkBitmap.SetPixels(data);
        }

        public Bitmap(int width, int height, int stride, PixelFormat bgra8888 = Imaging.PixelFormat.Format32bppArgb,
            IntPtr data = default(IntPtr))
        {
            nativeSkBitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Bgra8888));
            nativeSkBitmap.SetPixels(data);
        }

        public Bitmap(int width, int height, SKColorType colorType = (SKColorType.Bgra8888))
        {
            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
            nativeSkBitmap = new SKBitmap(new SKImageInfo(width, height, colorType));
            nativeSkBitmap.Erase(SKColor.Empty);
        }

        public Bitmap(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            try
            {
                var skimage = SKImage.FromEncodedData(stream);
                nativeSkBitmap = SKBitmap.FromImage(skimage); //SKBitmap.Decode(stream);
            }
            catch (Exception ex)
            {
                nativeSkBitmap = new SKBitmap(new SKImageInfo(1, 1));
                Console.WriteLine(ex);
            }
        }

        public static implicit operator Bitmap(byte[] data)
        {
            return new Bitmap(new MemoryStream(data));
        }

        protected Bitmap(SerializationInfo info, StreamingContext context): base (info, context)
        {
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
            if (clientSizeHeight < 1)
                clientSizeHeight = 1;
            if (clientSizeWidth < 1)
                clientSizeWidth = 1;
            nativeSkBitmap = new SKBitmap(new SKImageInfo(clientSizeWidth, clientSizeHeight, SKColorType.Bgra8888));
            nativeSkBitmap.Erase(SKColor.Empty);
            //nativeSkBitmap.SetPixels(realDc._surface.);
        }

        public Bitmap(string filename)
        {
            if (filename is null)
            {
                throw new ArgumentNullException(nameof(filename));
            }

            using (var f = File.OpenRead(filename))
                nativeSkBitmap = SKBitmap.Decode(f);
        }

        public Bitmap(Image image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            nativeSkBitmap = image.nativeSkBitmap.Copy();
        }

        public new static Bitmap FromFile(string filename)
        {
            using (var ms = File.OpenRead(filename))
                return FromStream(ms);
        }

        public new static Bitmap FromStream(Stream ms)
        {
            if (ms is null)
            {
                throw new ArgumentNullException(nameof(ms));
            }

            MemoryStream ms2 = new MemoryStream();
            ms.CopyTo(ms2);
            ms2.Position = 0;
            var skimage = SKImage.FromEncodedData(ms2);
            if (skimage == null)
                return null;
            var ans = new Bitmap() {nativeSkBitmap = SKBitmap.FromImage(skimage)};
            return ans;
        }
        public Bitmap(byte[] largeIconsImage, Size clientSizeHeight)
        {
            nativeSkBitmap = SKBitmap.Decode(SKData.CreateCopy(largeIconsImage)).Resize(
                new SKImageInfo(clientSizeHeight.Width, clientSizeHeight.Height),
                SKFilterQuality.High);
        }

        public Bitmap(Image largeIconsImage, Size clientSizeHeight)
        {
            SKBitmap ans = new SKBitmap(clientSizeHeight.Width, clientSizeHeight.Height, SKColorType.Bgra8888,
                SKAlphaType.Premul);
            largeIconsImage.nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            nativeSkBitmap = ans;
        }

        public Bitmap(Image largeIconsImage, int x, int y)
        {
            SKBitmap ans = new SKBitmap(x, y, SKColorType.Bgra8888, SKAlphaType.Premul);
            largeIconsImage.nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            nativeSkBitmap = ans;
        }

        public Bitmap(int width, int height, PixelFormat format4BppIndexed) : this(width, height, SKColorType.Bgra8888)
        {
        }

        public Bitmap(Type clientSizeWidth, string propertygridCategorizedPng)
        {
            // no idea
        }

        public Bitmap(Bitmap bmp, Size size) : this(bmp, size.Width, size.Height)
        {
        }

        public Bitmap(byte[] camera_icon_G, int v1, int v2) : this(camera_icon_G, new Size(v1, v2))
        {
        }

        public ColorPalette Palette { get; set; } = new ColorPalette(256);

        public BitmapData LockBits(Rectangle rectangle, object writeOnly, SKColorType imgPixelFormat)
        {
            return new BitmapData()
            {
                Scan0 = nativeSkBitmap.GetPixels(),
                Stride = nativeSkBitmap.RowBytes,
                Width = nativeSkBitmap.Width,
                Height = nativeSkBitmap.Height
            };
        }

        public BitmapData LockBits(int x,int y, int w, int h, object writeOnly, SKColorType imgPixelFormat)
        {
            return new BitmapData()
            {
                Scan0 = nativeSkBitmap.GetPixels(),
                Stride = nativeSkBitmap.RowBytes,
                Width = nativeSkBitmap.Width,
                Height = nativeSkBitmap.Height
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
            if (nativeSkBitmap.IsEmpty)
                nativeSkBitmap.Erase(SKColor.Empty);
        }

        public Color GetPixel(int c2, int c1)
        {
            return nativeSkBitmap.GetPixel(c2, c1).ToColor();
        }

        public void SetPixel(int i, int i1, Color transparent)
        {
            nativeSkBitmap.SetPixel(i, i1, transparent.ToSKColor());
        }

        public Image GetThumbnailImage(int v1, int v2, GetThumbnailImageAbort myCallback, IntPtr zero)
        {
            SKBitmap ans = new SKBitmap(v1, v2, SKColorType.Bgra8888, SKAlphaType.Premul);
            nativeSkBitmap.ScalePixels(ans, SKFilterQuality.Medium);
            return new Bitmap() {nativeSkBitmap = ans, PixelFormat = PixelFormat.Format32bppArgb};
        }

        public BitmapData LockBits(Rectangle rectangle, ImageLockMode readWrite, PixelFormat format32BppArgb)
        {
            return LockBits(rectangle, readWrite, SKColorType.Rgba8888);
        }

        public object GetHicon()
        {
            return nativeSkBitmap.Handle;
        }

        public void SetResolution(float imageHorizontalResolution, float imageVerticalResolution)
        {
        }

        public void Save(MemoryStream streamjpg, ImageCodecInfo ici, EncoderParameters eps)
        {
            Save(streamjpg, SKEncodedImageFormat.Jpeg);
        }

        public IntPtr GetHbitmap()
        {
            return IntPtr.Zero;
        }
    }
}