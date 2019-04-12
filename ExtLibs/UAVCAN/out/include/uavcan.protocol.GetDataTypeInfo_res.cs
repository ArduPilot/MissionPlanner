

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


public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_MAX_PACK_SIZE = 93;
public const ulong UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_DT_SIG = 0x1B283338A7BED2D8;

public const int UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_DT_ID = 2;





public const double UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_FLAG_KNOWN = 1; // saturated uint8

public const double UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_FLAG_SUBSCRIBED = 2; // saturated uint8

public const double UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_FLAG_PUBLISHING = 4; // saturated uint8

public const double UAVCAN_PROTOCOL_GETDATATYPEINFO_RES_FLAG_SERVING = 8; // saturated uint8




public class uavcan_protocol_GetDataTypeInfo_res: IUAVCANSerialize {



    public uint64_t signature = new uint64_t();



    public uint16_t id = new uint16_t();



    public uavcan_protocol_DataTypeKind kind = new uavcan_protocol_DataTypeKind();



    public uint8_t flags = new uint8_t();



    public uint8_t name_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=80)] public uint8_t[] name = new uint8_t[80];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_GetDataTypeInfo_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_GetDataTypeInfo_res(transfer, this);
}

};

}
}