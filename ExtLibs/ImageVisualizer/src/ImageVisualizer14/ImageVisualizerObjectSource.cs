using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SkiaSharp;

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    public class ImageVisualizerObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            if (target is SKBitmap image1)
            {
                var bitmapSource = PngBitmapDecoder.Create(image1.Encode(SKEncodedImageFormat.Png, 100).AsStream(),
                    BitmapCreateOptions.None,
                    BitmapCacheOption.Default);
                base.GetData(new SerializableBitmapImage(bitmapSource.Frames[0]), outgoingData);
            }
            else if (target is SKImage image2)
            {
                var bitmapSource = PngBitmapDecoder.Create(image2.Encode().AsStream(), BitmapCreateOptions.None,
                    BitmapCacheOption.Default).Frames[0];
                base.GetData(new SerializableBitmapImage(bitmapSource), outgoingData);
            }
            else if (target is ImageSource image3)
            {
                base.GetData(new SerializableBitmapImage((BitmapSource) image3), outgoingData);
            }
            else
            {
                base.GetData(target, outgoingData);
            }
        }
    }
}
