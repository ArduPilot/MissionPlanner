using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.test;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFirmwareManifest : UserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        APFirmware.RELEASE_TYPES REL_Type = APFirmware.RELEASE_TYPES.OFFICIAL;
        private string detectedport { get; set; } = "";
        private long? detectedboardid;

        public ConfigFirmwareManifest()
        {
            InitializeComponent();

            pictureAntennaTracker.Tag = APFirmware.MAV_TYPE.ANTENNA_TRACKER;
            pictureBoxHeli.Tag = APFirmware.MAV_TYPE.HELICOPTER;
            pictureBoxSub.Tag = APFirmware.MAV_TYPE.SUBMARINE;
            pictureBoxRover.Tag = APFirmware.MAV_TYPE.GROUND_ROVER;
            pictureBoxQuad.Tag = APFirmware.MAV_TYPE.Copter;
            pictureBoxPlane.Tag = APFirmware.MAV_TYPE.FIXED_WING;
        }

        public void Activate()
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;
            MainV2.instance.DeviceChanged += Instance_DeviceChanged;

            // Until we connect to the internet, disable all controls that require it
            // Disable all ImageLabels
            foreach (Control c in this.Controls)
            {
                if (c is ImageLabel)
                {
                    c.Enabled = false;
                }
            }
            // Disable "All Options" and "Beta Firmwares"
            lbl_alloptions.Enabled = false;
            lbl_devfw.Enabled = false;

            flashdone = false;

            Task.Run(() =>
            {
                APFirmware.GetList("https://firmware.oborne.me/manifest.json.gz");

                var options = APFirmware.GetRelease(REL_Type);

                // get max version for each mavtype
                options = options.GroupBy(b => b.MavType).Select(a =>
                    a.Where(b => a.Key == b.MavType && b.MavFirmwareVersion == a.Max(c => c.MavFirmwareVersion))
                        .FirstOrDefault()).ToList();

                UpdateIconName(pictureAntennaTracker,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.ANTENNA_TRACKER.ToString()));
                UpdateIconName(pictureBoxHeli,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.HELICOPTER.ToString()));
                UpdateIconName(pictureBoxSub,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.SUBMARINE.ToString()));
                UpdateIconName(pictureBoxRover,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.GROUND_ROVER.ToString()));
                UpdateIconName(pictureBoxQuad, options.First(a => a.MavType == APFirmware.MAV_TYPE.Copter.ToString()));
                UpdateIconName(pictureBoxPlane,
                    options.First(a => a.MavType == APFirmware.MAV_TYPE.FIXED_WING.ToString()));
            });
        }

        private void UpdateIconName(ImageLabel imageLabel, APFirmware.FirmwareInfo first)
        {
            if (first == null)
                return;

            if (this.Disposing)
                return;
            if (this.IsDisposed)
                return;

            this.BeginInvoke((MethodInvoker)delegate
           {
               if (String.IsNullOrEmpty(first.MavFirmwareVersionStr))
                   imageLabel.Text = first.VehicleType?.ToString() + " " + first.MavFirmwareVersion.ToString() + " " +
                                     first.MavFirmwareVersionType.ToString();
               else
                   imageLabel.Text = first.VehicleType?.ToString() + " " + first.MavFirmwareVersionStr + " " +
                                     first.MavFirmwareVersionType.ToString();

               // Re-enable this ImageLabel
               imageLabel.Enabled = true;
               // Re-enable the "All Options" and "Beta Firmwares" controls
               // (only needs to be done once, but easier to do here)
               lbl_alloptions.Enabled = true;
               lbl_devfw.Enabled = true;
           });
        }

        public void Deactivate()
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;

            // reset to official on any reload
            REL_Type = APFirmware.RELEASE_TYPES.OFFICIAL;

            flashdone = false;
        }

        private void Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum cause)
        {
            if (cause != MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL)
                return;

            if (flashdone == true)
                return;

            Task.Run(() =>
            {
                Parallel.ForEach(SerialPort.GetPortNames(), port =>
                {
                    px4uploader.Uploader up;

                    try
                    {
                        // time to appear
                        Thread.Sleep(20);
                        up = new px4uploader.Uploader(port, 115200);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return;
                    }

                    try
                    {
                        up.identify();
                        var msg = String.Format("Found board type {0} brdrev {1} blrev {2} fwmax {3} chip {5:X} chipdes {6} on {4}", up.board_type,
                        up.board_rev, up.bl_rev, up.fw_maxsize, port, up.chip, up.chip_desc);
                        log.Info(msg);

                        up.close();

                        this.InvokeIfRequired(() => lbl_status.Text = msg);
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

        private void PictureBox_Click(object sender, EventArgs e)
        {
            var mavtype = ((APFirmware.MAV_TYPE)(sender as ImageLabel).Tag);
            var dr = CustomMessageBox.Show(Strings.AreYouSureYouWantToUpload + (sender as ImageLabel)?.Text + Strings.QuestionMark,
                Strings.Continue, MessageBoxButtons.YesNo);
            if (dr == (int)DialogResult.Yes)
            {
                LookForPort(mavtype);
            }
        }

        private void LookForPort(APFirmware.MAV_TYPE mavtype, bool alloptions = false)
        {
            var ports = Win32DeviceMgmt.GetAllCOMPorts();
            ports.AddRange(Linux.GetAllCOMPorts());

            if (ExtraDeviceInfo != null)
            {
                try
                {
                    ports.AddRange(ExtraDeviceInfo.Invoke());
                }
                catch
                {

                }
            }

            if (alloptions)
                ports.Add(default(ArduPilot.DeviceInfo));

            foreach (var deviceInfo in ports)
            {
                long? devid = detectedboardid;
                long[] devids = null;

                // make best guess at board_id based on usb info
                if (!devid.HasValue)
                    devids = APFirmware.GetBoardID(deviceInfo);

                if (devid.HasValue && devid.Value != 0 || alloptions == true || devids != null)
                {
                    log.InfoFormat("{0}: {1} - {2}", deviceInfo.name, deviceInfo.description, deviceInfo.board);

                    if (devids == null && devid.HasValue)
                        devids = new long[] {devid.Value};
                    else if (devids == null)
                        devids = new long[]{};

                    var baseurl = "";

                    // get the options for this device
                    var fwitems = APFirmware.Manifest.Firmware.Where(a =>
                        devids.Any(devidlocal => devidlocal == a.BoardId) && a.MavType == mavtype.ToString() &&
                        a.MavFirmwareVersionType == REL_Type.ToString()).ToList();

                    if (alloptions)
                        fwitems = APFirmware.Manifest.Firmware.ToList();

                    if (fwitems?.Count == 1)
                    {
                        baseurl = fwitems[0].Url.ToString();
                    }
                    else if (fwitems?.Count > 0)
                    {
                        FirmwareSelection fws = new FirmwareSelection(fwitems, deviceInfo);
                        fws.ShowXamarinControl(550, 400);
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
                        var client = new HttpClient();
                        client.DefaultRequestHeaders.Add("User-Agent", Settings.Instance.UserAgent);
                        client.Timeout = TimeSpan.FromSeconds(30);
                        using (var response = client.GetAsync(baseurl))
                        {
                            // Display the status.
                            log.Info(baseurl + " " + response.Result.ReasonPhrase);
                            // Get the stream containing content returned by the server.
                            using (var dataStream = response.Result.Content.ReadAsStreamAsync().Result)
                            {
                                long bytes = int.Parse(response.Result.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
                                long contlen = bytes;

                                byte[] buf1 = new byte[1024];

                                using (FileStream fs = new FileStream(tempfile, FileMode.Create))
                                {
                                    fw_Progress1(0, Strings.DownloadingFromInternet);

                                    long length = int.Parse(response.Result.Content.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
                                    long progress = 0;

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

                    flashdone = true;

                    fw.UploadFlash(deviceInfo.name, tempfile, BoardDetect.boards.pass);
                    
                    var uploadtime = (DateTime.Now - uploadstarttime).TotalMilliseconds;

                    Tracking.AddTiming("Firmware Upload", deviceInfo.board, uploadtime, deviceInfo.description);

                    return;

                }
                /*
                // allow scanning multiple ports
                else
                {
                    CustomMessageBox.Show("Failed to discover board id. Please reconnect via usb and try again.", Strings.ERROR);
                    return;
                }
                */
            }

            CustomMessageBox.Show("Failed to detect port to upload to (Unknown VID/PID or Board String)\r\nPlease try Disconnect/Reconnect and upload while on this screen", Strings.ERROR);
            return;
        }

        public static Func<List<ArduPilot.DeviceInfo>> ExtraDeviceInfo;

        /// <summary>
        ///     for when updating fw to hardware
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        private void fw_Progress1(int progress, string status)
        {
            this.BeginInvokeIfRequired(() =>
            {
                var change = false;

                if (progress != -1)
                {
                    if (this.progress.Value != progress)
                    {
                        this.progress.Value = progress;
                        change = true;
                    }
                }

                if (lbl_status.Text != status)
                {
                    lbl_status.Text = status;
                    change = true;
                }

                if (change)
                    this.Refresh();
            });
        }

        private void Lbl_devfw_Click(object sender, EventArgs e)
        {
            REL_Type = APFirmware.RELEASE_TYPES.BETA;
            Activate();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Q))
            {
                CustomMessageBox.Show(Strings.TrunkWarning, Strings.Trunk);
                REL_Type = APFirmware.RELEASE_TYPES.DEV;
                Activate();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string custom_fw_dir = Settings.Instance["FirmwareFileDirectory"] ?? "";
        private bool flashdone = false;

        private void Lbl_Custom_firmware_label_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog
                {Filter = "Firmware (*.hex;*.px4;*.vrx;*.apj)|*.hex;*.px4;*.vrx;*.apj|DFU|*.hex;*.bin;*.dfu|All files (*.*)|*.*"})
            {
                if (Directory.Exists(custom_fw_dir))
                    fd.InitialDirectory = custom_fw_dir;
                fd.ShowDialog();
                if (File.Exists(fd.FileName))
                {
                    custom_fw_dir = Path.GetDirectoryName(fd.FileName);
                    Settings.Instance["FirmwareFileDirectory"] = custom_fw_dir;

                    var fw = new Firmware();

                    fw.Progress += fw_Progress1;

                    var boardtype = BoardDetect.boards.none;
                    try
                    {
                        if (fd.FileName.ToLower().EndsWith(".dfu"))
                        {
                            DFU.Progress = fw_Progress1;
                            DFU.Flash(fd.FileName);
                            return;
                        }
                        else if (fd.FileName.ToLower().EndsWith(".hex"))
                        {
                            DFU.Progress = fw_Progress1;
                            if (CustomMessageBox.Show("Do you want to upload this via DFU?", "", CustomMessageBox.MessageBoxButtons.OKCancel) == CustomMessageBox.DialogResult.OK)
                            {
                                DFU.Flash(fd.FileName);
                                return;
                            }
                        }
                        else if(fd.FileName.ToLower().EndsWith(".bin"))
                        {
                            DFU.Progress = fw_Progress1;
                            if (CustomMessageBox.Show("Flashing image to 0x08000000", "", CustomMessageBox.MessageBoxButtons.OKCancel) == CustomMessageBox.DialogResult.OK)
                                DFU.Flash(fd.FileName, 0x08000000);
                            return;
                        }
                        else if (fd.FileName.ToLower().EndsWith(".px4") || fd.FileName.ToLower().EndsWith(".apj"))
                        {
                            if (solo.Solo.is_solo_alive &&
                                CustomMessageBox.Show("Solo", "Is this a Solo?",
                                    CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
                            {
                                boardtype = BoardDetect.boards.solo;
                            }
                            else
                            {
                                boardtype = BoardDetect.boards.px4v2;
                            }
                        }
                        else
                        {
                            var ports = Win32DeviceMgmt.GetAllCOMPorts();
                            ports.AddRange(Linux.GetAllCOMPorts());

                            if (ExtraDeviceInfo != null)
                            {
                                try
                                {
                                    ports.AddRange(ExtraDeviceInfo.Invoke());
                                }
                                catch
                                {

                                }
                            }

                            boardtype = BoardDetect.DetectBoard(MainV2.comPortName, ports);
                        }

                        if (boardtype == BoardDetect.boards.none)
                        {
                            CustomMessageBox.Show(Strings.CantDetectBoardVersion);
                            return;
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.CanNotConnectToComPortAnd, Strings.ERROR);
                        return;
                    }

                    try
                    {
                        fw.UploadFlash(MainV2.comPortName, fd.FileName, boardtype);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.ToString(), Strings.ERROR);
                    }
                }
            }

        }

        private void Lbl_px4bl_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.Open(false);

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    MainV2.comPort.doReboot(true, false);
                    CustomMessageBox.Show("Please ignore the unplug and plug back in when uploading flight firmware.");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                CustomMessageBox.Show("Failed to connect and send the reboot command", Strings.ERROR);
            }
        }

        private void Lbl_bootloaderupdate_Click(object sender, EventArgs e)
        {
            // connect to mavlink
            var mav = new MAVLinkInterface();
            MainV2.instance.doConnect(mav, MainV2._connectionControl.CMB_serialport.Text,
                MainV2._connectionControl.CMB_baudrate.Text, false);

            if (mav.BaseStream == null || !mav.BaseStream.IsOpen)
            {
                CustomMessageBox.Show("Failed to find device on mavlink");
                return;
            }

            if (CustomMessageBox.Show("Are you sure you want to upgrade the bootloader? This can brick your board",
                    "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int)DialogResult.Yes)
                if (CustomMessageBox.Show("Are you sure you want to upgrade the bootloader? This can brick your board, Please allow 5 mins for this process",
                        "BL Update", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == (int)DialogResult.Yes)
                    if (mav.doCommand(MAVLink.MAV_CMD.FLASH_BOOTLOADER, 0, 0, 0, 0, 290876, 0, 0))
                    {
                        CustomMessageBox.Show("Upgraded bootloader");
                    }
                    else
                    {
                        CustomMessageBox.Show("Failed to upgrade bootloader");
                    }

            mav?.Close();
        }

        private void lbl_alloptions_Click(object sender, EventArgs e)
        {
            LookForPort(APFirmware.MAV_TYPE.Copter, true);
        }
    }
}
