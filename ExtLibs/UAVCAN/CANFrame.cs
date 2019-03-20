using System;
using System.Linq;

namespace UAVCAN
{
    /// <summary>
    /// https://uavcan.org/Specification/4._CAN_bus_transport_layer/
    /// </summary>
    public class CANFrame
    {
        private byte[] packet_data;

        public enum FrameType
        {
            anonymous,
            service,
            message
        }

        public CANFrame(byte[] packet_data)
        {
            this.packet_data = packet_data;
        }

        public FrameType TransferType
        {
            get
            {
                if (SourceNode == 0)
                    return FrameType.anonymous;
                if (IsServiceMsg)
                    return FrameType.service;
                return FrameType.message;
            }
        }

        // message frame
        //0-127
        public byte SourceNode
        {
            get { return (byte) (packet_data[0] & 0x7f); }
            set { packet_data[0] = (byte) ((packet_data[0] & (~0x7f)) | (value & 0x7f)); }
        }

        public bool IsServiceMsg
        {
            get { return (packet_data[0] & 0x80) > 0; }
            set { packet_data[0] = (byte) ((packet_data[0] & (~0x80)) | (value ? 0x80 : 0x0)); }
        }

        // 0 - 65535    anon 0-3
        public UInt16 MsgTypeID
        {
            get
            {
                if (TransferType == FrameType.anonymous) return (UInt16)(BitConverter.ToUInt16(packet_data, 1) & 0x3);
                if (TransferType == FrameType.message) return BitConverter.ToUInt16(packet_data, 1);
                return SvcTypeID;
            }
            set
            {
                packet_data[1] = (byte)value;
                packet_data[2] = (byte)(value >> 8);
            }
        }

        // 0-31 high-low
        public byte Priority
        {
            get { return (byte) (packet_data[3] & 0x1f); }
            set { packet_data[3] = (byte)((packet_data[3] & (~0x1f)) | (value & 0x1f)); }
        }

        // anon frame
        public UInt16 AnonDiscriminator
        {
            get { return (UInt16)(BitConverter.ToUInt16(packet_data, 1) >> 2); }
        }

        // service frame
        //0-127
        public byte SvcDestinationNode
        {
            get { return (byte) (packet_data[1] & 0x7f); }
            set { packet_data[1] = (byte)((packet_data[1] & (~0x7f)) | (value & 0x7f)); }
        }

        public bool SvcIsRequest
        {
            get { return (packet_data[1] & 0x80) > 0; }
            set { packet_data[1] = (byte)((packet_data[1] & (~0x80)) | (value ? 0x80 : 0x0)); }
        }

        //0-255
        public byte SvcTypeID
        {
            get
            {
                if (TransferType == FrameType.service) return (byte) (packet_data[2]);
                return 0;
            }
            set { packet_data[2] = value; }
        }

        public string ToHex()
        {
            var ans = "";
            foreach (var b in packet_data.Reverse())
            {
                ans += b.ToString("X2");
            }

            return ans;
        }
    }
}