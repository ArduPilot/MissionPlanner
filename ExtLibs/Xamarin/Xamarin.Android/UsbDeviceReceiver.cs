using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Util;
using Hoho.Android.UsbSerial;
using Hoho.Android.UsbSerial.Driver;

namespace Xamarin.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class UsbDeviceReceiver
        : BroadcastReceiver
    {
        readonly string TAG = "MP-"+typeof(UsbDeviceReceiver).Name;           

        public UsbDeviceReceiver()
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
            var device = intent.GetParcelableExtra(UsbManager.ExtraDevice) as UsbDevice;

            Log.Info(TAG,
                "USB device: " + device.DeviceName + " " + device.ProductName + " " + device.VendorId + " " +
                device.ProductId);

            if (intent.Action.Equals(UsbManager.ActionUsbDeviceAttached))
            {
                Test.UsbDevices.USBEventCallBack(this, device);
            }
        }
    }
}