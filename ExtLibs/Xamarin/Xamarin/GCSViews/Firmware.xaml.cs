using MissionPlanner;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.GCSViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Firmware : ContentPage
    {
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
        }

        private void UpdateIconName(ImageCell imageLabel, APFirmware.FirmwareInfo first)
        {
            if (first == null)
                return;


            imageLabel.Text = first.VehicleType?.ToString() + " " + first.MavFirmwareVersion.ToString() + " " +
                              first.MavFirmwareVersionType.ToString();
        }

        private async void Image_OnTapped(object sender, EventArgs e)
        {
            var accept = await DisplayAlert(Strings.AreYouSureYouWantToUpload,
                Strings.AreYouSureYouWantToUpload + (sender as ImageCell)?.Text + Strings.QuestionMark, Strings.OK,
                Strings.Cancel);
            if (accept)
            {
                var mavtype = ((APFirmware.MAV_TYPE)(sender as ImageCell).CommandParameter);

                LookForPort(mavtype);
            }
        }


        private void LookForPort(APFirmware.MAV_TYPE mavtype)
        {

            /*
            var ports = Win32DeviceMgmt.GetAllCOMPorts();

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
                    }
                    else if (fwitems?.Count > 0)
                    {
                        FirmwareSelection fws = new FirmwareSelection(fwitems, deviceInfo);
                        fws.ShowXamarinControl(400, 400);
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
                    }

                    var tempfile = Path.GetTempFileName();
                    try
                    {
                        // update to use mirror url
                        log.Info("Using " + baseurl);

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
                            log.Info(((HttpWebResponse)response).StatusDescription);
                            // Get the stream containing content returned by the server.
                            using (dataStream = response.GetResponseStream())
                            {
                                long bytes = response.ContentLength;
                                long contlen = bytes;

                                byte[] buf1 = new byte[1024];

                                using (FileStream fs = new FileStream(tempfile, FileMode.Create))
                                {
                                    fw_Progress1(0, Strings.DownloadingFromInternet);

                                    long length = response.ContentLength;
                                    long progress = 0;
                                    dataStream.ReadTimeout = 30000;

                                    while (dataStream.CanRead)
                                    {
                                        try
                                        {
                                            fw_Progress1(length == 0 ? 50 : (int)((progress * 100) / length), Strings.DownloadingFromInternet);
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

                        Tracking.AddTiming("Firmware Download", deviceInfo.board, timetook, deviceInfo.description);

                        fw_Progress1(100, Strings.DownloadedFromInternet);
                        log.Info("Downloaded");
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.FailedDownload, Strings.ERROR);
                        return;
                    }

                    MissionPlanner.Utilities.Tracking.AddFW(mavtype.ToString(), deviceInfo.board);

                    var fw = new Firmware();
                    fw.Progress += fw_Progress1;

                    var uploadstarttime = DateTime.Now;

                    fw.UploadFlash(deviceInfo.name, tempfile, BoardDetect.boards.pass);

                    var uploadtime = (DateTime.Now - uploadstarttime).TotalMilliseconds;

                    Tracking.AddTiming("Firmware Upload", deviceInfo.board, uploadtime, deviceInfo.description);

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
            */
        }
    }
}