using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class MAVLink
{
    public class MAVLinkMessage
    {
        public byte[] buffer;

        public byte header;
        public byte payloadlength;

        public byte incompat_flags;
        public byte compat_flags;

        public byte seq;
        public byte sysid;
        public byte compid;

        public uint msgid;
        object _data;
        public object data
        {
            get
            {
                //_data the object specified by the packet type
                _data = Activator.CreateInstance(MAVLINK_MESSAGE_INFO[msgid]);

                // fill in the data of the object
                if (buffer[0] == 0xfd)
                {
                    MavlinkUtil.ByteArrayToStructure(buffer, ref _data, 10);
                }
                else
                {
                    MavlinkUtil.ByteArrayToStructure(buffer, ref _data, 6);
                }

                return _data;
            }
        }

        public T ByteArrayToStructure<T>()
        {
            return (T)data;
        }

        public ushort crc16;

        public byte[] sig;

        /*public byte this[int index]
        {
            get { return buffer[index]; }
        }*/

        public int Length { get
        {
            if (buffer == null) return 0; return buffer.Length; } }

        public MAVLinkMessage()
        {

        }

        public MAVLinkMessage(byte[] buffer)
        {
            this.buffer = buffer;

            if (buffer[0] == 0xfd)
            {
                header = buffer[0];
                payloadlength = buffer[1];
                incompat_flags = buffer[2];
                compat_flags = buffer[3];
                seq = buffer[4];
                sysid = buffer[5];
                compid = buffer[6];
                msgid = (uint)((buffer[9] << 16) + (buffer[8] << 8) + buffer[7]);

                var crc1 = 9 + payloadlength+1;
                var crc2 = 9 + payloadlength+2;

                crc16 = (ushort)((buffer[crc2] << 8) + buffer[crc1]);
            }
            else
            {
                header = buffer[0];
                payloadlength = buffer[1];
                seq = buffer[2];
                sysid = buffer[3];
                compid = buffer[4];
                msgid = buffer[5];

                var crc1 = 5 + payloadlength + 1;
                var crc2 = 5 + payloadlength + 2;

                crc16 = (ushort)((buffer[crc2] << 8) + buffer[crc1]);
            }
        }
    }
}
