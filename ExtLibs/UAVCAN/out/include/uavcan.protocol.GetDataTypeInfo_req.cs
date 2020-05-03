
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

//using uavcan.protocol.DataTypeKind.cs

public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_MAX_PACK_SIZE = 84;
public const ulong UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_DT_SIG = 0x1B283338A7BED2D8;
public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_REQ_DT_ID = 2;



public class uavcan_protocol_GetDataTypeInfo_req: IUAVCANSerialize {
    public uint16_t id = new uint16_t();
    public uavcan_protocol_DataTypeKind kind = new uavcan_protocol_DataTypeKind();
    public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)] public uint8_t[] name = new uint8_t[80];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_GetDataTypeInfo_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_GetDataTypeInfo_req(transfer, this);
}

};

}
}