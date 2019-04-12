

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



//using uavcan.protocol.file.Error.cs


public const int UAVCAN_PROTOCOL_FILE_WRITE_RES_MAX_PACK_SIZE = 2;
public const ulong UAVCAN_PROTOCOL_FILE_WRITE_RES_DT_SIG = 0x515AA1DC77E58429;

public const int UAVCAN_PROTOCOL_FILE_WRITE_RES_DT_ID = 49;






public class uavcan_protocol_file_Write_res: IUAVCANSerialize {



    public uavcan_protocol_file_Error error = new uavcan_protocol_file_Error();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_file_Write_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_file_Write_res(transfer, this);
}

};

}
}