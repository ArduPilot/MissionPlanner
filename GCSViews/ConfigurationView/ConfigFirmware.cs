using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static List<Firmware.software> softwares = new List<Firmware.software>();
        private readonly Firmware fw = new Firmware();
        private string custom_fw_dir = "";
        private string firmwareurl = "";
        private bool firstrun = true;
        private IProgressReporterDialogue pdr;

        public ConfigFirmware()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (firstrun)
            {
                UpdateFWList();
                firstrun = false;
                MainV2.instance.DeviceChanged += Instance_DeviceChanged;
            }

            if (Program.WindowsStoreApp)
            {

                CustomMessageBox.Show("Not Available", "Unfortunately the windows store version of this app does not support uploading.", MessageBoxButtons.OK);
                this.Enabled = false;
                return;
            }

            if (MainV2.DisplayConfiguration.isAdvancedMode)
            {
                lbl_devfw.Visible = true;
                lbl_Custom_firmware_label.Visible = true;
                lbl_dlfw.Visible = true;
                CMB_history_label.Visible = true;
            }
            else
            {
                lbl_devfw.Visible = false;
                lbl_Custom_firmware_label.Visible = false;
                lbl_dlfw.Visible = false;
                CMB_history_label.Visible = false;
            }
        }

        private void Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum cause)
        {
            if (cause != MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL)
                return;

            Parallel.ForEach(SerialPort.GetPortNames(), port =>
            //Task.Run(delegate
            {
                px4uploader.Uploader up;

                try
                {
                    up = new px4uploader.Uploader(port, 115200);
                }
                catch (Exception ex)
                {
                    //System.Threading.Thread.Sleep(50);
                    Console.WriteLine(ex.Message);
                    return;
                }

                try
                {
                    up.identify();
                    log.InfoFormat("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type,
                        up.board_rev, up.bl_rev, up.fw_maxsize, port);

                    up.close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Not There..");
                    //Console.WriteLine(ex.Message);
                    up.close();
                }
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //CTRL+R moved to pictureBoxRover_Click
            //CTRL+O moved to CMB_history_label_Click
            //CTRL+C moved to Custom_firmware_label_Click

            if (keyData == (Keys.Control | Keys.Q))
            {
                CustomMessageBox.Show(Strings.TrunkWarning, Strings.Trunk);
                firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmwarelatest.xml";
                softwares.Clear();
                UpdateFWList();
                CMB_history.Visible = false;
            }
            else if (keyData == (Keys.Control | Keys.P))
            {
                findfirmware(softwares.First(a => { return a.name.ToLower().Contains("px4"); }));
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void UpdateFWList()
        {
            pdr = new ProgressReporterDialogue();

            pdr.DoWork -= pdr_DoWork;

            pdr.DoWork += pdr_DoWork;

            ThemeManager.ApplyThemeTo(pdr);

            pdr.RunBackgroundOperationAsync();

            pdr.Dispose();
        }

        private void pdr_DoWork(IProgressReporterDialogue sender)
        {
            var fw = new Firmware();
            fw.Progress -= fw_Progress1;
            fw.Progress += fw_Progress;
            softwares = fw.getFWList(firmwareurl);

            foreach (var soft in softwares)
            {
                updateDisplayNameInvoke(soft);
            }
        }

        /// <summary>
        ///     for updating fw list
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        private void fw_Progress(int progress, string status)
        {
            pdr.UpdateProgressAndStatus(progress, status);
        }

        /// <summary>
        ///     for when updating fw to hardware
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        private void fw_Progress1(int progress, string status)
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
                Refresh();
        }

        private void updateDisplayNameInvoke(Firmware.software temp)
        {
            Invoke((MethodInvoker) delegate { updateDisplayName(temp); });
        }

        private void updateDisplayName(Firmware.software temp)
        {
            if (temp.url2560.ToLower().Contains("AR2".ToLower()) ||
                temp.url2560.ToLower().Contains("apm1/APMRover".ToLower()))
            {
                pictureBoxRover.Text = temp.name;
                pictureBoxRover.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("AP-".ToLower()) ||
                     temp.url2560.ToLower().Contains("apm1/ArduPlane".ToLower()))
            {
                pictureBoxAPM.Text = temp.name;
                pictureBoxAPM.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-quad-".ToLower()) ||
                     temp.url2560.ToLower().Contains("1-quad/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter quad") ||
                     temp.desc.ToLower().Contains("arducopter quad")
                )
            {
                pictureBoxQuad.Text = temp.name += " Quad";
                pictureBoxQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-tri".ToLower()) ||
                     temp.url2560.ToLower().Contains("-tri/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter tri") ||
                     temp.desc.ToLower().Contains("arducopter tri"))
            {
                pictureBoxTri.Text = temp.name += " Tri";
                pictureBoxTri.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-hexa".ToLower()) ||
                     temp.url2560.ToLower().Contains("-hexa/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter hexa") ||
                     temp.desc.ToLower().Contains("arducopter hexa"))
            {
                pictureBoxHexa.Text = temp.name += " Hexa";
                pictureBoxHexa.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-y6".ToLower()) ||
                     temp.url2560.ToLower().Contains("-y6/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter y6") ||
                     temp.desc.ToLower().Contains("arducopter y6"))
            {
                pictureBoxY6.Text = temp.name += " Y6";
                pictureBoxY6.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-heli-".ToLower()) ||
                     temp.url2560.ToLower().Contains("-heli/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter heli") ||
                     temp.desc.ToLower().Contains("arducopter heli"))
            {
                pictureBoxHeli.Text = temp.name += " heli";
                pictureBoxHeli.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octaquad-".ToLower()) ||
                     temp.url2560.ToLower().Contains("-octa-quad/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter octa quad") ||
                     temp.desc.ToLower().Contains("arducopter octa quad"))
            {
                pictureBoxOctaQuad.Text = temp.name += " Octa Quad";
                pictureBoxOctaQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octa-".ToLower()) ||
                     temp.url2560.ToLower().Contains("-octa/ArduCopter".ToLower()) ||
                     temp.name.ToLower().Contains("arducopter octa") ||
                     temp.desc.ToLower().Contains("arducopter octa"))
            {
                pictureBoxOcta.Text = temp.name += " Octa";
                pictureBoxOcta.Tag = temp;
            }
            else if (temp.url2560_2.ToLower().Contains("antennatracker"))
            {
                pictureAntennaTracker.Text = temp.name;
                pictureAntennaTracker.Tag = temp;
            }
            else if (temp.urlpx4v2.ToLower().Contains("ardusub"))
            {
                pictureBoxSub.Text = temp.name;
                pictureBoxSub.Tag = temp;
            }
            else
            {
                log.Info("No Home " + temp.name + " " + temp.url2560);
            }
        }

        private void findfirmware(Firmware.software fwtoupload)
        {
            var dr = CustomMessageBox.Show(Strings.AreYouSureYouWantToUpload + fwtoupload.name + Strings.QuestionMark,
                Strings.Continue, MessageBoxButtons.YesNo);
            if (dr == (int)DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.BaseStream.Close();
                }
                catch
                {
                }
                fw.Progress -= fw_Progress;
                fw.Progress += fw_Progress1;

                var history = (CMB_history.SelectedValue == null) ? "" : CMB_history.SelectedValue.ToString();

                if (history != "")
                {
                    foreach (var propertyInfo in fwtoupload.GetType().GetFields())
                    {
                        try
                        {
                            if (propertyInfo.Name.Contains("url"))
                            {
                                var oldurl = propertyInfo.GetValue(fwtoupload).ToString();
                                if(oldurl == "")
                                    continue;
                                var newurl = Firmware.getUrl(history, oldurl);
                                propertyInfo.SetValue(fwtoupload, newurl);
                            }
                        } catch { }
                    }

                    history = "";
                }

                var updated = fw.update(MainV2.comPortName, fwtoupload, history);

                if (updated)
                {
                    if (fwtoupload.url2560_2 != null && fwtoupload.url2560_2.ToLower().Contains("copter") &&
                        fwtoupload.name.ToLower().Contains("3.1"))
                        CustomMessageBox.Show(Strings.WarningAC31, Strings.Warning);

                    if (fwtoupload.url2560_2 != null && fwtoupload.url2560_2.ToLower().Contains("copter") &&
                        fwtoupload.name.ToLower().Contains("3.2"))
                        CustomMessageBox.Show(Strings.WarningAC32, Strings.Warning);
                }
                else
                {
                    CustomMessageBox.Show(Strings.ErrorUploadingFirmware, Strings.ERROR);
                }
            }
        }

        private void pictureBoxFW_Click(object sender, EventArgs e)
        {
            if (((Control) sender).Tag.GetType() != typeof (Firmware.software))
            {
                CustomMessageBox.Show(Strings.ErrorFirmwareFile, Strings.ERROR);
                return;
            }

            findfirmware((Firmware.software) ((Control) sender).Tag);
        }

        private void up_LogEvent(string message, int level = 0)
        {
            lbl_status.Text = message;
            Application.DoEvents();
        }

        private void up_ProgressEvent(double completed)
        {
            progress.Value = (int) completed;
            Application.DoEvents();
        }

        private void port_Progress(int progress, string status)
        {
            log.InfoFormat("Progress {0} ", progress);
            this.progress.Value = progress;
            this.progress.Refresh();
        }

        private void CMB_history_SelectedIndexChanged(object sender, EventArgs e)
        {
            firmwareurl = Firmware.getUrl(CMB_history.SelectedValue.ToString(), "");

            softwares.Clear();
            UpdateFWList();
        }

        //Show list of previous firmware versions (old CTRL+O shortcut)
        private void CMB_history_label_Click(object sender, EventArgs e)
        {
            CMB_history.Enabled = false;

            //CMB_history.Items.Clear();
            //CMB_history.Items.AddRange(fw.gholdurls);
            //CMB_history.Items.AddRange(fw.gcoldurls);
            CMB_history.DisplayMember = "Value";
            CMB_history.ValueMember = "Key";
            CMB_history.DataSource = Firmware.niceNames;

            CMB_history.Enabled = true;
            CMB_history.Visible = true;
            CMB_history_label.Visible = false;
        }

        //Load custom firmware (old CTRL+C shortcut)
        private void Custom_firmware_label_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog {Filter = "Firmware (*.hex;*.px4;*.vrx;*.apj)|*.hex;*.px4;*.vrx;*.apj|All files (*.*)|*.*" })
            {
                if (Directory.Exists(custom_fw_dir))
                    fd.InitialDirectory = custom_fw_dir;
                fd.ShowDialog();
                if (File.Exists(fd.FileName))
                {
                    custom_fw_dir = Path.GetDirectoryName(fd.FileName);

                    fw.Progress -= fw_Progress;
                    fw.Progress += fw_Progress1;

                    var boardtype = BoardDetect.boards.none;
                    try
                    {
                        if (fd.FileName.ToLower().EndsWith(".px4") || fd.FileName.ToLower().EndsWith(".apj"))
                        {
                            if (solo.Solo.is_solo_alive && 
                                CustomMessageBox.Show("Solo","Is this a Solo?",CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
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
                            boardtype = BoardDetect.DetectBoard(MainV2.comPortName);
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

                    fw.UploadFlash(MainV2.comPortName, fd.FileName, boardtype);
                }
            }
        }

        private void lbl_devfw_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show(Strings.BetaWarning, Strings.Beta);
            firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmware2.xml";
            softwares.Clear();
            UpdateFWList();
            CMB_history.Visible = false;
        }
        

        private void lbl_dlfw_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://firmware.ardupilot.org/");
            }
            catch
            {
                CustomMessageBox.Show("Can not open url http://firmware.ardupilot.org/", Strings.ERROR);
            }
        }

        private void lbl_px4bl_Click(object sender, EventArgs e)
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(@"http://copter.ardupilot.com/wiki/connect-escs-and-motors/#motor_order_diagrams");
            }
            catch
            {
                CustomMessageBox.Show("http://copter.ardupilot.com/wiki/connect-escs-and-motors/#motor_order_diagrams", Strings.ERROR);
            }
        }

        private void picturebox_ph2_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"http://www.proficnc.com/?utm_source=missionplanner&utm_medium=click&utm_campaign=mission");
            }
            catch
            {
                CustomMessageBox.Show("http://www.proficnc.com/?utm_source=missionplanner&utm_medium=click&utm_campaign=mission", Strings.ERROR);
            }
        }

        public void Deactivate()
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;
        }
    }
}