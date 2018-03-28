using System;
using System.Collections.Generic;
using u8 = System.Byte;
using u16 = System.UInt16;
using s32 = System.Int32;
using u32 = System.UInt32;
using gps_time_t = System.UInt64;
using System.Runtime.InteropServices;
using RTCM3 = MissionPlanner.Utilities.rtcm3;

namespace MissionPlanner.Utilities
{
    public class rtcm3 : ICorrections
    {
        private const byte RTCM3PREAMB = 0xD3;
        private const double PRUNIT_GPS = 299792.458; /* rtcm ver.3 unit of gps pseudorange (m) */
        private const double PRUNIT_GLO = 599584.916; /* rtcm 3 unit of glo pseudorange (m) */
        private const double CLIGHT = 299792458.0; /* speed of light (m/s) */
        private const double SC2RAD = 3.1415926535898; /* semi-circle to radian (IS-GPS) */
        private const double FREQ1 = 1.57542E9; /* L1/E1  frequency (Hz) */
        private const double FREQ2 = 1.22760E9; /* L2     frequency (Hz) */
        private const double RANGE_MS = CLIGHT*0.001; /* range in 1 ms */
        private const double P2_5 = 0.03125; /* 2^-5 */
        private const double P2_6 = 0.015625; /* 2^-6 */
        private const double P2_10 = 0.0009765625; /* 2^-10 */
        private const double P2_11 = 4.882812500000000E-04; /* 2^-11 */
        private const double P2_15 = 3.051757812500000E-05; /* 2^-15 */
        private const double P2_17 = 7.629394531250000E-06; /* 2^-17 */
        private const double P2_19 = 1.907348632812500E-06; /* 2^-19 */
        private const double P2_20 = 9.536743164062500E-07; /* 2^-20 */
        private const double P2_21 = 4.768371582031250E-07; /* 2^-21 */
        private const double P2_23 = 1.192092895507810E-07; /* 2^-23 */
        private const double P2_24 = 5.960464477539063E-08; /* 2^-24 */
        private const double P2_27 = 7.450580596923828E-09; /* 2^-27 */
        private const double P2_29 = 1.862645149230957E-09; /* 2^-29 */
        private const double P2_30 = 9.313225746154785E-10; /* 2^-30 */
        private const double P2_31 = 4.656612873077393E-10; /* 2^-31 */
        private const double P2_32 = 2.328306436538696E-10; /* 2^-32 */
        private const double P2_33 = 1.164153218269348E-10; /* 2^-33 */
        private const double P2_35 = 2.910383045673370E-11; /* 2^-35 */
        private const double P2_38 = 3.637978807091710E-12; /* 2^-38 */
        private const double P2_39 = 1.818989403545856E-12; /* 2^-39 */
        private const double P2_40 = 9.094947017729280E-13; /* 2^-40 */
        private const double P2_43 = 1.136868377216160E-13; /* 2^-43 */
        private const double P2_48 = 3.552713678800501E-15; /* 2^-48 */
        private const double P2_50 = 8.881784197001252E-16; /* 2^-50 */
        private const double P2_55 = 2.775557561562891E-17; /* 2^-55 */
        private const double RE_WGS84 = 6378137.0; /* earth semimajor axis (WGS84) (m) */
        private const double FE_WGS84 = (1.0/298.257223563); /* earth flattening (WGS84) */
        private const double PI = Math.PI; /* pi */
        public const double D2R = (PI/180.0); /* deg to rad */
        public const double R2D = (180.0/PI); /* rad to deg */

        public u32 sync;

        private static double lasttow = 0;

        public static int weekGuess = 0;

        private int msglencount;
        private uint payloadlen;
        private rtcmpreamble pre;
        public int step;

        public event EventHandler ObsMessage;
        public event EventHandler BasePosMessage;
        public event EventHandler EphMessage;

        public int length
        {
            get { return (int) (payloadlen + 2 + 1); }
        }

        public byte[] packet { get; } = new byte[1024*4];

        public bool resetParser()
        {
            step = 0;
            return true;
        }

        public int Read(byte data)
        {
            switch (step)
            {
                default:
                case 0:
                    if (data == RTCM3PREAMB)
                    {
                        step = 1;
                        packet[0] = data;
                    }
                    break;
                case 1:
                    packet[1] = data;
                    step++;
                    break;
                case 2:
                    packet[2] = data;
                    step++;
                    pre = new rtcmpreamble();
                    pre.Read(packet);
                    payloadlen = pre.length;
                    msglencount = 0;
                    // reset on oversize packet
                    if (payloadlen > packet.Length)
                        step = 0;
                    break;
                case 3:
                    if (msglencount < (payloadlen))
                    {
                        packet[msglencount + 3] = data;
                        msglencount++;
                    }
                    else
                    {
                        step++;
                        goto case 4;
                    }
                    break;
                case 4:
                    packet[payloadlen + 3] = data;
                    step++;
                    break;
                case 5:
                    packet[payloadlen + 3 + 1] = data;
                    step++;
                    break;
                case 6:
                    packet[payloadlen + 3 + 2] = data;

                    payloadlen = payloadlen + 3;
                    var crc = crc24.crc24q(packet, payloadlen, 0);
                    var crcpacket = getbitu(packet, payloadlen*8, 24);

                    if (crc == crcpacket)
                    {
                        var head = new rtcmheader();
                        head.Read(packet);

                        sync = head.sync;

                        step = 0;

                        if (head.messageno == 1002)
                        {
                            var tp = new type1002();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1004)
                        {
                            var tp = new type1004();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1012)
                        {
                            var tp = new type1012();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1074)
                        {
                            var tp = new type1074();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1077)
                        {
                            var tp = new type1077();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1084)
                        {
                            var tp = new type1084();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1087)
                        {
                            var tp = new type1087();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1124)
                        {
                            var tp = new type1124();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1127)
                        {
                            var tp = new type1127();

                            tp.Read(packet);

                            if (ObsMessage != null)
                                ObsMessage(tp.obs, null);
                        }
                        else if (head.messageno == 1005)
                        {
                            var tp = new type1005();

                            tp.Read(packet);

                            if (BasePosMessage != null)
                                BasePosMessage(tp, null);
                        }
                        else if (head.messageno == 1006)
                        {
                            var tp = new type1006();

                            tp.Read(packet);

                            if (BasePosMessage != null)
                                BasePosMessage(tp, null);
                        }
                        else if (head.messageno == 1019)
                        {
                            var tp = new type1019();

                            tp.Read(packet);

                            var week = (int)tp.week + 1024;

                            // both at start of a week
                            if (tp.toes < (60 * 60 * 24) && lasttow < (60 * 60 * 24))
                            {
                                RTCM3.weekGuess = week;
                            } // eph at end of week and tow at start of week
                            else if (tp.toes > 500000 && lasttow < (60 * 60 * 24))
                            {
                                RTCM3.weekGuess = week + 1;
                            } // everything else
                            else
                            {
                                RTCM3.weekGuess = week;
                            }

                            if (EphMessage != null)
                                EphMessage(tp, null);
                        }

                        return head.messageno;
                    }
                    step = 0;
                    break;
            }

            return -1;
        }

        private static uint getbitu(byte[] buff, uint pos, uint len)
        {
            uint bits = 0;
            uint i;
            for (i = pos; i < pos + len; i++)
                bits = (uint) ((bits << 1) + ((buff[i/8] >> (int) (7 - i%8)) & 1u));
            return bits;
        }

        private static void setbitu(byte[] buff, uint pos, uint len, double data)
        {
            setbitu(buff, pos, len, (uint)data);
        }

        private static void setbitu(byte[] buff, uint pos, uint len, uint data)
        {
            var mask = 1u << (int) (len - 1);

            if (len <= 0 || 32 < len) return;

            for (var i = pos; i < pos + len; i++, mask >>= 1)
            {
                if ((data & mask) > 0)
                    buff[i/8] |= (byte) (1u << (int) (7 - i%8));
                else
                    buff[i/8] &= (byte) (~(1u << (int) (7 - i%8)));
            }
        }

        private static double ROUND(double x)
        {
            return (int) Math.Floor(x + 0.5);
        }

        /* carrier-phase - pseudorange in cycle --------------------------------------*/

        private static double cp_pr(double cp, double pr_cyc)
        {
            var x = (cp - pr_cyc + 1500.0)%3000.0;
            if (x < 0)
                x += 3000;
            x -= 1500.0;
            return x;
        }

        private static double getbits_38(byte[] buff, uint pos)
        {
            return getbits(buff, pos, 32)*64.0 + getbitu(buff, pos + 32, 6);
        }

        private static int getbits(byte[] buff, uint pos, uint len)
        {
            var bits = getbitu(buff, pos, len);
            if (len <= 0 || 32 <= len || !((bits & (1u << (int) (len - 1))) != 0))
                return (int) bits;
            return (int) (bits | (~0u << (int) len)); /* extend sign */
        }

        private static void set38bits(byte[] buff, uint pos, double value)
        {
            var word_h = (int) Math.Floor(value/64.0);
            var word_l = (uint) (value - word_h*64.0);
            setbits(buff, pos, 32, word_h);
            setbitu(buff, pos + 32, 6, word_l);
        }

        private static void setbits(byte[] buff, uint pos, uint len, double data)
        {
            setbits(buff, pos, len, (int)data);
        }

        private static void setbits(byte[] buff, uint pos, uint len, int data)
        {
            if (data < 0)
                data |= 1 << (int) (len - 1);
            else
                data &= ~(1 << (int) (len - 1)); /* set sign bit */
            setbitu(buff, pos, len, (uint) data);
        }

        public static void ecef2pos(double[] r, ref double[] pos)
        {
            double e2 = FE_WGS84*(2.0 - FE_WGS84), r2 = dot(r, r, 2), z, zk, v = RE_WGS84, sinp;

            for (z = r[2], zk = 0.0; Math.Abs(z - zk) >= 1E-4;)
            {
                zk = z;
                sinp = z/Math.Sqrt(r2 + z*z);
                v = RE_WGS84/Math.Sqrt(1.0 - e2*sinp*sinp);
                z = r[2] + v*e2*sinp;
            }
            pos[0] = r2 > 1E-12 ? Math.Atan(z/Math.Sqrt(r2)) : (r[2] > 0.0 ? PI/2.0 : -PI/2.0);
            pos[1] = r2 > 1E-12 ? Math.Atan2(r[1], r[0]) : 0.0;
            pos[2] = Math.Sqrt(r2 + z*z) - v;
        }

        public static void pos2ecef(double[] pos, ref double[] r)
        {
            double sinp = Math.Sin(pos[0]),
                cosp = Math.Cos(pos[0]),
                sinl = Math.Sin(pos[1]),
                cosl = Math.Cos(pos[1]);
            double e2 = FE_WGS84 * (2.0 - FE_WGS84), v = RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);

            r[0] = (v + pos[2]) * cosp * cosl;
            r[1] = (v + pos[2]) * cosp * sinl;
            r[2] = (v * (1.0 - e2) + pos[2]) * sinp;
        }

        private static double dot(double[] a, double[] b, int n)
        {
            var c = 0.0;

            while (--n >= 0) c += a[n]*b[n];
            return c;
        }

        public class rtcmpreamble
        {
            public ushort length;
            public byte preamble = RTCM3PREAMB;
            public byte resv1;

            public void Read(byte[] buffer)
            {
                uint i = 0;

                preamble = (byte) getbitu(buffer, i, 8);
                i += 8;
                resv1 = (byte) getbitu(buffer, i, 6);
                i += 6;
                length = (ushort) getbitu(buffer, i, 10);
                i += 10;
            }

            public byte[] Write(byte[] buffer)
            {
                uint i = 0;

                setbitu(buffer, i, 8, RTCM3PREAMB);
                i += 8;
                setbitu(buffer, i, 6, resv1);
                i += 6;
                setbitu(buffer, i, 10, length);
                i += 10;

                return buffer;
            }
        }

        public class rtcmheader
        {
            public uint epoch;
            public ushort messageno;
            public byte nsat;
            public ushort refstationid;
            public byte smoothind;
            public byte smoothint;
            public byte sync;

            public void Read(byte[] buffer)
            {
                uint i = 24;

                messageno = (ushort) getbitu(buffer, i, 12);
                i += 12; /* message no */
                refstationid = (ushort) getbitu(buffer, i, 12);
                i += 12; /* ref station id */
                if (messageno < 1009 || messageno > 1012)
                {
                    epoch = getbitu(buffer, i, 30);
                    i += 30; /* gps epoch time */
                }
                else
                {
                    epoch = getbitu(buffer, i, 27);
                    i += 27; /* glonass epoch time */
                }
                sync = (byte) getbitu(buffer, i, 1);
                i += 1; /* synchronous gnss flag */
                nsat = (byte) getbitu(buffer, i, 5);
                i += 5; /* no of satellites */
                smoothind = (byte) getbitu(buffer, i, 1);
                i += 1; /* smoothing indicator */
                smoothint = (byte) getbitu(buffer, i, 3);
                i += 3; /* smoothing interval */
            }

            public byte[] Write(byte[] buffer)
            {
                uint i = 24;

                setbitu(buffer, i, 12, messageno);
                i += 12; /* message no */
                setbitu(buffer, i, 12, refstationid);
                i += 12; /* ref station id */
                setbitu(buffer, i, 30, epoch);
                i += 30; /* gps epoch time */
                setbitu(buffer, i, 1, sync);
                i += 1; /* synchronous gnss flag */
                setbitu(buffer, i, 5, nsat);
                i += 5; /* no of satellites */
                setbitu(buffer, i, 1, smoothind);
                i += 1; /* smoothing indicator */
                setbitu(buffer, i, 3, smoothint);
                i += 3; /* smoothing interval */
                return buffer;
            }
        }

        public class crc24
        {
            private static readonly uint[] crc24qtab =
            {
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

            public static uint crc24q(byte[] buf, uint len, uint crc)
            {
                for (uint i = 0; i < len; i++)
                    crc = ((crc << 8) & 0xFFFFFF) ^ crc24qtab[(crc >> 16) ^ buf[i]];
                return crc;
            }
        }

        public class type1002
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30)*0.001;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var nsat = getbitu(buffer, i, 5);

                i = 24 + 64;

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);

                for (var a = 0; a < nsat; a++)
                {
                    var ob = new ob();
                    ob.sys = 'G';
                    ob.tow = tow;
                    ob.week = week;

                    ob.raw.prn = (byte) getbitu(buffer, i, 6);
                    i += 6;
                    ob.raw.code1 = (byte) getbitu(buffer, i, 1);
                    i += 1;
                    ob.raw.pr1 = getbitu(buffer, i, 24);
                    i += 24;
                    ob.raw.ppr1 = getbits(buffer, i, 20);
                    i += 20;
                    ob.raw.lock1 = (byte) getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.amb = (byte) getbitu(buffer, i, 8);
                    i += 8;
                    ob.raw.cnr1 = (byte) getbitu(buffer, i, 8);
                    i += 8;

                    var pr1 = ob.raw.pr1*0.02 + ob.raw.amb*PRUNIT_GPS;

                    var lam1 = CLIGHT/FREQ1;

                    var cp1 = ob.raw.ppr1*0.0005/lam1;

                    if ((uint) ob.raw.ppr1 != 0xFFF80000)
                    {
                        ob.prn = ob.raw.prn;
                        ob.cp = pr1/lam1 + cp1;
                        ob.pr = pr1;
                        ob.snr = (byte) (ob.raw.cnr1*0.25); // *4.0+0.5

                        obs.Add(ob);

                        //Console.WriteLine("G{0,2} {1,13} {2,16} {3,30}", ob.prn, ob.pr.ToString("0.000"),ob.cp.ToString("0.0000"), ob.snr.ToString("0.000"));
                    }
                }

                obs.Sort(delegate(ob a, ob b) { return a.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24 + 64;

                foreach (var ob in obs)
                {
                    var lam1 = CLIGHT/FREQ1;

                    var amb = (int) Math.Floor(ob.pr/PRUNIT_GPS);
                    var pr1 = ROUND((ob.pr - amb*PRUNIT_GPS)/0.02);
                    var pr1c = pr1*0.02 + amb*PRUNIT_GPS;

                    var ppr = cp_pr(ob.cp, pr1c/lam1);

                    var ppr1 = ROUND(ppr*lam1/0.0005);

                    setbitu(buffer, i, 6, ob.prn);
                    i += 6;
                    setbitu(buffer, i, 1, 0);
                    i += 1;
                    setbitu(buffer, i, 24, (uint) pr1);
                    i += 24;
                    setbits(buffer, i, 20, (int) ppr1);
                    i += 20;
                    setbitu(buffer, i, 7, ob.raw.lock1);
                    i += 7;
                    setbitu(buffer, i, 8, (byte) amb);
                    i += 8;
                    setbitu(buffer, i, 8, (byte) (ob.snr*4));
                    i += 8;
                }

                nbits = i;

                return i;
            }
        }

        internal static class StaticUtils
        {
            public static DateTime GetFromGps(int weeknumber, double seconds)
            {
                var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
                var week = datum.AddDays(weeknumber*7);
                var time = week.AddSeconds(seconds);
                return time;
            }

            public static int LeapSecondsGPS(int year, int month)
            {
                return LeapSecondsTAI(year, month) - 19;
            }

            public static int LeapSecondsTAI(int year, int month)
            {
                //http://maia.usno.navy.mil/ser7/tai-utc.dat

                var yyyymm = year * 100 + month;
                if (yyyymm >= 201701) return 37;
                if (yyyymm >= 201507) return 36;
                if (yyyymm >= 201207) return 35;
                if (yyyymm >= 200901) return 34;
                if (yyyymm >= 200601) return 33;
                if (yyyymm >= 199901) return 32;
                if (yyyymm >= 199707) return 31;
                if (yyyymm >= 199601) return 30;

                return 0;
            }

            public static void GetFromTime(DateTime time, ref int week, ref double seconds)
            {
                var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

                var dif = time - datum;

                var weeks = (int) (dif.TotalDays/7);

                week = weeks;

                dif = time - datum.AddDays(weeks*7);

                seconds = dif.TotalSeconds;
            }
        }

        public class type1004
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30)*0.001;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var nsat = getbitu(buffer, i, 5);
                i += 5;

                i = 24 + 64;

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0} {1} {2} {3,2} {4} {5} {6} {7}", gpstime.Year, gpstime.Month, gpstime.Day,gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);

                for (var a = 0; a < nsat; a++)
                {
                    var ob = new ob();
                    ob.sys = 'G';
                    ob.tow = tow;
                    ob.week = week;

                    ob.raw.prn = (byte) getbitu(buffer, i, 6);
                    i += 6;
                    ob.raw.code1 = (byte) getbitu(buffer, i, 1);
                    i += 1;
                    ob.raw.pr1 = getbitu(buffer, i, 24);
                    i += 24;
                    ob.raw.ppr1 = getbits(buffer, i, 20);
                    i += 20;
                    ob.raw.lock1 = (byte) getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.amb = (byte) getbitu(buffer, i, 8);
                    i += 8;
                    ob.raw.cnr1 = (byte) getbitu(buffer, i, 8);
                    i += 8;
                    ob.raw.code2 = (byte) getbitu(buffer, i, 2);
                    i += 2;
                    ob.raw.pr21 = getbits(buffer, i, 14);
                    i += 14;
                    ob.raw.ppr2 = getbits(buffer, i, 20);
                    i += 20;
                    ob.raw.lock2 = (byte) getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.cnr2 = (byte) getbitu(buffer, i, 8);
                    i += 8;

                    var pr1 = ob.raw.pr1*0.02 + ob.raw.amb*PRUNIT_GPS;

                    var lam1 = CLIGHT/FREQ1;
                    var lam2 = CLIGHT/FREQ2;

                    var cp1 = ob.raw.ppr1*0.0005/lam1;

                    if ((uint) ob.raw.ppr1 != 0xFFF80000)
                    {
                        ob.prn = ob.raw.prn;
                        ob.cp = pr1/lam1 + cp1;
                        ob.pr = pr1;
                        ob.snr = (byte) (ob.raw.cnr1*0.25); // *4.0+0.5

                        ob.pr2 = pr1 + ob.raw.pr21*0.02;
                        ob.cp2 = pr1/lam2 + ob.raw.ppr2*0.0005/lam2;

                        obs.Add(ob);

                        //   Console.WriteLine("G{0,2} {1,13} {2,15}0{3,15} {4,15}0{5,15}", ob.prn, ob.pr.ToString("0.000"), ob.cp.ToString("0.000"), ob.snr.ToString("0.000"),
                        //       ob.pr2.ToString("0.000"), ob.cp2.ToString("0.000"));
                    }
                }

                obs.Sort(delegate(ob a, ob b) { return a.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24 + 64;

                foreach (var ob in obs)
                {
                    var lam1 = CLIGHT/FREQ1;

                    var amb = (int) Math.Floor(ob.pr/PRUNIT_GPS);
                    var pr1 = ROUND((ob.pr - amb*PRUNIT_GPS)/0.02);
                    var pr1c = pr1*0.02 + amb*PRUNIT_GPS;

                    var ppr = cp_pr(ob.cp, pr1c/lam1);
                    var ppr1 = ROUND(ppr*lam1/0.0005);

                    setbitu(buffer, i, 6, ob.prn);
                    i += 6;
                    setbitu(buffer, i, 1, 0);
                    i += 1;
                    setbitu(buffer, i, 24, (uint) pr1);
                    i += 24;
                    setbits(buffer, i, 20, (int) ppr1);
                    i += 20;
                    setbitu(buffer, i, 7, ob.raw.lock1);
                    i += 7;
                    setbitu(buffer, i, 8, (byte) amb);
                    i += 8;
                    setbitu(buffer, i, 8, (byte) (ob.snr*4));
                    i += 8;
                    // l2 - all 0's
                    setbitu(buffer, i, 2, ob.raw.code2);
                    i += 2;
                    setbits(buffer, i, 14, ob.raw.pr21);
                    i += 14;
                    setbits(buffer, i, 20, ob.raw.ppr2);
                    i += 20;
                    setbitu(buffer, i, 7, ob.raw.lock2);
                    i += 7;
                    setbitu(buffer, i, 8, ob.raw.cnr2);
                    i += 8;
                }

                nbits = i;

                return i;
            }
        }

        public class type1012
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 27) * 0.001;
                i += 27;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var nsat = getbitu(buffer, i, 5);
                i += 5;
                var smoothind = getbitu(buffer, i, 1);
                i += 1;
                var smoothint = getbitu(buffer, i, 3);
                i += 3;

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow - 10800 - (60*60*24));

                gpstime = gpstime.AddSeconds(StaticUtils.LeapSecondsGPS(gpstime.Year, gpstime.Month));

                lasttow = tow;

                Console.WriteLine("> {0} {1} {2} {3,2} {4} {5} {6} {7}", gpstime.Year, gpstime.Month, gpstime.Day,gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);

                for (var a = 0; a < nsat; a++)
                {
                    var ob = new ob();
                    ob.sys = 'R';
                    ob.tow = tow;
                    ob.week = week;

                    ob.raw.prn = (byte)getbitu(buffer, i, 6);
                    i += 6;
                    ob.raw.code1 = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    ob.raw.fcn = (byte)getbitu(buffer, i, 5);
                    i += 5;
                    ob.raw.pr1 = getbitu(buffer, i, 25);
                    i += 25;
                    ob.raw.ppr1 = getbits(buffer, i, 20);
                    i += 20;
                    ob.raw.lock1 = (byte)getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.amb = (byte)getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.cnr1 = (byte)getbitu(buffer, i, 8);
                    i += 8;
                    ob.raw.code2 = (byte)getbitu(buffer, i, 2);
                    i += 2;
                    ob.raw.pr21 = getbits(buffer, i, 14);
                    i += 14;
                    ob.raw.ppr2 = getbits(buffer, i, 20);
                    i += 20;
                    ob.raw.lock2 = (byte)getbitu(buffer, i, 7);
                    i += 7;
                    ob.raw.cnr2 = (byte)getbitu(buffer, i, 8);
                    i += 8;

                    var pr1 = ob.raw.pr1 * 0.02 + ob.raw.amb * PRUNIT_GLO;

                    var lam1 = CLIGHT / FREQ1;
                    var lam2 = CLIGHT / FREQ2;

                    var cp1 = ob.raw.ppr1 * 0.0005 / lam1;

                    if ((uint)ob.raw.ppr1 != 0xFFF80000)
                    {
                        ob.prn = ob.raw.prn;
                        ob.cp = pr1 / lam1 + cp1;
                        ob.pr = pr1;
                        ob.snr = (byte)(ob.raw.cnr1 * 0.25); // *4.0+0.5

                        ob.pr2 = pr1 + ob.raw.pr21 * 0.02;
                        ob.cp2 = pr1 / lam2 + ob.raw.ppr2 * 0.0005 / lam2;

                        obs.Add(ob);

                           Console.WriteLine("R{0,2} {1,13} {2,15}0{3,15} {4,15}0{5,15}", ob.prn, ob.pr.ToString("0.000"), ob.cp.ToString("0.000"), ob.snr.ToString("0.000"),
                               ob.pr2.ToString("0.000"), ob.cp2.ToString("0.000"));
                    }
                }

                obs.Sort(delegate (ob a, ob b) { return a.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24 + 64;

                foreach (var ob in obs)
                {
                    var lam1 = CLIGHT/FREQ1;

                    var amb = (int) Math.Floor(ob.pr/PRUNIT_GPS);
                    var pr1 = ROUND((ob.pr - amb*PRUNIT_GPS)/0.02);
                    var pr1c = pr1*0.02 + amb*PRUNIT_GPS;

                    var ppr = cp_pr(ob.cp, pr1c/lam1);
                    var ppr1 = ROUND(ppr*lam1/0.0005);

                }

                return i;
            }
        }


        public class ob
        {
            public double cp;
            public double cp2;
            public double pr;
            public double pr2;
			
            public byte prn;
            public rawrtcm raw = new rawrtcm();
            public byte snr;
            public double tow;
            public int week;
            public char sys;

            public class rawrtcm
            {
                public byte amb;
                public byte cnr1;
                public byte cnr2;
                public byte code1;
                public byte code2;
                public byte lock1;
                public byte lock2;
                public int ppr1;
                public int ppr2;
                public uint pr1;
                public int pr21;
                public byte prn;
                public byte fcn;
            }
        }

        public class type1074
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30) * 0.001;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);

                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 15);
                    i += 15;
                    if (prv != -16384) pr[j] = prv * P2_24 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 22);
                    i += 22;
                    if (cpv != -2097152) cp[j] = cpv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 6)*1;// * 0.0625;
                    i += 6;
                }

                var lam1 = CLIGHT / FREQ1;
                var lam2 = CLIGHT / FREQ2;

                var sig = 0;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'G';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)sats[j];

                    ob.pr = r[j] + pr[sig];
                    ob.cp = (r[j] + cp[sig]) / lam1;
                    ob.snr = (byte)(cnr[sig]);

                    for (int k = 0; k < nsig; k++)
                    {
                        if (k == 1)
                        {
                            ob.pr2 = r[j] + pr[sig];
                            ob.cp2 = (r[j] + cp[sig]);
                        }
                        sig++;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate (ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
        }


        public class type1077
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30) * 0.001;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);


                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* extended info */
                    ex[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* phaserangerate */
                    var rate = getbits(buffer, i, 14);
                    i += 14;
                    if (rate != -8192) rr[j] = rate * 1.0;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 20);
                    i += 20;
                    if (prv != -524288) pr[j] = prv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 24);
                    i += 24;
                    if (cpv != -8388608) cp[j] = cpv * P2_31 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 10);
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 10) * 0.0625;
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserangerate */
                    var rrv = getbits(buffer, i, 15);
                    i += 15;
                    if (rrv != -16384) rrf[j] = rrv * 0.0001;
                }

                var lam1 = CLIGHT / FREQ1;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'G';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)sats[j];

                    ob.pr = r[j] + pr[j];
                    ob.cp = (r[j] + cp[j])/lam1;
                    ob.snr = (byte) (cnr[j]);

                    if (nsig > 1)
                    {
                        ob.pr2 = r[j] + pr[j + sats.Count * 1];
                        ob.cp2 = (r[j] + cp[j + sats.Count * 1]); // / lam2;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate(ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
        }

        public class type1084
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var dow = getbitu(buffer, i, 3);
                i += 3;
                var tod = getbitu(buffer, i, 27) * 0.001;
                i += 27;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var tow = 0.0;

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week , tod - 10800);

                gpstime = gpstime.AddSeconds(StaticUtils.LeapSecondsGPS(gpstime.Year, gpstime.Month));

                StaticUtils.GetFromTime(gpstime, ref week, ref tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);


                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 15);
                    i += 15;
                    if (prv != -16384) pr[j] = prv * P2_24 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 22);
                    i += 22;
                    if (cpv != -2097152) cp[j] = cpv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 6) * 1;// * 0.0625;
                    i += 6;
                }

                var lam1 = CLIGHT / FREQ1;

                var sig = 0;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'R';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)(sats[j]);

                    ob.pr = r[j] + pr[sig];
                    ob.cp = (r[j] + cp[sig]) / lam1;
                    ob.snr = (byte)(cnr[sig]);

                    for (int k = 0; k < nsig; k++)
                    {
                        if (k == 1)
                        {
                            ob.pr2 = r[j] + pr[sig];
                            ob.cp2 = (r[j] + cp[sig]);
                        }
                        sig++;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate (ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
        }


        public class type1087
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var dow = getbitu(buffer, i, 3);
                i += 3;
                var tod = getbitu(buffer, i, 27) * 0.001;
                i += 27;
                var tow = tod;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);


                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* extended info */
                    ex[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* phaserangerate */
                    var rate = getbits(buffer, i, 14);
                    i += 14;
                    if (rate != -8192) rr[j] = rate * 1.0;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 20);
                    i += 20;
                    if (prv != -524288) pr[j] = prv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 24);
                    i += 24;
                    if (cpv != -8388608) cp[j] = cpv * P2_31 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 10);
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 10) * 0.0625;
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserangerate */
                    var rrv = getbits(buffer, i, 15);
                    i += 15;
                    if (rrv != -16384) rrf[j] = rrv * 0.0001;
                }

                var lam1 = CLIGHT / FREQ1;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'R';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)sats[j];

                    ob.pr = r[j] + pr[j];
                    ob.cp = (r[j] + cp[j]) / lam1;
                    ob.snr = (byte)(cnr[j]);

                    if (nsig > 1)
                    {
                        ob.pr2 = r[j] + pr[j + sats.Count * 1];
                        ob.cp2 = (r[j] + cp[j + sats.Count * 1]); // / lam2;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate (ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
        }

        public class type1124
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30) * 0.001 + 14;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);


                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 15);
                    i += 15;
                    if (prv != -16384) pr[j] = prv * P2_24 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 22);
                    i += 22;
                    if (cpv != -2097152) cp[j] = cpv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 6) * 1;// * 0.0625;
                    i += 6;
                }

                var lam1 = CLIGHT / FREQ1;

                var sig = 0;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'C';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)(sats[j]);

                    ob.pr = r[j] + pr[sig];
                    ob.cp = (r[j] + cp[sig]) / lam1;
                    ob.snr = (byte)(cnr[sig]);

                    for (int k = 0; k < nsig; k++)
                    {
                        if (k == 1)
                        {
                            ob.pr2 = r[j] + pr[sig];
                            ob.cp2 = (r[j] + cp[sig]);
                        }
                        sig++;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate (ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
        }


        public class type1127
        {
            public uint nbits;
            public List<ob> obs = new List<ob>();

            public void Read(byte[] buffer)
            {
                uint i = 24;

                var type = getbitu(buffer, i, 12);
                i += 12;

                var staid = getbitu(buffer, i, 12);
                i += 12;
                var tow = getbitu(buffer, i, 30) * 0.001 + 14;
                i += 30;
                var sync = getbitu(buffer, i, 1);
                i += 1;
                var iod = getbitu(buffer, i, 3);
                i += 3;

                var time_s = getbitu(buffer, i, 7);
                i += 7;
                var clk_str = getbitu(buffer, i, 2);
                i += 2;
                var clk_ext = getbitu(buffer, i, 2);
                i += 2;
                var smooth = getbitu(buffer, i, 1);
                i += 1;
                var tint_s = getbitu(buffer, i, 3);
                i += 3;

                var nsat = 0;
                var nsig = 0;
                var ncell = 0;
                var j = 0;

                var sats = new Dictionary<int, double>();
                var sigs = new Dictionary<int, double>();
                var cellmask = new byte[64];

                for (j = 1; j <= 64; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sats[nsat++] = j;
                }
                for (j = 1; j <= 32; j++)
                {
                    var mask = getbitu(buffer, i, 1);
                    i += 1;
                    if (mask > 0) sigs[nsig++] = j;
                }

                for (j = 0; j < nsat * nsig; j++)
                {
                    cellmask[j] = (byte)getbitu(buffer, i, 1);
                    i += 1;
                    if (cellmask[j] > 0) ncell++;
                }

                // end of header  i=202

                var week = RTCM3.weekGuess;

                var gpstime = StaticUtils.GetFromGps(week, tow);

                lasttow = tow;

                //Console.WriteLine("> {0,4} {1,2} {2,2} {3,2} {4,2} {5,10} {6,2} {7,2}", gpstime.Year, gpstime.Month,gpstime.Day, gpstime.Hour, gpstime.Minute, gpstime.Second + gpstime.Millisecond/1000.0, 0, nsat);


                var r = new double[64];
                var rr = new double[64];
                var pr = new double[64];
                var cp = new double[64];
                var rrf = new double[64];
                var cnr = new double[64];
                var ex = new uint[64];
                var half = new uint[64];
                var @lock = new uint[64];

                for (j = 0; j < nsat; j++)
                {
                    r[j] = rr[j] = 0.0;
                    ex[j] = 15;
                }
                for (j = 0; j < ncell; j++) pr[j] = cp[j] = rrf[j] = -1E16;

                /* decode satellite data */
                for (j = 0; j < nsat; j++)
                {
                    /* range */
                    var rng = getbitu(buffer, i, 8);
                    i += 8;
                    if (rng != 255) r[j] = rng * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* extended info */
                    ex[j] = getbitu(buffer, i, 4);
                    i += 4;
                }
                for (j = 0; j < nsat; j++)
                {
                    var rng_m = getbitu(buffer, i, 10);
                    i += 10;
                    if (r[j] != 0.0) r[j] += rng_m * P2_10 * RANGE_MS;
                }
                for (j = 0; j < nsat; j++)
                {
                    /* phaserangerate */
                    var rate = getbits(buffer, i, 14);
                    i += 14;
                    if (rate != -8192) rr[j] = rate * 1.0;
                }
                /* decode signal data */
                for (j = 0; j < ncell; j++)
                {
                    /* pseudorange */
                    var prv = getbits(buffer, i, 20);
                    i += 20;
                    if (prv != -524288) pr[j] = prv * P2_29 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserange */
                    var cpv = getbits(buffer, i, 24);
                    i += 24;
                    if (cpv != -8388608) cp[j] = cpv * P2_31 * RANGE_MS;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* lock time */
                    @lock[j] = getbitu(buffer, i, 10);
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* half-cycle amiguity */
                    half[j] = getbitu(buffer, i, 1);
                    i += 1;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* cnr */
                    cnr[j] = getbitu(buffer, i, 10) * 0.0625;
                    i += 10;
                }
                for (j = 0; j < ncell; j++)
                {
                    /* phaserangerate */
                    var rrv = getbits(buffer, i, 15);
                    i += 15;
                    if (rrv != -16384) rrf[j] = rrv * 0.0001;
                }

                var lam1 = CLIGHT / FREQ1;

                for (j = 0; j < nsat; j++)
                {
                    var ob = new ob();
                    ob.sys = 'C';
                    ob.tow = tow;
                    ob.week = week;

                    ob.prn = (byte)sats[j];

                    ob.pr = r[j] + pr[j];
                    ob.cp = (r[j] + cp[j]) / lam1;
                    ob.snr = (byte)(cnr[j]);

                    if (nsig > 1)
                    {
                        ob.pr2 = r[j] + pr[j + sats.Count * 1];
                        ob.cp2 = (r[j] + cp[j + sats.Count * 1]); // / lam2;
                    }

                    obs.Add(ob);
                }

                obs.Sort(delegate (ob a1, ob b) { return a1.prn.CompareTo(b.prn); });

                nbits = i;
            }

            public
                uint Write(byte[] buffer)
            {
                return 0;
            }
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
                    rr0 = value[0]/0.0001;
                    rr1 = value[1]/0.0001;
                    rr2 = value[2]/0.0001;
                }
            }

            public void Read(byte[] buffer)
            {
                uint i = 24 + 12;

                staid = (ushort) getbitu(buffer, i, 12);
                i += 12;
                itrf = (byte) getbitu(buffer, i, 6);
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
                set38bits(buffer, i, ecefposition[0]/0.0001);
                i += 38; /* antenna ref point ecef-x */
                setbitu(buffer, i, 1, 1);
                i += 1; /* oscillator indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* reserved */
                set38bits(buffer, i, ecefposition[1]/0.0001);
                i += 38; /* antenna ref point ecef-y */
                setbitu(buffer, i, 2, 0);
                i += 2; /* quarter cycle indicator */
                set38bits(buffer, i, ecefposition[2]/0.0001);
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
                    rr0 = value[0]/0.0001;
                    rr1 = value[1]/0.0001;
                    rr2 = value[2]/0.0001;
                }
            }

            public void Read(byte[] buffer)
            {
                uint i = 24 + 12;

                staid = (ushort) getbitu(buffer, i, 12);
                i += 12;
                itrf = (byte) getbitu(buffer, i, 6);
                i += 6 + 4;
                rr0 = getbits_38(buffer, i);
                i += 38 + 2;
                rr1 = getbits_38(buffer, i);
                i += 38 + 2;
                rr2 = getbits_38(buffer, i);
                i += 38;
                anth = (ushort) getbitu(buffer, i, 16);
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
                set38bits(buffer, i, ecefposition[0]/0.0001);
                i += 38; /* antenna ref point ecef-x */
                setbitu(buffer, i, 1, 1);
                i += 1; /* oscillator indicator */
                setbitu(buffer, i, 1, 0);
                i += 1; /* reserved */
                set38bits(buffer, i, ecefposition[1]/0.0001);
                i += 38; /* antenna ref point ecef-y */
                setbitu(buffer, i, 2, 0);
                i += 2; /* quarter cycle indicator */
                set38bits(buffer, i, ecefposition[2]/0.0001);
                i += 38; /* antenna ref point ecef-z */
                setbitu(buffer, i, 16, anth);
                i += 16; /* antenna height */

                return i;
            }
        }

        public class type1019
        {
            public double A;
            public double af0;
            public double af1;
            public double af2;
            public double cic;
            public double cis;
            public double code;
            public double crc;
            public double crs;
            public double cuc;
            public double cus;
            public double deln;
            public double e;
            public bool fit;
            public uint flag;
            public double i0;
            public double idot;
            public uint iodc;
            public uint iode;
            public double M0;
            public double omg;
            public double OMG0;
            public double OMGd;
            public double prn;
            public double sqrtA;
            public double sva;
            public uint svh;
            public double tgd;
            public double toc;
            public double toes;
            public double week;


            public void Read(byte[] buffer)
            {
                uint i = 24 + 12;

                prn = getbitu(buffer, i, 6);
                i += 6;
                week = getbitu(buffer, i, 10);
                i += 10;
                sva = getbitu(buffer, i, 4);
                i += 4;
                code = getbitu(buffer, i, 2);
                i += 2;
                idot = getbits(buffer, i, 14)*P2_43*SC2RAD;
                i += 14;
                iode = getbitu(buffer, i, 8);
                i += 8;
                toc = getbitu(buffer, i, 16)*16.0;
                i += 16;
                af2 = getbits(buffer, i, 8)*P2_55;
                i += 8;
                af1 = getbits(buffer, i, 16)*P2_43;
                i += 16;
                af0 = getbits(buffer, i, 22)*P2_31;
                i += 22;
                iodc = getbitu(buffer, i, 10);
                i += 10;
                crs = getbits(buffer, i, 16)*P2_5;
                i += 16;
                deln = getbits(buffer, i, 16)*P2_43*SC2RAD;
                i += 16;
                M0 = getbits(buffer, i, 32)*P2_31*SC2RAD;
                i += 32;
                cuc = getbits(buffer, i, 16)*P2_29;
                i += 16;
                e = getbitu(buffer, i, 32)*P2_33;
                i += 32;
                cus = getbits(buffer, i, 16)*P2_29;
                i += 16;
                sqrtA = getbitu(buffer, i, 32)*P2_19;
                i += 32;
                toes = getbitu(buffer, i, 16)*16.0;
                i += 16;
                cic = getbits(buffer, i, 16)*P2_29;
                i += 16;
                OMG0 = getbits(buffer, i, 32)*P2_31*SC2RAD;
                i += 32;
                cis = getbits(buffer, i, 16)*P2_29;
                i += 16;
                i0 = getbits(buffer, i, 32)*P2_31*SC2RAD;
                i += 32;
                crc = getbits(buffer, i, 16)*P2_5;
                i += 16;
                omg = getbits(buffer, i, 32)*P2_31*SC2RAD;
                i += 32;
                OMGd = getbits(buffer, i, 24)*P2_43*SC2RAD;
                i += 24;
                tgd = getbits(buffer, i, 8)*P2_31;
                i += 8;
                svh = getbitu(buffer, i, 6);
                i += 6;
                flag = getbitu(buffer, i, 1);
                i += 1;
                fit = getbitu(buffer, i, 1) > 0 ? true : false; /* 0:4hr,1:>4hr */

                A = sqrtA*sqrtA;
            }

            public uint Write(byte[] buffer)
            {
                uint i = 24;

                var toe = (uint) (toes/16.0);

                setbitu(buffer, i, 12, 1019);
                i += 12;
                setbitu(buffer, i, 6, prn);
                i += 6;
                setbitu(buffer, i, 10, week%1024);
                i += 10;
                setbitu(buffer, i, 4, sva);
                i += 4;
                setbitu(buffer, i, 2, code);
                i += 2;
                setbits(buffer, i, 14, idot/P2_43/SC2RAD);
                i += 14;
                setbitu(buffer, i, 8, iode);
                i += 8;
                setbitu(buffer, i, 16, toc/16.0);
                i += 16;
                setbits(buffer, i, 8, af2/P2_55);
                i += 8;
                setbits(buffer, i, 16, af1/P2_43);
                i += 16;
                setbits(buffer, i, 22, af0/P2_31);
                i += 22;
                setbitu(buffer, i, 10, iodc);
                i += 10;
                setbits(buffer, i, 16, crs/P2_5);
                i += 16;
                setbits(buffer, i, 16, deln/P2_43/SC2RAD);
                i += 16;
                setbits(buffer, i, 32, M0/P2_31/SC2RAD);
                i += 32;
                setbits(buffer, i, 16, cuc/P2_29);
                i += 16;
                setbitu(buffer, i, 32, e/P2_33);
                i += 32;
                setbits(buffer, i, 16, cus/P2_29);
                i += 16;
                setbitu(buffer, i, 32, sqrtA/P2_19);
                i += 32;
                setbitu(buffer, i, 16, toe);
                i += 16;
                setbits(buffer, i, 16, cic/P2_29);
                i += 16;
                setbits(buffer, i, 32, OMG0/P2_31/SC2RAD);
                i += 32;
                setbits(buffer, i, 16, cis/P2_29);
                i += 16;
                setbits(buffer, i, 32, i0/P2_31/SC2RAD);
                i += 32;
                setbits(buffer, i, 16, crc/P2_5);
                i += 16;
                setbits(buffer, i, 32, omg/P2_31/SC2RAD);
                i += 32;
                setbits(buffer, i, 24, OMGd/P2_43/SC2RAD);
                i += 24;
                setbits(buffer, i, 8, tgd/P2_31);
                i += 8;
                setbitu(buffer, i, 6, svh);
                i += 6;
                setbitu(buffer, i, 1, flag);
                i += 1;
                setbitu(buffer, i, 1, fit ? 1 : 0);
                i += 1;

                return i;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct gps_time_t
            {
                public double tow; /**< Seconds since the GPS start of week. */
                public u16 wn;     /**< GPS week number. */

                public gps_time_t(double tow, u16 wn)
                {
                    this.tow = tow;
                    this.wn = wn;
                }

                public override string ToString()
                {
                    return String.Format("{0}:{1}", wn,tow);
                }

                public static bool operator ==(gps_time_t ob1, gps_time_t ob2)
                {
                    return ob1.wn == ob2.wn && ob1.tow == ob2.tow;
                }

                public static bool operator !=(gps_time_t ob1, gps_time_t ob2)
                {
                    return !(ob1 == ob2);
                }
            }

            private double gpsdifftime(gps_time_t end, gps_time_t beginning)
            {
                return (end.wn - beginning.wn) * 7 * 24 * 3600 +
                       end.tow - beginning.tow;
            }

            public double[] pos = new double[3];
            public double[] vel = new double[3];
            public double clock_err;
            public double clock_rate_err;
            public gps_time_t tot;


            public int calc_sat_pos(double[] pos, double[] vel,
                ref double clock_err, ref double clock_rate_err,
                gps_time_t tot)
            {
                if (tot == this.tot)
                {
                    Array.Copy(this.pos, pos, 3);
                    Array.Copy(this.vel, vel, 3);
                    clock_err = this.clock_err;
                    clock_rate_err = this.clock_rate_err;

                    return 0;
                }

                double tempd1 = 0, tempd2, tempd3;
                double tdiff;
                double a; // semi major axis
                double ma, ma_dot; // mean anomoly and first derivative (mean motion)
                double ea, ea_dot, ea_old; // eccentric anomoly, first deriv, iteration var
                double einstein; // relativistic correction
                double al, al_dot; // argument of lattitude and first derivative
                double cal, cal_dot; // corrected argument of lattitude and first deriv
                double r, r_dot; // radius and first derivative
                double inc, inc_dot; // inclination and first derivative
                double x, x_dot, y, y_dot; // position in orbital plan and first derivatives
                double om, om_dot; // omega and first derivatives

                const double NAV_OMEGAE_DOT = 7.2921151467e-005;
                const double NAV_GM = 3.986005e14;

                var toc = new gps_time_t(this.toc, (u16)(week+1024 ));
                var toe = new gps_time_t(toes, (u16)(week+1024));

                // Satellite clock terms
                // Seconds from clock data reference time (toc)
                tdiff = gpsdifftime(tot, toc);

                if (tdiff > 4 * 3600)
                    tdiff = gpsdifftime(tot, toe);

                clock_err = this.af0 + tdiff * (this.af1 + tdiff * this.af2) - this.tgd;
                clock_rate_err = this.af1 + 2.0 * tdiff * this.af2;

                // Seconds from the time from ephemeris reference epoch (toe)
                tdiff = gpsdifftime(tot, toe);

                // If tdiff is too large our ephemeris isn't valid, maybe we want to wait until we get a
                // new one? At least let's warn the user.
                // TODO: this doesn't exclude ephemerides older than a week so could be made better.
                if (Math.Abs(tdiff) > 4 * 3600)
                {
                    Console.Write(" WARNING: using ephemeris older (or newer!) than 4 hours. {0} {1} hrs       \r", prn,
                        tdiff / 3600);
                    return -1;
                }

                // Calculate position per IS-GPS-200D p 97 Table 20-IV
                a = sqrtA * sqrtA; // [m] Semi-major axis
                ma_dot = Math.Sqrt(NAV_GM / (a * a * a)) + deln; // [rad/sec] Corrected mean motion
                ma = M0 + ma_dot * tdiff; // [rad] Corrected mean anomaly

                // Iteratively solve for the Eccentric Anomaly (from Keith Alter and David Johnston)
                ea = ma; // Starting value for E
                double ecc = e;
                u32 count = 0;

                /* TODO: Implement convergence test uSing integer difference of doubles,
   * http://www.cygnus-software.com/papers/comparingfloats/comparingfloats.htm */
                do
                {
                    ea_old = ea;
                    tempd1 = 1.0 - ecc * Math.Cos(ea_old);
                    ea = ea + (ma - ea_old + ecc * Math.Sin(ea_old)) / tempd1;
                    count++;
                    if (count > 5)
                        break;
                } while (Math.Abs(ea - ea_old) > 1.0E-14);
                ea_dot = ma_dot / tempd1;

                // Relativistic correction term
                einstein = -4.442807633E-10 * ecc * sqrtA * Math.Sin(ea);

                // Begin calc for True Anomaly and Argument of Latitude
                tempd2 = Math.Sqrt(1.0 - ecc * ecc);
                al = Math.Atan2(tempd2 * Math.Sin(ea), Math.Cos(ea) - ecc) + this.omg;
                // [rad] Argument of Latitude = True Anomaly + Argument of Perigee
                al_dot = tempd2 * ea_dot / tempd1;

                // Calculate corrected argument of latitude based on position
                cal = al + this.cus * Math.Sin(2.0 * al) + this.cuc * Math.Cos(2.0 * al);
                cal_dot =
                    al_dot * (1.0 +
                            2.0 * (this.cus * Math.Cos(2.0 * al) -
                                 this.cuc * Math.Sin(2.0 * al)));

                // Calculate corrected radius based on argument of latitude
                r =
                    a * tempd1 + this.crc * Math.Cos(2.0 * al) +
                    this.crs * Math.Sin(2.0 * al);
                r_dot =
                    a * ecc * Math.Sin(ea) * ea_dot +
                    2.0 * al_dot * (this.crs * Math.Cos(2.0 * al) -
                                this.crc * Math.Sin(2.0 * al));

                // Calculate inclination based on argument of latitude
                inc =
                    i0 + idot * tdiff +
                    this.cic * Math.Cos(2.0 * al) + this.cis * Math.Sin(2.0 * al);
                inc_dot =
                    idot + 2.0 * al_dot * (this.cis * Math.Cos(2.0 * al) -
                                               this.cic * Math.Sin(2.0 * al));

                // Calculate position and velocity in orbital plane
                x = r * Math.Cos(cal);
                y = r * Math.Sin(cal);
                x_dot = r_dot * Math.Cos(cal) - y * cal_dot;
                y_dot = r_dot * Math.Sin(cal) + x * cal_dot;

                // Corrected longitude of ascenting node
                om_dot = OMGd - NAV_OMEGAE_DOT;
                om = OMG0 + tdiff * om_dot - NAV_OMEGAE_DOT * toe.tow;

                // Compute the satellite's position in Earth-Centered Earth-Fixed coordiates
                pos[0] = x * Math.Cos(om) - y * Math.Cos(inc) * Math.Sin(om);
                pos[1] = x * Math.Sin(om) + y * Math.Cos(inc) * Math.Cos(om);
                pos[2] = y * Math.Sin(inc);

                tempd3 = y_dot * Math.Cos(inc) - y * Math.Sin(inc) * inc_dot;

                // Compute the satellite's velocity in Earth-Centered Earth-Fixed coordiates
                vel[0] = -om_dot * pos[1] + x_dot * Math.Cos(om) - tempd3 * Math.Sin(om);
                vel[1] = om_dot * pos[0] + x_dot * Math.Sin(om) + tempd3 * Math.Cos(om);
                vel[2] = y * Math.Cos(inc) * inc_dot + y_dot * Math.Sin(inc);

                clock_err += einstein;

                this.pos = pos;
                this.vel = vel;
                this.clock_err = clock_err;
                this.clock_rate_err = clock_rate_err;
                this.tot = tot;

                return 0;
            }
        }
    }
}