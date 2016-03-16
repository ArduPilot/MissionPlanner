using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Comms;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using log4net;
using System.Collections;

namespace MissionPlanner
{
    public partial class SerialInjectGPS : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // serialport
        internal static ICommsSerial comPort = new SerialPort();
        // rtcm detection
        private Utilities.rtcm3 rtcm3 = new Utilities.rtcm3();
        // sbp detection
        private Utilities.sbp sbp = new Utilities.sbp();
        // background thread 
        private System.Threading.Thread t12;
        private static bool threadrun = false;
        // track rtcm msg's seen
        static Hashtable msgseen = new Hashtable();
        // track bytes seen
        static int bytes = 0;
        static int bps = 0;

        // Thread signal. 
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public SerialInjectGPS()
        {
            InitializeComponent();

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("UDP Host");
            CMB_serialport.Items.Add("UDP Client");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("NTRIP");

            if (threadrun)
            {
                BUT_connect.Text = Strings.Stop;
            }

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                threadrun = false;
                comPort.Close();
                BUT_connect.Text = Strings.Connect;
            }
            else
            {
                try
                {
                    switch (CMB_serialport.Text)
                    {
                        case "NTRIP":
                            comPort = new CommsNTRIP();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "TCP Client":
                            comPort = new TcpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host":
                            comPort = new UdpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Client":
                            comPort = new UdpSerialConnect();
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        default:
                            comPort = new SerialPort();
                            comPort.PortName = CMB_serialport.Text;
                            break;
                    }
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidPortName);
                    return;
                }
                try
                {
                    comPort.BaudRate = int.Parse(CMB_baudrate.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate);
                    return;
                }
                try
                {
                    comPort.Open();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??\n" +
                                          ex.ToString());
                    return;
                }

                t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
                {
                    IsBackground = true,
                    Name = "injectgps"
                };
                t12.Start();

                BUT_connect.Text = Strings.Stop;

                msgseen.Clear();
                bytes = 0;
            }
        }

        private void updateLabel(string label)
        {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            this.lbl_status.Text = label;
                        }
                    );
            }
        }

        private void mainloop()
        {
            DateTime lastrecv = DateTime.MinValue;
            threadrun = true;
            while (threadrun)
            {
                try
                {
                    // reconnect logic - 10 seconds with no data, or comport is closed
                    try
                    {
                        if ((DateTime.Now - lastrecv).TotalSeconds > 10 || !comPort.IsOpen)
                        {
                            log.Info("Reconnecting");
                            // close existing
                            comPort.Close();
                            // reopen
                            comPort.Open();
                            // reset timer
                            lastrecv = DateTime.Now;
                        }
                    }
                    catch
                    {
                        log.Error("Failed to reconnect");
                        // sleep for 10 seconds on error
                        System.Threading.Thread.Sleep(10000);
                    }

                    // limit to 110 byte packets
                    byte[] buffer = new byte[110];

                    while (comPort.BytesToRead > 0)
                    {
                        int read = comPort.Read(buffer, 0, Math.Min(buffer.Length, comPort.BytesToRead));

                        if (read > 0)
                            lastrecv = DateTime.Now;

                        bytes += read;
                        bps += read;

                        MainV2.comPort.InjectGpsData(buffer, (byte) read);

                        // check for valid rtcm packets
                        for (int a = 0; a < read; a++)
                        {
                            int seen = -1;
                            // rtcm
                            if ((seen = rtcm3.Read(buffer[a])) > 0)
                            {
                                if (!msgseen.ContainsKey(seen))
                                    msgseen[seen] = 0;
                                msgseen[seen] = (int)msgseen[seen] + 1;
                            }
                            // sbp
                            if ((seen = sbp.read(buffer[a])) > 0)
                            {
                                if (!msgseen.ContainsKey(seen))
                                    msgseen[seen] = 0;
                                msgseen[seen] = (int)msgseen[seen] + 1;
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        private void CMB_serialport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!CMB_serialport.Text.ToLower().Contains("com"))
                CMB_baudrate.Enabled = false;
            else
                CMB_baudrate.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var item in msgseen.Keys)
                {
                    sb.Append(item + "=" + msgseen[item] + " ");
                }
            }
            catch
            {
            }

            updateLabel("bytes " + bytes + " bps " + bps + "\n" + sb.ToString());
            bps = 0;
        }

        private void SerialInjectGPS_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void SerialInjectGPS_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }
    }
}