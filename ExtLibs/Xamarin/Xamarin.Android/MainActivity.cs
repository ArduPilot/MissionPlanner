using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Companion;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Java.Interop;
using Java.Lang.Reflect;
using Mono.Unix;
using SharpDX.Text;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;
using Thread = System.Threading.Thread;

[assembly: UsesFeature("android.hardware.usb.host")]
[assembly: UsesLibrary("org.apache.http.legacy", false)]
[assembly: UsesPermission("android.permission.RECEIVE_D2D_COMMANDS")]

namespace Xamarin.Droid
{
    [IntentFilter(new[] { global::Android.Content.Intent.ActionMain , UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached }, Categories = new []{global::Android.Content.Intent.CategoryHome, global::Android.Content.Intent.CategoryDefault, global::Android.Content.Intent.CategoryLauncher})]
    [Activity(Label = "MissionPlanner", ScreenOrientation = ScreenOrientation.Landscape, Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public UsbDeviceReceiver UsbBroadcastReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();

            UserDialogs.Init(this);

            //register the broadcast receivers
            UsbBroadcastReceiver = new UsbDeviceReceiver(this);
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));

            TelephonyManager telephonyManager = (TelephonyManager)GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(new SignalStrengthBroadcastReceiver(), PhoneStateListenerFlags.SignalStrength);
            telephonyManager.Listen(new SignalStrengthBroadcastReceiver(), PhoneStateListenerFlags.SignalStrengths);

            var d2d = Intent.SetClassName("com.pinecone.telephony", "com.pinecone.telephony.D2DService");
            this.BindService(d2d, new D2DService(), Bind.AutoCreate);

            if(false)
            {
                var d2dinfo = "/tmp/qgccmd";
                var unixep = new UnixEndPoint(d2dinfo);
                var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
                socket.Connect(unixep);
                Task.Run(() =>
                {
                    while (true)
                    {
                        byte[] buffer = new byte[100];
                        var readlen = socket.Receive(buffer);
                        Log.Info("socket", ASCIIEncoding.ASCII.GetString(buffer, 0, readlen));
                        Thread.Sleep(1);
                    }
                });
                
            }


            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Warn("Xamarin", e.ExceptionObject.ToString());
        }


    }
}