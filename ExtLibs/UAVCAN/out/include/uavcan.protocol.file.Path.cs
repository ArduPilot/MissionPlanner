

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




public const int UAVCAN_PROTOCOL_FILE_PATH_MAX_PACK_SIZE = 201;
public const ulong UAVCAN_PROTOCOL_FILE_PATH_DT_SIG = 0x12AEFC50878A43E2;





public const double UAVCAN_PROTOCOL_FILE_PATH_SEPARATOR = 47; // saturated uint8




public class uavcan_protocol_file_Path: IUAVCANSerialize {



    public uint8_t path_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=200)] public uint8_t[] path = new uint8_t[200];




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_file_Path(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_file_Path(transfer, this);
}

};

}
}