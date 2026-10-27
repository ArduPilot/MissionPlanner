﻿using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class SerialOutputCoT : Form
    {
        enum Mode
        {
            TakMulticast,
            UdpClient,
            TcpClient,
            TcpHost,
            Serial
        }

        class ModeItem
        {
            public Mode Mode;
            public string Port;
            public string Text;

            /// <summary>What is saved: the mode's name, or the COM port's.</summary>
            public string Key
            {
                get { return Mode == Mode.Serial ? Port : Mode.ToString(); }
            }

            public override string ToString()
            {
                return Text;
            }
        }

        class AdapterItem
        {
            /// <summary>null for every adapter</summary>
            public CotAdapter Adapter;
            public string Text;

            public override string ToString()
            {
                return Text;
            }
        }

        static SerialOutputCoT open;

        // what was typed for each mode, so switching between them keeps it
        readonly Dictionary<Mode, string> hosts = new Dictionary<Mode, string>();
        readonly Dictionary<Mode, string> ports = new Dictionary<Mode, string>();
        ModeItem shown;
        bool loading;

        /// <summary>Shows the window, or brings it forward when it is open already: there is one output.</summary>
        public static void ShowWindow()
        {
            if (open != null && !open.IsDisposed)
            {
                if (open.WindowState == FormWindowState.Minimized)
                    open.WindowState = FormWindowState.Normal;
                open.Activate();
                return;
            }

            open = new SerialOutputCoT();
            open.Show();
        }

        public SerialOutputCoT()
        {
            InitializeComponent();

            CMB_serialport.Items.Add(new ModeItem { Mode = Mode.TakMulticast, Text = Strings.CotModeTakMulticast });
            CMB_serialport.Items.Add(new ModeItem { Mode = Mode.UdpClient, Text = Strings.CotModeUdpClient });
            CMB_serialport.Items.Add(new ModeItem { Mode = Mode.TcpClient, Text = Strings.CotModeTcpClient });
            CMB_serialport.Items.Add(new ModeItem { Mode = Mode.TcpHost, Text = Strings.CotModeTcpHost });
            foreach (var port in SerialPort.GetPortNames())
                CMB_serialport.Items.Add(new ModeItem { Mode = Mode.Serial, Port = port, Text = port });

            CMB_listen.Items.Add(Strings.CotAllAdapters);
            CMB_listen.Items.Add(Strings.CotThisPcOnly);

            LBL_status.Text = Strings.CotNotSending;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void SerialOutputCoT_Load(object sender, EventArgs e)
        {
            loading = true;
            try
            {
                // this can crash if you're connecting as you're loading the screen so the row count may change as you're populating it.
                myDataGridView1.Deserialize(Settings.Instance["CoTUID"]);
            } catch { }

            updateRate_numericUpDown.Value = Math.Max(updateRate_numericUpDown.Minimum,
                Math.Min(updateRate_numericUpDown.Maximum, Settings.Instance.GetDecimal("CoT_updateRate", 10)));
            var baud = Settings.Instance.GetInt32("CoT_CMB_baudrate", 0);
            CMB_baudrate.SelectedIndex = baud >= 0 && baud < CMB_baudrate.Items.Count ? baud : 0;
            CB_advancedMode.Checked = Settings.Instance.GetBoolean("CoT_CB_advancedMode", true);
            chk_indent.Checked = Settings.Instance.GetBoolean("CoT_chk_indent", true);
            CMB_listen.SelectedIndex = Settings.Instance.GetBoolean("CoT_tcphost_local", false) ? 1 : 0;

            hosts[Mode.TakMulticast] = Settings.Instance["CoT_mcast_group", CotMulticastTransport.SaGroup];
            ports[Mode.TakMulticast] = Settings.Instance["CoT_mcast_port", CotMulticastTransport.SaPort.ToString()];
            hosts[Mode.UdpClient] = Settings.Instance["CoT_udp_host", ""];
            ports[Mode.UdpClient] = Settings.Instance["CoT_udp_port", "4242"];
            hosts[Mode.TcpClient] = Settings.Instance["CoT_tcp_host", ""];
            ports[Mode.TcpClient] = Settings.Instance["CoT_tcp_port", "8087"];
            ports[Mode.TcpHost] = Settings.Instance["CoT_tcphost_port", "14551"];

            FillAdapters(Settings.Instance["CoT_mcast_adapter"]);
            CMB_serialport.SelectedItem = SavedMode();
            loading = false;

            CB_advancedMode_CheckedChanged(null, null);
            ShowMode();

            // a sender that is already running carries on; the window only shows it
            var running = CotSender.Current;
            if (running != null)
            {
                running.Changed += Sender_Changed;
                ShowSender(running);
            }
        }

        ModeItem SavedMode()
        {
            var items = CMB_serialport.Items.OfType<ModeItem>().ToList();
            var key = Settings.Instance["CoT_mode"];
            var saved = items.FirstOrDefault(i => i.Key == key);
            if (saved != null)
                return saved;

            // before the mode was saved by name: the position in the old list
            switch (Settings.Instance.GetInt32("CoT_CMB_serialport", 0))
            {
                case 1:
                    return items.First(i => i.Mode == Mode.TcpHost);
                case 2:
                    return items.First(i => i.Mode == Mode.TcpClient);
                case 3:
                case 4:
                    return items.First(i => i.Mode == Mode.UdpClient);
                default:
                    return items[0];
            }
        }

        void FillAdapters(string saved)
        {
            var adapters = CotNetwork.Adapters();
            CMB_adapter.Items.Clear();
            CMB_adapter.Items.Add(new AdapterItem { Text = Strings.CotAllAdapters });
            foreach (var adapter in adapters)
                CMB_adapter.Items.Add(new AdapterItem { Adapter = adapter, Text = adapter.ToString() });

            var items = CMB_adapter.Items.OfType<AdapterItem>().ToList();
            AdapterItem pick = null;
            if (saved == "*")
                pick = items[0];
            else if (!string.IsNullOrEmpty(saved))
                pick = items.FirstOrDefault(i => i.Adapter != null && i.Adapter.Name == saved);
            if (pick == null)
            {
                var preferred = CotNetwork.DefaultAdapter(adapters);
                pick = items.FirstOrDefault(i => i.Adapter == preferred) ?? items[0];
            }

            CMB_adapter.SelectedItem = pick;
        }

        private void CMB_serialport_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
                ShowMode();
        }

        /// <summary>Shows the fields the chosen connection needs, keeping what was typed for the last one.</summary>
        void ShowMode()
        {
            KeepTyped();

            shown = CMB_serialport.SelectedItem as ModeItem;
            if (shown == null)
                return;

            var mode = shown.Mode;
            label_host.Text = mode == Mode.TakMulticast ? Strings.CotGroup : Strings.CotHost;
            label_host.Visible = TB_host.Visible = HasHost(mode);
            label_port.Visible = TB_port.Visible = mode != Mode.Serial;
            TB_host.Text = hosts.ContainsKey(mode) ? hosts[mode] : "";
            TB_port.Text = ports.ContainsKey(mode) ? ports[mode] : "";

            CMB_adapter.Visible = mode == Mode.TakMulticast;
            CMB_listen.Visible = mode == Mode.TcpHost;
            CMB_baudrate.Visible = mode == Mode.Serial;
            label_option.Visible = CMB_adapter.Visible || CMB_listen.Visible || CMB_baudrate.Visible;
            label_option.Text = mode == Mode.TakMulticast ? Strings.CotAdapter :
                mode == Mode.TcpHost ? Strings.CotListenOn : Strings.CotBaud;
        }

        static bool HasHost(Mode mode)
        {
            return mode == Mode.TakMulticast || mode == Mode.UdpClient || mode == Mode.TcpClient;
        }

        void KeepTyped()
        {
            if (shown == null || shown.Mode == Mode.Serial)
                return;
            if (HasHost(shown.Mode))
                hosts[shown.Mode] = TB_host.Text.Trim();
            ports[shown.Mode] = TB_port.Text.Trim();
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            var running = CotSender.Current;
            if (running != null)
            {
                running.Changed -= Sender_Changed;
                running.Stop();
                BUT_connect.Text = Strings.Connect;
                LBL_status.Text = running.Status;
                return;
            }

            ICotTransport transport;
            try
            {
                transport = Transport();
            }
            catch (FormatException ex)
            {
                LBL_status.Text = ex.Message;
                return;
            }

            SaveSettings();

            var started = CotSender.Start(transport, Snapshot());
            started.Changed += Sender_Changed;
            ShowSender(started);
        }

        ICotTransport Transport()
        {
            KeepTyped();
            var mode = (ModeItem)CMB_serialport.SelectedItem;
            switch (mode.Mode)
            {
                case Mode.TakMulticast:
                    IPAddress group;
                    if (!IPAddress.TryParse(TB_host.Text.Trim(), out group) || !CotNetwork.IsMulticast(group))
                        throw new FormatException(Strings.CotEnterGroup);
                    return new CotMulticastTransport(group, PortTyped(), ((AdapterItem)CMB_adapter.SelectedItem).Adapter);
                case Mode.UdpClient:
                    return new CotUdpTransport(HostTyped(), PortTyped());
                case Mode.TcpClient:
                    return new CotTcpClientTransport(HostTyped(), PortTyped());
                case Mode.TcpHost:
                    return new CotTcpServerTransport(PortTyped(), CMB_listen.SelectedIndex == 1);
                default:
                    return new CotSerialTransport(mode.Port, int.Parse(CMB_baudrate.Text));
            }
        }

        string HostTyped()
        {
            var host = TB_host.Text.Trim();
            if (host.Length == 0)
                throw new FormatException(Strings.CotEnterHost);
            return host;
        }

        int PortTyped()
        {
            int port;
            if (!int.TryParse(TB_port.Text.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out port) ||
                port < 1 || port > 65535)
                throw new FormatException(Strings.CotEnterPort);
            return port;
        }

        /// <summary>The settings as the sending thread gets them, so it never reads a control.</summary>
        CotSettings Snapshot()
        {
            var rows = new List<CotRow>();
            foreach (DataGridViewRow row in myDataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;
                var takvCell = row.Cells[takv.Index].Value;
                rows.Add(new CotRow
                {
                    SysId = CellText(row, sysid),
                    Uid = CellText(row, UID),
                    Takv = takvCell != null && Convert.ToBoolean(takvCell),
                    Callsign = CellText(row, ContactCallsign),
                    Endpoint = CellText(row, ContactEndPointIP),
                    Vmf = CellText(row, VMF)
                });
            }

            return new CotSettings
            {
                Type = TB_xml_type.Text.Trim(),
                Details = CB_advancedMode.Checked,
                Indent = chk_indent.Checked,
                IntervalSeconds = (double)updateRate_numericUpDown.Value,
                Rows = rows
            };
        }

        static string CellText(DataGridViewRow row, DataGridViewColumn column)
        {
            var value = row.Cells[column.Index].Value;
            return value == null ? "" : value.ToString().Trim();
        }

        /// <summary>Hands the running sender the settings as they are now.</summary>
        void PushSettings()
        {
            if (loading)
                return;
            var running = CotSender.Current;
            if (running != null)
                running.Settings = Snapshot();
        }

        void SettingChanged(object sender, EventArgs e)
        {
            PushSettings();
        }

        void Sender_Changed(object sender, EventArgs e)
        {
            // the sending thread: never wait on the window from there, Stop() may be holding it
            try
            {
                if (!IsDisposed && IsHandleCreated)
                    BeginInvoke((Action)(() => ShowSender((CotSender)sender)));
            }
            catch (InvalidOperationException)
            {
                // closing
            }
        }

        void ShowSender(CotSender cot)
        {
            if (IsDisposed)
                return;
            LBL_status.Text = cot.Status;
            TB_output.Text = cot.LastRound;
            BUT_connect.Text = CotSender.Current == cot ? Strings.Stop : Strings.Connect;
        }

        void SaveSettings()
        {
            KeepTyped();
            Settings.Instance["CoT_updateRate"] = updateRate_numericUpDown.Value.ToString(CultureInfo.InvariantCulture);
            if (CMB_serialport.SelectedItem is ModeItem mode)
                Settings.Instance["CoT_mode"] = mode.Key;
            Settings.Instance["CoT_CMB_baudrate"] = CMB_baudrate.SelectedIndex.ToString();
            Settings.Instance["CoT_CB_advancedMode"] = CB_advancedMode.Checked.ToString();
            Settings.Instance["CoT_chk_indent"] = chk_indent.Checked.ToString();
            Settings.Instance["CoT_tcphost_local"] = (CMB_listen.SelectedIndex == 1).ToString();
            if (CMB_adapter.SelectedItem is AdapterItem adapter)
                Settings.Instance["CoT_mcast_adapter"] = adapter.Adapter == null ? "*" : adapter.Adapter.Name;

            Settings.Instance["CoT_mcast_group"] = hosts[Mode.TakMulticast];
            Settings.Instance["CoT_mcast_port"] = ports[Mode.TakMulticast];
            Settings.Instance["CoT_udp_host"] = hosts[Mode.UdpClient];
            Settings.Instance["CoT_udp_port"] = ports[Mode.UdpClient];
            Settings.Instance["CoT_tcp_host"] = hosts[Mode.TcpClient];
            Settings.Instance["CoT_tcp_port"] = ports[Mode.TcpClient];
            Settings.Instance["CoT_tcphost_port"] = ports[Mode.TcpHost];
        }

        private void BTN_clear_TB_Click(object sender, EventArgs e)
        {
            TB_output.Text = "";
        }

        private void SerialOutputCoT_FormClosing(object sender, FormClosingEventArgs e)
        {
            // the output keeps sending, with the settings it has, until Stop
            var running = CotSender.Current;
            if (running != null)
                running.Changed -= Sender_Changed;

            SaveSettings();
            Settings.Instance["CoTUID"] = myDataGridView1.Serialize();
        }

        private void myDataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            Settings.Instance["CoTUID"] = myDataGridView1.Serialize();
            PushSettings();
        }

        private void myDataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            PushSettings();
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

            PushSettings();
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
    }
}