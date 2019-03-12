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
using Newtonsoft.Json;


namespace UAVCAN
{
    public class UAVCAN
    {
        public class statetracking
        {
            public BigInteger bi = new BigInteger();
            public int bit = 0;

            public byte[] ToBytes()
            {
                int get = (bit / 32) + 1;

                System.Numerics.BigInteger sbi = System.Numerics.BigInteger.Zero;

                for (int a = 0; a < get; a++)
                {
                    sbi += new System.Numerics.BigInteger(bi.data[a]) << (a * 32);
                }
                //bi.data

                var data2 = sbi.ToByteArray();

                Array.Resize(ref data2, (bit + 7) / 8);

                return data2;
            }
        }

        public static bool testconversion<T>(T input, byte bitlength, bool signed) where T : struct
        {
            var buf = new byte[8];
            var ctx = new statetracking();
            T ans = input;
            uavcan.canardEncodeScalar(buf, 0, bitlength, input);
            chunk_cb(buf, bitlength, ctx);
            uavcan.canardDecodeScalar(new uavcan.CanardRxTransfer(buf), 0, bitlength, signed, ref ans);

            if (input.ToString() != ans.ToString())
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
                    if (!nodeList.ContainsKey(frame.SourceNode))
                    {
                        nodeList.Add(frame.SourceNode, msg as uavcan.uavcan_protocol_NodeStatus);
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
            if(sr != null && closestream)
                sr.Close();
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

            MessageReceived += (frame, msg, transferID) =>
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

                    Console.WriteLine(ASCIIEncoding.ASCII.GetString(debug.text, 0, debug.text_len));
                }
            };



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
                        throw new FileNotFoundException("File read request for file we are not serving " +
                                  ASCIIEncoding.ASCII.GetString(frreq.path.path).TrimEnd('\0'));

                    using (var file = File.OpenRead(firmware.First().Value))
                    {
                        Console.WriteLine("file_Read: {0} at {1}", requestedfile, frreq.offset);
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
                    }
                }
            };
        }

        Dictionary<int, uavcan.uavcan_protocol_NodeStatus> nodeList = new Dictionary<int, uavcan.uavcan_protocol_NodeStatus>();

        List<byte> dynamicBytes = new List<byte>();

        public void SetupDynamicNodeAllocator()
        {
            MessageReceived += (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
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
                                if (!nodeList.ContainsKey(a))
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

        public async void Update(string devicename, double hwversion, string firmware_name)
        {
            ServeFile(firmware_name);

            var firmware_namebytes = ASCIIEncoding.ASCII.GetBytes(Path.GetFileName(firmware_name.ToLower()));
            ulong firmware_crc = ulong.MaxValue;

            MessageReceived += (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != SourceNode)
                    return;

                if (msg.GetType() == typeof(uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res))
                {
                    var bfures = msg as uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res;
                    if (bfures.error != 0)
                        throw new Exception("Begin Firmware Update returned an error");
                }
                else if (msg.GetType() == typeof(uavcan.uavcan_protocol_GetNodeInfo_res))
                {
                    var gnires = msg as uavcan.uavcan_protocol_GetNodeInfo_res;
                    Console.WriteLine("GetNodeInfo: seen '{0}' from {1}", ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0'), frame.SourceNode);
                    if (devicename == ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0'))
                    {
                        if (firmware_crc != gnires.software_version.image_crc)
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
                            }
                        }
                        else
                        {
                            throw new Exception(String.Format( "{0} - No need to upload, crc matchs", frame.SourceNode));
                        }
                    }
                }
            };

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

            

            NodeAdded += (NodeID, msg) =>
            {
                var statetracking = new statetracking();
                // get node info
                uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new uavcan.uavcan_protocol_GetNodeInfo_req() { };
                gnireq.encode(chunk_cb, statetracking);

                var slcan = PackageMessage(NodeID, 30, transferID++, gnireq);
                lock (sr_lock)
                    WriteToStream(slcan);
            };

            foreach (var i in nodeList.Keys.ToArray())
            {
                var statetracking = new statetracking();
                // get node info
                uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new uavcan.uavcan_protocol_GetNodeInfo_req() { };
                gnireq.encode(chunk_cb, statetracking);

                var slcan = PackageMessage((byte)i, 30, transferID++, gnireq);
                lock (sr_lock)
                    WriteToStream(slcan);
            }

            int b = 0;
            while (true)
            {
                await Task.Delay(1000);

                if (nodeList.Values.Any(a => a.mode == uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE))
                {
                   
                }
                else
                {
                    b++;
                    if(b > 100)
                        break;
                }
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
            msg.encode(chunk_cb, state);

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

        public delegate void NodeAddedArgs(byte NodeID, uavcan.uavcan_protocol_NodeStatus nodeStatus);
        public event NodeAddedArgs NodeAdded;

        private byte transferID = 0;

        public UAVCAN(Byte sourceNode)
        {
            SourceNode = sourceNode;
        }

        public UAVCAN()
        {

        }

        public static void test()
        {
            var fix = new uavcan.uavcan_equipment_gnss_Fix()
            {
                timestamp = new uavcan.uavcan_Timestamp() { usec = 1 },
                gnss_timestamp = new uavcan.uavcan_Timestamp() { usec = 2 },
                gnss_time_standard = 3,
                height_ellipsoid_mm = 4,
                height_msl_mm = 5,
                latitude_deg_1e8 = 6,
                longitude_deg_1e8 = 7,
                num_leap_seconds = 17,
                pdop = 8f,
                sats_used = 10,
                ned_velocity = new[] { 1f, 2f, 3f },
                status = 3
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
            // will fail
            //testconversion((int)(1 << 26), 27, true);

            var state = new statetracking();

            fix.encode(chunk_cb, state);

            var data = state.ToBytes();//
            var data2 = state.bi.getBytes().Reverse().ToArray();

            Array.Resize(ref data2, (state.bit + 7) / 8);

            var fixtest = new uavcan.uavcan_equipment_gnss_Fix();
            fixtest.decode(new uavcan.CanardRxTransfer(data));

            if (fix != fixtest)
            {

            }

            {
                var basecan = new UAVCAN();

                var file = File.OpenRead(@"C:\Users\mich1\Google Drive\Here2-crc.bin");

                var buffer = new byte[256];
                var read = file.Read(buffer, 0, 256);
                var readRes = new uavcan.uavcan_protocol_file_Read_res()
                {
                    data = buffer,
                    data_len = (ushort)read,
                    error = new uavcan.uavcan_protocol_file_Error()
                    { value = (short)uavcan.UAVCAN_PROTOCOL_FILE_ERROR_OK }
                };

                var slcan = basecan.PackageMessage(125, 12, 0, readRes);


                var liness = slcan.Split('\r');

                foreach (var line in liness)
                {
                    basecan.ReadMessage(line);
                }

                var lines = File.ReadAllLines(@"C:\Users\mich1\OneDrive\canlog gpsupdate2-8mhz.txt");

                //var basecan = new UAVCAN();

                basecan.MessageReceived += (a1, a2, a3) => { };

                int l = 0;
                foreach (var line in lines)
                {
                    l++;

                    // tab delimiter file
                    var splitline = line.Split('\t');

                    for (int a = 0; a < splitline.Length; a++)
                    {
                        splitline[a] = splitline[a].Trim().Replace(" ", "");
                    }

                    try
                    {
                        basecan.ReadMessage("T" + splitline[2] + (splitline[3].Length / 2) + splitline[3]);
                    }
                    catch { }
                }

            }

            {
                var lines = File.ReadAllLines(@"C:\Users\michael\OneDrive\canlog.can");
                var id_len = 0;

                // need sourcenode, msgid, transfer id


                var basecan = new UAVCAN();

                int l = 0;
                foreach (var line in lines)
                {
                    l++;

                    basecan.ReadMessage(line);
                }
            }
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
                catch
                {
                }
            }
        }

        public static void chunk_cb(byte[] buffer, int sizeinbits, object ctx)
        {
            var stuff = (statetracking)ctx;
            if (buffer == null)
            {
                stuff.bit += sizeinbits;
                return;
            }
            /*
            BigInteger input = new BigInteger(buffer.ToArray());

            for (uint a = 0; a < sizeinbits; a++)
            {
                if ((input & (1L << (int) a)) > 0)
                {
                    stuff.bi += BigInteger.One << (int) (stuff.bit + a);
                }
            }
            */

            //todo try replace this with built in dot net type Biginterger


            BigInteger input = new BigInteger(buffer.Reverse().ToArray());

            int extrabits = 0;

            if ((sizeinbits % 8) != 0)
            {
                // coverity[overrun-local]
                extrabits = ((8 - (sizeinbits % 8)) & 7);
            }

            // do the entire bit, incase it has been shifted left as it was not byte alligned
            for (uint a = 0; a < (sizeinbits + extrabits); a++)
            {
                if ((input & (1L << (int)a)) > 0)
                {
                    stuff.bi.setBit((uint)stuff.bit + a);
                }
            }

            stuff.bit += sizeinbits;
        }
    }
}