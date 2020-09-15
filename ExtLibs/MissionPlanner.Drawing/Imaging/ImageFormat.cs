using System;
using SkiaSharp;

namespace System.Drawing.Imaging
{
    public sealed class ImageFormat
    {
        internal SKEncodedImageFormat format;

        public ImageFormat(SKEncodedImageFormat format)
        {
            this.format = format;
        }

        public static ImageFormat Bmp { get; set; } = new ImageFormat(SKEncodedImageFormat.Bmp);

        public static ImageFormat Png { get; set; } = new ImageFormat(SKEncodedImageFormat.Png);

        public static ImageFormat MemoryBmp { get; set; } = new ImageFormat(SKEncodedImageFormat.Wbmp);


        public static ImageFormat Tiff { get; set; } = new ImageFormat(SKEncodedImageFormat.Jpeg);


        public static ImageFormat Gif { get; set; } = new ImageFormat(SKEncodedImageFormat.Gif);

        public static ImageFormat Jpeg { get; set; } = new ImageFormat(SKEncodedImageFormat.Jpeg);


        public static ImageFormat Icon { get; set; } = new ImageFormat(SKEncodedImageFormat.Ico);
    }
}