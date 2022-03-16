using Android.Graphics;
using Android.Media;
using Android.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ExifLibrary;

namespace Xamarin.Droid
{
    public class AndroidVideo
    {
        private MediaCodec codec;
        private CallBacks callbacks;
        private bool h265;
        private bool h264;
        private CancellationTokenSource rtspCancel;

        public string url
        {
            get
            {
                return url1;
            }
            set {
                url1 = value;
            }
        }
        public bool RtspRunning { get => rtsprunning; set => rtsprunning = value; }

        private bool rtsprunning;

        private string url1 = "rtsp://192.168.0.10:8554/H264Video";

        public static event EventHandler<byte[]> onNewImage;

        public class CallBacks : MediaCodec.Callback
        {
            private AndroidVideo videoPlayerRenderer;
            MediaFormat mOutputFormat;
            internal Stack<int> buffers = new Stack<int>();

            public CallBacks(AndroidVideo videoPlayerRenderer)
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
                var buffer = codec.GetOutputBuffer(index);
                Console.WriteLine("OnOutputBufferAvailable");

                //var img = codec.GetOutputImage(index);

                buffer.Position(info.Offset);
                buffer.Limit(info.Offset + info.Size);
                var datain = new byte[buffer.Remaining()];
                var data = new byte[buffer.Remaining()];
                buffer.Get(datain);
                
                /*
I420: YYYYYYYY UU VV =>YUV420P
YV12: YYYYYYYY VV UU =>YUV420P
NV12: YYYYYYYY UVUV =>YUV420SP
NV21: YYYYYYYY VUVU =>YUV420SP
                 */

                I420toNV21(datain, data, 1920, 1080);

                var yuvimage = new YuvImage(data, ImageFormatType.Nv21, 1920, 1080, null);
                var jpg = new MemoryStream();
                yuvimage.CompressToJpeg(new Rect(0, 0, 1920, 1080), 100, jpg);

                //File.WriteAllBytes("/tmp/testin.yuv", datain);
                //File.WriteAllBytes("/tmp/test.yuv", data);

                onNewImage?.Invoke(this, jpg.ToArray());

                codec.ReleaseOutputBuffer(index, false);
            }

            /** Convert I420 (YYYYYYYY:UU:VV) to NV21 (YYYYYYYYY:VUVU) */
            public byte[] I420toNV21(byte[] input, byte[] output, int width, int height)
            {
                if (output == null)
                {
                    output = new byte[input.Length];
                }
                int size = width * height;
                int quarter = size / 4;
                int v0 = size + quarter;

                System.Array.Copy(input, 0, output, 0, size); // Y is same

                for (int u = size, v = v0, o = size; u < v0; u++, v++, o += 2)
                {
                    output[o] = input[v]; // For NV21, V first
                    output[o + 1] = input[u]; // For NV21, U second
                }
                return output;
            }

            public override void OnOutputFormatChanged(MediaCodec codec, MediaFormat format)
            {
                Console.WriteLine("OnOutputFormatChanged");
                mOutputFormat = format;
                var colformat = (MediaCodecCapabilities)mOutputFormat.GetInteger(MediaFormat.KeyColorFormat);
                var width = mOutputFormat.GetInteger(MediaFormat.KeyWidth);
                var height = mOutputFormat.GetInteger(MediaFormat.KeyHeight);
            }
        }

        public void Start()
        {
            if (rtspCancel == null)
            {
                rtspClientStart();
            }
        }

        void loop()
        {
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

                //videoView.Holder.Surface
                codec.Configure(mediafmt, null, null, MediaCodecConfigFlags.None);

                codec.Start();
            }
        }

        public void Stop()
        {
            rtspCancel.Cancel();
            codec.Stop();
        }


        public async void rtspClientStart()
        {
            rtspCancel = new CancellationTokenSource();

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
            c.Received_SPS_PPS += (byte[] sps, byte[] pps) =>
            {
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

            c.Received_VPS_SPS_PPS += (byte[] vps, byte[] sps, byte[] pps) =>
            {
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
                        fs_v.Write(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 0, 4); // Write Start Code
                        fs_v.Write(nal_unit, 0, nal_unit.Length); // Write NAL
                    }

                    if (callbacks == null || fs_v == null || callbacks.buffers.Count == 0)
                        return;
                    try
                    {
                        var index = callbacks.buffers.Pop();
                        var buffer = codec.GetInputBuffer(index);
                        buffer.Put(fs_v.ToArray());
                        codec.QueueInputBuffer(index, 0, (int)fs_v.Length, 0, MediaCodecBufferFlags.None);
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
                        {
                            c.Stop();
                            return;
                        }

                        c.Connect(url, RTSPClient.RTP_TRANSPORT.UDP);
                        var lastrtp = 0;
                        int cnt = 0;
                        while (!c.StreamingFinished())
                        {
                            rtsprunning = true;
                            loop();
                            Thread.Sleep(500);

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
    }
}