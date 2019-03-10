

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




public const int UAVCAN_PROTOCOL_DEBUG_KEYVALUE_MAX_PACK_SIZE = 63;
public const ulong UAVCAN_PROTOCOL_DEBUG_KEYVALUE_DT_SIG = 0xE02F25D6E0C98AE0;

public const int UAVCAN_PROTOCOL_DEBUG_KEYVALUE_DT_ID = 16370;






public class uavcan_protocol_debug_KeyValue: IUAVCANSerialize {



    public Single value = new Single();



    public uint8_t key_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=58)] public uint8_t[] key = new uint8_t[58];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_debug_KeyValue(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_debug_KeyValue(transfer, this);
}

};

}
}