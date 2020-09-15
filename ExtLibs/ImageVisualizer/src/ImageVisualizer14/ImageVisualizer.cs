using System;
using Aberus.VisualStudio.Debugger.ImageVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SkiaSharp;
#if VS16
using Microsoft.VisualStudio.Utilities;
#endif

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKImage), Description = "SKImage Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKBitmap), Description = "SKBitmap Visualizer")]

//System.Drawing.Bitmap
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(VisualizerObjectSource), Target = typeof(System.Drawing.Bitmap), Description = "Image Visualizer")]
//System.Windows.Media.ImageSource, System.Windows.Media.Imaging.BitmapImage, System.Windows.Media.Imaging.BitmapSource
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(System.Windows.Media.ImageSource), Description = "Image Visualizer")]

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    /// <summary>
    /// A Visualizer for <see cref="System.Windows.Media.ImageSource"/> and <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public class ImageVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService), "This debugger does not support modal visualizers.");
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));
#if VS16
            using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
#endif
            using (var imageForm = new ImageForm(objectProvider))
            {

                 windowService.ShowDialog(imageForm);
            }
        }

#if DEBUG
        /// <summary>
        /// Tests the visualizer by hosting it outside of the debugger.
        /// </summary>
        /// <param name="objectToVisualize">The object to display in the visualizer.</param>
        public static void TestShowVisualizer(object objectToVisualize)
        {
            var visualizerHost = new VisualizerDevelopmentHost(objectToVisualize, typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource));
            visualizerHost.ShowVisualizer();
        }
#endif
    }

    class Program
    { 
        [STAThread]
        static void Main(string[] args)
        {
            var image5 = new SKBitmap(1280, 720);
            ImageVisualizer.TestShowVisualizer(image5);
        }
    }
}
