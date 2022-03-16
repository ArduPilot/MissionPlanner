using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs.Infrastructure;
using Foundation;
using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using SkiaSharp.Views.Forms;
using UIKit;
using Xamarin.GCSViews;

namespace Xamarin.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // load my version

            new System.Drawing.android.android();

            
            Test.BlueToothDevice = new BTDevice();
            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();

            WinForms.IOS = true;

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    public class Radio : IRadio
    {
        public void Toggle()
        {
          
        }
    }

    public class BTDevice : IBlueToothDevice
    {
        Task<List<DeviceInfo>> IBlueToothDevice.GetDeviceInfoList()
        {
            throw new NotImplementedException();
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

        public Task GetDeviceInfoList()
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
