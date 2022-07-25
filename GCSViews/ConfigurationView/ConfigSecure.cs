using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using DeviceProgramming.FileFormat;
using LibUsbDotNet;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Crypto.Digests;
using px4uploader;
using Firmware = px4uploader.Firmware;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigSecure : UserControl, IDeactivate, IActivate
    {
        private Uri _result;
        private HttpClient httpclient;
        private TcpListener listener;

        private Uri tokenurl = new Uri("https://secure.cubepilot.com/api/Auth/GetToken");

        private Uri login = new Uri("https://secure.cubepilot.com/api/Auth/Login");

        private Uri firmwareurl = new Uri("https://secure.cubepilot.com/api/Firmware/CreateFirmware");

        private Uri bootloaderurl = new Uri("https://secure.cubepilot.com/api/Firmware/CreateBootLoader");
        
        private Uri dfufirmware = new Uri("https://secure.cubepilot.com/CubeOrange_dfusetup.apj");

        private Uri sncheck = new Uri("https://secure.cubepilot.com/api/Firmware/CheckSN");

        private string token;
        private string detectedport;

        public ConfigSecure()
        {
            InitializeComponent();
            httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.ExpectContinue = false;

            UpdateStatus("Waiting...", 0);

            timer1.Start();
        }

        private async void but_login_Click(object sender, EventArgs e)
        {
            var port = new Random().Next(1025, 65500);

            listener?.Stop();
            listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            UpdateStatus("Started Auth", 0);

            Common.OpenUrl(login.ToString() + $"?return_url=http://localhost:{port}{tokenurl.AbsolutePath}");

            UpdateStatus("Waiting for Auth token", 50);
            var tcpClient = await listener.AcceptTcpClientAsync();
            tcpClient.NoDelay = true;
            var stream = tcpClient.GetStream();
            stream.ReadTimeout = 5000;
            StreamReader sr = new StreamReader(stream);
            var line = await sr.ReadLineAsync();
            //GET /dfsddf HTTP/1.0
            var uri = new Uri(tokenurl, line.Split(' ')[1]);

            token = await httpclient.GetStringAsync(uri);

            httpclient.DefaultRequestHeaders.Remove("Authorization");
            httpclient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

            byte[] data =
                Encoding.UTF8.GetBytes(
                    "HTTP/1.0 200 OK\r\nContent-Type: text/html\r\nConnection: close\r\n\r\n<html><script>window.open('', '_self').close()</script>"
                        .PadRight(2048));

            stream.Write(data, 0, data.Length);
            await stream.FlushAsync();

            await Task.Delay(2000);

            tcpClient.Close();

            listener?.Stop();

            UpdateStatus("Login Done.", 100);

            // run a single scan incase its already in BL mode
            Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL);
        }

        private void UpdateStatus(string v1, int v2)
        {
            try
            {
                this.BeginInvoke((Action)delegate
                {
                    if (v1 != "")
                        label2.Text = v1;
                    if (v2 >= 0)
                        progressBar1.Value = v2;
                });
            } catch { }
        }

        private void but_getsn_Click(object sender, EventArgs e)
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;

            Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL);

            MainV2.instance.DeviceChanged += Instance_DeviceChanged;

            CustomMessageBox.Show("Please re-power to autopilot");
            UpdateStatus("Please re-power to autopilot", 100);
        }

        private void Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum cause)
        {
            if (cause != MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL)
                return;

            timer1_Tick(this, null);

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
                        up.ProgressEvent += completed => UpdateStatus("", (int)completed);
                        up.LogEvent += (message, level) => UpdateStatus(message, -1);
                        up.identify();
                        up.close();
                        detectedport = port;
                        UpdateStatus("Detected " + port, 100);
                        this.InvokeIfRequired(() =>
                        {
                            this.textBox1.Text = BitConverter.ToString(up.sn).Replace("-", "");
                            timer1_Tick(this, null);
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Not There..");
                        up.close();
                    }
                });
            });
        }

        private async void but_dfu_Click(object sender, EventArgs e)
        {
            var resp = await httpclient.GetAsync(sncheck + "?SN=" + this.textBox1.Text);
            if (!resp.IsSuccessStatusCode)
            {
                CustomMessageBox.Show(resp.ReasonPhrase, "Error");
                UpdateStatus(resp.ReasonPhrase, 0);
                return;
            }

            progressBar1.Value = 0;
            UpdateStatus("Downloading DFU FW", 0);
            var dfufw = await httpclient.GetByteArrayAsync(dfufirmware);
            MemoryStream ms = new MemoryStream(dfufw);
            var fw = Firmware.ProcessFirmware(new StreamReader(ms));
            var up = new Uploader(detectedport, 115200);
            up.ProgressEvent += completed => UpdateStatus("", (int)completed);
            up.LogEvent += (message, level) => UpdateStatus(message, -1);
            up.identify();
            up.currentChecksum(fw);
            up.upload(fw);
            up.close();
        }

        private async void but_bootloader_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            //https://www.st.com/resource/en/application_note/cd00264379-usb-dfu-protocol-used-in-the-stm32-bootloader-stmicroelectronics.pdf
            //https://www.st.com/resource/en/application_note/an1577-device-firmware-upgrade-dfu-implementation-in-st7-usb-devices-stmicroelectronics.pdf
            var sn = DFU.GetSN();

            textBox1.Text = BitConverter.ToString(sn).Replace("-", String.Empty);

            UpdateStatus("Downloading Bootloader FW", 0);
            var bl = await httpclient.PostAsync(bootloaderurl + $"?SN={textBox1.Text}", null);
            if (!bl.IsSuccessStatusCode)
            {
                UpdateStatus("Failed to download " + bl.ReasonPhrase + " " + await bl.Content.ReadAsStringAsync(), 0);
                return;
            }
            var tempfile = Path.GetTempFileName();
            File.WriteAllBytes(tempfile, await bl.Content.ReadAsByteArrayAsync());
            DFU.Progress += (i, s) => UpdateStatus("DFU Download Bootloader", (int)i);
            DFU.Flash(tempfile, 0x08000000);
        }

        private async void but_firmware_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.apj|*.apj";
            ofd.ShowDialog();
            if (File.Exists(ofd.FileName))
            {
                {
                    var fw = Firmware.ProcessFirmware(new StreamReader(ofd.FileName));
                    var sha = new Sha256Digest();
                    sha.BlockUpdate(fw.imagebyte, 0, fw.imagebyte.Length);
                    var hash = new byte[32];
                    sha.DoFinal(hash, 0);
                    txt_sha.Text = hash.ToHexString();
                }
                var bytes = File.ReadAllBytes(ofd.FileName);
                var filecontent = new ByteArrayContent(bytes);
                filecontent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    { FileName = Path.GetFileName(ofd.FileName), Name = "file" };
                filecontent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var mp = new MultipartContent("form-data");
                mp.Add(filecontent);
                UpdateStatus("Uploading/Downloading FW", 0);
                var fwresp = await httpclient.PostAsync(firmwareurl + $"?SN={textBox1.Text}", mp);
                if (fwresp.IsSuccessStatusCode)
                {                   
                    var fw = Firmware.ProcessFirmware(new StreamReader(await fwresp.Content.ReadAsStreamAsync()));

                    var up = new Uploader(detectedport);
                    up.ProgressEvent += completed => UpdateStatus("", (int)completed);
                    up.LogEvent += (message, level) => UpdateStatus(message, -1);
                    up.identify();
                    up.currentChecksum(fw);
                    up.upload(fw);
                    up.close();
                }
                else
                {
                    UpdateStatus("Firmware Uploading FAILED", (int)0);
                    CustomMessageBox.Show("Web request failed: " + fwresp.ReasonPhrase);
                    Console.WriteLine(await fwresp.Content.ReadAsStringAsync());
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // logged in
            if (!String.IsNullOrEmpty(token))
            {
                if (UsbDevice.AllDevices.Count > 0)
                {
                    // is in dfu mode, enable BL upload
                    but_bootloader.Enabled = true;
                    but_getsn.Enabled = false;
                    label4.Text = String.Format("Detected DFU Devices {0}", UsbDevice.AllDevices.Count);
                }
                else
                {
                    but_bootloader.Enabled = false;

                    var ports = Win32DeviceMgmt.GetAllCOMPorts();
                    var validbl = ports.Where(a => a.board != null && a.board.EndsWith("BL"));

                    // valid BL detected and SN rx'ed
                    if (validbl.Count() > 0 && textBox1.Text != "")
                    {
                        // in BL already
                        but_getsn.Enabled = false;
                        if (validbl.First().board.Contains("SecureBL"))
                        {
                            // secure bootloader
                            but_firmware.Enabled = true;
                            but_dfu.Enabled = false;
                        }
                        else
                        {
                            // is in bootloader but not secure
                            but_dfu.Enabled = true;
                        }
                    }
                    else
                    {
                        // no bl detected - prompt user
                        but_getsn.Enabled = true;
                        but_firmware.Enabled = false;
                        but_bootloader.Enabled = false;
                    }

                    label4.Text = String.Format("Detected DFU Devices {0}, Bootloader Devices {1}", UsbDevice.AllDevices.Count, validbl.Count());
                }
            }
        }

        public void Deactivate()
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;
        }

        public void Activate()
        {
            MainV2.instance.DeviceChanged -= Instance_DeviceChanged;
            Instance_DeviceChanged(MainV2.WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL);
        }
    }
}
