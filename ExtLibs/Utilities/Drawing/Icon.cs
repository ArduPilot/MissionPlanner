using System;
using System.IO;

namespace MissionPlanner.Utilities.Drawing
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

        public Bitmap ToBitmap()
        {
            return (Bitmap)this;
        }
    }
}