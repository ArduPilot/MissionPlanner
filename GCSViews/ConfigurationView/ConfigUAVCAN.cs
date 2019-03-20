using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using UAVCAN;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigUAVCAN : UserControl, MissionPlanner.Controls.IDeactivate
    {
        public ConfigUAVCAN()
        {
            InitializeComponent();

            uAVCANModelBindingSource.DataSource = allnodes;
        }

        private void configUAVCANBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        List<UAVCANModel> allnodes = new List<UAVCANModel>();

        private void but_slcanmode_Click(object sender, EventArgs e)
        {
            startslcan(1);
        }

        private void but_slcanmode2_Click(object sender, EventArgs e)
        {
            startslcan(2);
        }

        public void startslcan(byte canport)
        {
            but_slcanmode1.Enabled = false;
            but_slcanmode2.Enabled = false;
            try
            {
                MainV2.comPort.setParam("CAN_SLCAN_CPORT", canport, true);
                MainV2.comPort.setParam("CAN_SLCAN_SERNUM", 0, true);
            }
            catch
            {

            }
            {
                // fail is good

                var port = MainV2.comPort.BaseStream;

                MainV2.comPort.BaseStream = new Comms.SerialPort() { PortName = port.PortName, BaudRate = port.BaudRate };

                can.SourceNode = 127;

                can.NodeAdded += (id, msg) =>
                {
                    this.BeginInvoke((Action)delegate
                    {
                        allnodes.Add(new UAVCANModel()
                        {
                            ID = id,
                            Name = "?",
                            Health = msg.health.ToString(),
                            Mode = msg.mode.ToString(),
                            Uptime = TimeSpan.FromSeconds(msg.uptime_sec)
                        });
                    });
                };

                if (!port.IsOpen)
                    port.Open();

                can.StartSLCAN(port.BaseStream);

                can.SetupFileServer();

                can.SetupDynamicNodeAllocator();

                can.MessageReceived += (frame, msg, transferID) =>
                {
                    if (msg.GetType() == typeof(UAVCAN.uavcan.uavcan_protocol_NodeStatus))
                    {
                        var ns = msg as UAVCAN.uavcan.uavcan_protocol_NodeStatus;

                        var nodes = allnodes.Where((a) => a.ID == frame.SourceNode);

                        if (nodes.Count() > 0 && nodes.First().Name == "?")
                        {
                            var statetracking = new UAVCAN.uavcan.statetracking();
                            // get node info
                            UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_req() { };
                            gnireq.encode(UAVCAN.uavcan.uavcan_transmit_chunk_handler, statetracking);

                            var slcan = can.PackageMessage(frame.SourceNode, 30, 0, gnireq);
                            can.WriteToStream(slcan);
                        }

                        foreach (var item in nodes)
                        {
                            switch (ns.health)
                            {
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK:
                                    item.Health = "OK";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_WARNING:
                                    item.Health = "WARNING";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_ERROR:
                                    item.Health = "ERROR";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_CRITICAL:
                                    item.Health = "CRITICAL";
                                    break;
                            }
                            switch (ns.mode)
                            {
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL:
                                    item.Mode = "OPERATIONAL";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_INITIALIZATION:
                                    item.Mode = "INITIALIZATION";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_MAINTENANCE:
                                    item.Mode = "MAINTENANCE";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE:
                                    item.Mode = "SOFTWARE_UPDATE";
                                    break;
                                case (byte)UAVCAN.uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_OFFLINE:
                                    item.Mode = "OFFLINE";
                                    break;
                            }
                            item.Uptime = TimeSpan.FromSeconds(ns.uptime_sec);
                        }

                        this.BeginInvoke((Action)delegate
                        {
                            uAVCANModelBindingSource.ResetBindings(false);
                        });
                    }
                    else if (msg.GetType() == typeof(UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_res))
                    {
                        var gnires = msg as UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_res;

                        var nodes = allnodes.Where((a) => a.ID == frame.SourceNode);

                        foreach (var item in nodes)
                        {
                            item.Name = ASCIIEncoding.ASCII.GetString(gnires.name, 0, gnires.name_len);
                            item.HardwareVersion = gnires.hardware_version.major + "." + gnires.hardware_version.minor;
                        }

                        this.BeginInvoke((Action)delegate
                        {
                            uAVCANModelBindingSource.ResetBindings(false);
                        });
                    }
                };
            }
        }

        UAVCAN.uavcan can = new UAVCAN.uavcan();

        private void myDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                myDataGridView1.Columns["updateDataGridViewTextBoxColumn"].Index &&
                e.ColumnIndex != myDataGridView1.Columns["Parameter"].Index) return;

            byte nodeID = (byte)myDataGridView1[iDDataGridViewTextBoxColumn.Index, e.RowIndex].Value;

            if (e.ColumnIndex == myDataGridView1.Columns["Parameter"].Index)
            {
                var paramlist = can.GetParameters(nodeID);

                new UAVCANParams(can, nodeID, paramlist).ShowUserControl();
            }
            else if (e.ColumnIndex == myDataGridView1.Columns["updateDataGridViewTextBoxColumn"].Index)
            {
                FileDialog fd = new OpenFileDialog();
                fd.RestoreDirectory = true;
                fd.Filter = "*-crc.bin|*-crc.bin";
                var dia = fd.ShowDialog();

                if (fd.CheckFileExists && dia == DialogResult.OK)
                    can.Update(myDataGridView1[nameDataGridViewTextBoxColumn.Index, e.RowIndex].Value.ToString(), 0,
                        fd.FileName);
            }
        }

        public void Deactivate()
        {
            can.Stop();
            can = null;
        }

        private void myDataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            myDataGridView1[updateDataGridViewTextBoxColumn.Index, e.RowIndex].Value = "Update";
            myDataGridView1[Parameter.Index, e.RowIndex].Value = "Parameters";
        }


    }

    public class UAVCANModel
    {
        public byte ID { get; set; }
        public string Name { get; set; } = "?";
        public string Mode { get; set; }
        public string Health { get; set; }
        public TimeSpan Uptime { get; set; }
        public string HardwareVersion { get; set; }
    }
}
