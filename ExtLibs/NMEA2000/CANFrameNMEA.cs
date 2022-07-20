using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int64_t = System.Int64;

using float32 = System.Single;

namespace NMEA2000
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Newtonsoft.Json.Converters;


    public class CANFrameNMEA
    {
        private byte[] packet_data;

        public CANFrameNMEA(Span<byte> packet_data)
        {
            this.packet_data = packet_data.ToArray();
        }

        //0-256
        public byte SourceAddress
        {
            get { return (byte)(packet_data[0]); }
            set { packet_data[0] = (byte)((packet_data[0]) | (value)); }
        }

        public byte DestAddress
        {
            get
            {
                if (packet_data[2] < 240)
                    return packet_data[1];
                else return 0xff;
            }
        }

        // 0 - 65535    anon 0-3
        public UInt32 PDU //  PGN
        {
            get
            {
                var pf = packet_data[2];
                var ps = packet_data[1];

                if (pf < 240)
                {
                    return (UInt32)((DataPage << 16) + (pf << 8));
                }
                else
                {
                    return (UInt32)((DataPage << 16) + (pf << 8) + ps);
                }
            }
            set
            {
                packet_data[1] = (byte)(value >> 8);
                packet_data[2] = (byte)value;
            }
        }

        // 0 = high
        public byte Priority
        {
            get { return (byte)((packet_data[3] & 0x1c) >> 2); }
            set { packet_data[3] = (byte)((packet_data[3] & (~0x1c)) | ((value << 2) & 0x1c)); }
        }

        public byte DataPage
        {
            get { return (byte)(packet_data[3] & 3); }
        }

        public string ToHex()
        {
            var ans = "";
            foreach (var b in packet_data.Reverse())
            {
                ans += b.ToString("X2");
            }

            return ans;
        }
    }
}
