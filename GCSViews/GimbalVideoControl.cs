#if !LIB
// XXX: We need both the System.Drawing.Bitmap from System.Drawing and MissionPlanner.Drawing
extern alias Drawing;
using MPBitmap = Drawing::System.Drawing.Bitmap;
#else
using MPBitmap = System.Drawing.Bitmap;
#endif

using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MissionPlanner
{
    public partial class GimbalVideoControl : UserControl
    {
        public GimbalVideoControl()
        {
            InitializeComponent();

            MissionPlanner.Utilities.GStreamer.onNewImage += RenderFrame;
        }

        private void RenderFrame(object sender, MPBitmap image)
        {
            try
            {
                if (image == null)
                {
                    VideoBox.Image?.Dispose();
                    VideoBox.Image = null;
                    return;
                }

                var old = VideoBox.Image;
                VideoBox.Image = new Bitmap(
                    image.Width, image.Height, 4 * image.Width,
                    PixelFormat.Format32bppPArgb,
                    image.LockBits(Rectangle.Empty, null, SKColorType.Bgra8888).Scan0);

                old?.Dispose();
            }
            catch
            {
            }
        }

        private void GimbalVideoControl_Disposed(object sender, System.EventArgs e)
        {
            MissionPlanner.Utilities.GStreamer.onNewImage -= RenderFrame;
        }
    }
}
