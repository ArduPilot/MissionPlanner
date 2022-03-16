using SkiaSharp;
using SkiaSharp.Views.UWP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using log4net.Util;
using MissionPlanner;
using MissionPlanner.Controls;
using Microsoft.Scripting.Utils;
using MissionPlanner.Utilities;
using MissionPlanner.Comms;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SkiaSharpTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Console.WriteLine("MainPage ctor");

            SKCanvasForms.PaintSurface += OnPaintSurface;
            SKCanvasForms.PointerMoved += SKCanvasForms_PointerMoved;

            //Task.Run(() => start());

            log4net.Repository.Hierarchy.Hierarchy hierarchy =
                (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetAssembly(typeof(App)));

            var patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "[%thread] %-5level %logger %memory - %message\n";
            //patternLayout.AddConverter(new ConverterInfo() {Name = "memory", Type = typeof(PatternConverter)});
            patternLayout.ActivateOptions();

            var cca = new ConsoleAppender();
            cca.Layout = patternLayout;
            cca.ActivateOptions();
            hierarchy.Root.AddAppender(cca);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;

            Console.WriteLine("MainPage ctor end");

            //var th = new Thread(start);
            //th.Start();

            SerialPort.DefaultType = (self, s, i) => { return null; };

            start();

            pollDraw();
        }

        private void SKCanvasForms_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pnt = e.GetCurrentPoint(SKCanvasForms);
            XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_MOUSEMOVE, new IntPtr(),
                (IntPtr) ((int) (pnt.Position.Y) << 16 | (int) (pnt.Position.X)));
        }

        private async void pollDraw()
        {
            SKCanvasForms.Invalidate();

            await Task.Yield();
        }

        private async void start()
        {
            Console.WriteLine("start");
            //AddTypeConverter(typeof(System.Drawing.Bitmap), typeof(BitmapClassConverter));
            //AddTypeConverter(typeof(System.Drawing.Icon), typeof(IconClassConverter));

            //var convert = TypeDescriptor.GetConverter(typeof(System.Drawing.Bitmap));
            //convert.ConvertTo()

            Application.AddMessageFilter(new AsyncFilter());

            Application.Idle += Application_Idle;

            pollDraw();

            try
            {

                MissionPlanner.Program.Main(new string[] { });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("start end");
        }

        private async void Application_Idle(object sender, EventArgs e)
        {
            Console.WriteLine("Application_Idle");
            if (XplatUIMine.PaintPending)
            {
                SKCanvasForms.Invalidate();
                XplatUIMine.PaintPending = false;
            }

            await Task.Delay(1);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // the the canvas and properties
            var canvas = e.Surface.Canvas;

            // get the screen density for scaling
            var display = DisplayInformation.GetForCurrentView();
            var scale = display.LogicalDpi / 96.0f;
            var scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.Yellow);

            // draw some text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };
            var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
            canvas.DrawText("SkiaSharp", coord, paint);

            OnSkiaPaint(e.Surface);
            // Width 41.6587026 => 144.34135
            // Height 56 => 147
        }




                  protected  void OnSkiaPaint(SKSurface e)
            {
                try
                {

                    var surface = e;

                    surface.Canvas.Clear(SKColors.Gray);

                    surface.Canvas.DrawCircle(0, 0, 50, new SKPaint() {Color = SKColor.Parse("ff0000")});

                    surface.Canvas.Scale((float) scale.Width, (float) scale.Height);

                    foreach (Form form in Application.OpenForms.Select(a => a).ToArray())
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
                    lock (Hwnd.windows)
                        menu = Hwnd.windows.Values.OfType<Hwnd>()
                            .Where(hw => hw.topmost && hw.Mapped && hw.Visible).ToArray();
                    foreach (Hwnd hw in menu)
                    {
                        if (hw.topmost && hw.Mapped && hw.Visible)
                        {
                            var ctlmenu = System.Windows.Forms.Control.FromHandle(hw.ClientWindow);
                            if (ctlmenu != null)
                                DrawOntoSurface(hw.ClientWindow, surface);
                        }
                    }

                    {
                        surface.Canvas.ClipRect(
                            SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width*2,
                                Screen.PrimaryScreen.Bounds.Height*2), (SKClipOperation) 5);

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
                            {
                                Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeJoin = SKStrokeJoin.Miter,
                                IsAntialias = true
                            });
                    }

                    surface.Canvas.Flush();

                    //return;

                    surface.Canvas.ClipRect(new SKRect(0, 0, Screen.PrimaryScreen.Bounds.Right*2,
                        Screen.PrimaryScreen.Bounds.Bottom*2), (SKClipOperation) 5);

                    /*surface.Canvas.DrawText("PixelScreenSize " + Device.Info.PixelScreenSize.ToString(),
                        new SKPoint(50, 10), new SKPaint() {Color = SKColor.Parse("ffff00")});
                    */

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

                        XplatUI.driver.ScreenToClient(Application.OpenForms[Application.OpenForms.Count - 1].Handle,
                            ref x,
                            ref y);

                        var ctl = XplatUIMine.FindControlAtPoint(Application.OpenForms[Application.OpenForms.Count - 1],
                            new Point(x, y));
                        if (ctl != null)
                        {
                            XplatUI.driver.ScreenToClient(ctl.Handle, ref mx, ref my);
                            surface.Canvas.DrawText("client " + mx + " " + my, new SKPoint(50, 90),
                                new SKPaint() {Color = SKColor.Parse("ffff00")});

                            surface.Canvas.DrawRect(x, y, ctl.Width, ctl.Height, new SKPaint(){Color = SKColors.Red, Style = SKPaintStyle.Stroke });

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
                    SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width*2,
                        Screen.PrimaryScreen.Bounds.Height*2), (SKClipOperation) 5);

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
                    var frm = System.Windows.Forms.Control.FromHandle(hwnd.ClientWindow) as Form;

                    Hwnd.Borders borders = new Hwnd.Borders();

                    if (frm != null)
                    {
                        borders = Hwnd.GetBorders(frm.GetCreateParams(), null);

                        surface.Canvas.ClipRect(
                            SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width*2,
                                Screen.PrimaryScreen.Bounds.Height*2), (SKClipOperation) 5);
                    }

                    if (surface.Canvas.DeviceClipBounds.Width > 0 &&
                        surface.Canvas.DeviceClipBounds.Height > 0)
                    {
                        if (hwnd.hwndbmpNC != null)
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
/*
                        surface.Canvas.DrawText(Control.FromHandle(hwnd.ClientWindow).Name,
                            new SKPoint(x, y + 15),
                            new SKPaint() {Color = SKColor.Parse("55ffff00")});
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

            private SizeF scale = new SizeF(1, 1);

    }

    internal class AsyncFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            Console.WriteLine("PreFilterMessage " + m);

            return false;
        }
    }
}
