//extern alias MPLib;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews.ConfigurationView;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = System.Windows.Forms.Application;
using Extensions = MissionPlanner.Utilities.Extensions;
using Form = System.Windows.Forms.Form;
using Label = System.Windows.Forms.Label;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using Size = Xamarin.Forms.Size;

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
            base.OnAppearing();
            SkCanvasView.IgnorePixelScaling = false;

            if (!start)
            {
                StartThreads();

                XplatUIMine.GetInstance().Keyboard = new Keyboard(Entry);
                start = true;
            }
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

      

                return;

                if (e.OldTextValue == null)
                {
                    XplatUI.driver.SendMessage(_focusWindow, Msg.WM_CHAR,
                        (IntPtr) e.NewTextValue[e.NewTextValue.Length-1], (IntPtr) 0);
                } 
                else if (e.OldTextValue.Length > e.NewTextValue.Length)
                {
                    XplatUI.driver.SendMessage(_focusWindow, Msg.WM_CHAR, (IntPtr) 8, (IntPtr) 0);
                }
                else if (e.OldTextValue.Length < e.NewTextValue.Length)
                {
                    XplatUI.driver.SendMessage(_focusWindow, Msg.WM_CHAR,
                        (IntPtr) e.NewTextValue[e.NewTextValue.Length-1], (IntPtr) 0);
                }

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
                return true;
            }

            return false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Application.Exit();
        }

        private void StartThreads()
        {
            size = Device.Info.ScaledScreenSize;
            size = Device.Info.PixelScreenSize;

            size = new Forms.Size(1022, 575);
            scale = new Forms.Size((Device.Info.PixelScreenSize.Width / size.Width),
                (Device.Info.PixelScreenSize.Height / size.Height));

            XplatUIMine.GetInstance()._virtualScreen = new Rectangle(0, 0, (int) size.Width, (int) size.Height);
            XplatUIMine.GetInstance()._workingArea = new Rectangle(0, 0, (int) size.Width, (int) size.Height);

            new Thread(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                MissionPlanner.Program.Main(new string[0]);

                MessageBox.Show("Application Exit");

                Application.Exit();

                return;
                var frm = new Form()
                {
                    Width = (int) Forms.Application.Current.MainPage.Width,
                    Height = (int) Forms.Application.Current.MainPage.Height,
                    Location = new System.Drawing.Point(0,0)
                };

                frm.Controls.Add(new Label()
                    {Text = "this is a test", Location = new System.Drawing.Point(frm.Width / 2, frm.Height / 2)});
                
                /*
                frm.Controls.Add(new RadioButton());
                frm.Controls.Add(new CheckBox());
                frm.Controls.Add(new ComboBox());
                frm.Controls.Add(new DomainUpDown());
                frm.Controls.Add(new DataGridView());
                frm.Controls.Add(new TextBox());
                */
                var param =  new ConfigRawParams()
                    {Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(frm.Width, frm.Height)};
                frm.Controls.Add(param);

                frm.Shown += (sender, args) => { param.Activate(); };

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                Application.Run(frm);
            }).Start();

            Forms.Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                this.SkCanvasView.InvalidateSurface();
                return true;
            });

            this.SkCanvasView.PaintSurface += SkCanvasView_PaintSurface;

            this.SkCanvasView.EnableTouchEvents = true;
            this.SkCanvasView.Touch += SkCanvasView_Touch;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject);
        }

        //Double-clicking the left mouse button actually generates a sequence of four messages: WM_LBUTTONDOWN, WM_LBUTTONUP, WM_LBUTTONDBLCLK, and WM_LBUTTONUP.
        DateTime LastPressed = DateTime.MinValue;

        private int LastPressedX;
        private int LastPressedY;
        private Forms.Size size;
        private Forms.Size scale;
        private (DateTime time, int x, int y) DownTime = (DateTime.MinValue, 0, 0);
        private bool start;

        private void SkCanvasView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            try
            {
                var x = (int) (e.Location.X / scale.Width);
                var y = (int) (e.Location.Y / scale.Height);

                Console.WriteLine(Extensions.ToJSON(e, Formatting.None));

                if (e.ActionType == SKTouchAction.Moved)
                {
                    if (e.InContact)
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr((int) MsgButtons.MK_LBUTTON),
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
                    DownTime = (DateTime.Now, x, y);
                    
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr)((y) << 16 | (x)));

                    if (LastPressed.AddMilliseconds(500) > DateTime.Now && Math.Abs(LastPressedX - x) < 10 &&
                        Math.Abs(LastPressedY - y) < 10)
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
                    if ( Math.Abs(DownTime.x - x) < 10 && Math.Abs(DownTime.y - y) < 10 && DownTime.time.AddMilliseconds(2000) < DateTime.Now)
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONDOWN,
                            new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONUP,
                            new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));

                        DownTime.time = DateTime.MinValue;
                        return;
                    }

                    DownTime.time = DateTime.MinValue;
                    //XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr((int) MsgButtons.MK_LBUTTON), (IntPtr)((y) << 16 | (x)));

                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONUP, new IntPtr((int) MsgButtons.MK_LBUTTON), (IntPtr) ((y) << 16 | (x)));
                    LastPressed = DateTime.Now;
                    LastPressedX = x;
                    LastPressedY = y;
                }

                if (e.ActionType == SKTouchAction.Entered)
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr) ((y) << 16 | (x)));

                if (e.ActionType == SKTouchAction.Exited)
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr) ((y) << 16 | (x)));

                e.Handled = true;

            } catch {}
        }

        private void SkCanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            try
            {
                var surface = e.Surface;

                e.Surface.Canvas.Clear(SKColors.Transparent);

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

                    Monitor.Enter(XplatUIMine.paintlock);
                    if (hwnd.hwndbmp != null && hwnd.Mapped && hwnd.Visible)
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

                            surface.Canvas.DrawImage(hwnd.hwndbmpNC.ToSKImage(),
                                new SKPoint(x - borders.left, y - borders.top),
                                new SKPaint() {FilterQuality = SKFilterQuality.Low});

                            surface.Canvas.ClipRect(
                                SKRect.Create(x, y, hwnd.width - borders.right - borders.left,
                                    hwnd.height - borders.top - borders.bottom), SKClipOperation.Intersect);

                            surface.Canvas.DrawImage(hwnd.hwndbmp.ToSKImage(),
                                new SKPoint(x, y),
                                new SKPaint() {FilterQuality = SKFilterQuality.Low});


                        }
                        else
                        {
                            surface.Canvas.DrawImage(hwnd.hwndbmp.ToSKImage(),
                                new SKPoint(x + 0, y + 0),
                                new SKPaint() {FilterQuality = SKFilterQuality.Low});
                        }
                    }

                    Monitor.Exit(XplatUIMine.paintlock);

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
                        if (form.Location.X < 0 || form.Location.Y < 0)
                        {
                            form.Location = new Point(Math.Max(form.Location.X,0),Math.Max(form.Location.Y,0));
                        }

                        var border = Hwnd.GetBorders(form.GetCreateParams(), null);

                        if (form.Size.Width > Screen.PrimaryScreen.Bounds.Width || form.Size.Height > Screen.PrimaryScreen.Bounds.Height)
                        {
                            //form.Size = new System.Drawing.Size((int) Screen.PrimaryScreen.Bounds.Width, (int) Screen.PrimaryScreen.Bounds.Height);
                            XplatUI.driver.SetWindowPos(form.Handle, 0, 0, (int) Screen.PrimaryScreen.Bounds.Width,
                                (int) Screen.PrimaryScreen.Bounds.Height);
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

                return;
                surface.Canvas.ClipRect(new SKRect(0, 0, Screen.PrimaryScreen.Bounds.Right,
                    Screen.PrimaryScreen.Bounds.Bottom), (SKClipOperation)5);

                surface.Canvas.DrawText("PixelScreenSize " + Device.Info.PixelScreenSize.ToString(), new SKPoint(50, 10), new SKPaint() { Color = SKColor.Parse("ffff00") });
                

                surface.Canvas.DrawText("screen " + Screen.PrimaryScreen.ToString(), new SKPoint(50, 30), new SKPaint() { Color = SKColor.Parse("ffff00") });

                int mx = 0, my = 0;
                XplatUI.driver.GetCursorPos(IntPtr.Zero, out mx, out my);

                surface.Canvas.DrawText("mouse " + XplatUI.driver.MousePosition.ToString(), new SKPoint(50, 50), new SKPaint() { Color = SKColor.Parse("ffff00") });
                surface.Canvas.DrawText(mx + " " + my, new SKPoint(50, 70), new SKPaint() { Color = SKColor.Parse("ffff00") });


                if (Application.OpenForms.Count > 0 && Application.OpenForms[Application.OpenForms.Count - 1].IsHandleCreated)
                {
                    var x = XplatUI.driver.MousePosition.X;
                    var y = XplatUI.driver.MousePosition.Y;

                    XplatUI.driver.ScreenToClient(Application.OpenForms[Application.OpenForms.Count - 1].Handle, ref x, ref y);

                    var ctl = XplatUIMine.FindControlAtPoint(Application.OpenForms[Application.OpenForms.Count - 1], new Point(x, y));
                    if (ctl != null)
                    {
                        XplatUI.driver.ScreenToClient(ctl.Handle, ref mx, ref my);
                        surface.Canvas.DrawText("client " + mx + " " + my, new SKPoint(50, 90), new SKPaint() { Color = SKColor.Parse("ffff00") });

                        surface.Canvas.DrawText(ctl?.ToString(), new SKPoint(50, 130),
                            new SKPaint() { Color = SKColor.Parse("ffff00") });

                        var hwnd = Hwnd.ObjectFromHandle(ctl.Handle);

                        surface.Canvas.DrawText(ctl.Location.ToString(), new SKPoint(50, 150),
                            new SKPaint() { Color = SKColor.Parse("ffff00") });
                    }
                }
                surface.Canvas.DrawText("!", new SKPoint(XplatUI.driver.MousePosition.X,
                        XplatUI.driver.MousePosition.Y),
                    new SKPaint() { Color = SKColor.Parse("ffff00") });
            }
            catch { }
        }
    }
}