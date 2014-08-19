using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Arduino;
using MissionPlanner.Utilities;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using px4uploader;
using MissionPlanner.Controls;
using System.Collections;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl, IActivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string firmwareurl = "";

        string custom_fw_dir = "";
        
        Utilities.Firmware fw = new Utilities.Firmware();

        bool firstrun = true;

        ProgressReporterDialogue pdr;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //CTRL+R moved to pictureBoxRover_Click
            //CTRL+O moved to CMB_history_label_Click
            //CTRL+C moved to Custom_firmware_label_Click

            if (keyData == (Keys.Control | Keys.Q))
            {
                CustomMessageBox.Show("These are the latest trunk firmware, use at your own risk!!!", "trunk");
                firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmwarelatest.xml";
                softwares.Clear();
                UpdateFWList();
                CMB_history.Visible = false;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        static List<Utilities.Firmware.software> softwares = new List<Utilities.Firmware.software>();
        bool flashing = false;

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
            }

            if (MainV2.Advanced)
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

        void UpdateFWList() 
        {
            pdr = new ProgressReporterDialogue();

            pdr.DoWork -= pdr_DoWork;

            pdr.DoWork += pdr_DoWork;

            ThemeManager.ApplyThemeTo(pdr);

            pdr.RunBackgroundOperationAsync();

        }

        void pdr_DoWork(object sender, Controls.ProgressWorkerEventArgs e, object passdata = null)
        {
            Utilities.Firmware fw = new Utilities.Firmware();
            fw.Progress -= fw_Progress1;
            fw.Progress += fw_Progress;
            softwares = fw.getFWList(firmwareurl);

            foreach (var soft in softwares)
            {
                updateDisplayNameInvoke(soft);
            }
        }

        /// <summary>
        /// for updating fw list
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        void fw_Progress(int progress, string status)
        {
            pdr.UpdateProgressAndStatus(progress, status);
        }

        /// <summary>
        /// for when updating fw to hardware
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="status"></param>
        void fw_Progress1(int progress, string status)
        {
            bool change = false;

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
        }

        void updateDisplayNameInvoke(Utilities.Firmware.software temp)
        {
            this.Invoke((MethodInvoker)delegate
            {
                updateDisplayName(temp);
            });
        }

        void updateDisplayName(Utilities.Firmware.software temp)
        {
            if (temp.url2560.ToLower().Contains("AR2".ToLower()) || temp.url2560.ToLower().Contains("apm1/APMRover".ToLower()))
            {
                pictureBoxRover.Text = temp.name;
                pictureBoxRover.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("AP-".ToLower()) || temp.url2560.ToLower().Contains("apm1/ArduPlane".ToLower()))
            {
                pictureBoxAPM.Text = temp.name;
                pictureBoxAPM.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("APHIL-".ToLower()) || temp.url2560.ToLower().Contains("apm1-hilsensors/ArduPlane".ToLower()))
            {
                pictureBoxAPHil.Text = temp.name;
                pictureBoxAPHil.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-quad-".ToLower()) || temp.url2560.ToLower().Contains("1-quad/ArduCopter".ToLower()))
            {
                pictureBoxQuad.Text = temp.name += " Quad";
                pictureBoxQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-tri".ToLower()) || temp.url2560.ToLower().Contains("-tri/ArduCopter".ToLower()))
            {
                pictureBoxTri.Text = temp.name += " Tri";
                pictureBoxTri.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-hexa".ToLower()) || temp.url2560.ToLower().Contains("-hexa/ArduCopter".ToLower()))
            {
                pictureBoxHexa.Text = temp.name += " Hexa";
                pictureBoxHexa.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-y6".ToLower()) || temp.url2560.ToLower().Contains("-y6/ArduCopter".ToLower()))
            {
                pictureBoxY6.Text = temp.name += " Y6";
                pictureBoxY6.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-heli-".ToLower()) || temp.url2560.ToLower().Contains("-heli/ArduCopter".ToLower()))
            {
                pictureBoxHeli.Text = temp.name+= " heli";
                pictureBoxHeli.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-helhil".ToLower()) || temp.url2560.ToLower().Contains("-heli-hil/ArduCopter".ToLower()))
            {
                pictureBoxACHHil.Text = temp.name +=  " heli hil";
                pictureBoxACHHil.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-quadhil".ToLower()) || temp.url2560.ToLower().Contains("-quad-hil/ArduCopter".ToLower()))
            {
                pictureBoxACHil.Text = temp.name +=  " hil";
                pictureBoxACHil.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octaquad-".ToLower()) || temp.url2560.ToLower().Contains("-octa-quad/ArduCopter".ToLower()))
            {
                pictureBoxOctaQuad.Text = temp.name += " Octa Quad";
                pictureBoxOctaQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octa-".ToLower()) || temp.url2560.ToLower().Contains("-octa/ArduCopter".ToLower()))
            {
                pictureBoxOcta.Text = temp.name += " Octa";
                pictureBoxOcta.Tag = temp;
            }
            else if (temp.url2560_2.ToLower().Contains("antennatracker"))
            {
                pictureAntennaTracker.Text = temp.name;
                pictureAntennaTracker.Tag = temp;
            }
            else
            {
                log.Info("No Home " + temp.name + " " + temp.url2560);
            }
        }

        void findfirmware(Utilities.Firmware.software fwtoupload)
        {
            DialogResult dr = CustomMessageBox.Show("Are you sure you want to upload " + fwtoupload.name + "?", "Continue", MessageBoxButtons.YesNo);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.BaseStream.Close();
                }
                catch { }
                fw.Progress -= fw_Progress;
                fw.Progress += fw_Progress1;

                string history = (CMB_history.SelectedValue == null) ? "" : CMB_history.SelectedValue.ToString();

                bool updated = fw.update(MainV2.comPortName, fwtoupload, history);

                if (updated)
                {
                    if (fwtoupload.url2560_2 != null && fwtoupload.url2560_2.ToLower().Contains("copter"))
                        CustomMessageBox.Show("Warning, as of AC 3.1 motors will spin when armed, configurable through the MOT_SPIN_ARMED parameter", "Warning");
                }
                else
                {
                    CustomMessageBox.Show("Error uploading firmware","Error");
                }
            }

        }

        private void pictureBoxFW_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(Utilities.Firmware.software))
            {
                CustomMessageBox.Show("Bad Firmware", "Error"); return;
            }

            findfirmware((Utilities.Firmware.software)((Control)sender).Tag);
        }

        void up_LogEvent(string message, int level = 0)
        {
            lbl_status.Text = message;
            Application.DoEvents();
        }

        void up_ProgressEvent(double completed)
        {
            progress.Value = (int)completed;
            Application.DoEvents();
        }

        void port_Progress(int progress, string status)
        {
            log.InfoFormat("Progress {0} ", progress);
            this.progress.Value = progress;
            this.progress.Refresh();
        }

        private void FirmwareVisual_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (flashing == true)
            {
                e.Cancel = true;
                CustomMessageBox.Show("Cant exit while updating", "Error");
            }
        }

        private void CMB_history_SelectedIndexChanged(object sender, EventArgs e)
        {
            firmwareurl = fw.getUrl(CMB_history.SelectedValue.ToString(), "");

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
            CMB_history.DataSource = fw.niceNames;        

            CMB_history.Enabled = true;
            CMB_history.Visible = true;
            CMB_history_label.Visible = false;
        }

        //Load custom firmware (old CTRL+C shortcut)
        private void Custom_firmware_label_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog { Filter = "Firmware (*.hex;*.px4;*.vrx)|*.hex;*.px4;*.vrx" };
            if (Directory.Exists(custom_fw_dir))
                fd.InitialDirectory = custom_fw_dir;
            fd.ShowDialog();
            if (File.Exists(fd.FileName))
            {
                custom_fw_dir = Path.GetDirectoryName(fd.FileName);

                fw.Progress -= fw_Progress;
                fw.Progress += fw_Progress1;

                BoardDetect.boards boardtype = BoardDetect.boards.none;
                try
                {
                    boardtype = BoardDetect.DetectBoard(MainV2.comPortName);
                }
                catch
                {
                    CustomMessageBox.Show("Can not connect to com port and detect board type", "Error");
                    return;
                }

                fw.UploadFlash(MainV2.comPortName, fd.FileName, boardtype);
            }
        }

        private void lbl_devfw_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("These are beta firmware, use at your own risk!!!", "Beta");
            firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmware2.xml";
            softwares.Clear();
            UpdateFWList();
            CMB_history.Visible = false;
        }

        private void lbl_px4io_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("Please save the px4io.bin file to your microsd card to insert into your px4.", "IO");

            string baseurl = "http://firmware.diydrones.com/PX4IO/latest/PX4IO/px4io.bin";

            try
            {
                // Create a request using a URL that can receive a post. 
                WebRequest request = WebRequest.Create(baseurl);
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the request stream.
                Stream dataStream; //= request.GetRequestStream();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                log.Info(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                long bytes = response.ContentLength;
                long contlen = bytes;

                byte[] buf1 = new byte[1024];

                FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"px4io.bin", FileMode.Create);

                lbl_status.Text = "Downloading from Internet";

                this.Refresh();
                Application.DoEvents();

                dataStream.ReadTimeout = 30000;

                while (dataStream.CanRead)
                {
                    try
                    {
                        progress.Value = 50;// (int)(((float)(response.ContentLength - bytes) / (float)response.ContentLength) * 100);
                        this.progress.Refresh();
                    }
                    catch { }
                    int len = dataStream.Read(buf1, 0, 1024);
                    if (len == 0)
                        break;
                    bytes -= len;
                    fs.Write(buf1, 0, len);
                }

                fs.Close();
                dataStream.Close();
                response.Close();

                lbl_status.Text = "Done";
                Application.DoEvents();
            }
            catch { CustomMessageBox.Show("Error receiving firmware", "Error"); return; }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "px4io.bin";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (sfd.FileName != "")
                {
                    if (File.Exists(sfd.FileName))
                        File.Delete(sfd.FileName);

                    File.Copy(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"px4io.bin", sfd.FileName);
                }
            }

            progress.Value = 100;

            CustomMessageBox.Show("Please eject the microsd card, place into the px4, hold down the ioboard safety button, power on,\nand wait 60 seconds for the automated upgrade to take place.\nA upgrade status is created on your microsd card.");
        }

        private void lbl_dlfw_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://firmware.diydrones.com/");
            }
            catch { CustomMessageBox.Show("Can not open url http://firmware.diydrones.com/", "Error"); }
        }

        private void lbl_px4bl_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.Open(false);

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    MainV2.comPort.doReboot(true);
                    CustomMessageBox.Show("Please ignore the unplug and plug back in when uploading flight firmware.");
                }
                else
                {
                    throw new Exception();
                }
            }
            catch { CustomMessageBox.Show("Failed to connect and send the reboot command","Error"); }
        }
    }
}