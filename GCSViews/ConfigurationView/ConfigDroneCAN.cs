using MissionPlanner.Comms;
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
using DroneCAN;
using Timer = System.Windows.Forms.Timer;
using static DroneCAN.DroneCAN;
using System.ComponentModel;
using System.Drawing;
using MissionPlanner.ArduPilot;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigDroneCAN : MyUserControl, MissionPlanner.Controls.IDeactivate, IActivate
    {
        public ConfigDroneCAN()
        {
            InitializeComponent();

            uAVCANModelBindingSource.DataSource = allnodes;

            if (MainV2.comPort.BaseStream.IsOpen && !MainV2.comPort.MAV.param.ContainsKey("CAN_SLCAN_TIMOUT"))
                but_slcanmode1.Enabled = false;
        }

        List<DroneCANModel> allnodes = new List<DroneCANModel>();

        public void Activate()
        {
            if (MainV2.comPort.MAV.param.Count > 5 && !MainV2.comPort.MAV.param.ContainsKey("CAN_SLCAN_TIMOUT"))
                but_slcanmode1.Enabled = false;

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

        private void but_slcandirect_Click(object sender, EventArgs e)
        {
            startslcan(1);
        }

        private void but_slcanmavlink_Click(object sender, EventArgs e)
        {
            mavlinkCANRun = false;
            StartMavlinkCAN(1);
        }

        private void StartMavlinkCAN(byte bus = 1)
        {
            BusInUse = bus;
            but_slcanmode1.Enabled = false;
            but_mavlinkcanmode2.Enabled = true;
            but_mavlinkcanmode2_2.Enabled = true;
            but_filter.Enabled = true;

            Task.Run(() =>
            {
                // allows old instance to exit
                Thread.Sleep(1000);
                mavlinkCANRun = true;
                // send every second, timeout is in 5 seconds
                while (mavlinkCANRun)
                {
                    try
                    {
                        // setup forwarding on can port 1
                        var ans = MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.CAN_FORWARD, bus, 0, 0, 0, 0, 0, 0,
                            false);

                        if (ans == false) // MAVLink.MAV_RESULT.UNSUPPORTED)
                        {
                            //return;
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    if (mavlinkCANRun)
                        Thread.Sleep(1000);
                }
            });

            var port = new CommsInjection();

            var can = new DroneCAN.DroneCAN();
            can.FrameReceived += (frame, payload) =>
            {
                //https://github.com/dronecan/pydronecan/blob/master/dronecan/driver/mavcan.py#L114
                //if frame.extended:
                //  message_id |= 1 << 31

                if (payload.packet_data.Length > 8)
                    MainV2.comPort.sendPacket(new MAVLink.mavlink_canfd_frame_t(
                            BitConverter.ToUInt32(frame.packet_data, 0) + (frame.Extended ? 0x80000000 : 0),
                            (byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent, (byte)(bus - 1),
                            (byte)DroneCAN.DroneCAN.dataLengthToDlc(payload.packet_data.Length),
                            payload.packet_data),
                        (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent);
                else
                {
                    var frame2 = new MAVLink.mavlink_can_frame_t(
                        BitConverter.ToUInt32(frame.packet_data, 0) + (frame.Extended ? 0x80000000 : 0),
                        (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, (byte)(bus - 1),
                        (byte)DroneCAN.DroneCAN.dataLengthToDlc(payload.packet_data.Length),
                        payload.packet_data);
                    MainV2.comPort.sendPacket(frame2,
                        (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent);
                }
            };

            port.ReadBufferUpdate += (o, i) => { };
            port.WriteCallback += (o, bytes) =>
            {
                var lines = ASCIIEncoding.ASCII.GetString(bytes.ToArray())
                    .Split(new[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    can.ReadMessageSLCAN(line);

                }

            };

            // mavlink to slcan
            MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CAN_FRAME, (m) =>
            {
                if (m.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAN_FRAME)
                {
                    var canfd = false;
                    var pkt = (MAVLink.mavlink_can_frame_t)m.data;
                    var cf = new CANFrame(BitConverter.GetBytes(pkt.id));
                    var length = pkt.len;
                    var payload = new CANPayload(pkt.data);

                    var ans2 = String.Format("{0}{1}{2}{3}\r", canfd ? 'B' : 'T', cf.ToHex(), length.ToString("X")
                        , payload.ToHex(DroneCAN.DroneCAN.dlcToDataLength(length)));

                    port.AppendBuffer(ASCIIEncoding.ASCII.GetBytes(ans2));
                }
                else if (m.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CANFD_FRAME)
                {
                    var canfd = true;
                    var pkt = (MAVLink.mavlink_canfd_frame_t)m.data;
                    var cf = new CANFrame(BitConverter.GetBytes(pkt.id));
                    var length = pkt.len;
                    var payload = new CANPayload(pkt.data);

                    var ans2 = String.Format("{0}{1}{2}{3}\r", canfd ? 'B' : 'T', cf.ToHex(), length.ToString("X")
                        , payload.ToHex(DroneCAN.DroneCAN.dlcToDataLength(length)));

                    port.AppendBuffer(ASCIIEncoding.ASCII.GetBytes(ans2));
                }

                return true;
            }, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, true);

            SetupSLCanPort(port);
        }

        public void startslcan(byte canport)
        {
            but_slcanmode1.Enabled = false;
            but_mavlinkcanmode2.Enabled = false;
            but_mavlinkcanmode2_2.Enabled = false;
            but_filter.Enabled = false;

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
                        target_system = (byte)MainV2.comPort.sysidcurrent,
                        target_component = (byte)MainV2.comPort.compidcurrent,
                        param_type = (byte)MainV2.comPort
                            .MAVlist[(byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent]
                            .param_types[paramname],
                        param_id = paramname.MakeBytesSize(16)
                    };
                    MainV2.comPort.sendPacket(req, (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent);
                    MainV2.comPort.sendPacket(req, (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent);
                }
            }
            catch
            {

            }

            // grab the connected port
            var port = MainV2.comPort.BaseStream;
            // place an invalid port in its place
            if (port != null)
                MainV2.comPort.BaseStream = new Comms.SerialPort()
                    { PortName = port.PortName, BaudRate = port.BaudRate };
            SetupSLCanPort(port);

        }

        private void SetupSLCanPort(ICommsSerial port)
        {
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
                        port = new SerialPort()
                        {
                            PortName = MainV2._connectionControl.CMB_serialport.Text,
                            BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text)
                        };
                        break;
                }
            }

            if (can == null)
                can = new DroneCAN.DroneCAN();

            can.SourceNode = 127;

            can.NodeAdded += (id, msg) =>
            {
                this.BeginInvoke((Action)delegate
                {
                    allnodes.Add(new DroneCANModel()
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
                if (msg.GetType() == typeof(DroneCAN.DroneCAN.uavcan_protocol_NodeStatus))
                {
                    var ns = msg as DroneCAN.DroneCAN.uavcan_protocol_NodeStatus;

                    var nodes = allnodes.Where((a) => a.ID == frame.SourceNode);

                    if (nodes.Count() > 0 && nodes.First().Name == "?")
                    {
                        var statetracking = new DroneCAN.DroneCAN.statetracking();
                        // get node info
                        DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req gnireq =
                            new DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req() { };
                        gnireq.encode(DroneCAN.DroneCAN.dronecan_transmit_chunk_handler, statetracking);

                        var slcan = can.PackageMessageSLCAN(frame.SourceNode, 30, can.TransferID++, gnireq);
                        can.WriteToStreamSLCAN(slcan);
                    }

                    foreach (var item in nodes)
                    {
                        switch (ns.health)
                        {
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK:
                                item.Health = "OK";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_HEALTH_WARNING:
                                item.Health = "WARNING";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_HEALTH_ERROR:
                                item.Health = "ERROR";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_HEALTH_CRITICAL:
                                item.Health = "CRITICAL";
                                break;
                        }

                        switch (ns.mode)
                        {
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL:
                                item.Mode = "OPERATIONAL";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_MODE_INITIALIZATION:
                                item.Mode = "INITIALIZATION";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_MODE_MAINTENANCE:
                                item.Mode = "MAINTENANCE";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE:
                                item.Mode = "SOFTWARE_UPDATE";
                                break;
                            case (byte)DroneCAN.DroneCAN.uavcan_protocol_NodeStatus
                                .UAVCAN_PROTOCOL_NODESTATUS_MODE_OFFLINE:
                                item.Mode = "OFFLINE";
                                break;
                        }

                        item.Uptime = TimeSpan.FromSeconds(ns.uptime_sec);
                    }

                    _updatePending = true;
                }
                else if (msg.GetType() == typeof(DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res))
                {
                    var gnires = msg as DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_res;

                    var nodes = allnodes.Where((a) => a.ID == frame.SourceNode);

                    foreach (var item in nodes)
                    {
                        item.Name = ASCIIEncoding.ASCII.GetString(gnires.name, 0, gnires.name_len);
                        item.HardwareVersion = gnires.hardware_version.major + "." + gnires.hardware_version.minor;
                        item.SoftwareVersion = gnires.software_version.major + "." + gnires.software_version.minor +
                                               "." +
                                               gnires.software_version.vcs_commit.ToString("X");
                        item.SoftwareCRC = gnires.software_version.image_crc;
                        item.HardwareUID = gnires.hardware_version.unique_id.Select(a => a.ToString("X2"))
                            .Aggregate((a, b) => { return a + " " + b; });
                        item.RawMsg = gnires;
                        item.VSC = gnires.status.vendor_specific_status_code;
                    }

                    _updatePending = true;
                }
                else if (msg.GetType() == typeof(DroneCAN.DroneCAN.uavcan_protocol_debug_LogMessage))
                {
                    var debug = msg as DroneCAN.DroneCAN.uavcan_protocol_debug_LogMessage;

                    this.BeginInvoke((Action)delegate()
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

        DroneCAN.DroneCAN can = new DroneCAN.DroneCAN();
        private bool _updatePending;
        private Timer timer;
        private TcpListener listener;
        private bool mavlinkCANRun;
        private byte BusInUse;

        private void myDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0) return;

            try
            {
                if (e.ColumnIndex == myDataGridView1.Columns["Menu"].Index)
                {
                    contextMenu1.Show(myDataGridView1, myDataGridView1.PointToClient(Control.MousePosition));
                }
            }
            catch
            {

            }
        }

        private void FirmwareUpdate(byte nodeID, bool beta = false)
        {
            ProgressReporterDialogue prd = new ProgressReporterDialogue();
            DroneCAN.DroneCAN.FileSendProgressArgs filesend = (id, file, percent) =>
            {
                prd.UpdateProgressAndStatus((int)percent, id + " " + file);
            };
            can.FileSendProgress += filesend;
            var devicename = can.GetNodeName(nodeID);
            var hwversion =
                double.Parse(
                    can.NodeInfo[nodeID].hardware_version.major + "." + can.NodeInfo[nodeID].hardware_version.minor,
                    CultureInfo.InvariantCulture);

            if (CustomMessageBox.Show("Do you want to search the internet for an update?", "Update",
                    CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                var url = can.LookForUpdate(devicename, hwversion, beta);

                if (url == string.Empty)
                    url = APFirmware.Manifest.Firmware.Where(a => a.MavFirmwareVersionType == (beta ? APFirmware.RELEASE_TYPES.BETA.ToString() : APFirmware.RELEASE_TYPES.OFFICIAL.ToString()) &&
                    a.VehicleType == "AP_Periph" && a.Format == "bin" &&
                    a.MavType == "CAN_PERIPHERAL" &&
                    devicename.EndsWith(a.Platform)).First()?.Url.ToString();

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

                            DroneCAN.DroneCAN.FileSendCompleteArgs file = (p, s) =>
                            {
                                prd.UpdateProgressAndStatus(100, "File send complete");
                            };
                            DroneCAN.DroneCAN.FileSendProgressArgs fileprog = (n, f, p) =>
                            {
                                prd.UpdateProgressAndStatus((int)p, f);
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
                fd.Filter = "*.bin;*.apj|*.bin;*.apj";
                var dia = fd.ShowDialog();

                if (fd.CheckFileExists && dia == DialogResult.OK)
                {
                    DroneCAN.DroneCAN.FileSendCompleteArgs file = (p, s) =>
                    {
                        prd.UpdateProgressAndStatus(100, "File send complete");
                    };
                    DroneCAN.DroneCAN.FileSendProgressArgs fileprog = (n, f, p) =>
                    {
                        prd.UpdateProgressAndStatus((int)p, f);
                    };
                    can.FileSendComplete += file;
                    can.FileSendProgress += fileprog;

                    try
                    {
                        if (fd.FileName.ToLower().EndsWith(".apj"))
                        {
                            var fw = px4uploader.Firmware.ProcessFirmware(fd.FileName);
                            var tmp = Path.GetTempFileName();
                            File.WriteAllBytes(tmp, fw.imagebyte);
                            fd.FileName = tmp;
                        }

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
            List<DroneCAN.DroneCAN.uavcan_protocol_param_GetSet_res> paramlist =
                new List<DroneCAN.DroneCAN.uavcan_protocol_param_GetSet_res>();
            prd.doWorkArgs.ForceExit = true;
            prd.doWorkArgs.CancelRequestChanged += (sender2, args) => { prd.doWorkArgs.CancelAcknowledged = true; };
            prd.DoWork += dialogue => { paramlist = can.GetParameters(nodeID); };
            prd.UpdateProgressAndStatus(-1, Strings.GettingParams);
            prd.RunBackgroundOperationAsync();

            if (!prd.doWorkArgs.CancelRequested)
                new DroneCANParams(can, nodeID, paramlist).ShowUserControl();
        }

        public void Deactivate()
        {
            mavlinkCANRun = false;
            try
            {
                if (listener != null)
                    listener.Stop();
            }
            catch
            {
            }

            listener = null;
            can?.Stop(chk_canonclose.Checked);
            can = null;
            timer?.Stop();
        }

        private void myDataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            myDataGridView1[Menu.Index, e.RowIndex].Value = "Menu";
        }


        private void But_uavcaninspector_Click(object sender, EventArgs e)
        {
            if (can == null)
            {
                CustomMessageBox.Show(Strings.PleaseConnect);
                return;
            }

            new DroneCANInspector(can).Show();
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
                        listener.Start(1);

                        int tcpbps = 0;
                        int rtcmbps = 0;
                        int combps = 0;
                        int second = 0;

                        while (true)
                        {
                            var client = listener.AcceptTcpClient();
                            client.NoDelay = true;

                            var st = client.GetStream();

                            DroneCAN.DroneCAN.MessageRecievedDel mrd = (frame, msg, id) =>
                            {
                                combps += frame.SizeofEntireMsg;
                                if (frame.MsgTypeID == DroneCAN.DroneCAN.uavcan_equipment_gnss_RTCMStream
                                        .UAVCAN_EQUIPMENT_GNSS_RTCMSTREAM_DT_ID)
                                {
                                    var data = msg as DroneCAN.DroneCAN.uavcan_equipment_gnss_RTCMStream;
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

                                if (frame.MsgTypeID == DroneCAN.DroneCAN.ardupilot_gnss_MovingBaselineData
                                        .ARDUPILOT_GNSS_MOVINGBASELINEDATA_DT_ID)
                                {
                                    var data = msg as DroneCAN.DroneCAN.ardupilot_gnss_MovingBaselineData;
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
                                        var slcan = can.PackageMessageSLCAN(0, 30, can.TransferID++,
                                            new DroneCAN.DroneCAN.uavcan_equipment_gnss_RTCMStream()
                                                { protocol_id = 3, data = buffer, data_len = (byte)read });
                                        can.WriteToStreamSLCAN(slcan);
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
            FirmwareUpdate(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value
                .ToString()));
        }

        private void menu_parameters_Click(object sender, EventArgs e)
        {
            GetParameters(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value
                .ToString()));
        }

        private void menu_restart_Click(object sender, EventArgs e)
        {
            can.RestartNode(byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value
                .ToString()));
        }

        private void menu_updatebeta_Click(object sender, EventArgs e)
        {
            FirmwareUpdate(
                byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString()), true);
        }

        private void but_slcanmode2_2_Click(object sender, EventArgs e)
        {
            mavlinkCANRun = false;
            StartMavlinkCAN(2);
        }

        private void but_filter_Click(object sender, EventArgs e)
        {
            List<ushort> defaultfilter = new List<ushort>
            {
                (UInt16)0,
                DroneCAN.DroneCAN.uavcan_protocol_NodeStatus.UAVCAN_PROTOCOL_NODESTATUS_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_GetNodeInfo_req.UAVCAN_PROTOCOL_GETNODEINFO_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_RestartNode_req.UAVCAN_PROTOCOL_RESTARTNODE_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_param_GetSet_req.UAVCAN_PROTOCOL_PARAM_GETSET_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_param_ExecuteOpcode_req.UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_file_BeginFirmwareUpdate_req
                    .UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_file_Read_req.UAVCAN_PROTOCOL_FILE_READ_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_file_GetInfo_req.UAVCAN_PROTOCOL_FILE_GETINFO_REQ_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_dynamic_node_id_Allocation
                    .UAVCAN_PROTOCOL_DYNAMIC_NODE_ID_ALLOCATION_DT_ID,
                DroneCAN.DroneCAN.uavcan_protocol_debug_LogMessage.UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_ID,
            };

            var list = DroneCAN.DroneCAN.MSG_INFO.Select(a => (a.msgid, a.type.Name)).OrderBy(a => a.Name.ToLower());

            var paneltop = new Panel() { Width = (int)(320 * 2.3), Height = 600 };

            var panel = new FlowLayoutPanel() { Text = "DroneCAN Messages", AutoScroll = true, Dock = DockStyle.Fill };

            paneltop.Controls.Add(panel);

            var cball = new CheckBox() { Text = "ALL", Width = 320 };
            cball.CheckedChanged += (s, e2) =>
            {
                // update custom
                MAVLink.mavlink_can_filter_modify_t filter2 = new MAVLink.mavlink_can_filter_modify_t(
                    defaultfilter.ToArray().MakeSize(16), (byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent, BusInUse, (byte)MAVLink.CAN_FILTER_OP.CAN_FILTER_REPLACE, 0);

                if (mavlinkCANRun)
                {
                    try
                    {
                        MainV2.comPort.sendPacket(filter2, (byte)MainV2.comPort.sysidcurrent,
                            (byte)MainV2.comPort.compidcurrent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            };
            panel.Controls.Add(cball);

            panel.Controls.AddRange(list.Select(a =>
            {
                var cb = new CheckBox()
                    { Name = a.Name, Text = a.Name, Checked = defaultfilter.Contains(a.msgid), Width = 320 };
                cb.CheckedChanged += (s, e2) =>
                {
                    if (cb.Checked)
                    {
                        if (!defaultfilter.Contains(a.msgid))
                            defaultfilter.Add(a.msgid);
                    }
                    else
                    {
                        defaultfilter.Remove(a.msgid);
                    }

                    // update custom
                    MAVLink.mavlink_can_filter_modify_t filter2 = new MAVLink.mavlink_can_filter_modify_t(
                        defaultfilter.ToArray().MakeSize(16), (byte)MainV2.comPort.sysidcurrent,
                        (byte)MainV2.comPort.compidcurrent, BusInUse, (byte)MAVLink.CAN_FILTER_OP.CAN_FILTER_REPLACE,
                        (byte)defaultfilter.Count());

                    if (mavlinkCANRun)
                    {
                        try
                        {
                            MainV2.comPort.sendPacket(filter2, (byte)MainV2.comPort.sysidcurrent,
                                (byte)MainV2.comPort.compidcurrent);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                };
                return cb;
            }).ToArray());

            var frm = paneltop.ShowUserControl();

            frm.Invalidate();
        }

        private void menu_passthrough4_Click(object sender, EventArgs e)
        {
            if (listener != null)
            {
                menu_passthrough4.Checked = false;
                listener.Stop();
                CustomMessageBox.Show("Stop", "Disabled forwarding");
                listener = null;
                return;
            }

            var port = 500;
            var baudrate = 230400;
            var target_node =
                byte.Parse(myDataGridView1.CurrentRow.Cells[iDDataGridViewTextBoxColumn.Index].Value.ToString());
            if (InputBox.Show("Enter TCP Port", "Enter TCP Port", ref port) != DialogResult.OK)
            {
                return;
            }

            if (InputBox.Show("Enter Baudrate", "Enter Baudrate", ref baudrate) != DialogResult.OK)
            {
                return;
            }

            menu_passthrough4.Checked = true;

            Task.Run(() =>
            {
                try
                {

                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start(1);

                    int tcpbps = 0;
                    int tunnelbps = 0;
                    int combps = 0;
                    int second = 0;

                    {
                        var bauds = new[] { 9600, 38400, 57600, 115200, 230400, 460800 };

                        foreach (var baud in bauds)
                        {
                            var packet = Ubx.generate(0x6, 0x00, new byte[]
                            {
                                0x01, 0x00, 0x00, 0x00, 0xD0, 0x08, 0x00, 0x00,
                                (byte)(baudrate & 0xff),
                                (byte)((baudrate >> 8) & 0xff),
                                (byte)((baudrate >> 16) & 0xff), 0x00, 0x23, 0x00, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00
                            });

                            packet = new byte[] { 0x55, 0x55 }.Concat(packet).ToArray();

                            var slcan = can.PackageMessageSLCAN(0, 30, can.TransferID++,
                                new DroneCAN.DroneCAN.uavcan_tunnel_Targetted()
                                {
                                    protocol = new uavcan_tunnel_Protocol()
                                    {
                                        protocol = (byte)uavcan_tunnel_Protocol
                                            .UAVCAN_TUNNEL_PROTOCOL_GPS_GENERIC
                                    },
                                    target_node = target_node,
                                    serial_id = -1,
                                    baudrate = (uint)baud,
                                    options = (byte)uavcan_tunnel_Targetted
                                        .UAVCAN_TUNNEL_TARGETTED_OPTION_LOCK_PORT,
                                    buffer_len = (byte)packet.Length,
                                    buffer = packet
                                });
                            can.WriteToStreamSLCAN(slcan);
                            Thread.Sleep(100);
                        }
                    }

                    while (true)
                    {
                        using (var client = listener.AcceptTcpClient())
                        {
                            client.NoDelay = true;

                            using (var st = client.GetStream())
                            {
                                DroneCAN.DroneCAN.MessageRecievedDel mrd = (frame, msg, id) =>
                                {
                                    combps += frame.SizeofEntireMsg;
                                    if (frame.MsgTypeID == DroneCAN.DroneCAN.uavcan_tunnel_Targetted
                                            .UAVCAN_TUNNEL_TARGETTED_DT_ID)
                                    {
                                        var data = msg as DroneCAN.DroneCAN.uavcan_tunnel_Targetted;
                                        if (frame.SourceNode != target_node)
                                        {
                                            // not addressed to us
                                            return;
                                        }

                                        if (data.target_node != can.SourceNode)
                                        {
                                            // not for us
                                            return;
                                        }

                                        try
                                        {
                                            tunnelbps += data.buffer_len;
                                            st.Write(data.buffer, 0, data.buffer_len);
                                            st.Flush();
                                        }
                                        catch
                                        {
                                            return;
                                        }
                                    }
                                };

                                can.MessageReceived -= mrd;
                                can.MessageReceived += mrd;

                                try
                                {
                                    var lastsend = DateTime.MinValue;
                                    while (client.Connected)
                                    {
                                        if (listener == null)
                                        {
                                            if (can != null)
                                                can.MessageReceived -= mrd;
                                            client.Close();
                                            return;
                                        }

                                        if (client.Available > 0)
                                        {
                                            var toread = Math.Min(client.Available, 120);
                                            byte[] buffer = new byte[toread];
                                            var read = st.Read(buffer, 0, toread);
                                            /*foreach (var b in buffer)
                                    {
                                        Console.Write("0x{0:X} ", b);
                                    }

                                    Console.WriteLine();*/
                                            tcpbps += read;
                                            var slcan = can.PackageMessageSLCAN(0, 30, can.TransferID++,
                                                new DroneCAN.DroneCAN.uavcan_tunnel_Targetted()
                                                {
                                                    protocol = new uavcan_tunnel_Protocol()
                                                    {
                                                        protocol = (byte)uavcan_tunnel_Protocol
                                                            .UAVCAN_TUNNEL_PROTOCOL_GPS_GENERIC
                                                    },
                                                    target_node = target_node, serial_id = -1,
                                                    baudrate = (uint)baudrate,
                                                    options = (byte)uavcan_tunnel_Targetted
                                                        .UAVCAN_TUNNEL_TARGETTED_OPTION_LOCK_PORT,
                                                    buffer = buffer, buffer_len = (byte)read
                                                });
                                            can.WriteToStreamSLCAN(slcan);
                                            lastsend = DateTime.Now;
                                        }
                                        else
                                            Thread.Sleep(1);

                                        if ((DateTime.Now - lastsend).TotalSeconds >= 0.5)
                                        {
                                            var slcan = can.PackageMessageSLCAN(0, 30, can.TransferID++,
                                                new DroneCAN.DroneCAN.uavcan_tunnel_Targetted()
                                                {
                                                    protocol = new uavcan_tunnel_Protocol()
                                                    {
                                                        protocol = (byte)uavcan_tunnel_Protocol
                                                            .UAVCAN_TUNNEL_PROTOCOL_GPS_GENERIC
                                                    },
                                                    target_node = target_node,
                                                    serial_id = -1,
                                                    baudrate = (uint)baudrate,
                                                    options = (byte)uavcan_tunnel_Targetted
                                                        .UAVCAN_TUNNEL_TARGETTED_OPTION_LOCK_PORT,
                                                    buffer_len = 0,
                                                    buffer = Array.Empty<byte>()
                                                });
                                            can.WriteToStreamSLCAN(slcan);
                                            lastsend = DateTime.Now;
                                        }

                                        if (second != DateTime.Now.Second)
                                        {
                                            Console.WriteLine("tcp:{0} can:{1} tunn:{2} avail:{3}", tcpbps, combps,
                                                tunnelbps,
                                                client.Available);
                                            tcpbps = combps = tunnelbps = 0;
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
                    }
                }
                catch (Exception exception)
                {
                    CustomMessageBox.Show(Strings.ERROR, "Forwarder problem " + exception.ToString());
                    if (listener != null)
                        listener.Stop();
                    listener = null;
                    this.InvokeIfRequired(() => { menu_passthrough4.Checked = true; });
                }
            });
        }

        public class CanStats : dronecan_protocol_CanStats
        {
            public int NodeID { get; set; }

            public CanStats(byte NodeID, dronecan_protocol_CanStats data)
            {
                this.NodeID = NodeID;
                
                busoff_errors = data.busoff_errors;
                rx_errors = data.rx_errors;
                @interface = data.@interface;
                tx_requests = data.tx_requests;
                tx_rejected = data.tx_rejected;
                tx_overflow = data.tx_overflow;
                tx_success = data.tx_success;
                tx_timedout = data.tx_timedout;
                tx_abort = data.tx_abort;
                rx_overflow= data.rx_overflow;
                rx_received= data.rx_received;
            }

            public new byte @interface
            {
                get { return base.@interface; }
                set { base.@interface = value; }
            }

            public new uint tx_requests
            {
                get { return base.tx_requests; }
                set { base.tx_requests = value; }
            }

            public new ushort tx_rejected
            {
                get { return base.tx_rejected; }
                set { base.tx_rejected = value; }
            }

            public new ushort tx_overflow
            {
                get { return base.tx_overflow; }
                set { base.tx_overflow = value; }
            }

            public new ushort tx_success
            {
                get { return base.tx_success; }
                set { base.tx_success = value; }
            }

            public new ushort tx_timedout
            {
                get { return base.tx_timedout; }
                set { base.tx_timedout = value; }
            }

            public new ushort tx_abort
            {
                get { return base.tx_abort; }
                set { base.tx_abort = value; }
            }

            public new uint rx_received
            {
                get { return base.rx_received; }
                set { base.rx_received = value; }
            }

            public new ushort rx_overflow
            {
                get { return base.rx_overflow; }
                set { base.rx_overflow = value; }
            }

            public new ushort rx_errors
            {
                get { return base.rx_errors; }
                set { base.rx_errors = value; }
            }

            public new ushort busoff_errors
            {
                get { return base.busoff_errors; }
                set { base.busoff_errors = value; }
            }
        }

        public class Stats : dronecan_protocol_Stats
        {
            public int NodeID { get; set; }

            public Stats(byte NodeID, dronecan_protocol_Stats data) : base()
            {
                this.NodeID = NodeID;
                tx_frames = data.tx_frames;
                tx_errors = data.tx_errors;
                rx_frames = data.rx_frames;
                rx_error_oom = data.rx_error_oom;
                rx_error_internal = data.rx_error_internal;
                rx_error_missed_start = data.rx_error_missed_start;
                rx_error_bad_crc = data.rx_error_bad_crc;
                rx_error_wrong_toggle = data.rx_error_wrong_toggle;
                rx_error_short_frame = data.rx_error_short_frame;
                rx_ignored_wrong_address = data.rx_ignored_wrong_address;
                rx_ignored_not_wanted = data.rx_ignored_not_wanted;
                rx_ignored_unexpected_tid = data.rx_ignored_unexpected_tid;
            }

            public new uint tx_frames
            {
                get { return base.tx_frames; }
                set { base.tx_frames = value; }
            }

            public new ushort tx_errors
            {
                get { return base.tx_errors; }
                set { base.tx_errors = value; }
            }

            public new uint rx_frames
            {
                get { return base.rx_frames; }
                set { base.rx_frames = value; }
            }

            public new ushort rx_error_oom
            {
                get { return base.rx_error_oom; }
                set { base.rx_error_oom = value; }
            }

            public new ushort rx_error_internal
            {
                get { return base.rx_error_internal; }
                set { base.rx_error_internal = value; }
            }

            public new ushort rx_error_missed_start
            {
                get { return base.rx_error_missed_start; }
                set { base.rx_error_missed_start = value; }
            }

            public new ushort rx_error_wrong_toggle
            {
                get { return base.rx_error_wrong_toggle; }
                set { base.rx_error_wrong_toggle = value; }
            }

            public new ushort rx_error_short_frame
            {
                get { return base.rx_error_short_frame; }
                set { base.rx_error_short_frame = value; }
            }

            public new ushort rx_error_bad_crc
            {
                get { return base.rx_error_bad_crc; }
                set { base.rx_error_bad_crc = value; }
            }

            public new ushort rx_ignored_wrong_address
            {
                get { return base.rx_ignored_wrong_address; }
                set { base.rx_ignored_wrong_address = value; }
            }

            public new ushort rx_ignored_not_wanted
            {
                get { return base.rx_ignored_not_wanted; }
                set { base.rx_ignored_not_wanted = value; }
            }

            public new ushort rx_ignored_unexpected_tid
            {
                get { return base.rx_ignored_unexpected_tid; }
                set { base.rx_ignored_unexpected_tid = value; }
            }
        }

        private void but_stats_Click(object sender, EventArgs e)
        {
            DataGridView dgvcanstats = new DataGridView();
            dgvcanstats.AutoGenerateColumns = true;

            DataGridView dgvstats = new DataGridView();
            dgvstats.AutoGenerateColumns = true;

            var list1 = new BindingList<CanStats>();
            var list2 = new BindingList<Stats>();

            DroneCAN.DroneCAN.MessageRecievedDel mrd = (frame, msg, id) =>
            {
                if (frame.MsgTypeID == DroneCAN.DroneCAN.dronecan_protocol_CanStats.DRONECAN_PROTOCOL_CANSTATS_DT_ID)
                {
                    var data = msg as dronecan_protocol_CanStats;
                    var d1 = new CanStats(frame.SourceNode, data);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (list1.Any(a => a.@interface == d1.@interface && a.NodeID == frame.SourceNode))
                            list1[list1.IndexOf(
                                list1.First(a => a.@interface == d1.@interface && a.NodeID == frame.SourceNode))] = d1;
                        else
                        {

                            list1.Add(d1);

                        }
                    }));
                }
                else if (frame.MsgTypeID == DroneCAN.DroneCAN.dronecan_protocol_Stats.DRONECAN_PROTOCOL_STATS_DT_ID)
                {
                    var data = msg as dronecan_protocol_Stats;
                    var d1 = new Stats(frame.SourceNode, data);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (list2.Any(a => a.NodeID == frame.SourceNode))
                            list2[list2.IndexOf(list2.First(a => a.NodeID == frame.SourceNode))] = d1;
                        else
                        {
                            list2.Add(d1);
                        }
                    }));
                }
            };


            Panel p = new Panel()
            {
                Width = (int)(Screen.PrimaryScreen.WorkingArea.Width / 1.2),
                Height = (int)(Screen.PrimaryScreen.WorkingArea.Height / 1.2)
            };
            dgvcanstats.Width = p.Width;
            dgvcanstats.Height = p.Height / 2;
            dgvstats.Width = p.Width;
            dgvstats.Height = p.Height / 2;
            dgvstats.Location = new Point(0, p.Height / 2);
            dgvcanstats.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvstats.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            p.Controls.Add(dgvcanstats);
            p.Controls.Add(dgvstats);
            var frm = p.ShowUserControl();
            frm.Text = "Stats";
            frm.StartPosition = FormStartPosition.CenterScreen;

            can.MessageReceived += mrd;

            var bs1 = new BindingSource();
            bs1.DataSource = list1;
            dgvcanstats.DataSource = bs1;

            var bs2 = new BindingSource();
            bs2.DataSource = list2;
            dgvstats.DataSource = bs2;

            frm.Refresh();

            frm.Closing += (o, args) => { can.MessageReceived -= mrd; };
        }
    }
}