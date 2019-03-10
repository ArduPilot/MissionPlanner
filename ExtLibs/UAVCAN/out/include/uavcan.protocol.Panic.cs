

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




public const int UAVCAN_PROTOCOL_PANIC_MAX_PACK_SIZE = 8;
public const ulong UAVCAN_PROTOCOL_PANIC_DT_SIG = 0x8B79B4101811C1D7;

public const int UAVCAN_PROTOCOL_PANIC_DT_ID = 5;





public const double UAVCAN_PROTOCOL_PANIC_MIN_MESSAGES = 3; // saturated uint8

public const double UAVCAN_PROTOCOL_PANIC_MAX_INTERVAL_MS = 500; // saturated uint16




public class uavcan_protocol_Panic: IUAVCANSerialize {



    public uint8_t reason_text_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=7)] public uint8_t[] reason_text = new uint8_t[7];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_Panic(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_Panic(transfer, this);
}

};

}
}