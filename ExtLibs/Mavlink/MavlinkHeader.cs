using System;

public partial class MAVLink
{
    public const byte MAVLINK_IFLAG_SYSID32 = 0x02;
    public const byte MAVLINK_IFLAG_TARGET32 = 0x04;
    public const byte MAVLINK_SUPPORTED_IFLAGS = MAVLINK_IFLAG_SIGNED | MAVLINK_IFLAG_SYSID32 | MAVLINK_IFLAG_TARGET32;

    public static int GetHeaderLength(byte flags)
    {
        bool wide = (flags & MAVLINK_IFLAG_SYSID32) != 0;
        return MAVLINK_NUM_HEADER_BYTES + (wide ? 3 : 0) +
            ((flags & MAVLINK_IFLAG_TARGET32) != 0 ? 4 : 0);
    }

    internal static uint ReadSystemId(byte[] buffer, ref int offset, bool wide)
    {
        uint value = buffer[offset++];
        if (wide)
        {
            value |= (uint)buffer[offset++] << 8;
            value |= (uint)buffer[offset++] << 16;
            value |= (uint)buffer[offset++] << 24;
        }
        return value;
    }

    private static void WriteSystemId(byte[] buffer, ref int offset, uint value, bool wide)
    {
        buffer[offset++] = (byte)value;
        if (wide)
        {
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 24);
        }
    }

    public static System.Reflection.FieldInfo GetTargetSystemField(Type type)
    {
        return type.GetField("target_system") ??
            (type == typeof(mavlink_manual_control_t) ? type.GetField("target") : null);
    }

    // Generated payload structs retain byte fields. Supply their full target here
    // before trimming the payload, so small targets and broadcast stay on the wire.
    public static bool SetPayloadTarget(object message, byte[] payload, uint system, byte component)
    {
        var type = message.GetType();
        var field = GetTargetSystemField(type);
        if (field == null) return false;
        payload[(int)System.Runtime.InteropServices.Marshal.OffsetOf(type, field.Name)] = system > 255 ? (byte)255 : (byte)system;
        var componentField = type.GetField("target_component");
        if (componentField != null)
            payload[(int)System.Runtime.InteropServices.Marshal.OffsetOf(type, componentField.Name)] = component;
        return true;
    }

    // Source and target widths are independent; TARGET32 always adds four bytes.
    public static int WriteHeader(byte[] packet, byte payloadLength, byte flags, byte sequence,
        uint system, byte component, uint message, uint targetSystem = 0)
    {
        if ((flags & ~MAVLINK_SUPPORTED_IFLAGS) != 0)
            throw new ArgumentOutOfRangeException(nameof(flags));
        if ((flags & MAVLINK_IFLAG_SYSID32) == 0 && system > 255)
            throw new ArgumentOutOfRangeException(nameof(flags), "32-bit system IDs require SYSID32");
        packet[0] = MAVLINK_STX;
        packet[1] = payloadLength;
        packet[2] = flags;
        packet[3] = 0;
        packet[4] = sequence;
        bool wide = (flags & MAVLINK_IFLAG_SYSID32) != 0;
        int offset = 5;
        WriteSystemId(packet, ref offset, system, wide);
        packet[offset++] = component;
        packet[offset++] = (byte)message;
        packet[offset++] = (byte)(message >> 8);
        packet[offset++] = (byte)(message >> 16);
        if ((flags & MAVLINK_IFLAG_TARGET32) != 0)
        {
            WriteSystemId(packet, ref offset, targetSystem, true);
        }
        return offset;
    }
}
