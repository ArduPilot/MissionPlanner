using System;
using System.Drawing;
using MissionPlanner.Utilities.Drawing;

namespace Xamarin.Controls
{
    public class PaintEventArgs : EventArgs
    {
        private Rectangle clientRectangle;
        private IGraphics gg;
        public PaintEventArgs(IGraphics gg, Rectangle clientRectangle)
        {
            this.gg = gg;
            this.clientRectangle = clientRectangle;
        }

        public IGraphics Graphics => gg;
    }
}