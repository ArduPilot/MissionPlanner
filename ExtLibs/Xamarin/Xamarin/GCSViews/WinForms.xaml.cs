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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Acr.UserDialogs.Infrastructure;
using Microsoft.Scripting.Utils;
using MissionPlanner.Comms;
using MissionPlanner.GCSViews;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Application = System.Windows.Forms.Application;
using Device = Xamarin.Forms.Device;
using Extensions = MissionPlanner.Utilities.Extensions;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using MissionPlanner.Controls;

namespace Xamarin.GCSViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WinForms : ContentPage
    {
        readonly string TAG = "MP";

        static WinForms Instance;

        public WinForms()
        {
            InitializeComponent();

            size = Device.Info.ScaledScreenSize;
            size = Device.Info.PixelScreenSize;

            var scale = size.Width / size.Height; // 1.77 1.6  1.33


            size = new Forms.Size(540 * scale, 540);
            if (size.Width < 960)
                size = new Forms.Size(960, 960 / scale);
            
            Instance = this;
            MainV2.speechEngine = new Speech();

            // init seril port type
            SerialPort.DefaultType = (self, s, i) =>
            {
                return Task.Run(async () =>
                {
                    Log.Info(TAG, "SerialPort.DefaultType in " + s + " " + i);

                    // no valid portname to start
                    if (String.IsNullOrEmpty(s))
                    {
                        Log.Info(TAG, "SerialPort.DefaultType passthrough s = null");
                        return self._baseport;
                    }
                    else
                    {
                        var dil = await Test.UsbDevices.GetDeviceInfoList();

                        var di = dil.Where(a => a.board == s);

                        if (di.Count() > 0)
                        {
                            Log.Info(TAG, "SerialPort.DefaultType found device " + di.First().board + " search " + s);
                            return await Test.UsbDevices.GetUSB(di.First());
                        }
                    }

                    Log.Info(TAG, "SerialPort.DefaultType passthrough no board match");
                    return self._baseport;
                }).Result;
            };

            // report back device list
            SerialPort.GetCustomPorts = () =>
            {
                var list1 = Task.Run(async () =>
                {
                    var list = await Test.BlueToothDevice.GetDeviceInfoList();
                    return list.Select(a => a.board).ToList();
                }).Result;

                var list2 = Task.Run(async () =>
                {
                    var list = await Test.UsbDevices.GetDeviceInfoList();
                    return list.Select(a => a.board).ToList();
                }).Result;

                list1.AddRange(list2);
                return list1;
            };

            // support for fw upload
            MissionPlanner.GCSViews.ConfigurationView.ConfigFirmwareManifest.ExtraDeviceInfo += () =>
            {
                return Task.Run(async () => { return await Test.UsbDevices.GetDeviceInfoList(); }).Result;
            };

            MissionPlanner.GCSViews.ConfigurationView.ConfigFirmware.ExtraDeviceInfo += () =>
            {
                return Task.Run(async () => { return await Test.UsbDevices.GetDeviceInfoList(); }).Result;
            };
        }

        public static string BundledPath
        {
            get { return SITL.BundledPath; }
            set { SITL.BundledPath = value; }
        }

        public static Action InitDevice
        {
            get => _initDevice;
            set => _initDevice = value;
        }

        protected override void OnAppearing()
        {
            if (!start)
            {
                StartThreads();

                XplatUIMine.GetInstance().Keyboard = new Keyboard(Entry);
                start = true;
            }

            SkCanvasView.InvalidateSurface();

            Activate();

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
                        // ensure we run on the right thread
                        Application.Idle += (sender, args) =>
                        {
                            Application.Exit();
                            
                        };
                        XplatUIMine.PaintPending = true;
                    }
                });

                return true;
            }

            return false;
        }

        protected override void OnDisappearing()
        {
            Deactivate();
            base.OnDisappearing();
        }

        private void StartThreads()
        {
            XplatUIMine.GetInstance()._virtualScreen = new Rectangle(0, 0, (int) size.Width, (int) size.Height);
            XplatUIMine.GetInstance()._workingArea = new Rectangle(0, 0, (int) size.Width, (int) size.Height);

            winforms = new Thread(() =>
            {
                var init = true;

                Application.Idle += (sender, args) =>
                {
                    if (MainV2.instance != null && MainV2.instance.IsHandleCreated)
                    {
                        if (init)
                        {
                            Device.BeginInvokeOnMainThread(() => { InitDevice?.Invoke(); });
                            init = false;
                        }
                    }

                    Thread.Sleep(0);
                };

                MissionPlanner.Program.Main(new string[0]);
                
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            });
            winforms.Start();

            Forms.Device.StartTimer(TimeSpan.FromMilliseconds(1000/30), () =>
            {
                Monitor.Enter(XplatUIMine.paintlock);
                if (XplatUIMine.PaintPending)
                {
                    if (Instance.SkCanvasView != null)
                    {
                        Instance.scale = new Forms.Size((Instance.SkCanvasView.CanvasSize.Width / Instance.size.Width),
                            (Instance.SkCanvasView.CanvasSize.Height / Instance.size.Height));

                        XplatUIMine.GetInstance()._virtualScreen =
                            new Rectangle(0, 0, (int) Instance.size.Width, (int) Instance.size.Height);
                        XplatUIMine.GetInstance()._workingArea =
                            new Rectangle(0, 0, (int) Instance.size.Width, (int) Instance.size.Height);

                        Device.BeginInvokeOnMainThread(() => { Instance.SkCanvasView.InvalidateSurface(); });
                        XplatUIMine.PaintPending = false;
                    }
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
                        if (Math.Abs(touchDictionary[e.Id].atdown.Location.X / scale.Width - x) > 2 &&
                            Math.Abs(touchDictionary[e.Id].atdown.Location.Y / scale.Height - y) > 2)
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

        private SKPaint paint = new SKPaint() {FilterQuality = SKFilterQuality.Low};

        private bool DrawOntoSurface(IntPtr handle, SKSurface surface)
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
                        if (hwnd.hwndbmpNC != null)
                            surface.Canvas.DrawImage(hwnd.hwndbmpNC,
                                new SKPoint(x - borders.left, y - borders.top), paint);

                        surface.Canvas.ClipRect(
                            SKRect.Create(x, y, hwnd.width - borders.right - borders.left,
                                hwnd.height - borders.top - borders.bottom), SKClipOperation.Intersect);

                        surface.Canvas.DrawImage(hwnd.hwndbmp,
                            new SKPoint(x, y), paint);

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
                            new SKPoint(x + 0, y + 0), paint);

                        /*surface.Canvas.DrawText(hwnd.ClientWindow.ToString(), new SKPoint(x,y+15),
                            new SKPaint() {Color = SKColor.Parse("ffff00")});*/

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
                IEnumerable<Hwnd> children;
                lock (Hwnd.windows)
                    children = Hwnd.windows.OfType<System.Collections.DictionaryEntry>()
                        .Where(hwnd2 =>
                        {
                            var Key = (IntPtr) hwnd2.Key;
                            var Value = (Hwnd) hwnd2.Value;
                            if (Value.ClientWindow == Key && Value.Parent == hwnd && Value.Visible &&
                                Value.Mapped && !Value.zombie)
                                return true;
                            return false;
                        }).Select(a => (Hwnd) a.Value).ToArray();

                children = children.OrderBy((hwnd2) =>
                {
                    var info = XplatUIMine.GetInstance().GetZOrder(hwnd2.client_window);
                    if (info.top)
                        return 1000;
                    if (info.bottom)
                        return 0;
                    return 500;

                });

                foreach (var child in children)
                {
                    DrawOntoSurface(child.ClientWindow, surface);
                }
            }

            return true;
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

                surface.Canvas.Clear(SKColors.Gray);

                surface.Canvas.DrawCircle(0, 0, 50, new SKPaint() {Color = SKColor.Parse("ff0000")});

                surface.Canvas.Scale((float) scale.Width, (float) scale.Height);

                foreach (Form form in Application.OpenForms.Select(a=>a).ToArray())
                {
                    if (form.IsHandleCreated)
                    {
                        if (form is MainV2 && form.WindowState != FormWindowState.Maximized)
                            form.BeginInvokeIfRequired(() => { form.WindowState = FormWindowState.Maximized; });

                        try
                        {
                            DrawOntoSurface(form.Handle, surface);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }

                IEnumerable<Hwnd> menu;
                lock(Hwnd.windows)
                    menu = Hwnd.windows.Values.OfType<Hwnd>()
                    .Where(hw => hw.topmost && hw.Mapped && hw.Visible).ToArray();
                foreach (Hwnd hw in menu)
                {
                    if (hw.topmost && hw.Mapped && hw.Visible)
                    {
                        var ctlmenu = Control.FromHandle(hw.ClientWindow);
                        if (ctlmenu != null)
                            DrawOntoSurface(hw.ClientWindow, surface);
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

                surface.Canvas.DrawText("" + DateTime.Now.ToString("HH:mm:ss.fff"),
                    new SKPoint(10, 10), new SKPaint() {Color = SKColor.Parse("ffff00")});

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
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
        private static Action _initDevice;

        public void Activate()
        {
            Test.UsbDevices.USBEvent += DeviceAttached;
        }

        private async void DeviceAttached(object sender, MissionPlanner.ArduPilot.DeviceInfo e)
        {
            var portUsb = await Test.UsbDevices.GetUSB(e);

            if (portUsb == null)
                return;

            if (MainV2.comPort.BaseStream.IsOpen)
                return;

            try
            {
                // send hook
                const int DBT_DEVTYP_PORT = 0x00000003;

                var prt = new MainV2.DEV_BROADCAST_PORT();
                prt.dbcp_devicetype = DBT_DEVTYP_PORT;
                prt.dbcp_name = ASCIIEncoding.Unicode.GetBytes(e.board);
                prt.dbcp_size = prt.dbcp_name.Length * 2 + 4 * 3;

                IntPtr tosend;
                tosend = Marshal.AllocHGlobal(Marshal.SizeOf(prt));
                Marshal.StructureToPtr(prt, tosend, true);

                XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_DEVICECHANGE,
                    (IntPtr) MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL, tosend);
            }
            catch
            {

            }

            // autoconnect
            if (!e.board.ToLower().Contains("-bl") && !e.board.ToLower().Contains("-P2"))
            {
                var ans = await DisplayAlert("Connect", "Connect to USB Device? " + e.board, "Yes", "No");
                if (ans)
                {
                    MainV2.comPort.BaseStream = portUsb;
                    MainV2.instance.BeginInvoke((Action) delegate()
                    {
                        MainV2.instance.doConnect(MainV2.comPort, "preset", "0");
                    });
                }
            }
        }

        public void Deactivate()
        {
            Test.UsbDevices.USBEvent -= DeviceAttached;
        }
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

    public class Speech : ISpeech
    {
        public bool speechEnable { get; set; }

        public bool IsReady
        {
            get { return !isBusy; }
        }

        CancellationTokenSource cts;
        bool isBusy = false;

        public void SpeakAsync(string text)
        {
            if (!speechEnable)
                return;
            cts = new CancellationTokenSource();
            isBusy = true;
            TextToSpeech.SpeakAsync(text, cts.Token).ContinueWith((t) => { isBusy = false; },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void SpeakAsyncCancelAll()
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
        }
    }

    public class Browser : IBrowserOpen
    {
        public bool OpenURL(Uri uri)
        {
            Xamarin.Essentials.Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            return true;
        }
    }
}