using log4net;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;


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

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts

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

            // setup http server
            try
            {
                log.Info("start http");
                httpthread = new Thread(new httpserver().listernforclients)
                {
                    Name = "motion jpg stream-network kml",
                    IsBackground = true
                };
                httpthread.Start();
            }
            catch (Exception ex)
            {
                log.Error("Error starting TCP listener thread: ", ex);
                CustomMessageBox.Show(ex.ToString());
            }

            CustomMessageBox.ShowEvent += CustomMessageBox_ShowEvent;
            MAVLinkInterface.CreateIProgressReporterDialogue += CreateIProgressReporterDialogue;

        }

        private CustomMessageBox.DialogResult CustomMessageBox_ShowEvent(string text, string caption = "",
            CustomMessageBox.MessageBoxButtons buttons = CustomMessageBox.MessageBoxButtons.OK,
            CustomMessageBox.MessageBoxIcon icon = CustomMessageBox.MessageBoxIcon.None, string YesText = "Yes",
            string NoText = "No")
        {
            var ans = MainPage.DisplayAlert(caption, text, "OK", "Cancel");
            ans.Wait();
            if(ans.Result)
                return CustomMessageBox.DialogResult.OK;

            return CustomMessageBox.DialogResult.Cancel;
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

                Forms.Device.BeginInvokeOnMainThread(() =>
                {
                   
                });

                Task.Run(() =>
                {
                    while (true)
                    {
                        try
                        {
                            while (mav.BaseStream.BytesToRead < 10 || mav.giveComport == true)
                                Thread.Sleep(20);

                            var packet = mav.readPacket();

                            mav.MAV.cs.UpdateCurrentSettings(null);
                        }
                        catch (Exception ex)
                        {
                            Log.Warning("", ex.ToString());
                            Thread.Sleep(10);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Warning("", ex.ToString());
            }
        }
    }
}
