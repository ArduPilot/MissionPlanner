
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

//using uavcan.protocol.debug.LogLevel.cs

public const int UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_MAX_PACK_SIZE = 123;
public const ulong UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_SIG = 0xD654A48E0C049D75;
public const int UAVCAN_PROTOCOL_DEBUG_LOGMESSAGE_DT_ID = 16383;



public class uavcan_protocol_debug_LogMessage: IUAVCANSerialize {
    public uavcan_protocol_debug_LogLevel level = new uavcan_protocol_debug_LogLevel();
    public uint8_t source_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=31)] public uint8_t[] source = new uint8_t[31];
    public uint8_t text_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=90)] public uint8_t[] text = new uint8_t[90];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_debug_LogMessage(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_debug_LogMessage(transfer, this);
}

};

}
}