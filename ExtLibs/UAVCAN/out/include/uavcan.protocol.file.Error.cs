

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




public const int UAVCAN_PROTOCOL_FILE_ERROR_MAX_PACK_SIZE = 2;
public const ulong UAVCAN_PROTOCOL_FILE_ERROR_DT_SIG = 0xA83071FFEA4FAE15;





public const double UAVCAN_PROTOCOL_FILE_ERROR_OK = 0; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_UNKNOWN_ERROR = 32767; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_NOT_FOUND = 2; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_IO_ERROR = 5; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_ACCESS_DENIED = 13; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_IS_DIRECTORY = 21; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_INVALID_VALUE = 22; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_FILE_TOO_LARGE = 27; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_OUT_OF_SPACE = 28; // saturated int16

public const double UAVCAN_PROTOCOL_FILE_ERROR_NOT_IMPLEMENTED = 38; // saturated int16




public class uavcan_protocol_file_Error: IUAVCANSerialize {



    public int16_t value = new int16_t();




public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_file_Error(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_file_Error(transfer, this);
}

};

}
}