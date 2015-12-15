using LibVLC.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MissionPlanner.GCSViews;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public class vlcrender
    {
        static List<vlcrender> store = new List<vlcrender>();

        LibVLCLibrary library;
        IntPtr inst, mp, m;
        int Width, Height;

        LibVLCLibrary.libvlc_video_lock_cb vlc_lock_delegate;
        LibVLCLibrary.libvlc_video_unlock_cb vlc_unlock_delegate;
        LibVLCLibrary.libvlc_video_display_cb vlc_picture_delegate;

        public string playurl = "rtsp://192.168.1.253:554/Streaming/Channels/1";
        public void Start(int Width, int Height)
        {
            if (store.Count > 0)
                store[0].Stop();

            store.Add(this);

            this.Width = Width;
            this.Height = Height;

            library = LibVLCLibrary.Load(null);
            
            inst = library.m_libvlc_new(4,
                new string[] {":sout-udp-caching=0", ":udp-caching=0", ":rtsp-caching=0", ":tcp-caching=0"});
                //libvlc_new()                                    // Load the VLC engine 
            m = library.libvlc_media_new_location(inst, playurl);
                // Create a new item 
            mp = library.libvlc_media_player_new_from_media(m); // Create a media player playing environement 
            library.libvlc_media_release(m); // No need to keep the media now 

            vlc_lock_delegate = new LibVLCLibrary.libvlc_video_lock_cb(vlc_lock);
            vlc_unlock_delegate = new LibVLCLibrary.libvlc_video_unlock_cb(vlc_unlock);
            vlc_picture_delegate = new LibVLCLibrary.libvlc_video_display_cb(vlc_picture);

            library.libvlc_video_set_callbacks(mp,
                vlc_lock_delegate,
                vlc_unlock_delegate,
                vlc_picture_delegate);

            library.libvlc_video_set_format(mp, "RV24", (uint)Width, (uint)Height, (uint)Width*4);

            library.libvlc_media_player_play(mp); // play the media_player 
        }

        public void Stop()
        {
            store.Remove(this);

            if (library == null)
                return;

            library.libvlc_media_player_stop(mp);                             // Stop playing 
            library.libvlc_media_player_release(mp);                          // Free the media_player 
            library.libvlc_release(inst);

            LibVLCLibrary.Free(library);
        }

        IntPtr imageIntPtr = IntPtr.Zero;

        private void vlc_unlock(IntPtr opaque, IntPtr picture, ref IntPtr planes)
        {
            //Marshal.Release(planes);
        }

        private IntPtr vlc_lock(IntPtr opaque, ref IntPtr planes)
        {
            if (imageIntPtr == IntPtr.Zero)
                imageIntPtr = Marshal.AllocHGlobal(Width * 4 * Height);

            planes = imageIntPtr;
            return imageIntPtr;
        }

        private void vlc_picture(IntPtr opaque, IntPtr picture)
        {
            var image = new Bitmap(Width, Height, 4 * Width, System.Drawing.Imaging.PixelFormat.Format24bppRgb, picture);
            //image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            FlightData.myhud.bgimage = image;
        }
    }
}
