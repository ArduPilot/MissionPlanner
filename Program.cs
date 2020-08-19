﻿using GMap.NET.MapProviders;
using log4net;
using log4net.Config;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Diagnostics.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Architecture = System.Runtime.InteropServices.Architecture;

namespace MissionPlanner
{
    public static class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static DateTime starttime = DateTime.Now;

        public static string name { get; internal set; }

        public static bool WindowsStoreApp
        {
            get { return Application.ExecutablePath.Contains("WindowsApps"); }
        }

        /// <summary>
        /// MissionPlanner text image
        /// </summary>
        public static Image Logo = null;
        /// <summary>
        /// Ardupilot logo
        /// </summary>
        public static Image Logo2 = null;
        /// <summary>
        /// icon
        /// </summary>
        public static Image IconFile = null;

        public static Splash Splash;

        internal static Thread Thread;

        public static string[] args = new string[] { };
        public static Bitmap SplashBG = null;

        public static string[] names = new string[] { "VVVVZ" };
        public static bool MONO = false;

        static Program()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            //AppDomain.CurrentDomain.AssemblyResolve += Resolver;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Start(args);
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Start(string[] args)
        {
            Program.args = args;
            Console.WriteLine(
                "If your error is about Microsoft.DirectX.DirectInput, please install the latest directx redist from here http://www.microsoft.com/en-us/download/details.aspx?id=35 \n\n");
            Console.WriteLine("Debug under mono    MONO_LOG_LEVEL=debug mono MissionPlanner.exe");
            Console.WriteLine("To fix any filename case issues under mono use    export MONO_IOMAP=drive:case");

            Console.WriteLine("Data Dir " + Settings.GetDataDirectory());
            Console.WriteLine("Log Dir " + Settings.GetDefaultLogDir());
            Console.WriteLine("Running Dir " + Settings.GetRunningDirectory());
            Console.WriteLine("User Data Dir " + Settings.GetUserDataDirectory());

            var t = Type.GetType("Mono.Runtime");
            MONO = (t != null);

            Directory.SetCurrentDirectory(Settings.GetRunningDirectory());

            var listener = new TextWriterTraceListener(Settings.GetDataDirectory() + Path.DirectorySeparatorChar + "trace.log",
                "defaulttrace");

            if (args.Any(a=>a.Contains("trace")))
                Trace.Listeners.Add(listener);

            Thread = Thread.CurrentThread;

            System.Windows.Forms.Application.EnableVisualStyles();
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetCallingAssembly()));
            log.Info("******************* Logging Configured *******************");

            ServicePointManager.DefaultConnectionLimit = 10;

            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            if (args.Length > 0 && args[0] == "/update")
            {
                Utilities.Update.DoUpdate();
                return;
            }
            if (args.Length > 0 && args[0] == "/updatebeta")
            {
                Utilities.Update.dobeta = true;
                Utilities.Update.DoUpdate();
                return;
            }

            name = "Mission Planner";

            try
            {
                if (File.Exists(Settings.GetRunningDirectory() + "logo.txt"))
                    name = File.ReadAllLines(Settings.GetRunningDirectory() + "logo.txt",
                        Encoding.UTF8)[0];
            }
            catch
            {
            }

            if (File.Exists(Settings.GetRunningDirectory() + "logo.png"))
                Logo = new Bitmap(Settings.GetRunningDirectory() + "logo.png");

            if (File.Exists(Settings.GetRunningDirectory() + "logo2.png"))
                Logo2 = new Bitmap(Settings.GetRunningDirectory() + "logo2.png");

            if (File.Exists(Settings.GetRunningDirectory() + "icon.png"))
            {
                // 128*128
                IconFile = new Bitmap(Settings.GetRunningDirectory() + "icon.png");
            }
            else
            {
                IconFile = MissionPlanner.Properties.Resources.mpdesktop.ToBitmap();
            }

            if (File.Exists(Settings.GetRunningDirectory() + "splashbg.png")) // 600*375
                SplashBG = new Bitmap(Settings.GetRunningDirectory() + "splashbg.png");

            try
            {
                var file = MissionPlanner.Utilities.NativeLibrary.GetLibraryPathname("libSkiaSharp");
                log.Info(file);
                IntPtr ptr;
                if(MONO)
                    ptr = MissionPlanner.Utilities.NativeLibrary.dlopen(file+".so", MissionPlanner.Utilities.NativeLibrary.RTLD_NOW);
                else
                    ptr = MissionPlanner.Utilities.NativeLibrary.LoadLibrary(file+".dll");

                if (ptr != IntPtr.Zero)
                {
                    log.Info("SkiaLoaded");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Splash = new MissionPlanner.Splash();
            if (SplashBG != null)
            {
                Splash.BackgroundImage = SplashBG;
                Splash.pictureBox1.Visible = false;
            }

            if (IconFile != null)
                Splash.Icon = Icon.FromHandle(((Bitmap)IconFile).GetHicon());

            string strVersion = File.Exists("version.txt")
                ? File.ReadAllText("version.txt")
                : System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Splash.Text = name + " " + Application.ProductVersion + " build " + strVersion;
            Splash.Show();

            if (Debugger.IsAttached)
                Splash.TopMost = false;

            Application.DoEvents();
            Application.DoEvents();

            CustomMessageBox.ShowEvent += (text, caption, buttons, icon, yestext, notext) =>
                {
                    return (CustomMessageBox.DialogResult)(int)MsgBox.CustomMessageBox.Show(text, caption,
                        (MessageBoxButtons)(int)buttons, (MessageBoxIcon)(int)icon, yestext, notext);
                };

            // setup theme provider
            MsgBox.CustomMessageBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            Controls.MainSwitcher.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            MissionPlanner.Controls.InputBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            Controls.BackstageView.BackstageViewPage.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;

            Controls.MainSwitcher.Tracking += MissionPlanner.Utilities.Tracking.AddPage;
            Controls.BackstageView.BackstageView.Tracking += MissionPlanner.Utilities.Tracking.AddPage;

            // setup settings provider
            MissionPlanner.Comms.CommsBase.Settings += CommsBase_Settings;
            MissionPlanner.Comms.CommsBase.InputBoxShow += CommsBaseOnInputBoxShow;
            MissionPlanner.Comms.CommsBase.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            MissionPlanner.Comms.SerialPort.GetDeviceName += SerialPort_GetDeviceName;

            MissionPlanner.Utilities.Extensions.MessageLoop = new Action(() => Application.DoEvents());

            // set the cache provider to my custom version
            GMap.NET.GMaps.Instance.PrimaryCache = new Maps.MyImageCache();
            // add my custom map providers
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.WMSProvider.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Custom.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Earthbuilder.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Statkart_Topo2.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Eniro_Topo.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapBox.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapboxNoFly.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapboxUser.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_Lake.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_1974.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_1979.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_1984.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_1988.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_Relief.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_Slopezone.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Japan_Sea.Instance);

            GoogleMapProvider.APIKey = "AIzaSyA5nFp39fEHruCezXnG3r8rGyZtuAkmCug";
            if (Settings.Instance["GoogleApiKey"] != null) GoogleMapProvider.APIKey = Settings.Instance["GoogleApiKey"];
            
            Tracking.productName = Application.ProductName;
            Tracking.productVersion = Application.ProductVersion;
            Tracking.currentCultureName = Application.CurrentCulture.Name;
            Tracking.primaryScreenBitsPerPixel = Screen.PrimaryScreen.BitsPerPixel;
            Tracking.boundsWidth = Screen.PrimaryScreen.Bounds.Width;
            Tracking.boundsHeight = Screen.PrimaryScreen.Bounds.Height;

            Settings.Instance.UserAgent = Application.ProductName + " " + Application.ProductVersion + " (" + Environment.OSVersion.VersionString + ")";

            // optionally add gdal support
            if (Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "gdal"))
                GMap.NET.MapProviders.GMapProviders.List.Add(GDAL.GDALProvider.Instance);

            // add proxy settings
            GMap.NET.MapProviders.GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMap.NET.MapProviders.GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            // generic status report screen
            MAVLinkInterface.CreateIProgressReporterDialogue += title =>
                new ProgressReporterDialogue() { StartPosition = FormStartPosition.CenterScreen, Text = title };

            WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();
            WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            if (name == "VVVVZ")
            {
                // set pw
                Settings.Instance["password"] = "viDQSk/lmA2qEE8GA7SIHqu0RG2hpkH973MPpYO87CI=";
                Settings.Instance["password_protect"] = "True";
                // prevent wizard
                Settings.Instance["newuser"] = "11/02/2014";
                // invalidate update url
                System.Configuration.ConfigurationManager.AppSettings["UpdateLocationVersion"] = "";
            }

            CleanupFiles();

            log.InfoFormat("64bit os {0}, 64bit process {1}, OS Arch {2}", System.Environment.Is64BitOperatingSystem,
                System.Environment.Is64BitProcess, RuntimeInformation.OSArchitecture);

            log.InfoFormat("Runtime Version {0}",
                System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion);

            try
            {
                log.Debug(Process.GetCurrentProcess().Modules.ToJSON());
            }
            catch
            {
            }

            Type type = Type.GetType("Mono.Runtime");
            if (type != null)
            {
                MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
                if (displayName != null)
                {
                    log.Info(displayName.Invoke(null, null));
                    //6.6.0.161 (tarball Tue Dec 10 10:36:32 UTC 2019)

                    var match = Regex.Match(displayName.Invoke(null, null).ToString(), @"([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)");
                    if(match.Success)
                    {
                        if (int.Parse(match.Groups[1].Value) < 6)
                        {
                            CustomMessageBox.Show(
                                "Please upgrade your mono version to 6+ https://www.mono-project.com/download/stable/");
                        }
                    }
                }
            }

            try
            {
                Thread.CurrentThread.Name = "Base Thread";
                Application.Run(new MainV2());
            }
            catch (Exception ex)
            {
                log.Fatal("Fatal app exception", ex);
                Console.WriteLine(ex.ToString());

                Console.WriteLine("\nPress any key to exit!");
                Console.ReadLine();
            }

            try
            {
                // kill sim background process if its still running
                GCSViews.SITL.simulator.ForEach(a =>
                {
                    try
                    {
                        a.Kill();
                    }
                    catch { }
                });
            }
            catch
            {
            }
        }

        private static string SerialPort_GetDeviceName(string port)
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_SerialPort");                // Win32_USBControllerDevice
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    //DeviceID
                    if (obj2.Properties["DeviceID"].Value.ToString().ToUpper() == port.ToUpper())
                    {
                        return obj2.Properties["Name"].Value.ToString();
                    }
                }
            }

            return "";
        }

        private static void LoadDlls()
        {
            var dlls = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.dll");

            foreach (var dll in dlls)
            {
                try
                {
                    log.Debug("Load: " + dll);
                    Assembly.LoadFile(dll);
                }
                catch (Exception ex)
                {
                    log.Debug(ex);
                }
            }
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            log.Debug("FirstChanceException in: " + e.Exception.Source, e.Exception);
        }

        private static Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            log.Debug("TypeResolve Failed: " + args.Name + " from " + args.RequestingAssembly);
            return null;
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            log.Debug("Loaded: " + args.LoadedAssembly);
        }

        private static inputboxreturn CommsBaseOnInputBoxShow(string title, string prompttext, ref string text)
        {
            var ans = InputBox.Show(title, prompttext, ref text);

            if (ans == DialogResult.Cancel || ans == DialogResult.Abort)
                return inputboxreturn.Cancel;
            if (ans == DialogResult.OK)
                return inputboxreturn.OK;

            return inputboxreturn.NotSet;
        }

        static void CleanupFiles()
        {
            try
            {
                //cleanup bad file
                string file = Settings.GetRunningDirectory() +
                              @"LogAnalyzer\tests\TestUnderpowered.py";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch { }

            try
            {
                var file = "NumpyDotNet.dll";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {

            }

            try
            {
                var file = "px4uploader.exe";
                var file1 = "px4uploader.dll";
                if (File.Exists(file1) && File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {

            }

            try
            {
                var file = "libSkiaSharp.dll";
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {

            }

            try
            {
                foreach (string newupdater in Directory.GetFiles(Settings.GetRunningDirectory(), "Updater.exe*.new"))
                {
                    File.Copy(newupdater, newupdater.Remove(newupdater.Length - 4), true);
                    File.Delete(newupdater);
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during update", ex);
            }

            try
            {
                foreach (string newupdater in Directory.GetFiles(Settings.GetRunningDirectory(), "tlogThumbnailHandler.dll.new"))
                {
                    File.Copy(newupdater, newupdater.Remove(newupdater.Length - 4), true);
                    File.Delete(newupdater);
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception during update", ex);
            }
        }


        static string CommsBase_Settings(string name, string value, bool set = false)
        {
            if (set)
            {
                Settings.Instance[name] = value;
                return value;
            }

            if (Settings.Instance.ContainsKey(name))
            {
                return Settings.Instance[name].ToString();
            }

            return "";
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var list = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            log.Error(list);

            handleException((Exception)e.ExceptionObject);
        }

        static string GetStackTrace(Exception e)
        {
            string stackTrace = "";
            try
            {
                StackTrace st = new System.Diagnostics.StackTrace(e);
                foreach (StackFrame frame in st.GetFrames())
                {
                    stackTrace = "at " + frame.GetMethod().Module.Name + "." +
                                 frame.GetMethod().ReflectedType.Name + "."
                                 + frame.GetMethod().Name
                                 + "  (IL offset: 0x" + frame.GetILOffset().ToString("x") + ")\n" + stackTrace;
                }
                Console.Write(stackTrace);
                Console.WriteLine("Message: " + e.Message);
            }
            catch
            {
            }
            return stackTrace;
        }

        static void handleException(Exception ex)
        {
            if (ex.Message == "Safe handle has been closed")
            {
                return;
            }

            if (MainV2.instance != null && MainV2.instance.IsDisposed)
                return;

            MissionPlanner.Utilities.Tracking.AddException(ex);

            log.Debug(ex.ToString());

            GetStackTrace(ex);

            // hyperlinks error
            if (ex.Message == "Requested registry access is not allowed." ||
                ex.ToString().Contains("System.Windows.Forms.LinkUtilities.GetIELinkBehavior"))
            {
                return;
            }
            if (ex.Message.Contains("The port is closed"))
            {
                CustomMessageBox.Show("Serial connection has been lost");
                return;
            }
            if (ex.Message.Contains("Array.Empty"))
            {
                CustomMessageBox.Show("Please install Microsoft Dot Net 4.6.2");
                Application.Exit();
                return;
            }
            if (ex.Message.Contains("A device attached to the system is not functioning"))
            {
                CustomMessageBox.Show("Serial connection has been lost");
                return;
            }
            if (ex.GetType() == typeof(OpenTK.Graphics.GraphicsContextException))
            {
                CustomMessageBox.Show("Please update your graphics card drivers. Failed to create opengl surface\n" + ex.Message);
                return;
            }
            if (ex.GetType() == typeof(MissingMethodException) || ex.GetType() == typeof(TypeLoadException))
            {
                CustomMessageBox.Show("Please Update - Some older library dlls are causing problems\n" + ex.Message);
                return;
            }
            if (ex.GetType() == typeof(ObjectDisposedException) || ex.GetType() == typeof(InvalidOperationException))
            // something is trying to update while the form, is closing.
            {
                log.Error(ex);
                return; // ignore
            }
            if (ex.GetType() == typeof(FileNotFoundException) || ex.GetType() == typeof(BadImageFormatException))
            // i get alot of error from people who click the exe from inside a zip file.
            {
                CustomMessageBox.Show(
                    "You are missing some DLL's. Please extract the zip file somewhere. OR Use the update feature from the menu " +
                    ex.ToString());
                // return;
            }
            // windows and mono
            if (ex.StackTrace != null && ex.StackTrace.Contains("System.IO.Ports.SerialStream.Dispose") ||
                ex.StackTrace != null && ex.StackTrace.Contains("System.IO.Ports.SerialPortStream.Dispose"))
            {
                log.Error(ex);
                return; // ignore
            }

            log.Info("Th Name " + Thread?.Name);

            var dr =
                CustomMessageBox.Show("An error has occurred\n" + ex.ToString() + "\n\nReport this Error???",
                    "Send Error", MessageBoxButtons.YesNo);
            if ((int)DialogResult.Yes == dr)
            {
                try
                {
                    string data = "";
                    foreach (System.Collections.DictionaryEntry de in ex.Data)
                        data += String.Format("-> {0}: {1}", de.Key, de.Value);

                    string message = "";

                    try
                    {
                        Controls.InputBox.Show("Message", "Please enter a message about this error if you can.",
                            ref message);
                    }
                    catch
                    {
                    }

                    string processinfo = "";

                    try
                    {
                        var result = new Dictionary<int, string[]>();

                        var pid = Process.GetCurrentProcess().Id;

                        using (var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive))
                        {
                            ClrInfo runtimeInfo = dataTarget.ClrVersions[0];
                            var runtime = runtimeInfo.CreateRuntime();

                            foreach (var t in runtime.Threads)
                            {
                                result.Add(
                                    t.ManagedThreadId,
                                    t.StackTrace.Select(f =>
                                    {
                                        if (f.Method != null)
                                        {
                                            return f.Method.Type.Name + "." + f.Method.Name;
                                        }

                                        return null;
                                    }).ToArray()
                                );
                            }
                        }

                        processinfo = result.ToJSON(Formatting.Indented); //;Process.GetCurrentProcess().Modules.ToJSON();
                    }
                    catch
                    {
                       
                    }

                    // Create a request using a URL that can receive a post.
                    WebRequest request = WebRequest.Create("http://vps.oborne.me/mail.php");
                    request.Timeout = 10000; // 10 sec
                    // Set the Method property of the request to POST.
                    request.Method = "POST";
                    // Create POST data and convert it to a byte array.
                    string postData = "message=" + Environment.OSVersion.VersionString + " " +
                                      System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                                      + " " + Application.ProductVersion
                                      + "\nException " + ex.ToString().Replace('&', ' ').Replace('=', ' ')
                                      + "\nStack: " + ex.StackTrace.ToString().Replace('&', ' ').Replace('=', ' ')
                                      + "\nTargetSite " + ex.TargetSite + " " + ex.TargetSite.DeclaringType
                                      + "\ndata " + data
                                      + "\nmessage " + message.Replace('&', ' ').Replace('=', ' ')
                                      + "\n\n" + processinfo;
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    // Set the ContentType property of the WebRequest.
                    request.ContentType = "application/x-www-form-urlencoded";
                    // Set the ContentLength property of the WebRequest.
                    request.ContentLength = byteArray.Length;
                    // Get the request stream.
                    using (Stream dataStream = request.GetRequestStream())
                    {
                        // Write the data to the request stream.
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }
                    // Get the response.
                    using (WebResponse response = request.GetResponse())
                    {
                        // Display the status.
                        Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                        // Get the stream containing content returned by the server.
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            // Open the stream using a StreamReader for easy access.
                            using (StreamReader reader = new StreamReader(dataStream))
                            {
                                // Read the content.
                                string responseFromServer = reader.ReadToEnd();
                                // Display the content.
                                Console.WriteLine(responseFromServer);
                            }
                        }
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString());
                    log.Error(exp);
                    CustomMessageBox.Show("Could not send report! Typically due to lack of internet connection.");
                }
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;

            handleException(ex);
        }

        //kill -QUIT pid
        [DllImport("__Internal")]
        public static extern void mono_threads_request_thread_dump();

    }
}
