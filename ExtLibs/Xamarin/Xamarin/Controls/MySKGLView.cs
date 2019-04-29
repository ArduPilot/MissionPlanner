using MissionPlanner.Utilities.Drawing;
using OpenTK.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using MissionPlanner.Utilities.Drawing;
using Graphics = MissionPlanner.Utilities.Drawing.Graphics;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using PointF = System.Drawing.PointF;
using RectangleF = System.Drawing.RectangleF;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
using SizeF = System.Drawing.SizeF;

namespace Xamarin.Controls
{
    public abstract class MySKGLView : SKGLView, IDisposable
    {
        private bool started = false;

        public void SetStyle(object optimizedDoubleBuffer, bool b)
        {

        }  
        public MySKGLView():base()
        {
            EnableTouchEvents = true;
            Touch+= OnTouch;

            base.MeasureInvalidated += MySKGLView_MeasureInvalidated;

            base.SizeChanged += MySKGLView_SizeChanged;

            OnLoad(null);
        }

        private void MySKGLView_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                OnSizeChanged(null);

                if(CanvasSize != null)
                    OnResize(null);
            }
            catch (Exception ex)
            {

            }
        }

        private void MySKGLView_MeasureInvalidated(object sender, EventArgs e)
        {

            InvalidateSurface();
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            var mouse = new MouseEventArgs()
            {
                X = (int) e.Location.X, Y = (int) e.Location.Y,
                Button = e.MouseButton == SKMouseButton.Left ? MouseButtons.Left : MouseButtons.Right
            };

            if (e.ActionType == SKTouchAction.Moved)
                OnMouseMove(mouse);

            if (e.ActionType == SKTouchAction.Pressed)
            {
                OnMouseDown(mouse);
            }

            if (e.ActionType == SKTouchAction.Released)
            {
                OnMouseUp(mouse);
                OnMouseClick(mouse);
            }

            if (e.ActionType == SKTouchAction.Entered)
            {
                OnMouseEnter(null);
            }

            if(e.ActionType == SKTouchAction.Exited)
                OnMouseLeave(null);


            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // start of a stroke
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    //temporaryPaths[e.Id] = p;
                    Invalidate();
                    break;
                case SKTouchAction.Moved:
                    // the stroke, while pressed
                   // if (e.InContact)
                       // temporaryPaths[e.Id].LineTo(e.Location);
                    break;
                case SKTouchAction.Released:
                    // end of a stroke
                    // paths.Add(temporaryPaths[e.Id]);
                    //temporaryPaths.Remove(e.Id);
                    Invalidate();
                    break;
                case SKTouchAction.Cancelled:
                    // we don't want that stroke
                   // temporaryPaths.Remove(e.Id);
                    break;
            }

            // we have handled these events
            e.Handled = true;
        }

        public virtual Color BackColor { get; set; } = Color.Transparent;

        public Rectangle ClientRectangle
        {
            get { return Rectangle.FromLTRB(0, 0, (int)this.Width, (int)this.Height); }
        }

        public Size ClientSize { get; set; }

        public Cursor Cursor { get; set; } = new Cursor();

        public Cursor DefaultCursor { get; set; }

        public bool DesignMode { get; set; } = false;

        public GraphicsMode GraphicsMode { get; set; } = GraphicsMode.Default;

        public new int Height
        {
            get { return (int)CanvasSize.Height; }
            set { }
        }

        public bool IsHandleCreated { get; set; } = true;

        public string Name { get; set; } = "";

        public bool Visible { get; set; } = true;

        public bool VSync { get; set; } = true;

        public new int Width
        {
            get { return (int)CanvasSize.Width;}
            set { }
        }

        public bool DoubleBuffered { get; set; }
        public Size Size {
            get { return new Size(Width,Height); }
            set {
                
            }
        }

        public virtual ImageLayout BackgroundImageLayout { get; set; }
        public virtual Font Font { get; set; } = SystemFonts.DefaultFont;
        public Image BackgroundImage { get; set; }

        public virtual void Refresh()
        {
            InvalidateSurface();
        }

        protected void Invalidate()
        {
            InvalidateSurface();
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
            if (!started)
            {
                started = true;
                MySKGLView_SizeChanged(null,null);
            }

            e.Surface.Canvas.Clear(SKColors.AliceBlue);
            base.OnPaintSurface(e);
            var sk = new Graphics(e.Surface);
            OnPaint(new PaintEventArgs(sk, ClientRectangle));
            
        }
        protected virtual void OnResize(EventArgs eventArgs)
        {
            InvalidateSurface();
            // throw new NotImplementedException();
        }


        protected void SuspendLayout()
        {
          
        }

        protected void ResumeLayout(bool b)
        {
           
        }

   

        protected virtual void OnKeyDown(KeyEventArgs keyEventArgs)
        {
           // throw new NotImplementedException();
        }

        protected virtual void OnKeyUp(KeyEventArgs keyEventArgs)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseDown(MouseEventArgs mouseEventArgs)
        {
           // throw new NotImplementedException();
        }

        protected virtual void OnMouseLeave(EventArgs eventArgs)
        {
           // throw new NotImplementedException();
        }

        protected virtual void OnMouseEnter(EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseWheel(MouseEventArgs mouseEventArgs)
        {
           // throw new NotImplementedException();
        }

        protected virtual void OnSizeChanged(EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            Font?.Dispose();
            BackgroundImage?.Dispose();
        }

        protected virtual void OnMouseUp(MouseEventArgs mouseEventArgs)
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        protected virtual void OnCreateControl()
        {
            
        }
    }
}
