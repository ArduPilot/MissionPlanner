
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;

using float32 = System.Single;

using System;
using System.Runtime.InteropServices;

namespace UAVCAN
{
public partial class uavcan {


public const int UAVCAN_PROTOCOL_HARDWAREVERSION_MAX_PACK_SIZE = 274;
public const ulong UAVCAN_PROTOCOL_HARDWAREVERSION_DT_SIG = 0xAD5C4C933F4A0C4;



public class uavcan_protocol_HardwareVersion: IUAVCANSerialize {
    public uint8_t major = new uint8_t();
    public uint8_t minor = new uint8_t();
    [MarshalAs(UnmanagedType.ByValArray,SizeConst=16)] public uint8_t[] unique_id = new uint8_t[16];
    public uint8_t certificate_of_authenticity_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=255)] public uint8_t[] certificate_of_authenticity = new uint8_t[255];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_HardwareVersion(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_HardwareVersion(transfer, this);
}

};

}
}