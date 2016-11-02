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
using System.Runtime.InteropServices;

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
        // ubx detection
        private Utilities.ubx_m8p ubx_m8p = new Utilities.ubx_m8p();
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
                if (chk_m8pautoconfig.Checked)
                {
                    ubx_m8p.SetupM8P(comPort);
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

        private void updateSVINLabel(string label)
        {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            this.lbl_svin.Text = label;
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

                    // limit to 180 byte packet if using new packet
                    if (rtcm_msg)
                        buffer = new byte[180];

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
                            // ubx
                            if ((seen = ubx_m8p.Read(buffer[a])) > 0)
                            {
                                ProcessUBXMessage();
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

        private void ProcessUBXMessage()
        {
            try
            {
                // survey in
                if (ubx_m8p.@class == 0x1 && ubx_m8p.subclass == 0x3b)
                {
                    var svin = ubx_m8p.packet.ByteArrayToStructure<Utilities.ubx_m8p.ubx_nav_svin>(6);

                    var X = svin.meanX / 100.0 + svin.meanXHP * 0.0001;
                    var Y = svin.meanY / 100.0 + svin.meanYHP * 0.0001;
                    var Z = svin.meanZ / 100.0 + svin.meanZHP * 0.0001;

                    var pos = new double[] { X, Y, Z };

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0] * Utilities.rtcm3.R2D, baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2]);

                    updateSVINLabel("Survey IN Valid: " + (svin.valid == 1) + " InProgress: " + (svin.active == 1) + " Duration: " + svin.dur + " Obs: " + svin.obs + " Acc: " + svin.meanAcc / 10000.0);
                }

                //pvt
                if (ubx_m8p.@class == 0x1 && ubx_m8p.subclass == 0x7)
                {
                    var pvt = ubx_m8p.packet.ByteArrayToStructure<Utilities.ubx_m8p.ubx_nav_pvt>(6);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(pvt.lat / 1e7, pvt.lon / 1e7, pvt.h_msl / 1000.0);
                }
            }
            catch
            {

            }
        }

        private void ExtractBasePos(int seen)
        {
            try
            {
                if (seen == 1005)
                {
                    var basepos = new Utilities.rtcm3.type1005();
                    basepos.Read(rtcm3.packet);

                    var pos = basepos.ecefposition;

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0] * Utilities.rtcm3.R2D, baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2]);

                    updateSVINLabel(String.Format("RTCM Base {0} {1} {2}", baseposllh[0] * Utilities.rtcm3.R2D, baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2]));
                }
                else if (seen == 1006)
                {
                    var basepos = new Utilities.rtcm3.type1006();
                    basepos.Read(rtcm3.packet);

                    var pos = basepos.ecefposition;

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0], baseposllh[1], baseposllh[2]);

                    updateSVINLabel(String.Format("RTCM Base {0} {1} {2}", baseposllh[0] * Utilities.rtcm3.R2D, baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2]));
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

        private void chk_m8pautoconfig_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}