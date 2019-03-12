
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


public const int UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_MAX_PACK_SIZE = 95;
public const ulong UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_DT_SIG = 0x196AE06426A3B5D8;
public const int UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_DT_ID = 15;



public const double UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_TIMEOUT_CANCEL = 0; // saturated uint16
public const double UAVCAN_PROTOCOL_ENUMERATION_BEGIN_REQ_TIMEOUT_INFINITE = 65535; // saturated uint16

public class uavcan_protocol_enumeration_Begin_req: IUAVCANSerialize {
    public uint16_t timeout_sec = new uint16_t();
    public uint8_t parameter_name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=92)] public uint8_t[] parameter_name = new uint8_t[92];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_enumeration_Begin_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_enumeration_Begin_req(transfer, this);
}

};

}
}