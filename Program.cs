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
using System.Text.RegularExpressions;

namespace MissionPlanner
{
    public static class Program
    {
        private static readonly ILog log = LogManager.GetLogger("Program");

        public static DateTime starttime = DateTime.Now;

        public static bool vvvvz = false;
        public static Image Logo = null;

        public static Splash Splash;

        internal static Thread Thread;

        public static string[] args = new string[]{};
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Program.args = args;
            Console.WriteLine("If your error is about Microsoft.DirectX.DirectInput, please install the latest directx redist from here http://www.microsoft.com/en-us/download/details.aspx?id=35 \n\n");
            Console.WriteLine("Debug under mono    MONO_LOG_LEVEL=debug mono MissionPlanner.exe");

            Thread = Thread.CurrentThread;

            System.Windows.Forms.Application.EnableVisualStyles();
            XmlConfigurator.Configure();
            log.Info("******************* Logging Configured *******************");
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            ServicePointManager.DefaultConnectionLimit = 10;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // fix ssl on mono
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

            if (args.Length > 0 && args[0] == "/update")
            {
                Utilities.Update.DoUpdate();
            }

            // setup theme provider
            CustomMessageBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            Controls.MainSwitcher.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;
            MissionPlanner.Controls.InputBox.ApplyTheme += MissionPlanner.Utilities.ThemeManager.ApplyThemeTo;

            // setup settings provider
            MissionPlanner.Comms.CommsBase.Settings += CommsBase_Settings;

            // set the cache provider to my custom version
            GMap.NET.GMaps.Instance.PrimaryCache = new Maps.MyImageCache();
            // add my custom map providers
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.WMSProvider.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Custom.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.Earthbuilder.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapBox.Instance);
            GMap.NET.MapProviders.GMapProviders.List.Add(Maps.MapboxNoFly.Instance);

            // add proxy settings
            GMap.NET.MapProviders.GMapProvider.WebProxy = WebRequest.GetSystemWebProxy();
            GMap.NET.MapProviders.GMapProvider.WebProxy.Credentials = CredentialCache.DefaultCredentials;

            string name = "Mission Planner";

            if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "logo.txt"))
                name = File.ReadAllText(Application.StartupPath + Path.DirectorySeparatorChar + "logo.txt", Encoding.UTF8);

            if (File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "logo.png"))
                Logo = new Bitmap(Application.StartupPath + Path.DirectorySeparatorChar + "logo.png");

            if (name == "VVVVZ")
            {
                vvvvz = true;
                // set pw
                MainV2.config["password"] = "viDQSk/lmA2qEE8GA7SIHqu0RG2hpkH973MPpYO87CI=";
                MainV2.config["password_protect"] = "True";
                // prevent wizard
                MainV2.config["newuser"] = "11/02/2014";
                // invalidate update url
                System.Configuration.ConfigurationManager.AppSettings["UpdateLocationVersion"] = "";
            }

            //fontgen.dowork();

            //adsb.server = "64.93.124.152";
            //adsb.serverport = 31001;
            //adsb.serverport = 30003;

            //Utilities.Airports.ReadUNLOCODE(@"C:\Users\hog\Desktop\2013-2 UNLOCODE CodeListPart1.csv");
            //Utilities.Airports.ReadUNLOCODE(@"C:\Users\hog\Desktop\2013-2 UNLOCODE CodeListPart2.csv");
            //Utilities.Airports.ReadUNLOCODE(@"C:\Users\hog\Desktop\2013-2 UNLOCODE CodeListPart3.csv");
            //Utilities.Airports.ReadPartow(@"C:\Users\hog\Desktop\GlobalAirportDatabase.txt");


            /*
            Arduino.ArduinoSTKv2 comport = new Arduino.ArduinoSTKv2();

            comport.PortName = "com8";

            comport.BaudRate = 115200;

            comport.Open();

            Arduino.Chip.Populate();

            if (comport.connectAP())
            {
                Arduino.Chip chip = comport.getChipType();
                Console.WriteLine(chip);
            }
            Console.ReadLine();

            return;
            */
            /*
            Comms.SerialPort sp = new Comms.SerialPort();

            sp.PortName = "com8";
            sp.BaudRate = 115200;

            CurrentState cs = new CurrentState();

            MAVLink mav = new MAVLink();

            mav.BaseStream = sp;

            mav.Open();

            HIL.XPlane xp = new HIL.XPlane();

            xp.SetupSockets(49005, 49000, "127.0.0.1");

            HIL.Hil.sitl_fdm data = new HIL.Hil.sitl_fdm();

            while (true)
            {
                while (mav.BaseStream.BytesToRead > 0)
                    mav.readPacket();

                // update all stats
                cs.UpdateCurrentSettings(null);

                xp.GetFromSim(ref data);
                xp.GetFromAP(); // no function

                xp.SendToAP(data);
                xp.SendToSim();

                MAVLink.mavlink_rc_channels_override_t rc = new MAVLink.mavlink_rc_channels_override_t();

                rc.chan3_raw = 1500;

                mav.sendPacket(rc);
                
            }       */
        
           // return;
          //  OSDVideo vid = new OSDVideo();

         //   vid.ShowDialog();

         //   return;
             
            if (Debugger.IsAttached)
                ThemeManager.doxamlgen();

            if (File.Exists("simple.txt"))
            {
                Application.Run(new GCSViews.Simple());
                return;
            }

            Splash = new MissionPlanner.Splash();
            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Splash.Text = name+" " + Application.ProductVersion + " build " + strVersion;
            Splash.Show();

            Application.DoEvents();

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
        }



        static string CommsBase_Settings(string name, string value, bool set = false)
        {
            if (set) {
                MainV2.config[name] = value;
                return value;
            }

            if (MainV2.config.ContainsKey(name)) {
                return MainV2.config[name].ToString();
            }

            return "";
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            handleException((Exception)e.ExceptionObject);
        }

        static string GetStackTrace(Exception e)
        {
            StackTrace st = new System.Diagnostics.StackTrace(e);
            string stackTrace = "";
            foreach (StackFrame frame in st.GetFrames())
            {
                stackTrace = "at " + frame.GetMethod().Module.Name + "." +
                    frame.GetMethod().ReflectedType.Name + "."
                    + frame.GetMethod().Name
                    + "  (IL offset: 0x" + frame.GetILOffset().ToString("x") + ")\n" + stackTrace;
            }
            Console.Write(stackTrace);
            Console.WriteLine("Message: " + e.Message);

            return stackTrace;
        }

        static void handleException(Exception ex)
        {
            MissionPlanner.Utilities.Tracking.AddException(ex);

            log.Debug(ex.ToString());

            GetStackTrace(ex);

            // hyperlinks error
            if (ex.Message == "Requested registry access is not allowed." || ex.ToString().Contains("System.Windows.Forms.LinkUtilities.GetIELinkBehavior"))
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
            if (ex.GetType() == typeof(MissingMethodException))
            {
                CustomMessageBox.Show("Please Update - Some older library dlls are causing problems\n" + ex.Message);
                return;
            }
            if (ex.GetType() == typeof(ObjectDisposedException) || ex.GetType() == typeof(InvalidOperationException)) // something is trying to update while the form, is closing.
            {
                log.Error(ex);
                return; // ignore
            }
            if (ex.GetType() == typeof(FileNotFoundException) || ex.GetType() == typeof(BadImageFormatException)) // i get alot of error from people who click the exe from inside a zip file.
            {
                CustomMessageBox.Show("You are missing some DLL's. Please extract the zip file somewhere. OR Use the update feature from the menu " + ex.ToString());
                // return;
            }
            // windows and mono
            if (ex.StackTrace.Contains("System.IO.Ports.SerialStream.Dispose") || ex.StackTrace.Contains("System.IO.Ports.SerialPortStream.Dispose"))
            {
                log.Error(ex);
                return; // ignore
            }

            log.Info("Th Name "+Thread.Name);

            DialogResult dr = CustomMessageBox.Show("An error has occurred\n" + ex.ToString() + "\n\nReport this Error???", "Send Error", MessageBoxButtons.YesNo);
            if (DialogResult.Yes == dr)
            {
                try
                {
                    string data = "";
                        foreach (System.Collections.DictionaryEntry de in ex.Data)
                            data += String.Format("-> {0}: {1}", de.Key, de.Value);
              

                    // Create a request using a URL that can receive a post. 
                    WebRequest request = WebRequest.Create("http://vps.oborne.me/mail.php");
                    request.Timeout = 10000; // 10 sec
                    // Set the Method property of the request to POST.
                    request.Method = "POST";
                    // Create POST data and convert it to a byte array.
                    string postData = "message=" + Environment.OSVersion.VersionString + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() 
                        + " " + Application.ProductVersion 
                        + "\nException " + ex.ToString().Replace('&', ' ').Replace('=', ' ') 
                        + "\nStack: " + ex.StackTrace.ToString().Replace('&', ' ').Replace('=', ' ') 
                        + "\nTargetSite " + ex.TargetSite + " " + ex.TargetSite.DeclaringType
                        + "\ndata " + data;
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
    }
}