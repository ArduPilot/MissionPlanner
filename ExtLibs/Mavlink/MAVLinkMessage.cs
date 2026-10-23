
using System;
using System.Diagnostics;
using System.IO;

public partial class MAVLink
{
    public class MAVLinkMessage
    {
        public static readonly MAVLinkMessage Invalid = new MAVLinkMessage();
        object _locker = new object();

        private byte[] _buffer;

        public byte[] buffer
        {
            get { return _buffer; }
            set
            {
                _buffer = value;
                processBuffer(_buffer);
            }
        }

        public DateTime rxtime { get; set; }
        public byte header { get; internal set; }
        public byte payloadlength { get; internal set; }

        public byte incompat_flags { get; internal set; }
        public byte compat_flags { get; internal set; }

        public byte seq { get; internal set; }
        public uint sysid { get; internal set; }
        public byte compid { get; internal set; }

        public int headerlength { get; private set; }
        public uint? target_system { get; private set; }

        // A wide header target takes precedence over the legacy payload byte.
        public uint? GetTargetSystem()
        {
            if (target_system.HasValue) return target_system;
            var type = MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid).type;
            if (type == null) return null;
            var field = GetTargetSystemField(type);
            return field == null ? (uint?)null : Convert.ToUInt32(field.GetValue(data));
        }

        public byte? GetTargetComponent()
        {
            var type = MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid).type;
            var field = type?.GetField("target_component");
            return field == null ? (byte?)null : Convert.ToByte(field.GetValue(data));
        }

        public bool IsTargetedTo(uint system, byte component)
        {
            uint? targetSystem = GetTargetSystem();
            byte? targetComponent = GetTargetComponent();
            return (!targetSystem.HasValue || targetSystem == 0 || targetSystem == system) &&
                (!targetComponent.HasValue || targetComponent == 0 || targetComponent == component);
        }

        public uint msgid { get; internal set; }

        public bool ismavlink2 {
            get
            {
                if (buffer != null && buffer.Length > 0)
                    return (buffer[0] == MAVLINK_STX);

                return false;
            }
        }

        public string msgtypename
        {
            get { return MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid).name; }
        }

        object _data;
        public object data
        {
            get
            {
                // lock the entire creation of the packet. to prevent returning a incomplete packet.
                lock (_locker)
                {
                    if (_data != null)
                        return _data;

                    var typeinfo = MAVLINK_MESSAGE_INFOS.GetMessageInfo(msgid);

                    if (typeinfo.type == null)
                        return null;

                    _data = Activator.CreateInstance(typeinfo.type);

                    try
                    {
                        if (payloadlength == 0)
                            return _data;
                        // fill in the data of the object
                        if (ismavlink2)
                        {
                            _data = MavlinkUtil.ByteArrayToStructureGC(buffer, typeinfo.type, (byte)headerlength,
                                payloadlength);
                            //MavlinkUtil.ByteArrayToStructure(buffer, ref _data, MAVLINK_NUM_HEADER_BYTES, payloadlength);
                        }
                        else
                        {
                            _data = MavlinkUtil.ByteArrayToStructureGC(buffer, typeinfo.type, 6, payloadlength);
                        }
                    }
                    catch (Exception ex)
                    {
                        // should not happen
                        if(Debugger.IsAttached)
                            Debugger.Break();
                        System.Diagnostics.Debug.WriteLine(ex);
                    }
                }

                return _data;
            }
        }

        public T ToStructure<T>()
        {
            return (T)data;
        }

        public ushort crc16 { get; internal set; }

        public byte[] sig { get; internal set; }

        public byte sigLinkid
        {
            get
            {
                if (sig != null)
                {
                    return sig[0];
                }

                return 0;
            }
        }

        public ulong sigTimestamp 
        {
            get
            {
                if (sig != null)
                {
                    byte[] temp = new byte[8];
                    Array.Copy(sig, 1, temp, 0, 6);
                    return BitConverter.ToUInt64(temp, 0);
                }

                return 0;
            }
        }

        public int Length
        {
            get
            {
                if (buffer == null) return 0;
                return buffer.Length;
            }
        }

        public MAVLinkMessage()
        {
            this.rxtime = DateTime.MinValue;
        }

        public MAVLinkMessage(byte[] buffer): this(buffer, DateTime.UtcNow)
        {
        }

        public MAVLinkMessage(byte[] buffer, DateTime rxTime)
        {
            this.buffer = buffer;
            this.rxtime = rxTime;
        }

        internal void processBuffer(byte[] buffer)
        {
            _data = null;
            sig = null;
            target_system = null;
            incompat_flags = compat_flags = 0;
            if (buffer == null || buffer.Length < 2)
                throw new InvalidDataException("Truncated MAVLink frame");
            header = buffer[0];
            payloadlength = buffer[1];
            if (header == MAVLINK_STX)
            {
                if (buffer.Length < MAVLINK_NUM_HEADER_BYTES)
                    throw new InvalidDataException("Truncated MAVLink header");
                incompat_flags = buffer[2];
                compat_flags = buffer[3];
                if ((incompat_flags & ~MAVLINK_SUPPORTED_IFLAGS) != 0)
                    throw new InvalidDataException("Unsupported MAVLink incompatible flags");
                headerlength = GetHeaderLength(incompat_flags);
            }
            else if (header == MAVLINK_STX_MAVLINK1)
                headerlength = 6;
            else
                throw new InvalidDataException("Invalid MAVLink magic");

            int signatureLength = (incompat_flags & MAVLINK_IFLAG_SIGNED) != 0 ? MAVLINK_SIGNATURE_BLOCK_LEN : 0;
            if (buffer.Length != headerlength + payloadlength + 2 + signatureLength)
                throw new InvalidDataException("Invalid MAVLink frame length");
            if (header == MAVLINK_STX)
            {
                seq = buffer[4];
                bool wide = (incompat_flags & MAVLINK_IFLAG_SYSID32) != 0;
                int offset = 5;
                sysid = ReadSystemId(buffer, ref offset, wide);
                compid = buffer[offset++];
                msgid = (uint)(buffer[offset] | buffer[offset + 1] << 8 | buffer[offset + 2] << 16);
                offset += 3;
                if ((incompat_flags & MAVLINK_IFLAG_TARGET32) != 0)
                {
                    target_system = ReadSystemId(buffer, ref offset, true);
                }
            }
            else
            {
                seq = buffer[2];
                sysid = buffer[3];
                compid = buffer[4];
                msgid = buffer[5];
            }
            int crcOffset = headerlength + payloadlength;
            crc16 = (ushort)(buffer[crcOffset] | buffer[crcOffset + 1] << 8);
            if (signatureLength != 0)
            {
                sig = new byte[signatureLength];
                Array.Copy(buffer, crcOffset + 2, sig, 0, signatureLength);
            }
        }

        public override string ToString()
        {
            return String.Format("{5},{4},{0},{1},{2},{3}", sysid, compid, msgid, msgtypename, ismavlink2, rxtime);
        }
    }
}
