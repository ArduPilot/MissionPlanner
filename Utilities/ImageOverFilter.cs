using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using DirectShow;
using DirectShow.BaseClasses;
using Sonic;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using Microsoft.Win32.SafeHandles;  

namespace MissionPlanner.Utilities
{

    // C:\Windows\Microsoft.NET\Framework\v2.0.50727\RegAsm.exe %1.dll /nologo /codebase /tlb: %1.tlbs


    #region Image Over Filter

    [ComVisible(true)]
    [Guid("53648b36-e268-4094-90d5-fec2ea8458b1")]
    [AMovieSetup(true)]
    //[PropPageSetup(typeof(OSDVideo))]
    public class ImageOverFilter : TransformFilter
    {
        OSDVideo vid = new OSDVideo();

        Bitmap lasthudcache = new Bitmap(5,5);
        double lasthudtime = 0;

        DateTime time = DateTime.MinValue;
        double datain = 0;
        double dataout = 0;
        double hudframes = 0;
        double frames = 0;
        double secondstaken = 0;
        double secondstaken1 = 0;
        double secondstaken2 = 0;
        double secondstaken3 = 0;
        double secondstaken4 = 0;
        double secondstaken5 = 0;
        double secondstaken6 = 0;

        private static class NativeMethods
        {
            [DllImport("kernel32.dll",
                   EntryPoint = "GetStdHandle",
                   SetLastError = true,
                   CharSet = CharSet.Auto,
                  CallingConvention = CallingConvention.StdCall)]
            internal static extern IntPtr GetStdHandle(int nStdHandle);
            [DllImport("kernel32.dll",
                EntryPoint = "AllocConsole",
                SetLastError = true,
                CharSet = CharSet.Auto,
                CallingConvention = CallingConvention.StdCall)]

            internal static extern int AllocConsole();
        }
       private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 437;  


        #region Constructor

        public ImageOverFilter()
            : base("CSharp Image Overlay Filter")
        {
            vid.DSplugin = true;



            NativeMethods.AllocConsole();
            IntPtr stdHandle = NativeMethods.GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
            StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Console.WriteLine("This text you can see in console window.");

            AMMediaType pmt = new AMMediaType() { majorType = MediaType.Video, subType = MediaSubType.YUY2, formatType = MediaType.Video, formatPtr = IntPtr.Zero };
            SetMediaType(PinDirection.Input, pmt);
            pmt.Free();
            
            pmt = new AMMediaType() { majorType = MediaType.Video, subType = MediaSubType.RGB24, formatType = MediaType.Video, formatPtr = IntPtr.Zero };
            SetMediaType(PinDirection.Output, pmt);
            pmt.Free();
        }

        #endregion

        #region Overridden Methods

        public override int CheckInputType(AMMediaType pmt)
        {
            Console.WriteLine("CheckInputType");

            if (pmt.majorType != MediaType.Video)
            {
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (pmt.subType != MediaSubType.YUY2)
            {
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (pmt.formatType != FormatType.VideoInfo)
            {
                return VFW_E_TYPE_NOT_ACCEPTED;
            }
            if (pmt.formatPtr == IntPtr.Zero)
            {
                return VFW_E_TYPE_NOT_ACCEPTED;
            }

            vid.Show();

            return NOERROR;
        }

        Bitmap converttobitmap(IntPtr ptrIn,IntPtr ptrOut,int length,int height, int width)
        {
            Console.WriteLine("converttobitmap " + DateTime.Now.Millisecond);

            byte[] dataOut = new byte[6];

            length = width * height * 3;

            int posin = 0;
            int posout = 0;
            /*
            try
            {
                string fn = frames.ToString("000000") + ".yuv2";
                using (BinaryWriter bw = new BinaryWriter(File.Open(fn, FileMode.Create)))
                {
                    for (; posin < length; posin++)
                    {
                        bw.Write(Marshal.ReadByte(ptrIn, posin + 0));
                    }
                    bw.Close();
                }

                frames++;
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }


                posin = 0;
            */          

            for (int g = 0; g < height; ++g)
            {
                for (int i = 0; i < width / 2; ++i)
                {
                    int y0 = Marshal.ReadByte(ptrIn, posin + 0);
                    int u0 = Marshal.ReadByte(ptrIn, posin + 1);
                    int y1 = Marshal.ReadByte(ptrIn, posin + 2);
                    int v0 = Marshal.ReadByte(ptrIn, posin + 3);
                    //ptrIn += 4;
                    int c = y0 - 16;
                    int d = u0 - 128;
                    int e = v0 - 128;
                    dataOut[0] = clip((298 * c + 516 * d + 128) >> 8); // blue
                    dataOut[1] = clip((298 * c - 100 * d - 208 * e + 128) >> 8); // green
                    dataOut[2] = clip((298 * c + 409 * e + 128) >> 8); // red
                    c = y1 - 16;
                    dataOut[3] = clip((298 * c + 516 * d + 128) >> 8); // blue
                    dataOut[4] = clip((298 * c - 100 * d - 208 * e + 128) >> 8); // green
                    dataOut[5] = clip((298 * c + 409 * e + 128) >> 8); // red
                    //ptrOut += 6;
                    Marshal.WriteByte(ptrOut, posout + 0, dataOut[0]);
                    Marshal.WriteByte(ptrOut, posout + 1, dataOut[1]);
                    Marshal.WriteByte(ptrOut, posout + 2, dataOut[2]);
                    Marshal.WriteByte(ptrOut, posout + 3, dataOut[3]);
                    Marshal.WriteByte(ptrOut, posout + 4, dataOut[4]);
                    Marshal.WriteByte(ptrOut, posout + 5, dataOut[5]);

                    posout += 6;
                    posin += 4;
                }
            }

            Bitmap ans = new Bitmap(width, height, width * 3, PixelFormat.Format24bppRgb, ptrOut);
            ans.RotateFlip(RotateFlipType.RotateNoneFlipY);

            Console.WriteLine("converttobitmap done " + DateTime.Now.Millisecond);
            return ans;
        }

        byte clip(int input)
        {
            if (input < 0)
                return 0;
            if (input > 255)
                return 255;
            return (byte)input;
        }

        public override int Transform(ref IMediaSampleImpl _input, ref IMediaSampleImpl _sample)
        {
            Console.WriteLine("Transform " + DateTime.Now.Millisecond);

            DateTime starttime = DateTime.Now;

            int lDataLength = _input.GetActualDataLength();
            IntPtr _ptrIn;
            IntPtr _ptrOut;

            BitmapInfoHeader _bmiIn = (BitmapInfoHeader)Input.CurrentMediaType;
            BitmapInfoHeader _bmiOut = (BitmapInfoHeader)Output.CurrentMediaType;

            _sample.SetActualDataLength(_bmiOut.GetBitmapSize());

            Console.WriteLine("inlen " + lDataLength + " outlen" + (_bmiIn.Width* _bmiIn.Height * 3) +" "+ DateTime.Now.Millisecond);

            _input.GetPointer(out _ptrIn);
            _sample.GetPointer(out _ptrOut);

            Bitmap _bmpOut = converttobitmap(_ptrIn, _ptrOut, lDataLength, _bmiIn.Height, _bmiIn.Width);

            Console.WriteLine("gotbitmap " + DateTime.Now.Millisecond);
  
            datain += _bmiIn.Height * _bmiIn.Width * 3;
            frames++;
            // Application.DoEvents();

            secondstaken1 += (DateTime.Now - starttime).TotalSeconds;

            long start, end;
            _input.GetTime(out start, out end);

            {
                //_bmpIn.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Graphics _graphics = Graphics.FromImage(_bmpOut);

                _graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                _graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;

                secondstaken2 += (DateTime.Now - starttime).TotalSeconds;

                try
                {
                    double videotime = (start / 10000000.0);

                    DateTime temp = vid.videopos;

                    vid.videopos = temp.AddSeconds(videotime);

                    Bitmap img;

                    // 10 hz
                    if (videotime > (lasthudtime + 0.1))
                    {
                        // get hud
                        img = vid.gethud(_bmpOut, videotime);
                        lasthudcache = (Bitmap)img.Clone();

                        lasthudtime = videotime;
                        hudframes++;
                    }
                    else
                    {
                        img = (Bitmap)lasthudcache.Clone();
                    }

                    secondstaken3 += (DateTime.Now - starttime).TotalSeconds;

                    //_graphics.DrawImage(_bmpOut, 0, 0);

                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);

                    //img.MakeTransparent();
                    secondstaken4 += (DateTime.Now - starttime).TotalSeconds;

                    _graphics.DrawImage(img, 0, 0, _bmpOut.Width, _bmpOut.Height);

                    secondstaken5 += (DateTime.Now - starttime).TotalSeconds;

                    img.Dispose();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

                _graphics.Dispose();
              //  _bmpIn.RotateFlip(RotateFlipType.RotateNoneFlipY);

            }
                secondstaken6 += (DateTime.Now - starttime).TotalSeconds;


                dataout += _bmpOut.Height * _bmpOut.Width * 3;

            _bmpOut.Dispose();

            secondstaken += (DateTime.Now - starttime).TotalSeconds;

            if (time.Second != DateTime.Now.Second)
            {
                vid.textbox = string.Format(
@"input: {0}
output: {1}
frames: {2}
hudframes: {3}
pos: {4}
start: {5}
end: {6}
seconds {7}
seconds1 {8}
seconds2 {9}
seconds3 {10}
seconds4 {11}
seconds5 {12}
seconds6 {13}", datain, dataout, frames, hudframes, vid.videopos, start / 10000000.0, end / 10000000.0, secondstaken, secondstaken1, secondstaken2, secondstaken3, secondstaken4, secondstaken5, secondstaken6);

                datain = dataout = frames = hudframes = 0;


                secondstaken = secondstaken1 = secondstaken2 = secondstaken3 = secondstaken4 = secondstaken5 = secondstaken6 = 0;
                time = DateTime.Now;
            }

            Console.WriteLine("Transform Done " + DateTime.Now.Millisecond);

            return S_OK;
        }

        public override int DecideBufferSize(ref IMemAllocatorImpl pAlloc, ref AllocatorProperties prop)
        {
            Console.WriteLine("DecideBufferSize");

            if (!Output.IsConnected) return VFW_E_NOT_CONNECTED;
            if (Output.CurrentMediaType.majorType != MediaType.Video) return VFW_E_INVALIDMEDIATYPE;
            AllocatorProperties _actual = new AllocatorProperties();
            BitmapInfoHeader _bmi = (BitmapInfoHeader)Output.CurrentMediaType;
            if (_bmi == null) return VFW_E_INVALIDMEDIATYPE;
            prop.cbBuffer = _bmi.GetBitmapSize();
            if (prop.cbBuffer < _bmi.ImageSize)
            {
                prop.cbBuffer = _bmi.ImageSize;
            }
            prop.cBuffers = 1;
            int hr = pAlloc.SetProperties(prop, _actual);
            return hr;
        }

        /*
        public override int OnReceive(ref IMediaSampleImpl _sample)
        {
            Console.WriteLine("OnReceive ");

            Output.Deliver(ref _sample);
           
            return S_OK;
        }
        */
        public override int GetMediaType(int iPosition, ref AMMediaType pMediaType)
        {
            Console.WriteLine("GetMediaType");
            if (iPosition > 0) return VFW_S_NO_MORE_ITEMS;
            if (pMediaType == null) return E_INVALIDARG;
            if (!Input.IsConnected) return VFW_E_NOT_CONNECTED;

            AMMediaType.Copy(Input.CurrentMediaType, ref pMediaType);

            VideoInfoHeader vhi = new VideoInfoHeader();
            Marshal.PtrToStructure(pMediaType.formatPtr, vhi);
            vhi.BmiHeader.Compression = 0;
            vhi.BmiHeader.BitCount = 24;
            vhi.BmiHeader.ImageSize = vhi.BmiHeader.Width * vhi.BmiHeader.Height * 3;
            pMediaType.formatPtr = Marshal.AllocCoTaskMem(pMediaType.formatSize);
            Marshal.StructureToPtr(vhi, pMediaType.formatPtr, false);

            pMediaType.majorType = MediaType.Video;
            pMediaType.subType = MediaSubType.RGB24;
            pMediaType.formatType = FormatType.VideoInfo;
            pMediaType.sampleSize = vhi.BmiHeader.ImageSize;

            return NOERROR;
        }

        public override int CheckTransform(AMMediaType mtIn, AMMediaType mtOut)
        {
            Console.WriteLine("CheckTransform");
            if (mtIn.subType == MediaSubType.YUY2 && mtOut.subType == MediaSubType.RGB24)
            {
                return NOERROR;
            }

            return VFW_E_INVALIDMEDIATYPE;

            //return AMMediaType.AreEquals(mtIn, mtOut) ? NOERROR : VFW_E_INVALIDMEDIATYPE;
        }

        /// <summary>
        /// Converts RGB to YUV.
        /// </summary>
        /// <param name="red">Red must be in [0, 255].</param>
        /// <param name="green">Green must be in [0, 255].</param>
        /// <param name="blue">Blue must be in [0, 255].</param>
        public static YUV RGBtoYUV(int red, int green, int blue)
        {
            YUV yuv = new YUV();

            // normalizes red, green, blue values
            double r = (double)red / 255.0;
            double g = (double)green / 255.0;
            double b = (double)blue / 255.0;

            yuv.Y = 0.299 * r + 0.587 * g + 0.114 * b;
            yuv.U = -0.14713 * r - 0.28886 * g + 0.436 * b;
            yuv.V = 0.615 * r - 0.51499 * g - 0.10001 * b;

            return yuv;
        }

        /// <summary>
        /// Structure to define YUV.
        /// </summary>
        public struct YUV
        {
            /// <summary>
            /// Gets an empty YUV structure.
            /// </summary>
            public static readonly YUV Empty = new YUV();

            private double y;
            private double u;
            private double v;

            public static bool operator ==(YUV item1, YUV item2)
            {
                return (
                    item1.Y == item2.Y
                    && item1.U == item2.U
                    && item1.V == item2.V
                    );
            }

            public static bool operator !=(YUV item1, YUV item2)
            {
                return (
                    item1.Y != item2.Y
                    || item1.U != item2.U
                    || item1.V != item2.V
                    );
            }

            public double Y
            {
                get
                {
                    return y;
                }
                set
                {
                    y = value;
                    y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
                }
            }

            public double U
            {
                get
                {
                    return u;
                }
                set
                {
                    u = value;
                    u = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
                }
            }

            public double V
            {
                get
                {
                    return v;
                }
                set
                {
                    v = value;
                    v = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
                }
            }

            /// <summary>
            /// Creates an instance of a YUV structure.
            /// </summary>
            public YUV(double y, double u, double v)
            {
                this.y = (y > 1) ? 1 : ((y < 0) ? 0 : y);
                this.u = (u > 0.436) ? 0.436 : ((u < -0.436) ? -0.436 : u);
                this.v = (v > 0.615) ? 0.615 : ((v < -0.615) ? -0.615 : v);
            }

            public override bool Equals(Object obj)
            {
                if (obj == null || GetType() != obj.GetType()) return false;

                return (this == (YUV)obj);
            }

            public override int GetHashCode()
            {
                return Y.GetHashCode() ^ U.GetHashCode() ^ V.GetHashCode();
            }

        }


        #endregion
    }

    #endregion
}
