

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



//using uavcan.protocol.file.Path.cs


public const int UAVCAN_PROTOCOL_FILE_READ_REQ_MAX_PACK_SIZE = 206;
public const ulong UAVCAN_PROTOCOL_FILE_READ_REQ_DT_SIG = 0x8DCDCA939F33F678;

public const int UAVCAN_PROTOCOL_FILE_READ_REQ_DT_ID = 48;






public class uavcan_protocol_file_Read_req: IUAVCANSerialize {



    public uint64_t offset = new uint64_t();



    public uavcan_protocol_file_Path path = new uavcan_protocol_file_Path();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_file_Read_req(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_file_Read_req(transfer, this);
}

};

}
}