using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;
using System.Diagnostics;
using System.Linq;
using MissionPlanner.Utilities;
using MissionPlanner;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MissionPlanner
{
    public static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");

        public static DateTime starttime = DateTime.Now;

        public static string name { get; internal set; }

        public static bool WindowsStoreApp { get { return Application.ExecutablePath.Contains("WindowsApps"); } }

        public static Image Logo = null;
        public static Image IconFile = null;

        public static Splash Splash;

        internal static Thread Thread;

        public static string[] args = new string[] {};
        public static Bitmap SplashBG = null;

        public static string[] names = new string[] { "VVVVZ" };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Program.args = args;
            Console.WriteLine(
                "If your error is about Microsoft.DirectX.DirectInput, please install the latest directx redist from here http://www.microsoft.com/en-us/download/details.aspx?id=35 \n\n");
            Console.WriteLine("Debug under mono    MONO_LOG_LEVEL=debug mono MissionPlanner.exe");

            Thread = Thread.CurrentThread;

            System.Windows.Forms.Application.EnableVisualStyles();
            XmlConfigurator.Configure();
            log.Info("******************* Logging Configured *******************");
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            ServicePointManager.DefaultConnectionLimit = 10;

            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // fix ssl on mono
            ServicePointManager.ServerCertificateValidationCallback =
                new System.Net.Security.RemoteCertificateValidationCallback(
                    (sender, certificate, chain, policyErrors) => { return true; });

            if (args.Length > 0 && args[0] == "/update")
            {
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


            Splash = new MissionPlanner.Splash();
            if (SplashBG != null)
            {
                Splash.BackgroundImage = SplashBG;
                Splash.pictureBox1.Visible = false;
            }

            if (IconFile != null)
                Splash.Icon = Icon.FromHandle(((Bitmap)IconFile).GetHicon());

            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Splash.Text = name + " " + Application.ProductVersion + " build " + strVersion;
            Splash.Show();

            Application.DoEvents();
            Application.DoEvents();

            // setup theme provider
            CustomMessageBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            Controls.MainSwitcher.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            MissionPlanner.Controls.InputBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            Controls.BackstageView.BackstageViewPage.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;

            // setup settings provider
            MissionPlanner.Comms.CommsBase.Settings += CommsBase_Settings;
            MissionPlanner.Comms.CommsBase.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;

            // set the cache provider to my custom version
            GMap.NET.GMaps.Instance.PrimaryCache = new Maps.MyImageCache();
            // add my custom map providers
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.WMSProvider.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Custom.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Earthbuilder.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Statkart_Topo2.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapBox.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapboxNoFly.Instance);
            // optionally add gdal support
            if (Directory.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "gdal"))
                GMap.NET.MapProviders.GMapProviders.List.Add(GDAL.GDALProvider.Instance);

            // add proxy settings
            GMap.NET.MapProviders.GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMap.NET.MapProviders.GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

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

            log.InfoFormat("64bit os {0}, 64bit process {1}", System.Environment.Is64BitOperatingSystem,
                System.Environment.Is64BitProcess);


            Device.DeviceStructure test1 = new Device.DeviceStructure(73225);
            Device.DeviceStructure test2 = new Device.DeviceStructure(262434);
            Device.DeviceStructure test3 = new Device.DeviceStructure(131874);

            MAVLink.MavlinkParse tmp = new MAVLink.MavlinkParse();
            MAVLink.mavlink_heartbeat_t hb = new MAVLink.mavlink_heartbeat_t()
            {
                autopilot = 1,
                base_mode = 2,
                custom_mode = 3,
                mavlink_version = 2,
                system_status = 6,
                type = 7
            };
            var t1 = tmp.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);
            var t2 = tmp.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);
            tmp.GenerateMAVLinkPacket10(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);
            tmp.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb);

            tmp.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, true);
            tmp.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.HEARTBEAT, hb, true);


            try
            {
                //System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.RealTime;
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
                if (Controls.SITL.simulator != null)
                    Controls.SITL.simulator.Kill();
            }
            catch
            {
            }
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
            handleException((Exception) e.ExceptionObject);
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
            if (ex.Message == "The port is closed.")
            {
                CustomMessageBox.Show("Serial connection has been lost");
                return;
            }
            if (ex.Message == "A device attached to the system is not functioning.")
            {
                CustomMessageBox.Show("Serial connection has been lost");
                return;
            }
            if (ex.GetType() == typeof(OpenTK.Graphics.GraphicsContextException))
            {
                CustomMessageBox.Show("Please update your graphics card drivers. Failed to create opengl surface\n" + ex.Message);
                return;
            }
            if (ex.GetType() == typeof (MissingMethodException) || ex.GetType() == typeof (TypeLoadException))
            {
                CustomMessageBox.Show("Please Update - Some older library dlls are causing problems\n" + ex.Message);
                return;
            }
            if (ex.GetType() == typeof (ObjectDisposedException) || ex.GetType() == typeof (InvalidOperationException))
                // something is trying to update while the form, is closing.
            {
                log.Error(ex);
                return; // ignore
            }
            if (ex.GetType() == typeof (FileNotFoundException) || ex.GetType() == typeof (BadImageFormatException))
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

            log.Info("Th Name " + Thread.Name);

            DialogResult dr =
                CustomMessageBox.Show("An error has occurred\n" + ex.ToString() + "\n\nReport this Error???",
                    "Send Error", MessageBoxButtons.YesNo);
            if (DialogResult.Yes == dr)
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
                                      + "\nmessage " + message.Replace('&', ' ').Replace('=', ' ');
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
                        Console.WriteLine(((HttpWebResponse) response).StatusDescription);
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
    }
}