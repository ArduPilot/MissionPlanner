using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;

namespace Xamarin.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Xamarin.App.builder.RegisterInstance(new Serial()).As<ICommsSerial>();

            Test.Radio = new Radio();
            Test.UsbDevices = new USBDevices();
            Test.BlueToothDevice = new BlueTooth();

            LoadApplication(new Xamarin.App());

        
        }
    }

    public class Radio : IRadio
    {
        public void Toggle()
        {
           
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

    public class BlueTooth : IBlueToothDevice
    {
        async Task<List<DeviceInfo>> IBlueToothDevice.GetDeviceInfoList()
        {
            return new List<DeviceInfo>();
        }

        public Task<ICommsSerial> GetBT(DeviceInfo first)
        {
            return null;
        }
    }
}
