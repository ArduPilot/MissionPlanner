using Android.App;
using Android.Content;
using Android.Hardware.Usb;
using Android.Util;

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

            Test.UsbDevices.USBEventCallBack(this, device);
        }
    }
}