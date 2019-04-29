using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Core.Geometry;
using MissionPlanner.Utilities.Drawing;
using OpenTK.Graphics;
using OpenTK.Input;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;

namespace Xamarin.Controls
{
    public class Context
    {
        public void MakeCurrent(object o)
        {

        }
    }

    public class Cursors
    {
        public static object Hand { get; set; }
        public object Current { get; set; }
    }

    public class MouseEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public abstract class MySKGLView : SKGLView
    {

        public MySKGLView():base()
        {
            EnableTouchEvents = true;
            Touch+= OnTouch;

            OnLoad(null);
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            var mouse = new MouseEventArgs() {X = (int) e.Location.X, Y = (int) e.Location.Y};

            if (e.ActionType == SKTouchAction.Moved)
                OnMouseMove(mouse);

            if (e.ActionType == SKTouchAction.Pressed)
                OnMouseClick(mouse);

            InvalidateSurface();
        }

        public Color BackColor { get; set; } = Color.Transparent;

        public Rectangle ClientRectangle
        {
            get { return Rectangle.FromLTRB(0, 0, (int)this.Width, (int)this.Height); }
        }

        public Size ClientSize { get; set; }

        public Cursors Cursor { get; set; } = new Cursors();

        public Cursors DefaultCursor { get; set; }

        public bool DesignMode { get; set; } = false;

        public GraphicsMode GraphicsMode { get; set; } = GraphicsMode.Default;

        public new int Height
        {
            get { return (int)base.Height; }
            set { }
        }

        public bool IsHandleCreated { get; set; } = true;

        public string Name { get; set; } = "";

        public bool Visible { get; set; } = true;

        public bool VSync { get; set; } = true;

        public new int Width
        {
            get { return (int)base.Width;}
            set { }
        }
        public virtual void Refresh()
        {
            //throw new NotImplementedException();
        }

        protected void Invalidate()
        {
            //throw new NotImplementedException();
        }

        protected void MakeCurrent()
        {

        }

        protected virtual void OnHandleCreated(EventArgs e)
        {
            // throw new NotImplementedException();
        }

        protected virtual void OnHandleDestroyed(EventArgs eventArgs)
        {
            // throw new NotImplementedException();
        }

        protected virtual void OnLoad(EventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(SKColors.AliceBlue);
            base.OnPaintSurface(e);
            var sk = new Graphics(e.Surface);
            OnPaint(new PaintEventArgs(sk, ClientRectangle));
            
        }
        protected virtual void OnResize(EventArgs eventArgs)
        {
            // throw new NotImplementedException();
        }

        protected void SwapBuffers()
        {
            
        }
    }
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
