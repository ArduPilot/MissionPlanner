using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using size_t = System.Int32;

namespace UAVCAN
{
    public partial class uavcan
    {
        public class statetracking
        {
            public byte[] data = new byte[14];
            int _bit = 0;
            public int bit { get { return _bit; } set { _bit = value; if (data.Length < ((_bit + 7) / 8)+7 ) Array.Resize(ref data, data.Length + 7); } }

            public byte[] ToBytes()
            {
                return data.Take((bit + 7) / 8).ToArray();
            }
        }

        public static bool testconversion<T>(T input, byte bitlength, bool signed, byte bit_offset = 0) where T : struct
        {
            var buf = new byte[16];
            var ctx = new statetracking() { bit = bit_offset };
            T ans = input;
            T ans2 = input;
            uavcan.canardEncodeScalar(buf, bit_offset, bitlength, input);
            uavcan_transmit_chunk_handler(buf, bitlength, ctx);
            uavcan.canardEncodeScalar(buf, bit_offset, bitlength, input);
            uavcan_transmit_chunk_handler(buf, bitlength, ctx);

            uavcan.canardDecodeScalar(new uavcan.CanardRxTransfer(ctx.ToBytes()), bit_offset, bitlength, signed, ref ans);
            uavcan.canardDecodeScalar(new uavcan.CanardRxTransfer(ctx.ToBytes()), bit_offset + (uint)bitlength, bitlength, signed, ref ans2);

            if (input.ToString() != ans.ToString())
                throw new Exception();

            if (input.ToString() != ans2.ToString())
                throw new Exception();

            return true;
        }

        public delegate void MessageRecievedDel(CANFrame frame, object msg, byte transferID);

        public event MessageRecievedDel MessageReceived;

        private object sr_lock = new object();
        private Stream sr;
        DateTime uptime = DateTime.Now;

        string ReadLine(Stream st)
        {
            StringBuilder sb = new StringBuilder();

            char cha;

            do
            {
                cha = (char)st.ReadByte();
                if (cha == -1)
                    break;
                sb.Append(cha);
            } while (cha != '\r' && cha != '\a');

            return sb.ToString();
        }

        /// <summary>
        /// Start slcan stream sending a nodestatus packet every second
        /// </summary>
        /// <param name="stream"></param>
        public void StartSLCAN(Stream stream)
        {
            //cleanup
            stream.Write(new byte[] { (byte)'\r' }, 0, 1);
            // close
            stream.Write(new byte[] { (byte)'C', (byte)'\r' }, 0, 2);
            // speed 
            stream.Write(new byte[] { (byte)'S', (byte)'8', (byte)'\r' }, 0, 3);
            //hwid
            stream.Write(new byte[] { (byte)'N', (byte)'\r' }, 0, 2);
            // open
            stream.Write(new byte[] { (byte)'O', (byte)'\r' }, 0, 2);
            // clear status
            stream.Write(new byte[] { (byte)'F', (byte)'\r' }, 0, 2);

            sr = stream;

            // read everything
            Task.Run(() =>
            {
                while (sr.CanRead)
                {                   
                    try
                    {
                        var line = ReadLine(stream);
                        ReadMessage(line);
                    }
                    catch
                    {
                    }
                }
            });

            // 1 second nodestatus send
            Task.Run(() =>
            {
                while (stream.CanWrite)
                {
                    try
                    {
                        if (NodeStatus)
                        {
                            var slcan = PackageMessage(SourceNode, 20, transferID++,
                                new uavcan.uavcan_protocol_NodeStatus()
                                { health = (byte)uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK, mode = (byte)uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL, sub_mode = 0, uptime_sec = (uint)(DateTime.Now - uptime).TotalSeconds, vendor_specific_status_code = 0 });

                            lock (sr_lock)
                                WriteToStream(slcan);
                        }
                    }
                    catch
                    {
                    }
                    Thread.Sleep(1000);

                }
            });

            // build nodelist
            MessageReceived += (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_NodeStatus))
                {
                    if (!NodeList.ContainsKey(frame.SourceNode))
                    {
                        NodeList.Add(frame.SourceNode, msg as uavcan.uavcan_protocol_NodeStatus);
                        NodeAdded?.Invoke(frame.SourceNode, msg as uavcan.uavcan_protocol_NodeStatus);
                    }
                }
                else if (msg.GetType() == typeof(uavcan.uavcan_protocol_GetNodeInfo_req))
                {
                    var gnires = new uavcan.uavcan_protocol_GetNodeInfo_res();
                    gnires.software_version.major = (byte)Assembly.GetExecutingAssembly().GetName().Version.Major;
                    gnires.software_version.minor = (byte)Assembly.GetExecutingAssembly().GetName().Version.Minor;
                    gnires.hardware_version.major = 0;
                    gnires.hardware_version.unique_id = ASCIIEncoding.ASCII.GetBytes("MissionPlanner\x0\x0\x0\x0\x0\x0");
                    gnires.name = ASCIIEncoding.ASCII.GetBytes("org.missionplanner");
                    gnires.name_len = (byte)gnires.name.Length;
                    gnires.status = new uavcan.uavcan_protocol_NodeStatus()
                    { health = (byte)uavcan.UAVCAN_PROTOCOL_NODESTATUS_HEALTH_OK, mode = (byte)uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_OPERATIONAL, sub_mode = 0, uptime_sec = (uint)(DateTime.Now - uptime).TotalSeconds, vendor_specific_status_code = 0 };

                    var slcan = PackageMessage(frame.SourceNode, frame.Priority, transferID, gnires);
                    lock (sr_lock)
                        WriteToStream(slcan);
                }
            };
        }

        public void Stop(bool closestream = true)
        {
            if (sr != null && closestream)
            {
                // close
                try
                {
                    sr.Write(new byte[] { (byte)'C', (byte)'\r' }, 0, 2);
                    sr.Close();
                } catch { }
            }
        }

        public void RequestFile(byte nodeID, string filename)
        {
            bool reading = true;
            int gotbytes = 0;

            var req = new uavcan.uavcan_protocol_file_Read_req()
            {
                offset = 0,
                path = new uavcan.uavcan_protocol_file_Path()
                {
                    path = ASCIIEncoding.ASCII.GetBytes(filename),
                    path_len = (byte)filename.Length
                }
            };

            MessageRecievedDel reqfile = (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_file_Read_res))
                {
                    var readres = msg as uavcan.uavcan_protocol_file_Read_res;

                    //readres.data

                    req.offset += readres.data_len;

                    var slcan2 = PackageMessage(nodeID, 20, transferID, req);
                    lock (sr_lock)
                        WriteToStream(slcan2);
                }
                else if (msg.GetType() == typeof(uavcan.uavcan_protocol_debug_LogMessage))
                {
                    var debug = msg as uavcan.uavcan_protocol_debug_LogMessage;

                    Console.WriteLine(frame.SourceNode + " " + ASCIIEncoding.ASCII.GetString(debug.text, 0, debug.text_len));
                }
            };
            MessageReceived += reqfile;

            var slcan = PackageMessage(nodeID, 20, transferID, req);
            lock (sr_lock)
                WriteToStream(slcan);

            while (reading)
            {
                Thread.Sleep(3000);

                if(gotbytes == 0)
                    lock (sr_lock)
                        WriteToStream(slcan);
            }
            MessageReceived -= reqfile;
        }


        public bool SaveConfig(byte node)
        {
            //TX  15:23:27.460269 1E0AF9FE    00 00 00 00 00 00 00 C1........    126 121 uavcan.protocol.param.ExecuteOpcode
            //RX	15:23:27.471806	1E0A7EF9	00 00 00 00 00 00 80 C1 	........	121	126	uavcan.protocol.param.ExecuteOpcode

            bool? ok = null;

            MessageRecievedDel configdelgate = (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_param_ExecuteOpcode_res))
                {
                    var exopres = msg as uavcan.uavcan_protocol_param_ExecuteOpcode_res;

                    if (frame.SourceNode != node)
                        return;

                    ok = exopres.ok;
                }
            };

            MessageReceived += configdelgate;

            var req = new uavcan.uavcan_protocol_param_ExecuteOpcode_req() { opcode = (byte)uavcan.UAVCAN_PROTOCOL_PARAM_EXECUTEOPCODE_REQ_OPCODE_SAVE};

            var trys = 0;
            DateTime nextsend = DateTime.MinValue;

            while (!ok.HasValue)
            {
                if (trys > 3)
                    return false;

                if (nextsend < DateTime.Now)
                {
                    var slcan = PackageMessage(node, 30, transferID++, req);

                    WriteToStream(slcan);

                    nextsend = DateTime.Now.AddSeconds(1);
                    trys++;
                }
                Thread.Sleep(20);
            }

            MessageReceived -= configdelgate;

            return ok.Value;
        }

        public List<uavcan.uavcan_protocol_param_GetSet_res> GetParameters(byte node)
        {
            List<uavcan.uavcan_protocol_param_GetSet_res> paramlist = new List<uavcan.uavcan_protocol_param_GetSet_res>();
            ushort index = 0;
            var timeout = DateTime.Now.AddSeconds(2);

            MessageRecievedDel paramdelegate = (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_param_GetSet_res))
                {
                    var getsetreq = msg as uavcan.uavcan_protocol_param_GetSet_res;

                    if (getsetreq.name_len == 0)
                    {
                        timeout = DateTime.MinValue;
                        return;
                    }

                    paramlist.Add(getsetreq);

                    var value = getsetreq.value;

                    var name = ASCIIEncoding.ASCII.GetString(getsetreq.name, 0, getsetreq.name_len);

                    Console.WriteLine("{0}: {1}", name, value);

                    timeout = DateTime.Now.AddSeconds(2);
                    index++;
                }
            };
            MessageReceived += paramdelegate;

            while (true)
            {
                if (DateTime.Now > timeout)
                    break;

                uavcan.uavcan_protocol_param_GetSet_req req = new uavcan.uavcan_protocol_param_GetSet_req()
                {
                    index = index
                };

                var slcan = PackageMessage(node, 30, transferID++, req);

                lock (sr_lock)
                {
                    WriteToStream(slcan);
                }

                Thread.Sleep(333);
            }

            MessageReceived -= paramdelegate;

            _paramlistcache = paramlist;

            return paramlist;
        }

        Dictionary<string, string> fileServerList = new Dictionary<string, string>();

        public void ServeFile(string filetoserve)
        {
            fileServerList[Path.GetFileName(filetoserve.ToLower())] = filetoserve;
        }

        public void SetupFileServer()
        {
            MessageReceived += (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_file_Read_req))
                {
                    var frreq = msg as uavcan.uavcan_protocol_file_Read_req;

                    var requestedfile = ASCIIEncoding.ASCII.GetString(frreq.path.path).TrimEnd('\0');

                    var firmware = fileServerList.Where(a => a.Key == requestedfile);

                    if (firmware.Count() == 0)
                        throw new FileNotFoundException(frame.SourceNode + " " + "File read request for file we are not serving " +
                                  ASCIIEncoding.ASCII.GetString(frreq.path.path).TrimEnd('\0'));

                    using (var file = File.OpenRead(firmware.First().Value))
                    {
                        Console.WriteLine(frame.SourceNode + " " + "file_Read: {0} at {1}", requestedfile, frreq.offset);
                        file.Seek((long)frreq.offset, SeekOrigin.Begin);
                        var buffer = new byte[256];
                        var read = file.Read(buffer, 0, 256);
                        var readRes = new uavcan.uavcan_protocol_file_Read_res()
                        {
                            data = buffer,
                            data_len = (ushort)read,
                            error = new uavcan.uavcan_protocol_file_Error()
                            { value = (short)uavcan.UAVCAN_PROTOCOL_FILE_ERROR_OK }
                        };

                        var slcan = PackageMessage(frame.SourceNode, frame.Priority, transferID, readRes);

                        lock (sr_lock)
                        {
                            WriteToStream(slcan);
                        }

                        if (file.Length == ((long)frreq.offset + read))
                        {
                            FileSendComplete?.Invoke(frame.SourceNode, requestedfile);
                        }
                    }
                }
            };
        }

        List<byte> dynamicBytes = new List<byte>();

        public void SetupDynamicNodeAllocator()
        {
            MessageReceived += (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (frame.TransferType != CANFrame.FrameType.anonymous)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_dynamic_node_id_Allocation))
                {
                    var allocation = msg as uavcan.uavcan_protocol_dynamic_node_id_Allocation;

                    if (allocation.first_part_of_unique_id)
                    {
                        // first part of id
                        allocation.first_part_of_unique_id = false;
                        dynamicBytes.Clear();
                        dynamicBytes.AddRange(allocation.unique_id.Take(allocation.unique_id_len));

                        var slcan = PackageMessage(SourceNode, frame.Priority, transferID, allocation);
                        lock (sr_lock)
                            WriteToStream(slcan);
                    }
                    else
                    {
                        dynamicBytes.AddRange(allocation.unique_id.Take(allocation.unique_id_len));

                        allocation.unique_id = dynamicBytes.ToArray();

                        allocation.unique_id_len = (byte)allocation.unique_id.Length;

                        if (allocation.unique_id_len >= 16)
                        {
                            for (int a = 125; a >= 1; a--)
                            {
                                if (!NodeList.ContainsKey(a))
                                {
                                    allocation.node_id = (byte)a;
                                    Console.WriteLine("Allocate " + a);
                                    break;
                                }
                            }
                            dynamicBytes.Clear();
                        }
                        var slcan = PackageMessage(SourceNode, frame.Priority, transferID, allocation);
                        lock (sr_lock)
                            WriteToStream(slcan);

                    }
                }
            };
        }

        public string LookForUpdate(string devicename, double hwversion)
        {
            string[] servers = new string[] { "http://firmware.cubepilot.org:81/UAVCAN/" };

            foreach (var server in servers)
            {
                var url = String.Format("{0}{1}/{2}/{3}", server, devicename, hwversion.ToString("0.0##"), "firmware.bin");
                Console.WriteLine("LookForUpdate at " + url);
                var req = WebRequest.Create(url);
                req.Timeout = 4000; // miliseconds
                req.Method = "HEAD";

                try
                {
                    var res = (HttpWebResponse)req.GetResponse();
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("LookForUpdate valid url " + url);
                        return url;
                    }
                }
                catch
                {
                }
            }

            return String.Empty;
        }

        public void Update(string devicename, double hwversion, string firmware_name)
        {
            Console.WriteLine("Update {0} {1} {2}", devicename, hwversion, firmware_name);

            ServeFile(firmware_name);

            var firmware_namebytes = ASCIIEncoding.ASCII.GetBytes(Path.GetFileName(firmware_name.ToLower()));
            ulong firmware_crc = ulong.MaxValue;
            Exception exception = null;

            MessageRecievedDel updatedelegate = (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res))
                {
                    var bfures = msg as uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res;
                    if (bfures.error != 0)
                        exception = new Exception(frame.SourceNode + " " + "Begin Firmware Update returned an error");
                }
                else if (msg.GetType() == typeof(uavcan.uavcan_protocol_GetNodeInfo_res))
                {
                    var gnires = msg as uavcan.uavcan_protocol_GetNodeInfo_res;
                    Console.WriteLine(frame.SourceNode + " " + "GetNodeInfo: seen '{0}' from {1}", ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0'), frame.SourceNode);
                    if (devicename == ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0') || devicename == ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0') + "-BL")
                    {
                        if (firmware_crc != gnires.software_version.image_crc || firmware_crc == ulong.MaxValue)
                        {
                            if (hwversion == double.Parse(gnires.hardware_version.major + "." + gnires.hardware_version.minor, CultureInfo.InvariantCulture) || hwversion == 0)
                            {
                                if (gnires.status.mode != uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE)
                                {
                                    var req_msg =
                                        new uavcan.uavcan_protocol_file_BeginFirmwareUpdate_req()
                                        {
                                            image_file_remote_path = new uavcan.uavcan_protocol_file_Path()
                                            { path = firmware_namebytes },
                                            source_node_id = SourceNode
                                        };
                                    req_msg.image_file_remote_path.path_len = (byte)firmware_namebytes.Length;

                                    var slcan = PackageMessage(frame.SourceNode, frame.Priority, transferID, req_msg);
                                    lock (sr_lock)
                                        WriteToStream(slcan);
                                }
                                else
                                {
                                    exception = new Exception(frame.SourceNode + " " + "already in update mode");
                                    return;
                                }
                            }
                            else
                            {
                                exception = new Exception(frame.SourceNode + " " + "hwversion does not match");
                                return;
                            }
                        }
                        else
                        {
                            exception = new Exception(String.Format( "{0} - No need to upload, crc matchs", frame.SourceNode));
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine(frame.SourceNode + " " + "device name does not match {0} vs {1}", devicename, ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0'));
                        return;
                    }
                }
            };
            MessageReceived += updatedelegate;

            // getfile crc
            using (var stream = File.OpenRead(fileServerList[Path.GetFileName(firmware_name.ToLower())]))
            {
                string app_descriptor_fmt = "<8cQI";
                var SHARED_APP_DESCRIPTOR_SIGNATURES = new byte[][] {
                    new byte[] { 0xd7, 0xe4, 0xf7, 0xba, 0xd0, 0x0f, 0x9b, 0xee },
                    new byte[] { 0x40, 0xa2, 0xe4, 0xf1, 0x64, 0x68, 0x91, 0x06 } };

                var app_descriptor_len = 8 * 1 + 8 + 4;

                var location = GetPatternPositions(stream, SHARED_APP_DESCRIPTOR_SIGNATURES[0]);
                stream.Seek(0, SeekOrigin.Begin);
                var location2 = GetPatternPositions(stream, SHARED_APP_DESCRIPTOR_SIGNATURES[1]);

                if(location.Count > 0 || location2.Count > 0)
                {
                    var offset = location.Count > 0 ? location[0] : location2[0];
                    stream.Seek(offset, SeekOrigin.Begin);
                    byte[] buf = new byte[app_descriptor_len];
                    stream.Read(buf, 0, app_descriptor_len);
                    firmware_crc = BitConverter.ToUInt64(buf, 8);
                }
            }

            foreach (var i in NodeList.Keys.ToArray())
            {
                var statetracking = new statetracking();
                // get node info
                uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new uavcan.uavcan_protocol_GetNodeInfo_req() { };
                gnireq.encode(uavcan_transmit_chunk_handler, statetracking);

                var slcan = PackageMessage((byte)i, 30, transferID++, gnireq);
                lock (sr_lock)
                    WriteToStream(slcan);
            }

            int b = 0;
            while (true)
            {
                Thread.Sleep(1000);

                if (exception != null)
                {
                    break;
                }

                if (NodeList.Values.Any(a => a.mode == uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE))
                {

                }
                else
                {
                    b++;
                    if (b > 100)
                        break;
                }
            }

            Console.WriteLine("Update EXIT");

            MessageReceived -= updatedelegate;

            if (exception != null)
            {
                throw exception;
            }
        }

        List<long> GetPatternPositions(Stream stream, byte[] pattern)
        {
            List<long> searchResults = new List<long>(); //The results as offsets within the file
            int patternPosition = 0; //Track of how much of the array has been matched
            long filePosition = 0;
            long bufferSize = Math.Min(stream.Length, 100_000);

            byte[] buffer = new byte[bufferSize];
            int readCount = 0;

            while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < readCount; i++)
                {
                    byte currentByte = buffer[i];

                    if (currentByte == pattern[0])
                        patternPosition = 0;

                    if (currentByte == pattern[patternPosition])
                    {
                        patternPosition++;
                        if (patternPosition == pattern.Length)
                        {
                            searchResults.Add(filePosition + 1 - pattern.Length);
                            patternPosition = 0;
                        }
                    }
                    else
                    {
                        patternPosition = 0;
                    }
                    filePosition++;
                }
            }

            return searchResults;

        }

        public void WriteToStream(string slcan)
        {
            var lines = slcan.Split(new[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                lock (sr_lock)
                {
                    sr.Write(ASCIIEncoding.ASCII.GetBytes(line + '\r'), 0, line.Length + 1);
                    sr.Flush();
                }
                // var ans = sr.Peek();
                // Console.WriteLine((char)ans);
            }
        }

        public string PackageMessage(byte destNode, byte priority, byte transferID, IUAVCANSerialize msg)
        {
            var state = new statetracking();
            msg.encode(uavcan_transmit_chunk_handler, state);

            var msgtype = uavcan.MSG_INFO.First(a => a.Item1 == msg.GetType());

            CANFrame cf = new CANFrame(new byte[4]);
            cf.SourceNode = SourceNode;
            cf.Priority = priority;

            if (msg.GetType().FullName.EndsWith("_res") || msg.GetType().FullName.EndsWith("_req"))
            {
                // service
                cf.IsServiceMsg = true;
                cf.SvcDestinationNode = destNode;
                cf.SvcIsRequest = msg.GetType().FullName.EndsWith("_req") ? true : false;
                cf.SvcTypeID = (byte)msgtype.Item2;
            }
            else
            {
                // message
                cf.MsgTypeID = (ushort)msgtype.Item2;
            }

            string ans = "";

            var payloaddata = state.ToBytes();

            if (payloaddata.Length > 7)
            {
                var dt_sig = BitConverter.GetBytes(msgtype.Item3);

                var crcprocess = new TransferCRC();
                crcprocess.add(dt_sig, 8);
                crcprocess.add(payloaddata, payloaddata.Length);
                var crc = crcprocess.get();

                var buffer = new byte[8];
                var toogle = false;
                var size = 7;
                for (int a = 0; a < payloaddata.Length; a += size)
                {
                    if (a == 0)
                    {
                        buffer[0] = (byte)(crc & 0xff);
                        buffer[1] = (byte)(crc >> 8);
                        size = 5;
                        Array.ConstrainedCopy(payloaddata, a, buffer, 2, 5);
                    }
                    else
                    {
                        size = payloaddata.Length - a <= 7 ? payloaddata.Length - a : 7;
                        Array.ConstrainedCopy(payloaddata, a, buffer, 0, size);
                        if (buffer.Length != size + 1)
                            Array.Resize(ref buffer, size + 1);
                    }
                    CANPayload payload = new CANPayload(buffer);
                    payload.SOT = a == 0 ? true : false;
                    payload.EOT = a + size == payloaddata.Length ? true : false;
                    payload.TransferID = (byte)transferID;
                    payload.Toggle = toogle;
                    toogle = !toogle;

                    ans += String.Format("T{0}{1}{2}\r", cf.ToHex(), a == 0 ? 8 : size + 1, payload.ToHex());
                }
            }
            else
            {
                var buffer = new byte[payloaddata.Length + 1];
                Array.Copy(payloaddata, buffer, payloaddata.Length);
                CANPayload payload = new CANPayload(buffer);
                payload.SOT = payload.EOT = true;
                payload.TransferID = (byte)transferID;

                ans = String.Format("T{0}{1}{2}\r", cf.ToHex(), buffer.Length, payload.ToHex());
            }

            //Console.Write("TX " + ans.Replace("\r", "\r\n"));
            //Console.WriteLine("TX " + msgtype.Item1 + " " + JsonConvert.SerializeObject(msg));
            return ans;
        }

        Dictionary<(uint, int), List<byte>> transfer = new Dictionary<(uint, int), List<byte>>();

        public byte SourceNode { get; set; } = 127;
        public bool NodeStatus { get; set; } = true;

        public delegate void FileSendCompleteArgs(byte NodeID, string file);

        public event FileSendCompleteArgs FileSendComplete;

        public Dictionary<int, uavcan_protocol_NodeStatus> NodeList = new Dictionary<int,uavcan_protocol_NodeStatus>();

        public delegate void NodeAddedArgs(byte NodeID, uavcan.uavcan_protocol_NodeStatus nodeStatus);
        public event NodeAddedArgs NodeAdded;

        private byte transferID = 0;
        private List<uavcan.uavcan_protocol_param_GetSet_res> _paramlistcache;

        public uavcan(Byte sourceNode)
        {
            SourceNode = sourceNode;
            test();
        }

        public uavcan()
        {
            test();
        }

        public static void test()
        {
            /*
RX	09:19:57.327377	08042479	EB 3C 00 00 00 00 00 86 	.<......	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	00 00 30 77 4F D6 17 26 	..0wO..&	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	84 05 5F 00 0E BE CF 06 	.._.....	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	B1 15 84 23 F1 CF CA 26 	...#...&	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	F0 3F FD 21 D0 00 8D 06 	.?.!....	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	2B 42 B0 A0 B6 1F 6C 26 	+B....l&	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	40 F9 E2 50 00 00 00 06 	@..P....	121		uavcan.equipment.gnss.Fix
RX	09:19:57.327377	08042479	00 00 00 E2 50 00 00 26 	....P..&	121		uavcan.equipment.gnss.Fix
RX	09:19:57.328377	08042479	00 00 00 00 16 59 69 06 	.....Yi.	121		uavcan.equipment.gnss.Fix
RX	09:19:57.328377	08042479	3F 00 00 00 00 00 00 26 	?......&	121		uavcan.equipment.gnss.Fix
RX	09:19:57.328377	08042479	69 3F 00 00 00 00 00 06 	i?......	121		uavcan.equipment.gnss.Fix
RX	09:19:57.328377	08042479	00 69 3F 66             	.i?f	121		uavcan.equipment.gnss.Fix
timestamp: 
  usec: 0 # UNKNOWN
gnss_timestamp: 
  usec: 1552612798199600
gnss_time_standard: 2 # UTC
num_leap_seconds: 0 # UNKNOWN
longitude_deg_1e8: 11573116430
latitude_deg_1e8: -3330374480
height_ellipsoid_mm: -16341
height_msl_mm: 15012
ned_velocity: [0.0590, -0.1331, -0.4141]
sats_used: 7
status: 3 # 3D_FIX
pdop: 2.2109
position_covariance: [39.0625, 0.0000, 0.0000, 0.0000, 39.0625, 0.0000, 0.0000, 0.0000, 162.7500]
velocity_covariance: [1.8525, 0.0000, 0.0000, 0.0000, 1.8525, 0.0000, 0.0000, 0.0000, 1.8525]
             */
            var datafix = new CanardRxTransfer(new byte[] {0xEB, 0x3C, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x00, 0x30, 0x77, 0x4F, 0xD6, 0x17,
0x84, 0x05, 0x5F, 0x00, 0x0E, 0xBE, 0xCF,
0xB1, 0x15, 0x84, 0x23, 0xF1, 0xCF, 0xCA,
0xF0, 0x3F, 0xFD, 0x21, 0xD0, 0x00, 0x8D,
0x2B, 0x42, 0xB0, 0xA0, 0xB6, 0x1F, 0x6C,
0x40, 0xF9, 0xE2, 0x50, 0x00, 0x00, 0x00,
0x00, 0x00, 0x00, 0xE2, 0x50, 0x00, 0x00,
0x00, 0x00, 0x00, 0x00, 0x16, 0x59, 0x69,
0x3F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
0x69, 0x3F, 0x00, 0x00, 0x00, 0x00, 0x00,
0x00, 0x69, 0x3F}.Skip(2).ToArray());

            var fixdecodetest = new uavcan.uavcan_equipment_gnss_Fix();
            fixdecodetest.decode(datafix);

            var fix = new uavcan.uavcan_equipment_gnss_Fix()
            {
                timestamp = new uavcan.uavcan_Timestamp() { usec = 0 },
                gnss_timestamp = new uavcan.uavcan_Timestamp() { usec = 1552612798199600 },
                gnss_time_standard = 2,
                num_leap_seconds = 0,
                latitude_deg_1e8 = 11573116430,
                longitude_deg_1e8 = -3330374480,
                height_ellipsoid_mm = -16341,
                height_msl_mm = 15012,
                ned_velocity = new[] { 0.0590f, -0.1331f, -0.4141f },
                sats_used = 7,
                status = 3,
                pdop = 2.2109f,
                position_covariance = new[] { 39.0625f, 0.0000f, 0.0000f, 0.0000f, 39.0625f, 0.0000f, 0.0000f, 0.0000f, 162.7500f },
                position_covariance_len = 9,
                velocity_covariance = new[] { 1.8525f, 0.0000f, 0.0000f, 0.0000f, 1.8525f, 0.0000f, 0.0000f, 0.0000f, 1.8525f },
                velocity_covariance_len = 9,
            };

            testconversion((byte)125, 7, false);
            testconversion((byte)3, 3, false);
            testconversion((byte)3, 3, false);
            testconversion((sbyte)-3, 3, true);
            testconversion((byte)3, 5, false);
            testconversion((sbyte)-3, 5, true);
            testconversion((sbyte)-3, 5, true);
            testconversion((ulong)1234567890, 55, false);
            testconversion((ulong)1234567890, 33, false);
            testconversion((long)-1234567890, 33, true);
            testconversion((int)-12345678, 27, true);
            testconversion((int)(1 << 25), 27, true);

            testconversion(11573116430L, 37, true);
            testconversion(-3330374480L, 37, true);
            
            // will fail
            //testconversion((int)(1 << 26), 27, true);

            var state = new statetracking();

            fix.encode(uavcan_transmit_chunk_handler, state);

            var data = state.ToBytes();

            var fixtest = new uavcan.uavcan_equipment_gnss_Fix();
            fixtest.decode(new uavcan.CanardRxTransfer(data));


            
        }

        public void ReadMessage(string line)
        {
            int id_len;
            var line_len = line.Length;

            if (line_len <= 4)
                return;

            if (line.StartsWith("\a"))
                line = line.Replace("\a", "");

            if (line[0] == 'T') // 29 bit data frame
            {
                id_len = 8;
            }
            else if (line[0] == 't') // 11 bit data frame
            {
                id_len = 3;
            }
            else if (line[0] == 'N')
            {
                Console.WriteLine(line);
                return;
            }
            else
            {
                return;
            }

            //T12ABCDEF2AA55 : extended can_id 0x12ABCDEF, can_dlc 2, data 0xAA 0x55
            var packet_id = Convert.ToUInt32(new string(line.Skip(1).Take(id_len).ToArray()), 16); // id
            var packet_len = line[1 + id_len] - 48; // dlc
            var with_timestamp = line_len > (2 + id_len + packet_len * 2);

            if (packet_len == 0)
                return;

            var frame = new CANFrame(BitConverter.GetBytes(packet_id));

            var packet_data = line.Skip(2 + id_len).Take(packet_len * 2).NowNextBy2().Select(a =>
            {
                return Convert.ToByte(a.Item1 + "" + a.Item2, 16);
            });

            //Console.WriteLine(ASCIIEncoding.ASCII.GetString( packet_data));
            //Console.WriteLine("RX " + line.Replace("\r", "\r\n"));

            var payload = new CANPayload(packet_data.ToArray());

            if (payload.SOT)
                transfer[(packet_id, payload.TransferID)] = new List<byte>();

            // if have not seen SOT, abort
            if (!transfer.ContainsKey((packet_id, payload.TransferID)))
                return;

            transfer[(packet_id, payload.TransferID)].AddRange(payload.Payload);

            //todo check toggle

            if (payload.SOT && !payload.EOT)
            {
                //todo first 2 bytes are checksum
            }

            if (payload.EOT)
            {
                var result = transfer[(packet_id, payload.TransferID)].ToArray();

                transfer.Remove((packet_id, payload.TransferID));

                if (frame.TransferType == CANFrame.FrameType.anonymous)
                {
                    // dynamic node allocation
                    if (!uavcan.MSG_INFO.Any(a =>
                        a.Item2 == frame.MsgTypeID && frame.TransferType == CANFrame.FrameType.anonymous &&
                        !a.Item1.Name.EndsWith("_req") && !a.Item1.Name.EndsWith("_res")))
                    {
                        Console.WriteLine("No Message ID " + frame.SvcTypeID);
                        return;
                    }
                }

                if (frame.TransferType == CANFrame.FrameType.service)
                {
                    if (!uavcan.MSG_INFO.Any(a =>
                        a.Item2 == frame.SvcTypeID && frame.TransferType == CANFrame.FrameType.service))
                    {
                        Console.WriteLine("No Message ID " + frame.SvcTypeID);
                        return;
                    }
                }

                if (frame.TransferType == CANFrame.FrameType.message)
                {
                    if (!uavcan.MSG_INFO.Any(a =>
                        a.Item2 == frame.MsgTypeID && frame.TransferType == CANFrame.FrameType.message))
                    {
                        Console.WriteLine("No Message ID " + frame.MsgTypeID);
                        return;
                    }
                }

                var msgtype = uavcan.MSG_INFO.First(a =>
                    a.Item2 == frame.MsgTypeID && frame.TransferType == CANFrame.FrameType.message &&
                    !a.Item1.Name.EndsWith("_req") && !a.Item1.Name.EndsWith("_res") ||
                    a.Item2 == frame.MsgTypeID && frame.TransferType == CANFrame.FrameType.anonymous &&
                    !a.Item1.Name.EndsWith("_req") && !a.Item1.Name.EndsWith("_res") ||
                    a.Item2 == frame.SvcTypeID && frame.TransferType == CANFrame.FrameType.service &&
                    frame.SvcIsRequest && a.Item1.Name.EndsWith("_req") ||
                    a.Item2 == frame.SvcTypeID && frame.TransferType == CANFrame.FrameType.service &&
                    !frame.SvcIsRequest && a.Item1.Name.EndsWith("_res"));

                var dt_sig = BitConverter.GetBytes(msgtype.Item3);

                var startbyte = 0;

                if (!payload.SOT && payload.EOT)
                {
                    startbyte = 2;

                    if (result.Length <= 1)
                        return;

                    var payload_crc = result[0] | result[1] << 8;

                    var crcprocess = new TransferCRC();
                    crcprocess.add(dt_sig, 8);
                    crcprocess.add(result.Skip(startbyte).ToArray(), result.Length - startbyte);
                    var crc = crcprocess.get();

                    if (crc != payload_crc)
                    {
                        Console.WriteLine("Bad Message " + frame.MsgTypeID);
                        return;
                    }
                }
                else
                {
                }

                //Console.WriteLine(msgtype);

                MethodInfo method = typeof(Extension).GetMethod("ByteArrayToUAVCANMsg");
                MethodInfo generic = method.MakeGenericMethod(msgtype.Item1);
                try
                {
                    var ans = generic.Invoke(null, new object[] { result, startbyte });

                    //Console.WriteLine(("RX") + " " + msgtype.Item1 + " " + JsonConvert.SerializeObject(ans));

                    MessageReceived?.Invoke(frame, ans, payload.TransferID);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void SetParameter(byte node, string name, object value)
        {
            uavcan.uavcan_protocol_param_GetSet_req req = new uavcan.uavcan_protocol_param_GetSet_req()
            {
                name = ASCIIEncoding.ASCII.GetBytes(name), index = 0
            };
            req.name_len = (byte) req.name.Length;

            var param = _paramlistcache.Where(a => ASCIIEncoding.ASCII.GetString(a.name, 0, a.name_len) == name);

            if (param.Count() == 0)
                return;

            switch (param.First().value.uavcan_protocol_param_Value_type)
            {
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE:
                    req.value = new uavcan.uavcan_protocol_param_Value()
                    {
                        uavcan_protocol_param_Value_type = uavcan.uavcan_protocol_param_Value_type_t
                            .UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_BOOLEAN_VALUE,
                        union = new uavcan.uavcan_protocol_param_Value.unions()
                            {boolean_value = (float)value > 0 ? (byte) 1 : (byte) 0}
                    };
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE:
                    req.value = new uavcan.uavcan_protocol_param_Value()
                    {
                        uavcan_protocol_param_Value_type = uavcan.uavcan_protocol_param_Value_type_t
                            .UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_INTEGER_VALUE,
                        union = new uavcan.uavcan_protocol_param_Value.unions() {integer_value = (int)(float)value }
                    };
                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE:
                    if (value.GetType() == typeof(float))
                        req.value = new uavcan.uavcan_protocol_param_Value()
                        {
                            uavcan_protocol_param_Value_type = uavcan.uavcan_protocol_param_Value_type_t
                                .UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_REAL_VALUE,
                            union = new uavcan.uavcan_protocol_param_Value.unions() {real_value = (float) value}
                        };

                    break;
                case uavcan.uavcan_protocol_param_Value_type_t.UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE:
                    req.value = new uavcan.uavcan_protocol_param_Value()
                    {
                        uavcan_protocol_param_Value_type = uavcan.uavcan_protocol_param_Value_type_t
                            .UAVCAN_PROTOCOL_PARAM_VALUE_TYPE_STRING_VALUE,
                        union = new uavcan.uavcan_protocol_param_Value.unions()
                        {
                            string_value = ASCIIEncoding.ASCII.GetBytes(value.ToString()),
                            string_value_len = (byte) value.ToString().Length
                        }
                    };
                    break;
            }

            var slcan = PackageMessage(node, 30, transferID++, req);

            lock (sr_lock)
            {
                WriteToStream(slcan);
            }

        }
    }
}