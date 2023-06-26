using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using MissionPlanner.ArduPilot;
using Xamarin.Forms.Platform.MacOS;
using Xamarin.Forms;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using Xamarin.GCSViews;
using Device = Xamarin.Forms.Device;

namespace Xamarin.MacOS
{
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;
            var rect = new CoreGraphics.CGRect(200,200,1024,768);
            mainWindow = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            mainWindow.Title = "Mission Planner on Mac!";
            mainWindow.TitleVisibility = NSWindowTitleVisibility.Hidden;
            mainWindow.DidResize += MainWindow_DidResize;
            mainWindow.WillClose += MainWindow_WillClose;
            NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.KeyDown, KeyboardEventHandler);

            Test.BlueToothDevice = new BTDevice();
            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();
            Test.GPS = new GPS();
            Test.SystemInfo = new SystemInfo();
            Test.Speech = new OSXSpeech();

            WinForms.OSX = true;

            Acr.UserDialogs.Infrastructure.Log.Out += (s, s1, arg3) =>
            {
                Console.WriteLine(s + ": " + s1);
            };

            new System.Drawing.android.android();
        }

        private NSEvent KeyboardEventHandler(NSEvent keyEvent)
        {
            Console.WriteLine(keyEvent.KeyCode);

            return keyEvent;
        }

        private void MainWindow_WillClose(object sender, EventArgs e)
        {
            Console.WriteLine("MainWindow_WillClose");
            WinForms.Exit();
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        private void MainWindow_DidResize(object sender, EventArgs e)
        {
            Console.WriteLine("MainWindow_DidResize " + mainWindow.Frame.Size);
            WinForms.Resize((int)mainWindow.Frame.Size.Width, (int)mainWindow.Frame.Size.Height);
        }

        private  NSWindow mainWindow;

        public override NSWindow MainWindow { get => mainWindow; }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
            Forms.Forms.Init();
            LoadApplication(new Xamarin.App());// new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        
        public class Radio : IRadio
        {
            public void Toggle()
            {
          
            }
        }

        public class BTDevice : IBlueToothDevice
        {
            public async Task<List<DeviceInfo>> GetDeviceInfoList()
            {
                return new List<DeviceInfo>();
            }

            public Task<ICommsSerial> GetBT(DeviceInfo first)
            {
                throw new NotImplementedException();
            }
        }

        public class USBDevices : IUSBDevices
        {
            public event EventHandler<DeviceInfo> USBEvent;

            public DeviceInfo GetDeviceInfo(object devicein)
            {
                throw new NotImplementedException();
            }

            public void USBEventCallBack(object usbDeviceReceiver, object device)
            {
                throw new NotImplementedException();
            }

            public async Task<ICommsSerial> GetUSB(DeviceInfo di)
            {
                var data = GetUSBList();

                var devs = GetDevList().Split(new[] { '\r', '\n' });

                var regex = new Regex(@"-o\s+([\w\s]+@.{4}).*\n.*idProduct""\s+=\s+(.*)\n.*\n.*idVendor""\s+=\s+(.*)");

                Console.WriteLine("Looking for " + di.board);

                foreach (Match match in regex.Matches(data))
                {
                    var name = match.Groups[1].Value;
                    var loc = name.Split("@").Last();
                    var pid = int.Parse(match.Groups[2].Value);
                    var vid = int.Parse(match.Groups[3].Value);
                    
                    foreach (var dev in devs)
                    {
                        Console.WriteLine("usb location {0} == {1} && {2}.Contains({3})", di.board, name, dev.ToLower(),
                            loc.ToLower());
                        if (di.board == name && dev.ToLower().Contains(loc.ToLower()))
                        {
                            Console.WriteLine("GetUSB: Port: " + dev.TrimEnd());
                            return new SerialPort(dev.TrimEnd());
                        }
                    }
                }

                return null;
            }

            async Task<List<DeviceInfo>> IUSBDevices.GetDeviceInfoList()
            {
                var ans =  new List<DeviceInfo>();

                var data = GetUSBList();

                var devs = GetDevList().Split(new[] { '\r', '\n' });

                var regex = new Regex(@"-o\s+([\w\s]+@.{4}).*\n.*idProduct""\s+=\s+(.*)\n.*\n.*idVendor""\s+=\s+(.*)");

                foreach (Match match in regex.Matches(data))
                {
                    var name = match.Groups[1].Value;
                    var loc = name.Split("@").Last();
                    var pid = int.Parse(match.Groups[2].Value);
                    var vid = int.Parse(match.Groups[3].Value);

                    var deviceInfo = new DeviceInfo()
                    {
                        board = name,
                        description = name,
                        hardwareid = String.Format("USB\\VID_{0:X4}&PID_{1:X4}&", vid, pid),
                        name = name
                    };

                    foreach (var dev in devs)
                    {
                        // only add ports where we find a matching tty dev
                        if (deviceInfo.board == name && dev.ToLower().Contains(loc.ToLower()))
                            ans.Add(deviceInfo);
                    }
                }

                return ans;
            }

            private static string GetDevList()
            {
                var proc = System.Diagnostics.Process.Start("bash",
                    @"-c ""ls /dev/tty.* > /tmp/dev.list""");
                proc.WaitForExit();

                var data = File.ReadAllText("/tmp/dev.list");
                return data;
            }

        private static string GetUSBList()
            {
                var proc = System.Diagnostics.Process.Start("bash",
                    @"-c ""ioreg -p IOUSB -w0 -l | grep -E '@|idVendor|idProduct|bcdDevice' > /tmp/usb.list""");
                proc.WaitForExit();

                var data = File.ReadAllText("/tmp/usb.list");
                return data;
            }
        }
    }

    public class OSXSpeech : ISpeech
    {
        private NSSpeechSynthesizer speechSynthesizer = new NSSpeechSynthesizer();

        public bool speechEnable { get; set; }
        public bool IsReady { get; set; }
        public void SpeakAsync(string text)
        {
            if (!speechEnable)
                return;
            
            _ = Task.Run(() =>
            {
                IsReady = false;
                try
                {
                    //NSRunLoop.Main.BeginInvokeOnMainThread();
                    Device.InvokeOnMainThreadAsync(() => speechSynthesizer.StartSpeakingString(text));
                }
                catch
                {
                }
                IsReady = true;
            });
        }

        public void SpeakAsyncCancelAll()
        {
            Device.InvokeOnMainThreadAsync(() => speechSynthesizer.StopSpeaking());
        }
    }
}