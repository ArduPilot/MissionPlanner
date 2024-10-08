﻿using System;
using Aberus.VisualStudio.Debugger.ImageVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;
using SkiaSharp;

[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKImage), Description = "SKImage Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKBitmap), Description = "SKBitmap Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKSurface), Description = "SKSurface Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKDrawable), Description = "SKDrawable Visualizer")]
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKPicture), Description = "SKPicture Visualizer")]
//[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(ImageVisualizerObjectSource), Target = typeof(SKCanvas), Description = "SKCanvas Visualizer")]

//System.Drawing.Bitmap
[assembly: System.Diagnostics.DebuggerVisualizer(typeof(ImageVisualizer), typeof(VisualizerObjectSource), Target = typeof(System.Drawing.Bitmap), Description = "Image Visualizer")]

namespace Aberus.VisualStudio.Debugger.ImageVisualizer
{
    /// <summary>
    /// A Visualizer for <see cref="System.Windows.Media.ImageSource"/> and <see cref="System.Drawing.Bitmap"/>.
    /// </summary>
    public class ImageVisualizer : DialogDebuggerVisualizer
    {
        public ImageVisualizer() : base(FormatterPolicy.Legacy) // or FormatterPolicy.Json
        {
        }

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            if (windowService == null)
                throw new ArgumentNullException(nameof(windowService), "This debugger does not support modal visualizers.");
            if (objectProvider == null)
                throw new ArgumentNullException(nameof(objectProvider));

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
            image5.SetPixel(5, 5, SKColors.Red);
            ImageVisualizer.TestShowVisualizer(image5);
        }
    }
}
