using System.Collections.Generic;

namespace System.Drawing.Imaging
{
    public sealed class ImageCodecInfo
    {
        public static IEnumerable<ImageCodecInfo> GetImageEncoders()
        {
            return new ImageCodecInfo[0];
        }

        public string MimeType { get; set; }
    }
}