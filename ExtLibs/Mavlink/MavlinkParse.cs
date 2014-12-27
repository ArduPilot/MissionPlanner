using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MavlinkParse
    {
        public static int packetcount = 0;

        public object ReadPacket(Stream BaseStream)
        {
            byte[] buffer = new byte[270];

            int readcount = 0;

            while (readcount < 200)
            {
                // read STX byte
                BaseStream.Read(buffer, 0, 1);

                if (buffer[0] == MAVLink.MAVLINK_STX)
                    break;

                readcount++;
            }

            // read header
            int read = BaseStream.Read(buffer, 1, 5);

            // packet length
            int lengthtoread = buffer[1] + 6 + 2 - 2; // data + header + checksum - STX - length

            //read rest of packet
            read = BaseStream.Read(buffer, 6, lengthtoread - 4);

            // resize the packet to the correct length
            Array.Resize<byte>(ref buffer, lengthtoread+2);

            // calc crc
            ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

            // calc extra bit of crc for mavlink 1.0
            if (buffer.Length > 5 && buffer[0] == 254)
            {
                crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[buffer[5]], crc);
            }

            // check message length vs table
            if (buffer.Length > 5 && buffer[1] != MAVLINK_MESSAGE_LENGTHS[buffer[5]])
            {
                // bad or unknown packet
                return null;
            }

            // check crc
            if (buffer.Length < 5 || buffer[buffer.Length - 1] != (crc >> 8) || buffer[buffer.Length - 2] != (crc & 0xff))
            {
                // crc fail
                return null;
            }

            byte header = buffer[0];
            byte length = buffer[1];
            byte seq = buffer[2];
            byte sysid = buffer[3];
            byte compid = buffer[4];
            byte messid = buffer[5];

            //create the object spcified by the packet type
            object data = Activator.CreateInstance(MAVLINK_MESSAGE_INFO[messid]);

            // fill in the data of the object
            MavlinkUtil.ByteArrayToStructure(buffer, ref data, 6);

            return data;
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
