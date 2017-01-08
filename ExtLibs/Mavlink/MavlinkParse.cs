using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        public MAVLinkMessage ReadPacket(Stream BaseStream)
        {
            byte[] buffer = new byte[270];

            int readcount = 0;

            while (readcount < 200)
            {
                // read STX byte
                ReadWithTimeout(BaseStream, buffer, 0, 1);

                if (buffer[0] == MAVLink.MAVLINK_STX || buffer[0] == MAVLINK_STX_MAVLINK1)
                    break;

                readcount++;
            }

            var headerlength = buffer[0] == MAVLINK_STX ? MAVLINK_CORE_HEADER_LEN : MAVLINK_CORE_HEADER_MAVLINK1_LEN;
            var headerlengthstx = headerlength + 1;

            // read header
            ReadWithTimeout(BaseStream, buffer, 1, headerlength);

            // packet length
            int lengthtoread = 0;
            if (buffer[0] == MAVLINK_STX)
            {
                lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - magic - length
                if ((buffer[2] & MAVLINK_IFLAG_SIGNED) > 0)
                {
                    lengthtoread += MAVLINK_SIGNATURE_BLOCK_LEN;
                }
            }
            else
            {
                lengthtoread = buffer[1] + headerlengthstx + 2 - 2; // data + header + checksum - U - length    
            }

            //read rest of packet
            ReadWithTimeout(BaseStream, buffer, 6, lengthtoread - (headerlengthstx-2));

            // resize the packet to the correct length
            Array.Resize<byte>(ref buffer, lengthtoread + 2);

            MAVLinkMessage message = new MAVLinkMessage(buffer);

            // calc crc
            ushort crc = MavlinkCRC.crc_calculate(buffer, buffer.Length - 2);

            // calc extra bit of crc for mavlink 1.0+
            if (message.header == MAVLINK_STX || message.header == MAVLINK_STX_MAVLINK1)
            {
                crc = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo(message.msgid).crc, crc);
            }

            // check crc
            if ((message.crc16 >> 8) != (crc >> 8) ||
                      (message.crc16 & 0xff) != (crc & 0xff))
            {
                badCRC++;
                // crc fail
                return null;
            }

            return message;
        }

        public byte[] GenerateMAVLinkPacket10(MAVLINK_MSG_ID messageType, object indata)
        {
            byte[] data;

            data = MavlinkUtil.StructureToByteArray(indata);

            byte[] packet = new byte[data.Length + 6 + 2];

            packet[0] = MAVLINK_STX_MAVLINK1;
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

            checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint)messageType).crc, checksum);

            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            return packet;
        }

        public byte[] GenerateMAVLinkPacket20(MAVLINK_MSG_ID messageType, object indata, bool sign = false)
        {
            byte[] data;

            data = MavlinkUtil.StructureToByteArray(indata);

            MavlinkUtil.trim_payload(ref data);

            int extra = 0;
            if (sign)
                extra = MAVLINK_SIGNATURE_BLOCK_LEN;

            byte[] packet = new byte[data.Length + MAVLINK_NUM_NON_PAYLOAD_BYTES + extra];

            packet[0] = MAVLINK_STX;
            packet[1] = (byte)data.Length;
            packet[2] = 0;//incompat  signing
            if (sign)
                packet[2] |= MAVLINK_IFLAG_SIGNED;
            packet[3] = 0;//compat
            packet[4] = (byte)packetcount;

            packetcount++;

            packet[5] = 255; // this is always 255 - MYGCS
            packet[6] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
            packet[7] = (byte)((UInt32)messageType);
            packet[8] = (byte)((UInt32)messageType >> 8);
            packet[9] = (byte)((UInt32)messageType >> 16);

            int i = MAVLINK_NUM_HEADER_BYTES;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }

            ushort checksum = MavlinkCRC.crc_calculate(packet, data.Length + MAVLINK_NUM_HEADER_BYTES);

            checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint)messageType).crc, checksum);

            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            if (sign)
            {
                //https://docs.google.com/document/d/1ETle6qQRcaNWAmpG2wz0oOpFKSF_bcTmYMQvtTGI8ns/edit

                /*
                8 bits of link ID
                48 bits of timestamp
                48 bits of signature
                */

                // signature = sha256_48(secret_key + header + payload + CRC + link-ID + timestamp)

                var timestamp = (UInt64)((DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalMilliseconds * 100);

                if (timestamp == lasttimestamp)
                    timestamp++;

                lasttimestamp = timestamp;

                var timebytes = BitConverter.GetBytes(timestamp);

                var sig = new byte[7]; // 13 includes the outgoing hash
                sig[0] = sendlinkid;
                Array.Copy(timebytes, 0, sig, 1, 6); // timestamp

                //Console.WriteLine("gen linkid {0}, time {1} {2} {3} {4} {5} {6} {7}", sig[0], sig[1], sig[2], sig[3], sig[4], sig[5], sig[6], timestamp);
                
                if (signingKey == null || signingKey.Length != 32)
                {
                    signingKey = new byte[32];
                }

                using (SHA256Managed signit = new SHA256Managed())
                {
                    signit.TransformBlock(signingKey, 0, signingKey.Length, null, 0);
                    signit.TransformBlock(packet, 0, i, null, 0);
                    signit.TransformFinalBlock(sig, 0, sig.Length);
                    var ctx = signit.Hash;
                    // trim to 48
                    Array.Resize(ref ctx, 6);

                    foreach (byte b in sig)
                    {
                        packet[i] = b;
                        i++;
                    }

                    foreach (byte b in ctx)
                    {
                        packet[i] = b;
                        i++;
                    }
                }
            }

            return packet;
        }

        public byte sendlinkid { get; set; }

        public ulong lasttimestamp { get; set; }

        public byte[] signingKey { get; set; }
    }
}
