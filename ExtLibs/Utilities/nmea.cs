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
    public class nmea : ICorrections
    {
        int step = 0;

        public byte[] buffer = new byte[1024 * 1];
        int payloadlen = 0;
        int msglencount = 0;

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
                    if (data == '$')
                    {
                        step = 1;
                        msglencount = 0;
                        buffer[0] = data;
                    }
                    break;
                case 1:
                    if (data == 'G')
                    {
                        buffer[1] = data;
                        step++;
                    }
                    else
                        step = 0;
                    break;
                case 2:
                    if (msglencount > 1000)
                    {
                        step = 0;

                    }

                        buffer[msglencount + 2] = data;
                        msglencount++;
                    if(data == '\n')
                    {
                        var line = System.Text.ASCIIEncoding.ASCII.GetString(buffer, 0, msglencount + 2);
                        string[] items = line.Trim().Split(',', '*');
                        if (items[items.Length-1] == GetChecksum(line))
                        {
                            return 1;
                        }
                        step = 0;
                    }
                    break;
            }

            return -1;
        }

        // Calculates the checksum for a sentence
        string GetChecksum(string sentence)
        {
            // Loop through all chars to get a checksum
            int Checksum = 0;
            foreach (char Character in sentence.ToCharArray())
            {
                switch (Character)
                {
                    case '$':
                        // Ignore the dollar sign
                        break;
                    case '*':
                        // Stop processing before the asterisk
                        return Checksum.ToString("X2");
                    default:
                        // Is this the first value for the checksum?
                        if (Checksum == 0)
                        {
                            // Yes. Set the checksum to the value
                            Checksum = Convert.ToByte(Character);
                        }
                        else
                        {
                            // No. XOR the checksum with this character's value
                            Checksum = Checksum ^ Convert.ToByte(Character);
                        }
                        break;
                }
            }
            // Return the checksum formatted as a two-character hexadecimal
            return Checksum.ToString("X2");
        }
    }
}