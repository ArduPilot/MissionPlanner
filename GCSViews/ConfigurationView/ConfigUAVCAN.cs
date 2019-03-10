using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            try
            {
                MainV2.comPort.setParam("CAN_SLCAN_SERNUM", 0, true);
            } catch
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

                if(!port.IsOpen)
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
                            var statetracking = new UAVCAN.UAVCAN.statetracking();
                            // get node info
                            UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_req() { };
                            gnireq.encode(UAVCAN.UAVCAN.chunk_cb, statetracking);

                            var slcan = can.PackageMessage(frame.SourceNode, 30, 0, gnireq);
                            can.WriteToStream(slcan);
                        }

                        foreach (var item in nodes)
                        {
                            item.Health = ns.health.ToString();
                            item.Mode = ns.mode.ToString();
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

        UAVCAN.UAVCAN can = new UAVCAN.UAVCAN();

        private void myDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                myDataGridView1.Columns["updateDataGridViewTextBoxColumn"].Index) return;

            byte nodeID = (byte)myDataGridView1[iDDataGridViewTextBoxColumn.Index, e.RowIndex].Value;
            FileDialog fd = new OpenFileDialog();
            fd.RestoreDirectory = true;
            fd.Filter = "*-crc.bin|*-crc.bin";
            var dia = fd.ShowDialog();

            if(fd.CheckFileExists && dia == DialogResult.OK)
                can.Update(myDataGridView1[nameDataGridViewTextBoxColumn.Index, e.RowIndex].Value.ToString(), 0, fd.FileName);
        }

        public void Deactivate()
        {
            can.Stop();
            can = null;
        }

        private void myDataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            myDataGridView1[updateDataGridViewTextBoxColumn.Index, e.RowIndex].Value = "Update";
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
