using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Text;



namespace GMap.NET.WindowsForms
{
    public static class Extensions2
    {
        public static Bitmap ToBitmap(this byte[] input)
        {
            return (Bitmap)Image.FromStream(new MemoryStream(input));
        }
    }
}
