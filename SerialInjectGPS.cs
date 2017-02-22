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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MissionPlanner
{
    public partial class SerialInjectGPS : UserControl, IActivate, IDeactivate
    {
        private static ILog log = LogManager.GetLogger(typeof (SerialInjectGPS).FullName);

        // serialport
        internal static ICommsSerial comPort = new SerialPort();
        // rtcm detection
        private static Utilities.rtcm3 rtcm3 = new Utilities.rtcm3();
        // sbp detection
        private static Utilities.sbp sbp = new Utilities.sbp();
        // ubx detection
        private static Utilities.ubx_m8p ubx_m8p = new Utilities.ubx_m8p();
        // background thread 
        private static System.Threading.Thread t12;
        private static bool threadrun = false;
        // track rtcm msg's seen
        private static Hashtable msgseen = new Hashtable();
        // track bytes seen
        private static int bytes = 0;
        private static int bps = 0;
        private static int bpsusefull = 0;

        private static bool rtcm_msg = true;

        private static SerialInjectGPS Instance;

        private PointLatLngAlt basepos = PointLatLngAlt.Zero;

        [XmlElement(ElementName = "baseposList")]
        List<PointLatLngAlt> baseposList = new List<PointLatLngAlt>();

        static private BinaryWriter basedata;

        // Thread signal. 
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);
        private static string status_line3;

        private string basepostlistfile = Settings.GetUserDataDirectory() + Path.DirectorySeparatorChar +
                                          "baseposlist.xml";

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

            // restore current static state
            chk_rtcmmsg.Checked = rtcm_msg;

            // restore setting
            if(Settings.Instance.ContainsKey("SerialInjectGPS_m8pautoconfig"))
                chk_m8pautoconfig.Checked = bool.Parse(Settings.Instance["SerialInjectGPS_m8pautoconfig"]);

            if (Settings.Instance.ContainsKey("SerialInjectGPS_m8p_130p"))
                chk_m8p_130p.Checked = bool.Parse(Settings.Instance["SerialInjectGPS_m8p_130p"]);

            loadBasePosList();

            loadBasePOS();

            rtcm3.ObsMessage += Rtcm3_ObsMessage;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void Rtcm3_ObsMessage(object sender, EventArgs e)
        {
            MainV2.instance.BeginInvoke((MethodInvoker) delegate
            {
                List<rtcm3.ob> obs = sender as List<rtcm3.ob>;

                // get system controls
                Func<char,List<VerticalProgressBar2>> ctls = delegate (char sys)
                {
                    return panel1.Controls.OfType<VerticalProgressBar2>()
                        .Where(ctl => { return ctl.Label.StartsWith(sys + ""); }).ToList();
                };

                // we need more ctls for this system
                while (ctls.Invoke(obs[0].sys).Count() < obs.Count)
                    panel1.Controls.Add(new VerticalProgressBar2()
                    {
                        Height = panel1.Height - 30,
                        Label = obs[0].sys + ""
                    });

                // we need to remove ctls for this system
                while (ctls.Invoke(obs[0].sys).Count() > obs.Count)
                {
                    panel1.Controls.Remove(ctls.Invoke(obs[0].sys).First());
                }

                int width = panel1.Width/panel1.Controls.OfType<VerticalProgressBar2>().Count();

                var tmp = ctls('G');
                var tmp2 = ctls('R');

                // if G 0, if R = G.count (2 system support)
                var a = obs[0].sys == 'G' ? 0 : ctls.Invoke('G').Count;

                var sysctls = ctls.Invoke(obs[0].sys);
                var cnt = 0;
                foreach (var ob in obs)
                {
                    var vpb = sysctls[cnt];
                    vpb.Value = (int) ob.snr;
                    //vpb.Text = ob.snr.ToString();
                    vpb.Label = ob.sys + ob.prn.ToString();
                    vpb.Location = new Point(width*(a + cnt), 0);
                    vpb.DrawLabel = true;
                    vpb.Width = width;
                    vpb.Height = panel1.Height-30;
                    vpb.Minimum = 25;
                    vpb.Maximum = 55;
                    vpb.minline = 30;
                    vpb.maxline = 99;
                    cnt++;
                }

                ThemeManager.ApplyThemeTo(panel1);
            }
            );
        }

        ~SerialInjectGPS()
        {
            log.Info("destroy");
        }

        void loadBasePosList()
        {
            if (File.Exists(basepostlistfile))
            {
                //load config
                System.Xml.Serialization.XmlSerializer reader =
                    new System.Xml.Serialization.XmlSerializer(typeof (List<PointLatLngAlt>), new Type[] { typeof(Color) });

                using (StreamReader sr = new StreamReader(basepostlistfile))
                {
                    try
                    {
                        baseposList = (List<PointLatLngAlt>) reader.Deserialize(sr);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        CustomMessageBox.Show("Failed to load Base Position List\n" + ex.ToString(), Strings.ERROR);
                    }
                }
            }

            updateBasePosDG();
        }

        void saveBasePosList()
        {
            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<PointLatLngAlt>), new Type[] { typeof(Color) });

            using (StreamWriter sw = new StreamWriter(basepostlistfile))
            {
                writer.Serialize(sw, baseposList);
            }
        }

        public new void Show()
        {
            this.ShowUserControl();
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            threadrun = false;
            if (comPort.IsOpen)
            {
                threadrun = false;
                groupBox1.Enabled = true;
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
                groupBox1.Enabled = false;

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
                        double.Parse(txt_surveyinAcc.Text, CultureInfo.InvariantCulture), chk_m8p_130p.Checked);

                    this.LogInfo("Setup M8P done");
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

        private static void updateSVINLabel(string label, string line2 = "")
        {
            if (!Instance.IsDisposed)
            {
                Instance.BeginInvoke(
                    (MethodInvoker)
                        delegate
                        {
                            Instance.lbl_svin.Text = label + DateTime.Now.ToString(" - HH:mm:ss") + '\n' + line2;
                        }
                    );
            }
        }

        private static void mainloop()
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
                            if (comPort is CommsNTRIP || comPort is UdpSerialConnect || comPort is UdpSerial)
                            {

                            }
                            else
                            {
                                log.Warn("Reconnecting");
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

                        try
                        {
                            if (basedata != null)
                                basedata.Write(buffer, 0, read);
                        }
                        catch
                        {
                        }

                        // if this is raw data transport of unknown packet types
                        if (!(isrtcm || issbp))
                            sendData(buffer, (byte) read);

                        // check for valid rtcm/sbp/ubx packets
                        for (int a = 0; a < read; a++)
                        {
                            int seenmsg = -1;
                            // rtcm
                            if ((seenmsg = rtcm3.Read(buffer[a])) > 0)
                            {
                                isrtcm = true;
                                sendData(rtcm3.packet, (byte) rtcm3.length);
                                bpsusefull += rtcm3.length;
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
                                bpsusefull += sbp.length;
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
                    log.Error(ex);
                }
            }
        }

        private static void ProcessUBXMessage()
        {
            try
            {
                // survey in
                if (ubx_m8p.@class == 0x1 && ubx_m8p.subclass == 0x3b)
                {
                    var svin = ubx_m8p.packet.ByteArrayToStructure<Utilities.ubx_m8p.ubx_nav_svin>(6);

                    updateSVINLabel("Survey IN Valid: " + (svin.valid == 1) + " InProgress: " + (svin.active == 1) +
                                    " Duration: " + svin.dur + " Obs: " + svin.obs + " Acc: " + svin.meanAcc/10000.0);

                    var X = svin.meanX/100.0 + svin.meanXHP*0.0001;
                    var Y = svin.meanY/100.0 + svin.meanYHP*0.0001;
                    var Z = svin.meanZ/100.0 + svin.meanZHP*0.0001;

                    if (X == 0 || Y == 0 || Z == 0)
                        return;

                    var pos = new double[] {X, Y, Z};

                    double[] baseposllh = new double[3];

                    Utilities.rtcm3.ecef2pos(pos, ref baseposllh);

                    //MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(baseposllh[0]*Utilities.rtcm3.R2D,
                    //baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2]);

                    //if (svin.valid == 1)
                    //ubx_m8p.turnon_off(comPort, 0x1, 0x3b, 0);
                }
                else if (ubx_m8p.@class == 0x1 && ubx_m8p.subclass == 0x7)
                {
                    var pvt = ubx_m8p.packet.ByteArrayToStructure<Utilities.ubx_m8p.ubx_nav_pvt>(6);

                    //MainV2.comPort.MAV.cs.MovingBase = new Utilities.PointLatLngAlt(pvt.lat / 1e7, pvt.lon / 1e7, pvt.height / 1000.0);


                }
                else if (ubx_m8p.@class == 0x5 && ubx_m8p.subclass == 0x1)
                {
                    log.InfoFormat("ubx ack {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);
                }
                else if (ubx_m8p.@class == 0x5 && ubx_m8p.subclass == 0x0)
                {
                    log.InfoFormat("ubx Nack {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);
                }
                else if (ubx_m8p.@class == 0xa && ubx_m8p.subclass == 0x4)
                {
                    log.InfoFormat("ubx mon-ver {0} {1}", ubx_m8p.packet[6], ubx_m8p.packet[7]);
                }
                else if (ubx_m8p.@class == 0xf5)
                {
                    // rtcm
                }
                else if (ubx_m8p.@class == 0x02)
                {
                    // rxm-raw
                }
                else
                {
                    ubx_m8p.turnon_off(comPort, ubx_m8p.@class, ubx_m8p.subclass, 0);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void ExtractBasePos(int seen)
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
                        (String.Format("RTCM Base {0} {1} {2} - {3}", baseposllh[0]*Utilities.rtcm3.R2D,
                            baseposllh[1]*Utilities.rtcm3.R2D, baseposllh[2], DateTime.Now.ToString("HH:mm:ss")));

                    if (!Instance.IsDisposed && Instance.but_save_basepos.Enabled == false)
                        Instance.but_save_basepos.Enabled = true;
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
                       (String.Format("RTCM Base {0} {1} {2} - {3}", baseposllh[0] * Utilities.rtcm3.R2D,
                           baseposllh[1] * Utilities.rtcm3.R2D, baseposllh[2], DateTime.Now.ToString("HH:mm:ss")));

                    if (!Instance.IsDisposed && Instance.but_save_basepos.Enabled == false)
                        Instance.but_save_basepos.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void sendData(byte[] data, byte length)
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

            updateLabel(String.Format("{0,10} bytes {1,10} bps {2,10} bps sent", bytes, bps, bpsusefull), sb.ToString(), status_line3);
            bps = 0;
            bpsusefull = 0;

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

                log.Info("basepos: "+ Settings.Instance["base_pos"].ToString());

                basepos = new PointLatLngAlt(double.Parse(bspos[0], CultureInfo.InvariantCulture),
                    double.Parse(bspos[1], CultureInfo.InvariantCulture),
                    double.Parse(bspos[2], CultureInfo.InvariantCulture), 
                    bspos[3]);
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

                baseposList.Add(new PointLatLngAlt(this.basepos));

                updateBasePosDG();
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

                baseposList.Add(new PointLatLngAlt(basepos) {Tag = location});

                updateBasePosDG();
            }
        }

        private void chk_m8pautoconfig_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["SerialInjectGPS_m8pautoconfig"] = chk_m8pautoconfig.Checked.ToString();

            if(chk_m8pautoconfig.Checked)
                dg_basepos.Enabled = true;
            else
                dg_basepos.Enabled = false;
        }

        void updateBasePosDG()
        {
            if (baseposList.Count == 0)
                return;

            //dont trigger on clear
            dg_basepos.RowsRemoved -= dg_basepos_RowsRemoved;
            dg_basepos.Rows.Clear();
            dg_basepos.RowsRemoved += dg_basepos_RowsRemoved;

            foreach (var pointLatLngAlt in baseposList)
            {
                dg_basepos.Rows.Add(pointLatLngAlt.Lat, pointLatLngAlt.Lng, pointLatLngAlt.Alt, pointLatLngAlt.Tag);
            }

            saveBasePosList();
        }

        private void dg_basepos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Use.Index)
            {
                Settings.Instance["base_pos"] = String.Format("{0},{1},{2},{3}",
                    dg_basepos[Lat.Index, e.RowIndex].Value,
                    dg_basepos[Long.Index, e.RowIndex].Value,
                    dg_basepos[Alt.Index, e.RowIndex].Value,
                    dg_basepos[BaseName1.Index, e.RowIndex].Value);

                loadBasePOS();
            }
        }

        private void dg_basepos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            while (baseposList.Count <= e.RowIndex)
                baseposList.Add(new PointLatLngAlt());

            if (e.ColumnIndex == Lat.Index)
            {
                baseposList[e.RowIndex].Lat = double.Parse(dg_basepos[e.ColumnIndex, e.RowIndex].Value.ToString());
            }
            if (e.ColumnIndex == Long.Index)
            {
                baseposList[e.RowIndex].Lng = double.Parse(dg_basepos[e.ColumnIndex, e.RowIndex].Value.ToString());
            }
            if (e.ColumnIndex == Alt.Index)
            {
                baseposList[e.RowIndex].Alt = double.Parse(dg_basepos[e.ColumnIndex, e.RowIndex].Value.ToString());
            }
            if (e.ColumnIndex == BaseName1.Index)
            {
                baseposList[e.RowIndex].Tag = dg_basepos[e.ColumnIndex, e.RowIndex].Value.ToString();
            }

            saveBasePosList();
        }

        private void dg_basepos_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            baseposList.RemoveAt(e.RowIndex);

            saveBasePosList();
        }

        private void chk_m8p_130p_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Instance["SerialInjectGPS_m8p_130p"] = chk_m8p_130p.Checked.ToString();
        }
    }
}