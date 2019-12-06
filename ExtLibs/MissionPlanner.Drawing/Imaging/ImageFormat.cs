using System;
using SkiaSharp;

namespace MissionPlanner.Drawing.Imaging
{
    public sealed class ImageFormat : Object
    {
        public static SKEncodedImageFormat Bmp { get; set; } = SKEncodedImageFormat.Bmp;

        public static SKEncodedImageFormat Png { get; set; } = SKEncodedImageFormat.Png;
    }
}