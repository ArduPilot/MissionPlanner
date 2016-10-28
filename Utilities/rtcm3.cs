using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using u8 = System.Byte;
using u16 = System.UInt16;
using s32 = System.Int32;
using u32 = System.UInt32;
using gps_time_t = System.UInt64;

namespace MissionPlanner.Utilities
{
    public class rtcm3 : ICorrections
    {
        const byte RTCM3PREAMB = 0xD3;

        int step = 0;

        byte[] buffer = new u8[1024 * 4];
        u32 payloadlen = 0;
        int msglencount = 0;
        rtcmpreamble pre;

        public int Read(byte data)
        {
            switch (step)
            {
                default:
                case 0:
                    if (data == RTCM3PREAMB)
                    {
                        step = 1;
                        buffer[0] = data;
                    }
                    break;
                case 1:
                    buffer[1] = data;
                    step++;
                    break;
                case 2:
                    buffer[2] = data;
                    step++;
                    pre = new rtcmpreamble();
                    pre.Read(buffer);
                    payloadlen = pre.length;
                    msglencount = 0;
                    // reset on oversize packet
                    if (payloadlen > buffer.Length)
                        step = 0;
                    break;
                case 3:
                    if (msglencount < (payloadlen))
                    {
                        buffer[msglencount + 3] = data;
                        msglencount++;
                    }
                    else
                    {
                        step++;
                        goto case 4;
                    }
                    break;
                case 4:
                    buffer[payloadlen + 3] = data;
                    step++;
                    break;
                case 5:
                    buffer[payloadlen + 3 + 1] = data;
                    step++;
                    break;
                case 6:
                    buffer[payloadlen + 3 + 2] = data;

                    payloadlen = payloadlen + 3;
                    u32 crc = crc24.crc24q(buffer, payloadlen, 0);
                    u32 crcpacket = getbitu(buffer, payloadlen * 8, 24);

                    if (crc == crcpacket)
                    {
                        rtcmheader head = new rtcmheader();
                        head.Read(buffer);

                        step = 0;

                        return head.messageno;
                    }
                    step = 0;
                    break;
            }

            return -1;
        }

        static uint getbitu(u8[] buff, u32 pos, u32 len)
        {
            uint bits = 0;
            u32 i;
            for (i = pos; i < pos + len; i++)
                bits = (uint)((bits << 1) + ((buff[i / 8] >> (int)(7 - i % 8)) & 1u));
            return bits;
        }

        static void setbitu(u8[] buff, u32 pos, u32 len, u32 data)
        {
            u32 mask = 1u << (int)(len - 1);

            if (len <= 0 || 32 < len) return;

            for (u32 i = pos; i < pos + len; i++, mask >>= 1)
            {
                if ((data & mask) > 0)
                    buff[i / 8] |= (byte)(1u << (int)(7 - i % 8));
                else
                    buff[i / 8] &= (byte)(~(1u << (int)(7 - i % 8)));
            }
        }

        public class rtcmpreamble
        {
            public u8 preamble = RTCM3PREAMB;
            public u8 resv1;
            public u16 length;

            public void Read(byte[] buffer)
            {
                uint i = 0;

                preamble = (byte)getbitu(buffer, i, 8); i += 8;
                resv1 = (byte)getbitu(buffer, i, 6); i += 6;
                length = (u16)getbitu(buffer, i, 10); i += 10;
            }

            public byte[] Write(byte[] buffer)
            {
                uint i = 0;

                setbitu(buffer, i, 8, RTCM3PREAMB); i += 8;
                setbitu(buffer, i, 6, resv1); i += 6;
                setbitu(buffer, i, 10, length); i += 10;

                return buffer;
            }
        }

        public class rtcmheader
        {
            public u16 messageno;
            public u16 refstationid;
            public u32 epoch;
            public u8 sync;
            public u8 nsat;
            public u8 smoothind;
            public u8 smoothint;

            public void Read(byte[] buffer)
            {
                u32 i = 24;

                messageno = (u16)getbitu(buffer, i, 12); i += 12;        /* message no */
                refstationid = (u16)getbitu(buffer, i, 12); i += 12;        /* ref station id */
                epoch = (u32)getbitu(buffer, i, 30); i += 30;       /* gps epoch time */
                sync = (u8)getbitu(buffer, i, 1); i += 1;          /* synchronous gnss flag */
                nsat = (u8)getbitu(buffer, i, 5); i += 5;          /* no of satellites */
                smoothind = (u8)getbitu(buffer, i, 1); i += 1;          /* smoothing indicator */
                smoothint = (u8)getbitu(buffer, i, 3); i += 3;          /* smoothing interval */
            }

            public byte[] Write(byte[] buffer)
            {
                u32 i = 24;

                setbitu(buffer, i, 12, messageno); i += 12;        /* message no */
                setbitu(buffer, i, 12, refstationid); i += 12;        /* ref station id */
                setbitu(buffer, i, 30, epoch); i += 30;       /* gps epoch time */
                setbitu(buffer, i, 1, sync); i += 1;          /* synchronous gnss flag */
                setbitu(buffer, i, 5, nsat); i += 5;          /* no of satellites */
                setbitu(buffer, i, 1, smoothind); i += 1;          /* smoothing indicator */
                setbitu(buffer, i, 3, smoothint); i += 3;          /* smoothing interval */
                return buffer;
            }
        }

        public class crc24
        {
            static u32[] crc24qtab = new u32[] {
                0x000000, 0x864CFB, 0x8AD50D, 0x0C99F6, 0x93E6E1, 0x15AA1A, 0x1933EC, 0x9F7F17,
                0xA18139, 0x27CDC2, 0x2B5434, 0xAD18CF, 0x3267D8, 0xB42B23, 0xB8B2D5, 0x3EFE2E,
                0xC54E89, 0x430272, 0x4F9B84, 0xC9D77F, 0x56A868, 0xD0E493, 0xDC7D65, 0x5A319E,
                0x64CFB0, 0xE2834B, 0xEE1ABD, 0x685646, 0xF72951, 0x7165AA, 0x7DFC5C, 0xFBB0A7,
                0x0CD1E9, 0x8A9D12, 0x8604E4, 0x00481F, 0x9F3708, 0x197BF3, 0x15E205, 0x93AEFE,
                0xAD50D0, 0x2B1C2B, 0x2785DD, 0xA1C926, 0x3EB631, 0xB8FACA, 0xB4633C, 0x322FC7,
                0xC99F60, 0x4FD39B, 0x434A6D, 0xC50696, 0x5A7981, 0xDC357A, 0xD0AC8C, 0x56E077,
                0x681E59, 0xEE52A2, 0xE2CB54, 0x6487AF, 0xFBF8B8, 0x7DB443, 0x712DB5, 0xF7614E,
                0x19A3D2, 0x9FEF29, 0x9376DF, 0x153A24, 0x8A4533, 0x0C09C8, 0x00903E, 0x86DCC5,
                0xB822EB, 0x3E6E10, 0x32F7E6, 0xB4BB1D, 0x2BC40A, 0xAD88F1, 0xA11107, 0x275DFC,
                0xDCED5B, 0x5AA1A0, 0x563856, 0xD074AD, 0x4F0BBA, 0xC94741, 0xC5DEB7, 0x43924C,
                0x7D6C62, 0xFB2099, 0xF7B96F, 0x71F594, 0xEE8A83, 0x68C678, 0x645F8E, 0xE21375,
                0x15723B, 0x933EC0, 0x9FA736, 0x19EBCD, 0x8694DA, 0x00D821, 0x0C41D7, 0x8A0D2C,
                0xB4F302, 0x32BFF9, 0x3E260F, 0xB86AF4, 0x2715E3, 0xA15918, 0xADC0EE, 0x2B8C15,
                0xD03CB2, 0x567049, 0x5AE9BF, 0xDCA544, 0x43DA53, 0xC596A8, 0xC90F5E, 0x4F43A5,
                0x71BD8B, 0xF7F170, 0xFB6886, 0x7D247D, 0xE25B6A, 0x641791, 0x688E67, 0xEEC29C,
                0x3347A4, 0xB50B5F, 0xB992A9, 0x3FDE52, 0xA0A145, 0x26EDBE, 0x2A7448, 0xAC38B3,
                0x92C69D, 0x148A66, 0x181390, 0x9E5F6B, 0x01207C, 0x876C87, 0x8BF571, 0x0DB98A,
                0xF6092D, 0x7045D6, 0x7CDC20, 0xFA90DB, 0x65EFCC, 0xE3A337, 0xEF3AC1, 0x69763A,
                0x578814, 0xD1C4EF, 0xDD5D19, 0x5B11E2, 0xC46EF5, 0x42220E, 0x4EBBF8, 0xC8F703,
                0x3F964D, 0xB9DAB6, 0xB54340, 0x330FBB, 0xAC70AC, 0x2A3C57, 0x26A5A1, 0xA0E95A,
                0x9E1774, 0x185B8F, 0x14C279, 0x928E82, 0x0DF195, 0x8BBD6E, 0x872498, 0x016863,
                0xFAD8C4, 0x7C943F, 0x700DC9, 0xF64132, 0x693E25, 0xEF72DE, 0xE3EB28, 0x65A7D3,
                0x5B59FD, 0xDD1506, 0xD18CF0, 0x57C00B, 0xC8BF1C, 0x4EF3E7, 0x426A11, 0xC426EA,
                0x2AE476, 0xACA88D, 0xA0317B, 0x267D80, 0xB90297, 0x3F4E6C, 0x33D79A, 0xB59B61,
                0x8B654F, 0x0D29B4, 0x01B042, 0x87FCB9, 0x1883AE, 0x9ECF55, 0x9256A3, 0x141A58,
                0xEFAAFF, 0x69E604, 0x657FF2, 0xE33309, 0x7C4C1E, 0xFA00E5, 0xF69913, 0x70D5E8,
                0x4E2BC6, 0xC8673D, 0xC4FECB, 0x42B230, 0xDDCD27, 0x5B81DC, 0x57182A, 0xD154D1,
                0x26359F, 0xA07964, 0xACE092, 0x2AAC69, 0xB5D37E, 0x339F85, 0x3F0673, 0xB94A88,
                0x87B4A6, 0x01F85D, 0x0D61AB, 0x8B2D50, 0x145247, 0x921EBC, 0x9E874A, 0x18CBB1,
                0xE37B16, 0x6537ED, 0x69AE1B, 0xEFE2E0, 0x709DF7, 0xF6D10C, 0xFA48FA, 0x7C0401,
                0x42FA2F, 0xC4B6D4, 0xC82F22, 0x4E63D9, 0xD11CCE, 0x575035, 0x5BC9C3, 0xDD8538
            };

            /** Calculate Qualcomm 24-bit Cyclical Redundancy Check (CRC-24Q).
             *
             * The CRC polynomial used is:
             * \f[
             *   x^{24} + x^{23} + x^{18} + x^{17} + x^{14} + x^{11} + x^{10} +
             *   x^7    + x^6    + x^5    + x^4    + x^3    + x+1
             * \f]
             * Mask 0x1864CFB, not reversed, not XOR'd
             *
             * \param buf Array of data to calculate CRC for
             * \param len Length of data array
             * \param crc Initial CRC value
             *
             * \return CRC-24Q value
             */
            public static u32 crc24q(u8[] buf, u32 len, u32 crc)
            {
                for (u32 i = 0; i < len; i++)
                    crc = ((crc << 8) & 0xFFFFFF) ^ crc24qtab[(crc >> 16) ^ buf[i]];
                return crc;
            }
        }

        public s32 length
        {
            get { return (s32)(payloadlen + 2 + 1); }
        }

        public u8[] packet
        {
            get { return buffer; }
        }

        public class type1005
        {
            public byte galileoind = 0;
            public byte glonassind = 0;
            public byte gpsind = 1;
            public byte itrf;
            public byte oscind = 1;
            public byte quatcycind = 0;
            public byte refstatind = 0;
            public byte resv = 0;
            public double rr0;
            public double rr1;
            public double rr2;
            public ushort staid = 1;

            public double[] ecefposition
            {
                get
                {
                    return new[]
                    {
                        rr0*0.0001,
                        rr1*0.0001,
                        rr2*0.0001
                    };
                }
                set
                {
                    rr0 = value[0] / 0.0001;
                    rr1 = value[1] / 0.0001;
                    rr2 = value[2] / 0.0001;
                }
            }

            public void Read(byte[] buffer)
            {
                uint i = 24 + 12;

                staid = (ushort)getbitu(buffer, i, 12);
                i += 12;
                itrf = (byte)getbitu(buffer, i, 6);
                i += 6 + 4;
                rr0 = getbits_38(buffer, i);
                i += 38 + 2;
                rr1 = getbits_38(buffer, i);
                i += 38 + 2;
                rr2 = getbits_38(buffer, i);
                i += 38;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24;

                setbitu(buffer, i, 12, 1005);
                i += 12; /* message no */
                setbitu(buffer, i, 12, staid);
                i += 12; /* ref station id */
                setbitu(buffer, i, 6, 0);
                i += 6; /* itrf realization year */
                setbitu(buffer, i, 1, 1);
                i += 1; /* gps indicator */
                setbitu(buffer, i, 1, 1);
                i += 1; /* glonass indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* galileo indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* ref station indicator */
                set38bits(buffer, i, ecefposition[0] / 0.0001);
                i += 38; /* antenna ref point ecef-x */
                setbitu(buffer, i, 1, 1);
                i += 1; /* oscillator indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* reserved */
                set38bits(buffer, i, ecefposition[1] / 0.0001);
                i += 38; /* antenna ref point ecef-y */
                setbitu(buffer, i, 2, 0);
                i += 2; /* quarter cycle indicator */
                set38bits(buffer, i, ecefposition[2] / 0.0001);
                i += 38; /* antenna ref point ecef-z */

                return i;
            }
        }

        public class type1006
        {
            public ushort anth;
            public byte galileoind = 0;
            public byte glonassind = 0;
            public byte gpsind = 1;
            public byte itrf;
            public byte oscind = 1;
            public byte quatcycind = 0;
            public byte refstatind = 0;
            public byte resv = 0;
            public double rr0;
            public double rr1;
            public double rr2;
            public ushort staid = 1;

            public double[] ecefposition
            {
                get
                {
                    return new[]
                    {
                        rr0*0.0001,
                        rr1*0.0001,
                        rr2*0.0001
                    };
                }
                set
                {
                    rr0 = value[0] / 0.0001;
                    rr1 = value[1] / 0.0001;
                    rr2 = value[2] / 0.0001;
                }
            }

            public void Read(byte[] buffer)
            {
                uint i = 24 + 12;

                staid = (ushort)getbitu(buffer, i, 12);
                i += 12;
                itrf = (byte)getbitu(buffer, i, 6);
                i += 6 + 4;
                rr0 = getbits_38(buffer, i);
                i += 38 + 2;
                rr1 = getbits_38(buffer, i);
                i += 38 + 2;
                rr2 = getbits_38(buffer, i);
                i += 38;
                anth = (ushort)getbitu(buffer, i, 16);
                i += 16;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24;

                setbitu(buffer, i, 12, 1005);
                i += 12; /* message no */
                setbitu(buffer, i, 12, staid);
                i += 12; /* ref station id */
                setbitu(buffer, i, 6, 0);
                i += 6; /* itrf realization year */
                setbitu(buffer, i, 1, 1);
                i += 1; /* gps indicator */
                setbitu(buffer, i, 1, 1);
                i += 1; /* glonass indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* galileo indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* ref station indicator */
                set38bits(buffer, i, ecefposition[0] / 0.0001);
                i += 38; /* antenna ref point ecef-x */
                setbitu(buffer, i, 1, 1);
                i += 1; /* oscillator indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* reserved */
                set38bits(buffer, i, ecefposition[1] / 0.0001);
                i += 38; /* antenna ref point ecef-y */
                setbitu(buffer, i, 2, 0);
                i += 2; /* quarter cycle indicator */
                set38bits(buffer, i, ecefposition[2] / 0.0001);
                i += 38; /* antenna ref point ecef-z */
                setbitu(buffer, i, 16, anth);
                i += 16; /* antenna height */

                return i;
            }
        }

        private static double getbits_38(byte[] buff, uint pos)
        {
            return getbits(buff, pos, 32) * 64.0 + getbitu(buff, pos + 32, 6);
        }

        private static int getbits(byte[] buff, uint pos, uint len)
        {
            var bits = getbitu(buff, pos, len);
            if (len <= 0 || 32 <= len || !((bits & (1u << (int)(len - 1))) != 0))
                return (int)bits;
            return (int)(bits | (~0u << (int)len)); /* extend sign */
        }

        private static void set38bits(byte[] buff, uint pos, double value)
        {
            var word_h = (int)Math.Floor(value / 64.0);
            var word_l = (uint)(value - word_h * 64.0);
            setbits(buff, pos, 32, word_h);
            setbitu(buff, pos + 32, 6, word_l);
        }

        private static void setbits(byte[] buff, uint pos, uint len, int data)
        {
            if (data < 0)
                data |= 1 << (int)(len - 1);
            else
                data &= ~(1 << (int)(len - 1)); /* set sign bit */
            setbitu(buff, pos, len, (uint)data);
        }

        const double RE_WGS84 = 6378137.0; /* earth semimajor axis (WGS84) (m) */

        const double FE_WGS84 = (1.0 / 298.257223563); /* earth flattening (WGS84) */

        const double PI = Math.PI; /* pi */
        public const double D2R = (PI / 180.0); /* deg to rad */
        public const double R2D = (180.0 / PI); /* rad to deg */

        public static void ecef2pos(double[] r, ref double[] pos)
        {
            double e2 = FE_WGS84 * (2.0 - FE_WGS84), r2 = dot(r, r, 2), z, zk, v = RE_WGS84, sinp;

            for (z = r[2], zk = 0.0; Math.Abs(z - zk) >= 1E-4;)
            {
                zk = z;
                sinp = z / Math.Sqrt(r2 + z * z);
                v = RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);
                z = r[2] + v * e2 * sinp;
            }
            pos[0] = r2 > 1E-12 ? Math.Atan(z / Math.Sqrt(r2)) : (r[2] > 0.0 ? PI / 2.0 : -PI / 2.0);
            pos[1] = r2 > 1E-12 ? Math.Atan2(r[1], r[0]) : 0.0;
            pos[2] = Math.Sqrt(r2 + z * z) - v;
        }

        static double dot(double[] a, double[] b, int n)
        {
            double c = 0.0;

            while (--n >= 0) c += a[n] * b[n];
            return c;
        }
    }
}