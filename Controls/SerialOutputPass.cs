using DeviceProgramming.Dfu;
using Microsoft.Scripting.Utils;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class SerialOutputPass : Form
    {
        static TcpListener listener;
        // Thread signal. 
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public SerialOutputPass()
        {
            InitializeComponent();

            Go.DefaultCellStyle.NullValue = "Go";
            myDataGridView1.UserDeletingRow += myDataGridView1_UserDeletingRow;
            myDataGridView1.UserDeletedRow += myDataGridView1_UserDeletedRow;

            chk_write.Checked = MainV2.comPort.MirrorStreamWrite;

            CMB_serialport.Items.AddRange(SerialPort.GetPortNames());
            CMB_serialport.Items.Add("TCP Host - 14550");
            CMB_serialport.Items.Add("TCP Client");
            CMB_serialport.Items.Add("UDP Host - 14550");
            CMB_serialport.Items.Add("UDP Client");

            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen || listener != null)
            {
                BUT_connect.Text = Strings.Stop;
            }

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);

            try
            {
                Load();
            }
            catch (Exception ex) {
                CustomMessageBox.Show("Failed to load list: " + ex.Message);
            }
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.MirrorStream != null && MainV2.comPort.MirrorStream.IsOpen || listener != null)
            {
                // stop the listener first, so it cannot attach a new client to the closed stream
                listener?.Stop();
                listener = null;
                MainV2.comPort.MirrorStream?.Close();
                BUT_connect.Text = Strings.Connect;
            }
            else
            {
                try
                {
                    switch (CMB_serialport.Text)
                    {
                        case "TCP Host - 14550":
                        case "TCP Host":
                            {
                                var stream = new TcpSerial();
                                MainV2.comPort.MirrorStream = stream;
                                CMB_baudrate.SelectedIndex = 0;
                                int port = 14550;
                                if (InputBox.Show("Port", "Enter port", ref port) != DialogResult.OK)
                                    return;
                                listener = new TcpListener(System.Net.IPAddress.Any, port);
                                listener.Start(0);
                                // the callback only needs the stream to hand the accepted client to
                                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback),
                                    (listener, stream));
                                BUT_connect.Text = Strings.Stop;
                                return;
                            }

                        case "TCP Client":

                            MainV2.comPort.MirrorStream = new TcpSerial() { retrys = 999999, autoReconnect = true, ConfigRef = "SerialOutputPassTCP" };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        case "UDP Host - 14550":
                            {
                                int port = 14550;
                                if (InputBox.Show("Port", "Enter port", ref port) != DialogResult.OK)
                                    return;
                                MainV2.comPort.MirrorStream = new UdpSerial()
                                { ConfigRef = "SerialOutputPassUDP", Port = port.ToString() };
                                CMB_baudrate.SelectedIndex = 0;
                                break;
                            }

                        case "UDP Client":
                            MainV2.comPort.MirrorStream = new UdpSerialConnect() { ConfigRef = "SerialOutputPassUDPCL" };
                            CMB_baudrate.SelectedIndex = 0;
                            break;
                        default:
                            MainV2.comPort.MirrorStream = new SerialPort();
                            MainV2.comPort.MirrorStream.PortName = CMB_serialport.Text;
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
                    MainV2.comPort.MirrorStream.BaudRate = int.Parse(CMB_baudrate.Text);
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate);
                    return;
                }
                try
                {
                    MainV2.comPort.MirrorStream.Open();
                }
                catch
                {
                    CustomMessageBox.Show("Error Connecting\nif using com0com please rename the ports to COM??");
                    return;
                }
            }
        }

        void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // the listener that accepted the client and the stream the client is handed to
            var state = (ValueTuple<TcpListener, TcpSerial>)ar.AsyncState;
            TcpListener listener = state.Item1;
            TcpSerial tcp = state.Item2;

            TcpClient client = null;
            try
            {
                client = listener.EndAcceptTcpClient(ar);

                var previous = tcp.client;
                tcp.client = client;
                previous?.Close();

                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), state);
            }
            catch (Exception ex) when (ex is ObjectDisposedException || ex is SocketException || ex is InvalidOperationException)
            {
                // listener was stopped. StopMirror stops the listener before closing the stream, so a client
                // accepted while stopping may have been set after the close and has to be closed here
                client?.Close();
            }
        }

        private void chk_write_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.comPort.MirrorStreamWrite = chk_write.Checked;
        }


        private void myDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Write is not part of the row key, so it can be changed on a running mirror
            if (e.ColumnIndex == Write.Index && e.RowIndex >= 0 &&
                Active.TryGetValue(RowKey(e.RowIndex), out var active))
                active.Mirror.MirrorStreamWrite =
                    Convert.ToBoolean(myDataGridView1[Write.Index, e.RowIndex].Value ?? false);

            Save();
            UpdateRowStates();
        }

        private void Save() 
        {
            List<string> ans = new List<string>();
            myDataGridView1.Rows.ForEach<DataGridViewRow>(x => 
            {
                if (x.IsNewRow)
                    return;

                // the Go/Stop text is state, not a setting
                var line = x.Cells.Select(i =>
                        ((DataGridViewCell)i).ColumnIndex == Go.Index ? "Go" : ((DataGridViewCell)i).FormattedValue)
                    .ToJSON(Formatting.None);
                ans.Add(line);
            });

            // SetList ignores an empty list, so deleting the last row has to remove the key
            if (ans.Count == 0)
                Settings.Instance.Remove(configlist);
            else
                Settings.Instance.SetList(configlist, ans);
        }

        private void Load()
        {
            var ans = Settings.Instance.GetList(configlist);

            foreach (string row in ans)
            {
                if (row == null || row == "")
                    continue;
                var data = ((JArray)JsonConvert.DeserializeObject(row)).Select(a => ((JValue)a).Value).ToArray();
                myDataGridView1.Rows.Add(data);
            }

            UpdateRowStates();
        }

        string configlist = "serialpasslist";

        class ActiveMirror
        {
            public MAVLinkInterface ComPort;
            public MAVLinkInterface.Mirror Mirror;
            public TcpListener Listener;
        }

        // keyed by the row settings, not the row index: rows are sorted, deleted and reloaded when the form reopens
        static private Dictionary<string, ActiveMirror> Active = new Dictionary<string, ActiveMirror>();

        private string CellText(DataGridViewColumn column, int rowIndex)
        {
            return myDataGridView1[column.Index, rowIndex].Value?.ToString().Trim() ?? "";
        }

        private string RowKey(int rowIndex)
        {
            return string.Join("|", CellText(Type, rowIndex), CellText(Direction, rowIndex), CellText(Port, rowIndex),
                CellText(Extra, rowIndex));
        }

        private void UpdateRowStates()
        {
            foreach (DataGridViewRow row in myDataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                var running = Active.ContainsKey(RowKey(row.Index));
                row.Cells[Go.Index].Value = running ? "Stop" : "Go";

                // a running row must keep its key, otherwise it can no longer be stopped
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ColumnIndex != Go.Index && cell.ColumnIndex != Write.Index)
                        cell.ReadOnly = running;
                }
            }
        }

        private void myDataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void myDataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var key = RowKey(e.Row.Index);
            if (Active.ContainsKey(key))
            {
                StopMirror(key);
                UpdateRowStates();
            }
        }

        private void myDataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            Save();
        }

        private void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != Go.Index || e.RowIndex < 0 || myDataGridView1.Rows[e.RowIndex].IsNewRow)
                return;

            var key = RowKey(e.RowIndex);

            try
            {
                if (Active.ContainsKey(key))
                    StopMirror(key);
                else
                    StartMirror(e.RowIndex, key);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error: " + ex.Message);
            }

            UpdateRowStates();
        }

        private void StartMirror(int rowIndex, string key)
        {
            var protocol = CellText(Type, rowIndex);
            var direction = CellText(Direction, rowIndex);
            var port = CellText(Port, rowIndex);
            var extra = CellText(Extra, rowIndex);
            var write = Convert.ToBoolean(myDataGridView1[Write.Index, rowIndex].Value ?? false);

            MAVLinkInterface.Mirror mirror = new MAVLinkInterface.Mirror() { MirrorStreamWrite = write };
            TcpListener tcpListener = null;

            if (protocol == "TCP")
            {
                if (direction == "Inbound")
                {
                    var tcp = new TcpSerial();
                    mirror.MirrorStream = tcp;
                    tcpListener = new TcpListener(System.Net.IPAddress.Any, int.Parse(port));
                    tcpListener.Start(0);
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), (tcpListener, tcp));
                }
                else if (direction == "Outbound")
                {
                    mirror.MirrorStream = new TcpSerial() { retrys = 999999, autoReconnect = true, Host = extra, Port = port, ConfigRef = "SerialOutputPassTCP" };
                    mirror.MirrorStream.Open();
                }
            }
            else if (protocol == "UDP")
            {
                if (direction == "Inbound")
                {
                    var udp = new UdpSerial()
                    { ConfigRef = "SerialOutputPassUDP", Port = port };
                    udp.client = new UdpClient(int.Parse(port));
                    mirror.MirrorStream = udp;
                    udp.IsOpen = true;
                    mirror.MirrorStream.Open();
                }
                else if (direction == "Outbound")
                {
                    var udp = new UdpSerialConnect() { ConfigRef = "SerialOutputPassUDPCL" };
                    udp.hostEndPoint = new IPEndPoint(IPAddress.Parse(extra), int.Parse(port));
                    udp.client = new UdpClient();
                    udp.IsOpen = true;
                    mirror.MirrorStream = udp;
                }
            }
            else if (protocol == "Serial")
            {
                mirror.MirrorStream = new SerialPort();
                mirror.MirrorStream.PortName = port;
                mirror.MirrorStream.BaudRate = int.Parse(extra);
                mirror.MirrorStream.Open();
            }

            if (mirror.MirrorStream == null)
                throw new ArgumentException("Select Type and Direction");

            var comPort = MainV2.comPort;
            // AddMirror replaces the list under the interface's lock: the reader thread enumerates
            // Mirrors, and the legacy MirrorStream setter may be replacing it from another thread
            comPort.AddMirror(mirror);

            Active[key] = new ActiveMirror() { ComPort = comPort, Mirror = mirror, Listener = tcpListener };
        }

        private void StopMirror(string key)
        {
            var active = Active[key];
            Active.Remove(key);

            active.ComPort.RemoveMirror(active.Mirror);

            try
            {
                active.Listener?.Stop();
            }
            catch
            {
            }

            try
            {
                active.Mirror.MirrorStream?.Close();
            }
            catch
            {
            }
        }
    }
}