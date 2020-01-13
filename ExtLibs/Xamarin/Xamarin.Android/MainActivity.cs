using System;
using Android.App;
using Android.Companion;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Xamarin.Forms;
[assembly: UsesFeature("android.hardware.usb.host")]
[assembly: UsesLibrary("org.apache.http.legacy", false)]

namespace Xamarin.Droid
{
  
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached })]
    [Activity(Label = "Xamarin", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public UsbDeviceReceiver UsbBroadcastReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            Test.TestMethod = new USBDevices();

            //register the broadcast receivers
            UsbBroadcastReceiver = new UsbDeviceReceiver(this);
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        #region UsbDeviceDetachedReceiver implementation

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

                Test.TestMethod.USBEventCallBack(this, device);
            }
        }

        #endregion


    }

}