using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SkiaSharp;

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    public class ImageVisualizerObjectSource : VisualizerObjectSource
    {
        /// <summary>
        /// Grab the source, and return a png stream
        /// </summary>
        /// <param name="target"></param>
        /// <param name="outgoingData"></param>
        public override void GetData(object target, Stream outgoingData)
        {
            if (target is SKBitmap image1)
            {
                var bitmapSource = image1.Encode(SKEncodedImageFormat.Png, 100).AsStream();
                base.GetData(new SerializableBitmapImage(bitmapSource), outgoingData);
            }
            else if (target is SKImage image2)
            {
                var bitmapSource = image2.Encode(SKEncodedImageFormat.Png, 100).AsStream();
                base.GetData(new SerializableBitmapImage(bitmapSource), outgoingData);
            }
            else if (target is SKSurface image3)
            {
                base.GetData(
                    new SerializableBitmapImage(image3.Snapshot().Encode(SKEncodedImageFormat.Png, 100).AsStream()),
                    outgoingData);
            }
            else if (target is SKDrawable image4)
            {
                base.GetData(
                    new SerializableBitmapImage(SKImage.FromPicture(image4.Snapshot(), new SKSizeI(1024,1024)).Encode(SKEncodedImageFormat.Png, 100).AsStream()),
                    outgoingData);
            }
            else if (target is SKCanvas image5)
            {
                base.GetData(
                    new SerializableBitmapImage(new byte[0]),
                    outgoingData);
            }
            else if (target is SKPicture image6)
            {
                base.GetData(
                    new SerializableBitmapImage(SKImage.FromPicture(image6, new SKSizeI(512,512)).Encode(SKEncodedImageFormat.Png, 100).AsStream()),
                    outgoingData);
            }
            else
            {
                base.GetData(target, outgoingData);
            }
        }
    }
}
