using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.test;
using px4uploader;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Acr.UserDialogs;

namespace Xamarin.GCSViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Firmware : ContentPage, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Firmware()
        {
            InitializeComponent();

            quad.CommandParameter = APFirmware.MAV_TYPE.Copter;
            rover.CommandParameter = APFirmware.MAV_TYPE.GROUND_ROVER;
            plane.CommandParameter = APFirmware.MAV_TYPE.FIXED_WING;
            sub.CommandParameter = APFirmware.MAV_TYPE.SUBMARINE;
            heli.CommandParameter = APFirmware.MAV_TYPE.HELICOPTER;
            antennatracker.CommandParameter = APFirmware.MAV_TYPE.ANTENNA_TRACKER;

            quad.ImageSource = ImageSource.FromStream(()=>Xamarin.Properties.Resources.FW_icons_2013_logos_04.ToMemoryStream());
            rover.ImageSource = ImageSource.FromStream(()=>Xamarin.Properties.Resources.rover_11.ToMemoryStream());
            plane.ImageSource = ImageSource.FromStream(() => Xamarin.Properties.Resources.APM_airframes_001.ToMemoryStream());
            sub.ImageSource = ImageSource.FromStream(() => Xamarin.Properties.Resources.sub.ToMemoryStream());
            heli.ImageSource = ImageSource.FromStream(() => Xamarin.Properties.Resources.APM_airframes_08.ToMemoryStream());
            antennatracker.ImageSource = ImageSource.FromStream(() => Xamarin.Properties.Resources.Antenna_Tracker_01.ToMemoryStream());

            Task.Run(() =>
            {
                APFirmware.GetList();

                var options = APFirmware.GetRelease(APFirmware.RELEASE_TYPES.OFFICIAL);

                // get max version for each mavtype
                options = options.GroupBy(b => b.MavType).Select(a =>
                    a.Where(b => a.Key == b.MavType && b.MavFirmwareVersion == a.Max(c => c.MavFirmwareVersion))
                        .FirstOrDefault()).ToList();

                UpdateIconName(antennatracker,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.ANTENNA_TRACKER.ToString()));
                UpdateIconName(heli,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.HELICOPTER.ToString()));
                UpdateIconName(sub,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.SUBMARINE.ToString()));
                UpdateIconName(rover,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.GROUND_ROVER.ToString()));
                UpdateIconName(quad,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.Copter.ToString()));
                UpdateIconName(plane,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.FIXED_WING.ToString()));

                SetLoading(false);
            });
        }

        void SetLoading(bool isloading)
        {
            Forms.Device.BeginInvokeOnMainThread(() => { ActivityIndicator.IsVisible = isloading; });
        }

        private void UpdateIconName(ImageCell imageLabel, APFirmware.FirmwareInfo first)
        {
            if (first == null)
                return;

            Forms.Device.BeginInvokeOnMainThread(()=>{
                imageLabel.Text = first.VehicleType?.ToString() + " " + first.MavFirmwareVersion.ToString() + " " +
                              first.MavFirmwareVersionType.ToString();
            });
        }

        private async void Image_OnTapped(object sender, EventArgs e)
        {
            //string output = "this is the prefilled";
            //output = await InputBox.InputBox1("header", "enter some data", this.Navigation);

            //output = await InputBox.Show("header", "enter some data");

            var accept = await DisplayAlert(Strings.AreYouSureYouWantToUpload,
                Strings.AreYouSureYouWantToUpload + (sender as ImageCell)?.Text + Strings.QuestionMark, Strings.OK,
                Strings.Cancel).ConfigureAwait(false);
            if (accept)
            {
                var mavtype = ((APFirmware.MAV_TYPE)(sender as ImageCell).CommandParameter);

                await LookForPort(mavtype).ConfigureAwait(false);
            }
        }
        APFirmware.RELEASE_TYPES REL_Type = APFirmware.RELEASE_TYPES.OFFICIAL;
        private string _message;
        private DeviceInfo detectedport;
        private long? detectedboardid;
        private async Task LookForPort(APFirmware.MAV_TYPE mavtype)
        {
            var ports = await Test.UsbDevices.GetDeviceInfoList().ConfigureAwait(false);

            foreach (var deviceInfo in ports)
            {
                long? devid = detectedboardid;

                // make best guess at board_id based on usb info
                if (!devid.HasValue)
                    devid = APFirmware.GetBoardID(deviceInfo);

                if (devid.HasValue && devid.Value != 0)
                {
                    log.InfoFormat("{0}: {1} - {2}", deviceInfo.name, deviceInfo.description, deviceInfo.board);

                    var baseurl = "";

                    // get the options for this device
                    var fwitems = APFirmware.Manifest.Firmware.Where(a =>
                        a.BoardId == devid && a.MavType == mavtype.ToString() &&
                        a.MavFirmwareVersionType == REL_Type.ToString()).ToList();

                    if (fwitems?.Count == 1)
                    {
                        baseurl = fwitems[0].Url.ToString();

                        await DownloadAndUpload(baseurl).ConfigureAwait(false);
                    }
                    else if (fwitems?.Count > 0)
                    {
                        FirmwareSelection fws = new FirmwareSelection(fwitems, deviceInfo);
                        fws.CloseAction += async () =>
                        {
                            Navigation.PopModalAsync();
                            baseurl = fws.FinalResult;

                            await DownloadAndUpload(baseurl).ConfigureAwait(false);

                            return;
                        };
                        await this.Navigation.PushModalAsync(fws).ConfigureAwait(false);
                        Debug.WriteLine(fws.FinalResult);
                        baseurl = fws.FinalResult;
                        if (fws.FinalResult == null)
                        {
                            // user canceled
                            return;
                        }
                    }
                    else
                    {
                        CustomMessageBox.Show(Strings.No_firmware_available_for_this_board, Strings.ERROR);
                        return;
                    }
                    
                    return;
                }
                else
                {
                    CustomMessageBox.Show("Failed to discover board id. Please reconnect via usb and try again.", Strings.ERROR);
                    return;
                }
            }

            CustomMessageBox.Show("Failed to detect port to upload to", Strings.ERROR);
            return;

        }

        private async Task DownloadAndUpload(string baseurl)
        {
            var tempfile = Path.GetTempFileName();
            try
            {
                // update to use mirror url
                log.Info("Using " + baseurl);

                SetLoading(true);

                var starttime = DateTime.Now;

                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(baseurl);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the request stream.
                Stream dataStream; //= request.GetRequestStream();
                // Get the response (using statement is exception safe)
                using (WebResponse response = request.GetResponse())
                {
                    // Display the status.
                    log.Info(((HttpWebResponse) response).StatusDescription);
                    // Get the stream containing content returned by the server.
                    using (dataStream = response.GetResponseStream())
                    {
                        long bytes = response.ContentLength;
                        long contlen = bytes;

                        byte[] buf1 = new byte[1024];

                        using (FileStream fs = new FileStream(tempfile, FileMode.Create))
                        {
                            //fw_Progress1(0, Strings.DownloadingFromInternet);

                            long length = response.ContentLength;
                            long progress = 0;
                            dataStream.ReadTimeout = 30000;

                            while (dataStream.CanRead)
                            {
                                try
                                {
                                    //fw_Progress1(length == 0 ? 50 : (int)((progress * 100) / length), Strings.DownloadingFromInternet);
                                }
                                catch
                                {
                                }

                                int len = dataStream.Read(buf1, 0, 1024);
                                if (len == 0)
                                    break;
                                progress += len;
                                bytes -= len;
                                fs.Write(buf1, 0, len);
                            }

                            fs.Close();
                        }

                        dataStream.Close();
                    }

                    response.Close();
                }

                var timetook = (DateTime.Now - starttime).TotalMilliseconds;

                //Tracking.AddTiming("Firmware Download", deviceInfo.board, timetook, deviceInfo.description);

                //fw_Progress1(100, Strings.DownloadedFromInternet);
                log.Info("Downloaded");
            }
            catch
            {
                CustomMessageBox.Show(Strings.FailedDownload, Strings.ERROR);
                return;
            }
            finally
            {
                SetLoading(false);
            }

            //MissionPlanner.Utilities.Tracking.AddFW(mavtype.ToString(), deviceInfo.board);

            //var fw = new Firmware();
            //fw.Progress += fw_Progress1;

            var uploadstarttime = DateTime.Now;

            await UploadPX4(tempfile).ConfigureAwait(false);
            //fw.UploadFlash(deviceInfo.name, tempfile, BoardDetect.boards.pass);

            var uploadtime = (DateTime.Now - uploadstarttime).TotalMilliseconds;

            //Tracking.AddTiming("Firmware Upload", deviceInfo.board, uploadtime, deviceInfo.description);

            return;
        }

        public event ProgressEventHandler Progress;

        void updateProgress(int percent, string status)
        {
            log.Info(status);

            UserDialogs.Instance.Toast(status, TimeSpan.FromSeconds(3));

            if (Progress != null)
            {
                Progress(percent, status);
            }
        }
        /// <summary>
        /// upload to px4 standalone
        /// </summary>
        /// <param name="filename"></param>
        public async Task<bool> UploadPX4(string filename)
        {
            Uploader up;
            updateProgress(-1, "Reading Hex File");
            px4uploader.Firmware fw;
            try
            {
                fw = px4uploader.Firmware.ProcessFirmware(filename);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorFirmwareFile + "\n\n" + ex.ToString(), Strings.ERROR);
                return false;
            }

            SetLoading(true);

            await AttemptRebootToBootloader().ConfigureAwait(false);

            DateTime DEADLINE = DateTime.Now.AddSeconds(30);

            updateProgress(-1, "Scanning comports");

            while (DateTime.Now < DEADLINE)
            {
                //string[] allports = SerialPort.GetPortNames();
                var di = await Test.UsbDevices.GetDeviceInfoList().ConfigureAwait(false);

                foreach (var port in di)
                {
                    log.Info(DateTime.Now.Millisecond + " Trying Port " + port);

                    var portUsb = await Test.UsbDevices.GetUSB(port).ConfigureAwait(false);

                    if(portUsb == null)
                        continue;

                    try
                    {
                        portUsb.BaudRate = 115200;
                        up = new Uploader(portUsb);
                    }
                    catch (Exception ex)
                    {
                        //System.Threading.Thread.Sleep(50);
                        Console.WriteLine(ex.Message);
                        continue;
                    }

                    try
                    {
                        up.identify();
                        updateProgress(-1, "Identify");
                        log.InfoFormat("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type,
                            up.board_rev, up.bl_rev, up.fw_maxsize, port);

                        up.ProgressEvent += new Uploader.ProgressEventHandler(up_ProgressEvent);
                        up.LogEvent += new Uploader.LogEventHandler(up_LogEvent);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not There..");
                        //Console.WriteLine(ex.Message);
                        up.close();
                        continue;
                    }

                    updateProgress(-1, "Connecting");

                    // test if pausing here stops - System.TimeoutException: The write timed out.
                    System.Threading.Thread.Sleep(500);

                    try
                    {
                        up.currentChecksum(fw);
                    }
                    catch (IOException ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("lost communication with the board.", "lost comms");
                        SetLoading(false);
                        up.close();
                        return false;
                    }
                    catch
                    {
                        up.__reboot();
                        up.close();
                        CustomMessageBox.Show(Strings.NoNeedToUpload);
                        SetLoading(false);
                        return true;
                    }

                    try
                    {
                        updateProgress(0, "Upload");
                        up.upload(fw);
                        updateProgress(100, "Upload Done");
                    }
                    catch (Exception ex)
                    {
                        updateProgress(0, "ERROR: " + ex.Message);
                        log.Info(ex);
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                    finally
                    {
                        up.close();
                        SetLoading(false);
                    }

                    // wait for IO firmware upgrade and boot to a mavlink state
                    CustomMessageBox.Show(Strings.PleaseWaitForTheMusicalTones);

                    return true;
                }
            }

            updateProgress(0, "ERROR: No Response from board");
            SetLoading(false);
            return false;
        }

        private async Task AttemptRebootToBootloader()
        {
            updateProgress(0, "AttemptRebootToBootloader");

            List<Task<bool>> tasklist = new List<Task<bool>>();

            //string[] allports = SerialPort.GetPortNames();
            var di = await Test.UsbDevices.GetDeviceInfoList().ConfigureAwait(false);

            foreach (var port in di)
            {
                log.Info(DateTime.Now.Millisecond + " Trying Port " + port);

                var portUsb = await Test.UsbDevices.GetUSB(port).ConfigureAwait(false);

                if (portUsb == null)
                    continue;

                log.Info(DateTime.Now.Millisecond + " Trying Port " + port);
                try
                {
                    var task = Task.Run(() =>
                    {
                        portUsb.BaudRate = 115200;
                        using (var up = new Uploader(portUsb))
                        {
                            up.identify();
                            return true;
                        }

                        return false;
                    });

                    tasklist.Add(task);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            foreach (var task in tasklist)
            {
                try
                {
                    if (task.Wait(TimeSpan.FromSeconds(3)) && await task.ConfigureAwait(false) == true)
                        return;
                    else
                    {
                        //not there
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            //if (MainV2.comPort.BaseStream is SerialPort)
            {
                try
                {
                    updateProgress(-1, "Look for HeartBeat");

                    MainV2.comPort.BaseStream =
                        await Test.UsbDevices.GetUSB((await Test.UsbDevices.GetDeviceInfoList().ConfigureAwait(false)).First()).ConfigureAwait(false);

                    var task = Task.Run(() =>
                    {
                        // check if we are seeing heartbeats
                        MainV2.comPort.BaseStream.Open();
                        MainV2.comPort.giveComport = true;

                        if (MainV2.comPort.getHeartBeat().Length > 0)
                        {
                            MainV2.comPort.doReboot(true, false);
                            MainV2.comPort.Close();
                        }
                        else
                        {
                            MainV2.comPort.BaseStream.Close();
                            throw new Exception("No HeartBeat found");
                        }
                    });
                    if (task.Wait(TimeSpan.FromSeconds(3)))
                    {
                        updateProgress(-1, "Reboot to Bootloader");
                    }
                    else
                    {
                        CustomMessageBox.Show(Strings.PleaseUnplugTheBoardAnd);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    CustomMessageBox.Show(Strings.PleaseUnplugTheBoardAnd);
                    return;
                }
            }
        }

        public void DeviceAttached(object USBDevice, object usbdevice)
        {
            Task.Run(async () =>
            {
                Parallel.ForEach(await Test.UsbDevices.GetDeviceInfoList(), async (port)=>
                {
                    var portUsb = await Test.UsbDevices.GetUSB(port).ConfigureAwait(false);

                    if (portUsb == null)
                        return;

                    px4uploader.Uploader up;

                    try
                    {
                        portUsb.BaudRate = 115200;
                        up = new px4uploader.Uploader(portUsb);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }

                    try
                    {
                        up.identify();
                        var msg = String.Format("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}",
                            up.board_type,
                            up.board_rev, up.bl_rev, up.fw_maxsize, port);
                        log.Info(msg);

                        up.close();

                        //this.InvokeIfRequired(() => lbl_status.Text = msg);
                        detectedport = port;
                        detectedboardid = up.board_type;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not There..");
                        up.close();
                    }
                });
            });
        }

        void up_LogEvent(string message, int level = 0)
        {
            log.Debug(message);

            _message = message;
            updateProgress(-1, message);
        }

        void up_ProgressEvent(double completed)
        {
            updateProgress((int)completed, _message);
        }

        public void Activate()
        {
            Test.UsbDevices.USBEvent += DeviceAttached;
        }

        public void Deactivate()
        {
            Test.UsbDevices.USBEvent -= DeviceAttached;
        }
    }
}