using System;
using System.Drawing;
using MissionPlanner.Utilities.Drawing;

namespace System.Windows.Forms
{
    public class Control
    {
        public int Width { get; internal set; } = 100;
        public int Height { get; internal set; } = 100;

        internal void DrawToBitmap(Bitmap bmp, Rectangle rectangle)
        {
            //throw new NotImplementedException();
        }

        internal void Refresh()
        {
            //throw new NotImplementedException();
        }
    }
}