using SkiaSharp;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;

namespace MissionPlanner.Utilities.Drawing
{
    [Serializable]
    public abstract class Image : ISerializable, ICloneable, IDisposable
    {
        public delegate bool GetThumbnailImageAbort();

        private object userData;
        private SKBitmap _skBitmap;
        internal SKBitmap nativeSkBitmap
        {
            get
            {
                if (_skBitmap != null) return _skBitmap; _skBitmap = SKBitmap.FromImage(nativeSKImage);
                return _skBitmap;
            }
            set { nativeSKImage = SKImage.FromBitmap(value);
                _skBitmap = value;
            }
        }

        internal SKImage nativeSKImage;

        public int Width
        {
            get { return nativeSKImage.Width; }
        }
        public int Height
        {
            get { return nativeSKImage.Height; }
        }

        /// <summary>Gets the pixel format for this <see cref="T:System.Drawing.Image" />.</summary>
        /// <returns>A <see cref="T:System.Drawing.Imaging.PixelFormat" /> that represents the pixel format for this <see cref="T:System.Drawing.Image" />.</returns>
        public SKColorType PixelFormat
        {
            get { return nativeSKImage.ColorType; }
        }


        // System.Drawing.Image
        /// <summary>Gets the width and height, in pixels, of this image.</summary>
        /// <returns>A <see cref="T:System.Drawing.Size" /> structure that represents the width and height, in pixels, of this image.</returns>
        public Size Size => new Size(nativeSKImage.Width, nativeSKImage.Height);

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
            var skimage = SKImage.FromEncodedData(ms2);
            if (skimage == null)
                return null;
            var ans = new Bitmap() {nativeSKImage = skimage};
            return ans;
        }

        public object Clone()
        {
            return new Bitmap() {nativeSKImage = this.nativeSKImage.ToRasterImage()};
        }

        internal Image()
        {

        }
        protected Image(SerializationInfo info, StreamingContext context)
        {
            FromStream(new MemoryStream((byte[]) info.GetValue("pngdata", typeof(byte[]))));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            MemoryStream ms = new MemoryStream();
            Save(ms, SKEncodedImageFormat.Png);
            info.AddValue("pngdata", ms.ToArray());
        }

        public void Save(string filename, SKEncodedImageFormat format)
        {
            using (var stream = File.OpenWrite(filename))
                nativeSKImage.Encode(format, 100).SaveTo(stream);
        }

        public void Save(Stream stream, SKEncodedImageFormat format)
        {
            nativeSKImage.Encode(format, 100).SaveTo(stream);
        }

        public void Save(string outputfilename)
        {
            Save(outputfilename, SKEncodedImageFormat.Jpeg);
        }

        public void Dispose()
        {
            nativeSkBitmap?.Dispose();
            nativeSKImage?.Dispose();
        }
    }
}

namespace System.Drawing.Imaging
{
}
