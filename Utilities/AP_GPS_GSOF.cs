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
using System.IO.Ports;
using System.Reflection;

namespace MissionPlanner.Utilities
{
    public class AP_GPS_GSOF
    {
        const uint8_t GSOF_STX = 0x02;
        const uint8_t GSOF_ETX = 0x03;

        public enum readstate
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

        readstate gsof_state;

        gsof_msg_parser gsof_msg = new gsof_msg_parser();

        AP_GPS_SBF.GPS_State state = new AP_GPS_SBF.GPS_State();

        public AP_GPS_GSOF()
        {
            var sport = new SerialPort("COM4", 115200);

            sport.Open();

            port = sport.BaseStream;


            requestGSOF();

            requestPostion();

            gsof_msg.data = new byte[1024 * 20];

            read();
        }

        public void requestGSOF()
        {
            byte[] messages = { 1,2,8,9 };

            byte count = 0;
            foreach (var gsofmsg in messages)
            {
                byte[] buffer = { 0x2, 0x0, 0x64, 15, count, 0x0, 0x0,
                                    3,0,1,0,
                                0x7,0x6, 10, 0x0, 0x2, 0, gsofmsg, 0
                                ,0x0, 0x3 };

                uint8_t check = 0;
                buffer[buffer.Length - 2] = (byte)(buffer.Sum(num => num) - 3 - 2); // 3 = etx 2 = stx

                port.Write(buffer, 0, buffer.Length);

                System.Threading.Thread.Sleep(100);

                count++;
            }
        }

        public void requestPostion()
        {
            byte[] buffer = { 0x2, 0x0, 0x56, 0x3, 0x1, 0x1, 0x2, 0x0, 0x3 };

            uint8_t check = 0;
            buffer[buffer.Length - 2] = (byte)(buffer.Sum(num => num) - 3 - 2);

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
                ret |= parse((byte)temp);

                st.WriteByte((byte)temp);
            }

            return ret;
        }

        private bool parse(uint8_t temp)
        {
            switch (gsof_state)
            {
                case readstate.STARTTX:
                    if (temp == GSOF_STX)
                    {
                        gsof_msg.starttx = temp;
                        gsof_state++;
                    }
                    else
                        Console.Write(".");
                    gsof_msg.read = 0;
                    gsof_msg.checksumcalc = 0;
                    break;
                case readstate.STATUS:
                    gsof_msg.status = temp;
                    gsof_state++;
                    gsof_msg.checksumcalc += temp;
                    break;
                case readstate.PACKETTYPE:
                    gsof_msg.packettype = temp;
                    gsof_state++;
                    gsof_msg.checksumcalc += temp;
                    break;
                case readstate.LENGTH:
                    gsof_msg.length = temp;
                    gsof_state++;
                    gsof_msg.checksumcalc += temp;
                    break;
                case readstate.DATA:
                    gsof_msg.data[gsof_msg.read] = temp;
                    gsof_msg.read++;
                    gsof_msg.checksumcalc += temp;
                    if (gsof_msg.read >= gsof_msg.length)
                    {
                        gsof_state++;
                    }
                    break;
                case readstate.CHECKSUM:
                    gsof_msg.checksum = temp;
                    gsof_state++;
                    if (gsof_msg.checksum == gsof_msg.checksumcalc)
                    {
                        return process_message();
                    }
                    break;
                case readstate.ENDTX:
                    gsof_msg.endtx = temp;
                    gsof_state = 0;
                    break;


            }

            return false;
        }

        private double ToDeg(double p)
        {
            return p * (180 / Math.PI);
        }

        private double ToRad(double p)
        {
            return p * (Math.PI/180);
        }        

        private byte[] reverse(byte[] data, uint32_t pos, uint32_t size)
        {
            uint8_t[] newdata = new uint8_t[size];
            Array.Copy(data, pos, newdata, 0, size);
            Array.Reverse(newdata);
            return newdata;
        }

        void fill_3d_velocity()
        {
            double gps_heading = ToRad(state.ground_course_cd * 0.01);

            state.velocity.X = state.ground_speed * (float)Math.Cos(gps_heading);
            state.velocity.Y = state.ground_speed * (float)Math.Sin(gps_heading);
            state.velocity.Z = 0;
            state.have_vertical_velocity = false;
        }

        private bool process_message()
        {
            Console.WriteLine("packet: "+gsof_msg.packettype.ToString("X") + " len: " + gsof_msg.length + " status: " + gsof_msg.status );

            if (gsof_msg.packettype == 0x57) // RAWDATA 
            {
                uint8_t record_type = gsof_msg.data[0];
                uint8_t page_number = gsof_msg.data[1];
                uint8_t reply_number = gsof_msg.data[2];
                uint8_t recordintflags = gsof_msg.data[3];

                Console.WriteLine(DateTime.Now.Second + " record type: " + record_type);
            }

            if (gsof_msg.packettype == 0x40) // GSOF
            {
                uint8_t trans_number = gsof_msg.data[0];
                uint8_t pageidx = gsof_msg.data[1];
                uint8_t maxpageidx = gsof_msg.data[2];

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
                        state.location.lat = (int32_t)(ToDeg(BitConverter.ToDouble(reverse(gsof_msg.data, a, 8),0)) * 1e7);
                        state.location.lng = (int32_t)(ToDeg(BitConverter.ToDouble(reverse(gsof_msg.data, a + 8, 8), 0)) * 1e7);
                        state.location.alt = (int32_t)(BitConverter.ToDouble(reverse(gsof_msg.data, a + 16, 8), 0) * 1e2);

                        state.last_gps_time_ms = state.time_week_ms;
                    }
                    else if (output_type == 8) // velocity
                    {
                        state.ground_speed = BitConverter.ToSingle(reverse(gsof_msg.data, a + 1, 4), 0);
                        state.ground_course_cd = (int32_t)(ToDeg(BitConverter.ToSingle(reverse(gsof_msg.data, a + 5, 4), 0)) * 100);
                        fill_3d_velocity();
                        state.velocity.Z = BitConverter.ToSingle(reverse(gsof_msg.data, a + 9, 4), 0);
                        state.have_vertical_velocity = true;
                    }
                    else if (output_type == 9) //dop
                    {
                        state.hdop = (uint16_t)(BitConverter.ToSingle(reverse(gsof_msg.data, a + 4,4),0) * 100);
                    }
                    else if (output_type == 1) // pos time
                    {
                        state.time_week_ms = BitConverter.ToUInt32(reverse(gsof_msg.data, a, 4), 0);
                        state.time_week = BitConverter.ToUInt16(reverse(gsof_msg.data, a + 4, 2), 0);
                        state.num_sats = gsof_msg.data[a + 6];
                        uint8_t posf1 = gsof_msg.data[a + 7];
                        uint8_t posf2 = gsof_msg.data[a + 8];

                        Console.WriteLine("POSTIME: " + posf1 + " " + posf2);

                        if ((posf1 & 47) == 47)
                        {
                            state.status = AP_GPS_SBF.GPS_Status.GPS_OK_FIX_3D;
                            if ((posf2 & 1) == 1)
                            {
                                state.status = AP_GPS_SBF.GPS_Status.GPS_OK_FIX_3D_DGPS;
                                if ((posf2 & 4) == 4)
                                {
                                    state.status = AP_GPS_SBF.GPS_Status.GPS_OK_FIX_3D_RTK;
                                }
                            }
                        }
                        else
                        {
                            state.status = AP_GPS_SBF.GPS_Status.NO_FIX;
                        }
                    }

                    a += output_length-1u;
                    
                }


                Type t = state.GetType();//where obj is object whose properties you need.
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
