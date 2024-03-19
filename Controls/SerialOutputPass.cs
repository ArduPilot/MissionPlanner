﻿using Microsoft.Scripting.Utils;
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
                MainV2.comPort.MirrorStream.Close();
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
                                MainV2.comPort.MirrorStream = new TcpSerial();
                                CMB_baudrate.SelectedIndex = 0;
                                int port = 14550;
                                if (InputBox.Show("Port", "Enter port", ref port) != DialogResult.OK)
                                    return;
                                listener = new TcpListener(System.Net.IPAddress.Any, port);
                                listener.Start(0);
                                listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
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
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);

            ((TcpSerial)MainV2.comPort.MirrorStream).client = client;

            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }

        private void chk_write_CheckedChanged(object sender, EventArgs e)
        {
            MainV2.comPort.MirrorStreamWrite = chk_write.Checked;
        }


        private void myDataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Save();
        }

        private void Save() 
        {
            List<string> ans = new List<string>();
            myDataGridView1.Rows.ForEach<DataGridViewRow>(x => 
            {
                var line = x.Cells.Select(i => ((DataGridViewCell)i).FormattedValue).ToJSON(Formatting.None);
                ans.Add(line);
            });

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
        }

        string configlist = "serialpasslist";

        private void myDataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        private void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Go.Index) 
            {
                try
                {
                    var protocol = myDataGridView1[Type.Index, e.RowIndex].Value.ToString();
                    var direction = myDataGridView1[Direction.Index, e.RowIndex].Value.ToString();
                    var port = myDataGridView1[Port.Index, e.RowIndex].Value.ToString();
                    var extra = myDataGridView1[Extra.Index, e.RowIndex].Value.ToString();
                    if (protocol == "TCP")
                    {
                        if (direction == "Inbound")
                        {
                            MainV2.comPort.MirrorStream = new TcpSerial();
                            CMB_baudrate.SelectedIndex = 0;
                            listener = new TcpListener(System.Net.IPAddress.Any, int.Parse(port.ToString()));
                            listener.Start(0);
                            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
                            BUT_connect.Text = Strings.Stop;
                        }
                        else if (direction == "Outbound")
                        {
                            MainV2.comPort.MirrorStream = new TcpSerial() { retrys = 999999, autoReconnect = true, Host = extra, Port = port, ConfigRef = "SerialOutputPassTCP" };
                            CMB_baudrate.SelectedIndex = 0;
                            MainV2.comPort.MirrorStream.Open();
                        }
                    } 
                    else if (protocol == "UDP")
                    {
                        if (direction == "Inbound")
                        {
                            var udp = new UdpSerial()
                            { ConfigRef = "SerialOutputPassUDP", Port = port.ToString() };
                            udp.client = new UdpClient(int.Parse(port));
                            MainV2.comPort.MirrorStream = udp;
                            CMB_baudrate.SelectedIndex = 0;
                            MainV2.comPort.MirrorStream.Open();
                        }
                        else if (direction == "Outbound")
                        {
                            var udp = new UdpSerialConnect() { ConfigRef = "SerialOutputPassUDPCL" };
                            udp.hostEndPoint = new IPEndPoint(IPAddress.Parse(extra), int.Parse(port));
                            udp.client = new UdpClient();
                            udp.IsOpen = true;
                            MainV2.comPort.MirrorStream = udp;
                            CMB_baudrate.SelectedIndex = 0;
                        }
                    } 
                    else if (protocol == "Serial")
                    {
                        MainV2.comPort.MirrorStream = new SerialPort();
                        MainV2.comPort.MirrorStream.PortName = port;
                        MainV2.comPort.MirrorStream.BaudRate = int.Parse(extra);
                        MainV2.comPort.MirrorStream.Open();
                    }
                }
                catch (Exception ex) {
                    CustomMessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}