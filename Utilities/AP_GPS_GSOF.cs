using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uint32_t = System.UInt32;
using uint16_t = System.UInt16;
using uint8_t = System.Byte;
using int32_t = System.Int32;
using int8_t = System.SByte;
using System.Reflection;
using System.Runtime.InteropServices;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class AP_GPS_GSOF : AP_GPS_base
    {
        const uint8_t GSOF_STX = 0x02;
        const uint8_t GSOF_ETX = 0x03;

        public enum gsof_msg_parser_t
        {
            STARTTX = 0,
            STATUS,
            PACKETTYPE,
            LENGTH,
            DATA,
            CHECKSUM,
            ENDTX
        }

        struct gsof_msg_parser
        {
            public uint8_t starttx;
            public uint8_t status;
            public uint8_t packettype;
            public uint8_t length;
            public uint8_t[] data;
            public uint8_t checksum;
            public uint8_t endtx;

            public uint16_t read;
            public uint8_t checksumcalc;
        }

        Stream port = null;

        gsof_msg_parser_t gsof_state;

        gsof_msg_parser gsof_msg = new gsof_msg_parser();

        public AP_GPS_GSOF(string portname)
        {
            var sport = new TcpSerial();
            //new SerialPort(portname, 38400);

            sport.Open();

            port = sport.BaseStream;

            requestBaud();


            sport.BaudRate = 115200;

            requestGSOF();

            //requestPostion();

            gsof_msg.data = new byte[1024*20];

            read();
        }

        public void requestBaud()
        {
            byte[] buffer =
            {
                0x2, 0x0, 0x64, 13, 0, 0x0, 0x0,
                3, 0, 1, 0,
                0x2, 0x4, 0x0, 0x07, 0x0, 0x0
                , 0x0, 0x3
            };

            buffer[buffer.Length - 2] = (byte) (buffer.Sum(num => num) - 3 - 2); // 3 = etx 2 = stx

            port.Write(buffer, 0, buffer.Length);

            System.Threading.Thread.Sleep(100);
        }

        public void requestGSOF()
        {
            byte[] messages = {1, 2, 8, 9, 12};

            var st = File.OpenWrite("trim.dat");

            byte count = 1;
            foreach (var gsofmsg in messages)
            {
                byte[] buffer =
                {
                    0x2, 0x0, 0x64, 15, count, 0x0, 0x0,
                    3, 0, 1, 0,
                    0x7, 0x6, 10, 0x0, 0x1, 0, gsofmsg, 0
                    , 0x0, 0x3
                };

                buffer[buffer.Length - 2] = (byte) (buffer.Sum(num => num) - 3 - 2); // 3 = etx 2 = stx

                port.Write(buffer, 0, buffer.Length);

                st.Write(buffer, 0, buffer.Length);

                System.Threading.Thread.Sleep(100);

                count++;
            }

            st.Close();
        }

        public void requestPostion()
        {
            byte[] buffer =
            {
                0x2, 0x0, 0x64, 20, 0, 0x0, 0x0, //appfile
                3, 0, 1, 0, // file control information block
                0x7, 11, 4, 0x0, 0x2, 0, // output message record
                0xf, 0x1, 0, 0, 0, 0, 0xf0 // output message rt17/27
                , 0x0, 0x3
            }; // checksum and END

            buffer[buffer.Length - 2] = (byte) (buffer.Sum(num => num) - 3 - 2); // 3 = etx 2 = stx

            port.Write(buffer, 0, buffer.Length);
        }

        public bool read()
        {
            var st = File.OpenWrite("rt17.log");

            bool ret = false;
            //port->available()
            //(port.Length - port.Position) > 0
            while (true)
            {
                //port->read()
                var temp = port.ReadByte();
                ret |= parse((byte) temp);

                st.WriteByte((byte) temp);
            }

            //return ret;
        }

        private bool parse(uint8_t temp)
        {
            switch (gsof_state)
            {
                case gsof_msg_parser_t.STARTTX:
                    if (temp == GSOF_STX)
                    {
                        gsof_msg.starttx = temp;
                        gsof_state = gsof_msg_parser_t.STATUS;
                        gsof_msg.read = 0;
                        gsof_msg.checksumcalc = 0;
                    }

                    break;
                case gsof_msg_parser_t.STATUS:
                    gsof_msg.status = temp;
                    gsof_state = gsof_msg_parser_t.PACKETTYPE;
                    gsof_msg.checksumcalc += temp;
                    break;
                case gsof_msg_parser_t.PACKETTYPE:
                    gsof_msg.packettype = temp;
                    gsof_state = gsof_msg_parser_t.LENGTH;
                    gsof_msg.checksumcalc += temp;
                    break;
                case gsof_msg_parser_t.LENGTH:
                    gsof_msg.length = temp;
                    gsof_state = gsof_msg_parser_t.DATA;
                    gsof_msg.checksumcalc += temp;
                    break;
                case gsof_msg_parser_t.DATA:
                    gsof_msg.data[gsof_msg.read] = temp;
                    gsof_msg.read++;
                    gsof_msg.checksumcalc += temp;
                    if (gsof_msg.read >= gsof_msg.length)
                    {
                        gsof_state = gsof_msg_parser_t.CHECKSUM;
                    }
                    break;
                case gsof_msg_parser_t.CHECKSUM:
                    gsof_msg.checksum = temp;
                    gsof_state = gsof_msg_parser_t.ENDTX;
                    if (gsof_msg.checksum == gsof_msg.checksumcalc)
                    {
                        return process_message();
                    }
                    break;
                case gsof_msg_parser_t.ENDTX:
                    gsof_msg.endtx = temp;
                    gsof_state = gsof_msg_parser_t.STARTTX;
                    break;
            }

            return false;
        }

        private double SwapDouble(byte[] src, uint32_t pos)
        {
            uint8_t[] dst = new uint8_t[sizeof (double)];
            dst[0] = src[pos + 7];
            dst[1] = src[pos + 6];
            dst[2] = src[pos + 5];
            dst[3] = src[pos + 4];
            dst[4] = src[pos + 3];
            dst[5] = src[pos + 2];
            dst[6] = src[pos + 1];
            dst[7] = src[pos + 0];

            return BitConverter.ToDouble(dst, 0);
        }

        private float SwapFloat(byte[] src, uint32_t pos)
        {
            uint8_t[] dst = new uint8_t[sizeof (float)];
            dst[0] = src[pos + 3];
            dst[1] = src[pos + 2];
            dst[2] = src[pos + 1];
            dst[3] = src[pos + 0];

            return BitConverter.ToSingle(dst, 0);
        }


        private UInt32 SwapUint32(byte[] src, uint32_t pos)
        {
            uint8_t[] dst = new uint8_t[sizeof (UInt32)];
            dst[0] = src[pos + 3];
            dst[1] = src[pos + 2];
            dst[2] = src[pos + 1];
            dst[3] = src[pos + 0];

            return BitConverter.ToUInt32(dst, 0);
        }

        private UInt16 SwapUint16(byte[] src, uint32_t pos)
        {
            uint8_t[] dst = new uint8_t[sizeof (UInt16)];
            dst[0] = src[pos + 1];
            dst[1] = src[pos + 0];

            return BitConverter.ToUInt16(dst, 0);
        }

        private long byte2long(byte[] data)
        {
            long r = 0;

            unchecked
            {
                if ((data[0] & 0x80) != 0)
                {
                    r |= (long) 0xffff000000000000;
                    //   r |= (long)0xffffffffffffffff;
                }
            }

            r |= (long) data[0] << 40;
            r |= (long) data[1] << 32;
            r |= (long) data[2] << 24;
            r |= (long) data[3] << 16;
            r |= (long) data[4] << 8;
            r |= (long) data[5];

            return (long) r;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_6_27_epochHeader
        {
            public byte blockLength;
            public ushort gpsWeek;
            public UInt32 tmeasRCVR;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] clockOffset;
            public byte nSVs;
            public byte epochFlags;
            // if epochFlags & 2
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] gpsGlonassOffset;

            // if epochFlags & 16
            public byte raimInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_6_27_sysOffsets
        {
            public byte blockLength;
            public byte offsetHeader;
            //public byte referenceSystem; // offsetHeader & 15;
            //public byte numSystems; // offsetHeader >> 4 & 7
            // length = numSystems
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)] public byte[] satelliteSystem;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)] public double[] systemClockOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_6_27_measurement
        {
            public rt27_6_27_measurementHeader header;
            public rt27_6_27_measurementBlock block;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_6_27_measurementHeader
        {
            public byte blockLength;
            public byte prn;
            public byte svType;
            public byte svChannel;
            public byte nBlocks;
            public sbyte elevation;
            public byte azimuth;
            public byte svFlags;
            // if flags & 128 == 0 no more
            public byte svFlags2;
            // if flags2 & 128 == 0 no more

            public UInt32 pseudoiode;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_6_27_measurementBlock
        {
            public byte blockLength;
            public byte blockType;
            public byte trackType;
            public ushort snr;
            public UInt32 pseudorange;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] phase;
            public byte cycleSlipCount;
            public byte measurementFlags;
            public byte measurementFlags2;
            public byte measurementFlags3;
            public double doppler;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_2_19
        {
            public byte Source;
            public byte Port;
            public short Number;
            public double TOW;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_1_11
        {
            public double Latitude;
            public double Longitude;
            public double Altitude;
            public double ClockOffset;
            public double FreqOffset;
            public double PDOP;
            public double LatRate;
            public double LongRate;
            public double AltRate;
            public uint TOW;
            public byte PosFlag;
            public byte SVs;
            //public byte[] Channel = new byte[SVs];
            //public byte[] Prn;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct rt27_7_29
        {
            public uint8_t Length;
            public uint16_t Week;
            public uint32_t Time;
            public uint8_t Motion_state;
            public uint8_t TrackedSvs;
            public uint8_t SvSolution;
            public uint8_t Reserve;
            public uint8_t Pos_Sys_Flag;
            public uint8_t Pos_Sol_Flag;
            public uint8_t Pos_Aug_Type;
            public uint8_t Pos_Proc_Type;
            // Pos Block
            public uint8_t PosLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public uint8_t[] Lat;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)] public uint8_t[] Lng;
            public int32_t Alt;
            public int32_t veln;
            public int32_t vele;
            public int32_t velu;
            public int32_t rx_clock_offset;
            public int32_t rx_clock_drift;
            public uint32_t hdop;
            public uint32_t vdop;
            public uint32_t tdop;
            public uint16_t sigman;
            public uint16_t sigmae;
            public uint16_t sigmau;
            public uint16_t rms;
            public uint16_t std_dev;
        }

        byte[] obsdata = new byte[20*1024];

        private bool process_message()
        {
            Console.WriteLine("packet: " + gsof_msg.packettype.ToString("X") + " len: " + gsof_msg.length + " status: " +
                              gsof_msg.status);

            if (gsof_msg.packettype == 0x57) // RAWDATA 
            {
                uint8_t record_type = gsof_msg.data[0];
                uint8_t page_total = (byte)(gsof_msg.data[1] & 0xf);
                uint8_t page_number = (byte)((gsof_msg.data[1] >> 4) & 0xf);
                uint8_t reply_number =(byte) gsof_msg.data[2];
                uint8_t recordintflags = gsof_msg.data[3];

                bool consise = (recordintflags & 0x1) > 0;

                Console.WriteLine(DateTime.Now.Second + " record type: " + record_type + " page_no " + page_number + "/" + page_total + " consise " + consise);

                if (record_type == 7)
                {
                    double p40 = Math.Pow(2, 40);
                    double p39 = Math.Pow(2, 39);
                    double p11 = Math.Pow(2, 11);
                    double p12 = Math.Pow(2, 12);
                    double p14 = Math.Pow(2, 14);
                    double p21 = Math.Pow(2, 21);
                    double p26 = Math.Pow(2, 26);
                    double p17 = Math.Pow(2, 17);
                    double p4 = Math.Pow(2, 4);

                    var test2 = (rt27_7_29) gsof_msg.data.ByteArrayToStructureBigEndian<rt27_7_29>(4);

                    long lat = byte2long(test2.Lat);
                    long lng = byte2long(test2.Lng);

                    Console.WriteLine("{0} {1} {2} {3} {4} {5} ", lat/p40, lng/p39, test2.Alt/p12,
                        (test2.veln/p21), (test2.vele/p21), (test2.velu/p21),
                        test2.rx_clock_offset/p26, test2.rx_clock_drift/p17, test2.hdop/p4);
                }
                else if (record_type == 1)
                {
                    var test2 = (rt27_1_11)gsof_msg.data.ByteArrayToStructureBigEndian<rt27_1_11>(4);

                    Console.WriteLine("{0} {1} {2}", ToDeg(test2.Latitude*Math.PI), ToDeg(test2.Longitude*Math.PI), test2.Altitude);
                }
                else if (record_type == 2)
                {
                    var test2 = (rt27_2_19)gsof_msg.data.ByteArrayToStructureBigEndian<rt27_2_19>(4);

                    Console.WriteLine("Event {0} {1} {2} {3}", test2.Source, test2.Port, test2.Number, test2.TOW);
                }
                else if (record_type == 6) // there is no documentation on this
                {
                    if (page_number == 1)
                    {
                        obsdata.Initialize();
                        Array.ConstrainedCopy(gsof_msg.data, 0, obsdata, 0, gsof_msg.length);
                    }
                    else
                    {
                        Array.ConstrainedCopy(gsof_msg.data, 4, obsdata, 248 + (page_number - 2) * 244, gsof_msg.length - 4);
                    }
                    

                    if (page_number == page_total)
                    {
                        var m_epochHeader =
                            (rt27_6_27_epochHeader)
                                obsdata.ByteArrayToStructureBigEndian<rt27_6_27_epochHeader>(4);

                        var head_len = m_epochHeader.blockLength;

                        var m_sysOffsets = new rt27_6_27_sysOffsets();
                        var obj = m_sysOffsets as object;
                        MavlinkUtil.ByteArrayToStructureEndian(obsdata, ref obj, (4 + head_len));
                        m_sysOffsets = (rt27_6_27_sysOffsets) obj;

                        var sysoffset_lemn = m_sysOffsets.blockLength;

                        var m_measurement = new List<rt27_6_27_measurement>();
                        var SVs = m_epochHeader.nSVs;

                        var offset = 4 + head_len + sysoffset_lemn;

                        for (int index = 0; index < SVs; ++index)
                        {
                            var measheader = new rt27_6_27_measurementHeader();
                            obj = measheader as object;
                            MavlinkUtil.ByteArrayToStructureEndian(obsdata, ref obj, offset);
                            measheader = (rt27_6_27_measurementHeader)obj;
                            var meashead_len = measheader.blockLength;
                            offset += meashead_len;

                            string type = "";
                            switch (measheader.svType)
                            {
                                case 0:
                                    type = "Gps";
                                    break;
                                case 1:
                                    type = "Sbas";
                                    break;
                                case 2:
                                    type = "Glonass";
                                    break;
                                case 3:
                                    type = "Galileo";
                                    break;
                                case 4:
                                    type = "QZSS";
                                    break;
                                case 7:
                                    type = "beidou";
                                    break;
                            }

                            Console.WriteLine("prn {0} type {1} el {2} az {3} blocks {4}", measheader.prn, type, (double)(sbyte)measheader.elevation, (double)measheader.azimuth / Math.Pow(2.0, -1.0), measheader.nBlocks);

                            for (int a = 0; a < measheader.nBlocks;a++)
                            {
                                var measblock = new rt27_6_27_measurementBlock();
                                obj = measblock as object;
                                MavlinkUtil.ByteArrayToStructureEndian(obsdata, ref obj, offset);
                                measblock = (rt27_6_27_measurementBlock)obj;

                                if (a == 0)
                                {
                                    Console.WriteLine("{0} {1}", measblock.pseudorange / Math.Pow(2.0, 7.0), byte2long(measblock.phase) / Math.Pow(2.0, 15.0));
                                }
                                else
                                {
                                }

                                var measblock_len = measblock.blockLength;
                                offset += measblock_len;

                                m_measurement.Add(new rt27_6_27_measurement() { block = measblock, header = measheader });
                            }
                        }
                    }
                }
            }

            if (gsof_msg.packettype == 0x40) // GSOF
            {
                uint8_t trans_number = gsof_msg.data[0];
                uint8_t pageidx = gsof_msg.data[1];
                uint8_t maxpageidx = gsof_msg.data[2];

                //http://www.trimble.com/OEM_ReceiverHelp/V4.81/en/default.html
                //http://www.trimble.com/EC_ReceiverHelp/V4.19/en/GSOFmessages_Overview.htm
                Console.WriteLine("GSOF page: " + pageidx + " of " + maxpageidx);

                // want 2 8 9 38

                for (uint a = 3; a < gsof_msg.length; a++)
                {
                    uint8_t output_type = gsof_msg.data[a];
                    a++;
                    uint8_t output_length = gsof_msg.data[a];
                    a++;
                    Console.WriteLine("GSOF type: " + output_type + " len: " + output_length);

                    if (output_type == 2) // position
                    {
                        state.location.lat = (int32_t) (ToDeg(SwapDouble(gsof_msg.data, a))*1e7);
                        state.location.lng = (int32_t) (ToDeg(SwapDouble(gsof_msg.data, a + 8))*1e7);
                        state.location.alt = (int32_t) (SwapDouble(gsof_msg.data, a + 16)*1e2);

                        state.last_gps_time_ms = state.time_week_ms;
                    }
                    else if (output_type == 8) // velocity
                    {
                        uint8_t vflag = gsof_msg.data[a];
                        if ((vflag & 1) == 1)
                        {
                            state.ground_speed = SwapFloat(gsof_msg.data, a + 1);
                            state.ground_course = (float)(ToDeg(SwapFloat(gsof_msg.data, a + 5)));
                            fill_3d_velocity();
                            state.velocity.z = -SwapFloat(gsof_msg.data, a + 9);
                            state.have_vertical_velocity = true;
                        }
                    }
                    else if (output_type == 9) //dop
                    {
                        state.hdop = (uint16_t) (SwapFloat(gsof_msg.data, a + 4)*100);
                    }
                    else if (output_type == 12) // position sigma
                    {
                        state.horizontal_accuracy = (SwapFloat(gsof_msg.data, a + 4) + SwapFloat(gsof_msg.data, a + 8))/
                                                    2;
                        state.vertical_accuracy = SwapFloat(gsof_msg.data, a + 16);
                        state.have_horizontal_accuracy = true;
                        state.have_vertical_accuracy = true;
                    }
                    else if (output_type == 1) // pos time
                    {
                        state.time_week_ms = SwapUint32(gsof_msg.data, a);
                        state.time_week = SwapUint16(gsof_msg.data, a + 4);
                        state.num_sats = gsof_msg.data[a + 6];
                        uint8_t posf1 = gsof_msg.data[a + 7];
                        uint8_t posf2 = gsof_msg.data[a + 8];

                        Console.WriteLine("POSTIME: " + posf1 + " " + posf2);

                        if ((posf1 & 1) == 1)
                        {
                            state.status = AP_GPS.GPS_OK_FIX_3D;
                            if ((posf2 & 1) == 1)
                            {
                                state.status = AP_GPS.GPS_OK_FIX_3D_DGPS;
                                if ((posf2 & 4) == 4)
                                {
                                    state.status = AP_GPS.GPS_OK_FIX_3D_RTK;
                                }
                            }
                        }
                        else
                        {
                            state.status = AP_GPS.NO_FIX;
                        }
                    }

                    a += output_length - 1u;
                }


                Type t = state.GetType(); //where obj is object whose properties you need.
                FieldInfo[] pi = t.GetFields();
                foreach (var p in pi)
                {
                    System.Console.WriteLine(p.Name + "    " + p.GetValue(state).ToString());
                }

                return true;
            }

            return false;
        }
    }
}