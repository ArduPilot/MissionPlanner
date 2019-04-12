using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using UAVCAN;

namespace UAVCANFlasher
{
    class Program
    {
        private static bool run;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;

            if (args.Length < 2)
            {
                Console.WriteLine("usage: UAVCANFlasher com1 flow-crc.bin");
                Console.WriteLine("this program will flash the firmware to every connected uavcan node. DONT connect devices that its not intended for");
                Console.ReadKey();
                return;
            }

            var portname = args[0];
            var firmware_name = args[1];

            var port = new MissionPlanner.Comms.SerialPort(args[0], 115200);

            port.Open();

            if (!File.Exists(firmware_name))
            {
                Console.WriteLine("firmware file not found");
                Console.ReadKey();
                return;
            }

            UAVCAN.uavcan can = new UAVCAN.uavcan();

            can.SourceNode = 127;

            //can.Update();
            

            // updater
            var firmware_namebytes = ASCIIEncoding.ASCII.GetBytes(Path.GetFileName(firmware_name.ToLower()));
            ulong firmware_crc = ulong.MaxValue;
            Exception exception = null;
            Dictionary<int, DateTime> lastseenDateTimes = new Dictionary<int, DateTime>();

            UAVCAN.uavcan.MessageRecievedDel updatedelegate = (frame, msg, transferID) =>
            {
                if (frame.IsServiceMsg && frame.SvcDestinationNode != can.SourceNode)
                    return;

                if (frame.SourceNode == 0)
                    return;

                lastseenDateTimes[frame.SourceNode] = DateTime.Now;
                
                if (msg.GetType() == typeof(uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res))
                {
                    var bfures = msg as uavcan.uavcan_protocol_file_BeginFirmwareUpdate_res;
                    if (bfures.error != 0)
                        exception = new Exception(frame.SourceNode + " Begin Firmware Update returned an error");
                }
                else if (msg.GetType() == typeof(uavcan.uavcan_protocol_GetNodeInfo_res))
                {
                    var gnires = msg as uavcan.uavcan_protocol_GetNodeInfo_res;
                    Console.WriteLine("GetNodeInfo: seen '{0}' from {1}",
                        ASCIIEncoding.ASCII.GetString(gnires.name).TrimEnd('\0'), frame.SourceNode);
                    if (firmware_crc != gnires.software_version.image_crc || firmware_crc == ulong.MaxValue)
                    {
                        if (gnires.status.mode != uavcan.UAVCAN_PROTOCOL_NODESTATUS_MODE_SOFTWARE_UPDATE)
                        {
                            Console.WriteLine("Update node " + frame.SourceNode);
                            var req_msg =
                                new uavcan.uavcan_protocol_file_BeginFirmwareUpdate_req()
                                {
                                    image_file_remote_path = new uavcan.uavcan_protocol_file_Path()
                                        {path = firmware_namebytes},
                                    source_node_id = can.SourceNode
                                };
                            req_msg.image_file_remote_path.path_len = (byte) firmware_namebytes.Length;

                            var slcan = can.PackageMessage(frame.SourceNode, frame.Priority, transferID, req_msg);
                                can.WriteToStream(slcan);
                            
                        }
                        else
                        {
                            //exception = new Exception("already in update mode");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine(frame.SourceNode + " CRC matchs");
                    }
                }
            };
            can.MessageReceived += updatedelegate;

            // getfile crc
            using (var stream = File.OpenRead(firmware_name))
            {
                string app_descriptor_fmt = "<8cQI";
                var SHARED_APP_DESCRIPTOR_SIGNATURES = new byte[][] {
                    new byte[] { 0xd7, 0xe4, 0xf7, 0xba, 0xd0, 0x0f, 0x9b, 0xee },
                    new byte[] { 0x40, 0xa2, 0xe4, 0xf1, 0x64, 0x68, 0x91, 0x06 } };

                var app_descriptor_len = 8 * 1 + 8 + 4;

                var location = GetPatternPositions(stream, SHARED_APP_DESCRIPTOR_SIGNATURES[0]);
                stream.Seek(0, SeekOrigin.Begin);
                var location2 = GetPatternPositions(stream, SHARED_APP_DESCRIPTOR_SIGNATURES[1]);

                if (location.Count > 0 || location2.Count > 0)
                {
                    var offset = location.Count > 0 ? location[0] : location2[0];
                    stream.Seek(offset, SeekOrigin.Begin);
                    byte[] buf = new byte[app_descriptor_len];
                    stream.Read(buf, 0, app_descriptor_len);
                    firmware_crc = BitConverter.ToUInt64(buf, 8);
                }
            }

            can.NodeAdded += (id, status) =>
            {
                Console.WriteLine(id + " Node status seen");
                // get node info
                uavcan.uavcan_protocol_GetNodeInfo_req gnireq = new uavcan.uavcan_protocol_GetNodeInfo_req() { };

                var slcan = can.PackageMessage((byte)id, 30, 0, gnireq);

                   can.WriteToStream(slcan);
            };

            can.FileSendComplete += (id, file) =>
            {
                Console.WriteLine(id + " File send complete " + file);

                can.NodeList.Remove(id);
                lastseenDateTimes.Remove(id);
            };

            can.StartSLCAN(port.BaseStream);

            can.ServeFile(args[1]);

            can.SetupFileServer();

            can.SetupDynamicNodeAllocator();

            Console.WriteLine("Ready.. " + portname + " -> " + firmware_name);
            run = true;
            while (run)
            {
                Thread.Sleep(100);

                foreach (var lastseenDateTime in lastseenDateTimes.ToArray())
                {
                    if (lastseenDateTime.Value.AddSeconds(3) < DateTime.Now)
                    {
                        can.NodeList.Remove(lastseenDateTime.Key);
                        lastseenDateTimes.Remove(lastseenDateTime.Key);
                        Console.WriteLine(lastseenDateTime.Key + " Remove old node - 3 seconds of no data");
                    }
                }

                if (exception != null)
                {
                    Console.WriteLine(exception.ToString());
                    exception = null;
                }
            }

        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            run = false;
        }


        static List<long> GetPatternPositions(Stream stream, byte[] pattern)
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
    }
}
