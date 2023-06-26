//extern alias MPLib;

using MissionPlanner;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Globalization;
using log4net;
using System.Text.RegularExpressions;
using Size = System.Drawing.Size;

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
            Console.WriteLine("ScaledScreenSize " + size);
            size = Device.Info.PixelScreenSize;
            Console.WriteLine("PixelScreenSize " + size);

            Xamarin.Forms.Platform.WinForms.Forms.UIThread = Thread.CurrentThread.ManagedThreadId;

            var scale = size.Width / size.Height; // 1.77 1.6  1.33

            if (scale < 1)
            {
                size = new Forms.Size(960, 960 * scale);
            }
            else
            {
                size = new Forms.Size(540 * scale, 540);
                if (size.Width < 960)
                    size = new Forms.Size(960, 960 / scale);
            }

            if (Device.RuntimePlatform == Device.macOS || Device.RuntimePlatform == Device.UWP)
            {
                size = Device.Info.PixelScreenSize;
                // scale if higher than full hd
                if (size.Width > 1920)
                {
                    size.Width /= 2;
                    size.Height /= 2;

                }
            }

            Console.WriteLine("Final Size " + size);

            Instance = this;
            try
            {
                if (Test.Speech != null)
                    MainV2.speechEngine = Test.Speech;
                else
                    MainV2.speechEngine = new Speech();
            } catch{}

            RestoreFiles();

            FileDialog.CustomDirectory = Settings.GetUserDataDirectory();

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
                        if (s.StartsWith("BT_"))
                        {
                            var bt = await Test.BlueToothDevice.GetDeviceInfoList();

                            var di = bt.Where(a => a.board == s);

                            if (di.Count() > 0)
                            {
                                Log.Info(TAG, "SerialPort.DefaultType found device " + di.First().board + " search " + s);
                                return await Test.BlueToothDevice.GetBT(di.First());
                            }
                        }

                        if (s.StartsWith("GPS"))
                        {
                            var com = new CommsInjection();
                            Task.Run(async () => {
                                while (true)
                                {
                                    var (lat, lng, alt) = await Test.GPS.GetPosition();
                                    var latdms = (int)lat + (lat - (int)lat) * .6f;
                                    var lngdms = (int)lng + (lng - (int)lng) * .6f;

                                    var line = string.Format(CultureInfo.InvariantCulture,
                                        "$GP{0},{1:HHmmss.ff},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", "GGA",
                                        DateTime.Now.ToUniversalTime(),
                                        Math.Abs(latdms * 100).ToString("0000.00", CultureInfo.InvariantCulture), lat < 0 ? "S" : "N",
                                        Math.Abs(lngdms * 100).ToString("00000.00", CultureInfo.InvariantCulture), lng < 0 ? "W" : "E",
                                        1, 10,
                                        1, alt.ToString("0.00", CultureInfo.InvariantCulture), "M", 0, "M", "0.0", "0");

                                    var checksum = GetChecksum(line);
                                    com.AppendBuffer(ASCIIEncoding.ASCII.GetBytes(line + "*" + checksum + "\r\n"));

                                    await Task.Delay(200);
                                }
                            });                           

                            return com;
                        }

                        {
                            var dil = await Test.UsbDevices.GetDeviceInfoList();

                            var di = dil.Where(a => a.board == s);

                            if (di.Count() > 0)
                            {
                                Log.Info(TAG,
                                    "SerialPort.DefaultType found device " + di.First().board + " search " + s);
                                return await Test.UsbDevices.GetUSB(di.First());
                            }
                        }

                        if (Device.RuntimePlatform == Device.macOS || s != null && File.Exists(s))
                        {
                            Log.Info(TAG, "SerialPort.DefaultType in " + s + " " + i + " for " + Device.RuntimePlatform);
                            if (s != null && i > 0)
                                return new MonoSerialPort(s, i);
                            if(s!= null)
                                return new MonoSerialPort(s);
                            return new MonoSerialPort();
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
                if (Device.RuntimePlatform == Device.Android)
                    list1.Add("GPS");

                return list1;
            };

            if (Device.RuntimePlatform == Device.macOS)
            {
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
        }

        // Calculates the checksum for a sentence
        private string GetChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            var Checksum = 0;
            foreach (var Character in sentence)
                switch (Character)
                {
                    case '$':
                        // Ignore the dollar sign
                        break;

                    case '*':
                        // Stop processing before the asterisk
                        continue;
                    default:
                        // Is this the first value for the checksum?
                        if (Checksum == 0)
                            Checksum = Convert.ToByte(Character);
                        else
                            Checksum = Checksum ^ Convert.ToByte(Character);
                        break;
                }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }
        public static void SetHUDbg(byte[] buffer)
        {
            try
            {
                MissionPlanner.GCSViews.FlightData.myhud.bgimage = Bitmap.FromStream(new MemoryStream(buffer));
            }
            catch (Exception ex)
            {
                Log.Error("MP", ex.ToString());
            }
        }

        private void RestoreFiles()
        {
            try
            {
                // nofly dir
                Directory.CreateDirectory(Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "NoFly");

                // restore assets
                Directory.CreateDirectory(Settings.GetUserDataDirectory());

                File.WriteAllText(Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "airports.csv",
                    files.airports);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "BurntKermit.mpsystheme",
                    files.BurntKermit);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "HighContrast.mpsystheme",
                    files.HighContrast);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "ParameterMetaData.xml",
                    files.ParameterMetaDataBackup);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "camerasBuiltin.xml",
                    files.camerasBuiltin);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "checklistDefault.xml",
                    files.checklistDefault);

                File.WriteAllText(
                    Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "mavcmd.xml", 
                    files.mavcmd);


                try {
                        var pluginsdir = Settings.GetRunningDirectory() + "plugins";
                        Directory.CreateDirectory(pluginsdir);

                        string[] files = new[]
                        {
                            "example10_canlogfile", "example11_trace", "example3_fencedist", "example4_herelink",
                            "example5_latencytracker", "example6_mapicondesc", "example7_canrtcm", "example8_modechange",
                            "example9_hudonoff", "generator"
                        };

                        foreach (var file in files)
                        {
                            try
                            {
                                var id = (int) typeof(MissionPlanner.files)
                                    .GetProperty(file)
                                    .GetValue(null);

                                var filename = pluginsdir + Path.DirectorySeparatorChar + file + ".cs";

                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }


                                File.WriteAllText(filename, MissionPlanner.files.ResourceManager.GetString(file));

                            }
                            catch
                            {

                            }
                        }
                    } catch { }

                    try {
                        var graphsdir = Settings.GetRunningDirectory() + "graphs";
                        Directory.CreateDirectory(graphsdir);

                        string[] files1 = new[]
                        {
                            "ekf3Graphs", "ekfGraphs", "mavgraphs", "mavgraphs2", "mavgraphsMP"
                        };

                        foreach (var file in files1)
                        {
                            try
                            {
                                var id = typeof(MissionPlanner.files)
                                    .GetProperty(file)
                                    .GetValue(null);

                                File.WriteAllText(
                                    graphsdir + Path.DirectorySeparatorChar + file + ".xml",
                                    files.ResourceManager.GetString(file));
                            }
                            catch
                            {

                            }
                        }
                    } catch { }
            }
            catch (Exception ex)
            {
                DisplayAlert(Strings.ERROR, "Failed to stage files " + ex.ToString(), "OK");
            }
        }

        public static string BundledPath
        {
            get { return SITL.BundledPath; }
            set { SITL.BundledPath = value; }
        }

        public static bool Android
        {
            get { return MainV2.Android; }
            set { MainV2.Android = value; }
        }
        public static bool IOS
        {
            get { return MainV2.IOS; }
            set { MainV2.IOS = value; }
        }
        public static bool OSX
        {
            get { return MainV2.OSX; }
            set { MainV2.OSX = value; }
        }

        public static Action InitDevice
        {
            get => _initDevice;
            set => _initDevice = value;
        }

        public static void Exit()
        {
            Application.Exit();
        }

        public static void Resize(int width, int height)
        {
            Instance.size = new Forms.Size(width, height);
            Screen.PrimaryScreen.Bounds = new Rectangle(0, 0, width, height);
            Screen.PrimaryScreen.WorkingArea = new Rectangle(0, 0, width, height);
            var pos = new XplatUIMine.tagWINDOWPOS() {cx = width, cy = height + XplatUIMine.GetInstance().CaptionHeight, flags = 0x2, x = 0, y = 0};
            int size = Marshal.SizeOf(typeof(XplatUIMine.tagWINDOWPOS));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(pos, ptr, true);
            XplatUIMine.GetInstance().SendMessage(IntPtr.Zero, Msg.WM_WINDOWPOSCHANGED, IntPtr.Zero, ptr);
            Marshal.FreeHGlobal(ptr);
            //.SetWindowPos(IntPtr.Zero, 0, 0, width, height);
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

                var ctl = Control.FromHandle(_focusWindow);
                var nw = NativeWindow.FromHandle(_focusWindow);

                Console.WriteLine("FocusIn name {0} type {1} nw {2}", ctl?.Name,ctl?.GetType(), nw?.Handle);
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
                var ctl = Control.FromHandle(_focusWindow);
                var nw = NativeWindow.FromHandle(_focusWindow);

                var caretl = caret;

                //if (_focusWindow == handle && caret.Hwnd == _focusWindow)                                
                //Device.BeginInvokeOnMainThread(() =>
                _inputView.Dispatcher.BeginInvokeOnMainThread(()=>
                    {
                        if(caretptr == handle)
                            return;

                        var focusctl = Control.FromHandle(_focusWindow);
                        if (focusctl == null)
                            return;
                        var p = focusctl.PointToClient(Form.MousePosition);

                        var handlectl = Control.FromHandle(handle);
                        var p2 = handlectl.PointToClient(Form.MousePosition);

                        if(focusctl is ComboBox)
                        {
                            var cb = (ComboBox)focusctl;
                            if(cb.DropDownStyle == ComboBoxStyle.DropDownList)
                            {
                                return;
                            }
                        }

                        if (handlectl.ClientRectangle.Contains(p))
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
                Console.WriteLine("_inputView_Completed");
                var focusctl = Control.FromHandle(_focusWindow);
                var text = (sender as Entry)?.Text;
                focusctl.BeginInvokeIfRequired(()=>{ 
                    focusctl.Text = text;
                });
                _inputView.Dispatcher.BeginInvokeOnMainThread(() =>
                    {
                _inputView.IsVisible = false; });
            }

            private void _inputView_Unfocused(object sender, FocusEventArgs e)
            {
                Console.WriteLine("_inputView_Unfocused");
                if(Device.RuntimePlatform == Device.macOS)
                {
                    // osx only accepts the enter key - which in testing doesnt work
                    _inputView_Completed(sender, new EventArgs());
                }
                caretptr = IntPtr.Zero;                
                _inputView.Dispatcher.BeginInvokeOnMainThread(() =>
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
            Screen.PrimaryScreen.Bounds = new Rectangle(0, 0, (int) size.Width, (int) size.Height);
            Screen.PrimaryScreen.WorkingArea = new Rectangle(0, 0, (int) size.Width, (int) size.Height);

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

                    Thread.Yield();
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

                        Screen.PrimaryScreen.WorkingArea =
                            new Rectangle(0, 0, (int) Instance.size.Width, (int) Instance.size.Height);
                        Screen.PrimaryScreen.Bounds =
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
                        // osx has right click, so ignore holding left down
                        if (Device.RuntimePlatform == Device.macOS)
                            return false;
                            /*
                             Console.WriteLine("Mouse rightclick check true={0} 1={1} {2} {3} {4}",                         
                                touchDictionary.ContainsKey(e.Id),
                                touchDictionary.Count, 
                                touchDictionary.ContainsKey(e.Id) ? touchDictionary[e.Id] : null, now, DateTime.Now);
                            */
                            if (touchDictionary.ContainsKey(e.Id) && touchDictionary.Count == 1)
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

                if (e.ActionType == SKTouchAction.Pressed && e.MouseButton == SKMouseButton.Right)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONDOWN,
                        new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));
                    touchDictionary.Clear();
                }

                if (e.ActionType == SKTouchAction.Released && e.MouseButton == SKMouseButton.Right)
                {
                    XplatUI.driver.SendMessage(IntPtr.Zero, Msg.WM_RBUTTONUP,
                        new IntPtr((int) MsgButtons.MK_RBUTTON), (IntPtr) ((y) << 16 | (x)));
                    touchDictionary.Clear();
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

   private bool DrawOntoCanvas(IntPtr handle, SKCanvas Canvas, bool forcerender = false)
        {
            var hwnd = Hwnd.ObjectFromHandle(handle);

            var x = 0;
            var y = 0;
            var wasdrawn = false;

            XplatUI.driver.ClientToScreen(hwnd.client_window, ref x, ref y);

            var width = 0;
            var height = 0;
            var client_width = 0;
            var client_height = 0;


            if (hwnd.hwndbmp != null && hwnd.Mapped && hwnd.Visible && !hwnd.zombie)
            {
                // setup clip
                var parent = hwnd;
                Canvas.ClipRect(
                    SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width*2,
                        Screen.PrimaryScreen.Bounds.Height*2), (SKClipOperation) 5);

                while (parent != null)
                {
                    var xp = 0;
                    var yp = 0;
                    XplatUI.driver.ClientToScreen(parent.client_window, ref xp, ref yp);

                    Canvas.ClipRect(SKRect.Create(xp, yp, parent.Width, parent.Height),
                        SKClipOperation.Intersect);

                    parent = parent.parent;
                }

                Monitor.Enter(XplatUIMine.paintlock);
                try
                {
                    if (hwnd.ClientWindow != hwnd.WholeWindow)
                    {
                        var frm = Control.FromHandle(hwnd.ClientWindow) as Form;

                        Hwnd.Borders borders = new Hwnd.Borders();

                        if (frm != null)
                        {
                            borders = Hwnd.GetBorders(frm.GetCreateParams(), null);

                            Canvas.ClipRect(
                                SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width * 2,
                                    Screen.PrimaryScreen.Bounds.Height * 2), (SKClipOperation) 5);
                        }

                        if (Canvas.DeviceClipBounds.Width > 0 &&
                            Canvas.DeviceClipBounds.Height > 0)
                        {
                            if (hwnd.DrawNeeded || forcerender)
                            {
                                if (hwnd.hwndbmpNC != null)
                                    Canvas.DrawImage(hwnd.hwndbmpNC,
                                        new SKPoint(x - borders.left, y - borders.top), paint);

                                Canvas.ClipRect(
                                    SKRect.Create(x, y, hwnd.width - borders.right - borders.left,
                                        hwnd.height - borders.top - borders.bottom), SKClipOperation.Intersect);

                                if (hwnd.hwndbmp != null)
                                    Canvas.DrawDrawable(hwnd.hwndbmp,
                                        new SKPoint(x, y));

                                wasdrawn = true;
                            }

                            hwnd.DrawNeeded = false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Canvas.DeviceClipBounds.Width > 0 &&
                            Canvas.DeviceClipBounds.Height > 0)
                        {
                            if (hwnd.DrawNeeded || forcerender)
                            {
                                if (hwnd.hwndbmp != null)
                                    Canvas.DrawDrawable(hwnd.hwndbmp,
                                        new SKPoint(x + 0, y + 0));

                                wasdrawn = true;
                            }

                            hwnd.DrawNeeded = false;
/*
                        surface.Canvas.DrawText(Control.FromHandle(hwnd.ClientWindow).Name,
                            new SKPoint(x, y + 15),
                            new SKPaint() {Color = SKColor.Parse("55ffff00")});
                        /*surface.Canvas.DrawText(hwnd.ClientWindow.ToString(), new SKPoint(x,y+15),
                            new SKPaint() {Color = SKColor.Parse("ffff00")});*/

                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return true;
                }
                finally
                {
                    Monitor.Exit(XplatUIMine.paintlock);
                }
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
                    DrawOntoCanvas(child.ClientWindow, Canvas, true);
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

                var canvas = e.Surface.Canvas;

                canvas.Clear(SKColors.Gray);

                canvas.Scale((float) scale.Width, (float) scale.Height);

                foreach (Form form in Application.OpenForms.Select(a=>a).ToArray())
                {
                    if (form.IsHandleCreated)
                    {
                        if (form is MainV2 && form.WindowState != FormWindowState.Maximized)
                            form.BeginInvokeIfRequired(() => { form.WindowState = FormWindowState.Maximized; });

                        try
                        {
                          DrawOntoCanvas(form.Handle, canvas, true);
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
                    var ctlmenu = Control.FromHandle(hw.ClientWindow);
                        if (ctlmenu != null)
                            DrawOntoCanvas(hw.ClientWindow, canvas, true);
                }

                if (Device.RuntimePlatform != Device.macOS && Device.RuntimePlatform != Device.UWP)
                {
                    canvas.ClipRect(
                        SKRect.Create(0, 0, Screen.PrimaryScreen.Bounds.Width,
                            Screen.PrimaryScreen.Bounds.Height), (SKClipOperation) 5);

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
                            {Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeJoin = SKStrokeJoin.Miter, IsAntialias = true});
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

#pragma warning disable AsyncFixer03 // Fire-and-forget async-void methods or delegates
        private async void DeviceAttached(object sender, MissionPlanner.ArduPilot.DeviceInfo e)
#pragma warning restore AsyncFixer03 // Fire-and-forget async-void methods or delegates
        {
            ICommsSerial portUsb = null;
            try
            {
                portUsb = await Test.UsbDevices.GetUSB(e).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

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
                prt.dbcp_name = e.board;
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
                try
                {
                    var ans = await DisplayAlert("Connect", "Connect to USB Device? " + e.board, "Yes", "No").ConfigureAwait(false);
                    if (ans)
                    {
                        MainV2.comPort.BaseStream = portUsb;
                        MainV2.instance.BeginInvoke((Action) delegate()
                        {
                            MainV2.instance.doConnect(MainV2.comPort, "preset", "0");
                        });
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
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
        DateTime lastmsg = DateTime.MinValue;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public bool speechEnable { get; set; }

        public Speech()
        {

        }

        public bool IsReady
        {
            get { if (lastmsg.AddSeconds(5) < DateTime.Now) return true;  return !isBusy; }
        }

        CancellationTokenSource cts;
        bool isBusy = false;

        public void SpeakAsync(string text)
        {
            if (!MainV2.speechEnabled())
                return;

            if (text == null || String.IsNullOrWhiteSpace(text))
                return;

            text = Regex.Replace(text, @"\bPreArm\b", "Pre Arm", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bdist\b", "distance", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\bNAV\b", "Navigation", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\b([0-9]+)m\b", "$1 meters", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\b([0-9]+)ft\b", "$1 feet", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\b([0-9]+)\bbaud\b", "$1 baudrate", RegexOptions.IgnoreCase);

            cts = new CancellationTokenSource();
            lastmsg = DateTime.Now;
            isBusy = true;
            log.Info("TTS: say " + text);
            _ = Task.Run(async () =>
              {
                  try
                  {
                    var locales = await TextToSpeech.GetLocalesAsync();

                    // Grab the first locale
                    var locale = locales.FirstOrDefault();

                      var settings = new SpeechOptions()
                      {
                          Volume = 1.0f,
                          Pitch = 1.0f,
                          //Locale = locale
                      };

                      await TextToSpeech.SpeakAsync(text, settings, cts.Token).ConfigureAwait(false);
                  }
                  catch (Exception e)
                  {
                  }
                  finally
                  {
                      isBusy = false;
                  }
              });
        }

        public void SpeakAsyncCancelAll()
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();

            isBusy = false;
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