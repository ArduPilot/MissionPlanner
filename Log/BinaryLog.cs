using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using uint8_t = System.Byte;

namespace ArdupilotMega.Log
{
    public class BinaryLog
    {
        const byte HEAD_BYTE1 = 0xA3;    // Decimal 163  
        const byte HEAD_BYTE2 = 0x95;    // Decimal 149  

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct log_Format
        {
            public uint8_t type;
            public uint8_t length;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] name;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] format;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] labels;
        }

        static Dictionary<string, log_Format> logformat = new Dictionary<string, log_Format>();

        /// <summary>
        /// Read and return the log in assci
        /// </summary>
        /// <param name="fn">FileName</param>
        /// <returns>Log</returns>
        public static List<string> ReadLog(string fn)
        {
            List<string> lines = new List<string>();

            BinaryReader br = new BinaryReader(File.OpenRead(fn));

            int log_step = 0;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                byte data = br.ReadByte();

                switch (log_step)
                {
                    case 0:
                        if (data == HEAD_BYTE1)
                        {
                            log_step++;
                        }
                        break;

                    case 1:
                        if (data == HEAD_BYTE2)
                        {
                            log_step++;
                        }
                        else
                        {
                            log_step = 0;
                        }
                        break;

                    case 2:
                        log_step = 0;
                        try
                        {
                            string line = logEntry(data, br);

                            // we need to know the mav type to use the correct mode list.
                            if (line.Contains("PARM, RATE_RLL_P"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;
                            }
                            else if ((line.Contains("PARM, H_SWASH_PLATE")))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduHeli;
                            }
                            else if (line.Contains("PARM, PTCH2SRV_P"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduPlane;
                            }
                            else if (line.Contains("PARM, SKID_STEER_OUT"))
                            {
                                MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduRover;
                            }

                            lines.Add(line);
                        }
                        catch { Console.WriteLine("Bad Binary log line {0}", data); }
                        break;
                }
            }

            return lines;
        }

        /// <summary>
        /// Process each log entry
        /// </summary>
        /// <param name="packettype">packet type</param>
        /// <param name="br">input file</param>
        /// <returns>string of converted data</returns>
        static string logEntry(byte packettype, BinaryReader br)
        {

            switch (packettype)
            {

                case 0x80:  // FMT
                    
                    log_Format logfmt = new log_Format();

                    object obj = logfmt;

                    int len = Marshal.SizeOf(obj);

                    byte[] bytearray = br.ReadBytes(len);

                    IntPtr i = Marshal.AllocHGlobal(len);

                    // create structure from ptr
                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    // copy byte array to ptr
                    Marshal.Copy(bytearray, 0, i, len);

                    obj = Marshal.PtrToStructure(i, obj.GetType());

                    Marshal.FreeHGlobal(i);

                    logfmt = (log_Format)obj;

                    string lgname = ASCIIEncoding.ASCII.GetString(logfmt.name).Trim(new char[] { '\0' });
                    string lgformat = ASCIIEncoding.ASCII.GetString(logfmt.format).Trim(new char[] { '\0' });
                    string lglabels = ASCIIEncoding.ASCII.GetString(logfmt.labels).Trim(new char[] { '\0' });

                    logformat[lgname] = logfmt;

                    string line = String.Format("FMT, {0}, {1}, {2}, {3}, {4}\r\n", logfmt.type, logfmt.length, lgname, lgformat, lglabels);

                    return line;

                default:
                    string format = "";
                    string name = "";
                    int size = 0;

                    foreach (log_Format fmt in logformat.Values) 
                    {
                        if (fmt.type == packettype)
                        {
                            name = ASCIIEncoding.ASCII.GetString(fmt.name).Trim(new char[] { '\0' });
                            format = ASCIIEncoding.ASCII.GetString(fmt.format).Trim(new char[] { '\0' });
                            size = fmt.length;
                            break;
                        }
                    }

                    // didnt find a match, return unknown packet type
                    if (size == 0)
                        return "UNKW, " + packettype;

                    return ProcessMessage(br.ReadBytes(size - 3),name,format); // size - 3 = message - messagetype - (header *2)
            }
        }


        /*  
    105    +Format characters in the format string for binary log messages  
    106    +  b   : int8_t  
    107    +  B   : uint8_t  
    108    +  h   : int16_t  
    109    +  H   : uint16_t  
    110    +  i   : int32_t  
    111    +  I   : uint32_t  
    112    +  f   : float  
    113    +  N   : char[16]  
    114    +  c   : int16_t * 100  
    115    +  C   : uint16_t * 100  
    116    +  e   : int32_t * 100  
    117    +  E   : uint32_t * 100  
    118    +  L   : uint32_t latitude/longitude  
    119    + */

        /// <summary>
        /// Convert to ascii based on the existing format message
        /// </summary>
        /// <param name="message">raw binary message</param>
        /// <param name="name">Message type name</param>
        /// <param name="format">format string containing packet structure</param>
        /// <returns>formated ascii string</returns>
        static string ProcessMessage(byte[] message,string name, string format)
        {
            char[] form = format.ToCharArray();

            int offset = 0;

            string line = name;

            foreach (char ch in form)
            {
                switch (ch)
                {
                    case 'b':
                        line += ", " + (sbyte)message[offset];
                        offset++;
                        break;
                    case 'B':
                        line += ", " + message[offset];
                        offset++;
                        break;
                    case 'h':
                        line += ", " + BitConverter.ToInt16(message, offset);
                        offset += 2;
                        break;
                    case 'H':
                        line += ", " + BitConverter.ToUInt16(message, offset);
                        offset += 2;
                        break;
                    case 'i':
                        line += ", " + BitConverter.ToInt32(message, offset);
                        offset += 4;
                        break;
                    case 'I':
                        line += ", " + BitConverter.ToUInt32(message, offset);
                        offset += 4;
                        break;
                    case 'f':
                        line += ", " + BitConverter.ToSingle(message, offset);
                        offset += 4;
                        break;
                    case 'c':
                        line += ", " + (BitConverter.ToInt16(message, offset) / 100.0).ToString("0.00");
                        offset += 2;
                        break;
                    case 'C':
                        line += ", " + (BitConverter.ToUInt16(message, offset) / 100.0).ToString("0.00");
                        offset += 2;
                        break;
                    case 'e':
                        line += ", " + (BitConverter.ToInt32(message, offset) / 100.0).ToString("0.00");
                        offset += 4;
                        break;
                    case 'E':
                        line += ", " + (BitConverter.ToUInt32(message, offset) / 100.0).ToString("0.00");
                        offset += 4;
                        break;
                    case 'L':
                        line += ", " + (double)BitConverter.ToInt32(message, offset) / 10000000.0;
                        offset += 4;
                        break;
                    case 'n':
                        line += ", " + ASCIIEncoding.ASCII.GetString(message, offset, 4).Trim(new char[] { '\0' });
                        offset += 4;
                        break;
                    case 'N':
                        line += ", " + ASCIIEncoding.ASCII.GetString(message, offset, 16).Trim(new char[] { '\0' });
                        offset += 16;
                        break;
                    case 'M':
                        line += ", " + Common.getModesList()[message[offset]].Value;
                        offset++;
                        break;
                    case 'Z':
                        line += ", " + ASCIIEncoding.ASCII.GetString(message, offset, 64).Trim(new char[] { '\0' });
                        offset += 64;
                        break;
                    default:

                        break;
                }
            }

            return line + "\r\n";
        }
    }
}
