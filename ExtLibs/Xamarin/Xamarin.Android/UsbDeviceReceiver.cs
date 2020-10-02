using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Util;
using Hoho.Android.UsbSerial;
using Hoho.Android.UsbSerial.Driver;

namespace Xamarin.Droid
{
    public class UsbDeviceReceiver
        : BroadcastReceiver
    {
        readonly string TAG = typeof(UsbDeviceReceiver).Name;
        readonly Activity activity;
            

        public UsbDeviceReceiver(Activity activity)
        {
            this.activity = activity;

        }

        public override void OnReceive(Context context, Intent intent)
        {
            var device = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;

            Log.Info(TAG,
                "USB device: " + device.DeviceName + " " + device.ProductName + " " + device.VendorId + " " +
                device.ProductId);

            if (intent.Action.Equals(UsbManager.ActionUsbDeviceAttached))
            {
                // cdc and composite
                if (device.DeviceClass == UsbClass.Comm ||
                    device.DeviceClass == UsbClass.Misc && device.DeviceSubclass == UsbClass.Comm)
                {
                    var item = (device.VendorId, device.DeviceId);
                    if(!AndroidSerialBase.cdcacmTuples.Contains(item))
                        AndroidSerialBase.cdcacmTuples.Add((device.VendorId, device.DeviceId));
                }

                Test.UsbDevices.USBEventCallBack(this, device);
            }
        }
    }
}