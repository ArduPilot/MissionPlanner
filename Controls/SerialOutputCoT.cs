using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.CoT;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class SerialOutputCoT : Form
    {
        static TcpListener listener;
        static ICommsSerial CoTStream = new SerialPort();
        static int updaterate_ms = 10*1000;
        System.Threading.Thread t12;
        static bool threadrun = false;
        private bool indent;

        public SerialOutputCoT()
        {
            InitializeComponent();

            CMB_serialport.Items.Add("TAK Multicast");
            CMB_serialport.Items.Add("TCP Host - 14551");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14551");
            CMB_serialport.Items.Add("UDP Client");
            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {

            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }

            if (threadrun)
            {
                threadrun = false;
                if (CoTStream != null && CoTStream.IsOpen)
                {
                    CoTStream.Close();
                }
                BUT_connect.Text = Strings.Connect;
                return;
            }

            try
            {
                switch (CMB_serialport.Text)
                {
                    case "TCP Host - 14551":
                    case "TCP Host":
                        CoTStream = new TcpSerial();
                        CMB_baudrate.SelectedIndex = 0;
                        listener = new TcpListener(System.Net.IPAddress.Any, 14551);
                        listener.Start(0);
                        listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                        BUT_connect.Text = Strings.Stop;
                        break;
                    case "TCP Client":
                        CoTStream = new TcpSerial() { retrys = 999999, autoReconnect = true, ConfigRef = "SerialOutputCoTTCP" };
                        CMB_baudrate.SelectedIndex = 0;
                        break;
                    case "UDP Host - 14551":
                        CoTStream = new UdpSerial();
                        CMB_baudrate.SelectedIndex = 0;
                        break;
                    case "UDP Client":
                        CoTStream = new UdpSerialConnect();
                        CMB_baudrate.SelectedIndex = 0;
                        break;
                    case "TAK Multicast":
                        CoTStream = new UdpSerialConnect() { ConfigRef = "TAK_Multicast" };
                        ((UdpSerialConnect)CoTStream).Open("239.2.3.1", "6969");
                        CMB_baudrate.SelectedIndex = 0;
                        break;
                    default:
                        CoTStream = new SerialPort();
                        CoTStream.PortName = CMB_serialport.Text;
                        break;
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                // if we're trying to open a port but another app, like WinTAK, is already running and using the port then our open-port attempt will fail.
                // expected error string: "Only one usage of each socket address (protocol/network address/port) is normally permitted"
                if (ex.Message.StartsWith("Only one usage"))
                    CustomMessageBox.Show("TAK IP Port in use. Is another app on this system already using it?");
                else
                    CustomMessageBox.Show(Strings.InvalidPortName);
                return;
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidPortName);
                return;
            }

            try
            {
                CoTStream.BaudRate = int.Parse(CMB_baudrate.Text);
            }
            catch
            {
                CustomMessageBox.Show(Strings.InvalidBaudRate);
                return;
            }
            try
            {
                if (listener == null)
                    System.Threading.ThreadPool.QueueUserWorkItem(background_DoOpen);
            }
            catch
            {
                CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??");
                return;
            }

            t12 = new System.Threading.Thread(new System.Threading.ThreadStart(mainloop))
            {
                IsBackground = true,
                Name = "CoT output"
            };
            t12.Start();

            BUT_connect.Text = Strings.Stop;
        }

        void background_DoOpen(object state)
        {
            if (CoTStream == null)
            {
                return;
            }

            try
            {
                CoTStream.Open();
            }
            catch { CoTStream = null; } // don't care if we crash
        }

        void mainloop()
        {
            threadrun = true;

            var round = new CotRound();
            while (threadrun)
            {
                try
                {
                    // each vehicle from the connection it was heard on
                    var vehicles = MainV2.Comports.ToArray()
                        .SelectMany(port => port.MAVlist.Select(mav => CotVehicles.Read(mav, port)))
                        .ToList();

                    var events = round.Build(vehicles, IdentityFor, TB_xml_type.Text, CB_advancedMode.Checked,
                        DateTime.UtcNow, CotVehicles.StaleSeconds(updaterate_ms / 1000.0));

                    string view = "";
                    foreach (var cotevent in events)
                    {
                        var xmlStr = CotXml.Serialize(cotevent, indent);
                        view += xmlStr + "\r\n";

                        if (CoTStream != null && CoTStream.IsOpen)
                        {
                            CoTStream.WriteLine(xmlStr.Replace("\r", ""));
                        }
                    }

                    if (TB_output.IsHandleCreated)
                        TB_output.Invoke((Action)delegate
                        {
                            TB_output.Text = view;
                        });

                    Thread.Sleep(updaterate_ms);
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            try
            {
                // End the operation and display the received data on  
                // the console.
                TcpClient client = listener.EndAcceptTcpClient(ar);

                ((TcpSerial)CoTStream).client = client;

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
            }
            catch { }
        }

        CotIdentity IdentityFor(CotVehicle vehicle)
        {
            var uid = FindUIDviaSysid(vehicle.SysId);
            var id = new CotIdentity { Uid = string.IsNullOrEmpty(uid) ? "NOsysid" + vehicle.SysId : uid };

            int row = FindRowviaUID(uid);
            if (row >= 0)
            {
                var takv = myDataGridView1[this.takv.Index, row].Value;
                id.Takv = takv != null && Convert.ToBoolean(takv);

                var callsign = myDataGridView1[this.ContactCallsign.Index, row].Value;
                if (isValidStr(callsign))
                    id.Callsign = callsign.ToString();

                var endpoint = myDataGridView1[this.ContactEndPointIP.Index, row].Value;
                if (isValidStr(endpoint))
                    id.Endpoint = endpoint.ToString();

                var vmf = myDataGridView1[this.VMF.Index, row].Value;
                if (isValidStr(vmf))
                    id.Vmf = vmf.ToString();
            }

            return id;
        }

        int FindRowviaUID(string uid)
        {
            if (uid == null || uid.Length <= 0)
                return -1;

            var rcnt = myDataGridView1.Rows.Count;
            for (int x = 0; x < rcnt - 1; x++)
                if (myDataGridView1[this.UID.Index, x].Value?.ToString() == uid)
                    return x;

            return -1;
        }

        string FindUIDviaSysid(uint sysid)
        {
            var rcnt = myDataGridView1.Rows.Count;
            for (int x = 0; x < rcnt - 1; x++)
                if (myDataGridView1[this.sysid.Index, x].Value?.ToString() == sysid.ToString())
                    return myDataGridView1[this.UID.Index, x].Value?.ToString();

            return "NOsysid" + sysid;
        }

        bool isValidStr(object obj)
        {
            return (obj != null && obj.ToString().Length > 0);
        }

        private void BTN_clear_TB_Click(object sender, EventArgs e)
        {
            TB_output.Text = "";
        }

        private void SerialOutputCoT_Load(object sender, EventArgs e)
        {
            try
            {
                // this can crash if you're connecting as you're loading the screen so the row count may change as you're populating it.
                myDataGridView1.Deserialize(Settings.Instance["CoTUID"]);
            } catch { }


            updateRate_numericUpDown.Value = Settings.Instance.GetDecimal("CoT_updateRate", 10);
            CMB_serialport.SelectedIndex = Settings.Instance.GetInt32("CoT_CMB_serialport", 0);
            CMB_baudrate.SelectedIndex = Settings.Instance.GetInt32("CoT_CMB_baudrate", 0);
            CB_advancedMode.Checked = Settings.Instance.GetBoolean("CoT_CB_advancedMode", true);
            chk_indent.Checked = Settings.Instance.GetBoolean("CoT_chk_indent", true);

            CB_advancedMode_CheckedChanged(null, null);
            chk_indent_CheckedChanged(null, null);
            updateRate_numericUpDown_ValueChanged(null, null);
            CMB_serialport_SelectedIndexChanged(null, null);

            if (threadrun)
            {
                // stop
                BUT_connect_Click(null, null);

                // restart
                BUT_connect_Click(null, null);
            }

        }

        private void SerialOutputCoT_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Instance["CoT_updateRate"] = updateRate_numericUpDown.Value.ToString();
            Settings.Instance["CoT_CMB_serialport"] = CMB_serialport.SelectedIndex.ToString();
            Settings.Instance["CoT_CMB_baudrate"] = CMB_baudrate.SelectedIndex.ToString();
            Settings.Instance["CoT_CB_advancedMode"] = CB_advancedMode.Checked.ToString();
            Settings.Instance["CoT_chk_indent"] = chk_indent.Checked.ToString();
        }

        private void myDataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            Settings.Instance["CoTUID"] = myDataGridView1.Serialize();
        }

        private void CB_advancedMode_CheckedChanged(object sender, EventArgs e)
        {
            for (int col = 0; col < myDataGridView1.Columns.Count; col++)
            {
                if (col != this.sysid.Index && col != this.UID.Index)
                {
                    myDataGridView1.Columns[col].Visible = CB_advancedMode.Checked;
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

        private void chk_indent_CheckedChanged(object sender, EventArgs e)
        {
            indent = chk_indent.Checked;
        }

        private void myDataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            // End of edition on each click on column of checkbox
            if (e.ColumnIndex == this.takv.Index && e.RowIndex != -1)
            {
                // refresh
                myDataGridView1.EndEdit();
            }
        }

        private void myDataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            myDataGridView1.EndEdit();
        }

        private void updateRate_numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            updaterate_ms = (int)(updateRate_numericUpDown.Value * 1000);
        }
    }
}