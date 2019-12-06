using System;
using System.Linq;

namespace UAVCAN
{
    /// <summary>
    /// https://uavcan.org/Specification/4._CAN_bus_transport_layer/
    /// </summary>
    public class CANPayload
    {
        public byte[] packet_data;

        public CANPayload(byte[] packet_data)
        {
            this.packet_data = packet_data;
        }

        //0-31
        public byte TransferID
        {
            get { return (byte) (packet_data[packet_data.Length - 1] & 0x1f); }
            set
            {
                packet_data[packet_data.Length - 1] =
                    (byte) ((packet_data[packet_data.Length - 1] & (~0x1f)) | (value & 0x1f));
            }
        }

        public bool Toggle
        {
            get { return (packet_data[packet_data.Length - 1] & 0x20) > 0; }
            set
            {
                packet_data[packet_data.Length - 1] =
                    (byte) ((packet_data[packet_data.Length - 1] & (~0x20)) | (value ? 0x20 : 0x0));
            }
        }

        public bool EOT
        {
            get { return (packet_data[packet_data.Length - 1] & 0x40) > 0; }
            set
            {
                packet_data[packet_data.Length - 1] =
                    (byte) ((packet_data[packet_data.Length - 1] & (~0x40)) | (value ? 0x40 : 0x0));
            }
        }

        public bool SOT
        {
            get { return (packet_data[packet_data.Length - 1] & 0x80) > 0; }
            set
            {
                packet_data[packet_data.Length - 1] =
                    (byte) ((packet_data[packet_data.Length - 1] & (~0x80)) | (value ? 0x80 : 0x0));
            }
        }

        public byte[] Payload
        {
            get { return packet_data.Take(packet_data.Length - 1).ToArray(); }
        }

        public string ToHex()
        {
            var ans = "";
            foreach (var b in packet_data)
            {
                ans += b.ToString("X2");
            }

            return ans;
        }
    }
}