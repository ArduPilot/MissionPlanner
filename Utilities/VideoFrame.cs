#if !LIB
// We need both the System.Drawing.Bitmap from System.Drawing and the one from MissionPlanner.Drawing
extern alias Drawing;
using MPBitmap = Drawing::System.Drawing.Bitmap;
#else
using MPBitmap = System.Drawing.Bitmap;
#endif

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace MissionPlanner.Utilities
{
    public static class VideoFrame
    {
        /// <summary>
        /// Copy a frame handed out by a video source (GStreamer, vlc, mjpeg) into a Bitmap that owns its pixels.
        /// The source frame's memory is only valid while its event handler runs - GStreamer unmaps the buffer as
        /// soon as the handler returns - so anything kept for a later paint has to be a copy.
        /// </summary>
        public static Bitmap ToOwnedBitmap(MPBitmap frame)
        {
            var copy = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppPArgb);
            try
            {
                var data = copy.LockBits(new Rectangle(0, 0, copy.Width, copy.Height), ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppPArgb);
                try
                {
                    if (!frame.CopyPixelsTo(data.Scan0, data.Stride))
                        throw new InvalidOperationException("Failed to copy the video frame");
                }
                finally
                {
                    copy.UnlockBits(data);
                }

                GC.KeepAlive(frame);
                return copy;
            }
            catch
            {
                copy.Dispose();
                throw;
            }
        }
    }
}
