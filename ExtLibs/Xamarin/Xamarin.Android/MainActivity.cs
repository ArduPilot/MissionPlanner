using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware.Usb;
using Android.OS;
using Android.Util;
using Android.Views;
using Mono.Unix;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Android;
using Android.Runtime;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Xamarin.Essentials;
using MissionPlanner.GCSViews;
using Environment = Android.OS.Environment;
using Settings = MissionPlanner.Utilities.Settings;
using Thread = System.Threading.Thread;
using Android.Content;
using Java.Lang;
using Xamarin.GCSViews;
using Exception = System.Exception;
using Process = Android.OS.Process;
using String = System.String;

[assembly: UsesFeature("android.hardware.usb.host", Required = false)]
[assembly: UsesLibrary("org.apache.http.legacy", false)]
[assembly: UsesPermission("android.permission.RECEIVE_D2D_COMMANDS")]

namespace Xamarin.Droid
{ //global::Android.Content.Intent.CategoryLauncher
  //global::Android.Content.Intent.CategoryHome,
    [IntentFilter(new[] { global::Android.Content.Intent.ActionMain, global::Android.Content.Intent.ActionAirplaneModeChanged , global::Android.Content.Intent.ActionBootCompleted , UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached }, Categories = new []{ global::Android.Content.Intent.CategoryDefault})]
    [Activity(Label = "Mission Planner", ScreenOrientation = ScreenOrientation.SensorLandscape, Icon = "@mipmap/icon", Theme = "@style/MainTheme", 
        MainLauncher = true, HardwareAccelerated = true)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string TAG = typeof(MainActivity).FullName;
        private Socket server;
        public UsbDeviceReceiver UsbBroadcastReceiver;

        public static MainActivity Current { private set; get; }
        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<string> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    // Set the filename as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(data.DataString);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Current = this;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetSupportActionBar((Toolbar) FindViewById(ToolbarResource));

            this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn | WindowManagerFlags.HardwareAccelerated);
            this.Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Settings.CustomUserDataDirectory = Application.Context.GetExternalFilesDir(null).ToString();

            WinForms.BundledPath = Application.Context.ApplicationInfo.NativeLibraryDir;

            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();

            UserDialogs.Init(this);

            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) !=
                    (int) Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
                    (int) Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this,
                        new String[]
                        {
                            Manifest.Permission.AccessFineLocation, Manifest.Permission.LocationHardware,
                            Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage
                        }, 1);
                }

                while (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
                       (int) Permission.Granted)
                {
                    Thread.Sleep(1000);
                }
            }

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : Permissions.BasePermission
        {
            Console.WriteLine("Check Perm " + permission.ToString());
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted)
            {
                Console.WriteLine("Request Perm " + permission.ToString());
                status = await permission.RequestAsync();
            }

            Console.WriteLine("Status Perm " + permission.ToString() + " " + status);
            return status;
        }

        private async Task CheckPerm()
        {
            await CheckAndRequestPermissionAsync((new Permissions.LocationWhenInUse()));
            await CheckAndRequestPermissionAsync((new Permissions.StorageWrite()));
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            Log.Error("MP", e.Exception.ToString());
            Debugger.Break();
            e.Handled = true;
            throw e.Exception;
        }

        protected override void OnResume()
        {
            base.OnResume();

            StartD2DInfo();

            //register the broadcast receivers
            UsbBroadcastReceiver = new UsbDeviceReceiver(this);
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));
        }

        protected override void OnPause()
        {
            base.OnPause();

            StopD2DInfo();

            UnregisterReceiver(UsbBroadcastReceiver);            
        }

        public void StopD2DInfo()
        {
            server.Close();
            server = null;
        }

        public void StartD2DInfo()
        {
            {
                try
                {
                    //var d2dinfo = new UnixEndPoint("/tmp/d2dinfo");
                    //var d2dinfo = "songdebugmessage";
                    var d2dinfo = "linkstate";
                    //"d2dsignal";

                    server = new Socket(AddressFamily.Unix, SocketType.Stream, 0);
                    server.Bind(new AbstractUnixEndPoint(d2dinfo));

                    server.Listen(50);

                    Task.Run(() =>
                    {
                        while (server != null)
                        {
                            try
                            {
                                var socket = server.Accept();
                                Thread.Sleep(1);
                                byte[] buffer = new byte[100];
                                var readlen = 0;
                                do
                                {
                                    readlen = socket.Receive(buffer);
                                    if ((readlen > 4) && (readlen >= (4 + buffer[3])))
                                    {
                                        Log.Info(TAG, "Got " + ASCIIEncoding.ASCII.GetString(buffer, 4, buffer[3]));
                                    }
                                } while (readlen > 0);
                                socket.Close();

                            }
                            catch (Exception ex) { Log.Warn(TAG, ex.ToString()); Thread.Sleep(1000); }
                        }
                    });

                }
                catch (Exception ex) { Log.Warn(TAG, ex.ToString()); }
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Warn(TAG, e.ExceptionObject.ToString());
        }


    }
}