using System;

namespace System.Drawing.Imaging


{
    public class BitmapData
    {
        public IntPtr Scan0 { get; set; }
        public int Stride { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}