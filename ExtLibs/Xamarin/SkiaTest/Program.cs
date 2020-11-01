using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Controls;
using SkiaSharp.Views.Desktop;
using SkiaSharp;
using WinApi.Desktop;
using WinApi.Gdi32;
using WinApi.User32;
using WinApi.Utils;
using WinApi.Windows;
using WinApi.Windows.Helpers;
using System.Xml.Serialization;
using Installer;
using LibVLC.NET;
using Microsoft.Scripting.Utils;
using MissionPlanner.Utilities;
using Rectangle = NetCoreEx.Geometry.Rectangle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Installer
{
    static class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            {
                var path = Assembly.GetExecutingAssembly().CodeBase?.ToString();
                var dir = Path.GetDirectoryName(path).Replace("file:\\", "");
                Assembly.LoadFile(dir +
                                         Path.DirectorySeparatorChar +
                                         "System.Drawing.dll");
                  Assembly.LoadFile(dir +
                                         Path.DirectorySeparatorChar +
                                         "System.Drawing.Common.dll");

                var file = File.ReadAllText("SkiaTest.deps.json");
                var fileobject = JsonConvert.DeserializeObject(file) as JObject;
                var baditem = ((JObject)fileobject["targets"][".NETCoreApp,Version=v3.1"]).Property("System.Drawing.Common/4.7.0");
                if(baditem != null)
                    baditem.Remove();
                File.WriteAllText("SkiaTest.deps.json", fileobject.ToString());
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            try
            {
                ApplicationHelpers.SetupDefaultExceptionHandlers();
                var factory = WindowFactory.Create(hBgBrush: IntPtr.Zero);
                using (var win = factory.CreateWindow(() => new SkiaWindow(), "Hello",
                    constructionParams: new FrameWindowConstructionParams()))
                {
                    win.SetSize(900, 540 + 30);
                    win.Show();
                    return new EventLoop().Run(win);
                }
            }
            catch (Exception ex)
            {
                MessageBoxHelpers.ShowError(ex);
                return 1;
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("System.Drawing"))
            {
                var path = args.RequestingAssembly.CodeBase?.ToString();
                var dir = Path.GetDirectoryName(path).Replace("file:\\", "");
                return Assembly.LoadFile(dir +
                                         Path.DirectorySeparatorChar +
                                         "System.Drawing.dll");
            }

            return null;
        }

        public class SkiaPainter
        {
            private static GRContext grContext;

            public static unsafe void ProcessPaint(ref PaintPacket packet, NativePixelBuffer pixelBuffer,
                Action<SKSurface> handler)
            {

                var hwnd = packet.Hwnd;
                Rectangle clientRect;
                User32Methods.GetClientRect(hwnd, out clientRect);
                var size = clientRect.Size;
                pixelBuffer.EnsureSize(size.Width, size.Height);
                PaintStruct ps;
                var hdc = User32Methods.BeginPaint(hwnd, out ps);
                var skPainted = false;
                try
                {
                    using (var surface = SKSurface.Create(
                        size.Width,
                        size.Height,
                        SKColorType.Bgra8888,
                        SKAlphaType.Premul,
                        pixelBuffer.Handle,
                        pixelBuffer.Stride))
                    {
                        if (surface != null)
                        {
                            handler(surface);
                            /*
                            foreach (var VARIABLE in Enumerable.Range(0, pixelBuffer.Stride * size.Height / 4))
                            {
                                var value = Marshal.ReadInt32(pixelBuffer.Handle, VARIABLE*4);
                                // ABGR >> ARGB
                                unchecked
                                {
                                    value = ((value & (int)0xff000000) >> 0) +
                                        ((value & 0xff0000) >> 16 )+
                                        ((value & 0xff00) << 0) +
                                        ((value & 0xff) << 16);
                                }

                                Marshal.WriteInt32(pixelBuffer.Handle, VARIABLE * 4, value);
                            }*/

                            var max = pixelBuffer.Stride * size.Height / 4;

                            int* arr = (int*) pixelBuffer.Handle.ToPointer();
                            unchecked
                            {
                                var a = 0;
                                while (a < max)
                                {/*
                                    *arr = ((*arr & (int) 0xff000000) >> 0) +
                                           ((*arr & 0xff0000) >> 16) +
                                           ((*arr & 0xff00) << 0) +
                                           ((*arr & 0xff) << 16);
                                    arr++;*/
                                    a++;
                                }
                            }

                            skPainted = true;
                        }
                    }
                }
                finally
                {
                    if (skPainted) Gdi32Helpers.SetRgbBitsToDevice(hdc, size.Width, size.Height, pixelBuffer.Handle);
                    User32Methods.EndPaint(hwnd, ref ps);
                }
            }
        }
        public class SkiaWindowBase : EventedWindowCore
        {
            private readonly NativePixelBuffer m_pixelBuffer = new NativePixelBuffer();

            public SkiaWindowBase()
            {
                var th = new Thread(start);
                th.Start();

            }

            private void start()
            {

                //AddTypeConverter(typeof(System.Drawing.Bitmap), typeof(BitmapClassConverter));
                //AddTypeConverter(typeof(System.Drawing.Icon), typeof(IconClassConverter));

                //var convert = TypeDescriptor.GetConverter(typeof(System.Drawing.Bitmap));
                //convert.ConvertTo()

                Application.Idle += Application_Idle;

                MissionPlanner.Program.Main(new string[] { });
            }

            protected override void OnClose(ref Packet packet)
            {
                base.OnClose(ref packet);

                Application.Exit();
            }

            private void Application_Idle(object sender, EventArgs e)
            {
                if (XplatUIMine.PaintPending)
                {
                    this.Invalidate();
                    XplatUIMine.PaintPending = false;
                }

                //Thread.Sleep(10);
            }

            private static void AddTypeConverter(Type type, Type type1)
            {
                Attribute[] newAttributes = new Attribute[1];
                newAttributes[0] = new TypeConverterAttribute(type1);

                TypeDescriptor.AddAttributes(type, newAttributes);
            }

            protected virtual void OnSkiaPaint(SKSurface surface) { }

            protected override void OnPaint(ref PaintPacket packet)
            {
                SkiaPainter.ProcessPaint(ref packet, this.m_pixelBuffer, this.OnSkiaPaint);
            }

            protected override void OnMessage(ref WindowMessage msg)
            {
                var hnd = msg.Hwnd;
                
                XplatUIMine.FosterParentLast = msg.Hwnd;

                if (Application.OpenForms.Count > 0)
                {
                    if (Application.OpenForms[0].IsHandleCreated)
                    {
                        hnd = IntPtr.Zero;

                        Msg msgid = (Msg)msg.Id;
                        var wparam = msg.WParam;
                        var lparam = msg.LParam;

                        XplatUI.driver.SendMessage(hnd, msgid, wparam, lparam);                       
                    }
                }

                //if (msg.Id == WM.NCPAINT)
                    //return;

                base.OnMessage(ref msg);
            }

            protected override void Dispose(bool disposing)
            {
                this.m_pixelBuffer.Dispose();
                base.Dispose(disposing);
            }
        }
        
        public sealed class SkiaWindow : SkiaWindowBase
        {
            protected override void OnSkiaPaint(SKSurface e)
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
                            {
                                Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeJoin = SKStrokeJoin.Miter,
                                IsAntialias = true
                            });
                    }

                    surface.Canvas.Flush();

                    //return;

                    surface.Canvas.ClipRect(new SKRect(0, 0, Screen.PrimaryScreen.Bounds.Right,
                        Screen.PrimaryScreen.Bounds.Bottom), (SKClipOperation) 5);

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

                children = children.OrderBy((hwnd) =>
                {
                    var info = XplatUIMine.GetInstance().GetZOrder(hwnd.client_window);
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

        /*

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromDC(IntPtr hDC);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetDC(IntPtr hWnd);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Graphics.HwndToGraphics += ptr =>
            {
                var window = NativeWindow.FromHandle(ptr);
                var ctl = Control.FromHandle(ptr);

                var rect = new RECT(0, 0, 1, 1);
                XplatUIWin32.Win32GetClientRect(ptr, out rect);

                var size = ctl == null ? new Rectangle(0, 0, rect.Width, rect.Height) : ctl.ClientRectangle;
                return new Graphics(IntPtr.Zero, size.Width, size.Height);
            };

            Graphics.GraphicsFromHdc += ptr =>
            {
                var window = NativeWindow.FromHandle(ptr);
                var ctl = Control.FromHandle(ptr);

                var rect = new RECT();
                var windows2 = WindowFromDC(ptr);
                XplatUIWin32.Win32GetClientRect(windows2, out rect);

                IntPtr srcHdc = XplatUIWin32.Win32CreateCompatibleDC(ptr);
                IntPtr srcBmp = XplatUIWin32.Win32CreateCompatibleBitmap(ptr, rect.Width, rect.Height);
                XplatUIWin32.Win32SelectObject(srcHdc, srcBmp);

                //offscreen_drawable = new WinBuffer(srcHdc, srcBmp);

                //XplatUIWin32.Win32GetObject(g_hbmBall, sizeof(bm), &bm);

                XplatUIWin32.Win32BitBlt(ptr, 0, 0, rect.Width, rect.Height, srcHdc, 0, 0,
                    TernaryRasterOperations.SRCCOPY);


                return new Graphics(IntPtr.Zero, rect.Width, rect.Height);
            };

            var frm = new Form();
            frm.Controls.Add(new HUDSkia() { Dock = DockStyle.Fill });


            Application.Run(frm);
        }
        */
    }
    /*
    [TypeConverter(typeof(BitmapClassConverter))]
    public class BitmapClassConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(System.Drawing.Bitmap))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is byte[])
            {
                return new System.Drawing.Bitmap(new MemoryStream((byte[])value));
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)) { return "___"; }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
    [TypeConverter(typeof(IconClassConverter))]
    public class IconClassConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(System.Drawing.Icon))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (value is byte[])
            {
                return new System.Drawing.Icon(new MemoryStream((byte[]) value));
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context,
            CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string)) { return "___"; }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }*/
}
