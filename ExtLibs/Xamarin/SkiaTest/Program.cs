using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
using Microsoft.Scripting.Utils;
using MissionPlanner.Utilities;
using Rectangle = NetCoreEx.Geometry.Rectangle;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SkiaTest
{

    public class TreeNode
    {
        public int? Id { get; set; }
        public object Value { get; set; }
        public int? Parent { get; set; }
        public List<TreeNode> Children { get; set; } = new List<TreeNode>();

        public TreeNode()
        {
        }

        public static TreeNode BuildTree(List<TreeNode> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }
            return new TreeNode().BuildTreeI(nodes);
        }

        private TreeNode BuildTreeI(List<TreeNode> nodes)
        {
            if (nodes.Count == 0) { return this; }

            var children = FetchChildren(this, nodes).ToList();
            Children.AddRange(children);
            RemoveChildren(this, nodes);

            for (int i = 0; i < children.Count; i++)
            {
                children[i] = children[i].BuildTreeI(nodes);
                if (nodes.Count == 0) { break; }
            }

            return this;
        }

        public static IEnumerable<TreeNode> FetchChildren(TreeNode root, List<TreeNode> nodes)
        {
            return nodes.Where(n => n.Parent == root.Id);
        }

        public static void RemoveChildren(TreeNode root, List<TreeNode> nodes)
        {
            foreach (var node in root.Children)
            {
                nodes.Remove(node);
            }
        }

        public TreeNode FindID(int handle)
        {
            var search = this.Children.Where(a => a.Id == handle);
            if (search.Count() > 0)
                return search.First();

            foreach (var child in this.Children)
            {
                var ans = child.FindID(handle);
                if(ans != null)
                    return ans;
            }

            return null;
        }
    }


    static class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            {
                //new System.Drawing.android.android();

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
                var baditem =
                    ((JObject) fileobject["targets"][".NETCoreApp,Version=v6.0"]).Property(
                        "System.Drawing.Common/5.0.0");
                if (baditem != null)
                    baditem.Remove();
                baditem = ((JObject) fileobject["libraries"]).Property("System.Drawing.Common/5.0.0");
                if (baditem != null)
                    baditem.Remove();

                baditem = ((JObject) fileobject["libraries"]).Property("System.Drawing.Common/6.0.0");
                if (baditem != null)
                    baditem.Remove();

                baditem = ((JObject) fileobject["libraries"]).Property("System.Windows.Extensions/5.0.0");
                if (baditem != null)
                    baditem.Remove();

                baditem = ((JObject) fileobject["targets"]).Property("System.Windows.Extensions/5.0.0");
                if (baditem != null)
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
                    new EventLoop().Run(win);
                    Application.Exit();
                    return 0;
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
                    // sets the max size
                    Screen.PrimaryScreen.Bounds = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);
                    Screen.PrimaryScreen.WorkingArea = new System.Drawing.Rectangle(0, 0, size.Width, size.Height);

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
                                {
                                    /*
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
            private bool exit;

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

                exit = true;
            }

            private void Application_Idle(object sender, EventArgs e)
            {
                if (XplatUIMine.PaintPending)
                {
                    this.Invalidate();
                    XplatUIMine.PaintPending = false;
                }

                if (exit)
                    Application.Exit();

                //Thread.Sleep(10);
            }

            private static void AddTypeConverter(Type type, Type type1)
            {
                Attribute[] newAttributes = new Attribute[1];
                newAttributes[0] = new TypeConverterAttribute(type1);

                TypeDescriptor.AddAttributes(type, newAttributes);
            }

            protected virtual void OnSkiaPaint(SKSurface surface)
            {
            }

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

                        Msg msgid = (Msg) msg.Id;
                        var wparam = msg.WParam;
                        var lparam = msg.LParam;

                        if (msgid == Msg.WM_MOUSEMOVE || msgid == Msg.WM_LBUTTONDOWN || msgid == Msg.WM_LBUTTONUP ||
                            msgid == Msg.WM_MOUSEMOVE
                            || msgid == Msg.WM_RBUTTONDOWN || msgid == Msg.WM_RBUTTONUP || msgid == Msg.WM_QUIT ||
                            msgid == Msg.WM_CLOSE ||
                            msgid == Msg.WM_LBUTTONDBLCLK || msgid == Msg.WM_RBUTTONDBLCLK)
                        {
                            Application.OpenForms[0].BeginInvoke(new Action(() =>
                                {
                                    XplatUI.driver.SendMessage(hnd, msgid, wparam, lparam);
                                })
                            );
                        }
                        else
                        {
                            //Console.WriteLine($"{msgid} {wparam} {lparam} ");
                            if (msgid == Msg.WM_WINDOWPOSCHANGING || msgid == Msg.WM_WINDOWPOSCHANGED)
                            {
                                //Application.OpenForms[0].BeginInvoke(new Action(() =>
                                {
                                    XplatUI.driver.SendMessage(hnd, msgid, wparam, lparam);
                                }//));
                            }
                            else if (msgid == Msg.WM_PAINT || msgid == Msg.WM_NCHITTEST || msgid == Msg.WM_SETCURSOR || msgid == Msg.WM_NCMOUSEMOVE)
                            {

                            }
                            else
                            {

                            }

                        }
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

                    var canvas = e.Canvas;

                    canvas.Clear(SKColors.Gray);

                    canvas.Scale((float) scale.Width, (float) scale.Height);

                    canvas.Save();

                    var treeflat = Hwnd.windows
                        .OfType<DictionaryEntry>()
                        .Select(a=>new KeyValuePair<IntPtr,Hwnd>((IntPtr)a.Key,(Hwnd)a.Value))
                        .Where(a => a.Value.Mapped && a.Value.Visible && !a.Value.zombie)
                        .Select((a, idx) => new TreeNode()
                    {
                        Id = (int?)a.Key,
                        Value = a.Value,
                        Parent = (int?)(a.Value.Parent?.Handle.ToInt64())
                    });

                    var newtree = TreeNode.BuildTree(treeflat.ToList());

                    foreach (Form form in Application.OpenForms.Select(a => a).ToArray())
                    {
                        if (form.IsHandleCreated)
                        {
                            if (form is MainV2 && form.WindowState != FormWindowState.Maximized)
                                form.BeginInvokeIfRequired(() => { form.WindowState = FormWindowState.Maximized; });

                            try
                            {
                                FormsRender.DrawOntoCanvas(form.Handle, canvas, true /* Application.OpenForms.Count > 1*/);
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
                        var ctlmenu = Control.FromHandle(hw.ClientWindow);
                        if (ctlmenu != null)
                            FormsRender.DrawOntoCanvas(hw.ClientWindow, canvas, true);
                    }

                    canvas.Restore();

                    //if (Device.RuntimePlatform != Device.macOS && Device.RuntimePlatform != Device.UWP)
                    {/*
                        canvas.ClipRect(
                            SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width,
                                Screen.PrimaryScreen.Bounds.Height), SKClipOperation.Intersect);
                        */
                        var path = new SKPath();

                        path.MoveTo(cursorPoints.First());
                        cursorPoints.ForEach(a => path.LineTo(a));
                        path.Transform(new SKMatrix(1, 0, XplatUI.driver.MousePosition.X, 0, 1,
                            XplatUI.driver.MousePosition.Y, 0, 0, 1));

                        canvas.DrawPath(path,
                            new SKPaint()
                                {Color = SKColors.White, Style = SKPaintStyle.Fill, StrokeJoin = SKStrokeJoin.Miter});
                        canvas.DrawPath(path,
                            new SKPaint()
                            {
                                Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeJoin = SKStrokeJoin.Miter,
                                IsAntialias = true
                            });


                        { 
                            Control ctl = null;
                          
                            var cnt = Application.OpenForms.Count;
                            while (cnt > 0 && ctl == null)
                            {
                                int x = XplatUI.driver.MousePosition.X, y = XplatUI.driver.MousePosition.Y;

                                if (Application.OpenForms[cnt - 1].IsDisposed)
                                {
                                    cnt--;
                                    continue;
                                }
                                var hwnd = Hwnd.ObjectFromHandle(Application.OpenForms[cnt - 1].Handle);
                                if (x < hwnd.X || y < hwnd.Y || x > hwnd.x+hwnd.width || y > hwnd.y+hwnd.height)
                                {
                                    cnt--;
                                    continue;
                                }
                                
                                XplatUI.driver.ScreenToClient(Application.OpenForms[cnt - 1].Handle,
                                    ref x, ref y);
                                
                                ctl = XplatUIMine.FindControlAtPoint(Application.OpenForms[cnt - 1],
                                    new Point(x, y));
                                if (ctl == null)
                                    ctl = Application.OpenForms[cnt - 1];

                                cnt--;
                            }

                            canvas.DrawText(""+ctl?.Name + "HERE!!!",
                                new SKPoint(100, 100), new SKPaint() { Color = SKColor.Parse("ffff00") });
                        }
                    }

                    canvas.DrawText("" + DateTime.Now.ToString("HH:mm:ss.fff"),
                        new SKPoint(10, 10), new SKPaint() {Color = SKColor.Parse("ffff00")});

                    canvas.Flush();

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
                new SKPoint(0f, 0f),
                new SKPoint(0f, 16.512804f),
                new SKPoint(4.205124f, 12.717936f),
                new SKPoint(7.589736f, 19.99998f),
                new SKPoint(9.641016f, 19.076904f),
                new SKPoint(6.256404f, 11.79486f),
                new SKPoint(12.102552f, 11.179476f),
                new SKPoint(0f, 0f),
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