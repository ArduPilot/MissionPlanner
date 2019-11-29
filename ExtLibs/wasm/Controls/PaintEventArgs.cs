using MissionPlanner.Drawing;
using System;
using System.Drawing;
using wasm;


namespace MissionPlanner.Drawing
{
    public class Form
    {
        public static Point MousePosition { get; set; }
        public static Keys ModifierKeys { get; set; }
    }

    public class PaintEventArgs : EventArgs
    {
        private Rectangle clientRectangle;
        private CanvasGraphics gg;
        public PaintEventArgs(CanvasGraphics gg, Rectangle clientRectangle)
        {
            this.gg = gg;
            this.clientRectangle = clientRectangle;
        }

        public CanvasGraphics Graphics => gg;
    }
}