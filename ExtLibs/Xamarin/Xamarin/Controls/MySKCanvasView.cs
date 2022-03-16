
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Xamarin.Forms;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;

using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = System.Drawing.Size;
using Timer = System.Threading.Timer;

namespace Xamarin.Controls
{
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public abstract class MySKCanvasView : SKCanvasView, IDisposable
    {
        private object EventMouseDown = new object();
        private object EventMouseLeave = new object();
        private object EventMouseMove = new object();
        private EventHandlerList events;
        private float mousewheeldelta = 0;
        private bool started = false;
        private Dictionary<long, SKPath> temporaryPaths = new Dictionary<long, SKPath>();
        private object EventMouseWheel = new object();
        private object EventMouseUp = new object();
        private object EventMouseEnter = new object();

        public MySKCanvasView() : base()
        {
            EnableTouchEvents = true;
            Touch += OnTouch;
            /*
            var pinch = new PinchGestureRecognizer();
            pinch.PinchUpdated += PinchUpdated;

            GestureRecognizers.Add(pinch);
            */
            base.MeasureInvalidated += MySKGLView_MeasureInvalidated;

            base.SizeChanged += MySKGLView_SizeChanged;

            OnLoad(null);
        }

        public object Invoke(Action p0)
        {
            Forms.Device.BeginInvokeOnMainThread(p0);
            return null;
        }

        public event MouseEventHandler MouseDown
        {
            add
            {
                Events.AddHandler(EventMouseDown, value);
            }
            remove
            {
                Events.RemoveHandler(EventMouseDown, value);
            }
        }

        public event EventHandler MouseLeave
        {
            add
            {
                Events.AddHandler(EventMouseLeave, value);
            }
            remove
            {
                Events.RemoveHandler(EventMouseLeave, value);
            }
        }

        public event MouseEventHandler MouseMove
        {
            add
            {
                Events.AddHandler(EventMouseMove, value);
            }
            remove
            {
                Events.RemoveHandler(EventMouseMove, value);
            }
        }

        public virtual Color BackColor { get; set; } = Color.Transparent;

        public Image BackgroundImage { get; set; }

        public virtual ImageLayout BackgroundImageLayout { get; set; }

        public Rectangle ClientRectangle
        {
            get { return Rectangle.FromLTRB(0, 0, (int)this.Width, (int)this.Height); }
        }

        public Size ClientSize { get; set; }

        public Cursor Cursor { get; set; } = Cursors.Arrow;

        public Cursor DefaultCursor { get; set; }

        public bool DesignMode { get; set; } = false;

        public bool DoubleBuffered { get; set; }

        public virtual System.Drawing.Font Font { get; set; } = SystemFonts.DefaultFont;

      //  public GraphicsMode GraphicsMode { get; set; } = GraphicsMode.Default;

        public new int Height
        {
            get { return (int)CanvasSize.Height; }
            set { }
        }

        public bool IsHandleCreated { get; set; } = true;

        public string Name { get; set; } = "";

        public bool ResizeRedraw { get; set; } = false;

        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {

            }
        }

        public bool Visible { get; set; } = true;

        public bool VSync { get; set; } = true;

        public new int Width
        {
            get { return (int)CanvasSize.Width; }
            set { }
        }

        protected EventHandlerList Events
        {
            get
            {
                if (events == null)
                {
                    events = new EventHandlerList();
                }
                return events;
            }
        }

        public bool InvokeRequired
        {
            get { return Forms.Device.IsInvokeRequired; }
        }

        public virtual void Dispose()
        {

        }

        public SKPoint PointToClient(Point pt)
        {
            return new SKPoint((float)(CanvasSize.Width * pt.X / this.Width),
                (float)(CanvasSize.Height * pt.Y / this.Height));
        }

        private bool pendingredraw = false;
        public virtual void Refresh()
        {
            if (!pendingredraw)
            {
                pendingredraw = true;
                Forms.Device.BeginInvokeOnMainThread(() => { InvalidateSurface(); });
            }
        }

        public void SetStyle(object optimizedDoubleBuffer, bool b)
        {

        }
        protected Graphics CreateGraphics()
        {
            return null;//new Graphics(base.GRContext.Handle, this.Width, this.Height);
        }

        protected virtual void Dispose(bool disposing)
        {
            Font?.Dispose();
            BackgroundImage?.Dispose();
        }

        protected void Invalidate()
        {
            if (_timer == null)
                _timer = new Timer(state => { Forms.Device.BeginInvokeOnMainThread(() => { InvalidateSurface(); }); },
                    null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

            if (pendingredraw)
            {
                _timer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(1000));
            }
        }

        protected void MakeCurrent()
        {

        }

        protected virtual void OnCreateControl()
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

        protected virtual void OnKeyDown(KeyEventArgs keyEventArgs)
        {
            // throw new NotImplementedException();
        }

        protected virtual void OnKeyUp(KeyEventArgs keyEventArgs)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnLoad(EventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EventMouseDown])?.Invoke(this, e);
        }

        protected virtual void OnMouseEnter(EventArgs e)
        {
            ((EventHandler)Events[EventMouseEnter])?.Invoke(this, e);
        }

        protected virtual void OnMouseLeave(EventArgs e)
        {
            ((EventHandler)Events[EventMouseLeave])?.Invoke(this, e);
        }

        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EventMouseMove])?.Invoke(this, e);
        }

        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EventMouseUp])?.Invoke(this, e);
        }


        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EventMouseWheel])?.Invoke(this, e);
        }


        protected virtual void OnPaint(PaintEventArgs e)
        {
           // ((PaintEventHandler)Events[EventPaint])?.Invoke(this, e);
        }


        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private DateTime lastrender = DateTime.MinValue;
        private Timer _timer;

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            //     base.OnPaintSurface(e);
            // }

            //  protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
            //  {

            //if (lastrender.AddMilliseconds(30) > DateTime.Now)
              //  return;

            base.OnPaintSurface(e);

            lastrender = DateTime.Now;
            var start = DateTime.Now;
            pendingredraw = false;
            if (!started)
            {
                started = true;
                MySKGLView_SizeChanged(null, null);
            }

            e.Surface.Canvas.Clear(SKColors.AliceBlue);
           
            var sk = new Graphics(e.Surface);
            try
            {
                OnPaint(new PaintEventArgs(sk, ClientRectangle));
                sk.Flush();
            }
            catch (Exception ex) { Debug.Write(ex); }
          
            //System.Diagnostics.Debug.WriteLine(this.GetType() + " OnPaintSurface " + (DateTime.Now - start).TotalSeconds);
        }

        protected virtual void OnResize(EventArgs eventArgs)
        {
            InvalidateSurface();
            // throw new NotImplementedException();
        }

        protected virtual void OnSizeChanged(EventArgs eventArgs)
        {
            //throw new NotImplementedException();
        }

        protected void ResumeLayout(bool b)
        {

        }

        protected void SuspendLayout()
        {

        }

        private void MySKGLView_MeasureInvalidated(object sender, EventArgs e)
        {

            InvalidateSurface();
        }

        private void MySKGLView_SizeChanged(object sender, EventArgs e)
        {
            try
            {
               // if (GRContext == null)
               //     return;

                if (CanvasSize != null)
                    OnResize(null);

                OnSizeChanged(null); 
            }
            catch (Exception ex)
            {

            }
        }

        private void OnTouch(object sender, SKTouchEventArgs e)
        {
            var mouse = new MouseEventArgs()
            {
                X = (int)e.Location.X,
                Y = (int)e.Location.Y,
                Button = e.MouseButton == SKMouseButton.Left ? MouseButtons.Left :
                    e.MouseButton == SKMouseButton.Unknown ? MouseButtons.None :
                    e.MouseButton == SKMouseButton.Right ? MouseButtons.Right : MouseButtons.Middle,
                Delta = 0 // mouse wheel
            };

            //Form.MousePosition = new Point((int)e.Location.X, (int)e.Location.Y);

            // pinching - multiple paths at once
            if (temporaryPaths.Count > 1)
            {
                //System.Diagnostics.Debug.WriteLine(temporaryPaths.ToJSON());

                var key1 = temporaryPaths.Keys.ElementAt(0);
                var key2 = temporaryPaths.Keys.ElementAt(1);

                var startdist = temporaryPaths[key1].Points.First() - temporaryPaths[key2].Points.First();
                var enddist = temporaryPaths[key1].Points.Last() - temporaryPaths[key2].Points.Last();

                var delta = enddist - startdist;



                //tru is smaller
                //System.Diagnostics.Debug.WriteLine(delta + " " + delta.Length + " " + (startdist.Length > enddist.Length));

                var m1 = new SKPoint(temporaryPaths[key1].Bounds.MidX, temporaryPaths[key1].Bounds.MidY);
                var m2 = new SKPoint(temporaryPaths[key2].Bounds.MidX, temporaryPaths[key2].Bounds.MidY);

                var mid = m1 - m2;

                mouse.X = (int)(m2.X + mid.X/2.0);
                mouse.Y = (int)(m2.Y + mid.Y/2.0);

                // set it only after length movement
                if (delta.Length > mousewheeldelta + 10 || delta.Length < mousewheeldelta - 10)
                {
                    mouse.Delta = (startdist.Length > enddist.Length) ? -1 : 1;

                    System.Diagnostics.Debug.WriteLine(delta + " " + delta.Length + " " +
                                                       (startdist.Length > enddist.Length) + " " +
                                                       ((startdist.Length > enddist.Length) ? -1 : 1));


                    mousewheeldelta = delta.Length;
                }
                OnMouseEnter(null);
                // set mouse centre on pinch
                OnMouseMove(mouse);
                // do the scroll
                OnMouseWheel(mouse);
                OnMouseLeave(null);
            }
            else
            {
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

                if (e.ActionType == SKTouchAction.Exited)
                    OnMouseLeave(null);
            }

            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    // start of a stroke
                    var p = new SKPath();
                    p.MoveTo(e.Location);
                    temporaryPaths[e.Id] = p;
                    Invalidate();
                    break;
                case SKTouchAction.Moved:
                    // the stroke, while pressed
                    if (e.InContact && temporaryPaths.ContainsKey(e.Id))
                        temporaryPaths[e.Id].LineTo(e.Location);
                    break;
                case SKTouchAction.Released:
                    // end of a stroke
                    //paths.Add(temporaryPaths[e.Id]);
                    temporaryPaths.Remove(e.Id);
                    Invalidate();
                    break;
                case SKTouchAction.Cancelled:
                    // we don't want that stroke
                    temporaryPaths.Remove(e.Id);
                    break;
            }

            //System.Diagnostics.Debug.WriteLine(e.ToJSON());

            // we have handled these events
            e.Handled = true;
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {

        }
    }
}
