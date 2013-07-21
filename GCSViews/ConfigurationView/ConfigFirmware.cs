using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;
using log4net;
using ArdupilotMega.Comms;
using ArdupilotMega.Arduino;
using ArdupilotMega.Utilities;
using System.Text.RegularExpressions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using px4uploader;
using ArdupilotMega.Controls;
using System.Collections;

namespace ArdupilotMega.GCSViews
{
    partial class ConfigFirmware : MyUserControl
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        string firmwareurl = "https://raw.github.com/diydrones/binary/master/Firmware/firmware2.xml";

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //CTRL+R moved to pictureBoxRover_Click
            //CTRL+O moved to CMB_history_label_Click
            //CTRL+C moved to Custom_firmware_label_Click

            if (keyData == (Keys.Control | Keys.Q))
            {
                CustomMessageBox.Show("These are the latest trunk firmware, use at your own risk!!!", "trunk");
                firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmwarelatest.xml";
                Firmware_Load(null, null);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

      

        List<software> softwares = new List<software>();
        bool flashing = false;

        public struct software
        {
            public string url;
            public string url2560;
            public string url2560_2;
            public string urlpx4;
            public string name;
            public string desc;
            public int k_format_version;
        }

        public ConfigFirmware()
        {
            InitializeComponent();

            WebRequest.DefaultWebProxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

            this.pictureBoxAPM.Image = ArdupilotMega.Properties.Resources.APM_airframes_001;
            this.pictureBoxRover.Image = ArdupilotMega.Properties.Resources.rover_11;
            this.pictureBoxQuad.Image = ArdupilotMega.Properties.Resources.quad;
            this.pictureBoxHexa.Image = ArdupilotMega.Properties.Resources.hexa;
            this.pictureBoxTri.Image = ArdupilotMega.Properties.Resources.tri;
            this.pictureBoxY6.Image = ArdupilotMega.Properties.Resources.y6;
            this.pictureBoxHeli.Image = ArdupilotMega.Properties.Resources.APM_airframes_08;
            this.pictureBoxHilimage.Image = ArdupilotMega.Properties.Resources.hil;
            this.pictureBoxAPHil.Image = ArdupilotMega.Properties.Resources.hilplane;
            this.pictureBoxACHil.Image = ArdupilotMega.Properties.Resources.hilquad;
            this.pictureBoxACHHil.Image = ArdupilotMega.Properties.Resources.hilheli;
            this.pictureBoxOcta.Image = ArdupilotMega.Properties.Resources.octo;
            this.pictureBoxOctaQuad.Image = ArdupilotMega.Properties.Resources.x8;

        }

        internal void Firmware_Load(object sender, EventArgs e)
        {
            ProgressReporterDialogue pdr = new ArdupilotMega.Controls.ProgressReporterDialogue();

            pdr.DoWork += pdr_DoWork;

            pdr.UpdateProgressAndStatus(-1,"Getting Firmware List");

            ThemeManager.ApplyThemeTo(pdr);

            pdr.RunBackgroundOperationAsync();
        }

        void pdr_DoWork(object sender, Controls.ProgressWorkerEventArgs e, object passdata = null)
        {
            log.Info("FW load");

            string url = "";
            string url2560 = "";
            string url2560_2 = "";
            string px4 = "";
            string name = "";
            string desc = "";
            int k_format_version = 0;

            softwares.Clear();

            software temp = new software();

            // this is for mono to a ssl server
            //ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy(); 
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback((sender1, certificate, chain, policyErrors) => { return true; });

            try
            {
                log.Info("url: " + firmwareurl);
                using (XmlTextReader xmlreader = new XmlTextReader(firmwareurl))
                {
                    while (xmlreader.Read())
                    {
                        xmlreader.MoveToElement();
                        switch (xmlreader.Name)
                        {
                            case "url":
                                url = xmlreader.ReadString();
                                break;
                            case "url2560":
                                url2560 = xmlreader.ReadString();
                                break;
                            case "url2560-2":
                                url2560_2 = xmlreader.ReadString();
                                break;
                            case "urlpx4":
                                px4 = xmlreader.ReadString();
                                break;
                            case "name":
                                name = xmlreader.ReadString();
                                break;
                            case "format_version":
                                k_format_version = int.Parse(xmlreader.ReadString());
                                break;
                            case "desc":
                                desc = xmlreader.ReadString();
                                break;
                            case "Firmware":
                                if (!url2560.Equals("") && !name.Equals("") && !desc.Equals("Please Update"))
                                {
                                    temp.desc = desc;
                                    temp.name = name;
                                    temp.url = url;
                                    temp.url2560 = url2560;
                                    temp.url2560_2 = url2560_2;
                                    temp.urlpx4 = px4;
                                    temp.k_format_version = k_format_version;

                                    try
                                    {
                                        try
                                        {
                                            if (!url2560.Contains("github"))
                                            {
                                                name = getAPMVersion(temp.url2560);
                                                if (name != "")
                                                    temp.name = name;
                                            }
                                        }
                                        catch { }

                                        updateDisplayNameInvoke(temp);
                                    }
                                    catch { } // just in case

                                    softwares.Add(temp);
                                }
                                url = "";
                                url2560 = "";
                                name = "";
                                desc = "";
                                k_format_version = 0;
                                temp = new software();
                                break;
                            default:
                                break;
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                log.Error(ex);
                //CustomMessageBox.Show("Failed to get Firmware List : " + ex.Message);
                throw ex;
            }
            log.Info("FW load done");

            ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(100, "Done");
        }

        string getAPMVersion(string fwurl)
        {
            Uri url = new Uri(new Uri(fwurl), "git-version.txt");

            log.Info("Get url " + url.ToString());

            WebRequest wr = WebRequest.Create(url);
            WebResponse wresp = wr.GetResponse();

            StreamReader sr = new StreamReader(wresp.GetResponseStream());

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                if (line.Contains("APMVERSION:"))
                {
                    log.Info(line);
                    return line.Substring(line.IndexOf(':') + 2);
                }
            }

            log.Info("no answer");
            return "";
        }

        void updateDisplayNameInvoke(software temp)
        {
            this.Invoke((MethodInvoker)delegate
            {
                updateDisplayName(temp);
            });
        }

        void updateDisplayName(software temp)
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
                pictureBoxQuad.Text = temp.name;
                pictureBoxQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-tri".ToLower()) || temp.url2560.ToLower().Contains("-tri/ArduCopter".ToLower()))
            {
                pictureBoxTri.Text = temp.name;
                pictureBoxTri.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-hexa".ToLower()) || temp.url2560.ToLower().Contains("-hexa/ArduCopter".ToLower()))
            {
                pictureBoxHexa.Text = temp.name;
                pictureBoxHexa.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-y6".ToLower()) || temp.url2560.ToLower().Contains("-y6/ArduCopter".ToLower()))
            {
                pictureBoxY6.Text = temp.name;
                pictureBoxY6.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-heli-".ToLower()) || temp.url2560.ToLower().Contains("-heli/ArduCopter".ToLower()))
            {
                pictureBoxHeli.Text = temp.name;
                pictureBoxHeli.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-helhil".ToLower()) || temp.url2560.ToLower().Contains("-heli-hil/ArduCopter".ToLower()))
            {
                pictureBoxACHHil.Text = temp.name;
                pictureBoxACHHil.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-quadhil".ToLower()) || temp.url2560.ToLower().Contains("-quad-hil/ArduCopter".ToLower()))
            {
                pictureBoxACHil.Text = temp.name;
                pictureBoxACHil.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octaquad-".ToLower()) || temp.url2560.ToLower().Contains("-octa-quad/ArduCopter".ToLower()))
            {
                pictureBoxOctaQuad.Text = temp.name;
                pictureBoxOctaQuad.Tag = temp;
            }
            else if (temp.url2560.ToLower().Contains("ac2-octa-".ToLower()) || temp.url2560.ToLower().Contains("-octa/ArduCopter".ToLower()))
            {
                pictureBoxOcta.Text = temp.name;
                pictureBoxOcta.Tag = temp;
            }
            else
            {
                log.Info("No Home " + temp.name + " " + temp.url2560);
            }
        }

        void findfirmware(software findwhat)
        {
            DialogResult dr = CustomMessageBox.Show("Are you sure you want to upload " + findwhat.name + "?", "Continue", MessageBoxButtons.YesNo);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                update(findwhat);
            }

        }

        private void pictureBoxRover_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxAPM_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxAPMHIL_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxQuad_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxHexa_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxTri_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxY6_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxHeli_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxQuadHil_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }


        private void pictureBoxOctav_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxOcta_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxAPHil_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxACHil_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }

        private void pictureBoxACHHil_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag.GetType() != typeof(software))
            {
                CustomMessageBox.Show("Bad Firmware"); return;
            }

            findfirmware((software)((Control)sender).Tag);
        }


        private void update(software temp)
        {
            string board = "";
            MainV2.comPort.BaseStream.DtrEnable = false;
            MainV2.comPort.Close();
            System.Threading.Thread.Sleep(100);
            MainV2.comPort.giveComport = true;

            try
            {
                if (softwares.Count == 0)
                {
                    CustomMessageBox.Show("No valid options");
                    return;
                }

                lbl_status.Text = "Detecting APM Version";

                this.Refresh();
                Application.DoEvents();

                /*
                ArdupilotMega.Controls.Firmware_Board fwb = new ArdupilotMega.Controls.Firmware_Board();
                fwb.ShowDialog();

                var boardname = ArdupilotMega.Controls.Firmware_Board.fw;

                switch (boardname)
                {
                    case ArdupilotMega.Controls.Firmware_Board.Firmware.apm1:
                        board = "2560";
                        break;
                    case ArdupilotMega.Controls.Firmware_Board.Firmware.apm2:
                        board = "2560-2";
                        break;
                    case ArdupilotMega.Controls.Firmware_Board.Firmware.apm2_5:
                        board = "2560-2";
                        break;
                    case ArdupilotMega.Controls.Firmware_Board.Firmware.px4:
                        board = "px4";
                        break;
                }
                */
                board = ArduinoDetect.DetectBoard(MainV2.comPortName);

                if (board == "")
                {
                    CustomMessageBox.Show("Cant detect your APM version. Please check your cabling");
                    return;
                }

                int apmformat_version = -1; // fail continue

                if (board != "px4")
                {
                    try
                    {

                        apmformat_version = ArduinoDetect.decodeApVar(MainV2.comPortName, board);
                    }
                    catch { }

                    if (apmformat_version != -1 && apmformat_version != temp.k_format_version)
                    {
                        if (DialogResult.No == CustomMessageBox.Show("Epprom changed, all your setting will be lost during the update,\nDo you wish to continue?", "Epprom format changed (" + apmformat_version + " vs " + temp.k_format_version + ")", MessageBoxButtons.YesNo))
                        {
                            CustomMessageBox.Show("Please connect and backup your config in the configuration tab.");
                            return;
                        }
                    }
                }


                log.Info("Detected a " + board);

                string baseurl = "";
                if (board == "2560")
                {
                    baseurl = temp.url2560.ToString();
                }
                else if (board == "1280")
                {
                    baseurl = temp.url.ToString();
                }
                else if (board == "2560-2")
                {
                    baseurl = temp.url2560_2.ToString();
                }
                else if (board == "px4")
                {
                    baseurl = temp.urlpx4.ToString();
                }
                else
                {
                    CustomMessageBox.Show("Invalid Board Type");
                    return;
                }

                log.Info("Using " + baseurl);

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

                FileStream fs = new FileStream(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex", FileMode.Create);

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

                progress.Value = 100;
                this.Refresh();
                Application.DoEvents();
                log.Info("Downloaded");
            }
            catch (Exception ex) { lbl_status.Text = "Failed download"; CustomMessageBox.Show("Failed to download new firmware : " + ex.ToString()); return; }

            System.Threading.ThreadPool.QueueUserWorkItem(apmtype, temp.name + "!" + board);

            UploadFlash(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"firmware.hex", board);
        }

        void apmtype(object temp)
        {
            try
            {
                // Create a request using a URL that can receive a post. 
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("http://vps.oborne.me/axs/ax.pl?" + (string)temp);
                //request.AllowAutoRedirect = true;
                request.UserAgent = MainV2.instance.Text + " (res" + Screen.PrimaryScreen.Bounds.Width + "x" + Screen.PrimaryScreen.Bounds.Height + "; " + Environment.OSVersion.VersionString + "; cores " + Environment.ProcessorCount + ")";
                request.Timeout = 10000;
                // Set the Method property of the request to POST.
                request.Method = "GET";
                // Get the request stream.
                // Get the response.
                WebResponse response = request.GetResponse();
            }
            catch { }
        }

        public void UploadPX4(string filename)
        {

            DateTime DEADLINE = DateTime.Now.AddSeconds(30);

            Uploader up;
            px4uploader.Firmware fw = px4uploader.Firmware.ProcessFirmware(filename);

            CustomMessageBox.Show("Press reset the board, and then press OK within 5 seconds.\nMission Planner will look for 30 seconds to find the board");

            while (DateTime.Now < DEADLINE)
            {

                string port = MainV2.comPortName;

                Console.WriteLine(DateTime.Now.Millisecond + " Trying Port " + port);

                lbl_status.Text = "Connecting";
                Application.DoEvents();

                try
                {
                    up = new Uploader(port, 115200);
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
                    lbl_status.Text = "Identify";
                    Application.DoEvents();
                    Console.WriteLine("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type, up.board_rev, up.bl_rev, up.fw_maxsize, port);

                    up.currentChecksum(fw);
                }
                catch (Exception)
                {
                    Console.WriteLine("Not There..");
                    //Console.WriteLine(ex.Message);
                    up.close();
                    continue;
                }

                try
                {
                    up.ProgressEvent += new Uploader.ProgressEventHandler(up_ProgressEvent);
                    up.LogEvent += new Uploader.LogEventHandler(up_LogEvent);

                    progress.Value = 0;
                    Application.DoEvents();
                    up.upload(fw);
                    lbl_status.Text = "Done";
                    Application.DoEvents();
                }
                catch (Exception ex)
                {
                    lbl_status.Text = "ERROR: " + ex.Message;
                    Application.DoEvents();
                    Console.WriteLine(ex.ToString());

                }
                up.close();

                break;
            }

            CustomMessageBox.Show("Please unplug, and plug back in your px4, before you try connecting");
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

        public void UploadFlash(string filename, string board)
        {
            if (board == "px4")
            {
                UploadPX4(filename);
                return;
            }

            byte[] FLASH = new byte[1];
            StreamReader sr = null;
            try
            {
                lbl_status.Text = "Reading Hex File";
                this.Refresh();
                Application.DoEvents();
                sr = new StreamReader(filename);
                FLASH = readIntelHEXv2(sr);
                sr.Close();
                log.InfoFormat("\n\nSize: {0}\n\n", FLASH.Length);
            }
            catch (Exception ex)
            {
                if (sr != null)
                {
                    sr.Dispose();
                }
                lbl_status.Text = "Failed read HEX";
                CustomMessageBox.Show("Failed to read firmware.hex : " + ex.Message);
                return;
            }
            IArduinoComms port = new ArduinoSTK();

            if (board == "1280")
            {
                if (FLASH.Length > 126976)
                {
                    CustomMessageBox.Show("Firmware is to big for a 1280, Please upgrade your hardware!!");
                    return;
                }
                //port = new ArduinoSTK();
                port.BaudRate = 57600;
            }
            else if (board == "2560" || board == "2560-2")
            {
                port = new ArduinoSTKv2
                           {
                               BaudRate = 115200
                           };
            }
            port.DataBits = 8;
            port.StopBits = System.IO.Ports.StopBits.One;
            port.Parity = System.IO.Ports.Parity.None;
            port.DtrEnable = true;

            try
            {
                port.PortName = MainV2.comPortName;

                port.Open();

                flashing = true;

                if (port.connectAP())
                {
                    log.Info("starting");
                    lbl_status.Text = "Uploading " + FLASH.Length + " bytes to APM Board: " + board;
                    progress.Value = 0;
                    this.Refresh();

                    // this is enough to make ap_var reset
                    //port.upload(new byte[256], 0, 2, 0);

                    port.Progress += port_Progress;

                    if (!port.uploadflash(FLASH, 0, FLASH.Length, 0))
                    {
                        flashing = false;
                        if (port.IsOpen)
                            port.Close();
                        throw new Exception("Upload failed. Lost sync. Try Arduino!!");
                    }

                    port.Progress -= new ProgressEventHandler(port_Progress);

                    progress.Value = 100;

                    log.Info("Uploaded");

                    this.Refresh();

                    int start = 0;
                    short length = 0x100;

                    byte[] flashverify = new byte[FLASH.Length + 256];

                    lbl_status.Text = "Verify APM";
                    progress.Value = 0;
                    this.Refresh();

                    while (start < FLASH.Length)
                    {
                        progress.Value = (int)((start / (float)FLASH.Length) * 100);
                        progress.Refresh();
                        port.setaddress(start);
                        log.Info("Downloading " + length + " at " + start);
                        port.downloadflash(length).CopyTo(flashverify, start);
                        start += length;
                    }

                    progress.Value = 100;

                    for (int s = 0; s < FLASH.Length; s++)
                    {
                        if (FLASH[s] != flashverify[s])
                        {
                            CustomMessageBox.Show("Upload succeeded, but verify failed: exp " + FLASH[s].ToString("X") + " got " + flashverify[s].ToString("X") + " at " + s);
                            break;
                        }
                    }

                    lbl_status.Text = "Write Done... Waiting (17 sec)";
                }
                else
                {
                    lbl_status.Text = "Failed upload";
                    CustomMessageBox.Show("Communication Error - no connection");
                }
                port.Close();

                flashing = false;

                Application.DoEvents();

                try
                {
                    ((SerialPort)port).Open();
                }
                catch { }

                DateTime startwait = DateTime.Now;

                CustomMessageBox.Show("Please ensure you do a live compass calibration after installing arducopter V 3.x");

                try
                {
                    ((SerialPort)port).Close();
                }
                catch { }

                progress.Value = 100;
                lbl_status.Text = "Done";
            }
            catch (Exception ex)
            {
                lbl_status.Text = "Failed upload";
                CustomMessageBox.Show("Check port settings or Port in use? " + ex);
                try
                {
                    port.Close();
                }
                catch { }
            }
            flashing = false;
            MainV2.comPort.giveComport = false;
        }

        void port_Progress(int progress, string status)
        {
            log.InfoFormat("Progress {0} ", progress);
            this.progress.Value = progress;
            this.progress.Refresh();
        }

        byte[] readIntelHEXv2(StreamReader sr)
        {
            byte[] FLASH = new byte[1024 * 1024];

            int optionoffset = 0;
            int total = 0;
            bool hitend = false;

            while (!sr.EndOfStream)
            {
                progress.Value = (int)(((float)sr.BaseStream.Position / (float)sr.BaseStream.Length) * 100);
                progress.Refresh();

                string line = sr.ReadLine();

                if (line.StartsWith(":"))
                {
                    int length = Convert.ToInt32(line.Substring(1, 2), 16);
                    int address = Convert.ToInt32(line.Substring(3, 4), 16);
                    int option = Convert.ToInt32(line.Substring(7, 2), 16);
                    log.InfoFormat("len {0} add {1} opt {2}", length, address, option);

                    if (option == 0)
                    {
                        string data = line.Substring(9, length * 2);
                        for (int i = 0; i < length; i++)
                        {
                            byte byte1 = Convert.ToByte(data.Substring(i * 2, 2), 16);
                            FLASH[optionoffset + address] = byte1;
                            address++;
                            if ((optionoffset + address) > total)
                                total = optionoffset + address;
                        }
                    }
                    else if (option == 2)
                    {
                        optionoffset = (int)Convert.ToUInt16(line.Substring(9, 4), 16) << 4;
                    }
                    else if (option == 1)
                    {
                        hitend = true;
                    }
                    int checksum = Convert.ToInt32(line.Substring(line.Length - 2, 2), 16);

                    byte checksumact = 0;
                    for (int z = 0; z < ((line.Length - 1 - 2) / 2); z++) // minus 1 for : then mins 2 for checksum itself
                    {
                        checksumact += Convert.ToByte(line.Substring(z * 2 + 1, 2), 16);
                    }
                    checksumact = (byte)(0x100 - checksumact);

                    if (checksumact != checksum)
                    {
                        CustomMessageBox.Show("The hex file loaded is invalid, please try again.");
                        throw new Exception("Checksum Failed - Invalid Hex");
                    }
                }
                //Regex regex = new Regex(@"^:(..)(....)(..)(.*)(..)$"); // length - address - option - data - checksum
            }

            if (!hitend)
            {
                CustomMessageBox.Show("The hex file did no contain an end flag. aborting");
                throw new Exception("No end flag in file");
            }

            Array.Resize<byte>(ref FLASH, total);

            return FLASH;
        }

        private void FirmwareVisual_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (flashing == true)
            {
                e.Cancel = true;
                CustomMessageBox.Show("Cant exit while updating");
            }
        }

        private void BUT_setup_Click(object sender, EventArgs e)
        {
            Form temp = new Form();
            MyUserControl configview = new GCSViews.ConfigurationView.Setup();
            temp.Controls.Add(configview);
            ThemeManager.ApplyThemeTo(temp);
            // fix title
            temp.Text = configview.Name;
            // fix size
            temp.Size = configview.Size;
            configview.Dock = DockStyle.Fill;
            temp.FormClosing += configview.Close;
            temp.ShowDialog();
        }

        //Load custom firmware (old CTRL+C shortcut)
        private void Custom_firmware_label_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog { Filter = "Firmware (*.hex;*.px4)|*.hex;*.px4" };
            fd.ShowDialog();
            if (File.Exists(fd.FileName))
            {
                string boardtype = "";
                try
                {
                    boardtype = ArduinoDetect.DetectBoard(MainV2.comPortName);
                }
                catch
                {
                    CustomMessageBox.Show("Can not connect to com port and detect board type");
                    return;
                }


                UploadFlash(fd.FileName, boardtype);
            }
        }

        private void lbl_devfw_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("These are beta firmware, use at your own risk!!!", "Beta");
            firmwareurl = "https://raw.github.com/diydrones/binary/master/dev/firmware2.xml";
            Firmware_Load(null, null);
        }

        private void lbl_px4io_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("Please save the px4io.bin file to your microsd card to insert into your px4.");

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
            catch { CustomMessageBox.Show("Error receiving firmware"); return; }

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
            System.Diagnostics.Process.Start("http://firmware.diydrones.com/");
        }
    }
}