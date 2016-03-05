using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using uint32_t = System.UInt32;
using uint16_t = System.UInt16;
using uint8_t = System.Byte;
using int32_t = System.Int32;
using int8_t = System.SByte;
using System.Runtime.InteropServices;
using System.ComponentModel;
using netDxf;
using System.IO.Ports;
using System.Reflection;

namespace MissionPlanner.Utilities
{
    class AP_GPS_SBF
    {
        const uint8_t SBF_PREAMBLE1 = (byte) '$';
        const uint8_t SBF_PREAMBLE2 = (byte) '@';

        public enum readstate
        {
            PREAMBLE1 = 0,
            PREAMBLE2,
            CRC1,
            CRC2,
            BLOCKID1,
            BLOCKID2,
            LENGTH1,
            LENGTH2,
            DATA
        }

        struct sbf_msg_parser
        {
            public uint16_t preamble;
            public uint16_t crc;
            public uint16_t blockid;
            public uint16_t length;
            public uint8_t[] data;
            public uint16_t read;
        }


        readstate sbf_state;
        ushort last_hdop;
        uint32_t crc_error_counter;
        bool validcommand = false;

        GPS_State state = new GPS_State();

        sbf_msg_parser sbf_msg = new sbf_msg_parser();

        Stream port = null;//File.Open(@"T:\rtk4\SEPTX.15_", FileMode.Open);

        public AP_GPS_SBF()
        {
            crc_error_counter = 0;
            last_hdop = 999;
            sbf_state = readstate.PREAMBLE1;

            var sport = new SerialPort("com10", 115200);

            sport.Open();

            port = sport.BaseStream;

            // setcommsettings
            //string command1 = "scs, COM1, baud115200\n";
            // setdatainout
            //string command2 = "sdio, COM1, auto, SBF\n";
            // copy current config to boot config - wont use
            //eccf, Current, Boot <CR>

            //setreceiverdynamics
            //srd, High, UAV

            // setelevationmask
            //sem, PVT, 5

            // setSBFOutput
            validcommand = false;
            string command2 = "sso, Stream1, USB1, PVTGeodetic+DOP+ExtEventPVTGeodetic, msec100\n";
            port.Write(ASCIIEncoding.ASCII.GetBytes(command2), 0, command2.Length);

            System.Threading.Thread.Sleep(70);

            //string command3 = "sso, Stream2, USB1, PVTGeodetic+DOP+ExtEventPVTGeodetic, msec100\n";
            //port.Write(ASCIIEncoding.ASCII.GetBytes(command3),0,command3.Length);

            //System.Threading.Thread.Sleep(70);

            //command3 = "sso, Stream3, USB2, PVTGeodetic+DOP+ExtEventPVTGeodetic, msec100\n";
            //port.Write(ASCIIEncoding.ASCII.GetBytes(command3), 0, command3.Length);

            //System.Threading.Thread.Sleep(70);

            string command4 = "srd, High, UAV\n";
            //port.Write(ASCIIEncoding.ASCII.GetBytes(command4), 0, command4.Length);

            System.Threading.Thread.Sleep(70);

            string command5 = "sem, PVT, 5\n";
            //port.Write(ASCIIEncoding.ASCII.GetBytes(command5), 0, command5.Length);

            System.Threading.Thread.Sleep(70);

            // enable sbas "+SBAS"
            string command6 = "spm, Rover, StandAlone+DGPS+RTK\n";
            //port.Write(ASCIIEncoding.ASCII.GetBytes(command6), 0, command6.Length);

            System.Threading.Thread.Sleep(70);
            int btr = sport.BytesToRead;
            byte[] data = new byte[btr];
            //port.Read(data, 0, btr);

            //Console.WriteLine(ASCIIEncoding.ASCII.GetString(data));

            //System.Threading.Thread.Sleep(100);

            sbf_msg.data = new byte[1024*20];
            read();
        }

        public bool read()
        {
            var st = File.OpenWrite("sept.log");

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
            switch (sbf_state)
            {
                case readstate.PREAMBLE1:
                    if (temp == SBF_PREAMBLE1)
                        sbf_state++;
                    else if (temp == '$')
                        sbf_state++;
                    else
                        Console.Write(".");
                    sbf_msg.read = 0;
                    break;
                case readstate.PREAMBLE2:
                    if (temp == SBF_PREAMBLE2)
                    {
                        sbf_state++;
                    }
                    else if (temp == 'R')
                    {
                        validcommand = true;
                    }
                    else
                    {
                        Console.WriteLine("Bad Sync " + temp);
                        sbf_state = readstate.PREAMBLE1;
                    }
                    break;
                case readstate.CRC1:
                    sbf_msg.crc = temp;
                    sbf_state++;
                    break;
                case readstate.CRC2:
                    sbf_msg.crc += (uint16_t) (temp << 8);
                    sbf_state++;
                    break;
                case readstate.BLOCKID1:
                    sbf_msg.blockid = temp;
                    sbf_state++;
                    break;
                case readstate.BLOCKID2:
                    sbf_msg.blockid += (uint16_t) (temp << 8);
                    sbf_state++;
                    break;
                case readstate.LENGTH1:
                    sbf_msg.length = temp;
                    sbf_state++;
                    break;
                case readstate.LENGTH2:
                    sbf_msg.length += (uint16_t) (temp << 8);
                    sbf_state++;
                    sbf_msg.data = new uint8_t[sbf_msg.length];

                    //Console.WriteLine((sbf_msg.blockid & 4095u) + " len " + sbf_msg.length);

                    if (sbf_msg.length%4 != 0)
                        sbf_state = readstate.PREAMBLE1;
                    break;
                case readstate.DATA:
                    sbf_msg.data[sbf_msg.read] = temp;
                    sbf_msg.read++;
                    if (sbf_msg.read >= (sbf_msg.length - 8))
                    {
                        uint16_t crc = crc16.ccitt(sbf_msg.blockid, 2, 0);
                        crc = crc16.ccitt(sbf_msg.length, 2, crc);
                        crc = crc16.ccitt(sbf_msg.data, sbf_msg.length - 8, crc);

                        sbf_state = readstate.PREAMBLE1;

                        if (sbf_msg.crc == crc)
                        {
                            return process_message();
                        }
                        else
                        {
                            crc_error_counter++;
                        }
                    }
                    break;
            }

            return false;
        }

        const double CLIGHT = 299792458.0; /* speed of light (m/s) */

        static double[] lam = new double[]
        {
            /* carrier wave length (m) */
            CLIGHT/FREQ1, CLIGHT/FREQ2, CLIGHT/FREQ5, CLIGHT/FREQ6, CLIGHT/FREQ7, CLIGHT/FREQ8
        };

        const double FREQ1 = 1.57542E9; /* L1/E1  frequency (Hz) */
        const double FREQ2 = 1.22760E9; /* L2     frequency (Hz) */
        const double FREQ5 = 1.17645E9; /* L5/E5a frequency (Hz) */
        const double FREQ6 = 1.27875E9; /* E6/LEX frequency (Hz) */
        const double FREQ7 = 1.20714E9; /* E5b    frequency (Hz) */
        const double FREQ8 = 1.191795E9; /* E5a+b  frequency (Hz) */

        private bool process_message()
        {
            uint32_t blockid = (sbf_msg.blockid & 4095u);

            if (blockid == 4027) // obs
            {
                Console.WriteLine("Obs");
                var pos = 0;

                var m4027 = (msg4027) sbf_msg.data.ByteArrayToStructure<msg4027>(pos);

                var sizemsg4027 = Marshal.SizeOf(m4027);

                pos += sizemsg4027;

                int sizemsgmeas1 = m4027.SB1Length;
                int sizemsgmeas2 = m4027.SB2Length;

                for (int a = 0; a < m4027.N1; a++)
                {
                    var meas1 = (MeasEpochChannelType1) sbf_msg.data.ByteArrayToStructure<MeasEpochChannelType1>(pos);

                    pos += sizemsgmeas1;

                    double code = ((meas1.Misc & 15)*4294967296 + meas1.CodeLSB)*0.001;
                    double doppler = meas1.Doppler*0.0001;
                    double carrier = code/lam[0] + (meas1.CarrierMSB*65536 + meas1.CarrierLSB)*0.001;
                    double snr = meas1.CN0*0.25 + 10;
                    type type1 = (type) (meas1.Type & 31);

                    Console.WriteLine("SV " + meas1.SVID + " " + type1 + " " + snr + " " + code + " " + carrier + " " +
                                      doppler);

                    for (int b = 0; b < meas1.N2; b++)
                    {
                        var meas2 =
                            (MeasEpochChannelType2) sbf_msg.data.ByteArrayToStructure<MeasEpochChannelType2>(pos);

                        pos += sizemsgmeas2;

                        // need to fix carrier base

                        int32_t CodeOffsetMSB
                            = ExtendSignBit(meas2.OffsetMSB, 3);
                        int32_t DopplerOffsetMSB
                            = ExtendSignBit(meas2.OffsetMSB >> 3, 5);

                        double cno = meas2.CN0*0.25 + 10;
                        type type2 = (type) (meas2.Type & 31);
                        double code2 = code + (CodeOffsetMSB*65536 + meas2.CodeOffsetLSB)*0.001;
                        double carrier2 = code2/lam[1] + (meas2.CarrierMSB*65536 + meas2.CarrierLSB)*0.001;
                        double doppler2 = meas1.Doppler*(lam[0]/lam[1]) +
                                          (DopplerOffsetMSB*65536 + meas2.DopplerOffsetLSB)*0.0001;

                        Console.WriteLine("SV " + meas1.SVID + " " + type2 + " " + cno + " " + code2 + " " + carrier2 +
                                          " " + doppler2);
                    }
                }
            }
            // ExtEventPVTGeodetic
            if (blockid == 4038)
            {
                var temp = (msg4038) sbf_msg.data.ByteArrayToStructure<msg4038>(0);
            }
            // PVTGeodetic
            if (blockid == 4007) // geo position
            {
                var temp = (msg4007) sbf_msg.data.ByteArrayToStructure<msg4007>(0);

                Console.WriteLine("pos " + temp.WNc + " " + (temp.TOW*0.001));

                // Update time state
                if (temp.WNc != 65535)
                {
                    state.time_week = temp.WNc;
                    state.time_week_ms = (uint32_t) (temp.TOW);
                }

                state.hdop = last_hdop;

                // Update velocity state (dont use −2·10^10)
                if (temp.Vn > -20000000000)
                {
                    state.velocity[0] = (float) (temp.Vn/1000.0);
                    state.velocity[1] = (float) (temp.Ve/1000.0);
                    state.velocity[2] = (float) (-temp.Vu/1000.0);

                    state.have_vertical_velocity = true;

                    float ground_vector_sq = state.velocity[0]*state.velocity[0] + state.velocity[1]*state.velocity[1];
                    state.ground_speed = (float) safe_sqrt(ground_vector_sq);

                    state.ground_course_cd = (int32_t) (100*ToDeg(atan2f(state.velocity[1], state.velocity[0])));
                    if (state.ground_course_cd < 0)
                    {
                        state.ground_course_cd += 36000;
                    }

                    state.horizontal_accuracy = (float) temp.HAccuracy*0.01f;
                    state.vertical_accuracy = (float) temp.VAccuracy*0.01f;
                    state.have_horizontal_accuracy = true;
                    state.have_vertical_accuracy = true;
                }

                // Update position state (dont use −2·10^10)
                if (temp.Latitude > -20000000000)
                {
                    state.location.lat = (int32_t) (ToDeg(temp.Latitude)*1e7);
                    state.location.lng = (int32_t) (ToDeg(temp.Longitude)*1e7);
                    state.location.alt = (int32_t) (temp.Height*1e2);
                }

                if (temp.NrSV != 255)
                {
                    state.num_sats = temp.NrSV;
                }

                switch (temp.Mode & 15)
                {
                    case 0:
                        state.status = GPS_Status.NO_FIX;
                        break;
                    case 1:
                        state.status = GPS_Status.GPS_OK_FIX_3D;
                        break;
                    case 2:
                        state.status = GPS_Status.GPS_OK_FIX_3D_DGPS;
                        break;
                    case 3:
                        state.status = GPS_Status.GPS_OK_FIX_3D;
                        break;
                    case 4:
                        state.status = GPS_Status.GPS_OK_FIX_3D_RTK;
                        break;
                    case 5:
                        state.status = GPS_Status.GPS_OK_FIX_3D_DGPS;
                        break;
                    case 6:
                        state.status = GPS_Status.GPS_OK_FIX_3D;
                        break;
                    case 7:
                        state.status = GPS_Status.GPS_OK_FIX_3D_RTK;
                        break;
                    case 8:
                        state.status = GPS_Status.GPS_OK_FIX_3D_DGPS;
                        break;
                }

                if ((temp.Mode & 64) > 0) // gps is in base mode
                    state.status = GPS_Status.NO_FIX;
                if ((temp.Mode & 128) > 0) // gps only has 2d fix
                    state.status = GPS_Status.GPS_OK_FIX_2D;

                Type t = state.GetType(); //where obj is object whose properties you need.
                FieldInfo[] pi = t.GetFields();
                foreach (var p in pi)
                {
                    System.Console.WriteLine(p.Name + "    " + p.GetValue(state).ToString());
                }

                return true;
            }
            if (blockid == 4001) // dops
            {
                var temp = (msg4001) sbf_msg.data.ByteArrayToStructure<msg4001>(0);

                Console.WriteLine("dop " + temp.WNc + " " + (temp.TOW*0.001) + " " + temp.HDOP);

                last_hdop = temp.HDOP;
            }

            return false;
        }

        enum type
        {
            GPS_L1CA = 0,
            GPS_L1PY,
            GPS_L2PY,
            GPS_L2C,
            GPS_L5,

            QZSS_L1CA = 6,
            QZSS_L2C,
            GLO_L1CA,

            GLO_L2P = 10,
            GLO_L2CA,
            GLO_L3,

            GAL_L1BC = 17,

            GAL_E6BC = 19,
            GAL_E5a,
            GAL_E5b,
            GAL_E5,

            GEO_L1CA = 24,
            GEO_L5,
            QZSS_L5,

            CMP_L1 = 28,
            CMP_E5b,
            CMP_B3
        }

        int32_t ExtendSignBit(int32_t x, int32_t N)
            /* extend the sign bit of a 2's complement N-bit integer
         *
         * Arguments:
         *   x:  the N-bit integer in two's complement
         *
         *   N:  the size of the integer in bits
        */
        {
            if ((x & (1 << (N - 1))) == 0)
            {
                /* the sign bit is 0, clear all the bits that are not part of a
                   N-bit integer */
                return x & ((1 << N) - 1);
            }
            else
            {
                /* the sign bit is 1, set all the bits that are not part of a
                   N-bit integer */
                return x | (~((1 << N) - 1));
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct msg4027
        {
            public uint32_t TOW;
            public uint16_t WNc;
            public uint8_t N1;
            public uint8_t SB1Length;
            public uint8_t SB2Length;
            public uint8_t CommonFlags;
            public uint8_t CumClkJumps;
            public uint8_t Reserved;
            //MeasEpochChannelType1[] Type1;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MeasEpochChannelType1
        {
            public uint8_t RxChannel;
            public uint8_t Type;
            public uint8_t SVID;
            public uint8_t Misc;
            public uint32_t CodeLSB;
            public int32_t Doppler;
            public uint16_t CarrierLSB;
            public int8_t CarrierMSB;
            public uint8_t CN0;
            public uint16_t LockTime;
            public uint8_t ObsInfo;
            public uint8_t N2;

            //MeasEpochChannelType2[] Type2;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MeasEpochChannelType2
        {
            public uint8_t Type;
            public uint8_t LockTime;
            public uint8_t CN0;
            public uint8_t OffsetMSB;
            public int8_t CarrierMSB;
            public uint8_t ObsInfo;
            public uint16_t CodeOffsetLSB;
            public uint16_t CarrierLSB;
            public uint16_t DopplerOffsetLSB;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct msg4007
        {
            public uint32_t TOW;
            public uint16_t WNc;
            public uint8_t Mode;
            public uint8_t Error;
            public double Latitude;
            public double Longitude;
            public double Height;
            public float Undulation;
            public float Vn;
            public float Ve;
            public float Vu;
            public float COG;
            public double RxClkBias;
            public float RxClkDrift;
            public uint8_t TimeSystem;
            public uint8_t Datum;
            public uint8_t NrSV;
            public uint8_t WACorrInfo;
            public uint16_t ReferenceID;
            public uint16_t MeanCorrAge;
            public uint32_t SignalInfo;
            public uint8_t AlertFlag;
            // rev1
            public uint8_t NrBases;
            public uint16_t PPPInfo;
            // rev2
            public uint16_t Latency;
            public uint16_t HAccuracy;
            public uint16_t VAccuracy;
            public uint8_t Misc;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct msg4038
        {
            public uint32_t TOW;
            public uint16_t WNc;
            public uint8_t Mode;
            public uint8_t Error;
            public double Latitude;
            public double Longitude;
            public double Height;
            public float Undulation;
            public float Vn;
            public float Ve;
            public float Vu;
            public float COG;
            public double RxClkBias;
            public float RxClkDrift;
            public uint8_t TimeSystem;
            public uint8_t Datum;
            public uint8_t NrSV;
            public uint8_t WACorrInfo;
            public uint16_t ReferenceID;
            public uint16_t MeanCorrAge;
            public uint32_t SignalInfo;
            public uint8_t AlertFlag;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct msg4001
        {
            public uint32_t TOW;
            public uint16_t WNc;
            public uint8_t NrSV;
            public uint8_t Reserved;
            public uint16_t PDOP;
            public uint16_t TDOP;
            public uint16_t HDOP;
            public uint16_t VDOP;
            public float HPL;
            public float VPL;
        }

        private double safe_sqrt(double ground_vector_sq)
        {
            return Math.Sqrt(ground_vector_sq);
        }

        private double ToDeg(double p)
        {
            return p*(180/Math.PI);
        }

        private double atan2f(double p1, double p2)
        {
            return Math.Atan2(p1, p2);
        }

        public struct Location
        {
            public uint8_t options;

            /// allows writing all flags to eeprom as one byte

            // by making alt 24 bit we can make p1 in a command 16 bit,
            // allowing an accurate angle in centi-degrees. This keeps the
            // storage cost per mission item at 15 bytes, and allows mission
            // altitudes of up to +/- 83km
            public int32_t alt;

            ///< param 2 - Altitude in centimeters (meters * 100)
            public int32_t lat;

            ///< param 3 - Lattitude * 10**7
            public int32_t lng;

            ///< param 4 - Longitude * 10**7
            ///
            public override string ToString()
            {
                return lat + ", " + lng + ", " + alt;
            }
        };

        public enum GPS_Status
        {
            NO_GPS = 0,

            ///< No GPS connected/detected
            NO_FIX = 1,

            ///< Receiving valid GPS messages but no lock
            GPS_OK_FIX_2D = 2,

            ///< Receiving valid messages and 2D lock
            GPS_OK_FIX_3D = 3,

            ///< Receiving valid messages and 3D lock
            GPS_OK_FIX_3D_DGPS = 4,

            ///< Receiving valid messages and 3D lock with differential improvements
            GPS_OK_FIX_3D_RTK = 5, ///< Receiving valid messages and 3D lock, with relative-positioning improvements
        };

        public struct GPS_State
        {
            public uint8_t instance; // the instance number of this GPS

            // all the following fields must all be filled by the backend driver
            public GPS_Status status;

            ///< driver fix status
            public uint32_t time_week_ms;

            ///< GPS time (milliseconds from start of GPS week)
            public uint16_t time_week;

            ///< GPS week number
            public Location location;

            ///< last fix location
            public float ground_speed;

            ///< ground speed in m/sec
            public int32_t ground_course_cd;

            ///< ground course in 100ths of a degree
            public uint16_t hdop;

            ///< horizontal dilution of precision in cm
            public uint8_t num_sats;

            ///< Number of visible satelites        
            public Vector3f velocity;

            ///< 3D velocitiy in m/s, in NED format
            public float speed_accuracy;

            public float horizontal_accuracy;
            public float vertical_accuracy;
            public bool have_vertical_velocity; //:1;      ///< does this GPS give vertical velocity?
            public bool have_speed_accuracy; //:1;
            public bool have_horizontal_accuracy; //:1;
            public bool have_vertical_accuracy; //:1;
            public uint32_t last_gps_time_ms; ///< the system time we got the last GPS timestamp, milliseconds
        }


        public class crc16
        {
            const ushort poly = 4129;
            static ushort[] table;

            public static ushort ccitt(uint16_t indata, int count, ushort crc)
            {
                byte[] data = BitConverter.GetBytes(indata);
                data.Reverse();
                return ccitt(data, count, crc);
            }

            public static ushort ccitt(byte[] indata, int count, ushort crc)
            {
                int total = count;

                while (count > 0)
                {
                    byte data = indata[total - count];

                    crc = (ushort) ((crc << 8) ^ table[((crc >> 8) ^ (0xff & data))]);

                    count--;
                }

                return crc;
            }

            static crc16()
            {
                if (table == null)
                {
                    table = new ushort[256];
                    ushort temp, a;
                    for (int i = 0; i < table.Length; i++)
                    {
                        temp = 0;
                        a = (ushort) (i << 8);
                        for (int j = 0; j < 8; j++)
                        {
                            if (((temp ^ a) & 0x8000) != 0)
                            {
                                temp = (ushort) ((temp << 1) ^ poly);
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