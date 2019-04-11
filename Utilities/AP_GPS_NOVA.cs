using System;
using System.IO;
using System.Text;
using uint32_t = System.UInt32;
using uint16_t = System.UInt16;
using uint8_t = System.Byte;
using int32_t = System.Int32;
using int8_t = System.SByte;
using System.Runtime.InteropServices;
using System.Reflection;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    class AP_GPS_NOVA : AP_GPS_base
    {
        const uint8_t NOVA_PREAMBLE1 = (byte) 0xaa;
        const uint8_t NOVA_PREAMBLE2 = (byte) 0x44;
        const uint8_t NOVA_PREAMBLE3 = (byte) 0x12; // oem4 = 0x12 (oem 3 = 0x11)

           // do we have new position information?
    bool            _new_position;
    // do we have new speed information?
    bool            _new_speed;
    
    uint32_t        _last_vel_time;

        public enum readstate
        {
            PREAMBLE1 = 0,
            PREAMBLE2,
            PREAMBLE3,
            HEADERLENGTH,
            HEADERDATA,
            DATA,
            CRC1,
            CRC2,
            CRC3,
            CRC4,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
        struct nova_header
        {
            // 0
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] 
            public byte[] preamble;
            // 3
            public uint8_t headerlength;
            // 4
            public uint16_t messageid;
            // 6
            public uint8_t messagetype;
            //7
            public uint8_t portaddr;
            //8
            public uint16_t messagelength;
            //10
            public uint16_t sequence;
            //12
            public uint8_t idletime;
            //13
            public uint8_t timestatus;
            //14
            public uint16_t week;
            //16
            public uint32_t tow;
            //20
            public uint32_t recvstatus;
            // 24
            public uint16_t resv;
            //26
            public uint16_t recvswver;
        }

        struct nova_msg_parser
        {
            public readstate nova_state;

            public nova_header header;

            // 32
            public uint8_t[] data;

            public uint32_t crc;

            public uint8_t[] headerdata;

            public uint16_t read;
        }


        
        uint32_t crc_error_counter;

        nova_msg_parser nova_msg = new nova_msg_parser();

        Stream port = null;

        public AP_GPS_NOVA()
        {
            crc_error_counter = 0;
            nova_msg.nova_state = readstate.PREAMBLE1;

            var sport = new SerialPort("com14", 115200);

            sport.Open();

            port = sport.BaseStream;

            /*
log com_30 rangeb ontime 1
log com_30 satvisb ontime 1
log com_30 bestvelb ontime 1
log com_30 bestposb ontime 1
log com_30 psrdopb onchanged
log com_30 timeb ontime 1
log com_30 headingb onchanged
             */

            string command2 =
                "\r\n\r\nunlogall\r\nlog bestposb ontime 0.2 0 nohold\r\nlog bestvelb ontime 0.2 0 nohold\r\nlog psrdopb onchanged\r\nlog psrdopb ontime 0.2\r\n";
            port.Write(ASCIIEncoding.ASCII.GetBytes(command2), 0, command2.Length);

            System.Threading.Thread.Sleep(70);

            read();
        }

        public bool read()
        {
            var st = File.OpenWrite("tersus.log");

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
            switch (nova_msg.nova_state)
            {
                default:
                case readstate.PREAMBLE1:
                    if (temp == NOVA_PREAMBLE1)
                        nova_msg.nova_state++;
                    //else
                      //  Console.Write(".");
                    nova_msg.read = 0;
                    break;
                case readstate.PREAMBLE2:
                    if (temp == NOVA_PREAMBLE2)
                    {
                        nova_msg.nova_state++;
                    }
                    else
                    {
                        //Console.WriteLine("Bad Sync " + temp);
                        nova_msg.nova_state = readstate.PREAMBLE1;
                    }
                    break;
                case readstate.PREAMBLE3:
                    if (temp == NOVA_PREAMBLE3)
                    {
                        nova_msg.nova_state++;
                    }
                    else
                    {
                       // Console.WriteLine("Bad Sync " + temp);
                        nova_msg.nova_state = readstate.PREAMBLE1;
                    }
                    break;
                case readstate.HEADERLENGTH:
                    nova_msg.headerdata = new byte[temp];
                    nova_msg.headerdata[0] = NOVA_PREAMBLE1;
                    nova_msg.headerdata[1] = NOVA_PREAMBLE2;
                    nova_msg.headerdata[2] = NOVA_PREAMBLE3;
                    nova_msg.headerdata[3] = temp;
                    nova_msg.header.headerlength = temp;
                    nova_msg.nova_state++;
                    nova_msg.read = 4;
                    break;
                case readstate.HEADERDATA:
                    nova_msg.headerdata[nova_msg.read] = temp;
                    nova_msg.read++;
                    if ((nova_msg.read) >= nova_msg.header.headerlength)
                    {
                        nova_msg.header = (nova_header) nova_msg.headerdata.ByteArrayToStructure<nova_header>(0);

                        nova_msg.data = new uint8_t[nova_msg.header.messagelength];

                        Console.WriteLine("{0} {1} {2}         ", nova_msg.header.week, nova_msg.header.tow,
                            nova_msg.header.messageid);

                        nova_msg.nova_state++;
                    }
                    break;
                case readstate.DATA:
                    nova_msg.data[nova_msg.read - nova_msg.header.headerlength] = temp;
                    nova_msg.read++;
                    if (nova_msg.read >= (nova_msg.header.messagelength + nova_msg.header.headerlength))
                    {
                        nova_msg.nova_state++;
                    }
                    break;
                case readstate.CRC1:
                    nova_msg.crc = (uint32_t) (temp << 0);
                    nova_msg.nova_state++;
                    break;
                case readstate.CRC2:
                    nova_msg.crc += (uint32_t) (temp << 8);
                    nova_msg.nova_state++;
                    break;
                case readstate.CRC3:
                    nova_msg.crc += (uint32_t) (temp << 16);
                    nova_msg.nova_state++;
                    break;
                case readstate.CRC4:
                    nova_msg.crc += (uint32_t) (temp << 24);
                    nova_msg.nova_state = readstate.PREAMBLE1;

                    UInt32 crc = crc32.CalculateBlockCRC32(nova_msg.headerdata);
                    crc = crc32.CalculateBlockCRC32(nova_msg.data, crc);

                    if (nova_msg.crc == crc)
                    {
                        return process_message();
                    }
                    else
                    {
                        crc_error_counter++;
                    }
                    break;
            }

            return false;
        }

        private bool process_message()
        {
            uint16_t messageid = nova_msg.header.messageid;

            if (messageid == 42) // bestpos
            {
                var size = Marshal.SizeOf(new bestpos());
                var bestpos = nova_msg.data.ByteArrayToStructure<bestpos>(0);

                state.time_week = nova_msg.header.week;
                state.time_week_ms = (uint32_t) nova_msg.header.tow;
                state.last_gps_time_ms = state.time_week_ms;

                state.location.lat = (int32_t) (bestpos.lat*1e7);
                state.location.lng = (int32_t) (bestpos.lng*1e7);
                state.location.alt = (int32_t) (bestpos.hgt*1e2);

                state.num_sats = bestpos.svsused;

                state.horizontal_accuracy = (float) ((bestpos.latsdev + bestpos.lngsdev)/2);
                state.vertical_accuracy = (float) bestpos.hgtsdev;
                state.have_horizontal_accuracy = true;
                state.have_vertical_accuracy = true;

                if (bestpos.solstat == 0) // have a solution
                {
                    switch (bestpos.postype)
                    {
                        case 16:
                            state.status = AP_GPS.GPS_OK_FIX_3D;
                            break;
                        case 17: // psrdiff
                        case 18: // waas
                        case 20: // omnistar
                        case 68: // ppp_converg
                        case 69: // ppp
                            state.status = AP_GPS.GPS_OK_FIX_3D_DGPS;
                            break;
                        case 32: // l1 float
                        case 33: // iono float
                        case 34: // narrow float
                        case 48: // l1 int
                        case 50: // narrow int
                            state.status = AP_GPS.GPS_OK_FIX_3D_RTK;
                            break;
                        case 0: // NONE
                        case 1: // FIXEDPOS
                        case 2: // FIXEDHEIGHT
                        default:
                            state.status = AP_GPS.NO_FIX;
                            break;
                    }
                }
                else
                {
                    state.status = AP_GPS.NO_FIX;
                }

                _new_position = true;
            }

            if (messageid == 99) // bestvel
            {
                var bestvel = nova_msg.data.ByteArrayToStructure<bestvel>(0);

                state.ground_speed = (float) bestvel.horspd;
                state.ground_course = (float) (bestvel.trkgnd);
                fill_3d_velocity();
                state.velocity.z = -(float) bestvel.vertspd;
                state.have_vertical_velocity = true;

                _last_vel_time = (uint32_t)nova_msg.header.tow;
                _new_speed = true;
            }

            if (messageid == 174) // psrdop
            {
                var psrdop = nova_msg.data.ByteArrayToStructure<psrdop>(0);

                state.hdop = (ushort) (psrdop.hdop*100);
                state.vdop = (ushort) (psrdop.htdop*100);
                return false;
            }

            // ensure out position and velocity stay insync
            if (_new_position && _new_speed && _last_vel_time == state.last_gps_time_ms)
            {
                _new_speed = _new_position = false;

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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct psrdop
        {
            public float gdop;
            public float pdop;
            public float hdop;
            public float htdop;
            public float tdop;
            public float cutoff;
            public uint32_t svcount;
            // extra data for individual prns
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct bestpos
        {
            public uint32_t solstat;
            public uint32_t postype;
            public double lat;
            public double lng;
            public double hgt;
            public float undulation;
            public uint32_t datumid;
            public float latsdev;
            public float lngsdev;
            public float hgtsdev;
            // 4 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public byte[] stnid;
            public float diffage;
            public float sol_age;
            public uint8_t svstracked;
            public uint8_t svsused;
            public uint8_t svsl1;
            public uint8_t svsmultfreq;
            public uint8_t resv;
            public uint8_t extsolstat;
            public uint8_t galbeisigmask;
            public uint8_t gpsglosigmask;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct bestvel
        {
            public uint32_t solstat;
            public uint32_t veltype;
            public float latency;
            public float age;
            public double horspd;
            public double trkgnd;
            // + up
            public double vertspd;
            public float resv;
        }

        public class crc32
        {
            static uint32_t CRC32_POLYNOMIAL = 0xEDB88320;
/* --------------------------------------------------------------------------
Calculate a CRC value to be used by CRC calculation functions.
-------------------------------------------------------------------------- */

            public static uint32_t CRC32Value(uint32_t i)
            {
                int j;
                uint32_t ulCRC;
                ulCRC = i;
                for (j = 8; j > 0; j--)
                {
                    if ((ulCRC & 1) > 0)
                        ulCRC = (ulCRC >> 1) ^ CRC32_POLYNOMIAL;
                    else
                        ulCRC >>= 1;
                }
                return ulCRC;
            }

/* --------------------------------------------------------------------------
Calculates the CRC-32 of a block of data all at once
-------------------------------------------------------------------------- */

            public static uint32_t CalculateBlockCRC32(byte[] ucBuffer, uint32_t ulCRC = 0) /* Data block */
            {
                uint32_t ulCount = (uint32_t) ucBuffer.Length;
                uint32_t index = 0;
                uint32_t ulTemp1;
                uint32_t ulTemp2;
                while (ulCount-- != 0)
                {
                    ulTemp1 = (ulCRC >> 8) & 0x00FFFFFF;
                    ulTemp2 = CRC32Value(((uint32_t) ulCRC ^ ucBuffer[index++]) & 0xff);
                    ulCRC = ulTemp1 ^ ulTemp2;
                }
                return (ulCRC);
            }
        }
    }
}