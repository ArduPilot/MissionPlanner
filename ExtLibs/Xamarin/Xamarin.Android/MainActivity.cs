using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Hoho.Android.UsbSerial;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Util;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;

namespace Xamarin.Droid
{
    [Activity(Label = "Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Test.TestMethod = new test();

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

    }

    public class test: ITest
    {
        static readonly string TAG = typeof(MainActivity).Name;

        public async void DoUSB()
        {
            var usbManager = (UsbManager) Application.Context.GetSystemService(Context.UsbService);

            foreach (var deviceListValue in usbManager.DeviceList.Values)
            {
                //usbManager.RequestPermission(deviceListValue, );

                Log.Info(TAG, deviceListValue.ToJSON());
            }

            Log.Info(TAG, "Refreshing device list ...");

            var drivers = await AndroidSerialBase.GetPorts(usbManager);

            if (drivers.Count == 0)
            {
                Log.Info(TAG, "No usb devices");
                return;
            }

            foreach (var driver in drivers)
            {
                Log.Info(TAG, string.Format("+ {0}: {1} port{2}", driver, drivers.Count, drivers.Count == 1 ? string.Empty : "s"));
            }

            var permissionGranted =
                await usbManager.RequestPermissionAsync(drivers.First().Device, Application.Context);
            if (permissionGranted)
            {
                var portInfo = new UsbSerialPortInfo(drivers.First().Ports.First());

                int vendorId = portInfo.VendorId;
                int deviceId = portInfo.DeviceId;
                int portNumber = portInfo.PortNumber;

                Log.Info(TAG, string.Format("VendorId: {0} DeviceId: {1} PortNumber: {2}", vendorId, deviceId, portNumber));

                var driver = drivers.Where((d) => d.Device.VendorId == vendorId && d.Device.DeviceId == deviceId).FirstOrDefault();
                var port = driver.Ports[portNumber];

                var serialIoManager = new SerialInputOutputManager(usbManager, port)
                {
                    BaudRate = 57600,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };


                Log.Info(TAG, "Starting IO manager ..");
                try
                {
                    serialIoManager.Open();
                }
                catch (Java.IO.IOException e)
                {
                    Log.Info(TAG, "Error opening device: " + e.Message);
                    return;
                }

                MainV2.comPort.BaseStream = new AndroidSerial(serialIoManager);
            }
        }
    }
}