using System;
using System.ComponentModel;
using System.IO;

using Android.Content;
using Android.Widget;
using ARelativeLayout = Android.Widget.RelativeLayout;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Acr.UserDialogs.Infrastructure;
using Android.Media;
using Android.Views;
using Android.Net.Rtp;
using Xamarin.GCSViews;
using Java.Interop;
using Android.Runtime;
using Android.Graphics;
using Java.Nio;

using System.Threading;

using System.Threading.Tasks;

using static Android.Media.MediaCodec;
using System.Collections.Generic;
using Xamarin;
using Stream = System.IO.Stream;

[assembly: ExportRenderer(typeof(FormsVideoLibrary.VideoPlayer),
                          typeof(FormsVideoLibrary.Droid.VideoPlayerRenderer))]

namespace FormsVideoLibrary.Droid
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, ARelativeLayout>
    {
        SurfaceView videoView;
        private MediaCodec codec;
        private CallBacks callbacks;
        MediaController mediaController;    // Used to display transport controls
        bool isPrepared;
        private bool iframestart;
        private CancellationTokenSource rtspCancel;
        private bool h264;
        private bool h265;
        private bool rtsprunning;

        public VideoPlayerRenderer(Context context) : base(context)
        {
        }

        public class CallBacks : MediaCodec.Callback
        {
            private VideoPlayerRenderer videoPlayerRenderer;
            MediaFormat mOutputFormat;
            internal Stack<int> buffers = new Stack<int>();

            public CallBacks(VideoPlayerRenderer videoPlayerRenderer)
            {
                this.videoPlayerRenderer = videoPlayerRenderer;
            }

            public override void OnError(MediaCodec codec, MediaCodec.CodecException e)
            {
                Console.WriteLine(e.DiagnosticInfo);
            }

            public override void OnInputBufferAvailable(MediaCodec codec, int index)
            {
                //var buffer = codec.GetInputBuffer(index);
                //Console.WriteLine("OnInputBufferAvailable " + index);

                buffers.Push(index);
            }

            public override void OnOutputBufferAvailable(MediaCodec codec, int index, MediaCodec.BufferInfo info)
            {
                //var buffer = codec.GetOutputBuffer(index);
                //Console.WriteLine("OnOutputBufferAvailable");

                codec.ReleaseOutputBuffer(index, true);
            }

            public override void OnOutputFormatChanged(MediaCodec codec, MediaFormat format)
            {
                Console.WriteLine("OnOutputFormatChanged");
                mOutputFormat = format;
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (rtspCancel == null)
            {
                rtspClientStart();
            }

            if (codec == null && (h264 || h265))
            {
                codec = MediaCodec.CreateDecoderByType(h265
                    ? MediaFormat.MimetypeVideoHevc
                    : MediaFormat.MimetypeVideoAvc);

                callbacks = new CallBacks(this);

                codec.SetCallback(callbacks);

                var mediafmt = MediaFormat.CreateVideoFormat(h265
                    ? MediaFormat.MimetypeVideoHevc
                    : MediaFormat.MimetypeVideoAvc, 1920, 1080);

                codec.Configure(mediafmt, videoView.Holder.Surface, null, MediaCodecConfigFlags.None);

                codec.Start();
            }

            base.OnDraw(canvas);
        }


        public async void rtspClientStart()
        {
            rtspCancel = new CancellationTokenSource();

            var url = "rtsp://192.168.0.10:8554/H264Video";

            String now = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            MemoryStream fs_vps = null;   // used to write the video
            MemoryStream fs_v = null;   // used to write the video
            MemoryStream fs_a = null;   // used to write the audio
            h264 = false;
            h265 = false;
            bool spsdone = false;

            RTSPClient c = new RTSPClient();

            // The SPS/PPS comes from the SDP data
            // or it is the first SPS/PPS from the H264 video stream
            c.Received_SPS_PPS += (byte[] sps, byte[] pps) => {
                h264 = true;
                if (fs_vps == null)
                {
                    String filename = "rtsp_capture_" + now + ".264";
                    fs_vps = new MemoryStream();
                }

                if (fs_vps != null)
                {
                    fs_vps.SetLength(0);
                    fs_vps.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);  // Write Start Code
                    fs_vps.Write(sps, 0, sps.Length);
                    fs_vps.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);  // Write Start Code
                    fs_vps.Write(pps, 0, pps.Length);
                }
            };

            c.Received_VPS_SPS_PPS += (byte[] vps, byte[] sps, byte[] pps) => {
                h265 = true;
                if (fs_vps == null)
                {
                    String filename = "rtsp_capture_" + now + ".265";
                    fs_vps = new MemoryStream();
                }

                if (fs_vps != null)
                {
                    fs_vps.SetLength(0);
                    fs_vps.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);  // Write Start Code
                    fs_vps.Write(vps, 0, vps.Length); // Video parameter set
                    fs_vps.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);  // Write Start Code
                    fs_vps.Write(sps, 0, sps.Length); // Sequence Parameter Set
                    fs_vps.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4);  // Write Start Code
                    fs_vps.Write(pps, 0, pps.Length); // Picture Parameter Set
                }
            };

            // Video NALs. May also include the SPS and PPS in-band for H264
            c.Received_NALs += (List<byte[]> nal_units) =>
            {
                foreach (byte[] nal_unit in nal_units)
                {
                    // Output some H264 stream information
                    if (h264 && nal_unit.Length > 0)
                    {
                        int nal_ref_idc = (nal_unit[0] >> 5) & 0x03;
                        int nal_unit_type = nal_unit[0] & 0x1F;
                        String description = "";
                        if (nal_unit_type == 1) description = "NON IDR NAL";
                        else if (nal_unit_type == 5) description = "IDR NAL";
                        else if (nal_unit_type == 6) description = "SEI NAL";
                        else if (nal_unit_type == 7) description = "SPS NAL";
                        else if (nal_unit_type == 8) description = "PPS NAL";
                        else if (nal_unit_type == 9) description = "ACCESS UNIT DELIMITER NAL";
                        else description = "OTHER NAL";
                        //Console.WriteLine("NAL Ref = " + nal_ref_idc + " NAL Type = " + nal_unit_type + " " + description);
                    }

                    // Output some H265 stream information
                    if (h265 && nal_unit.Length > 0)
                    {
                        int nal_unit_type = (nal_unit[0] >> 1) & 0x3F;
                        String description = "";
                        if (nal_unit_type == 1) description = "NON IDR NAL";
                        else if (nal_unit_type == 19) description = "IDR NAL";
                        else if (nal_unit_type == 32) description = "VPS NAL";
                        else if (nal_unit_type == 33) description = "SPS NAL";
                        else if (nal_unit_type == 34) description = "PPS NAL";
                        else if (nal_unit_type == 39) description = "SEI NAL";
                        else description = "OTHER NAL";
                        //Console.WriteLine("NAL Type = " + nal_unit_type + " " + description);
                    }

                    // we need sps... first
                    if (!h264 && !h265)
                        return;

                    if (!spsdone)
                    {
                        if (callbacks == null || callbacks.buffers.Count == 0)
                            return;
                        var index = callbacks.buffers.Pop();
                        var buffer = codec.GetInputBuffer(index);
                        buffer.Put(fs_vps.ToArray());
                        codec.QueueInputBuffer(index, 0, (int)fs_vps.Length, 0, MediaCodecBufferFlags.CodecConfig);
                        spsdone = true;

                        fs_v = new MemoryStream();
                    }

                    if (fs_v != null)
                    {
                        fs_v.Write(new byte[] {0x00, 0x00, 0x00, 0x01}, 0, 4); // Write Start Code
                        fs_v.Write(nal_unit, 0, nal_unit.Length); // Write NAL
                    }

                    if (callbacks == null || fs_v == null || callbacks.buffers.Count == 0)
                        return;
                    try
                    {

                        var index = callbacks.buffers.Pop();
                        var buffer = codec.GetInputBuffer(index);
                        buffer.Put(fs_v.ToArray());
                        codec.QueueInputBuffer(index, 0, (int) fs_v.Length, 0, MediaCodecBufferFlags.None);
                        fs_v.SetLength(0);
                    }
                    catch
                    {

                    }
                }
            };

            // seperate and stay running
            Task.Run(() =>
            {
                while (rtspCancel != null && true)
                {
                    try
                    {
                        if (rtspCancel.Token.IsCancellationRequested)
                            return;

                        c.Connect(url, RTSPClient.RTP_TRANSPORT.UDP);
                        var lastrtp = 0;
                        int cnt = 0;
                        while (!c.StreamingFinished())
                        {
                            rtsprunning = true;
                            Thread.Sleep(500);

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                try
                                {
                                    this.Invalidate();
                                }
                                catch (ObjectDisposedException)
                                {
                                    rtspCancel.Cancel();
                                }
                                catch
                                {
                                }
                            });

                            // existing
                            if (rtspCancel.Token.IsCancellationRequested)
                            {
                                c.Stop();
                                return;
                            }

                            // no rtp in .5 sec
                            if (lastrtp == c.rtp_count && cnt++ > 5)
                            {
                                c.Stop();
                                rtspCancel = null;
                                return;
                            }

                            lastrtp = c.rtp_count;
                        }

                        c.Stop();

                        rtsprunning = false;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MP", ex.ToString());
                    }
                }
            });
        }

       
        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    // Save the VideoView for future reference
                    videoView = new SurfaceView(Context);

                    // Put the VideoView in a RelativeLayout
                    ARelativeLayout relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(videoView);

                    // Center the VideoView in the RelativeLayout
                    ARelativeLayout.LayoutParams layoutParams =
                        new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);
                    videoView.LayoutParameters = layoutParams;

                    SetNativeControl(relativeLayout);
                }

                SetAreTransportControlsEnabled();
                SetSource();

                args.NewElement.UpdateStatus += OnUpdateStatus;
                args.NewElement.PlayRequested += OnPlayRequested;
                args.NewElement.PauseRequested += OnPauseRequested;
                args.NewElement.StopRequested += OnStopRequested;
            }

            if (args.OldElement != null)
            {
                args.OldElement.UpdateStatus -= OnUpdateStatus;
                args.OldElement.PlayRequested -= OnPlayRequested;
                args.OldElement.PauseRequested -= OnPauseRequested;
                args.OldElement.StopRequested -= OnStopRequested;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null && videoView != null)
            {
                //videoView.Prepared -= OnVideoViewPrepared;
            }

            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            rtspCancel.Cancel();
            Thread.Sleep(100);
            try
            {
                codec.Stop();
            }
            catch
            {
            }

            try
            {
                codec.Dispose();
            }
            catch
            {
            }

            rtspCancel = null;
            codec = null;

            base.Dispose(disposing);
        }

        void OnVideoViewPrepared(object sender, EventArgs args)
        {
            isPrepared = true;
            //((IVideoPlayerController)Element).Duration = TimeSpan.FromMilliseconds(videoView.Duration);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VideoPlayer.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (args.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (args.PropertyName == VideoPlayer.PositionProperty.PropertyName)
            {
                //if (Math.Abs(videoView.CurrentPosition - Element.Position.TotalMilliseconds) > 1000)
                {
                  //  videoView.SeekTo((int)Element.Position.TotalMilliseconds);
                }
            }
        }

        void SetAreTransportControlsEnabled()
        {
            if (Element.AreTransportControlsEnabled)
            {
                mediaController = new MediaController(Context);
              //  mediaController.SetMediaPlayer(videoView);
              //  videoView.SetMediaController(mediaController);
            }
            else
            {
              //  videoView.SetMediaController(null);

                if (mediaController != null)
                {
                    mediaController.SetMediaPlayer(null);
                    mediaController = null;
                }
            }
        }

        void SetSource()
        {
            Console.WriteLine("SetSource");
            isPrepared = false;
            bool hasSetSource = false;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!String.IsNullOrWhiteSpace(uri))
                {
              //      videoView.SetVideoURI(Android.Net.Uri.Parse(uri));
                    hasSetSource = true;
                }
            }
            else if (Element.Source is FileVideoSource)
            {
                string filename = (Element.Source as FileVideoSource).File;

                if (!String.IsNullOrWhiteSpace(filename))
                {
               //     videoView.SetVideoPath(filename);
                    hasSetSource = true;
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string package = Context.PackageName;
                string path = (Element.Source as ResourceVideoSource).Path;

                if (!String.IsNullOrWhiteSpace(path))
                {
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                    string uri = "android.resource://" + package + "/raw/" + filename;
                //    videoView.SetVideoURI(Android.Net.Uri.Parse(uri));
                    hasSetSource = true;
                }
            }
              
            if (hasSetSource && Element.AutoPlay)
            {
              //  videoView.Start();
            }
        }

        // Event handler to update status
        void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoStatus status = VideoStatus.NotReady;

            if (isPrepared)
            {
             //   status = videoView.IsPlaying ? VideoStatus.Playing : VideoStatus.Paused;
            }

            ((IVideoPlayerController)Element).Status = status;

            // Set Position property
          //  TimeSpan timeSpan = TimeSpan.FromMilliseconds(videoView.CurrentPosition);
          //  ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, timeSpan);
        }

        // Event handlers to implement methods
        void OnPlayRequested(object sender, EventArgs args)
        {
          //  videoView.Start();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
           // videoView.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
          //  videoView.StopPlayback();
        }
    }
}