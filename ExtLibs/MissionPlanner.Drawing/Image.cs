﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using SkiaSharp;

namespace System.Drawing
{
    [Serializable]
    public abstract class Image : ISerializable, ICloneable, IDisposable
    {
        public delegate bool GetThumbnailImageAbort();

        private object userData;
        private SKBitmap _skBitmap;

        internal SKBitmap nativeSkBitmap
        {
            get { return _skBitmap; }
            set { _skBitmap = value; }
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
        public SKColorType PixelFormat
        {
            get { return nativeSkBitmap.ColorType; }
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
            var skimage = SKImage.FromEncodedData(ms2);
            if (skimage == null)
                return null;
            var ans = new Bitmap() {nativeSkBitmap = SKBitmap.FromImage(skimage)};
            return ans;
        }

        public object Clone()
        {
            return new Bitmap() {nativeSkBitmap = this.nativeSkBitmap.Copy()};
        }

        internal Image()
        {
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
            nativeSkBitmap?.Dispose();
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