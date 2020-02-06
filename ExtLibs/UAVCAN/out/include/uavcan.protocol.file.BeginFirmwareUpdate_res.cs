
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


public const int UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_MAX_PACK_SIZE = 129;
public const ulong UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_DT_SIG = 0xB7D725DF72724126;
public const int UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_DT_ID = 40;



public const double UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_ERROR_OK = 0; // saturated uint8
public const double UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_ERROR_INVALID_MODE = 1; // saturated uint8
public const double UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_ERROR_IN_PROGRESS = 2; // saturated uint8
public const double UAVCAN_PROTOCOL_FILE_BEGINFIRMWAREUPDATE_RES_ERROR_UNKNOWN = 255; // saturated uint8

public class uavcan_protocol_file_BeginFirmwareUpdate_res: IUAVCANSerialize {
    public uint8_t error = new uint8_t();
    public uint8_t optional_error_message_len; [MarshalAs(UnmanagedType.ByValArray,SizeConst=127)] public uint8_t[] optional_error_message = new uint8_t[127];

public void encode(uavcan_serializer_chunk_cb_ptr_t chunk_cb, object ctx)
{
	encode_uavcan_protocol_file_BeginFirmwareUpdate_res(this, chunk_cb, ctx);
}

public void decode(CanardRxTransfer transfer)
{
	decode_uavcan_protocol_file_BeginFirmwareUpdate_res(transfer, this);
}

};

}
}