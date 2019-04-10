
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


public const int UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_MAX_PACK_SIZE = 1;
public const ulong UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_DT_SIG = 0x711BF141AF572346;



public const double UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_DEBUG = 0; // saturated uint3
public const double UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_INFO = 1; // saturated uint3
public const double UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_WARNING = 2; // saturated uint3
public const double UAVCAN_PROTOCOL_DEBUG_LOGLEVEL_ERROR = 3; // saturated uint3

public class uavcan_protocol_debug_LogLevel: IUAVCANSerialize {
    public uint8_t value = new uint8_t();

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_debug_LogLevel(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_debug_LogLevel(transfer, this);
}

};

}
}