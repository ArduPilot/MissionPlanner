using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppKit;
using Foundation;
using MissionPlanner.ArduPilot;
using Xamarin.Forms.Platform.MacOS;
using Xamarin.Forms;
using MissionPlanner.Comms;
using Xamarin.GCSViews;

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

            Test.BlueToothDevice = new BTDevice();
            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();
            Test.GPS = new GPS();
            Test.SystemInfo = new SystemInfo();

            WinForms.OSX = true;

            new System.Drawing.android.android();
        }

        private void MainWindow_WillClose(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        private void MainWindow_DidResize(object sender, EventArgs e)
        {
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

            public Task<ICommsSerial> GetUSB(DeviceInfo di)
            {
                throw new NotImplementedException();
            }

            async Task<List<DeviceInfo>> IUSBDevices.GetDeviceInfoList()
            {
                return new List<DeviceInfo>();
            }
        }
    }
}