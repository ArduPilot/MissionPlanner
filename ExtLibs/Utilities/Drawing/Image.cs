using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using Core.ExtendedObjects;
using ExifLibrary;

namespace MissionPlanner.Utilities.Drawing
{
    public abstract class Image : ISerializable, ICloneable, IDisposable
    {
        private object userData;
        internal SKBitmap nativeSkBitmap;

        public int Width
        {
            get { return nativeSkBitmap.Width; }
        }
        public int Height
        {
            get { return nativeSkBitmap.Height; }
        }

        /// <summary>Gets the pixel format for this <see cref="T:System.Drawing.Image" />.</summary>
        /// <returns>A <see cref="T:System.Drawing.Imaging.PixelFormat" /> that represents the pixel format for this <see cref="T:System.Drawing.Image" />.</returns>
        public SKColorType PixelFormat
        {
            get { return nativeSkBitmap.ColorType; }
        }


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
            var skbitmap = SKBitmap.FromImage(skimage);
            var ans = new Bitmap() { nativeSkBitmap = skbitmap };
            return ans;
        }

        public object Clone()
        {
            return new Bitmap() {nativeSkBitmap = this.nativeSkBitmap.Copy()};
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
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

        public void Save(string outputfilename)
        {
            Save(outputfilename, SKEncodedImageFormat.Jpeg);
        }

        public void Dispose()
        {
            nativeSkBitmap?.Dispose();
        }
    }
}

namespace System.Drawing.Imaging
{
}
