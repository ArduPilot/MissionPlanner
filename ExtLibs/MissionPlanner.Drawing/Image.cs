using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using SkiaSharp;

namespace System.Drawing
{
    [Serializable]
    public abstract class Image : ISerializable, ICloneable, IDisposable
    {
        public delegate bool GetThumbnailImageAbort();

        private object userData;
        private SKBitmap _skBitmap;

        // bytes of native pixel memory reported to the GC for _skBitmap
        private long _memoryPressure;

        internal SKBitmap nativeSkBitmap
        {
            get { return _skBitmap; }
            set
            {
                _skBitmap = value;
                UpdateMemoryPressure(value);
                InvalidateDrawCache();
            }
        }

        // open writers: Graphics.FromImage until the Graphics is disposed, LockBits until UnlockBits
        private int _writers;

        // pixels are caller-owned memory (the Bitmap stride constructors), written without telling us
        internal bool externalPixels;
        private SKImage _drawCache;

        internal void AddWriter()
        {
            Interlocked.Increment(ref _writers);
            InvalidateDrawCache();
        }

        internal void ReleaseWriter()
        {
            if (Interlocked.Decrement(ref _writers) < 0)
                Interlocked.Exchange(ref _writers, 0);
            InvalidateDrawCache();
        }

        internal void InvalidateDrawCache()
        {
            var cache = Interlocked.Exchange(ref _drawCache, null);
            cache?.Dispose();
        }

        /// <summary>
        /// An SKImage of the pixels to draw with, or null while something may still be writing to them.
        /// SkiaSharp 3 has no native drawBitmap, so drawing an SKBitmap goes through SKImage.FromBitmap,
        /// which copies a mutable bitmap's pixels on every draw (every HUD glyph, every frame). Built
        /// once here and reused until the pixels can change.
        /// </summary>
        internal SKImage GetDrawCache()
        {
            if (externalPixels || Volatile.Read(ref _writers) > 0)
                return null;
            var cache = _drawCache;
            if (cache != null)
                return cache;
            var bitmap = nativeSkBitmap;
            if (bitmap == null || bitmap.IsEmpty)
                return null;
            cache = SKImage.FromBitmap(bitmap);
            return Interlocked.CompareExchange(ref _drawCache, cache, null) ?? cache;
        }

        /// <summary>
        /// Marks a freshly decoded bitmap immutable. SkiaSharp 3 has no native drawBitmap, so drawing an
        /// SKBitmap goes through SKImage.FromBitmap, which copies the pixels of a mutable bitmap on every
        /// draw (map tiles, icons); an immutable one is shared instead. Writers call EnsureWritable first.
        /// </summary>
        internal static SKBitmap Immutable(SKBitmap bitmap)
        {
            bitmap?.SetImmutable();
            return bitmap;
        }

        /// <summary>Copy-on-write before changing pixels of a bitmap marked immutable.</summary>
        internal void EnsureWritable()
        {
            var bitmap = nativeSkBitmap;
            if (bitmap != null && bitmap.IsImmutable)
                nativeSkBitmap = bitmap.Copy();
            InvalidateDrawCache();
        }

        /// <summary>
        /// The pixels are native memory the GC can't see, so map tile and image caches only got freed by
        /// finalizers when managed allocations happened to trigger a collection. Report the size, as GDI+
        /// does for its images, so collections keep up with the native memory. Dispose() and the finalizer
        /// clear nativeSkBitmap, which removes the pressure again; Dispose() doesn't free the pixels (the
        /// SKBitmap may be shared), the SKBitmap's own finalizer does. Bitmaps wrapping external pixels
        /// (InstallPixels) are set while still empty and report nothing.
        /// </summary>
        private void UpdateMemoryPressure(SKBitmap bitmap)
        {
            long bytes = bitmap == null ? 0 : bitmap.ByteCount;
            if (bytes == _memoryPressure)
                return;
            if (_memoryPressure > 0)
                GC.RemoveMemoryPressure(_memoryPressure);
            if (bytes > 0)
                GC.AddMemoryPressure(bytes);
            _memoryPressure = bytes;
        }

        public SKImage ToSKImage()
        {
            return SKImage.FromBitmap(nativeSkBitmap);
        }

        public int Width
        {
            get { return nativeSkBitmap.Width; }
        }

        public int Height
        {
            get { return nativeSkBitmap.Height; }
        }

        /// <summary>Gets the pixel format for this <see cref="T:MissionPlanner.Drawing.Image" />.</summary>
        /// <returns>A <see cref="T:MissionPlanner.Drawing.PixelFormat" /> that represents the pixel format for this <see cref="T:MissionPlanner.Drawing.Image" />.</returns>
        public PixelFormat PixelFormat
        {
            get
            {
                switch (nativeSkBitmap.ColorType)
                {
                    case SKColorType.Bgra8888:
                        return Imaging.PixelFormat.Format32bppArgb;
                    case SKColorType.Rgb888x:
                        return Imaging.PixelFormat.Format32bppRgb;
                    case SKColorType.Argb4444:
                        return Imaging.PixelFormat.Format16bppArgb1555;
                    case SKColorType.Rgb565:
                        return Imaging.PixelFormat.Format16bppRgb565;
                    default:
                        return Imaging.PixelFormat.Format32bppArgb;
                }
            }

            set
            {
                Console.WriteLine("PixelFormat set " + value);
            }
        }

        public PropertyItem[] PropertyItems { get; }

        // System.Drawing.Image
        /// <summary>Gets the width and height, in pixels, of this image.</summary>
        /// <returns>A <see cref="T:System.Drawing.Size" /> structure that represents the width and height, in pixels, of this image.</returns>
        public Size Size => new Size(nativeSkBitmap.Width, nativeSkBitmap.Height);

        public static Image FromStream(Stream stream, bool useEmbeddedColorManagement, bool validateImageData)
        {
            return FromStream(stream);
        }

        /// <summary>Gets or sets an object that provides additional data about the image.</summary>
        /// <returns>The <see cref="T:System.Object" /> that provides additional data about the image.</returns>
        [Localizable(false)]
        [Bindable(true)]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get { return userData; }
            set { userData = value; }
        }

        public IEnumerable<Guid> FrameDimensionsList { get; set; } = new List<Guid>();

        public int VerticalResolution = 72;

        public int HorizontalResolution = 72;


        public static Image FromFile(string filename)
        {
            using (var ms = File.OpenRead(filename))
                return FromStream(ms);
        }

        public static Image FromStream(Stream ms)
        {
            MemoryStream ms2 = new MemoryStream();
            ms.CopyTo(ms2);
            ms2.Position = 0;
            using (var skimage = SKImage.FromEncodedData(ms2))
            {
                if (skimage == null)
                    return null;
                var ans = new Bitmap() {nativeSkBitmap = Immutable(SKBitmap.FromImage(skimage))};
                return ans;
            }
        }

        public object Clone()
        {
            return new Bitmap() {nativeSkBitmap = this.nativeSkBitmap.Copy()};
        }

        public Bitmap Clone(Rectangle rectangle, PixelFormat format24bppRgb)
        {
            return new Bitmap() { nativeSkBitmap = this.nativeSkBitmap.Copy(SKColorType.Rgb888x) };
        }

        internal Image()
        {
        }

        ~Image()
        {
             Dispose();
        }

        protected Image(SerializationInfo info, StreamingContext context)
        {
            nativeSkBitmap = FromStream(new MemoryStream((byte[]) info.GetValue("Data", typeof(byte[]))))
                .nativeSkBitmap;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            MemoryStream ms = new MemoryStream();
            Save(ms, SKEncodedImageFormat.Png);
            info.AddValue("Data", ms.GetBuffer());
        }


        public void Save(string filename, SKEncodedImageFormat format)
        {
            using (var stream = File.OpenWrite(filename))
                SKImage.FromBitmap(nativeSkBitmap).Encode(format, 100).SaveTo(stream);
        }


        public void Save(Stream stream, SKEncodedImageFormat format)
        {
            SKImage.FromBitmap(nativeSkBitmap).Encode(format, 100).SaveTo(stream);
        }

        public void Save(string filename, ImageFormat format)
        {
            using (var stream = File.OpenWrite(filename))
                SKImage.FromBitmap(nativeSkBitmap).Encode(format.format, 100).SaveTo(stream);
        }


        public void Save(Stream stream, ImageFormat format)
        {
            SKImage.FromBitmap(nativeSkBitmap).Encode(format.format, 100).SaveTo(stream);
        }

        public void Save(string outputfilename)
        {
            Save(outputfilename, SKEncodedImageFormat.Jpeg);
        }

        public void Dispose()
        {
            try
            {
               // nativeSkBitmap?.Dispose();
                nativeSkBitmap = null;
            }
            catch
            {
            }
        }

        public void RotateFlip(RotateFlipType rotateNoneFlipY)
        {
        }

        public int GetFrameCount(FrameDimension time)
        {
            return 1;
        }

        public int SelectActiveFrame(FrameDimension dimension, int frameIndex)
        {
            return 1;
        }
    }
}