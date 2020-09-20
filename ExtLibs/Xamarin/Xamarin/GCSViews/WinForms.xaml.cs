//extern alias MPLib;

using MissionPlanner;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = System.Windows.Forms.Application;
using Device = Xamarin.Forms.Device;
using Extensions = MissionPlanner.Utilities.Extensions;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace Xamarin.GCSViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WinForms : ContentPage
    {
        public WinForms()
        {
            InitializeComponent();
        }
        
        protected override void OnAppearing()
        {
            if (!start)
            {
                StartThreads();

                XplatUIMine.GetInstance().Keyboard = new Keyboard(Entry);
                start = true;
            }

            base.OnAppearing();
        }
        public class Keyboard: KeyboardXplat
        {
            private readonly Forms.Entry _inputView;

            private Entry view;
            private IntPtr _focusWindow;

            public Keyboard(Entry inputView)
            {
                _inputView = inputView;
            }

            public void FocusIn(IntPtr focusWindow)
            {
                FocusOut(_focusWindow);
                caretptr = IntPtr.Zero;
                _focusWindow = focusWindow;
            }

            private void View_TextChanged(object sender, TextChangedEventArgs e)
            {
                var current = Control.FromHandle(_focusWindow).Text;

                Console.WriteLine("TextChanged {0} {1} {2}", current, e.OldTextValue, e.NewTextValue);
            }

            public void FocusOut(IntPtr focusWindow)
            {
                _inputView.TextChanged -= View_TextChanged;
            }

            private IntPtr caretptr;
            public void SetCaretPos(CaretStruct caret, IntPtr handle, int x, int y)
            {
                if (_focusWindow == handle && caret.Hwnd == _focusWindow)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if(caretptr == handle)
                            return;

                        var focusctl = Control.FromHandle(_focusWindow);
                        var p = focusctl.PointToClient(Form.MousePosition);

                        if (focusctl.ClientRectangle.Contains(p))
                        {
                            // unbind
                            _inputView.Unfocused -= _inputView_Unfocused;                            
                            _inputView.TextChanged -= View_TextChanged;
                            _inputView.Completed -= _inputView_Completed;
                            // set                  
                            
                            _inputView.Text = focusctl.Text;
                            // rebind
                            _inputView.Completed += _inputView_Completed;
                            _inputView.TextChanged += View_TextChanged;
                            _inputView.Unfocused += _inputView_Unfocused;
                            //show
                            _inputView.IsVisible = true;
                            _inputView.Focus();                            

                             caretptr = handle;
                        }                      
                    });
            }

            private void _inputView_Completed(object sender, EventArgs e)
            {
                var focusctl = Control.FromHandle(_focusWindow);
                focusctl.Text = (sender as Entry)?.Text;
                 Device.BeginInvokeOnMainThread(() =>
                    {
                _inputView.IsVisible = false; });
            }

            private void _inputView_Unfocused(object sender, FocusEventArgs e)
            {
                caretptr = IntPtr.Zero;   
                         Device.BeginInvokeOnMainThread(() =>
                    {
                _inputView.IsVisible = false; });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (Application.OpenForms.Count > 1)
            {
                Application.OpenForms[Application.OpenForms.Count - 1].Close();
                XplatUIMine.PaintPending = true;
                return true;
            }
            else if (Application.OpenForms.Count == 1)
            {
                MainPage.Instance.DisplayAlert("", "Exit?", "Yes", "No").ContinueWith((result) =>
                {
                    if (result.Result)
                    {
                        Application.OpenForms[Application.OpenForms.Count - 1].Close();
                        XplatUIMine.PaintPending = true;
                    }
                });

                return true;
            }

            return false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void StartThreads()
        {
            size = Device.Info.ScaledScreenSize;
            size = Device.Info.PixelScreenSize;

            size = new Forms.Size(950, 536); // 1.77
            scale = new Forms.Size((Device.Info.PixelScreenSize.Width / size.Width),
                (Device.Info.PixelScreenSize.Height / size.Height));

            XplatUIMine.GetInstance()._virtualScreen = new Rectangle(0, 0, (int) size.Width, (int) size.Height);
            XplatUIMine.GetInstance()._workingArea = new Rectangle(0, 0, (int) size.Width, (int) size.Height);

            winforms = new Thread(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.Idle += (sender, args) => Thread.Sleep(0);

                MissionPlanner.Program.Main(new string[0]);
             
            });
            winforms.Start();

            Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000/30), () =>
            {
                Monitor.Enter(XplatUIMine.paintlock);
                if (XplatUIMine.PaintPending)
                {
                    scale = new Forms.Size( (SkCanvasView.CanvasSize.Width / size.Width),
                        (SkCanvasView.CanvasSize.Height / size.Height));

                    XplatUIMine.GetInstance()._virtualScreen = new Rectangle(0, 0, (int) size.Width, (int) size.Height);
                    XplatUIMine.GetInstance()._workingArea = new Rectangle(0, 0, (int) size.Width, (int) size.Height);

                    this.SkCanvasView.InvalidateSurface();
                    XplatUIMine.PaintPending = false;
                }
                Monitor.Exit(XplatUIMine.paintlock);

                return true;
            });
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }

        float Magnitude(SKPoint point)
        {
            return (float)Math.Sqrt(Math.Pow(point.X, 2) + Math.Pow(point.Y, 2));
        }

        //Double-clicking the left mouse button actually generates a sequence of four messages: WM_LBUTTONDOWN, WM_LBUTTONUP, WM_LBUTTONDBLCLK, and WM_LBUTTONUP.
        DateTime LastPressed = DateTime.MinValue;

        private int LastPressedX;
        private int LastPressedY;
        private Forms.Size size;
        private Forms.Size scale;
        private Dictionary<long, TouchInfo> touchDictionary = new Dictionary<long, TouchInfo>(10);
        static bool start;

        private void SkCanvasView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            try
            {
                var x = (int) (e.Location.X / scale.Width);
                var y = (int) (e.Location.Y / scale.Height);

                Console.WriteLine(Extensions.ToJSON(e, Formatting.None));
                XplatUIMine.PaintPending = true;

                if (e.ActionType == SKTouchAction.Moved)
                {
                    if (touchDictionary.ContainsKey(e.Id))
                    {
                        if (Math.Abs(touchDictionary[e.Id].atdown.Location.X / scale.Width - x) > 5 &&
                            Math.Abs(touchDictionary[e.Id].atdown.Location.Y / scale.Height - y) > 5)
                        {
                            //Console.WriteLine("Mouse has moved");
                            touchDictionary[e.Id].hasmoved = true;
                        }

                        touchDictionary[e.Id].prev = touchDictionary[e.Id].now;
                        touchDictionary[e.Id].now = e;
                    }

                    if (touchDictionary.Count >= 2)
                    {
                        // Copy two dictionary keys into array
                        long[] keys = new long[touchDictionary.Count];
                        touchDictionary.Keys.CopyTo(keys, 0);

                        // Find index non-moving (pivot) finger
                        int pivotIndex = (keys[0] == e.Id) ? 1 : 0;

                        // Get the three points in the transform
                        SKPoint pivotPoint = touchDictionary[keys[pivotIndex]].atdown.Location;
                        SKPoint prevPoint = touchDictionary[e.Id].atdown.Location;
                        SKPoint newPoint = e.Location;

                        // Calculate two vectors
                        SKPoint oldVector = prevPoint - pivotPoint;
                        SKPoint newVector = newPoint - pivotPoint;

                        SKPoint center = (pivotPoint + prevPoint);
                        center.X /= 2;
                        center.Y /= 2;

                        // Find angles from pivot point to touch points
                        float oldAngle = (float) Math.Atan2(oldVector.Y, oldVector.X);
                        float newAngle = (float) Math.Atan2(newVector.Y, newVector.X);

                        float scale1 = Magnitude(newVector) / Magnitude(oldVector);

                        if (!float.IsNaN(scale1) && !float.IsInfinity(scale1))
                        {
                            //var centre = pivotPoint;
                            x = (int) (center.X / scale.Width);
                            y = (int) (center.Y / scale.Height);

                            Console.WriteLine("scale: {0} {1} {2}", scale, newVector.Length, oldVector.Length);
                            if (scale1 >= 2)
                            {
                                XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEWHEEL,
                                    new IntPtr((int) (1) << 16),
                                    (IntPtr) ((y) << 16 | (x)));
                                touchDictionary[e.Id].atdown = e;
                            }

                            if (scale1 <= 0.5)
                            {
                                XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEWHEEL,
                                    new IntPtr((int) (-1) << 16),
                                    (IntPtr) ((y) << 16 | (x)));
                                touchDictionary[e.Id].atdown = e;
                            }
                        }

                        e.Handled = true;
                        return;
                    }

                    if (e.InContact)
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE,
                            new IntPtr((int) MsgButtons.MK_LBUTTON),
                            (IntPtr) ((y) << 16 | (x)));
                    }
                    else
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(),
                            (IntPtr) ((y) << 16 | (x)));
                    }

                }

                if (e.ActionType == SKTouchAction.Pressed && e.MouseButton == SKMouseButton.Left)
                {
                    var now = DateTime.Now;
                    touchDictionary.Add(e.Id, new TouchInfo() {now = e, prev = e, DownTime = now, atdown = e});

                    // right click handler
                    Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
                    {
                        /*
                         Console.WriteLine("Mouse rightclick check true={0} 1={1} {2} {3} {4}",                         
                            touchDictionary.ContainsKey(e.Id),
                            touchDictionary.Count, 
                            touchDictionary.ContainsKey(e.Id) ? touchDictionary[e.Id] : null, now, DateTime.Now);
                        */
                        if(touchDictionary.ContainsKey(e.Id) && touchDictionary.Count == 1)
                            if (!touchDictionary[e.Id].hasmoved && touchDictionary[e.Id].DownTime == now)
                            {
                                touchDictionary[e.Id].wasright = true;
                                XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONDOWN,
                                    new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));
                                XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONUP,
                                    new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));
                                touchDictionary.Remove(e.Id);
                                return false;
                            }
                        return false;
                    });

                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr)((y) << 16 | (x)));

                    if (LastPressed.AddMilliseconds(500) > DateTime.Now && Math.Abs(LastPressedX - x) < 20 &&
                        Math.Abs(LastPressedY - y) < 20)
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONDBLCLK, new IntPtr((int) MsgButtons.MK_LBUTTON),
                            (IntPtr) ((y) << 16 | (x)));
                        LastPressed = DateTime.MinValue;
                    }
                    else
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONDOWN,
                            new IntPtr((int) MsgButtons.MK_LBUTTON), (IntPtr) ((y) << 16 | (x)));
                }

                if (e.ActionType == SKTouchAction.Released && e.MouseButton == SKMouseButton.Left)
                {
                    if (touchDictionary.ContainsKey(e.Id) && touchDictionary[e.Id].wasright)
                    {
                        // no action here
                    }
                    else
                    {
                        // only up if we have seen the down
                        if(touchDictionary.ContainsKey(e.Id))
                            XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONUP,
                            new IntPtr((int) MsgButtons.MK_LBUTTON), (IntPtr) ((y) << 16 | (x)));
                    }

                    LastPressed = DateTime.Now;
                    LastPressedX = x;
                    LastPressedY = y;
                    touchDictionary.Remove(e.Id);
                }

                if (e.ActionType == SKTouchAction.Entered)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr) ((y) << 16 | (x)));
                    touchDictionary.Clear();
                }

                if (e.ActionType == SKTouchAction.Cancelled)
                {
                    touchDictionary.Clear();
                }

                if (e.ActionType == SKTouchAction.Exited)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr) ((y) << 16 | (x)));
                    touchDictionary.Clear();
                }

                e.Handled = true;

            } catch {}
        }

        private void SkCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SkCanvasView_PaintSurface(sender, new SKPaintGLSurfaceEventArgs(e.Surface, null));
        }

        private void SkCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintGLSurfaceEventArgs e) // .SKPaintSurfaceEventArgs // SKPaintGLSurfaceEventArgs
        {
            try
            {

                var surface = e.Surface;

                e.Surface.Canvas.Clear(SKColors.Gray);

                surface.Canvas.DrawCircle(0, 0, 50, new SKPaint() {Color = SKColor.Parse("ff0000")});

                e.Surface.Canvas.Scale((float) scale.Width, (float) scale.Height);

                Func<IntPtr, bool> func = null;
                func = (handle) =>
                {
                    var hwnd = Hwnd.ObjectFromHandle(handle);

                    var x = 0;
                    var y = 0;

                    XplatUI.driver.ClientToScreen(hwnd.client_window, ref x, ref y);

                    var width = 0;
                    var height = 0;
                    var client_width = 0;
                    var client_height = 0;


                    if (hwnd.hwndbmp != null && hwnd.Mapped && hwnd.Visible && !hwnd.zombie)
                    {
                        // setup clip
                        var parent = hwnd;
                        surface.Canvas.ClipRect(
                            SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width,
                                Screen.PrimaryScreen.Bounds.Height), (SKClipOperation) 5);

                        while (parent != null)
                        {
                            var xp = 0;
                            var yp = 0;
                            XplatUI.driver.ClientToScreen(parent.client_window, ref xp, ref yp);

                            surface.Canvas.ClipRect(SKRect.Create(xp, yp, parent.Width, parent.Height),
                                SKClipOperation.Intersect);
                            /*
                            surface.Canvas.DrawRect(xp, yp, parent.Width, parent.Height,
                                new SKPaint()
                                {

                                    Color = new SKColor(255, 0, 0),
                                    Style = SKPaintStyle.Stroke


                                });
                            */
                            parent = parent.parent;
                        }

                        Monitor.Enter(XplatUIMine.paintlock);

                        if (hwnd.ClientWindow != hwnd.WholeWindow)
                        {
                            var frm = Control.FromHandle(hwnd.ClientWindow) as Form;

                            Hwnd.Borders borders = new Hwnd.Borders();

                            if (frm != null)
                            {
                                borders = Hwnd.GetBorders(frm.GetCreateParams(), null);

                                surface.Canvas.ClipRect(
                                    SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width,
                                        Screen.PrimaryScreen.Bounds.Height), (SKClipOperation) 5);
                            }

                            if (surface.Canvas.DeviceClipBounds.Width > 0 &&
                                surface.Canvas.DeviceClipBounds.Height > 0)
                            {
                                surface.Canvas.DrawImage(hwnd.hwndbmpNC,
                                    new SKPoint(x - borders.left, y - borders.top),
                                    new SKPaint() {FilterQuality = SKFilterQuality.Low});

                                surface.Canvas.ClipRect(
                                    SKRect.Create(x, y, hwnd.width - borders.right - borders.left,
                                        hwnd.height - borders.top - borders.bottom), SKClipOperation.Intersect);
                                
                                surface.Canvas.DrawImage(hwnd.hwndbmp,
                                    new SKPoint(x, y),
                                    new SKPaint() {FilterQuality = SKFilterQuality.Low});
                            } 
                            else
                            {
                                Monitor.Exit(XplatUIMine.paintlock);
                                return true;
                            }
                        }
                        else
                        {
                            if (surface.Canvas.DeviceClipBounds.Width > 0 &&
                                surface.Canvas.DeviceClipBounds.Height > 0)
                            {
                                surface.Canvas.DrawImage(hwnd.hwndbmp,
                                    new SKPoint(x + 0, y + 0),
                                    new SKPaint() {FilterQuality = SKFilterQuality.Low});
                            } 
                            else
                            {
                                Monitor.Exit(XplatUIMine.paintlock);
                                return true;
                            }
                        }

                        Monitor.Exit(XplatUIMine.paintlock);
                    }

                    //surface.Canvas.DrawText(x + " " + y, x, y+10, new SKPaint() { Color =  SKColors.Red});

                    if (hwnd.Mapped && hwnd.Visible)
                    {
                        var enumer = Hwnd.windows.GetEnumerator();
                        while (enumer.MoveNext())
                        {
                            var hwnd2 = (System.Collections.DictionaryEntry) enumer.Current;
                            var Key = (IntPtr) hwnd2.Key;
                            var Value = (Hwnd) hwnd2.Value;
                            if (Value.ClientWindow == Key && Value.Parent == hwnd && Value.Visible && Value.Mapped)
                                func(Value.ClientWindow);
                        }
                    }

                    return true;
                };

                foreach (Form form in Application.OpenForms)
                {
                    if (form.IsHandleCreated)
                    {
                        if (form is MainV2 && form.WindowState != FormWindowState.Maximized)
                            form.WindowState = FormWindowState.Maximized;

                        if (form.WindowState == FormWindowState.Maximized)
                        {
                            var border = Hwnd.GetBorders(form.GetCreateParams(), null);

                            //XplatUI.driver.SetWindowPos(form.Handle, 0, 0, (int) Screen.PrimaryScreen.Bounds.Width + border.right + border.left,                            (int) Screen.PrimaryScreen.Bounds.Height + border.top + border.bottom);
                        }
                        else
                        {
                            if (form.Location.X < 0 || form.Location.Y < 0)
                            {
                                form.Location = new Point(Math.Max(form.Location.X, 0), Math.Max(form.Location.Y, 0));
                            }

                            var border = Hwnd.GetBorders(form.GetCreateParams(), null);

                            if (form.Size.Width > Screen.PrimaryScreen.Bounds.Width ||
                                form.Size.Height > Screen.PrimaryScreen.Bounds.Height)
                            {
                                //form.Size = new System.Drawing.Size((int) Screen.PrimaryScreen.Bounds.Width, (int) Screen.PrimaryScreen.Bounds.Height);
                                XplatUI.driver.SetWindowPos(form.Handle, 0, 0, (int) Screen.PrimaryScreen.Bounds.Width,
                                    (int) Screen.PrimaryScreen.Bounds.Height);
                            }
                        }

                        try
                        {
                            func(form.Handle);
                        }
                        catch
                        {

                        }
                    }
                }

                foreach (Hwnd hw in Hwnd.windows.Values)
                {
                    if (hw.topmost && hw.Mapped && hw.Visible)
                    {
                        var ctlmenu = Control.FromHandle(hw.ClientWindow);
                        if (ctlmenu != null)
                            func(hw.ClientWindow);
                    }
                }

                {
                    surface.Canvas.ClipRect(
                        SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width,
                            Screen.PrimaryScreen.Bounds.Height), (SKClipOperation) 5);

                    var path = new SKPath();
            
                    path.MoveTo(cursorPoints.First());
                    cursorPoints.ForEach(a => path.LineTo(a));
                    path.Transform(new SKMatrix(1, 0, XplatUI.driver.MousePosition.X, 0, 1,
                        XplatUI.driver.MousePosition.Y, 0, 0, 1));

                    surface.Canvas.DrawPath(path,
                        new SKPaint()
                            {Color = SKColors.White, Style = SKPaintStyle.Fill, StrokeJoin = SKStrokeJoin.Miter});
                    surface.Canvas.DrawPath(path,
                        new SKPaint()
                            {Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeJoin = SKStrokeJoin.Miter, IsAntialias = true});
                }

                surface.Canvas.Flush();

                return;

                surface.Canvas.ClipRect(new SKRect(0, 0, Screen.PrimaryScreen.Bounds.Right,
                    Screen.PrimaryScreen.Bounds.Bottom), (SKClipOperation) 5);

                surface.Canvas.DrawText("PixelScreenSize " + Device.Info.PixelScreenSize.ToString(),
                    new SKPoint(50, 10), new SKPaint() {Color = SKColor.Parse("ffff00")});


                surface.Canvas.DrawText("screen " + Screen.PrimaryScreen.ToString(), new SKPoint(50, 30),
                    new SKPaint() {Color = SKColor.Parse("ffff00")});

                int mx = 0, my = 0;
                XplatUI.driver.GetCursorPos(IntPtr.Zero, out mx, out my);

                surface.Canvas.DrawText("mouse " + XplatUI.driver.MousePosition.ToString(), new SKPoint(50, 50),
                    new SKPaint() {Color = SKColor.Parse("ffff00")});
                surface.Canvas.DrawText(mx + " " + my, new SKPoint(50, 70),
                    new SKPaint() {Color = SKColor.Parse("ffff00")});


                if (Application.OpenForms.Count > 0 &&
                    Application.OpenForms[Application.OpenForms.Count - 1].IsHandleCreated)
                {
                    var x = XplatUI.driver.MousePosition.X;
                    var y = XplatUI.driver.MousePosition.Y;

                    XplatUI.driver.ScreenToClient(Application.OpenForms[Application.OpenForms.Count - 1].Handle, ref x,
                        ref y);

                    var ctl = XplatUIMine.FindControlAtPoint(Application.OpenForms[Application.OpenForms.Count - 1],
                        new Point(x, y));
                    if (ctl != null)
                    {
                        XplatUI.driver.ScreenToClient(ctl.Handle, ref mx, ref my);
                        surface.Canvas.DrawText("client " + mx + " " + my, new SKPoint(50, 90),
                            new SKPaint() {Color = SKColor.Parse("ffff00")});

                        surface.Canvas.DrawText(ctl?.ToString(), new SKPoint(50, 130),
                            new SKPaint() {Color = SKColor.Parse("ffff00")});

                        var hwnd = Hwnd.ObjectFromHandle(ctl.Handle);

                        surface.Canvas.DrawText(ctl.Location.ToString(), new SKPoint(50, 150),
                            new SKPaint() {Color = SKColor.Parse("ffff00")});
                    }
                }

                surface.Canvas.DrawText("!", new SKPoint(XplatUI.driver.MousePosition.X,
                        XplatUI.driver.MousePosition.Y),
                    new SKPaint() {Color = SKColor.Parse("ffff00")});
            }
            catch
            {
            }
            finally
            {
               
            }
        }

        private SKPoint[] cursorPoints = new SKPoint[]
        {
            new SKPoint(0f,0f),
            new SKPoint(0f,16.512804f),
            new SKPoint(4.205124f,12.717936f),
            new SKPoint(7.589736f,19.99998f),
            new SKPoint(9.641016f,19.076904f),
            new SKPoint(6.256404f,11.79486f),
            new SKPoint(12.102552f,11.179476f),
            new SKPoint(0f,0f),
        };

        static private Thread winforms;
    }

    public class TouchInfo
    {
        public DateTime DownTime = DateTime.MinValue;
        public SKTouchEventArgs atdown;
        public SKTouchEventArgs prev;
        public SKTouchEventArgs now;
        public bool hasmoved = false;
        public bool wasright = false;
    }
}