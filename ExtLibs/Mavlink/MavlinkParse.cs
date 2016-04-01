using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MavlinkParse
    {
        public int packetcount = 0;

        public int badCRC = 0;
        public int badLength = 0;

        public static void ReadWithTimeout(Stream BaseStream, byte[] buffer, int offset, int count)
        {
            int timeout = BaseStream.ReadTimeout;

            if (timeout == -1)
                timeout = 60000;

            DateTime to = DateTime.Now.AddMilliseconds(timeout);

            int toread = count;
            int pos = offset;

            while (true)
            {
                // read from stream
                int read = BaseStream.Read(buffer, pos, toread);

                // update counter
                toread -= read;
                pos += read;

                // reset timeout if we get data
                if (read > 0)
                    to = DateTime.Now.AddMilliseconds(timeout);

                if (toread == 0)
                    break;

                if (DateTime.Now > to)
                {
                    throw new TimeoutException("Timeout waiting for data");
                }
                System.Threading.Thread.Sleep(1);
            }
        }

        public byte[] ReadPacket(Stream BaseStream)
        {
            byte[] buffer = new byte[270];

            int readcount = 0;

            while (readcount < 200)
            {
                // read STX byte
                ReadWithTimeout(BaseStream, buffer, 0, 1);

                if (buffer[0] == MAVLink.MAVLINK_STX)
                    break;

                readcount++;
            }

            // read header
            ReadWithTimeout(BaseStream, buffer, 1, 5);

            // packet length
            int lengthtoread = buffer[1] + 6 + 2 - 2; // data + header + checksum - STX - length

            //read rest of packet
            ReadWithTimeout(BaseStream, buffer, 6, lengthtoread - 4);

            // check message length vs table
            if (buffer[1] != MAVLINK_MESSAGE_LENGTHS[buffer[5]])
            {
                badLength++;
                // bad or unknown packet
                return null;
            }

            // resize the packet to the correct length
            Array.Resize<byte>(ref buffer, lengthtoread + 2);

            // calc crc
            ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

            // calc extra bit of crc for mavlink 1.0
            if (buffer.Length > 5 && buffer[0] == 254)
            {
                crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[buffer[5]], crc);
            }

            // check crc
            if (buffer.Length < 5 || buffer[buffer.Length - 1] != (crc >> 8) ||
                buffer[buffer.Length - 2] != (crc & 0xff))
            {
                badCRC++;
                // crc fail
                return null;
            }

            return buffer;
        }

        public MAVLinkMessage ReadPacketObj(Stream BaseStream)
        {
            byte[] buffer = ReadPacket(BaseStream);

            byte header = buffer[0];
            byte length = buffer[1];
            byte seq = buffer[2];
            byte sysid = buffer[3];
            byte compid = buffer[4];
            byte messid = buffer[5];

            //create the object specified by the packet type
            object data = Activator.CreateInstance(MAVLINK_MESSAGE_INFO[messid]);

            // fill in the data of the object
            MavlinkUtil.ByteArrayToStructure(buffer, ref data, 6);

            return new MAVLinkMessage(header, length, seq, sysid, compid, messid, data);
        }

        public byte[] GenerateMAVLinkPacket(MAVLINK_MSG_ID messageType, object indata)
        {
            byte[] data;

            data = MavlinkUtil.StructureToByteArray(indata);

            byte[] packet = new byte[data.Length + 6 + 2];

            packet[0] = 254;
            packet[1] = (byte)data.Length;
            packet[2] = (byte)packetcount;

            packetcount++;

            packet[3] = 255; // this is always 255 - MYGCS
            packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
            packet[5] = (byte)messageType;


            int i = 6;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }

            ushort checksum = MavlinkCRC.crc_calculate(packet, packet[1] + 6);

            checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[(byte)messageType], checksum);

            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            return packet;
        }
    }
}
