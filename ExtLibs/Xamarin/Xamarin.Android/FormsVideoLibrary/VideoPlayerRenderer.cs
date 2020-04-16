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
using RtspClientSharp;
using System.Threading;
using RtspClientSharp.Rtsp;
using System.Threading.Tasks;
using RtspClientSharp.RawFrames.Video;
using RtspClientSharp.RawFrames.Audio;
using static Android.Media.MediaCodec;
using System.Collections.Generic;

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
        private RtspClient rtspClient;
        private bool iframestart;
        private CancellationTokenSource rtspCancel;

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
            if (codec == null)
            {
                codec = MediaCodec.CreateDecoderByType("video/avc");

                callbacks = new CallBacks(this);

                codec.SetCallback(callbacks);

                var mediafmt = MediaFormat.CreateVideoFormat(MediaFormat.MimetypeVideoAvc, 1920, 1080);

                codec.Configure(mediafmt, videoView.Holder.Surface, null, MediaCodecConfigFlags.None);

                codec.Start();

                rtspClientStart();
            }

            base.OnDraw(canvas);
        }


        public async void rtspClientStart()
        {
            rtspClient = new RtspClientSharp.RtspClient(new RtspClientSharp.ConnectionParameters(new Uri("rtsp://192.168.0.10:8554/H264Video"))
            {
                RtpTransport = RtpTransportProtocol.TCP,
                ConnectTimeout = TimeSpan.FromSeconds(3),
                ReceiveTimeout = TimeSpan.FromSeconds(3)
            });

            rtspClient.FrameReceived += RtspClient_FrameReceived;

            rtspCancel = new CancellationTokenSource();

            int delay = 200;

            using (rtspClient)
                while (true)
                {
                    if (rtspCancel.Token.IsCancellationRequested)
                        return;

                    try
                    {
                        await rtspClient.ConnectAsync(rtspCancel.Token);
                        iframestart = true;
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (RtspClientException e)
                    {
                        Console.WriteLine(e.ToString());

                        await Task.Delay(delay, rtspCancel.Token);
                        continue;
                    }
                    Console.WriteLine("Connected.");

                    try
                    {
                        await rtspClient.ReceiveAsync(rtspCancel.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (RtspClientException e)
                    {
                        Console.WriteLine(e.ToString());
                        await Task.Delay(delay, rtspCancel.Token);
                    }
                }
        }

        private void RtspClient_FrameReceived(object sender, RtspClientSharp.RawFrames.RawFrame e)
        {
            if (rtspCancel.Token.IsCancellationRequested)
                return;
            //Console.WriteLine("Got Frame " + e.ToString());

            switch (e)
            {
                case RawH264IFrame h264IFrame:
                    {                      
                        if (callbacks.buffers.Count == 0)
                            return;
                        var index = callbacks.buffers.Pop();
                        var buffer = codec.GetInputBuffer(index);
                        buffer.Clear();
                        buffer.Put(h264IFrame.SpsPpsSegment.Array);
                        codec.QueueInputBuffer(index, 0, h264IFrame.SpsPpsSegment.Count, 0, MediaCodecBufferFlags.CodecConfig);

                        if (callbacks.buffers.Count == 0)
                            return;
                        index = callbacks.buffers.Pop();
                        buffer = codec.GetInputBuffer(index);
                        buffer.Clear();
                        buffer.Put(h264IFrame.FrameSegment.Array);
                        codec.QueueInputBuffer(index, 0, h264IFrame.FrameSegment.Count, 0, MediaCodecBufferFlags.None);

                        iframestart = false;
                        break;
                    }
                case RawH264PFrame h264PFrame:
                    {
                        if (iframestart)
                            return;
                        if (callbacks.buffers.Count == 0)
                            return;
                        var index = callbacks.buffers.Pop();
                        var buffer = codec.GetInputBuffer(index);
                        buffer.Clear();
                        buffer.Put(h264PFrame.FrameSegment.Array);
                        codec.QueueInputBuffer(index, 0, h264PFrame.FrameSegment.Count, 0, 0);
                        break;
                    }
                case RawJpegFrame jpegFrame:
                case RawAACFrame aacFrame:
                case RawG711AFrame g711AFrame:
                case RawG711UFrame g711UFrame:
                case RawPCMFrame pcmFrame:
                case RawG726Frame g726Frame:
                    break;
            }
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
            codec.Stop();
            codec.Dispose();

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