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
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Globalization;
using System.IO;

namespace MissionPlanner
{
    public partial class SerialInjectGPS : UserControl, IActivate, IDeactivate
    {
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
        private static Hashtable msgseen = new Hashtable();
        // track bytes seen
        private static int bytes = 0;
        private static int bps = 0;

        private static bool rtcm_msg = true;

        private static SerialInjectGPS Instance;

        private PointLatLngAlt basepos = PointLatLngAlt.Zero;

        static private BinaryWriter basedata;

        // Thread signal. 
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private static string status_line3;

        public SerialInjectGPS()
        {
            InitializeComponent();

            Instance = this;

            status_line3 = null;

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("UDP Host");
            CMB_serialport.Items.Add("UDP Client");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("NTRIP");

            if (threadrun)
            {
                BUT_connect.Text = Strings.Stop;
            }

            // restore last port and baud - its the simple things that make life better
            if (Settings.Instance.ContainsKey("SerialInjectGPS_port"))
            {
                CMB_serialport.Text = Settings.Instance["SerialInjectGPS_port"];
            }
            if (Settings.Instance.ContainsKey("SerialInjectGPS_baud"))
            {
                CMB_baudrate.Text = Settings.Instance["SerialInjectGPS_baud"];
            }

            chk_rtcmmsg.Checked = rtcm_msg;

            loadBasePOS();

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            threadrun = false;
            if (comPort.IsOpen)
            {
                threadrun = false;
                comPort.Close();
                BUT_connect.Text = Strings.Connect;
                try
                {
                    basedata.Close();

                    basedata = null;
                }
                catch
                {
                }
            }
            else
            {
                status_line3 = null;

                try
                {
                    switch (CMB_serialport.Text)
                    {
                        case "NTRIP":
                            comPort = new CommsNTRIP();
                            CMB_baudrate.SelectedIndex = 0;
                            ((CommsNTRIP) comPort).lat = MainV2.comPort.MAV.cs.HomeLocation.Lat;
                            ((CommsNTRIP) comPort).lng = MainV2.comPort.MAV.cs.HomeLocation.Lng;
                            ((CommsNTRIP) comPort).alt = MainV2.comPort.MAV.cs.HomeLocation.Alt;
                            chk_m8pautoconfig.Checked = false;
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

                    Settings.Instance["SerialInjectGPS_port"] = CMB_serialport.Text;
                    Settings.Instance["SerialInjectGPS_baud"] = CMB_baudrate.Text;
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
                    comPort.ReadBufferSize = 1024*64;

                    comPort.Open();

                    try
                    {
                        basedata = new BinaryWriter(new BufferedStream(
                            File.Open(
                                Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".gpsbase", FileMode.CreateNew,
                                FileAccess.ReadWrite, FileShare.None)));
                    }
                    catch (Exception ex2)
                    {
                        CustomMessageBox.Show("Error creating file to save base data into " + ex2.ToString());
                    }
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
                    this.LogInfo("Setup M8P");
                    ubx_m8p.SetupM8P(comPort, basepos, int.Parse(txt_surveyinDur.Text, CultureInfo.InvariantCulture),
                        double.Parse(txt_surveyinAcc.Text, CultureInfo.InvariantCulture));
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

        private void updateLabel(string line1, string line2, string line3)
        {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            this.lbl_status.Text = line1 + '\n' + line2 + '\n' + line3;
                        }
                    );
            }
        }

        private void updateSVINLabel(string label, string line2 = "")
        {
            if (!this.IsDisposed)
            {
                this.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            this.lbl_svin.Text = label + '\n' + line2;
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

            int reconnecttimeout = 10;

            while (threadrun)
            {
                try
                {
                    // reconnect logic - 10 seconds with no data, or comport is closed
                    try
                    {
                        if ((DateTime.Now - lastrecv).TotalSeconds > reconnecttimeout || !comPort.IsOpen)
                        {
                            if (comPort is CommsNTRIP)
                            {

                            }
                            else
                            {
                                this.LogInfo("Reconnecting");
                                // close existing
                                comPort.Close();
                                // reopen
                                comPort.Open();
                            }
                            // reset timer
                            lastrecv = DateTime.Now;
                        }
                    }
                    catch
                    {
                        this.LogError("Failed to reconnect");
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

                        try
                        {
                            if (basedata != null)
                                basedata.Write(buffer, 0, read);
                        }
                        catch
                        {
                        }

                        if (!(isrtcm || issbp))
                            sendData(buffer, (byte) read);


                        // check for valid rtcm packets
                        for (int a = 0; a < read; a++)
                        {
                            int seenmsg = -1;
                            // rtcm
                            if ((seenmsg = rtcm3.Read(buffer[a])) > 0)
                            {
                                isrtcm = true;
                                sendData(rtcm3.packet, (byte) rtcm3.length);
                                string msgname = "Rtcm" + seenmsg;
                                if (!msgseen.ContainsKey(msgname))
                                    msgseen[msgname] = 0;
                                msgseen[msgname] = (int) msgseen[msgname] + 1;

                                ExtractBasePos(seenmsg);
                            }
                            // sbp
                            if ((seenmsg = sbp.read(buffer[a])) > 0)
                            {
                                issbp = true;
                                sendData(sbp.packet, (byte) sbp.length);
                                string msgname = "Sbp" + seenmsg.ToString("X4");
                                if (!msgseen.ContainsKey(msgname))
                                    msgseen[msgname] = 0;
                                msgseen[msgname] = (int) msgseen[msgname] + 1;
                            }
                            // ubx
                            if ((seenmsg = ubx_m8p.Read(buffer[a])) > 0)
                            {
                                ProcessUBXMessage();
                                string msgname = "Ubx" + seenmsg.ToString("X4");
                                if (!msgseen.ContainsKey(msgname))
                                    msgseen[msgname] = 0;
                                msgseen[msgname] = (int) msgseen[msgname] + 1;
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    this.LogError(ex);
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

                    var X = svin.meanX/100.0 + svin.meanXHP*0.0001;
                    var Y = svin.meanY/100.0 + svin.meanYHP*0.0001;
                    var Z = svin.meanZ/100.0 + svin.meanZHP*0.0001;

                    var pos = new double[] {X, Y, Z};

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0]*Utilities.rtcm3.R2D,
                        baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2]);

                    updateSVINLabel("Survey IN Valid: " + (svin.valid == 1) + " InProgress: " + (svin.active == 1) +
                                    " Duration: " + svin.dur + " Obs: " + svin.obs + " Acc: " + svin.meanAcc/10000.0);

                    if (svin.valid == 1)
                        ubx_m8p.turnon_off(comPort, 0x1, 0x3b, 0);
                }

                //pvt
                if (ubx_m8p.@class == 0x1 && ubx_m8p.subclass == 0x7)
                {
                    var pvt = ubx_m8p.packet.ByteArrayToStructure<Utilities.ubx_m8p.ubx_nav_pvt>(6);

                    //MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(pvt.lat / 1e7, pvt.lon / 1e7, pvt.height / 1000.0);


                }

                if (ubx_m8p.@class == 0x5 && ubx_m8p.subclass == 0x1)
                {
                    this.LogInfoFormat("ubx ack {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);

                }

                if (ubx_m8p.@class == 0x5 && ubx_m8p.subclass == 0x0)
                {
                    this.LogInfoFormat("ubx Nack {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);

                }

                if (ubx_m8p.@class == 0xa && ubx_m8p.subclass == 0x4)
                {
                    this.LogInfoFormat("ubx mon-ver {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);

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

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0]*Utilities.rtcm3.R2D,
                        baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2]);

                    status_line3 =
                        (String.Format("RTCM Base {0} {1} {2}", baseposllh[0]*Utilities.rtcm3.R2D,
                            baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2]));

                    if (!Instance.IsDisposed && but_save_basepos.Enabled == false)
                        but_save_basepos.Enabled = true;
                }
                else if (seen == 1006)
                {
                    var basepos = new Utilities.rtcm3.type1006();
                    basepos.Read(rtcm3.packet);

                    var pos = basepos.ecefposition;

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0], baseposllh[1],
                        baseposllh[2]);

                    status_line3 =
                        (String.Format("RTCM Base {0} {1} {2}", baseposllh[0]*Utilities.rtcm3.R2D,
                            baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2]));

                    if (!Instance.IsDisposed && but_save_basepos.Enabled == false)
                        but_save_basepos.Enabled = true;
                }
            }
            catch
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

            updateLabel("bytes " + bytes + " bps " + bps, sb.ToString(), status_line3);
            bps = 0;

            try
            {
                if (basedata != null)
                    basedata.Flush();
            }
            catch
            {
                basedata = null;
            }
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        private void chk_rtcmmsg_CheckedChanged(object sender, EventArgs e)
        {
            rtcm_msg = chk_rtcmmsg.Checked;
        }

        private void loadBasePOS()
        {
            try
            {
                string[] bspos = Settings.Instance["base_pos"].Split(',');

                basepos = new PointLatLngAlt(double.Parse(bspos[0], CultureInfo.InvariantCulture),
                    double.Parse(bspos[1], CultureInfo.InvariantCulture),
                    double.Parse(bspos[2], CultureInfo.InvariantCulture));
            }
            catch
            {
                basepos = PointLatLngAlt.Zero;
            }
        }

        private void but_base_pos_Click(object sender, EventArgs e)
        {
            string basepos = Settings.Instance["base_pos"];
            if (InputBox.Show("Base POS", "Please enter base pos location 'lat,lng,alt,name'", ref basepos) ==
                DialogResult.OK)
            {
                Settings.Instance["base_pos"] = basepos;

                loadBasePOS();
            }
            else
            {
                this.basepos = PointLatLngAlt.Zero;
            }
        }

        private void but_save_basepos_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MAV.cs.MovingBase == null)
            {
                CustomMessageBox.Show("No valid base position determined by gps yet", Strings.ERROR);
                return;
            }

            string location = "";
            if (InputBox.Show("Enter Location", "Enter a friendly name for this location.", ref location) ==
                DialogResult.OK)
            {
                var basepos = MainV2.comPort.MAV.cs.MovingBase;
                Settings.Instance["base_pos"] = String.Format("{0},{1},{2},{3}", basepos.Lat.ToString(CultureInfo.InvariantCulture), basepos.Lng.ToString(CultureInfo.InvariantCulture), basepos.Alt.ToString(CultureInfo.InvariantCulture),
                    location);
            }
        }
    }
}