//extern alias MPLib;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.GCSViews.ConfigurationView;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Application = System.Windows.Forms.Application;
using Form = System.Windows.Forms.Form;
using Label = System.Windows.Forms.Label;
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
            base.OnAppearing();
            StartThreads();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            Application.Exit();
        }

        private void StartThreads()
        {
            var size = Device.Info.ScaledScreenSize;

            XplatUIMine.GetInstance()._virtualScreen = new Rectangle(0, 0, (int) size.Height, (int) size.Width);
            XplatUIMine.GetInstance()._workingArea = new Rectangle(0, 0, (int) size.Height, (int) size.Width);

            new Thread(() =>
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                //MPLib::MissionPlanner.Program.Main(new string[0]);

                //return;
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
            }) {Name = "WinForms"}.Start();

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

        private void SkCanvasView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            try
            {
                var x = (int) e.Location.X;
                var y = (int) e.Location.Y;

                if (e.ActionType == SKTouchAction.Moved)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr)((y) << 16 | (x)));
                }

                if (e.ActionType == SKTouchAction.Pressed && e.MouseButton == SKMouseButton.Left)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr)((y) << 16 | (x)));

                    if (LastPressed.AddMilliseconds(500) > DateTime.Now && Math.Abs(LastPressedX - x) < 10 && Math.Abs(LastPressedY - y) < 10)
                    {
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONDBLCLK, new IntPtr(),
                            (IntPtr) ((y) << 16 | (x)));
                        LastPressed = DateTime.MinValue;
                    }
                    else
                        XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONDOWN, new IntPtr(),
                            (IntPtr) ((y) << 16 | (x)));
                }

                if (e.ActionType == SKTouchAction.Released && e.MouseButton == SKMouseButton.Left)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(), (IntPtr)((y) << 16 | (x)));

                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_LBUTTONUP, new IntPtr(), (IntPtr) ((y) << 16 | (x)));
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

                e.Surface.Canvas.Clear();

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
                        if (hwnd.ClientWindow != hwnd.WholeWindow)
                        {
                            surface.Canvas.DrawImage(hwnd.hwndbmpNC.ToSKImage(), new SKPoint(hwnd.X, hwnd.Y - XplatUI.CaptionHeight), new SKPaint() { FilterQuality = SKFilterQuality.Low });
                        }
                        surface.Canvas.DrawImage(hwnd.hwndbmp.ToSKImage(), new SKPoint(x + hwnd.ClientRect.X, y + hwnd.ClientRect.Y), new SKPaint() { FilterQuality = SKFilterQuality.Low });
                    }
                    Monitor.Exit(XplatUIMine.paintlock);

                    surface.Canvas.DrawText(x + " " + y, x, y + 10, new SKPaint() { Color = SKColors.Red });

                    if (hwnd.Mapped && hwnd.Visible)
                    {
                        var enumer = Hwnd.windows.GetEnumerator();
                        while (enumer.MoveNext())
                        {
                            var hwnd2 = (System.Collections.DictionaryEntry)enumer.Current;
                            var Key = (IntPtr)hwnd2.Key;
                            var Value = (Hwnd)hwnd2.Value;
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
                        try
                        {

                            func(form.Handle);
                        }
                        finally
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