using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;
using Hoho.Android.UsbSerial;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Util;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;

namespace Xamarin.Droid
{
    public class USBDevices: IUSBDevices
    {
        static readonly string TAG = typeof(MainActivity).Name;


        public async Task<List<DeviceInfo>> GetDeviceInfoList()
        {
            var usbManager = (UsbManager)Application.Context.GetSystemService(Context.UsbService);

            foreach (var deviceListValue in usbManager.DeviceList.Values)
            {
                Log.Info(TAG, deviceListValue.ToJSON());
            }

            Log.Info(TAG, "Refreshing device list ...");

            var drivers = await AndroidSerialBase.GetPorts(usbManager);

            if (drivers.Count == 0)
            {
                Log.Info(TAG, "No usb devices");
                return new List<DeviceInfo>();
            }

            List<DeviceInfo> ans = new List<DeviceInfo>();

            foreach (var driver in drivers)
            {
                Log.Info(TAG, string.Format("+ {0}: {1} port{2}", driver, drivers.Count, drivers.Count == 1 ? string.Empty : "s"));

                Log.Info(TAG, string.Format("+ {0}: {1} ", driver.Device.ProductName, driver.Device.ManufacturerName));

                var deviceInfo = GetDeviceInfo(driver.Device);

                ans.Add(deviceInfo);

                await usbManager.RequestPermissionAsync(driver.Device, Application.Context);
            }

            return ans;
        }

        public void USBEventCallBack(object usbDeviceReceiver, object device)
        {
            USBEvent?.Invoke(usbDeviceReceiver, device);
        }

        public event EventHandler<object> USBEvent;

        /// <summary>
        /// UsbDevice to DeviceInfo
        /// </summary>
        /// <param name="devicein"></param>
        /// <returns></returns>
        public DeviceInfo GetDeviceInfo(object devicein)
        {
            var device = (devicein as UsbDevice);
            var deviceInfo = new DeviceInfo()
            {
                board = device.ProductName,
                description = device.ProductName,
                hardwareid = String.Format("USB\\VID_{0:X4}&PID_{1:X4}", device.VendorId, device.ProductId),
                name = device.DeviceName
            };
            return deviceInfo;
        }


        public async Task<ICommsSerial> GetUSB(DeviceInfo di)
        {
            var usbManager = (UsbManager) Application.Context.GetSystemService(Context.UsbService);

            foreach (var deviceListValue in usbManager.DeviceList.Values)
            {
                Log.Info(TAG, deviceListValue.ToJSON());
            }

            Log.Info(TAG, "Refreshing device list ...");

            var drivers = await AndroidSerialBase.GetPorts(usbManager);

            if (drivers.Count == 0)
            {
                Log.Info(TAG, "No usb devices");
                return null;
            }

            foreach (var driver in drivers)
            {
                Log.Info(TAG, string.Format("+ {0}: {1} port{2}", driver, drivers.Count, drivers.Count == 1 ? string.Empty : "s"));

                Log.Info(TAG, string.Format("+ {0}: {1} ", driver.Device.ProductName, driver.Device.ManufacturerName));
            }

            var usbdevice = drivers.First(a =>
                di.hardwareid.Contains(a.Device.VendorId.ToString("X4")) &&
                di.hardwareid.Contains(a.Device.ProductId.ToString("X4")));

            var permissionGranted =
                await usbManager.RequestPermissionAsync(usbdevice.Device, Application.Context);
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

                return new AndroidSerial(serialIoManager);
            }

            return null;
        }
    }
}