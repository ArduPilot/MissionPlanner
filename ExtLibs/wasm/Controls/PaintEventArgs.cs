extern alias MPDrawing;
using System;
using System.Drawing;
using IGraphics = MPDrawing::System.Drawing.IGraphics;

namespace MissionPlanner.Controls
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