using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using u8 = System.Byte;
using u16 = System.UInt16;
using s32 = System.Int32;
using u32 = System.UInt32;
using s64 = System.Int64;

namespace MissionPlanner.Utilities
{
    public class sbp
    {
        int state = 0;

        piksimsg msg = new piksimsg();

        int length = 0;

        Crc16Ccitt crc;

        ushort crcpacket = 0;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct piksimsg
        {
            public byte preamble; // 0x55
            public UInt16 msg_type;
            public UInt16 sender;
            public byte length; // payload length
            [MarshalAs(UnmanagedType.ByValArray)]
            public byte[] payload;
            public UInt16 crc; // - preamble
        }

        public int read(byte data)
        {
            switch (state)
            {
                case 0:
                    if (data == 0x55)
                    {
                        state++;
                        msg = new piksimsg();
                        msg.preamble = data;
                        crc = new Crc16Ccitt(InitialCrcValue.Zeros);
                        crcpacket = (ushort)InitialCrcValue.Zeros;
                    }
                    break;
                case 1:
                    msg.msg_type = (u16)(data);
                    crcpacket = crc.Accumulate(data, crcpacket);
                    state++;
                    break;
                case 2:
                    msg.msg_type = (u16)(msg.msg_type + (data << 8));
                    crcpacket = crc.Accumulate(data, crcpacket);
                    state++;
                    break;
                case 3:
                    msg.sender = (u16)(data);
                    crcpacket = crc.Accumulate(data, crcpacket);
                    state++;
                    break;
                case 4:
                    msg.sender = (u16)(msg.sender + (data << 8));
                    crcpacket = crc.Accumulate(data, crcpacket);
                    state++;
                    break;
                case 5:
                    msg.length = data;
                    crcpacket = crc.Accumulate(data, crcpacket);
                    msg.payload = new u8[msg.length];
                    length = 0;
                    state++;
                    break;
                case 6:
                    if (length == msg.length)
                    {
                        state++;
                        goto case 7;
                    }
                    else
                    {
                        msg.payload[length] = data;
                        crcpacket = crc.Accumulate(data, crcpacket);
                        length++;
                    }
                    break;
                case 7:
                    msg.crc = (u16)(data);
                    state++;
                    break;
                case 8:
                    msg.crc = (u16)(msg.crc + (data << 8));
                    state = 0;

                    if (msg.crc == crcpacket)
                    {
                        return msg.msg_type;
                    }
                    break;
            }

            return -1;
        }

        public enum InitialCrcValue { Zeros, NonZero1 = 0xffff, NonZero2 = 0x1D0F }

        public class Crc16Ccitt
        {
            const ushort poly = 4129;
            static ushort[] table;
            ushort initialValue = 0;

            public ushort Accumulate(byte data, ushort crc)
            {
                crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & data))]);

                return crc;
            }

            public ushort ComputeChecksum(byte[] bytes)
            {
                ushort crc = this.initialValue;
                for (int i = 0; i < bytes.Length; i++)
                {
                    crc = (ushort)((crc << 8) ^ table[((crc >> 8) ^ (0xff & bytes[i]))]);
                }
                return crc;
            }

            public byte[] ComputeChecksumBytes(byte[] bytes)
            {
                ushort crc = ComputeChecksum(bytes);
                return new byte[] { (byte)(crc >> 8), (byte)(crc & 0x00ff) };
            }

            public Crc16Ccitt(InitialCrcValue initialValue)
            {
                this.initialValue = (ushort)initialValue;
                if (table == null)
                {
                    table = new ushort[256];
                    ushort temp, a;
                    for (int i = 0; i < table.Length; i++)
                    {
                        temp = 0;
                        a = (ushort)(i << 8);
                        for (int j = 0; j < 8; j++)
                        {
                            if (((temp ^ a) & 0x8000) != 0)
                            {
                                temp = (ushort)((temp << 1) ^ poly);
                            }
                            else
                            {
                                temp <<= 1;
                            }
                            a <<= 1;
                        }
                        table[i] = temp;
                    }
                }
            }
        }
    }
}
