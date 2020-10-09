using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Hoho.Android.UsbSerial;
using Hoho.Android.UsbSerial.Driver;
using Hoho.Android.UsbSerial.Util;
using Java.Lang;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using Exception = System.Exception;
using String = System.String;

namespace Xamarin.Droid
{
    public class USBDevices: IUSBDevices
    {
        static readonly string TAG = "MP";


        public async Task<List<DeviceInfo>> GetDeviceInfoList()
        {
            var usbManager = (UsbManager)Application.Context.GetSystemService(Context.UsbService);

            foreach (var device in usbManager.DeviceList.Values)
            {
                Log.Info(TAG,
                    "GetDeviceInfoList " + device.DeviceName + " " + device.ProductName + " " +
                    device.VendorId + " " + device.ProductId);

                // cdc and composite
                if (device.DeviceClass == UsbClass.Comm ||
                    device.DeviceClass == UsbClass.Misc && device.DeviceSubclass == UsbClass.Comm)
                {
                    var item = (device.VendorId, device.ProductId);
                    if(!AndroidSerialBase.cdcacmTuples.Contains(item))
                        AndroidSerialBase.cdcacmTuples.Add((device.VendorId, device.ProductId));
                }
            }

            Log.Info(TAG,"GetDeviceInfoList "+ "Refreshing device list ...");

            var drivers = await AndroidSerialBase.GetPorts(usbManager);

            if (drivers == null || drivers.Count == 0)
            {
                Log.Info(TAG, "GetDeviceInfoList "+"No usb devices");
                return new List<DeviceInfo>();
            }

            List<DeviceInfo> ans = new List<DeviceInfo>();

            foreach (var driver in drivers.ToArray())
            {
                try
                {
                    Log.Info(TAG, string.Format("GetDeviceInfoList "+"+ {0}: {1} ports {2}", driver, drivers.Count, driver.Ports.Count));

                    Log.Info(TAG, string.Format("GetDeviceInfoList "+"+ {0}: {1} ", driver.Device.ProductName, driver.Device.ManufacturerName));

                    var deviceInfo = GetDeviceInfo(driver.Device);

                    // support one more
                    if (driver.Ports.Count > 1)
                    {
                        var deviceInfo2 = GetDeviceInfo(driver.Device);
                        deviceInfo2.board += "-P2";
                        ans.Add(deviceInfo);
                        ans.Add(deviceInfo2);
                    }
                    else
                    {
                        ans.Add(deviceInfo);
                    }
                }
                catch (Exception e)
                {
                    Log.Error("MP", "GetDeviceInfoList "+e.StackTrace);
                }
            }

            return ans;
        }

        public void USBEventCallBack(object usbDeviceReceiver, object device)
        {
            USBEvent?.Invoke(usbDeviceReceiver, GetDeviceInfo(device));
        }

        public event EventHandler<DeviceInfo> USBEvent;

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
                hardwareid = String.Format("USB\\VID_{0:X4}&PID_{1:X4}&", device.VendorId, device.ProductId),
                name = device.DeviceName
            };
            return deviceInfo;
        }


        public async Task<ICommsSerial> GetUSB(DeviceInfo di)
        {
            var usbManager = (UsbManager) Application.Context.GetSystemService(Context.UsbService);
            
            foreach (var deviceListValue in usbManager.DeviceList.Values)
            {
                Log.Info(TAG,"GetUSB "+ deviceListValue.DeviceName);
            }

            Log.Info(TAG, "GetUSB "+"Refreshing device list ...");

            var drivers = await AndroidSerialBase.GetPorts(usbManager);

            if (drivers.Count == 0)
            {
                Log.Info(TAG, "GetUSB "+"No usb devices");
                return null;
            }

            foreach (var driver in drivers.ToArray())
            {
                Log.Info(TAG, string.Format("GetUSB "+"+ {0}: {1} ports {2}", driver, drivers.Count, driver.Ports.Count));

                Log.Info(TAG, string.Format("GetUSB "+"+ {0}: {1} ", driver.Device.ProductName, driver.Device.ManufacturerName));
            }

            var usbdevice = drivers.First(a =>
                di.hardwareid.Contains(a.Device.VendorId.ToString("X4")) &&
                di.hardwareid.Contains(a.Device.ProductId.ToString("X4")));

            var hasPermission = usbManager.HasPermission(usbdevice.Device);

            var permissionGranted =
                await usbManager.RequestPermissionAsync(usbdevice.Device, Application.Context);
            if (permissionGranted)
            {
                if (!hasPermission)
                    return await GetUSB(di);

                var portnumber = 0;
                var port = usbdevice.Ports.First();
                if (usbdevice.Ports.Count > 1)
                {
                    if (di.board.EndsWith("-P2"))
                    {
                        port = usbdevice.Ports[1];
                        portnumber = 1;
                    }
                }
                /*
                var defaultport = usbdevice.Ports.First();
                if (usbdevice.Ports.Count > 1)
                {
                    ManualResetEvent mre = new ManualResetEvent(false);

                    var handler = new Handler(MainActivity.Current.MainLooper);

                    handler.Post(() =>
                    {
                        AlertDialog.Builder alert = new AlertDialog.Builder(MainActivity.Current);
                        alert.SetTitle("Multiple Ports");
                        alert.SetCancelable(false);
                        var items = usbdevice.Ports.Select(a =>
                                a.Device.GetInterface(a.PortNumber).Name ?? a.PortNumber.ToString())
                            .ToArray();
                        alert.SetSingleChoiceItems(items, 0, (sender, args) =>
                        {
                            defaultport = usbdevice.Ports[args.Which];
                        });

                        alert.SetNeutralButton("OK", (senderAlert, args) => { mre.Set(); });

                        Dialog dialog = alert.Create();
                        if(!MainActivity.Current.IsFinishing)
                            dialog.Show();
                    });

                    mre.WaitOne();
                }
                */

                var portInfo = new UsbSerialPortInfo(port);

                int vendorId = portInfo.VendorId;
                int deviceId = portInfo.DeviceId;

                Log.Info(TAG,
                    string.Format("GetUSB " + "VendorId: {0} DeviceId: {1} PortNumber: {2}", vendorId, deviceId,
                        portnumber));

                var serialIoManager = new SerialInputOutputManager(usbManager, port);

                return new AndroidSerial(serialIoManager) {PortName = usbdevice.Device.ProductName};
            }

            return null;
        }
    }
}