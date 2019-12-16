using System;
using System.IO;

namespace MissionPlanner.Drawing
{
    public class Icon: Bitmap
    {
        public Icon(Icon value, int i, int i1): base(value,i,i1)
        {
        }

        public Icon(MemoryStream value): base(value)
        {
          
        }

        public Icon(Stream stream) : base(stream)
        {
        }

        public IntPtr Handle
        {
            get
            {
                return base.nativeSkBitmap.Handle; }
        }

        public Bitmap ToBitmap()
        {
            return (Bitmap)this;
        }
    }
}