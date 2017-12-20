using System;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using int32_t = System.Int32;
using uint32_t = System.UInt32;
using int8_t = System.SByte;
using MissionPlanner.Comms;
using System.Runtime.InteropServices;

namespace MissionPlanner.Utilities
{
    public class ubx_m8p : ICorrections
    {
        int step = 0;

        public byte[] buffer = new byte[1024 * 8];
        int payloadlen = 0;
        int msglencount = 0;

        public byte @class { get { return buffer[2]; } }
        public byte subclass { get { return buffer[3]; } }

        public int length
        {
            get
            {
                return 2 + 2 + 2 + 2 + payloadlen; // header2, class,subclass,length2,data,crc2
            }
        }

        public byte[] packet
        {
            get
            {
                return buffer;
            }
        }

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
                    if (data == 0xb5)
                    {
                        step = 1;
                        buffer[0] = data;
                    }
                    break;
                case 1:
                    if (data == 0x62)
                    {
                        buffer[1] = data;
                        step++;
                    }
                    else
                        step = 0;
                    break;
                case 2:
                    buffer[2] = data;
                    step++;
                    break;
                case 3:
                    buffer[3] = data;
                    step++;
                    break;
                case 4:
                    buffer[4] = data;
                    payloadlen = data;
                    step++;
                    break;
                case 5:
                    buffer[5] = data;
                    step++;
                    payloadlen += (data << 8);
                    msglencount = 0;
                    // reset on oversize packet
                    if (payloadlen > buffer.Length)
                        step = 0;
                    break;
                case 6:
                    if (msglencount < (payloadlen))
                    {
                        buffer[msglencount + 6] = data;
                        msglencount++;

                        if (msglencount == payloadlen)
                            step++;
                    }
                    break;
                case 7:
                    buffer[msglencount + 6] = data;
                    step++;
                    break;
                case 8:
                    buffer[msglencount + 6 + 1] = data;

                    var crc = ubx_checksum(buffer, payloadlen + 6);

                    var crcpacket = new byte[] { buffer[msglencount + 6], data };

                    if (crc[0] == crcpacket[0] && crc[1] == crcpacket[1])
                    {
                        step = 0;
                        return (@class << 8) + subclass;
                    }
                    step = 0;
                    break;
            }

            return -1;
        }

        private static byte[] ubx_checksum(byte[] packet, int size, int offset = 2)
        {
            uint a = 0x00;
            uint b = 0x00;
            var i = offset;
            while (i < size)
            {
                a += packet[i++];
                b += a;
            }

            var ans = new byte[2];

            ans[0] = (byte)(a & 0xFF);
            ans[1] = (byte)(b & 0xFF);

            return ans;
        }

        public static byte[] generate(byte cl, byte subclass, byte[] payload)
        {
            var data = new byte[2 + 2 + 2 + 2 + payload.Length];
            data[0] = 0xb5;
            data[1] = 0x62;
            data[2] = cl;
            data[3] = subclass;
            data[4] = (byte)(payload.Length & 0xff);
            data[5] = (byte)((payload.Length >> 8) & 0xff);

            Array.ConstrainedCopy(payload, 0, data, 6, payload.Length);

            var checksum = ubx_checksum(data, data.Length - 2);

            data[data.Length - 2] = checksum[0];
            data[data.Length - 1] = checksum[1];

            return data;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ubx_mon_ver
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] swVersion;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public Byte[] hwVersion;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 30)]
            public byte[] extension;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ubx_mon_hw
        {
            public int pinSel;
            public int pinBank;
            public int pinDir;
            public int pinVal;
            public ushort noisePerMS;
            public ushort agcCnt;
            public byte aStatus;
            public byte aPower;
            public byte flags;
            public byte reserved1;
            public int usedMask;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public byte[] VP;
            public byte jamInd;
            public ushort reserved3;
            public int pinIrq;
            public int pullH;
            public int pullL;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ubx_nav_pvt
        {
            public uint32_t itow;
            public uint16_t year;
            public uint8_t month, day, hour, min, sec;
            public uint8_t valid;
            public uint32_t t_acc;
            public int32_t nano;
            public uint8_t fix_type;
            public uint8_t flags;
            public uint8_t flags2;
            public uint8_t num_sv;
            public int32_t lon, lat;
            public int32_t height, h_msl;
            public uint32_t h_acc, v_acc;
            public int32_t velN, velE, velD, gspeed;
            public int32_t head_mot;
            public uint32_t s_acc;
            public uint32_t head_acc;
            public uint16_t p_dop;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public uint8_t[] reserved1;
            public uint32_t headVeh;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public uint8_t[] reserved2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ubx_nav_svin
        {
            public uint8_t version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public uint8_t[] reserved1;
            public uint32_t iTOW;
            public uint32_t dur;
            public int32_t meanX;
            public int32_t meanY;
            public int32_t meanZ;
            public int8_t meanXHP;
            public int8_t meanYHP;
            public int8_t meanZHP;
            public uint8_t reserved2;
            public uint32_t meanAcc;
            public uint32_t obs;
            public uint8_t valid;
            public uint8_t active;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public uint8_t[] reserved3;

            public double[] getECEF()
            {
                var X = meanX / 100.0 + meanXHP * 0.0001;
                var Y = meanY / 100.0 + meanYHP * 0.0001;
                var Z = meanZ / 100.0 + meanZHP * 0.0001;

                return new double[] { X, Y, Z };
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ubx_nav_velned
        {
            public uint iTOW;
            public int velN;
            public int velE;
            public int velD;
            public uint speed;
            public uint gSpeed;
            public int heading;
            public uint sAcc;
            public uint cAcc;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
        public struct ubx_cfg_tmode3
        {
            public ubx_cfg_tmode3(double lat, double lng, double alt, double acc = 0.001)
            {
                version = 0;
                reserved1 = 0;
                if (Math.Abs(lat) > 90)
                {
                    flags = 2; // fixed mode ecef
                    ecefXorLat = (int)(lat*100);
                    ecefYorLon = (int)(lng * 100);
                    ecefZorAlt = (int)(alt * 100);
                    ecefXOrLatHP = (sbyte)((lat * 100 - ecefXorLat) * 100.0);
                    ecefYOrLonHP = (sbyte)((lng * 100 - ecefYorLon) * 100.0);
                    ecefZOrAltHP = (sbyte)((alt * 100 - ecefZorAlt) * 100.0);
                } else {
                    flags = 256 + 2; // lla + fixed mode
                    ecefXorLat = (int)(lat * 1e7);
                    ecefYorLon = (int)(lng * 1e7);
                    ecefZorAlt = (int)(alt * 100.0);
                    ecefXOrLatHP = (sbyte)((lat * 1e7 - ecefXorLat) * 100.0);
                    ecefYOrLonHP = (sbyte)((lng * 1e7 - ecefYorLon) * 100.0);
                    ecefZOrAltHP = (sbyte)((alt * 100.0 - ecefZorAlt) * 100.0);
                }
                reserved2 = 0;
                fixedPosAcc = (uint)(acc * 1000.0);
                svinMinDur = 60;
                svinAccLimit = 2000;
                reserved3 = new byte[8];
            }

            public ubx_cfg_tmode3(uint DurationS, double AccLimit)
            {
                version = 0;
                reserved1 = 0;
                flags = 1; // surveyin mode
                ecefXorLat = 0;
                ecefYorLon = 0;
                ecefZorAlt = 0;
                ecefXOrLatHP = (sbyte)0;
                ecefYOrLonHP = (sbyte)0;
                ecefZOrAltHP = (sbyte)0;
                reserved2 = 0;
                fixedPosAcc = 0;
                svinMinDur = DurationS;
                svinAccLimit = (uint)(AccLimit * 10000);
                reserved3 = new byte[8];
            }

            public static ubx_cfg_tmode3 Disable
            {
                get
                {
                    return new ubx_cfg_tmode3()
                    {
                        flags = 0, // disable
                        reserved3 = new byte[8]
                    };
                }
            }

            public static implicit operator byte[] (ubx_cfg_tmode3 input)
            {
                return MavlinkUtil.StructureToByteArray(input);
            }

            public enum modeflags
            {
                Disabled =0,
                SurveyIn=1,
                FixedECEF=2,
                LLA=256,
                FixedLLA=258
            }

            public byte version;
            public byte reserved1;
            public ushort flags;
            public int ecefXorLat; // 1e7
            public int ecefYorLon;
            public int ecefZorAlt;
            public sbyte ecefXOrLatHP;
            public sbyte ecefYOrLonHP;
            public sbyte ecefZOrAltHP;
            public byte reserved2;
            public uint fixedPosAcc;
            public uint svinMinDur;
            public uint svinAccLimit;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] reserved3;

            public PointLatLngAlt getPointLatLngAlt()
            {
                if (flags == 2)
                {
                    var X = ecefXorLat / 100.0 + ecefXOrLatHP * 0.0001;
                    var Y = ecefYorLon / 100.0 + ecefYOrLonHP * 0.0001;
                    var Z = ecefZorAlt / 100.0 + ecefZOrAltHP * 0.0001;

                    var pos = new double[] { X, Y, Z };

                    return new PointLatLngAlt(pos);
                }
                else if (flags == 258)
                {
                    var X = ecefXorLat / 1e7 + ecefXOrLatHP / 1e9;
                    var Y = ecefYorLon / 1e7 + ecefYOrLonHP / 1e9;
                    var Z = ecefZorAlt / 100.0 + ecefZOrAltHP * 0.0001;

                    var pos = new double[] { X, Y, Z };

                    return new PointLatLngAlt(pos);
                }

                return null;
            }
        }

        public void SetupM8P(ICommsSerial port, bool m8p_130plus = false, bool movingbase = false)
        {
            port.BaseStream.Flush();

            port.BaudRate = 9600;

            System.Threading.Thread.Sleep(100);

            // port config - 115200 - uart1
            var packet = generate(0x6, 0x00, new byte[] { 0x01, 0x00, 0x00, 0x00, 0xD0, 0x08, 0x00, 0x00, 0x00, 0xC2,
                0x01, 0x00, 0x23, 0x00, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00 });
            port.Write(packet, 0, packet.Length);
            System.Threading.Thread.Sleep(300);

            // port config - usb
            packet = generate(0x6, 0x00, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x23, 0x00, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00 });
            port.Write(packet, 0, packet.Length);
            System.Threading.Thread.Sleep(300);

            port.BaseStream.Flush();

            port.BaudRate = 115200;

            // port config - 115200 - uart1
            packet = generate(0x6, 0x00, new byte[] { 0x01, 0x00, 0x00, 0x00, 0xD0, 0x08, 0x00, 0x00, 0x00, 0xC2,
                0x01, 0x00, 0x23, 0x00, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00 });
            port.Write(packet, 0, packet.Length);
            System.Threading.Thread.Sleep(300);

            // port config - usb
            packet = generate(0x6, 0x00, new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x23, 0x00, 0x23, 0x00, 0x00, 0x00, 0x00, 0x00 });
            port.Write(packet, 0, packet.Length);
            System.Threading.Thread.Sleep(300);

            // set rate to 1hz
            packet = generate(0x6, 0x8, new byte[] { 0xE8, 0x03, 0x01, 0x00, 0x01, 0x00 });
            port.Write(packet, 0, packet.Length);
            System.Threading.Thread.Sleep(200);

            // set navmode to stationary
            if (!movingbase)
            {
                packet = generate(0x6, 0x24,
                    new byte[]
                    {
                        0xFF, 0xFF, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x10, 0x27, 0x00, 0x00, 0x05, 0x00, 0xFA, 0x00,
                        0xFA, 0x00, 0x64, 0x00, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x00, 0x10, 0x27, 0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00
                    });
                port.Write(packet, 0, packet.Length);
                System.Threading.Thread.Sleep(200);
            }

            // turn off all nmea
            for (int a = 0; a <= 0xf; a++)
            {
                if (a == 0xb || a == 0xc || a == 0xe)
                    continue;
                turnon_off(port, 0xf0, (byte)a, 0);
            }

            // mon-ver
            poll_msg(port, 0xa, 0x4);

            // surveyin msg - for feedback
            turnon_off(port, 0x01, 0x3b, 1);

            // pvt msg - for feedback
            turnon_off(port, 0x01, 0x07, 1);

            // 1005 - 5s
            turnon_off(port, 0xf5, 0x05, 5);

            byte rate1 = 1;
            byte rate2 = 0;

            if (m8p_130plus)
            {
                rate1 = 0;
                rate2 = 1;
            }

            // 1074 - 1s
            turnon_off(port, 0xf5, 0x4a, rate2);
            // 1077 - 1s
            turnon_off(port, 0xf5, 0x4d, rate1);

            // 1084 - 1s
            turnon_off(port, 0xf5, 0x54, rate2);
            // 1087 - 1s
            turnon_off(port, 0xf5, 0x57, rate1);

            // 1124 - 1s
            turnon_off(port, 0xf5, 0x7c, rate2);
            // 1127 - 1s
            turnon_off(port, 0xf5, 0x7f, rate1);

            if (movingbase)
            {
                // 4072
                turnon_off(port, 0xf5, 0xFE, 1);
            }
            else
            {
                // 4072
                turnon_off(port, 0xf5, 0xFE, 0);
            }

            // 1230 - 5s
            turnon_off(port, 0xf5, 0xE6, 5);

            // NAV-VELNED - 1s
            turnon_off(port, 0x01, 0x12, 1);

            // rxm-raw/rawx - 1s
            turnon_off(port, 0x02, 0x15, 1);
            turnon_off(port, 0x02, 0x10, 1);

            // rxm-sfrb/sfrb - 2s
            turnon_off(port, 0x02, 0x13, 2);
            turnon_off(port, 0x02, 0x11, 2);

            // mon-hw - 2s
            turnon_off(port, 0x0a, 0x09, 2);

            System.Threading.Thread.Sleep(100);
        }

        public void SetupBasePos(ICommsSerial port, PointLatLngAlt basepos, int surveyindur = 0, double surveyinacc = 0, bool disable = false, bool movingbase = false)
        {
            if (movingbase)
                disable = true;

            System.Threading.Thread.Sleep(100);
            System.Threading.Thread.Sleep(100);

            if (surveyindur == 0)
                surveyindur = 60;
            if (surveyinacc == 0)
                surveyinacc = 2;

            if (disable)
            {
                var packet = generate(0x6, 0x71, ubx_cfg_tmode3.Disable);
                port.Write(packet, 0, packet.Length);
                return;
            }

            if (basepos == PointLatLngAlt.Zero)
            {
                // survey in config
                var packet = generate(0x6, 0x71, new ubx_cfg_tmode3((uint)surveyindur, surveyinacc));
                port.Write(packet, 0, packet.Length);
            }
            else
            {
                byte[] data = new ubx_cfg_tmode3(basepos.Lat, basepos.Lng, basepos.Alt);
                var packet = generate(0x6, 0x71, data);
                port.Write(packet, 0, packet.Length);
            }

        }

        public void turnon_off(ICommsSerial port, byte clas, byte subclass, byte every_xsamples)
        {
            byte[] datastruct1 = { clas, subclass, 0, every_xsamples, 0, every_xsamples, 0, 0 };

            var packet = generate(0x6, 0x1, datastruct1);

            port.Write(packet, 0, packet.Length);

            System.Threading.Thread.Sleep(10);
        }

        public void poll_msg(ICommsSerial port, byte clas, byte subclass)
        {
            byte[] datastruct1 = { };

            var packet = generate(clas, subclass, datastruct1);

            port.Write(packet, 0, packet.Length);

            System.Threading.Thread.Sleep(10);
        }
    }
}