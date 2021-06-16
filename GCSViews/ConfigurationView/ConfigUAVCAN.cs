﻿using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UAVCAN;
using Timer = System.Windows.Forms.Timer;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigUAVCAN : MyUserControl, MissionPlanner.Controls.IDeactivate, IActivate
    {
        public ConfigUAVCAN()
        {
            InitializeComponent();

            uAVCANModelBindingSource.DataSource = allnodes;

            if (MainV2.comPort.BaseStream.IsOpen && !MainV2.comPort.MAV.param.ContainsKey("CAN_SLCAN_TIMOUT"))
                this.Enabled = false;
        }

        List<UAVCANModel> allnodes = new List<UAVCANModel>();

        public void Activate()
        {
            if (MainV2.comPort.MAV.param.Count > 5 && !MainV2.comPort.MAV.param.ContainsKey("CAN_SLCAN_TIMOUT"))
                this.Enabled = false;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_updatePending)
            {
                _updatePending = false;
                uAVCANModelBindingSource.ResetBindings(false);
            }
        }

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
                if (!MainV2.comPort.BaseStream.IsOpen)
                {
                    if (CustomMessageBox.Show(
                            "You are not currently connected via mavlink. Please make sure the device is already in slcan mode or this is the slcan serialport.",
                            "SLCAN", CustomMessageBox.MessageBoxButtons.OKCancel) != CustomMessageBox.DialogResult.OK)
                        return;
                }

                if (MainV2.comPort.BaseStream.IsOpen)
                {
                    var cport = MainV2.comPort.MAV.param["CAN_SLCAN_CPORT"].Value;
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        "CAN_SLCAN_CPORT", canport, true);
                    if (cport == 0)
                    {
                        CustomMessageBox.Show("Reboot required" + " after setting CPORT. Please reboot!",
                            Strings.ERROR);
                        return;
                    }

                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        "CAN_SLCAN_TIMOUT", 2, true);
                    MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent,
                        "CAN_P" + canport + "_DRIVER", 1);
                    //MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, "CAN_SLCAN_SERNUM", 0, true); // usb
                    // blind send
                    var paramname = "CAN_SLCAN_SERNUM";
                    var req = new MAVLink.mavlink_param_set_t
                    {
                        target_system = (byte) MainV2.comPort.sysidcurrent,
                        target_component = (byte) MainV2.comPort.compidcurrent,
                        param_type = (byte) MainV2.comPort
                            .MAVlist[(byte) MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent]
                            .param_types[paramname],
                        param_id = paramname.MakeBytesSize(16)
                    };
                    MainV2.comPort.sendPacket(req, (byte) MainV2.comPort.sysidcurrent,
                        (byte) MainV2.comPort.compidcurrent);
                    MainV2.comPort.sendPacket(req, (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent);
                }
            }
            catch
            {

            }

            {
                // grab the connected port
                var port = MainV2.comPort.BaseStream;

                // place an invalid port in its place
                if (port != null)
                    MainV2.comPort.BaseStream = new Comms.SerialPort() { PortName = port.PortName, BaudRate = port.BaudRate };

                //check if we started from within mavlink - if not get settings from menu and create port
                if (port == null || !port.IsOpen)
                {
                    switch (MainV2._connectionControl.CMB_serialport.Text)
                    {
                        case "TCP":
                            port = new TcpSerial();                            
                            break;
                        case "UDP":
                         port = new UdpSerial();                            
                            break;
                        case "WS":
                         port = new WebSocket();                            
                            break;
                        case "UDPCl":
                        port = new UdpSerialConnect();                            
                            break;
                        default:
                           port = new SerialPort()    {
                                PortName = MainV2._connectionControl.CMB_serialport.Text,
                                BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text)
                            };
                        break;
                    }
                }

                if (can == null)
                    can = new uavcan();

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
                            Uptime = TimeSpan.FromSeconds(msg.uptime_sec),
                            VSC = msg.vendor_specific_status_code
                        });

                        uAVCANModelBindingSource.ResetBindings(false);
                    });
                };

                if (!port.IsOpen)
                {
                    try
                    {
                        port.Open();
                    }
                    catch (Exception)
                    {
                        CustomMessageBox.Show(Strings.CheckPortSettingsOr);
                        return;
                    }
                }

                if (chk_log.Checked)
                    can.LogFile = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                              DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".can";

                var prd = new ProgressReporterDialogue();
                prd.UpdateProgressAndStatus(-1, "Trying to connect");
                prd.DoWork += sender => can.StartSLCAN(port.BaseStream);
                prd.btnCancel.Click += (sender, args) =>
                {
                    prd.doWorkArgs.CancelAcknowledged = true;
                    port.Close();
                };
                prd.RunBackgroundOperationAsync();

                if (prd.doWorkArgs.CancelRequested || prd.doWorkArgs.ErrorMessage != null)
                    return;

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

                        _updatePending = true;
                    }
                    else if (msg.GetType() == typeof(UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_res))
                    {
                        var gnires = msg as UAVCAN.uavcan.uavcan_protocol_GetNodeInfo_res;

                        var nodes = allnodes.Where((a) => a.ID == frame.SourceNode);

                        foreach (var item in nodes)
                        {
                            item.Name = ASCIIEncoding.ASCII.GetString(gnires.name, 0, gnires.name_len);
                            item.HardwareVersion = gnires.hardware_version.major + "." + gnires.hardware_version.minor;
                            item.SoftwareVersion = gnires.software_version.major + "." + gnires.software_version.minor + "." + gnires.software_version.vcs_commit.ToString("X");
                            item.SoftwareCRC = gnires.software_version.image_crc;
                            item.HardwareUID = gnires.hardware_version.unique_id.Select(a => a.ToString("X2")).Aggregate((a, b) =>
                                  {
                                      return a + " " + b;
                                  });
                            item.RawMsg = gnires;
                            item.VSC = gnires.status.vendor_specific_status_code;
                        }

                        _updatePending = true;
                    } 
                    else if (msg.GetType() == typeof(UAVCAN.uavcan.uavcan_protocol_debug_LogMessage))
                    {
                        var debug = msg as UAVCAN.uavcan.uavcan_protocol_debug_LogMessage;

                        this.BeginInvoke((Action) delegate()
                        {
                            DGDebug.Rows.Insert(0, new object[]
                            {
                                frame.SourceNode, debug.level.value,
                                ASCIIEncoding.ASCII.GetString(debug.source, 0, debug.source_len),
                                ASCIIEncoding.ASCII.GetString(debug.text, 0, debug.text_len)
                            });
                            if (DGDebug.Rows.Count > 100)
                            {
                                DGDebug.Rows.RemoveAt(DGDebug.Rows.Count - 1);
                            }
                        });
                    }
                };
            }
        }

        UAVCAN.uavcan can = new UAVCAN.uavcan();
        private bool _updatePending;
        private Timer timer;
        private TcpListener listener;

        private void myDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0) return;

            try {
                if (e.ColumnIndex == myDataGridView1.Columns["Menu"].Index)
                {
                    contextMenu1.Show(myDataGridView1, myDataGridView1.PointToClient(Control.MousePosition));
                }
            } catch
            {

            }
        }

        private void FirmwareUpdate(byte nodeID, bool beta = false)
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            uavcan.FileSendProgressArgs filesend = (id, file, percent) =>
            {
                prd.UpdateProgressAndStatus((int) percent, id + " " + file);
            };
            can.FileSendProgress += filesend;
            var devicename = can.GetNodeName(nodeID);
            var hwversion =
                double.Parse(
                    can.NodeInfo[nodeID].hardware_version.major +"."+ can.NodeInfo[nodeID].hardware_version.minor,
                    CultureInfo.InvariantCulture);

            if (CustomMessageBox.Show("Do you want to search the internet for an update?", "Update",
                CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                var url = can.LookForUpdate(devicename, hwversion, beta);

                if (url != string.Empty)
                {
                    try
                    {
                        var cancel = new CancellationTokenSource();

                        prd.DoWork += dialogue =>
                        {
                            prd.UpdateProgressAndStatus(5, "Download FW");
                            var tempfile = Path.GetTempFileName();
                            Download.getFilefromNet(url, tempfile);

                            uavcan.FileSendCompleteArgs file = (p, s) =>
                            {
                                prd.UpdateProgressAndStatus(100, "File send complete");
                            };
                            uavcan.FileSendProgressArgs fileprog = (n, f, p) =>
                            {
                                prd.UpdateProgressAndStatus((int) p, f);
                            };
                            can.FileSendComplete += file;
                            can.FileSendProgress += fileprog;

                            try
                            {
                                can.Update(nodeID, devicename, hwversion, tempfile, cancel.Token);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                            finally
                            {
                                can.FileSendComplete -= file;
                                can.FileSendProgress -= fileprog;
                            }

                            return;
                        };

                        prd.btnCancel.Click += (sender, args) =>
                        {
                            prd.doWorkArgs.CancelAcknowledged = true;
                            cancel.Cancel();
                        };

                        prd.RunBackgroundOperationAsync();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.Message, Strings.ERROR);
                    }
                }
                else
                {
                    CustomMessageBox.Show(Strings.UpdateNotFound, Strings.UpdateNotFound);
                }
            }
            else
            {
                FileDialog fd = new OpenFileDialog();
                fd.RestoreDirectory = true;
                fd.Filter = "*.bin|*.bin";
                var dia = fd.ShowDialog();

                if (fd.CheckFileExists && dia == DialogResult.OK)
                {
                    uavcan.FileSendCompleteArgs file = (p, s) =>
                    {
                        prd.UpdateProgressAndStatus(100, "File send complete");
                    };
                    uavcan.FileSendProgressArgs fileprog = (n, f, p) =>
                    {
                        prd.UpdateProgressAndStatus((int) p, f);
                    };
                    can.FileSendComplete += file;
                    can.FileSendProgress += fileprog;

                    try
                    {
                        var cancel = new CancellationTokenSource();

                        prd.DoWork += dialogue =>
                        {
                            can.Update(nodeID,
                                devicename, 0,
                                fd.FileName, cancel.Token);

                            return;
                        };

                        prd.btnCancel.Click += (sender, args) =>
                        {
                            prd.doWorkArgs.CancelAcknowledged = true;
                            cancel.Cancel();
                        };

                        prd.RunBackgroundOperationAsync();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.Message, Strings.ERROR);
                    }   
                    finally
                    {
                        can.FileSendComplete -= file;
                        can.FileSendProgress -= fileprog;
                    }
                }
            }

            can.FileSendProgress -= filesend;
            prd.Dispose();
        }

        private void GetParameters(byte nodeID)
        {
            IProgressReporterDialogue prd = new ProgressReporterDialogue();
            List<uavcan.uavcan_protocol_param_GetSet_res> paramlist =
                new List<uavcan.uavcan_protocol_param_GetSet_res>();
            prd.doWorkArgs.ForceExit = true;
            prd.doWorkArgs.CancelRequestChanged += (sender2, args) => { prd.doWorkArgs.CancelAcknowledged = true; };
            prd.DoWork += dialogue => { paramlist = can.GetParameters(nodeID); };
            prd.UpdateProgressAndStatus(-1, Strings.GettingParams);
            prd.RunBackgroundOperationAsync();

            if (!prd.doWorkArgs.CancelRequested)
                new UAVCANParams(can, nodeID, paramlist).ShowUserControl();
        }

        public void Deactivate()
        {
            can?.Stop(chk_canonclose.Checked);
            can = null;
            timer?.Stop();
        }

        private void myDataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int RowIndex = 0; RowIndex < myDataGridView1.RowCount; RowIndex++)
            {
                myDataGridView1[Menu.Index, RowIndex].Value = "Menu";
            }
        }

        private void uAVCANModelBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void But_uavcaninspector_Click(object sender, EventArgs e)
        {
            if (can == null)
            {
                CustomMessageBox.Show(Strings.PleaseConnect);
                return;
            }

            new UAVCANInspector(can).Show();
        }

        private void myDataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var id = myDataGridView1[iDDataGridViewTextBoxColumn.Index, e.RowIndex].Value.ToString();

            var node = allnodes.First(a => a.ID.ToString() == id);

            //uAVCANModelBindingSource.ResetItem(e.RowIndex);

            this.BeginInvoke((Action)delegate
            {
                //var index = uAVCANModelBindingSource.Find("ID", id);
                //uAVCANModelBindingSource.Position = index;
            });
        }

        private void but_uavcanfilebrowser_Click(object sender, EventArgs e)
        {
            if (can == null)
            {
                CustomMessageBox.Show(Strings.PleaseConnect);
                return;
            }

            if (myDataGridView1.SelectedCells.Count <= 0)
            {
                CustomMessageBox.Show(Strings.InvalidField + " Row");
                return;
            }

            var id = byte.Parse(myDataGridView1[iDDataGridViewTextBoxColumn.Index, myDataGridView1.SelectedCells[0].RowIndex].Value
                .ToString());

            new UAVCANFileUI(can, id).ShowUserControl();
        }

        private void menu_passthrough_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                menu_passthrough.Checked = false;
                listener.Stop();
                CustomMessageBox.Show("Stop", "Disabled forwarding");
                listener = null;
                return;
            }

            var port = 500;
            if (InputBox.Show("Enter TCP Port", "Enter TCP Port", ref port) == DialogResult.OK)
            {
                menu_passthrough.Checked = true;

                Task.Run(() =>
                {
                    try
                    {

                        listener = new TcpListener(IPAddress.Any, port);
                        listener.Start();

                        int tcpbps = 0;
                        int rtcmbps = 0;
                        int combps = 0;
                        int second = 0;

                        while (true)
                        {
                            var client = listener.AcceptTcpClient();
                            client.NoDelay = true;

                            var st = client.GetStream();

                            uavcan.MessageRecievedDel mrd =  (frame, msg, id) =>
                            {
                                combps += frame.SizeofEntireMsg;
                                if (frame.MsgTypeID == uavcan.UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_ID)
                                {
                                    var data = msg as uavcan.uavcan_equipment_gnss_RTCMStream;
                                    try
                                    {
                                        rtcmbps += data.data_len;
                                        st.Write(data.data, 0, data.data_len);
                                        st.Flush();
                                    }
                                    catch
                                    {
                                        client = null;
                                    }
                                }
                            };

                            can.MessageReceived -= mrd;
                            can.MessageReceived += mrd;

                            try
                            {
                                while (true)
                                {
                                    if (client.Available > 0)
                                    {
                                        var toread = Math.Min(client.Available, 128);
                                        byte[] buffer = new byte[toread];
                                        var read = st.Read(buffer, 0, toread);
                                        foreach (var b in buffer)
                                        {
                                            Console.Write("0x{0:X} ", b);
                                        }

                                        Console.WriteLine();
                                        tcpbps += read;
                                        var slcan = can.PackageMessage(0, 30, 0,
                                            new uavcan.uavcan_equipment_gnss_RTCMStream()
                                                {protocol_id = 3, data = buffer, data_len = (byte) read});
                                        can.WriteToStream(slcan);
                                    }

                                    Thread.Sleep(1);

                                    if (second != DateTime.Now.Second)
                                    {
                                        Console.WriteLine("tcp:{0} can:{1} data:{2} avail:{3}", tcpbps, combps, rtcmbps,
                                            client.Available);
                                        tcpbps = combps = rtcmbps = 0;
                                        second = DateTime.Now.Second;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                can.MessageReceived -= mrd;
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        CustomMessageBox.Show(Strings.ERROR, "Forwarder problem " + exception.ToString());
                        if (listener != null)
                            listener.Stop();
                        listener = null;
                        this.InvokeIfRequired(() => { menu_passthrough.Checked = true; });
                    }
                });
            }
        }

        private void menu_update_Click(object sender, EventArgs e)
        {
            FirmwareUpdate(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString()));
        }

        private void menu_parameters_Click(object sender, EventArgs e)
        {
            GetParameters(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString()));
        }

        private void menu_restart_Click(object sender, EventArgs e)
        {
            can.RestartNode(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString()));
        }

        private void menu_updatebeta_Click(object sender, EventArgs e)
        {
            FirmwareUpdate(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString()), true);
        }
    }
}
