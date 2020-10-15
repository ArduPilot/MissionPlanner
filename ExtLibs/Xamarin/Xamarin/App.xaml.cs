using log4net;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Acr.UserDialogs;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using NLog;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Device = Xamarin.Forms.Device;
using LogManager = log4net.LogManager;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Xamarin
{

    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Thread httpthread;

        public App()
        {
            InitializeComponent();

            log4net.Repository.Hierarchy.Hierarchy hierarchy =
                (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetAssembly(typeof(App)));

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "[%thread] %-5level %logger - %message";
            patternLayout.ActivateOptions();

            var cca = new ConsoleAppender();
            cca.Layout = patternLayout;
            cca.ActivateOptions();
            hierarchy.Root.AddAppender(cca);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;

            {
                var config = new NLog.Config.LoggingConfiguration();

                // Targets where to log to: File and Console
                var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

                // Rules for mapping loggers to targets            
               // config.AddRule(LogLevel.Warn, LogLevel.Fatal, logconsole);

                // Apply config           
                NLog.LogManager.Configuration = config;
            }

            MainPage = new MainPage();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Warning("Xamarin", e.ExceptionObject.ToString());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            /*
            Task.Run(async () =>
            {
                try
                {
                    var client = new UdpClient(14551, AddressFamily.InterNetwork);
                    client.BeginReceive(clientdata, client);
                }
                catch (Exception ex)
                {
                    Log.Warning("", ex.ToString());
                }
            });

            Task.Run(async () =>
            {
                try
                {
                    var client = new UdpClient(14550, AddressFamily.InterNetwork);
                    client.BeginReceive(clientdata, client);
                }
                catch (Exception ex)
                {
                    Log.Warning("", ex.ToString());
                }
            });
            */
        }

        private CustomMessageBox.DialogResult CustomMessageBox_ShowEvent(string text, string caption = "",
            CustomMessageBox.MessageBoxButtons buttons = CustomMessageBox.MessageBoxButtons.OK,
            CustomMessageBox.MessageBoxIcon icon = CustomMessageBox.MessageBoxIcon.None, string YesText = "Yes",
            string NoText = "No")
        {
            var ans = ShowMessageBoxAsync(text, caption);

            //if (ans)
                return CustomMessageBox.DialogResult.OK;

            //return CustomMessageBox.DialogResult.Cancel;
        }

        public Task<bool> ShowMessageBoxAsync(string message, string caption)
        {
            var tcs = new TaskCompletionSource<bool>();
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var result = await MainPage.DisplayAlert(message, caption,"OK","Cancel");
                    tcs.TrySetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });
            return tcs.Task;
        }

        private IProgressReporterDialogue CreateIProgressReporterDialogue(string title)
        {
            return new ProgressReporterDialogue(title);
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Log.Warning("", "OnSleep");
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            Log.Warning("", "OnResume");
        }


        private void clientdata(IAsyncResult ar)
        {
            var client = ((UdpClient)ar.AsyncState);

            if (client == null || client.Client == null)
                return;
            try
            {
                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;

                var udpclient = new UdpSerial(client);

                var mav = new MAVLinkInterface();
                mav.BaseStream = udpclient;

                MainV2.comPort = mav;
                MainV2.Comports.Add(mav);

                mav.Open(false, true);

                mav.getParamList();
            }
            catch (Exception ex)
            {
                Log.Warning("", ex.ToString());
            }
        }
    }
}
