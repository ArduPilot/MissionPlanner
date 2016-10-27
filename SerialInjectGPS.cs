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

        static bool rtcm_msg = true;

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

            chk_rtcmmsg.Checked = rtcm_msg;

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

                // inject init strings - m8p
                if (true)
                {
                    var ubloxm8p_timepulse_60s_1m = new byte[]
                    {
                        0xB5, 0x62, 0x06, 0x71, 0x28, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3C, 0x00,
                        0x00, 0x00, 0x20, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4A, 0x53
                    };
                    comPort.Write(ubloxm8p_timepulse_60s_1m, 0, ubloxm8p_timepulse_60s_1m.Length);
                    var ubloxm8p_msg_f5_05_5s = new byte[]
                    {0xB5, 0x62, 0x06, 0x01, 0x08, 0x00, 0xF5, 0x05, 0x00, 0x05, 0x00, 0x05, 0x00, 0x00, 0x13, 0x96};
                    comPort.Write(ubloxm8p_msg_f5_05_5s, 0, ubloxm8p_msg_f5_05_5s.Length);
                    var ubloxm8p_msg_f5_4d_1s = new byte[]
                    {0xB5, 0x62, 0x06, 0x01, 0x08, 0x00, 0xF5, 0x4D, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x53, 0x6E};
                    comPort.Write(ubloxm8p_msg_f5_4d_1s, 0, ubloxm8p_msg_f5_4d_1s.Length);
                    var ubloxm8p_msg_f5_57_1s = new byte[]
                    {0xB5, 0x62, 0x06, 0x01, 0x08, 0x00, 0xF5, 0x57, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x5D, 0xB4};
                    comPort.Write(ubloxm8p_msg_f5_57_1s, 0, ubloxm8p_msg_f5_57_1s.Length);
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

            bool isrtcm = false;
            bool issbp = false;

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

                        if (!(isrtcm || issbp))
                            sendData(buffer, (byte) read);
                        

                        // check for valid rtcm packets
                        for (int a = 0; a < read; a++)
                        {
                            int seen = -1;
                            // rtcm
                            if ((seen = rtcm3.Read(buffer[a])) > 0)
                            {
                                isrtcm = true;
                                sendData(rtcm3.packet, (byte)rtcm3.length);
                                if (!msgseen.ContainsKey(seen))
                                    msgseen[seen] = 0;
                                msgseen[seen] = (int)msgseen[seen] + 1;

                                ExtractBasePos(seen);
                            }
                            // sbp
                            if ((seen = sbp.read(buffer[a])) > 0)
                            {
                                issbp = true;
                                sendData(sbp.packet, (byte)sbp.length);
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

        private void ExtractBasePos(int seen)
        {
            try
            {
                if (seen == 1005)
                {
                    var basepos = new Utilities.rtcm3.type1005();
                    basepos.Read(rtcm3.buffer);

                    var pos = basepos.ecefposition;

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0] * Utilities.rtcm3.R2D, baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2]);
                }
                else if (seen == 1006)
                {
                    var basepos = new Utilities.rtcm3.type1006();
                    basepos.Read(rtcm3.buffer);

                    var pos = basepos.ecefposition;

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0], baseposllh[1], baseposllh[2]);
                }
            } catch
            {

            }
        }

        private void sendData(byte[] data, byte length)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var MAV in port.MAVlist)
                {
                    port.InjectGpsData(MAV.sysid, MAV.compid, data, (byte) length, rtcm_msg);
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

        private void chk_rtcmmsg_CheckedChanged(object sender, EventArgs e)
        {
            rtcm_msg = chk_rtcmmsg.Checked;
        }
    }
}