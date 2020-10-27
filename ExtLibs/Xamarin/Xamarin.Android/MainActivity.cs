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
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.Bluetooth;
using Android.Runtime;
using AndroidX.Core.App;
using Android.Bluetooth;
using AndroidX.Core.Content;
using Xamarin.Essentials;
using MissionPlanner.GCSViews;
using MissionPlanner.GCSViews.ConfigurationView;
using Environment = Android.OS.Environment;
using Settings = MissionPlanner.Utilities.Settings;
using Thread = System.Threading.Thread;
using Android.Content;
using Android.Provider;
using Android.Widget;
using Hoho.Android.UsbSerial.Util;
using Java.Lang;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using Xamarin.Forms;
using Xamarin.GCSViews;
using Application = Android.App.Application;
using Exception = System.Exception;
using Process = Android.OS.Process;
using String = System.String;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

[assembly: UsesFeature("android.hardware.usb.host", Required = false)]
[assembly: UsesFeature("android.hardware.bluetooth", Required = false)]
[assembly: UsesLibrary("org.apache.http.legacy", false)]
[assembly: UsesPermission("android.permission.RECEIVE_D2D_COMMANDS")]
//[assembly: UsesPermission("android.permission.MANAGE_EXTERNAL_STORAGE")]

namespace Xamarin.Droid
{ //global::Android.Content.Intent.CategoryLauncher
  //global::Android.Content.Intent.CategoryHome,
    [IntentFilter(new[] { global::Android.Content.Intent.ActionMain, global::Android.Content.Intent.ActionAirplaneModeChanged , 
        global::Android.Content.Intent.ActionBootCompleted , UsbManager.ActionUsbDeviceAttached, UsbManager.ActionUsbDeviceDetached, 
        global::Android.Bluetooth.BluetoothDevice.ActionFound, global::Android.Bluetooth.BluetoothDevice.ActionAclConnected, UsbManager.ActionUsbAccessoryAttached}, 
        Categories = new []{ global::Android.Content.Intent.CategoryLauncher})]
    [MetaData("android.hardware.usb.action.USB_DEVICE_ATTACHED", Resource = "@xml/device_filter")]
    [Activity(Label = "Mission Planner", ScreenOrientation = ScreenOrientation.SensorLandscape, Icon = "@mipmap/icon", Theme = "@style/MainTheme", 
        MainLauncher = true, HardwareAccelerated = true, DirectBootAware = true, Immersive = true, LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly string TAG = "MP";
        private Socket server;
        public UsbDeviceReceiver UsbBroadcastReceiver;

        public static MainActivity Current { private set; get; }
        public static readonly int PickImageId = 1000;
        private DeviceDiscoveredReceiver BTBroadcastReceiver;

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

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            SetSupportActionBar((Toolbar) FindViewById(ToolbarResource));

            this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn | WindowManagerFlags.HardwareAccelerated);
            
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Settings.CustomUserDataDirectory = Application.Context.GetExternalFilesDir(null).ToString();
            Log.Info("MP", "Settings.CustomUserDataDirectory " + Settings.CustomUserDataDirectory);

            WinForms.BundledPath = Application.Context.ApplicationInfo.NativeLibraryDir;
            Log.Info("MP", "WinForms.BundledPath " + WinForms.BundledPath);

            Test.BlueToothDevice = new BTDevice();
            Test.UsbDevices = new USBDevices();
            Test.Radio = new Radio();

          
            
            //ConfigFirmwareManifest.ExtraDeviceInfo
            /*
            var intent = new global::Android.Content.Intent(Intent.ActionOpenDocumentTree);

            intent.AddFlags(ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            intent.PutExtra(DocumentsContract.ExtraInitialUri, "Mission Planner");

            StartActivityForResult(intent, 1);
            */

            UserDialogs.Init(this);

            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;

            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) !=
                    (int) Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
                    (int) Permission.Granted ||
                    ContextCompat.CheckSelfPermission(this, Manifest.Permission.Bluetooth) !=
                    (int) Permission.Granted)
                {
                    ActivityCompat.RequestPermissions(this,
                        new String[]
                        {
                            Manifest.Permission.AccessFineLocation, Manifest.Permission.LocationHardware,
                            Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.Bluetooth
                        }, 1);
                }

                while (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
                       (int) Permission.Granted)
                {
                    Thread.Sleep(1000);
                    var text = "Checking Permissions - " + DateTime.Now.ToString("T");

                    DoToastMessage(text);
                }
            }

            try {
                // print some info
                var pm = this.PackageManager;
                var name = this.PackageName;

                var pi = pm.GetPackageInfo(name, PackageInfoFlags.Activities);

                Console.WriteLine("pi.ApplicationInfo.DataDir " + pi?.ApplicationInfo?.DataDir);
                Console.WriteLine("pi.ApplicationInfo.NativeLibraryDir " + pi?.ApplicationInfo?.NativeLibraryDir);

                // api level 24 - android 7
                Console.WriteLine("pi.ApplicationInfo.DeviceProtectedDataDir " +
                                  pi?.ApplicationInfo?.DeviceProtectedDataDir);
            } catch {}

            {
                DoToastMessage("Staging Files");
                try
                {
                    // nofly dir
                    Directory.CreateDirectory(Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "NoFly");

                    // restore assets
                    Directory.CreateDirectory(Settings.GetUserDataDirectory());

                    File.WriteAllText(Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "airports.csv",
                        new StreamReader(Resources.OpenRawResource(
                            Xamarin.Droid.Resource.Raw.airports)).ReadToEnd());

                    File.WriteAllText(
                        Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "BurntKermit.mpsystheme",
                        new StreamReader(
                            Resources.OpenRawResource(
                                Droid.Resource.Raw.BurntKermit)).ReadToEnd());

                    File.WriteAllText(
                        Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "ParameterMetaData.xml",
                        new StreamReader(
                            Resources.OpenRawResource(
                                Droid.Resource.Raw.ParameterMetaDataBackup)).ReadToEnd());

                    File.WriteAllText(
                        Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "camerasBuiltin.xml",
                        new StreamReader(
                            Resources.OpenRawResource(
                                Droid.Resource.Raw.camerasBuiltin)).ReadToEnd());

                    File.WriteAllText(
                        Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "checklistDefault.xml",
                        new StreamReader(
                            Resources.OpenRawResource(
                                Droid.Resource.Raw.checklistDefault)).ReadToEnd());

                    File.WriteAllText(
                        Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar + "mavcmd.xml", new StreamReader(
                            Resources.OpenRawResource(
                                Droid.Resource.Raw.mavcmd)).ReadToEnd());

                    {
                        var pluginsdir = Settings.GetRunningDirectory() + "plugins";
                        Directory.CreateDirectory(pluginsdir);

                        string[] files = new[]
                        {
                            "example2menu", "example3fencedist", "example4herelink", "example5latencytracker",
                            "example6mapicondesc", "example7canrtcm", "example8modechange", "example9hudonoff",
                            "examplewatchbutton", "generator", "InitialParamsCalculator"
                        };

                        foreach (var file in files)
                        {
                            try
                            {
                                var id = (int) typeof(Droid.Resource.Raw)
                                    .GetField(file)
                                    .GetValue(null);

                                var filename = pluginsdir + Path.DirectorySeparatorChar + file + ".cs";

                                if (File.Exists(filename))
                                {
                                    File.Delete(filename);
                                }

                                /*
                                File.WriteAllText(filename
                                    ,
                                    new StreamReader(
                                        Resources.OpenRawResource(id)).ReadToEnd());
                                */
                            }
                            catch
                            {

                            }
                        }
                    }

                    {
                        var graphsdir = Settings.GetRunningDirectory() + "graphs";
                        Directory.CreateDirectory(graphsdir);

                        string[] files = new[]
                        {
                            "ekf3Graphs", "ekfGraphs", "mavgraphs", "mavgraphs2", "mavgraphsMP"
                        };

                        foreach (var file in files)
                        {
                            try
                            {
                                var id = (int) typeof(Droid.Resource.Raw)
                                    .GetField(file)
                                    .GetValue(null);

                                File.WriteAllText(
                                    graphsdir + Path.DirectorySeparatorChar + file + ".xml",
                                    new StreamReader(
                                        Resources.OpenRawResource(id)).ReadToEnd());
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("Error");
                    alert.SetMessage("Failed to save to storage " + ex.ToString());

                    alert.SetNeutralButton("OK", (senderAlert, args) =>
                    {
                        
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            {
                // clean start, see if it was an intent/usb attach
                if (savedInstanceState == null)
                {
                    DoToastMessage("Init Saved State");
                    proxyIfUsbAttached(this.Intent);
                }
            }

            GC.Collect();
            Task.Run(() =>
            {
                var gdaldir = Settings.GetRunningDirectory() + "gdalimages";
                Directory.CreateDirectory(gdaldir);

                MissionPlanner.Utilities.GDAL.GDALBase = new GDAL.GDAL();

                GDAL.GDAL.ScanDirectory(gdaldir);

                GMap.NET.MapProviders.GMapProviders.List.Add(GDAL.GDALProvider.Instance);
            });


            DoToastMessage("Launch App");

            LoadApplication(new App());
        }

        private void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            Log.Error(TAG, e.Exception.ToString());
            Debugger.Break();
        }

        private void DoToastMessage(string text, ToastLength toastLength = ToastLength.Short)
        {
            try
            {
                // thread to force invoke into ui thread
                Task.Run(() =>
                {
                    if (!this.IsFinishing)
                    {
                        //if (Looper.MainLooper.IsCurrentThread)
                        {
                            // On UI thread.
                            RunOnUiThread(() =>
                            {
                                Toast toast = Toast.MakeText(this, text, toastLength);
                                toast.Show();
                            });
                        }
                    }
                });
            } catch {}
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Console.WriteLine("OnNewIntent " + intent.Action);
        }

        private void proxyIfUsbAttached(Intent intent) {

            if (intent == null) return;

            if (!UsbManager.ActionUsbDeviceAttached.Equals(intent.Action)) return;

            Log.Verbose(TAG, "usb device attached");

            WinForms.InitDevice = ()=>
            {
                Log.Info(TAG, "WinForms.InitDevice");
                UsbBroadcastReceiver.OnReceive(this.ApplicationContext, intent);
            };
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
            Log.Error("MP", e.Exception.StackTrace.ToString());
            Debugger.Break();
            e.Handled = true;
            DoToastMessage("ERROR " + e.Exception.Message, ToastLength.Long);
            throw e.Exception;
        }

        protected override void OnResume()
        {
            base.OnResume();

            this.Window.DecorView.SystemUiVisibility =
                (StatusBarVisibility) (SystemUiFlags.LowProfile
                                       | SystemUiFlags.Fullscreen
                                       | SystemUiFlags.HideNavigation
                                       | SystemUiFlags.Immersive
                                       | SystemUiFlags.ImmersiveSticky);

            StartD2DInfo();

            //register the broadcast receivers
            UsbBroadcastReceiver = new UsbDeviceReceiver(this);
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceDetached));
            RegisterReceiver(UsbBroadcastReceiver, new IntentFilter(UsbManager.ActionUsbDeviceAttached));

            // Register for broadcasts when a device is discovered
            BTBroadcastReceiver = new DeviceDiscoveredReceiver(this);
            RegisterReceiver(BTBroadcastReceiver, new IntentFilter(BluetoothDevice.ActionFound));
            RegisterReceiver(BTBroadcastReceiver, new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
        }

        protected override void OnPause()
        {
            base.OnPause();

            StopD2DInfo();

            UnregisterReceiver(UsbBroadcastReceiver);

            UnregisterReceiver(BTBroadcastReceiver);
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
            Log.Error(TAG, e.ExceptionObject.ToString());
            Debugger.Break();
        }

    }
}